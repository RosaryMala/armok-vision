Shader "Hidden/IndirectSpecularLight"
{
	Properties
	{
		_Skybox("_Skybox", CUBE) = "white" {}
		_A ("Normal", 2D) = "white" {}
		_B ("Smoothness", 2D) = "white" {}
		_C ("Occlusion", 2D) = "white" {}
	}

	SubShader
	{
		Pass // not connected
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"

			uniform samplerCUBE _Skybox;
			sampler2D _A;
			sampler2D _B;
			sampler2D _C;

			float4 frag(v2f_img i) : SV_Target
			{
				float2 xy = 2 * i.uv - 1;
				float z = -sqrt(1-saturate(dot(xy,xy)));
				float3 worldNormal = normalize(float3(xy, z));
				float3 vertexPos = float3(xy, z);
				float3 worldViewDir = normalize(float3(0,0,-5) - vertexPos);

				float3 worldRefl = -worldViewDir;
				worldRefl = normalize(reflect( worldRefl, worldNormal ));

				float3 sky = texCUBElod( _Skybox, float4(worldRefl, (1-saturate(tex2D(_B,i.uv).r)) * 6) ).rgb;

				return float4(sky * tex2D(_C,i.uv).r, 1);
			}
			ENDCG
		}

		Pass // connected tangent
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"

			uniform samplerCUBE _Skybox;
			sampler2D _A;
			sampler2D _B;
			sampler2D _C;

			float4 frag(v2f_img i) : SV_Target
			{
				float2 xy = 2 * i.uv - 1;
				float z = -sqrt(1-saturate(dot(xy,xy)));
				float3 vertexPos = float3(xy, z);
				float3 normal = normalize(vertexPos);
				float3 worldNormal = UnityObjectToWorldNormal(normal);

				float3 tangent = normalize(float3( -z, xy.y*0.01, xy.x ));
				float3 worldPos = mul(unity_ObjectToWorld, float4(vertexPos,1)).xyz;
				float3 worldViewDir = normalize(float3(0,0,-5) - vertexPos);
				
				float3 worldTangent = UnityObjectToWorldDir(tangent);
				float tangentSign = -1;
				float3 worldBinormal = normalize( cross(worldNormal, worldTangent) * tangentSign);
				float4 tSpace0 = float4(worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x);
				float4 tSpace1 = float4(worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y);
				float4 tSpace2 = float4(worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z);

				float3 worldRefl = -worldViewDir;

				float2 sphereUVs = i.uv;
				sphereUVs.x = atan2(vertexPos.x, -vertexPos.z) / (UNITY_PI) + 0.5;
				
				// Needs further checking
				//float3 tangentNormal = tex2Dlod(_A, float4(sphereUVs,0,0)).xyz;
				float3 tangentNormal = tex2D(_A, sphereUVs).xyz;

				worldRefl = reflect( worldRefl, half3( dot( tSpace0.xyz, tangentNormal ), dot( tSpace1.xyz, tangentNormal ), dot( tSpace2.xyz, tangentNormal ) ) );

				float3 sky = texCUBElod( _Skybox, float4(worldRefl, (1-saturate(tex2D(_B,i.uv).r)) * 6) ).rgb;

				return float4(sky * tex2D(_C,i.uv).r, 1);
			}
			ENDCG
		}

		Pass // connected world
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"

			uniform samplerCUBE _Skybox;
			sampler2D _A;
			sampler2D _B;
			sampler2D _C;

			float4 frag(v2f_img i) : SV_Target
			{
				float2 xy = 2 * i.uv - 1;
				float z = -sqrt(1-saturate(dot(xy,xy)));
				float3 vertexPos = float3(xy, z);
				float3 normal = normalize(vertexPos);
				float3 worldNormal = tex2D( _A, i.uv );
				float3 worldViewDir = normalize(float3(0,0,-5) - vertexPos);
				
				float3 worldRefl = reflect( -worldViewDir, worldNormal );

				float3 sky = texCUBElod( _Skybox, float4(worldRefl, (1-saturate(tex2D(_B,i.uv).r)) * 6) ).rgb;

				return float4(sky * tex2D(_C,i.uv).r, 1);
			}
			ENDCG
		}
	}
}
