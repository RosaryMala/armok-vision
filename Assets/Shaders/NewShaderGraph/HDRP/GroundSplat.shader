// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "GroundSplat"
{
	Properties
	{
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		[PerRendererData]_Tint("Tint", 2D) = "black" {}
		[PerRendererData]_GrassTint("GrassTint", 2D) = "black" {}
		_Control("Control", 2D) = "black" {}
		[PerRendererData]_GrassControl("GrassControl", 2D) = "black" {}
		[PerRendererData]_SpatterTex("SpatterTex", 2D) = "white" {}
		_MetalLevel("Metallic", Range( 0 , 1)) = 0
		_Rough("Roughness", Range( 0 , 1)) = 0.5
		_WorldBounds("WorldBounds", Vector) = (0,0,1,1)
		_SpatterNoise("SpatterNoise", 2D) = "white" {}
		_MatTexArray("MatTexArray", 2DArray) = "white" {}
		[ASEEnd]_ShapeMap("Shape Texture Splat", 2DArray) = "bump" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}

		[HideInInspector]_EmissionColor("Color", Color) = (1, 1, 1, 1)
		[HideInInspector] _RenderQueueType("Render Queue Type", Float) = 1
		//[HideInInspector] [ToggleUI] _AddPrecomputedVelocity("Add Precomputed Velocity", Float) = 1
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
		[HideInInspector] [ToggleUI] _AlphaCutoffEnable("Alpha Cutoff Enable", Float) = 1
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

			#define _DISABLE_SSR_TRANSPARENT 1
			#define _DISABLE_DECALS 1
			#define _DISABLE_SSR 1
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
			float4 _WorldBounds;
			float4 _MatTexArray_ST;
			float4 _Control_TexelSize;
			float4 _ShapeMap_ST;
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
			sampler2D _SpatterTex;
			sampler2D _SpatterNoise;
			TEXTURE2D_ARRAY(_MatTexArray);
			sampler2D _Control;
			SAMPLER(sampler_MatTexArray);
			sampler2D _GrassControl;
			TEXTURE2D_ARRAY(_ShapeMap);
			SAMPLER(sampler_ShapeMap);
			sampler2D _GrassTint;
			sampler2D _Tint;


			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/NormalSurfaceGradient.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Lit/Lit.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/BuiltinUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/MaterialUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Decal/DecalUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Lit/LitDecalData.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderGraphFunctions.hlsl"

			#define ASE_NEEDS_FRAG_WORLD_NORMAL
			#define ASE_NEEDS_FRAG_RELATIVE_WORLD_POS
			#pragma multi_compile __ CONTAMINANTS
			#pragma multi_compile __ GRASS
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
				float dotResult145_g79 = dot( normalWS , float3( 0,1,0 ) );
				float3 ase_worldPos = GetAbsolutePositionWS( positionRWS );
				float3 break180_g79 = ase_worldPos;
				float2 appendResult102_g79 = (float2(break180_g79.x , break180_g79.z));
				float4 break174_g79 = _WorldBounds;
				float2 appendResult129_g79 = (float2(break174_g79.x , break174_g79.y));
				float2 appendResult130_g79 = (float2(break174_g79.z , break174_g79.w));
				float4 tex2DNode133_g79 = tex2D( _SpatterTex, ( ( appendResult102_g79 - appendResult129_g79 ) / ( appendResult130_g79 - appendResult129_g79 ) ) );
				float4 temp_cast_0 = (tex2DNode133_g79.a).xxxx;
				float4 lerpResult139_g79 = lerp( float4( 1,0,0,0 ) , float4( -1,0,0,0 ) , ( temp_cast_0 - tex2D( _SpatterNoise, appendResult102_g79 ) ));
				float temp_output_147_0_g79 = ( dotResult145_g79 > lerpResult139_g79.r ? 1.0 : 0.0 );
				float2 uv_MatTexArray = packedInput.ase_texcoord5.xy;
				float2 appendResult69 = (float2(ase_worldPos.x , ase_worldPos.z));
				float2 appendResult70 = (float2(_WorldBounds.x , _WorldBounds.y));
				float2 appendResult72 = (float2(_WorldBounds.z , _WorldBounds.w));
				float2 appendResult92 = (float2(_Control_TexelSize.z , _Control_TexelSize.w));
				float2 temp_output_90_0 = ( ( ( appendResult69 - appendResult70 ) / ( appendResult72 - appendResult70 ) ) * appendResult92 );
				float2 temp_output_94_0 = floor( ( ( temp_output_90_0 + float2( 0.5,0.5 ) ) - float2( 0.5,0.5 ) ) );
				float2 temp_output_12_0_g28 = ( ( temp_output_94_0 + float2( 0,0 ) ) / appendResult92 );
				float4 tex2DNode6_g28 = tex2D( _Control, temp_output_12_0_g28 );
				float4 tex2DArrayNode23_g28 = SAMPLE_TEXTURE2D_ARRAY( _MatTexArray, sampler_MatTexArray, uv_MatTexArray,( tex2DNode6_g28.r * 255.0 ) );
				float4 tex2DNode15_g28 = tex2D( _GrassControl, temp_output_12_0_g28 );
				float2 uv_ShapeMap = packedInput.ase_texcoord5.xy;
				float4 tex2DArrayNode35_g28 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode15_g28.g * 255.0 ) );
				float temp_output_24_0_g30 = tex2DArrayNode35_g28.b;
				float4 tex2DNode16_g28 = tex2D( _GrassTint, temp_output_12_0_g28 );
				float temp_output_25_0_g30 = tex2DNode16_g28.a;
				float4 tex2DArrayNode32_g28 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode6_g28.g * 255.0 ) );
				float temp_output_26_0_g30 = tex2DArrayNode32_g28.b;
				float temp_output_41_0_g28 = ( 1.0 - tex2DNode16_g28.a );
				float temp_output_27_0_g30 = temp_output_41_0_g28;
				float temp_output_1_0_g30 = ( max( ( temp_output_24_0_g30 + temp_output_25_0_g30 ) , ( temp_output_26_0_g30 + temp_output_27_0_g30 ) ) - 0.2 );
				float temp_output_21_0_g30 = max( ( ( temp_output_24_0_g30 + temp_output_25_0_g30 ) - temp_output_1_0_g30 ) , 0.0 );
				float temp_output_19_0_g30 = max( ( ( temp_output_26_0_g30 + temp_output_27_0_g30 ) - temp_output_1_0_g30 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch29_g28 = ( ( ( SAMPLE_TEXTURE2D_ARRAY( _MatTexArray, sampler_MatTexArray, uv_MatTexArray,( tex2DNode15_g28.r * 255.0 ) ) * temp_output_21_0_g30 ) + ( tex2DArrayNode23_g28 * temp_output_19_0_g30 ) ) / ( temp_output_21_0_g30 + temp_output_19_0_g30 ) );
				#else
				float4 staticSwitch29_g28 = tex2DArrayNode23_g28;
				#endif
				float temp_output_24_0_g29 = tex2DArrayNode35_g28.b;
				float temp_output_25_0_g29 = tex2DNode16_g28.a;
				float temp_output_26_0_g29 = tex2DArrayNode32_g28.b;
				float temp_output_27_0_g29 = temp_output_41_0_g28;
				float temp_output_1_0_g29 = ( max( ( temp_output_24_0_g29 + temp_output_25_0_g29 ) , ( temp_output_26_0_g29 + temp_output_27_0_g29 ) ) - 0.2 );
				float temp_output_21_0_g29 = max( ( ( temp_output_24_0_g29 + temp_output_25_0_g29 ) - temp_output_1_0_g29 ) , 0.0 );
				float temp_output_19_0_g29 = max( ( ( temp_output_26_0_g29 + temp_output_27_0_g29 ) - temp_output_1_0_g29 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch31_g28 = ( ( ( tex2DArrayNode35_g28 * temp_output_21_0_g29 ) + ( tex2DArrayNode32_g28 * temp_output_19_0_g29 ) ) / ( temp_output_21_0_g29 + temp_output_19_0_g29 ) );
				#else
				float4 staticSwitch31_g28 = tex2DArrayNode32_g28;
				#endif
				float temp_output_334_3 = staticSwitch31_g28.b;
				float temp_output_24_0_g72 = temp_output_334_3;
				float2 break248 = ( temp_output_90_0 - temp_output_94_0 );
				float temp_output_264_0 = ( 1.0 - break248.x );
				float temp_output_25_0_g72 = temp_output_264_0;
				float2 temp_output_12_0_g40 = ( ( temp_output_94_0 + float2( 1,0 ) ) / appendResult92 );
				float4 tex2DNode6_g40 = tex2D( _Control, temp_output_12_0_g40 );
				float4 tex2DArrayNode32_g40 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode6_g40.g * 255.0 ) );
				float4 tex2DNode15_g40 = tex2D( _GrassControl, temp_output_12_0_g40 );
				float4 tex2DArrayNode35_g40 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode15_g40.g * 255.0 ) );
				float temp_output_24_0_g41 = tex2DArrayNode35_g40.b;
				float4 tex2DNode16_g40 = tex2D( _GrassTint, temp_output_12_0_g40 );
				float temp_output_25_0_g41 = tex2DNode16_g40.a;
				float temp_output_26_0_g41 = tex2DArrayNode32_g40.b;
				float temp_output_41_0_g40 = ( 1.0 - tex2DNode16_g40.a );
				float temp_output_27_0_g41 = temp_output_41_0_g40;
				float temp_output_1_0_g41 = ( max( ( temp_output_24_0_g41 + temp_output_25_0_g41 ) , ( temp_output_26_0_g41 + temp_output_27_0_g41 ) ) - 0.2 );
				float temp_output_21_0_g41 = max( ( ( temp_output_24_0_g41 + temp_output_25_0_g41 ) - temp_output_1_0_g41 ) , 0.0 );
				float temp_output_19_0_g41 = max( ( ( temp_output_26_0_g41 + temp_output_27_0_g41 ) - temp_output_1_0_g41 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch31_g40 = ( ( ( tex2DArrayNode35_g40 * temp_output_21_0_g41 ) + ( tex2DArrayNode32_g40 * temp_output_19_0_g41 ) ) / ( temp_output_21_0_g41 + temp_output_19_0_g41 ) );
				#else
				float4 staticSwitch31_g40 = tex2DArrayNode32_g40;
				#endif
				float temp_output_332_3 = staticSwitch31_g40.b;
				float temp_output_26_0_g72 = temp_output_332_3;
				float temp_output_27_0_g72 = break248.x;
				float temp_output_1_0_g72 = ( max( ( temp_output_24_0_g72 + temp_output_25_0_g72 ) , ( temp_output_26_0_g72 + temp_output_27_0_g72 ) ) - 0.2 );
				float temp_output_21_0_g72 = max( ( ( temp_output_24_0_g72 + temp_output_25_0_g72 ) - temp_output_1_0_g72 ) , 0.0 );
				float4 tex2DArrayNode23_g40 = SAMPLE_TEXTURE2D_ARRAY( _MatTexArray, sampler_MatTexArray, uv_MatTexArray,( tex2DNode6_g40.r * 255.0 ) );
				float temp_output_24_0_g42 = tex2DArrayNode35_g40.b;
				float temp_output_25_0_g42 = tex2DNode16_g40.a;
				float temp_output_26_0_g42 = tex2DArrayNode32_g40.b;
				float temp_output_27_0_g42 = temp_output_41_0_g40;
				float temp_output_1_0_g42 = ( max( ( temp_output_24_0_g42 + temp_output_25_0_g42 ) , ( temp_output_26_0_g42 + temp_output_27_0_g42 ) ) - 0.2 );
				float temp_output_21_0_g42 = max( ( ( temp_output_24_0_g42 + temp_output_25_0_g42 ) - temp_output_1_0_g42 ) , 0.0 );
				float temp_output_19_0_g42 = max( ( ( temp_output_26_0_g42 + temp_output_27_0_g42 ) - temp_output_1_0_g42 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch29_g40 = ( ( ( SAMPLE_TEXTURE2D_ARRAY( _MatTexArray, sampler_MatTexArray, uv_MatTexArray,( tex2DNode15_g40.r * 255.0 ) ) * temp_output_21_0_g42 ) + ( tex2DArrayNode23_g40 * temp_output_19_0_g42 ) ) / ( temp_output_21_0_g42 + temp_output_19_0_g42 ) );
				#else
				float4 staticSwitch29_g40 = tex2DArrayNode23_g40;
				#endif
				float temp_output_19_0_g72 = max( ( ( temp_output_26_0_g72 + temp_output_27_0_g72 ) - temp_output_1_0_g72 ) , 0.0 );
				float temp_output_24_0_g66 = temp_output_334_3;
				float temp_output_25_0_g66 = temp_output_264_0;
				float temp_output_26_0_g66 = temp_output_332_3;
				float temp_output_27_0_g66 = break248.x;
				float temp_output_1_0_g66 = ( max( ( temp_output_24_0_g66 + temp_output_25_0_g66 ) , ( temp_output_26_0_g66 + temp_output_27_0_g66 ) ) - 0.2 );
				float temp_output_21_0_g66 = max( ( ( temp_output_24_0_g66 + temp_output_25_0_g66 ) - temp_output_1_0_g66 ) , 0.0 );
				float temp_output_19_0_g66 = max( ( ( temp_output_26_0_g66 + temp_output_27_0_g66 ) - temp_output_1_0_g66 ) , 0.0 );
				float4 temp_output_304_0 = ( ( ( staticSwitch31_g28 * temp_output_21_0_g66 ) + ( staticSwitch31_g40 * temp_output_19_0_g66 ) ) / ( temp_output_21_0_g66 + temp_output_19_0_g66 ) );
				float temp_output_24_0_g76 = temp_output_304_0.z;
				float temp_output_269_0 = ( 1.0 - break248.y );
				float temp_output_25_0_g76 = temp_output_269_0;
				float2 temp_output_12_0_g32 = ( ( temp_output_94_0 + float2( 0,1 ) ) / appendResult92 );
				float4 tex2DNode6_g32 = tex2D( _Control, temp_output_12_0_g32 );
				float4 tex2DArrayNode32_g32 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode6_g32.g * 255.0 ) );
				float4 tex2DNode15_g32 = tex2D( _GrassControl, temp_output_12_0_g32 );
				float4 tex2DArrayNode35_g32 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode15_g32.g * 255.0 ) );
				float temp_output_24_0_g33 = tex2DArrayNode35_g32.b;
				float4 tex2DNode16_g32 = tex2D( _GrassTint, temp_output_12_0_g32 );
				float temp_output_25_0_g33 = tex2DNode16_g32.a;
				float temp_output_26_0_g33 = tex2DArrayNode32_g32.b;
				float temp_output_41_0_g32 = ( 1.0 - tex2DNode16_g32.a );
				float temp_output_27_0_g33 = temp_output_41_0_g32;
				float temp_output_1_0_g33 = ( max( ( temp_output_24_0_g33 + temp_output_25_0_g33 ) , ( temp_output_26_0_g33 + temp_output_27_0_g33 ) ) - 0.2 );
				float temp_output_21_0_g33 = max( ( ( temp_output_24_0_g33 + temp_output_25_0_g33 ) - temp_output_1_0_g33 ) , 0.0 );
				float temp_output_19_0_g33 = max( ( ( temp_output_26_0_g33 + temp_output_27_0_g33 ) - temp_output_1_0_g33 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch31_g32 = ( ( ( tex2DArrayNode35_g32 * temp_output_21_0_g33 ) + ( tex2DArrayNode32_g32 * temp_output_19_0_g33 ) ) / ( temp_output_21_0_g33 + temp_output_19_0_g33 ) );
				#else
				float4 staticSwitch31_g32 = tex2DArrayNode32_g32;
				#endif
				float temp_output_335_3 = staticSwitch31_g32.b;
				float temp_output_24_0_g67 = temp_output_335_3;
				float temp_output_25_0_g67 = temp_output_264_0;
				float2 temp_output_12_0_g36 = ( ( temp_output_94_0 + float2( 1,1 ) ) / appendResult92 );
				float4 tex2DNode6_g36 = tex2D( _Control, temp_output_12_0_g36 );
				float4 tex2DArrayNode32_g36 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode6_g36.g * 255.0 ) );
				float4 tex2DNode15_g36 = tex2D( _GrassControl, temp_output_12_0_g36 );
				float4 tex2DArrayNode35_g36 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode15_g36.g * 255.0 ) );
				float temp_output_24_0_g37 = tex2DArrayNode35_g36.b;
				float4 tex2DNode16_g36 = tex2D( _GrassTint, temp_output_12_0_g36 );
				float temp_output_25_0_g37 = tex2DNode16_g36.a;
				float temp_output_26_0_g37 = tex2DArrayNode32_g36.b;
				float temp_output_41_0_g36 = ( 1.0 - tex2DNode16_g36.a );
				float temp_output_27_0_g37 = temp_output_41_0_g36;
				float temp_output_1_0_g37 = ( max( ( temp_output_24_0_g37 + temp_output_25_0_g37 ) , ( temp_output_26_0_g37 + temp_output_27_0_g37 ) ) - 0.2 );
				float temp_output_21_0_g37 = max( ( ( temp_output_24_0_g37 + temp_output_25_0_g37 ) - temp_output_1_0_g37 ) , 0.0 );
				float temp_output_19_0_g37 = max( ( ( temp_output_26_0_g37 + temp_output_27_0_g37 ) - temp_output_1_0_g37 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch31_g36 = ( ( ( tex2DArrayNode35_g36 * temp_output_21_0_g37 ) + ( tex2DArrayNode32_g36 * temp_output_19_0_g37 ) ) / ( temp_output_21_0_g37 + temp_output_19_0_g37 ) );
				#else
				float4 staticSwitch31_g36 = tex2DArrayNode32_g36;
				#endif
				float temp_output_333_3 = staticSwitch31_g36.b;
				float temp_output_26_0_g67 = temp_output_333_3;
				float temp_output_27_0_g67 = break248.x;
				float temp_output_1_0_g67 = ( max( ( temp_output_24_0_g67 + temp_output_25_0_g67 ) , ( temp_output_26_0_g67 + temp_output_27_0_g67 ) ) - 0.2 );
				float temp_output_21_0_g67 = max( ( ( temp_output_24_0_g67 + temp_output_25_0_g67 ) - temp_output_1_0_g67 ) , 0.0 );
				float temp_output_19_0_g67 = max( ( ( temp_output_26_0_g67 + temp_output_27_0_g67 ) - temp_output_1_0_g67 ) , 0.0 );
				float4 temp_output_305_0 = ( ( ( staticSwitch31_g32 * temp_output_21_0_g67 ) + ( staticSwitch31_g36 * temp_output_19_0_g67 ) ) / ( temp_output_21_0_g67 + temp_output_19_0_g67 ) );
				float temp_output_26_0_g76 = temp_output_305_0.z;
				float temp_output_27_0_g76 = break248.y;
				float temp_output_1_0_g76 = ( max( ( temp_output_24_0_g76 + temp_output_25_0_g76 ) , ( temp_output_26_0_g76 + temp_output_27_0_g76 ) ) - 0.2 );
				float temp_output_21_0_g76 = max( ( ( temp_output_24_0_g76 + temp_output_25_0_g76 ) - temp_output_1_0_g76 ) , 0.0 );
				float4 tex2DArrayNode23_g32 = SAMPLE_TEXTURE2D_ARRAY( _MatTexArray, sampler_MatTexArray, uv_MatTexArray,( tex2DNode6_g32.r * 255.0 ) );
				float temp_output_24_0_g34 = tex2DArrayNode35_g32.b;
				float temp_output_25_0_g34 = tex2DNode16_g32.a;
				float temp_output_26_0_g34 = tex2DArrayNode32_g32.b;
				float temp_output_27_0_g34 = temp_output_41_0_g32;
				float temp_output_1_0_g34 = ( max( ( temp_output_24_0_g34 + temp_output_25_0_g34 ) , ( temp_output_26_0_g34 + temp_output_27_0_g34 ) ) - 0.2 );
				float temp_output_21_0_g34 = max( ( ( temp_output_24_0_g34 + temp_output_25_0_g34 ) - temp_output_1_0_g34 ) , 0.0 );
				float temp_output_19_0_g34 = max( ( ( temp_output_26_0_g34 + temp_output_27_0_g34 ) - temp_output_1_0_g34 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch29_g32 = ( ( ( SAMPLE_TEXTURE2D_ARRAY( _MatTexArray, sampler_MatTexArray, uv_MatTexArray,( tex2DNode15_g32.r * 255.0 ) ) * temp_output_21_0_g34 ) + ( tex2DArrayNode23_g32 * temp_output_19_0_g34 ) ) / ( temp_output_21_0_g34 + temp_output_19_0_g34 ) );
				#else
				float4 staticSwitch29_g32 = tex2DArrayNode23_g32;
				#endif
				float temp_output_24_0_g71 = temp_output_335_3;
				float temp_output_25_0_g71 = temp_output_264_0;
				float temp_output_26_0_g71 = temp_output_333_3;
				float temp_output_27_0_g71 = break248.x;
				float temp_output_1_0_g71 = ( max( ( temp_output_24_0_g71 + temp_output_25_0_g71 ) , ( temp_output_26_0_g71 + temp_output_27_0_g71 ) ) - 0.2 );
				float temp_output_21_0_g71 = max( ( ( temp_output_24_0_g71 + temp_output_25_0_g71 ) - temp_output_1_0_g71 ) , 0.0 );
				float4 tex2DArrayNode23_g36 = SAMPLE_TEXTURE2D_ARRAY( _MatTexArray, sampler_MatTexArray, uv_MatTexArray,( tex2DNode6_g36.r * 255.0 ) );
				float temp_output_24_0_g38 = tex2DArrayNode35_g36.b;
				float temp_output_25_0_g38 = tex2DNode16_g36.a;
				float temp_output_26_0_g38 = tex2DArrayNode32_g36.b;
				float temp_output_27_0_g38 = temp_output_41_0_g36;
				float temp_output_1_0_g38 = ( max( ( temp_output_24_0_g38 + temp_output_25_0_g38 ) , ( temp_output_26_0_g38 + temp_output_27_0_g38 ) ) - 0.2 );
				float temp_output_21_0_g38 = max( ( ( temp_output_24_0_g38 + temp_output_25_0_g38 ) - temp_output_1_0_g38 ) , 0.0 );
				float temp_output_19_0_g38 = max( ( ( temp_output_26_0_g38 + temp_output_27_0_g38 ) - temp_output_1_0_g38 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch29_g36 = ( ( ( SAMPLE_TEXTURE2D_ARRAY( _MatTexArray, sampler_MatTexArray, uv_MatTexArray,( tex2DNode15_g36.r * 255.0 ) ) * temp_output_21_0_g38 ) + ( tex2DArrayNode23_g36 * temp_output_19_0_g38 ) ) / ( temp_output_21_0_g38 + temp_output_19_0_g38 ) );
				#else
				float4 staticSwitch29_g36 = tex2DArrayNode23_g36;
				#endif
				float temp_output_19_0_g71 = max( ( ( temp_output_26_0_g71 + temp_output_27_0_g71 ) - temp_output_1_0_g71 ) , 0.0 );
				float temp_output_19_0_g76 = max( ( ( temp_output_26_0_g76 + temp_output_27_0_g76 ) - temp_output_1_0_g76 ) , 0.0 );
				float4 tex2DNode14_g28 = tex2D( _Tint, temp_output_12_0_g28 );
				float temp_output_24_0_g31 = tex2DArrayNode35_g28.b;
				float temp_output_25_0_g31 = tex2DNode16_g28.a;
				float temp_output_26_0_g31 = tex2DArrayNode32_g28.b;
				float temp_output_27_0_g31 = temp_output_41_0_g28;
				float temp_output_1_0_g31 = ( max( ( temp_output_24_0_g31 + temp_output_25_0_g31 ) , ( temp_output_26_0_g31 + temp_output_27_0_g31 ) ) - 0.2 );
				float temp_output_21_0_g31 = max( ( ( temp_output_24_0_g31 + temp_output_25_0_g31 ) - temp_output_1_0_g31 ) , 0.0 );
				float temp_output_19_0_g31 = max( ( ( temp_output_26_0_g31 + temp_output_27_0_g31 ) - temp_output_1_0_g31 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch30_g28 = ( ( ( tex2DNode16_g28 * temp_output_21_0_g31 ) + ( tex2DNode14_g28 * temp_output_19_0_g31 ) ) / ( temp_output_21_0_g31 + temp_output_19_0_g31 ) );
				#else
				float4 staticSwitch30_g28 = tex2DNode14_g28;
				#endif
				float temp_output_24_0_g73 = temp_output_334_3;
				float temp_output_25_0_g73 = temp_output_264_0;
				float temp_output_26_0_g73 = temp_output_332_3;
				float temp_output_27_0_g73 = break248.x;
				float temp_output_1_0_g73 = ( max( ( temp_output_24_0_g73 + temp_output_25_0_g73 ) , ( temp_output_26_0_g73 + temp_output_27_0_g73 ) ) - 0.2 );
				float temp_output_21_0_g73 = max( ( ( temp_output_24_0_g73 + temp_output_25_0_g73 ) - temp_output_1_0_g73 ) , 0.0 );
				float4 tex2DNode14_g40 = tex2D( _Tint, temp_output_12_0_g40 );
				float temp_output_24_0_g43 = tex2DArrayNode35_g40.b;
				float temp_output_25_0_g43 = tex2DNode16_g40.a;
				float temp_output_26_0_g43 = tex2DArrayNode32_g40.b;
				float temp_output_27_0_g43 = temp_output_41_0_g40;
				float temp_output_1_0_g43 = ( max( ( temp_output_24_0_g43 + temp_output_25_0_g43 ) , ( temp_output_26_0_g43 + temp_output_27_0_g43 ) ) - 0.2 );
				float temp_output_21_0_g43 = max( ( ( temp_output_24_0_g43 + temp_output_25_0_g43 ) - temp_output_1_0_g43 ) , 0.0 );
				float temp_output_19_0_g43 = max( ( ( temp_output_26_0_g43 + temp_output_27_0_g43 ) - temp_output_1_0_g43 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch30_g40 = ( ( ( tex2DNode16_g40 * temp_output_21_0_g43 ) + ( tex2DNode14_g40 * temp_output_19_0_g43 ) ) / ( temp_output_21_0_g43 + temp_output_19_0_g43 ) );
				#else
				float4 staticSwitch30_g40 = tex2DNode14_g40;
				#endif
				float temp_output_19_0_g73 = max( ( ( temp_output_26_0_g73 + temp_output_27_0_g73 ) - temp_output_1_0_g73 ) , 0.0 );
				float temp_output_24_0_g75 = temp_output_304_0.z;
				float temp_output_25_0_g75 = temp_output_269_0;
				float temp_output_26_0_g75 = temp_output_305_0.z;
				float temp_output_27_0_g75 = break248.y;
				float temp_output_1_0_g75 = ( max( ( temp_output_24_0_g75 + temp_output_25_0_g75 ) , ( temp_output_26_0_g75 + temp_output_27_0_g75 ) ) - 0.2 );
				float temp_output_21_0_g75 = max( ( ( temp_output_24_0_g75 + temp_output_25_0_g75 ) - temp_output_1_0_g75 ) , 0.0 );
				float4 tex2DNode14_g32 = tex2D( _Tint, temp_output_12_0_g32 );
				float temp_output_24_0_g35 = tex2DArrayNode35_g32.b;
				float temp_output_25_0_g35 = tex2DNode16_g32.a;
				float temp_output_26_0_g35 = tex2DArrayNode32_g32.b;
				float temp_output_27_0_g35 = temp_output_41_0_g32;
				float temp_output_1_0_g35 = ( max( ( temp_output_24_0_g35 + temp_output_25_0_g35 ) , ( temp_output_26_0_g35 + temp_output_27_0_g35 ) ) - 0.2 );
				float temp_output_21_0_g35 = max( ( ( temp_output_24_0_g35 + temp_output_25_0_g35 ) - temp_output_1_0_g35 ) , 0.0 );
				float temp_output_19_0_g35 = max( ( ( temp_output_26_0_g35 + temp_output_27_0_g35 ) - temp_output_1_0_g35 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch30_g32 = ( ( ( tex2DNode16_g32 * temp_output_21_0_g35 ) + ( tex2DNode14_g32 * temp_output_19_0_g35 ) ) / ( temp_output_21_0_g35 + temp_output_19_0_g35 ) );
				#else
				float4 staticSwitch30_g32 = tex2DNode14_g32;
				#endif
				float temp_output_24_0_g74 = temp_output_335_3;
				float temp_output_25_0_g74 = temp_output_264_0;
				float temp_output_26_0_g74 = temp_output_333_3;
				float temp_output_27_0_g74 = break248.x;
				float temp_output_1_0_g74 = ( max( ( temp_output_24_0_g74 + temp_output_25_0_g74 ) , ( temp_output_26_0_g74 + temp_output_27_0_g74 ) ) - 0.2 );
				float temp_output_21_0_g74 = max( ( ( temp_output_24_0_g74 + temp_output_25_0_g74 ) - temp_output_1_0_g74 ) , 0.0 );
				float4 tex2DNode14_g36 = tex2D( _Tint, temp_output_12_0_g36 );
				float temp_output_24_0_g39 = tex2DArrayNode35_g36.b;
				float temp_output_25_0_g39 = tex2DNode16_g36.a;
				float temp_output_26_0_g39 = tex2DArrayNode32_g36.b;
				float temp_output_27_0_g39 = temp_output_41_0_g36;
				float temp_output_1_0_g39 = ( max( ( temp_output_24_0_g39 + temp_output_25_0_g39 ) , ( temp_output_26_0_g39 + temp_output_27_0_g39 ) ) - 0.2 );
				float temp_output_21_0_g39 = max( ( ( temp_output_24_0_g39 + temp_output_25_0_g39 ) - temp_output_1_0_g39 ) , 0.0 );
				float temp_output_19_0_g39 = max( ( ( temp_output_26_0_g39 + temp_output_27_0_g39 ) - temp_output_1_0_g39 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch30_g36 = ( ( ( tex2DNode16_g36 * temp_output_21_0_g39 ) + ( tex2DNode14_g36 * temp_output_19_0_g39 ) ) / ( temp_output_21_0_g39 + temp_output_19_0_g39 ) );
				#else
				float4 staticSwitch30_g36 = tex2DNode14_g36;
				#endif
				float temp_output_19_0_g74 = max( ( ( temp_output_26_0_g74 + temp_output_27_0_g74 ) - temp_output_1_0_g74 ) , 0.0 );
				float temp_output_19_0_g75 = max( ( ( temp_output_26_0_g75 + temp_output_27_0_g75 ) - temp_output_1_0_g75 ) , 0.0 );
				float4 ifLocalVar157_g79 = 0;
				UNITY_BRANCH 
				if( temp_output_147_0_g79 > 0.0 )
				ifLocalVar157_g79 = ( tex2DNode133_g79 / tex2DNode133_g79.a );
				else if( temp_output_147_0_g79 < 0.0 )
				ifLocalVar157_g79 = ( ( ( ( ( ( ( staticSwitch29_g28 * temp_output_21_0_g72 ) + ( staticSwitch29_g40 * temp_output_19_0_g72 ) ) / ( temp_output_21_0_g72 + temp_output_19_0_g72 ) ) * temp_output_21_0_g76 ) + ( ( ( ( staticSwitch29_g32 * temp_output_21_0_g71 ) + ( staticSwitch29_g36 * temp_output_19_0_g71 ) ) / ( temp_output_21_0_g71 + temp_output_19_0_g71 ) ) * temp_output_19_0_g76 ) ) / ( temp_output_21_0_g76 + temp_output_19_0_g76 ) ) * ( ( ( ( ( ( staticSwitch30_g28 * temp_output_21_0_g73 ) + ( staticSwitch30_g40 * temp_output_19_0_g73 ) ) / ( temp_output_21_0_g73 + temp_output_19_0_g73 ) ) * temp_output_21_0_g75 ) + ( ( ( ( staticSwitch30_g32 * temp_output_21_0_g74 ) + ( staticSwitch30_g36 * temp_output_19_0_g74 ) ) / ( temp_output_21_0_g74 + temp_output_19_0_g74 ) ) * temp_output_19_0_g75 ) ) / ( temp_output_21_0_g75 + temp_output_19_0_g75 ) ) );
				
				float temp_output_24_0_g80 = temp_output_304_0.z;
				float temp_output_25_0_g80 = temp_output_269_0;
				float temp_output_26_0_g80 = temp_output_305_0.z;
				float temp_output_27_0_g80 = break248.y;
				float temp_output_1_0_g80 = ( max( ( temp_output_24_0_g80 + temp_output_25_0_g80 ) , ( temp_output_26_0_g80 + temp_output_27_0_g80 ) ) - 0.2 );
				float temp_output_21_0_g80 = max( ( ( temp_output_24_0_g80 + temp_output_25_0_g80 ) - temp_output_1_0_g80 ) , 0.0 );
				float temp_output_19_0_g80 = max( ( ( temp_output_26_0_g80 + temp_output_27_0_g80 ) - temp_output_1_0_g80 ) , 0.0 );
				
				surfaceDescription.Albedo = ifLocalVar157_g79.rgb;
				surfaceDescription.Normal = UnpackNormalScale( ( ( ( temp_output_304_0 * temp_output_21_0_g80 ) + ( temp_output_305_0 * temp_output_19_0_g80 ) ) / ( temp_output_21_0_g80 + temp_output_19_0_g80 ) ), 1.0 );
				surfaceDescription.BentNormal = float3( 0, 0, 1 );
				surfaceDescription.CoatMask = 0;
				surfaceDescription.Metallic = ( temp_output_147_0_g79 * _MetalLevel );

				#ifdef _MATERIAL_FEATURE_SPECULAR_COLOR
				surfaceDescription.Specular = 0;
				#endif

				surfaceDescription.Emission = 0;
				surfaceDescription.Smoothness = ( temp_output_147_0_g79 * _Rough );
				surfaceDescription.Occlusion = 1;
				surfaceDescription.Alpha = temp_output_147_0_g79;

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

			#define _DISABLE_SSR_TRANSPARENT 1
			#define _DISABLE_DECALS 1
			#define _DISABLE_SSR 1
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
			float4 _WorldBounds;
			float4 _MatTexArray_ST;
			float4 _Control_TexelSize;
			float4 _ShapeMap_ST;
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
			sampler2D _SpatterTex;
			sampler2D _SpatterNoise;
			TEXTURE2D_ARRAY(_MatTexArray);
			sampler2D _Control;
			SAMPLER(sampler_MatTexArray);
			sampler2D _GrassControl;
			TEXTURE2D_ARRAY(_ShapeMap);
			SAMPLER(sampler_ShapeMap);
			sampler2D _GrassTint;
			sampler2D _Tint;


			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/NormalSurfaceGradient.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Lit/Lit.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/BuiltinUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/MaterialUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Decal/DecalUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Lit/LitDecalData.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderGraphFunctions.hlsl"

			#define ASE_NEEDS_VERT_NORMAL
			#pragma multi_compile __ CONTAMINANTS
			#pragma multi_compile __ GRASS
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
				float4 ase_texcoord4 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				#if defined(SHADER_STAGE_FRAGMENT) && defined(ASE_NEED_CULLFACE)
				FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
				#endif
			};

			
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

				float3 ase_worldNormal = TransformObjectToWorldNormal(inputMesh.normalOS);
				outputPackedVaryingsMeshToPS.ase_texcoord2.xyz = ase_worldNormal;
				float3 ase_worldPos = GetAbsolutePositionWS( TransformObjectToWorld( (inputMesh.positionOS).xyz ) );
				outputPackedVaryingsMeshToPS.ase_texcoord3.xyz = ase_worldPos;
				
				outputPackedVaryingsMeshToPS.ase_texcoord4.xy = inputMesh.uv0.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				outputPackedVaryingsMeshToPS.ase_texcoord2.w = 0;
				outputPackedVaryingsMeshToPS.ase_texcoord3.w = 0;
				outputPackedVaryingsMeshToPS.ase_texcoord4.zw = 0;

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
				float3 ase_worldNormal = packedInput.ase_texcoord2.xyz;
				float dotResult145_g79 = dot( ase_worldNormal , float3( 0,1,0 ) );
				float3 ase_worldPos = packedInput.ase_texcoord3.xyz;
				float3 break180_g79 = ase_worldPos;
				float2 appendResult102_g79 = (float2(break180_g79.x , break180_g79.z));
				float4 break174_g79 = _WorldBounds;
				float2 appendResult129_g79 = (float2(break174_g79.x , break174_g79.y));
				float2 appendResult130_g79 = (float2(break174_g79.z , break174_g79.w));
				float4 tex2DNode133_g79 = tex2D( _SpatterTex, ( ( appendResult102_g79 - appendResult129_g79 ) / ( appendResult130_g79 - appendResult129_g79 ) ) );
				float4 temp_cast_0 = (tex2DNode133_g79.a).xxxx;
				float4 lerpResult139_g79 = lerp( float4( 1,0,0,0 ) , float4( -1,0,0,0 ) , ( temp_cast_0 - tex2D( _SpatterNoise, appendResult102_g79 ) ));
				float temp_output_147_0_g79 = ( dotResult145_g79 > lerpResult139_g79.r ? 1.0 : 0.0 );
				float2 uv_MatTexArray = packedInput.ase_texcoord4.xy;
				float2 appendResult69 = (float2(ase_worldPos.x , ase_worldPos.z));
				float2 appendResult70 = (float2(_WorldBounds.x , _WorldBounds.y));
				float2 appendResult72 = (float2(_WorldBounds.z , _WorldBounds.w));
				float2 appendResult92 = (float2(_Control_TexelSize.z , _Control_TexelSize.w));
				float2 temp_output_90_0 = ( ( ( appendResult69 - appendResult70 ) / ( appendResult72 - appendResult70 ) ) * appendResult92 );
				float2 temp_output_94_0 = floor( ( ( temp_output_90_0 + float2( 0.5,0.5 ) ) - float2( 0.5,0.5 ) ) );
				float2 temp_output_12_0_g28 = ( ( temp_output_94_0 + float2( 0,0 ) ) / appendResult92 );
				float4 tex2DNode6_g28 = tex2D( _Control, temp_output_12_0_g28 );
				float4 tex2DArrayNode23_g28 = SAMPLE_TEXTURE2D_ARRAY( _MatTexArray, sampler_MatTexArray, uv_MatTexArray,( tex2DNode6_g28.r * 255.0 ) );
				float4 tex2DNode15_g28 = tex2D( _GrassControl, temp_output_12_0_g28 );
				float2 uv_ShapeMap = packedInput.ase_texcoord4.xy;
				float4 tex2DArrayNode35_g28 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode15_g28.g * 255.0 ) );
				float temp_output_24_0_g30 = tex2DArrayNode35_g28.b;
				float4 tex2DNode16_g28 = tex2D( _GrassTint, temp_output_12_0_g28 );
				float temp_output_25_0_g30 = tex2DNode16_g28.a;
				float4 tex2DArrayNode32_g28 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode6_g28.g * 255.0 ) );
				float temp_output_26_0_g30 = tex2DArrayNode32_g28.b;
				float temp_output_41_0_g28 = ( 1.0 - tex2DNode16_g28.a );
				float temp_output_27_0_g30 = temp_output_41_0_g28;
				float temp_output_1_0_g30 = ( max( ( temp_output_24_0_g30 + temp_output_25_0_g30 ) , ( temp_output_26_0_g30 + temp_output_27_0_g30 ) ) - 0.2 );
				float temp_output_21_0_g30 = max( ( ( temp_output_24_0_g30 + temp_output_25_0_g30 ) - temp_output_1_0_g30 ) , 0.0 );
				float temp_output_19_0_g30 = max( ( ( temp_output_26_0_g30 + temp_output_27_0_g30 ) - temp_output_1_0_g30 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch29_g28 = ( ( ( SAMPLE_TEXTURE2D_ARRAY( _MatTexArray, sampler_MatTexArray, uv_MatTexArray,( tex2DNode15_g28.r * 255.0 ) ) * temp_output_21_0_g30 ) + ( tex2DArrayNode23_g28 * temp_output_19_0_g30 ) ) / ( temp_output_21_0_g30 + temp_output_19_0_g30 ) );
				#else
				float4 staticSwitch29_g28 = tex2DArrayNode23_g28;
				#endif
				float temp_output_24_0_g29 = tex2DArrayNode35_g28.b;
				float temp_output_25_0_g29 = tex2DNode16_g28.a;
				float temp_output_26_0_g29 = tex2DArrayNode32_g28.b;
				float temp_output_27_0_g29 = temp_output_41_0_g28;
				float temp_output_1_0_g29 = ( max( ( temp_output_24_0_g29 + temp_output_25_0_g29 ) , ( temp_output_26_0_g29 + temp_output_27_0_g29 ) ) - 0.2 );
				float temp_output_21_0_g29 = max( ( ( temp_output_24_0_g29 + temp_output_25_0_g29 ) - temp_output_1_0_g29 ) , 0.0 );
				float temp_output_19_0_g29 = max( ( ( temp_output_26_0_g29 + temp_output_27_0_g29 ) - temp_output_1_0_g29 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch31_g28 = ( ( ( tex2DArrayNode35_g28 * temp_output_21_0_g29 ) + ( tex2DArrayNode32_g28 * temp_output_19_0_g29 ) ) / ( temp_output_21_0_g29 + temp_output_19_0_g29 ) );
				#else
				float4 staticSwitch31_g28 = tex2DArrayNode32_g28;
				#endif
				float temp_output_334_3 = staticSwitch31_g28.b;
				float temp_output_24_0_g72 = temp_output_334_3;
				float2 break248 = ( temp_output_90_0 - temp_output_94_0 );
				float temp_output_264_0 = ( 1.0 - break248.x );
				float temp_output_25_0_g72 = temp_output_264_0;
				float2 temp_output_12_0_g40 = ( ( temp_output_94_0 + float2( 1,0 ) ) / appendResult92 );
				float4 tex2DNode6_g40 = tex2D( _Control, temp_output_12_0_g40 );
				float4 tex2DArrayNode32_g40 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode6_g40.g * 255.0 ) );
				float4 tex2DNode15_g40 = tex2D( _GrassControl, temp_output_12_0_g40 );
				float4 tex2DArrayNode35_g40 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode15_g40.g * 255.0 ) );
				float temp_output_24_0_g41 = tex2DArrayNode35_g40.b;
				float4 tex2DNode16_g40 = tex2D( _GrassTint, temp_output_12_0_g40 );
				float temp_output_25_0_g41 = tex2DNode16_g40.a;
				float temp_output_26_0_g41 = tex2DArrayNode32_g40.b;
				float temp_output_41_0_g40 = ( 1.0 - tex2DNode16_g40.a );
				float temp_output_27_0_g41 = temp_output_41_0_g40;
				float temp_output_1_0_g41 = ( max( ( temp_output_24_0_g41 + temp_output_25_0_g41 ) , ( temp_output_26_0_g41 + temp_output_27_0_g41 ) ) - 0.2 );
				float temp_output_21_0_g41 = max( ( ( temp_output_24_0_g41 + temp_output_25_0_g41 ) - temp_output_1_0_g41 ) , 0.0 );
				float temp_output_19_0_g41 = max( ( ( temp_output_26_0_g41 + temp_output_27_0_g41 ) - temp_output_1_0_g41 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch31_g40 = ( ( ( tex2DArrayNode35_g40 * temp_output_21_0_g41 ) + ( tex2DArrayNode32_g40 * temp_output_19_0_g41 ) ) / ( temp_output_21_0_g41 + temp_output_19_0_g41 ) );
				#else
				float4 staticSwitch31_g40 = tex2DArrayNode32_g40;
				#endif
				float temp_output_332_3 = staticSwitch31_g40.b;
				float temp_output_26_0_g72 = temp_output_332_3;
				float temp_output_27_0_g72 = break248.x;
				float temp_output_1_0_g72 = ( max( ( temp_output_24_0_g72 + temp_output_25_0_g72 ) , ( temp_output_26_0_g72 + temp_output_27_0_g72 ) ) - 0.2 );
				float temp_output_21_0_g72 = max( ( ( temp_output_24_0_g72 + temp_output_25_0_g72 ) - temp_output_1_0_g72 ) , 0.0 );
				float4 tex2DArrayNode23_g40 = SAMPLE_TEXTURE2D_ARRAY( _MatTexArray, sampler_MatTexArray, uv_MatTexArray,( tex2DNode6_g40.r * 255.0 ) );
				float temp_output_24_0_g42 = tex2DArrayNode35_g40.b;
				float temp_output_25_0_g42 = tex2DNode16_g40.a;
				float temp_output_26_0_g42 = tex2DArrayNode32_g40.b;
				float temp_output_27_0_g42 = temp_output_41_0_g40;
				float temp_output_1_0_g42 = ( max( ( temp_output_24_0_g42 + temp_output_25_0_g42 ) , ( temp_output_26_0_g42 + temp_output_27_0_g42 ) ) - 0.2 );
				float temp_output_21_0_g42 = max( ( ( temp_output_24_0_g42 + temp_output_25_0_g42 ) - temp_output_1_0_g42 ) , 0.0 );
				float temp_output_19_0_g42 = max( ( ( temp_output_26_0_g42 + temp_output_27_0_g42 ) - temp_output_1_0_g42 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch29_g40 = ( ( ( SAMPLE_TEXTURE2D_ARRAY( _MatTexArray, sampler_MatTexArray, uv_MatTexArray,( tex2DNode15_g40.r * 255.0 ) ) * temp_output_21_0_g42 ) + ( tex2DArrayNode23_g40 * temp_output_19_0_g42 ) ) / ( temp_output_21_0_g42 + temp_output_19_0_g42 ) );
				#else
				float4 staticSwitch29_g40 = tex2DArrayNode23_g40;
				#endif
				float temp_output_19_0_g72 = max( ( ( temp_output_26_0_g72 + temp_output_27_0_g72 ) - temp_output_1_0_g72 ) , 0.0 );
				float temp_output_24_0_g66 = temp_output_334_3;
				float temp_output_25_0_g66 = temp_output_264_0;
				float temp_output_26_0_g66 = temp_output_332_3;
				float temp_output_27_0_g66 = break248.x;
				float temp_output_1_0_g66 = ( max( ( temp_output_24_0_g66 + temp_output_25_0_g66 ) , ( temp_output_26_0_g66 + temp_output_27_0_g66 ) ) - 0.2 );
				float temp_output_21_0_g66 = max( ( ( temp_output_24_0_g66 + temp_output_25_0_g66 ) - temp_output_1_0_g66 ) , 0.0 );
				float temp_output_19_0_g66 = max( ( ( temp_output_26_0_g66 + temp_output_27_0_g66 ) - temp_output_1_0_g66 ) , 0.0 );
				float4 temp_output_304_0 = ( ( ( staticSwitch31_g28 * temp_output_21_0_g66 ) + ( staticSwitch31_g40 * temp_output_19_0_g66 ) ) / ( temp_output_21_0_g66 + temp_output_19_0_g66 ) );
				float temp_output_24_0_g76 = temp_output_304_0.z;
				float temp_output_269_0 = ( 1.0 - break248.y );
				float temp_output_25_0_g76 = temp_output_269_0;
				float2 temp_output_12_0_g32 = ( ( temp_output_94_0 + float2( 0,1 ) ) / appendResult92 );
				float4 tex2DNode6_g32 = tex2D( _Control, temp_output_12_0_g32 );
				float4 tex2DArrayNode32_g32 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode6_g32.g * 255.0 ) );
				float4 tex2DNode15_g32 = tex2D( _GrassControl, temp_output_12_0_g32 );
				float4 tex2DArrayNode35_g32 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode15_g32.g * 255.0 ) );
				float temp_output_24_0_g33 = tex2DArrayNode35_g32.b;
				float4 tex2DNode16_g32 = tex2D( _GrassTint, temp_output_12_0_g32 );
				float temp_output_25_0_g33 = tex2DNode16_g32.a;
				float temp_output_26_0_g33 = tex2DArrayNode32_g32.b;
				float temp_output_41_0_g32 = ( 1.0 - tex2DNode16_g32.a );
				float temp_output_27_0_g33 = temp_output_41_0_g32;
				float temp_output_1_0_g33 = ( max( ( temp_output_24_0_g33 + temp_output_25_0_g33 ) , ( temp_output_26_0_g33 + temp_output_27_0_g33 ) ) - 0.2 );
				float temp_output_21_0_g33 = max( ( ( temp_output_24_0_g33 + temp_output_25_0_g33 ) - temp_output_1_0_g33 ) , 0.0 );
				float temp_output_19_0_g33 = max( ( ( temp_output_26_0_g33 + temp_output_27_0_g33 ) - temp_output_1_0_g33 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch31_g32 = ( ( ( tex2DArrayNode35_g32 * temp_output_21_0_g33 ) + ( tex2DArrayNode32_g32 * temp_output_19_0_g33 ) ) / ( temp_output_21_0_g33 + temp_output_19_0_g33 ) );
				#else
				float4 staticSwitch31_g32 = tex2DArrayNode32_g32;
				#endif
				float temp_output_335_3 = staticSwitch31_g32.b;
				float temp_output_24_0_g67 = temp_output_335_3;
				float temp_output_25_0_g67 = temp_output_264_0;
				float2 temp_output_12_0_g36 = ( ( temp_output_94_0 + float2( 1,1 ) ) / appendResult92 );
				float4 tex2DNode6_g36 = tex2D( _Control, temp_output_12_0_g36 );
				float4 tex2DArrayNode32_g36 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode6_g36.g * 255.0 ) );
				float4 tex2DNode15_g36 = tex2D( _GrassControl, temp_output_12_0_g36 );
				float4 tex2DArrayNode35_g36 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode15_g36.g * 255.0 ) );
				float temp_output_24_0_g37 = tex2DArrayNode35_g36.b;
				float4 tex2DNode16_g36 = tex2D( _GrassTint, temp_output_12_0_g36 );
				float temp_output_25_0_g37 = tex2DNode16_g36.a;
				float temp_output_26_0_g37 = tex2DArrayNode32_g36.b;
				float temp_output_41_0_g36 = ( 1.0 - tex2DNode16_g36.a );
				float temp_output_27_0_g37 = temp_output_41_0_g36;
				float temp_output_1_0_g37 = ( max( ( temp_output_24_0_g37 + temp_output_25_0_g37 ) , ( temp_output_26_0_g37 + temp_output_27_0_g37 ) ) - 0.2 );
				float temp_output_21_0_g37 = max( ( ( temp_output_24_0_g37 + temp_output_25_0_g37 ) - temp_output_1_0_g37 ) , 0.0 );
				float temp_output_19_0_g37 = max( ( ( temp_output_26_0_g37 + temp_output_27_0_g37 ) - temp_output_1_0_g37 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch31_g36 = ( ( ( tex2DArrayNode35_g36 * temp_output_21_0_g37 ) + ( tex2DArrayNode32_g36 * temp_output_19_0_g37 ) ) / ( temp_output_21_0_g37 + temp_output_19_0_g37 ) );
				#else
				float4 staticSwitch31_g36 = tex2DArrayNode32_g36;
				#endif
				float temp_output_333_3 = staticSwitch31_g36.b;
				float temp_output_26_0_g67 = temp_output_333_3;
				float temp_output_27_0_g67 = break248.x;
				float temp_output_1_0_g67 = ( max( ( temp_output_24_0_g67 + temp_output_25_0_g67 ) , ( temp_output_26_0_g67 + temp_output_27_0_g67 ) ) - 0.2 );
				float temp_output_21_0_g67 = max( ( ( temp_output_24_0_g67 + temp_output_25_0_g67 ) - temp_output_1_0_g67 ) , 0.0 );
				float temp_output_19_0_g67 = max( ( ( temp_output_26_0_g67 + temp_output_27_0_g67 ) - temp_output_1_0_g67 ) , 0.0 );
				float4 temp_output_305_0 = ( ( ( staticSwitch31_g32 * temp_output_21_0_g67 ) + ( staticSwitch31_g36 * temp_output_19_0_g67 ) ) / ( temp_output_21_0_g67 + temp_output_19_0_g67 ) );
				float temp_output_26_0_g76 = temp_output_305_0.z;
				float temp_output_27_0_g76 = break248.y;
				float temp_output_1_0_g76 = ( max( ( temp_output_24_0_g76 + temp_output_25_0_g76 ) , ( temp_output_26_0_g76 + temp_output_27_0_g76 ) ) - 0.2 );
				float temp_output_21_0_g76 = max( ( ( temp_output_24_0_g76 + temp_output_25_0_g76 ) - temp_output_1_0_g76 ) , 0.0 );
				float4 tex2DArrayNode23_g32 = SAMPLE_TEXTURE2D_ARRAY( _MatTexArray, sampler_MatTexArray, uv_MatTexArray,( tex2DNode6_g32.r * 255.0 ) );
				float temp_output_24_0_g34 = tex2DArrayNode35_g32.b;
				float temp_output_25_0_g34 = tex2DNode16_g32.a;
				float temp_output_26_0_g34 = tex2DArrayNode32_g32.b;
				float temp_output_27_0_g34 = temp_output_41_0_g32;
				float temp_output_1_0_g34 = ( max( ( temp_output_24_0_g34 + temp_output_25_0_g34 ) , ( temp_output_26_0_g34 + temp_output_27_0_g34 ) ) - 0.2 );
				float temp_output_21_0_g34 = max( ( ( temp_output_24_0_g34 + temp_output_25_0_g34 ) - temp_output_1_0_g34 ) , 0.0 );
				float temp_output_19_0_g34 = max( ( ( temp_output_26_0_g34 + temp_output_27_0_g34 ) - temp_output_1_0_g34 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch29_g32 = ( ( ( SAMPLE_TEXTURE2D_ARRAY( _MatTexArray, sampler_MatTexArray, uv_MatTexArray,( tex2DNode15_g32.r * 255.0 ) ) * temp_output_21_0_g34 ) + ( tex2DArrayNode23_g32 * temp_output_19_0_g34 ) ) / ( temp_output_21_0_g34 + temp_output_19_0_g34 ) );
				#else
				float4 staticSwitch29_g32 = tex2DArrayNode23_g32;
				#endif
				float temp_output_24_0_g71 = temp_output_335_3;
				float temp_output_25_0_g71 = temp_output_264_0;
				float temp_output_26_0_g71 = temp_output_333_3;
				float temp_output_27_0_g71 = break248.x;
				float temp_output_1_0_g71 = ( max( ( temp_output_24_0_g71 + temp_output_25_0_g71 ) , ( temp_output_26_0_g71 + temp_output_27_0_g71 ) ) - 0.2 );
				float temp_output_21_0_g71 = max( ( ( temp_output_24_0_g71 + temp_output_25_0_g71 ) - temp_output_1_0_g71 ) , 0.0 );
				float4 tex2DArrayNode23_g36 = SAMPLE_TEXTURE2D_ARRAY( _MatTexArray, sampler_MatTexArray, uv_MatTexArray,( tex2DNode6_g36.r * 255.0 ) );
				float temp_output_24_0_g38 = tex2DArrayNode35_g36.b;
				float temp_output_25_0_g38 = tex2DNode16_g36.a;
				float temp_output_26_0_g38 = tex2DArrayNode32_g36.b;
				float temp_output_27_0_g38 = temp_output_41_0_g36;
				float temp_output_1_0_g38 = ( max( ( temp_output_24_0_g38 + temp_output_25_0_g38 ) , ( temp_output_26_0_g38 + temp_output_27_0_g38 ) ) - 0.2 );
				float temp_output_21_0_g38 = max( ( ( temp_output_24_0_g38 + temp_output_25_0_g38 ) - temp_output_1_0_g38 ) , 0.0 );
				float temp_output_19_0_g38 = max( ( ( temp_output_26_0_g38 + temp_output_27_0_g38 ) - temp_output_1_0_g38 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch29_g36 = ( ( ( SAMPLE_TEXTURE2D_ARRAY( _MatTexArray, sampler_MatTexArray, uv_MatTexArray,( tex2DNode15_g36.r * 255.0 ) ) * temp_output_21_0_g38 ) + ( tex2DArrayNode23_g36 * temp_output_19_0_g38 ) ) / ( temp_output_21_0_g38 + temp_output_19_0_g38 ) );
				#else
				float4 staticSwitch29_g36 = tex2DArrayNode23_g36;
				#endif
				float temp_output_19_0_g71 = max( ( ( temp_output_26_0_g71 + temp_output_27_0_g71 ) - temp_output_1_0_g71 ) , 0.0 );
				float temp_output_19_0_g76 = max( ( ( temp_output_26_0_g76 + temp_output_27_0_g76 ) - temp_output_1_0_g76 ) , 0.0 );
				float4 tex2DNode14_g28 = tex2D( _Tint, temp_output_12_0_g28 );
				float temp_output_24_0_g31 = tex2DArrayNode35_g28.b;
				float temp_output_25_0_g31 = tex2DNode16_g28.a;
				float temp_output_26_0_g31 = tex2DArrayNode32_g28.b;
				float temp_output_27_0_g31 = temp_output_41_0_g28;
				float temp_output_1_0_g31 = ( max( ( temp_output_24_0_g31 + temp_output_25_0_g31 ) , ( temp_output_26_0_g31 + temp_output_27_0_g31 ) ) - 0.2 );
				float temp_output_21_0_g31 = max( ( ( temp_output_24_0_g31 + temp_output_25_0_g31 ) - temp_output_1_0_g31 ) , 0.0 );
				float temp_output_19_0_g31 = max( ( ( temp_output_26_0_g31 + temp_output_27_0_g31 ) - temp_output_1_0_g31 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch30_g28 = ( ( ( tex2DNode16_g28 * temp_output_21_0_g31 ) + ( tex2DNode14_g28 * temp_output_19_0_g31 ) ) / ( temp_output_21_0_g31 + temp_output_19_0_g31 ) );
				#else
				float4 staticSwitch30_g28 = tex2DNode14_g28;
				#endif
				float temp_output_24_0_g73 = temp_output_334_3;
				float temp_output_25_0_g73 = temp_output_264_0;
				float temp_output_26_0_g73 = temp_output_332_3;
				float temp_output_27_0_g73 = break248.x;
				float temp_output_1_0_g73 = ( max( ( temp_output_24_0_g73 + temp_output_25_0_g73 ) , ( temp_output_26_0_g73 + temp_output_27_0_g73 ) ) - 0.2 );
				float temp_output_21_0_g73 = max( ( ( temp_output_24_0_g73 + temp_output_25_0_g73 ) - temp_output_1_0_g73 ) , 0.0 );
				float4 tex2DNode14_g40 = tex2D( _Tint, temp_output_12_0_g40 );
				float temp_output_24_0_g43 = tex2DArrayNode35_g40.b;
				float temp_output_25_0_g43 = tex2DNode16_g40.a;
				float temp_output_26_0_g43 = tex2DArrayNode32_g40.b;
				float temp_output_27_0_g43 = temp_output_41_0_g40;
				float temp_output_1_0_g43 = ( max( ( temp_output_24_0_g43 + temp_output_25_0_g43 ) , ( temp_output_26_0_g43 + temp_output_27_0_g43 ) ) - 0.2 );
				float temp_output_21_0_g43 = max( ( ( temp_output_24_0_g43 + temp_output_25_0_g43 ) - temp_output_1_0_g43 ) , 0.0 );
				float temp_output_19_0_g43 = max( ( ( temp_output_26_0_g43 + temp_output_27_0_g43 ) - temp_output_1_0_g43 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch30_g40 = ( ( ( tex2DNode16_g40 * temp_output_21_0_g43 ) + ( tex2DNode14_g40 * temp_output_19_0_g43 ) ) / ( temp_output_21_0_g43 + temp_output_19_0_g43 ) );
				#else
				float4 staticSwitch30_g40 = tex2DNode14_g40;
				#endif
				float temp_output_19_0_g73 = max( ( ( temp_output_26_0_g73 + temp_output_27_0_g73 ) - temp_output_1_0_g73 ) , 0.0 );
				float temp_output_24_0_g75 = temp_output_304_0.z;
				float temp_output_25_0_g75 = temp_output_269_0;
				float temp_output_26_0_g75 = temp_output_305_0.z;
				float temp_output_27_0_g75 = break248.y;
				float temp_output_1_0_g75 = ( max( ( temp_output_24_0_g75 + temp_output_25_0_g75 ) , ( temp_output_26_0_g75 + temp_output_27_0_g75 ) ) - 0.2 );
				float temp_output_21_0_g75 = max( ( ( temp_output_24_0_g75 + temp_output_25_0_g75 ) - temp_output_1_0_g75 ) , 0.0 );
				float4 tex2DNode14_g32 = tex2D( _Tint, temp_output_12_0_g32 );
				float temp_output_24_0_g35 = tex2DArrayNode35_g32.b;
				float temp_output_25_0_g35 = tex2DNode16_g32.a;
				float temp_output_26_0_g35 = tex2DArrayNode32_g32.b;
				float temp_output_27_0_g35 = temp_output_41_0_g32;
				float temp_output_1_0_g35 = ( max( ( temp_output_24_0_g35 + temp_output_25_0_g35 ) , ( temp_output_26_0_g35 + temp_output_27_0_g35 ) ) - 0.2 );
				float temp_output_21_0_g35 = max( ( ( temp_output_24_0_g35 + temp_output_25_0_g35 ) - temp_output_1_0_g35 ) , 0.0 );
				float temp_output_19_0_g35 = max( ( ( temp_output_26_0_g35 + temp_output_27_0_g35 ) - temp_output_1_0_g35 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch30_g32 = ( ( ( tex2DNode16_g32 * temp_output_21_0_g35 ) + ( tex2DNode14_g32 * temp_output_19_0_g35 ) ) / ( temp_output_21_0_g35 + temp_output_19_0_g35 ) );
				#else
				float4 staticSwitch30_g32 = tex2DNode14_g32;
				#endif
				float temp_output_24_0_g74 = temp_output_335_3;
				float temp_output_25_0_g74 = temp_output_264_0;
				float temp_output_26_0_g74 = temp_output_333_3;
				float temp_output_27_0_g74 = break248.x;
				float temp_output_1_0_g74 = ( max( ( temp_output_24_0_g74 + temp_output_25_0_g74 ) , ( temp_output_26_0_g74 + temp_output_27_0_g74 ) ) - 0.2 );
				float temp_output_21_0_g74 = max( ( ( temp_output_24_0_g74 + temp_output_25_0_g74 ) - temp_output_1_0_g74 ) , 0.0 );
				float4 tex2DNode14_g36 = tex2D( _Tint, temp_output_12_0_g36 );
				float temp_output_24_0_g39 = tex2DArrayNode35_g36.b;
				float temp_output_25_0_g39 = tex2DNode16_g36.a;
				float temp_output_26_0_g39 = tex2DArrayNode32_g36.b;
				float temp_output_27_0_g39 = temp_output_41_0_g36;
				float temp_output_1_0_g39 = ( max( ( temp_output_24_0_g39 + temp_output_25_0_g39 ) , ( temp_output_26_0_g39 + temp_output_27_0_g39 ) ) - 0.2 );
				float temp_output_21_0_g39 = max( ( ( temp_output_24_0_g39 + temp_output_25_0_g39 ) - temp_output_1_0_g39 ) , 0.0 );
				float temp_output_19_0_g39 = max( ( ( temp_output_26_0_g39 + temp_output_27_0_g39 ) - temp_output_1_0_g39 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch30_g36 = ( ( ( tex2DNode16_g36 * temp_output_21_0_g39 ) + ( tex2DNode14_g36 * temp_output_19_0_g39 ) ) / ( temp_output_21_0_g39 + temp_output_19_0_g39 ) );
				#else
				float4 staticSwitch30_g36 = tex2DNode14_g36;
				#endif
				float temp_output_19_0_g74 = max( ( ( temp_output_26_0_g74 + temp_output_27_0_g74 ) - temp_output_1_0_g74 ) , 0.0 );
				float temp_output_19_0_g75 = max( ( ( temp_output_26_0_g75 + temp_output_27_0_g75 ) - temp_output_1_0_g75 ) , 0.0 );
				float4 ifLocalVar157_g79 = 0;
				UNITY_BRANCH 
				if( temp_output_147_0_g79 > 0.0 )
				ifLocalVar157_g79 = ( tex2DNode133_g79 / tex2DNode133_g79.a );
				else if( temp_output_147_0_g79 < 0.0 )
				ifLocalVar157_g79 = ( ( ( ( ( ( ( staticSwitch29_g28 * temp_output_21_0_g72 ) + ( staticSwitch29_g40 * temp_output_19_0_g72 ) ) / ( temp_output_21_0_g72 + temp_output_19_0_g72 ) ) * temp_output_21_0_g76 ) + ( ( ( ( staticSwitch29_g32 * temp_output_21_0_g71 ) + ( staticSwitch29_g36 * temp_output_19_0_g71 ) ) / ( temp_output_21_0_g71 + temp_output_19_0_g71 ) ) * temp_output_19_0_g76 ) ) / ( temp_output_21_0_g76 + temp_output_19_0_g76 ) ) * ( ( ( ( ( ( staticSwitch30_g28 * temp_output_21_0_g73 ) + ( staticSwitch30_g40 * temp_output_19_0_g73 ) ) / ( temp_output_21_0_g73 + temp_output_19_0_g73 ) ) * temp_output_21_0_g75 ) + ( ( ( ( staticSwitch30_g32 * temp_output_21_0_g74 ) + ( staticSwitch30_g36 * temp_output_19_0_g74 ) ) / ( temp_output_21_0_g74 + temp_output_19_0_g74 ) ) * temp_output_19_0_g75 ) ) / ( temp_output_21_0_g75 + temp_output_19_0_g75 ) ) );
				
				float temp_output_24_0_g80 = temp_output_304_0.z;
				float temp_output_25_0_g80 = temp_output_269_0;
				float temp_output_26_0_g80 = temp_output_305_0.z;
				float temp_output_27_0_g80 = break248.y;
				float temp_output_1_0_g80 = ( max( ( temp_output_24_0_g80 + temp_output_25_0_g80 ) , ( temp_output_26_0_g80 + temp_output_27_0_g80 ) ) - 0.2 );
				float temp_output_21_0_g80 = max( ( ( temp_output_24_0_g80 + temp_output_25_0_g80 ) - temp_output_1_0_g80 ) , 0.0 );
				float temp_output_19_0_g80 = max( ( ( temp_output_26_0_g80 + temp_output_27_0_g80 ) - temp_output_1_0_g80 ) , 0.0 );
				
				surfaceDescription.Albedo = ifLocalVar157_g79.rgb;
				surfaceDescription.Normal = UnpackNormalScale( ( ( ( temp_output_304_0 * temp_output_21_0_g80 ) + ( temp_output_305_0 * temp_output_19_0_g80 ) ) / ( temp_output_21_0_g80 + temp_output_19_0_g80 ) ), 1.0 );
				surfaceDescription.BentNormal = float3( 0, 0, 1 );
				surfaceDescription.CoatMask = 0;
				surfaceDescription.Metallic = ( temp_output_147_0_g79 * _MetalLevel );

				#ifdef _MATERIAL_FEATURE_SPECULAR_COLOR
				surfaceDescription.Specular = 0;
				#endif

				surfaceDescription.Emission = 0;
				surfaceDescription.Smoothness = ( temp_output_147_0_g79 * _Rough );
				surfaceDescription.Occlusion = 1;
				surfaceDescription.Alpha = temp_output_147_0_g79;

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

			#define _DISABLE_SSR_TRANSPARENT 1
			#define _DISABLE_DECALS 1
			#define _DISABLE_SSR 1
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
			float4 _WorldBounds;
			float4 _MatTexArray_ST;
			float4 _Control_TexelSize;
			float4 _ShapeMap_ST;
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
			sampler2D _SpatterTex;
			sampler2D _SpatterNoise;


			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/NormalSurfaceGradient.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Lit/Lit.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/BuiltinUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/MaterialUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Decal/DecalUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Lit/LitDecalData.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderGraphFunctions.hlsl"

			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_FRAG_RELATIVE_WORLD_POS
			#pragma multi_compile __ CONTAMINANTS
			#pragma multi_compile __ GRASS
			#define GRASS


			#if defined(_DOUBLESIDED_ON) && !defined(ASE_NEED_CULLFACE)
				#define ASE_NEED_CULLFACE 1
			#endif

			struct AttributesMesh
			{
				float3 positionOS : POSITION;
				float3 normalOS : NORMAL;
				
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

				float3 ase_worldNormal = TransformObjectToWorldNormal(inputMesh.normalOS);
				outputPackedVaryingsMeshToPS.ase_texcoord1.xyz = ase_worldNormal;
				
				
				//setting value to unused interpolator channels and avoid initialization warnings
				outputPackedVaryingsMeshToPS.ase_texcoord1.w = 0;

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
				float3 ase_worldNormal = packedInput.ase_texcoord1.xyz;
				float dotResult145_g79 = dot( ase_worldNormal , float3( 0,1,0 ) );
				float3 ase_worldPos = GetAbsolutePositionWS( positionRWS );
				float3 break180_g79 = ase_worldPos;
				float2 appendResult102_g79 = (float2(break180_g79.x , break180_g79.z));
				float4 break174_g79 = _WorldBounds;
				float2 appendResult129_g79 = (float2(break174_g79.x , break174_g79.y));
				float2 appendResult130_g79 = (float2(break174_g79.z , break174_g79.w));
				float4 tex2DNode133_g79 = tex2D( _SpatterTex, ( ( appendResult102_g79 - appendResult129_g79 ) / ( appendResult130_g79 - appendResult129_g79 ) ) );
				float4 temp_cast_0 = (tex2DNode133_g79.a).xxxx;
				float4 lerpResult139_g79 = lerp( float4( 1,0,0,0 ) , float4( -1,0,0,0 ) , ( temp_cast_0 - tex2D( _SpatterNoise, appendResult102_g79 ) ));
				float temp_output_147_0_g79 = ( dotResult145_g79 > lerpResult139_g79.r ? 1.0 : 0.0 );
				
				surfaceDescription.Alpha = temp_output_147_0_g79;

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

			#define _DISABLE_SSR_TRANSPARENT 1
			#define _DISABLE_DECALS 1
			#define _DISABLE_SSR 1
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
			float4 _WorldBounds;
			float4 _MatTexArray_ST;
			float4 _Control_TexelSize;
			float4 _ShapeMap_ST;
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

			sampler2D _SpatterTex;
			sampler2D _SpatterNoise;


			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/PickingSpaceTransforms.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/NormalSurfaceGradient.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Lit/Lit.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/BuiltinUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/MaterialUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Decal/DecalUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Lit/LitDecalData.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderGraphFunctions.hlsl"

			#define ASE_NEEDS_VERT_NORMAL
			#define ASE_NEEDS_FRAG_RELATIVE_WORLD_POS
			#pragma multi_compile __ CONTAMINANTS
			#pragma multi_compile __ GRASS
			#define GRASS


			#if defined(_DOUBLESIDED_ON) && !defined(ASE_NEED_CULLFACE)
				#define ASE_NEED_CULLFACE 1
			#endif

			struct AttributesMesh
			{
				float3 positionOS : POSITION;
				float3 normalOS : NORMAL;
				
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

				float3 ase_worldNormal = TransformObjectToWorldNormal(inputMesh.normalOS);
				outputPackedVaryingsMeshToPS.ase_texcoord1.xyz = ase_worldNormal;
				
				
				//setting value to unused interpolator channels and avoid initialization warnings
				outputPackedVaryingsMeshToPS.ase_texcoord1.w = 0;

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
				float3 ase_worldNormal = packedInput.ase_texcoord1.xyz;
				float dotResult145_g79 = dot( ase_worldNormal , float3( 0,1,0 ) );
				float3 ase_worldPos = GetAbsolutePositionWS( positionRWS );
				float3 break180_g79 = ase_worldPos;
				float2 appendResult102_g79 = (float2(break180_g79.x , break180_g79.z));
				float4 break174_g79 = _WorldBounds;
				float2 appendResult129_g79 = (float2(break174_g79.x , break174_g79.y));
				float2 appendResult130_g79 = (float2(break174_g79.z , break174_g79.w));
				float4 tex2DNode133_g79 = tex2D( _SpatterTex, ( ( appendResult102_g79 - appendResult129_g79 ) / ( appendResult130_g79 - appendResult129_g79 ) ) );
				float4 temp_cast_0 = (tex2DNode133_g79.a).xxxx;
				float4 lerpResult139_g79 = lerp( float4( 1,0,0,0 ) , float4( -1,0,0,0 ) , ( temp_cast_0 - tex2D( _SpatterNoise, appendResult102_g79 ) ));
				float temp_output_147_0_g79 = ( dotResult145_g79 > lerpResult139_g79.r ? 1.0 : 0.0 );
				
				surfaceDescription.Alpha = temp_output_147_0_g79;

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

			#define _DISABLE_SSR_TRANSPARENT 1
			#define _DISABLE_DECALS 1
			#define _DISABLE_SSR 1
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
			float4 _WorldBounds;
			float4 _MatTexArray_ST;
			float4 _Control_TexelSize;
			float4 _ShapeMap_ST;
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
			sampler2D _SpatterTex;
			sampler2D _SpatterNoise;


			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/NormalSurfaceGradient.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Lit/Lit.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/BuiltinUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/MaterialUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Decal/DecalUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Lit/LitDecalData.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderGraphFunctions.hlsl"

			#define ASE_NEEDS_FRAG_RELATIVE_WORLD_POS
			#define ASE_NEEDS_FRAG_WORLD_NORMAL
			#pragma multi_compile __ CONTAMINANTS
			#pragma multi_compile __ GRASS
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
				float2 appendResult69 = (float2(ase_worldPos.x , ase_worldPos.z));
				float2 appendResult70 = (float2(_WorldBounds.x , _WorldBounds.y));
				float2 appendResult72 = (float2(_WorldBounds.z , _WorldBounds.w));
				float2 appendResult92 = (float2(_Control_TexelSize.z , _Control_TexelSize.w));
				float2 temp_output_90_0 = ( ( ( appendResult69 - appendResult70 ) / ( appendResult72 - appendResult70 ) ) * appendResult92 );
				float2 temp_output_94_0 = floor( ( ( temp_output_90_0 + float2( 0.5,0.5 ) ) - float2( 0.5,0.5 ) ) );
				float2 temp_output_12_0_g28 = ( ( temp_output_94_0 + float2( 0,0 ) ) / appendResult92 );
				float4 tex2DNode6_g28 = tex2D( _Control, temp_output_12_0_g28 );
				float4 tex2DArrayNode32_g28 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode6_g28.g * 255.0 ) );
				float4 tex2DNode15_g28 = tex2D( _GrassControl, temp_output_12_0_g28 );
				float4 tex2DArrayNode35_g28 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode15_g28.g * 255.0 ) );
				float temp_output_24_0_g29 = tex2DArrayNode35_g28.b;
				float4 tex2DNode16_g28 = tex2D( _GrassTint, temp_output_12_0_g28 );
				float temp_output_25_0_g29 = tex2DNode16_g28.a;
				float temp_output_26_0_g29 = tex2DArrayNode32_g28.b;
				float temp_output_41_0_g28 = ( 1.0 - tex2DNode16_g28.a );
				float temp_output_27_0_g29 = temp_output_41_0_g28;
				float temp_output_1_0_g29 = ( max( ( temp_output_24_0_g29 + temp_output_25_0_g29 ) , ( temp_output_26_0_g29 + temp_output_27_0_g29 ) ) - 0.2 );
				float temp_output_21_0_g29 = max( ( ( temp_output_24_0_g29 + temp_output_25_0_g29 ) - temp_output_1_0_g29 ) , 0.0 );
				float temp_output_19_0_g29 = max( ( ( temp_output_26_0_g29 + temp_output_27_0_g29 ) - temp_output_1_0_g29 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch31_g28 = ( ( ( tex2DArrayNode35_g28 * temp_output_21_0_g29 ) + ( tex2DArrayNode32_g28 * temp_output_19_0_g29 ) ) / ( temp_output_21_0_g29 + temp_output_19_0_g29 ) );
				#else
				float4 staticSwitch31_g28 = tex2DArrayNode32_g28;
				#endif
				float temp_output_334_3 = staticSwitch31_g28.b;
				float temp_output_24_0_g66 = temp_output_334_3;
				float2 break248 = ( temp_output_90_0 - temp_output_94_0 );
				float temp_output_264_0 = ( 1.0 - break248.x );
				float temp_output_25_0_g66 = temp_output_264_0;
				float2 temp_output_12_0_g40 = ( ( temp_output_94_0 + float2( 1,0 ) ) / appendResult92 );
				float4 tex2DNode6_g40 = tex2D( _Control, temp_output_12_0_g40 );
				float4 tex2DArrayNode32_g40 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode6_g40.g * 255.0 ) );
				float4 tex2DNode15_g40 = tex2D( _GrassControl, temp_output_12_0_g40 );
				float4 tex2DArrayNode35_g40 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode15_g40.g * 255.0 ) );
				float temp_output_24_0_g41 = tex2DArrayNode35_g40.b;
				float4 tex2DNode16_g40 = tex2D( _GrassTint, temp_output_12_0_g40 );
				float temp_output_25_0_g41 = tex2DNode16_g40.a;
				float temp_output_26_0_g41 = tex2DArrayNode32_g40.b;
				float temp_output_41_0_g40 = ( 1.0 - tex2DNode16_g40.a );
				float temp_output_27_0_g41 = temp_output_41_0_g40;
				float temp_output_1_0_g41 = ( max( ( temp_output_24_0_g41 + temp_output_25_0_g41 ) , ( temp_output_26_0_g41 + temp_output_27_0_g41 ) ) - 0.2 );
				float temp_output_21_0_g41 = max( ( ( temp_output_24_0_g41 + temp_output_25_0_g41 ) - temp_output_1_0_g41 ) , 0.0 );
				float temp_output_19_0_g41 = max( ( ( temp_output_26_0_g41 + temp_output_27_0_g41 ) - temp_output_1_0_g41 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch31_g40 = ( ( ( tex2DArrayNode35_g40 * temp_output_21_0_g41 ) + ( tex2DArrayNode32_g40 * temp_output_19_0_g41 ) ) / ( temp_output_21_0_g41 + temp_output_19_0_g41 ) );
				#else
				float4 staticSwitch31_g40 = tex2DArrayNode32_g40;
				#endif
				float temp_output_332_3 = staticSwitch31_g40.b;
				float temp_output_26_0_g66 = temp_output_332_3;
				float temp_output_27_0_g66 = break248.x;
				float temp_output_1_0_g66 = ( max( ( temp_output_24_0_g66 + temp_output_25_0_g66 ) , ( temp_output_26_0_g66 + temp_output_27_0_g66 ) ) - 0.2 );
				float temp_output_21_0_g66 = max( ( ( temp_output_24_0_g66 + temp_output_25_0_g66 ) - temp_output_1_0_g66 ) , 0.0 );
				float temp_output_19_0_g66 = max( ( ( temp_output_26_0_g66 + temp_output_27_0_g66 ) - temp_output_1_0_g66 ) , 0.0 );
				float4 temp_output_304_0 = ( ( ( staticSwitch31_g28 * temp_output_21_0_g66 ) + ( staticSwitch31_g40 * temp_output_19_0_g66 ) ) / ( temp_output_21_0_g66 + temp_output_19_0_g66 ) );
				float temp_output_24_0_g80 = temp_output_304_0.z;
				float temp_output_269_0 = ( 1.0 - break248.y );
				float temp_output_25_0_g80 = temp_output_269_0;
				float2 temp_output_12_0_g32 = ( ( temp_output_94_0 + float2( 0,1 ) ) / appendResult92 );
				float4 tex2DNode6_g32 = tex2D( _Control, temp_output_12_0_g32 );
				float4 tex2DArrayNode32_g32 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode6_g32.g * 255.0 ) );
				float4 tex2DNode15_g32 = tex2D( _GrassControl, temp_output_12_0_g32 );
				float4 tex2DArrayNode35_g32 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode15_g32.g * 255.0 ) );
				float temp_output_24_0_g33 = tex2DArrayNode35_g32.b;
				float4 tex2DNode16_g32 = tex2D( _GrassTint, temp_output_12_0_g32 );
				float temp_output_25_0_g33 = tex2DNode16_g32.a;
				float temp_output_26_0_g33 = tex2DArrayNode32_g32.b;
				float temp_output_41_0_g32 = ( 1.0 - tex2DNode16_g32.a );
				float temp_output_27_0_g33 = temp_output_41_0_g32;
				float temp_output_1_0_g33 = ( max( ( temp_output_24_0_g33 + temp_output_25_0_g33 ) , ( temp_output_26_0_g33 + temp_output_27_0_g33 ) ) - 0.2 );
				float temp_output_21_0_g33 = max( ( ( temp_output_24_0_g33 + temp_output_25_0_g33 ) - temp_output_1_0_g33 ) , 0.0 );
				float temp_output_19_0_g33 = max( ( ( temp_output_26_0_g33 + temp_output_27_0_g33 ) - temp_output_1_0_g33 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch31_g32 = ( ( ( tex2DArrayNode35_g32 * temp_output_21_0_g33 ) + ( tex2DArrayNode32_g32 * temp_output_19_0_g33 ) ) / ( temp_output_21_0_g33 + temp_output_19_0_g33 ) );
				#else
				float4 staticSwitch31_g32 = tex2DArrayNode32_g32;
				#endif
				float temp_output_335_3 = staticSwitch31_g32.b;
				float temp_output_24_0_g67 = temp_output_335_3;
				float temp_output_25_0_g67 = temp_output_264_0;
				float2 temp_output_12_0_g36 = ( ( temp_output_94_0 + float2( 1,1 ) ) / appendResult92 );
				float4 tex2DNode6_g36 = tex2D( _Control, temp_output_12_0_g36 );
				float4 tex2DArrayNode32_g36 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode6_g36.g * 255.0 ) );
				float4 tex2DNode15_g36 = tex2D( _GrassControl, temp_output_12_0_g36 );
				float4 tex2DArrayNode35_g36 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode15_g36.g * 255.0 ) );
				float temp_output_24_0_g37 = tex2DArrayNode35_g36.b;
				float4 tex2DNode16_g36 = tex2D( _GrassTint, temp_output_12_0_g36 );
				float temp_output_25_0_g37 = tex2DNode16_g36.a;
				float temp_output_26_0_g37 = tex2DArrayNode32_g36.b;
				float temp_output_41_0_g36 = ( 1.0 - tex2DNode16_g36.a );
				float temp_output_27_0_g37 = temp_output_41_0_g36;
				float temp_output_1_0_g37 = ( max( ( temp_output_24_0_g37 + temp_output_25_0_g37 ) , ( temp_output_26_0_g37 + temp_output_27_0_g37 ) ) - 0.2 );
				float temp_output_21_0_g37 = max( ( ( temp_output_24_0_g37 + temp_output_25_0_g37 ) - temp_output_1_0_g37 ) , 0.0 );
				float temp_output_19_0_g37 = max( ( ( temp_output_26_0_g37 + temp_output_27_0_g37 ) - temp_output_1_0_g37 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch31_g36 = ( ( ( tex2DArrayNode35_g36 * temp_output_21_0_g37 ) + ( tex2DArrayNode32_g36 * temp_output_19_0_g37 ) ) / ( temp_output_21_0_g37 + temp_output_19_0_g37 ) );
				#else
				float4 staticSwitch31_g36 = tex2DArrayNode32_g36;
				#endif
				float temp_output_333_3 = staticSwitch31_g36.b;
				float temp_output_26_0_g67 = temp_output_333_3;
				float temp_output_27_0_g67 = break248.x;
				float temp_output_1_0_g67 = ( max( ( temp_output_24_0_g67 + temp_output_25_0_g67 ) , ( temp_output_26_0_g67 + temp_output_27_0_g67 ) ) - 0.2 );
				float temp_output_21_0_g67 = max( ( ( temp_output_24_0_g67 + temp_output_25_0_g67 ) - temp_output_1_0_g67 ) , 0.0 );
				float temp_output_19_0_g67 = max( ( ( temp_output_26_0_g67 + temp_output_27_0_g67 ) - temp_output_1_0_g67 ) , 0.0 );
				float4 temp_output_305_0 = ( ( ( staticSwitch31_g32 * temp_output_21_0_g67 ) + ( staticSwitch31_g36 * temp_output_19_0_g67 ) ) / ( temp_output_21_0_g67 + temp_output_19_0_g67 ) );
				float temp_output_26_0_g80 = temp_output_305_0.z;
				float temp_output_27_0_g80 = break248.y;
				float temp_output_1_0_g80 = ( max( ( temp_output_24_0_g80 + temp_output_25_0_g80 ) , ( temp_output_26_0_g80 + temp_output_27_0_g80 ) ) - 0.2 );
				float temp_output_21_0_g80 = max( ( ( temp_output_24_0_g80 + temp_output_25_0_g80 ) - temp_output_1_0_g80 ) , 0.0 );
				float temp_output_19_0_g80 = max( ( ( temp_output_26_0_g80 + temp_output_27_0_g80 ) - temp_output_1_0_g80 ) , 0.0 );
				
				float dotResult145_g79 = dot( normalWS , float3( 0,1,0 ) );
				float3 break180_g79 = ase_worldPos;
				float2 appendResult102_g79 = (float2(break180_g79.x , break180_g79.z));
				float4 break174_g79 = _WorldBounds;
				float2 appendResult129_g79 = (float2(break174_g79.x , break174_g79.y));
				float2 appendResult130_g79 = (float2(break174_g79.z , break174_g79.w));
				float4 tex2DNode133_g79 = tex2D( _SpatterTex, ( ( appendResult102_g79 - appendResult129_g79 ) / ( appendResult130_g79 - appendResult129_g79 ) ) );
				float4 temp_cast_16 = (tex2DNode133_g79.a).xxxx;
				float4 lerpResult139_g79 = lerp( float4( 1,0,0,0 ) , float4( -1,0,0,0 ) , ( temp_cast_16 - tex2D( _SpatterNoise, appendResult102_g79 ) ));
				float temp_output_147_0_g79 = ( dotResult145_g79 > lerpResult139_g79.r ? 1.0 : 0.0 );
				
				surfaceDescription.Normal = UnpackNormalScale( ( ( ( temp_output_304_0 * temp_output_21_0_g80 ) + ( temp_output_305_0 * temp_output_19_0_g80 ) ) / ( temp_output_21_0_g80 + temp_output_19_0_g80 ) ), 1.0 );
				surfaceDescription.Smoothness = ( temp_output_147_0_g79 * _Rough );
				surfaceDescription.Alpha = temp_output_147_0_g79;

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

			#define _DISABLE_SSR_TRANSPARENT 1
			#define _DISABLE_DECALS 1
			#define _DISABLE_SSR 1
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
			float4 _WorldBounds;
			float4 _MatTexArray_ST;
			float4 _Control_TexelSize;
			float4 _ShapeMap_ST;
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
			sampler2D _SpatterTex;
			sampler2D _SpatterNoise;


			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/NormalSurfaceGradient.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Lit/Lit.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/BuiltinUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/MaterialUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Decal/DecalUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Lit/LitDecalData.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderGraphFunctions.hlsl"

			#define ASE_NEEDS_VERT_NORMAL
			#pragma multi_compile __ CONTAMINANTS
			#pragma multi_compile __ GRASS
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
				float4 ase_texcoord5 : TEXCOORD5;
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
				
				float3 ase_worldNormal = TransformObjectToWorldNormal(inputMesh.normalOS);
				outputPackedVaryingsMeshToPS.ase_texcoord5.xyz = ase_worldNormal;
				
				outputPackedVaryingsMeshToPS.ase_texcoord3.xy = inputMesh.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				outputPackedVaryingsMeshToPS.ase_texcoord3.zw = 0;
				outputPackedVaryingsMeshToPS.ase_texcoord4.w = 0;
				outputPackedVaryingsMeshToPS.ase_texcoord5.w = 0;

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
				float2 appendResult69 = (float2(ase_worldPos.x , ase_worldPos.z));
				float2 appendResult70 = (float2(_WorldBounds.x , _WorldBounds.y));
				float2 appendResult72 = (float2(_WorldBounds.z , _WorldBounds.w));
				float2 appendResult92 = (float2(_Control_TexelSize.z , _Control_TexelSize.w));
				float2 temp_output_90_0 = ( ( ( appendResult69 - appendResult70 ) / ( appendResult72 - appendResult70 ) ) * appendResult92 );
				float2 temp_output_94_0 = floor( ( ( temp_output_90_0 + float2( 0.5,0.5 ) ) - float2( 0.5,0.5 ) ) );
				float2 temp_output_12_0_g28 = ( ( temp_output_94_0 + float2( 0,0 ) ) / appendResult92 );
				float4 tex2DNode6_g28 = tex2D( _Control, temp_output_12_0_g28 );
				float4 tex2DArrayNode32_g28 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode6_g28.g * 255.0 ) );
				float4 tex2DNode15_g28 = tex2D( _GrassControl, temp_output_12_0_g28 );
				float4 tex2DArrayNode35_g28 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode15_g28.g * 255.0 ) );
				float temp_output_24_0_g29 = tex2DArrayNode35_g28.b;
				float4 tex2DNode16_g28 = tex2D( _GrassTint, temp_output_12_0_g28 );
				float temp_output_25_0_g29 = tex2DNode16_g28.a;
				float temp_output_26_0_g29 = tex2DArrayNode32_g28.b;
				float temp_output_41_0_g28 = ( 1.0 - tex2DNode16_g28.a );
				float temp_output_27_0_g29 = temp_output_41_0_g28;
				float temp_output_1_0_g29 = ( max( ( temp_output_24_0_g29 + temp_output_25_0_g29 ) , ( temp_output_26_0_g29 + temp_output_27_0_g29 ) ) - 0.2 );
				float temp_output_21_0_g29 = max( ( ( temp_output_24_0_g29 + temp_output_25_0_g29 ) - temp_output_1_0_g29 ) , 0.0 );
				float temp_output_19_0_g29 = max( ( ( temp_output_26_0_g29 + temp_output_27_0_g29 ) - temp_output_1_0_g29 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch31_g28 = ( ( ( tex2DArrayNode35_g28 * temp_output_21_0_g29 ) + ( tex2DArrayNode32_g28 * temp_output_19_0_g29 ) ) / ( temp_output_21_0_g29 + temp_output_19_0_g29 ) );
				#else
				float4 staticSwitch31_g28 = tex2DArrayNode32_g28;
				#endif
				float temp_output_334_3 = staticSwitch31_g28.b;
				float temp_output_24_0_g66 = temp_output_334_3;
				float2 break248 = ( temp_output_90_0 - temp_output_94_0 );
				float temp_output_264_0 = ( 1.0 - break248.x );
				float temp_output_25_0_g66 = temp_output_264_0;
				float2 temp_output_12_0_g40 = ( ( temp_output_94_0 + float2( 1,0 ) ) / appendResult92 );
				float4 tex2DNode6_g40 = tex2D( _Control, temp_output_12_0_g40 );
				float4 tex2DArrayNode32_g40 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode6_g40.g * 255.0 ) );
				float4 tex2DNode15_g40 = tex2D( _GrassControl, temp_output_12_0_g40 );
				float4 tex2DArrayNode35_g40 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode15_g40.g * 255.0 ) );
				float temp_output_24_0_g41 = tex2DArrayNode35_g40.b;
				float4 tex2DNode16_g40 = tex2D( _GrassTint, temp_output_12_0_g40 );
				float temp_output_25_0_g41 = tex2DNode16_g40.a;
				float temp_output_26_0_g41 = tex2DArrayNode32_g40.b;
				float temp_output_41_0_g40 = ( 1.0 - tex2DNode16_g40.a );
				float temp_output_27_0_g41 = temp_output_41_0_g40;
				float temp_output_1_0_g41 = ( max( ( temp_output_24_0_g41 + temp_output_25_0_g41 ) , ( temp_output_26_0_g41 + temp_output_27_0_g41 ) ) - 0.2 );
				float temp_output_21_0_g41 = max( ( ( temp_output_24_0_g41 + temp_output_25_0_g41 ) - temp_output_1_0_g41 ) , 0.0 );
				float temp_output_19_0_g41 = max( ( ( temp_output_26_0_g41 + temp_output_27_0_g41 ) - temp_output_1_0_g41 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch31_g40 = ( ( ( tex2DArrayNode35_g40 * temp_output_21_0_g41 ) + ( tex2DArrayNode32_g40 * temp_output_19_0_g41 ) ) / ( temp_output_21_0_g41 + temp_output_19_0_g41 ) );
				#else
				float4 staticSwitch31_g40 = tex2DArrayNode32_g40;
				#endif
				float temp_output_332_3 = staticSwitch31_g40.b;
				float temp_output_26_0_g66 = temp_output_332_3;
				float temp_output_27_0_g66 = break248.x;
				float temp_output_1_0_g66 = ( max( ( temp_output_24_0_g66 + temp_output_25_0_g66 ) , ( temp_output_26_0_g66 + temp_output_27_0_g66 ) ) - 0.2 );
				float temp_output_21_0_g66 = max( ( ( temp_output_24_0_g66 + temp_output_25_0_g66 ) - temp_output_1_0_g66 ) , 0.0 );
				float temp_output_19_0_g66 = max( ( ( temp_output_26_0_g66 + temp_output_27_0_g66 ) - temp_output_1_0_g66 ) , 0.0 );
				float4 temp_output_304_0 = ( ( ( staticSwitch31_g28 * temp_output_21_0_g66 ) + ( staticSwitch31_g40 * temp_output_19_0_g66 ) ) / ( temp_output_21_0_g66 + temp_output_19_0_g66 ) );
				float temp_output_24_0_g80 = temp_output_304_0.z;
				float temp_output_269_0 = ( 1.0 - break248.y );
				float temp_output_25_0_g80 = temp_output_269_0;
				float2 temp_output_12_0_g32 = ( ( temp_output_94_0 + float2( 0,1 ) ) / appendResult92 );
				float4 tex2DNode6_g32 = tex2D( _Control, temp_output_12_0_g32 );
				float4 tex2DArrayNode32_g32 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode6_g32.g * 255.0 ) );
				float4 tex2DNode15_g32 = tex2D( _GrassControl, temp_output_12_0_g32 );
				float4 tex2DArrayNode35_g32 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode15_g32.g * 255.0 ) );
				float temp_output_24_0_g33 = tex2DArrayNode35_g32.b;
				float4 tex2DNode16_g32 = tex2D( _GrassTint, temp_output_12_0_g32 );
				float temp_output_25_0_g33 = tex2DNode16_g32.a;
				float temp_output_26_0_g33 = tex2DArrayNode32_g32.b;
				float temp_output_41_0_g32 = ( 1.0 - tex2DNode16_g32.a );
				float temp_output_27_0_g33 = temp_output_41_0_g32;
				float temp_output_1_0_g33 = ( max( ( temp_output_24_0_g33 + temp_output_25_0_g33 ) , ( temp_output_26_0_g33 + temp_output_27_0_g33 ) ) - 0.2 );
				float temp_output_21_0_g33 = max( ( ( temp_output_24_0_g33 + temp_output_25_0_g33 ) - temp_output_1_0_g33 ) , 0.0 );
				float temp_output_19_0_g33 = max( ( ( temp_output_26_0_g33 + temp_output_27_0_g33 ) - temp_output_1_0_g33 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch31_g32 = ( ( ( tex2DArrayNode35_g32 * temp_output_21_0_g33 ) + ( tex2DArrayNode32_g32 * temp_output_19_0_g33 ) ) / ( temp_output_21_0_g33 + temp_output_19_0_g33 ) );
				#else
				float4 staticSwitch31_g32 = tex2DArrayNode32_g32;
				#endif
				float temp_output_335_3 = staticSwitch31_g32.b;
				float temp_output_24_0_g67 = temp_output_335_3;
				float temp_output_25_0_g67 = temp_output_264_0;
				float2 temp_output_12_0_g36 = ( ( temp_output_94_0 + float2( 1,1 ) ) / appendResult92 );
				float4 tex2DNode6_g36 = tex2D( _Control, temp_output_12_0_g36 );
				float4 tex2DArrayNode32_g36 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode6_g36.g * 255.0 ) );
				float4 tex2DNode15_g36 = tex2D( _GrassControl, temp_output_12_0_g36 );
				float4 tex2DArrayNode35_g36 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode15_g36.g * 255.0 ) );
				float temp_output_24_0_g37 = tex2DArrayNode35_g36.b;
				float4 tex2DNode16_g36 = tex2D( _GrassTint, temp_output_12_0_g36 );
				float temp_output_25_0_g37 = tex2DNode16_g36.a;
				float temp_output_26_0_g37 = tex2DArrayNode32_g36.b;
				float temp_output_41_0_g36 = ( 1.0 - tex2DNode16_g36.a );
				float temp_output_27_0_g37 = temp_output_41_0_g36;
				float temp_output_1_0_g37 = ( max( ( temp_output_24_0_g37 + temp_output_25_0_g37 ) , ( temp_output_26_0_g37 + temp_output_27_0_g37 ) ) - 0.2 );
				float temp_output_21_0_g37 = max( ( ( temp_output_24_0_g37 + temp_output_25_0_g37 ) - temp_output_1_0_g37 ) , 0.0 );
				float temp_output_19_0_g37 = max( ( ( temp_output_26_0_g37 + temp_output_27_0_g37 ) - temp_output_1_0_g37 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch31_g36 = ( ( ( tex2DArrayNode35_g36 * temp_output_21_0_g37 ) + ( tex2DArrayNode32_g36 * temp_output_19_0_g37 ) ) / ( temp_output_21_0_g37 + temp_output_19_0_g37 ) );
				#else
				float4 staticSwitch31_g36 = tex2DArrayNode32_g36;
				#endif
				float temp_output_333_3 = staticSwitch31_g36.b;
				float temp_output_26_0_g67 = temp_output_333_3;
				float temp_output_27_0_g67 = break248.x;
				float temp_output_1_0_g67 = ( max( ( temp_output_24_0_g67 + temp_output_25_0_g67 ) , ( temp_output_26_0_g67 + temp_output_27_0_g67 ) ) - 0.2 );
				float temp_output_21_0_g67 = max( ( ( temp_output_24_0_g67 + temp_output_25_0_g67 ) - temp_output_1_0_g67 ) , 0.0 );
				float temp_output_19_0_g67 = max( ( ( temp_output_26_0_g67 + temp_output_27_0_g67 ) - temp_output_1_0_g67 ) , 0.0 );
				float4 temp_output_305_0 = ( ( ( staticSwitch31_g32 * temp_output_21_0_g67 ) + ( staticSwitch31_g36 * temp_output_19_0_g67 ) ) / ( temp_output_21_0_g67 + temp_output_19_0_g67 ) );
				float temp_output_26_0_g80 = temp_output_305_0.z;
				float temp_output_27_0_g80 = break248.y;
				float temp_output_1_0_g80 = ( max( ( temp_output_24_0_g80 + temp_output_25_0_g80 ) , ( temp_output_26_0_g80 + temp_output_27_0_g80 ) ) - 0.2 );
				float temp_output_21_0_g80 = max( ( ( temp_output_24_0_g80 + temp_output_25_0_g80 ) - temp_output_1_0_g80 ) , 0.0 );
				float temp_output_19_0_g80 = max( ( ( temp_output_26_0_g80 + temp_output_27_0_g80 ) - temp_output_1_0_g80 ) , 0.0 );
				
				float3 ase_worldNormal = packedInput.ase_texcoord5.xyz;
				float dotResult145_g79 = dot( ase_worldNormal , float3( 0,1,0 ) );
				float3 break180_g79 = ase_worldPos;
				float2 appendResult102_g79 = (float2(break180_g79.x , break180_g79.z));
				float4 break174_g79 = _WorldBounds;
				float2 appendResult129_g79 = (float2(break174_g79.x , break174_g79.y));
				float2 appendResult130_g79 = (float2(break174_g79.z , break174_g79.w));
				float4 tex2DNode133_g79 = tex2D( _SpatterTex, ( ( appendResult102_g79 - appendResult129_g79 ) / ( appendResult130_g79 - appendResult129_g79 ) ) );
				float4 temp_cast_16 = (tex2DNode133_g79.a).xxxx;
				float4 lerpResult139_g79 = lerp( float4( 1,0,0,0 ) , float4( -1,0,0,0 ) , ( temp_cast_16 - tex2D( _SpatterNoise, appendResult102_g79 ) ));
				float temp_output_147_0_g79 = ( dotResult145_g79 > lerpResult139_g79.r ? 1.0 : 0.0 );
				
				surfaceDescription.Normal = UnpackNormalScale( ( ( ( temp_output_304_0 * temp_output_21_0_g80 ) + ( temp_output_305_0 * temp_output_19_0_g80 ) ) / ( temp_output_21_0_g80 + temp_output_19_0_g80 ) ), 1.0 );
				surfaceDescription.Smoothness = ( temp_output_147_0_g79 * _Rough );
				surfaceDescription.Alpha = temp_output_147_0_g79;

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

			#define _DISABLE_SSR_TRANSPARENT 1
			#define _DISABLE_DECALS 1
			#define _DISABLE_SSR 1
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
			float4 _WorldBounds;
			float4 _MatTexArray_ST;
			float4 _Control_TexelSize;
			float4 _ShapeMap_ST;
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
			sampler2D _SpatterTex;
			sampler2D _SpatterNoise;
			TEXTURE2D_ARRAY(_MatTexArray);
			sampler2D _Control;
			SAMPLER(sampler_MatTexArray);
			sampler2D _GrassControl;
			TEXTURE2D_ARRAY(_ShapeMap);
			SAMPLER(sampler_ShapeMap);
			sampler2D _GrassTint;
			sampler2D _Tint;


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

			#define ASE_NEEDS_FRAG_WORLD_NORMAL
			#define ASE_NEEDS_FRAG_RELATIVE_WORLD_POS
			#pragma multi_compile __ CONTAMINANTS
			#pragma multi_compile __ GRASS
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
				float dotResult145_g79 = dot( normalWS , float3( 0,1,0 ) );
				float3 ase_worldPos = GetAbsolutePositionWS( positionRWS );
				float3 break180_g79 = ase_worldPos;
				float2 appendResult102_g79 = (float2(break180_g79.x , break180_g79.z));
				float4 break174_g79 = _WorldBounds;
				float2 appendResult129_g79 = (float2(break174_g79.x , break174_g79.y));
				float2 appendResult130_g79 = (float2(break174_g79.z , break174_g79.w));
				float4 tex2DNode133_g79 = tex2D( _SpatterTex, ( ( appendResult102_g79 - appendResult129_g79 ) / ( appendResult130_g79 - appendResult129_g79 ) ) );
				float4 temp_cast_0 = (tex2DNode133_g79.a).xxxx;
				float4 lerpResult139_g79 = lerp( float4( 1,0,0,0 ) , float4( -1,0,0,0 ) , ( temp_cast_0 - tex2D( _SpatterNoise, appendResult102_g79 ) ));
				float temp_output_147_0_g79 = ( dotResult145_g79 > lerpResult139_g79.r ? 1.0 : 0.0 );
				float2 uv_MatTexArray = packedInput.ase_texcoord7.xy;
				float2 appendResult69 = (float2(ase_worldPos.x , ase_worldPos.z));
				float2 appendResult70 = (float2(_WorldBounds.x , _WorldBounds.y));
				float2 appendResult72 = (float2(_WorldBounds.z , _WorldBounds.w));
				float2 appendResult92 = (float2(_Control_TexelSize.z , _Control_TexelSize.w));
				float2 temp_output_90_0 = ( ( ( appendResult69 - appendResult70 ) / ( appendResult72 - appendResult70 ) ) * appendResult92 );
				float2 temp_output_94_0 = floor( ( ( temp_output_90_0 + float2( 0.5,0.5 ) ) - float2( 0.5,0.5 ) ) );
				float2 temp_output_12_0_g28 = ( ( temp_output_94_0 + float2( 0,0 ) ) / appendResult92 );
				float4 tex2DNode6_g28 = tex2D( _Control, temp_output_12_0_g28 );
				float4 tex2DArrayNode23_g28 = SAMPLE_TEXTURE2D_ARRAY( _MatTexArray, sampler_MatTexArray, uv_MatTexArray,( tex2DNode6_g28.r * 255.0 ) );
				float4 tex2DNode15_g28 = tex2D( _GrassControl, temp_output_12_0_g28 );
				float2 uv_ShapeMap = packedInput.ase_texcoord7.xy;
				float4 tex2DArrayNode35_g28 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode15_g28.g * 255.0 ) );
				float temp_output_24_0_g30 = tex2DArrayNode35_g28.b;
				float4 tex2DNode16_g28 = tex2D( _GrassTint, temp_output_12_0_g28 );
				float temp_output_25_0_g30 = tex2DNode16_g28.a;
				float4 tex2DArrayNode32_g28 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode6_g28.g * 255.0 ) );
				float temp_output_26_0_g30 = tex2DArrayNode32_g28.b;
				float temp_output_41_0_g28 = ( 1.0 - tex2DNode16_g28.a );
				float temp_output_27_0_g30 = temp_output_41_0_g28;
				float temp_output_1_0_g30 = ( max( ( temp_output_24_0_g30 + temp_output_25_0_g30 ) , ( temp_output_26_0_g30 + temp_output_27_0_g30 ) ) - 0.2 );
				float temp_output_21_0_g30 = max( ( ( temp_output_24_0_g30 + temp_output_25_0_g30 ) - temp_output_1_0_g30 ) , 0.0 );
				float temp_output_19_0_g30 = max( ( ( temp_output_26_0_g30 + temp_output_27_0_g30 ) - temp_output_1_0_g30 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch29_g28 = ( ( ( SAMPLE_TEXTURE2D_ARRAY( _MatTexArray, sampler_MatTexArray, uv_MatTexArray,( tex2DNode15_g28.r * 255.0 ) ) * temp_output_21_0_g30 ) + ( tex2DArrayNode23_g28 * temp_output_19_0_g30 ) ) / ( temp_output_21_0_g30 + temp_output_19_0_g30 ) );
				#else
				float4 staticSwitch29_g28 = tex2DArrayNode23_g28;
				#endif
				float temp_output_24_0_g29 = tex2DArrayNode35_g28.b;
				float temp_output_25_0_g29 = tex2DNode16_g28.a;
				float temp_output_26_0_g29 = tex2DArrayNode32_g28.b;
				float temp_output_27_0_g29 = temp_output_41_0_g28;
				float temp_output_1_0_g29 = ( max( ( temp_output_24_0_g29 + temp_output_25_0_g29 ) , ( temp_output_26_0_g29 + temp_output_27_0_g29 ) ) - 0.2 );
				float temp_output_21_0_g29 = max( ( ( temp_output_24_0_g29 + temp_output_25_0_g29 ) - temp_output_1_0_g29 ) , 0.0 );
				float temp_output_19_0_g29 = max( ( ( temp_output_26_0_g29 + temp_output_27_0_g29 ) - temp_output_1_0_g29 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch31_g28 = ( ( ( tex2DArrayNode35_g28 * temp_output_21_0_g29 ) + ( tex2DArrayNode32_g28 * temp_output_19_0_g29 ) ) / ( temp_output_21_0_g29 + temp_output_19_0_g29 ) );
				#else
				float4 staticSwitch31_g28 = tex2DArrayNode32_g28;
				#endif
				float temp_output_334_3 = staticSwitch31_g28.b;
				float temp_output_24_0_g72 = temp_output_334_3;
				float2 break248 = ( temp_output_90_0 - temp_output_94_0 );
				float temp_output_264_0 = ( 1.0 - break248.x );
				float temp_output_25_0_g72 = temp_output_264_0;
				float2 temp_output_12_0_g40 = ( ( temp_output_94_0 + float2( 1,0 ) ) / appendResult92 );
				float4 tex2DNode6_g40 = tex2D( _Control, temp_output_12_0_g40 );
				float4 tex2DArrayNode32_g40 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode6_g40.g * 255.0 ) );
				float4 tex2DNode15_g40 = tex2D( _GrassControl, temp_output_12_0_g40 );
				float4 tex2DArrayNode35_g40 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode15_g40.g * 255.0 ) );
				float temp_output_24_0_g41 = tex2DArrayNode35_g40.b;
				float4 tex2DNode16_g40 = tex2D( _GrassTint, temp_output_12_0_g40 );
				float temp_output_25_0_g41 = tex2DNode16_g40.a;
				float temp_output_26_0_g41 = tex2DArrayNode32_g40.b;
				float temp_output_41_0_g40 = ( 1.0 - tex2DNode16_g40.a );
				float temp_output_27_0_g41 = temp_output_41_0_g40;
				float temp_output_1_0_g41 = ( max( ( temp_output_24_0_g41 + temp_output_25_0_g41 ) , ( temp_output_26_0_g41 + temp_output_27_0_g41 ) ) - 0.2 );
				float temp_output_21_0_g41 = max( ( ( temp_output_24_0_g41 + temp_output_25_0_g41 ) - temp_output_1_0_g41 ) , 0.0 );
				float temp_output_19_0_g41 = max( ( ( temp_output_26_0_g41 + temp_output_27_0_g41 ) - temp_output_1_0_g41 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch31_g40 = ( ( ( tex2DArrayNode35_g40 * temp_output_21_0_g41 ) + ( tex2DArrayNode32_g40 * temp_output_19_0_g41 ) ) / ( temp_output_21_0_g41 + temp_output_19_0_g41 ) );
				#else
				float4 staticSwitch31_g40 = tex2DArrayNode32_g40;
				#endif
				float temp_output_332_3 = staticSwitch31_g40.b;
				float temp_output_26_0_g72 = temp_output_332_3;
				float temp_output_27_0_g72 = break248.x;
				float temp_output_1_0_g72 = ( max( ( temp_output_24_0_g72 + temp_output_25_0_g72 ) , ( temp_output_26_0_g72 + temp_output_27_0_g72 ) ) - 0.2 );
				float temp_output_21_0_g72 = max( ( ( temp_output_24_0_g72 + temp_output_25_0_g72 ) - temp_output_1_0_g72 ) , 0.0 );
				float4 tex2DArrayNode23_g40 = SAMPLE_TEXTURE2D_ARRAY( _MatTexArray, sampler_MatTexArray, uv_MatTexArray,( tex2DNode6_g40.r * 255.0 ) );
				float temp_output_24_0_g42 = tex2DArrayNode35_g40.b;
				float temp_output_25_0_g42 = tex2DNode16_g40.a;
				float temp_output_26_0_g42 = tex2DArrayNode32_g40.b;
				float temp_output_27_0_g42 = temp_output_41_0_g40;
				float temp_output_1_0_g42 = ( max( ( temp_output_24_0_g42 + temp_output_25_0_g42 ) , ( temp_output_26_0_g42 + temp_output_27_0_g42 ) ) - 0.2 );
				float temp_output_21_0_g42 = max( ( ( temp_output_24_0_g42 + temp_output_25_0_g42 ) - temp_output_1_0_g42 ) , 0.0 );
				float temp_output_19_0_g42 = max( ( ( temp_output_26_0_g42 + temp_output_27_0_g42 ) - temp_output_1_0_g42 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch29_g40 = ( ( ( SAMPLE_TEXTURE2D_ARRAY( _MatTexArray, sampler_MatTexArray, uv_MatTexArray,( tex2DNode15_g40.r * 255.0 ) ) * temp_output_21_0_g42 ) + ( tex2DArrayNode23_g40 * temp_output_19_0_g42 ) ) / ( temp_output_21_0_g42 + temp_output_19_0_g42 ) );
				#else
				float4 staticSwitch29_g40 = tex2DArrayNode23_g40;
				#endif
				float temp_output_19_0_g72 = max( ( ( temp_output_26_0_g72 + temp_output_27_0_g72 ) - temp_output_1_0_g72 ) , 0.0 );
				float temp_output_24_0_g66 = temp_output_334_3;
				float temp_output_25_0_g66 = temp_output_264_0;
				float temp_output_26_0_g66 = temp_output_332_3;
				float temp_output_27_0_g66 = break248.x;
				float temp_output_1_0_g66 = ( max( ( temp_output_24_0_g66 + temp_output_25_0_g66 ) , ( temp_output_26_0_g66 + temp_output_27_0_g66 ) ) - 0.2 );
				float temp_output_21_0_g66 = max( ( ( temp_output_24_0_g66 + temp_output_25_0_g66 ) - temp_output_1_0_g66 ) , 0.0 );
				float temp_output_19_0_g66 = max( ( ( temp_output_26_0_g66 + temp_output_27_0_g66 ) - temp_output_1_0_g66 ) , 0.0 );
				float4 temp_output_304_0 = ( ( ( staticSwitch31_g28 * temp_output_21_0_g66 ) + ( staticSwitch31_g40 * temp_output_19_0_g66 ) ) / ( temp_output_21_0_g66 + temp_output_19_0_g66 ) );
				float temp_output_24_0_g76 = temp_output_304_0.z;
				float temp_output_269_0 = ( 1.0 - break248.y );
				float temp_output_25_0_g76 = temp_output_269_0;
				float2 temp_output_12_0_g32 = ( ( temp_output_94_0 + float2( 0,1 ) ) / appendResult92 );
				float4 tex2DNode6_g32 = tex2D( _Control, temp_output_12_0_g32 );
				float4 tex2DArrayNode32_g32 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode6_g32.g * 255.0 ) );
				float4 tex2DNode15_g32 = tex2D( _GrassControl, temp_output_12_0_g32 );
				float4 tex2DArrayNode35_g32 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode15_g32.g * 255.0 ) );
				float temp_output_24_0_g33 = tex2DArrayNode35_g32.b;
				float4 tex2DNode16_g32 = tex2D( _GrassTint, temp_output_12_0_g32 );
				float temp_output_25_0_g33 = tex2DNode16_g32.a;
				float temp_output_26_0_g33 = tex2DArrayNode32_g32.b;
				float temp_output_41_0_g32 = ( 1.0 - tex2DNode16_g32.a );
				float temp_output_27_0_g33 = temp_output_41_0_g32;
				float temp_output_1_0_g33 = ( max( ( temp_output_24_0_g33 + temp_output_25_0_g33 ) , ( temp_output_26_0_g33 + temp_output_27_0_g33 ) ) - 0.2 );
				float temp_output_21_0_g33 = max( ( ( temp_output_24_0_g33 + temp_output_25_0_g33 ) - temp_output_1_0_g33 ) , 0.0 );
				float temp_output_19_0_g33 = max( ( ( temp_output_26_0_g33 + temp_output_27_0_g33 ) - temp_output_1_0_g33 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch31_g32 = ( ( ( tex2DArrayNode35_g32 * temp_output_21_0_g33 ) + ( tex2DArrayNode32_g32 * temp_output_19_0_g33 ) ) / ( temp_output_21_0_g33 + temp_output_19_0_g33 ) );
				#else
				float4 staticSwitch31_g32 = tex2DArrayNode32_g32;
				#endif
				float temp_output_335_3 = staticSwitch31_g32.b;
				float temp_output_24_0_g67 = temp_output_335_3;
				float temp_output_25_0_g67 = temp_output_264_0;
				float2 temp_output_12_0_g36 = ( ( temp_output_94_0 + float2( 1,1 ) ) / appendResult92 );
				float4 tex2DNode6_g36 = tex2D( _Control, temp_output_12_0_g36 );
				float4 tex2DArrayNode32_g36 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode6_g36.g * 255.0 ) );
				float4 tex2DNode15_g36 = tex2D( _GrassControl, temp_output_12_0_g36 );
				float4 tex2DArrayNode35_g36 = SAMPLE_TEXTURE2D_ARRAY( _ShapeMap, sampler_ShapeMap, uv_ShapeMap,( tex2DNode15_g36.g * 255.0 ) );
				float temp_output_24_0_g37 = tex2DArrayNode35_g36.b;
				float4 tex2DNode16_g36 = tex2D( _GrassTint, temp_output_12_0_g36 );
				float temp_output_25_0_g37 = tex2DNode16_g36.a;
				float temp_output_26_0_g37 = tex2DArrayNode32_g36.b;
				float temp_output_41_0_g36 = ( 1.0 - tex2DNode16_g36.a );
				float temp_output_27_0_g37 = temp_output_41_0_g36;
				float temp_output_1_0_g37 = ( max( ( temp_output_24_0_g37 + temp_output_25_0_g37 ) , ( temp_output_26_0_g37 + temp_output_27_0_g37 ) ) - 0.2 );
				float temp_output_21_0_g37 = max( ( ( temp_output_24_0_g37 + temp_output_25_0_g37 ) - temp_output_1_0_g37 ) , 0.0 );
				float temp_output_19_0_g37 = max( ( ( temp_output_26_0_g37 + temp_output_27_0_g37 ) - temp_output_1_0_g37 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch31_g36 = ( ( ( tex2DArrayNode35_g36 * temp_output_21_0_g37 ) + ( tex2DArrayNode32_g36 * temp_output_19_0_g37 ) ) / ( temp_output_21_0_g37 + temp_output_19_0_g37 ) );
				#else
				float4 staticSwitch31_g36 = tex2DArrayNode32_g36;
				#endif
				float temp_output_333_3 = staticSwitch31_g36.b;
				float temp_output_26_0_g67 = temp_output_333_3;
				float temp_output_27_0_g67 = break248.x;
				float temp_output_1_0_g67 = ( max( ( temp_output_24_0_g67 + temp_output_25_0_g67 ) , ( temp_output_26_0_g67 + temp_output_27_0_g67 ) ) - 0.2 );
				float temp_output_21_0_g67 = max( ( ( temp_output_24_0_g67 + temp_output_25_0_g67 ) - temp_output_1_0_g67 ) , 0.0 );
				float temp_output_19_0_g67 = max( ( ( temp_output_26_0_g67 + temp_output_27_0_g67 ) - temp_output_1_0_g67 ) , 0.0 );
				float4 temp_output_305_0 = ( ( ( staticSwitch31_g32 * temp_output_21_0_g67 ) + ( staticSwitch31_g36 * temp_output_19_0_g67 ) ) / ( temp_output_21_0_g67 + temp_output_19_0_g67 ) );
				float temp_output_26_0_g76 = temp_output_305_0.z;
				float temp_output_27_0_g76 = break248.y;
				float temp_output_1_0_g76 = ( max( ( temp_output_24_0_g76 + temp_output_25_0_g76 ) , ( temp_output_26_0_g76 + temp_output_27_0_g76 ) ) - 0.2 );
				float temp_output_21_0_g76 = max( ( ( temp_output_24_0_g76 + temp_output_25_0_g76 ) - temp_output_1_0_g76 ) , 0.0 );
				float4 tex2DArrayNode23_g32 = SAMPLE_TEXTURE2D_ARRAY( _MatTexArray, sampler_MatTexArray, uv_MatTexArray,( tex2DNode6_g32.r * 255.0 ) );
				float temp_output_24_0_g34 = tex2DArrayNode35_g32.b;
				float temp_output_25_0_g34 = tex2DNode16_g32.a;
				float temp_output_26_0_g34 = tex2DArrayNode32_g32.b;
				float temp_output_27_0_g34 = temp_output_41_0_g32;
				float temp_output_1_0_g34 = ( max( ( temp_output_24_0_g34 + temp_output_25_0_g34 ) , ( temp_output_26_0_g34 + temp_output_27_0_g34 ) ) - 0.2 );
				float temp_output_21_0_g34 = max( ( ( temp_output_24_0_g34 + temp_output_25_0_g34 ) - temp_output_1_0_g34 ) , 0.0 );
				float temp_output_19_0_g34 = max( ( ( temp_output_26_0_g34 + temp_output_27_0_g34 ) - temp_output_1_0_g34 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch29_g32 = ( ( ( SAMPLE_TEXTURE2D_ARRAY( _MatTexArray, sampler_MatTexArray, uv_MatTexArray,( tex2DNode15_g32.r * 255.0 ) ) * temp_output_21_0_g34 ) + ( tex2DArrayNode23_g32 * temp_output_19_0_g34 ) ) / ( temp_output_21_0_g34 + temp_output_19_0_g34 ) );
				#else
				float4 staticSwitch29_g32 = tex2DArrayNode23_g32;
				#endif
				float temp_output_24_0_g71 = temp_output_335_3;
				float temp_output_25_0_g71 = temp_output_264_0;
				float temp_output_26_0_g71 = temp_output_333_3;
				float temp_output_27_0_g71 = break248.x;
				float temp_output_1_0_g71 = ( max( ( temp_output_24_0_g71 + temp_output_25_0_g71 ) , ( temp_output_26_0_g71 + temp_output_27_0_g71 ) ) - 0.2 );
				float temp_output_21_0_g71 = max( ( ( temp_output_24_0_g71 + temp_output_25_0_g71 ) - temp_output_1_0_g71 ) , 0.0 );
				float4 tex2DArrayNode23_g36 = SAMPLE_TEXTURE2D_ARRAY( _MatTexArray, sampler_MatTexArray, uv_MatTexArray,( tex2DNode6_g36.r * 255.0 ) );
				float temp_output_24_0_g38 = tex2DArrayNode35_g36.b;
				float temp_output_25_0_g38 = tex2DNode16_g36.a;
				float temp_output_26_0_g38 = tex2DArrayNode32_g36.b;
				float temp_output_27_0_g38 = temp_output_41_0_g36;
				float temp_output_1_0_g38 = ( max( ( temp_output_24_0_g38 + temp_output_25_0_g38 ) , ( temp_output_26_0_g38 + temp_output_27_0_g38 ) ) - 0.2 );
				float temp_output_21_0_g38 = max( ( ( temp_output_24_0_g38 + temp_output_25_0_g38 ) - temp_output_1_0_g38 ) , 0.0 );
				float temp_output_19_0_g38 = max( ( ( temp_output_26_0_g38 + temp_output_27_0_g38 ) - temp_output_1_0_g38 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch29_g36 = ( ( ( SAMPLE_TEXTURE2D_ARRAY( _MatTexArray, sampler_MatTexArray, uv_MatTexArray,( tex2DNode15_g36.r * 255.0 ) ) * temp_output_21_0_g38 ) + ( tex2DArrayNode23_g36 * temp_output_19_0_g38 ) ) / ( temp_output_21_0_g38 + temp_output_19_0_g38 ) );
				#else
				float4 staticSwitch29_g36 = tex2DArrayNode23_g36;
				#endif
				float temp_output_19_0_g71 = max( ( ( temp_output_26_0_g71 + temp_output_27_0_g71 ) - temp_output_1_0_g71 ) , 0.0 );
				float temp_output_19_0_g76 = max( ( ( temp_output_26_0_g76 + temp_output_27_0_g76 ) - temp_output_1_0_g76 ) , 0.0 );
				float4 tex2DNode14_g28 = tex2D( _Tint, temp_output_12_0_g28 );
				float temp_output_24_0_g31 = tex2DArrayNode35_g28.b;
				float temp_output_25_0_g31 = tex2DNode16_g28.a;
				float temp_output_26_0_g31 = tex2DArrayNode32_g28.b;
				float temp_output_27_0_g31 = temp_output_41_0_g28;
				float temp_output_1_0_g31 = ( max( ( temp_output_24_0_g31 + temp_output_25_0_g31 ) , ( temp_output_26_0_g31 + temp_output_27_0_g31 ) ) - 0.2 );
				float temp_output_21_0_g31 = max( ( ( temp_output_24_0_g31 + temp_output_25_0_g31 ) - temp_output_1_0_g31 ) , 0.0 );
				float temp_output_19_0_g31 = max( ( ( temp_output_26_0_g31 + temp_output_27_0_g31 ) - temp_output_1_0_g31 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch30_g28 = ( ( ( tex2DNode16_g28 * temp_output_21_0_g31 ) + ( tex2DNode14_g28 * temp_output_19_0_g31 ) ) / ( temp_output_21_0_g31 + temp_output_19_0_g31 ) );
				#else
				float4 staticSwitch30_g28 = tex2DNode14_g28;
				#endif
				float temp_output_24_0_g73 = temp_output_334_3;
				float temp_output_25_0_g73 = temp_output_264_0;
				float temp_output_26_0_g73 = temp_output_332_3;
				float temp_output_27_0_g73 = break248.x;
				float temp_output_1_0_g73 = ( max( ( temp_output_24_0_g73 + temp_output_25_0_g73 ) , ( temp_output_26_0_g73 + temp_output_27_0_g73 ) ) - 0.2 );
				float temp_output_21_0_g73 = max( ( ( temp_output_24_0_g73 + temp_output_25_0_g73 ) - temp_output_1_0_g73 ) , 0.0 );
				float4 tex2DNode14_g40 = tex2D( _Tint, temp_output_12_0_g40 );
				float temp_output_24_0_g43 = tex2DArrayNode35_g40.b;
				float temp_output_25_0_g43 = tex2DNode16_g40.a;
				float temp_output_26_0_g43 = tex2DArrayNode32_g40.b;
				float temp_output_27_0_g43 = temp_output_41_0_g40;
				float temp_output_1_0_g43 = ( max( ( temp_output_24_0_g43 + temp_output_25_0_g43 ) , ( temp_output_26_0_g43 + temp_output_27_0_g43 ) ) - 0.2 );
				float temp_output_21_0_g43 = max( ( ( temp_output_24_0_g43 + temp_output_25_0_g43 ) - temp_output_1_0_g43 ) , 0.0 );
				float temp_output_19_0_g43 = max( ( ( temp_output_26_0_g43 + temp_output_27_0_g43 ) - temp_output_1_0_g43 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch30_g40 = ( ( ( tex2DNode16_g40 * temp_output_21_0_g43 ) + ( tex2DNode14_g40 * temp_output_19_0_g43 ) ) / ( temp_output_21_0_g43 + temp_output_19_0_g43 ) );
				#else
				float4 staticSwitch30_g40 = tex2DNode14_g40;
				#endif
				float temp_output_19_0_g73 = max( ( ( temp_output_26_0_g73 + temp_output_27_0_g73 ) - temp_output_1_0_g73 ) , 0.0 );
				float temp_output_24_0_g75 = temp_output_304_0.z;
				float temp_output_25_0_g75 = temp_output_269_0;
				float temp_output_26_0_g75 = temp_output_305_0.z;
				float temp_output_27_0_g75 = break248.y;
				float temp_output_1_0_g75 = ( max( ( temp_output_24_0_g75 + temp_output_25_0_g75 ) , ( temp_output_26_0_g75 + temp_output_27_0_g75 ) ) - 0.2 );
				float temp_output_21_0_g75 = max( ( ( temp_output_24_0_g75 + temp_output_25_0_g75 ) - temp_output_1_0_g75 ) , 0.0 );
				float4 tex2DNode14_g32 = tex2D( _Tint, temp_output_12_0_g32 );
				float temp_output_24_0_g35 = tex2DArrayNode35_g32.b;
				float temp_output_25_0_g35 = tex2DNode16_g32.a;
				float temp_output_26_0_g35 = tex2DArrayNode32_g32.b;
				float temp_output_27_0_g35 = temp_output_41_0_g32;
				float temp_output_1_0_g35 = ( max( ( temp_output_24_0_g35 + temp_output_25_0_g35 ) , ( temp_output_26_0_g35 + temp_output_27_0_g35 ) ) - 0.2 );
				float temp_output_21_0_g35 = max( ( ( temp_output_24_0_g35 + temp_output_25_0_g35 ) - temp_output_1_0_g35 ) , 0.0 );
				float temp_output_19_0_g35 = max( ( ( temp_output_26_0_g35 + temp_output_27_0_g35 ) - temp_output_1_0_g35 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch30_g32 = ( ( ( tex2DNode16_g32 * temp_output_21_0_g35 ) + ( tex2DNode14_g32 * temp_output_19_0_g35 ) ) / ( temp_output_21_0_g35 + temp_output_19_0_g35 ) );
				#else
				float4 staticSwitch30_g32 = tex2DNode14_g32;
				#endif
				float temp_output_24_0_g74 = temp_output_335_3;
				float temp_output_25_0_g74 = temp_output_264_0;
				float temp_output_26_0_g74 = temp_output_333_3;
				float temp_output_27_0_g74 = break248.x;
				float temp_output_1_0_g74 = ( max( ( temp_output_24_0_g74 + temp_output_25_0_g74 ) , ( temp_output_26_0_g74 + temp_output_27_0_g74 ) ) - 0.2 );
				float temp_output_21_0_g74 = max( ( ( temp_output_24_0_g74 + temp_output_25_0_g74 ) - temp_output_1_0_g74 ) , 0.0 );
				float4 tex2DNode14_g36 = tex2D( _Tint, temp_output_12_0_g36 );
				float temp_output_24_0_g39 = tex2DArrayNode35_g36.b;
				float temp_output_25_0_g39 = tex2DNode16_g36.a;
				float temp_output_26_0_g39 = tex2DArrayNode32_g36.b;
				float temp_output_27_0_g39 = temp_output_41_0_g36;
				float temp_output_1_0_g39 = ( max( ( temp_output_24_0_g39 + temp_output_25_0_g39 ) , ( temp_output_26_0_g39 + temp_output_27_0_g39 ) ) - 0.2 );
				float temp_output_21_0_g39 = max( ( ( temp_output_24_0_g39 + temp_output_25_0_g39 ) - temp_output_1_0_g39 ) , 0.0 );
				float temp_output_19_0_g39 = max( ( ( temp_output_26_0_g39 + temp_output_27_0_g39 ) - temp_output_1_0_g39 ) , 0.0 );
				#ifdef GRASS
				float4 staticSwitch30_g36 = ( ( ( tex2DNode16_g36 * temp_output_21_0_g39 ) + ( tex2DNode14_g36 * temp_output_19_0_g39 ) ) / ( temp_output_21_0_g39 + temp_output_19_0_g39 ) );
				#else
				float4 staticSwitch30_g36 = tex2DNode14_g36;
				#endif
				float temp_output_19_0_g74 = max( ( ( temp_output_26_0_g74 + temp_output_27_0_g74 ) - temp_output_1_0_g74 ) , 0.0 );
				float temp_output_19_0_g75 = max( ( ( temp_output_26_0_g75 + temp_output_27_0_g75 ) - temp_output_1_0_g75 ) , 0.0 );
				float4 ifLocalVar157_g79 = 0;
				UNITY_BRANCH 
				if( temp_output_147_0_g79 > 0.0 )
				ifLocalVar157_g79 = ( tex2DNode133_g79 / tex2DNode133_g79.a );
				else if( temp_output_147_0_g79 < 0.0 )
				ifLocalVar157_g79 = ( ( ( ( ( ( ( staticSwitch29_g28 * temp_output_21_0_g72 ) + ( staticSwitch29_g40 * temp_output_19_0_g72 ) ) / ( temp_output_21_0_g72 + temp_output_19_0_g72 ) ) * temp_output_21_0_g76 ) + ( ( ( ( staticSwitch29_g32 * temp_output_21_0_g71 ) + ( staticSwitch29_g36 * temp_output_19_0_g71 ) ) / ( temp_output_21_0_g71 + temp_output_19_0_g71 ) ) * temp_output_19_0_g76 ) ) / ( temp_output_21_0_g76 + temp_output_19_0_g76 ) ) * ( ( ( ( ( ( staticSwitch30_g28 * temp_output_21_0_g73 ) + ( staticSwitch30_g40 * temp_output_19_0_g73 ) ) / ( temp_output_21_0_g73 + temp_output_19_0_g73 ) ) * temp_output_21_0_g75 ) + ( ( ( ( staticSwitch30_g32 * temp_output_21_0_g74 ) + ( staticSwitch30_g36 * temp_output_19_0_g74 ) ) / ( temp_output_21_0_g74 + temp_output_19_0_g74 ) ) * temp_output_19_0_g75 ) ) / ( temp_output_21_0_g75 + temp_output_19_0_g75 ) ) );
				
				float temp_output_24_0_g80 = temp_output_304_0.z;
				float temp_output_25_0_g80 = temp_output_269_0;
				float temp_output_26_0_g80 = temp_output_305_0.z;
				float temp_output_27_0_g80 = break248.y;
				float temp_output_1_0_g80 = ( max( ( temp_output_24_0_g80 + temp_output_25_0_g80 ) , ( temp_output_26_0_g80 + temp_output_27_0_g80 ) ) - 0.2 );
				float temp_output_21_0_g80 = max( ( ( temp_output_24_0_g80 + temp_output_25_0_g80 ) - temp_output_1_0_g80 ) , 0.0 );
				float temp_output_19_0_g80 = max( ( ( temp_output_26_0_g80 + temp_output_27_0_g80 ) - temp_output_1_0_g80 ) , 0.0 );
				
				surfaceDescription.Albedo = ifLocalVar157_g79.rgb;
				surfaceDescription.Normal = UnpackNormalScale( ( ( ( temp_output_304_0 * temp_output_21_0_g80 ) + ( temp_output_305_0 * temp_output_19_0_g80 ) ) / ( temp_output_21_0_g80 + temp_output_19_0_g80 ) ), 1.0 );
				surfaceDescription.BentNormal = float3( 0, 0, 1 );
				surfaceDescription.CoatMask = 0;
				surfaceDescription.Metallic = ( temp_output_147_0_g79 * _MetalLevel );

				#ifdef _MATERIAL_FEATURE_SPECULAR_COLOR
				surfaceDescription.Specular = 0;
				#endif

				surfaceDescription.Emission = 0;
				surfaceDescription.Smoothness = ( temp_output_147_0_g79 * _Rough );
				surfaceDescription.Occlusion = 1;
				surfaceDescription.Alpha = temp_output_147_0_g79;

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
        
			#define _DISABLE_SSR_TRANSPARENT 1
			#define _DISABLE_DECALS 1
			#define _DISABLE_SSR 1
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
			float4 _WorldBounds;
			float4 _MatTexArray_ST;
			float4 _Control_TexelSize;
			float4 _ShapeMap_ST;
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
			sampler2D _SpatterTex;
			sampler2D _SpatterNoise;

        
            
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/PickingSpaceTransforms.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Unlit/Unlit.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/BuiltinUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/MaterialUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderGraphFunctions.hlsl"
        

			#pragma multi_compile __ CONTAMINANTS
			#pragma multi_compile __ GRASS
			#define GRASS


			struct VertexInput
			{
				float3 positionOS : POSITION;
				float3 normalOS : NORMAL;
				float4 tangentOS : TANGENT;
				
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct VertexOutput
			{
				float4 positionCS : SV_POSITION;
				float3 normalWS : TEXCOORD0;
				float4 tangentWS : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
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
				
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord2.w = 0;
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
				float dotResult145_g79 = dot( packedInput.normalWS , float3( 0,1,0 ) );
				float3 ase_worldPos = packedInput.ase_texcoord2.xyz;
				float3 break180_g79 = ase_worldPos;
				float2 appendResult102_g79 = (float2(break180_g79.x , break180_g79.z));
				float4 break174_g79 = _WorldBounds;
				float2 appendResult129_g79 = (float2(break174_g79.x , break174_g79.y));
				float2 appendResult130_g79 = (float2(break174_g79.z , break174_g79.w));
				float4 tex2DNode133_g79 = tex2D( _SpatterTex, ( ( appendResult102_g79 - appendResult129_g79 ) / ( appendResult130_g79 - appendResult129_g79 ) ) );
				float4 temp_cast_0 = (tex2DNode133_g79.a).xxxx;
				float4 lerpResult139_g79 = lerp( float4( 1,0,0,0 ) , float4( -1,0,0,0 ) , ( temp_cast_0 - tex2D( _SpatterNoise, appendResult102_g79 ) ));
				float temp_output_147_0_g79 = ( dotResult145_g79 > lerpResult139_g79.r ? 1.0 : 0.0 );
				
				surfaceDescription.Alpha = temp_output_147_0_g79;
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
	
	Fallback "HDRP/Lit"
}
/*ASEBEGIN
Version=18935
-7;24;2560;1347;2931.216;1808.622;1;True;True
Node;AmplifyShaderEditor.CommentaryNode;76;-2259.77,-871.5117;Inherit;False;613;372;World UV;8;71;73;68;72;70;69;66;22;;1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldPosInputsNode;66;-2251.77,-827.5116;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.CommentaryNode;93;-2306.484,-1144.117;Inherit;False;703.2;252.2002;ControlCords;4;14;26;92;90;;1,1,1,1;0;0
Node;AmplifyShaderEditor.Vector4Node;22;-2251.904,-672.6466;Inherit;False;Property;_WorldBounds;WorldBounds;9;0;Create;True;0;0;0;False;0;False;0,0,1,1;0,0,1,1;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;69;-2079.77,-829.5117;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;72;-2084.77,-597.5121;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;70;-2085.77,-694.512;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;14;-2285.373,-1103.788;Inherit;True;Property;_Control;Control;2;0;Create;True;0;0;0;True;0;False;None;None;False;black;LockedToTexture2D;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.TexelSizeNode;26;-2068.823,-1101.684;Inherit;False;14;1;0;SAMPLER2D;;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;73;-1933.77,-642.512;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;68;-1921.771,-776.5118;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;71;-1776.471,-722.9119;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;92;-1881.233,-1102.093;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;90;-1742.884,-1105.117;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;95;-1520.606,-1114.636;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;99;-1542.106,-1160.753;Inherit;False;415.7999;163.6;ControlCordsBase;2;97;94;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;97;-1403.207,-1115.636;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;101;-1085.407,-1158.537;Inherit;False;277.1;155.1;ControlFraction;2;248;100;;1,1,1,1;0;0
Node;AmplifyShaderEditor.FloorOpNode;94;-1256.99,-1106.276;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;100;-1077.507,-1112.137;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.BreakToComponentsNode;248;-938.2512,-1111.159;Inherit;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.Vector2Node;314;-1350.716,-1379.621;Inherit;False;Constant;_Offset;Offset;13;0;Create;True;0;0;0;False;0;False;1,1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;317;-1359.716,-2248.621;Inherit;False;Constant;_Offset3;Offset;13;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;315;-1350.716,-1643.621;Inherit;False;Constant;_Offset1;Offset;13;0;Create;True;0;0;0;False;0;False;0,1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;316;-1359.716,-1939.621;Inherit;False;Constant;_Offset2;Offset;13;0;Create;True;0;0;0;False;0;False;1,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TexturePropertyNode;195;-2055.247,-2229.665;Inherit;True;Property;_MatTexArray;MatTexArray;11;0;Create;True;0;0;0;True;0;False;507d82b7de2c9404cb0a71f3cdb0331f;507d82b7de2c9404cb0a71f3cdb0331f;False;white;LockedToTexture2DArray;Texture2DArray;-1;0;2;SAMPLER2DARRAY;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.TexturePropertyNode;13;-2055.246,-1461.057;Inherit;True;Property;_Tint;Tint;0;1;[PerRendererData];Create;True;0;0;0;True;0;False;None;None;False;black;LockedToTexture2D;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.TexturePropertyNode;16;-2055.963,-1656.519;Inherit;True;Property;_GrassControl;GrassControl;3;1;[PerRendererData];Create;True;0;0;0;True;0;False;None;None;False;black;LockedToTexture2D;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.TexturePropertyNode;15;-2049.512,-1843.752;Inherit;True;Property;_GrassTint;GrassTint;1;1;[PerRendererData];Create;True;0;0;0;True;0;False;None;None;False;black;LockedToTexture2D;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.TexturePropertyNode;214;-2050.438,-2035.258;Inherit;True;Property;_ShapeMap;Shape Texture Splat;12;0;Create;False;0;0;0;True;0;False;9eea02fcbbb4cc24aaf9072337d93cd4;9eea02fcbbb4cc24aaf9072337d93cd4;False;bump;LockedToTexture2DArray;Texture2DArray;-1;0;2;SAMPLER2DARRAY;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.FunctionNode;335;-1091.777,-1743.874;Inherit;False;Sample Terrain Tile;-1;;32;68db4cbbb16712044b9cfe83a0a98414;0;9;5;SAMPLER2D;0;False;7;FLOAT2;0,0;False;8;FLOAT2;0,0;False;9;FLOAT2;0,0;False;10;SAMPLER2D;0;False;13;SAMPLER2D;0;False;17;SAMPLER2D;0;False;22;SAMPLER2DARRAY;0;False;33;SAMPLER2DARRAY;0;False;4;COLOR;0;COLOR;1;COLOR;2;FLOAT;3
Node;AmplifyShaderEditor.FunctionNode;333;-1091.176,-1467.372;Inherit;False;Sample Terrain Tile;-1;;36;68db4cbbb16712044b9cfe83a0a98414;0;9;5;SAMPLER2D;0;False;7;FLOAT2;0,0;False;8;FLOAT2;0,0;False;9;FLOAT2;0,0;False;10;SAMPLER2D;0;False;13;SAMPLER2D;0;False;17;SAMPLER2D;0;False;22;SAMPLER2DARRAY;0;False;33;SAMPLER2DARRAY;0;False;4;COLOR;0;COLOR;1;COLOR;2;FLOAT;3
Node;AmplifyShaderEditor.OneMinusNode;264;-769.7535,-1132.226;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;332;-1085.028,-2018.956;Inherit;False;Sample Terrain Tile;-1;;40;68db4cbbb16712044b9cfe83a0a98414;0;9;5;SAMPLER2D;0;False;7;FLOAT2;0,0;False;8;FLOAT2;0,0;False;9;FLOAT2;0,0;False;10;SAMPLER2D;0;False;13;SAMPLER2D;0;False;17;SAMPLER2D;0;False;22;SAMPLER2DARRAY;0;False;33;SAMPLER2DARRAY;0;False;4;COLOR;0;COLOR;1;COLOR;2;FLOAT;3
Node;AmplifyShaderEditor.FunctionNode;334;-1085.61,-2292.275;Inherit;False;Sample Terrain Tile;-1;;28;68db4cbbb16712044b9cfe83a0a98414;0;9;5;SAMPLER2D;0;False;7;FLOAT2;0,0;False;8;FLOAT2;0,0;False;9;FLOAT2;0,0;False;10;SAMPLER2D;0;False;13;SAMPLER2D;0;False;17;SAMPLER2D;0;False;22;SAMPLER2DARRAY;0;False;33;SAMPLER2DARRAY;0;False;4;COLOR;0;COLOR;1;COLOR;2;FLOAT;3
Node;AmplifyShaderEditor.FunctionNode;305;-566.4338,-1441.056;Inherit;False;MixColor;-1;;67;c73e8a542d6aa7b41a97cfb308620336;0;6;29;FLOAT4;0,0,0,0;False;24;FLOAT;0;False;25;FLOAT;0;False;30;FLOAT4;0,0,0,0;False;26;FLOAT;0;False;27;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.OneMinusNode;269;-756.2487,-1053.022;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;304;-562.2628,-2106.333;Inherit;False;MixColor;-1;;66;c73e8a542d6aa7b41a97cfb308620336;0;6;29;FLOAT4;0,0,0,0;False;24;FLOAT;0;False;25;FLOAT;0;False;30;FLOAT4;0,0,0,0;False;26;FLOAT;0;False;27;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.BreakToComponentsNode;270;-311.6479,-1441.723;Inherit;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.FunctionNode;308;-571.4855,-1631.505;Inherit;False;MixColor;-1;;71;c73e8a542d6aa7b41a97cfb308620336;0;6;29;FLOAT4;0,0,0,0;False;24;FLOAT;0;False;25;FLOAT;0;False;30;FLOAT4;0,0,0,0;False;26;FLOAT;0;False;27;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.FunctionNode;307;-558.5106,-2316.945;Inherit;False;MixColor;-1;;72;c73e8a542d6aa7b41a97cfb308620336;0;6;29;FLOAT4;0,0,0,0;False;24;FLOAT;0;False;25;FLOAT;0;False;30;FLOAT4;0,0,0,0;False;26;FLOAT;0;False;27;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.WireNode;293;-326.7778,-1618.781;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;309;-563.6239,-1914.388;Inherit;False;MixColor;-1;;73;c73e8a542d6aa7b41a97cfb308620336;0;6;29;FLOAT4;0,0,0,0;False;24;FLOAT;0;False;25;FLOAT;0;False;30;FLOAT4;0,0,0,0;False;26;FLOAT;0;False;27;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.BreakToComponentsNode;265;-283.0494,-2113.824;Inherit;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.FunctionNode;306;-571.6339,-1247.357;Inherit;False;MixColor;-1;;74;c73e8a542d6aa7b41a97cfb308620336;0;6;29;FLOAT4;0,0,0,0;False;24;FLOAT;0;False;25;FLOAT;0;False;30;FLOAT4;0,0,0,0;False;26;FLOAT;0;False;27;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.FunctionNode;313;-129.6492,-2018.925;Inherit;False;MixColor;-1;;76;c73e8a542d6aa7b41a97cfb308620336;0;6;29;FLOAT4;0,0,0,0;False;24;FLOAT;0;False;25;FLOAT;0;False;30;FLOAT4;0,0,0,0;False;26;FLOAT;0;False;27;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.FunctionNode;310;-132.249,-1634.124;Inherit;False;MixColor;-1;;75;c73e8a542d6aa7b41a97cfb308620336;0;6;29;FLOAT4;0,0,0,0;False;24;FLOAT;0;False;25;FLOAT;0;False;30;FLOAT4;0,0,0,0;False;26;FLOAT;0;False;27;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;271;239.5524,-1970.822;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TexturePropertyNode;17;61.7062,-2349.63;Inherit;True;Property;_SpatterTex;SpatterTex;4;1;[PerRendererData];Create;True;0;0;0;True;0;False;None;None;False;white;LockedToTexture2D;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.TexturePropertyNode;23;69.6608,-2620.011;Inherit;True;Property;_SpatterNoise;SpatterNoise;10;0;Create;True;0;0;0;True;0;False;14e8ef81985b3e040a0065d14e34ad3b;14e8ef81985b3e040a0065d14e34ad3b;False;white;LockedToTexture2D;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;273;699.2483,-1849.252;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;19;-12.9837,-1232.188;Inherit;False;Property;_SpatterSmoothness;SpatterSmoothness;6;0;Create;True;0;0;0;False;0;False;0.5;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;312;-130.949,-1831.723;Inherit;False;MixColor;-1;;80;c73e8a542d6aa7b41a97cfb308620336;0;6;29;FLOAT4;0,0,0,0;False;24;FLOAT;0;False;25;FLOAT;0;False;30;FLOAT4;0,0,0,0;False;26;FLOAT;0;False;27;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.UnpackScaleNormalNode;277;240.8526,-1738.123;Inherit;False;Tangent;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector3Node;18;-3.842987,-1119.326;Inherit;False;Property;_SpatterDirection;SpatterDirection;5;0;Create;True;0;0;0;False;0;False;0,1,0;0,1,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;20;477.2726,-1650.237;Inherit;False;Property;_MetalLevel;Metallic;7;0;Create;False;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;21;478.3676,-1579.339;Inherit;False;Property;_Rough;Roughness;8;0;Create;False;0;0;0;False;0;False;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;274;748.2972,-1727.937;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;326;437.2032,-2140.629;Inherit;False;Add Spatter;-1;;79;c2f011ec950e65243864f0c5daebe4b8;0;4;184;SAMPLER2D;0;False;173;FLOAT4;0,0,0,0;False;175;SAMPLER2D;0;False;179;FLOAT4;0,0,0,0;False;4;FLOAT;170;FLOAT;171;FLOAT;172;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;11;0,0;Float;False;False;-1;2;Rendering.HighDefinition.LightingShaderGraphGUI;0;2;New Amplify Shader;53b46d85872c5b24c8f4f0a1c3fe4c87;True;ScenePickingPass;0;11;ScenePickingPass;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=HDRenderPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;5;True;7;d3d11;metal;vulkan;xboxone;xboxseries;playstation;switch;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;0;True;-30;False;False;False;False;False;False;False;False;False;False;False;True;2;False;-1;True;3;False;-1;False;True;1;LightMode=Picking;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;900.4985,-1929.59;Float;False;True;-1;2;Rendering.HighDefinition.LightingShaderGraphGUI;0;2;GroundSplat;53b46d85872c5b24c8f4f0a1c3fe4c87;True;GBuffer;0;0;GBuffer;35;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=HDRenderPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;5;True;7;d3d11;metal;vulkan;xboxone;xboxseries;playstation;switch;0;False;False;False;False;False;False;False;False;False;False;False;False;False;True;True;0;True;-30;False;False;False;False;False;False;False;False;False;True;True;0;True;-15;255;False;-1;255;True;-14;7;False;-1;3;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;False;True;0;True;-16;False;True;1;LightMode=GBuffer;False;False;3;Include;;False;;Native;Pragma;multi_compile __ CONTAMINANTS;False;;Custom;Pragma;multi_compile __ GRASS;False;;Custom;HDRP/Lit;0;0;Standard;42;Surface Type;0;638025876410814995;  Rendering Pass;1;0;  Refraction Model;0;0;    Blending Mode;0;0;    Blend Preserves Specular;0;638025759502664784;  Receive Fog;0;638025759498265455;  Back Then Front Rendering;0;0;  Transparent Depth Prepass;0;0;  Transparent Depth Postpass;0;0;  Transparent Writes Motion Vector;0;0;  Distortion;0;0;    Distortion Mode;0;0;    Distortion Depth Test;1;0;  ZWrite;0;0;  Z Test;4;0;Double-Sided;0;0;Alpha Clipping;1;638025876452014736;  Use Shadow Threshold;0;0;Material Type,InvertActionOnDeselection;0;0;  Energy Conserving Specular;1;0;  Transmission;1;0;Receive Decals;0;638024092057805306;Receives SSR;0;638024092061258028;Receive SSR Transparent;0;0;Motion Vectors;0;638025759553262774;  Add Precomputed Velocity;1;638025596539928886;Specular AA;0;0;Specular Occlusion Mode;0;638024316885607009;Override Baked GI;0;0;Depth Offset;0;0;DOTS Instancing;0;0;LOD CrossFade;0;0;Tessellation;0;0;  Phong;0;0;  Strength;0.5,False,-1;0;  Type;0;0;  Tess;16,False,-1;0;  Min;10,False,-1;0;  Max;25,False,-1;0;  Edge Length;16,False,-1;0;  Max Displacement;25,False,-1;0;Vertex Position;1;0;0;12;True;True;True;True;True;True;False;False;False;False;True;True;False;;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;9;0,0;Float;False;False;-1;2;Rendering.HighDefinition.LightingShaderGraphGUI;0;2;New Amplify Shader;53b46d85872c5b24c8f4f0a1c3fe4c87;True;TransparentDepthPostpass;0;9;TransparentDepthPostpass;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=HDRenderPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;5;True;7;d3d11;metal;vulkan;xboxone;xboxseries;playstation;switch;0;False;True;1;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;True;0;True;-30;False;True;False;False;False;False;0;False;-1;False;False;False;False;False;False;False;False;False;True;1;False;-1;False;False;True;1;LightMode=TransparentDepthPostpass;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;3;0,0;Float;False;False;-1;2;Rendering.HighDefinition.LightingShaderGraphGUI;0;2;New Amplify Shader;53b46d85872c5b24c8f4f0a1c3fe4c87;True;SceneSelectionPass;0;3;SceneSelectionPass;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=HDRenderPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;5;True;7;d3d11;metal;vulkan;xboxone;xboxseries;playstation;switch;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=SceneSelectionPass;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;5;0,0;Float;False;False;-1;2;Rendering.HighDefinition.LightingShaderGraphGUI;0;2;New Amplify Shader;53b46d85872c5b24c8f4f0a1c3fe4c87;True;Motion Vectors;0;5;Motion Vectors;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=HDRenderPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;5;True;7;d3d11;metal;vulkan;xboxone;xboxseries;playstation;switch;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;0;True;-30;False;False;False;False;False;False;False;False;False;True;True;0;True;-10;255;False;-1;255;True;-11;7;False;-1;3;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;1;False;-1;False;False;True;1;LightMode=MotionVectors;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;7;0,0;Float;False;False;-1;2;Rendering.HighDefinition.LightingShaderGraphGUI;0;2;New Amplify Shader;53b46d85872c5b24c8f4f0a1c3fe4c87;True;TransparentBackface;0;7;TransparentBackface;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=HDRenderPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;5;True;7;d3d11;metal;vulkan;xboxone;xboxseries;playstation;switch;0;False;True;1;0;True;-22;0;True;-23;1;0;True;-24;0;True;-25;False;False;False;False;False;False;False;False;False;False;False;False;True;1;False;-1;False;False;False;True;True;True;True;True;0;True;-53;False;False;False;False;False;False;False;True;0;True;-28;True;0;True;-37;False;True;1;LightMode=TransparentBackface;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;6;0,0;Float;False;False;-1;2;Rendering.HighDefinition.LightingShaderGraphGUI;0;2;New Amplify Shader;53b46d85872c5b24c8f4f0a1c3fe4c87;True;Distortion;0;6;Distortion;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=HDRenderPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;5;True;7;d3d11;metal;vulkan;xboxone;xboxseries;playstation;switch;0;False;True;4;1;False;-1;1;False;-1;4;1;False;-1;1;False;-1;True;1;False;-1;1;False;-1;False;False;False;False;False;False;False;False;False;False;False;True;0;True;-30;False;False;False;False;False;False;False;False;False;True;True;0;True;-12;255;False;-1;255;True;-13;7;False;-1;3;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;2;False;-1;True;3;False;-1;False;True;1;LightMode=DistortionVectors;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;4;0,0;Float;False;False;-1;2;Rendering.HighDefinition.LightingShaderGraphGUI;0;2;New Amplify Shader;53b46d85872c5b24c8f4f0a1c3fe4c87;True;DepthOnly;0;4;DepthOnly;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=HDRenderPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;5;True;7;d3d11;metal;vulkan;xboxone;xboxseries;playstation;switch;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;0;True;-30;False;False;False;False;False;False;False;False;False;True;True;0;True;-8;255;False;-1;255;True;-9;7;False;-1;3;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;1;False;-1;False;False;True;1;LightMode=DepthOnly;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;1;0,0;Float;False;False;-1;2;Rendering.HighDefinition.LightingShaderGraphGUI;0;2;New Amplify Shader;53b46d85872c5b24c8f4f0a1c3fe4c87;True;META;0;1;META;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=HDRenderPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;5;True;7;d3d11;metal;vulkan;xboxone;xboxseries;playstation;switch;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Meta;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;10;0,0;Float;False;False;-1;2;Rendering.HighDefinition.LightingShaderGraphGUI;0;2;New Amplify Shader;53b46d85872c5b24c8f4f0a1c3fe4c87;True;Forward;0;10;Forward;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=HDRenderPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;5;True;7;d3d11;metal;vulkan;xboxone;xboxseries;playstation;switch;0;False;True;1;0;True;-22;0;True;-23;1;0;True;-24;0;True;-25;False;False;False;False;False;False;False;False;False;False;False;False;True;0;True;-33;False;False;False;True;True;True;True;True;0;True;-53;False;False;False;False;False;True;True;0;True;-6;255;False;-1;255;True;-7;7;False;-1;3;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;0;True;-28;True;0;True;-36;False;True;1;LightMode=Forward;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;8;0,0;Float;False;False;-1;2;Rendering.HighDefinition.LightingShaderGraphGUI;0;2;New Amplify Shader;53b46d85872c5b24c8f4f0a1c3fe4c87;True;TransparentDepthPrepass;0;8;TransparentDepthPrepass;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=HDRenderPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;5;True;7;d3d11;metal;vulkan;xboxone;xboxseries;playstation;switch;0;False;True;1;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;True;0;True;-30;False;False;False;False;False;False;False;False;False;True;True;0;True;-8;255;False;-1;255;True;-9;7;False;-1;3;False;-1;1;False;-1;1;False;-1;7;False;-1;3;False;-1;1;False;-1;1;False;-1;False;True;1;False;-1;False;False;True;1;LightMode=TransparentDepthPrepass;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;2;0,0;Float;False;False;-1;2;Rendering.HighDefinition.LightingShaderGraphGUI;0;2;New Amplify Shader;53b46d85872c5b24c8f4f0a1c3fe4c87;True;ShadowCaster;0;2;ShadowCaster;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=HDRenderPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;5;True;7;d3d11;metal;vulkan;xboxone;xboxseries;playstation;switch;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;0;True;-30;False;True;False;False;False;False;0;False;-1;False;False;False;False;False;False;False;False;False;True;1;False;-1;True;3;False;-1;False;True;1;LightMode=ShadowCaster;False;False;0;;0;0;Standard;0;False;0
WireConnection;69;0;66;1
WireConnection;69;1;66;3
WireConnection;72;0;22;3
WireConnection;72;1;22;4
WireConnection;70;0;22;1
WireConnection;70;1;22;2
WireConnection;26;0;14;0
WireConnection;73;0;72;0
WireConnection;73;1;70;0
WireConnection;68;0;69;0
WireConnection;68;1;70;0
WireConnection;71;0;68;0
WireConnection;71;1;73;0
WireConnection;92;0;26;3
WireConnection;92;1;26;4
WireConnection;90;0;71;0
WireConnection;90;1;92;0
WireConnection;95;0;90;0
WireConnection;97;0;95;0
WireConnection;94;0;97;0
WireConnection;100;0;90;0
WireConnection;100;1;94;0
WireConnection;248;0;100;0
WireConnection;335;5;14;0
WireConnection;335;7;94;0
WireConnection;335;8;315;0
WireConnection;335;9;92;0
WireConnection;335;10;13;0
WireConnection;335;13;16;0
WireConnection;335;17;15;0
WireConnection;335;22;195;0
WireConnection;335;33;214;0
WireConnection;333;5;14;0
WireConnection;333;7;94;0
WireConnection;333;8;314;0
WireConnection;333;9;92;0
WireConnection;333;10;13;0
WireConnection;333;13;16;0
WireConnection;333;17;15;0
WireConnection;333;22;195;0
WireConnection;333;33;214;0
WireConnection;264;0;248;0
WireConnection;332;5;14;0
WireConnection;332;7;94;0
WireConnection;332;8;316;0
WireConnection;332;9;92;0
WireConnection;332;10;13;0
WireConnection;332;13;16;0
WireConnection;332;17;15;0
WireConnection;332;22;195;0
WireConnection;332;33;214;0
WireConnection;334;5;14;0
WireConnection;334;7;94;0
WireConnection;334;8;317;0
WireConnection;334;9;92;0
WireConnection;334;10;13;0
WireConnection;334;13;16;0
WireConnection;334;17;15;0
WireConnection;334;22;195;0
WireConnection;334;33;214;0
WireConnection;305;29;335;1
WireConnection;305;24;335;3
WireConnection;305;25;264;0
WireConnection;305;30;333;1
WireConnection;305;26;333;3
WireConnection;305;27;248;0
WireConnection;269;0;248;1
WireConnection;304;29;334;1
WireConnection;304;24;334;3
WireConnection;304;25;264;0
WireConnection;304;30;332;1
WireConnection;304;26;332;3
WireConnection;304;27;248;0
WireConnection;270;0;305;0
WireConnection;308;29;335;0
WireConnection;308;24;335;3
WireConnection;308;25;264;0
WireConnection;308;30;333;0
WireConnection;308;26;333;3
WireConnection;308;27;248;0
WireConnection;307;29;334;0
WireConnection;307;24;334;3
WireConnection;307;25;264;0
WireConnection;307;30;332;0
WireConnection;307;26;332;3
WireConnection;307;27;248;0
WireConnection;293;0;269;0
WireConnection;309;29;334;2
WireConnection;309;24;334;3
WireConnection;309;25;264;0
WireConnection;309;30;332;2
WireConnection;309;26;332;3
WireConnection;309;27;248;0
WireConnection;265;0;304;0
WireConnection;306;29;335;2
WireConnection;306;24;335;3
WireConnection;306;25;264;0
WireConnection;306;30;333;2
WireConnection;306;26;333;3
WireConnection;306;27;248;0
WireConnection;313;29;307;0
WireConnection;313;24;265;2
WireConnection;313;25;293;0
WireConnection;313;30;308;0
WireConnection;313;26;270;2
WireConnection;313;27;248;1
WireConnection;310;29;309;0
WireConnection;310;24;265;2
WireConnection;310;25;269;0
WireConnection;310;30;306;0
WireConnection;310;26;270;2
WireConnection;310;27;248;1
WireConnection;271;0;313;0
WireConnection;271;1;310;0
WireConnection;273;0;326;170
WireConnection;273;1;20;0
WireConnection;312;29;304;0
WireConnection;312;24;265;2
WireConnection;312;25;269;0
WireConnection;312;30;305;0
WireConnection;312;26;270;2
WireConnection;312;27;248;1
WireConnection;277;0;312;0
WireConnection;274;0;326;171
WireConnection;274;1;21;0
WireConnection;326;184;23;0
WireConnection;326;173;22;0
WireConnection;326;175;17;0
WireConnection;326;179;271;0
WireConnection;0;0;326;0
WireConnection;0;1;277;0
WireConnection;0;4;273;0
WireConnection;0;7;274;0
WireConnection;0;9;326;172
ASEEND*/
//CHKSM=B9E655C4F6C12BE203D7ED116905507562E89760