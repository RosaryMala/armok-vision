// Based on:
// http://amd-dev.wpengine.netdna-cdn.com/wordpress/media/2012/10/Scheuermann_HairRendering.pdf
// http://blog.leocov.com/2010/08/lchairshadercgfx-maya-realtime-hair.html

Shader "Lux/Human/Hair" {
      Properties {
          _Color ("Main Color", Color) = (1,1,1,1)
          _MainTex ("Diffuse (RGB) Alpha (A)", 2D) = "white" {}
          _SpecularTex ("Specular Shift (R) Roughness (G) Noise (B)", 2D) = "black" {}
          _BumpMap ("Normal (Normal)", 2D) = "bump" {}
          
          _AnisoDir ("Anisotropic Direction (XYZ)" , Vector) = (0.0,1.0,0.0,0.0)

          _SpecularColor1 ("Specular Color1", Color) = (1,1,1,1)
          _PrimaryShift ("Primary Spec Shift", Float)  = 0.5
          _Roughness1 ("Roughness1", Range(0,1)) = 0.5

          _SpecularColor2 ("Specular Color2", Color) = (1,1,1,1)
          _SecondaryShift ("Secondary Spec Shift", Float)  = 0.5
          _Roughness2 ("Roughness2", Range(0,1)) = 0.5

          _RimStrength("Rim Light Strength", Range(0,1)) = 0.5
          _Cutoff ("Alpha cutoff", Range(0,1)) = 0.5

          _DiffCubeIBL ("Custom Diffuse Cube", Cube) = "black" {}
          _SpecCubeIBL ("Custom Specular Cube", Cube) = "black" {}
          
          // _Shininess property is needed by the lightmapper - otherwise it throws errors
      [HideInInspector] _Shininess ("Shininess (only for Lightmapper)", Float) = 0.5
      
      }

      SubShader{
      //  We have use LuxTransparentCutout here
          Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="LuxTransparentCutout"}
      //  Built in Fog breaks rendering using directX and only one pixel light
          Fog { Mode Off }
    
          CGPROGRAM
          #pragma surface surf LuxHair fullforwardshadows noambient nodirlightmap nolightmap alphatest:_Cutoff vertex:vert finalcolor:customFogExp2
          #pragma target 3.0
          #pragma glsl

          #pragma multi_compile LUX_LINEAR LUX_GAMMA
          #pragma multi_compile DIFFCUBE_ON DIFFCUBE_OFF
          #pragma multi_compile SPECCUBE_ON SPECCUBE_OFF

      //  #define LUX_LINEAR
      //  #define DIFFCUBE_ON
      //  #define SPECCUBE_ON
      
      //  Ambient Occlusion is stored in vertex color red
          #define LUX_AO_OFF

      //  Needed in case the diffCUbe is disabled to fall back to sh
          #define LUX_LIGHTMAP_OFF
              
          fixed4 _Color;
          fixed4 _AnisoDir;
          fixed4 _SpecularColor1;
          fixed4 _SpecularColor2;

          float _PrimaryShift;
          float _SecondaryShift;
          float _Roughness1;
          float _Roughness2;
          float _RimStrength;

          sampler2D _MainTex;
          sampler2D _SpecularTex;
          sampler2D _BumpMap;

          #ifdef DIFFCUBE_ON
            samplerCUBE _DiffCubeIBL;
          #endif
          #ifdef SPECCUBE_ON
            samplerCUBE _SpecCubeIBL;
          #endif
          
          // Is set by script
          float4 ExposureIBL;

          // As we do not include Lux direct lighting functions we have to define these constants here
          #define OneOnLN2_x6 8.656170
          #define Pi 3.14159265358979323846

          struct SurfaceOutputLuxHair {
              fixed3 Albedo;
              fixed3 Normal;
              fixed3 Emission;
              half Specular;
              half2 Specular12;
              fixed3 SpecularColor;
              fixed4 AnisoDir;
              fixed SpecShift;
              fixed SpecNoise;
              fixed Alpha;
          };

          struct Input
          {
              float2 uv_MainTex;
              float4 color : COLOR; // R stores Ambient Occlusion
              float4 worldPosLux; // needed by custom fog
              float3 viewDir;
              float3 worldNormal;
              float3 worldRefl;
              INTERNAL_DATA
          };
          
          // Include CustomFog
          #define SurfaceOutputLux SurfaceOutputLuxHair
          // Define LUX_CAMERADISTANCE as IN.worldPosLux.w
          #define LUX_CAMERADISTANCE IN.worldPosLux.w
          #include "../../LuxCore/LuxCustomFog.cginc"
          
          void vert (inout appdata_full v, out Input o) 
          {
              UNITY_INITIALIZE_OUTPUT(Input,o);
              o.worldPosLux.xyz = 0; //mul(_Object2World, v.vertex).xyz;
              o.worldPosLux.w = length(mul(UNITY_MATRIX_MV, v.vertex).xyz);
          }

          void surf (Input IN, inout SurfaceOutputLuxHair o)
          {
              fixed4 albedo = tex2D(_MainTex, IN.uv_MainTex);
              o.Albedo = albedo.rgb * _Color.rgb;
              o.Alpha = albedo.a * _Color.a;
              o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
              // r: spec shift / g: roughness / b: noise
              fixed3 spec = tex2D(_SpecularTex, IN.uv_MainTex).rgb;
              // store per pixel Spec Shift
              o.SpecShift = spec.r * 2 -1;
              // Calculate primary per Pixel Roughness * Roughness1
              o.Specular = spec.g * _Roughness1;
              // Store Roughness for direct lighting
              o.Specular12 = half2(o.Specular, spec.g * _Roughness2);
              // store per pixel Spec Noise
              o.SpecNoise = spec.b;

              // Lux Ambient Lighting functions also need o.SpecularColor(rgb) 
              // So we have to make it a bit more complicated here
              // Tweak Roughness for ambient lighting
              o.Specular *= o.Specular; 
              o.SpecularColor = _SpecularColor1.rgb;

              #include "../../LuxCore/LuxLightingAmbient.cginc"

              // Add ambient occlusion stored in vertex color red
              o.Emission *= IN.color.r;
          }

//  Custom Lighting
          
          inline float3 KajiyaKay (float3 N, float3 T, float3 H, float specNoise) 
          {
            float3 B = normalize(T + N * specNoise);
            //return sqrt(1-pow(dot(B,H),2));
            float dotBH = dot(B,H);
            return sqrt(1-dotBH*dotBH);
          }

          inline fixed4 LightingLuxHair (SurfaceOutputLuxHair s, fixed3 lightDir, fixed3 viewDir, fixed atten)
          {
            fixed3 h = normalize(normalize(lightDir) + normalize(viewDir));
            float dotNL = max(0,dot(s.Normal, lightDir));
            
        //  Spec
            float2 specPower = exp2(10 * s.Specular12 + 1) - 1.75;

            // First specular Highlight / Do not add specNoise here 
            float3 H = normalize(lightDir + viewDir);
            float3 spec1 = specPower.x * pow( KajiyaKay(s.Normal, _AnisoDir * s.SpecShift, H, _PrimaryShift), specPower.x);
            // Add 2nd specular Highlight
            float3 spec2 = specPower.y * pow( KajiyaKay(s.Normal, _AnisoDir * s.SpecShift, H, _SecondaryShift ), specPower.y) * s.SpecNoise;
        
        //  Fresnel
            fixed fresnel = exp2(-OneOnLN2_x6 * dot(h, lightDir));
            spec1 *= _SpecularColor1 + ( 1.0 - _SpecularColor1 ) * fresnel;
            spec2 *= _SpecularColor2 + ( 1.0 - _SpecularColor2 ) * fresnel;    
            spec1 += spec2;

            // Normalize
            spec1 *= 0.125 * dotNL;

            // Rim
            fixed RimPower = saturate (1.0 - dot(s.Normal, viewDir));
            fixed Rim = _RimStrength * RimPower*RimPower;

            fixed4 c;
            // Diffuse Lighting: Lerp shifts the shadow boundrary for a softer look
            float3 diffuse = saturate (lerp (0.25, 1.0, dotNL));
            // Combine
            c.rgb = ((s.Albedo + Rim) * diffuse + spec1) * _LightColor0.rgb  * (atten * 2);
            c.a = s.Alpha;
            return c;
          }

          ENDCG
      }
      Fallback "Transparent/Cutout/VertexLit"
      CustomEditor "LuxMaterialInspector"
}