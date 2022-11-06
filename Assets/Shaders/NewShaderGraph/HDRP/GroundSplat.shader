// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "DF/GroundSplat"
{
	Properties
	{
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		_Tint("Tint", 2D) = "black" {}
		_GrassTint("GrassTint", 2D) = "black" {}
		_Control("Control", 2D) = "black" {}
		_GrassControl("GrassControl", 2D) = "black" {}
		_MetalLevel("Metallic", Range( 0 , 1)) = 0
		_Rough("Roughness", Range( 0 , 1)) = 0.5
		_WorldBounds("WorldBounds", Vector) = (0,0,1,1)
		_MatTexArray("MatTexArray", 2DArray) = "white" {}
		[Normal]_ShapeMap("Shape Texture Splat", 2DArray) = "bump" {}
		_NormalScale("Normal Scale", Float) = 0
		[ASEEnd]_SpatterTex("SpatterTex", 2D) = "black" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}

		[HideInInspector]_EmissionColor("Color", Color) = (1, 1, 1, 1)
		[HideInInspector] _RenderQueueType("Render Queue Type", Float) = 1
		[HideInInspector] [ToggleUI] _AddPrecomputedVelocity("Add Precomputed Velocity", Float) = 1
		[HideInInspector] [ToggleUI]_SupportDecals("Boolean", Float) = 1
		[HideInInspector] _StencilRef("Stencil Ref", Int) = 0
		[HideInInspector] _StencilWriteMask("Stencil Write Mask", Int) = 6
		[HideInInspector] _StencilRefDepth("Stencil Ref Depth", Int) = 8
		[HideInInspector] _StencilWriteMaskDepth("Stencil Write Mask Depth", Int) = 8
		[HideInInspector] _StencilRefMV("Stencil Ref MV", Int) = 40
		[HideInInspector] _StencilWriteMaskMV("Stencil Write Mask MV", Int) = 40
		[HideInInspector] _StencilRefDistortionVec("Stencil Ref Distortion Vec", Int) = 4
		[HideInInspector] _StencilWriteMaskDistortionVec("Stencil Write Mask Distortion Vec", Int) = 4
		[HideInInspector] _StencilWriteMaskGBuffer("Stencil Write Mask GBuffer", Int) = 14
		[HideInInspector] _StencilRefGBuffer("Stencil Ref GBuffer", Int) = 10
		[HideInInspector] _ZTestGBuffer("ZTest GBuffer", Int) = 4
		[HideInInspector] [ToggleUI] _RequireSplitLighting("Require Split Lighting", Float) = 0
		[HideInInspector] [ToggleUI] _ReceivesSSR("Receives SSR", Float) = 0
		[HideInInspector] [ToggleUI] _ReceivesSSRTransparent("Boolean", Float) = 0
		[HideInInspector] _SurfaceType("Surface Type", Float) = 0
		[HideInInspector] _BlendMode("Blend Mode", Float) = 0
		[HideInInspector] _SrcBlend("Src Blend", Float) = 1
		[HideInInspector] _DstBlend("Dst Blend", Float) = 0
		[HideInInspector] _AlphaSrcBlend("Alpha Src Blend", Float) = 1
		[HideInInspector] _AlphaDstBlend("Alpha Dst Blend", Float) = 0
		[HideInInspector][ToggleUI]_AlphaToMask("Boolean", Float) = 0//New
        [HideInInspector][ToggleUI]_AlphaToMaskInspectorValue("Boolean", Float) = 0//New
		[HideInInspector] [ToggleUI] _ZWrite("ZWrite", Float) = 1
		[HideInInspector] [ToggleUI] _TransparentZWrite("Transparent ZWrite", Float) = 1
		[HideInInspector] _CullMode("Cull Mode", Float) = 2
		[HideInInspector] _TransparentSortPriority("Transparent Sort Priority", Int) = 0
		[HideInInspector] [ToggleUI] _EnableFogOnTransparent("Enable Fog On Transparent", Float) = 1
		[HideInInspector] _CullModeForward("Cull Mode Forward", Float) = 2
		[HideInInspector] [Enum(Front, 1, Back, 2)] _TransparentCullMode("Transparent Cull Mode", Float) = 2
		[HideInInspector][Enum(UnityEditor.Rendering.HighDefinition.OpaqueCullMode)]_OpaqueCullMode("Float", Float) = 2
		[HideInInspector] _ZTestDepthEqualForOpaque("ZTest Depth Equal For Opaque", Int) = 4
		[HideInInspector] [Enum(UnityEngine.Rendering.CompareFunction)] _ZTestTransparent("ZTest Transparent", Float) = 4
		[HideInInspector] [ToggleUI] _TransparentBackfaceEnable("Transparent Backface Enable", Float) = 0
		[HideInInspector] [ToggleUI] _AlphaCutoffEnable("Alpha Cutoff Enable", Float) = 0
		[HideInInspector] [ToggleUI] _UseShadowThreshold("Use Shadow Threshold", Float) = 0
		[HideInInspector] [ToggleUI] _DoubleSidedEnable("Double Sided Enable", Float) = 0
		[HideInInspector] [Enum(Flip, 0, Mirror, 1, None, 2)] _DoubleSidedNormalMode("Double Sided Normal Mode", Float) = 2
		[HideInInspector] _DoubleSidedConstants("DoubleSidedConstants", Vector) = (1,1,-1,0)
		[HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}//New
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}//New
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}//New
		//_TessPhongStrength( "Tess Phong Strength", Range( 0, 1 ) ) = 0.5
		//_TessValue( "Tess Max Tessellation", Range( 1, 32 ) ) = 16
		//_TessMin( "Tess Min Distance", Float ) = 10
		//_TessMax( "Tess Max Distance", Float ) = 25
		//_TessEdgeLength ( "Tess Edge length", Range( 2, 50 ) ) = 16
		//_TessMaxDisp( "Tess Max Displacement", Float ) = 25
	}

	SubShader
	{
		LOD 0

		

		Tags { "RenderPipeline"="HDRenderPipeline" "RenderType"="Opaque" "Queue"="Geometry" }

		HLSLINCLUDE
		#pragma target 4.5
		#pragma only_renderers d3d11 metal vulkan xboxone xboxseries playstation switch 
		#pragma multi_compile_instancing
		#pragma instancing_options renderinglayer

		struct GlobalSurfaceDescription // GBuffer Forward META TransparentBackface
		{
			float3 Albedo;
			float3 Normal;
			float3 BentNormal;
			float3 Specular;
			float CoatMask;
			float Metallic;
			float3 Emission;
			float Smoothness;
			float Occlusion;
			float Alpha;
			float AlphaClipThreshold;
			float AlphaClipThresholdShadow;
			float AlphaClipThresholdDepthPrepass;
			float AlphaClipThresholdDepthPostpass;
			float SpecularAAScreenSpaceVariance;
			float SpecularAAThreshold;
			float SpecularOcclusion;
			float DepthOffset;
			//Refraction
			float RefractionIndex;
			float3 RefractionColor;
			float RefractionDistance;
			//SSS/Translucent
			float Thickness;
			float SubsurfaceMask;
			float DiffusionProfile;
			//Anisotropy
			float Anisotropy;
			float3 Tangent;
			//Iridescent
			float IridescenceMask;
			float IridescenceThickness;
			//BakedGI
			float3 BakedGI;
			float3 BakedBackGI;
			//Virtual Texturing
			float4 VTPackedFeedback;
		};

		struct AlphaSurfaceDescription // ShadowCaster
		{
			float Alpha;
			float AlphaClipThreshold;
			float AlphaClipThresholdShadow;
			float DepthOffset;
		};

		struct SceneSurfaceDescription // SceneSelection
		{
			float Alpha;
			float AlphaClipThreshold;
			float DepthOffset;
		};

		struct PrePassSurfaceDescription // DepthPrePass
		{
			float3 Normal;
			float Smoothness;
			float Alpha;
			float AlphaClipThresholdDepthPrepass;
			float DepthOffset;
		};

		struct PostPassSurfaceDescription //DepthPostPass
		{
			float Alpha;
			float AlphaClipThresholdDepthPostpass;
			float DepthOffset;
		};

		struct SmoothSurfaceDescription // MotionVectors DepthOnly
		{
			float3 Normal;
			float Smoothness;
			float Alpha;
			float AlphaClipThreshold;
			float DepthOffset;
		};

		struct DistortionSurfaceDescription //Distortion
		{
			float Alpha;
			float2 Distortion;
			float DistortionBlur;
			float AlphaClipThreshold;
		};
		
		#ifndef ASE_TESS_FUNCS
		#define ASE_TESS_FUNCS
		float4 FixedTess( float tessValue )
		{
			return tessValue;
		}
		
		float CalcDistanceTessFactor (float4 vertex, float minDist, float maxDist, float tess, float4x4 o2w, float3 cameraPos )
		{
			float3 wpos = mul(o2w,vertex).xyz;
			float dist = distance (wpos, cameraPos);
			float f = clamp(1.0 - (dist - minDist) / (maxDist - minDist), 0.01, 1.0) * tess;
			return f;
		}

		float4 CalcTriEdgeTessFactors (float3 triVertexFactors)
		{
			float4 tess;
			tess.x = 0.5 * (triVertexFactors.y + triVertexFactors.z);
			tess.y = 0.5 * (triVertexFactors.x + triVertexFactors.z);
			tess.z = 0.5 * (triVertexFactors.x + triVertexFactors.y);
			tess.w = (triVertexFactors.x + triVertexFactors.y + triVertexFactors.z) / 3.0f;
			return tess;
		}

		float CalcEdgeTessFactor (float3 wpos0, float3 wpos1, float edgeLen, float3 cameraPos, float4 scParams )
		{
			float dist = distance (0.5 * (wpos0+wpos1), cameraPos);
			float len = distance(wpos0, wpos1);
			float f = max(len * scParams.y / (edgeLen * dist), 1.0);
			return f;
		}

		float DistanceFromPlaneASE (float3 pos, float4 plane)
		{
			return dot (float4(pos,1.0f), plane);
		}

		bool WorldViewFrustumCull (float3 wpos0, float3 wpos1, float3 wpos2, float cullEps, float4 planes[6] )
		{
			float4 planeTest;
			planeTest.x = (( DistanceFromPlaneASE(wpos0, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlaneASE(wpos1, planes[0]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlaneASE(wpos2, planes[0]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.y = (( DistanceFromPlaneASE(wpos0, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlaneASE(wpos1, planes[1]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlaneASE(wpos2, planes[1]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.z = (( DistanceFromPlaneASE(wpos0, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlaneASE(wpos1, planes[2]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlaneASE(wpos2, planes[2]) > -cullEps) ? 1.0f : 0.0f );
			planeTest.w = (( DistanceFromPlaneASE(wpos0, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlaneASE(wpos1, planes[3]) > -cullEps) ? 1.0f : 0.0f ) +
						  (( DistanceFromPlaneASE(wpos2, planes[3]) > -cullEps) ? 1.0f : 0.0f );
			return !all (planeTest);
		}

		float4 DistanceBasedTess( float4 v0, float4 v1, float4 v2, float tess, float minDist, float maxDist, float4x4 o2w, float3 cameraPos )
		{
			float3 f;
			f.x = CalcDistanceTessFactor (v0,minDist,maxDist,tess,o2w,cameraPos);
			f.y = CalcDistanceTessFactor (v1,minDist,maxDist,tess,o2w,cameraPos);
			f.z = CalcDistanceTessFactor (v2,minDist,maxDist,tess,o2w,cameraPos);

			return CalcTriEdgeTessFactors (f);
		}

		float4 EdgeLengthBasedTess( float4 v0, float4 v1, float4 v2, float edgeLength, float4x4 o2w, float3 cameraPos, float4 scParams )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;
			tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
			tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
			tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
			tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			return tess;
		}

		float4 EdgeLengthBasedTessCull( float4 v0, float4 v1, float4 v2, float edgeLength, float maxDisplacement, float4x4 o2w, float3 cameraPos, float4 scParams, float4 planes[6] )
		{
			float3 pos0 = mul(o2w,v0).xyz;
			float3 pos1 = mul(o2w,v1).xyz;
			float3 pos2 = mul(o2w,v2).xyz;
			float4 tess;

			if (WorldViewFrustumCull(pos0, pos1, pos2, maxDisplacement, planes))
			{
				tess = 0.0f;
			}
			else
			{
				tess.x = CalcEdgeTessFactor (pos1, pos2, edgeLength, cameraPos, scParams);
				tess.y = CalcEdgeTessFactor (pos2, pos0, edgeLength, cameraPos, scParams);
				tess.z = CalcEdgeTessFactor (pos0, pos1, edgeLength, cameraPos, scParams);
				tess.w = (tess.x + tess.y + tess.z) / 3.0f;
			}
			return tess;
		}
		#endif //ASE_TESS_FUNCS
		ENDHLSL
		
		Pass
		{
			
			Name "GBuffer"
			Tags { "LightMode"="GBuffer" }
			Cull [_CullMode]
			ZTest [_ZTestGBuffer]

			Stencil
			{
				Ref [_StencilRefGBuffer]
				WriteMask [_StencilWriteMaskGBuffer]
				Comp Always
				Pass Replace
				Fail Keep
				ZFail Keep
			}


			HLSLPROGRAM

			#define _SPECULAR_OCCLUSION_FROM_AO 1
			#define _DISABLE_DECALS 1
			#define _DISABLE_SSR 1
			#define _AMBIENT_OCCLUSION 1
			#define ASE_SRP_VERSION 999999


			#pragma shader_feature _SURFACE_TYPE_TRANSPARENT
			#pragma shader_feature_local _DOUBLESIDED_ON
			#pragma shader_feature_local _BLENDMODE_OFF _BLENDMODE_ALPHA _BLENDMODE_ADD _BLENDMODE_PRE_MULTIPLY
			#pragma shader_feature_local_fragment _ _ENABLE_FOG_ON_TRANSPARENT
			#pragma shader_feature_local _ALPHATEST_ON
			#pragma shader_feature_local _ _TRANSPARENT_WRITES_MOTION_VEC

			#define SHADERPASS SHADERPASS_GBUFFER
			#pragma multi_compile _ DEBUG_DISPLAY
			#pragma multi_compile _ LIGHTMAP_ON
			#pragma multi_compile _ DIRLIGHTMAP_COMBINED
			#pragma multi_compile _ DYNAMICLIGHTMAP_ON
			#pragma multi_compile_fragment _ SHADOWS_SHADOWMASK
			#pragma multi_compile_fragment DECALS_OFF DECALS_3RT DECALS_4RT
			#pragma multi_compile_fragment _ LIGHT_LAYERS
			#pragma multi_compile_fragment PROBE_VOLUMES_OFF PROBE_VOLUMES_L1 PROBE_VOLUMES_L2
			#pragma multi_compile_fragment _ DECAL_SURFACE_GRADIENT

			#if !defined(DEBUG_DISPLAY) && defined(_ALPHATEST_ON)
			#define SHADERPASS_GBUFFER_BYPASS_ALPHA_TEST
			#endif

			#pragma vertex Vert
			#pragma fragment Frag
			
			//#define UNITY_MATERIAL_LIT

			#if defined(_MATERIAL_FEATURE_SUBSURFACE_SCATTERING) && !defined(_SURFACE_TYPE_TRANSPARENT)
			#define OUTPUT_SPLIT_LIGHTING
			#endif

		    #if defined(SHADER_LIT) && !defined(_SURFACE_TYPE_TRANSPARENT)
                #define _DEFERRED_CAPABLE_MATERIAL
            #endif
            
            #if defined(_TRANSPARENT_WRITES_MOTION_VEC) && defined(_SURFACE_TYPE_TRANSPARENT)
                #define _WRITE_TRANSPARENT_MOTION_VECTOR
            #endif
        

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
        	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/GeometricTools.hlsl" 
        	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Tessellation.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/FragInputs.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl" 
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderGraphHeader.hlsl" 
			#ifdef DEBUG_DISPLAY
				#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Debug/DebugDisplay.hlsl"
			#endif

			CBUFFER_START( UnityPerMaterial )
			float4 _MatTexArray_ST;
			float4 _WorldBounds;
			float4 _Control_TexelSize;
			float4 _ShapeMap_ST;
			float _NormalScale;
			float _MetalLevel;
			float _Rough;
			float4 _EmissionColor;
			float _AlphaCutoff;
			float _RenderQueueType;
			#ifdef _ADD_PRECOMPUTED_VELOCITY
			float _AddPrecomputedVelocity;
			#endif
			float _StencilRef;
			float _StencilWriteMask;
			float _StencilRefDepth;
			float _StencilWriteMaskDepth;
			float _StencilRefMV;
			float _StencilWriteMaskMV;
			float _StencilRefDistortionVec;
			float _StencilWriteMaskDistortionVec;
			float _StencilWriteMaskGBuffer;
			float _StencilRefGBuffer;
			float _ZTestGBuffer;
			float _RequireSplitLighting;
			float _ReceivesSSR;
			float _SurfaceType;
			float _BlendMode;
			float _SrcBlend;
			float _DstBlend;
			float _AlphaSrcBlend;
			float _AlphaDstBlend;
			float _ZWrite;
			float _TransparentZWrite;
			float _CullMode;
			float _TransparentSortPriority;
			float _EnableFogOnTransparent;
			float _CullModeForward;
			float _TransparentCullMode;
			float _ZTestDepthEqualForOpaque;
			float _ZTestTransparent;
			float _TransparentBackfaceEnable;
			float _AlphaCutoffEnable;
			float _UseShadowThreshold;
			float _DoubleSidedEnable;
			float _DoubleSidedNormalMode;
			float4 _DoubleSidedConstants;
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			TEXTURE2D_ARRAY(_MatTexArray);
			sampler2D _Control;
			SAMPLER(sampler_MatTexArray);
			sampler2D _GrassControl;
			TEXTURE2D_ARRAY(_ShapeMap);
			SAMPLER(sampler_ShapeMap);
			sampler2D _GrassTint;
			sampler2D _Tint;
			sampler2D _SpatterTex;


			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/NormalSurfaceGradient.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Lit/Lit.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/BuiltinUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/MaterialUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Decal/DecalUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Lit/LitDecalData.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderGraphFunctions.hlsl"

			#define ASE_NEEDS_FRAG_RELATIVE_WORLD_POS
			#define GRASS


			#if defined(_DOUBLESIDED_ON) && !defined(ASE_NEED_CULLFACE)
				#define ASE_NEED_CULLFACE 1
			#endif

			struct AttributesMesh
			{
				float3 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 tangentOS : TANGENT;
				float4 uv1 : TEXCOORD1;
				float4 uv2 : TEXCOORD2;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryingsMeshToPS
			{
				float4 positionCS : SV_Position;
				float3 positionRWS : TEXCOORD0;
				float3 normalWS : TEXCOORD1;
				float4 tangentWS : TEXCOORD2;
				float4 uv1 : TEXCOORD3;
				float4 uv2 : TEXCOORD4;
				float4 ase_texcoord5 : TEXCOORD5;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
				#if defined(SHADER_STAGE_FRAGMENT) && defined(ASE_NEED_CULLFACE)
				FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
				#endif
			};


			float3 mod3D289( float3 x ) { return x - floor( x / 289.0 ) * 289.0; }
			float4 mod3D289( float4 x ) { return x - floor( x / 289.0 ) * 289.0; }
			float4 permute( float4 x ) { return mod3D289( ( x * 34.0 + 1.0 ) * x ); }
			float4 taylorInvSqrt( float4 r ) { return 1.79284291400159 - r * 0.85373472095314; }
			float snoise( float3 v )
			{
				const float2 C = float2( 1.0 / 6.0, 1.0 / 3.0 );
				float3 i = floor( v + dot( v, C.yyy ) );
				float3 x0 = v - i + dot( i, C.xxx );
				float3 g = step( x0.yzx, x0.xyz );
				float3 l = 1.0 - g;
				float3 i1 = min( g.xyz, l.zxy );
				float3 i2 = max( g.xyz, l.zxy );
				float3 x1 = x0 - i1 + C.xxx;
				float3 x2 = x0 - i2 + C.yyy;
				float3 x3 = x0 - 0.5;
				i = mod3D289( i);
				float4 p = permute( permute( permute( i.z + float4( 0.0, i1.z, i2.z, 1.0 ) ) + i.y + float4( 0.0, i1.y, i2.y, 1.0 ) ) + i.x + float4( 0.0, i1.x, i2.x, 1.0 ) );
				float4 j = p - 49.0 * floor( p / 49.0 );  // mod(p,7*7)
				float4 x_ = floor( j / 7.0 );
				float4 y_ = floor( j - 7.0 * x_ );  // mod(j,N)
				float4 x = ( x_ * 2.0 + 0.5 ) / 7.0 - 1.0;
				float4 y = ( y_ * 2.0 + 0.5 ) / 7.0 - 1.0;
				float4 h = 1.0 - abs( x ) - abs( y );
				float4 b0 = float4( x.xy, y.xy );
				float4 b1 = float4( x.zw, y.zw );
				float4 s0 = floor( b0 ) * 2.0 + 1.0;
				float4 s1 = floor( b1 ) * 2.0 + 1.0;
				float4 sh = -step( h, 0.0 );
				float4 a0 = b0.xzyw + s0.xzyw * sh.xxyy;
				float4 a1 = b1.xzyw + s1.xzyw * sh.zzww;
				float3 g0 = float3( a0.xy, h.x );
				float3 g1 = float3( a0.zw, h.y );
				float3 g2 = float3( a1.xy, h.z );
				float3 g3 = float3( a1.zw, h.w );
				float4 norm = taylorInvSqrt( float4( dot( g0, g0 ), dot( g1, g1 ), dot( g2, g2 ), dot( g3, g3 ) ) );
				g0 *= norm.x;
				g1 *= norm.y;
				g2 *= norm.z;
				g3 *= norm.w;
				float4 m = max( 0.6 - float4( dot( x0, x0 ), dot( x1, x1 ), dot( x2, x2 ), dot( x3, x3 ) ), 0.0 );
				m = m* m;
				m = m* m;
				float4 px = float4( dot( x0, g0 ), dot( x1, g1 ), dot( x2, g2 ), dot( x3, g3 ) );
				return 42.0 * dot( m, px);
			}
			

			void BuildSurfaceData(FragInputs fragInputs, inout GlobalSurfaceDescription surfaceDescription, float3 V, PositionInputs posInput, out SurfaceData surfaceData, out float3 bentNormalWS)
			{
				ZERO_INITIALIZE(SurfaceData, surfaceData);

				surfaceData.specularOcclusion = 1.0;

				// surface data
				surfaceData.baseColor =					surfaceDescription.Albedo;
				surfaceData.perceptualSmoothness =		surfaceDescription.Smoothness;
				surfaceData.ambientOcclusion =			surfaceDescription.Occlusion;
				surfaceData.metallic =					surfaceDescription.Metallic;
				surfaceData.coatMask =					surfaceDescription.CoatMask;

				#ifdef _SPECULAR_OCCLUSION_CUSTOM
				surfaceData.specularOcclusion =			surfaceDescription.SpecularOcclusion;
				#endif
				#ifdef _MATERIAL_FEATURE_SUBSURFACE_SCATTERING
				surfaceData.subsurfaceMask =			surfaceDescription.SubsurfaceMask;
				#endif
				#if defined(_HAS_REFRACTION) || defined(_MATERIAL_FEATURE_TRANSMISSION)
				surfaceData.thickness =					surfaceDescription.Thickness;
				#endif
				#if defined( _MATERIAL_FEATURE_SUBSURFACE_SCATTERING ) || defined( _MATERIAL_FEATURE_TRANSMISSION )
				surfaceData.diffusionProfileHash =		asuint(surfaceDescription.DiffusionProfile);
				#endif
				#ifdef _MATERIAL_FEATURE_SPECULAR_COLOR
				surfaceData.specularColor =				surfaceDescription.Specular;
				#endif
				#ifdef _MATERIAL_FEATURE_ANISOTROPY
				surfaceData.anisotropy =				surfaceDescription.Anisotropy;
				#endif
				#ifdef _MATERIAL_FEATURE_IRIDESCENCE
				surfaceData.iridescenceMask =			surfaceDescription.IridescenceMask;
				surfaceData.iridescenceThickness =		surfaceDescription.IridescenceThickness;
				#endif

				// refraction
				#ifdef _HAS_REFRACTION
				if( _EnableSSRefraction )
				{
					surfaceData.ior = surfaceDescription.RefractionIndex;
					surfaceData.transmittanceColor = surfaceDescription.RefractionColor;
					surfaceData.atDistance = surfaceDescription.RefractionDistance;

					surfaceData.transmittanceMask = ( 1.0 - surfaceDescription.Alpha );
					surfaceDescription.Alpha = 1.0;
				}
				else
				{
					surfaceData.ior = 1.0;
					surfaceData.transmittanceColor = float3( 1.0, 1.0, 1.0 );
					surfaceData.atDistance = 1.0;
					surfaceData.transmittanceMask = 0.0;
					surfaceDescription.Alpha = 1.0;
				}
				#else
				surfaceData.ior = 1.0;
				surfaceData.transmittanceColor = float3( 1.0, 1.0, 1.0 );
				surfaceData.atDistance = 1.0;
				surfaceData.transmittanceMask = 0.0;
				#endif


				// material features
				surfaceData.materialFeatures = MATERIALFEATUREFLAGS_LIT_STANDARD;
				#ifdef _MATERIAL_FEATURE_SUBSURFACE_SCATTERING
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_SUBSURFACE_SCATTERING;
				#endif
				#ifdef _MATERIAL_FEATURE_TRANSMISSION
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_TRANSMISSION;
				#endif
				#ifdef _MATERIAL_FEATURE_ANISOTROPY
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_ANISOTROPY;
				#endif
				#ifdef ASE_LIT_CLEAR_COAT
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_CLEAR_COAT;
				#endif
				#ifdef _MATERIAL_FEATURE_IRIDESCENCE
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_IRIDESCENCE;
				#endif
				#ifdef _MATERIAL_FEATURE_SPECULAR_COLOR
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_SPECULAR_COLOR;
				#endif

				// others
				#if defined (_MATERIAL_FEATURE_SPECULAR_COLOR) && defined (_ENERGY_CONSERVING_SPECULAR)
				surfaceData.baseColor *= ( 1.0 - Max3( surfaceData.specularColor.r, surfaceData.specularColor.g, surfaceData.specularColor.b ) );
				#endif
				#ifdef _DOUBLESIDED_ON
				float3 doubleSidedConstants = _DoubleSidedConstants.xyz;
				#else
				float3 doubleSidedConstants = float3( 1.0, 1.0, 1.0 );
				#endif

				// normals
				float3 normalTS = float3(0.0f, 0.0f, 1.0f);
				normalTS = surfaceDescription.Normal;
				GetNormalWS( fragInputs, normalTS, surfaceData.normalWS, doubleSidedConstants );

				surfaceData.geomNormalWS = fragInputs.tangentToWorld[2];
				surfaceData.tangentWS = normalize( fragInputs.tangentToWorld[ 0 ].xyz );

				// decals
				#if HAVE_DECALS
				if( _EnableDecals )
				{
					DecalSurfaceData decalSurfaceData = GetDecalSurfaceData(posInput, fragInputs.tangentToWorld[2], surfaceDescription.Alpha);
					ApplyDecalToSurfaceData(decalSurfaceData, fragInputs.tangentToWorld[2], surfaceData);
				}
				#endif

				bentNormalWS = surfaceData.normalWS;

				#ifdef ASE_BENT_NORMAL
				GetNormalWS( fragInputs, surfaceDescription.BentNormal, bentNormalWS, doubleSidedConstants );
				#endif

				#ifdef _MATERIAL_FEATURE_ANISOTROPY
				surfaceData.tangentWS = TransformTangentToWorld( surfaceDescription.Tangent, fragInputs.tangentToWorld );
				#endif
				surfaceData.tangentWS = Orthonormalize( surfaceData.tangentWS, surfaceData.normalWS );


				#if defined(_SPECULAR_OCCLUSION_CUSTOM)
				#elif defined(_SPECULAR_OCCLUSION_FROM_AO_BENT_NORMAL)
				surfaceData.specularOcclusion = GetSpecularOcclusionFromBentAO( V, bentNormalWS, surfaceData.normalWS, surfaceData.ambientOcclusion, PerceptualSmoothnessToPerceptualRoughness( surfaceData.perceptualSmoothness ) );
				#elif defined(_AMBIENT_OCCLUSION) && defined(_SPECULAR_OCCLUSION_FROM_AO)
				surfaceData.specularOcclusion = GetSpecularOcclusionFromAmbientOcclusion( ClampNdotV( dot( surfaceData.normalWS, V ) ), surfaceData.ambientOcclusion, PerceptualSmoothnessToRoughness( surfaceData.perceptualSmoothness ) );
				#endif

				#ifdef _ENABLE_GEOMETRIC_SPECULAR_AA
				surfaceData.perceptualSmoothness = GeometricNormalFiltering( surfaceData.perceptualSmoothness, fragInputs.tangentToWorld[ 2 ], surfaceDescription.SpecularAAScreenSpaceVariance, surfaceDescription.SpecularAAThreshold );
				#endif

				// debug
				#if defined(DEBUG_DISPLAY)
				if (_DebugMipMapMode != DEBUGMIPMAPMODE_NONE)
				{
					surfaceData.metallic = 0;
				}
				ApplyDebugToSurfaceData(fragInputs.tangentToWorld, surfaceData);
				#endif
			}

			void GetSurfaceAndBuiltinData(GlobalSurfaceDescription surfaceDescription, FragInputs fragInputs, float3 V, inout PositionInputs posInput, out SurfaceData surfaceData, out BuiltinData builtinData)
			{
				#ifdef LOD_FADE_CROSSFADE
				LODDitheringTransition(ComputeFadeMaskSeed(V, posInput.positionSS), unity_LODFade.x);
				#endif

				#ifdef _DOUBLESIDED_ON
				float3 doubleSidedConstants = _DoubleSidedConstants.xyz;
				#else
				float3 doubleSidedConstants = float3( 1.0, 1.0, 1.0 );
				#endif

				ApplyDoubleSidedFlipOrMirror( fragInputs, doubleSidedConstants );

				#ifdef _ALPHATEST_ON
				DoAlphaTest( surfaceDescription.Alpha, surfaceDescription.AlphaClipThreshold );
				#endif

				#ifdef _DEPTHOFFSET_ON
				ApplyDepthOffsetPositionInput( V, surfaceDescription.DepthOffset, GetViewForwardDir(), GetWorldToHClipMatrix(), posInput );
				#endif


				float3 bentNormalWS;
				BuildSurfaceData( fragInputs, surfaceDescription, V, posInput, surfaceData, bentNormalWS );

				InitBuiltinData( posInput, surfaceDescription.Alpha, bentNormalWS, -fragInputs.tangentToWorld[ 2 ], fragInputs.texCoord1, fragInputs.texCoord2, builtinData );

				#ifdef _DEPTHOFFSET_ON
				builtinData.depthOffset = surfaceDescription.DepthOffset;
				#endif
				
				#ifdef _ALPHATEST_ON 
				builtinData.alphaClipTreshold = surfaceDescription.AlphaClipThreshold;
                #endif

				#ifdef UNITY_VIRTUAL_TEXTURING
                builtinData.vtPackedFeedback = surfaceDescription.VTPackedFeedback;
                #endif

				#ifdef _ASE_BAKEDGI
				builtinData.bakeDiffuseLighting = surfaceDescription.BakedGI;
				#endif
				#ifdef _ASE_BAKEDBACKGI
				builtinData.backBakeDiffuseLighting = surfaceDescription.BakedBackGI;
				#endif

				builtinData.emissiveColor = surfaceDescription.Emission;

				builtinData.distortion = float2(0.0, 0.0);
				builtinData.distortionBlur = 0.0;

				PostInitBuiltinData(V, posInput, surfaceData, builtinData);
			}

			PackedVaryingsMeshToPS VertexFunction(AttributesMesh inputMesh )
			{
				PackedVaryingsMeshToPS outputPackedVaryingsMeshToPS;

				UNITY_SETUP_INSTANCE_ID(inputMesh);
				UNITY_TRANSFER_INSTANCE_ID(inputMesh, outputPackedVaryingsMeshToPS);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( outputPackedVaryingsMeshToPS );

				outputPackedVaryingsMeshToPS.ase_texcoord5.xy = inputMesh.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				outputPackedVaryingsMeshToPS.ase_texcoord5.zw = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
				float3 defaultVertexValue = inputMesh.positionOS.xyz;
				#else
				float3 defaultVertexValue = float3( 0, 0, 0 );
				#endif
				float3 vertexValue =  defaultVertexValue ;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
				inputMesh.positionOS.xyz = vertexValue;
				#else
				inputMesh.positionOS.xyz += vertexValue;
				#endif

				inputMesh.normalOS =  inputMesh.normalOS ;
				inputMesh.tangentOS =  inputMesh.tangentOS ;

				float3 positionRWS = TransformObjectToWorld(inputMesh.positionOS);
				float3 normalWS = TransformObjectToWorldNormal(inputMesh.normalOS);
				float4 tangentWS = float4(TransformObjectToWorldDir(inputMesh.tangentOS.xyz), inputMesh.tangentOS.w);

				outputPackedVaryingsMeshToPS.positionCS = TransformWorldToHClip(positionRWS);
				outputPackedVaryingsMeshToPS.positionRWS.xyz = positionRWS;
				outputPackedVaryingsMeshToPS.normalWS.xyz = normalWS;
				outputPackedVaryingsMeshToPS.tangentWS.xyzw = tangentWS;
				outputPackedVaryingsMeshToPS.uv1.xyzw = inputMesh.uv1;
				outputPackedVaryingsMeshToPS.uv2.xyzw = inputMesh.uv2;
				return outputPackedVaryingsMeshToPS;
			}

			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float3 positionOS : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 tangentOS : TANGENT;
				float4 uv1 : TEXCOORD1;
				float4 uv2 : TEXCOORD2;
				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl Vert ( AttributesMesh v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.positionOS = v.positionOS;
				o.normalOS = v.normalOS;
				o.tangentOS = v.tangentOS;
				o.uv1 = v.uv1;
				o.uv2 = v.uv2;
				o.ase_texcoord = v.ase_texcoord;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if (SHADEROPTIONS_CAMERA_RELATIVE_RENDERING != 0)
				float3 cameraPos = 0;
				#else
				float3 cameraPos = _WorldSpaceCameraPos;
				#endif
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(float4(v[0].positionOS,1), float4(v[1].positionOS,1), float4(v[2].positionOS,1), tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), cameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(float4(v[0].positionOS,1), float4(v[1].positionOS,1), float4(v[2].positionOS,1), edgeLength, GetObjectToWorldMatrix(), cameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(float4(v[0].positionOS,1), float4(v[1].positionOS,1), float4(v[2].positionOS,1), edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), cameraPos, _ScreenParams, _FrustumPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			PackedVaryingsMeshToPS DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				AttributesMesh o = (AttributesMesh) 0;
				o.positionOS = patch[0].positionOS * bary.x + patch[1].positionOS * bary.y + patch[2].positionOS * bary.z;
				o.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				o.tangentOS = patch[0].tangentOS * bary.x + patch[1].tangentOS * bary.y + patch[2].tangentOS * bary.z;
				o.uv1 = patch[0].uv1 * bary.x + patch[1].uv1 * bary.y + patch[2].uv1 * bary.z;
				o.uv2 = patch[0].uv2 * bary.x + patch[1].uv2 * bary.y + patch[2].uv2 * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.positionOS.xyz - patch[i].normalOS * (dot(o.positionOS.xyz, patch[i].normalOS) - dot(patch[i].positionOS.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				o.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			PackedVaryingsMeshToPS Vert ( AttributesMesh v )
			{
				return VertexFunction( v );
			}
			#endif

			void Frag( PackedVaryingsMeshToPS packedInput,
						OUTPUT_GBUFFER(outGBuffer)
						#ifdef _DEPTHOFFSET_ON
						, out float outputDepth : SV_Depth
						#endif
						
						)
			{
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( packedInput );
				UNITY_SETUP_INSTANCE_ID( packedInput );
				FragInputs input;
				ZERO_INITIALIZE(FragInputs, input);
				input.tangentToWorld = k_identity3x3;
				float3 positionRWS = packedInput.positionRWS.xyz;
				float3 normalWS = packedInput.normalWS.xyz;
				float4 tangentWS = packedInput.tangentWS.xyzw;

				input.positionSS = packedInput.positionCS;
				input.positionRWS = positionRWS;
				input.tangentToWorld = BuildTangentToWorld(tangentWS, normalWS);
				input.texCoord1 = packedInput.uv1.xyzw;
				input.texCoord2 = packedInput.uv2.xyzw;

				#if _DOUBLESIDED_ON && SHADER_STAGE_FRAGMENT
				input.isFrontFace = IS_FRONT_VFACE( packedInput.cullFace, true, false);
				#elif SHADER_STAGE_FRAGMENT
				#if defined(ASE_NEED_CULLFACE)
				input.isFrontFace = IS_FRONT_VFACE( packedInput.cullFace, true, false );
				#endif
				#endif
				half isFrontFace = input.isFrontFace;

				PositionInputs posInput = GetPositionInput(input.positionSS.xy, _ScreenSize.zw, input.positionSS.z, input.positionSS.w, input.positionRWS);
				float3 V = GetWorldSpaceNormalizeViewDir(input.positionRWS);
				SurfaceData surfaceData;
				BuiltinData builtinData;

				GlobalSurfaceDescription surfaceDescription = (GlobalSurfaceDescription)0;
				float2 uv_MatTexArray = packedInput.ase_texcoord5.xy;
				float3 ase_worldPos = GetAbsolutePositionWS( positionRWS );
				float2 appendResult257 = (float2(ase_worldPos.x , ase_worldPos.z));
				float2 appendResult260 = (float2(_WorldBounds.x , _WorldBounds.y));
				float2 appendResult259 = (float2(_WorldBounds.z , _WorldBounds.w));
				float2 temp_output_264_0 = ( ( appendResult257 - appendResult260 ) / ( appendResult259 - appendResult260 ) );
				float2 appendResult265 = (float2(_Control_TexelSize.z , _Control_TexelSize.w));
				float2 temp_output_266_0 = ( temp_output_264_0 * appendResult265 );
				float2 temp_output_270_0 = floor( ( ( temp_output_266_0 + float2( 0.5,0.5 ) ) - float2( 0.5,0.5 ) ) );
				float2 temp_output_12_0_g423 = ( ( temp_output_270_0 + float2( 0,0 ) ) / appendResult265 );
				float4 tex2DNode6_g423 = tex2D( _Control, temp_output_12_0_g423 );
				float4 tex2DArrayNode23_g423 = SAMPLE_TEXTURE2D_ARRAY( _MatTexArray, sampler_MatTexArray, uv_MatTexArray,( tex2DNode6_g423.r * 255.0 ) );
				float4 tex2DNode15_g423 = tex2D( _GrassControl, temp_output_12_0_g423 );
				float2 uv_ShapeMap = packedInput.ase_texcoord5.xy;
				float4 tex2DArrayNode35_g423 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode15_g423.g * 255.0 ) );
				float temp_output_24_0_g425 = tex2DArrayNode35_g423.b;
				float4 tex2DNode16_g423 = tex2D( _GrassTint, temp_output_12_0_g423 );
				float temp_output_25_0_g425 = tex2DNode16_g423.a;
				float4 tex2DArrayNode32_g423 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode6_g423.g * 255.0 ) );
				float temp_output_26_0_g425 = tex2DArrayNode32_g423.b;
				float temp_output_41_0_g423 = ( 1.0 - tex2DNode16_g423.a );
				float temp_output_27_0_g425 = temp_output_41_0_g423;
				float temp_output_1_0_g425 = ( max( ( temp_output_24_0_g425 + temp_output_25_0_g425 ) , ( temp_output_26_0_g425 + temp_output_27_0_g425 ) ) - 0.2 );
				float temp_output_21_0_g425 = max( ( ( temp_output_24_0_g425 + temp_output_25_0_g425 ) - temp_output_1_0_g425 ) , 0.0 );
				float temp_output_19_0_g425 = max( ( ( temp_output_26_0_g425 + temp_output_27_0_g425 ) - temp_output_1_0_g425 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch29_g423 = ( ( ( SAMPLE_TEXTURE2D_ARRAY( _MatTexArray, sampler_MatTexArray, uv_MatTexArray,( tex2DNode15_g423.r * 255.0 ) ) * temp_output_21_0_g425 ) + ( tex2DArrayNode23_g423 * temp_output_19_0_g425 ) ) / ( temp_output_21_0_g425 + temp_output_19_0_g425 ) );
				#else
				float4 staticSwitch29_g423 = tex2DArrayNode23_g423;
				#endif
				float temp_output_24_0_g424 = tex2DArrayNode35_g423.b;
				float temp_output_25_0_g424 = tex2DNode16_g423.a;
				float temp_output_26_0_g424 = tex2DArrayNode32_g423.b;
				float temp_output_27_0_g424 = temp_output_41_0_g423;
				float temp_output_1_0_g424 = ( max( ( temp_output_24_0_g424 + temp_output_25_0_g424 ) , ( temp_output_26_0_g424 + temp_output_27_0_g424 ) ) - 0.2 );
				float temp_output_21_0_g424 = max( ( ( temp_output_24_0_g424 + temp_output_25_0_g424 ) - temp_output_1_0_g424 ) , 0.0 );
				float temp_output_19_0_g424 = max( ( ( temp_output_26_0_g424 + temp_output_27_0_g424 ) - temp_output_1_0_g424 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch31_g423 = ( ( ( tex2DArrayNode35_g423 * temp_output_21_0_g424 ) + ( tex2DArrayNode32_g423 * temp_output_19_0_g424 ) ) / ( temp_output_21_0_g424 + temp_output_19_0_g424 ) );
				#else
				float4 staticSwitch31_g423 = tex2DArrayNode32_g423;
				#endif
				float temp_output_302_3 = staticSwitch31_g423.b;
				float temp_output_24_0_g443 = temp_output_302_3;
				float2 break273 = ( temp_output_266_0 - temp_output_270_0 );
				float temp_output_274_0 = ( 1.0 - break273.x );
				float temp_output_25_0_g443 = temp_output_274_0;
				float2 temp_output_12_0_g419 = ( ( temp_output_270_0 + float2( 1,0 ) ) / appendResult265 );
				float4 tex2DNode6_g419 = tex2D( _Control, temp_output_12_0_g419 );
				float4 tex2DArrayNode32_g419 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode6_g419.g * 255.0 ) );
				float4 tex2DNode15_g419 = tex2D( _GrassControl, temp_output_12_0_g419 );
				float4 tex2DArrayNode35_g419 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode15_g419.g * 255.0 ) );
				float temp_output_24_0_g420 = tex2DArrayNode35_g419.b;
				float4 tex2DNode16_g419 = tex2D( _GrassTint, temp_output_12_0_g419 );
				float temp_output_25_0_g420 = tex2DNode16_g419.a;
				float temp_output_26_0_g420 = tex2DArrayNode32_g419.b;
				float temp_output_41_0_g419 = ( 1.0 - tex2DNode16_g419.a );
				float temp_output_27_0_g420 = temp_output_41_0_g419;
				float temp_output_1_0_g420 = ( max( ( temp_output_24_0_g420 + temp_output_25_0_g420 ) , ( temp_output_26_0_g420 + temp_output_27_0_g420 ) ) - 0.2 );
				float temp_output_21_0_g420 = max( ( ( temp_output_24_0_g420 + temp_output_25_0_g420 ) - temp_output_1_0_g420 ) , 0.0 );
				float temp_output_19_0_g420 = max( ( ( temp_output_26_0_g420 + temp_output_27_0_g420 ) - temp_output_1_0_g420 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch31_g419 = ( ( ( tex2DArrayNode35_g419 * temp_output_21_0_g420 ) + ( tex2DArrayNode32_g419 * temp_output_19_0_g420 ) ) / ( temp_output_21_0_g420 + temp_output_19_0_g420 ) );
				#else
				float4 staticSwitch31_g419 = tex2DArrayNode32_g419;
				#endif
				float temp_output_299_3 = staticSwitch31_g419.b;
				float temp_output_26_0_g443 = temp_output_299_3;
				float temp_output_27_0_g443 = break273.x;
				float temp_output_1_0_g443 = ( max( ( temp_output_24_0_g443 + temp_output_25_0_g443 ) , ( temp_output_26_0_g443 + temp_output_27_0_g443 ) ) - 0.2 );
				float temp_output_21_0_g443 = max( ( ( temp_output_24_0_g443 + temp_output_25_0_g443 ) - temp_output_1_0_g443 ) , 0.0 );
				float4 tex2DArrayNode23_g419 = SAMPLE_TEXTURE2D_ARRAY( _MatTexArray, sampler_MatTexArray, uv_MatTexArray,( tex2DNode6_g419.r * 255.0 ) );
				float temp_output_24_0_g421 = tex2DArrayNode35_g419.b;
				float temp_output_25_0_g421 = tex2DNode16_g419.a;
				float temp_output_26_0_g421 = tex2DArrayNode32_g419.b;
				float temp_output_27_0_g421 = temp_output_41_0_g419;
				float temp_output_1_0_g421 = ( max( ( temp_output_24_0_g421 + temp_output_25_0_g421 ) , ( temp_output_26_0_g421 + temp_output_27_0_g421 ) ) - 0.2 );
				float temp_output_21_0_g421 = max( ( ( temp_output_24_0_g421 + temp_output_25_0_g421 ) - temp_output_1_0_g421 ) , 0.0 );
				float temp_output_19_0_g421 = max( ( ( temp_output_26_0_g421 + temp_output_27_0_g421 ) - temp_output_1_0_g421 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch29_g419 = ( ( ( SAMPLE_TEXTURE2D_ARRAY( _MatTexArray, sampler_MatTexArray, uv_MatTexArray,( tex2DNode15_g419.r * 255.0 ) ) * temp_output_21_0_g421 ) + ( tex2DArrayNode23_g419 * temp_output_19_0_g421 ) ) / ( temp_output_21_0_g421 + temp_output_19_0_g421 ) );
				#else
				float4 staticSwitch29_g419 = tex2DArrayNode23_g419;
				#endif
				float temp_output_19_0_g443 = max( ( ( temp_output_26_0_g443 + temp_output_27_0_g443 ) - temp_output_1_0_g443 ) , 0.0 );
				float temp_output_24_0_g436 = temp_output_302_3;
				float temp_output_25_0_g436 = temp_output_274_0;
				float temp_output_26_0_g436 = temp_output_299_3;
				float temp_output_27_0_g436 = break273.x;
				float temp_output_1_0_g436 = ( max( ( temp_output_24_0_g436 + temp_output_25_0_g436 ) , ( temp_output_26_0_g436 + temp_output_27_0_g436 ) ) - 0.2 );
				float temp_output_21_0_g436 = max( ( ( temp_output_24_0_g436 + temp_output_25_0_g436 ) - temp_output_1_0_g436 ) , 0.0 );
				float temp_output_19_0_g436 = max( ( ( temp_output_26_0_g436 + temp_output_27_0_g436 ) - temp_output_1_0_g436 ) , 0.0 );
				float4 temp_output_290_0 = ( ( ( staticSwitch31_g423 * temp_output_21_0_g436 ) + ( staticSwitch31_g419 * temp_output_19_0_g436 ) ) / ( temp_output_21_0_g436 + temp_output_19_0_g436 ) );
				float temp_output_24_0_g442 = temp_output_290_0.z;
				float temp_output_275_0 = ( 1.0 - break273.y );
				float temp_output_25_0_g442 = temp_output_275_0;
				float2 temp_output_12_0_g431 = ( ( temp_output_270_0 + float2( 0,1 ) ) / appendResult265 );
				float4 tex2DNode6_g431 = tex2D( _Control, temp_output_12_0_g431 );
				float4 tex2DArrayNode32_g431 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode6_g431.g * 255.0 ) );
				float4 tex2DNode15_g431 = tex2D( _GrassControl, temp_output_12_0_g431 );
				float4 tex2DArrayNode35_g431 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode15_g431.g * 255.0 ) );
				float temp_output_24_0_g432 = tex2DArrayNode35_g431.b;
				float4 tex2DNode16_g431 = tex2D( _GrassTint, temp_output_12_0_g431 );
				float temp_output_25_0_g432 = tex2DNode16_g431.a;
				float temp_output_26_0_g432 = tex2DArrayNode32_g431.b;
				float temp_output_41_0_g431 = ( 1.0 - tex2DNode16_g431.a );
				float temp_output_27_0_g432 = temp_output_41_0_g431;
				float temp_output_1_0_g432 = ( max( ( temp_output_24_0_g432 + temp_output_25_0_g432 ) , ( temp_output_26_0_g432 + temp_output_27_0_g432 ) ) - 0.2 );
				float temp_output_21_0_g432 = max( ( ( temp_output_24_0_g432 + temp_output_25_0_g432 ) - temp_output_1_0_g432 ) , 0.0 );
				float temp_output_19_0_g432 = max( ( ( temp_output_26_0_g432 + temp_output_27_0_g432 ) - temp_output_1_0_g432 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch31_g431 = ( ( ( tex2DArrayNode35_g431 * temp_output_21_0_g432 ) + ( tex2DArrayNode32_g431 * temp_output_19_0_g432 ) ) / ( temp_output_21_0_g432 + temp_output_19_0_g432 ) );
				#else
				float4 staticSwitch31_g431 = tex2DArrayNode32_g431;
				#endif
				float temp_output_300_3 = staticSwitch31_g431.b;
				float temp_output_24_0_g435 = temp_output_300_3;
				float temp_output_25_0_g435 = temp_output_274_0;
				float2 temp_output_12_0_g427 = ( ( temp_output_270_0 + float2( 1,1 ) ) / appendResult265 );
				float4 tex2DNode6_g427 = tex2D( _Control, temp_output_12_0_g427 );
				float4 tex2DArrayNode32_g427 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode6_g427.g * 255.0 ) );
				float4 tex2DNode15_g427 = tex2D( _GrassControl, temp_output_12_0_g427 );
				float4 tex2DArrayNode35_g427 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode15_g427.g * 255.0 ) );
				float temp_output_24_0_g428 = tex2DArrayNode35_g427.b;
				float4 tex2DNode16_g427 = tex2D( _GrassTint, temp_output_12_0_g427 );
				float temp_output_25_0_g428 = tex2DNode16_g427.a;
				float temp_output_26_0_g428 = tex2DArrayNode32_g427.b;
				float temp_output_41_0_g427 = ( 1.0 - tex2DNode16_g427.a );
				float temp_output_27_0_g428 = temp_output_41_0_g427;
				float temp_output_1_0_g428 = ( max( ( temp_output_24_0_g428 + temp_output_25_0_g428 ) , ( temp_output_26_0_g428 + temp_output_27_0_g428 ) ) - 0.2 );
				float temp_output_21_0_g428 = max( ( ( temp_output_24_0_g428 + temp_output_25_0_g428 ) - temp_output_1_0_g428 ) , 0.0 );
				float temp_output_19_0_g428 = max( ( ( temp_output_26_0_g428 + temp_output_27_0_g428 ) - temp_output_1_0_g428 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch31_g427 = ( ( ( tex2DArrayNode35_g427 * temp_output_21_0_g428 ) + ( tex2DArrayNode32_g427 * temp_output_19_0_g428 ) ) / ( temp_output_21_0_g428 + temp_output_19_0_g428 ) );
				#else
				float4 staticSwitch31_g427 = tex2DArrayNode32_g427;
				#endif
				float temp_output_301_3 = staticSwitch31_g427.b;
				float temp_output_26_0_g435 = temp_output_301_3;
				float temp_output_27_0_g435 = break273.x;
				float temp_output_1_0_g435 = ( max( ( temp_output_24_0_g435 + temp_output_25_0_g435 ) , ( temp_output_26_0_g435 + temp_output_27_0_g435 ) ) - 0.2 );
				float temp_output_21_0_g435 = max( ( ( temp_output_24_0_g435 + temp_output_25_0_g435 ) - temp_output_1_0_g435 ) , 0.0 );
				float temp_output_19_0_g435 = max( ( ( temp_output_26_0_g435 + temp_output_27_0_g435 ) - temp_output_1_0_g435 ) , 0.0 );
				float4 temp_output_293_0 = ( ( ( staticSwitch31_g431 * temp_output_21_0_g435 ) + ( staticSwitch31_g427 * temp_output_19_0_g435 ) ) / ( temp_output_21_0_g435 + temp_output_19_0_g435 ) );
				float temp_output_26_0_g442 = temp_output_293_0.z;
				float temp_output_27_0_g442 = break273.y;
				float temp_output_1_0_g442 = ( max( ( temp_output_24_0_g442 + temp_output_25_0_g442 ) , ( temp_output_26_0_g442 + temp_output_27_0_g442 ) ) - 0.2 );
				float temp_output_21_0_g442 = max( ( ( temp_output_24_0_g442 + temp_output_25_0_g442 ) - temp_output_1_0_g442 ) , 0.0 );
				float4 tex2DArrayNode23_g431 = SAMPLE_TEXTURE2D_ARRAY( _MatTexArray, sampler_MatTexArray, uv_MatTexArray,( tex2DNode6_g431.r * 255.0 ) );
				float temp_output_24_0_g433 = tex2DArrayNode35_g431.b;
				float temp_output_25_0_g433 = tex2DNode16_g431.a;
				float temp_output_26_0_g433 = tex2DArrayNode32_g431.b;
				float temp_output_27_0_g433 = temp_output_41_0_g431;
				float temp_output_1_0_g433 = ( max( ( temp_output_24_0_g433 + temp_output_25_0_g433 ) , ( temp_output_26_0_g433 + temp_output_27_0_g433 ) ) - 0.2 );
				float temp_output_21_0_g433 = max( ( ( temp_output_24_0_g433 + temp_output_25_0_g433 ) - temp_output_1_0_g433 ) , 0.0 );
				float temp_output_19_0_g433 = max( ( ( temp_output_26_0_g433 + temp_output_27_0_g433 ) - temp_output_1_0_g433 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch29_g431 = ( ( ( SAMPLE_TEXTURE2D_ARRAY( _MatTexArray, sampler_MatTexArray, uv_MatTexArray,( tex2DNode15_g431.r * 255.0 ) ) * temp_output_21_0_g433 ) + ( tex2DArrayNode23_g431 * temp_output_19_0_g433 ) ) / ( temp_output_21_0_g433 + temp_output_19_0_g433 ) );
				#else
				float4 staticSwitch29_g431 = tex2DArrayNode23_g431;
				#endif
				float temp_output_24_0_g488 = temp_output_300_3;
				float temp_output_25_0_g488 = temp_output_274_0;
				float temp_output_26_0_g488 = temp_output_301_3;
				float temp_output_27_0_g488 = break273.x;
				float temp_output_1_0_g488 = ( max( ( temp_output_24_0_g488 + temp_output_25_0_g488 ) , ( temp_output_26_0_g488 + temp_output_27_0_g488 ) ) - 0.2 );
				float temp_output_21_0_g488 = max( ( ( temp_output_24_0_g488 + temp_output_25_0_g488 ) - temp_output_1_0_g488 ) , 0.0 );
				float4 tex2DArrayNode23_g427 = SAMPLE_TEXTURE2D_ARRAY( _MatTexArray, sampler_MatTexArray, uv_MatTexArray,( tex2DNode6_g427.r * 255.0 ) );
				float temp_output_24_0_g429 = tex2DArrayNode35_g427.b;
				float temp_output_25_0_g429 = tex2DNode16_g427.a;
				float temp_output_26_0_g429 = tex2DArrayNode32_g427.b;
				float temp_output_27_0_g429 = temp_output_41_0_g427;
				float temp_output_1_0_g429 = ( max( ( temp_output_24_0_g429 + temp_output_25_0_g429 ) , ( temp_output_26_0_g429 + temp_output_27_0_g429 ) ) - 0.2 );
				float temp_output_21_0_g429 = max( ( ( temp_output_24_0_g429 + temp_output_25_0_g429 ) - temp_output_1_0_g429 ) , 0.0 );
				float temp_output_19_0_g429 = max( ( ( temp_output_26_0_g429 + temp_output_27_0_g429 ) - temp_output_1_0_g429 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch29_g427 = ( ( ( SAMPLE_TEXTURE2D_ARRAY( _MatTexArray, sampler_MatTexArray, uv_MatTexArray,( tex2DNode15_g427.r * 255.0 ) ) * temp_output_21_0_g429 ) + ( tex2DArrayNode23_g427 * temp_output_19_0_g429 ) ) / ( temp_output_21_0_g429 + temp_output_19_0_g429 ) );
				#else
				float4 staticSwitch29_g427 = tex2DArrayNode23_g427;
				#endif
				float temp_output_19_0_g488 = max( ( ( temp_output_26_0_g488 + temp_output_27_0_g488 ) - temp_output_1_0_g488 ) , 0.0 );
				float temp_output_19_0_g442 = max( ( ( temp_output_26_0_g442 + temp_output_27_0_g442 ) - temp_output_1_0_g442 ) , 0.0 );
				float4 temp_output_159_0 = ( ( ( ( ( ( staticSwitch29_g423 * temp_output_21_0_g443 ) + ( staticSwitch29_g419 * temp_output_19_0_g443 ) ) / ( temp_output_21_0_g443 + temp_output_19_0_g443 ) ) * temp_output_21_0_g442 ) + ( ( ( ( staticSwitch29_g431 * temp_output_21_0_g488 ) + ( staticSwitch29_g427 * temp_output_19_0_g488 ) ) / ( temp_output_21_0_g488 + temp_output_19_0_g488 ) ) * temp_output_19_0_g442 ) ) / ( temp_output_21_0_g442 + temp_output_19_0_g442 ) );
				float4 tex2DNode14_g423 = tex2D( _Tint, temp_output_12_0_g423 );
				float temp_output_24_0_g426 = tex2DArrayNode35_g423.b;
				float temp_output_25_0_g426 = tex2DNode16_g423.a;
				float temp_output_26_0_g426 = tex2DArrayNode32_g423.b;
				float temp_output_27_0_g426 = temp_output_41_0_g423;
				float temp_output_1_0_g426 = ( max( ( temp_output_24_0_g426 + temp_output_25_0_g426 ) , ( temp_output_26_0_g426 + temp_output_27_0_g426 ) ) - 0.2 );
				float temp_output_21_0_g426 = max( ( ( temp_output_24_0_g426 + temp_output_25_0_g426 ) - temp_output_1_0_g426 ) , 0.0 );
				float temp_output_19_0_g426 = max( ( ( temp_output_26_0_g426 + temp_output_27_0_g426 ) - temp_output_1_0_g426 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch30_g423 = ( ( ( tex2DNode16_g423 * temp_output_21_0_g426 ) + ( tex2DNode14_g423 * temp_output_19_0_g426 ) ) / ( temp_output_21_0_g426 + temp_output_19_0_g426 ) );
				#else
				float4 staticSwitch30_g423 = tex2DNode14_g423;
				#endif
				float temp_output_24_0_g437 = temp_output_302_3;
				float temp_output_25_0_g437 = temp_output_274_0;
				float temp_output_26_0_g437 = temp_output_299_3;
				float temp_output_27_0_g437 = break273.x;
				float temp_output_1_0_g437 = ( max( ( temp_output_24_0_g437 + temp_output_25_0_g437 ) , ( temp_output_26_0_g437 + temp_output_27_0_g437 ) ) - 0.2 );
				float temp_output_21_0_g437 = max( ( ( temp_output_24_0_g437 + temp_output_25_0_g437 ) - temp_output_1_0_g437 ) , 0.0 );
				float4 tex2DNode14_g419 = tex2D( _Tint, temp_output_12_0_g419 );
				float temp_output_24_0_g422 = tex2DArrayNode35_g419.b;
				float temp_output_25_0_g422 = tex2DNode16_g419.a;
				float temp_output_26_0_g422 = tex2DArrayNode32_g419.b;
				float temp_output_27_0_g422 = temp_output_41_0_g419;
				float temp_output_1_0_g422 = ( max( ( temp_output_24_0_g422 + temp_output_25_0_g422 ) , ( temp_output_26_0_g422 + temp_output_27_0_g422 ) ) - 0.2 );
				float temp_output_21_0_g422 = max( ( ( temp_output_24_0_g422 + temp_output_25_0_g422 ) - temp_output_1_0_g422 ) , 0.0 );
				float temp_output_19_0_g422 = max( ( ( temp_output_26_0_g422 + temp_output_27_0_g422 ) - temp_output_1_0_g422 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch30_g419 = ( ( ( tex2DNode16_g419 * temp_output_21_0_g422 ) + ( tex2DNode14_g419 * temp_output_19_0_g422 ) ) / ( temp_output_21_0_g422 + temp_output_19_0_g422 ) );
				#else
				float4 staticSwitch30_g419 = tex2DNode14_g419;
				#endif
				float temp_output_19_0_g437 = max( ( ( temp_output_26_0_g437 + temp_output_27_0_g437 ) - temp_output_1_0_g437 ) , 0.0 );
				float temp_output_24_0_g439 = temp_output_290_0.z;
				float temp_output_25_0_g439 = temp_output_275_0;
				float temp_output_26_0_g439 = temp_output_293_0.z;
				float temp_output_27_0_g439 = break273.y;
				float temp_output_1_0_g439 = ( max( ( temp_output_24_0_g439 + temp_output_25_0_g439 ) , ( temp_output_26_0_g439 + temp_output_27_0_g439 ) ) - 0.2 );
				float temp_output_21_0_g439 = max( ( ( temp_output_24_0_g439 + temp_output_25_0_g439 ) - temp_output_1_0_g439 ) , 0.0 );
				float4 tex2DNode14_g431 = tex2D( _Tint, temp_output_12_0_g431 );
				float temp_output_24_0_g434 = tex2DArrayNode35_g431.b;
				float temp_output_25_0_g434 = tex2DNode16_g431.a;
				float temp_output_26_0_g434 = tex2DArrayNode32_g431.b;
				float temp_output_27_0_g434 = temp_output_41_0_g431;
				float temp_output_1_0_g434 = ( max( ( temp_output_24_0_g434 + temp_output_25_0_g434 ) , ( temp_output_26_0_g434 + temp_output_27_0_g434 ) ) - 0.2 );
				float temp_output_21_0_g434 = max( ( ( temp_output_24_0_g434 + temp_output_25_0_g434 ) - temp_output_1_0_g434 ) , 0.0 );
				float temp_output_19_0_g434 = max( ( ( temp_output_26_0_g434 + temp_output_27_0_g434 ) - temp_output_1_0_g434 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch30_g431 = ( ( ( tex2DNode16_g431 * temp_output_21_0_g434 ) + ( tex2DNode14_g431 * temp_output_19_0_g434 ) ) / ( temp_output_21_0_g434 + temp_output_19_0_g434 ) );
				#else
				float4 staticSwitch30_g431 = tex2DNode14_g431;
				#endif
				float temp_output_24_0_g438 = temp_output_300_3;
				float temp_output_25_0_g438 = temp_output_274_0;
				float temp_output_26_0_g438 = temp_output_301_3;
				float temp_output_27_0_g438 = break273.x;
				float temp_output_1_0_g438 = ( max( ( temp_output_24_0_g438 + temp_output_25_0_g438 ) , ( temp_output_26_0_g438 + temp_output_27_0_g438 ) ) - 0.2 );
				float temp_output_21_0_g438 = max( ( ( temp_output_24_0_g438 + temp_output_25_0_g438 ) - temp_output_1_0_g438 ) , 0.0 );
				float4 tex2DNode14_g427 = tex2D( _Tint, temp_output_12_0_g427 );
				float temp_output_24_0_g430 = tex2DArrayNode35_g427.b;
				float temp_output_25_0_g430 = tex2DNode16_g427.a;
				float temp_output_26_0_g430 = tex2DArrayNode32_g427.b;
				float temp_output_27_0_g430 = temp_output_41_0_g427;
				float temp_output_1_0_g430 = ( max( ( temp_output_24_0_g430 + temp_output_25_0_g430 ) , ( temp_output_26_0_g430 + temp_output_27_0_g430 ) ) - 0.2 );
				float temp_output_21_0_g430 = max( ( ( temp_output_24_0_g430 + temp_output_25_0_g430 ) - temp_output_1_0_g430 ) , 0.0 );
				float temp_output_19_0_g430 = max( ( ( temp_output_26_0_g430 + temp_output_27_0_g430 ) - temp_output_1_0_g430 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch30_g427 = ( ( ( tex2DNode16_g427 * temp_output_21_0_g430 ) + ( tex2DNode14_g427 * temp_output_19_0_g430 ) ) / ( temp_output_21_0_g430 + temp_output_19_0_g430 ) );
				#else
				float4 staticSwitch30_g427 = tex2DNode14_g427;
				#endif
				float temp_output_19_0_g438 = max( ( ( temp_output_26_0_g438 + temp_output_27_0_g438 ) - temp_output_1_0_g438 ) , 0.0 );
				float temp_output_19_0_g439 = max( ( ( temp_output_26_0_g439 + temp_output_27_0_g439 ) - temp_output_1_0_g439 ) , 0.0 );
				float4 temp_output_221_0 = ( ( ( ( ( ( staticSwitch30_g423 * temp_output_21_0_g437 ) + ( staticSwitch30_g419 * temp_output_19_0_g437 ) ) / ( temp_output_21_0_g437 + temp_output_19_0_g437 ) ) * temp_output_21_0_g439 ) + ( ( ( ( staticSwitch30_g431 * temp_output_21_0_g438 ) + ( staticSwitch30_g427 * temp_output_19_0_g438 ) ) / ( temp_output_21_0_g438 + temp_output_19_0_g438 ) ) * temp_output_19_0_g439 ) ) / ( temp_output_21_0_g439 + temp_output_19_0_g439 ) );
				float2 appendResult12_g487 = (float2(ase_worldPos.x , ase_worldPos.z));
				float4 break7_g487 = _WorldBounds;
				float2 appendResult8_g487 = (float2(break7_g487.x , break7_g487.y));
				float2 appendResult9_g487 = (float2(break7_g487.z , break7_g487.w));
				float4 tex2DNode15_g487 = tex2D( _SpatterTex, ( ( appendResult12_g487 - appendResult8_g487 ) / ( appendResult9_g487 - temp_output_264_0 ) ) );
				float simplePerlin3D16_g487 = snoise( float3( appendResult12_g487 ,  0.0 )*2.0 );
				simplePerlin3D16_g487 = simplePerlin3D16_g487*0.5 + 0.5;
				float temp_output_17_0_g487 = ( tex2DNode15_g487.a - simplePerlin3D16_g487 );
				float HeightMask33_g487 = saturate(pow(max( (((simplePerlin3D16_g487*temp_output_17_0_g487)*4)+(temp_output_17_0_g487*2)), 0 ),1.0));
				float4 lerpResult34_g487 = lerp( ( temp_output_159_0 * temp_output_221_0 ) , tex2DNode15_g487 , HeightMask33_g487);
				
				float temp_output_24_0_g441 = temp_output_290_0.z;
				float temp_output_25_0_g441 = temp_output_275_0;
				float temp_output_26_0_g441 = temp_output_293_0.z;
				float temp_output_27_0_g441 = break273.y;
				float temp_output_1_0_g441 = ( max( ( temp_output_24_0_g441 + temp_output_25_0_g441 ) , ( temp_output_26_0_g441 + temp_output_27_0_g441 ) ) - 0.2 );
				float temp_output_21_0_g441 = max( ( ( temp_output_24_0_g441 + temp_output_25_0_g441 ) - temp_output_1_0_g441 ) , 0.0 );
				float temp_output_19_0_g441 = max( ( ( temp_output_26_0_g441 + temp_output_27_0_g441 ) - temp_output_1_0_g441 ) , 0.0 );
				float4 temp_output_180_0 = ( ( ( temp_output_290_0 * temp_output_21_0_g441 ) + ( temp_output_293_0 * temp_output_19_0_g441 ) ) / ( temp_output_21_0_g441 + temp_output_19_0_g441 ) );
				float3 unpack220 = UnpackNormalScale( temp_output_180_0, _NormalScale );
				unpack220.z = lerp( 1, unpack220.z, saturate(_NormalScale) );
				
				float temp_output_3_0_g440 = ( temp_output_221_0.w * 2.0 );
				
				surfaceDescription.Albedo = lerpResult34_g487.rgb;
				surfaceDescription.Normal = unpack220;
				surfaceDescription.BentNormal = float3( 0, 0, 1 );
				surfaceDescription.CoatMask = 0;
				surfaceDescription.Metallic = ( 0.0 * _MetalLevel );

				#ifdef _MATERIAL_FEATURE_SPECULAR_COLOR
				surfaceDescription.Specular = 0;
				#endif

				surfaceDescription.Emission = 0;
				surfaceDescription.Smoothness = ( temp_output_159_0.w * _Rough );
				surfaceDescription.Occlusion = temp_output_180_0.x;
				surfaceDescription.Alpha = min( temp_output_3_0_g440 , 1.0 );

				#ifdef _ALPHATEST_ON
				surfaceDescription.AlphaClipThreshold = _AlphaCutoff;
				#endif

				#ifdef _ALPHATEST_SHADOW_ON
				surfaceDescription.AlphaClipThresholdShadow = 0.5;
				#endif

				surfaceDescription.AlphaClipThresholdDepthPrepass = 0.5;
				surfaceDescription.AlphaClipThresholdDepthPostpass = 0.5;

				#ifdef _ENABLE_GEOMETRIC_SPECULAR_AA
				surfaceDescription.SpecularAAScreenSpaceVariance = 0;
				surfaceDescription.SpecularAAThreshold = 0;
				#endif

				#ifdef _SPECULAR_OCCLUSION_CUSTOM
				surfaceDescription.SpecularOcclusion = 0;
				#endif

				#if defined(_HAS_REFRACTION) || defined(_MATERIAL_FEATURE_TRANSMISSION)
				surfaceDescription.Thickness = 1;
				#endif

				#ifdef _HAS_REFRACTION
				surfaceDescription.RefractionIndex = 1;
				surfaceDescription.RefractionColor = float3( 1, 1, 1 );
				surfaceDescription.RefractionDistance = 0;
				#endif

				#ifdef _MATERIAL_FEATURE_SUBSURFACE_SCATTERING
				surfaceDescription.SubsurfaceMask = 1;
				#endif

				#if defined( _MATERIAL_FEATURE_SUBSURFACE_SCATTERING ) || defined( _MATERIAL_FEATURE_TRANSMISSION )
				surfaceDescription.DiffusionProfile = 0;
				#endif

				#ifdef _MATERIAL_FEATURE_ANISOTROPY
				surfaceDescription.Anisotropy = 1;
				surfaceDescription.Tangent = float3( 1, 0, 0 );
				#endif

				#ifdef _MATERIAL_FEATURE_IRIDESCENCE
				surfaceDescription.IridescenceMask = 0;
				surfaceDescription.IridescenceThickness = 0;
				#endif

				#ifdef _ASE_DISTORTION
				surfaceDescription.Distortion = float2 ( 2, -1 );
				surfaceDescription.DistortionBlur = 1;
				#endif

				#ifdef _ASE_BAKEDGI
				surfaceDescription.BakedGI = 0;
				#endif
				#ifdef _ASE_BAKEDBACKGI
				surfaceDescription.BakedBackGI = 0;
				#endif

				#ifdef _DEPTHOFFSET_ON
				surfaceDescription.DepthOffset = 0;
				#endif

				#ifdef UNITY_VIRTUAL_TEXTURING
				surfaceDescription.VTPackedFeedback = float4(1.0f,1.0f,1.0f,1.0f);
				#endif

				GetSurfaceAndBuiltinData( surfaceDescription, input, V, posInput, surfaceData, builtinData );
				ENCODE_INTO_GBUFFER( surfaceData, builtinData, posInput.positionSS, outGBuffer );
				#ifdef _DEPTHOFFSET_ON
				outputDepth = posInput.deviceDepth;
				#endif
			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "META"
			Tags { "LightMode"="Meta" }
			Cull Off

			HLSLPROGRAM

			#define _SPECULAR_OCCLUSION_FROM_AO 1
			#define _DISABLE_DECALS 1
			#define _DISABLE_SSR 1
			#define _AMBIENT_OCCLUSION 1
			#define ASE_SRP_VERSION 999999


			#pragma shader_feature _SURFACE_TYPE_TRANSPARENT
			#pragma shader_feature _ EDITOR_VISUALIZATION
			#pragma shader_feature_local _DOUBLESIDED_ON
			#pragma shader_feature_local _BLENDMODE_OFF _BLENDMODE_ALPHA _BLENDMODE_ADD _BLENDMODE_PRE_MULTIPLY
			#pragma shader_feature_local_fragment _ _ENABLE_FOG_ON_TRANSPARENT
			#pragma shader_feature_local _ALPHATEST_ON
			#pragma shader_feature_local _ _TRANSPARENT_WRITES_MOTION_VEC
			

			#define SHADERPASS SHADERPASS_LIGHT_TRANSPORT

			#pragma vertex Vert
			#pragma fragment Frag
			
			
			//#define UNITY_MATERIAL_LIT

			#if defined(_MATERIAL_FEATURE_SUBSURFACE_SCATTERING) && !defined(_SURFACE_TYPE_TRANSPARENT)
			#define OUTPUT_SPLIT_LIGHTING
			#endif

            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
        	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/GeometricTools.hlsl" 
        	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Tessellation.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/FragInputs.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl" 
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderGraphHeader.hlsl"

			#ifdef DEBUG_DISPLAY
				#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Debug/DebugDisplay.hlsl"
			#endif
			
			#if defined(SHADER_LIT) && !defined(_SURFACE_TYPE_TRANSPARENT)
                #define _DEFERRED_CAPABLE_MATERIAL
            #endif
            
            #if defined(_TRANSPARENT_WRITES_MOTION_VEC) && defined(_SURFACE_TYPE_TRANSPARENT)
                #define _WRITE_TRANSPARENT_MOTION_VECTOR
            #endif
        

			CBUFFER_START( UnityPerMaterial )
			float4 _MatTexArray_ST;
			float4 _WorldBounds;
			float4 _Control_TexelSize;
			float4 _ShapeMap_ST;
			float _NormalScale;
			float _MetalLevel;
			float _Rough;
			float4 _EmissionColor;
			float _AlphaCutoff;
			float _RenderQueueType;
			#ifdef _ADD_PRECOMPUTED_VELOCITY
			float _AddPrecomputedVelocity;
			#endif
			float _StencilRef;
			float _StencilWriteMask;
			float _StencilRefDepth;
			float _StencilWriteMaskDepth;
			float _StencilRefMV;
			float _StencilWriteMaskMV;
			float _StencilRefDistortionVec;
			float _StencilWriteMaskDistortionVec;
			float _StencilWriteMaskGBuffer;
			float _StencilRefGBuffer;
			float _ZTestGBuffer;
			float _RequireSplitLighting;
			float _ReceivesSSR;
			float _SurfaceType;
			float _BlendMode;
			float _SrcBlend;
			float _DstBlend;
			float _AlphaSrcBlend;
			float _AlphaDstBlend;
			float _ZWrite;
			float _TransparentZWrite;
			float _CullMode;
			float _TransparentSortPriority;
			float _EnableFogOnTransparent;
			float _CullModeForward;
			float _TransparentCullMode;
			float _ZTestDepthEqualForOpaque;
			float _ZTestTransparent;
			float _TransparentBackfaceEnable;
			float _AlphaCutoffEnable;
			float _UseShadowThreshold;
			float _DoubleSidedEnable;
			float _DoubleSidedNormalMode;
			float4 _DoubleSidedConstants;
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			TEXTURE2D_ARRAY(_MatTexArray);
			sampler2D _Control;
			SAMPLER(sampler_MatTexArray);
			sampler2D _GrassControl;
			TEXTURE2D_ARRAY(_ShapeMap);
			SAMPLER(sampler_ShapeMap);
			sampler2D _GrassTint;
			sampler2D _Tint;
			sampler2D _SpatterTex;


			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/NormalSurfaceGradient.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Lit/Lit.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/BuiltinUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/MaterialUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Decal/DecalUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Lit/LitDecalData.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderGraphFunctions.hlsl"

			#define GRASS


			#if defined(_DOUBLESIDED_ON) && !defined(ASE_NEED_CULLFACE)
				#define ASE_NEED_CULLFACE 1
			#endif

			struct AttributesMesh
			{
				float3 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 tangentOS : TANGENT;
				float4 uv0 : TEXCOORD0;
				float4 uv1 : TEXCOORD1;
				float4 uv2 : TEXCOORD2;
				float4 uv3 : TEXCOORD3;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryingsMeshToPS
			{
				float4 positionCS : SV_Position;
				#ifdef EDITOR_VISUALIZATION
				float2 VizUV : TEXCOORD0;
				float4 LightCoord : TEXCOORD1;
				#endif
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				#if defined(SHADER_STAGE_FRAGMENT) && defined(ASE_NEED_CULLFACE)
				FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
				#endif
			};

			float3 mod3D289( float3 x ) { return x - floor( x / 289.0 ) * 289.0; }
			float4 mod3D289( float4 x ) { return x - floor( x / 289.0 ) * 289.0; }
			float4 permute( float4 x ) { return mod3D289( ( x * 34.0 + 1.0 ) * x ); }
			float4 taylorInvSqrt( float4 r ) { return 1.79284291400159 - r * 0.85373472095314; }
			float snoise( float3 v )
			{
				const float2 C = float2( 1.0 / 6.0, 1.0 / 3.0 );
				float3 i = floor( v + dot( v, C.yyy ) );
				float3 x0 = v - i + dot( i, C.xxx );
				float3 g = step( x0.yzx, x0.xyz );
				float3 l = 1.0 - g;
				float3 i1 = min( g.xyz, l.zxy );
				float3 i2 = max( g.xyz, l.zxy );
				float3 x1 = x0 - i1 + C.xxx;
				float3 x2 = x0 - i2 + C.yyy;
				float3 x3 = x0 - 0.5;
				i = mod3D289( i);
				float4 p = permute( permute( permute( i.z + float4( 0.0, i1.z, i2.z, 1.0 ) ) + i.y + float4( 0.0, i1.y, i2.y, 1.0 ) ) + i.x + float4( 0.0, i1.x, i2.x, 1.0 ) );
				float4 j = p - 49.0 * floor( p / 49.0 );  // mod(p,7*7)
				float4 x_ = floor( j / 7.0 );
				float4 y_ = floor( j - 7.0 * x_ );  // mod(j,N)
				float4 x = ( x_ * 2.0 + 0.5 ) / 7.0 - 1.0;
				float4 y = ( y_ * 2.0 + 0.5 ) / 7.0 - 1.0;
				float4 h = 1.0 - abs( x ) - abs( y );
				float4 b0 = float4( x.xy, y.xy );
				float4 b1 = float4( x.zw, y.zw );
				float4 s0 = floor( b0 ) * 2.0 + 1.0;
				float4 s1 = floor( b1 ) * 2.0 + 1.0;
				float4 sh = -step( h, 0.0 );
				float4 a0 = b0.xzyw + s0.xzyw * sh.xxyy;
				float4 a1 = b1.xzyw + s1.xzyw * sh.zzww;
				float3 g0 = float3( a0.xy, h.x );
				float3 g1 = float3( a0.zw, h.y );
				float3 g2 = float3( a1.xy, h.z );
				float3 g3 = float3( a1.zw, h.w );
				float4 norm = taylorInvSqrt( float4( dot( g0, g0 ), dot( g1, g1 ), dot( g2, g2 ), dot( g3, g3 ) ) );
				g0 *= norm.x;
				g1 *= norm.y;
				g2 *= norm.z;
				g3 *= norm.w;
				float4 m = max( 0.6 - float4( dot( x0, x0 ), dot( x1, x1 ), dot( x2, x2 ), dot( x3, x3 ) ), 0.0 );
				m = m* m;
				m = m* m;
				float4 px = float4( dot( x0, g0 ), dot( x1, g1 ), dot( x2, g2 ), dot( x3, g3 ) );
				return 42.0 * dot( m, px);
			}
			

			void BuildSurfaceData(FragInputs fragInputs, inout GlobalSurfaceDescription surfaceDescription, float3 V, PositionInputs posInput, out SurfaceData surfaceData, out float3 bentNormalWS)
			{
				ZERO_INITIALIZE(SurfaceData, surfaceData);

				surfaceData.specularOcclusion = 1.0;

				// surface data
				surfaceData.baseColor =					surfaceDescription.Albedo;
				surfaceData.perceptualSmoothness =		surfaceDescription.Smoothness;
				surfaceData.ambientOcclusion =			surfaceDescription.Occlusion;
				surfaceData.metallic =					surfaceDescription.Metallic;
				surfaceData.coatMask =					surfaceDescription.CoatMask;

				#ifdef _SPECULAR_OCCLUSION_CUSTOM
				surfaceData.specularOcclusion =			surfaceDescription.SpecularOcclusion;
				#endif
				#ifdef _MATERIAL_FEATURE_SUBSURFACE_SCATTERING
				surfaceData.subsurfaceMask =			surfaceDescription.SubsurfaceMask;
				#endif
				#if defined(_HAS_REFRACTION) || defined(_MATERIAL_FEATURE_TRANSMISSION)
				surfaceData.thickness =					surfaceDescription.Thickness;
				#endif
				#if defined( _MATERIAL_FEATURE_SUBSURFACE_SCATTERING ) || defined( _MATERIAL_FEATURE_TRANSMISSION )
				surfaceData.diffusionProfileHash =		asuint(surfaceDescription.DiffusionProfile);
				#endif
				#ifdef _MATERIAL_FEATURE_SPECULAR_COLOR
				surfaceData.specularColor =				surfaceDescription.Specular;
				#endif
				#ifdef _MATERIAL_FEATURE_ANISOTROPY
				surfaceData.anisotropy =				surfaceDescription.Anisotropy;
				#endif
				#ifdef _MATERIAL_FEATURE_IRIDESCENCE
				surfaceData.iridescenceMask =			surfaceDescription.IridescenceMask;
				surfaceData.iridescenceThickness =		surfaceDescription.IridescenceThickness;
				#endif

				// refraction
				#ifdef _HAS_REFRACTION
				if( _EnableSSRefraction )
				{
					surfaceData.ior = surfaceDescription.RefractionIndex;
					surfaceData.transmittanceColor = surfaceDescription.RefractionColor;
					surfaceData.atDistance = surfaceDescription.RefractionDistance;

					surfaceData.transmittanceMask = ( 1.0 - surfaceDescription.Alpha );
					surfaceDescription.Alpha = 1.0;
				}
				else
				{
					surfaceData.ior = 1.0;
					surfaceData.transmittanceColor = float3( 1.0, 1.0, 1.0 );
					surfaceData.atDistance = 1.0;
					surfaceData.transmittanceMask = 0.0;
					surfaceDescription.Alpha = 1.0;
				}
				#else
				surfaceData.ior = 1.0;
				surfaceData.transmittanceColor = float3( 1.0, 1.0, 1.0 );
				surfaceData.atDistance = 1.0;
				surfaceData.transmittanceMask = 0.0;
				#endif


				// material features
				surfaceData.materialFeatures = MATERIALFEATUREFLAGS_LIT_STANDARD;
				#ifdef _MATERIAL_FEATURE_SUBSURFACE_SCATTERING
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_SUBSURFACE_SCATTERING;
				#endif
				#ifdef _MATERIAL_FEATURE_TRANSMISSION
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_TRANSMISSION;
				#endif
				#ifdef _MATERIAL_FEATURE_ANISOTROPY
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_ANISOTROPY;
				#endif
				#ifdef ASE_LIT_CLEAR_COAT
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_CLEAR_COAT;
				#endif
				#ifdef _MATERIAL_FEATURE_IRIDESCENCE
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_IRIDESCENCE;
				#endif
				#ifdef _MATERIAL_FEATURE_SPECULAR_COLOR
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_SPECULAR_COLOR;
				#endif

				// others
				#if defined (_MATERIAL_FEATURE_SPECULAR_COLOR) && defined (_ENERGY_CONSERVING_SPECULAR)
				surfaceData.baseColor *= ( 1.0 - Max3( surfaceData.specularColor.r, surfaceData.specularColor.g, surfaceData.specularColor.b ) );
				#endif
				#ifdef _DOUBLESIDED_ON
				float3 doubleSidedConstants = _DoubleSidedConstants.xyz;
				#else
				float3 doubleSidedConstants = float3( 1.0, 1.0, 1.0 );
				#endif

				// normals
				float3 normalTS = float3(0.0f, 0.0f, 1.0f);
				normalTS = surfaceDescription.Normal;
				GetNormalWS( fragInputs, normalTS, surfaceData.normalWS, doubleSidedConstants );

				surfaceData.geomNormalWS = fragInputs.tangentToWorld[2];
				surfaceData.tangentWS = normalize( fragInputs.tangentToWorld[ 0 ].xyz );

				// decals
				#if HAVE_DECALS
				if( _EnableDecals )
				{
					DecalSurfaceData decalSurfaceData = GetDecalSurfaceData(posInput, fragInputs.tangentToWorld[2], surfaceDescription.Alpha);
					ApplyDecalToSurfaceData(decalSurfaceData, fragInputs.tangentToWorld[2], surfaceData);
				}
				#endif

				bentNormalWS = surfaceData.normalWS;
				
				#ifdef ASE_BENT_NORMAL
				GetNormalWS( fragInputs, surfaceDescription.BentNormal, bentNormalWS, doubleSidedConstants );
				#endif

				#ifdef _MATERIAL_FEATURE_ANISOTROPY
				surfaceData.tangentWS = TransformTangentToWorld( surfaceDescription.Tangent, fragInputs.tangentToWorld );
				#endif
				surfaceData.tangentWS = Orthonormalize( surfaceData.tangentWS, surfaceData.normalWS );


				#if defined(_SPECULAR_OCCLUSION_CUSTOM)
				#elif defined(_SPECULAR_OCCLUSION_FROM_AO_BENT_NORMAL)
				surfaceData.specularOcclusion = GetSpecularOcclusionFromBentAO( V, bentNormalWS, surfaceData.normalWS, surfaceData.ambientOcclusion, PerceptualSmoothnessToPerceptualRoughness( surfaceData.perceptualSmoothness ) );
				#elif defined(_AMBIENT_OCCLUSION) && defined(_SPECULAR_OCCLUSION_FROM_AO)
				surfaceData.specularOcclusion = GetSpecularOcclusionFromAmbientOcclusion( ClampNdotV( dot( surfaceData.normalWS, V ) ), surfaceData.ambientOcclusion, PerceptualSmoothnessToRoughness( surfaceData.perceptualSmoothness ) );
				#endif

				#ifdef _ENABLE_GEOMETRIC_SPECULAR_AA
				surfaceData.perceptualSmoothness = GeometricNormalFiltering( surfaceData.perceptualSmoothness, fragInputs.tangentToWorld[ 2 ], surfaceDescription.SpecularAAScreenSpaceVariance, surfaceDescription.SpecularAAThreshold );
				#endif

				// debug
				#if defined(DEBUG_DISPLAY)
				if (_DebugMipMapMode != DEBUGMIPMAPMODE_NONE)
				{
					surfaceData.metallic = 0;
				}
				ApplyDebugToSurfaceData(fragInputs.tangentToWorld, surfaceData);
				#endif
			}

			void GetSurfaceAndBuiltinData(GlobalSurfaceDescription surfaceDescription, FragInputs fragInputs, float3 V, inout PositionInputs posInput, out SurfaceData surfaceData, out BuiltinData builtinData)
			{
				#ifdef LOD_FADE_CROSSFADE
				LODDitheringTransition(ComputeFadeMaskSeed(V, posInput.positionSS), unity_LODFade.x);
				#endif

				#ifdef _DOUBLESIDED_ON
				float3 doubleSidedConstants = _DoubleSidedConstants.xyz;
				#else
				float3 doubleSidedConstants = float3( 1.0, 1.0, 1.0 );
				#endif

				ApplyDoubleSidedFlipOrMirror( fragInputs, doubleSidedConstants );

				#ifdef _ALPHATEST_ON
				DoAlphaTest( surfaceDescription.Alpha, surfaceDescription.AlphaClipThreshold );
				#endif

				#ifdef _DEPTHOFFSET_ON
				builtinData.depthOffset = surfaceDescription.DepthOffset;
				ApplyDepthOffsetPositionInput( V, surfaceDescription.DepthOffset, GetViewForwardDir(), GetWorldToHClipMatrix(), posInput );
				#endif

				float3 bentNormalWS;
				BuildSurfaceData( fragInputs, surfaceDescription, V, posInput, surfaceData, bentNormalWS );

				InitBuiltinData( posInput, surfaceDescription.Alpha, bentNormalWS, -fragInputs.tangentToWorld[ 2 ], fragInputs.texCoord1, fragInputs.texCoord2, builtinData );

				builtinData.emissiveColor = surfaceDescription.Emission;

				#if (SHADERPASS == SHADERPASS_DISTORTION)
				builtinData.distortion = surfaceDescription.Distortion;
				builtinData.distortionBlur = surfaceDescription.DistortionBlur;
				#else
				builtinData.distortion = float2(0.0, 0.0);
				builtinData.distortionBlur = 0.0;
				#endif

				PostInitBuiltinData(V, posInput, surfaceData, builtinData);
			}

			#if SHADERPASS == SHADERPASS_LIGHT_TRANSPORT			
			#define SCENEPICKINGPASS
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/PickingSpaceTransforms.hlsl"
			#endif

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/MetaPass.hlsl"

			PackedVaryingsMeshToPS VertexFunction(AttributesMesh inputMesh  )
			{
				PackedVaryingsMeshToPS outputPackedVaryingsMeshToPS;

				UNITY_SETUP_INSTANCE_ID(inputMesh);
				UNITY_TRANSFER_INSTANCE_ID(inputMesh, outputPackedVaryingsMeshToPS);

				float3 ase_worldPos = GetAbsolutePositionWS( TransformObjectToWorld( (inputMesh.positionOS).xyz ) );
				outputPackedVaryingsMeshToPS.ase_texcoord3.xyz = ase_worldPos;
				
				outputPackedVaryingsMeshToPS.ase_texcoord2.xy = inputMesh.uv0.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				outputPackedVaryingsMeshToPS.ase_texcoord2.zw = 0;
				outputPackedVaryingsMeshToPS.ase_texcoord3.w = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
				float3 defaultVertexValue = inputMesh.positionOS.xyz;
				#else
				float3 defaultVertexValue = float3( 0, 0, 0 );
				#endif
				float3 vertexValue =  defaultVertexValue ;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
				inputMesh.positionOS.xyz = vertexValue;
				#else
				inputMesh.positionOS.xyz += vertexValue;
				#endif

				inputMesh.normalOS =  inputMesh.normalOS ;
				inputMesh.tangentOS =  inputMesh.tangentOS ;

				outputPackedVaryingsMeshToPS.positionCS = UnityMetaVertexPosition(inputMesh.positionOS, inputMesh.uv1.xy, inputMesh.uv2.xy, unity_LightmapST, unity_DynamicLightmapST);


				#ifdef EDITOR_VISUALIZATION
					float2 vizUV = 0;
					float4 lightCoord = 0;
					UnityEditorVizData(inputMesh.positionOS.xyz, inputMesh.uv0.xy, inputMesh.uv1.xy, inputMesh.uv2.xy, vizUV, lightCoord);

					outputPackedVaryingsMeshToPS.VizUV.xy = vizUV;
					outputPackedVaryingsMeshToPS.LightCoord = lightCoord;
				#endif

				return outputPackedVaryingsMeshToPS;
			}

			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float3 positionOS : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 tangentOS : TANGENT;
				float4 uv0 : TEXCOORD0;
				float4 uv1 : TEXCOORD1;
				float4 uv2 : TEXCOORD2;
				float4 uv3 : TEXCOORD3;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl Vert ( AttributesMesh v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.positionOS = v.positionOS;
				o.normalOS = v.normalOS;
				o.tangentOS = v.tangentOS;
				o.uv0 = v.uv0;
				o.uv1 = v.uv1;
				o.uv2 = v.uv2;
				o.uv3 = v.uv3;
				
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if (SHADEROPTIONS_CAMERA_RELATIVE_RENDERING != 0)
				float3 cameraPos = 0;
				#else
				float3 cameraPos = _WorldSpaceCameraPos;
				#endif
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(float4(v[0].positionOS,1), float4(v[1].positionOS,1), float4(v[2].positionOS,1), tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), cameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(float4(v[0].positionOS,1), float4(v[1].positionOS,1), float4(v[2].positionOS,1), edgeLength, GetObjectToWorldMatrix(), cameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(float4(v[0].positionOS,1), float4(v[1].positionOS,1), float4(v[2].positionOS,1), edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), cameraPos, _ScreenParams, _FrustumPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			PackedVaryingsMeshToPS DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				AttributesMesh o = (AttributesMesh) 0;
				o.positionOS = patch[0].positionOS * bary.x + patch[1].positionOS * bary.y + patch[2].positionOS * bary.z;
				o.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				o.tangentOS = patch[0].tangentOS * bary.x + patch[1].tangentOS * bary.y + patch[2].tangentOS * bary.z;
				o.uv0 = patch[0].uv0 * bary.x + patch[1].uv0 * bary.y + patch[2].uv0 * bary.z;
				o.uv1 = patch[0].uv1 * bary.x + patch[1].uv1 * bary.y + patch[2].uv1 * bary.z;
				o.uv2 = patch[0].uv2 * bary.x + patch[1].uv2 * bary.y + patch[2].uv2 * bary.z;
				o.uv3 = patch[0].uv3 * bary.x + patch[1].uv3 * bary.y + patch[2].uv3 * bary.z;
				
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.positionOS.xyz - patch[i].normalOS * (dot(o.positionOS.xyz, patch[i].normalOS) - dot(patch[i].positionOS.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				o.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			PackedVaryingsMeshToPS Vert ( AttributesMesh v )
			{
				return VertexFunction( v );
			}
			#endif

			float4 Frag(PackedVaryingsMeshToPS packedInput  ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( packedInput );
				FragInputs input;
				ZERO_INITIALIZE(FragInputs, input);
				input.tangentToWorld = k_identity3x3;
				input.positionSS = packedInput.positionCS;

				#if _DOUBLESIDED_ON && SHADER_STAGE_FRAGMENT
				input.isFrontFace = IS_FRONT_VFACE( packedInput.cullFace, true, false);
				#elif SHADER_STAGE_FRAGMENT
				#if defined(ASE_NEED_CULLFACE)
				input.isFrontFace = IS_FRONT_VFACE(packedInput.cullFace, true, false);
				#endif
				#endif
				half isFrontFace = input.isFrontFace;

				PositionInputs posInput = GetPositionInput(input.positionSS.xy, _ScreenSize.zw, input.positionSS.z, input.positionSS.w, input.positionRWS);
				float3 V = float3(1.0, 1.0, 1.0);

				SurfaceData surfaceData;
				BuiltinData builtinData;
				GlobalSurfaceDescription surfaceDescription = (GlobalSurfaceDescription)0;
				float2 uv_MatTexArray = packedInput.ase_texcoord2.xy;
				float3 ase_worldPos = packedInput.ase_texcoord3.xyz;
				float2 appendResult257 = (float2(ase_worldPos.x , ase_worldPos.z));
				float2 appendResult260 = (float2(_WorldBounds.x , _WorldBounds.y));
				float2 appendResult259 = (float2(_WorldBounds.z , _WorldBounds.w));
				float2 temp_output_264_0 = ( ( appendResult257 - appendResult260 ) / ( appendResult259 - appendResult260 ) );
				float2 appendResult265 = (float2(_Control_TexelSize.z , _Control_TexelSize.w));
				float2 temp_output_266_0 = ( temp_output_264_0 * appendResult265 );
				float2 temp_output_270_0 = floor( ( ( temp_output_266_0 + float2( 0.5,0.5 ) ) - float2( 0.5,0.5 ) ) );
				float2 temp_output_12_0_g423 = ( ( temp_output_270_0 + float2( 0,0 ) ) / appendResult265 );
				float4 tex2DNode6_g423 = tex2D( _Control, temp_output_12_0_g423 );
				float4 tex2DArrayNode23_g423 = SAMPLE_TEXTURE2D_ARRAY( _MatTexArray, sampler_MatTexArray, uv_MatTexArray,( tex2DNode6_g423.r * 255.0 ) );
				float4 tex2DNode15_g423 = tex2D( _GrassControl, temp_output_12_0_g423 );
				float2 uv_ShapeMap = packedInput.ase_texcoord2.xy;
				float4 tex2DArrayNode35_g423 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode15_g423.g * 255.0 ) );
				float temp_output_24_0_g425 = tex2DArrayNode35_g423.b;
				float4 tex2DNode16_g423 = tex2D( _GrassTint, temp_output_12_0_g423 );
				float temp_output_25_0_g425 = tex2DNode16_g423.a;
				float4 tex2DArrayNode32_g423 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode6_g423.g * 255.0 ) );
				float temp_output_26_0_g425 = tex2DArrayNode32_g423.b;
				float temp_output_41_0_g423 = ( 1.0 - tex2DNode16_g423.a );
				float temp_output_27_0_g425 = temp_output_41_0_g423;
				float temp_output_1_0_g425 = ( max( ( temp_output_24_0_g425 + temp_output_25_0_g425 ) , ( temp_output_26_0_g425 + temp_output_27_0_g425 ) ) - 0.2 );
				float temp_output_21_0_g425 = max( ( ( temp_output_24_0_g425 + temp_output_25_0_g425 ) - temp_output_1_0_g425 ) , 0.0 );
				float temp_output_19_0_g425 = max( ( ( temp_output_26_0_g425 + temp_output_27_0_g425 ) - temp_output_1_0_g425 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch29_g423 = ( ( ( SAMPLE_TEXTURE2D_ARRAY( _MatTexArray, sampler_MatTexArray, uv_MatTexArray,( tex2DNode15_g423.r * 255.0 ) ) * temp_output_21_0_g425 ) + ( tex2DArrayNode23_g423 * temp_output_19_0_g425 ) ) / ( temp_output_21_0_g425 + temp_output_19_0_g425 ) );
				#else
				float4 staticSwitch29_g423 = tex2DArrayNode23_g423;
				#endif
				float temp_output_24_0_g424 = tex2DArrayNode35_g423.b;
				float temp_output_25_0_g424 = tex2DNode16_g423.a;
				float temp_output_26_0_g424 = tex2DArrayNode32_g423.b;
				float temp_output_27_0_g424 = temp_output_41_0_g423;
				float temp_output_1_0_g424 = ( max( ( temp_output_24_0_g424 + temp_output_25_0_g424 ) , ( temp_output_26_0_g424 + temp_output_27_0_g424 ) ) - 0.2 );
				float temp_output_21_0_g424 = max( ( ( temp_output_24_0_g424 + temp_output_25_0_g424 ) - temp_output_1_0_g424 ) , 0.0 );
				float temp_output_19_0_g424 = max( ( ( temp_output_26_0_g424 + temp_output_27_0_g424 ) - temp_output_1_0_g424 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch31_g423 = ( ( ( tex2DArrayNode35_g423 * temp_output_21_0_g424 ) + ( tex2DArrayNode32_g423 * temp_output_19_0_g424 ) ) / ( temp_output_21_0_g424 + temp_output_19_0_g424 ) );
				#else
				float4 staticSwitch31_g423 = tex2DArrayNode32_g423;
				#endif
				float temp_output_302_3 = staticSwitch31_g423.b;
				float temp_output_24_0_g443 = temp_output_302_3;
				float2 break273 = ( temp_output_266_0 - temp_output_270_0 );
				float temp_output_274_0 = ( 1.0 - break273.x );
				float temp_output_25_0_g443 = temp_output_274_0;
				float2 temp_output_12_0_g419 = ( ( temp_output_270_0 + float2( 1,0 ) ) / appendResult265 );
				float4 tex2DNode6_g419 = tex2D( _Control, temp_output_12_0_g419 );
				float4 tex2DArrayNode32_g419 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode6_g419.g * 255.0 ) );
				float4 tex2DNode15_g419 = tex2D( _GrassControl, temp_output_12_0_g419 );
				float4 tex2DArrayNode35_g419 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode15_g419.g * 255.0 ) );
				float temp_output_24_0_g420 = tex2DArrayNode35_g419.b;
				float4 tex2DNode16_g419 = tex2D( _GrassTint, temp_output_12_0_g419 );
				float temp_output_25_0_g420 = tex2DNode16_g419.a;
				float temp_output_26_0_g420 = tex2DArrayNode32_g419.b;
				float temp_output_41_0_g419 = ( 1.0 - tex2DNode16_g419.a );
				float temp_output_27_0_g420 = temp_output_41_0_g419;
				float temp_output_1_0_g420 = ( max( ( temp_output_24_0_g420 + temp_output_25_0_g420 ) , ( temp_output_26_0_g420 + temp_output_27_0_g420 ) ) - 0.2 );
				float temp_output_21_0_g420 = max( ( ( temp_output_24_0_g420 + temp_output_25_0_g420 ) - temp_output_1_0_g420 ) , 0.0 );
				float temp_output_19_0_g420 = max( ( ( temp_output_26_0_g420 + temp_output_27_0_g420 ) - temp_output_1_0_g420 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch31_g419 = ( ( ( tex2DArrayNode35_g419 * temp_output_21_0_g420 ) + ( tex2DArrayNode32_g419 * temp_output_19_0_g420 ) ) / ( temp_output_21_0_g420 + temp_output_19_0_g420 ) );
				#else
				float4 staticSwitch31_g419 = tex2DArrayNode32_g419;
				#endif
				float temp_output_299_3 = staticSwitch31_g419.b;
				float temp_output_26_0_g443 = temp_output_299_3;
				float temp_output_27_0_g443 = break273.x;
				float temp_output_1_0_g443 = ( max( ( temp_output_24_0_g443 + temp_output_25_0_g443 ) , ( temp_output_26_0_g443 + temp_output_27_0_g443 ) ) - 0.2 );
				float temp_output_21_0_g443 = max( ( ( temp_output_24_0_g443 + temp_output_25_0_g443 ) - temp_output_1_0_g443 ) , 0.0 );
				float4 tex2DArrayNode23_g419 = SAMPLE_TEXTURE2D_ARRAY( _MatTexArray, sampler_MatTexArray, uv_MatTexArray,( tex2DNode6_g419.r * 255.0 ) );
				float temp_output_24_0_g421 = tex2DArrayNode35_g419.b;
				float temp_output_25_0_g421 = tex2DNode16_g419.a;
				float temp_output_26_0_g421 = tex2DArrayNode32_g419.b;
				float temp_output_27_0_g421 = temp_output_41_0_g419;
				float temp_output_1_0_g421 = ( max( ( temp_output_24_0_g421 + temp_output_25_0_g421 ) , ( temp_output_26_0_g421 + temp_output_27_0_g421 ) ) - 0.2 );
				float temp_output_21_0_g421 = max( ( ( temp_output_24_0_g421 + temp_output_25_0_g421 ) - temp_output_1_0_g421 ) , 0.0 );
				float temp_output_19_0_g421 = max( ( ( temp_output_26_0_g421 + temp_output_27_0_g421 ) - temp_output_1_0_g421 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch29_g419 = ( ( ( SAMPLE_TEXTURE2D_ARRAY( _MatTexArray, sampler_MatTexArray, uv_MatTexArray,( tex2DNode15_g419.r * 255.0 ) ) * temp_output_21_0_g421 ) + ( tex2DArrayNode23_g419 * temp_output_19_0_g421 ) ) / ( temp_output_21_0_g421 + temp_output_19_0_g421 ) );
				#else
				float4 staticSwitch29_g419 = tex2DArrayNode23_g419;
				#endif
				float temp_output_19_0_g443 = max( ( ( temp_output_26_0_g443 + temp_output_27_0_g443 ) - temp_output_1_0_g443 ) , 0.0 );
				float temp_output_24_0_g436 = temp_output_302_3;
				float temp_output_25_0_g436 = temp_output_274_0;
				float temp_output_26_0_g436 = temp_output_299_3;
				float temp_output_27_0_g436 = break273.x;
				float temp_output_1_0_g436 = ( max( ( temp_output_24_0_g436 + temp_output_25_0_g436 ) , ( temp_output_26_0_g436 + temp_output_27_0_g436 ) ) - 0.2 );
				float temp_output_21_0_g436 = max( ( ( temp_output_24_0_g436 + temp_output_25_0_g436 ) - temp_output_1_0_g436 ) , 0.0 );
				float temp_output_19_0_g436 = max( ( ( temp_output_26_0_g436 + temp_output_27_0_g436 ) - temp_output_1_0_g436 ) , 0.0 );
				float4 temp_output_290_0 = ( ( ( staticSwitch31_g423 * temp_output_21_0_g436 ) + ( staticSwitch31_g419 * temp_output_19_0_g436 ) ) / ( temp_output_21_0_g436 + temp_output_19_0_g436 ) );
				float temp_output_24_0_g442 = temp_output_290_0.z;
				float temp_output_275_0 = ( 1.0 - break273.y );
				float temp_output_25_0_g442 = temp_output_275_0;
				float2 temp_output_12_0_g431 = ( ( temp_output_270_0 + float2( 0,1 ) ) / appendResult265 );
				float4 tex2DNode6_g431 = tex2D( _Control, temp_output_12_0_g431 );
				float4 tex2DArrayNode32_g431 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode6_g431.g * 255.0 ) );
				float4 tex2DNode15_g431 = tex2D( _GrassControl, temp_output_12_0_g431 );
				float4 tex2DArrayNode35_g431 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode15_g431.g * 255.0 ) );
				float temp_output_24_0_g432 = tex2DArrayNode35_g431.b;
				float4 tex2DNode16_g431 = tex2D( _GrassTint, temp_output_12_0_g431 );
				float temp_output_25_0_g432 = tex2DNode16_g431.a;
				float temp_output_26_0_g432 = tex2DArrayNode32_g431.b;
				float temp_output_41_0_g431 = ( 1.0 - tex2DNode16_g431.a );
				float temp_output_27_0_g432 = temp_output_41_0_g431;
				float temp_output_1_0_g432 = ( max( ( temp_output_24_0_g432 + temp_output_25_0_g432 ) , ( temp_output_26_0_g432 + temp_output_27_0_g432 ) ) - 0.2 );
				float temp_output_21_0_g432 = max( ( ( temp_output_24_0_g432 + temp_output_25_0_g432 ) - temp_output_1_0_g432 ) , 0.0 );
				float temp_output_19_0_g432 = max( ( ( temp_output_26_0_g432 + temp_output_27_0_g432 ) - temp_output_1_0_g432 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch31_g431 = ( ( ( tex2DArrayNode35_g431 * temp_output_21_0_g432 ) + ( tex2DArrayNode32_g431 * temp_output_19_0_g432 ) ) / ( temp_output_21_0_g432 + temp_output_19_0_g432 ) );
				#else
				float4 staticSwitch31_g431 = tex2DArrayNode32_g431;
				#endif
				float temp_output_300_3 = staticSwitch31_g431.b;
				float temp_output_24_0_g435 = temp_output_300_3;
				float temp_output_25_0_g435 = temp_output_274_0;
				float2 temp_output_12_0_g427 = ( ( temp_output_270_0 + float2( 1,1 ) ) / appendResult265 );
				float4 tex2DNode6_g427 = tex2D( _Control, temp_output_12_0_g427 );
				float4 tex2DArrayNode32_g427 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode6_g427.g * 255.0 ) );
				float4 tex2DNode15_g427 = tex2D( _GrassControl, temp_output_12_0_g427 );
				float4 tex2DArrayNode35_g427 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode15_g427.g * 255.0 ) );
				float temp_output_24_0_g428 = tex2DArrayNode35_g427.b;
				float4 tex2DNode16_g427 = tex2D( _GrassTint, temp_output_12_0_g427 );
				float temp_output_25_0_g428 = tex2DNode16_g427.a;
				float temp_output_26_0_g428 = tex2DArrayNode32_g427.b;
				float temp_output_41_0_g427 = ( 1.0 - tex2DNode16_g427.a );
				float temp_output_27_0_g428 = temp_output_41_0_g427;
				float temp_output_1_0_g428 = ( max( ( temp_output_24_0_g428 + temp_output_25_0_g428 ) , ( temp_output_26_0_g428 + temp_output_27_0_g428 ) ) - 0.2 );
				float temp_output_21_0_g428 = max( ( ( temp_output_24_0_g428 + temp_output_25_0_g428 ) - temp_output_1_0_g428 ) , 0.0 );
				float temp_output_19_0_g428 = max( ( ( temp_output_26_0_g428 + temp_output_27_0_g428 ) - temp_output_1_0_g428 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch31_g427 = ( ( ( tex2DArrayNode35_g427 * temp_output_21_0_g428 ) + ( tex2DArrayNode32_g427 * temp_output_19_0_g428 ) ) / ( temp_output_21_0_g428 + temp_output_19_0_g428 ) );
				#else
				float4 staticSwitch31_g427 = tex2DArrayNode32_g427;
				#endif
				float temp_output_301_3 = staticSwitch31_g427.b;
				float temp_output_26_0_g435 = temp_output_301_3;
				float temp_output_27_0_g435 = break273.x;
				float temp_output_1_0_g435 = ( max( ( temp_output_24_0_g435 + temp_output_25_0_g435 ) , ( temp_output_26_0_g435 + temp_output_27_0_g435 ) ) - 0.2 );
				float temp_output_21_0_g435 = max( ( ( temp_output_24_0_g435 + temp_output_25_0_g435 ) - temp_output_1_0_g435 ) , 0.0 );
				float temp_output_19_0_g435 = max( ( ( temp_output_26_0_g435 + temp_output_27_0_g435 ) - temp_output_1_0_g435 ) , 0.0 );
				float4 temp_output_293_0 = ( ( ( staticSwitch31_g431 * temp_output_21_0_g435 ) + ( staticSwitch31_g427 * temp_output_19_0_g435 ) ) / ( temp_output_21_0_g435 + temp_output_19_0_g435 ) );
				float temp_output_26_0_g442 = temp_output_293_0.z;
				float temp_output_27_0_g442 = break273.y;
				float temp_output_1_0_g442 = ( max( ( temp_output_24_0_g442 + temp_output_25_0_g442 ) , ( temp_output_26_0_g442 + temp_output_27_0_g442 ) ) - 0.2 );
				float temp_output_21_0_g442 = max( ( ( temp_output_24_0_g442 + temp_output_25_0_g442 ) - temp_output_1_0_g442 ) , 0.0 );
				float4 tex2DArrayNode23_g431 = SAMPLE_TEXTURE2D_ARRAY( _MatTexArray, sampler_MatTexArray, uv_MatTexArray,( tex2DNode6_g431.r * 255.0 ) );
				float temp_output_24_0_g433 = tex2DArrayNode35_g431.b;
				float temp_output_25_0_g433 = tex2DNode16_g431.a;
				float temp_output_26_0_g433 = tex2DArrayNode32_g431.b;
				float temp_output_27_0_g433 = temp_output_41_0_g431;
				float temp_output_1_0_g433 = ( max( ( temp_output_24_0_g433 + temp_output_25_0_g433 ) , ( temp_output_26_0_g433 + temp_output_27_0_g433 ) ) - 0.2 );
				float temp_output_21_0_g433 = max( ( ( temp_output_24_0_g433 + temp_output_25_0_g433 ) - temp_output_1_0_g433 ) , 0.0 );
				float temp_output_19_0_g433 = max( ( ( temp_output_26_0_g433 + temp_output_27_0_g433 ) - temp_output_1_0_g433 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch29_g431 = ( ( ( SAMPLE_TEXTURE2D_ARRAY( _MatTexArray, sampler_MatTexArray, uv_MatTexArray,( tex2DNode15_g431.r * 255.0 ) ) * temp_output_21_0_g433 ) + ( tex2DArrayNode23_g431 * temp_output_19_0_g433 ) ) / ( temp_output_21_0_g433 + temp_output_19_0_g433 ) );
				#else
				float4 staticSwitch29_g431 = tex2DArrayNode23_g431;
				#endif
				float temp_output_24_0_g488 = temp_output_300_3;
				float temp_output_25_0_g488 = temp_output_274_0;
				float temp_output_26_0_g488 = temp_output_301_3;
				float temp_output_27_0_g488 = break273.x;
				float temp_output_1_0_g488 = ( max( ( temp_output_24_0_g488 + temp_output_25_0_g488 ) , ( temp_output_26_0_g488 + temp_output_27_0_g488 ) ) - 0.2 );
				float temp_output_21_0_g488 = max( ( ( temp_output_24_0_g488 + temp_output_25_0_g488 ) - temp_output_1_0_g488 ) , 0.0 );
				float4 tex2DArrayNode23_g427 = SAMPLE_TEXTURE2D_ARRAY( _MatTexArray, sampler_MatTexArray, uv_MatTexArray,( tex2DNode6_g427.r * 255.0 ) );
				float temp_output_24_0_g429 = tex2DArrayNode35_g427.b;
				float temp_output_25_0_g429 = tex2DNode16_g427.a;
				float temp_output_26_0_g429 = tex2DArrayNode32_g427.b;
				float temp_output_27_0_g429 = temp_output_41_0_g427;
				float temp_output_1_0_g429 = ( max( ( temp_output_24_0_g429 + temp_output_25_0_g429 ) , ( temp_output_26_0_g429 + temp_output_27_0_g429 ) ) - 0.2 );
				float temp_output_21_0_g429 = max( ( ( temp_output_24_0_g429 + temp_output_25_0_g429 ) - temp_output_1_0_g429 ) , 0.0 );
				float temp_output_19_0_g429 = max( ( ( temp_output_26_0_g429 + temp_output_27_0_g429 ) - temp_output_1_0_g429 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch29_g427 = ( ( ( SAMPLE_TEXTURE2D_ARRAY( _MatTexArray, sampler_MatTexArray, uv_MatTexArray,( tex2DNode15_g427.r * 255.0 ) ) * temp_output_21_0_g429 ) + ( tex2DArrayNode23_g427 * temp_output_19_0_g429 ) ) / ( temp_output_21_0_g429 + temp_output_19_0_g429 ) );
				#else
				float4 staticSwitch29_g427 = tex2DArrayNode23_g427;
				#endif
				float temp_output_19_0_g488 = max( ( ( temp_output_26_0_g488 + temp_output_27_0_g488 ) - temp_output_1_0_g488 ) , 0.0 );
				float temp_output_19_0_g442 = max( ( ( temp_output_26_0_g442 + temp_output_27_0_g442 ) - temp_output_1_0_g442 ) , 0.0 );
				float4 temp_output_159_0 = ( ( ( ( ( ( staticSwitch29_g423 * temp_output_21_0_g443 ) + ( staticSwitch29_g419 * temp_output_19_0_g443 ) ) / ( temp_output_21_0_g443 + temp_output_19_0_g443 ) ) * temp_output_21_0_g442 ) + ( ( ( ( staticSwitch29_g431 * temp_output_21_0_g488 ) + ( staticSwitch29_g427 * temp_output_19_0_g488 ) ) / ( temp_output_21_0_g488 + temp_output_19_0_g488 ) ) * temp_output_19_0_g442 ) ) / ( temp_output_21_0_g442 + temp_output_19_0_g442 ) );
				float4 tex2DNode14_g423 = tex2D( _Tint, temp_output_12_0_g423 );
				float temp_output_24_0_g426 = tex2DArrayNode35_g423.b;
				float temp_output_25_0_g426 = tex2DNode16_g423.a;
				float temp_output_26_0_g426 = tex2DArrayNode32_g423.b;
				float temp_output_27_0_g426 = temp_output_41_0_g423;
				float temp_output_1_0_g426 = ( max( ( temp_output_24_0_g426 + temp_output_25_0_g426 ) , ( temp_output_26_0_g426 + temp_output_27_0_g426 ) ) - 0.2 );
				float temp_output_21_0_g426 = max( ( ( temp_output_24_0_g426 + temp_output_25_0_g426 ) - temp_output_1_0_g426 ) , 0.0 );
				float temp_output_19_0_g426 = max( ( ( temp_output_26_0_g426 + temp_output_27_0_g426 ) - temp_output_1_0_g426 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch30_g423 = ( ( ( tex2DNode16_g423 * temp_output_21_0_g426 ) + ( tex2DNode14_g423 * temp_output_19_0_g426 ) ) / ( temp_output_21_0_g426 + temp_output_19_0_g426 ) );
				#else
				float4 staticSwitch30_g423 = tex2DNode14_g423;
				#endif
				float temp_output_24_0_g437 = temp_output_302_3;
				float temp_output_25_0_g437 = temp_output_274_0;
				float temp_output_26_0_g437 = temp_output_299_3;
				float temp_output_27_0_g437 = break273.x;
				float temp_output_1_0_g437 = ( max( ( temp_output_24_0_g437 + temp_output_25_0_g437 ) , ( temp_output_26_0_g437 + temp_output_27_0_g437 ) ) - 0.2 );
				float temp_output_21_0_g437 = max( ( ( temp_output_24_0_g437 + temp_output_25_0_g437 ) - temp_output_1_0_g437 ) , 0.0 );
				float4 tex2DNode14_g419 = tex2D( _Tint, temp_output_12_0_g419 );
				float temp_output_24_0_g422 = tex2DArrayNode35_g419.b;
				float temp_output_25_0_g422 = tex2DNode16_g419.a;
				float temp_output_26_0_g422 = tex2DArrayNode32_g419.b;
				float temp_output_27_0_g422 = temp_output_41_0_g419;
				float temp_output_1_0_g422 = ( max( ( temp_output_24_0_g422 + temp_output_25_0_g422 ) , ( temp_output_26_0_g422 + temp_output_27_0_g422 ) ) - 0.2 );
				float temp_output_21_0_g422 = max( ( ( temp_output_24_0_g422 + temp_output_25_0_g422 ) - temp_output_1_0_g422 ) , 0.0 );
				float temp_output_19_0_g422 = max( ( ( temp_output_26_0_g422 + temp_output_27_0_g422 ) - temp_output_1_0_g422 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch30_g419 = ( ( ( tex2DNode16_g419 * temp_output_21_0_g422 ) + ( tex2DNode14_g419 * temp_output_19_0_g422 ) ) / ( temp_output_21_0_g422 + temp_output_19_0_g422 ) );
				#else
				float4 staticSwitch30_g419 = tex2DNode14_g419;
				#endif
				float temp_output_19_0_g437 = max( ( ( temp_output_26_0_g437 + temp_output_27_0_g437 ) - temp_output_1_0_g437 ) , 0.0 );
				float temp_output_24_0_g439 = temp_output_290_0.z;
				float temp_output_25_0_g439 = temp_output_275_0;
				float temp_output_26_0_g439 = temp_output_293_0.z;
				float temp_output_27_0_g439 = break273.y;
				float temp_output_1_0_g439 = ( max( ( temp_output_24_0_g439 + temp_output_25_0_g439 ) , ( temp_output_26_0_g439 + temp_output_27_0_g439 ) ) - 0.2 );
				float temp_output_21_0_g439 = max( ( ( temp_output_24_0_g439 + temp_output_25_0_g439 ) - temp_output_1_0_g439 ) , 0.0 );
				float4 tex2DNode14_g431 = tex2D( _Tint, temp_output_12_0_g431 );
				float temp_output_24_0_g434 = tex2DArrayNode35_g431.b;
				float temp_output_25_0_g434 = tex2DNode16_g431.a;
				float temp_output_26_0_g434 = tex2DArrayNode32_g431.b;
				float temp_output_27_0_g434 = temp_output_41_0_g431;
				float temp_output_1_0_g434 = ( max( ( temp_output_24_0_g434 + temp_output_25_0_g434 ) , ( temp_output_26_0_g434 + temp_output_27_0_g434 ) ) - 0.2 );
				float temp_output_21_0_g434 = max( ( ( temp_output_24_0_g434 + temp_output_25_0_g434 ) - temp_output_1_0_g434 ) , 0.0 );
				float temp_output_19_0_g434 = max( ( ( temp_output_26_0_g434 + temp_output_27_0_g434 ) - temp_output_1_0_g434 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch30_g431 = ( ( ( tex2DNode16_g431 * temp_output_21_0_g434 ) + ( tex2DNode14_g431 * temp_output_19_0_g434 ) ) / ( temp_output_21_0_g434 + temp_output_19_0_g434 ) );
				#else
				float4 staticSwitch30_g431 = tex2DNode14_g431;
				#endif
				float temp_output_24_0_g438 = temp_output_300_3;
				float temp_output_25_0_g438 = temp_output_274_0;
				float temp_output_26_0_g438 = temp_output_301_3;
				float temp_output_27_0_g438 = break273.x;
				float temp_output_1_0_g438 = ( max( ( temp_output_24_0_g438 + temp_output_25_0_g438 ) , ( temp_output_26_0_g438 + temp_output_27_0_g438 ) ) - 0.2 );
				float temp_output_21_0_g438 = max( ( ( temp_output_24_0_g438 + temp_output_25_0_g438 ) - temp_output_1_0_g438 ) , 0.0 );
				float4 tex2DNode14_g427 = tex2D( _Tint, temp_output_12_0_g427 );
				float temp_output_24_0_g430 = tex2DArrayNode35_g427.b;
				float temp_output_25_0_g430 = tex2DNode16_g427.a;
				float temp_output_26_0_g430 = tex2DArrayNode32_g427.b;
				float temp_output_27_0_g430 = temp_output_41_0_g427;
				float temp_output_1_0_g430 = ( max( ( temp_output_24_0_g430 + temp_output_25_0_g430 ) , ( temp_output_26_0_g430 + temp_output_27_0_g430 ) ) - 0.2 );
				float temp_output_21_0_g430 = max( ( ( temp_output_24_0_g430 + temp_output_25_0_g430 ) - temp_output_1_0_g430 ) , 0.0 );
				float temp_output_19_0_g430 = max( ( ( temp_output_26_0_g430 + temp_output_27_0_g430 ) - temp_output_1_0_g430 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch30_g427 = ( ( ( tex2DNode16_g427 * temp_output_21_0_g430 ) + ( tex2DNode14_g427 * temp_output_19_0_g430 ) ) / ( temp_output_21_0_g430 + temp_output_19_0_g430 ) );
				#else
				float4 staticSwitch30_g427 = tex2DNode14_g427;
				#endif
				float temp_output_19_0_g438 = max( ( ( temp_output_26_0_g438 + temp_output_27_0_g438 ) - temp_output_1_0_g438 ) , 0.0 );
				float temp_output_19_0_g439 = max( ( ( temp_output_26_0_g439 + temp_output_27_0_g439 ) - temp_output_1_0_g439 ) , 0.0 );
				float4 temp_output_221_0 = ( ( ( ( ( ( staticSwitch30_g423 * temp_output_21_0_g437 ) + ( staticSwitch30_g419 * temp_output_19_0_g437 ) ) / ( temp_output_21_0_g437 + temp_output_19_0_g437 ) ) * temp_output_21_0_g439 ) + ( ( ( ( staticSwitch30_g431 * temp_output_21_0_g438 ) + ( staticSwitch30_g427 * temp_output_19_0_g438 ) ) / ( temp_output_21_0_g438 + temp_output_19_0_g438 ) ) * temp_output_19_0_g439 ) ) / ( temp_output_21_0_g439 + temp_output_19_0_g439 ) );
				float2 appendResult12_g487 = (float2(ase_worldPos.x , ase_worldPos.z));
				float4 break7_g487 = _WorldBounds;
				float2 appendResult8_g487 = (float2(break7_g487.x , break7_g487.y));
				float2 appendResult9_g487 = (float2(break7_g487.z , break7_g487.w));
				float4 tex2DNode15_g487 = tex2D( _SpatterTex, ( ( appendResult12_g487 - appendResult8_g487 ) / ( appendResult9_g487 - temp_output_264_0 ) ) );
				float simplePerlin3D16_g487 = snoise( float3( appendResult12_g487 ,  0.0 )*2.0 );
				simplePerlin3D16_g487 = simplePerlin3D16_g487*0.5 + 0.5;
				float temp_output_17_0_g487 = ( tex2DNode15_g487.a - simplePerlin3D16_g487 );
				float HeightMask33_g487 = saturate(pow(max( (((simplePerlin3D16_g487*temp_output_17_0_g487)*4)+(temp_output_17_0_g487*2)), 0 ),1.0));
				float4 lerpResult34_g487 = lerp( ( temp_output_159_0 * temp_output_221_0 ) , tex2DNode15_g487 , HeightMask33_g487);
				
				float temp_output_24_0_g441 = temp_output_290_0.z;
				float temp_output_25_0_g441 = temp_output_275_0;
				float temp_output_26_0_g441 = temp_output_293_0.z;
				float temp_output_27_0_g441 = break273.y;
				float temp_output_1_0_g441 = ( max( ( temp_output_24_0_g441 + temp_output_25_0_g441 ) , ( temp_output_26_0_g441 + temp_output_27_0_g441 ) ) - 0.2 );
				float temp_output_21_0_g441 = max( ( ( temp_output_24_0_g441 + temp_output_25_0_g441 ) - temp_output_1_0_g441 ) , 0.0 );
				float temp_output_19_0_g441 = max( ( ( temp_output_26_0_g441 + temp_output_27_0_g441 ) - temp_output_1_0_g441 ) , 0.0 );
				float4 temp_output_180_0 = ( ( ( temp_output_290_0 * temp_output_21_0_g441 ) + ( temp_output_293_0 * temp_output_19_0_g441 ) ) / ( temp_output_21_0_g441 + temp_output_19_0_g441 ) );
				float3 unpack220 = UnpackNormalScale( temp_output_180_0, _NormalScale );
				unpack220.z = lerp( 1, unpack220.z, saturate(_NormalScale) );
				
				float temp_output_3_0_g440 = ( temp_output_221_0.w * 2.0 );
				
				surfaceDescription.Albedo = lerpResult34_g487.rgb;
				surfaceDescription.Normal = unpack220;
				surfaceDescription.BentNormal = float3( 0, 0, 1 );
				surfaceDescription.CoatMask = 0;
				surfaceDescription.Metallic = ( 0.0 * _MetalLevel );

				#ifdef _MATERIAL_FEATURE_SPECULAR_COLOR
				surfaceDescription.Specular = 0;
				#endif

				surfaceDescription.Emission = 0;
				surfaceDescription.Smoothness = ( temp_output_159_0.w * _Rough );
				surfaceDescription.Occlusion = temp_output_180_0.x;
				surfaceDescription.Alpha = min( temp_output_3_0_g440 , 1.0 );

				#ifdef _ALPHATEST_ON
				surfaceDescription.AlphaClipThreshold = _AlphaCutoff;
				#endif

				#ifdef _ENABLE_GEOMETRIC_SPECULAR_AA
				surfaceDescription.SpecularAAScreenSpaceVariance = 0;
				surfaceDescription.SpecularAAThreshold = 0;
				#endif

				#ifdef _SPECULAR_OCCLUSION_CUSTOM
				surfaceDescription.SpecularOcclusion = 0;
				#endif

				#if defined(_HAS_REFRACTION) || defined(_MATERIAL_FEATURE_TRANSMISSION)
				surfaceDescription.Thickness = 1;
				#endif

				#ifdef _HAS_REFRACTION
				surfaceDescription.RefractionIndex = 1;
				surfaceDescription.RefractionColor = float3( 1, 1, 1 );
				surfaceDescription.RefractionDistance = 0;
				#endif

				#ifdef _MATERIAL_FEATURE_SUBSURFACE_SCATTERING
				surfaceDescription.SubsurfaceMask = 1;
				#endif

				#if defined( _MATERIAL_FEATURE_SUBSURFACE_SCATTERING ) || defined( _MATERIAL_FEATURE_TRANSMISSION )
				surfaceDescription.DiffusionProfile = 0;
				#endif

				#ifdef _MATERIAL_FEATURE_ANISOTROPY
				surfaceDescription.Anisotropy = 1;
				surfaceDescription.Tangent = float3( 1, 0, 0 );
				#endif

				#ifdef _MATERIAL_FEATURE_IRIDESCENCE
				surfaceDescription.IridescenceMask = 0;
				surfaceDescription.IridescenceThickness = 0;
				#endif

				GetSurfaceAndBuiltinData(surfaceDescription,input, V, posInput, surfaceData, builtinData);

				BSDFData bsdfData = ConvertSurfaceDataToBSDFData(input.positionSS.xy, surfaceData);
				LightTransportData lightTransportData = GetLightTransportData(surfaceData, builtinData, bsdfData);

				float4 res = float4( 0.0, 0.0, 0.0, 1.0 );
				UnityMetaInput metaInput;
				metaInput.Albedo = lightTransportData.diffuseColor.rgb;
				metaInput.Emission = lightTransportData.emissiveColor;
			#ifdef EDITOR_VISUALIZATION
				metaInput.VizUV = packedInput.VizUV;
				metaInput.LightCoord = packedInput.LightCoord;
			#endif
				res = UnityMetaFragment(metaInput);

				return res;
			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "ShadowCaster"
			Tags { "LightMode"="ShadowCaster" }
			Cull [_CullMode]
			ZWrite On
			ZClip [_ZClip]
			ZTest LEqual
			ColorMask 0

			HLSLPROGRAM

			#define _SPECULAR_OCCLUSION_FROM_AO 1
			#define _DISABLE_DECALS 1
			#define _DISABLE_SSR 1
			#define _AMBIENT_OCCLUSION 1
			#define ASE_SRP_VERSION 999999


			#pragma shader_feature _SURFACE_TYPE_TRANSPARENT
			#pragma shader_feature_local _DOUBLESIDED_ON
			#pragma shader_feature_local _BLENDMODE_OFF _BLENDMODE_ALPHA _BLENDMODE_ADD _BLENDMODE_PRE_MULTIPLY
			#pragma shader_feature_local_fragment _ _ENABLE_FOG_ON_TRANSPARENT
			#pragma shader_feature_local _ALPHATEST_ON

			#define SHADERPASS SHADERPASS_SHADOWS

			#pragma vertex Vert
			#pragma fragment Frag

			//#define UNITY_MATERIAL_LIT

			#if defined(_MATERIAL_FEATURE_SUBSURFACE_SCATTERING) && !defined(_SURFACE_TYPE_TRANSPARENT)
			#define OUTPUT_SPLIT_LIGHTING
			#endif

			#if defined(SHADER_LIT) && !defined(_SURFACE_TYPE_TRANSPARENT)
                #define _DEFERRED_CAPABLE_MATERIAL
            #endif
        
            
            #if defined(_TRANSPARENT_WRITES_MOTION_VEC) && defined(_SURFACE_TYPE_TRANSPARENT)
                #define _WRITE_TRANSPARENT_MOTION_VECTOR
            #endif

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
        	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/GeometricTools.hlsl" 
        	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Tessellation.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/FragInputs.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl" 
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderGraphHeader.hlsl"

			//#define USE_LEGACY_UNITY_MATRIX_VARIABLES

			#ifdef DEBUG_DISPLAY
				#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Debug/DebugDisplay.hlsl"
			#endif

			CBUFFER_START( UnityPerMaterial )
			float4 _MatTexArray_ST;
			float4 _WorldBounds;
			float4 _Control_TexelSize;
			float4 _ShapeMap_ST;
			float _NormalScale;
			float _MetalLevel;
			float _Rough;
			float4 _EmissionColor;
			float _AlphaCutoff;
			float _RenderQueueType;
			#ifdef _ADD_PRECOMPUTED_VELOCITY
			float _AddPrecomputedVelocity;
			#endif
			float _StencilRef;
			float _StencilWriteMask;
			float _StencilRefDepth;
			float _StencilWriteMaskDepth;
			float _StencilRefMV;
			float _StencilWriteMaskMV;
			float _StencilRefDistortionVec;
			float _StencilWriteMaskDistortionVec;
			float _StencilWriteMaskGBuffer;
			float _StencilRefGBuffer;
			float _ZTestGBuffer;
			float _RequireSplitLighting;
			float _ReceivesSSR;
			float _SurfaceType;
			float _BlendMode;
			float _SrcBlend;
			float _DstBlend;
			float _AlphaSrcBlend;
			float _AlphaDstBlend;
			float _ZWrite;
			float _TransparentZWrite;
			float _CullMode;
			float _TransparentSortPriority;
			float _EnableFogOnTransparent;
			float _CullModeForward;
			float _TransparentCullMode;
			float _ZTestDepthEqualForOpaque;
			float _ZTestTransparent;
			float _TransparentBackfaceEnable;
			float _AlphaCutoffEnable;
			float _UseShadowThreshold;
			float _DoubleSidedEnable;
			float _DoubleSidedNormalMode;
			float4 _DoubleSidedConstants;
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			sampler2D _Tint;
			sampler2D _Control;
			sampler2D _GrassTint;
			TEXTURE2D_ARRAY(_ShapeMap);
			sampler2D _GrassControl;
			SAMPLER(sampler_ShapeMap);


			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/NormalSurfaceGradient.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Lit/Lit.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/BuiltinUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/MaterialUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Decal/DecalUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Lit/LitDecalData.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderGraphFunctions.hlsl"

			#define ASE_NEEDS_FRAG_RELATIVE_WORLD_POS
			#define GRASS


			#if defined(_DOUBLESIDED_ON) && !defined(ASE_NEED_CULLFACE)
				#define ASE_NEED_CULLFACE 1
			#endif

			struct AttributesMesh
			{
				float3 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryingsMeshToPS
			{
				float4 positionCS : SV_Position;
				float3 positionRWS : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
				#if defined(SHADER_STAGE_FRAGMENT) && defined(ASE_NEED_CULLFACE)
				FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
				#endif
			};

			
			void BuildSurfaceData(FragInputs fragInputs, inout AlphaSurfaceDescription surfaceDescription, float3 V, PositionInputs posInput, out SurfaceData surfaceData, out float3 bentNormalWS)
			{
				ZERO_INITIALIZE(SurfaceData, surfaceData);

				surfaceData.specularOcclusion = 1.0;

				// surface data

				// refraction
				#ifdef _HAS_REFRACTION
				if( _EnableSSRefraction )
				{
					surfaceData.transmittanceMask = ( 1.0 - surfaceDescription.Alpha );
					surfaceDescription.Alpha = 1.0;
				}
				else
				{
					surfaceData.ior = 1.0;
					surfaceData.transmittanceColor = float3( 1.0, 1.0, 1.0 );
					surfaceData.atDistance = 1.0;
					surfaceData.transmittanceMask = 0.0;
					surfaceDescription.Alpha = 1.0;
				}
				#else
				surfaceData.ior = 1.0;
				surfaceData.transmittanceColor = float3( 1.0, 1.0, 1.0 );
				surfaceData.atDistance = 1.0;
				surfaceData.transmittanceMask = 0.0;
				#endif


				// material features
				surfaceData.materialFeatures = MATERIALFEATUREFLAGS_LIT_STANDARD;
				#ifdef _MATERIAL_FEATURE_SUBSURFACE_SCATTERING
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_SUBSURFACE_SCATTERING;
				#endif
				#ifdef _MATERIAL_FEATURE_TRANSMISSION
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_TRANSMISSION;
				#endif
				#ifdef _MATERIAL_FEATURE_ANISOTROPY
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_ANISOTROPY;
				#endif
				#ifdef ASE_LIT_CLEAR_COAT
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_CLEAR_COAT;
				#endif
				#ifdef _MATERIAL_FEATURE_IRIDESCENCE
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_IRIDESCENCE;
				#endif
				#ifdef _MATERIAL_FEATURE_SPECULAR_COLOR
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_SPECULAR_COLOR;
				#endif

				// others
				#if defined (_MATERIAL_FEATURE_SPECULAR_COLOR) && defined (_ENERGY_CONSERVING_SPECULAR)
				surfaceData.baseColor *= ( 1.0 - Max3( surfaceData.specularColor.r, surfaceData.specularColor.g, surfaceData.specularColor.b ) );
				#endif
				#ifdef _DOUBLESIDED_ON
				float3 doubleSidedConstants = _DoubleSidedConstants.xyz;
				#else
				float3 doubleSidedConstants = float3( 1.0, 1.0, 1.0 );
				#endif

				// normals
				float3 normalTS = float3(0.0f, 0.0f, 1.0f);
				GetNormalWS( fragInputs, normalTS, surfaceData.normalWS, doubleSidedConstants );

				surfaceData.geomNormalWS = fragInputs.tangentToWorld[2];
				surfaceData.tangentWS = normalize( fragInputs.tangentToWorld[ 0 ].xyz );
				
				// decals
				#if HAVE_DECALS
				if( _EnableDecals )
				{
					DecalSurfaceData decalSurfaceData = GetDecalSurfaceData(posInput, fragInputs.tangentToWorld[2], surfaceDescription.Alpha);
					ApplyDecalToSurfaceData(decalSurfaceData, fragInputs.tangentToWorld[2], surfaceData);
				}
				#endif

				bentNormalWS = surfaceData.normalWS;
				surfaceData.tangentWS = Orthonormalize( surfaceData.tangentWS, surfaceData.normalWS );

				#if defined(_SPECULAR_OCCLUSION_CUSTOM)
				#elif defined(_SPECULAR_OCCLUSION_FROM_AO_BENT_NORMAL)
				surfaceData.specularOcclusion = GetSpecularOcclusionFromBentAO( V, bentNormalWS, surfaceData.normalWS, surfaceData.ambientOcclusion, PerceptualSmoothnessToPerceptualRoughness( surfaceData.perceptualSmoothness ) );
				#elif defined(_AMBIENT_OCCLUSION) && defined(_SPECULAR_OCCLUSION_FROM_AO)
				surfaceData.specularOcclusion = GetSpecularOcclusionFromAmbientOcclusion( ClampNdotV( dot( surfaceData.normalWS, V ) ), surfaceData.ambientOcclusion, PerceptualSmoothnessToRoughness( surfaceData.perceptualSmoothness ) );
				#endif

				// debug
				#if defined(DEBUG_DISPLAY)
				if (_DebugMipMapMode != DEBUGMIPMAPMODE_NONE)
				{
					surfaceData.metallic = 0;
				}
				ApplyDebugToSurfaceData(fragInputs.tangentToWorld, surfaceData);
				#endif
			}

			void GetSurfaceAndBuiltinData(AlphaSurfaceDescription surfaceDescription, FragInputs fragInputs, float3 V, inout PositionInputs posInput, out SurfaceData surfaceData, out BuiltinData builtinData)
			{
				#ifdef LOD_FADE_CROSSFADE
				LODDitheringTransition(ComputeFadeMaskSeed(V, posInput.positionSS), unity_LODFade.x);
				#endif

				#ifdef _DOUBLESIDED_ON
				float3 doubleSidedConstants = _DoubleSidedConstants.xyz;
				#else
				float3 doubleSidedConstants = float3( 1.0, 1.0, 1.0 );
				#endif

				ApplyDoubleSidedFlipOrMirror( fragInputs, doubleSidedConstants );

				#ifdef _ALPHATEST_ON
				#ifdef _ALPHATEST_SHADOW_ON
				DoAlphaTest( surfaceDescription.Alpha, surfaceDescription.AlphaClipThresholdShadow );
				#else
				DoAlphaTest( surfaceDescription.Alpha, surfaceDescription.AlphaClipThreshold );
				#endif
				#endif

				#ifdef _DEPTHOFFSET_ON
				builtinData.depthOffset = surfaceDescription.DepthOffset;
				ApplyDepthOffsetPositionInput( V, surfaceDescription.DepthOffset, GetViewForwardDir(), GetWorldToHClipMatrix(), posInput );
				#endif

				float3 bentNormalWS;
				BuildSurfaceData( fragInputs, surfaceDescription, V, posInput, surfaceData, bentNormalWS );

				InitBuiltinData( posInput, surfaceDescription.Alpha, bentNormalWS, -fragInputs.tangentToWorld[ 2 ], fragInputs.texCoord1, fragInputs.texCoord2, builtinData );

				PostInitBuiltinData(V, posInput, surfaceData, builtinData);
			}

			PackedVaryingsMeshToPS VertexFunction(AttributesMesh inputMesh )
			{
				PackedVaryingsMeshToPS outputPackedVaryingsMeshToPS;
				UNITY_SETUP_INSTANCE_ID(inputMesh);
				UNITY_TRANSFER_INSTANCE_ID(inputMesh, outputPackedVaryingsMeshToPS);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( outputPackedVaryingsMeshToPS );

				outputPackedVaryingsMeshToPS.ase_texcoord1.xy = inputMesh.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				outputPackedVaryingsMeshToPS.ase_texcoord1.zw = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
				float3 defaultVertexValue = inputMesh.positionOS.xyz;
				#else
				float3 defaultVertexValue = float3( 0, 0, 0 );
				#endif
				float3 vertexValue =  defaultVertexValue ;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
				inputMesh.positionOS.xyz = vertexValue;
				#else
				inputMesh.positionOS.xyz += vertexValue;
				#endif

				inputMesh.normalOS =  inputMesh.normalOS ;

				float3 positionRWS = TransformObjectToWorld(inputMesh.positionOS);
				outputPackedVaryingsMeshToPS.positionCS = TransformWorldToHClip(positionRWS);
				outputPackedVaryingsMeshToPS.positionRWS.xyz = positionRWS;
				return outputPackedVaryingsMeshToPS;
			}

			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float3 positionOS : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl Vert ( AttributesMesh v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.positionOS = v.positionOS;
				o.normalOS = v.normalOS;
				o.ase_texcoord = v.ase_texcoord;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if (SHADEROPTIONS_CAMERA_RELATIVE_RENDERING != 0)
				float3 cameraPos = 0;
				#else
				float3 cameraPos = _WorldSpaceCameraPos;
				#endif
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(float4(v[0].positionOS,1), float4(v[1].positionOS,1), float4(v[2].positionOS,1), tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), cameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(float4(v[0].positionOS,1), float4(v[1].positionOS,1), float4(v[2].positionOS,1), edgeLength, GetObjectToWorldMatrix(), cameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(float4(v[0].positionOS,1), float4(v[1].positionOS,1), float4(v[2].positionOS,1), edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), cameraPos, _ScreenParams, _FrustumPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			PackedVaryingsMeshToPS DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				AttributesMesh o = (AttributesMesh) 0;
				o.positionOS = patch[0].positionOS * bary.x + patch[1].positionOS * bary.y + patch[2].positionOS * bary.z;
				o.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.positionOS.xyz - patch[i].normalOS * (dot(o.positionOS.xyz, patch[i].normalOS) - dot(patch[i].positionOS.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				o.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			PackedVaryingsMeshToPS Vert ( AttributesMesh v )
			{
				return VertexFunction( v );
			}
			#endif
			
			#if defined(WRITE_NORMAL_BUFFER) && defined(WRITE_MSAA_DEPTH)
			#define SV_TARGET_DECAL SV_Target2
			#elif defined(WRITE_NORMAL_BUFFER) || defined(WRITE_MSAA_DEPTH)
			#define SV_TARGET_DECAL SV_Target1
			#else
			#define SV_TARGET_DECAL SV_Target0
			#endif

			void Frag( PackedVaryingsMeshToPS packedInput
						#if defined(SCENESELECTIONPASS) || defined(SCENEPICKINGPASS)
						, out float4 outColor : SV_Target0
						#else
							#ifdef WRITE_MSAA_DEPTH
							, out float4 depthColor : SV_Target0
								#ifdef WRITE_NORMAL_BUFFER
								, out float4 outNormalBuffer : SV_Target1
								#endif
							#else
								#ifdef WRITE_NORMAL_BUFFER
								, out float4 outNormalBuffer : SV_Target0
								#endif
							#endif

							#if defined(WRITE_DECAL_BUFFER) && !defined(_DISABLE_DECALS)
							, out float4 outDecalBuffer : SV_TARGET_DECAL
							#endif
						#endif

						#if defined(_DEPTHOFFSET_ON) && !defined(SCENEPICKINGPASS)
						, out float outputDepth : SV_Depth
						#endif
						
					)
			{
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( packedInput );
				UNITY_SETUP_INSTANCE_ID( packedInput );

				float3 positionRWS = packedInput.positionRWS.xyz;

				FragInputs input;
				ZERO_INITIALIZE(FragInputs, input);

				input.tangentToWorld = k_identity3x3;
				input.positionSS = packedInput.positionCS;

				input.positionRWS = positionRWS;

				#if _DOUBLESIDED_ON && SHADER_STAGE_FRAGMENT
				input.isFrontFace = IS_FRONT_VFACE( packedInput.cullFace, true, false);
				#elif SHADER_STAGE_FRAGMENT
				#if defined(ASE_NEED_CULLFACE)
				input.isFrontFace = IS_FRONT_VFACE( packedInput.cullFace, true, false );
				#endif
				#endif
				half isFrontFace = input.isFrontFace;

				PositionInputs posInput = GetPositionInput(input.positionSS.xy, _ScreenSize.zw, input.positionSS.z, input.positionSS.w, input.positionRWS);

				float3 V = GetWorldSpaceNormalizeViewDir(input.positionRWS);

				AlphaSurfaceDescription surfaceDescription = (AlphaSurfaceDescription)0;
				float3 ase_worldPos = GetAbsolutePositionWS( positionRWS );
				float2 appendResult257 = (float2(ase_worldPos.x , ase_worldPos.z));
				float2 appendResult260 = (float2(_WorldBounds.x , _WorldBounds.y));
				float2 appendResult259 = (float2(_WorldBounds.z , _WorldBounds.w));
				float2 temp_output_264_0 = ( ( appendResult257 - appendResult260 ) / ( appendResult259 - appendResult260 ) );
				float2 appendResult265 = (float2(_Control_TexelSize.z , _Control_TexelSize.w));
				float2 temp_output_266_0 = ( temp_output_264_0 * appendResult265 );
				float2 temp_output_270_0 = floor( ( ( temp_output_266_0 + float2( 0.5,0.5 ) ) - float2( 0.5,0.5 ) ) );
				float2 temp_output_12_0_g423 = ( ( temp_output_270_0 + float2( 0,0 ) ) / appendResult265 );
				float4 tex2DNode14_g423 = tex2D( _Tint, temp_output_12_0_g423 );
				float4 tex2DNode16_g423 = tex2D( _GrassTint, temp_output_12_0_g423 );
				float2 uv_ShapeMap = packedInput.ase_texcoord1.xy;
				float4 tex2DNode15_g423 = tex2D( _GrassControl, temp_output_12_0_g423 );
				float4 tex2DArrayNode35_g423 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode15_g423.g * 255.0 ) );
				float temp_output_24_0_g426 = tex2DArrayNode35_g423.b;
				float temp_output_25_0_g426 = tex2DNode16_g423.a;
				float4 tex2DNode6_g423 = tex2D( _Control, temp_output_12_0_g423 );
				float4 tex2DArrayNode32_g423 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode6_g423.g * 255.0 ) );
				float temp_output_26_0_g426 = tex2DArrayNode32_g423.b;
				float temp_output_41_0_g423 = ( 1.0 - tex2DNode16_g423.a );
				float temp_output_27_0_g426 = temp_output_41_0_g423;
				float temp_output_1_0_g426 = ( max( ( temp_output_24_0_g426 + temp_output_25_0_g426 ) , ( temp_output_26_0_g426 + temp_output_27_0_g426 ) ) - 0.2 );
				float temp_output_21_0_g426 = max( ( ( temp_output_24_0_g426 + temp_output_25_0_g426 ) - temp_output_1_0_g426 ) , 0.0 );
				float temp_output_19_0_g426 = max( ( ( temp_output_26_0_g426 + temp_output_27_0_g426 ) - temp_output_1_0_g426 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch30_g423 = ( ( ( tex2DNode16_g423 * temp_output_21_0_g426 ) + ( tex2DNode14_g423 * temp_output_19_0_g426 ) ) / ( temp_output_21_0_g426 + temp_output_19_0_g426 ) );
				#else
				float4 staticSwitch30_g423 = tex2DNode14_g423;
				#endif
				float temp_output_24_0_g424 = tex2DArrayNode35_g423.b;
				float temp_output_25_0_g424 = tex2DNode16_g423.a;
				float temp_output_26_0_g424 = tex2DArrayNode32_g423.b;
				float temp_output_27_0_g424 = temp_output_41_0_g423;
				float temp_output_1_0_g424 = ( max( ( temp_output_24_0_g424 + temp_output_25_0_g424 ) , ( temp_output_26_0_g424 + temp_output_27_0_g424 ) ) - 0.2 );
				float temp_output_21_0_g424 = max( ( ( temp_output_24_0_g424 + temp_output_25_0_g424 ) - temp_output_1_0_g424 ) , 0.0 );
				float temp_output_19_0_g424 = max( ( ( temp_output_26_0_g424 + temp_output_27_0_g424 ) - temp_output_1_0_g424 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch31_g423 = ( ( ( tex2DArrayNode35_g423 * temp_output_21_0_g424 ) + ( tex2DArrayNode32_g423 * temp_output_19_0_g424 ) ) / ( temp_output_21_0_g424 + temp_output_19_0_g424 ) );
				#else
				float4 staticSwitch31_g423 = tex2DArrayNode32_g423;
				#endif
				float temp_output_302_3 = staticSwitch31_g423.b;
				float temp_output_24_0_g437 = temp_output_302_3;
				float2 break273 = ( temp_output_266_0 - temp_output_270_0 );
				float temp_output_274_0 = ( 1.0 - break273.x );
				float temp_output_25_0_g437 = temp_output_274_0;
				float2 temp_output_12_0_g419 = ( ( temp_output_270_0 + float2( 1,0 ) ) / appendResult265 );
				float4 tex2DNode6_g419 = tex2D( _Control, temp_output_12_0_g419 );
				float4 tex2DArrayNode32_g419 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode6_g419.g * 255.0 ) );
				float4 tex2DNode15_g419 = tex2D( _GrassControl, temp_output_12_0_g419 );
				float4 tex2DArrayNode35_g419 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode15_g419.g * 255.0 ) );
				float temp_output_24_0_g420 = tex2DArrayNode35_g419.b;
				float4 tex2DNode16_g419 = tex2D( _GrassTint, temp_output_12_0_g419 );
				float temp_output_25_0_g420 = tex2DNode16_g419.a;
				float temp_output_26_0_g420 = tex2DArrayNode32_g419.b;
				float temp_output_41_0_g419 = ( 1.0 - tex2DNode16_g419.a );
				float temp_output_27_0_g420 = temp_output_41_0_g419;
				float temp_output_1_0_g420 = ( max( ( temp_output_24_0_g420 + temp_output_25_0_g420 ) , ( temp_output_26_0_g420 + temp_output_27_0_g420 ) ) - 0.2 );
				float temp_output_21_0_g420 = max( ( ( temp_output_24_0_g420 + temp_output_25_0_g420 ) - temp_output_1_0_g420 ) , 0.0 );
				float temp_output_19_0_g420 = max( ( ( temp_output_26_0_g420 + temp_output_27_0_g420 ) - temp_output_1_0_g420 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch31_g419 = ( ( ( tex2DArrayNode35_g419 * temp_output_21_0_g420 ) + ( tex2DArrayNode32_g419 * temp_output_19_0_g420 ) ) / ( temp_output_21_0_g420 + temp_output_19_0_g420 ) );
				#else
				float4 staticSwitch31_g419 = tex2DArrayNode32_g419;
				#endif
				float temp_output_299_3 = staticSwitch31_g419.b;
				float temp_output_26_0_g437 = temp_output_299_3;
				float temp_output_27_0_g437 = break273.x;
				float temp_output_1_0_g437 = ( max( ( temp_output_24_0_g437 + temp_output_25_0_g437 ) , ( temp_output_26_0_g437 + temp_output_27_0_g437 ) ) - 0.2 );
				float temp_output_21_0_g437 = max( ( ( temp_output_24_0_g437 + temp_output_25_0_g437 ) - temp_output_1_0_g437 ) , 0.0 );
				float4 tex2DNode14_g419 = tex2D( _Tint, temp_output_12_0_g419 );
				float temp_output_24_0_g422 = tex2DArrayNode35_g419.b;
				float temp_output_25_0_g422 = tex2DNode16_g419.a;
				float temp_output_26_0_g422 = tex2DArrayNode32_g419.b;
				float temp_output_27_0_g422 = temp_output_41_0_g419;
				float temp_output_1_0_g422 = ( max( ( temp_output_24_0_g422 + temp_output_25_0_g422 ) , ( temp_output_26_0_g422 + temp_output_27_0_g422 ) ) - 0.2 );
				float temp_output_21_0_g422 = max( ( ( temp_output_24_0_g422 + temp_output_25_0_g422 ) - temp_output_1_0_g422 ) , 0.0 );
				float temp_output_19_0_g422 = max( ( ( temp_output_26_0_g422 + temp_output_27_0_g422 ) - temp_output_1_0_g422 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch30_g419 = ( ( ( tex2DNode16_g419 * temp_output_21_0_g422 ) + ( tex2DNode14_g419 * temp_output_19_0_g422 ) ) / ( temp_output_21_0_g422 + temp_output_19_0_g422 ) );
				#else
				float4 staticSwitch30_g419 = tex2DNode14_g419;
				#endif
				float temp_output_19_0_g437 = max( ( ( temp_output_26_0_g437 + temp_output_27_0_g437 ) - temp_output_1_0_g437 ) , 0.0 );
				float temp_output_24_0_g436 = temp_output_302_3;
				float temp_output_25_0_g436 = temp_output_274_0;
				float temp_output_26_0_g436 = temp_output_299_3;
				float temp_output_27_0_g436 = break273.x;
				float temp_output_1_0_g436 = ( max( ( temp_output_24_0_g436 + temp_output_25_0_g436 ) , ( temp_output_26_0_g436 + temp_output_27_0_g436 ) ) - 0.2 );
				float temp_output_21_0_g436 = max( ( ( temp_output_24_0_g436 + temp_output_25_0_g436 ) - temp_output_1_0_g436 ) , 0.0 );
				float temp_output_19_0_g436 = max( ( ( temp_output_26_0_g436 + temp_output_27_0_g436 ) - temp_output_1_0_g436 ) , 0.0 );
				float4 temp_output_290_0 = ( ( ( staticSwitch31_g423 * temp_output_21_0_g436 ) + ( staticSwitch31_g419 * temp_output_19_0_g436 ) ) / ( temp_output_21_0_g436 + temp_output_19_0_g436 ) );
				float temp_output_24_0_g439 = temp_output_290_0.z;
				float temp_output_275_0 = ( 1.0 - break273.y );
				float temp_output_25_0_g439 = temp_output_275_0;
				float2 temp_output_12_0_g431 = ( ( temp_output_270_0 + float2( 0,1 ) ) / appendResult265 );
				float4 tex2DNode6_g431 = tex2D( _Control, temp_output_12_0_g431 );
				float4 tex2DArrayNode32_g431 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode6_g431.g * 255.0 ) );
				float4 tex2DNode15_g431 = tex2D( _GrassControl, temp_output_12_0_g431 );
				float4 tex2DArrayNode35_g431 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode15_g431.g * 255.0 ) );
				float temp_output_24_0_g432 = tex2DArrayNode35_g431.b;
				float4 tex2DNode16_g431 = tex2D( _GrassTint, temp_output_12_0_g431 );
				float temp_output_25_0_g432 = tex2DNode16_g431.a;
				float temp_output_26_0_g432 = tex2DArrayNode32_g431.b;
				float temp_output_41_0_g431 = ( 1.0 - tex2DNode16_g431.a );
				float temp_output_27_0_g432 = temp_output_41_0_g431;
				float temp_output_1_0_g432 = ( max( ( temp_output_24_0_g432 + temp_output_25_0_g432 ) , ( temp_output_26_0_g432 + temp_output_27_0_g432 ) ) - 0.2 );
				float temp_output_21_0_g432 = max( ( ( temp_output_24_0_g432 + temp_output_25_0_g432 ) - temp_output_1_0_g432 ) , 0.0 );
				float temp_output_19_0_g432 = max( ( ( temp_output_26_0_g432 + temp_output_27_0_g432 ) - temp_output_1_0_g432 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch31_g431 = ( ( ( tex2DArrayNode35_g431 * temp_output_21_0_g432 ) + ( tex2DArrayNode32_g431 * temp_output_19_0_g432 ) ) / ( temp_output_21_0_g432 + temp_output_19_0_g432 ) );
				#else
				float4 staticSwitch31_g431 = tex2DArrayNode32_g431;
				#endif
				float temp_output_300_3 = staticSwitch31_g431.b;
				float temp_output_24_0_g435 = temp_output_300_3;
				float temp_output_25_0_g435 = temp_output_274_0;
				float2 temp_output_12_0_g427 = ( ( temp_output_270_0 + float2( 1,1 ) ) / appendResult265 );
				float4 tex2DNode6_g427 = tex2D( _Control, temp_output_12_0_g427 );
				float4 tex2DArrayNode32_g427 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode6_g427.g * 255.0 ) );
				float4 tex2DNode15_g427 = tex2D( _GrassControl, temp_output_12_0_g427 );
				float4 tex2DArrayNode35_g427 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode15_g427.g * 255.0 ) );
				float temp_output_24_0_g428 = tex2DArrayNode35_g427.b;
				float4 tex2DNode16_g427 = tex2D( _GrassTint, temp_output_12_0_g427 );
				float temp_output_25_0_g428 = tex2DNode16_g427.a;
				float temp_output_26_0_g428 = tex2DArrayNode32_g427.b;
				float temp_output_41_0_g427 = ( 1.0 - tex2DNode16_g427.a );
				float temp_output_27_0_g428 = temp_output_41_0_g427;
				float temp_output_1_0_g428 = ( max( ( temp_output_24_0_g428 + temp_output_25_0_g428 ) , ( temp_output_26_0_g428 + temp_output_27_0_g428 ) ) - 0.2 );
				float temp_output_21_0_g428 = max( ( ( temp_output_24_0_g428 + temp_output_25_0_g428 ) - temp_output_1_0_g428 ) , 0.0 );
				float temp_output_19_0_g428 = max( ( ( temp_output_26_0_g428 + temp_output_27_0_g428 ) - temp_output_1_0_g428 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch31_g427 = ( ( ( tex2DArrayNode35_g427 * temp_output_21_0_g428 ) + ( tex2DArrayNode32_g427 * temp_output_19_0_g428 ) ) / ( temp_output_21_0_g428 + temp_output_19_0_g428 ) );
				#else
				float4 staticSwitch31_g427 = tex2DArrayNode32_g427;
				#endif
				float temp_output_301_3 = staticSwitch31_g427.b;
				float temp_output_26_0_g435 = temp_output_301_3;
				float temp_output_27_0_g435 = break273.x;
				float temp_output_1_0_g435 = ( max( ( temp_output_24_0_g435 + temp_output_25_0_g435 ) , ( temp_output_26_0_g435 + temp_output_27_0_g435 ) ) - 0.2 );
				float temp_output_21_0_g435 = max( ( ( temp_output_24_0_g435 + temp_output_25_0_g435 ) - temp_output_1_0_g435 ) , 0.0 );
				float temp_output_19_0_g435 = max( ( ( temp_output_26_0_g435 + temp_output_27_0_g435 ) - temp_output_1_0_g435 ) , 0.0 );
				float4 temp_output_293_0 = ( ( ( staticSwitch31_g431 * temp_output_21_0_g435 ) + ( staticSwitch31_g427 * temp_output_19_0_g435 ) ) / ( temp_output_21_0_g435 + temp_output_19_0_g435 ) );
				float temp_output_26_0_g439 = temp_output_293_0.z;
				float temp_output_27_0_g439 = break273.y;
				float temp_output_1_0_g439 = ( max( ( temp_output_24_0_g439 + temp_output_25_0_g439 ) , ( temp_output_26_0_g439 + temp_output_27_0_g439 ) ) - 0.2 );
				float temp_output_21_0_g439 = max( ( ( temp_output_24_0_g439 + temp_output_25_0_g439 ) - temp_output_1_0_g439 ) , 0.0 );
				float4 tex2DNode14_g431 = tex2D( _Tint, temp_output_12_0_g431 );
				float temp_output_24_0_g434 = tex2DArrayNode35_g431.b;
				float temp_output_25_0_g434 = tex2DNode16_g431.a;
				float temp_output_26_0_g434 = tex2DArrayNode32_g431.b;
				float temp_output_27_0_g434 = temp_output_41_0_g431;
				float temp_output_1_0_g434 = ( max( ( temp_output_24_0_g434 + temp_output_25_0_g434 ) , ( temp_output_26_0_g434 + temp_output_27_0_g434 ) ) - 0.2 );
				float temp_output_21_0_g434 = max( ( ( temp_output_24_0_g434 + temp_output_25_0_g434 ) - temp_output_1_0_g434 ) , 0.0 );
				float temp_output_19_0_g434 = max( ( ( temp_output_26_0_g434 + temp_output_27_0_g434 ) - temp_output_1_0_g434 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch30_g431 = ( ( ( tex2DNode16_g431 * temp_output_21_0_g434 ) + ( tex2DNode14_g431 * temp_output_19_0_g434 ) ) / ( temp_output_21_0_g434 + temp_output_19_0_g434 ) );
				#else
				float4 staticSwitch30_g431 = tex2DNode14_g431;
				#endif
				float temp_output_24_0_g438 = temp_output_300_3;
				float temp_output_25_0_g438 = temp_output_274_0;
				float temp_output_26_0_g438 = temp_output_301_3;
				float temp_output_27_0_g438 = break273.x;
				float temp_output_1_0_g438 = ( max( ( temp_output_24_0_g438 + temp_output_25_0_g438 ) , ( temp_output_26_0_g438 + temp_output_27_0_g438 ) ) - 0.2 );
				float temp_output_21_0_g438 = max( ( ( temp_output_24_0_g438 + temp_output_25_0_g438 ) - temp_output_1_0_g438 ) , 0.0 );
				float4 tex2DNode14_g427 = tex2D( _Tint, temp_output_12_0_g427 );
				float temp_output_24_0_g430 = tex2DArrayNode35_g427.b;
				float temp_output_25_0_g430 = tex2DNode16_g427.a;
				float temp_output_26_0_g430 = tex2DArrayNode32_g427.b;
				float temp_output_27_0_g430 = temp_output_41_0_g427;
				float temp_output_1_0_g430 = ( max( ( temp_output_24_0_g430 + temp_output_25_0_g430 ) , ( temp_output_26_0_g430 + temp_output_27_0_g430 ) ) - 0.2 );
				float temp_output_21_0_g430 = max( ( ( temp_output_24_0_g430 + temp_output_25_0_g430 ) - temp_output_1_0_g430 ) , 0.0 );
				float temp_output_19_0_g430 = max( ( ( temp_output_26_0_g430 + temp_output_27_0_g430 ) - temp_output_1_0_g430 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch30_g427 = ( ( ( tex2DNode16_g427 * temp_output_21_0_g430 ) + ( tex2DNode14_g427 * temp_output_19_0_g430 ) ) / ( temp_output_21_0_g430 + temp_output_19_0_g430 ) );
				#else
				float4 staticSwitch30_g427 = tex2DNode14_g427;
				#endif
				float temp_output_19_0_g438 = max( ( ( temp_output_26_0_g438 + temp_output_27_0_g438 ) - temp_output_1_0_g438 ) , 0.0 );
				float temp_output_19_0_g439 = max( ( ( temp_output_26_0_g439 + temp_output_27_0_g439 ) - temp_output_1_0_g439 ) , 0.0 );
				float4 temp_output_221_0 = ( ( ( ( ( ( staticSwitch30_g423 * temp_output_21_0_g437 ) + ( staticSwitch30_g419 * temp_output_19_0_g437 ) ) / ( temp_output_21_0_g437 + temp_output_19_0_g437 ) ) * temp_output_21_0_g439 ) + ( ( ( ( staticSwitch30_g431 * temp_output_21_0_g438 ) + ( staticSwitch30_g427 * temp_output_19_0_g438 ) ) / ( temp_output_21_0_g438 + temp_output_19_0_g438 ) ) * temp_output_19_0_g439 ) ) / ( temp_output_21_0_g439 + temp_output_19_0_g439 ) );
				float temp_output_3_0_g440 = ( temp_output_221_0.w * 2.0 );
				
				surfaceDescription.Alpha = min( temp_output_3_0_g440 , 1.0 );

				#ifdef _ALPHATEST_ON
				surfaceDescription.AlphaClipThreshold = _AlphaCutoff;
				#endif

				#ifdef _ALPHATEST_SHADOW_ON
				surfaceDescription.AlphaClipThresholdShadow = 0.5;
				#endif

				#ifdef _DEPTHOFFSET_ON
				surfaceDescription.DepthOffset = 0;
				#endif

				SurfaceData surfaceData;
				BuiltinData builtinData;
				GetSurfaceAndBuiltinData(surfaceDescription, input, V, posInput, surfaceData, builtinData);

				#ifdef _DEPTHOFFSET_ON
				outputDepth = posInput.deviceDepth;
				float bias = max(abs(ddx(posInput.deviceDepth)), abs(ddy(posInput.deviceDepth))) * _SlopeScaleDepthBias;
				outputDepth += bias;
				#endif

				#ifdef WRITE_MSAA_DEPTH
				depthColor = packedInput.vmesh.positionCS.z;

				#ifdef _ALPHATOMASK_ON
				depthColor.a = SharpenAlpha(builtinData.opacity, builtinData.alphaClipTreshold);
				#endif
				#endif

				#if defined(WRITE_NORMAL_BUFFER)
				EncodeIntoNormalBuffer(ConvertSurfaceDataToNormalData(surfaceData), outNormalBuffer);
				#endif

				#if defined(WRITE_DECAL_BUFFER) && !defined(_DISABLE_DECALS)
				DecalPrepassData decalPrepassData;
				decalPrepassData.geomNormalWS = surfaceData.geomNormalWS;
				decalPrepassData.decalLayerMask = GetMeshRenderingDecalLayer();
				EncodeIntoDecalPrepassBuffer(decalPrepassData, outDecalBuffer);
				#endif
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "SceneSelectionPass"
			Tags { "LightMode"="SceneSelectionPass" }
			Cull Off

			HLSLPROGRAM

			#define _SPECULAR_OCCLUSION_FROM_AO 1
			#define _DISABLE_DECALS 1
			#define _DISABLE_SSR 1
			#define _AMBIENT_OCCLUSION 1
			#define ASE_SRP_VERSION 999999


			#pragma shader_feature _SURFACE_TYPE_TRANSPARENT
			#pragma shader_feature_local _DOUBLESIDED_ON
			#pragma shader_feature_local _BLENDMODE_OFF _BLENDMODE_ALPHA _BLENDMODE_ADD _BLENDMODE_PRE_MULTIPLY
			#pragma shader_feature_local_fragment _ _ENABLE_FOG_ON_TRANSPARENT
			#pragma shader_feature_local _ALPHATEST_ON

			#define SHADERPASS SHADERPASS_DEPTH_ONLY
			#define SCENESELECTIONPASS 1

			#pragma editor_sync_compilation

			#pragma vertex Vert
			#pragma fragment Frag
			
			#pragma shader_feature_local _ _TRANSPARENT_WRITES_MOTION_VEC

			//#define UNITY_MATERIAL_LIT

			#if defined(_MATERIAL_FEATURE_SUBSURFACE_SCATTERING) && !defined(_SURFACE_TYPE_TRANSPARENT)
			#define OUTPUT_SPLIT_LIGHTING
			#endif

            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
        	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/GeometricTools.hlsl" 
        	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Tessellation.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/FragInputs.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl" 
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderGraphHeader.hlsl" 

			#ifdef DEBUG_DISPLAY
				#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Debug/DebugDisplay.hlsl"
			#endif

			#if defined(SHADER_LIT) && !defined(_SURFACE_TYPE_TRANSPARENT)
                #define _DEFERRED_CAPABLE_MATERIAL
            #endif
        
            
            #if defined(_TRANSPARENT_WRITES_MOTION_VEC) && defined(_SURFACE_TYPE_TRANSPARENT)
                #define _WRITE_TRANSPARENT_MOTION_VECTOR
            #endif
        

			CBUFFER_START( UnityPerMaterial )
			float4 _MatTexArray_ST;
			float4 _WorldBounds;
			float4 _Control_TexelSize;
			float4 _ShapeMap_ST;
			float _NormalScale;
			float _MetalLevel;
			float _Rough;
			float4 _EmissionColor;
			float _AlphaCutoff;
			float _RenderQueueType;
			#ifdef _ADD_PRECOMPUTED_VELOCITY
			float _AddPrecomputedVelocity;
			#endif
			float _StencilRef;
			float _StencilWriteMask;
			float _StencilRefDepth;
			float _StencilWriteMaskDepth;
			float _StencilRefMV;
			float _StencilWriteMaskMV;
			float _StencilRefDistortionVec;
			float _StencilWriteMaskDistortionVec;
			float _StencilWriteMaskGBuffer;
			float _StencilRefGBuffer;
			float _ZTestGBuffer;
			float _RequireSplitLighting;
			float _ReceivesSSR;
			float _SurfaceType;
			float _BlendMode;
			float _SrcBlend;
			float _DstBlend;
			float _AlphaSrcBlend;
			float _AlphaDstBlend;
			float _ZWrite;
			float _TransparentZWrite;
			float _CullMode;
			float _TransparentSortPriority;
			float _EnableFogOnTransparent;
			float _CullModeForward;
			float _TransparentCullMode;
			float _ZTestDepthEqualForOpaque;
			float _ZTestTransparent;
			float _TransparentBackfaceEnable;
			float _AlphaCutoffEnable;
			float _UseShadowThreshold;
			float _DoubleSidedEnable;
			float _DoubleSidedNormalMode;
			float4 _DoubleSidedConstants;
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END

			sampler2D _Tint;
			sampler2D _Control;
			sampler2D _GrassTint;
			TEXTURE2D_ARRAY(_ShapeMap);
			sampler2D _GrassControl;
			SAMPLER(sampler_ShapeMap);


			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/PickingSpaceTransforms.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/NormalSurfaceGradient.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Lit/Lit.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/BuiltinUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/MaterialUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Decal/DecalUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Lit/LitDecalData.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderGraphFunctions.hlsl"

			#define ASE_NEEDS_FRAG_RELATIVE_WORLD_POS
			#define GRASS


			#if defined(_DOUBLESIDED_ON) && !defined(ASE_NEED_CULLFACE)
				#define ASE_NEED_CULLFACE 1
			#endif

			struct AttributesMesh
			{
				float3 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryingsMeshToPS
			{
				float4 positionCS : SV_Position;
				float3 positionRWS : TEXCOORD0;
				float4 ase_texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
				#if defined(SHADER_STAGE_FRAGMENT) && defined(ASE_NEED_CULLFACE)
				FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
				#endif
			};

			int _ObjectId;
			int _PassValue;

			
			void BuildSurfaceData(FragInputs fragInputs, inout SceneSurfaceDescription surfaceDescription, float3 V, PositionInputs posInput, out SurfaceData surfaceData, out float3 bentNormalWS)
			{
				ZERO_INITIALIZE(SurfaceData, surfaceData);

				surfaceData.specularOcclusion = 1.0;

				// surface data

				// refraction
				#ifdef _HAS_REFRACTION
				if( _EnableSSRefraction )
				{
					surfaceData.transmittanceMask = ( 1.0 - surfaceDescription.Alpha );
					surfaceDescription.Alpha = 1.0;
				}
				else
				{
					surfaceData.ior = 1.0;
					surfaceData.transmittanceColor = float3( 1.0, 1.0, 1.0 );
					surfaceData.atDistance = 1.0;
					surfaceData.transmittanceMask = 0.0;
					surfaceDescription.Alpha = 1.0;
				}
				#else
				surfaceData.ior = 1.0;
				surfaceData.transmittanceColor = float3( 1.0, 1.0, 1.0 );
				surfaceData.atDistance = 1.0;
				surfaceData.transmittanceMask = 0.0;
				#endif


				// material features
				surfaceData.materialFeatures = MATERIALFEATUREFLAGS_LIT_STANDARD;
				#ifdef _MATERIAL_FEATURE_SUBSURFACE_SCATTERING
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_SUBSURFACE_SCATTERING;
				#endif
				#ifdef _MATERIAL_FEATURE_TRANSMISSION
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_TRANSMISSION;
				#endif
				#ifdef _MATERIAL_FEATURE_ANISOTROPY
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_ANISOTROPY;
				#endif
				#ifdef ASE_LIT_CLEAR_COAT
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_CLEAR_COAT;
				#endif
				#ifdef _MATERIAL_FEATURE_IRIDESCENCE
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_IRIDESCENCE;
				#endif
				#ifdef _MATERIAL_FEATURE_SPECULAR_COLOR
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_SPECULAR_COLOR;
				#endif

				// others
				#if defined (_MATERIAL_FEATURE_SPECULAR_COLOR) && defined (_ENERGY_CONSERVING_SPECULAR)
				surfaceData.baseColor *= ( 1.0 - Max3( surfaceData.specularColor.r, surfaceData.specularColor.g, surfaceData.specularColor.b ) );
				#endif
				#ifdef _DOUBLESIDED_ON
				float3 doubleSidedConstants = _DoubleSidedConstants.xyz;
				#else
				float3 doubleSidedConstants = float3( 1.0, 1.0, 1.0 );
				#endif

				// normals
				float3 normalTS = float3(0.0f, 0.0f, 1.0f);
				GetNormalWS( fragInputs, normalTS, surfaceData.normalWS, doubleSidedConstants );

				surfaceData.geomNormalWS = fragInputs.tangentToWorld[2];
				surfaceData.tangentWS = normalize( fragInputs.tangentToWorld[ 0 ].xyz );

				// decals
				#if HAVE_DECALS
				if( _EnableDecals )
				{
					DecalSurfaceData decalSurfaceData = GetDecalSurfaceData(posInput, fragInputs.tangentToWorld[2], surfaceDescription.Alpha);
					ApplyDecalToSurfaceData(decalSurfaceData, fragInputs.tangentToWorld[2], surfaceData);
				}
				#endif

				bentNormalWS = surfaceData.normalWS;
				surfaceData.tangentWS = Orthonormalize( surfaceData.tangentWS, surfaceData.normalWS );

				#if defined(_SPECULAR_OCCLUSION_CUSTOM)
				#elif defined(_SPECULAR_OCCLUSION_FROM_AO_BENT_NORMAL)
				surfaceData.specularOcclusion = GetSpecularOcclusionFromBentAO( V, bentNormalWS, surfaceData.normalWS, surfaceData.ambientOcclusion, PerceptualSmoothnessToPerceptualRoughness( surfaceData.perceptualSmoothness ) );
				#elif defined(_AMBIENT_OCCLUSION) && defined(_SPECULAR_OCCLUSION_FROM_AO)
				surfaceData.specularOcclusion = GetSpecularOcclusionFromAmbientOcclusion( ClampNdotV( dot( surfaceData.normalWS, V ) ), surfaceData.ambientOcclusion, PerceptualSmoothnessToRoughness( surfaceData.perceptualSmoothness ) );
				#endif

				// debug
				#if defined(DEBUG_DISPLAY)
				if (_DebugMipMapMode != DEBUGMIPMAPMODE_NONE)
				{
					surfaceData.metallic = 0;
				}
				ApplyDebugToSurfaceData(fragInputs.tangentToWorld, surfaceData);
				#endif
			}

			void GetSurfaceAndBuiltinData(SceneSurfaceDescription surfaceDescription, FragInputs fragInputs, float3 V, inout PositionInputs posInput, out SurfaceData surfaceData, out BuiltinData builtinData)
			{
				#ifdef LOD_FADE_CROSSFADE
				LODDitheringTransition(ComputeFadeMaskSeed(V, posInput.positionSS), unity_LODFade.x);
				#endif

				#ifdef _DOUBLESIDED_ON
				float3 doubleSidedConstants = _DoubleSidedConstants.xyz;
				#else
				float3 doubleSidedConstants = float3( 1.0, 1.0, 1.0 );
				#endif

				ApplyDoubleSidedFlipOrMirror( fragInputs, doubleSidedConstants );

				#ifdef _ALPHATEST_ON
				DoAlphaTest( surfaceDescription.Alpha, surfaceDescription.AlphaClipThreshold );
				#endif

				#ifdef _DEPTHOFFSET_ON
				builtinData.depthOffset = surfaceDescription.DepthOffset;
				ApplyDepthOffsetPositionInput( V, surfaceDescription.DepthOffset, GetViewForwardDir(), GetWorldToHClipMatrix(), posInput );
				#endif

				float3 bentNormalWS;
				BuildSurfaceData( fragInputs, surfaceDescription, V, posInput, surfaceData, bentNormalWS );

				InitBuiltinData( posInput, surfaceDescription.Alpha, bentNormalWS, -fragInputs.tangentToWorld[ 2 ], fragInputs.texCoord1, fragInputs.texCoord2, builtinData );

				PostInitBuiltinData(V, posInput, surfaceData, builtinData);
			}

			PackedVaryingsMeshToPS VertexFunction(AttributesMesh inputMesh )
			{
				PackedVaryingsMeshToPS outputPackedVaryingsMeshToPS;
				UNITY_SETUP_INSTANCE_ID(inputMesh);
				UNITY_TRANSFER_INSTANCE_ID(inputMesh, outputPackedVaryingsMeshToPS);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( outputPackedVaryingsMeshToPS );

				outputPackedVaryingsMeshToPS.ase_texcoord1.xy = inputMesh.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				outputPackedVaryingsMeshToPS.ase_texcoord1.zw = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
				float3 defaultVertexValue = inputMesh.positionOS.xyz;
				#else
				float3 defaultVertexValue = float3( 0, 0, 0 );
				#endif
				float3 vertexValue =  defaultVertexValue ;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
				inputMesh.positionOS.xyz = vertexValue;
				#else
				inputMesh.positionOS.xyz += vertexValue;
				#endif

				inputMesh.normalOS =  inputMesh.normalOS ;

				float3 positionRWS = TransformObjectToWorld(inputMesh.positionOS);
				outputPackedVaryingsMeshToPS.positionCS = TransformWorldToHClip(positionRWS);
				outputPackedVaryingsMeshToPS.positionRWS.xyz = positionRWS;
				return outputPackedVaryingsMeshToPS;
			}

			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float3 positionOS : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl Vert ( AttributesMesh v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.positionOS = v.positionOS;
				o.normalOS = v.normalOS;
				o.ase_texcoord = v.ase_texcoord;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if (SHADEROPTIONS_CAMERA_RELATIVE_RENDERING != 0)
				float3 cameraPos = 0;
				#else
				float3 cameraPos = _WorldSpaceCameraPos;
				#endif
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(float4(v[0].positionOS,1), float4(v[1].positionOS,1), float4(v[2].positionOS,1), tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), cameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(float4(v[0].positionOS,1), float4(v[1].positionOS,1), float4(v[2].positionOS,1), edgeLength, GetObjectToWorldMatrix(), cameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(float4(v[0].positionOS,1), float4(v[1].positionOS,1), float4(v[2].positionOS,1), edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), cameraPos, _ScreenParams, _FrustumPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			PackedVaryingsMeshToPS DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				AttributesMesh o = (AttributesMesh) 0;
				o.positionOS = patch[0].positionOS * bary.x + patch[1].positionOS * bary.y + patch[2].positionOS * bary.z;
				o.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.positionOS.xyz - patch[i].normalOS * (dot(o.positionOS.xyz, patch[i].normalOS) - dot(patch[i].positionOS.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				o.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			PackedVaryingsMeshToPS Vert ( AttributesMesh v )
			{
				return VertexFunction( v );
			}
			#endif
			
			#if defined(WRITE_NORMAL_BUFFER) && defined(WRITE_MSAA_DEPTH)
			#define SV_TARGET_DECAL SV_Target2
			#elif defined(WRITE_NORMAL_BUFFER) || defined(WRITE_MSAA_DEPTH)
			#define SV_TARGET_DECAL SV_Target1
			#else
			#define SV_TARGET_DECAL SV_Target0
			#endif

			void Frag( PackedVaryingsMeshToPS packedInput
						, out float4 outColor : SV_Target0
						#if defined(_DEPTHOFFSET_ON) && !defined(SCENEPICKINGPASS)
						, out float outputDepth : SV_Depth
						#endif
						
					)
			{
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( packedInput );
				UNITY_SETUP_INSTANCE_ID( packedInput );

				float3 positionRWS = packedInput.positionRWS.xyz;

				FragInputs input;
				ZERO_INITIALIZE(FragInputs, input);

				input.tangentToWorld = k_identity3x3;
				input.positionSS = packedInput.positionCS;

				input.positionRWS = positionRWS;

				#if _DOUBLESIDED_ON && SHADER_STAGE_FRAGMENT
				input.isFrontFace = IS_FRONT_VFACE( packedInput.cullFace, true, false);
				#elif SHADER_STAGE_FRAGMENT
				#if defined(ASE_NEED_CULLFACE)
				input.isFrontFace = IS_FRONT_VFACE( packedInput.cullFace, true, false );
				#endif
				#endif
				half isFrontFace = input.isFrontFace;

				PositionInputs posInput = GetPositionInput(input.positionSS.xy, _ScreenSize.zw, input.positionSS.z, input.positionSS.w, input.positionRWS);

				float3 V = GetWorldSpaceNormalizeViewDir(input.positionRWS);

				SceneSurfaceDescription surfaceDescription = (SceneSurfaceDescription)0;
				float3 ase_worldPos = GetAbsolutePositionWS( positionRWS );
				float2 appendResult257 = (float2(ase_worldPos.x , ase_worldPos.z));
				float2 appendResult260 = (float2(_WorldBounds.x , _WorldBounds.y));
				float2 appendResult259 = (float2(_WorldBounds.z , _WorldBounds.w));
				float2 temp_output_264_0 = ( ( appendResult257 - appendResult260 ) / ( appendResult259 - appendResult260 ) );
				float2 appendResult265 = (float2(_Control_TexelSize.z , _Control_TexelSize.w));
				float2 temp_output_266_0 = ( temp_output_264_0 * appendResult265 );
				float2 temp_output_270_0 = floor( ( ( temp_output_266_0 + float2( 0.5,0.5 ) ) - float2( 0.5,0.5 ) ) );
				float2 temp_output_12_0_g423 = ( ( temp_output_270_0 + float2( 0,0 ) ) / appendResult265 );
				float4 tex2DNode14_g423 = tex2D( _Tint, temp_output_12_0_g423 );
				float4 tex2DNode16_g423 = tex2D( _GrassTint, temp_output_12_0_g423 );
				float2 uv_ShapeMap = packedInput.ase_texcoord1.xy;
				float4 tex2DNode15_g423 = tex2D( _GrassControl, temp_output_12_0_g423 );
				float4 tex2DArrayNode35_g423 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode15_g423.g * 255.0 ) );
				float temp_output_24_0_g426 = tex2DArrayNode35_g423.b;
				float temp_output_25_0_g426 = tex2DNode16_g423.a;
				float4 tex2DNode6_g423 = tex2D( _Control, temp_output_12_0_g423 );
				float4 tex2DArrayNode32_g423 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode6_g423.g * 255.0 ) );
				float temp_output_26_0_g426 = tex2DArrayNode32_g423.b;
				float temp_output_41_0_g423 = ( 1.0 - tex2DNode16_g423.a );
				float temp_output_27_0_g426 = temp_output_41_0_g423;
				float temp_output_1_0_g426 = ( max( ( temp_output_24_0_g426 + temp_output_25_0_g426 ) , ( temp_output_26_0_g426 + temp_output_27_0_g426 ) ) - 0.2 );
				float temp_output_21_0_g426 = max( ( ( temp_output_24_0_g426 + temp_output_25_0_g426 ) - temp_output_1_0_g426 ) , 0.0 );
				float temp_output_19_0_g426 = max( ( ( temp_output_26_0_g426 + temp_output_27_0_g426 ) - temp_output_1_0_g426 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch30_g423 = ( ( ( tex2DNode16_g423 * temp_output_21_0_g426 ) + ( tex2DNode14_g423 * temp_output_19_0_g426 ) ) / ( temp_output_21_0_g426 + temp_output_19_0_g426 ) );
				#else
				float4 staticSwitch30_g423 = tex2DNode14_g423;
				#endif
				float temp_output_24_0_g424 = tex2DArrayNode35_g423.b;
				float temp_output_25_0_g424 = tex2DNode16_g423.a;
				float temp_output_26_0_g424 = tex2DArrayNode32_g423.b;
				float temp_output_27_0_g424 = temp_output_41_0_g423;
				float temp_output_1_0_g424 = ( max( ( temp_output_24_0_g424 + temp_output_25_0_g424 ) , ( temp_output_26_0_g424 + temp_output_27_0_g424 ) ) - 0.2 );
				float temp_output_21_0_g424 = max( ( ( temp_output_24_0_g424 + temp_output_25_0_g424 ) - temp_output_1_0_g424 ) , 0.0 );
				float temp_output_19_0_g424 = max( ( ( temp_output_26_0_g424 + temp_output_27_0_g424 ) - temp_output_1_0_g424 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch31_g423 = ( ( ( tex2DArrayNode35_g423 * temp_output_21_0_g424 ) + ( tex2DArrayNode32_g423 * temp_output_19_0_g424 ) ) / ( temp_output_21_0_g424 + temp_output_19_0_g424 ) );
				#else
				float4 staticSwitch31_g423 = tex2DArrayNode32_g423;
				#endif
				float temp_output_302_3 = staticSwitch31_g423.b;
				float temp_output_24_0_g437 = temp_output_302_3;
				float2 break273 = ( temp_output_266_0 - temp_output_270_0 );
				float temp_output_274_0 = ( 1.0 - break273.x );
				float temp_output_25_0_g437 = temp_output_274_0;
				float2 temp_output_12_0_g419 = ( ( temp_output_270_0 + float2( 1,0 ) ) / appendResult265 );
				float4 tex2DNode6_g419 = tex2D( _Control, temp_output_12_0_g419 );
				float4 tex2DArrayNode32_g419 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode6_g419.g * 255.0 ) );
				float4 tex2DNode15_g419 = tex2D( _GrassControl, temp_output_12_0_g419 );
				float4 tex2DArrayNode35_g419 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode15_g419.g * 255.0 ) );
				float temp_output_24_0_g420 = tex2DArrayNode35_g419.b;
				float4 tex2DNode16_g419 = tex2D( _GrassTint, temp_output_12_0_g419 );
				float temp_output_25_0_g420 = tex2DNode16_g419.a;
				float temp_output_26_0_g420 = tex2DArrayNode32_g419.b;
				float temp_output_41_0_g419 = ( 1.0 - tex2DNode16_g419.a );
				float temp_output_27_0_g420 = temp_output_41_0_g419;
				float temp_output_1_0_g420 = ( max( ( temp_output_24_0_g420 + temp_output_25_0_g420 ) , ( temp_output_26_0_g420 + temp_output_27_0_g420 ) ) - 0.2 );
				float temp_output_21_0_g420 = max( ( ( temp_output_24_0_g420 + temp_output_25_0_g420 ) - temp_output_1_0_g420 ) , 0.0 );
				float temp_output_19_0_g420 = max( ( ( temp_output_26_0_g420 + temp_output_27_0_g420 ) - temp_output_1_0_g420 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch31_g419 = ( ( ( tex2DArrayNode35_g419 * temp_output_21_0_g420 ) + ( tex2DArrayNode32_g419 * temp_output_19_0_g420 ) ) / ( temp_output_21_0_g420 + temp_output_19_0_g420 ) );
				#else
				float4 staticSwitch31_g419 = tex2DArrayNode32_g419;
				#endif
				float temp_output_299_3 = staticSwitch31_g419.b;
				float temp_output_26_0_g437 = temp_output_299_3;
				float temp_output_27_0_g437 = break273.x;
				float temp_output_1_0_g437 = ( max( ( temp_output_24_0_g437 + temp_output_25_0_g437 ) , ( temp_output_26_0_g437 + temp_output_27_0_g437 ) ) - 0.2 );
				float temp_output_21_0_g437 = max( ( ( temp_output_24_0_g437 + temp_output_25_0_g437 ) - temp_output_1_0_g437 ) , 0.0 );
				float4 tex2DNode14_g419 = tex2D( _Tint, temp_output_12_0_g419 );
				float temp_output_24_0_g422 = tex2DArrayNode35_g419.b;
				float temp_output_25_0_g422 = tex2DNode16_g419.a;
				float temp_output_26_0_g422 = tex2DArrayNode32_g419.b;
				float temp_output_27_0_g422 = temp_output_41_0_g419;
				float temp_output_1_0_g422 = ( max( ( temp_output_24_0_g422 + temp_output_25_0_g422 ) , ( temp_output_26_0_g422 + temp_output_27_0_g422 ) ) - 0.2 );
				float temp_output_21_0_g422 = max( ( ( temp_output_24_0_g422 + temp_output_25_0_g422 ) - temp_output_1_0_g422 ) , 0.0 );
				float temp_output_19_0_g422 = max( ( ( temp_output_26_0_g422 + temp_output_27_0_g422 ) - temp_output_1_0_g422 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch30_g419 = ( ( ( tex2DNode16_g419 * temp_output_21_0_g422 ) + ( tex2DNode14_g419 * temp_output_19_0_g422 ) ) / ( temp_output_21_0_g422 + temp_output_19_0_g422 ) );
				#else
				float4 staticSwitch30_g419 = tex2DNode14_g419;
				#endif
				float temp_output_19_0_g437 = max( ( ( temp_output_26_0_g437 + temp_output_27_0_g437 ) - temp_output_1_0_g437 ) , 0.0 );
				float temp_output_24_0_g436 = temp_output_302_3;
				float temp_output_25_0_g436 = temp_output_274_0;
				float temp_output_26_0_g436 = temp_output_299_3;
				float temp_output_27_0_g436 = break273.x;
				float temp_output_1_0_g436 = ( max( ( temp_output_24_0_g436 + temp_output_25_0_g436 ) , ( temp_output_26_0_g436 + temp_output_27_0_g436 ) ) - 0.2 );
				float temp_output_21_0_g436 = max( ( ( temp_output_24_0_g436 + temp_output_25_0_g436 ) - temp_output_1_0_g436 ) , 0.0 );
				float temp_output_19_0_g436 = max( ( ( temp_output_26_0_g436 + temp_output_27_0_g436 ) - temp_output_1_0_g436 ) , 0.0 );
				float4 temp_output_290_0 = ( ( ( staticSwitch31_g423 * temp_output_21_0_g436 ) + ( staticSwitch31_g419 * temp_output_19_0_g436 ) ) / ( temp_output_21_0_g436 + temp_output_19_0_g436 ) );
				float temp_output_24_0_g439 = temp_output_290_0.z;
				float temp_output_275_0 = ( 1.0 - break273.y );
				float temp_output_25_0_g439 = temp_output_275_0;
				float2 temp_output_12_0_g431 = ( ( temp_output_270_0 + float2( 0,1 ) ) / appendResult265 );
				float4 tex2DNode6_g431 = tex2D( _Control, temp_output_12_0_g431 );
				float4 tex2DArrayNode32_g431 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode6_g431.g * 255.0 ) );
				float4 tex2DNode15_g431 = tex2D( _GrassControl, temp_output_12_0_g431 );
				float4 tex2DArrayNode35_g431 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode15_g431.g * 255.0 ) );
				float temp_output_24_0_g432 = tex2DArrayNode35_g431.b;
				float4 tex2DNode16_g431 = tex2D( _GrassTint, temp_output_12_0_g431 );
				float temp_output_25_0_g432 = tex2DNode16_g431.a;
				float temp_output_26_0_g432 = tex2DArrayNode32_g431.b;
				float temp_output_41_0_g431 = ( 1.0 - tex2DNode16_g431.a );
				float temp_output_27_0_g432 = temp_output_41_0_g431;
				float temp_output_1_0_g432 = ( max( ( temp_output_24_0_g432 + temp_output_25_0_g432 ) , ( temp_output_26_0_g432 + temp_output_27_0_g432 ) ) - 0.2 );
				float temp_output_21_0_g432 = max( ( ( temp_output_24_0_g432 + temp_output_25_0_g432 ) - temp_output_1_0_g432 ) , 0.0 );
				float temp_output_19_0_g432 = max( ( ( temp_output_26_0_g432 + temp_output_27_0_g432 ) - temp_output_1_0_g432 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch31_g431 = ( ( ( tex2DArrayNode35_g431 * temp_output_21_0_g432 ) + ( tex2DArrayNode32_g431 * temp_output_19_0_g432 ) ) / ( temp_output_21_0_g432 + temp_output_19_0_g432 ) );
				#else
				float4 staticSwitch31_g431 = tex2DArrayNode32_g431;
				#endif
				float temp_output_300_3 = staticSwitch31_g431.b;
				float temp_output_24_0_g435 = temp_output_300_3;
				float temp_output_25_0_g435 = temp_output_274_0;
				float2 temp_output_12_0_g427 = ( ( temp_output_270_0 + float2( 1,1 ) ) / appendResult265 );
				float4 tex2DNode6_g427 = tex2D( _Control, temp_output_12_0_g427 );
				float4 tex2DArrayNode32_g427 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode6_g427.g * 255.0 ) );
				float4 tex2DNode15_g427 = tex2D( _GrassControl, temp_output_12_0_g427 );
				float4 tex2DArrayNode35_g427 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode15_g427.g * 255.0 ) );
				float temp_output_24_0_g428 = tex2DArrayNode35_g427.b;
				float4 tex2DNode16_g427 = tex2D( _GrassTint, temp_output_12_0_g427 );
				float temp_output_25_0_g428 = tex2DNode16_g427.a;
				float temp_output_26_0_g428 = tex2DArrayNode32_g427.b;
				float temp_output_41_0_g427 = ( 1.0 - tex2DNode16_g427.a );
				float temp_output_27_0_g428 = temp_output_41_0_g427;
				float temp_output_1_0_g428 = ( max( ( temp_output_24_0_g428 + temp_output_25_0_g428 ) , ( temp_output_26_0_g428 + temp_output_27_0_g428 ) ) - 0.2 );
				float temp_output_21_0_g428 = max( ( ( temp_output_24_0_g428 + temp_output_25_0_g428 ) - temp_output_1_0_g428 ) , 0.0 );
				float temp_output_19_0_g428 = max( ( ( temp_output_26_0_g428 + temp_output_27_0_g428 ) - temp_output_1_0_g428 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch31_g427 = ( ( ( tex2DArrayNode35_g427 * temp_output_21_0_g428 ) + ( tex2DArrayNode32_g427 * temp_output_19_0_g428 ) ) / ( temp_output_21_0_g428 + temp_output_19_0_g428 ) );
				#else
				float4 staticSwitch31_g427 = tex2DArrayNode32_g427;
				#endif
				float temp_output_301_3 = staticSwitch31_g427.b;
				float temp_output_26_0_g435 = temp_output_301_3;
				float temp_output_27_0_g435 = break273.x;
				float temp_output_1_0_g435 = ( max( ( temp_output_24_0_g435 + temp_output_25_0_g435 ) , ( temp_output_26_0_g435 + temp_output_27_0_g435 ) ) - 0.2 );
				float temp_output_21_0_g435 = max( ( ( temp_output_24_0_g435 + temp_output_25_0_g435 ) - temp_output_1_0_g435 ) , 0.0 );
				float temp_output_19_0_g435 = max( ( ( temp_output_26_0_g435 + temp_output_27_0_g435 ) - temp_output_1_0_g435 ) , 0.0 );
				float4 temp_output_293_0 = ( ( ( staticSwitch31_g431 * temp_output_21_0_g435 ) + ( staticSwitch31_g427 * temp_output_19_0_g435 ) ) / ( temp_output_21_0_g435 + temp_output_19_0_g435 ) );
				float temp_output_26_0_g439 = temp_output_293_0.z;
				float temp_output_27_0_g439 = break273.y;
				float temp_output_1_0_g439 = ( max( ( temp_output_24_0_g439 + temp_output_25_0_g439 ) , ( temp_output_26_0_g439 + temp_output_27_0_g439 ) ) - 0.2 );
				float temp_output_21_0_g439 = max( ( ( temp_output_24_0_g439 + temp_output_25_0_g439 ) - temp_output_1_0_g439 ) , 0.0 );
				float4 tex2DNode14_g431 = tex2D( _Tint, temp_output_12_0_g431 );
				float temp_output_24_0_g434 = tex2DArrayNode35_g431.b;
				float temp_output_25_0_g434 = tex2DNode16_g431.a;
				float temp_output_26_0_g434 = tex2DArrayNode32_g431.b;
				float temp_output_27_0_g434 = temp_output_41_0_g431;
				float temp_output_1_0_g434 = ( max( ( temp_output_24_0_g434 + temp_output_25_0_g434 ) , ( temp_output_26_0_g434 + temp_output_27_0_g434 ) ) - 0.2 );
				float temp_output_21_0_g434 = max( ( ( temp_output_24_0_g434 + temp_output_25_0_g434 ) - temp_output_1_0_g434 ) , 0.0 );
				float temp_output_19_0_g434 = max( ( ( temp_output_26_0_g434 + temp_output_27_0_g434 ) - temp_output_1_0_g434 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch30_g431 = ( ( ( tex2DNode16_g431 * temp_output_21_0_g434 ) + ( tex2DNode14_g431 * temp_output_19_0_g434 ) ) / ( temp_output_21_0_g434 + temp_output_19_0_g434 ) );
				#else
				float4 staticSwitch30_g431 = tex2DNode14_g431;
				#endif
				float temp_output_24_0_g438 = temp_output_300_3;
				float temp_output_25_0_g438 = temp_output_274_0;
				float temp_output_26_0_g438 = temp_output_301_3;
				float temp_output_27_0_g438 = break273.x;
				float temp_output_1_0_g438 = ( max( ( temp_output_24_0_g438 + temp_output_25_0_g438 ) , ( temp_output_26_0_g438 + temp_output_27_0_g438 ) ) - 0.2 );
				float temp_output_21_0_g438 = max( ( ( temp_output_24_0_g438 + temp_output_25_0_g438 ) - temp_output_1_0_g438 ) , 0.0 );
				float4 tex2DNode14_g427 = tex2D( _Tint, temp_output_12_0_g427 );
				float temp_output_24_0_g430 = tex2DArrayNode35_g427.b;
				float temp_output_25_0_g430 = tex2DNode16_g427.a;
				float temp_output_26_0_g430 = tex2DArrayNode32_g427.b;
				float temp_output_27_0_g430 = temp_output_41_0_g427;
				float temp_output_1_0_g430 = ( max( ( temp_output_24_0_g430 + temp_output_25_0_g430 ) , ( temp_output_26_0_g430 + temp_output_27_0_g430 ) ) - 0.2 );
				float temp_output_21_0_g430 = max( ( ( temp_output_24_0_g430 + temp_output_25_0_g430 ) - temp_output_1_0_g430 ) , 0.0 );
				float temp_output_19_0_g430 = max( ( ( temp_output_26_0_g430 + temp_output_27_0_g430 ) - temp_output_1_0_g430 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch30_g427 = ( ( ( tex2DNode16_g427 * temp_output_21_0_g430 ) + ( tex2DNode14_g427 * temp_output_19_0_g430 ) ) / ( temp_output_21_0_g430 + temp_output_19_0_g430 ) );
				#else
				float4 staticSwitch30_g427 = tex2DNode14_g427;
				#endif
				float temp_output_19_0_g438 = max( ( ( temp_output_26_0_g438 + temp_output_27_0_g438 ) - temp_output_1_0_g438 ) , 0.0 );
				float temp_output_19_0_g439 = max( ( ( temp_output_26_0_g439 + temp_output_27_0_g439 ) - temp_output_1_0_g439 ) , 0.0 );
				float4 temp_output_221_0 = ( ( ( ( ( ( staticSwitch30_g423 * temp_output_21_0_g437 ) + ( staticSwitch30_g419 * temp_output_19_0_g437 ) ) / ( temp_output_21_0_g437 + temp_output_19_0_g437 ) ) * temp_output_21_0_g439 ) + ( ( ( ( staticSwitch30_g431 * temp_output_21_0_g438 ) + ( staticSwitch30_g427 * temp_output_19_0_g438 ) ) / ( temp_output_21_0_g438 + temp_output_19_0_g438 ) ) * temp_output_19_0_g439 ) ) / ( temp_output_21_0_g439 + temp_output_19_0_g439 ) );
				float temp_output_3_0_g440 = ( temp_output_221_0.w * 2.0 );
				
				surfaceDescription.Alpha = min( temp_output_3_0_g440 , 1.0 );

				#ifdef _ALPHATEST_ON
				surfaceDescription.AlphaClipThreshold = _AlphaCutoff;
				#endif

				#ifdef _DEPTHOFFSET_ON
				surfaceDescription.DepthOffset = 0;
				#endif

				SurfaceData surfaceData;
				BuiltinData builtinData;
				GetSurfaceAndBuiltinData(surfaceDescription, input, V, posInput, surfaceData, builtinData);

				#ifdef _DEPTHOFFSET_ON
				outputDepth = posInput.deviceDepth;
				#endif

				outColor = float4( _ObjectId, _PassValue, 1.0, 1.0 );
			}
			ENDHLSL
		}

		
		Pass
		{
			
			Name "DepthOnly"
			Tags { "LightMode"="DepthOnly" }
			Cull [_CullMode]

			ZWrite On

			Stencil
			{
				Ref [_StencilRefDepth]
				WriteMask [_StencilWriteMaskDepth]
				Comp Always
				Pass Replace
				Fail Keep
				ZFail Keep
			}


			HLSLPROGRAM

			#define _SPECULAR_OCCLUSION_FROM_AO 1
			#define _DISABLE_DECALS 1
			#define _DISABLE_SSR 1
			#define _AMBIENT_OCCLUSION 1
			#define ASE_SRP_VERSION 999999


			#pragma shader_feature _SURFACE_TYPE_TRANSPARENT
			#pragma shader_feature_local _DOUBLESIDED_ON
			#pragma shader_feature_local _BLENDMODE_OFF _BLENDMODE_ALPHA _BLENDMODE_ADD _BLENDMODE_PRE_MULTIPLY
			#pragma shader_feature_local_fragment _ _ENABLE_FOG_ON_TRANSPARENT
			#pragma shader_feature_local _ALPHATEST_ON
			#pragma shader_feature_local_fragment _ _DISABLE_DECALS
			#pragma shader_feature_local _ _TRANSPARENT_WRITES_MOTION_VEC

			#define SHADERPASS SHADERPASS_DEPTH_ONLY
			#pragma multi_compile _ WRITE_NORMAL_BUFFER
			#pragma multi_compile_fragment _ WRITE_MSAA_DEPTH
			#pragma multi_compile _ WRITE_DECAL_BUFFER

			#pragma vertex Vert
			#pragma fragment Frag
			
			//#define UNITY_MATERIAL_LIT

			
			#if defined(_MATERIAL_FEATURE_SUBSURFACE_SCATTERING) && !defined(_SURFACE_TYPE_TRANSPARENT)
			#define OUTPUT_SPLIT_LIGHTING
			#endif

			#if defined(SHADER_LIT) && !defined(_SURFACE_TYPE_TRANSPARENT)
                #define _DEFERRED_CAPABLE_MATERIAL
            #endif
        
            
            #if defined(_TRANSPARENT_WRITES_MOTION_VEC) && defined(_SURFACE_TYPE_TRANSPARENT)
                #define _WRITE_TRANSPARENT_MOTION_VECTOR
            #endif

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
        	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/GeometricTools.hlsl" 
        	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Tessellation.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/FragInputs.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl" 
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderGraphHeader.hlsl" 
			#ifdef DEBUG_DISPLAY
				#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Debug/DebugDisplay.hlsl"
			#endif
			
			CBUFFER_START( UnityPerMaterial )
			float4 _MatTexArray_ST;
			float4 _WorldBounds;
			float4 _Control_TexelSize;
			float4 _ShapeMap_ST;
			float _NormalScale;
			float _MetalLevel;
			float _Rough;
			float4 _EmissionColor;
			float _AlphaCutoff;
			float _RenderQueueType;
			#ifdef _ADD_PRECOMPUTED_VELOCITY
			float _AddPrecomputedVelocity;
			#endif
			float _StencilRef;
			float _StencilWriteMask;
			float _StencilRefDepth;
			float _StencilWriteMaskDepth;
			float _StencilRefMV;
			float _StencilWriteMaskMV;
			float _StencilRefDistortionVec;
			float _StencilWriteMaskDistortionVec;
			float _StencilWriteMaskGBuffer;
			float _StencilRefGBuffer;
			float _ZTestGBuffer;
			float _RequireSplitLighting;
			float _ReceivesSSR;
			float _SurfaceType;
			float _BlendMode;
			float _SrcBlend;
			float _DstBlend;
			float _AlphaSrcBlend;
			float _AlphaDstBlend;
			float _ZWrite;
			float _TransparentZWrite;
			float _CullMode;
			float _TransparentSortPriority;
			float _EnableFogOnTransparent;
			float _CullModeForward;
			float _TransparentCullMode;
			float _ZTestDepthEqualForOpaque;
			float _ZTestTransparent;
			float _TransparentBackfaceEnable;
			float _AlphaCutoffEnable;
			float _UseShadowThreshold;
			float _DoubleSidedEnable;
			float _DoubleSidedNormalMode;
			float4 _DoubleSidedConstants;
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			TEXTURE2D_ARRAY(_ShapeMap);
			sampler2D _Control;
			SAMPLER(sampler_ShapeMap);
			sampler2D _GrassControl;
			sampler2D _GrassTint;
			TEXTURE2D_ARRAY(_MatTexArray);
			SAMPLER(sampler_MatTexArray);
			sampler2D _Tint;


			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/NormalSurfaceGradient.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Lit/Lit.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/BuiltinUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/MaterialUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Decal/DecalUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Lit/LitDecalData.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderGraphFunctions.hlsl"

			#define ASE_NEEDS_FRAG_RELATIVE_WORLD_POS
			#define GRASS


			#if defined(_DOUBLESIDED_ON) && !defined(ASE_NEED_CULLFACE)
				#define ASE_NEED_CULLFACE 1
			#endif

			struct AttributesMesh
			{
				float3 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 tangentOS : TANGENT;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryingsMeshToPS
			{
				float4 positionCS : SV_Position;
				float3 positionRWS : TEXCOORD0;
				float3 normalWS : TEXCOORD1;
				float4 tangentWS : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
				#if defined(SHADER_STAGE_FRAGMENT) && defined(ASE_NEED_CULLFACE)
				FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
				#endif
			};

			
			void BuildSurfaceData(FragInputs fragInputs, inout SmoothSurfaceDescription surfaceDescription, float3 V, PositionInputs posInput, out SurfaceData surfaceData, out float3 bentNormalWS)
			{
				ZERO_INITIALIZE(SurfaceData, surfaceData);

				surfaceData.specularOcclusion = 1.0;

				// surface data
				surfaceData.perceptualSmoothness =		surfaceDescription.Smoothness;

				// refraction
				#ifdef _HAS_REFRACTION
				if( _EnableSSRefraction )
				{
					surfaceData.transmittanceMask = ( 1.0 - surfaceDescription.Alpha );
					surfaceDescription.Alpha = 1.0;
				}
				else
				{
					surfaceData.ior = 1.0;
					surfaceData.transmittanceColor = float3( 1.0, 1.0, 1.0 );
					surfaceData.atDistance = 1.0;
					surfaceData.transmittanceMask = 0.0;
					surfaceDescription.Alpha = 1.0;
				}
				#else
				surfaceData.ior = 1.0;
				surfaceData.transmittanceColor = float3( 1.0, 1.0, 1.0 );
				surfaceData.atDistance = 1.0;
				surfaceData.transmittanceMask = 0.0;
				#endif


				// material features
				surfaceData.materialFeatures = MATERIALFEATUREFLAGS_LIT_STANDARD;
				#ifdef _MATERIAL_FEATURE_SUBSURFACE_SCATTERING
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_SUBSURFACE_SCATTERING;
				#endif
				#ifdef _MATERIAL_FEATURE_TRANSMISSION
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_TRANSMISSION;
				#endif
				#ifdef _MATERIAL_FEATURE_ANISOTROPY
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_ANISOTROPY;
				#endif
				#ifdef ASE_LIT_CLEAR_COAT
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_CLEAR_COAT;
				#endif
				#ifdef _MATERIAL_FEATURE_IRIDESCENCE
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_IRIDESCENCE;
				#endif
				#ifdef _MATERIAL_FEATURE_SPECULAR_COLOR
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_SPECULAR_COLOR;
				#endif

				// others
				#if defined (_MATERIAL_FEATURE_SPECULAR_COLOR) && defined (_ENERGY_CONSERVING_SPECULAR)
				surfaceData.baseColor *= ( 1.0 - Max3( surfaceData.specularColor.r, surfaceData.specularColor.g, surfaceData.specularColor.b ) );
				#endif
				#ifdef _DOUBLESIDED_ON
				float3 doubleSidedConstants = _DoubleSidedConstants.xyz;
				#else
				float3 doubleSidedConstants = float3( 1.0, 1.0, 1.0 );
				#endif

				// normals
				float3 normalTS = float3(0.0f, 0.0f, 1.0f);
				normalTS = surfaceDescription.Normal;
				GetNormalWS( fragInputs, normalTS, surfaceData.normalWS, doubleSidedConstants );

				surfaceData.geomNormalWS = fragInputs.tangentToWorld[2];
				surfaceData.tangentWS = normalize( fragInputs.tangentToWorld[ 0 ].xyz );
				
				// decals
				#if HAVE_DECALS
				if( _EnableDecals )
				{
					DecalSurfaceData decalSurfaceData = GetDecalSurfaceData(posInput, fragInputs.tangentToWorld[2], surfaceDescription.Alpha);
					ApplyDecalToSurfaceData(decalSurfaceData, fragInputs.tangentToWorld[2], surfaceData);
				}
				#endif

				bentNormalWS = surfaceData.normalWS;
				surfaceData.tangentWS = Orthonormalize( surfaceData.tangentWS, surfaceData.normalWS );

				#if defined(_SPECULAR_OCCLUSION_CUSTOM)
				#elif defined(_SPECULAR_OCCLUSION_FROM_AO_BENT_NORMAL)
				surfaceData.specularOcclusion = GetSpecularOcclusionFromBentAO( V, bentNormalWS, surfaceData.normalWS, surfaceData.ambientOcclusion, PerceptualSmoothnessToPerceptualRoughness( surfaceData.perceptualSmoothness ) );
				#elif defined(_AMBIENT_OCCLUSION) && defined(_SPECULAR_OCCLUSION_FROM_AO)
				surfaceData.specularOcclusion = GetSpecularOcclusionFromAmbientOcclusion( ClampNdotV( dot( surfaceData.normalWS, V ) ), surfaceData.ambientOcclusion, PerceptualSmoothnessToRoughness( surfaceData.perceptualSmoothness ) );
				#endif

				// debug
				#if defined(DEBUG_DISPLAY)
				if (_DebugMipMapMode != DEBUGMIPMAPMODE_NONE)
				{
					surfaceData.metallic = 0;
				}
				ApplyDebugToSurfaceData(fragInputs.tangentToWorld, surfaceData);
				#endif
			}

			void GetSurfaceAndBuiltinData(SmoothSurfaceDescription surfaceDescription, FragInputs fragInputs, float3 V, inout PositionInputs posInput, out SurfaceData surfaceData, out BuiltinData builtinData)
			{
				#ifdef LOD_FADE_CROSSFADE
				LODDitheringTransition(ComputeFadeMaskSeed(V, posInput.positionSS), unity_LODFade.x);
				#endif

				#ifdef _DOUBLESIDED_ON
				float3 doubleSidedConstants = _DoubleSidedConstants.xyz;
				#else
				float3 doubleSidedConstants = float3( 1.0, 1.0, 1.0 );
				#endif

				ApplyDoubleSidedFlipOrMirror( fragInputs, doubleSidedConstants );

				#ifdef _ALPHATEST_ON
				DoAlphaTest( surfaceDescription.Alpha, surfaceDescription.AlphaClipThreshold );
				#endif

				#ifdef _DEPTHOFFSET_ON
				builtinData.depthOffset = surfaceDescription.DepthOffset;
				ApplyDepthOffsetPositionInput( V, surfaceDescription.DepthOffset, GetViewForwardDir(), GetWorldToHClipMatrix(), posInput );
				#endif

				float3 bentNormalWS;
				BuildSurfaceData( fragInputs, surfaceDescription, V, posInput, surfaceData, bentNormalWS );

				InitBuiltinData( posInput, surfaceDescription.Alpha, bentNormalWS, -fragInputs.tangentToWorld[ 2 ], fragInputs.texCoord1, fragInputs.texCoord2, builtinData );

				PostInitBuiltinData(V, posInput, surfaceData, builtinData);
			}

			#if defined(WRITE_DECAL_BUFFER) && !defined(_DISABLE_DECALS)
				#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Decal/DecalPrepassBuffer.hlsl"
			#endif
			PackedVaryingsMeshToPS VertexFunction(AttributesMesh inputMesh )
			{
				PackedVaryingsMeshToPS outputPackedVaryingsMeshToPS;

				UNITY_SETUP_INSTANCE_ID(inputMesh);
				UNITY_TRANSFER_INSTANCE_ID(inputMesh, outputPackedVaryingsMeshToPS);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( outputPackedVaryingsMeshToPS );

				outputPackedVaryingsMeshToPS.ase_texcoord3.xy = inputMesh.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				outputPackedVaryingsMeshToPS.ase_texcoord3.zw = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
				float3 defaultVertexValue = inputMesh.positionOS.xyz;
				#else
				float3 defaultVertexValue = float3( 0, 0, 0 );
				#endif
				float3 vertexValue =  defaultVertexValue ;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
				inputMesh.positionOS.xyz = vertexValue;
				#else
				inputMesh.positionOS.xyz += vertexValue;
				#endif

				inputMesh.normalOS =  inputMesh.normalOS ;
				inputMesh.tangentOS =  inputMesh.tangentOS ;

				float3 positionRWS = TransformObjectToWorld(inputMesh.positionOS);
				float3 normalWS = TransformObjectToWorldNormal(inputMesh.normalOS);
				float4 tangentWS = float4(TransformObjectToWorldDir(inputMesh.tangentOS.xyz), inputMesh.tangentOS.w);

				outputPackedVaryingsMeshToPS.positionCS = TransformWorldToHClip(positionRWS);
				outputPackedVaryingsMeshToPS.positionRWS.xyz = positionRWS;
				outputPackedVaryingsMeshToPS.normalWS.xyz = normalWS;
				outputPackedVaryingsMeshToPS.tangentWS.xyzw = tangentWS;
				return outputPackedVaryingsMeshToPS;
			}
			
			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float3 positionOS : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 tangentOS : TANGENT;
				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl Vert ( AttributesMesh v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.positionOS = v.positionOS;
				o.normalOS = v.normalOS;
				o.tangentOS = v.tangentOS;
				o.ase_texcoord = v.ase_texcoord;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if (SHADEROPTIONS_CAMERA_RELATIVE_RENDERING != 0)
				float3 cameraPos = 0;
				#else
				float3 cameraPos = _WorldSpaceCameraPos;
				#endif
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(float4(v[0].positionOS,1), float4(v[1].positionOS,1), float4(v[2].positionOS,1), tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), cameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(float4(v[0].positionOS,1), float4(v[1].positionOS,1), float4(v[2].positionOS,1), edgeLength, GetObjectToWorldMatrix(), cameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(float4(v[0].positionOS,1), float4(v[1].positionOS,1), float4(v[2].positionOS,1), edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), cameraPos, _ScreenParams, _FrustumPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			PackedVaryingsMeshToPS DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				AttributesMesh o = (AttributesMesh) 0;
				o.positionOS = patch[0].positionOS * bary.x + patch[1].positionOS * bary.y + patch[2].positionOS * bary.z;
				o.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				o.tangentOS = patch[0].tangentOS * bary.x + patch[1].tangentOS * bary.y + patch[2].tangentOS * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.positionOS.xyz - patch[i].normalOS * (dot(o.positionOS.xyz, patch[i].normalOS) - dot(patch[i].positionOS.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				o.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			PackedVaryingsMeshToPS Vert ( AttributesMesh v )
			{
				return VertexFunction( v );
			}
			#endif

			#if defined(WRITE_NORMAL_BUFFER) && defined(WRITE_MSAA_DEPTH)
			#define SV_TARGET_DECAL SV_Target2
			#elif defined(WRITE_NORMAL_BUFFER) || defined(WRITE_MSAA_DEPTH)
			#define SV_TARGET_DECAL SV_Target1
			#else
			#define SV_TARGET_DECAL SV_Target0
			#endif

			void Frag( PackedVaryingsMeshToPS packedInput
						#if defined(SCENESELECTIONPASS) || defined(SCENEPICKINGPASS)
						, out float4 outColor : SV_Target0
						#else
							#ifdef WRITE_MSAA_DEPTH
							, out float4 depthColor : SV_Target0
								#ifdef WRITE_NORMAL_BUFFER
								, out float4 outNormalBuffer : SV_Target1
								#endif
							#else
								#ifdef WRITE_NORMAL_BUFFER
								, out float4 outNormalBuffer : SV_Target0
								#endif
							#endif

							#if defined(WRITE_DECAL_BUFFER) && !defined(_DISABLE_DECALS)
							, out float4 outDecalBuffer : SV_TARGET_DECAL
							#endif
						#endif

						#if defined(_DEPTHOFFSET_ON) && !defined(SCENEPICKINGPASS)
						, out float outputDepth : SV_Depth
						#endif
						
					)
			{
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( packedInput );
				UNITY_SETUP_INSTANCE_ID( packedInput );

				float3 positionRWS = packedInput.positionRWS.xyz;
				float3 normalWS = packedInput.normalWS.xyz;
				float4 tangentWS = packedInput.tangentWS.xyzw;

				FragInputs input;
				ZERO_INITIALIZE(FragInputs, input);

				input.tangentToWorld = k_identity3x3;
				input.positionSS = packedInput.positionCS;

				input.positionRWS = positionRWS;
				input.tangentToWorld = BuildTangentToWorld(tangentWS, normalWS);

				#if _DOUBLESIDED_ON && SHADER_STAGE_FRAGMENT
				input.isFrontFace = IS_FRONT_VFACE( packedInput.cullFace, true, false);
				#elif SHADER_STAGE_FRAGMENT
				#if defined(ASE_NEED_CULLFACE)
				input.isFrontFace = IS_FRONT_VFACE( packedInput.cullFace, true, false );
				#endif
				#endif
				half isFrontFace = input.isFrontFace;

				PositionInputs posInput = GetPositionInput(input.positionSS.xy, _ScreenSize.zw, input.positionSS.z, input.positionSS.w, input.positionRWS);

				float3 V = GetWorldSpaceNormalizeViewDir(input.positionRWS);

				SmoothSurfaceDescription surfaceDescription = (SmoothSurfaceDescription)0;
				float2 uv_ShapeMap = packedInput.ase_texcoord3.xy;
				float3 ase_worldPos = GetAbsolutePositionWS( positionRWS );
				float2 appendResult257 = (float2(ase_worldPos.x , ase_worldPos.z));
				float2 appendResult260 = (float2(_WorldBounds.x , _WorldBounds.y));
				float2 appendResult259 = (float2(_WorldBounds.z , _WorldBounds.w));
				float2 temp_output_264_0 = ( ( appendResult257 - appendResult260 ) / ( appendResult259 - appendResult260 ) );
				float2 appendResult265 = (float2(_Control_TexelSize.z , _Control_TexelSize.w));
				float2 temp_output_266_0 = ( temp_output_264_0 * appendResult265 );
				float2 temp_output_270_0 = floor( ( ( temp_output_266_0 + float2( 0.5,0.5 ) ) - float2( 0.5,0.5 ) ) );
				float2 temp_output_12_0_g423 = ( ( temp_output_270_0 + float2( 0,0 ) ) / appendResult265 );
				float4 tex2DNode6_g423 = tex2D( _Control, temp_output_12_0_g423 );
				float4 tex2DArrayNode32_g423 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode6_g423.g * 255.0 ) );
				float4 tex2DNode15_g423 = tex2D( _GrassControl, temp_output_12_0_g423 );
				float4 tex2DArrayNode35_g423 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode15_g423.g * 255.0 ) );
				float temp_output_24_0_g424 = tex2DArrayNode35_g423.b;
				float4 tex2DNode16_g423 = tex2D( _GrassTint, temp_output_12_0_g423 );
				float temp_output_25_0_g424 = tex2DNode16_g423.a;
				float temp_output_26_0_g424 = tex2DArrayNode32_g423.b;
				float temp_output_41_0_g423 = ( 1.0 - tex2DNode16_g423.a );
				float temp_output_27_0_g424 = temp_output_41_0_g423;
				float temp_output_1_0_g424 = ( max( ( temp_output_24_0_g424 + temp_output_25_0_g424 ) , ( temp_output_26_0_g424 + temp_output_27_0_g424 ) ) - 0.2 );
				float temp_output_21_0_g424 = max( ( ( temp_output_24_0_g424 + temp_output_25_0_g424 ) - temp_output_1_0_g424 ) , 0.0 );
				float temp_output_19_0_g424 = max( ( ( temp_output_26_0_g424 + temp_output_27_0_g424 ) - temp_output_1_0_g424 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch31_g423 = ( ( ( tex2DArrayNode35_g423 * temp_output_21_0_g424 ) + ( tex2DArrayNode32_g423 * temp_output_19_0_g424 ) ) / ( temp_output_21_0_g424 + temp_output_19_0_g424 ) );
				#else
				float4 staticSwitch31_g423 = tex2DArrayNode32_g423;
				#endif
				float temp_output_302_3 = staticSwitch31_g423.b;
				float temp_output_24_0_g436 = temp_output_302_3;
				float2 break273 = ( temp_output_266_0 - temp_output_270_0 );
				float temp_output_274_0 = ( 1.0 - break273.x );
				float temp_output_25_0_g436 = temp_output_274_0;
				float2 temp_output_12_0_g419 = ( ( temp_output_270_0 + float2( 1,0 ) ) / appendResult265 );
				float4 tex2DNode6_g419 = tex2D( _Control, temp_output_12_0_g419 );
				float4 tex2DArrayNode32_g419 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode6_g419.g * 255.0 ) );
				float4 tex2DNode15_g419 = tex2D( _GrassControl, temp_output_12_0_g419 );
				float4 tex2DArrayNode35_g419 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode15_g419.g * 255.0 ) );
				float temp_output_24_0_g420 = tex2DArrayNode35_g419.b;
				float4 tex2DNode16_g419 = tex2D( _GrassTint, temp_output_12_0_g419 );
				float temp_output_25_0_g420 = tex2DNode16_g419.a;
				float temp_output_26_0_g420 = tex2DArrayNode32_g419.b;
				float temp_output_41_0_g419 = ( 1.0 - tex2DNode16_g419.a );
				float temp_output_27_0_g420 = temp_output_41_0_g419;
				float temp_output_1_0_g420 = ( max( ( temp_output_24_0_g420 + temp_output_25_0_g420 ) , ( temp_output_26_0_g420 + temp_output_27_0_g420 ) ) - 0.2 );
				float temp_output_21_0_g420 = max( ( ( temp_output_24_0_g420 + temp_output_25_0_g420 ) - temp_output_1_0_g420 ) , 0.0 );
				float temp_output_19_0_g420 = max( ( ( temp_output_26_0_g420 + temp_output_27_0_g420 ) - temp_output_1_0_g420 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch31_g419 = ( ( ( tex2DArrayNode35_g419 * temp_output_21_0_g420 ) + ( tex2DArrayNode32_g419 * temp_output_19_0_g420 ) ) / ( temp_output_21_0_g420 + temp_output_19_0_g420 ) );
				#else
				float4 staticSwitch31_g419 = tex2DArrayNode32_g419;
				#endif
				float temp_output_299_3 = staticSwitch31_g419.b;
				float temp_output_26_0_g436 = temp_output_299_3;
				float temp_output_27_0_g436 = break273.x;
				float temp_output_1_0_g436 = ( max( ( temp_output_24_0_g436 + temp_output_25_0_g436 ) , ( temp_output_26_0_g436 + temp_output_27_0_g436 ) ) - 0.2 );
				float temp_output_21_0_g436 = max( ( ( temp_output_24_0_g436 + temp_output_25_0_g436 ) - temp_output_1_0_g436 ) , 0.0 );
				float temp_output_19_0_g436 = max( ( ( temp_output_26_0_g436 + temp_output_27_0_g436 ) - temp_output_1_0_g436 ) , 0.0 );
				float4 temp_output_290_0 = ( ( ( staticSwitch31_g423 * temp_output_21_0_g436 ) + ( staticSwitch31_g419 * temp_output_19_0_g436 ) ) / ( temp_output_21_0_g436 + temp_output_19_0_g436 ) );
				float temp_output_24_0_g441 = temp_output_290_0.z;
				float temp_output_275_0 = ( 1.0 - break273.y );
				float temp_output_25_0_g441 = temp_output_275_0;
				float2 temp_output_12_0_g431 = ( ( temp_output_270_0 + float2( 0,1 ) ) / appendResult265 );
				float4 tex2DNode6_g431 = tex2D( _Control, temp_output_12_0_g431 );
				float4 tex2DArrayNode32_g431 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode6_g431.g * 255.0 ) );
				float4 tex2DNode15_g431 = tex2D( _GrassControl, temp_output_12_0_g431 );
				float4 tex2DArrayNode35_g431 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode15_g431.g * 255.0 ) );
				float temp_output_24_0_g432 = tex2DArrayNode35_g431.b;
				float4 tex2DNode16_g431 = tex2D( _GrassTint, temp_output_12_0_g431 );
				float temp_output_25_0_g432 = tex2DNode16_g431.a;
				float temp_output_26_0_g432 = tex2DArrayNode32_g431.b;
				float temp_output_41_0_g431 = ( 1.0 - tex2DNode16_g431.a );
				float temp_output_27_0_g432 = temp_output_41_0_g431;
				float temp_output_1_0_g432 = ( max( ( temp_output_24_0_g432 + temp_output_25_0_g432 ) , ( temp_output_26_0_g432 + temp_output_27_0_g432 ) ) - 0.2 );
				float temp_output_21_0_g432 = max( ( ( temp_output_24_0_g432 + temp_output_25_0_g432 ) - temp_output_1_0_g432 ) , 0.0 );
				float temp_output_19_0_g432 = max( ( ( temp_output_26_0_g432 + temp_output_27_0_g432 ) - temp_output_1_0_g432 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch31_g431 = ( ( ( tex2DArrayNode35_g431 * temp_output_21_0_g432 ) + ( tex2DArrayNode32_g431 * temp_output_19_0_g432 ) ) / ( temp_output_21_0_g432 + temp_output_19_0_g432 ) );
				#else
				float4 staticSwitch31_g431 = tex2DArrayNode32_g431;
				#endif
				float temp_output_300_3 = staticSwitch31_g431.b;
				float temp_output_24_0_g435 = temp_output_300_3;
				float temp_output_25_0_g435 = temp_output_274_0;
				float2 temp_output_12_0_g427 = ( ( temp_output_270_0 + float2( 1,1 ) ) / appendResult265 );
				float4 tex2DNode6_g427 = tex2D( _Control, temp_output_12_0_g427 );
				float4 tex2DArrayNode32_g427 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode6_g427.g * 255.0 ) );
				float4 tex2DNode15_g427 = tex2D( _GrassControl, temp_output_12_0_g427 );
				float4 tex2DArrayNode35_g427 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode15_g427.g * 255.0 ) );
				float temp_output_24_0_g428 = tex2DArrayNode35_g427.b;
				float4 tex2DNode16_g427 = tex2D( _GrassTint, temp_output_12_0_g427 );
				float temp_output_25_0_g428 = tex2DNode16_g427.a;
				float temp_output_26_0_g428 = tex2DArrayNode32_g427.b;
				float temp_output_41_0_g427 = ( 1.0 - tex2DNode16_g427.a );
				float temp_output_27_0_g428 = temp_output_41_0_g427;
				float temp_output_1_0_g428 = ( max( ( temp_output_24_0_g428 + temp_output_25_0_g428 ) , ( temp_output_26_0_g428 + temp_output_27_0_g428 ) ) - 0.2 );
				float temp_output_21_0_g428 = max( ( ( temp_output_24_0_g428 + temp_output_25_0_g428 ) - temp_output_1_0_g428 ) , 0.0 );
				float temp_output_19_0_g428 = max( ( ( temp_output_26_0_g428 + temp_output_27_0_g428 ) - temp_output_1_0_g428 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch31_g427 = ( ( ( tex2DArrayNode35_g427 * temp_output_21_0_g428 ) + ( tex2DArrayNode32_g427 * temp_output_19_0_g428 ) ) / ( temp_output_21_0_g428 + temp_output_19_0_g428 ) );
				#else
				float4 staticSwitch31_g427 = tex2DArrayNode32_g427;
				#endif
				float temp_output_301_3 = staticSwitch31_g427.b;
				float temp_output_26_0_g435 = temp_output_301_3;
				float temp_output_27_0_g435 = break273.x;
				float temp_output_1_0_g435 = ( max( ( temp_output_24_0_g435 + temp_output_25_0_g435 ) , ( temp_output_26_0_g435 + temp_output_27_0_g435 ) ) - 0.2 );
				float temp_output_21_0_g435 = max( ( ( temp_output_24_0_g435 + temp_output_25_0_g435 ) - temp_output_1_0_g435 ) , 0.0 );
				float temp_output_19_0_g435 = max( ( ( temp_output_26_0_g435 + temp_output_27_0_g435 ) - temp_output_1_0_g435 ) , 0.0 );
				float4 temp_output_293_0 = ( ( ( staticSwitch31_g431 * temp_output_21_0_g435 ) + ( staticSwitch31_g427 * temp_output_19_0_g435 ) ) / ( temp_output_21_0_g435 + temp_output_19_0_g435 ) );
				float temp_output_26_0_g441 = temp_output_293_0.z;
				float temp_output_27_0_g441 = break273.y;
				float temp_output_1_0_g441 = ( max( ( temp_output_24_0_g441 + temp_output_25_0_g441 ) , ( temp_output_26_0_g441 + temp_output_27_0_g441 ) ) - 0.2 );
				float temp_output_21_0_g441 = max( ( ( temp_output_24_0_g441 + temp_output_25_0_g441 ) - temp_output_1_0_g441 ) , 0.0 );
				float temp_output_19_0_g441 = max( ( ( temp_output_26_0_g441 + temp_output_27_0_g441 ) - temp_output_1_0_g441 ) , 0.0 );
				float4 temp_output_180_0 = ( ( ( temp_output_290_0 * temp_output_21_0_g441 ) + ( temp_output_293_0 * temp_output_19_0_g441 ) ) / ( temp_output_21_0_g441 + temp_output_19_0_g441 ) );
				float3 unpack220 = UnpackNormalScale( temp_output_180_0, _NormalScale );
				unpack220.z = lerp( 1, unpack220.z, saturate(_NormalScale) );
				
				float2 uv_MatTexArray = packedInput.ase_texcoord3.xy;
				float4 tex2DArrayNode23_g423 = SAMPLE_TEXTURE2D_ARRAY( _MatTexArray, sampler_MatTexArray, uv_MatTexArray,( tex2DNode6_g423.r * 255.0 ) );
				float temp_output_24_0_g425 = tex2DArrayNode35_g423.b;
				float temp_output_25_0_g425 = tex2DNode16_g423.a;
				float temp_output_26_0_g425 = tex2DArrayNode32_g423.b;
				float temp_output_27_0_g425 = temp_output_41_0_g423;
				float temp_output_1_0_g425 = ( max( ( temp_output_24_0_g425 + temp_output_25_0_g425 ) , ( temp_output_26_0_g425 + temp_output_27_0_g425 ) ) - 0.2 );
				float temp_output_21_0_g425 = max( ( ( temp_output_24_0_g425 + temp_output_25_0_g425 ) - temp_output_1_0_g425 ) , 0.0 );
				float temp_output_19_0_g425 = max( ( ( temp_output_26_0_g425 + temp_output_27_0_g425 ) - temp_output_1_0_g425 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch29_g423 = ( ( ( SAMPLE_TEXTURE2D_ARRAY( _MatTexArray, sampler_MatTexArray, uv_MatTexArray,( tex2DNode15_g423.r * 255.0 ) ) * temp_output_21_0_g425 ) + ( tex2DArrayNode23_g423 * temp_output_19_0_g425 ) ) / ( temp_output_21_0_g425 + temp_output_19_0_g425 ) );
				#else
				float4 staticSwitch29_g423 = tex2DArrayNode23_g423;
				#endif
				float temp_output_24_0_g443 = temp_output_302_3;
				float temp_output_25_0_g443 = temp_output_274_0;
				float temp_output_26_0_g443 = temp_output_299_3;
				float temp_output_27_0_g443 = break273.x;
				float temp_output_1_0_g443 = ( max( ( temp_output_24_0_g443 + temp_output_25_0_g443 ) , ( temp_output_26_0_g443 + temp_output_27_0_g443 ) ) - 0.2 );
				float temp_output_21_0_g443 = max( ( ( temp_output_24_0_g443 + temp_output_25_0_g443 ) - temp_output_1_0_g443 ) , 0.0 );
				float4 tex2DArrayNode23_g419 = SAMPLE_TEXTURE2D_ARRAY( _MatTexArray, sampler_MatTexArray, uv_MatTexArray,( tex2DNode6_g419.r * 255.0 ) );
				float temp_output_24_0_g421 = tex2DArrayNode35_g419.b;
				float temp_output_25_0_g421 = tex2DNode16_g419.a;
				float temp_output_26_0_g421 = tex2DArrayNode32_g419.b;
				float temp_output_27_0_g421 = temp_output_41_0_g419;
				float temp_output_1_0_g421 = ( max( ( temp_output_24_0_g421 + temp_output_25_0_g421 ) , ( temp_output_26_0_g421 + temp_output_27_0_g421 ) ) - 0.2 );
				float temp_output_21_0_g421 = max( ( ( temp_output_24_0_g421 + temp_output_25_0_g421 ) - temp_output_1_0_g421 ) , 0.0 );
				float temp_output_19_0_g421 = max( ( ( temp_output_26_0_g421 + temp_output_27_0_g421 ) - temp_output_1_0_g421 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch29_g419 = ( ( ( SAMPLE_TEXTURE2D_ARRAY( _MatTexArray, sampler_MatTexArray, uv_MatTexArray,( tex2DNode15_g419.r * 255.0 ) ) * temp_output_21_0_g421 ) + ( tex2DArrayNode23_g419 * temp_output_19_0_g421 ) ) / ( temp_output_21_0_g421 + temp_output_19_0_g421 ) );
				#else
				float4 staticSwitch29_g419 = tex2DArrayNode23_g419;
				#endif
				float temp_output_19_0_g443 = max( ( ( temp_output_26_0_g443 + temp_output_27_0_g443 ) - temp_output_1_0_g443 ) , 0.0 );
				float temp_output_24_0_g442 = temp_output_290_0.z;
				float temp_output_25_0_g442 = temp_output_275_0;
				float temp_output_26_0_g442 = temp_output_293_0.z;
				float temp_output_27_0_g442 = break273.y;
				float temp_output_1_0_g442 = ( max( ( temp_output_24_0_g442 + temp_output_25_0_g442 ) , ( temp_output_26_0_g442 + temp_output_27_0_g442 ) ) - 0.2 );
				float temp_output_21_0_g442 = max( ( ( temp_output_24_0_g442 + temp_output_25_0_g442 ) - temp_output_1_0_g442 ) , 0.0 );
				float4 tex2DArrayNode23_g431 = SAMPLE_TEXTURE2D_ARRAY( _MatTexArray, sampler_MatTexArray, uv_MatTexArray,( tex2DNode6_g431.r * 255.0 ) );
				float temp_output_24_0_g433 = tex2DArrayNode35_g431.b;
				float temp_output_25_0_g433 = tex2DNode16_g431.a;
				float temp_output_26_0_g433 = tex2DArrayNode32_g431.b;
				float temp_output_27_0_g433 = temp_output_41_0_g431;
				float temp_output_1_0_g433 = ( max( ( temp_output_24_0_g433 + temp_output_25_0_g433 ) , ( temp_output_26_0_g433 + temp_output_27_0_g433 ) ) - 0.2 );
				float temp_output_21_0_g433 = max( ( ( temp_output_24_0_g433 + temp_output_25_0_g433 ) - temp_output_1_0_g433 ) , 0.0 );
				float temp_output_19_0_g433 = max( ( ( temp_output_26_0_g433 + temp_output_27_0_g433 ) - temp_output_1_0_g433 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch29_g431 = ( ( ( SAMPLE_TEXTURE2D_ARRAY( _MatTexArray, sampler_MatTexArray, uv_MatTexArray,( tex2DNode15_g431.r * 255.0 ) ) * temp_output_21_0_g433 ) + ( tex2DArrayNode23_g431 * temp_output_19_0_g433 ) ) / ( temp_output_21_0_g433 + temp_output_19_0_g433 ) );
				#else
				float4 staticSwitch29_g431 = tex2DArrayNode23_g431;
				#endif
				float temp_output_24_0_g488 = temp_output_300_3;
				float temp_output_25_0_g488 = temp_output_274_0;
				float temp_output_26_0_g488 = temp_output_301_3;
				float temp_output_27_0_g488 = break273.x;
				float temp_output_1_0_g488 = ( max( ( temp_output_24_0_g488 + temp_output_25_0_g488 ) , ( temp_output_26_0_g488 + temp_output_27_0_g488 ) ) - 0.2 );
				float temp_output_21_0_g488 = max( ( ( temp_output_24_0_g488 + temp_output_25_0_g488 ) - temp_output_1_0_g488 ) , 0.0 );
				float4 tex2DArrayNode23_g427 = SAMPLE_TEXTURE2D_ARRAY( _MatTexArray, sampler_MatTexArray, uv_MatTexArray,( tex2DNode6_g427.r * 255.0 ) );
				float temp_output_24_0_g429 = tex2DArrayNode35_g427.b;
				float temp_output_25_0_g429 = tex2DNode16_g427.a;
				float temp_output_26_0_g429 = tex2DArrayNode32_g427.b;
				float temp_output_27_0_g429 = temp_output_41_0_g427;
				float temp_output_1_0_g429 = ( max( ( temp_output_24_0_g429 + temp_output_25_0_g429 ) , ( temp_output_26_0_g429 + temp_output_27_0_g429 ) ) - 0.2 );
				float temp_output_21_0_g429 = max( ( ( temp_output_24_0_g429 + temp_output_25_0_g429 ) - temp_output_1_0_g429 ) , 0.0 );
				float temp_output_19_0_g429 = max( ( ( temp_output_26_0_g429 + temp_output_27_0_g429 ) - temp_output_1_0_g429 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch29_g427 = ( ( ( SAMPLE_TEXTURE2D_ARRAY( _MatTexArray, sampler_MatTexArray, uv_MatTexArray,( tex2DNode15_g427.r * 255.0 ) ) * temp_output_21_0_g429 ) + ( tex2DArrayNode23_g427 * temp_output_19_0_g429 ) ) / ( temp_output_21_0_g429 + temp_output_19_0_g429 ) );
				#else
				float4 staticSwitch29_g427 = tex2DArrayNode23_g427;
				#endif
				float temp_output_19_0_g488 = max( ( ( temp_output_26_0_g488 + temp_output_27_0_g488 ) - temp_output_1_0_g488 ) , 0.0 );
				float temp_output_19_0_g442 = max( ( ( temp_output_26_0_g442 + temp_output_27_0_g442 ) - temp_output_1_0_g442 ) , 0.0 );
				float4 temp_output_159_0 = ( ( ( ( ( ( staticSwitch29_g423 * temp_output_21_0_g443 ) + ( staticSwitch29_g419 * temp_output_19_0_g443 ) ) / ( temp_output_21_0_g443 + temp_output_19_0_g443 ) ) * temp_output_21_0_g442 ) + ( ( ( ( staticSwitch29_g431 * temp_output_21_0_g488 ) + ( staticSwitch29_g427 * temp_output_19_0_g488 ) ) / ( temp_output_21_0_g488 + temp_output_19_0_g488 ) ) * temp_output_19_0_g442 ) ) / ( temp_output_21_0_g442 + temp_output_19_0_g442 ) );
				
				float4 tex2DNode14_g423 = tex2D( _Tint, temp_output_12_0_g423 );
				float temp_output_24_0_g426 = tex2DArrayNode35_g423.b;
				float temp_output_25_0_g426 = tex2DNode16_g423.a;
				float temp_output_26_0_g426 = tex2DArrayNode32_g423.b;
				float temp_output_27_0_g426 = temp_output_41_0_g423;
				float temp_output_1_0_g426 = ( max( ( temp_output_24_0_g426 + temp_output_25_0_g426 ) , ( temp_output_26_0_g426 + temp_output_27_0_g426 ) ) - 0.2 );
				float temp_output_21_0_g426 = max( ( ( temp_output_24_0_g426 + temp_output_25_0_g426 ) - temp_output_1_0_g426 ) , 0.0 );
				float temp_output_19_0_g426 = max( ( ( temp_output_26_0_g426 + temp_output_27_0_g426 ) - temp_output_1_0_g426 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch30_g423 = ( ( ( tex2DNode16_g423 * temp_output_21_0_g426 ) + ( tex2DNode14_g423 * temp_output_19_0_g426 ) ) / ( temp_output_21_0_g426 + temp_output_19_0_g426 ) );
				#else
				float4 staticSwitch30_g423 = tex2DNode14_g423;
				#endif
				float temp_output_24_0_g437 = temp_output_302_3;
				float temp_output_25_0_g437 = temp_output_274_0;
				float temp_output_26_0_g437 = temp_output_299_3;
				float temp_output_27_0_g437 = break273.x;
				float temp_output_1_0_g437 = ( max( ( temp_output_24_0_g437 + temp_output_25_0_g437 ) , ( temp_output_26_0_g437 + temp_output_27_0_g437 ) ) - 0.2 );
				float temp_output_21_0_g437 = max( ( ( temp_output_24_0_g437 + temp_output_25_0_g437 ) - temp_output_1_0_g437 ) , 0.0 );
				float4 tex2DNode14_g419 = tex2D( _Tint, temp_output_12_0_g419 );
				float temp_output_24_0_g422 = tex2DArrayNode35_g419.b;
				float temp_output_25_0_g422 = tex2DNode16_g419.a;
				float temp_output_26_0_g422 = tex2DArrayNode32_g419.b;
				float temp_output_27_0_g422 = temp_output_41_0_g419;
				float temp_output_1_0_g422 = ( max( ( temp_output_24_0_g422 + temp_output_25_0_g422 ) , ( temp_output_26_0_g422 + temp_output_27_0_g422 ) ) - 0.2 );
				float temp_output_21_0_g422 = max( ( ( temp_output_24_0_g422 + temp_output_25_0_g422 ) - temp_output_1_0_g422 ) , 0.0 );
				float temp_output_19_0_g422 = max( ( ( temp_output_26_0_g422 + temp_output_27_0_g422 ) - temp_output_1_0_g422 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch30_g419 = ( ( ( tex2DNode16_g419 * temp_output_21_0_g422 ) + ( tex2DNode14_g419 * temp_output_19_0_g422 ) ) / ( temp_output_21_0_g422 + temp_output_19_0_g422 ) );
				#else
				float4 staticSwitch30_g419 = tex2DNode14_g419;
				#endif
				float temp_output_19_0_g437 = max( ( ( temp_output_26_0_g437 + temp_output_27_0_g437 ) - temp_output_1_0_g437 ) , 0.0 );
				float temp_output_24_0_g439 = temp_output_290_0.z;
				float temp_output_25_0_g439 = temp_output_275_0;
				float temp_output_26_0_g439 = temp_output_293_0.z;
				float temp_output_27_0_g439 = break273.y;
				float temp_output_1_0_g439 = ( max( ( temp_output_24_0_g439 + temp_output_25_0_g439 ) , ( temp_output_26_0_g439 + temp_output_27_0_g439 ) ) - 0.2 );
				float temp_output_21_0_g439 = max( ( ( temp_output_24_0_g439 + temp_output_25_0_g439 ) - temp_output_1_0_g439 ) , 0.0 );
				float4 tex2DNode14_g431 = tex2D( _Tint, temp_output_12_0_g431 );
				float temp_output_24_0_g434 = tex2DArrayNode35_g431.b;
				float temp_output_25_0_g434 = tex2DNode16_g431.a;
				float temp_output_26_0_g434 = tex2DArrayNode32_g431.b;
				float temp_output_27_0_g434 = temp_output_41_0_g431;
				float temp_output_1_0_g434 = ( max( ( temp_output_24_0_g434 + temp_output_25_0_g434 ) , ( temp_output_26_0_g434 + temp_output_27_0_g434 ) ) - 0.2 );
				float temp_output_21_0_g434 = max( ( ( temp_output_24_0_g434 + temp_output_25_0_g434 ) - temp_output_1_0_g434 ) , 0.0 );
				float temp_output_19_0_g434 = max( ( ( temp_output_26_0_g434 + temp_output_27_0_g434 ) - temp_output_1_0_g434 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch30_g431 = ( ( ( tex2DNode16_g431 * temp_output_21_0_g434 ) + ( tex2DNode14_g431 * temp_output_19_0_g434 ) ) / ( temp_output_21_0_g434 + temp_output_19_0_g434 ) );
				#else
				float4 staticSwitch30_g431 = tex2DNode14_g431;
				#endif
				float temp_output_24_0_g438 = temp_output_300_3;
				float temp_output_25_0_g438 = temp_output_274_0;
				float temp_output_26_0_g438 = temp_output_301_3;
				float temp_output_27_0_g438 = break273.x;
				float temp_output_1_0_g438 = ( max( ( temp_output_24_0_g438 + temp_output_25_0_g438 ) , ( temp_output_26_0_g438 + temp_output_27_0_g438 ) ) - 0.2 );
				float temp_output_21_0_g438 = max( ( ( temp_output_24_0_g438 + temp_output_25_0_g438 ) - temp_output_1_0_g438 ) , 0.0 );
				float4 tex2DNode14_g427 = tex2D( _Tint, temp_output_12_0_g427 );
				float temp_output_24_0_g430 = tex2DArrayNode35_g427.b;
				float temp_output_25_0_g430 = tex2DNode16_g427.a;
				float temp_output_26_0_g430 = tex2DArrayNode32_g427.b;
				float temp_output_27_0_g430 = temp_output_41_0_g427;
				float temp_output_1_0_g430 = ( max( ( temp_output_24_0_g430 + temp_output_25_0_g430 ) , ( temp_output_26_0_g430 + temp_output_27_0_g430 ) ) - 0.2 );
				float temp_output_21_0_g430 = max( ( ( temp_output_24_0_g430 + temp_output_25_0_g430 ) - temp_output_1_0_g430 ) , 0.0 );
				float temp_output_19_0_g430 = max( ( ( temp_output_26_0_g430 + temp_output_27_0_g430 ) - temp_output_1_0_g430 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch30_g427 = ( ( ( tex2DNode16_g427 * temp_output_21_0_g430 ) + ( tex2DNode14_g427 * temp_output_19_0_g430 ) ) / ( temp_output_21_0_g430 + temp_output_19_0_g430 ) );
				#else
				float4 staticSwitch30_g427 = tex2DNode14_g427;
				#endif
				float temp_output_19_0_g438 = max( ( ( temp_output_26_0_g438 + temp_output_27_0_g438 ) - temp_output_1_0_g438 ) , 0.0 );
				float temp_output_19_0_g439 = max( ( ( temp_output_26_0_g439 + temp_output_27_0_g439 ) - temp_output_1_0_g439 ) , 0.0 );
				float4 temp_output_221_0 = ( ( ( ( ( ( staticSwitch30_g423 * temp_output_21_0_g437 ) + ( staticSwitch30_g419 * temp_output_19_0_g437 ) ) / ( temp_output_21_0_g437 + temp_output_19_0_g437 ) ) * temp_output_21_0_g439 ) + ( ( ( ( staticSwitch30_g431 * temp_output_21_0_g438 ) + ( staticSwitch30_g427 * temp_output_19_0_g438 ) ) / ( temp_output_21_0_g438 + temp_output_19_0_g438 ) ) * temp_output_19_0_g439 ) ) / ( temp_output_21_0_g439 + temp_output_19_0_g439 ) );
				float temp_output_3_0_g440 = ( temp_output_221_0.w * 2.0 );
				
				surfaceDescription.Normal = unpack220;
				surfaceDescription.Smoothness = ( temp_output_159_0.w * _Rough );
				surfaceDescription.Alpha = min( temp_output_3_0_g440 , 1.0 );

				#ifdef _ALPHATEST_ON
				surfaceDescription.AlphaClipThreshold = _AlphaCutoff;
				#endif

				#ifdef _DEPTHOFFSET_ON
				surfaceDescription.DepthOffset = 0;
				#endif

				SurfaceData surfaceData;
				BuiltinData builtinData;
				GetSurfaceAndBuiltinData(surfaceDescription, input, V, posInput, surfaceData, builtinData);

				#ifdef _DEPTHOFFSET_ON
				outputDepth = posInput.deviceDepth;
				#endif

				#ifdef WRITE_MSAA_DEPTH	
					depthColor = packedInput.positionCS.z;
					#ifdef _ALPHATOMASK_ON
						depthColor.a = SharpenAlpha(builtinData.opacity, builtinData.alphaClipTreshold);
					#endif
				#endif

				#if defined(WRITE_NORMAL_BUFFER)
				EncodeIntoNormalBuffer(ConvertSurfaceDataToNormalData(surfaceData), outNormalBuffer);
				#endif

				#if defined(WRITE_DECAL_BUFFER) && !defined(_DISABLE_DECALS)
				DecalPrepassData decalPrepassData;
				decalPrepassData.geomNormalWS = surfaceData.geomNormalWS;
				decalPrepassData.decalLayerMask = GetMeshRenderingDecalLayer();
				EncodeIntoDecalPrepassBuffer(decalPrepassData, outDecalBuffer);
				#endif
			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "Motion Vectors"
			Tags { "LightMode"="MotionVectors" }
			// DONE
			Cull [_CullMode]

			ZWrite On

			Stencil
			{
				Ref [_StencilRefMV]
				WriteMask [_StencilWriteMaskMV]
				Comp Always
				Pass Replace
				Fail Keep
				ZFail Keep
			}


			HLSLPROGRAM

			#define _SPECULAR_OCCLUSION_FROM_AO 1
			#define _DISABLE_DECALS 1
			#define _DISABLE_SSR 1
			#define _AMBIENT_OCCLUSION 1
			#define ASE_SRP_VERSION 999999


			#pragma shader_feature _SURFACE_TYPE_TRANSPARENT
			#pragma shader_feature_local _DOUBLESIDED_ON
			#pragma shader_feature_local _BLENDMODE_OFF _BLENDMODE_ALPHA _BLENDMODE_ADD _BLENDMODE_PRE_MULTIPLY
			#pragma shader_feature_local_fragment _ _ENABLE_FOG_ON_TRANSPARENT
			#pragma shader_feature_local _ALPHATEST_ON
			#pragma shader_feature_local _ _TRANSPARENT_WRITES_MOTION_VEC

			#define SHADERPASS SHADERPASS_MOTION_VECTORS
			#pragma multi_compile _ WRITE_NORMAL_BUFFER
			#pragma multi_compile_fragment _ WRITE_MSAA_DEPTH
			#pragma multi_compile _ WRITE_DECAL_BUFFER

			#pragma vertex Vert
			#pragma fragment Frag
			
			//#define UNITY_MATERIAL_LIT

			#if defined(_MATERIAL_FEATURE_SUBSURFACE_SCATTERING) && !defined(_SURFACE_TYPE_TRANSPARENT)
			#define OUTPUT_SPLIT_LIGHTING
			#endif

			#if defined(SHADER_LIT) && !defined(_SURFACE_TYPE_TRANSPARENT)
                #define _DEFERRED_CAPABLE_MATERIAL
            #endif
        
            
            #if defined(_TRANSPARENT_WRITES_MOTION_VEC) && defined(_SURFACE_TYPE_TRANSPARENT)
                #define _WRITE_TRANSPARENT_MOTION_VECTOR
            #endif
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
        	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/GeometricTools.hlsl" 
        	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Tessellation.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/FragInputs.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl" 
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderGraphHeader.hlsl"

			#ifdef DEBUG_DISPLAY
				#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Debug/DebugDisplay.hlsl"
			#endif
			
			CBUFFER_START( UnityPerMaterial )
			float4 _MatTexArray_ST;
			float4 _WorldBounds;
			float4 _Control_TexelSize;
			float4 _ShapeMap_ST;
			float _NormalScale;
			float _MetalLevel;
			float _Rough;
			float4 _EmissionColor;
			float _AlphaCutoff;
			float _RenderQueueType;
			#ifdef _ADD_PRECOMPUTED_VELOCITY
			float _AddPrecomputedVelocity;
			#endif
			float _StencilRef;
			float _StencilWriteMask;
			float _StencilRefDepth;
			float _StencilWriteMaskDepth;
			float _StencilRefMV;
			float _StencilWriteMaskMV;
			float _StencilRefDistortionVec;
			float _StencilWriteMaskDistortionVec;
			float _StencilWriteMaskGBuffer;
			float _StencilRefGBuffer;
			float _ZTestGBuffer;
			float _RequireSplitLighting;
			float _ReceivesSSR;
			float _SurfaceType;
			float _BlendMode;
			float _SrcBlend;
			float _DstBlend;
			float _AlphaSrcBlend;
			float _AlphaDstBlend;
			float _ZWrite;
			float _TransparentZWrite;
			float _CullMode;
			float _TransparentSortPriority;
			float _EnableFogOnTransparent;
			float _CullModeForward;
			float _TransparentCullMode;
			float _ZTestDepthEqualForOpaque;
			float _ZTestTransparent;
			float _TransparentBackfaceEnable;
			float _AlphaCutoffEnable;
			float _UseShadowThreshold;
			float _DoubleSidedEnable;
			float _DoubleSidedNormalMode;
			float4 _DoubleSidedConstants;
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			TEXTURE2D_ARRAY(_ShapeMap);
			sampler2D _Control;
			SAMPLER(sampler_ShapeMap);
			sampler2D _GrassControl;
			sampler2D _GrassTint;
			TEXTURE2D_ARRAY(_MatTexArray);
			SAMPLER(sampler_MatTexArray);
			sampler2D _Tint;


			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/NormalSurfaceGradient.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Lit/Lit.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/BuiltinUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/MaterialUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Decal/DecalUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Lit/LitDecalData.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderGraphFunctions.hlsl"

			#define GRASS


			#if defined(_DOUBLESIDED_ON) && !defined(ASE_NEED_CULLFACE)
				#define ASE_NEED_CULLFACE 1
			#endif


			struct AttributesMesh
			{
				float3 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float3 previousPositionOS : TEXCOORD4;
				#if defined (_ADD_PRECOMPUTED_VELOCITY)
					float3 precomputedVelocity : TEXCOORD5;
				#endif
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryingsMeshToPS
			{
				float4 vmeshPositionCS : SV_Position;
				float3 vmeshInterp00 : TEXCOORD0;
				float3 vpassInterpolators0 : TEXCOORD1; //interpolators0
				float3 vpassInterpolators1 : TEXCOORD2; //interpolators1
				float4 ase_texcoord3 : TEXCOORD3;
				float4 ase_texcoord4 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
				#if defined(SHADER_STAGE_FRAGMENT) && defined(ASE_NEED_CULLFACE)
				FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
				#endif
			};


			
			void BuildSurfaceData(FragInputs fragInputs, inout SmoothSurfaceDescription surfaceDescription, float3 V, PositionInputs posInput, out SurfaceData surfaceData, out float3 bentNormalWS)
			{
				ZERO_INITIALIZE(SurfaceData, surfaceData);

				surfaceData.specularOcclusion = 1.0;

				// surface data
				surfaceData.perceptualSmoothness =		surfaceDescription.Smoothness;

				// refraction
				#ifdef _HAS_REFRACTION
				if( _EnableSSRefraction )
				{
					surfaceData.transmittanceMask = ( 1.0 - surfaceDescription.Alpha );
					surfaceDescription.Alpha = 1.0;
				}
				else
				{
					surfaceData.ior = 1.0;
					surfaceData.transmittanceColor = float3( 1.0, 1.0, 1.0 );
					surfaceData.atDistance = 1.0;
					surfaceData.transmittanceMask = 0.0;
					surfaceDescription.Alpha = 1.0;
				}
				#else
				surfaceData.ior = 1.0;
				surfaceData.transmittanceColor = float3( 1.0, 1.0, 1.0 );
				surfaceData.atDistance = 1.0;
				surfaceData.transmittanceMask = 0.0;
				#endif


				// material features
				surfaceData.materialFeatures = MATERIALFEATUREFLAGS_LIT_STANDARD;
				#ifdef _MATERIAL_FEATURE_SUBSURFACE_SCATTERING
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_SUBSURFACE_SCATTERING;
				#endif
				#ifdef _MATERIAL_FEATURE_TRANSMISSION
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_TRANSMISSION;
				#endif
				#ifdef _MATERIAL_FEATURE_ANISOTROPY
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_ANISOTROPY;
				#endif
				#ifdef ASE_LIT_CLEAR_COAT
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_CLEAR_COAT;
				#endif
				#ifdef _MATERIAL_FEATURE_IRIDESCENCE
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_IRIDESCENCE;
				#endif
				#ifdef _MATERIAL_FEATURE_SPECULAR_COLOR
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_SPECULAR_COLOR;
				#endif

				// others
				#if defined (_MATERIAL_FEATURE_SPECULAR_COLOR) && defined (_ENERGY_CONSERVING_SPECULAR)
				surfaceData.baseColor *= ( 1.0 - Max3( surfaceData.specularColor.r, surfaceData.specularColor.g, surfaceData.specularColor.b ) );
				#endif
				#ifdef _DOUBLESIDED_ON
				float3 doubleSidedConstants = _DoubleSidedConstants.xyz;
				#else
				float3 doubleSidedConstants = float3( 1.0, 1.0, 1.0 );
				#endif

				// normals
				float3 normalTS = float3(0.0f, 0.0f, 1.0f);
				normalTS = surfaceDescription.Normal;
				GetNormalWS( fragInputs, normalTS, surfaceData.normalWS, doubleSidedConstants );

				surfaceData.geomNormalWS = fragInputs.tangentToWorld[2];
				surfaceData.tangentWS = normalize( fragInputs.tangentToWorld[ 0 ].xyz );

				// decals
				#if HAVE_DECALS
				if( _EnableDecals )
				{
					DecalSurfaceData decalSurfaceData = GetDecalSurfaceData(posInput, fragInputs.tangentToWorld[2], surfaceDescription.Alpha);
					ApplyDecalToSurfaceData(decalSurfaceData, fragInputs.tangentToWorld[2], surfaceData);
				}
				#endif

				bentNormalWS = surfaceData.normalWS;
				surfaceData.tangentWS = Orthonormalize( surfaceData.tangentWS, surfaceData.normalWS );

				#if defined(_SPECULAR_OCCLUSION_CUSTOM)
				#elif defined(_SPECULAR_OCCLUSION_FROM_AO_BENT_NORMAL)
				surfaceData.specularOcclusion = GetSpecularOcclusionFromBentAO( V, bentNormalWS, surfaceData.normalWS, surfaceData.ambientOcclusion, PerceptualSmoothnessToPerceptualRoughness( surfaceData.perceptualSmoothness ) );
				#elif defined(_AMBIENT_OCCLUSION) && defined(_SPECULAR_OCCLUSION_FROM_AO)
				surfaceData.specularOcclusion = GetSpecularOcclusionFromAmbientOcclusion( ClampNdotV( dot( surfaceData.normalWS, V ) ), surfaceData.ambientOcclusion, PerceptualSmoothnessToRoughness( surfaceData.perceptualSmoothness ) );
				#endif

				// debug
				#if defined(DEBUG_DISPLAY)
				if (_DebugMipMapMode != DEBUGMIPMAPMODE_NONE)
				{
					surfaceData.metallic = 0;
				}
				ApplyDebugToSurfaceData(fragInputs.tangentToWorld, surfaceData);
				#endif
			}

			void GetSurfaceAndBuiltinData(SmoothSurfaceDescription surfaceDescription, FragInputs fragInputs, float3 V, inout PositionInputs posInput, out SurfaceData surfaceData, out BuiltinData builtinData)
			{
				#ifdef LOD_FADE_CROSSFADE
				LODDitheringTransition(ComputeFadeMaskSeed(V, posInput.positionSS), unity_LODFade.x);
				#endif

				#ifdef _DOUBLESIDED_ON
				float3 doubleSidedConstants = _DoubleSidedConstants.xyz;
				#else
				float3 doubleSidedConstants = float3( 1.0, 1.0, 1.0 );
				#endif

				ApplyDoubleSidedFlipOrMirror( fragInputs, doubleSidedConstants );

				#ifdef _ALPHATEST_ON
				DoAlphaTest( surfaceDescription.Alpha, surfaceDescription.AlphaClipThreshold );
				#endif

				#ifdef _DEPTHOFFSET_ON
				builtinData.depthOffset = surfaceDescription.DepthOffset;
				ApplyDepthOffsetPositionInput( V, surfaceDescription.DepthOffset, GetViewForwardDir(), GetWorldToHClipMatrix(), posInput );
				#endif

				float3 bentNormalWS;
				BuildSurfaceData( fragInputs, surfaceDescription, V, posInput, surfaceData, bentNormalWS );

				InitBuiltinData( posInput, surfaceDescription.Alpha, bentNormalWS, -fragInputs.tangentToWorld[ 2 ], fragInputs.texCoord1, fragInputs.texCoord2, builtinData );

				PostInitBuiltinData(V, posInput, surfaceData, builtinData);
			}

			AttributesMesh ApplyMeshModification(AttributesMesh inputMesh, float3 timeParameters, inout PackedVaryingsMeshToPS outputPackedVaryingsMeshToPS )
			{
				_TimeParameters.xyz = timeParameters;
				float3 ase_worldPos = GetAbsolutePositionWS( TransformObjectToWorld( (inputMesh.positionOS).xyz ) );
				outputPackedVaryingsMeshToPS.ase_texcoord4.xyz = ase_worldPos;
				
				outputPackedVaryingsMeshToPS.ase_texcoord3.xy = inputMesh.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				outputPackedVaryingsMeshToPS.ase_texcoord3.zw = 0;
				outputPackedVaryingsMeshToPS.ase_texcoord4.w = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
				float3 defaultVertexValue = inputMesh.positionOS.xyz;
				#else
				float3 defaultVertexValue = float3( 0, 0, 0 );
				#endif
				float3 vertexValue =  defaultVertexValue ;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
				inputMesh.positionOS.xyz = vertexValue;
				#else
				inputMesh.positionOS.xyz += vertexValue;
				#endif
				inputMesh.normalOS =  inputMesh.normalOS ;
				return inputMesh;
			}

			PackedVaryingsMeshToPS VertexFunction(AttributesMesh inputMesh)
			{
				PackedVaryingsMeshToPS outputPackedVaryingsMeshToPS = (PackedVaryingsMeshToPS)0;
				AttributesMesh defaultMesh = inputMesh;

				UNITY_SETUP_INSTANCE_ID(inputMesh);
				UNITY_TRANSFER_INSTANCE_ID(inputMesh, outputPackedVaryingsMeshToPS);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( outputPackedVaryingsMeshToPS );

				inputMesh = ApplyMeshModification( inputMesh, _TimeParameters.xyz, outputPackedVaryingsMeshToPS);

				float3 positionRWS = TransformObjectToWorld(inputMesh.positionOS);
				float3 normalWS = TransformObjectToWorldNormal(inputMesh.normalOS);

				float3 VMESHpositionRWS = positionRWS;
				float4 VMESHpositionCS = TransformWorldToHClip(positionRWS);

				#if UNITY_REVERSED_Z
				VMESHpositionCS.z -= unity_MotionVectorsParams.z * VMESHpositionCS.w;
				#else
				VMESHpositionCS.z += unity_MotionVectorsParams.z * VMESHpositionCS.w;
				#endif

				float4 VPASSpreviousPositionCS;
				float4 VPASSpositionCS = mul(UNITY_MATRIX_UNJITTERED_VP, float4(VMESHpositionRWS, 1.0));

				bool forceNoMotion = unity_MotionVectorsParams.y == 0.0;
				if (forceNoMotion)
				{
					VPASSpreviousPositionCS = float4(0.0, 0.0, 0.0, 1.0);
				}
				else
				{
					bool hasDeformation = unity_MotionVectorsParams.x > 0.0;
					float3 effectivePositionOS = (hasDeformation ? inputMesh.previousPositionOS : defaultMesh.positionOS);
					#if defined(_ADD_PRECOMPUTED_VELOCITY)
					effectivePositionOS -= inputMesh.precomputedVelocity;
					#endif

					#if defined(HAVE_MESH_MODIFICATION)
						AttributesMesh previousMesh = defaultMesh;
						previousMesh.positionOS = effectivePositionOS ;
						PackedVaryingsMeshToPS test = (PackedVaryingsMeshToPS)0;
						float3 curTime = _TimeParameters.xyz;
						previousMesh = ApplyMeshModification(previousMesh, _LastTimeParameters.xyz, test);
						_TimeParameters.xyz = curTime;
						float3 previousPositionRWS = TransformPreviousObjectToWorld(previousMesh.positionOS);
					#else
						float3 previousPositionRWS = TransformPreviousObjectToWorld(effectivePositionOS);
					#endif

					#ifdef ATTRIBUTES_NEED_NORMAL
						float3 normalWS = TransformPreviousObjectToWorldNormal(defaultMesh.normalOS);
					#else
						float3 normalWS = float3(0.0, 0.0, 0.0);
					#endif

					#if defined(HAVE_VERTEX_MODIFICATION)
						//ApplyVertexModification(inputMesh, normalWS, previousPositionRWS, _LastTimeParameters.xyz);
					#endif

					#ifdef _WRITE_TRANSPARENT_MOTION_VECTOR
						if (_TransparentCameraOnlyMotionVectors > 0)
						{
							previousPositionRWS = VMESHpositionRWS.xyz;
						}
					#endif

					VPASSpreviousPositionCS = mul(UNITY_MATRIX_PREV_VP, float4(previousPositionRWS, 1.0));
				}

				outputPackedVaryingsMeshToPS.vmeshPositionCS = VMESHpositionCS;
				outputPackedVaryingsMeshToPS.vmeshInterp00.xyz = VMESHpositionRWS;

				outputPackedVaryingsMeshToPS.vpassInterpolators0 = float3(VPASSpositionCS.xyw);
				outputPackedVaryingsMeshToPS.vpassInterpolators1 = float3(VPASSpreviousPositionCS.xyw);
				return outputPackedVaryingsMeshToPS;
			}

			#if defined(WRITE_DECAL_BUFFER) && !defined(_DISABLE_DECALS)
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Decal/DecalPrepassBuffer.hlsl"
			#endif


			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float3 positionOS : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float3 previousPositionOS : TEXCOORD4;
				#if defined (_ADD_PRECOMPUTED_VELOCITY)
					float3 precomputedVelocity : TEXCOORD5;
				#endif
				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl Vert ( AttributesMesh v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.positionOS = v.positionOS;
				o.normalOS = v.normalOS;
				o.previousPositionOS = v.previousPositionOS;
				#if defined (_ADD_PRECOMPUTED_VELOCITY)
				o.precomputedVelocity = v.precomputedVelocity;
				#endif
				o.ase_texcoord = v.ase_texcoord;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if (SHADEROPTIONS_CAMERA_RELATIVE_RENDERING != 0)
				float3 cameraPos = 0;
				#else
				float3 cameraPos = _WorldSpaceCameraPos;
				#endif
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(float4(v[0].positionOS,1), float4(v[1].positionOS,1), float4(v[2].positionOS,1), tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), cameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(float4(v[0].positionOS,1), float4(v[1].positionOS,1), float4(v[2].positionOS,1), edgeLength, GetObjectToWorldMatrix(), cameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(float4(v[0].positionOS,1), float4(v[1].positionOS,1), float4(v[2].positionOS,1), edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), cameraPos, _ScreenParams, _FrustumPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			PackedVaryingsMeshToPS DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				AttributesMesh o = (AttributesMesh) 0;
				o.positionOS = patch[0].positionOS * bary.x + patch[1].positionOS * bary.y + patch[2].positionOS * bary.z;
				o.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				o.previousPositionOS = patch[0].previousPositionOS * bary.x + patch[1].previousPositionOS * bary.y + patch[2].previousPositionOS * bary.z;
				#if defined (_ADD_PRECOMPUTED_VELOCITY)
					o.precomputedVelocity = patch[0].precomputedVelocity * bary.x + patch[1].precomputedVelocity * bary.y + patch[2].precomputedVelocity * bary.z;
				#endif
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.positionOS.xyz - patch[i].normalOS * (dot(o.positionOS.xyz, patch[i].normalOS) - dot(patch[i].positionOS.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				o.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			PackedVaryingsMeshToPS Vert ( AttributesMesh v )
			{
				return VertexFunction( v );
			}
			#endif

			#if defined(WRITE_DECAL_BUFFER) && defined(WRITE_MSAA_DEPTH)
			#define SV_TARGET_NORMAL SV_Target3
			#elif defined(WRITE_DECAL_BUFFER) || defined(WRITE_MSAA_DEPTH)
			#define SV_TARGET_NORMAL SV_Target2
			#else
			#define SV_TARGET_NORMAL SV_Target1
			#endif

			void Frag( PackedVaryingsMeshToPS packedInput
				#ifdef WRITE_MSAA_DEPTH
					, out float4 depthColor : SV_Target0
					, out float4 outMotionVector : SV_Target1
						#ifdef WRITE_DECAL_BUFFER
						, out float4 outDecalBuffer : SV_Target2
						#endif
					#else
					, out float4 outMotionVector : SV_Target0
						#ifdef WRITE_DECAL_BUFFER
						, out float4 outDecalBuffer : SV_Target1
						#endif
					#endif

					#ifdef WRITE_NORMAL_BUFFER
					, out float4 outNormalBuffer : SV_TARGET_NORMAL
					#endif

					#ifdef _DEPTHOFFSET_ON
					, out float outputDepth : SV_Depth
					#endif
				
				)
			{
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( packedInput );
				UNITY_SETUP_INSTANCE_ID( packedInput );
				FragInputs input;
				ZERO_INITIALIZE(FragInputs, input);
				input.tangentToWorld = k_identity3x3;
				input.positionSS = packedInput.vmeshPositionCS;
				input.positionRWS = packedInput.vmeshInterp00.xyz;

				PositionInputs posInput = GetPositionInput(input.positionSS.xy, _ScreenSize.zw, input.positionSS.z, input.positionSS.w, input.positionRWS);

				float3 V = GetWorldSpaceNormalizeViewDir(input.positionRWS);

				SurfaceData surfaceData;
				BuiltinData builtinData;

				SmoothSurfaceDescription surfaceDescription = (SmoothSurfaceDescription)0;
				float2 uv_ShapeMap = packedInput.ase_texcoord3.xy;
				float3 ase_worldPos = packedInput.ase_texcoord4.xyz;
				float2 appendResult257 = (float2(ase_worldPos.x , ase_worldPos.z));
				float2 appendResult260 = (float2(_WorldBounds.x , _WorldBounds.y));
				float2 appendResult259 = (float2(_WorldBounds.z , _WorldBounds.w));
				float2 temp_output_264_0 = ( ( appendResult257 - appendResult260 ) / ( appendResult259 - appendResult260 ) );
				float2 appendResult265 = (float2(_Control_TexelSize.z , _Control_TexelSize.w));
				float2 temp_output_266_0 = ( temp_output_264_0 * appendResult265 );
				float2 temp_output_270_0 = floor( ( ( temp_output_266_0 + float2( 0.5,0.5 ) ) - float2( 0.5,0.5 ) ) );
				float2 temp_output_12_0_g423 = ( ( temp_output_270_0 + float2( 0,0 ) ) / appendResult265 );
				float4 tex2DNode6_g423 = tex2D( _Control, temp_output_12_0_g423 );
				float4 tex2DArrayNode32_g423 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode6_g423.g * 255.0 ) );
				float4 tex2DNode15_g423 = tex2D( _GrassControl, temp_output_12_0_g423 );
				float4 tex2DArrayNode35_g423 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode15_g423.g * 255.0 ) );
				float temp_output_24_0_g424 = tex2DArrayNode35_g423.b;
				float4 tex2DNode16_g423 = tex2D( _GrassTint, temp_output_12_0_g423 );
				float temp_output_25_0_g424 = tex2DNode16_g423.a;
				float temp_output_26_0_g424 = tex2DArrayNode32_g423.b;
				float temp_output_41_0_g423 = ( 1.0 - tex2DNode16_g423.a );
				float temp_output_27_0_g424 = temp_output_41_0_g423;
				float temp_output_1_0_g424 = ( max( ( temp_output_24_0_g424 + temp_output_25_0_g424 ) , ( temp_output_26_0_g424 + temp_output_27_0_g424 ) ) - 0.2 );
				float temp_output_21_0_g424 = max( ( ( temp_output_24_0_g424 + temp_output_25_0_g424 ) - temp_output_1_0_g424 ) , 0.0 );
				float temp_output_19_0_g424 = max( ( ( temp_output_26_0_g424 + temp_output_27_0_g424 ) - temp_output_1_0_g424 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch31_g423 = ( ( ( tex2DArrayNode35_g423 * temp_output_21_0_g424 ) + ( tex2DArrayNode32_g423 * temp_output_19_0_g424 ) ) / ( temp_output_21_0_g424 + temp_output_19_0_g424 ) );
				#else
				float4 staticSwitch31_g423 = tex2DArrayNode32_g423;
				#endif
				float temp_output_302_3 = staticSwitch31_g423.b;
				float temp_output_24_0_g436 = temp_output_302_3;
				float2 break273 = ( temp_output_266_0 - temp_output_270_0 );
				float temp_output_274_0 = ( 1.0 - break273.x );
				float temp_output_25_0_g436 = temp_output_274_0;
				float2 temp_output_12_0_g419 = ( ( temp_output_270_0 + float2( 1,0 ) ) / appendResult265 );
				float4 tex2DNode6_g419 = tex2D( _Control, temp_output_12_0_g419 );
				float4 tex2DArrayNode32_g419 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode6_g419.g * 255.0 ) );
				float4 tex2DNode15_g419 = tex2D( _GrassControl, temp_output_12_0_g419 );
				float4 tex2DArrayNode35_g419 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode15_g419.g * 255.0 ) );
				float temp_output_24_0_g420 = tex2DArrayNode35_g419.b;
				float4 tex2DNode16_g419 = tex2D( _GrassTint, temp_output_12_0_g419 );
				float temp_output_25_0_g420 = tex2DNode16_g419.a;
				float temp_output_26_0_g420 = tex2DArrayNode32_g419.b;
				float temp_output_41_0_g419 = ( 1.0 - tex2DNode16_g419.a );
				float temp_output_27_0_g420 = temp_output_41_0_g419;
				float temp_output_1_0_g420 = ( max( ( temp_output_24_0_g420 + temp_output_25_0_g420 ) , ( temp_output_26_0_g420 + temp_output_27_0_g420 ) ) - 0.2 );
				float temp_output_21_0_g420 = max( ( ( temp_output_24_0_g420 + temp_output_25_0_g420 ) - temp_output_1_0_g420 ) , 0.0 );
				float temp_output_19_0_g420 = max( ( ( temp_output_26_0_g420 + temp_output_27_0_g420 ) - temp_output_1_0_g420 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch31_g419 = ( ( ( tex2DArrayNode35_g419 * temp_output_21_0_g420 ) + ( tex2DArrayNode32_g419 * temp_output_19_0_g420 ) ) / ( temp_output_21_0_g420 + temp_output_19_0_g420 ) );
				#else
				float4 staticSwitch31_g419 = tex2DArrayNode32_g419;
				#endif
				float temp_output_299_3 = staticSwitch31_g419.b;
				float temp_output_26_0_g436 = temp_output_299_3;
				float temp_output_27_0_g436 = break273.x;
				float temp_output_1_0_g436 = ( max( ( temp_output_24_0_g436 + temp_output_25_0_g436 ) , ( temp_output_26_0_g436 + temp_output_27_0_g436 ) ) - 0.2 );
				float temp_output_21_0_g436 = max( ( ( temp_output_24_0_g436 + temp_output_25_0_g436 ) - temp_output_1_0_g436 ) , 0.0 );
				float temp_output_19_0_g436 = max( ( ( temp_output_26_0_g436 + temp_output_27_0_g436 ) - temp_output_1_0_g436 ) , 0.0 );
				float4 temp_output_290_0 = ( ( ( staticSwitch31_g423 * temp_output_21_0_g436 ) + ( staticSwitch31_g419 * temp_output_19_0_g436 ) ) / ( temp_output_21_0_g436 + temp_output_19_0_g436 ) );
				float temp_output_24_0_g441 = temp_output_290_0.z;
				float temp_output_275_0 = ( 1.0 - break273.y );
				float temp_output_25_0_g441 = temp_output_275_0;
				float2 temp_output_12_0_g431 = ( ( temp_output_270_0 + float2( 0,1 ) ) / appendResult265 );
				float4 tex2DNode6_g431 = tex2D( _Control, temp_output_12_0_g431 );
				float4 tex2DArrayNode32_g431 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode6_g431.g * 255.0 ) );
				float4 tex2DNode15_g431 = tex2D( _GrassControl, temp_output_12_0_g431 );
				float4 tex2DArrayNode35_g431 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode15_g431.g * 255.0 ) );
				float temp_output_24_0_g432 = tex2DArrayNode35_g431.b;
				float4 tex2DNode16_g431 = tex2D( _GrassTint, temp_output_12_0_g431 );
				float temp_output_25_0_g432 = tex2DNode16_g431.a;
				float temp_output_26_0_g432 = tex2DArrayNode32_g431.b;
				float temp_output_41_0_g431 = ( 1.0 - tex2DNode16_g431.a );
				float temp_output_27_0_g432 = temp_output_41_0_g431;
				float temp_output_1_0_g432 = ( max( ( temp_output_24_0_g432 + temp_output_25_0_g432 ) , ( temp_output_26_0_g432 + temp_output_27_0_g432 ) ) - 0.2 );
				float temp_output_21_0_g432 = max( ( ( temp_output_24_0_g432 + temp_output_25_0_g432 ) - temp_output_1_0_g432 ) , 0.0 );
				float temp_output_19_0_g432 = max( ( ( temp_output_26_0_g432 + temp_output_27_0_g432 ) - temp_output_1_0_g432 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch31_g431 = ( ( ( tex2DArrayNode35_g431 * temp_output_21_0_g432 ) + ( tex2DArrayNode32_g431 * temp_output_19_0_g432 ) ) / ( temp_output_21_0_g432 + temp_output_19_0_g432 ) );
				#else
				float4 staticSwitch31_g431 = tex2DArrayNode32_g431;
				#endif
				float temp_output_300_3 = staticSwitch31_g431.b;
				float temp_output_24_0_g435 = temp_output_300_3;
				float temp_output_25_0_g435 = temp_output_274_0;
				float2 temp_output_12_0_g427 = ( ( temp_output_270_0 + float2( 1,1 ) ) / appendResult265 );
				float4 tex2DNode6_g427 = tex2D( _Control, temp_output_12_0_g427 );
				float4 tex2DArrayNode32_g427 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode6_g427.g * 255.0 ) );
				float4 tex2DNode15_g427 = tex2D( _GrassControl, temp_output_12_0_g427 );
				float4 tex2DArrayNode35_g427 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode15_g427.g * 255.0 ) );
				float temp_output_24_0_g428 = tex2DArrayNode35_g427.b;
				float4 tex2DNode16_g427 = tex2D( _GrassTint, temp_output_12_0_g427 );
				float temp_output_25_0_g428 = tex2DNode16_g427.a;
				float temp_output_26_0_g428 = tex2DArrayNode32_g427.b;
				float temp_output_41_0_g427 = ( 1.0 - tex2DNode16_g427.a );
				float temp_output_27_0_g428 = temp_output_41_0_g427;
				float temp_output_1_0_g428 = ( max( ( temp_output_24_0_g428 + temp_output_25_0_g428 ) , ( temp_output_26_0_g428 + temp_output_27_0_g428 ) ) - 0.2 );
				float temp_output_21_0_g428 = max( ( ( temp_output_24_0_g428 + temp_output_25_0_g428 ) - temp_output_1_0_g428 ) , 0.0 );
				float temp_output_19_0_g428 = max( ( ( temp_output_26_0_g428 + temp_output_27_0_g428 ) - temp_output_1_0_g428 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch31_g427 = ( ( ( tex2DArrayNode35_g427 * temp_output_21_0_g428 ) + ( tex2DArrayNode32_g427 * temp_output_19_0_g428 ) ) / ( temp_output_21_0_g428 + temp_output_19_0_g428 ) );
				#else
				float4 staticSwitch31_g427 = tex2DArrayNode32_g427;
				#endif
				float temp_output_301_3 = staticSwitch31_g427.b;
				float temp_output_26_0_g435 = temp_output_301_3;
				float temp_output_27_0_g435 = break273.x;
				float temp_output_1_0_g435 = ( max( ( temp_output_24_0_g435 + temp_output_25_0_g435 ) , ( temp_output_26_0_g435 + temp_output_27_0_g435 ) ) - 0.2 );
				float temp_output_21_0_g435 = max( ( ( temp_output_24_0_g435 + temp_output_25_0_g435 ) - temp_output_1_0_g435 ) , 0.0 );
				float temp_output_19_0_g435 = max( ( ( temp_output_26_0_g435 + temp_output_27_0_g435 ) - temp_output_1_0_g435 ) , 0.0 );
				float4 temp_output_293_0 = ( ( ( staticSwitch31_g431 * temp_output_21_0_g435 ) + ( staticSwitch31_g427 * temp_output_19_0_g435 ) ) / ( temp_output_21_0_g435 + temp_output_19_0_g435 ) );
				float temp_output_26_0_g441 = temp_output_293_0.z;
				float temp_output_27_0_g441 = break273.y;
				float temp_output_1_0_g441 = ( max( ( temp_output_24_0_g441 + temp_output_25_0_g441 ) , ( temp_output_26_0_g441 + temp_output_27_0_g441 ) ) - 0.2 );
				float temp_output_21_0_g441 = max( ( ( temp_output_24_0_g441 + temp_output_25_0_g441 ) - temp_output_1_0_g441 ) , 0.0 );
				float temp_output_19_0_g441 = max( ( ( temp_output_26_0_g441 + temp_output_27_0_g441 ) - temp_output_1_0_g441 ) , 0.0 );
				float4 temp_output_180_0 = ( ( ( temp_output_290_0 * temp_output_21_0_g441 ) + ( temp_output_293_0 * temp_output_19_0_g441 ) ) / ( temp_output_21_0_g441 + temp_output_19_0_g441 ) );
				float3 unpack220 = UnpackNormalScale( temp_output_180_0, _NormalScale );
				unpack220.z = lerp( 1, unpack220.z, saturate(_NormalScale) );
				
				float2 uv_MatTexArray = packedInput.ase_texcoord3.xy;
				float4 tex2DArrayNode23_g423 = SAMPLE_TEXTURE2D_ARRAY( _MatTexArray, sampler_MatTexArray, uv_MatTexArray,( tex2DNode6_g423.r * 255.0 ) );
				float temp_output_24_0_g425 = tex2DArrayNode35_g423.b;
				float temp_output_25_0_g425 = tex2DNode16_g423.a;
				float temp_output_26_0_g425 = tex2DArrayNode32_g423.b;
				float temp_output_27_0_g425 = temp_output_41_0_g423;
				float temp_output_1_0_g425 = ( max( ( temp_output_24_0_g425 + temp_output_25_0_g425 ) , ( temp_output_26_0_g425 + temp_output_27_0_g425 ) ) - 0.2 );
				float temp_output_21_0_g425 = max( ( ( temp_output_24_0_g425 + temp_output_25_0_g425 ) - temp_output_1_0_g425 ) , 0.0 );
				float temp_output_19_0_g425 = max( ( ( temp_output_26_0_g425 + temp_output_27_0_g425 ) - temp_output_1_0_g425 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch29_g423 = ( ( ( SAMPLE_TEXTURE2D_ARRAY( _MatTexArray, sampler_MatTexArray, uv_MatTexArray,( tex2DNode15_g423.r * 255.0 ) ) * temp_output_21_0_g425 ) + ( tex2DArrayNode23_g423 * temp_output_19_0_g425 ) ) / ( temp_output_21_0_g425 + temp_output_19_0_g425 ) );
				#else
				float4 staticSwitch29_g423 = tex2DArrayNode23_g423;
				#endif
				float temp_output_24_0_g443 = temp_output_302_3;
				float temp_output_25_0_g443 = temp_output_274_0;
				float temp_output_26_0_g443 = temp_output_299_3;
				float temp_output_27_0_g443 = break273.x;
				float temp_output_1_0_g443 = ( max( ( temp_output_24_0_g443 + temp_output_25_0_g443 ) , ( temp_output_26_0_g443 + temp_output_27_0_g443 ) ) - 0.2 );
				float temp_output_21_0_g443 = max( ( ( temp_output_24_0_g443 + temp_output_25_0_g443 ) - temp_output_1_0_g443 ) , 0.0 );
				float4 tex2DArrayNode23_g419 = SAMPLE_TEXTURE2D_ARRAY( _MatTexArray, sampler_MatTexArray, uv_MatTexArray,( tex2DNode6_g419.r * 255.0 ) );
				float temp_output_24_0_g421 = tex2DArrayNode35_g419.b;
				float temp_output_25_0_g421 = tex2DNode16_g419.a;
				float temp_output_26_0_g421 = tex2DArrayNode32_g419.b;
				float temp_output_27_0_g421 = temp_output_41_0_g419;
				float temp_output_1_0_g421 = ( max( ( temp_output_24_0_g421 + temp_output_25_0_g421 ) , ( temp_output_26_0_g421 + temp_output_27_0_g421 ) ) - 0.2 );
				float temp_output_21_0_g421 = max( ( ( temp_output_24_0_g421 + temp_output_25_0_g421 ) - temp_output_1_0_g421 ) , 0.0 );
				float temp_output_19_0_g421 = max( ( ( temp_output_26_0_g421 + temp_output_27_0_g421 ) - temp_output_1_0_g421 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch29_g419 = ( ( ( SAMPLE_TEXTURE2D_ARRAY( _MatTexArray, sampler_MatTexArray, uv_MatTexArray,( tex2DNode15_g419.r * 255.0 ) ) * temp_output_21_0_g421 ) + ( tex2DArrayNode23_g419 * temp_output_19_0_g421 ) ) / ( temp_output_21_0_g421 + temp_output_19_0_g421 ) );
				#else
				float4 staticSwitch29_g419 = tex2DArrayNode23_g419;
				#endif
				float temp_output_19_0_g443 = max( ( ( temp_output_26_0_g443 + temp_output_27_0_g443 ) - temp_output_1_0_g443 ) , 0.0 );
				float temp_output_24_0_g442 = temp_output_290_0.z;
				float temp_output_25_0_g442 = temp_output_275_0;
				float temp_output_26_0_g442 = temp_output_293_0.z;
				float temp_output_27_0_g442 = break273.y;
				float temp_output_1_0_g442 = ( max( ( temp_output_24_0_g442 + temp_output_25_0_g442 ) , ( temp_output_26_0_g442 + temp_output_27_0_g442 ) ) - 0.2 );
				float temp_output_21_0_g442 = max( ( ( temp_output_24_0_g442 + temp_output_25_0_g442 ) - temp_output_1_0_g442 ) , 0.0 );
				float4 tex2DArrayNode23_g431 = SAMPLE_TEXTURE2D_ARRAY( _MatTexArray, sampler_MatTexArray, uv_MatTexArray,( tex2DNode6_g431.r * 255.0 ) );
				float temp_output_24_0_g433 = tex2DArrayNode35_g431.b;
				float temp_output_25_0_g433 = tex2DNode16_g431.a;
				float temp_output_26_0_g433 = tex2DArrayNode32_g431.b;
				float temp_output_27_0_g433 = temp_output_41_0_g431;
				float temp_output_1_0_g433 = ( max( ( temp_output_24_0_g433 + temp_output_25_0_g433 ) , ( temp_output_26_0_g433 + temp_output_27_0_g433 ) ) - 0.2 );
				float temp_output_21_0_g433 = max( ( ( temp_output_24_0_g433 + temp_output_25_0_g433 ) - temp_output_1_0_g433 ) , 0.0 );
				float temp_output_19_0_g433 = max( ( ( temp_output_26_0_g433 + temp_output_27_0_g433 ) - temp_output_1_0_g433 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch29_g431 = ( ( ( SAMPLE_TEXTURE2D_ARRAY( _MatTexArray, sampler_MatTexArray, uv_MatTexArray,( tex2DNode15_g431.r * 255.0 ) ) * temp_output_21_0_g433 ) + ( tex2DArrayNode23_g431 * temp_output_19_0_g433 ) ) / ( temp_output_21_0_g433 + temp_output_19_0_g433 ) );
				#else
				float4 staticSwitch29_g431 = tex2DArrayNode23_g431;
				#endif
				float temp_output_24_0_g488 = temp_output_300_3;
				float temp_output_25_0_g488 = temp_output_274_0;
				float temp_output_26_0_g488 = temp_output_301_3;
				float temp_output_27_0_g488 = break273.x;
				float temp_output_1_0_g488 = ( max( ( temp_output_24_0_g488 + temp_output_25_0_g488 ) , ( temp_output_26_0_g488 + temp_output_27_0_g488 ) ) - 0.2 );
				float temp_output_21_0_g488 = max( ( ( temp_output_24_0_g488 + temp_output_25_0_g488 ) - temp_output_1_0_g488 ) , 0.0 );
				float4 tex2DArrayNode23_g427 = SAMPLE_TEXTURE2D_ARRAY( _MatTexArray, sampler_MatTexArray, uv_MatTexArray,( tex2DNode6_g427.r * 255.0 ) );
				float temp_output_24_0_g429 = tex2DArrayNode35_g427.b;
				float temp_output_25_0_g429 = tex2DNode16_g427.a;
				float temp_output_26_0_g429 = tex2DArrayNode32_g427.b;
				float temp_output_27_0_g429 = temp_output_41_0_g427;
				float temp_output_1_0_g429 = ( max( ( temp_output_24_0_g429 + temp_output_25_0_g429 ) , ( temp_output_26_0_g429 + temp_output_27_0_g429 ) ) - 0.2 );
				float temp_output_21_0_g429 = max( ( ( temp_output_24_0_g429 + temp_output_25_0_g429 ) - temp_output_1_0_g429 ) , 0.0 );
				float temp_output_19_0_g429 = max( ( ( temp_output_26_0_g429 + temp_output_27_0_g429 ) - temp_output_1_0_g429 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch29_g427 = ( ( ( SAMPLE_TEXTURE2D_ARRAY( _MatTexArray, sampler_MatTexArray, uv_MatTexArray,( tex2DNode15_g427.r * 255.0 ) ) * temp_output_21_0_g429 ) + ( tex2DArrayNode23_g427 * temp_output_19_0_g429 ) ) / ( temp_output_21_0_g429 + temp_output_19_0_g429 ) );
				#else
				float4 staticSwitch29_g427 = tex2DArrayNode23_g427;
				#endif
				float temp_output_19_0_g488 = max( ( ( temp_output_26_0_g488 + temp_output_27_0_g488 ) - temp_output_1_0_g488 ) , 0.0 );
				float temp_output_19_0_g442 = max( ( ( temp_output_26_0_g442 + temp_output_27_0_g442 ) - temp_output_1_0_g442 ) , 0.0 );
				float4 temp_output_159_0 = ( ( ( ( ( ( staticSwitch29_g423 * temp_output_21_0_g443 ) + ( staticSwitch29_g419 * temp_output_19_0_g443 ) ) / ( temp_output_21_0_g443 + temp_output_19_0_g443 ) ) * temp_output_21_0_g442 ) + ( ( ( ( staticSwitch29_g431 * temp_output_21_0_g488 ) + ( staticSwitch29_g427 * temp_output_19_0_g488 ) ) / ( temp_output_21_0_g488 + temp_output_19_0_g488 ) ) * temp_output_19_0_g442 ) ) / ( temp_output_21_0_g442 + temp_output_19_0_g442 ) );
				
				float4 tex2DNode14_g423 = tex2D( _Tint, temp_output_12_0_g423 );
				float temp_output_24_0_g426 = tex2DArrayNode35_g423.b;
				float temp_output_25_0_g426 = tex2DNode16_g423.a;
				float temp_output_26_0_g426 = tex2DArrayNode32_g423.b;
				float temp_output_27_0_g426 = temp_output_41_0_g423;
				float temp_output_1_0_g426 = ( max( ( temp_output_24_0_g426 + temp_output_25_0_g426 ) , ( temp_output_26_0_g426 + temp_output_27_0_g426 ) ) - 0.2 );
				float temp_output_21_0_g426 = max( ( ( temp_output_24_0_g426 + temp_output_25_0_g426 ) - temp_output_1_0_g426 ) , 0.0 );
				float temp_output_19_0_g426 = max( ( ( temp_output_26_0_g426 + temp_output_27_0_g426 ) - temp_output_1_0_g426 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch30_g423 = ( ( ( tex2DNode16_g423 * temp_output_21_0_g426 ) + ( tex2DNode14_g423 * temp_output_19_0_g426 ) ) / ( temp_output_21_0_g426 + temp_output_19_0_g426 ) );
				#else
				float4 staticSwitch30_g423 = tex2DNode14_g423;
				#endif
				float temp_output_24_0_g437 = temp_output_302_3;
				float temp_output_25_0_g437 = temp_output_274_0;
				float temp_output_26_0_g437 = temp_output_299_3;
				float temp_output_27_0_g437 = break273.x;
				float temp_output_1_0_g437 = ( max( ( temp_output_24_0_g437 + temp_output_25_0_g437 ) , ( temp_output_26_0_g437 + temp_output_27_0_g437 ) ) - 0.2 );
				float temp_output_21_0_g437 = max( ( ( temp_output_24_0_g437 + temp_output_25_0_g437 ) - temp_output_1_0_g437 ) , 0.0 );
				float4 tex2DNode14_g419 = tex2D( _Tint, temp_output_12_0_g419 );
				float temp_output_24_0_g422 = tex2DArrayNode35_g419.b;
				float temp_output_25_0_g422 = tex2DNode16_g419.a;
				float temp_output_26_0_g422 = tex2DArrayNode32_g419.b;
				float temp_output_27_0_g422 = temp_output_41_0_g419;
				float temp_output_1_0_g422 = ( max( ( temp_output_24_0_g422 + temp_output_25_0_g422 ) , ( temp_output_26_0_g422 + temp_output_27_0_g422 ) ) - 0.2 );
				float temp_output_21_0_g422 = max( ( ( temp_output_24_0_g422 + temp_output_25_0_g422 ) - temp_output_1_0_g422 ) , 0.0 );
				float temp_output_19_0_g422 = max( ( ( temp_output_26_0_g422 + temp_output_27_0_g422 ) - temp_output_1_0_g422 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch30_g419 = ( ( ( tex2DNode16_g419 * temp_output_21_0_g422 ) + ( tex2DNode14_g419 * temp_output_19_0_g422 ) ) / ( temp_output_21_0_g422 + temp_output_19_0_g422 ) );
				#else
				float4 staticSwitch30_g419 = tex2DNode14_g419;
				#endif
				float temp_output_19_0_g437 = max( ( ( temp_output_26_0_g437 + temp_output_27_0_g437 ) - temp_output_1_0_g437 ) , 0.0 );
				float temp_output_24_0_g439 = temp_output_290_0.z;
				float temp_output_25_0_g439 = temp_output_275_0;
				float temp_output_26_0_g439 = temp_output_293_0.z;
				float temp_output_27_0_g439 = break273.y;
				float temp_output_1_0_g439 = ( max( ( temp_output_24_0_g439 + temp_output_25_0_g439 ) , ( temp_output_26_0_g439 + temp_output_27_0_g439 ) ) - 0.2 );
				float temp_output_21_0_g439 = max( ( ( temp_output_24_0_g439 + temp_output_25_0_g439 ) - temp_output_1_0_g439 ) , 0.0 );
				float4 tex2DNode14_g431 = tex2D( _Tint, temp_output_12_0_g431 );
				float temp_output_24_0_g434 = tex2DArrayNode35_g431.b;
				float temp_output_25_0_g434 = tex2DNode16_g431.a;
				float temp_output_26_0_g434 = tex2DArrayNode32_g431.b;
				float temp_output_27_0_g434 = temp_output_41_0_g431;
				float temp_output_1_0_g434 = ( max( ( temp_output_24_0_g434 + temp_output_25_0_g434 ) , ( temp_output_26_0_g434 + temp_output_27_0_g434 ) ) - 0.2 );
				float temp_output_21_0_g434 = max( ( ( temp_output_24_0_g434 + temp_output_25_0_g434 ) - temp_output_1_0_g434 ) , 0.0 );
				float temp_output_19_0_g434 = max( ( ( temp_output_26_0_g434 + temp_output_27_0_g434 ) - temp_output_1_0_g434 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch30_g431 = ( ( ( tex2DNode16_g431 * temp_output_21_0_g434 ) + ( tex2DNode14_g431 * temp_output_19_0_g434 ) ) / ( temp_output_21_0_g434 + temp_output_19_0_g434 ) );
				#else
				float4 staticSwitch30_g431 = tex2DNode14_g431;
				#endif
				float temp_output_24_0_g438 = temp_output_300_3;
				float temp_output_25_0_g438 = temp_output_274_0;
				float temp_output_26_0_g438 = temp_output_301_3;
				float temp_output_27_0_g438 = break273.x;
				float temp_output_1_0_g438 = ( max( ( temp_output_24_0_g438 + temp_output_25_0_g438 ) , ( temp_output_26_0_g438 + temp_output_27_0_g438 ) ) - 0.2 );
				float temp_output_21_0_g438 = max( ( ( temp_output_24_0_g438 + temp_output_25_0_g438 ) - temp_output_1_0_g438 ) , 0.0 );
				float4 tex2DNode14_g427 = tex2D( _Tint, temp_output_12_0_g427 );
				float temp_output_24_0_g430 = tex2DArrayNode35_g427.b;
				float temp_output_25_0_g430 = tex2DNode16_g427.a;
				float temp_output_26_0_g430 = tex2DArrayNode32_g427.b;
				float temp_output_27_0_g430 = temp_output_41_0_g427;
				float temp_output_1_0_g430 = ( max( ( temp_output_24_0_g430 + temp_output_25_0_g430 ) , ( temp_output_26_0_g430 + temp_output_27_0_g430 ) ) - 0.2 );
				float temp_output_21_0_g430 = max( ( ( temp_output_24_0_g430 + temp_output_25_0_g430 ) - temp_output_1_0_g430 ) , 0.0 );
				float temp_output_19_0_g430 = max( ( ( temp_output_26_0_g430 + temp_output_27_0_g430 ) - temp_output_1_0_g430 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch30_g427 = ( ( ( tex2DNode16_g427 * temp_output_21_0_g430 ) + ( tex2DNode14_g427 * temp_output_19_0_g430 ) ) / ( temp_output_21_0_g430 + temp_output_19_0_g430 ) );
				#else
				float4 staticSwitch30_g427 = tex2DNode14_g427;
				#endif
				float temp_output_19_0_g438 = max( ( ( temp_output_26_0_g438 + temp_output_27_0_g438 ) - temp_output_1_0_g438 ) , 0.0 );
				float temp_output_19_0_g439 = max( ( ( temp_output_26_0_g439 + temp_output_27_0_g439 ) - temp_output_1_0_g439 ) , 0.0 );
				float4 temp_output_221_0 = ( ( ( ( ( ( staticSwitch30_g423 * temp_output_21_0_g437 ) + ( staticSwitch30_g419 * temp_output_19_0_g437 ) ) / ( temp_output_21_0_g437 + temp_output_19_0_g437 ) ) * temp_output_21_0_g439 ) + ( ( ( ( staticSwitch30_g431 * temp_output_21_0_g438 ) + ( staticSwitch30_g427 * temp_output_19_0_g438 ) ) / ( temp_output_21_0_g438 + temp_output_19_0_g438 ) ) * temp_output_19_0_g439 ) ) / ( temp_output_21_0_g439 + temp_output_19_0_g439 ) );
				float temp_output_3_0_g440 = ( temp_output_221_0.w * 2.0 );
				
				surfaceDescription.Normal = unpack220;
				surfaceDescription.Smoothness = ( temp_output_159_0.w * _Rough );
				surfaceDescription.Alpha = min( temp_output_3_0_g440 , 1.0 );

				#ifdef _ALPHATEST_ON
				surfaceDescription.AlphaClipThreshold = _AlphaCutoff;
				#endif

				#ifdef _DEPTHOFFSET_ON
				surfaceDescription.DepthOffset = 0;
				#endif

				GetSurfaceAndBuiltinData( surfaceDescription, input, V, posInput, surfaceData, builtinData );

				float4 VPASSpositionCS = float4(packedInput.vpassInterpolators0.xy, 0.0, packedInput.vpassInterpolators0.z);
				float4 VPASSpreviousPositionCS = float4(packedInput.vpassInterpolators1.xy, 0.0, packedInput.vpassInterpolators1.z);

				#ifdef _DEPTHOFFSET_ON
				VPASSpositionCS.w += builtinData.depthOffset;
				VPASSpreviousPositionCS.w += builtinData.depthOffset;
				#endif

				float2 motionVector = CalculateMotionVector( VPASSpositionCS, VPASSpreviousPositionCS );
				EncodeMotionVector( motionVector * 0.5, outMotionVector );

				bool forceNoMotion = unity_MotionVectorsParams.y == 0.0;
				if( forceNoMotion )
					outMotionVector = float4( 2.0, 0.0, 0.0, 0.0 );


				
				#ifdef WRITE_MSAA_DEPTH
					depthColor = packedInput.vmeshPositionCS.z;
					#ifdef _ALPHATOMASK_ON
					depthColor.a = SharpenAlpha(builtinData.opacity, builtinData.alphaClipTreshold);
					#endif
				#endif

			
				#ifdef WRITE_NORMAL_BUFFER
					EncodeIntoNormalBuffer(ConvertSurfaceDataToNormalData(surfaceData), outNormalBuffer);
				#endif

				#if defined(WRITE_DECAL_BUFFER)
					DecalPrepassData decalPrepassData;
					#ifdef _DISABLE_DECALS
					ZERO_INITIALIZE(DecalPrepassData, decalPrepassData);
					#else
					decalPrepassData.geomNormalWS = surfaceData.geomNormalWS;
					decalPrepassData.decalLayerMask = GetMeshRenderingDecalLayer();
					#endif
					EncodeIntoDecalPrepassBuffer(decalPrepassData, outDecalBuffer);
				#endif

				#ifdef _DEPTHOFFSET_ON
				outputDepth = posInput.deviceDepth;
				#endif
			}

			ENDHLSL
		}

		
		Pass
		{
			
			Name "Forward"
			Tags { "LightMode"="Forward" }
			Blend [_SrcBlend] [_DstBlend], [_AlphaSrcBlend] [_AlphaDstBlend]
			Cull [_CullModeForward]
			ZTest [_ZTestDepthEqualForOpaque]
			ZWrite [_ZWrite]

			Stencil
			{
				Ref [_StencilRef]
				WriteMask [_StencilWriteMask]
				Comp Always
				Pass Replace
				Fail Keep
				ZFail Keep
			}


			ColorMask [_ColorMaskTransparentVel] 1

			HLSLPROGRAM

			#define _SPECULAR_OCCLUSION_FROM_AO 1
			#define _DISABLE_DECALS 1
			#define _DISABLE_SSR 1
			#define _AMBIENT_OCCLUSION 1
			#define ASE_SRP_VERSION 999999


			#pragma shader_feature _SURFACE_TYPE_TRANSPARENT
			#pragma shader_feature_local _DOUBLESIDED_ON
			#pragma shader_feature_local _BLENDMODE_OFF _BLENDMODE_ALPHA _BLENDMODE_ADD _BLENDMODE_PRE_MULTIPLY
			#pragma shader_feature_local_fragment _ _ENABLE_FOG_ON_TRANSPARENT
			#pragma shader_feature_local _ALPHATEST_ON
			#pragma shader_feature_local _ _TRANSPARENT_WRITES_MOTION_VEC

			#define SHADERPASS SHADERPASS_FORWARD
			#pragma multi_compile _ DEBUG_DISPLAY
			#pragma multi_compile _ LIGHTMAP_ON
			#pragma multi_compile _ DIRLIGHTMAP_COMBINED
			#pragma multi_compile _ DYNAMICLIGHTMAP_ON
			#pragma multi_compile_fragment _ SHADOWS_SHADOWMASK
			#pragma multi_compile_fragment DECALS_OFF DECALS_3RT DECALS_4RT
			#pragma multi_compile_fragment USE_FPTL_LIGHTLIST USE_CLUSTERED_LIGHTLIST
			#pragma multi_compile_fragment SHADOW_LOW SHADOW_MEDIUM SHADOW_HIGH SHADOW_VERY_HIGH
			#pragma multi_compile_fragment PROBE_VOLUMES_OFF PROBE_VOLUMES_L1 PROBE_VOLUMES_L2
			#pragma multi_compile_fragment _ DECAL_SURFACE_GRADIENT
			#pragma multi_compile_fragment SCREEN_SPACE_SHADOWS_OFF SCREEN_SPACE_SHADOWS_ON

			#if !defined(DEBUG_DISPLAY) && defined(_ALPHATEST_ON)
			#define SHADERPASS_FORWARD_BYPASS_ALPHA_TEST
			#endif

			#if defined(SHADER_LIT) && !defined(_SURFACE_TYPE_TRANSPARENT)
                #define _DEFERRED_CAPABLE_MATERIAL
            #endif
        
            
            #if defined(_TRANSPARENT_WRITES_MOTION_VEC) && defined(_SURFACE_TYPE_TRANSPARENT)
                #define _WRITE_TRANSPARENT_MOTION_VECTOR
            #endif

			#pragma vertex Vert
			#pragma fragment Frag
			
			//#define UNITY_MATERIAL_LIT

			#if defined(_MATERIAL_FEATURE_SUBSURFACE_SCATTERING) && !defined(_SURFACE_TYPE_TRANSPARENT)
			#define OUTPUT_SPLIT_LIGHTING
			#endif

            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
        	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/GeometricTools.hlsl" 
        	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Tessellation.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/FragInputs.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl" 
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderGraphHeader.hlsl" 

			#ifdef DEBUG_DISPLAY
				#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Debug/DebugDisplay.hlsl"
			#endif

			// CBuffer must be declared before Material.hlsl since it internaly uses _BlendMode now
			CBUFFER_START( UnityPerMaterial )
			float4 _MatTexArray_ST;
			float4 _WorldBounds;
			float4 _Control_TexelSize;
			float4 _ShapeMap_ST;
			float _NormalScale;
			float _MetalLevel;
			float _Rough;
			float4 _EmissionColor;
			float _AlphaCutoff;
			float _RenderQueueType;
			#ifdef _ADD_PRECOMPUTED_VELOCITY
			float _AddPrecomputedVelocity;
			#endif
			float _StencilRef;
			float _StencilWriteMask;
			float _StencilRefDepth;
			float _StencilWriteMaskDepth;
			float _StencilRefMV;
			float _StencilWriteMaskMV;
			float _StencilRefDistortionVec;
			float _StencilWriteMaskDistortionVec;
			float _StencilWriteMaskGBuffer;
			float _StencilRefGBuffer;
			float _ZTestGBuffer;
			float _RequireSplitLighting;
			float _ReceivesSSR;
			float _SurfaceType;
			float _BlendMode;
			float _SrcBlend;
			float _DstBlend;
			float _AlphaSrcBlend;
			float _AlphaDstBlend;
			float _ZWrite;
			float _TransparentZWrite;
			float _CullMode;
			float _TransparentSortPriority;
			float _EnableFogOnTransparent;
			float _CullModeForward;
			float _TransparentCullMode;
			float _ZTestDepthEqualForOpaque;
			float _ZTestTransparent;
			float _TransparentBackfaceEnable;
			float _AlphaCutoffEnable;
			float _UseShadowThreshold;
			float _DoubleSidedEnable;
			float _DoubleSidedNormalMode;
			float4 _DoubleSidedConstants;
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			TEXTURE2D_ARRAY(_MatTexArray);
			sampler2D _Control;
			SAMPLER(sampler_MatTexArray);
			sampler2D _GrassControl;
			TEXTURE2D_ARRAY(_ShapeMap);
			SAMPLER(sampler_ShapeMap);
			sampler2D _GrassTint;
			sampler2D _Tint;
			sampler2D _SpatterTex;


			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Lighting/Lighting.hlsl"
			#define HAS_LIGHTLOOP
			#define SHADER_LIT 1
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/NormalSurfaceGradient.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Lighting/LightLoop/LightLoopDef.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Lit/Lit.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Lighting/LightLoop/LightLoop.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/BuiltinUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/MaterialUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Decal/DecalUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Lit/LitDecalData.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderGraphFunctions.hlsl"

			#define ASE_NEEDS_FRAG_RELATIVE_WORLD_POS
			#define GRASS


			#if defined(_DOUBLESIDED_ON) && !defined(ASE_NEED_CULLFACE)
				#define ASE_NEED_CULLFACE 1
			#endif

			struct AttributesMesh
			{
				float3 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 tangentOS : TANGENT;
				float4 uv1 : TEXCOORD1;
				float4 uv2 : TEXCOORD2;
				#ifdef _WRITE_TRANSPARENT_MOTION_VECTOR
					float3 previousPositionOS : TEXCOORD4;
					#if defined (_ADD_PRECOMPUTED_VELOCITY)
						float3 precomputedVelocity : TEXCOORD5;
					#endif
				#endif
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryingsMeshToPS
			{
				float4 positionCS : SV_Position;
				float3 positionRWS : TEXCOORD0;
				float3 normalWS : TEXCOORD1;
				float4 tangentWS : TEXCOORD2;
				float4 uv1 : TEXCOORD3;
				float4 uv2 : TEXCOORD4;
				#ifdef _WRITE_TRANSPARENT_MOTION_VECTOR
					float3 vpassPositionCS : TEXCOORD5;
					float3 vpassPreviousPositionCS : TEXCOORD6;
				#endif
				float4 ase_texcoord7 : TEXCOORD7;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
				#if defined(SHADER_STAGE_FRAGMENT) && defined(ASE_NEED_CULLFACE)
				FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
				#endif
			};

			float3 mod3D289( float3 x ) { return x - floor( x / 289.0 ) * 289.0; }
			float4 mod3D289( float4 x ) { return x - floor( x / 289.0 ) * 289.0; }
			float4 permute( float4 x ) { return mod3D289( ( x * 34.0 + 1.0 ) * x ); }
			float4 taylorInvSqrt( float4 r ) { return 1.79284291400159 - r * 0.85373472095314; }
			float snoise( float3 v )
			{
				const float2 C = float2( 1.0 / 6.0, 1.0 / 3.0 );
				float3 i = floor( v + dot( v, C.yyy ) );
				float3 x0 = v - i + dot( i, C.xxx );
				float3 g = step( x0.yzx, x0.xyz );
				float3 l = 1.0 - g;
				float3 i1 = min( g.xyz, l.zxy );
				float3 i2 = max( g.xyz, l.zxy );
				float3 x1 = x0 - i1 + C.xxx;
				float3 x2 = x0 - i2 + C.yyy;
				float3 x3 = x0 - 0.5;
				i = mod3D289( i);
				float4 p = permute( permute( permute( i.z + float4( 0.0, i1.z, i2.z, 1.0 ) ) + i.y + float4( 0.0, i1.y, i2.y, 1.0 ) ) + i.x + float4( 0.0, i1.x, i2.x, 1.0 ) );
				float4 j = p - 49.0 * floor( p / 49.0 );  // mod(p,7*7)
				float4 x_ = floor( j / 7.0 );
				float4 y_ = floor( j - 7.0 * x_ );  // mod(j,N)
				float4 x = ( x_ * 2.0 + 0.5 ) / 7.0 - 1.0;
				float4 y = ( y_ * 2.0 + 0.5 ) / 7.0 - 1.0;
				float4 h = 1.0 - abs( x ) - abs( y );
				float4 b0 = float4( x.xy, y.xy );
				float4 b1 = float4( x.zw, y.zw );
				float4 s0 = floor( b0 ) * 2.0 + 1.0;
				float4 s1 = floor( b1 ) * 2.0 + 1.0;
				float4 sh = -step( h, 0.0 );
				float4 a0 = b0.xzyw + s0.xzyw * sh.xxyy;
				float4 a1 = b1.xzyw + s1.xzyw * sh.zzww;
				float3 g0 = float3( a0.xy, h.x );
				float3 g1 = float3( a0.zw, h.y );
				float3 g2 = float3( a1.xy, h.z );
				float3 g3 = float3( a1.zw, h.w );
				float4 norm = taylorInvSqrt( float4( dot( g0, g0 ), dot( g1, g1 ), dot( g2, g2 ), dot( g3, g3 ) ) );
				g0 *= norm.x;
				g1 *= norm.y;
				g2 *= norm.z;
				g3 *= norm.w;
				float4 m = max( 0.6 - float4( dot( x0, x0 ), dot( x1, x1 ), dot( x2, x2 ), dot( x3, x3 ) ), 0.0 );
				m = m* m;
				m = m* m;
				float4 px = float4( dot( x0, g0 ), dot( x1, g1 ), dot( x2, g2 ), dot( x3, g3 ) );
				return 42.0 * dot( m, px);
			}
			

			void BuildSurfaceData(FragInputs fragInputs, inout GlobalSurfaceDescription surfaceDescription, float3 V, PositionInputs posInput, out SurfaceData surfaceData, out float3 bentNormalWS)
			{
				ZERO_INITIALIZE(SurfaceData, surfaceData);

				surfaceData.specularOcclusion = 1.0;

				// surface data
				surfaceData.baseColor =					surfaceDescription.Albedo;
				surfaceData.perceptualSmoothness =		surfaceDescription.Smoothness;
				surfaceData.ambientOcclusion =			surfaceDescription.Occlusion;
				surfaceData.metallic =					surfaceDescription.Metallic;
				surfaceData.coatMask =					surfaceDescription.CoatMask;

				#ifdef _SPECULAR_OCCLUSION_CUSTOM
				surfaceData.specularOcclusion =			surfaceDescription.SpecularOcclusion;
				#endif
				#ifdef _MATERIAL_FEATURE_SUBSURFACE_SCATTERING
				surfaceData.subsurfaceMask =			surfaceDescription.SubsurfaceMask;
				#endif
				#if defined(_HAS_REFRACTION) || defined(_MATERIAL_FEATURE_TRANSMISSION)
				surfaceData.thickness =					surfaceDescription.Thickness;
				#endif
				#if defined( _MATERIAL_FEATURE_SUBSURFACE_SCATTERING ) || defined( _MATERIAL_FEATURE_TRANSMISSION )
				surfaceData.diffusionProfileHash =		asuint(surfaceDescription.DiffusionProfile);
				#endif
				#ifdef _MATERIAL_FEATURE_SPECULAR_COLOR
				surfaceData.specularColor =				surfaceDescription.Specular;
				#endif
				#ifdef _MATERIAL_FEATURE_ANISOTROPY
				surfaceData.anisotropy =				surfaceDescription.Anisotropy;
				#endif
				#ifdef _MATERIAL_FEATURE_IRIDESCENCE
				surfaceData.iridescenceMask =			surfaceDescription.IridescenceMask;
				surfaceData.iridescenceThickness =		surfaceDescription.IridescenceThickness;
				#endif

				// refraction
				#ifdef _HAS_REFRACTION
				if( _EnableSSRefraction )
				{
					surfaceData.ior = surfaceDescription.RefractionIndex;
					surfaceData.transmittanceColor = surfaceDescription.RefractionColor;
					surfaceData.atDistance = surfaceDescription.RefractionDistance;

					surfaceData.transmittanceMask = ( 1.0 - surfaceDescription.Alpha );
					surfaceDescription.Alpha = 1.0;
				}
				else
				{
					surfaceData.ior = 1.0;
					surfaceData.transmittanceColor = float3( 1.0, 1.0, 1.0 );
					surfaceData.atDistance = 1.0;
					surfaceData.transmittanceMask = 0.0;
					surfaceDescription.Alpha = 1.0;
				}
				#else
				surfaceData.ior = 1.0;
				surfaceData.transmittanceColor = float3( 1.0, 1.0, 1.0 );
				surfaceData.atDistance = 1.0;
				surfaceData.transmittanceMask = 0.0;
				#endif


				// material features
				surfaceData.materialFeatures = MATERIALFEATUREFLAGS_LIT_STANDARD;
				#ifdef _MATERIAL_FEATURE_SUBSURFACE_SCATTERING
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_SUBSURFACE_SCATTERING;
				#endif
				#ifdef _MATERIAL_FEATURE_TRANSMISSION
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_TRANSMISSION;
				#endif
				#ifdef _MATERIAL_FEATURE_ANISOTROPY
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_ANISOTROPY;
				#endif
				#ifdef ASE_LIT_CLEAR_COAT
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_CLEAR_COAT;
				#endif
				#ifdef _MATERIAL_FEATURE_IRIDESCENCE
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_IRIDESCENCE;
				#endif
				#ifdef _MATERIAL_FEATURE_SPECULAR_COLOR
				surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_SPECULAR_COLOR;
				#endif

				// others
				#if defined (_MATERIAL_FEATURE_SPECULAR_COLOR) && defined (_ENERGY_CONSERVING_SPECULAR)
				surfaceData.baseColor *= ( 1.0 - Max3( surfaceData.specularColor.r, surfaceData.specularColor.g, surfaceData.specularColor.b ) );
				#endif
				#ifdef _DOUBLESIDED_ON
				float3 doubleSidedConstants = _DoubleSidedConstants.xyz;
				#else
				float3 doubleSidedConstants = float3( 1.0, 1.0, 1.0 );
				#endif

				// normals
				float3 normalTS = float3(0.0f, 0.0f, 1.0f);
				normalTS = surfaceDescription.Normal;
				GetNormalWS( fragInputs, normalTS, surfaceData.normalWS, doubleSidedConstants );

				surfaceData.geomNormalWS = fragInputs.tangentToWorld[2];
				surfaceData.tangentWS = normalize( fragInputs.tangentToWorld[ 0 ].xyz );

				// decals
				#if HAVE_DECALS
				if( _EnableDecals )
				{
					DecalSurfaceData decalSurfaceData = GetDecalSurfaceData(posInput, fragInputs.tangentToWorld[2], surfaceDescription.Alpha);
					ApplyDecalToSurfaceData(decalSurfaceData, fragInputs.tangentToWorld[2], surfaceData);
				}
				#endif

				bentNormalWS = surfaceData.normalWS;
				
				#ifdef ASE_BENT_NORMAL
				GetNormalWS( fragInputs, surfaceDescription.BentNormal, bentNormalWS, doubleSidedConstants );
				#endif

				#ifdef _MATERIAL_FEATURE_ANISOTROPY
				surfaceData.tangentWS = TransformTangentToWorld( surfaceDescription.Tangent, fragInputs.tangentToWorld );
				#endif
				surfaceData.tangentWS = Orthonormalize( surfaceData.tangentWS, surfaceData.normalWS );


				#if defined(_SPECULAR_OCCLUSION_CUSTOM)
				#elif defined(_SPECULAR_OCCLUSION_FROM_AO_BENT_NORMAL)
				surfaceData.specularOcclusion = GetSpecularOcclusionFromBentAO( V, bentNormalWS, surfaceData.normalWS, surfaceData.ambientOcclusion, PerceptualSmoothnessToPerceptualRoughness( surfaceData.perceptualSmoothness ) );
				#elif defined(_AMBIENT_OCCLUSION) && defined(_SPECULAR_OCCLUSION_FROM_AO)
				surfaceData.specularOcclusion = GetSpecularOcclusionFromAmbientOcclusion( ClampNdotV( dot( surfaceData.normalWS, V ) ), surfaceData.ambientOcclusion, PerceptualSmoothnessToRoughness( surfaceData.perceptualSmoothness ) );
				#endif

				#ifdef _ENABLE_GEOMETRIC_SPECULAR_AA
				surfaceData.perceptualSmoothness = GeometricNormalFiltering( surfaceData.perceptualSmoothness, fragInputs.tangentToWorld[ 2 ], surfaceDescription.SpecularAAScreenSpaceVariance, surfaceDescription.SpecularAAThreshold );
				#endif

				// debug
				#if defined(DEBUG_DISPLAY)
				if (_DebugMipMapMode != DEBUGMIPMAPMODE_NONE)
				{
					surfaceData.metallic = 0;
				}
				ApplyDebugToSurfaceData(fragInputs.tangentToWorld, surfaceData);
				#endif
			}

			void GetSurfaceAndBuiltinData(GlobalSurfaceDescription surfaceDescription, FragInputs fragInputs, float3 V, inout PositionInputs posInput, out SurfaceData surfaceData, out BuiltinData builtinData)
			{
				#ifdef LOD_FADE_CROSSFADE
				LODDitheringTransition(ComputeFadeMaskSeed(V, posInput.positionSS), unity_LODFade.x);
				#endif

				#ifdef _DOUBLESIDED_ON
				float3 doubleSidedConstants = _DoubleSidedConstants.xyz;
				#else
				float3 doubleSidedConstants = float3( 1.0, 1.0, 1.0 );
				#endif

				ApplyDoubleSidedFlipOrMirror( fragInputs, doubleSidedConstants );

				#ifdef _ALPHATEST_ON
				DoAlphaTest( surfaceDescription.Alpha, surfaceDescription.AlphaClipThreshold );
				#endif

				#ifdef _DEPTHOFFSET_ON
				ApplyDepthOffsetPositionInput( V, surfaceDescription.DepthOffset, GetViewForwardDir(), GetWorldToHClipMatrix(), posInput );
				#endif

				float3 bentNormalWS;
				BuildSurfaceData( fragInputs, surfaceDescription, V, posInput, surfaceData, bentNormalWS );

				InitBuiltinData( posInput, surfaceDescription.Alpha, bentNormalWS, -fragInputs.tangentToWorld[ 2 ], fragInputs.texCoord1, fragInputs.texCoord2, builtinData );

				#ifdef _DEPTHOFFSET_ON
				builtinData.depthOffset = surfaceDescription.DepthOffset;
				#endif

				#ifdef _ALPHATEST_ON    
                    builtinData.alphaClipTreshold = surfaceDescription.AlphaClipThreshold;
                #endif

				#ifdef UNITY_VIRTUAL_TEXTURING
                builtinData.vtPackedFeedback = surfaceDescription.VTPackedFeedback;
                #endif

				#ifdef _ASE_BAKEDGI
				builtinData.bakeDiffuseLighting = surfaceDescription.BakedGI;
				#endif

				#ifdef _ASE_BAKEDBACKGI
				builtinData.backBakeDiffuseLighting = surfaceDescription.BakedBackGI;
				#endif

				builtinData.emissiveColor = surfaceDescription.Emission;

				PostInitBuiltinData(V, posInput, surfaceData, builtinData);
			}

			AttributesMesh ApplyMeshModification(AttributesMesh inputMesh, float3 timeParameters, inout PackedVaryingsMeshToPS outputPackedVaryingsMeshToPS )
			{
				_TimeParameters.xyz = timeParameters;
				outputPackedVaryingsMeshToPS.ase_texcoord7.xy = inputMesh.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				outputPackedVaryingsMeshToPS.ase_texcoord7.zw = 0;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
				float3 defaultVertexValue = inputMesh.positionOS.xyz;
				#else
				float3 defaultVertexValue = float3( 0, 0, 0 );
				#endif
				float3 vertexValue = defaultVertexValue;

				#ifdef ASE_ABSOLUTE_VERTEX_POS
				inputMesh.positionOS.xyz = vertexValue;
				#else
				inputMesh.positionOS.xyz += vertexValue;
				#endif
				inputMesh.normalOS = inputMesh.normalOS;
				inputMesh.tangentOS = inputMesh.tangentOS;
				return inputMesh;
			}

			PackedVaryingsMeshToPS VertexFunction(AttributesMesh inputMesh)
			{
				PackedVaryingsMeshToPS outputPackedVaryingsMeshToPS = (PackedVaryingsMeshToPS)0;
				AttributesMesh defaultMesh = inputMesh;

				UNITY_SETUP_INSTANCE_ID(inputMesh);
				UNITY_TRANSFER_INSTANCE_ID(inputMesh, outputPackedVaryingsMeshToPS);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( outputPackedVaryingsMeshToPS );

				inputMesh = ApplyMeshModification( inputMesh, _TimeParameters.xyz, outputPackedVaryingsMeshToPS);

				float3 positionRWS = TransformObjectToWorld(inputMesh.positionOS);
				float3 normalWS = TransformObjectToWorldNormal(inputMesh.normalOS);
				float4 tangentWS = float4(TransformObjectToWorldDir(inputMesh.tangentOS.xyz), inputMesh.tangentOS.w);

				#ifdef _WRITE_TRANSPARENT_MOTION_VECTOR
				float4 VPASSpreviousPositionCS;
				float4 VPASSpositionCS = mul(UNITY_MATRIX_UNJITTERED_VP, float4(positionRWS, 1.0));

				bool forceNoMotion = unity_MotionVectorsParams.y == 0.0;
				if (forceNoMotion)
				{
					VPASSpreviousPositionCS = float4(0.0, 0.0, 0.0, 1.0);
				}
				else
				{
					bool hasDeformation = unity_MotionVectorsParams.x > 0.0;
					float3 effectivePositionOS = (hasDeformation ? inputMesh.previousPositionOS : defaultMesh.positionOS);
					#if defined(_ADD_PRECOMPUTED_VELOCITY)
					effectivePositionOS -= inputMesh.precomputedVelocity;
					#endif

					#if defined(HAVE_MESH_MODIFICATION)
						AttributesMesh previousMesh = defaultMesh;
						previousMesh.positionOS = effectivePositionOS ;
						PackedVaryingsMeshToPS test = (PackedVaryingsMeshToPS)0;
						float3 curTime = _TimeParameters.xyz;
						previousMesh = ApplyMeshModification(previousMesh, _LastTimeParameters.xyz, test);
						_TimeParameters.xyz = curTime;
						float3 previousPositionRWS = TransformPreviousObjectToWorld(previousMesh.positionOS);
					#else
						float3 previousPositionRWS = TransformPreviousObjectToWorld(effectivePositionOS);
					#endif

					#ifdef ATTRIBUTES_NEED_NORMAL
						float3 normalWS = TransformPreviousObjectToWorldNormal(defaultMesh.normalOS);
					#else
						float3 normalWS = float3(0.0, 0.0, 0.0);
					#endif

					#if defined(HAVE_VERTEX_MODIFICATION)
						//ApplyVertexModification(inputMesh, normalWS, previousPositionRWS, _LastTimeParameters.xyz);
					#endif

					VPASSpreviousPositionCS = mul(UNITY_MATRIX_PREV_VP, float4(previousPositionRWS, 1.0));
				}
				#endif

				outputPackedVaryingsMeshToPS.positionCS = TransformWorldToHClip(positionRWS);
				outputPackedVaryingsMeshToPS.positionRWS.xyz = positionRWS;
				outputPackedVaryingsMeshToPS.normalWS.xyz = normalWS;
				outputPackedVaryingsMeshToPS.tangentWS.xyzw = tangentWS;
				outputPackedVaryingsMeshToPS.uv1.xyzw = inputMesh.uv1;
				outputPackedVaryingsMeshToPS.uv2.xyzw = inputMesh.uv2;

				#ifdef _WRITE_TRANSPARENT_MOTION_VECTOR
					outputPackedVaryingsMeshToPS.vpassPositionCS = float3(VPASSpositionCS.xyw);
					outputPackedVaryingsMeshToPS.vpassPreviousPositionCS = float3(VPASSpreviousPositionCS.xyw);
				#endif
				return outputPackedVaryingsMeshToPS;
			}

			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float3 positionOS : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 tangentOS : TANGENT;
				float4 uv1 : TEXCOORD1;
				float4 uv2 : TEXCOORD2;
				#ifdef _WRITE_TRANSPARENT_MOTION_VECTOR
					float3 previousPositionOS : TEXCOORD4;
					#if defined (_ADD_PRECOMPUTED_VELOCITY)
						float3 precomputedVelocity : TEXCOORD5;
					#endif
				#endif
				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl Vert ( AttributesMesh v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.positionOS = v.positionOS;
				o.normalOS = v.normalOS;
				o.tangentOS = v.tangentOS;
				o.uv1 = v.uv1;
				o.uv2 = v.uv2;
				#ifdef _WRITE_TRANSPARENT_MOTION_VECTOR
					o.previousPositionOS = v.previousPositionOS;
					#if defined (_ADD_PRECOMPUTED_VELOCITY)
						o.precomputedVelocity = v.precomputedVelocity;
					#endif
				#endif
				o.ase_texcoord = v.ase_texcoord;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if (SHADEROPTIONS_CAMERA_RELATIVE_RENDERING != 0)
				float3 cameraPos = 0;
				#else
				float3 cameraPos = _WorldSpaceCameraPos;
				#endif
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(float4(v[0].positionOS,1), float4(v[1].positionOS,1), float4(v[2].positionOS,1), tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), cameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(float4(v[0].positionOS,1), float4(v[1].positionOS,1), float4(v[2].positionOS,1), edgeLength, GetObjectToWorldMatrix(), cameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(float4(v[0].positionOS,1), float4(v[1].positionOS,1), float4(v[2].positionOS,1), edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), cameraPos, _ScreenParams, _FrustumPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			PackedVaryingsMeshToPS DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				AttributesMesh o = (AttributesMesh) 0;
				o.positionOS = patch[0].positionOS * bary.x + patch[1].positionOS * bary.y + patch[2].positionOS * bary.z;
				o.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				o.tangentOS = patch[0].tangentOS * bary.x + patch[1].tangentOS * bary.y + patch[2].tangentOS * bary.z;
				o.uv1 = patch[0].uv1 * bary.x + patch[1].uv1 * bary.y + patch[2].uv1 * bary.z;
				o.uv2 = patch[0].uv2 * bary.x + patch[1].uv2 * bary.y + patch[2].uv2 * bary.z;
				#ifdef _WRITE_TRANSPARENT_MOTION_VECTOR
					o.previousPositionOS = patch[0].previousPositionOS * bary.x + patch[1].previousPositionOS * bary.y + patch[2].previousPositionOS * bary.z;
					#if defined (_ADD_PRECOMPUTED_VELOCITY)
						o.precomputedVelocity = patch[0].precomputedVelocity * bary.x + patch[1].precomputedVelocity * bary.y + patch[2].precomputedVelocity * bary.z;
					#endif
				#endif
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.positionOS.xyz - patch[i].normalOS * (dot(o.positionOS.xyz, patch[i].normalOS) - dot(patch[i].positionOS.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				o.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			PackedVaryingsMeshToPS Vert ( AttributesMesh v )
			{
				return VertexFunction( v );
			}
			#endif

			#ifdef UNITY_VIRTUAL_TEXTURING
			#define VT_BUFFER_TARGET SV_Target1
			#define EXTRA_BUFFER_TARGET SV_Target2
			#else
			#define EXTRA_BUFFER_TARGET SV_Target1
			#endif

			void Frag(PackedVaryingsMeshToPS packedInput,
					#ifdef OUTPUT_SPLIT_LIGHTING
						out float4 outColor : SV_Target0,
						#ifdef UNITY_VIRTUAL_TEXTURING
							out float4 outVTFeedback : VT_BUFFER_TARGET,
						#endif
						out float4 outDiffuseLighting : EXTRA_BUFFER_TARGET,
						OUTPUT_SSSBUFFER(outSSSBuffer)
					#else
						out float4 outColor : SV_Target0
						#ifdef UNITY_VIRTUAL_TEXTURING
							,out float4 outVTFeedback : VT_BUFFER_TARGET
						#endif
					#ifdef _WRITE_TRANSPARENT_MOTION_VECTOR
						, out float4 outMotionVec : SV_Target1
					#endif
					#endif
					#ifdef _DEPTHOFFSET_ON
						, out float outputDepth : SV_Depth
					#endif
					
						)
			{
				#ifdef _WRITE_TRANSPARENT_MOTION_VECTOR
					outMotionVec = float4(2.0, 0.0, 0.0, 0.0);
				#endif

				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( packedInput );
				UNITY_SETUP_INSTANCE_ID( packedInput );
				float3 positionRWS = packedInput.positionRWS.xyz;
				float3 normalWS = packedInput.normalWS.xyz;
				float4 tangentWS = packedInput.tangentWS.xyzw;

				FragInputs input;
				ZERO_INITIALIZE(FragInputs, input);
				input.tangentToWorld = k_identity3x3;
				input.positionSS = packedInput.positionCS;
				input.positionRWS = positionRWS;
				input.tangentToWorld = BuildTangentToWorld(tangentWS, normalWS);
				input.texCoord1 = packedInput.uv1.xyzw;
				input.texCoord2 = packedInput.uv2.xyzw;

				#if _DOUBLESIDED_ON && SHADER_STAGE_FRAGMENT
				input.isFrontFace = IS_FRONT_VFACE( packedInput.cullFace, true, false);
				#elif SHADER_STAGE_FRAGMENT
				#if defined(ASE_NEED_CULLFACE)
				input.isFrontFace = IS_FRONT_VFACE(packedInput.cullFace, true, false);
				#endif
				#endif
				half isFrontFace = input.isFrontFace;

				input.positionSS.xy = _OffScreenRendering > 0 ? (input.positionSS.xy * _OffScreenDownsampleFactor) : input.positionSS.xy;
				uint2 tileIndex = uint2(input.positionSS.xy) / GetTileSize ();

				PositionInputs posInput = GetPositionInput( input.positionSS.xy, _ScreenSize.zw, input.positionSS.z, input.positionSS.w, input.positionRWS.xyz, tileIndex );

				float3 V = GetWorldSpaceNormalizeViewDir(input.positionRWS);

				GlobalSurfaceDescription surfaceDescription = (GlobalSurfaceDescription)0;
				float2 uv_MatTexArray = packedInput.ase_texcoord7.xy;
				float3 ase_worldPos = GetAbsolutePositionWS( positionRWS );
				float2 appendResult257 = (float2(ase_worldPos.x , ase_worldPos.z));
				float2 appendResult260 = (float2(_WorldBounds.x , _WorldBounds.y));
				float2 appendResult259 = (float2(_WorldBounds.z , _WorldBounds.w));
				float2 temp_output_264_0 = ( ( appendResult257 - appendResult260 ) / ( appendResult259 - appendResult260 ) );
				float2 appendResult265 = (float2(_Control_TexelSize.z , _Control_TexelSize.w));
				float2 temp_output_266_0 = ( temp_output_264_0 * appendResult265 );
				float2 temp_output_270_0 = floor( ( ( temp_output_266_0 + float2( 0.5,0.5 ) ) - float2( 0.5,0.5 ) ) );
				float2 temp_output_12_0_g423 = ( ( temp_output_270_0 + float2( 0,0 ) ) / appendResult265 );
				float4 tex2DNode6_g423 = tex2D( _Control, temp_output_12_0_g423 );
				float4 tex2DArrayNode23_g423 = SAMPLE_TEXTURE2D_ARRAY( _MatTexArray, sampler_MatTexArray, uv_MatTexArray,( tex2DNode6_g423.r * 255.0 ) );
				float4 tex2DNode15_g423 = tex2D( _GrassControl, temp_output_12_0_g423 );
				float2 uv_ShapeMap = packedInput.ase_texcoord7.xy;
				float4 tex2DArrayNode35_g423 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode15_g423.g * 255.0 ) );
				float temp_output_24_0_g425 = tex2DArrayNode35_g423.b;
				float4 tex2DNode16_g423 = tex2D( _GrassTint, temp_output_12_0_g423 );
				float temp_output_25_0_g425 = tex2DNode16_g423.a;
				float4 tex2DArrayNode32_g423 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode6_g423.g * 255.0 ) );
				float temp_output_26_0_g425 = tex2DArrayNode32_g423.b;
				float temp_output_41_0_g423 = ( 1.0 - tex2DNode16_g423.a );
				float temp_output_27_0_g425 = temp_output_41_0_g423;
				float temp_output_1_0_g425 = ( max( ( temp_output_24_0_g425 + temp_output_25_0_g425 ) , ( temp_output_26_0_g425 + temp_output_27_0_g425 ) ) - 0.2 );
				float temp_output_21_0_g425 = max( ( ( temp_output_24_0_g425 + temp_output_25_0_g425 ) - temp_output_1_0_g425 ) , 0.0 );
				float temp_output_19_0_g425 = max( ( ( temp_output_26_0_g425 + temp_output_27_0_g425 ) - temp_output_1_0_g425 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch29_g423 = ( ( ( SAMPLE_TEXTURE2D_ARRAY( _MatTexArray, sampler_MatTexArray, uv_MatTexArray,( tex2DNode15_g423.r * 255.0 ) ) * temp_output_21_0_g425 ) + ( tex2DArrayNode23_g423 * temp_output_19_0_g425 ) ) / ( temp_output_21_0_g425 + temp_output_19_0_g425 ) );
				#else
				float4 staticSwitch29_g423 = tex2DArrayNode23_g423;
				#endif
				float temp_output_24_0_g424 = tex2DArrayNode35_g423.b;
				float temp_output_25_0_g424 = tex2DNode16_g423.a;
				float temp_output_26_0_g424 = tex2DArrayNode32_g423.b;
				float temp_output_27_0_g424 = temp_output_41_0_g423;
				float temp_output_1_0_g424 = ( max( ( temp_output_24_0_g424 + temp_output_25_0_g424 ) , ( temp_output_26_0_g424 + temp_output_27_0_g424 ) ) - 0.2 );
				float temp_output_21_0_g424 = max( ( ( temp_output_24_0_g424 + temp_output_25_0_g424 ) - temp_output_1_0_g424 ) , 0.0 );
				float temp_output_19_0_g424 = max( ( ( temp_output_26_0_g424 + temp_output_27_0_g424 ) - temp_output_1_0_g424 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch31_g423 = ( ( ( tex2DArrayNode35_g423 * temp_output_21_0_g424 ) + ( tex2DArrayNode32_g423 * temp_output_19_0_g424 ) ) / ( temp_output_21_0_g424 + temp_output_19_0_g424 ) );
				#else
				float4 staticSwitch31_g423 = tex2DArrayNode32_g423;
				#endif
				float temp_output_302_3 = staticSwitch31_g423.b;
				float temp_output_24_0_g443 = temp_output_302_3;
				float2 break273 = ( temp_output_266_0 - temp_output_270_0 );
				float temp_output_274_0 = ( 1.0 - break273.x );
				float temp_output_25_0_g443 = temp_output_274_0;
				float2 temp_output_12_0_g419 = ( ( temp_output_270_0 + float2( 1,0 ) ) / appendResult265 );
				float4 tex2DNode6_g419 = tex2D( _Control, temp_output_12_0_g419 );
				float4 tex2DArrayNode32_g419 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode6_g419.g * 255.0 ) );
				float4 tex2DNode15_g419 = tex2D( _GrassControl, temp_output_12_0_g419 );
				float4 tex2DArrayNode35_g419 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode15_g419.g * 255.0 ) );
				float temp_output_24_0_g420 = tex2DArrayNode35_g419.b;
				float4 tex2DNode16_g419 = tex2D( _GrassTint, temp_output_12_0_g419 );
				float temp_output_25_0_g420 = tex2DNode16_g419.a;
				float temp_output_26_0_g420 = tex2DArrayNode32_g419.b;
				float temp_output_41_0_g419 = ( 1.0 - tex2DNode16_g419.a );
				float temp_output_27_0_g420 = temp_output_41_0_g419;
				float temp_output_1_0_g420 = ( max( ( temp_output_24_0_g420 + temp_output_25_0_g420 ) , ( temp_output_26_0_g420 + temp_output_27_0_g420 ) ) - 0.2 );
				float temp_output_21_0_g420 = max( ( ( temp_output_24_0_g420 + temp_output_25_0_g420 ) - temp_output_1_0_g420 ) , 0.0 );
				float temp_output_19_0_g420 = max( ( ( temp_output_26_0_g420 + temp_output_27_0_g420 ) - temp_output_1_0_g420 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch31_g419 = ( ( ( tex2DArrayNode35_g419 * temp_output_21_0_g420 ) + ( tex2DArrayNode32_g419 * temp_output_19_0_g420 ) ) / ( temp_output_21_0_g420 + temp_output_19_0_g420 ) );
				#else
				float4 staticSwitch31_g419 = tex2DArrayNode32_g419;
				#endif
				float temp_output_299_3 = staticSwitch31_g419.b;
				float temp_output_26_0_g443 = temp_output_299_3;
				float temp_output_27_0_g443 = break273.x;
				float temp_output_1_0_g443 = ( max( ( temp_output_24_0_g443 + temp_output_25_0_g443 ) , ( temp_output_26_0_g443 + temp_output_27_0_g443 ) ) - 0.2 );
				float temp_output_21_0_g443 = max( ( ( temp_output_24_0_g443 + temp_output_25_0_g443 ) - temp_output_1_0_g443 ) , 0.0 );
				float4 tex2DArrayNode23_g419 = SAMPLE_TEXTURE2D_ARRAY( _MatTexArray, sampler_MatTexArray, uv_MatTexArray,( tex2DNode6_g419.r * 255.0 ) );
				float temp_output_24_0_g421 = tex2DArrayNode35_g419.b;
				float temp_output_25_0_g421 = tex2DNode16_g419.a;
				float temp_output_26_0_g421 = tex2DArrayNode32_g419.b;
				float temp_output_27_0_g421 = temp_output_41_0_g419;
				float temp_output_1_0_g421 = ( max( ( temp_output_24_0_g421 + temp_output_25_0_g421 ) , ( temp_output_26_0_g421 + temp_output_27_0_g421 ) ) - 0.2 );
				float temp_output_21_0_g421 = max( ( ( temp_output_24_0_g421 + temp_output_25_0_g421 ) - temp_output_1_0_g421 ) , 0.0 );
				float temp_output_19_0_g421 = max( ( ( temp_output_26_0_g421 + temp_output_27_0_g421 ) - temp_output_1_0_g421 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch29_g419 = ( ( ( SAMPLE_TEXTURE2D_ARRAY( _MatTexArray, sampler_MatTexArray, uv_MatTexArray,( tex2DNode15_g419.r * 255.0 ) ) * temp_output_21_0_g421 ) + ( tex2DArrayNode23_g419 * temp_output_19_0_g421 ) ) / ( temp_output_21_0_g421 + temp_output_19_0_g421 ) );
				#else
				float4 staticSwitch29_g419 = tex2DArrayNode23_g419;
				#endif
				float temp_output_19_0_g443 = max( ( ( temp_output_26_0_g443 + temp_output_27_0_g443 ) - temp_output_1_0_g443 ) , 0.0 );
				float temp_output_24_0_g436 = temp_output_302_3;
				float temp_output_25_0_g436 = temp_output_274_0;
				float temp_output_26_0_g436 = temp_output_299_3;
				float temp_output_27_0_g436 = break273.x;
				float temp_output_1_0_g436 = ( max( ( temp_output_24_0_g436 + temp_output_25_0_g436 ) , ( temp_output_26_0_g436 + temp_output_27_0_g436 ) ) - 0.2 );
				float temp_output_21_0_g436 = max( ( ( temp_output_24_0_g436 + temp_output_25_0_g436 ) - temp_output_1_0_g436 ) , 0.0 );
				float temp_output_19_0_g436 = max( ( ( temp_output_26_0_g436 + temp_output_27_0_g436 ) - temp_output_1_0_g436 ) , 0.0 );
				float4 temp_output_290_0 = ( ( ( staticSwitch31_g423 * temp_output_21_0_g436 ) + ( staticSwitch31_g419 * temp_output_19_0_g436 ) ) / ( temp_output_21_0_g436 + temp_output_19_0_g436 ) );
				float temp_output_24_0_g442 = temp_output_290_0.z;
				float temp_output_275_0 = ( 1.0 - break273.y );
				float temp_output_25_0_g442 = temp_output_275_0;
				float2 temp_output_12_0_g431 = ( ( temp_output_270_0 + float2( 0,1 ) ) / appendResult265 );
				float4 tex2DNode6_g431 = tex2D( _Control, temp_output_12_0_g431 );
				float4 tex2DArrayNode32_g431 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode6_g431.g * 255.0 ) );
				float4 tex2DNode15_g431 = tex2D( _GrassControl, temp_output_12_0_g431 );
				float4 tex2DArrayNode35_g431 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode15_g431.g * 255.0 ) );
				float temp_output_24_0_g432 = tex2DArrayNode35_g431.b;
				float4 tex2DNode16_g431 = tex2D( _GrassTint, temp_output_12_0_g431 );
				float temp_output_25_0_g432 = tex2DNode16_g431.a;
				float temp_output_26_0_g432 = tex2DArrayNode32_g431.b;
				float temp_output_41_0_g431 = ( 1.0 - tex2DNode16_g431.a );
				float temp_output_27_0_g432 = temp_output_41_0_g431;
				float temp_output_1_0_g432 = ( max( ( temp_output_24_0_g432 + temp_output_25_0_g432 ) , ( temp_output_26_0_g432 + temp_output_27_0_g432 ) ) - 0.2 );
				float temp_output_21_0_g432 = max( ( ( temp_output_24_0_g432 + temp_output_25_0_g432 ) - temp_output_1_0_g432 ) , 0.0 );
				float temp_output_19_0_g432 = max( ( ( temp_output_26_0_g432 + temp_output_27_0_g432 ) - temp_output_1_0_g432 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch31_g431 = ( ( ( tex2DArrayNode35_g431 * temp_output_21_0_g432 ) + ( tex2DArrayNode32_g431 * temp_output_19_0_g432 ) ) / ( temp_output_21_0_g432 + temp_output_19_0_g432 ) );
				#else
				float4 staticSwitch31_g431 = tex2DArrayNode32_g431;
				#endif
				float temp_output_300_3 = staticSwitch31_g431.b;
				float temp_output_24_0_g435 = temp_output_300_3;
				float temp_output_25_0_g435 = temp_output_274_0;
				float2 temp_output_12_0_g427 = ( ( temp_output_270_0 + float2( 1,1 ) ) / appendResult265 );
				float4 tex2DNode6_g427 = tex2D( _Control, temp_output_12_0_g427 );
				float4 tex2DArrayNode32_g427 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode6_g427.g * 255.0 ) );
				float4 tex2DNode15_g427 = tex2D( _GrassControl, temp_output_12_0_g427 );
				float4 tex2DArrayNode35_g427 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode15_g427.g * 255.0 ) );
				float temp_output_24_0_g428 = tex2DArrayNode35_g427.b;
				float4 tex2DNode16_g427 = tex2D( _GrassTint, temp_output_12_0_g427 );
				float temp_output_25_0_g428 = tex2DNode16_g427.a;
				float temp_output_26_0_g428 = tex2DArrayNode32_g427.b;
				float temp_output_41_0_g427 = ( 1.0 - tex2DNode16_g427.a );
				float temp_output_27_0_g428 = temp_output_41_0_g427;
				float temp_output_1_0_g428 = ( max( ( temp_output_24_0_g428 + temp_output_25_0_g428 ) , ( temp_output_26_0_g428 + temp_output_27_0_g428 ) ) - 0.2 );
				float temp_output_21_0_g428 = max( ( ( temp_output_24_0_g428 + temp_output_25_0_g428 ) - temp_output_1_0_g428 ) , 0.0 );
				float temp_output_19_0_g428 = max( ( ( temp_output_26_0_g428 + temp_output_27_0_g428 ) - temp_output_1_0_g428 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch31_g427 = ( ( ( tex2DArrayNode35_g427 * temp_output_21_0_g428 ) + ( tex2DArrayNode32_g427 * temp_output_19_0_g428 ) ) / ( temp_output_21_0_g428 + temp_output_19_0_g428 ) );
				#else
				float4 staticSwitch31_g427 = tex2DArrayNode32_g427;
				#endif
				float temp_output_301_3 = staticSwitch31_g427.b;
				float temp_output_26_0_g435 = temp_output_301_3;
				float temp_output_27_0_g435 = break273.x;
				float temp_output_1_0_g435 = ( max( ( temp_output_24_0_g435 + temp_output_25_0_g435 ) , ( temp_output_26_0_g435 + temp_output_27_0_g435 ) ) - 0.2 );
				float temp_output_21_0_g435 = max( ( ( temp_output_24_0_g435 + temp_output_25_0_g435 ) - temp_output_1_0_g435 ) , 0.0 );
				float temp_output_19_0_g435 = max( ( ( temp_output_26_0_g435 + temp_output_27_0_g435 ) - temp_output_1_0_g435 ) , 0.0 );
				float4 temp_output_293_0 = ( ( ( staticSwitch31_g431 * temp_output_21_0_g435 ) + ( staticSwitch31_g427 * temp_output_19_0_g435 ) ) / ( temp_output_21_0_g435 + temp_output_19_0_g435 ) );
				float temp_output_26_0_g442 = temp_output_293_0.z;
				float temp_output_27_0_g442 = break273.y;
				float temp_output_1_0_g442 = ( max( ( temp_output_24_0_g442 + temp_output_25_0_g442 ) , ( temp_output_26_0_g442 + temp_output_27_0_g442 ) ) - 0.2 );
				float temp_output_21_0_g442 = max( ( ( temp_output_24_0_g442 + temp_output_25_0_g442 ) - temp_output_1_0_g442 ) , 0.0 );
				float4 tex2DArrayNode23_g431 = SAMPLE_TEXTURE2D_ARRAY( _MatTexArray, sampler_MatTexArray, uv_MatTexArray,( tex2DNode6_g431.r * 255.0 ) );
				float temp_output_24_0_g433 = tex2DArrayNode35_g431.b;
				float temp_output_25_0_g433 = tex2DNode16_g431.a;
				float temp_output_26_0_g433 = tex2DArrayNode32_g431.b;
				float temp_output_27_0_g433 = temp_output_41_0_g431;
				float temp_output_1_0_g433 = ( max( ( temp_output_24_0_g433 + temp_output_25_0_g433 ) , ( temp_output_26_0_g433 + temp_output_27_0_g433 ) ) - 0.2 );
				float temp_output_21_0_g433 = max( ( ( temp_output_24_0_g433 + temp_output_25_0_g433 ) - temp_output_1_0_g433 ) , 0.0 );
				float temp_output_19_0_g433 = max( ( ( temp_output_26_0_g433 + temp_output_27_0_g433 ) - temp_output_1_0_g433 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch29_g431 = ( ( ( SAMPLE_TEXTURE2D_ARRAY( _MatTexArray, sampler_MatTexArray, uv_MatTexArray,( tex2DNode15_g431.r * 255.0 ) ) * temp_output_21_0_g433 ) + ( tex2DArrayNode23_g431 * temp_output_19_0_g433 ) ) / ( temp_output_21_0_g433 + temp_output_19_0_g433 ) );
				#else
				float4 staticSwitch29_g431 = tex2DArrayNode23_g431;
				#endif
				float temp_output_24_0_g488 = temp_output_300_3;
				float temp_output_25_0_g488 = temp_output_274_0;
				float temp_output_26_0_g488 = temp_output_301_3;
				float temp_output_27_0_g488 = break273.x;
				float temp_output_1_0_g488 = ( max( ( temp_output_24_0_g488 + temp_output_25_0_g488 ) , ( temp_output_26_0_g488 + temp_output_27_0_g488 ) ) - 0.2 );
				float temp_output_21_0_g488 = max( ( ( temp_output_24_0_g488 + temp_output_25_0_g488 ) - temp_output_1_0_g488 ) , 0.0 );
				float4 tex2DArrayNode23_g427 = SAMPLE_TEXTURE2D_ARRAY( _MatTexArray, sampler_MatTexArray, uv_MatTexArray,( tex2DNode6_g427.r * 255.0 ) );
				float temp_output_24_0_g429 = tex2DArrayNode35_g427.b;
				float temp_output_25_0_g429 = tex2DNode16_g427.a;
				float temp_output_26_0_g429 = tex2DArrayNode32_g427.b;
				float temp_output_27_0_g429 = temp_output_41_0_g427;
				float temp_output_1_0_g429 = ( max( ( temp_output_24_0_g429 + temp_output_25_0_g429 ) , ( temp_output_26_0_g429 + temp_output_27_0_g429 ) ) - 0.2 );
				float temp_output_21_0_g429 = max( ( ( temp_output_24_0_g429 + temp_output_25_0_g429 ) - temp_output_1_0_g429 ) , 0.0 );
				float temp_output_19_0_g429 = max( ( ( temp_output_26_0_g429 + temp_output_27_0_g429 ) - temp_output_1_0_g429 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch29_g427 = ( ( ( SAMPLE_TEXTURE2D_ARRAY( _MatTexArray, sampler_MatTexArray, uv_MatTexArray,( tex2DNode15_g427.r * 255.0 ) ) * temp_output_21_0_g429 ) + ( tex2DArrayNode23_g427 * temp_output_19_0_g429 ) ) / ( temp_output_21_0_g429 + temp_output_19_0_g429 ) );
				#else
				float4 staticSwitch29_g427 = tex2DArrayNode23_g427;
				#endif
				float temp_output_19_0_g488 = max( ( ( temp_output_26_0_g488 + temp_output_27_0_g488 ) - temp_output_1_0_g488 ) , 0.0 );
				float temp_output_19_0_g442 = max( ( ( temp_output_26_0_g442 + temp_output_27_0_g442 ) - temp_output_1_0_g442 ) , 0.0 );
				float4 temp_output_159_0 = ( ( ( ( ( ( staticSwitch29_g423 * temp_output_21_0_g443 ) + ( staticSwitch29_g419 * temp_output_19_0_g443 ) ) / ( temp_output_21_0_g443 + temp_output_19_0_g443 ) ) * temp_output_21_0_g442 ) + ( ( ( ( staticSwitch29_g431 * temp_output_21_0_g488 ) + ( staticSwitch29_g427 * temp_output_19_0_g488 ) ) / ( temp_output_21_0_g488 + temp_output_19_0_g488 ) ) * temp_output_19_0_g442 ) ) / ( temp_output_21_0_g442 + temp_output_19_0_g442 ) );
				float4 tex2DNode14_g423 = tex2D( _Tint, temp_output_12_0_g423 );
				float temp_output_24_0_g426 = tex2DArrayNode35_g423.b;
				float temp_output_25_0_g426 = tex2DNode16_g423.a;
				float temp_output_26_0_g426 = tex2DArrayNode32_g423.b;
				float temp_output_27_0_g426 = temp_output_41_0_g423;
				float temp_output_1_0_g426 = ( max( ( temp_output_24_0_g426 + temp_output_25_0_g426 ) , ( temp_output_26_0_g426 + temp_output_27_0_g426 ) ) - 0.2 );
				float temp_output_21_0_g426 = max( ( ( temp_output_24_0_g426 + temp_output_25_0_g426 ) - temp_output_1_0_g426 ) , 0.0 );
				float temp_output_19_0_g426 = max( ( ( temp_output_26_0_g426 + temp_output_27_0_g426 ) - temp_output_1_0_g426 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch30_g423 = ( ( ( tex2DNode16_g423 * temp_output_21_0_g426 ) + ( tex2DNode14_g423 * temp_output_19_0_g426 ) ) / ( temp_output_21_0_g426 + temp_output_19_0_g426 ) );
				#else
				float4 staticSwitch30_g423 = tex2DNode14_g423;
				#endif
				float temp_output_24_0_g437 = temp_output_302_3;
				float temp_output_25_0_g437 = temp_output_274_0;
				float temp_output_26_0_g437 = temp_output_299_3;
				float temp_output_27_0_g437 = break273.x;
				float temp_output_1_0_g437 = ( max( ( temp_output_24_0_g437 + temp_output_25_0_g437 ) , ( temp_output_26_0_g437 + temp_output_27_0_g437 ) ) - 0.2 );
				float temp_output_21_0_g437 = max( ( ( temp_output_24_0_g437 + temp_output_25_0_g437 ) - temp_output_1_0_g437 ) , 0.0 );
				float4 tex2DNode14_g419 = tex2D( _Tint, temp_output_12_0_g419 );
				float temp_output_24_0_g422 = tex2DArrayNode35_g419.b;
				float temp_output_25_0_g422 = tex2DNode16_g419.a;
				float temp_output_26_0_g422 = tex2DArrayNode32_g419.b;
				float temp_output_27_0_g422 = temp_output_41_0_g419;
				float temp_output_1_0_g422 = ( max( ( temp_output_24_0_g422 + temp_output_25_0_g422 ) , ( temp_output_26_0_g422 + temp_output_27_0_g422 ) ) - 0.2 );
				float temp_output_21_0_g422 = max( ( ( temp_output_24_0_g422 + temp_output_25_0_g422 ) - temp_output_1_0_g422 ) , 0.0 );
				float temp_output_19_0_g422 = max( ( ( temp_output_26_0_g422 + temp_output_27_0_g422 ) - temp_output_1_0_g422 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch30_g419 = ( ( ( tex2DNode16_g419 * temp_output_21_0_g422 ) + ( tex2DNode14_g419 * temp_output_19_0_g422 ) ) / ( temp_output_21_0_g422 + temp_output_19_0_g422 ) );
				#else
				float4 staticSwitch30_g419 = tex2DNode14_g419;
				#endif
				float temp_output_19_0_g437 = max( ( ( temp_output_26_0_g437 + temp_output_27_0_g437 ) - temp_output_1_0_g437 ) , 0.0 );
				float temp_output_24_0_g439 = temp_output_290_0.z;
				float temp_output_25_0_g439 = temp_output_275_0;
				float temp_output_26_0_g439 = temp_output_293_0.z;
				float temp_output_27_0_g439 = break273.y;
				float temp_output_1_0_g439 = ( max( ( temp_output_24_0_g439 + temp_output_25_0_g439 ) , ( temp_output_26_0_g439 + temp_output_27_0_g439 ) ) - 0.2 );
				float temp_output_21_0_g439 = max( ( ( temp_output_24_0_g439 + temp_output_25_0_g439 ) - temp_output_1_0_g439 ) , 0.0 );
				float4 tex2DNode14_g431 = tex2D( _Tint, temp_output_12_0_g431 );
				float temp_output_24_0_g434 = tex2DArrayNode35_g431.b;
				float temp_output_25_0_g434 = tex2DNode16_g431.a;
				float temp_output_26_0_g434 = tex2DArrayNode32_g431.b;
				float temp_output_27_0_g434 = temp_output_41_0_g431;
				float temp_output_1_0_g434 = ( max( ( temp_output_24_0_g434 + temp_output_25_0_g434 ) , ( temp_output_26_0_g434 + temp_output_27_0_g434 ) ) - 0.2 );
				float temp_output_21_0_g434 = max( ( ( temp_output_24_0_g434 + temp_output_25_0_g434 ) - temp_output_1_0_g434 ) , 0.0 );
				float temp_output_19_0_g434 = max( ( ( temp_output_26_0_g434 + temp_output_27_0_g434 ) - temp_output_1_0_g434 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch30_g431 = ( ( ( tex2DNode16_g431 * temp_output_21_0_g434 ) + ( tex2DNode14_g431 * temp_output_19_0_g434 ) ) / ( temp_output_21_0_g434 + temp_output_19_0_g434 ) );
				#else
				float4 staticSwitch30_g431 = tex2DNode14_g431;
				#endif
				float temp_output_24_0_g438 = temp_output_300_3;
				float temp_output_25_0_g438 = temp_output_274_0;
				float temp_output_26_0_g438 = temp_output_301_3;
				float temp_output_27_0_g438 = break273.x;
				float temp_output_1_0_g438 = ( max( ( temp_output_24_0_g438 + temp_output_25_0_g438 ) , ( temp_output_26_0_g438 + temp_output_27_0_g438 ) ) - 0.2 );
				float temp_output_21_0_g438 = max( ( ( temp_output_24_0_g438 + temp_output_25_0_g438 ) - temp_output_1_0_g438 ) , 0.0 );
				float4 tex2DNode14_g427 = tex2D( _Tint, temp_output_12_0_g427 );
				float temp_output_24_0_g430 = tex2DArrayNode35_g427.b;
				float temp_output_25_0_g430 = tex2DNode16_g427.a;
				float temp_output_26_0_g430 = tex2DArrayNode32_g427.b;
				float temp_output_27_0_g430 = temp_output_41_0_g427;
				float temp_output_1_0_g430 = ( max( ( temp_output_24_0_g430 + temp_output_25_0_g430 ) , ( temp_output_26_0_g430 + temp_output_27_0_g430 ) ) - 0.2 );
				float temp_output_21_0_g430 = max( ( ( temp_output_24_0_g430 + temp_output_25_0_g430 ) - temp_output_1_0_g430 ) , 0.0 );
				float temp_output_19_0_g430 = max( ( ( temp_output_26_0_g430 + temp_output_27_0_g430 ) - temp_output_1_0_g430 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch30_g427 = ( ( ( tex2DNode16_g427 * temp_output_21_0_g430 ) + ( tex2DNode14_g427 * temp_output_19_0_g430 ) ) / ( temp_output_21_0_g430 + temp_output_19_0_g430 ) );
				#else
				float4 staticSwitch30_g427 = tex2DNode14_g427;
				#endif
				float temp_output_19_0_g438 = max( ( ( temp_output_26_0_g438 + temp_output_27_0_g438 ) - temp_output_1_0_g438 ) , 0.0 );
				float temp_output_19_0_g439 = max( ( ( temp_output_26_0_g439 + temp_output_27_0_g439 ) - temp_output_1_0_g439 ) , 0.0 );
				float4 temp_output_221_0 = ( ( ( ( ( ( staticSwitch30_g423 * temp_output_21_0_g437 ) + ( staticSwitch30_g419 * temp_output_19_0_g437 ) ) / ( temp_output_21_0_g437 + temp_output_19_0_g437 ) ) * temp_output_21_0_g439 ) + ( ( ( ( staticSwitch30_g431 * temp_output_21_0_g438 ) + ( staticSwitch30_g427 * temp_output_19_0_g438 ) ) / ( temp_output_21_0_g438 + temp_output_19_0_g438 ) ) * temp_output_19_0_g439 ) ) / ( temp_output_21_0_g439 + temp_output_19_0_g439 ) );
				float2 appendResult12_g487 = (float2(ase_worldPos.x , ase_worldPos.z));
				float4 break7_g487 = _WorldBounds;
				float2 appendResult8_g487 = (float2(break7_g487.x , break7_g487.y));
				float2 appendResult9_g487 = (float2(break7_g487.z , break7_g487.w));
				float4 tex2DNode15_g487 = tex2D( _SpatterTex, ( ( appendResult12_g487 - appendResult8_g487 ) / ( appendResult9_g487 - temp_output_264_0 ) ) );
				float simplePerlin3D16_g487 = snoise( float3( appendResult12_g487 ,  0.0 )*2.0 );
				simplePerlin3D16_g487 = simplePerlin3D16_g487*0.5 + 0.5;
				float temp_output_17_0_g487 = ( tex2DNode15_g487.a - simplePerlin3D16_g487 );
				float HeightMask33_g487 = saturate(pow(max( (((simplePerlin3D16_g487*temp_output_17_0_g487)*4)+(temp_output_17_0_g487*2)), 0 ),1.0));
				float4 lerpResult34_g487 = lerp( ( temp_output_159_0 * temp_output_221_0 ) , tex2DNode15_g487 , HeightMask33_g487);
				
				float temp_output_24_0_g441 = temp_output_290_0.z;
				float temp_output_25_0_g441 = temp_output_275_0;
				float temp_output_26_0_g441 = temp_output_293_0.z;
				float temp_output_27_0_g441 = break273.y;
				float temp_output_1_0_g441 = ( max( ( temp_output_24_0_g441 + temp_output_25_0_g441 ) , ( temp_output_26_0_g441 + temp_output_27_0_g441 ) ) - 0.2 );
				float temp_output_21_0_g441 = max( ( ( temp_output_24_0_g441 + temp_output_25_0_g441 ) - temp_output_1_0_g441 ) , 0.0 );
				float temp_output_19_0_g441 = max( ( ( temp_output_26_0_g441 + temp_output_27_0_g441 ) - temp_output_1_0_g441 ) , 0.0 );
				float4 temp_output_180_0 = ( ( ( temp_output_290_0 * temp_output_21_0_g441 ) + ( temp_output_293_0 * temp_output_19_0_g441 ) ) / ( temp_output_21_0_g441 + temp_output_19_0_g441 ) );
				float3 unpack220 = UnpackNormalScale( temp_output_180_0, _NormalScale );
				unpack220.z = lerp( 1, unpack220.z, saturate(_NormalScale) );
				
				float temp_output_3_0_g440 = ( temp_output_221_0.w * 2.0 );
				
				surfaceDescription.Albedo = lerpResult34_g487.rgb;
				surfaceDescription.Normal = unpack220;
				surfaceDescription.BentNormal = float3( 0, 0, 1 );
				surfaceDescription.CoatMask = 0;
				surfaceDescription.Metallic = ( 0.0 * _MetalLevel );

				#ifdef _MATERIAL_FEATURE_SPECULAR_COLOR
				surfaceDescription.Specular = 0;
				#endif

				surfaceDescription.Emission = 0;
				surfaceDescription.Smoothness = ( temp_output_159_0.w * _Rough );
				surfaceDescription.Occlusion = temp_output_180_0.x;
				surfaceDescription.Alpha = min( temp_output_3_0_g440 , 1.0 );

				#ifdef _ALPHATEST_ON
				surfaceDescription.AlphaClipThreshold = _AlphaCutoff;
				#endif

				#ifdef _ENABLE_GEOMETRIC_SPECULAR_AA
				surfaceDescription.SpecularAAScreenSpaceVariance = 0;
				surfaceDescription.SpecularAAThreshold = 0;
				#endif

				#ifdef _SPECULAR_OCCLUSION_CUSTOM
				surfaceDescription.SpecularOcclusion = 0;
				#endif

				#if defined(_HAS_REFRACTION) || defined(_MATERIAL_FEATURE_TRANSMISSION)
				surfaceDescription.Thickness = 1;
				#endif

				#ifdef _HAS_REFRACTION
				surfaceDescription.RefractionIndex = 1;
				surfaceDescription.RefractionColor = float3( 1, 1, 1 );
				surfaceDescription.RefractionDistance = 0;
				#endif

				#ifdef _MATERIAL_FEATURE_SUBSURFACE_SCATTERING
				surfaceDescription.SubsurfaceMask = 1;
				#endif

				#if defined( _MATERIAL_FEATURE_SUBSURFACE_SCATTERING ) || defined( _MATERIAL_FEATURE_TRANSMISSION )
				surfaceDescription.DiffusionProfile = 0;
				#endif

				#ifdef _MATERIAL_FEATURE_ANISOTROPY
				surfaceDescription.Anisotropy = 1;
				surfaceDescription.Tangent = float3( 1, 0, 0 );
				#endif

				#ifdef _MATERIAL_FEATURE_IRIDESCENCE
				surfaceDescription.IridescenceMask = 0;
				surfaceDescription.IridescenceThickness = 0;
				#endif

				#ifdef _ASE_BAKEDGI
				surfaceDescription.BakedGI = 0;
				#endif
				#ifdef _ASE_BAKEDBACKGI
				surfaceDescription.BakedBackGI = 0;
				#endif

				#ifdef _DEPTHOFFSET_ON
				surfaceDescription.DepthOffset = 0;
				#endif

				#ifdef UNITY_VIRTUAL_TEXTURING
				surfaceDescription.VTPackedFeedback = float4(1.0f,1.0f,1.0f,1.0f);
				#endif

				SurfaceData surfaceData;
				BuiltinData builtinData;
				GetSurfaceAndBuiltinData(surfaceDescription,input, V, posInput, surfaceData, builtinData);

				BSDFData bsdfData = ConvertSurfaceDataToBSDFData(input.positionSS.xy, surfaceData);

				PreLightData preLightData = GetPreLightData(V, posInput, bsdfData);

				outColor = float4(0.0, 0.0, 0.0, 0.0);
				#ifdef DEBUG_DISPLAY
				#ifdef OUTPUT_SPLIT_LIGHTING
					outDiffuseLighting = 0;
					ENCODE_INTO_SSSBUFFER(surfaceData, posInput.positionSS, outSSSBuffer);
				#endif

				bool viewMaterial = false;
				int bufferSize = _DebugViewMaterialArray[0].x;
				if (bufferSize != 0)
				{
					bool needLinearToSRGB = false;
					float3 result = float3(1.0, 0.0, 1.0);

					for (int index = 1; index <= bufferSize; index++)
					{
						int indexMaterialProperty = _DebugViewMaterialArray[index].x;

						if (indexMaterialProperty != 0)
						{
							viewMaterial = true;

							GetPropertiesDataDebug(indexMaterialProperty, result, needLinearToSRGB);
							GetVaryingsDataDebug(indexMaterialProperty, input, result, needLinearToSRGB);
							GetBuiltinDataDebug(indexMaterialProperty, builtinData, posInput, result, needLinearToSRGB);
							GetSurfaceDataDebug(indexMaterialProperty, surfaceData, result, needLinearToSRGB);
							GetBSDFDataDebug(indexMaterialProperty, bsdfData, result, needLinearToSRGB);
						}
					}

					if (!needLinearToSRGB)
						result = SRGBToLinear(max(0, result));

					outColor = float4(result, 1.0);
				}

				if (!viewMaterial)
				{
					if (_DebugFullScreenMode == FULLSCREENDEBUGMODE_VALIDATE_DIFFUSE_COLOR || _DebugFullScreenMode == FULLSCREENDEBUGMODE_VALIDATE_SPECULAR_COLOR)
					{
						float3 result = float3(0.0, 0.0, 0.0);

						GetPBRValidatorDebug(surfaceData, result);

						outColor = float4(result, 1.0f);
					}
					else if (_DebugFullScreenMode == FULLSCREENDEBUGMODE_TRANSPARENCY_OVERDRAW)
					{
						float4 result = _DebugTransparencyOverdrawWeight * float4(TRANSPARENCY_OVERDRAW_COST, TRANSPARENCY_OVERDRAW_COST, TRANSPARENCY_OVERDRAW_COST, TRANSPARENCY_OVERDRAW_A);
						outColor = result;
					}
					else
				#endif
					{
				#ifdef _SURFACE_TYPE_TRANSPARENT
						uint featureFlags = LIGHT_FEATURE_MASK_FLAGS_TRANSPARENT;
				#else
						uint featureFlags = LIGHT_FEATURE_MASK_FLAGS_OPAQUE;
				#endif
					
						LightLoopOutput lightLoopOutput;
						LightLoop(V, posInput, preLightData, bsdfData, builtinData, featureFlags, lightLoopOutput);

						// Alias
						float3 diffuseLighting = lightLoopOutput.diffuseLighting;
						float3 specularLighting = lightLoopOutput.specularLighting;
					
						diffuseLighting *= GetCurrentExposureMultiplier();
						specularLighting *= GetCurrentExposureMultiplier();

				#ifdef OUTPUT_SPLIT_LIGHTING
						if (_EnableSubsurfaceScattering != 0 && ShouldOutputSplitLighting(bsdfData))
						{
							outColor = float4(specularLighting, 1.0);
							outDiffuseLighting = float4(TagLightingForSSS(diffuseLighting), 1.0);
						}
						else
						{
							outColor = float4(diffuseLighting + specularLighting, 1.0);
							outDiffuseLighting = 0;
						}
						ENCODE_INTO_SSSBUFFER(surfaceData, posInput.positionSS, outSSSBuffer);
				#else
						outColor = ApplyBlendMode(diffuseLighting, specularLighting, builtinData.opacity);
						outColor = EvaluateAtmosphericScattering(posInput, V, outColor);
				#endif

				#ifdef _WRITE_TRANSPARENT_MOTION_VECTOR
						float4 VPASSpositionCS = float4(packedInput.vpassPositionCS.xy, 0.0, packedInput.vpassPositionCS.z);
						float4 VPASSpreviousPositionCS = float4(packedInput.vpassPreviousPositionCS.xy, 0.0, packedInput.vpassPreviousPositionCS.z);

						bool forceNoMotion = any(unity_MotionVectorsParams.yw == 0.0);
						if (!forceNoMotion)
						{
							float2 motionVec = CalculateMotionVector(VPASSpositionCS, VPASSpreviousPositionCS);
							EncodeMotionVector(motionVec * 0.5, outMotionVec);
							outMotionVec.zw = 1.0;
						}
				#endif
					}

				#ifdef DEBUG_DISPLAY
				}
				#endif

				#ifdef _DEPTHOFFSET_ON
				outputDepth = posInput.deviceDepth;
				#endif

				#ifdef UNITY_VIRTUAL_TEXTURING
					outVTFeedback = builtinData.vtPackedFeedback;
				#endif
			}
			ENDHLSL
		}
				
		
        Pass
        {
			
            Name "ScenePickingPass"
            Tags { "LightMode"="Picking" }
            
            Cull [_CullMode]
        
            HLSLPROGRAM
        
			#define _SPECULAR_OCCLUSION_FROM_AO 1
			#define _DISABLE_DECALS 1
			#define _DISABLE_SSR 1
			#define _AMBIENT_OCCLUSION 1
			#define ASE_SRP_VERSION 999999

        
			#pragma editor_sync_compilation	
			#pragma vertex Vert
			#pragma fragment Frag
		
            
			#pragma shader_feature _SURFACE_TYPE_TRANSPARENT
			#pragma shader_feature_local _DOUBLESIDED_ON
			#pragma shader_feature_local _BLENDMODE_OFF _BLENDMODE_ALPHA _BLENDMODE_ADD _BLENDMODE_PRE_MULTIPLY
			#pragma shader_feature_local_fragment _ _ENABLE_FOG_ON_TRANSPARENT
			#pragma shader_feature_local _ALPHATEST_ON
			#pragma shader_feature_local _ _TRANSPARENT_WRITES_MOTION_VEC

        
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
        	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/GeometricTools.hlsl" 
        	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Tessellation.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/FragInputs.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl" 
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderGraphHeader.hlsl" 
            
			#ifdef DEBUG_DISPLAY
				#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Debug/DebugDisplay.hlsl"
			#endif
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
            #define VARYINGS_NEED_TANGENT_TO_WORLD
        
        
            #define SHADERPASS SHADERPASS_DEPTH_ONLY
			#define SCENEPICKINGPASS 1
            
        
			#define SHADER_UNLIT

            
            #if defined(_TRANSPARENT_WRITES_MOTION_VEC) && defined(_SURFACE_TYPE_TRANSPARENT)
                #define _WRITE_TRANSPARENT_MOTION_VECTOR
            #endif
        
			float4 _SelectionID;
            
            CBUFFER_START( UnityPerMaterial )
			float4 _MatTexArray_ST;
			float4 _WorldBounds;
			float4 _Control_TexelSize;
			float4 _ShapeMap_ST;
			float _NormalScale;
			float _MetalLevel;
			float _Rough;
			float4 _EmissionColor;
			float _AlphaCutoff;
			float _RenderQueueType;
			#ifdef _ADD_PRECOMPUTED_VELOCITY
			float _AddPrecomputedVelocity;
			#endif
			float _StencilRef;
			float _StencilWriteMask;
			float _StencilRefDepth;
			float _StencilWriteMaskDepth;
			float _StencilRefMV;
			float _StencilWriteMaskMV;
			float _StencilRefDistortionVec;
			float _StencilWriteMaskDistortionVec;
			float _StencilWriteMaskGBuffer;
			float _StencilRefGBuffer;
			float _ZTestGBuffer;
			float _RequireSplitLighting;
			float _ReceivesSSR;
			float _SurfaceType;
			float _BlendMode;
			float _SrcBlend;
			float _DstBlend;
			float _AlphaSrcBlend;
			float _AlphaDstBlend;
			float _ZWrite;
			float _TransparentZWrite;
			float _CullMode;
			float _TransparentSortPriority;
			float _EnableFogOnTransparent;
			float _CullModeForward;
			float _TransparentCullMode;
			float _ZTestDepthEqualForOpaque;
			float _ZTestTransparent;
			float _TransparentBackfaceEnable;
			float _AlphaCutoffEnable;
			float _UseShadowThreshold;
			float _DoubleSidedEnable;
			float _DoubleSidedNormalMode;
			float4 _DoubleSidedConstants;
			#ifdef TESSELLATION_ON
				float _TessPhongStrength;
				float _TessValue;
				float _TessMin;
				float _TessMax;
				float _TessEdgeLength;
				float _TessMaxDisp;
			#endif
			CBUFFER_END
			sampler2D _Tint;
			sampler2D _Control;
			sampler2D _GrassTint;
			TEXTURE2D_ARRAY(_ShapeMap);
			sampler2D _GrassControl;
			SAMPLER(sampler_ShapeMap);

        
            
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/PickingSpaceTransforms.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Unlit/Unlit.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/BuiltinUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/MaterialUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderGraphFunctions.hlsl"
        

			#define GRASS


			struct VertexInput
			{
				float3 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 tangentOS : TANGENT;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 positionCS : SV_POSITION;
				float3 normalWS : TEXCOORD0;
				float4 tangentWS : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				float4 ase_texcoord3 : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			
            struct SurfaceDescription
			{
				float Alpha;
				float AlphaClipThreshold;
			};
        
        
            void GetSurfaceAndBuiltinData(SurfaceDescription surfaceDescription, FragInputs fragInputs, float3 V, inout PositionInputs posInput, out SurfaceData surfaceData, out BuiltinData builtinData RAY_TRACING_OPTIONAL_PARAMETERS)
            {    
                #ifdef LOD_FADE_CROSSFADE 
			        LODDitheringTransition(ComputeFadeMaskSeed(V, posInput.positionSS), unity_LODFade.x);
                #endif
        
                #ifdef _ALPHATEST_ON
                    float alphaCutoff = surfaceDescription.AlphaClipThreshold;
                    GENERIC_ALPHA_TEST(surfaceDescription.Alpha, alphaCutoff);
                #endif
        
                #if !defined(SHADER_STAGE_RAY_TRACING) && _DEPTHOFFSET_ON
                ApplyDepthOffsetPositionInput(V, surfaceDescription.DepthOffset, GetViewForwardDir(), GetWorldToHClipMatrix(), posInput);
                #endif
        
                
				ZERO_INITIALIZE(SurfaceData, surfaceData);
        
				ZERO_BUILTIN_INITIALIZE(builtinData); 
				builtinData.opacity = surfaceDescription.Alpha;
        
				#if defined(DEBUG_DISPLAY)
					builtinData.renderingLayers = GetMeshRenderingLightLayer();
				#endif
        
                #ifdef _ALPHATEST_ON
                    builtinData.alphaClipTreshold = alphaCutoff;
                #endif
        
                #if _DEPTHOFFSET_ON
                builtinData.depthOffset = surfaceDescription.DepthOffset;
                #endif

        
                ApplyDebugToBuiltinData(builtinData);
                
            }
        

			VertexOutput VertexFunction(VertexInput inputMesh  )
			{

				VertexOutput o;
				ZERO_INITIALIZE(VertexOutput, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				UNITY_SETUP_INSTANCE_ID(inputMesh);
				UNITY_TRANSFER_INSTANCE_ID(inputMesh, o );

				float3 ase_worldPos = GetAbsolutePositionWS( TransformObjectToWorld( (inputMesh.positionOS).xyz ) );
				o.ase_texcoord2.xyz = ase_worldPos;
				
				o.ase_texcoord3.xy = inputMesh.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord2.w = 0;
				o.ase_texcoord3.zw = 0;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
				float3 defaultVertexValue = inputMesh.positionOS.xyz;
				#else
				float3 defaultVertexValue = float3( 0, 0, 0 );
				#endif
				float3 vertexValue =   defaultVertexValue ;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
				inputMesh.positionOS.xyz = vertexValue;
				#else
				inputMesh.positionOS.xyz += vertexValue;
				#endif

				inputMesh.normalOS =  inputMesh.normalOS ;

				float3 positionRWS = TransformObjectToWorld(inputMesh.positionOS);
				float3 normalWS = TransformObjectToWorldNormal(inputMesh.normalOS);
				float4 tangentWS = float4(TransformObjectToWorldDir(inputMesh.tangentOS.xyz), inputMesh.tangentOS.w);
				
				o.positionCS = TransformWorldToHClip(positionRWS);
				o.normalWS.xyz =  normalWS;
				o.tangentWS.xyzw =  tangentWS;
		
				return o;
			}

			#if defined(TESSELLATION_ON)
			struct VertexControl
			{
				float3 positionOS : INTERNALTESSPOS;
				float3 normalOS : NORMAL;
				float4 tangentOS : TANGENT;
				float4 ase_texcoord : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct TessellationFactors
			{
				float edge[3] : SV_TessFactor;
				float inside : SV_InsideTessFactor;
			};

			VertexControl Vert ( VertexInput v )
			{
				VertexControl o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				o.positionOS = v.positionOS;
				o.normalOS = v.normalOS;
				o.tangentOS = v.tangentOS;
				o.ase_texcoord = v.ase_texcoord;
				return o;
			}

			TessellationFactors TessellationFunction (InputPatch<VertexControl,3> v)
			{
				TessellationFactors o;
				float4 tf = 1;
				float tessValue = _TessValue; float tessMin = _TessMin; float tessMax = _TessMax;
				float edgeLength = _TessEdgeLength; float tessMaxDisp = _TessMaxDisp;
				#if (SHADEROPTIONS_CAMERA_RELATIVE_RENDERING != 0)
				float3 cameraPos = 0;
				#else
				float3 cameraPos = _WorldSpaceCameraPos;
				#endif
				#if defined(ASE_FIXED_TESSELLATION)
				tf = FixedTess( tessValue );
				#elif defined(ASE_DISTANCE_TESSELLATION)
				tf = DistanceBasedTess(float4(v[0].positionOS,1), float4(v[1].positionOS,1), float4(v[2].positionOS,1), tessValue, tessMin, tessMax, GetObjectToWorldMatrix(), cameraPos );
				#elif defined(ASE_LENGTH_TESSELLATION)
				tf = EdgeLengthBasedTess(float4(v[0].positionOS,1), float4(v[1].positionOS,1), float4(v[2].positionOS,1), edgeLength, GetObjectToWorldMatrix(), cameraPos, _ScreenParams );
				#elif defined(ASE_LENGTH_CULL_TESSELLATION)
				tf = EdgeLengthBasedTessCull(float4(v[0].positionOS,1), float4(v[1].positionOS,1), float4(v[2].positionOS,1), edgeLength, tessMaxDisp, GetObjectToWorldMatrix(), cameraPos, _ScreenParams, _FrustumPlanes );
				#endif
				o.edge[0] = tf.x; o.edge[1] = tf.y; o.edge[2] = tf.z; o.inside = tf.w;
				return o;
			}

			[domain("tri")]
			[partitioning("fractional_odd")]
			[outputtopology("triangle_cw")]
			[patchconstantfunc("TessellationFunction")]
			[outputcontrolpoints(3)]
			VertexControl HullFunction(InputPatch<VertexControl, 3> patch, uint id : SV_OutputControlPointID)
			{
			   return patch[id];
			}

			[domain("tri")]
			VertexOutput DomainFunction(TessellationFactors factors, OutputPatch<VertexControl, 3> patch, float3 bary : SV_DomainLocation)
			{
				VertexInput o = (VertexInput) 0;
				o.positionOS = patch[0].positionOS * bary.x + patch[1].positionOS * bary.y + patch[2].positionOS * bary.z;
				o.normalOS = patch[0].normalOS * bary.x + patch[1].normalOS * bary.y + patch[2].normalOS * bary.z;
				o.tangentOS = patch[0].tangentOS * bary.x + patch[1].tangentOS * bary.y + patch[2].tangentOS * bary.z;
				o.ase_texcoord = patch[0].ase_texcoord * bary.x + patch[1].ase_texcoord * bary.y + patch[2].ase_texcoord * bary.z;
				#if defined(ASE_PHONG_TESSELLATION)
				float3 pp[3];
				for (int i = 0; i < 3; ++i)
					pp[i] = o.positionOS.xyz - patch[i].normalOS * (dot(o.positionOS.xyz, patch[i].normalOS) - dot(patch[i].positionOS.xyz, patch[i].normalOS));
				float phongStrength = _TessPhongStrength;
				o.positionOS.xyz = phongStrength * (pp[0]*bary.x + pp[1]*bary.y + pp[2]*bary.z) + (1.0f-phongStrength) * o.positionOS.xyz;
				#endif
				UNITY_TRANSFER_INSTANCE_ID(patch[0], o);
				return VertexFunction(o);
			}
			#else
			VertexOutput Vert ( VertexInput v )
			{
				return VertexFunction( v );
			}
			#endif

			void Frag(	VertexOutput packedInput
						, out float4 outColor : SV_Target0	
						
					)
			{
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(packedInput);
				UNITY_SETUP_INSTANCE_ID(packedInput);
								
				FragInputs input;
				ZERO_INITIALIZE(FragInputs, input);
				input.tangentToWorld = k_identity3x3;
				input.positionSS = packedInput.positionCS;       
        
				input.tangentToWorld = BuildTangentToWorld(packedInput.tangentWS.xyzw, packedInput.normalWS.xyz);

				PositionInputs posInput = GetPositionInput(input.positionSS.xy, _ScreenSize.zw, input.positionSS.z, input.positionSS.w, input.positionRWS);
				
				SurfaceDescription surfaceDescription = (SurfaceDescription)0;
				float3 ase_worldPos = packedInput.ase_texcoord2.xyz;
				float2 appendResult257 = (float2(ase_worldPos.x , ase_worldPos.z));
				float2 appendResult260 = (float2(_WorldBounds.x , _WorldBounds.y));
				float2 appendResult259 = (float2(_WorldBounds.z , _WorldBounds.w));
				float2 temp_output_264_0 = ( ( appendResult257 - appendResult260 ) / ( appendResult259 - appendResult260 ) );
				float2 appendResult265 = (float2(_Control_TexelSize.z , _Control_TexelSize.w));
				float2 temp_output_266_0 = ( temp_output_264_0 * appendResult265 );
				float2 temp_output_270_0 = floor( ( ( temp_output_266_0 + float2( 0.5,0.5 ) ) - float2( 0.5,0.5 ) ) );
				float2 temp_output_12_0_g423 = ( ( temp_output_270_0 + float2( 0,0 ) ) / appendResult265 );
				float4 tex2DNode14_g423 = tex2D( _Tint, temp_output_12_0_g423 );
				float4 tex2DNode16_g423 = tex2D( _GrassTint, temp_output_12_0_g423 );
				float2 uv_ShapeMap = packedInput.ase_texcoord3.xy;
				float4 tex2DNode15_g423 = tex2D( _GrassControl, temp_output_12_0_g423 );
				float4 tex2DArrayNode35_g423 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode15_g423.g * 255.0 ) );
				float temp_output_24_0_g426 = tex2DArrayNode35_g423.b;
				float temp_output_25_0_g426 = tex2DNode16_g423.a;
				float4 tex2DNode6_g423 = tex2D( _Control, temp_output_12_0_g423 );
				float4 tex2DArrayNode32_g423 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode6_g423.g * 255.0 ) );
				float temp_output_26_0_g426 = tex2DArrayNode32_g423.b;
				float temp_output_41_0_g423 = ( 1.0 - tex2DNode16_g423.a );
				float temp_output_27_0_g426 = temp_output_41_0_g423;
				float temp_output_1_0_g426 = ( max( ( temp_output_24_0_g426 + temp_output_25_0_g426 ) , ( temp_output_26_0_g426 + temp_output_27_0_g426 ) ) - 0.2 );
				float temp_output_21_0_g426 = max( ( ( temp_output_24_0_g426 + temp_output_25_0_g426 ) - temp_output_1_0_g426 ) , 0.0 );
				float temp_output_19_0_g426 = max( ( ( temp_output_26_0_g426 + temp_output_27_0_g426 ) - temp_output_1_0_g426 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch30_g423 = ( ( ( tex2DNode16_g423 * temp_output_21_0_g426 ) + ( tex2DNode14_g423 * temp_output_19_0_g426 ) ) / ( temp_output_21_0_g426 + temp_output_19_0_g426 ) );
				#else
				float4 staticSwitch30_g423 = tex2DNode14_g423;
				#endif
				float temp_output_24_0_g424 = tex2DArrayNode35_g423.b;
				float temp_output_25_0_g424 = tex2DNode16_g423.a;
				float temp_output_26_0_g424 = tex2DArrayNode32_g423.b;
				float temp_output_27_0_g424 = temp_output_41_0_g423;
				float temp_output_1_0_g424 = ( max( ( temp_output_24_0_g424 + temp_output_25_0_g424 ) , ( temp_output_26_0_g424 + temp_output_27_0_g424 ) ) - 0.2 );
				float temp_output_21_0_g424 = max( ( ( temp_output_24_0_g424 + temp_output_25_0_g424 ) - temp_output_1_0_g424 ) , 0.0 );
				float temp_output_19_0_g424 = max( ( ( temp_output_26_0_g424 + temp_output_27_0_g424 ) - temp_output_1_0_g424 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch31_g423 = ( ( ( tex2DArrayNode35_g423 * temp_output_21_0_g424 ) + ( tex2DArrayNode32_g423 * temp_output_19_0_g424 ) ) / ( temp_output_21_0_g424 + temp_output_19_0_g424 ) );
				#else
				float4 staticSwitch31_g423 = tex2DArrayNode32_g423;
				#endif
				float temp_output_302_3 = staticSwitch31_g423.b;
				float temp_output_24_0_g437 = temp_output_302_3;
				float2 break273 = ( temp_output_266_0 - temp_output_270_0 );
				float temp_output_274_0 = ( 1.0 - break273.x );
				float temp_output_25_0_g437 = temp_output_274_0;
				float2 temp_output_12_0_g419 = ( ( temp_output_270_0 + float2( 1,0 ) ) / appendResult265 );
				float4 tex2DNode6_g419 = tex2D( _Control, temp_output_12_0_g419 );
				float4 tex2DArrayNode32_g419 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode6_g419.g * 255.0 ) );
				float4 tex2DNode15_g419 = tex2D( _GrassControl, temp_output_12_0_g419 );
				float4 tex2DArrayNode35_g419 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode15_g419.g * 255.0 ) );
				float temp_output_24_0_g420 = tex2DArrayNode35_g419.b;
				float4 tex2DNode16_g419 = tex2D( _GrassTint, temp_output_12_0_g419 );
				float temp_output_25_0_g420 = tex2DNode16_g419.a;
				float temp_output_26_0_g420 = tex2DArrayNode32_g419.b;
				float temp_output_41_0_g419 = ( 1.0 - tex2DNode16_g419.a );
				float temp_output_27_0_g420 = temp_output_41_0_g419;
				float temp_output_1_0_g420 = ( max( ( temp_output_24_0_g420 + temp_output_25_0_g420 ) , ( temp_output_26_0_g420 + temp_output_27_0_g420 ) ) - 0.2 );
				float temp_output_21_0_g420 = max( ( ( temp_output_24_0_g420 + temp_output_25_0_g420 ) - temp_output_1_0_g420 ) , 0.0 );
				float temp_output_19_0_g420 = max( ( ( temp_output_26_0_g420 + temp_output_27_0_g420 ) - temp_output_1_0_g420 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch31_g419 = ( ( ( tex2DArrayNode35_g419 * temp_output_21_0_g420 ) + ( tex2DArrayNode32_g419 * temp_output_19_0_g420 ) ) / ( temp_output_21_0_g420 + temp_output_19_0_g420 ) );
				#else
				float4 staticSwitch31_g419 = tex2DArrayNode32_g419;
				#endif
				float temp_output_299_3 = staticSwitch31_g419.b;
				float temp_output_26_0_g437 = temp_output_299_3;
				float temp_output_27_0_g437 = break273.x;
				float temp_output_1_0_g437 = ( max( ( temp_output_24_0_g437 + temp_output_25_0_g437 ) , ( temp_output_26_0_g437 + temp_output_27_0_g437 ) ) - 0.2 );
				float temp_output_21_0_g437 = max( ( ( temp_output_24_0_g437 + temp_output_25_0_g437 ) - temp_output_1_0_g437 ) , 0.0 );
				float4 tex2DNode14_g419 = tex2D( _Tint, temp_output_12_0_g419 );
				float temp_output_24_0_g422 = tex2DArrayNode35_g419.b;
				float temp_output_25_0_g422 = tex2DNode16_g419.a;
				float temp_output_26_0_g422 = tex2DArrayNode32_g419.b;
				float temp_output_27_0_g422 = temp_output_41_0_g419;
				float temp_output_1_0_g422 = ( max( ( temp_output_24_0_g422 + temp_output_25_0_g422 ) , ( temp_output_26_0_g422 + temp_output_27_0_g422 ) ) - 0.2 );
				float temp_output_21_0_g422 = max( ( ( temp_output_24_0_g422 + temp_output_25_0_g422 ) - temp_output_1_0_g422 ) , 0.0 );
				float temp_output_19_0_g422 = max( ( ( temp_output_26_0_g422 + temp_output_27_0_g422 ) - temp_output_1_0_g422 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch30_g419 = ( ( ( tex2DNode16_g419 * temp_output_21_0_g422 ) + ( tex2DNode14_g419 * temp_output_19_0_g422 ) ) / ( temp_output_21_0_g422 + temp_output_19_0_g422 ) );
				#else
				float4 staticSwitch30_g419 = tex2DNode14_g419;
				#endif
				float temp_output_19_0_g437 = max( ( ( temp_output_26_0_g437 + temp_output_27_0_g437 ) - temp_output_1_0_g437 ) , 0.0 );
				float temp_output_24_0_g436 = temp_output_302_3;
				float temp_output_25_0_g436 = temp_output_274_0;
				float temp_output_26_0_g436 = temp_output_299_3;
				float temp_output_27_0_g436 = break273.x;
				float temp_output_1_0_g436 = ( max( ( temp_output_24_0_g436 + temp_output_25_0_g436 ) , ( temp_output_26_0_g436 + temp_output_27_0_g436 ) ) - 0.2 );
				float temp_output_21_0_g436 = max( ( ( temp_output_24_0_g436 + temp_output_25_0_g436 ) - temp_output_1_0_g436 ) , 0.0 );
				float temp_output_19_0_g436 = max( ( ( temp_output_26_0_g436 + temp_output_27_0_g436 ) - temp_output_1_0_g436 ) , 0.0 );
				float4 temp_output_290_0 = ( ( ( staticSwitch31_g423 * temp_output_21_0_g436 ) + ( staticSwitch31_g419 * temp_output_19_0_g436 ) ) / ( temp_output_21_0_g436 + temp_output_19_0_g436 ) );
				float temp_output_24_0_g439 = temp_output_290_0.z;
				float temp_output_275_0 = ( 1.0 - break273.y );
				float temp_output_25_0_g439 = temp_output_275_0;
				float2 temp_output_12_0_g431 = ( ( temp_output_270_0 + float2( 0,1 ) ) / appendResult265 );
				float4 tex2DNode6_g431 = tex2D( _Control, temp_output_12_0_g431 );
				float4 tex2DArrayNode32_g431 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode6_g431.g * 255.0 ) );
				float4 tex2DNode15_g431 = tex2D( _GrassControl, temp_output_12_0_g431 );
				float4 tex2DArrayNode35_g431 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode15_g431.g * 255.0 ) );
				float temp_output_24_0_g432 = tex2DArrayNode35_g431.b;
				float4 tex2DNode16_g431 = tex2D( _GrassTint, temp_output_12_0_g431 );
				float temp_output_25_0_g432 = tex2DNode16_g431.a;
				float temp_output_26_0_g432 = tex2DArrayNode32_g431.b;
				float temp_output_41_0_g431 = ( 1.0 - tex2DNode16_g431.a );
				float temp_output_27_0_g432 = temp_output_41_0_g431;
				float temp_output_1_0_g432 = ( max( ( temp_output_24_0_g432 + temp_output_25_0_g432 ) , ( temp_output_26_0_g432 + temp_output_27_0_g432 ) ) - 0.2 );
				float temp_output_21_0_g432 = max( ( ( temp_output_24_0_g432 + temp_output_25_0_g432 ) - temp_output_1_0_g432 ) , 0.0 );
				float temp_output_19_0_g432 = max( ( ( temp_output_26_0_g432 + temp_output_27_0_g432 ) - temp_output_1_0_g432 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch31_g431 = ( ( ( tex2DArrayNode35_g431 * temp_output_21_0_g432 ) + ( tex2DArrayNode32_g431 * temp_output_19_0_g432 ) ) / ( temp_output_21_0_g432 + temp_output_19_0_g432 ) );
				#else
				float4 staticSwitch31_g431 = tex2DArrayNode32_g431;
				#endif
				float temp_output_300_3 = staticSwitch31_g431.b;
				float temp_output_24_0_g435 = temp_output_300_3;
				float temp_output_25_0_g435 = temp_output_274_0;
				float2 temp_output_12_0_g427 = ( ( temp_output_270_0 + float2( 1,1 ) ) / appendResult265 );
				float4 tex2DNode6_g427 = tex2D( _Control, temp_output_12_0_g427 );
				float4 tex2DArrayNode32_g427 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode6_g427.g * 255.0 ) );
				float4 tex2DNode15_g427 = tex2D( _GrassControl, temp_output_12_0_g427 );
				float4 tex2DArrayNode35_g427 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode15_g427.g * 255.0 ) );
				float temp_output_24_0_g428 = tex2DArrayNode35_g427.b;
				float4 tex2DNode16_g427 = tex2D( _GrassTint, temp_output_12_0_g427 );
				float temp_output_25_0_g428 = tex2DNode16_g427.a;
				float temp_output_26_0_g428 = tex2DArrayNode32_g427.b;
				float temp_output_41_0_g427 = ( 1.0 - tex2DNode16_g427.a );
				float temp_output_27_0_g428 = temp_output_41_0_g427;
				float temp_output_1_0_g428 = ( max( ( temp_output_24_0_g428 + temp_output_25_0_g428 ) , ( temp_output_26_0_g428 + temp_output_27_0_g428 ) ) - 0.2 );
				float temp_output_21_0_g428 = max( ( ( temp_output_24_0_g428 + temp_output_25_0_g428 ) - temp_output_1_0_g428 ) , 0.0 );
				float temp_output_19_0_g428 = max( ( ( temp_output_26_0_g428 + temp_output_27_0_g428 ) - temp_output_1_0_g428 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch31_g427 = ( ( ( tex2DArrayNode35_g427 * temp_output_21_0_g428 ) + ( tex2DArrayNode32_g427 * temp_output_19_0_g428 ) ) / ( temp_output_21_0_g428 + temp_output_19_0_g428 ) );
				#else
				float4 staticSwitch31_g427 = tex2DArrayNode32_g427;
				#endif
				float temp_output_301_3 = staticSwitch31_g427.b;
				float temp_output_26_0_g435 = temp_output_301_3;
				float temp_output_27_0_g435 = break273.x;
				float temp_output_1_0_g435 = ( max( ( temp_output_24_0_g435 + temp_output_25_0_g435 ) , ( temp_output_26_0_g435 + temp_output_27_0_g435 ) ) - 0.2 );
				float temp_output_21_0_g435 = max( ( ( temp_output_24_0_g435 + temp_output_25_0_g435 ) - temp_output_1_0_g435 ) , 0.0 );
				float temp_output_19_0_g435 = max( ( ( temp_output_26_0_g435 + temp_output_27_0_g435 ) - temp_output_1_0_g435 ) , 0.0 );
				float4 temp_output_293_0 = ( ( ( staticSwitch31_g431 * temp_output_21_0_g435 ) + ( staticSwitch31_g427 * temp_output_19_0_g435 ) ) / ( temp_output_21_0_g435 + temp_output_19_0_g435 ) );
				float temp_output_26_0_g439 = temp_output_293_0.z;
				float temp_output_27_0_g439 = break273.y;
				float temp_output_1_0_g439 = ( max( ( temp_output_24_0_g439 + temp_output_25_0_g439 ) , ( temp_output_26_0_g439 + temp_output_27_0_g439 ) ) - 0.2 );
				float temp_output_21_0_g439 = max( ( ( temp_output_24_0_g439 + temp_output_25_0_g439 ) - temp_output_1_0_g439 ) , 0.0 );
				float4 tex2DNode14_g431 = tex2D( _Tint, temp_output_12_0_g431 );
				float temp_output_24_0_g434 = tex2DArrayNode35_g431.b;
				float temp_output_25_0_g434 = tex2DNode16_g431.a;
				float temp_output_26_0_g434 = tex2DArrayNode32_g431.b;
				float temp_output_27_0_g434 = temp_output_41_0_g431;
				float temp_output_1_0_g434 = ( max( ( temp_output_24_0_g434 + temp_output_25_0_g434 ) , ( temp_output_26_0_g434 + temp_output_27_0_g434 ) ) - 0.2 );
				float temp_output_21_0_g434 = max( ( ( temp_output_24_0_g434 + temp_output_25_0_g434 ) - temp_output_1_0_g434 ) , 0.0 );
				float temp_output_19_0_g434 = max( ( ( temp_output_26_0_g434 + temp_output_27_0_g434 ) - temp_output_1_0_g434 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch30_g431 = ( ( ( tex2DNode16_g431 * temp_output_21_0_g434 ) + ( tex2DNode14_g431 * temp_output_19_0_g434 ) ) / ( temp_output_21_0_g434 + temp_output_19_0_g434 ) );
				#else
				float4 staticSwitch30_g431 = tex2DNode14_g431;
				#endif
				float temp_output_24_0_g438 = temp_output_300_3;
				float temp_output_25_0_g438 = temp_output_274_0;
				float temp_output_26_0_g438 = temp_output_301_3;
				float temp_output_27_0_g438 = break273.x;
				float temp_output_1_0_g438 = ( max( ( temp_output_24_0_g438 + temp_output_25_0_g438 ) , ( temp_output_26_0_g438 + temp_output_27_0_g438 ) ) - 0.2 );
				float temp_output_21_0_g438 = max( ( ( temp_output_24_0_g438 + temp_output_25_0_g438 ) - temp_output_1_0_g438 ) , 0.0 );
				float4 tex2DNode14_g427 = tex2D( _Tint, temp_output_12_0_g427 );
				float temp_output_24_0_g430 = tex2DArrayNode35_g427.b;
				float temp_output_25_0_g430 = tex2DNode16_g427.a;
				float temp_output_26_0_g430 = tex2DArrayNode32_g427.b;
				float temp_output_27_0_g430 = temp_output_41_0_g427;
				float temp_output_1_0_g430 = ( max( ( temp_output_24_0_g430 + temp_output_25_0_g430 ) , ( temp_output_26_0_g430 + temp_output_27_0_g430 ) ) - 0.2 );
				float temp_output_21_0_g430 = max( ( ( temp_output_24_0_g430 + temp_output_25_0_g430 ) - temp_output_1_0_g430 ) , 0.0 );
				float temp_output_19_0_g430 = max( ( ( temp_output_26_0_g430 + temp_output_27_0_g430 ) - temp_output_1_0_g430 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch30_g427 = ( ( ( tex2DNode16_g427 * temp_output_21_0_g430 ) + ( tex2DNode14_g427 * temp_output_19_0_g430 ) ) / ( temp_output_21_0_g430 + temp_output_19_0_g430 ) );
				#else
				float4 staticSwitch30_g427 = tex2DNode14_g427;
				#endif
				float temp_output_19_0_g438 = max( ( ( temp_output_26_0_g438 + temp_output_27_0_g438 ) - temp_output_1_0_g438 ) , 0.0 );
				float temp_output_19_0_g439 = max( ( ( temp_output_26_0_g439 + temp_output_27_0_g439 ) - temp_output_1_0_g439 ) , 0.0 );
				float4 temp_output_221_0 = ( ( ( ( ( ( staticSwitch30_g423 * temp_output_21_0_g437 ) + ( staticSwitch30_g419 * temp_output_19_0_g437 ) ) / ( temp_output_21_0_g437 + temp_output_19_0_g437 ) ) * temp_output_21_0_g439 ) + ( ( ( ( staticSwitch30_g431 * temp_output_21_0_g438 ) + ( staticSwitch30_g427 * temp_output_19_0_g438 ) ) / ( temp_output_21_0_g438 + temp_output_19_0_g438 ) ) * temp_output_19_0_g439 ) ) / ( temp_output_21_0_g439 + temp_output_19_0_g439 ) );
				float temp_output_3_0_g440 = ( temp_output_221_0.w * 2.0 );
				
				surfaceDescription.Alpha = min( temp_output_3_0_g440 , 1.0 );
				surfaceDescription.AlphaClipThreshold =  _AlphaCutoff;
				

				float3 V = float3(1.0, 1.0, 1.0); 
			
				SurfaceData surfaceData;
				BuiltinData builtinData;
				GetSurfaceAndBuiltinData(surfaceDescription, input, V, posInput, surfaceData, builtinData);
				outColor = _SelectionID;
			}

            ENDHLSL
        }

        Pass
        {
            Name "FullScreenDebug"
            Tags
            {
                "LightMode" = "FullScreenDebug"
            }
            
            Cull [_CullMode]
			ZTest LEqual
			ZWrite Off
        
            HLSLPROGRAM
        
			/*ase_pragma_before*/
        
			#pragma vertex Vert
			#pragma fragment Frag
			
            #pragma shader_feature _ _SURFACE_TYPE_TRANSPARENT
			#pragma shader_feature_local _BLENDMODE_OFF _BLENDMODE_ALPHA _BLENDMODE_ADD _BLENDMODE_PRE_MULTIPLY
			#pragma shader_feature_local _ _DOUBLESIDED_ON
			#pragma shader_feature_local _ _TRANSPARENT_WRITES_MOTION_VEC
			#pragma shader_feature_local_fragment _ _ENABLE_FOG_ON_TRANSPARENT
            
        
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
        	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/GeometricTools.hlsl" 
        	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Tessellation.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/FragInputs.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl" 
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderGraphHeader.hlsl" 
        
            
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
        
            #define SHADERPASS SHADERPASS_FULL_SCREEN_DEBUG
        
			#define _SPECULAR_OCCLUSION_FROM_AO 1
			#define _ENERGY_CONSERVING_SPECULAR 1
        
			#if defined(_MATERIAL_FEATURE_SUBSURFACE_SCATTERING) && !defined(_SURFACE_TYPE_TRANSPARENT)
				#define OUTPUT_SPLIT_LIGHTING
			#endif
        
        
			#define HAVE_RECURSIVE_RENDERING

            #if SHADERPASS == SHADERPASS_TRANSPARENT_DEPTH_PREPASS
            #if !defined(_DISABLE_SSR_TRANSPARENT) && !defined(SHADER_UNLIT)
                #define WRITE_NORMAL_BUFFER
            #endif
            #endif
        
            #ifndef DEBUG_DISPLAY
                
                #if !defined(_SURFACE_TYPE_TRANSPARENT)
                    #if SHADERPASS == SHADERPASS_FORWARD
                    #define SHADERPASS_FORWARD_BYPASS_ALPHA_TEST
                    #elif SHADERPASS == SHADERPASS_GBUFFER
                    #define SHADERPASS_GBUFFER_BYPASS_ALPHA_TEST
                    #endif
                #endif
            #endif
        
            
            #if defined(SHADER_LIT) && !defined(_SURFACE_TYPE_TRANSPARENT)
                #define _DEFERRED_CAPABLE_MATERIAL
            #endif
        
            
            #if defined(_TRANSPARENT_WRITES_MOTION_VEC) && defined(_SURFACE_TYPE_TRANSPARENT)
                #define _WRITE_TRANSPARENT_MOTION_VECTOR
            #endif
        
            
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Debug/DebugDisplay.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/NormalSurfaceGradient.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Lit/Lit.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/BuiltinUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/MaterialUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderGraphFunctions.hlsl"
        	
        
            struct AttributesMesh
			{
				 float3 positionOS : POSITION;
				 float3 normalOS : NORMAL;
				 float4 tangentOS : TANGENT;
				#if UNITY_ANY_INSTANCING_ENABLED
				 uint instanceID : INSTANCEID_SEMANTIC;
				#endif
			};

			struct VaryingsMeshToPS
			{
				SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
				#if UNITY_ANY_INSTANCING_ENABLED
				 uint instanceID : CUSTOM_INSTANCE_ID;
				#endif
			};

			struct VertexDescriptionInputs
			{
				 float3 ObjectSpaceNormal;
				 float3 ObjectSpaceTangent;
				 float3 ObjectSpacePosition;
			};

			struct SurfaceDescriptionInputs
			{
				 float3 TangentSpaceNormal;
			};

			struct PackedVaryingsMeshToPS
			{
				SV_POSITION_QUALIFIERS float4 positionCS : SV_POSITION;
				#if UNITY_ANY_INSTANCING_ENABLED
				 uint instanceID : CUSTOM_INSTANCE_ID;
				#endif
			};
        
            PackedVaryingsMeshToPS PackVaryingsMeshToPS (VaryingsMeshToPS input)
			{
				PackedVaryingsMeshToPS output;
				ZERO_INITIALIZE(PackedVaryingsMeshToPS, output);
				output.positionCS = input.positionCS;
				#if UNITY_ANY_INSTANCING_ENABLED
				output.instanceID = input.instanceID;
				#endif
				return output;
			}
        
			VaryingsMeshToPS UnpackVaryingsMeshToPS (PackedVaryingsMeshToPS input)
			{
				VaryingsMeshToPS output;
				output.positionCS = input.positionCS;
				#if UNITY_ANY_INSTANCING_ENABLED
				output.instanceID = input.instanceID;
				#endif
				return output;
			}
        
            struct VertexDescription
			{
				float3 Position;
				float3 Normal;
				float3 Tangent;
			};
        
			VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
			{
				VertexDescription description = (VertexDescription)0;
				description.Position = IN.ObjectSpacePosition;
				description.Normal = IN.ObjectSpaceNormal;
				description.Tangent = IN.ObjectSpaceTangent;
				return description;
			}
        
            struct SurfaceDescription
			{
				float3 BaseColor;
				float3 Emission;
				float Alpha;
				float3 BentNormal;
				float Smoothness;
				float Occlusion;
				float3 NormalTS;
				float Metallic;
			};
        
			SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
			{
				SurfaceDescription surface = (SurfaceDescription)0;
				surface.BaseColor = IsGammaSpace() ? float3(0.5, 0.5, 0.5) : SRGBToLinear(float3(0.5, 0.5, 0.5));
				surface.Emission = float3(0, 0, 0);
				surface.Alpha = 1;
				surface.BentNormal = IN.TangentSpaceNormal;
				surface.Smoothness = 0.5;
				surface.Occlusion = 1;
				surface.NormalTS = IN.TangentSpaceNormal;
				surface.Metallic = 0;
				return surface;
			}

			VertexDescriptionInputs AttributesMeshToVertexDescriptionInputs(AttributesMesh input)
			{
				VertexDescriptionInputs output;
				ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
				output.ObjectSpaceNormal =                          input.normalOS;
				output.ObjectSpaceTangent =                         input.tangentOS.xyz;
				output.ObjectSpacePosition =                        input.positionOS;
        
				return output;
			}
        
			AttributesMesh ApplyMeshModification(AttributesMesh input, float3 timeParameters  )
			{
				VertexDescriptionInputs vertexDescriptionInputs = AttributesMeshToVertexDescriptionInputs(input);
           
				VertexDescription vertexDescription = VertexDescriptionFunction(vertexDescriptionInputs);

				input.positionOS = vertexDescription.Position;
				input.normalOS = vertexDescription.Normal;
				input.tangentOS.xyz = vertexDescription.Tangent;
				return input;
			}
        
			FragInputs BuildFragInputs(VaryingsMeshToPS input)
			{
				FragInputs output;
				ZERO_INITIALIZE(FragInputs, output);
        
				output.tangentToWorld = k_identity3x3;
				output.positionSS = input.positionCS;       
        
				return output;
			}
        
        
			FragInputs UnpackVaryingsMeshToFragInputs(PackedVaryingsMeshToPS input)
			{
				UNITY_SETUP_INSTANCE_ID(input);
				VaryingsMeshToPS unpacked = UnpackVaryingsMeshToPS(input);
				return BuildFragInputs(unpacked);
			}


            SurfaceDescriptionInputs FragInputsToSurfaceDescriptionInputs(FragInputs input, float3 viewWS)
			{
				SurfaceDescriptionInputs output;
				ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
				#if defined(SHADER_STAGE_RAY_TRACING)
				#else
				#endif
				output.TangentSpaceNormal =                         float3(0.0f, 0.0f, 1.0f);        
				return output;
			}
        
			void BuildSurfaceData(FragInputs fragInputs, inout SurfaceDescription surfaceDescription, float3 V, PositionInputs posInput, out SurfaceData surfaceData, out float3 bentNormalWS)
			{
				ZERO_INITIALIZE(SurfaceData, surfaceData);
        
				surfaceData.specularOcclusion = 1.0;
        
				surfaceData.baseColor =                 surfaceDescription.BaseColor;
				surfaceData.perceptualSmoothness =      surfaceDescription.Smoothness;
				surfaceData.ambientOcclusion =          surfaceDescription.Occlusion;
				surfaceData.metallic =                  surfaceDescription.Metallic;
        
				#if defined(_REFRACTION_PLANE) || defined(_REFRACTION_SPHERE) || defined(_REFRACTION_THIN)
					if (_EnableSSRefraction)
					{
        
						surfaceData.transmittanceMask = (1.0 - surfaceDescription.Alpha);
						surfaceDescription.Alpha = 1.0;
					}
					else
					{
						surfaceData.ior = 1.0;
						surfaceData.transmittanceColor = float3(1.0, 1.0, 1.0);
						surfaceData.atDistance = 1.0;
						surfaceData.transmittanceMask = 0.0;
						surfaceDescription.Alpha = 1.0;
					}
				#else
					surfaceData.ior = 1.0;
					surfaceData.transmittanceColor = float3(1.0, 1.0, 1.0);
					surfaceData.atDistance = 1.0;
					surfaceData.transmittanceMask = 0.0;
				#endif
        
            
				surfaceData.materialFeatures = MATERIALFEATUREFLAGS_LIT_STANDARD;
				#ifdef _MATERIAL_FEATURE_SUBSURFACE_SCATTERING
					surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_SUBSURFACE_SCATTERING;
				#endif
        
				#ifdef _MATERIAL_FEATURE_TRANSMISSION
					surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_TRANSMISSION;
				#endif
        
				#ifdef _MATERIAL_FEATURE_ANISOTROPY
					surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_ANISOTROPY;
        
                
					surfaceData.normalWS = float3(0, 1, 0);
				#endif
        
				#ifdef _MATERIAL_FEATURE_IRIDESCENCE
					surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_IRIDESCENCE;
				#endif
        
				#ifdef _MATERIAL_FEATURE_SPECULAR_COLOR
					surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_SPECULAR_COLOR;
				#endif
        
				#ifdef _MATERIAL_FEATURE_CLEAR_COAT
					surfaceData.materialFeatures |= MATERIALFEATUREFLAGS_LIT_CLEAR_COAT;
				#endif
        
				#if defined (_MATERIAL_FEATURE_SPECULAR_COLOR) && defined (_ENERGY_CONSERVING_SPECULAR)
                
                
					surfaceData.baseColor *= (1.0 - Max3(surfaceData.specularColor.r, surfaceData.specularColor.g, surfaceData.specularColor.b));
				#endif
        
				#ifdef _DOUBLESIDED_ON
					float3 doubleSidedConstants = _DoubleSidedConstants.xyz;
				#else
					float3 doubleSidedConstants = float3(1.0, 1.0, 1.0);
				#endif
        
            
				GetNormalWS(fragInputs, surfaceDescription.NormalTS, surfaceData.normalWS, doubleSidedConstants);
        
				surfaceData.geomNormalWS = fragInputs.tangentToWorld[2];
        
				surfaceData.tangentWS = normalize(fragInputs.tangentToWorld[0].xyz);    
        
        
				#if HAVE_DECALS
					if (_EnableDecals)
					{
						float alpha = 1.0;
						alpha = surfaceDescription.Alpha;
						DecalSurfaceData decalSurfaceData = GetDecalSurfaceData(posInput, fragInputs, alpha);
						ApplyDecalToSurfaceData(decalSurfaceData, fragInputs.tangentToWorld[2], surfaceData);
					}
				#endif
        
				bentNormalWS = surfaceData.normalWS;
        
				surfaceData.tangentWS = Orthonormalize(surfaceData.tangentWS, surfaceData.normalWS);
        
				#ifdef DEBUG_DISPLAY
					if (_DebugMipMapMode != DEBUGMIPMAPMODE_NONE)
					{
                    
						surfaceData.metallic = 0;
					}
        
					ApplyDebugToSurfaceData(fragInputs.tangentToWorld, surfaceData);
				#endif
            
				#if defined(_SPECULAR_OCCLUSION_CUSTOM)
                
				#elif defined(_SPECULAR_OCCLUSION_FROM_AO_BENT_NORMAL)
                
					surfaceData.specularOcclusion = GetSpecularOcclusionFromBentAO(V, bentNormalWS, surfaceData.normalWS, surfaceData.ambientOcclusion, PerceptualSmoothnessToPerceptualRoughness(surfaceData.perceptualSmoothness));
				#elif defined(_AMBIENT_OCCLUSION) && defined(_SPECULAR_OCCLUSION_FROM_AO)
					surfaceData.specularOcclusion = GetSpecularOcclusionFromAmbientOcclusion(ClampNdotV(dot(surfaceData.normalWS, V)), surfaceData.ambientOcclusion, PerceptualSmoothnessToRoughness(surfaceData.perceptualSmoothness));
				#endif
        
				#if defined(_ENABLE_GEOMETRIC_SPECULAR_AA) && !defined(SHADER_STAGE_RAY_TRACING)
					surfaceData.perceptualSmoothness = GeometricNormalFiltering(surfaceData.perceptualSmoothness, fragInputs.tangentToWorld[2], surfaceDescription.SpecularAAScreenSpaceVariance, surfaceDescription.SpecularAAThreshold);
				#endif
			}
        
            void GetSurfaceAndBuiltinData(FragInputs fragInputs, float3 V, inout PositionInputs posInput, out SurfaceData surfaceData, out BuiltinData builtinData RAY_TRACING_OPTIONAL_PARAMETERS)
            {
                
                #if !defined(SHADER_STAGE_RAY_TRACING) && !defined(_TESSELLATION_DISPLACEMENT)
                #ifdef LOD_FADE_CROSSFADE 
                LODDitheringTransition(ComputeFadeMaskSeed(V, posInput.positionSS), unity_LODFade.x);
                #endif
                #endif
        
                #ifndef SHADER_UNLIT
                #ifdef _DOUBLESIDED_ON
                    float3 doubleSidedConstants = _DoubleSidedConstants.xyz;
                #else
                    float3 doubleSidedConstants = float3(1.0, 1.0, 1.0);
                #endif
        
                ApplyDoubleSidedFlipOrMirror(fragInputs, doubleSidedConstants); 
                #endif 
        
                SurfaceDescriptionInputs surfaceDescriptionInputs = FragInputsToSurfaceDescriptionInputs(fragInputs, V);
        
                SurfaceDescription surfaceDescription = SurfaceDescriptionFunction(surfaceDescriptionInputs);

                #ifdef _ALPHATEST_ON
                    float alphaCutoff = surfaceDescription.AlphaClipThreshold;
                    #if SHADERPASS == SHADERPASS_TRANSPARENT_DEPTH_PREPASS
                    
                    #elif SHADERPASS == SHADERPASS_TRANSPARENT_DEPTH_POSTPASS
                    
                    alphaCutoff = surfaceDescription.AlphaClipThresholdDepthPostpass;
                    #elif (SHADERPASS == SHADERPASS_SHADOWS) || (SHADERPASS == SHADERPASS_RAYTRACING_VISIBILITY)
                    
                    #endif
        
                    GENERIC_ALPHA_TEST(surfaceDescription.Alpha, alphaCutoff);
                #endif
        
                #if !defined(SHADER_STAGE_RAY_TRACING) && _DEPTHOFFSET_ON
                ApplyDepthOffsetPositionInput(V, surfaceDescription.DepthOffset, GetViewForwardDir(), GetWorldToHClipMatrix(), posInput);
                #endif
        
                #ifndef SHADER_UNLIT
                float3 bentNormalWS;
                BuildSurfaceData(fragInputs, surfaceDescription, V, posInput, surfaceData, bentNormalWS);
        
                InitBuiltinData(posInput, surfaceDescription.Alpha, bentNormalWS, -fragInputs.tangentToWorld[2], fragInputs.texCoord1, fragInputs.texCoord2, builtinData);
        
                #else
                BuildSurfaceData(fragInputs, surfaceDescription, V, posInput, surfaceData);
        
                ZERO_BUILTIN_INITIALIZE(builtinData); 
                builtinData.opacity = surfaceDescription.Alpha;
        
                #if defined(DEBUG_DISPLAY)
                    builtinData.renderingLayers = GetMeshRenderingLightLayer();
                #endif
        
                #endif 
        
                #ifdef _ALPHATEST_ON
                    
                    builtinData.alphaClipTreshold = alphaCutoff;
                #endif
        
        
                builtinData.emissiveColor = surfaceDescription.Emission;

                #if _DEPTHOFFSET_ON
                builtinData.depthOffset = surfaceDescription.DepthOffset;
                #endif
        
                
                #if (SHADERPASS == SHADERPASS_DISTORTION)
                builtinData.distortion = surfaceDescription.Distortion;
                builtinData.distortionBlur = surfaceDescription.DistortionBlur;
                #endif
        
                #ifndef SHADER_UNLIT
                
                PostInitBuiltinData(V, posInput, surfaceData, builtinData);
                #else
                ApplyDebugToBuiltinData(builtinData);
                #endif
        
            }
        
			#define DEBUG_DISPLAY
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Debug/DebugDisplay.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Debug/FullScreenDebug.hlsl"

			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/VertMesh.hlsl"

			PackedVaryingsType Vert(AttributesMesh inputMesh)
			{
				VaryingsType varyingsType;
				varyingsType.vmesh = VertMesh(inputMesh);
				return PackVaryingsType(varyingsType);
			}



			#if !defined(_DEPTHOFFSET_ON)
			[earlydepthstencil] 
			#endif
			void Frag(PackedVaryingsToPS packedInput)
			{
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(packedInput);
				FragInputs input = UnpackVaryingsToFragInputs(packedInput);

				PositionInputs posInput = GetPositionInput(input.positionSS.xy, _ScreenSize.zw, input.positionSS.z, input.positionSS.w, input.positionRWS.xyz);

			#ifdef PLATFORM_SUPPORTS_PRIMITIVE_ID_IN_PIXEL_SHADER
				if (_DebugFullScreenMode == FULLSCREENDEBUGMODE_QUAD_OVERDRAW)
				{
					IncrementQuadOverdrawCounter(posInput.positionSS.xy, input.primitiveID);
				}
			#endif
			}

            ENDHLSL
        }
		
	}
	
	CustomEditor "Rendering.HighDefinition.LightingShaderGraphGUI"
	
	
}
/*ASEBEGIN
Version=18935
157;171;1920;1011;1745.212;2972.563;2.2;True;True
Node;AmplifyShaderEditor.CommentaryNode;253;-1403.167,-1201.167;Inherit;False;613;372;World UV;8;264;262;261;260;259;257;254;296;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;256;-1449.881,-1473.772;Inherit;False;703.2;252.2002;ControlCords;4;266;265;263;258;;1,1,1,1;0;0
Node;AmplifyShaderEditor.Vector4Node;254;-1395.301,-1002.302;Inherit;False;Property;_WorldBounds;WorldBounds;6;0;Create;True;0;0;0;False;0;False;0,0,1,1;0,0,1,1;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldPosInputsNode;296;-1392.025,-1161.401;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DynamicAppendNode;259;-1228.167,-927.1675;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;257;-1223.167,-1159.167;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;260;-1229.167,-1024.168;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;258;-1428.77,-1433.443;Inherit;True;Property;_Control;Control;2;0;Create;True;0;0;0;True;0;False;None;None;False;black;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SimpleSubtractOpNode;261;-1077.167,-972.1676;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;262;-1065.168,-1106.167;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexelSizeNode;263;-1213.22,-1431.339;Inherit;False;14;1;0;SAMPLER2D;;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleDivideOpNode;264;-919.8682,-1052.568;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;265;-1023.63,-1412.748;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;266;-886.2813,-1434.772;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;268;-685.5034,-1490.408;Inherit;False;415.7999;163.6;ControlCordsBase;3;270;269;267;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;267;-664.0034,-1444.291;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;269;-546.6046,-1445.291;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FloorOpNode;270;-400.3875,-1435.931;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;271;-228.8044,-1488.192;Inherit;False;277.1;155.1;ControlFraction;2;273;272;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;272;-220.9044,-1441.792;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;282;-424.5107,-1720.892;Inherit;False;Constant;_Offset;Offset;13;0;Create;True;0;0;0;False;0;False;1,1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;276;-424.5107,-1984.892;Inherit;False;Constant;_Vector2;Vector 2;13;0;Create;True;0;0;0;False;0;False;0,1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TexturePropertyNode;279;-1129.757,-1997.791;Inherit;True;Property;_GrassControl;GrassControl;3;0;Create;True;0;0;0;True;0;False;None;None;False;black;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.TexturePropertyNode;283;-1129.04,-1802.328;Inherit;True;Property;_Tint;Tint;0;0;Create;False;0;0;0;True;0;False;None;None;False;black;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.TexturePropertyNode;277;-1129.041,-2570.937;Inherit;True;Property;_MatTexArray;MatTexArray;7;0;Create;True;0;0;0;False;0;False;None;None;False;white;LockedToTexture2DArray;Texture2DArray;-1;0;2;SAMPLER2DARRAY;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.BreakToComponentsNode;273;-81.64874,-1440.814;Inherit;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.Vector2Node;280;-433.5106,-2589.893;Inherit;False;Constant;_Vector0;Vector 0;13;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TexturePropertyNode;284;-1123.306,-2185.024;Inherit;True;Property;_GrassTint;GrassTint;1;0;Create;True;0;0;0;True;0;False;None;None;False;black;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.Vector2Node;281;-433.5106,-2280.893;Inherit;False;Constant;_Vector1;Vector 1;13;0;Create;True;0;0;0;False;0;False;1,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TexturePropertyNode;278;-1126.232,-2378.53;Inherit;True;Property;_ShapeMap;Shape Texture Splat;8;1;[Normal];Create;False;0;0;0;False;0;False;9eea02fcbbb4cc24aaf9072337d93cd4;9eea02fcbbb4cc24aaf9072337d93cd4;False;bump;LockedToTexture2DArray;Texture2DArray;-1;0;2;SAMPLER2DARRAY;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.OneMinusNode;274;86.84914,-1461.881;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;299;-158.8227,-2360.228;Inherit;False;Sample Terrain Tile;-1;;419;68db4cbbb16712044b9cfe83a0a98414;0;9;5;SAMPLER2D;0;False;7;FLOAT2;0,0;False;8;FLOAT2;0,0;False;9;FLOAT2;0,0;False;10;SAMPLER2D;0;False;13;SAMPLER2D;0;False;17;SAMPLER2D;0;False;22;SAMPLER2DARRAY;0;False;33;SAMPLER2DARRAY;0;False;4;COLOR;0;COLOR;1;COLOR;2;FLOAT;3
Node;AmplifyShaderEditor.FunctionNode;302;-159.4047,-2633.547;Inherit;False;Sample Terrain Tile;-1;;423;68db4cbbb16712044b9cfe83a0a98414;0;9;5;SAMPLER2D;0;False;7;FLOAT2;0,0;False;8;FLOAT2;0,0;False;9;FLOAT2;0,0;False;10;SAMPLER2D;0;False;13;SAMPLER2D;0;False;17;SAMPLER2D;0;False;22;SAMPLER2DARRAY;0;False;33;SAMPLER2DARRAY;0;False;4;COLOR;0;COLOR;1;COLOR;2;FLOAT;3
Node;AmplifyShaderEditor.FunctionNode;301;-164.9707,-1808.643;Inherit;False;Sample Terrain Tile;-1;;427;68db4cbbb16712044b9cfe83a0a98414;0;9;5;SAMPLER2D;0;False;7;FLOAT2;0,0;False;8;FLOAT2;0,0;False;9;FLOAT2;0,0;False;10;SAMPLER2D;0;False;13;SAMPLER2D;0;False;17;SAMPLER2D;0;False;22;SAMPLER2DARRAY;0;False;33;SAMPLER2DARRAY;0;False;4;COLOR;0;COLOR;1;COLOR;2;FLOAT;3
Node;AmplifyShaderEditor.FunctionNode;300;-165.5717,-2085.146;Inherit;False;Sample Terrain Tile;-1;;431;68db4cbbb16712044b9cfe83a0a98414;0;9;5;SAMPLER2D;0;False;7;FLOAT2;0,0;False;8;FLOAT2;0,0;False;9;FLOAT2;0,0;False;10;SAMPLER2D;0;False;13;SAMPLER2D;0;False;17;SAMPLER2D;0;False;22;SAMPLER2DARRAY;0;False;33;SAMPLER2DARRAY;0;False;4;COLOR;0;COLOR;1;COLOR;2;FLOAT;3
Node;AmplifyShaderEditor.FunctionNode;290;350.3746,-2479.781;Inherit;False;MixColor;-1;;436;c73e8a542d6aa7b41a97cfb308620336;0;6;29;FLOAT4;0,0,0,0;False;24;FLOAT;0;False;25;FLOAT;0;False;30;FLOAT4;0,0,0,0;False;26;FLOAT;0;False;27;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.FunctionNode;293;351.6413,-1903.392;Inherit;False;MixColor;-1;;435;c73e8a542d6aa7b41a97cfb308620336;0;6;29;FLOAT4;0,0,0,0;False;24;FLOAT;0;False;25;FLOAT;0;False;30;FLOAT4;0,0,0,0;False;26;FLOAT;0;False;27;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.FunctionNode;291;351.6413,-2284.695;Inherit;False;MixColor;-1;;437;c73e8a542d6aa7b41a97cfb308620336;0;6;29;FLOAT4;0,0,0,0;False;24;FLOAT;0;False;25;FLOAT;0;False;30;FLOAT4;0,0,0,0;False;26;FLOAT;0;False;27;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.OneMinusNode;275;100.3539,-1382.677;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;294;352.9081,-1708.305;Inherit;False;MixColor;-1;;438;c73e8a542d6aa7b41a97cfb308620336;0;6;29;FLOAT4;0,0,0,0;False;24;FLOAT;0;False;25;FLOAT;0;False;30;FLOAT4;0,0,0,0;False;26;FLOAT;0;False;27;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.BreakToComponentsNode;223;703.6003,-2427.254;Inherit;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.BreakToComponentsNode;224;700.3339,-2252.92;Inherit;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.FunctionNode;221;986.623,-1917.384;Inherit;False;MixColor;-1;;439;c73e8a542d6aa7b41a97cfb308620336;0;6;29;FLOAT4;0,0,0,0;False;24;FLOAT;0;False;25;FLOAT;0;False;30;FLOAT4;0,0,0,0;False;26;FLOAT;0;False;27;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.BreakToComponentsNode;317;1236.73,-1876.341;Inherit;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.RangedFloatNode;313;1284.593,-1682.789;Inherit;False;Property;_NormalScale;Normal Scale;9;0;Create;True;0;0;0;False;0;False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;21;1290.476,-1512.45;Inherit;False;Property;_Rough;Roughness;5;0;Create;False;0;0;0;False;0;False;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;292;350.3744,-2094.677;Inherit;False;MixColor;-1;;488;c73e8a542d6aa7b41a97cfb308620336;0;6;29;FLOAT4;0,0,0,0;False;24;FLOAT;0;False;25;FLOAT;0;False;30;FLOAT4;0,0,0,0;False;26;FLOAT;0;False;27;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.FunctionNode;384;1721.864,-2351.74;Inherit;False;Add Spatter;-1;;487;7317899d8f248394e8b7e544d65b8cf2;0;4;31;FLOAT2;0,0;False;35;FLOAT4;0,0,0,0;False;6;FLOAT4;0,0,1,1;False;14;SAMPLER2D;0;False;3;COLOR;0;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.FunctionNode;289;349.1077,-2671.066;Inherit;False;MixColor;-1;;443;c73e8a542d6aa7b41a97cfb308620336;0;6;29;FLOAT4;0,0,0,0;False;24;FLOAT;0;False;25;FLOAT;0;False;30;FLOAT4;0,0,0,0;False;26;FLOAT;0;False;27;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.FunctionNode;159;984.686,-2335.74;Inherit;False;MixColor;-1;;442;c73e8a542d6aa7b41a97cfb308620336;0;6;29;FLOAT4;0,0,0,0;False;24;FLOAT;0;False;25;FLOAT;0;False;30;FLOAT4;0,0,0,0;False;26;FLOAT;0;False;27;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;161;1453.544,-2196.66;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;1,1,1,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.BreakToComponentsNode;298;1250.246,-2045.181;Inherit;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;318;1766.13,-1835.441;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;180;989.8365,-2135.369;Inherit;False;MixColor;-1;;441;c73e8a542d6aa7b41a97cfb308620336;0;6;29;FLOAT4;0,0,0,0;False;24;FLOAT;0;False;25;FLOAT;0;False;30;FLOAT4;0,0,0,0;False;26;FLOAT;0;False;27;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.FunctionNode;316;1448.329,-1841.741;Inherit;False;Alpha Split;-1;;440;15cd6f3f954a80543b41ed67ca95494c;0;1;2;FLOAT;0;False;2;FLOAT;0;FLOAT;1
Node;AmplifyShaderEditor.RangedFloatNode;20;1278.981,-1605.207;Inherit;False;Property;_MetalLevel;Metallic;4;0;Create;False;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;315;1162.629,-2682.341;Inherit;True;Property;_SpatterTex;SpatterTex;10;0;Create;True;0;0;0;False;0;False;None;None;False;black;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.UnpackScaleNormalNode;220;1371.664,-2094.727;Inherit;False;Tangent;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;2;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.BreakToComponentsNode;319;1269.229,-2437.94;Inherit;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;307;1781.773,-1728.569;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;241;1794.491,-1976.747;Float;False;False;-1;2;Rendering.HighDefinition.LightingShaderGraphGUI;0;1;New Amplify Shader;53b46d85872c5b24c8f4f0a1c3fe4c87;True;META;0;1;META;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=HDRenderPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;5;True;7;d3d11;metal;vulkan;xboxone;xboxseries;playstation;switch;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Meta;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;243;1794.491,-1976.747;Float;False;False;-1;2;Rendering.HighDefinition.LightingShaderGraphGUI;0;1;New Amplify Shader;53b46d85872c5b24c8f4f0a1c3fe4c87;True;SceneSelectionPass;0;3;SceneSelectionPass;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=HDRenderPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;5;True;7;d3d11;metal;vulkan;xboxone;xboxseries;playstation;switch;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=SceneSelectionPass;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;248;1794.491,-1976.747;Float;False;False;-1;2;Rendering.HighDefinition.LightingShaderGraphGUI;0;1;New Amplify Shader;53b46d85872c5b24c8f4f0a1c3fe4c87;True;TransparentDepthPrepass;0;8;TransparentDepthPrepass;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=HDRenderPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;5;True;7;d3d11;metal;vulkan;xboxone;xboxseries;playstation;switch;0;False;True;1;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;True;0;True;-30;False;False;False;False;False;False;False;False;False;True;True;0;True;-8;255;False;-1;255;True;-9;7;False;-1;3;False;-1;1;False;-1;1;False;-1;7;False;-1;3;False;-1;1;False;-1;1;False;-1;False;True;1;False;-1;False;False;True;1;LightMode=TransparentDepthPrepass;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;249;1794.491,-1976.747;Float;False;False;-1;2;Rendering.HighDefinition.LightingShaderGraphGUI;0;1;New Amplify Shader;53b46d85872c5b24c8f4f0a1c3fe4c87;True;TransparentDepthPostpass;0;9;TransparentDepthPostpass;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=HDRenderPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;5;True;7;d3d11;metal;vulkan;xboxone;xboxseries;playstation;switch;0;False;True;1;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;True;0;True;-30;False;True;False;False;False;False;0;False;-1;False;False;False;False;False;False;False;False;False;True;1;False;-1;False;False;True;1;LightMode=TransparentDepthPostpass;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;240;2123.491,-2096.747;Float;False;True;-1;2;Rendering.HighDefinition.LightingShaderGraphGUI;0;2;DF/GroundSplat;53b46d85872c5b24c8f4f0a1c3fe4c87;True;GBuffer;0;0;GBuffer;35;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=HDRenderPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;5;True;7;d3d11;metal;vulkan;xboxone;xboxseries;playstation;switch;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;0;True;-30;False;False;False;False;False;False;False;False;False;True;True;0;True;-15;255;False;-1;255;True;-14;7;False;-1;3;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;False;True;0;True;-16;False;True;1;LightMode=GBuffer;False;False;0;;0;0;Standard;42;Surface Type;0;0;  Rendering Pass;1;0;  Refraction Model;0;0;    Blending Mode;0;0;    Blend Preserves Specular;1;0;  Receive Fog;1;0;  Back Then Front Rendering;0;0;  Transparent Depth Prepass;0;0;  Transparent Depth Postpass;0;0;  Transparent Writes Motion Vector;0;0;  Distortion;0;0;    Distortion Mode;0;0;    Distortion Depth Test;1;0;  ZWrite;0;0;  Z Test;4;0;Double-Sided;0;0;Alpha Clipping;0;0;  Use Shadow Threshold;0;0;Material Type,InvertActionOnDeselection;0;0;  Energy Conserving Specular;1;0;  Transmission;1;0;Receive Decals;0;638025943539503617;Receives SSR;0;638025943542930856;Receive SSR Transparent;0;0;Motion Vectors;1;638025981245303722;  Add Precomputed Velocity;0;0;Specular AA;0;0;Specular Occlusion Mode;1;0;Override Baked GI;0;0;Depth Offset;0;0;DOTS Instancing;0;0;LOD CrossFade;0;0;Tessellation;0;0;  Phong;0;0;  Strength;0.5,False,-1;0;  Type;0;0;  Tess;16,False,-1;0;  Min;10,False,-1;0;  Max;25,False,-1;0;  Edge Length;16,False,-1;0;  Max Displacement;25,False,-1;0;Vertex Position;1;0;0;12;True;True;True;True;True;True;False;False;False;False;True;True;False;;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;246;1794.491,-1976.747;Float;False;False;-1;2;Rendering.HighDefinition.LightingShaderGraphGUI;0;1;New Amplify Shader;53b46d85872c5b24c8f4f0a1c3fe4c87;True;Distortion;0;6;Distortion;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=HDRenderPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;5;True;7;d3d11;metal;vulkan;xboxone;xboxseries;playstation;switch;0;False;True;4;1;False;-1;1;False;-1;4;1;False;-1;1;False;-1;True;1;False;-1;1;False;-1;False;False;False;False;False;False;False;False;False;False;False;True;0;True;-30;False;False;False;False;False;False;False;False;False;True;True;0;True;-12;255;False;-1;255;True;-13;7;False;-1;3;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;2;False;-1;True;3;False;-1;False;True;1;LightMode=DistortionVectors;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;250;1794.491,-1976.747;Float;False;False;-1;2;Rendering.HighDefinition.LightingShaderGraphGUI;0;1;New Amplify Shader;53b46d85872c5b24c8f4f0a1c3fe4c87;True;Forward;0;10;Forward;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=HDRenderPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;5;True;7;d3d11;metal;vulkan;xboxone;xboxseries;playstation;switch;0;False;True;1;0;True;-22;0;True;-23;1;0;True;-24;0;True;-25;False;False;False;False;False;False;False;False;False;False;False;False;True;0;True;-33;False;False;False;True;True;True;True;True;0;True;-53;False;False;False;False;False;True;True;0;True;-6;255;False;-1;255;True;-7;7;False;-1;3;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;0;True;-28;True;0;True;-36;False;True;1;LightMode=Forward;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;251;1794.491,-1976.747;Float;False;False;-1;2;Rendering.HighDefinition.LightingShaderGraphGUI;0;1;New Amplify Shader;53b46d85872c5b24c8f4f0a1c3fe4c87;True;ScenePickingPass;0;11;ScenePickingPass;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=HDRenderPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;5;True;7;d3d11;metal;vulkan;xboxone;xboxseries;playstation;switch;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;0;True;-30;False;False;False;False;False;False;False;False;False;False;False;True;2;False;-1;True;3;False;-1;False;True;1;LightMode=Picking;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;242;1794.491,-1976.747;Float;False;False;-1;2;Rendering.HighDefinition.LightingShaderGraphGUI;0;1;New Amplify Shader;53b46d85872c5b24c8f4f0a1c3fe4c87;True;ShadowCaster;0;2;ShadowCaster;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=HDRenderPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;5;True;7;d3d11;metal;vulkan;xboxone;xboxseries;playstation;switch;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;0;True;-30;False;True;False;False;False;False;0;False;-1;False;False;False;False;False;False;False;False;False;True;1;False;-1;True;3;False;-1;False;True;1;LightMode=ShadowCaster;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;245;1794.491,-1976.747;Float;False;False;-1;2;Rendering.HighDefinition.LightingShaderGraphGUI;0;1;New Amplify Shader;53b46d85872c5b24c8f4f0a1c3fe4c87;True;Motion Vectors;0;5;Motion Vectors;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=HDRenderPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;5;True;7;d3d11;metal;vulkan;xboxone;xboxseries;playstation;switch;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;0;True;-30;False;False;False;False;False;False;False;False;False;True;True;0;True;-10;255;False;-1;255;True;-11;7;False;-1;3;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;1;False;-1;False;False;True;1;LightMode=MotionVectors;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;244;1794.491,-1976.747;Float;False;False;-1;2;Rendering.HighDefinition.LightingShaderGraphGUI;0;1;New Amplify Shader;53b46d85872c5b24c8f4f0a1c3fe4c87;True;DepthOnly;0;4;DepthOnly;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=HDRenderPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;5;True;7;d3d11;metal;vulkan;xboxone;xboxseries;playstation;switch;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;0;True;-30;False;False;False;False;False;False;False;False;False;True;True;0;True;-8;255;False;-1;255;True;-9;7;False;-1;3;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;1;False;-1;False;False;True;1;LightMode=DepthOnly;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;247;1794.491,-1976.747;Float;False;False;-1;2;Rendering.HighDefinition.LightingShaderGraphGUI;0;1;New Amplify Shader;53b46d85872c5b24c8f4f0a1c3fe4c87;True;TransparentBackface;0;7;TransparentBackface;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=HDRenderPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;5;True;7;d3d11;metal;vulkan;xboxone;xboxseries;playstation;switch;0;False;True;1;0;True;-22;0;True;-23;1;0;True;-24;0;True;-25;False;False;False;False;False;False;False;False;False;False;False;False;True;1;False;-1;False;False;False;True;True;True;True;True;0;True;-53;False;False;False;False;False;False;False;True;0;True;-28;True;0;True;-37;False;True;1;LightMode=TransparentBackface;False;False;0;;0;0;Standard;0;False;0
WireConnection;259;0;254;3
WireConnection;259;1;254;4
WireConnection;257;0;296;1
WireConnection;257;1;296;3
WireConnection;260;0;254;1
WireConnection;260;1;254;2
WireConnection;261;0;259;0
WireConnection;261;1;260;0
WireConnection;262;0;257;0
WireConnection;262;1;260;0
WireConnection;263;0;258;0
WireConnection;264;0;262;0
WireConnection;264;1;261;0
WireConnection;265;0;263;3
WireConnection;265;1;263;4
WireConnection;266;0;264;0
WireConnection;266;1;265;0
WireConnection;267;0;266;0
WireConnection;269;0;267;0
WireConnection;270;0;269;0
WireConnection;272;0;266;0
WireConnection;272;1;270;0
WireConnection;273;0;272;0
WireConnection;274;0;273;0
WireConnection;299;5;258;0
WireConnection;299;7;270;0
WireConnection;299;8;281;0
WireConnection;299;9;265;0
WireConnection;299;10;283;0
WireConnection;299;13;279;0
WireConnection;299;17;284;0
WireConnection;299;22;277;0
WireConnection;299;33;278;0
WireConnection;302;5;258;0
WireConnection;302;7;270;0
WireConnection;302;8;280;0
WireConnection;302;9;265;0
WireConnection;302;10;283;0
WireConnection;302;13;279;0
WireConnection;302;17;284;0
WireConnection;302;22;277;0
WireConnection;302;33;278;0
WireConnection;301;5;258;0
WireConnection;301;7;270;0
WireConnection;301;8;282;0
WireConnection;301;9;265;0
WireConnection;301;10;283;0
WireConnection;301;13;279;0
WireConnection;301;17;284;0
WireConnection;301;22;277;0
WireConnection;301;33;278;0
WireConnection;300;5;258;0
WireConnection;300;7;270;0
WireConnection;300;8;276;0
WireConnection;300;9;265;0
WireConnection;300;10;283;0
WireConnection;300;13;279;0
WireConnection;300;17;284;0
WireConnection;300;22;277;0
WireConnection;300;33;278;0
WireConnection;290;29;302;1
WireConnection;290;24;302;3
WireConnection;290;25;274;0
WireConnection;290;30;299;1
WireConnection;290;26;299;3
WireConnection;290;27;273;0
WireConnection;293;29;300;1
WireConnection;293;24;300;3
WireConnection;293;25;274;0
WireConnection;293;30;301;1
WireConnection;293;26;301;3
WireConnection;293;27;273;0
WireConnection;291;29;302;2
WireConnection;291;24;302;3
WireConnection;291;25;274;0
WireConnection;291;30;299;2
WireConnection;291;26;299;3
WireConnection;291;27;273;0
WireConnection;275;0;273;1
WireConnection;294;29;300;2
WireConnection;294;24;300;3
WireConnection;294;25;274;0
WireConnection;294;30;301;2
WireConnection;294;26;301;3
WireConnection;294;27;273;0
WireConnection;223;0;290;0
WireConnection;224;0;293;0
WireConnection;221;29;291;0
WireConnection;221;24;223;2
WireConnection;221;25;275;0
WireConnection;221;30;294;0
WireConnection;221;26;224;2
WireConnection;221;27;273;1
WireConnection;317;0;221;0
WireConnection;292;29;300;0
WireConnection;292;24;300;3
WireConnection;292;25;274;0
WireConnection;292;30;301;0
WireConnection;292;26;301;3
WireConnection;292;27;273;0
WireConnection;384;31;264;0
WireConnection;384;35;161;0
WireConnection;384;6;254;0
WireConnection;384;14;315;0
WireConnection;289;29;302;0
WireConnection;289;24;302;3
WireConnection;289;25;274;0
WireConnection;289;30;299;0
WireConnection;289;26;299;3
WireConnection;289;27;273;0
WireConnection;159;29;289;0
WireConnection;159;24;223;2
WireConnection;159;25;275;0
WireConnection;159;30;292;0
WireConnection;159;26;224;2
WireConnection;159;27;273;1
WireConnection;161;0;159;0
WireConnection;161;1;221;0
WireConnection;298;0;180;0
WireConnection;318;1;20;0
WireConnection;180;29;290;0
WireConnection;180;24;223;2
WireConnection;180;25;275;0
WireConnection;180;30;293;0
WireConnection;180;26;224;2
WireConnection;180;27;273;1
WireConnection;316;2;317;3
WireConnection;220;0;180;0
WireConnection;220;1;313;0
WireConnection;319;0;159;0
WireConnection;307;0;319;3
WireConnection;307;1;21;0
WireConnection;240;0;384;0
WireConnection;240;1;220;0
WireConnection;240;4;318;0
WireConnection;240;7;307;0
WireConnection;240;8;298;0
WireConnection;240;9;316;1
ASEEND*/
//CHKSM=A96BCB26676D478FBD7852AB767AC48830E24B8A