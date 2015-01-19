Shader "Lux/Translucent/Lux Trans Bumped Specular" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_SpecTex ("Specular Color (RGB) Roughness (A)", 2D) = "black" {}
		_BumpMap ("Normal (Normal)", 2D) = "bump" {}

		_Thickness ("Thickness (G)", 2D) = "bump" {}
		
		_Power ("Subsurface Power (1.0 - 5.0)", Float) = 2.0
		_Distortion ("Subsurface Distortion (0.0 - 0.5)", Float) = 0.1
		_Scale ("Subsurface Scale (1.0 - )", Float) = 2.0
		_SubColor ("Subsurface Color", Color) = (1.0, 1.0, 1.0, 1.0)

		_DiffCubeIBL ("Custom Diffuse Cube", Cube) = "black" {}
		_SpecCubeIBL ("Custom Specular Cube", Cube) = "black" {}

		// _Shininess property is needed by the lightmapper - otherwise it throws errors
		[HideInInspector] _Shininess ("Shininess (only for Lightmapper)", Float) = 0.5
		[HideInInspector] _AO ("Ambient Occlusion Alpha (A)", 2D) = "white" {}
	
	}
	SubShader {
		Tags { "RenderType"="LuxOpaque" }
		LOD 400
	//	Built in Fog breaks rendering using directX and only one pixel light
		Fog { Mode Off } 

		CGPROGRAM
		#pragma surface surf LuxTranslucent noambient exclude_path:prepass fullforwardshadows vertex:vert nodirlightmap nolightmap finalcolor:customFogExp2
		#pragma glsl
		#pragma target 3.0


		#define LUX_LIGHTING_BP
		#define LUX_LINEAR
		#define DIFFCUBE_ON
		#define SPECCUBE_ON
	//	#define LUX_AO_OFF

		// include should be called after all defines
		#include "../LuxCore/LuxLightingDirect.cginc"


		sampler2D _MainTex;
		sampler2D _SpecTex;
		sampler2D _BumpMap;
		sampler2D _Thickness;
		#ifdef DIFFCUBE_ON
			samplerCUBE _DiffCubeIBL;
		#endif
		#ifdef SPECCUBE_ON
			samplerCUBE _SpecCubeIBL;
		#endif
		#ifdef LUX_AO_ON
			sampler2D _AO;
		#endif
		
		fixed4 _Color;
		float _Scale;
		float _Power;
		float _Distortion;
		fixed3 _SubColor;

		// Is set by script
		float4 ExposureIBL;

		struct Input {
			float2 uv_MainTex;
			// distance needed by custom fog
			float2 PureLightAtten_Distance;
			#ifdef LUX_AO_ON
				float2 uv_AO;
			#endif
			float3 viewDir;
			float3 worldNormal;
			float3 worldRefl;
			INTERNAL_DATA
		};
		
		//#define SurfaceOutputLux SurfaceOutputLuxSkin
		
		// Define LUX_CAMERADISTANCE as PureLightAtten_Distance.y
		#define LUX_CAMERADISTANCE IN.PureLightAtten_Distance.y
		#include "../LuxCore/LuxCustomFog.cginc"

		void vert (inout appdata_full v, out Input o) {
			UNITY_INITIALIZE_OUTPUT(Input,o);
			o.PureLightAtten_Distance = 1;
			#ifdef POINT
				float3 myLightCoord = mul(_LightMatrix0, mul(_Object2World, v.vertex)).xyz;
				// o.PureLightAtten = tex2D(_LightTexture0, dot(myLightCoord,myLightCoord).rr).UNITY_ATTEN_CHANNEL;
				// dx11 needs tex2Dlod here
				o.PureLightAtten_Distance.x = tex2Dlod(_LightTexture0, float4( dot(myLightCoord,myLightCoord).rr, 0, 1) ).UNITY_ATTEN_CHANNEL;
			#endif
			#ifdef SPOT
				float4 myLightCoord = mul(_LightMatrix0, mul(_Object2World, v.vertex));
				// o.PureLightAtten = UnitySpotCookie(myLightCoord.xyzw) * UnitySpotAttenuate(myLightCoord.xyz);
				// dx11 needs tex2Dlod here
				o.PureLightAtten_Distance.x = float( tex2Dlod(_LightTexture0, float4(myLightCoord.xy / myLightCoord.w + 0.5, 0, 1)) .a); // UnitySpotCookie
				o.PureLightAtten_Distance.x *= tex2Dlod(_LightTexture0, float4( dot(myLightCoord,myLightCoord).rr, 0, 1) ).UNITY_ATTEN_CHANNEL; // UnitySpotAttenuate
			#endif
			#ifdef DIRECTIONAL
				o.PureLightAtten_Distance.x = 1;
			#endif
			// Calc distance for custom fog function
			o.PureLightAtten_Distance.y = length(mul(UNITY_MATRIX_MV, v.vertex).xyz);

		}

		void surf (Input IN, inout SurfaceOutputLux o) {
			fixed4 diff_albedo = tex2D(_MainTex, IN.uv_MainTex);
			fixed4 spec_albedo = tex2D(_SpecTex, IN.uv_MainTex);
			o.Albedo = diff_albedo * _Color.rgb;
			o.Alpha = tex2D(_Thickness, IN.uv_MainTex).g * IN.PureLightAtten_Distance.x;
			o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
			// Specular Color
			o.SpecularColor = spec_albedo.rgb;
			// Roughness – gamma for BlinnPhong / linear for CookTorrence
			o.Specular = LuxAdjustSpecular(spec_albedo.a);

			#include "../LuxCore/LuxLightingAmbient.cginc"
		}

		inline fixed4 LightingLuxTranslucent (SurfaceOutputLux s, fixed3 lightDir, fixed3 viewDir, fixed atten)
		{		
			viewDir = normalize ( viewDir );
			lightDir = normalize ( lightDir );

			half3 h = normalize (lightDir + viewDir);
			// dotNL has to have max
			float dotNL = max (0, dot (s.Normal, lightDir));
			float dotNH = max (0, dot (s.Normal, h));

			// Translucency
			half3 transLightDir = lightDir + s.Normal * _Distortion;
			float transDot = pow ( saturate(dot ( viewDir, -transLightDir ) ), _Power ) * _Scale;
			fixed3 transLight = (s.Alpha * 2) * ( transDot * _SubColor.rgb );
			fixed3 transAlbedo = s.Albedo * _LightColor0.rgb * transLight;

		//	////////////////////////////////////////////////////////////
		//	Blinn Phong	
//			#if defined (LUX_LIGHTING_BP)
			// bring specPower into a range of 0.25 – 2048
			float specPower = exp2(10 * s.Specular + 1) - 1.75;

		//	Specular: Phong lobe normal distribution function
			float spec = specPower * 0.125 * pow(dotNH, specPower);

		//	Visibility: Schlick-Smith
			float alpha = 2.0 / sqrt( Pi * (specPower + 2) );
			float visibility = 1.0 / ( (dotNL * (1 - alpha) + alpha) * ( saturate(dot(s.Normal, viewDir)) * (1 - alpha) + alpha) ); 
			spec *= visibility;
//			#endif

		//	Fresnel: Schlick
			// fast fresnel approximation:
			fixed3 fresnel = s.SpecularColor.rgb + ( 1.0 - s.SpecularColor.rgb) * exp2(-OneOnLN2_x6 * dot(h, lightDir));
			// from here on we use fresnel instead of spec as it is fixed3 = color
			fresnel *= spec;
			
		// Final Composition
			fixed3 directLighting;
			// we only use fresnel here / and apply late dotNL
			directLighting = (s.Albedo + fresnel) * _LightColor0.rgb * dotNL * (atten * 2);

		//	Add the two together
			fixed4 c;
			c.rgb = directLighting + transAlbedo;
			c.a = _LightColor0.a * _SpecColor.a * spec * atten;
			return c;
		}

		ENDCG
	

	}
	FallBack "Bumped Diffuse"
	CustomEditor "LuxMaterialInspector"
}