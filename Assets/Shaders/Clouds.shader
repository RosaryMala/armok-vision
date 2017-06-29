Shader "Custom/Clouds" {
	Properties {
		_Color("Color ", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
            _Curvature("Curvature", Float) = 0.0001
	}
	SubShader {
		Tags { "Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout" }
		LOD 200
		Cull Off
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard addshadow alphatest:_Cutoff vertex:vert

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input {
			float2 uv_MainTex;
			float4 color: Color;
			float3 worldPos;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		float noise(float3 x) {
			float3 p = floor(x);
			float3 f = frac(x);
			f = f*f*(3. - 2.*f);

			float n = p.x + p.y*157. + 113.*p.z;

			float4 v1 = frac(753.5453123*sin(n + float4(0., 1., 157., 158.)));
			float4 v2 = frac(753.5453123*sin(n + float4(113., 114., 270., 271.)));
			float4 v3 = lerp(v1, v2, f.z);
			float2 v4 = lerp(v3.xy, v3.zw, f.y);
			return lerp(v4.x, v4.y, f.x);
		}

		float fnoise(float3 p) {
			// random rotation reduces artifacts
			p = mul(float3x3(0.28862355854826727, 0.6997227302779844, 0.6535170557707412,
				0.06997493955670424, 0.6653237235314099, -0.7432683571499161,
				-0.9548821651308448, 0.26025457467376617, 0.14306504491456504), p);
			return dot(float4(noise(p), noise(p*2.), noise(p*4.), noise(p*8.)),
				float4(0.5, 0.25, 0.125, 0.06));
		}

        float _Curvature;
        // This is where the curvature is applied
        void vert(inout appdata_full v)
        {
            // Transform the vertex coordinates from model space into world space
            float4 vv = mul(unity_ObjectToWorld, v.vertex);

            // Now adjust the coordinates to be relative to the camera position
            vv.xyz -= _WorldSpaceCameraPos.xyz;

            // Reduce the y coordinate (i.e. lower the "height") of each vertex based
            // on the square of the distance from the camera in the z axis, multiplied
            // by the chosen curvature factor
            vv = float4(0.0f, ((vv.z * vv.z) + (vv.x * vv.x)) * -_Curvature, 0.0f, 0.0f);

            // Now apply the offset back to the vertices in model space
            v.vertex += mul(unity_WorldToObject, vv);
        }

		void surf (Input IN, inout SurfaceOutputStandard o) {
			fixed n = 0;
			if(IN.color.a > 0 || IN.color.a < 1)
				n = fnoise(IN.worldPos * 0.001);
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = n + IN.color.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
