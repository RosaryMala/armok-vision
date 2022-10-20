// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Skybox/Cubemap Extended"
{
	Properties
	{
		[StyledBanner(Skybox Cubemap Extended)]_SkyboxExtended("< SkyboxExtended >", Float) = 1
		[StyledCategory(Cubemap Settings, 5, 10)]_Cubemapp("[ Cubemapp ]", Float) = 1
		[NoScaleOffset][StyledTextureSingleLine]_Tex("Cubemap (HDR)", CUBE) = "black" {}
		[Space(10)]_Exposure("Cubemap Exposure", Range( 0 , 8)) = 1
		[Gamma]_TintColor("Cubemap Tint Color", Color) = (0.5,0.5,0.5,1)
		_CubemapPosition("Cubemap Position", Float) = 0
		[StyledCategory(Rotation Settings)]_Rotationn("[ Rotationn ]", Float) = 1
		[Toggle(_ENABLEROTATION_ON)] _EnableRotation("Enable Rotation", Float) = 0
		[IntRange][Space(10)]_Rotation("Rotation", Range( 0 , 360)) = 0
		_RotationSpeed("Rotation Speed", Float) = 1
		[StyledCategory(Fog Settings)]_Fogg("[ Fogg ]", Float) = 1
		[Toggle(_ENABLEFOG_ON)] _EnableFog("Enable Fog", Float) = 0
		[StyledMessage(Info, The fog color is controlled by the fog color set in the Lighting panel., _EnableFog, 1, 10, 0)]_FogMessage("# FogMessage", Float) = 0
		[Space(10)]_FogIntensity("Fog Intensity", Range( 0 , 1)) = 1
		_FogHeight("Fog Height", Range( 0 , 1)) = 1
		_FogSmoothness("Fog Smoothness", Range( 0.01 , 1)) = 0.01
		_FogFill("Fog Fill", Range( 0 , 1)) = 0.5
		[HideInInspector]_Tex_HDR("DecodeInstructions", Vector) = (0,0,0,0)
		[ASEEnd]_FogPosition("Fog Position", Float) = 0

	}
	
	SubShader
	{
		
		
		Tags { "RenderType"="Background" "Queue"="Background" "PreviewType"="Skybox" }
	LOD 0

		CGINCLUDE
		#pragma target 2.0
		ENDCG
		Blend Off
		AlphaToMask Off
		Cull Off
		ColorMask RGBA
		ZWrite Off
		ZTest LEqual
		
		
		
		Pass
		{
			Name "Unlit"

			CGPROGRAM

			

			#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
			//only defining to not throw compilation error over Unity 5.5
			#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
			#endif
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"
			#include "UnityShaderVariables.cginc"
			#define ASE_NEEDS_VERT_POSITION
			#pragma shader_feature_local _ENABLEFOG_ON
			#pragma shader_feature_local _ENABLEROTATION_ON


			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 worldPos : TEXCOORD0;
				#endif
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			uniform half _Cubemapp;
			uniform half _SkyboxExtended;
			uniform half _Rotationn;
			uniform half _FogMessage;
			uniform half4 _Tex_HDR;
			uniform half _Fogg;
			uniform samplerCUBE _Tex;
			uniform float _CubemapPosition;
			uniform half _Rotation;
			uniform half _RotationSpeed;
			uniform half4 _TintColor;
			uniform half _Exposure;
			uniform float _FogPosition;
			uniform half _FogHeight;
			uniform half _FogSmoothness;
			uniform half _FogFill;
			uniform half _FogIntensity;
			inline half3 DecodeHDR1189( float4 Data )
			{
				return DecodeHDR(Data, _Tex_HDR);
			}
			

			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				float lerpResult268 = lerp( 1.0 , ( unity_OrthoParams.y / unity_OrthoParams.x ) , unity_OrthoParams.w);
				half CAMERA_MODE300 = lerpResult268;
				float3 appendResult1220 = (float3(v.vertex.xyz.x , ( v.vertex.xyz.y * CAMERA_MODE300 ) , v.vertex.xyz.z));
				float3 appendResult1208 = (float3(0.0 , -_CubemapPosition , 0.0));
				half3 VertexPos40_g1 = appendResult1220;
				float3 appendResult74_g1 = (float3(0.0 , VertexPos40_g1.y , 0.0));
				float3 VertexPosRotationAxis50_g1 = appendResult74_g1;
				float3 break84_g1 = VertexPos40_g1;
				float3 appendResult81_g1 = (float3(break84_g1.x , 0.0 , break84_g1.z));
				float3 VertexPosOtherAxis82_g1 = appendResult81_g1;
				half Angle44_g1 = ( 1.0 - radians( ( _Rotation + ( _Time.y * _RotationSpeed ) ) ) );
				#ifdef _ENABLEROTATION_ON
				float3 staticSwitch1164 = ( ( VertexPosRotationAxis50_g1 + ( VertexPosOtherAxis82_g1 * cos( Angle44_g1 ) ) + ( cross( float3(0,1,0) , VertexPosOtherAxis82_g1 ) * sin( Angle44_g1 ) ) ) + appendResult1208 );
				#else
				float3 staticSwitch1164 = ( appendResult1220 + appendResult1208 );
				#endif
				float3 vertexToFrag774 = staticSwitch1164;
				o.ase_texcoord1.xyz = vertexToFrag774;
				
				o.ase_texcoord2 = v.vertex;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord1.w = 0;
				float3 vertexValue = float3(0, 0, 0);
				#if ASE_ABSOLUTE_VERTEX_POS
				vertexValue = v.vertex.xyz;
				#endif
				vertexValue = vertexValue;
				#if ASE_ABSOLUTE_VERTEX_POS
				v.vertex.xyz = vertexValue;
				#else
				v.vertex.xyz += vertexValue;
				#endif
				o.vertex = UnityObjectToClipPos(v.vertex);

				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				#endif
				return o;
			}
			
			fixed4 frag (v2f i ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
				fixed4 finalColor;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 WorldPosition = i.worldPos;
				#endif
				float3 vertexToFrag774 = i.ase_texcoord1.xyz;
				half4 Data1189 = texCUBE( _Tex, vertexToFrag774 );
				half3 localDecodeHDR1189 = DecodeHDR1189( Data1189 );
				half4 CUBEMAP222 = ( float4( localDecodeHDR1189 , 0.0 ) * unity_ColorSpaceDouble * _TintColor * _Exposure );
				float lerpResult678 = lerp( saturate( pow( (0.0 + (abs( ( i.ase_texcoord2.xyz.y + -_FogPosition ) ) - 0.0) * (1.0 - 0.0) / (_FogHeight - 0.0)) , ( 1.0 - _FogSmoothness ) ) ) , 0.0 , _FogFill);
				float lerpResult1205 = lerp( 1.0 , lerpResult678 , _FogIntensity);
				half FOG_MASK359 = lerpResult1205;
				float4 lerpResult317 = lerp( unity_FogColor , CUBEMAP222 , FOG_MASK359);
				#ifdef _ENABLEFOG_ON
				float4 staticSwitch1179 = lerpResult317;
				#else
				float4 staticSwitch1179 = CUBEMAP222;
				#endif
				
				
				finalColor = staticSwitch1179;
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "SkyboxExtendedShaderGUI"
	
	Fallback "Skybox/Cubemap"
}
/*ASEBEGIN
Version=18908
1920;1;1906;1021;-560.318;-1111.413;1;True;False
Node;AmplifyShaderEditor.OrthoParams;267;-892.4822,894.6918;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;1007;-444.4821,894.6918;Half;False;Constant;_Float7;Float 7;47;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;309;-588.4823,894.6918;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;268;-252.4821,894.6918;Inherit;False;3;0;FLOAT;1;False;1;FLOAT;0.5;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;260;-896,2048;Half;False;Property;_RotationSpeed;Rotation Speed;9;0;Create;True;0;0;0;False;0;False;1;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;701;-896,1920;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;48;-896,1792;Half;False;Property;_Rotation;Rotation;8;1;[IntRange];Create;True;0;0;0;False;1;Space(10);False;0;0;0;360;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;300;3.51777,894.6918;Half;False;CAMERA_MODE;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;255;-640,1920;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;1221;-896,1536;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;1218;-896,1680;Inherit;False;300;CAMERA_MODE;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;276;-512,1792;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1211;-128,1920;Inherit;False;Property;_CubemapPosition;Cubemap Position;5;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RadiansOpNode;47;-384,1792;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1207;-896,3008;Inherit;False;Property;_FogPosition;Fog Position;18;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1219;-640,1664;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NegateNode;1214;-704,3008;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;1193;-896,2560;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;1220;-512,1536;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.NegateNode;1215;128,1920;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;1222;-256,1792;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;1208;320,1920;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.FunctionNode;1217;-128,1536;Inherit;False;Compute Rotation Y;-1;;1;693b7d13a80c93a4e8b791a9cd5e5ab2;0;2;38;FLOAT3;0,0,0;False;43;FLOAT;0;False;1;FLOAT3;19
Node;AmplifyShaderEditor.SimpleAddOpNode;1210;-640,2560;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;325;-896,2880;Half;False;Property;_FogSmoothness;Fog Smoothness;15;0;Create;True;0;0;0;False;0;False;0.01;0.01;0.01;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.AbsOpNode;314;-512,2560;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;1206;512,1792;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;313;-896,2752;Half;False;Property;_FogHeight;Fog Height;14;0;Create;True;0;0;0;False;0;False;1;0.6;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;1212;512,1536;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;1109;-512,2784;Half;False;Constant;_Float40;Float 40;55;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1108;-512,2688;Half;False;Constant;_Float39;Float 39;55;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;329;-256,2880;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;315;-320,2560;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;1164;704,1536;Float;False;Property;_EnableRotation;Enable Rotation;7;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;9;1;FLOAT3;0,0,0;False;0;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT3;0,0,0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.VertexToFragmentNode;774;1024,1536;Inherit;False;False;False;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.PowerNode;677;-64,2560;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1110;128,2752;Half;False;Constant;_Float41;Float 41;55;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;41;1536,1536;Inherit;True;Property;_Tex;Cubemap (HDR);2;1;[NoScaleOffset];Create;False;0;0;0;False;1;StyledTextureSingleLine;False;-1;None;beb1457d375110e468b8d8e1f29fccea;True;0;False;black;LockedToCube;False;Object;-1;Auto;Cube;8;0;SAMPLERCUBE;;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;316;128,2560;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;679;128,2880;Half;False;Property;_FogFill;Fog Fill;16;0;Create;True;0;0;0;False;0;False;0.5;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1177;1920,1968;Half;False;Property;_Exposure;Cubemap Exposure;3;0;Create;False;0;0;0;False;1;Space(10);False;1;1;0;8;0;1;FLOAT;0
Node;AmplifyShaderEditor.CustomExpressionNode;1189;1920,1536;Half;False;DecodeHDR(Data, _Tex_HDR);3;Create;1;True;Data;FLOAT4;0,0,0,0;In;;Float;False;DecodeHDR;True;False;0;;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;1173;1920,1792;Half;False;Property;_TintColor;Cubemap Tint Color;4;1;[Gamma];Create;False;0;0;0;False;0;False;0.5,0.5,0.5,1;0.5,0.5,0.5,1;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;1204;448,2880;Half;False;Property;_FogIntensity;Fog Intensity;13;0;Create;True;0;0;0;False;1;Space(10);False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;678;384,2560;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorSpaceDouble;1175;1920,1616;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;1174;2432,1536;Inherit;False;4;4;0;FLOAT3;0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;1205;640,2560;Inherit;False;3;0;FLOAT;1;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;359;832,2560;Half;False;FOG_MASK;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;222;2624,1536;Half;False;CUBEMAP;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;228;-896,240;Inherit;False;222;CUBEMAP;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.FogAndAmbientColorsNode;312;-896,128;Inherit;False;unity_FogColor;0;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;436;-896,320;Inherit;False;359;FOG_MASK;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;317;-512,128;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;1197;-640,-384;Half;False;Property;_Cubemapp;[ Cubemapp ];1;0;Create;True;0;0;0;True;1;StyledCategory(Cubemap Settings, 5, 10);False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1196;-896,-384;Half;False;Property;_SkyboxExtended;< SkyboxExtended >;0;0;Create;True;0;0;0;True;1;StyledBanner(Skybox Cubemap Extended);False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1198;-448,-384;Half;False;Property;_Rotationn;[ Rotationn ];6;0;Create;True;0;0;0;True;1;StyledCategory(Rotation Settings);False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1216;-896,3136;Half;False;Property;_FogMessage;# FogMessage;12;0;Create;True;0;0;0;True;1;StyledMessage(Info, The fog color is controlled by the fog color set in the Lighting panel., _EnableFog, 1, 10, 0);False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;1179;-224,224;Float;False;Property;_EnableFog;Enable Fog;11;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.Vector4Node;1190;1536,1792;Half;False;Property;_Tex_HDR;DecodeInstructions;17;1;[HideInInspector];Create;False;0;0;0;True;0;False;0,0,0,0;1,1,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;1199;-272,-384;Half;False;Property;_Fogg;[ Fogg ];10;0;Create;True;0;0;0;True;1;StyledCategory(Fog Settings);False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;1194;128,128;Float;False;True;-1;2;SkyboxExtendedShaderGUI;0;1;Skybox/Cubemap Extended;0770190933193b94aaa3065e307002fa;True;Unlit;0;0;Unlit;2;False;True;0;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;True;0;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;True;0;False;-1;False;True;2;False;-1;False;True;True;True;True;True;0;False;-1;False;False;False;False;False;False;False;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;2;False;-1;True;0;False;-1;True;False;0;False;-1;0;False;-1;True;3;RenderType=Background=RenderType;Queue=Background=Queue=0;PreviewType=Skybox;True;0;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;0;Skybox/Cubemap;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;1;True;False;;False;0
Node;AmplifyShaderEditor.CommentaryNode;700;-896,2432;Inherit;False;1920.275;100;Fog Coords on Screen;0;;0,0.4980392,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;1180;-896,1408;Inherit;False;2179.583;100;Cubemap Coordinates;0;;0,0.4980392,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;1195;-896,-512;Inherit;False;1203;100;Drawers;0;;1,0.6827586,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;1191;1536,1408;Inherit;False;1280.6;100;Base;0;;0,0.4980392,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;1167;-896,0;Inherit;False;1236;100;Final Color;0;;0.4980392,1,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;431;-896,768;Inherit;False;1094;100;Switch between Perspective / Orthographic camera;0;;1,0,1,1;0;0
WireConnection;309;0;267;2
WireConnection;309;1;267;1
WireConnection;268;0;1007;0
WireConnection;268;1;309;0
WireConnection;268;2;267;4
WireConnection;300;0;268;0
WireConnection;255;0;701;0
WireConnection;255;1;260;0
WireConnection;276;0;48;0
WireConnection;276;1;255;0
WireConnection;47;0;276;0
WireConnection;1219;0;1221;2
WireConnection;1219;1;1218;0
WireConnection;1214;0;1207;0
WireConnection;1220;0;1221;1
WireConnection;1220;1;1219;0
WireConnection;1220;2;1221;3
WireConnection;1215;0;1211;0
WireConnection;1222;0;47;0
WireConnection;1208;1;1215;0
WireConnection;1217;38;1220;0
WireConnection;1217;43;1222;0
WireConnection;1210;0;1193;2
WireConnection;1210;1;1214;0
WireConnection;314;0;1210;0
WireConnection;1206;0;1220;0
WireConnection;1206;1;1208;0
WireConnection;1212;0;1217;19
WireConnection;1212;1;1208;0
WireConnection;329;0;325;0
WireConnection;315;0;314;0
WireConnection;315;1;1108;0
WireConnection;315;2;313;0
WireConnection;315;3;1108;0
WireConnection;315;4;1109;0
WireConnection;1164;1;1206;0
WireConnection;1164;0;1212;0
WireConnection;774;0;1164;0
WireConnection;677;0;315;0
WireConnection;677;1;329;0
WireConnection;41;1;774;0
WireConnection;316;0;677;0
WireConnection;1189;0;41;0
WireConnection;678;0;316;0
WireConnection;678;1;1110;0
WireConnection;678;2;679;0
WireConnection;1174;0;1189;0
WireConnection;1174;1;1175;0
WireConnection;1174;2;1173;0
WireConnection;1174;3;1177;0
WireConnection;1205;1;678;0
WireConnection;1205;2;1204;0
WireConnection;359;0;1205;0
WireConnection;222;0;1174;0
WireConnection;317;0;312;0
WireConnection;317;1;228;0
WireConnection;317;2;436;0
WireConnection;1179;1;228;0
WireConnection;1179;0;317;0
WireConnection;1194;0;1179;0
ASEEND*/
//CHKSM=8F14882A178A7BE958CE2E140F1BAF4D7D416A0B