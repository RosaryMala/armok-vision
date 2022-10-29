// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Terrain Splatter"
{
	Properties
	{
		[HideInInspector] _AlphaCutoff("Alpha Cutoff ", Range(0, 1)) = 0.5
		_WorldBounds("World Bounds", Vector) = (0,0,1,1)
		_Noise("Noise", 2D) = "white" {}
		_Control("Control", 2D) = "white" {}
		_GrassControl("Grass Control", 2D) = "white" {}
		_Tint("Tint", 2D) = "white" {}
		_GrassTint("Grass Tint", 2D) = "white" {}
		_ShapeArray("Shape Array", 2DArray) = "white" {}
		_MaterialArray("Material Array", 2DArray) = "white" {}
		[ASEEnd]_SpatterTex("Spatter Texture", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}

		[HideInInspector]_EmissionColor("Color", Color) = (1, 1, 1, 1)
		[HideInInspector] _RenderQueueType("Render Queue Type", Float) = 5
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
		[HideInInspector] [ToggleUI] _ReceivesSSR("Receives SSR", Float) = 1
		[HideInInspector] [ToggleUI] _ReceivesSSRTransparent("Boolean", Float) = 0
		[HideInInspector] _SurfaceType("Surface Type", Float) = 1
		[HideInInspector] _BlendMode("Blend Mode", Float) = 0
		[HideInInspector] _SrcBlend("Src Blend", Float) = 1
		[HideInInspector] _DstBlend("Dst Blend", Float) = 0
		[HideInInspector] _AlphaSrcBlend("Alpha Src Blend", Float) = 1
		[HideInInspector] _AlphaDstBlend("Alpha Dst Blend", Float) = 0
		[HideInInspector][ToggleUI]_AlphaToMask("Boolean", Float) = 0//New
        [HideInInspector][ToggleUI]_AlphaToMaskInspectorValue("Boolean", Float) = 0//New
		[HideInInspector] [ToggleUI] _ZWrite("ZWrite", Float) = 0
		[HideInInspector] [ToggleUI] _TransparentZWrite("Transparent ZWrite", Float) = 0
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

		

		Tags { "RenderPipeline"="HDRenderPipeline" "RenderType"="Transparent" "Queue"="Transparent" }

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

			#define _BLENDMODE_PRESERVE_SPECULAR_LIGHTING 1
			#define _DISABLE_SSR_TRANSPARENT 1
			#define _SPECULAR_OCCLUSION_FROM_AO 1
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
			float4 _MaterialArray_ST;
			float4 _Control_TexelSize;
			float4 _ShapeArray_ST;
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
			sampler2D _Noise;
			TEXTURE2D_ARRAY(_MaterialArray);
			sampler2D _GrassControl;
			sampler2D _Control;
			SAMPLER(sampler_MaterialArray);
			TEXTURE2D_ARRAY(_ShapeArray);
			SAMPLER(sampler_ShapeArray);
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
				float dotResult145_g104 = dot( normalWS , float3( 0,1,0 ) );
				float3 ase_worldPos = GetAbsolutePositionWS( positionRWS );
				float3 break180_g104 = ase_worldPos;
				float2 appendResult102_g104 = (float2(break180_g104.x , break180_g104.z));
				float4 break174_g104 = _WorldBounds;
				float2 appendResult129_g104 = (float2(break174_g104.x , break174_g104.y));
				float2 appendResult130_g104 = (float2(break174_g104.z , break174_g104.w));
				float4 tex2DNode133_g104 = tex2D( _SpatterTex, ( ( appendResult102_g104 - appendResult129_g104 ) / ( appendResult130_g104 - appendResult129_g104 ) ) );
				float lerpResult139_g104 = lerp( 1.0 , -1.0 , ( tex2DNode133_g104.a - tex2D( _Noise, appendResult102_g104 ).r ));
				float temp_output_147_0_g104 = ( dotResult145_g104 >= lerpResult139_g104 ? 1.0 : 0.0 );
				float2 uv_MaterialArray = packedInput.ase_texcoord5.xy;
				float3 break17 = ase_worldPos;
				float2 appendResult26 = (float2(break17.x , break17.z));
				float4 break18 = _WorldBounds;
				float2 appendResult27 = (float2(break18.x , break18.y));
				float2 appendResult28 = (float2(break18.w , break18.z));
				float2 appendResult35 = (float2(_Control_TexelSize.z , _Control_TexelSize.w));
				float2 temp_output_36_0 = ( ( ( appendResult26 - appendResult27 ) / ( appendResult28 - appendResult27 ) ) * appendResult35 );
				float2 temp_output_39_0 = ( floor( ( temp_output_36_0 + float2( 0.5,0.5 ) ) ) - float2( 0.5,0.5 ) );
				float2 temp_output_6_0_g80 = ( ( temp_output_39_0 + float2( 0,0 ) ) / appendResult35 );
				float4 tex2DNode10_g80 = tex2D( _GrassControl, temp_output_6_0_g80 );
				float2 uv_ShapeArray = packedInput.ase_texcoord5.xy;
				float4 tex2DArrayNode27_g80 = SAMPLE_TEXTURE2D_ARRAY( _ShapeArray, sampler_ShapeArray, uv_ShapeArray,( tex2DNode10_g80.g * 255.0 ) );
				float4 tex2DNode11_g80 = tex2D( _GrassTint, temp_output_6_0_g80 );
				float temp_output_7_0_g83 = ( tex2DArrayNode27_g80.b + tex2DNode11_g80.a );
				float4 tex2DNode8_g80 = tex2D( _Control, temp_output_6_0_g80 );
				float4 tex2DArrayNode24_g80 = SAMPLE_TEXTURE2D_ARRAY( _ShapeArray, sampler_ShapeArray, uv_ShapeArray,( tex2DNode8_g80.g * 255.0 ) );
				float temp_output_30_0_g80 = ( 1.0 - tex2DNode11_g80.a );
				float temp_output_8_0_g83 = ( tex2DArrayNode24_g80.b + temp_output_30_0_g80 );
				float temp_output_10_0_g83 = ( max( temp_output_7_0_g83 , temp_output_8_0_g83 ) - 0.2 );
				float temp_output_13_0_g83 = max( ( temp_output_7_0_g83 - temp_output_10_0_g83 ) , 0.0 );
				float4 tex2DArrayNode21_g80 = SAMPLE_TEXTURE2D_ARRAY( _MaterialArray, sampler_MaterialArray, uv_MaterialArray,( tex2DNode8_g80.r * 255.0 ) );
				float temp_output_14_0_g83 = max( ( temp_output_8_0_g83 - temp_output_10_0_g83 ) , 0.0 );
				float temp_output_7_0_g82 = ( tex2DArrayNode27_g80.b + tex2DNode11_g80.a );
				float temp_output_8_0_g82 = ( tex2DArrayNode24_g80.b + temp_output_30_0_g80 );
				float temp_output_10_0_g82 = ( max( temp_output_7_0_g82 , temp_output_8_0_g82 ) - 0.2 );
				float temp_output_13_0_g82 = max( ( temp_output_7_0_g82 - temp_output_10_0_g82 ) , 0.0 );
				float temp_output_14_0_g82 = max( ( temp_output_8_0_g82 - temp_output_10_0_g82 ) , 0.0 );
				float temp_output_77_42 = ( ( ( tex2DArrayNode27_g80 * temp_output_13_0_g82 ) + ( tex2DArrayNode24_g80 * temp_output_14_0_g82 ) ) / ( temp_output_13_0_g82 + temp_output_14_0_g82 ) ).z;
				float2 break41 = ( temp_output_36_0 - temp_output_39_0 );
				float temp_output_49_0 = ( 1.0 - break41.x );
				float temp_output_7_0_g97 = ( temp_output_77_42 + temp_output_49_0 );
				float2 temp_output_6_0_g84 = ( ( temp_output_39_0 + float2( 0,0 ) ) / appendResult35 );
				float4 tex2DNode10_g84 = tex2D( _GrassControl, temp_output_6_0_g84 );
				float4 tex2DArrayNode27_g84 = SAMPLE_TEXTURE2D_ARRAY( _ShapeArray, sampler_ShapeArray, uv_ShapeArray,( tex2DNode10_g84.g * 255.0 ) );
				float4 tex2DNode11_g84 = tex2D( _GrassTint, temp_output_6_0_g84 );
				float temp_output_7_0_g86 = ( tex2DArrayNode27_g84.b + tex2DNode11_g84.a );
				float4 tex2DNode8_g84 = tex2D( _Control, temp_output_6_0_g84 );
				float4 tex2DArrayNode24_g84 = SAMPLE_TEXTURE2D_ARRAY( _ShapeArray, sampler_ShapeArray, uv_ShapeArray,( tex2DNode8_g84.g * 255.0 ) );
				float temp_output_30_0_g84 = ( 1.0 - tex2DNode11_g84.a );
				float temp_output_8_0_g86 = ( tex2DArrayNode24_g84.b + temp_output_30_0_g84 );
				float temp_output_10_0_g86 = ( max( temp_output_7_0_g86 , temp_output_8_0_g86 ) - 0.2 );
				float temp_output_13_0_g86 = max( ( temp_output_7_0_g86 - temp_output_10_0_g86 ) , 0.0 );
				float temp_output_14_0_g86 = max( ( temp_output_8_0_g86 - temp_output_10_0_g86 ) , 0.0 );
				float temp_output_79_42 = ( ( ( tex2DArrayNode27_g84 * temp_output_13_0_g86 ) + ( tex2DArrayNode24_g84 * temp_output_14_0_g86 ) ) / ( temp_output_13_0_g86 + temp_output_14_0_g86 ) ).z;
				float temp_output_8_0_g97 = ( temp_output_79_42 + break41.x );
				float temp_output_10_0_g97 = ( max( temp_output_7_0_g97 , temp_output_8_0_g97 ) - 0.2 );
				float temp_output_13_0_g97 = max( ( temp_output_7_0_g97 - temp_output_10_0_g97 ) , 0.0 );
				float temp_output_7_0_g87 = ( tex2DArrayNode27_g84.b + tex2DNode11_g84.a );
				float temp_output_8_0_g87 = ( tex2DArrayNode24_g84.b + temp_output_30_0_g84 );
				float temp_output_10_0_g87 = ( max( temp_output_7_0_g87 , temp_output_8_0_g87 ) - 0.2 );
				float temp_output_13_0_g87 = max( ( temp_output_7_0_g87 - temp_output_10_0_g87 ) , 0.0 );
				float4 tex2DArrayNode21_g84 = SAMPLE_TEXTURE2D_ARRAY( _MaterialArray, sampler_MaterialArray, uv_MaterialArray,( tex2DNode8_g84.r * 255.0 ) );
				float temp_output_14_0_g87 = max( ( temp_output_8_0_g87 - temp_output_10_0_g87 ) , 0.0 );
				float temp_output_14_0_g97 = max( ( temp_output_8_0_g97 - temp_output_10_0_g97 ) , 0.0 );
				float temp_output_7_0_g93 = ( temp_output_77_42 + temp_output_49_0 );
				float temp_output_8_0_g93 = ( temp_output_79_42 + break41.x );
				float temp_output_10_0_g93 = ( max( temp_output_7_0_g93 , temp_output_8_0_g93 ) - 0.2 );
				float temp_output_13_0_g93 = max( ( temp_output_7_0_g93 - temp_output_10_0_g93 ) , 0.0 );
				float temp_output_14_0_g93 = max( ( temp_output_8_0_g93 - temp_output_10_0_g93 ) , 0.0 );
				float4 temp_output_44_0 = ( ( ( ( ( ( tex2DArrayNode27_g80 * temp_output_13_0_g82 ) + ( tex2DArrayNode24_g80 * temp_output_14_0_g82 ) ) / ( temp_output_13_0_g82 + temp_output_14_0_g82 ) ) * temp_output_13_0_g93 ) + ( ( ( ( tex2DArrayNode27_g84 * temp_output_13_0_g86 ) + ( tex2DArrayNode24_g84 * temp_output_14_0_g86 ) ) / ( temp_output_13_0_g86 + temp_output_14_0_g86 ) ) * temp_output_14_0_g93 ) ) / ( temp_output_13_0_g93 + temp_output_14_0_g93 ) );
				float temp_output_61_0 = ( 1.0 - break41.y );
				float temp_output_7_0_g100 = ( temp_output_44_0.z + temp_output_61_0 );
				float2 temp_output_6_0_g88 = ( ( temp_output_39_0 + float2( 0,0 ) ) / appendResult35 );
				float4 tex2DNode10_g88 = tex2D( _GrassControl, temp_output_6_0_g88 );
				float4 tex2DArrayNode27_g88 = SAMPLE_TEXTURE2D_ARRAY( _ShapeArray, sampler_ShapeArray, uv_ShapeArray,( tex2DNode10_g88.g * 255.0 ) );
				float4 tex2DNode11_g88 = tex2D( _GrassTint, temp_output_6_0_g88 );
				float temp_output_7_0_g90 = ( tex2DArrayNode27_g88.b + tex2DNode11_g88.a );
				float4 tex2DNode8_g88 = tex2D( _Control, temp_output_6_0_g88 );
				float4 tex2DArrayNode24_g88 = SAMPLE_TEXTURE2D_ARRAY( _ShapeArray, sampler_ShapeArray, uv_ShapeArray,( tex2DNode8_g88.g * 255.0 ) );
				float temp_output_30_0_g88 = ( 1.0 - tex2DNode11_g88.a );
				float temp_output_8_0_g90 = ( tex2DArrayNode24_g88.b + temp_output_30_0_g88 );
				float temp_output_10_0_g90 = ( max( temp_output_7_0_g90 , temp_output_8_0_g90 ) - 0.2 );
				float temp_output_13_0_g90 = max( ( temp_output_7_0_g90 - temp_output_10_0_g90 ) , 0.0 );
				float temp_output_14_0_g90 = max( ( temp_output_8_0_g90 - temp_output_10_0_g90 ) , 0.0 );
				float temp_output_80_42 = ( ( ( tex2DArrayNode27_g88 * temp_output_13_0_g90 ) + ( tex2DArrayNode24_g88 * temp_output_14_0_g90 ) ) / ( temp_output_13_0_g90 + temp_output_14_0_g90 ) ).z;
				float temp_output_7_0_g94 = ( temp_output_80_42 + temp_output_49_0 );
				float2 temp_output_6_0_g76 = ( ( temp_output_39_0 + float2( 0,0 ) ) / appendResult35 );
				float4 tex2DNode10_g76 = tex2D( _GrassControl, temp_output_6_0_g76 );
				float4 tex2DArrayNode27_g76 = SAMPLE_TEXTURE2D_ARRAY( _ShapeArray, sampler_ShapeArray, uv_ShapeArray,( tex2DNode10_g76.g * 255.0 ) );
				float4 tex2DNode11_g76 = tex2D( _GrassTint, temp_output_6_0_g76 );
				float temp_output_7_0_g78 = ( tex2DArrayNode27_g76.b + tex2DNode11_g76.a );
				float4 tex2DNode8_g76 = tex2D( _Control, temp_output_6_0_g76 );
				float4 tex2DArrayNode24_g76 = SAMPLE_TEXTURE2D_ARRAY( _ShapeArray, sampler_ShapeArray, uv_ShapeArray,( tex2DNode8_g76.g * 255.0 ) );
				float temp_output_30_0_g76 = ( 1.0 - tex2DNode11_g76.a );
				float temp_output_8_0_g78 = ( tex2DArrayNode24_g76.b + temp_output_30_0_g76 );
				float temp_output_10_0_g78 = ( max( temp_output_7_0_g78 , temp_output_8_0_g78 ) - 0.2 );
				float temp_output_13_0_g78 = max( ( temp_output_7_0_g78 - temp_output_10_0_g78 ) , 0.0 );
				float temp_output_14_0_g78 = max( ( temp_output_8_0_g78 - temp_output_10_0_g78 ) , 0.0 );
				float temp_output_78_42 = ( ( ( tex2DArrayNode27_g76 * temp_output_13_0_g78 ) + ( tex2DArrayNode24_g76 * temp_output_14_0_g78 ) ) / ( temp_output_13_0_g78 + temp_output_14_0_g78 ) ).z;
				float temp_output_8_0_g94 = ( temp_output_78_42 + break41.x );
				float temp_output_10_0_g94 = ( max( temp_output_7_0_g94 , temp_output_8_0_g94 ) - 0.2 );
				float temp_output_13_0_g94 = max( ( temp_output_7_0_g94 - temp_output_10_0_g94 ) , 0.0 );
				float temp_output_14_0_g94 = max( ( temp_output_8_0_g94 - temp_output_10_0_g94 ) , 0.0 );
				float4 temp_output_47_0 = ( ( ( ( ( ( tex2DArrayNode27_g88 * temp_output_13_0_g90 ) + ( tex2DArrayNode24_g88 * temp_output_14_0_g90 ) ) / ( temp_output_13_0_g90 + temp_output_14_0_g90 ) ) * temp_output_13_0_g94 ) + ( ( ( ( tex2DArrayNode27_g76 * temp_output_13_0_g78 ) + ( tex2DArrayNode24_g76 * temp_output_14_0_g78 ) ) / ( temp_output_13_0_g78 + temp_output_14_0_g78 ) ) * temp_output_14_0_g94 ) ) / ( temp_output_13_0_g94 + temp_output_14_0_g94 ) );
				float temp_output_8_0_g100 = ( temp_output_47_0.z + break41.y );
				float temp_output_10_0_g100 = ( max( temp_output_7_0_g100 , temp_output_8_0_g100 ) - 0.2 );
				float temp_output_13_0_g100 = max( ( temp_output_7_0_g100 - temp_output_10_0_g100 ) , 0.0 );
				float temp_output_7_0_g91 = ( tex2DArrayNode27_g88.b + tex2DNode11_g88.a );
				float temp_output_8_0_g91 = ( tex2DArrayNode24_g88.b + temp_output_30_0_g88 );
				float temp_output_10_0_g91 = ( max( temp_output_7_0_g91 , temp_output_8_0_g91 ) - 0.2 );
				float temp_output_13_0_g91 = max( ( temp_output_7_0_g91 - temp_output_10_0_g91 ) , 0.0 );
				float4 tex2DArrayNode21_g88 = SAMPLE_TEXTURE2D_ARRAY( _MaterialArray, sampler_MaterialArray, uv_MaterialArray,( tex2DNode8_g88.r * 255.0 ) );
				float temp_output_14_0_g91 = max( ( temp_output_8_0_g91 - temp_output_10_0_g91 ) , 0.0 );
				float temp_output_7_0_g98 = ( temp_output_80_42 + temp_output_49_0 );
				float temp_output_8_0_g98 = ( temp_output_78_42 + break41.x );
				float temp_output_10_0_g98 = ( max( temp_output_7_0_g98 , temp_output_8_0_g98 ) - 0.2 );
				float temp_output_13_0_g98 = max( ( temp_output_7_0_g98 - temp_output_10_0_g98 ) , 0.0 );
				float temp_output_7_0_g79 = ( tex2DArrayNode27_g76.b + tex2DNode11_g76.a );
				float temp_output_8_0_g79 = ( tex2DArrayNode24_g76.b + temp_output_30_0_g76 );
				float temp_output_10_0_g79 = ( max( temp_output_7_0_g79 , temp_output_8_0_g79 ) - 0.2 );
				float temp_output_13_0_g79 = max( ( temp_output_7_0_g79 - temp_output_10_0_g79 ) , 0.0 );
				float4 tex2DArrayNode21_g76 = SAMPLE_TEXTURE2D_ARRAY( _MaterialArray, sampler_MaterialArray, uv_MaterialArray,( tex2DNode8_g76.r * 255.0 ) );
				float temp_output_14_0_g79 = max( ( temp_output_8_0_g79 - temp_output_10_0_g79 ) , 0.0 );
				float temp_output_14_0_g98 = max( ( temp_output_8_0_g98 - temp_output_10_0_g98 ) , 0.0 );
				float temp_output_14_0_g100 = max( ( temp_output_8_0_g100 - temp_output_10_0_g100 ) , 0.0 );
				float4 temp_output_62_0 = ( ( ( ( ( ( ( ( ( SAMPLE_TEXTURE2D_ARRAY( _MaterialArray, sampler_MaterialArray, uv_MaterialArray,( tex2DNode10_g80.r * 255.0 ) ) * temp_output_13_0_g83 ) + ( tex2DArrayNode21_g80 * temp_output_14_0_g83 ) ) / ( temp_output_13_0_g83 + temp_output_14_0_g83 ) ) * temp_output_13_0_g97 ) + ( ( ( ( SAMPLE_TEXTURE2D_ARRAY( _MaterialArray, sampler_MaterialArray, uv_MaterialArray,( tex2DNode10_g84.r * 255.0 ) ) * temp_output_13_0_g87 ) + ( tex2DArrayNode21_g84 * temp_output_14_0_g87 ) ) / ( temp_output_13_0_g87 + temp_output_14_0_g87 ) ) * temp_output_14_0_g97 ) ) / ( temp_output_13_0_g97 + temp_output_14_0_g97 ) ) * temp_output_13_0_g100 ) + ( ( ( ( ( ( ( SAMPLE_TEXTURE2D_ARRAY( _MaterialArray, sampler_MaterialArray, uv_MaterialArray,( tex2DNode10_g88.r * 255.0 ) ) * temp_output_13_0_g91 ) + ( tex2DArrayNode21_g88 * temp_output_14_0_g91 ) ) / ( temp_output_13_0_g91 + temp_output_14_0_g91 ) ) * temp_output_13_0_g98 ) + ( ( ( ( SAMPLE_TEXTURE2D_ARRAY( _MaterialArray, sampler_MaterialArray, uv_MaterialArray,( tex2DNode10_g76.r * 255.0 ) ) * temp_output_13_0_g79 ) + ( tex2DArrayNode21_g76 * temp_output_14_0_g79 ) ) / ( temp_output_13_0_g79 + temp_output_14_0_g79 ) ) * temp_output_14_0_g98 ) ) / ( temp_output_13_0_g98 + temp_output_14_0_g98 ) ) * temp_output_14_0_g100 ) ) / ( temp_output_13_0_g100 + temp_output_14_0_g100 ) );
				float temp_output_7_0_g81 = ( tex2DArrayNode27_g80.b + tex2DNode11_g80.a );
				float temp_output_8_0_g81 = ( tex2DArrayNode24_g80.b + temp_output_30_0_g80 );
				float temp_output_10_0_g81 = ( max( temp_output_7_0_g81 , temp_output_8_0_g81 ) - 0.2 );
				float temp_output_13_0_g81 = max( ( temp_output_7_0_g81 - temp_output_10_0_g81 ) , 0.0 );
				float4 tex2DNode9_g80 = tex2D( _Tint, temp_output_6_0_g80 );
				float temp_output_14_0_g81 = max( ( temp_output_8_0_g81 - temp_output_10_0_g81 ) , 0.0 );
				float temp_output_7_0_g95 = ( temp_output_77_42 + temp_output_49_0 );
				float temp_output_8_0_g95 = ( temp_output_79_42 + break41.x );
				float temp_output_10_0_g95 = ( max( temp_output_7_0_g95 , temp_output_8_0_g95 ) - 0.2 );
				float temp_output_13_0_g95 = max( ( temp_output_7_0_g95 - temp_output_10_0_g95 ) , 0.0 );
				float temp_output_7_0_g85 = ( tex2DArrayNode27_g84.b + tex2DNode11_g84.a );
				float temp_output_8_0_g85 = ( tex2DArrayNode24_g84.b + temp_output_30_0_g84 );
				float temp_output_10_0_g85 = ( max( temp_output_7_0_g85 , temp_output_8_0_g85 ) - 0.2 );
				float temp_output_13_0_g85 = max( ( temp_output_7_0_g85 - temp_output_10_0_g85 ) , 0.0 );
				float4 tex2DNode9_g84 = tex2D( _Tint, temp_output_6_0_g84 );
				float temp_output_14_0_g85 = max( ( temp_output_8_0_g85 - temp_output_10_0_g85 ) , 0.0 );
				float temp_output_14_0_g95 = max( ( temp_output_8_0_g95 - temp_output_10_0_g95 ) , 0.0 );
				float temp_output_7_0_g99 = ( temp_output_44_0.z + temp_output_61_0 );
				float temp_output_8_0_g99 = ( temp_output_47_0.z + break41.y );
				float temp_output_10_0_g99 = ( max( temp_output_7_0_g99 , temp_output_8_0_g99 ) - 0.2 );
				float temp_output_13_0_g99 = max( ( temp_output_7_0_g99 - temp_output_10_0_g99 ) , 0.0 );
				float temp_output_7_0_g89 = ( tex2DArrayNode27_g88.b + tex2DNode11_g88.a );
				float temp_output_8_0_g89 = ( tex2DArrayNode24_g88.b + temp_output_30_0_g88 );
				float temp_output_10_0_g89 = ( max( temp_output_7_0_g89 , temp_output_8_0_g89 ) - 0.2 );
				float temp_output_13_0_g89 = max( ( temp_output_7_0_g89 - temp_output_10_0_g89 ) , 0.0 );
				float4 tex2DNode9_g88 = tex2D( _Tint, temp_output_6_0_g88 );
				float temp_output_14_0_g89 = max( ( temp_output_8_0_g89 - temp_output_10_0_g89 ) , 0.0 );
				float temp_output_7_0_g96 = ( temp_output_80_42 + temp_output_49_0 );
				float temp_output_8_0_g96 = ( temp_output_78_42 + break41.x );
				float temp_output_10_0_g96 = ( max( temp_output_7_0_g96 , temp_output_8_0_g96 ) - 0.2 );
				float temp_output_13_0_g96 = max( ( temp_output_7_0_g96 - temp_output_10_0_g96 ) , 0.0 );
				float temp_output_7_0_g77 = ( tex2DArrayNode27_g76.b + tex2DNode11_g76.a );
				float temp_output_8_0_g77 = ( tex2DArrayNode24_g76.b + temp_output_30_0_g76 );
				float temp_output_10_0_g77 = ( max( temp_output_7_0_g77 , temp_output_8_0_g77 ) - 0.2 );
				float temp_output_13_0_g77 = max( ( temp_output_7_0_g77 - temp_output_10_0_g77 ) , 0.0 );
				float4 tex2DNode9_g76 = tex2D( _Tint, temp_output_6_0_g76 );
				float temp_output_14_0_g77 = max( ( temp_output_8_0_g77 - temp_output_10_0_g77 ) , 0.0 );
				float temp_output_14_0_g96 = max( ( temp_output_8_0_g96 - temp_output_10_0_g96 ) , 0.0 );
				float temp_output_14_0_g99 = max( ( temp_output_8_0_g99 - temp_output_10_0_g99 ) , 0.0 );
				float4 temp_output_64_0 = ( ( ( ( ( ( ( ( ( tex2DNode11_g80 * temp_output_13_0_g81 ) + ( tex2DNode9_g80 * temp_output_14_0_g81 ) ) / ( temp_output_13_0_g81 + temp_output_14_0_g81 ) ) * temp_output_13_0_g95 ) + ( ( ( ( tex2DNode11_g84 * temp_output_13_0_g85 ) + ( tex2DNode9_g84 * temp_output_14_0_g85 ) ) / ( temp_output_13_0_g85 + temp_output_14_0_g85 ) ) * temp_output_14_0_g95 ) ) / ( temp_output_13_0_g95 + temp_output_14_0_g95 ) ) * temp_output_13_0_g99 ) + ( ( ( ( ( ( ( tex2DNode11_g88 * temp_output_13_0_g89 ) + ( tex2DNode9_g88 * temp_output_14_0_g89 ) ) / ( temp_output_13_0_g89 + temp_output_14_0_g89 ) ) * temp_output_13_0_g96 ) + ( ( ( ( tex2DNode11_g76 * temp_output_13_0_g77 ) + ( tex2DNode9_g76 * temp_output_14_0_g77 ) ) / ( temp_output_13_0_g77 + temp_output_14_0_g77 ) ) * temp_output_14_0_g96 ) ) / ( temp_output_13_0_g96 + temp_output_14_0_g96 ) ) * temp_output_14_0_g99 ) ) / ( temp_output_13_0_g99 + temp_output_14_0_g99 ) );
				float4 ifLocalVar157_g104 = 0;
				if( temp_output_147_0_g104 > 0.0 )
				ifLocalVar157_g104 = ( tex2DNode133_g104 / tex2DNode133_g104.a );
				else if( temp_output_147_0_g104 < 0.0 )
				ifLocalVar157_g104 = ( temp_output_62_0 * temp_output_64_0 );
				
				float temp_output_7_0_g103 = ( temp_output_44_0.z + temp_output_61_0 );
				float temp_output_8_0_g103 = ( temp_output_47_0.z + break41.y );
				float temp_output_10_0_g103 = ( max( temp_output_7_0_g103 , temp_output_8_0_g103 ) - 0.2 );
				float temp_output_13_0_g103 = max( ( temp_output_7_0_g103 - temp_output_10_0_g103 ) , 0.0 );
				float temp_output_14_0_g103 = max( ( temp_output_8_0_g103 - temp_output_10_0_g103 ) , 0.0 );
				float4 break65 = ( ( ( temp_output_44_0 * temp_output_13_0_g103 ) + ( temp_output_47_0 * temp_output_14_0_g103 ) ) / ( temp_output_13_0_g103 + temp_output_14_0_g103 ) );
				float4 appendResult71 = (float4(0.0 , break65.y , break65.y , break65.w));
				
				float temp_output_3_0_g102 = ( temp_output_64_0.w * 2.0 );
				float ifLocalVar159_g104 = 0;
				if( temp_output_147_0_g104 < 0.0 )
				ifLocalVar159_g104 = max( ( temp_output_3_0_g102 - 1.0 ) , 0.0 );
				
				float ifLocalVar158_g104 = 0;
				UNITY_BRANCH 
				if( temp_output_147_0_g104 < 0.5 )
				ifLocalVar158_g104 = temp_output_62_0.w;
				
				float ifLocalVar169_g104 = 0;
				if( temp_output_147_0_g104 < 1.0 )
				ifLocalVar169_g104 = min( temp_output_3_0_g102 , 1.0 );
				
				surfaceDescription.Albedo = ifLocalVar157_g104.rgb;
				surfaceDescription.Normal = UnpackNormalScale( appendResult71, 1.0 );
				surfaceDescription.BentNormal = float3( 0, 0, 1 );
				surfaceDescription.CoatMask = 0;
				surfaceDescription.Metallic = ifLocalVar159_g104;

				#ifdef _MATERIAL_FEATURE_SPECULAR_COLOR
				surfaceDescription.Specular = 0;
				#endif

				surfaceDescription.Emission = 0;
				surfaceDescription.Smoothness = ifLocalVar158_g104;
				surfaceDescription.Occlusion = 1;
				surfaceDescription.Alpha = ifLocalVar169_g104;

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

			#define _BLENDMODE_PRESERVE_SPECULAR_LIGHTING 1
			#define _DISABLE_SSR_TRANSPARENT 1
			#define _SPECULAR_OCCLUSION_FROM_AO 1
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
			float4 _MaterialArray_ST;
			float4 _Control_TexelSize;
			float4 _ShapeArray_ST;
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
			sampler2D _Noise;
			TEXTURE2D_ARRAY(_MaterialArray);
			sampler2D _GrassControl;
			sampler2D _Control;
			SAMPLER(sampler_MaterialArray);
			TEXTURE2D_ARRAY(_ShapeArray);
			SAMPLER(sampler_ShapeArray);
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
				float dotResult145_g104 = dot( ase_worldNormal , float3( 0,1,0 ) );
				float3 ase_worldPos = packedInput.ase_texcoord3.xyz;
				float3 break180_g104 = ase_worldPos;
				float2 appendResult102_g104 = (float2(break180_g104.x , break180_g104.z));
				float4 break174_g104 = _WorldBounds;
				float2 appendResult129_g104 = (float2(break174_g104.x , break174_g104.y));
				float2 appendResult130_g104 = (float2(break174_g104.z , break174_g104.w));
				float4 tex2DNode133_g104 = tex2D( _SpatterTex, ( ( appendResult102_g104 - appendResult129_g104 ) / ( appendResult130_g104 - appendResult129_g104 ) ) );
				float lerpResult139_g104 = lerp( 1.0 , -1.0 , ( tex2DNode133_g104.a - tex2D( _Noise, appendResult102_g104 ).r ));
				float temp_output_147_0_g104 = ( dotResult145_g104 >= lerpResult139_g104 ? 1.0 : 0.0 );
				float2 uv_MaterialArray = packedInput.ase_texcoord4.xy;
				float3 break17 = ase_worldPos;
				float2 appendResult26 = (float2(break17.x , break17.z));
				float4 break18 = _WorldBounds;
				float2 appendResult27 = (float2(break18.x , break18.y));
				float2 appendResult28 = (float2(break18.w , break18.z));
				float2 appendResult35 = (float2(_Control_TexelSize.z , _Control_TexelSize.w));
				float2 temp_output_36_0 = ( ( ( appendResult26 - appendResult27 ) / ( appendResult28 - appendResult27 ) ) * appendResult35 );
				float2 temp_output_39_0 = ( floor( ( temp_output_36_0 + float2( 0.5,0.5 ) ) ) - float2( 0.5,0.5 ) );
				float2 temp_output_6_0_g80 = ( ( temp_output_39_0 + float2( 0,0 ) ) / appendResult35 );
				float4 tex2DNode10_g80 = tex2D( _GrassControl, temp_output_6_0_g80 );
				float2 uv_ShapeArray = packedInput.ase_texcoord4.xy;
				float4 tex2DArrayNode27_g80 = SAMPLE_TEXTURE2D_ARRAY( _ShapeArray, sampler_ShapeArray, uv_ShapeArray,( tex2DNode10_g80.g * 255.0 ) );
				float4 tex2DNode11_g80 = tex2D( _GrassTint, temp_output_6_0_g80 );
				float temp_output_7_0_g83 = ( tex2DArrayNode27_g80.b + tex2DNode11_g80.a );
				float4 tex2DNode8_g80 = tex2D( _Control, temp_output_6_0_g80 );
				float4 tex2DArrayNode24_g80 = SAMPLE_TEXTURE2D_ARRAY( _ShapeArray, sampler_ShapeArray, uv_ShapeArray,( tex2DNode8_g80.g * 255.0 ) );
				float temp_output_30_0_g80 = ( 1.0 - tex2DNode11_g80.a );
				float temp_output_8_0_g83 = ( tex2DArrayNode24_g80.b + temp_output_30_0_g80 );
				float temp_output_10_0_g83 = ( max( temp_output_7_0_g83 , temp_output_8_0_g83 ) - 0.2 );
				float temp_output_13_0_g83 = max( ( temp_output_7_0_g83 - temp_output_10_0_g83 ) , 0.0 );
				float4 tex2DArrayNode21_g80 = SAMPLE_TEXTURE2D_ARRAY( _MaterialArray, sampler_MaterialArray, uv_MaterialArray,( tex2DNode8_g80.r * 255.0 ) );
				float temp_output_14_0_g83 = max( ( temp_output_8_0_g83 - temp_output_10_0_g83 ) , 0.0 );
				float temp_output_7_0_g82 = ( tex2DArrayNode27_g80.b + tex2DNode11_g80.a );
				float temp_output_8_0_g82 = ( tex2DArrayNode24_g80.b + temp_output_30_0_g80 );
				float temp_output_10_0_g82 = ( max( temp_output_7_0_g82 , temp_output_8_0_g82 ) - 0.2 );
				float temp_output_13_0_g82 = max( ( temp_output_7_0_g82 - temp_output_10_0_g82 ) , 0.0 );
				float temp_output_14_0_g82 = max( ( temp_output_8_0_g82 - temp_output_10_0_g82 ) , 0.0 );
				float temp_output_77_42 = ( ( ( tex2DArrayNode27_g80 * temp_output_13_0_g82 ) + ( tex2DArrayNode24_g80 * temp_output_14_0_g82 ) ) / ( temp_output_13_0_g82 + temp_output_14_0_g82 ) ).z;
				float2 break41 = ( temp_output_36_0 - temp_output_39_0 );
				float temp_output_49_0 = ( 1.0 - break41.x );
				float temp_output_7_0_g97 = ( temp_output_77_42 + temp_output_49_0 );
				float2 temp_output_6_0_g84 = ( ( temp_output_39_0 + float2( 0,0 ) ) / appendResult35 );
				float4 tex2DNode10_g84 = tex2D( _GrassControl, temp_output_6_0_g84 );
				float4 tex2DArrayNode27_g84 = SAMPLE_TEXTURE2D_ARRAY( _ShapeArray, sampler_ShapeArray, uv_ShapeArray,( tex2DNode10_g84.g * 255.0 ) );
				float4 tex2DNode11_g84 = tex2D( _GrassTint, temp_output_6_0_g84 );
				float temp_output_7_0_g86 = ( tex2DArrayNode27_g84.b + tex2DNode11_g84.a );
				float4 tex2DNode8_g84 = tex2D( _Control, temp_output_6_0_g84 );
				float4 tex2DArrayNode24_g84 = SAMPLE_TEXTURE2D_ARRAY( _ShapeArray, sampler_ShapeArray, uv_ShapeArray,( tex2DNode8_g84.g * 255.0 ) );
				float temp_output_30_0_g84 = ( 1.0 - tex2DNode11_g84.a );
				float temp_output_8_0_g86 = ( tex2DArrayNode24_g84.b + temp_output_30_0_g84 );
				float temp_output_10_0_g86 = ( max( temp_output_7_0_g86 , temp_output_8_0_g86 ) - 0.2 );
				float temp_output_13_0_g86 = max( ( temp_output_7_0_g86 - temp_output_10_0_g86 ) , 0.0 );
				float temp_output_14_0_g86 = max( ( temp_output_8_0_g86 - temp_output_10_0_g86 ) , 0.0 );
				float temp_output_79_42 = ( ( ( tex2DArrayNode27_g84 * temp_output_13_0_g86 ) + ( tex2DArrayNode24_g84 * temp_output_14_0_g86 ) ) / ( temp_output_13_0_g86 + temp_output_14_0_g86 ) ).z;
				float temp_output_8_0_g97 = ( temp_output_79_42 + break41.x );
				float temp_output_10_0_g97 = ( max( temp_output_7_0_g97 , temp_output_8_0_g97 ) - 0.2 );
				float temp_output_13_0_g97 = max( ( temp_output_7_0_g97 - temp_output_10_0_g97 ) , 0.0 );
				float temp_output_7_0_g87 = ( tex2DArrayNode27_g84.b + tex2DNode11_g84.a );
				float temp_output_8_0_g87 = ( tex2DArrayNode24_g84.b + temp_output_30_0_g84 );
				float temp_output_10_0_g87 = ( max( temp_output_7_0_g87 , temp_output_8_0_g87 ) - 0.2 );
				float temp_output_13_0_g87 = max( ( temp_output_7_0_g87 - temp_output_10_0_g87 ) , 0.0 );
				float4 tex2DArrayNode21_g84 = SAMPLE_TEXTURE2D_ARRAY( _MaterialArray, sampler_MaterialArray, uv_MaterialArray,( tex2DNode8_g84.r * 255.0 ) );
				float temp_output_14_0_g87 = max( ( temp_output_8_0_g87 - temp_output_10_0_g87 ) , 0.0 );
				float temp_output_14_0_g97 = max( ( temp_output_8_0_g97 - temp_output_10_0_g97 ) , 0.0 );
				float temp_output_7_0_g93 = ( temp_output_77_42 + temp_output_49_0 );
				float temp_output_8_0_g93 = ( temp_output_79_42 + break41.x );
				float temp_output_10_0_g93 = ( max( temp_output_7_0_g93 , temp_output_8_0_g93 ) - 0.2 );
				float temp_output_13_0_g93 = max( ( temp_output_7_0_g93 - temp_output_10_0_g93 ) , 0.0 );
				float temp_output_14_0_g93 = max( ( temp_output_8_0_g93 - temp_output_10_0_g93 ) , 0.0 );
				float4 temp_output_44_0 = ( ( ( ( ( ( tex2DArrayNode27_g80 * temp_output_13_0_g82 ) + ( tex2DArrayNode24_g80 * temp_output_14_0_g82 ) ) / ( temp_output_13_0_g82 + temp_output_14_0_g82 ) ) * temp_output_13_0_g93 ) + ( ( ( ( tex2DArrayNode27_g84 * temp_output_13_0_g86 ) + ( tex2DArrayNode24_g84 * temp_output_14_0_g86 ) ) / ( temp_output_13_0_g86 + temp_output_14_0_g86 ) ) * temp_output_14_0_g93 ) ) / ( temp_output_13_0_g93 + temp_output_14_0_g93 ) );
				float temp_output_61_0 = ( 1.0 - break41.y );
				float temp_output_7_0_g100 = ( temp_output_44_0.z + temp_output_61_0 );
				float2 temp_output_6_0_g88 = ( ( temp_output_39_0 + float2( 0,0 ) ) / appendResult35 );
				float4 tex2DNode10_g88 = tex2D( _GrassControl, temp_output_6_0_g88 );
				float4 tex2DArrayNode27_g88 = SAMPLE_TEXTURE2D_ARRAY( _ShapeArray, sampler_ShapeArray, uv_ShapeArray,( tex2DNode10_g88.g * 255.0 ) );
				float4 tex2DNode11_g88 = tex2D( _GrassTint, temp_output_6_0_g88 );
				float temp_output_7_0_g90 = ( tex2DArrayNode27_g88.b + tex2DNode11_g88.a );
				float4 tex2DNode8_g88 = tex2D( _Control, temp_output_6_0_g88 );
				float4 tex2DArrayNode24_g88 = SAMPLE_TEXTURE2D_ARRAY( _ShapeArray, sampler_ShapeArray, uv_ShapeArray,( tex2DNode8_g88.g * 255.0 ) );
				float temp_output_30_0_g88 = ( 1.0 - tex2DNode11_g88.a );
				float temp_output_8_0_g90 = ( tex2DArrayNode24_g88.b + temp_output_30_0_g88 );
				float temp_output_10_0_g90 = ( max( temp_output_7_0_g90 , temp_output_8_0_g90 ) - 0.2 );
				float temp_output_13_0_g90 = max( ( temp_output_7_0_g90 - temp_output_10_0_g90 ) , 0.0 );
				float temp_output_14_0_g90 = max( ( temp_output_8_0_g90 - temp_output_10_0_g90 ) , 0.0 );
				float temp_output_80_42 = ( ( ( tex2DArrayNode27_g88 * temp_output_13_0_g90 ) + ( tex2DArrayNode24_g88 * temp_output_14_0_g90 ) ) / ( temp_output_13_0_g90 + temp_output_14_0_g90 ) ).z;
				float temp_output_7_0_g94 = ( temp_output_80_42 + temp_output_49_0 );
				float2 temp_output_6_0_g76 = ( ( temp_output_39_0 + float2( 0,0 ) ) / appendResult35 );
				float4 tex2DNode10_g76 = tex2D( _GrassControl, temp_output_6_0_g76 );
				float4 tex2DArrayNode27_g76 = SAMPLE_TEXTURE2D_ARRAY( _ShapeArray, sampler_ShapeArray, uv_ShapeArray,( tex2DNode10_g76.g * 255.0 ) );
				float4 tex2DNode11_g76 = tex2D( _GrassTint, temp_output_6_0_g76 );
				float temp_output_7_0_g78 = ( tex2DArrayNode27_g76.b + tex2DNode11_g76.a );
				float4 tex2DNode8_g76 = tex2D( _Control, temp_output_6_0_g76 );
				float4 tex2DArrayNode24_g76 = SAMPLE_TEXTURE2D_ARRAY( _ShapeArray, sampler_ShapeArray, uv_ShapeArray,( tex2DNode8_g76.g * 255.0 ) );
				float temp_output_30_0_g76 = ( 1.0 - tex2DNode11_g76.a );
				float temp_output_8_0_g78 = ( tex2DArrayNode24_g76.b + temp_output_30_0_g76 );
				float temp_output_10_0_g78 = ( max( temp_output_7_0_g78 , temp_output_8_0_g78 ) - 0.2 );
				float temp_output_13_0_g78 = max( ( temp_output_7_0_g78 - temp_output_10_0_g78 ) , 0.0 );
				float temp_output_14_0_g78 = max( ( temp_output_8_0_g78 - temp_output_10_0_g78 ) , 0.0 );
				float temp_output_78_42 = ( ( ( tex2DArrayNode27_g76 * temp_output_13_0_g78 ) + ( tex2DArrayNode24_g76 * temp_output_14_0_g78 ) ) / ( temp_output_13_0_g78 + temp_output_14_0_g78 ) ).z;
				float temp_output_8_0_g94 = ( temp_output_78_42 + break41.x );
				float temp_output_10_0_g94 = ( max( temp_output_7_0_g94 , temp_output_8_0_g94 ) - 0.2 );
				float temp_output_13_0_g94 = max( ( temp_output_7_0_g94 - temp_output_10_0_g94 ) , 0.0 );
				float temp_output_14_0_g94 = max( ( temp_output_8_0_g94 - temp_output_10_0_g94 ) , 0.0 );
				float4 temp_output_47_0 = ( ( ( ( ( ( tex2DArrayNode27_g88 * temp_output_13_0_g90 ) + ( tex2DArrayNode24_g88 * temp_output_14_0_g90 ) ) / ( temp_output_13_0_g90 + temp_output_14_0_g90 ) ) * temp_output_13_0_g94 ) + ( ( ( ( tex2DArrayNode27_g76 * temp_output_13_0_g78 ) + ( tex2DArrayNode24_g76 * temp_output_14_0_g78 ) ) / ( temp_output_13_0_g78 + temp_output_14_0_g78 ) ) * temp_output_14_0_g94 ) ) / ( temp_output_13_0_g94 + temp_output_14_0_g94 ) );
				float temp_output_8_0_g100 = ( temp_output_47_0.z + break41.y );
				float temp_output_10_0_g100 = ( max( temp_output_7_0_g100 , temp_output_8_0_g100 ) - 0.2 );
				float temp_output_13_0_g100 = max( ( temp_output_7_0_g100 - temp_output_10_0_g100 ) , 0.0 );
				float temp_output_7_0_g91 = ( tex2DArrayNode27_g88.b + tex2DNode11_g88.a );
				float temp_output_8_0_g91 = ( tex2DArrayNode24_g88.b + temp_output_30_0_g88 );
				float temp_output_10_0_g91 = ( max( temp_output_7_0_g91 , temp_output_8_0_g91 ) - 0.2 );
				float temp_output_13_0_g91 = max( ( temp_output_7_0_g91 - temp_output_10_0_g91 ) , 0.0 );
				float4 tex2DArrayNode21_g88 = SAMPLE_TEXTURE2D_ARRAY( _MaterialArray, sampler_MaterialArray, uv_MaterialArray,( tex2DNode8_g88.r * 255.0 ) );
				float temp_output_14_0_g91 = max( ( temp_output_8_0_g91 - temp_output_10_0_g91 ) , 0.0 );
				float temp_output_7_0_g98 = ( temp_output_80_42 + temp_output_49_0 );
				float temp_output_8_0_g98 = ( temp_output_78_42 + break41.x );
				float temp_output_10_0_g98 = ( max( temp_output_7_0_g98 , temp_output_8_0_g98 ) - 0.2 );
				float temp_output_13_0_g98 = max( ( temp_output_7_0_g98 - temp_output_10_0_g98 ) , 0.0 );
				float temp_output_7_0_g79 = ( tex2DArrayNode27_g76.b + tex2DNode11_g76.a );
				float temp_output_8_0_g79 = ( tex2DArrayNode24_g76.b + temp_output_30_0_g76 );
				float temp_output_10_0_g79 = ( max( temp_output_7_0_g79 , temp_output_8_0_g79 ) - 0.2 );
				float temp_output_13_0_g79 = max( ( temp_output_7_0_g79 - temp_output_10_0_g79 ) , 0.0 );
				float4 tex2DArrayNode21_g76 = SAMPLE_TEXTURE2D_ARRAY( _MaterialArray, sampler_MaterialArray, uv_MaterialArray,( tex2DNode8_g76.r * 255.0 ) );
				float temp_output_14_0_g79 = max( ( temp_output_8_0_g79 - temp_output_10_0_g79 ) , 0.0 );
				float temp_output_14_0_g98 = max( ( temp_output_8_0_g98 - temp_output_10_0_g98 ) , 0.0 );
				float temp_output_14_0_g100 = max( ( temp_output_8_0_g100 - temp_output_10_0_g100 ) , 0.0 );
				float4 temp_output_62_0 = ( ( ( ( ( ( ( ( ( SAMPLE_TEXTURE2D_ARRAY( _MaterialArray, sampler_MaterialArray, uv_MaterialArray,( tex2DNode10_g80.r * 255.0 ) ) * temp_output_13_0_g83 ) + ( tex2DArrayNode21_g80 * temp_output_14_0_g83 ) ) / ( temp_output_13_0_g83 + temp_output_14_0_g83 ) ) * temp_output_13_0_g97 ) + ( ( ( ( SAMPLE_TEXTURE2D_ARRAY( _MaterialArray, sampler_MaterialArray, uv_MaterialArray,( tex2DNode10_g84.r * 255.0 ) ) * temp_output_13_0_g87 ) + ( tex2DArrayNode21_g84 * temp_output_14_0_g87 ) ) / ( temp_output_13_0_g87 + temp_output_14_0_g87 ) ) * temp_output_14_0_g97 ) ) / ( temp_output_13_0_g97 + temp_output_14_0_g97 ) ) * temp_output_13_0_g100 ) + ( ( ( ( ( ( ( SAMPLE_TEXTURE2D_ARRAY( _MaterialArray, sampler_MaterialArray, uv_MaterialArray,( tex2DNode10_g88.r * 255.0 ) ) * temp_output_13_0_g91 ) + ( tex2DArrayNode21_g88 * temp_output_14_0_g91 ) ) / ( temp_output_13_0_g91 + temp_output_14_0_g91 ) ) * temp_output_13_0_g98 ) + ( ( ( ( SAMPLE_TEXTURE2D_ARRAY( _MaterialArray, sampler_MaterialArray, uv_MaterialArray,( tex2DNode10_g76.r * 255.0 ) ) * temp_output_13_0_g79 ) + ( tex2DArrayNode21_g76 * temp_output_14_0_g79 ) ) / ( temp_output_13_0_g79 + temp_output_14_0_g79 ) ) * temp_output_14_0_g98 ) ) / ( temp_output_13_0_g98 + temp_output_14_0_g98 ) ) * temp_output_14_0_g100 ) ) / ( temp_output_13_0_g100 + temp_output_14_0_g100 ) );
				float temp_output_7_0_g81 = ( tex2DArrayNode27_g80.b + tex2DNode11_g80.a );
				float temp_output_8_0_g81 = ( tex2DArrayNode24_g80.b + temp_output_30_0_g80 );
				float temp_output_10_0_g81 = ( max( temp_output_7_0_g81 , temp_output_8_0_g81 ) - 0.2 );
				float temp_output_13_0_g81 = max( ( temp_output_7_0_g81 - temp_output_10_0_g81 ) , 0.0 );
				float4 tex2DNode9_g80 = tex2D( _Tint, temp_output_6_0_g80 );
				float temp_output_14_0_g81 = max( ( temp_output_8_0_g81 - temp_output_10_0_g81 ) , 0.0 );
				float temp_output_7_0_g95 = ( temp_output_77_42 + temp_output_49_0 );
				float temp_output_8_0_g95 = ( temp_output_79_42 + break41.x );
				float temp_output_10_0_g95 = ( max( temp_output_7_0_g95 , temp_output_8_0_g95 ) - 0.2 );
				float temp_output_13_0_g95 = max( ( temp_output_7_0_g95 - temp_output_10_0_g95 ) , 0.0 );
				float temp_output_7_0_g85 = ( tex2DArrayNode27_g84.b + tex2DNode11_g84.a );
				float temp_output_8_0_g85 = ( tex2DArrayNode24_g84.b + temp_output_30_0_g84 );
				float temp_output_10_0_g85 = ( max( temp_output_7_0_g85 , temp_output_8_0_g85 ) - 0.2 );
				float temp_output_13_0_g85 = max( ( temp_output_7_0_g85 - temp_output_10_0_g85 ) , 0.0 );
				float4 tex2DNode9_g84 = tex2D( _Tint, temp_output_6_0_g84 );
				float temp_output_14_0_g85 = max( ( temp_output_8_0_g85 - temp_output_10_0_g85 ) , 0.0 );
				float temp_output_14_0_g95 = max( ( temp_output_8_0_g95 - temp_output_10_0_g95 ) , 0.0 );
				float temp_output_7_0_g99 = ( temp_output_44_0.z + temp_output_61_0 );
				float temp_output_8_0_g99 = ( temp_output_47_0.z + break41.y );
				float temp_output_10_0_g99 = ( max( temp_output_7_0_g99 , temp_output_8_0_g99 ) - 0.2 );
				float temp_output_13_0_g99 = max( ( temp_output_7_0_g99 - temp_output_10_0_g99 ) , 0.0 );
				float temp_output_7_0_g89 = ( tex2DArrayNode27_g88.b + tex2DNode11_g88.a );
				float temp_output_8_0_g89 = ( tex2DArrayNode24_g88.b + temp_output_30_0_g88 );
				float temp_output_10_0_g89 = ( max( temp_output_7_0_g89 , temp_output_8_0_g89 ) - 0.2 );
				float temp_output_13_0_g89 = max( ( temp_output_7_0_g89 - temp_output_10_0_g89 ) , 0.0 );
				float4 tex2DNode9_g88 = tex2D( _Tint, temp_output_6_0_g88 );
				float temp_output_14_0_g89 = max( ( temp_output_8_0_g89 - temp_output_10_0_g89 ) , 0.0 );
				float temp_output_7_0_g96 = ( temp_output_80_42 + temp_output_49_0 );
				float temp_output_8_0_g96 = ( temp_output_78_42 + break41.x );
				float temp_output_10_0_g96 = ( max( temp_output_7_0_g96 , temp_output_8_0_g96 ) - 0.2 );
				float temp_output_13_0_g96 = max( ( temp_output_7_0_g96 - temp_output_10_0_g96 ) , 0.0 );
				float temp_output_7_0_g77 = ( tex2DArrayNode27_g76.b + tex2DNode11_g76.a );
				float temp_output_8_0_g77 = ( tex2DArrayNode24_g76.b + temp_output_30_0_g76 );
				float temp_output_10_0_g77 = ( max( temp_output_7_0_g77 , temp_output_8_0_g77 ) - 0.2 );
				float temp_output_13_0_g77 = max( ( temp_output_7_0_g77 - temp_output_10_0_g77 ) , 0.0 );
				float4 tex2DNode9_g76 = tex2D( _Tint, temp_output_6_0_g76 );
				float temp_output_14_0_g77 = max( ( temp_output_8_0_g77 - temp_output_10_0_g77 ) , 0.0 );
				float temp_output_14_0_g96 = max( ( temp_output_8_0_g96 - temp_output_10_0_g96 ) , 0.0 );
				float temp_output_14_0_g99 = max( ( temp_output_8_0_g99 - temp_output_10_0_g99 ) , 0.0 );
				float4 temp_output_64_0 = ( ( ( ( ( ( ( ( ( tex2DNode11_g80 * temp_output_13_0_g81 ) + ( tex2DNode9_g80 * temp_output_14_0_g81 ) ) / ( temp_output_13_0_g81 + temp_output_14_0_g81 ) ) * temp_output_13_0_g95 ) + ( ( ( ( tex2DNode11_g84 * temp_output_13_0_g85 ) + ( tex2DNode9_g84 * temp_output_14_0_g85 ) ) / ( temp_output_13_0_g85 + temp_output_14_0_g85 ) ) * temp_output_14_0_g95 ) ) / ( temp_output_13_0_g95 + temp_output_14_0_g95 ) ) * temp_output_13_0_g99 ) + ( ( ( ( ( ( ( tex2DNode11_g88 * temp_output_13_0_g89 ) + ( tex2DNode9_g88 * temp_output_14_0_g89 ) ) / ( temp_output_13_0_g89 + temp_output_14_0_g89 ) ) * temp_output_13_0_g96 ) + ( ( ( ( tex2DNode11_g76 * temp_output_13_0_g77 ) + ( tex2DNode9_g76 * temp_output_14_0_g77 ) ) / ( temp_output_13_0_g77 + temp_output_14_0_g77 ) ) * temp_output_14_0_g96 ) ) / ( temp_output_13_0_g96 + temp_output_14_0_g96 ) ) * temp_output_14_0_g99 ) ) / ( temp_output_13_0_g99 + temp_output_14_0_g99 ) );
				float4 ifLocalVar157_g104 = 0;
				if( temp_output_147_0_g104 > 0.0 )
				ifLocalVar157_g104 = ( tex2DNode133_g104 / tex2DNode133_g104.a );
				else if( temp_output_147_0_g104 < 0.0 )
				ifLocalVar157_g104 = ( temp_output_62_0 * temp_output_64_0 );
				
				float temp_output_7_0_g103 = ( temp_output_44_0.z + temp_output_61_0 );
				float temp_output_8_0_g103 = ( temp_output_47_0.z + break41.y );
				float temp_output_10_0_g103 = ( max( temp_output_7_0_g103 , temp_output_8_0_g103 ) - 0.2 );
				float temp_output_13_0_g103 = max( ( temp_output_7_0_g103 - temp_output_10_0_g103 ) , 0.0 );
				float temp_output_14_0_g103 = max( ( temp_output_8_0_g103 - temp_output_10_0_g103 ) , 0.0 );
				float4 break65 = ( ( ( temp_output_44_0 * temp_output_13_0_g103 ) + ( temp_output_47_0 * temp_output_14_0_g103 ) ) / ( temp_output_13_0_g103 + temp_output_14_0_g103 ) );
				float4 appendResult71 = (float4(0.0 , break65.y , break65.y , break65.w));
				
				float temp_output_3_0_g102 = ( temp_output_64_0.w * 2.0 );
				float ifLocalVar159_g104 = 0;
				if( temp_output_147_0_g104 < 0.0 )
				ifLocalVar159_g104 = max( ( temp_output_3_0_g102 - 1.0 ) , 0.0 );
				
				float ifLocalVar158_g104 = 0;
				UNITY_BRANCH 
				if( temp_output_147_0_g104 < 0.5 )
				ifLocalVar158_g104 = temp_output_62_0.w;
				
				float ifLocalVar169_g104 = 0;
				if( temp_output_147_0_g104 < 1.0 )
				ifLocalVar169_g104 = min( temp_output_3_0_g102 , 1.0 );
				
				surfaceDescription.Albedo = ifLocalVar157_g104.rgb;
				surfaceDescription.Normal = UnpackNormalScale( appendResult71, 1.0 );
				surfaceDescription.BentNormal = float3( 0, 0, 1 );
				surfaceDescription.CoatMask = 0;
				surfaceDescription.Metallic = ifLocalVar159_g104;

				#ifdef _MATERIAL_FEATURE_SPECULAR_COLOR
				surfaceDescription.Specular = 0;
				#endif

				surfaceDescription.Emission = 0;
				surfaceDescription.Smoothness = ifLocalVar158_g104;
				surfaceDescription.Occlusion = 1;
				surfaceDescription.Alpha = ifLocalVar169_g104;

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

			#define _BLENDMODE_PRESERVE_SPECULAR_LIGHTING 1
			#define _DISABLE_SSR_TRANSPARENT 1
			#define _SPECULAR_OCCLUSION_FROM_AO 1
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
			float4 _MaterialArray_ST;
			float4 _Control_TexelSize;
			float4 _ShapeArray_ST;
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
			sampler2D _Noise;
			sampler2D _GrassTint;
			sampler2D _Control;
			TEXTURE2D_ARRAY(_ShapeArray);
			sampler2D _GrassControl;
			SAMPLER(sampler_ShapeArray);
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
			#define ASE_NEEDS_FRAG_RELATIVE_WORLD_POS


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
				float4 ase_texcoord2 : TEXCOORD2;
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
				
				outputPackedVaryingsMeshToPS.ase_texcoord2.xy = inputMesh.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				outputPackedVaryingsMeshToPS.ase_texcoord1.w = 0;
				outputPackedVaryingsMeshToPS.ase_texcoord2.zw = 0;

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
				float3 ase_worldNormal = packedInput.ase_texcoord1.xyz;
				float dotResult145_g104 = dot( ase_worldNormal , float3( 0,1,0 ) );
				float3 ase_worldPos = GetAbsolutePositionWS( positionRWS );
				float3 break180_g104 = ase_worldPos;
				float2 appendResult102_g104 = (float2(break180_g104.x , break180_g104.z));
				float4 break174_g104 = _WorldBounds;
				float2 appendResult129_g104 = (float2(break174_g104.x , break174_g104.y));
				float2 appendResult130_g104 = (float2(break174_g104.z , break174_g104.w));
				float4 tex2DNode133_g104 = tex2D( _SpatterTex, ( ( appendResult102_g104 - appendResult129_g104 ) / ( appendResult130_g104 - appendResult129_g104 ) ) );
				float lerpResult139_g104 = lerp( 1.0 , -1.0 , ( tex2DNode133_g104.a - tex2D( _Noise, appendResult102_g104 ).r ));
				float temp_output_147_0_g104 = ( dotResult145_g104 >= lerpResult139_g104 ? 1.0 : 0.0 );
				float3 break17 = ase_worldPos;
				float2 appendResult26 = (float2(break17.x , break17.z));
				float4 break18 = _WorldBounds;
				float2 appendResult27 = (float2(break18.x , break18.y));
				float2 appendResult28 = (float2(break18.w , break18.z));
				float2 appendResult35 = (float2(_Control_TexelSize.z , _Control_TexelSize.w));
				float2 temp_output_36_0 = ( ( ( appendResult26 - appendResult27 ) / ( appendResult28 - appendResult27 ) ) * appendResult35 );
				float2 temp_output_39_0 = ( floor( ( temp_output_36_0 + float2( 0.5,0.5 ) ) ) - float2( 0.5,0.5 ) );
				float2 temp_output_6_0_g80 = ( ( temp_output_39_0 + float2( 0,0 ) ) / appendResult35 );
				float4 tex2DNode11_g80 = tex2D( _GrassTint, temp_output_6_0_g80 );
				float2 uv_ShapeArray = packedInput.ase_texcoord2.xy;
				float4 tex2DNode10_g80 = tex2D( _GrassControl, temp_output_6_0_g80 );
				float4 tex2DArrayNode27_g80 = SAMPLE_TEXTURE2D_ARRAY( _ShapeArray, sampler_ShapeArray, uv_ShapeArray,( tex2DNode10_g80.g * 255.0 ) );
				float temp_output_7_0_g81 = ( tex2DArrayNode27_g80.b + tex2DNode11_g80.a );
				float4 tex2DNode8_g80 = tex2D( _Control, temp_output_6_0_g80 );
				float4 tex2DArrayNode24_g80 = SAMPLE_TEXTURE2D_ARRAY( _ShapeArray, sampler_ShapeArray, uv_ShapeArray,( tex2DNode8_g80.g * 255.0 ) );
				float temp_output_30_0_g80 = ( 1.0 - tex2DNode11_g80.a );
				float temp_output_8_0_g81 = ( tex2DArrayNode24_g80.b + temp_output_30_0_g80 );
				float temp_output_10_0_g81 = ( max( temp_output_7_0_g81 , temp_output_8_0_g81 ) - 0.2 );
				float temp_output_13_0_g81 = max( ( temp_output_7_0_g81 - temp_output_10_0_g81 ) , 0.0 );
				float4 tex2DNode9_g80 = tex2D( _Tint, temp_output_6_0_g80 );
				float temp_output_14_0_g81 = max( ( temp_output_8_0_g81 - temp_output_10_0_g81 ) , 0.0 );
				float temp_output_7_0_g82 = ( tex2DArrayNode27_g80.b + tex2DNode11_g80.a );
				float temp_output_8_0_g82 = ( tex2DArrayNode24_g80.b + temp_output_30_0_g80 );
				float temp_output_10_0_g82 = ( max( temp_output_7_0_g82 , temp_output_8_0_g82 ) - 0.2 );
				float temp_output_13_0_g82 = max( ( temp_output_7_0_g82 - temp_output_10_0_g82 ) , 0.0 );
				float temp_output_14_0_g82 = max( ( temp_output_8_0_g82 - temp_output_10_0_g82 ) , 0.0 );
				float temp_output_77_42 = ( ( ( tex2DArrayNode27_g80 * temp_output_13_0_g82 ) + ( tex2DArrayNode24_g80 * temp_output_14_0_g82 ) ) / ( temp_output_13_0_g82 + temp_output_14_0_g82 ) ).z;
				float2 break41 = ( temp_output_36_0 - temp_output_39_0 );
				float temp_output_49_0 = ( 1.0 - break41.x );
				float temp_output_7_0_g95 = ( temp_output_77_42 + temp_output_49_0 );
				float2 temp_output_6_0_g84 = ( ( temp_output_39_0 + float2( 0,0 ) ) / appendResult35 );
				float4 tex2DNode10_g84 = tex2D( _GrassControl, temp_output_6_0_g84 );
				float4 tex2DArrayNode27_g84 = SAMPLE_TEXTURE2D_ARRAY( _ShapeArray, sampler_ShapeArray, uv_ShapeArray,( tex2DNode10_g84.g * 255.0 ) );
				float4 tex2DNode11_g84 = tex2D( _GrassTint, temp_output_6_0_g84 );
				float temp_output_7_0_g86 = ( tex2DArrayNode27_g84.b + tex2DNode11_g84.a );
				float4 tex2DNode8_g84 = tex2D( _Control, temp_output_6_0_g84 );
				float4 tex2DArrayNode24_g84 = SAMPLE_TEXTURE2D_ARRAY( _ShapeArray, sampler_ShapeArray, uv_ShapeArray,( tex2DNode8_g84.g * 255.0 ) );
				float temp_output_30_0_g84 = ( 1.0 - tex2DNode11_g84.a );
				float temp_output_8_0_g86 = ( tex2DArrayNode24_g84.b + temp_output_30_0_g84 );
				float temp_output_10_0_g86 = ( max( temp_output_7_0_g86 , temp_output_8_0_g86 ) - 0.2 );
				float temp_output_13_0_g86 = max( ( temp_output_7_0_g86 - temp_output_10_0_g86 ) , 0.0 );
				float temp_output_14_0_g86 = max( ( temp_output_8_0_g86 - temp_output_10_0_g86 ) , 0.0 );
				float temp_output_79_42 = ( ( ( tex2DArrayNode27_g84 * temp_output_13_0_g86 ) + ( tex2DArrayNode24_g84 * temp_output_14_0_g86 ) ) / ( temp_output_13_0_g86 + temp_output_14_0_g86 ) ).z;
				float temp_output_8_0_g95 = ( temp_output_79_42 + break41.x );
				float temp_output_10_0_g95 = ( max( temp_output_7_0_g95 , temp_output_8_0_g95 ) - 0.2 );
				float temp_output_13_0_g95 = max( ( temp_output_7_0_g95 - temp_output_10_0_g95 ) , 0.0 );
				float temp_output_7_0_g85 = ( tex2DArrayNode27_g84.b + tex2DNode11_g84.a );
				float temp_output_8_0_g85 = ( tex2DArrayNode24_g84.b + temp_output_30_0_g84 );
				float temp_output_10_0_g85 = ( max( temp_output_7_0_g85 , temp_output_8_0_g85 ) - 0.2 );
				float temp_output_13_0_g85 = max( ( temp_output_7_0_g85 - temp_output_10_0_g85 ) , 0.0 );
				float4 tex2DNode9_g84 = tex2D( _Tint, temp_output_6_0_g84 );
				float temp_output_14_0_g85 = max( ( temp_output_8_0_g85 - temp_output_10_0_g85 ) , 0.0 );
				float temp_output_14_0_g95 = max( ( temp_output_8_0_g95 - temp_output_10_0_g95 ) , 0.0 );
				float temp_output_7_0_g93 = ( temp_output_77_42 + temp_output_49_0 );
				float temp_output_8_0_g93 = ( temp_output_79_42 + break41.x );
				float temp_output_10_0_g93 = ( max( temp_output_7_0_g93 , temp_output_8_0_g93 ) - 0.2 );
				float temp_output_13_0_g93 = max( ( temp_output_7_0_g93 - temp_output_10_0_g93 ) , 0.0 );
				float temp_output_14_0_g93 = max( ( temp_output_8_0_g93 - temp_output_10_0_g93 ) , 0.0 );
				float4 temp_output_44_0 = ( ( ( ( ( ( tex2DArrayNode27_g80 * temp_output_13_0_g82 ) + ( tex2DArrayNode24_g80 * temp_output_14_0_g82 ) ) / ( temp_output_13_0_g82 + temp_output_14_0_g82 ) ) * temp_output_13_0_g93 ) + ( ( ( ( tex2DArrayNode27_g84 * temp_output_13_0_g86 ) + ( tex2DArrayNode24_g84 * temp_output_14_0_g86 ) ) / ( temp_output_13_0_g86 + temp_output_14_0_g86 ) ) * temp_output_14_0_g93 ) ) / ( temp_output_13_0_g93 + temp_output_14_0_g93 ) );
				float temp_output_61_0 = ( 1.0 - break41.y );
				float temp_output_7_0_g99 = ( temp_output_44_0.z + temp_output_61_0 );
				float2 temp_output_6_0_g88 = ( ( temp_output_39_0 + float2( 0,0 ) ) / appendResult35 );
				float4 tex2DNode10_g88 = tex2D( _GrassControl, temp_output_6_0_g88 );
				float4 tex2DArrayNode27_g88 = SAMPLE_TEXTURE2D_ARRAY( _ShapeArray, sampler_ShapeArray, uv_ShapeArray,( tex2DNode10_g88.g * 255.0 ) );
				float4 tex2DNode11_g88 = tex2D( _GrassTint, temp_output_6_0_g88 );
				float temp_output_7_0_g90 = ( tex2DArrayNode27_g88.b + tex2DNode11_g88.a );
				float4 tex2DNode8_g88 = tex2D( _Control, temp_output_6_0_g88 );
				float4 tex2DArrayNode24_g88 = SAMPLE_TEXTURE2D_ARRAY( _ShapeArray, sampler_ShapeArray, uv_ShapeArray,( tex2DNode8_g88.g * 255.0 ) );
				float temp_output_30_0_g88 = ( 1.0 - tex2DNode11_g88.a );
				float temp_output_8_0_g90 = ( tex2DArrayNode24_g88.b + temp_output_30_0_g88 );
				float temp_output_10_0_g90 = ( max( temp_output_7_0_g90 , temp_output_8_0_g90 ) - 0.2 );
				float temp_output_13_0_g90 = max( ( temp_output_7_0_g90 - temp_output_10_0_g90 ) , 0.0 );
				float temp_output_14_0_g90 = max( ( temp_output_8_0_g90 - temp_output_10_0_g90 ) , 0.0 );
				float temp_output_80_42 = ( ( ( tex2DArrayNode27_g88 * temp_output_13_0_g90 ) + ( tex2DArrayNode24_g88 * temp_output_14_0_g90 ) ) / ( temp_output_13_0_g90 + temp_output_14_0_g90 ) ).z;
				float temp_output_7_0_g94 = ( temp_output_80_42 + temp_output_49_0 );
				float2 temp_output_6_0_g76 = ( ( temp_output_39_0 + float2( 0,0 ) ) / appendResult35 );
				float4 tex2DNode10_g76 = tex2D( _GrassControl, temp_output_6_0_g76 );
				float4 tex2DArrayNode27_g76 = SAMPLE_TEXTURE2D_ARRAY( _ShapeArray, sampler_ShapeArray, uv_ShapeArray,( tex2DNode10_g76.g * 255.0 ) );
				float4 tex2DNode11_g76 = tex2D( _GrassTint, temp_output_6_0_g76 );
				float temp_output_7_0_g78 = ( tex2DArrayNode27_g76.b + tex2DNode11_g76.a );
				float4 tex2DNode8_g76 = tex2D( _Control, temp_output_6_0_g76 );
				float4 tex2DArrayNode24_g76 = SAMPLE_TEXTURE2D_ARRAY( _ShapeArray, sampler_ShapeArray, uv_ShapeArray,( tex2DNode8_g76.g * 255.0 ) );
				float temp_output_30_0_g76 = ( 1.0 - tex2DNode11_g76.a );
				float temp_output_8_0_g78 = ( tex2DArrayNode24_g76.b + temp_output_30_0_g76 );
				float temp_output_10_0_g78 = ( max( temp_output_7_0_g78 , temp_output_8_0_g78 ) - 0.2 );
				float temp_output_13_0_g78 = max( ( temp_output_7_0_g78 - temp_output_10_0_g78 ) , 0.0 );
				float temp_output_14_0_g78 = max( ( temp_output_8_0_g78 - temp_output_10_0_g78 ) , 0.0 );
				float temp_output_78_42 = ( ( ( tex2DArrayNode27_g76 * temp_output_13_0_g78 ) + ( tex2DArrayNode24_g76 * temp_output_14_0_g78 ) ) / ( temp_output_13_0_g78 + temp_output_14_0_g78 ) ).z;
				float temp_output_8_0_g94 = ( temp_output_78_42 + break41.x );
				float temp_output_10_0_g94 = ( max( temp_output_7_0_g94 , temp_output_8_0_g94 ) - 0.2 );
				float temp_output_13_0_g94 = max( ( temp_output_7_0_g94 - temp_output_10_0_g94 ) , 0.0 );
				float temp_output_14_0_g94 = max( ( temp_output_8_0_g94 - temp_output_10_0_g94 ) , 0.0 );
				float4 temp_output_47_0 = ( ( ( ( ( ( tex2DArrayNode27_g88 * temp_output_13_0_g90 ) + ( tex2DArrayNode24_g88 * temp_output_14_0_g90 ) ) / ( temp_output_13_0_g90 + temp_output_14_0_g90 ) ) * temp_output_13_0_g94 ) + ( ( ( ( tex2DArrayNode27_g76 * temp_output_13_0_g78 ) + ( tex2DArrayNode24_g76 * temp_output_14_0_g78 ) ) / ( temp_output_13_0_g78 + temp_output_14_0_g78 ) ) * temp_output_14_0_g94 ) ) / ( temp_output_13_0_g94 + temp_output_14_0_g94 ) );
				float temp_output_8_0_g99 = ( temp_output_47_0.z + break41.y );
				float temp_output_10_0_g99 = ( max( temp_output_7_0_g99 , temp_output_8_0_g99 ) - 0.2 );
				float temp_output_13_0_g99 = max( ( temp_output_7_0_g99 - temp_output_10_0_g99 ) , 0.0 );
				float temp_output_7_0_g89 = ( tex2DArrayNode27_g88.b + tex2DNode11_g88.a );
				float temp_output_8_0_g89 = ( tex2DArrayNode24_g88.b + temp_output_30_0_g88 );
				float temp_output_10_0_g89 = ( max( temp_output_7_0_g89 , temp_output_8_0_g89 ) - 0.2 );
				float temp_output_13_0_g89 = max( ( temp_output_7_0_g89 - temp_output_10_0_g89 ) , 0.0 );
				float4 tex2DNode9_g88 = tex2D( _Tint, temp_output_6_0_g88 );
				float temp_output_14_0_g89 = max( ( temp_output_8_0_g89 - temp_output_10_0_g89 ) , 0.0 );
				float temp_output_7_0_g96 = ( temp_output_80_42 + temp_output_49_0 );
				float temp_output_8_0_g96 = ( temp_output_78_42 + break41.x );
				float temp_output_10_0_g96 = ( max( temp_output_7_0_g96 , temp_output_8_0_g96 ) - 0.2 );
				float temp_output_13_0_g96 = max( ( temp_output_7_0_g96 - temp_output_10_0_g96 ) , 0.0 );
				float temp_output_7_0_g77 = ( tex2DArrayNode27_g76.b + tex2DNode11_g76.a );
				float temp_output_8_0_g77 = ( tex2DArrayNode24_g76.b + temp_output_30_0_g76 );
				float temp_output_10_0_g77 = ( max( temp_output_7_0_g77 , temp_output_8_0_g77 ) - 0.2 );
				float temp_output_13_0_g77 = max( ( temp_output_7_0_g77 - temp_output_10_0_g77 ) , 0.0 );
				float4 tex2DNode9_g76 = tex2D( _Tint, temp_output_6_0_g76 );
				float temp_output_14_0_g77 = max( ( temp_output_8_0_g77 - temp_output_10_0_g77 ) , 0.0 );
				float temp_output_14_0_g96 = max( ( temp_output_8_0_g96 - temp_output_10_0_g96 ) , 0.0 );
				float temp_output_14_0_g99 = max( ( temp_output_8_0_g99 - temp_output_10_0_g99 ) , 0.0 );
				float4 temp_output_64_0 = ( ( ( ( ( ( ( ( ( tex2DNode11_g80 * temp_output_13_0_g81 ) + ( tex2DNode9_g80 * temp_output_14_0_g81 ) ) / ( temp_output_13_0_g81 + temp_output_14_0_g81 ) ) * temp_output_13_0_g95 ) + ( ( ( ( tex2DNode11_g84 * temp_output_13_0_g85 ) + ( tex2DNode9_g84 * temp_output_14_0_g85 ) ) / ( temp_output_13_0_g85 + temp_output_14_0_g85 ) ) * temp_output_14_0_g95 ) ) / ( temp_output_13_0_g95 + temp_output_14_0_g95 ) ) * temp_output_13_0_g99 ) + ( ( ( ( ( ( ( tex2DNode11_g88 * temp_output_13_0_g89 ) + ( tex2DNode9_g88 * temp_output_14_0_g89 ) ) / ( temp_output_13_0_g89 + temp_output_14_0_g89 ) ) * temp_output_13_0_g96 ) + ( ( ( ( tex2DNode11_g76 * temp_output_13_0_g77 ) + ( tex2DNode9_g76 * temp_output_14_0_g77 ) ) / ( temp_output_13_0_g77 + temp_output_14_0_g77 ) ) * temp_output_14_0_g96 ) ) / ( temp_output_13_0_g96 + temp_output_14_0_g96 ) ) * temp_output_14_0_g99 ) ) / ( temp_output_13_0_g99 + temp_output_14_0_g99 ) );
				float temp_output_3_0_g102 = ( temp_output_64_0.w * 2.0 );
				float ifLocalVar169_g104 = 0;
				if( temp_output_147_0_g104 < 1.0 )
				ifLocalVar169_g104 = min( temp_output_3_0_g102 , 1.0 );
				
				surfaceDescription.Alpha = ifLocalVar169_g104;

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

			#define _BLENDMODE_PRESERVE_SPECULAR_LIGHTING 1
			#define _DISABLE_SSR_TRANSPARENT 1
			#define _SPECULAR_OCCLUSION_FROM_AO 1
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
			float4 _MaterialArray_ST;
			float4 _Control_TexelSize;
			float4 _ShapeArray_ST;
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
			sampler2D _Noise;
			sampler2D _GrassTint;
			sampler2D _Control;
			TEXTURE2D_ARRAY(_ShapeArray);
			sampler2D _GrassControl;
			SAMPLER(sampler_ShapeArray);
			sampler2D _Tint;


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
				float4 ase_texcoord2 : TEXCOORD2;
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
				
				outputPackedVaryingsMeshToPS.ase_texcoord2.xy = inputMesh.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				outputPackedVaryingsMeshToPS.ase_texcoord1.w = 0;
				outputPackedVaryingsMeshToPS.ase_texcoord2.zw = 0;

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
				float3 ase_worldNormal = packedInput.ase_texcoord1.xyz;
				float dotResult145_g104 = dot( ase_worldNormal , float3( 0,1,0 ) );
				float3 ase_worldPos = GetAbsolutePositionWS( positionRWS );
				float3 break180_g104 = ase_worldPos;
				float2 appendResult102_g104 = (float2(break180_g104.x , break180_g104.z));
				float4 break174_g104 = _WorldBounds;
				float2 appendResult129_g104 = (float2(break174_g104.x , break174_g104.y));
				float2 appendResult130_g104 = (float2(break174_g104.z , break174_g104.w));
				float4 tex2DNode133_g104 = tex2D( _SpatterTex, ( ( appendResult102_g104 - appendResult129_g104 ) / ( appendResult130_g104 - appendResult129_g104 ) ) );
				float lerpResult139_g104 = lerp( 1.0 , -1.0 , ( tex2DNode133_g104.a - tex2D( _Noise, appendResult102_g104 ).r ));
				float temp_output_147_0_g104 = ( dotResult145_g104 >= lerpResult139_g104 ? 1.0 : 0.0 );
				float3 break17 = ase_worldPos;
				float2 appendResult26 = (float2(break17.x , break17.z));
				float4 break18 = _WorldBounds;
				float2 appendResult27 = (float2(break18.x , break18.y));
				float2 appendResult28 = (float2(break18.w , break18.z));
				float2 appendResult35 = (float2(_Control_TexelSize.z , _Control_TexelSize.w));
				float2 temp_output_36_0 = ( ( ( appendResult26 - appendResult27 ) / ( appendResult28 - appendResult27 ) ) * appendResult35 );
				float2 temp_output_39_0 = ( floor( ( temp_output_36_0 + float2( 0.5,0.5 ) ) ) - float2( 0.5,0.5 ) );
				float2 temp_output_6_0_g80 = ( ( temp_output_39_0 + float2( 0,0 ) ) / appendResult35 );
				float4 tex2DNode11_g80 = tex2D( _GrassTint, temp_output_6_0_g80 );
				float2 uv_ShapeArray = packedInput.ase_texcoord2.xy;
				float4 tex2DNode10_g80 = tex2D( _GrassControl, temp_output_6_0_g80 );
				float4 tex2DArrayNode27_g80 = SAMPLE_TEXTURE2D_ARRAY( _ShapeArray, sampler_ShapeArray, uv_ShapeArray,( tex2DNode10_g80.g * 255.0 ) );
				float temp_output_7_0_g81 = ( tex2DArrayNode27_g80.b + tex2DNode11_g80.a );
				float4 tex2DNode8_g80 = tex2D( _Control, temp_output_6_0_g80 );
				float4 tex2DArrayNode24_g80 = SAMPLE_TEXTURE2D_ARRAY( _ShapeArray, sampler_ShapeArray, uv_ShapeArray,( tex2DNode8_g80.g * 255.0 ) );
				float temp_output_30_0_g80 = ( 1.0 - tex2DNode11_g80.a );
				float temp_output_8_0_g81 = ( tex2DArrayNode24_g80.b + temp_output_30_0_g80 );
				float temp_output_10_0_g81 = ( max( temp_output_7_0_g81 , temp_output_8_0_g81 ) - 0.2 );
				float temp_output_13_0_g81 = max( ( temp_output_7_0_g81 - temp_output_10_0_g81 ) , 0.0 );
				float4 tex2DNode9_g80 = tex2D( _Tint, temp_output_6_0_g80 );
				float temp_output_14_0_g81 = max( ( temp_output_8_0_g81 - temp_output_10_0_g81 ) , 0.0 );
				float temp_output_7_0_g82 = ( tex2DArrayNode27_g80.b + tex2DNode11_g80.a );
				float temp_output_8_0_g82 = ( tex2DArrayNode24_g80.b + temp_output_30_0_g80 );
				float temp_output_10_0_g82 = ( max( temp_output_7_0_g82 , temp_output_8_0_g82 ) - 0.2 );
				float temp_output_13_0_g82 = max( ( temp_output_7_0_g82 - temp_output_10_0_g82 ) , 0.0 );
				float temp_output_14_0_g82 = max( ( temp_output_8_0_g82 - temp_output_10_0_g82 ) , 0.0 );
				float temp_output_77_42 = ( ( ( tex2DArrayNode27_g80 * temp_output_13_0_g82 ) + ( tex2DArrayNode24_g80 * temp_output_14_0_g82 ) ) / ( temp_output_13_0_g82 + temp_output_14_0_g82 ) ).z;
				float2 break41 = ( temp_output_36_0 - temp_output_39_0 );
				float temp_output_49_0 = ( 1.0 - break41.x );
				float temp_output_7_0_g95 = ( temp_output_77_42 + temp_output_49_0 );
				float2 temp_output_6_0_g84 = ( ( temp_output_39_0 + float2( 0,0 ) ) / appendResult35 );
				float4 tex2DNode10_g84 = tex2D( _GrassControl, temp_output_6_0_g84 );
				float4 tex2DArrayNode27_g84 = SAMPLE_TEXTURE2D_ARRAY( _ShapeArray, sampler_ShapeArray, uv_ShapeArray,( tex2DNode10_g84.g * 255.0 ) );
				float4 tex2DNode11_g84 = tex2D( _GrassTint, temp_output_6_0_g84 );
				float temp_output_7_0_g86 = ( tex2DArrayNode27_g84.b + tex2DNode11_g84.a );
				float4 tex2DNode8_g84 = tex2D( _Control, temp_output_6_0_g84 );
				float4 tex2DArrayNode24_g84 = SAMPLE_TEXTURE2D_ARRAY( _ShapeArray, sampler_ShapeArray, uv_ShapeArray,( tex2DNode8_g84.g * 255.0 ) );
				float temp_output_30_0_g84 = ( 1.0 - tex2DNode11_g84.a );
				float temp_output_8_0_g86 = ( tex2DArrayNode24_g84.b + temp_output_30_0_g84 );
				float temp_output_10_0_g86 = ( max( temp_output_7_0_g86 , temp_output_8_0_g86 ) - 0.2 );
				float temp_output_13_0_g86 = max( ( temp_output_7_0_g86 - temp_output_10_0_g86 ) , 0.0 );
				float temp_output_14_0_g86 = max( ( temp_output_8_0_g86 - temp_output_10_0_g86 ) , 0.0 );
				float temp_output_79_42 = ( ( ( tex2DArrayNode27_g84 * temp_output_13_0_g86 ) + ( tex2DArrayNode24_g84 * temp_output_14_0_g86 ) ) / ( temp_output_13_0_g86 + temp_output_14_0_g86 ) ).z;
				float temp_output_8_0_g95 = ( temp_output_79_42 + break41.x );
				float temp_output_10_0_g95 = ( max( temp_output_7_0_g95 , temp_output_8_0_g95 ) - 0.2 );
				float temp_output_13_0_g95 = max( ( temp_output_7_0_g95 - temp_output_10_0_g95 ) , 0.0 );
				float temp_output_7_0_g85 = ( tex2DArrayNode27_g84.b + tex2DNode11_g84.a );
				float temp_output_8_0_g85 = ( tex2DArrayNode24_g84.b + temp_output_30_0_g84 );
				float temp_output_10_0_g85 = ( max( temp_output_7_0_g85 , temp_output_8_0_g85 ) - 0.2 );
				float temp_output_13_0_g85 = max( ( temp_output_7_0_g85 - temp_output_10_0_g85 ) , 0.0 );
				float4 tex2DNode9_g84 = tex2D( _Tint, temp_output_6_0_g84 );
				float temp_output_14_0_g85 = max( ( temp_output_8_0_g85 - temp_output_10_0_g85 ) , 0.0 );
				float temp_output_14_0_g95 = max( ( temp_output_8_0_g95 - temp_output_10_0_g95 ) , 0.0 );
				float temp_output_7_0_g93 = ( temp_output_77_42 + temp_output_49_0 );
				float temp_output_8_0_g93 = ( temp_output_79_42 + break41.x );
				float temp_output_10_0_g93 = ( max( temp_output_7_0_g93 , temp_output_8_0_g93 ) - 0.2 );
				float temp_output_13_0_g93 = max( ( temp_output_7_0_g93 - temp_output_10_0_g93 ) , 0.0 );
				float temp_output_14_0_g93 = max( ( temp_output_8_0_g93 - temp_output_10_0_g93 ) , 0.0 );
				float4 temp_output_44_0 = ( ( ( ( ( ( tex2DArrayNode27_g80 * temp_output_13_0_g82 ) + ( tex2DArrayNode24_g80 * temp_output_14_0_g82 ) ) / ( temp_output_13_0_g82 + temp_output_14_0_g82 ) ) * temp_output_13_0_g93 ) + ( ( ( ( tex2DArrayNode27_g84 * temp_output_13_0_g86 ) + ( tex2DArrayNode24_g84 * temp_output_14_0_g86 ) ) / ( temp_output_13_0_g86 + temp_output_14_0_g86 ) ) * temp_output_14_0_g93 ) ) / ( temp_output_13_0_g93 + temp_output_14_0_g93 ) );
				float temp_output_61_0 = ( 1.0 - break41.y );
				float temp_output_7_0_g99 = ( temp_output_44_0.z + temp_output_61_0 );
				float2 temp_output_6_0_g88 = ( ( temp_output_39_0 + float2( 0,0 ) ) / appendResult35 );
				float4 tex2DNode10_g88 = tex2D( _GrassControl, temp_output_6_0_g88 );
				float4 tex2DArrayNode27_g88 = SAMPLE_TEXTURE2D_ARRAY( _ShapeArray, sampler_ShapeArray, uv_ShapeArray,( tex2DNode10_g88.g * 255.0 ) );
				float4 tex2DNode11_g88 = tex2D( _GrassTint, temp_output_6_0_g88 );
				float temp_output_7_0_g90 = ( tex2DArrayNode27_g88.b + tex2DNode11_g88.a );
				float4 tex2DNode8_g88 = tex2D( _Control, temp_output_6_0_g88 );
				float4 tex2DArrayNode24_g88 = SAMPLE_TEXTURE2D_ARRAY( _ShapeArray, sampler_ShapeArray, uv_ShapeArray,( tex2DNode8_g88.g * 255.0 ) );
				float temp_output_30_0_g88 = ( 1.0 - tex2DNode11_g88.a );
				float temp_output_8_0_g90 = ( tex2DArrayNode24_g88.b + temp_output_30_0_g88 );
				float temp_output_10_0_g90 = ( max( temp_output_7_0_g90 , temp_output_8_0_g90 ) - 0.2 );
				float temp_output_13_0_g90 = max( ( temp_output_7_0_g90 - temp_output_10_0_g90 ) , 0.0 );
				float temp_output_14_0_g90 = max( ( temp_output_8_0_g90 - temp_output_10_0_g90 ) , 0.0 );
				float temp_output_80_42 = ( ( ( tex2DArrayNode27_g88 * temp_output_13_0_g90 ) + ( tex2DArrayNode24_g88 * temp_output_14_0_g90 ) ) / ( temp_output_13_0_g90 + temp_output_14_0_g90 ) ).z;
				float temp_output_7_0_g94 = ( temp_output_80_42 + temp_output_49_0 );
				float2 temp_output_6_0_g76 = ( ( temp_output_39_0 + float2( 0,0 ) ) / appendResult35 );
				float4 tex2DNode10_g76 = tex2D( _GrassControl, temp_output_6_0_g76 );
				float4 tex2DArrayNode27_g76 = SAMPLE_TEXTURE2D_ARRAY( _ShapeArray, sampler_ShapeArray, uv_ShapeArray,( tex2DNode10_g76.g * 255.0 ) );
				float4 tex2DNode11_g76 = tex2D( _GrassTint, temp_output_6_0_g76 );
				float temp_output_7_0_g78 = ( tex2DArrayNode27_g76.b + tex2DNode11_g76.a );
				float4 tex2DNode8_g76 = tex2D( _Control, temp_output_6_0_g76 );
				float4 tex2DArrayNode24_g76 = SAMPLE_TEXTURE2D_ARRAY( _ShapeArray, sampler_ShapeArray, uv_ShapeArray,( tex2DNode8_g76.g * 255.0 ) );
				float temp_output_30_0_g76 = ( 1.0 - tex2DNode11_g76.a );
				float temp_output_8_0_g78 = ( tex2DArrayNode24_g76.b + temp_output_30_0_g76 );
				float temp_output_10_0_g78 = ( max( temp_output_7_0_g78 , temp_output_8_0_g78 ) - 0.2 );
				float temp_output_13_0_g78 = max( ( temp_output_7_0_g78 - temp_output_10_0_g78 ) , 0.0 );
				float temp_output_14_0_g78 = max( ( temp_output_8_0_g78 - temp_output_10_0_g78 ) , 0.0 );
				float temp_output_78_42 = ( ( ( tex2DArrayNode27_g76 * temp_output_13_0_g78 ) + ( tex2DArrayNode24_g76 * temp_output_14_0_g78 ) ) / ( temp_output_13_0_g78 + temp_output_14_0_g78 ) ).z;
				float temp_output_8_0_g94 = ( temp_output_78_42 + break41.x );
				float temp_output_10_0_g94 = ( max( temp_output_7_0_g94 , temp_output_8_0_g94 ) - 0.2 );
				float temp_output_13_0_g94 = max( ( temp_output_7_0_g94 - temp_output_10_0_g94 ) , 0.0 );
				float temp_output_14_0_g94 = max( ( temp_output_8_0_g94 - temp_output_10_0_g94 ) , 0.0 );
				float4 temp_output_47_0 = ( ( ( ( ( ( tex2DArrayNode27_g88 * temp_output_13_0_g90 ) + ( tex2DArrayNode24_g88 * temp_output_14_0_g90 ) ) / ( temp_output_13_0_g90 + temp_output_14_0_g90 ) ) * temp_output_13_0_g94 ) + ( ( ( ( tex2DArrayNode27_g76 * temp_output_13_0_g78 ) + ( tex2DArrayNode24_g76 * temp_output_14_0_g78 ) ) / ( temp_output_13_0_g78 + temp_output_14_0_g78 ) ) * temp_output_14_0_g94 ) ) / ( temp_output_13_0_g94 + temp_output_14_0_g94 ) );
				float temp_output_8_0_g99 = ( temp_output_47_0.z + break41.y );
				float temp_output_10_0_g99 = ( max( temp_output_7_0_g99 , temp_output_8_0_g99 ) - 0.2 );
				float temp_output_13_0_g99 = max( ( temp_output_7_0_g99 - temp_output_10_0_g99 ) , 0.0 );
				float temp_output_7_0_g89 = ( tex2DArrayNode27_g88.b + tex2DNode11_g88.a );
				float temp_output_8_0_g89 = ( tex2DArrayNode24_g88.b + temp_output_30_0_g88 );
				float temp_output_10_0_g89 = ( max( temp_output_7_0_g89 , temp_output_8_0_g89 ) - 0.2 );
				float temp_output_13_0_g89 = max( ( temp_output_7_0_g89 - temp_output_10_0_g89 ) , 0.0 );
				float4 tex2DNode9_g88 = tex2D( _Tint, temp_output_6_0_g88 );
				float temp_output_14_0_g89 = max( ( temp_output_8_0_g89 - temp_output_10_0_g89 ) , 0.0 );
				float temp_output_7_0_g96 = ( temp_output_80_42 + temp_output_49_0 );
				float temp_output_8_0_g96 = ( temp_output_78_42 + break41.x );
				float temp_output_10_0_g96 = ( max( temp_output_7_0_g96 , temp_output_8_0_g96 ) - 0.2 );
				float temp_output_13_0_g96 = max( ( temp_output_7_0_g96 - temp_output_10_0_g96 ) , 0.0 );
				float temp_output_7_0_g77 = ( tex2DArrayNode27_g76.b + tex2DNode11_g76.a );
				float temp_output_8_0_g77 = ( tex2DArrayNode24_g76.b + temp_output_30_0_g76 );
				float temp_output_10_0_g77 = ( max( temp_output_7_0_g77 , temp_output_8_0_g77 ) - 0.2 );
				float temp_output_13_0_g77 = max( ( temp_output_7_0_g77 - temp_output_10_0_g77 ) , 0.0 );
				float4 tex2DNode9_g76 = tex2D( _Tint, temp_output_6_0_g76 );
				float temp_output_14_0_g77 = max( ( temp_output_8_0_g77 - temp_output_10_0_g77 ) , 0.0 );
				float temp_output_14_0_g96 = max( ( temp_output_8_0_g96 - temp_output_10_0_g96 ) , 0.0 );
				float temp_output_14_0_g99 = max( ( temp_output_8_0_g99 - temp_output_10_0_g99 ) , 0.0 );
				float4 temp_output_64_0 = ( ( ( ( ( ( ( ( ( tex2DNode11_g80 * temp_output_13_0_g81 ) + ( tex2DNode9_g80 * temp_output_14_0_g81 ) ) / ( temp_output_13_0_g81 + temp_output_14_0_g81 ) ) * temp_output_13_0_g95 ) + ( ( ( ( tex2DNode11_g84 * temp_output_13_0_g85 ) + ( tex2DNode9_g84 * temp_output_14_0_g85 ) ) / ( temp_output_13_0_g85 + temp_output_14_0_g85 ) ) * temp_output_14_0_g95 ) ) / ( temp_output_13_0_g95 + temp_output_14_0_g95 ) ) * temp_output_13_0_g99 ) + ( ( ( ( ( ( ( tex2DNode11_g88 * temp_output_13_0_g89 ) + ( tex2DNode9_g88 * temp_output_14_0_g89 ) ) / ( temp_output_13_0_g89 + temp_output_14_0_g89 ) ) * temp_output_13_0_g96 ) + ( ( ( ( tex2DNode11_g76 * temp_output_13_0_g77 ) + ( tex2DNode9_g76 * temp_output_14_0_g77 ) ) / ( temp_output_13_0_g77 + temp_output_14_0_g77 ) ) * temp_output_14_0_g96 ) ) / ( temp_output_13_0_g96 + temp_output_14_0_g96 ) ) * temp_output_14_0_g99 ) ) / ( temp_output_13_0_g99 + temp_output_14_0_g99 ) );
				float temp_output_3_0_g102 = ( temp_output_64_0.w * 2.0 );
				float ifLocalVar169_g104 = 0;
				if( temp_output_147_0_g104 < 1.0 )
				ifLocalVar169_g104 = min( temp_output_3_0_g102 , 1.0 );
				
				surfaceDescription.Alpha = ifLocalVar169_g104;

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

			#define _BLENDMODE_PRESERVE_SPECULAR_LIGHTING 1
			#define _DISABLE_SSR_TRANSPARENT 1
			#define _SPECULAR_OCCLUSION_FROM_AO 1
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
			float4 _MaterialArray_ST;
			float4 _Control_TexelSize;
			float4 _ShapeArray_ST;
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
			TEXTURE2D_ARRAY(_ShapeArray);
			sampler2D _GrassControl;
			sampler2D _Control;
			SAMPLER(sampler_ShapeArray);
			sampler2D _GrassTint;
			sampler2D _SpatterTex;
			sampler2D _Noise;
			TEXTURE2D_ARRAY(_MaterialArray);
			SAMPLER(sampler_MaterialArray);
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
			#define ASE_NEEDS_FRAG_WORLD_NORMAL


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
				float2 uv_ShapeArray = packedInput.ase_texcoord3.xy;
				float3 ase_worldPos = GetAbsolutePositionWS( positionRWS );
				float3 break17 = ase_worldPos;
				float2 appendResult26 = (float2(break17.x , break17.z));
				float4 break18 = _WorldBounds;
				float2 appendResult27 = (float2(break18.x , break18.y));
				float2 appendResult28 = (float2(break18.w , break18.z));
				float2 appendResult35 = (float2(_Control_TexelSize.z , _Control_TexelSize.w));
				float2 temp_output_36_0 = ( ( ( appendResult26 - appendResult27 ) / ( appendResult28 - appendResult27 ) ) * appendResult35 );
				float2 temp_output_39_0 = ( floor( ( temp_output_36_0 + float2( 0.5,0.5 ) ) ) - float2( 0.5,0.5 ) );
				float2 temp_output_6_0_g80 = ( ( temp_output_39_0 + float2( 0,0 ) ) / appendResult35 );
				float4 tex2DNode10_g80 = tex2D( _GrassControl, temp_output_6_0_g80 );
				float4 tex2DArrayNode27_g80 = SAMPLE_TEXTURE2D_ARRAY( _ShapeArray, sampler_ShapeArray, uv_ShapeArray,( tex2DNode10_g80.g * 255.0 ) );
				float4 tex2DNode11_g80 = tex2D( _GrassTint, temp_output_6_0_g80 );
				float temp_output_7_0_g82 = ( tex2DArrayNode27_g80.b + tex2DNode11_g80.a );
				float4 tex2DNode8_g80 = tex2D( _Control, temp_output_6_0_g80 );
				float4 tex2DArrayNode24_g80 = SAMPLE_TEXTURE2D_ARRAY( _ShapeArray, sampler_ShapeArray, uv_ShapeArray,( tex2DNode8_g80.g * 255.0 ) );
				float temp_output_30_0_g80 = ( 1.0 - tex2DNode11_g80.a );
				float temp_output_8_0_g82 = ( tex2DArrayNode24_g80.b + temp_output_30_0_g80 );
				float temp_output_10_0_g82 = ( max( temp_output_7_0_g82 , temp_output_8_0_g82 ) - 0.2 );
				float temp_output_13_0_g82 = max( ( temp_output_7_0_g82 - temp_output_10_0_g82 ) , 0.0 );
				float temp_output_14_0_g82 = max( ( temp_output_8_0_g82 - temp_output_10_0_g82 ) , 0.0 );
				float temp_output_77_42 = ( ( ( tex2DArrayNode27_g80 * temp_output_13_0_g82 ) + ( tex2DArrayNode24_g80 * temp_output_14_0_g82 ) ) / ( temp_output_13_0_g82 + temp_output_14_0_g82 ) ).z;
				float2 break41 = ( temp_output_36_0 - temp_output_39_0 );
				float temp_output_49_0 = ( 1.0 - break41.x );
				float temp_output_7_0_g93 = ( temp_output_77_42 + temp_output_49_0 );
				float2 temp_output_6_0_g84 = ( ( temp_output_39_0 + float2( 0,0 ) ) / appendResult35 );
				float4 tex2DNode10_g84 = tex2D( _GrassControl, temp_output_6_0_g84 );
				float4 tex2DArrayNode27_g84 = SAMPLE_TEXTURE2D_ARRAY( _ShapeArray, sampler_ShapeArray, uv_ShapeArray,( tex2DNode10_g84.g * 255.0 ) );
				float4 tex2DNode11_g84 = tex2D( _GrassTint, temp_output_6_0_g84 );
				float temp_output_7_0_g86 = ( tex2DArrayNode27_g84.b + tex2DNode11_g84.a );
				float4 tex2DNode8_g84 = tex2D( _Control, temp_output_6_0_g84 );
				float4 tex2DArrayNode24_g84 = SAMPLE_TEXTURE2D_ARRAY( _ShapeArray, sampler_ShapeArray, uv_ShapeArray,( tex2DNode8_g84.g * 255.0 ) );
				float temp_output_30_0_g84 = ( 1.0 - tex2DNode11_g84.a );
				float temp_output_8_0_g86 = ( tex2DArrayNode24_g84.b + temp_output_30_0_g84 );
				float temp_output_10_0_g86 = ( max( temp_output_7_0_g86 , temp_output_8_0_g86 ) - 0.2 );
				float temp_output_13_0_g86 = max( ( temp_output_7_0_g86 - temp_output_10_0_g86 ) , 0.0 );
				float temp_output_14_0_g86 = max( ( temp_output_8_0_g86 - temp_output_10_0_g86 ) , 0.0 );
				float temp_output_79_42 = ( ( ( tex2DArrayNode27_g84 * temp_output_13_0_g86 ) + ( tex2DArrayNode24_g84 * temp_output_14_0_g86 ) ) / ( temp_output_13_0_g86 + temp_output_14_0_g86 ) ).z;
				float temp_output_8_0_g93 = ( temp_output_79_42 + break41.x );
				float temp_output_10_0_g93 = ( max( temp_output_7_0_g93 , temp_output_8_0_g93 ) - 0.2 );
				float temp_output_13_0_g93 = max( ( temp_output_7_0_g93 - temp_output_10_0_g93 ) , 0.0 );
				float temp_output_14_0_g93 = max( ( temp_output_8_0_g93 - temp_output_10_0_g93 ) , 0.0 );
				float4 temp_output_44_0 = ( ( ( ( ( ( tex2DArrayNode27_g80 * temp_output_13_0_g82 ) + ( tex2DArrayNode24_g80 * temp_output_14_0_g82 ) ) / ( temp_output_13_0_g82 + temp_output_14_0_g82 ) ) * temp_output_13_0_g93 ) + ( ( ( ( tex2DArrayNode27_g84 * temp_output_13_0_g86 ) + ( tex2DArrayNode24_g84 * temp_output_14_0_g86 ) ) / ( temp_output_13_0_g86 + temp_output_14_0_g86 ) ) * temp_output_14_0_g93 ) ) / ( temp_output_13_0_g93 + temp_output_14_0_g93 ) );
				float temp_output_61_0 = ( 1.0 - break41.y );
				float temp_output_7_0_g103 = ( temp_output_44_0.z + temp_output_61_0 );
				float2 temp_output_6_0_g88 = ( ( temp_output_39_0 + float2( 0,0 ) ) / appendResult35 );
				float4 tex2DNode10_g88 = tex2D( _GrassControl, temp_output_6_0_g88 );
				float4 tex2DArrayNode27_g88 = SAMPLE_TEXTURE2D_ARRAY( _ShapeArray, sampler_ShapeArray, uv_ShapeArray,( tex2DNode10_g88.g * 255.0 ) );
				float4 tex2DNode11_g88 = tex2D( _GrassTint, temp_output_6_0_g88 );
				float temp_output_7_0_g90 = ( tex2DArrayNode27_g88.b + tex2DNode11_g88.a );
				float4 tex2DNode8_g88 = tex2D( _Control, temp_output_6_0_g88 );
				float4 tex2DArrayNode24_g88 = SAMPLE_TEXTURE2D_ARRAY( _ShapeArray, sampler_ShapeArray, uv_ShapeArray,( tex2DNode8_g88.g * 255.0 ) );
				float temp_output_30_0_g88 = ( 1.0 - tex2DNode11_g88.a );
				float temp_output_8_0_g90 = ( tex2DArrayNode24_g88.b + temp_output_30_0_g88 );
				float temp_output_10_0_g90 = ( max( temp_output_7_0_g90 , temp_output_8_0_g90 ) - 0.2 );
				float temp_output_13_0_g90 = max( ( temp_output_7_0_g90 - temp_output_10_0_g90 ) , 0.0 );
				float temp_output_14_0_g90 = max( ( temp_output_8_0_g90 - temp_output_10_0_g90 ) , 0.0 );
				float temp_output_80_42 = ( ( ( tex2DArrayNode27_g88 * temp_output_13_0_g90 ) + ( tex2DArrayNode24_g88 * temp_output_14_0_g90 ) ) / ( temp_output_13_0_g90 + temp_output_14_0_g90 ) ).z;
				float temp_output_7_0_g94 = ( temp_output_80_42 + temp_output_49_0 );
				float2 temp_output_6_0_g76 = ( ( temp_output_39_0 + float2( 0,0 ) ) / appendResult35 );
				float4 tex2DNode10_g76 = tex2D( _GrassControl, temp_output_6_0_g76 );
				float4 tex2DArrayNode27_g76 = SAMPLE_TEXTURE2D_ARRAY( _ShapeArray, sampler_ShapeArray, uv_ShapeArray,( tex2DNode10_g76.g * 255.0 ) );
				float4 tex2DNode11_g76 = tex2D( _GrassTint, temp_output_6_0_g76 );
				float temp_output_7_0_g78 = ( tex2DArrayNode27_g76.b + tex2DNode11_g76.a );
				float4 tex2DNode8_g76 = tex2D( _Control, temp_output_6_0_g76 );
				float4 tex2DArrayNode24_g76 = SAMPLE_TEXTURE2D_ARRAY( _ShapeArray, sampler_ShapeArray, uv_ShapeArray,( tex2DNode8_g76.g * 255.0 ) );
				float temp_output_30_0_g76 = ( 1.0 - tex2DNode11_g76.a );
				float temp_output_8_0_g78 = ( tex2DArrayNode24_g76.b + temp_output_30_0_g76 );
				float temp_output_10_0_g78 = ( max( temp_output_7_0_g78 , temp_output_8_0_g78 ) - 0.2 );
				float temp_output_13_0_g78 = max( ( temp_output_7_0_g78 - temp_output_10_0_g78 ) , 0.0 );
				float temp_output_14_0_g78 = max( ( temp_output_8_0_g78 - temp_output_10_0_g78 ) , 0.0 );
				float temp_output_78_42 = ( ( ( tex2DArrayNode27_g76 * temp_output_13_0_g78 ) + ( tex2DArrayNode24_g76 * temp_output_14_0_g78 ) ) / ( temp_output_13_0_g78 + temp_output_14_0_g78 ) ).z;
				float temp_output_8_0_g94 = ( temp_output_78_42 + break41.x );
				float temp_output_10_0_g94 = ( max( temp_output_7_0_g94 , temp_output_8_0_g94 ) - 0.2 );
				float temp_output_13_0_g94 = max( ( temp_output_7_0_g94 - temp_output_10_0_g94 ) , 0.0 );
				float temp_output_14_0_g94 = max( ( temp_output_8_0_g94 - temp_output_10_0_g94 ) , 0.0 );
				float4 temp_output_47_0 = ( ( ( ( ( ( tex2DArrayNode27_g88 * temp_output_13_0_g90 ) + ( tex2DArrayNode24_g88 * temp_output_14_0_g90 ) ) / ( temp_output_13_0_g90 + temp_output_14_0_g90 ) ) * temp_output_13_0_g94 ) + ( ( ( ( tex2DArrayNode27_g76 * temp_output_13_0_g78 ) + ( tex2DArrayNode24_g76 * temp_output_14_0_g78 ) ) / ( temp_output_13_0_g78 + temp_output_14_0_g78 ) ) * temp_output_14_0_g94 ) ) / ( temp_output_13_0_g94 + temp_output_14_0_g94 ) );
				float temp_output_8_0_g103 = ( temp_output_47_0.z + break41.y );
				float temp_output_10_0_g103 = ( max( temp_output_7_0_g103 , temp_output_8_0_g103 ) - 0.2 );
				float temp_output_13_0_g103 = max( ( temp_output_7_0_g103 - temp_output_10_0_g103 ) , 0.0 );
				float temp_output_14_0_g103 = max( ( temp_output_8_0_g103 - temp_output_10_0_g103 ) , 0.0 );
				float4 break65 = ( ( ( temp_output_44_0 * temp_output_13_0_g103 ) + ( temp_output_47_0 * temp_output_14_0_g103 ) ) / ( temp_output_13_0_g103 + temp_output_14_0_g103 ) );
				float4 appendResult71 = (float4(0.0 , break65.y , break65.y , break65.w));
				
				float dotResult145_g104 = dot( normalWS , float3( 0,1,0 ) );
				float3 break180_g104 = ase_worldPos;
				float2 appendResult102_g104 = (float2(break180_g104.x , break180_g104.z));
				float4 break174_g104 = _WorldBounds;
				float2 appendResult129_g104 = (float2(break174_g104.x , break174_g104.y));
				float2 appendResult130_g104 = (float2(break174_g104.z , break174_g104.w));
				float4 tex2DNode133_g104 = tex2D( _SpatterTex, ( ( appendResult102_g104 - appendResult129_g104 ) / ( appendResult130_g104 - appendResult129_g104 ) ) );
				float lerpResult139_g104 = lerp( 1.0 , -1.0 , ( tex2DNode133_g104.a - tex2D( _Noise, appendResult102_g104 ).r ));
				float temp_output_147_0_g104 = ( dotResult145_g104 >= lerpResult139_g104 ? 1.0 : 0.0 );
				float2 uv_MaterialArray = packedInput.ase_texcoord3.xy;
				float temp_output_7_0_g83 = ( tex2DArrayNode27_g80.b + tex2DNode11_g80.a );
				float temp_output_8_0_g83 = ( tex2DArrayNode24_g80.b + temp_output_30_0_g80 );
				float temp_output_10_0_g83 = ( max( temp_output_7_0_g83 , temp_output_8_0_g83 ) - 0.2 );
				float temp_output_13_0_g83 = max( ( temp_output_7_0_g83 - temp_output_10_0_g83 ) , 0.0 );
				float4 tex2DArrayNode21_g80 = SAMPLE_TEXTURE2D_ARRAY( _MaterialArray, sampler_MaterialArray, uv_MaterialArray,( tex2DNode8_g80.r * 255.0 ) );
				float temp_output_14_0_g83 = max( ( temp_output_8_0_g83 - temp_output_10_0_g83 ) , 0.0 );
				float temp_output_7_0_g97 = ( temp_output_77_42 + temp_output_49_0 );
				float temp_output_8_0_g97 = ( temp_output_79_42 + break41.x );
				float temp_output_10_0_g97 = ( max( temp_output_7_0_g97 , temp_output_8_0_g97 ) - 0.2 );
				float temp_output_13_0_g97 = max( ( temp_output_7_0_g97 - temp_output_10_0_g97 ) , 0.0 );
				float temp_output_7_0_g87 = ( tex2DArrayNode27_g84.b + tex2DNode11_g84.a );
				float temp_output_8_0_g87 = ( tex2DArrayNode24_g84.b + temp_output_30_0_g84 );
				float temp_output_10_0_g87 = ( max( temp_output_7_0_g87 , temp_output_8_0_g87 ) - 0.2 );
				float temp_output_13_0_g87 = max( ( temp_output_7_0_g87 - temp_output_10_0_g87 ) , 0.0 );
				float4 tex2DArrayNode21_g84 = SAMPLE_TEXTURE2D_ARRAY( _MaterialArray, sampler_MaterialArray, uv_MaterialArray,( tex2DNode8_g84.r * 255.0 ) );
				float temp_output_14_0_g87 = max( ( temp_output_8_0_g87 - temp_output_10_0_g87 ) , 0.0 );
				float temp_output_14_0_g97 = max( ( temp_output_8_0_g97 - temp_output_10_0_g97 ) , 0.0 );
				float temp_output_7_0_g100 = ( temp_output_44_0.z + temp_output_61_0 );
				float temp_output_8_0_g100 = ( temp_output_47_0.z + break41.y );
				float temp_output_10_0_g100 = ( max( temp_output_7_0_g100 , temp_output_8_0_g100 ) - 0.2 );
				float temp_output_13_0_g100 = max( ( temp_output_7_0_g100 - temp_output_10_0_g100 ) , 0.0 );
				float temp_output_7_0_g91 = ( tex2DArrayNode27_g88.b + tex2DNode11_g88.a );
				float temp_output_8_0_g91 = ( tex2DArrayNode24_g88.b + temp_output_30_0_g88 );
				float temp_output_10_0_g91 = ( max( temp_output_7_0_g91 , temp_output_8_0_g91 ) - 0.2 );
				float temp_output_13_0_g91 = max( ( temp_output_7_0_g91 - temp_output_10_0_g91 ) , 0.0 );
				float4 tex2DArrayNode21_g88 = SAMPLE_TEXTURE2D_ARRAY( _MaterialArray, sampler_MaterialArray, uv_MaterialArray,( tex2DNode8_g88.r * 255.0 ) );
				float temp_output_14_0_g91 = max( ( temp_output_8_0_g91 - temp_output_10_0_g91 ) , 0.0 );
				float temp_output_7_0_g98 = ( temp_output_80_42 + temp_output_49_0 );
				float temp_output_8_0_g98 = ( temp_output_78_42 + break41.x );
				float temp_output_10_0_g98 = ( max( temp_output_7_0_g98 , temp_output_8_0_g98 ) - 0.2 );
				float temp_output_13_0_g98 = max( ( temp_output_7_0_g98 - temp_output_10_0_g98 ) , 0.0 );
				float temp_output_7_0_g79 = ( tex2DArrayNode27_g76.b + tex2DNode11_g76.a );
				float temp_output_8_0_g79 = ( tex2DArrayNode24_g76.b + temp_output_30_0_g76 );
				float temp_output_10_0_g79 = ( max( temp_output_7_0_g79 , temp_output_8_0_g79 ) - 0.2 );
				float temp_output_13_0_g79 = max( ( temp_output_7_0_g79 - temp_output_10_0_g79 ) , 0.0 );
				float4 tex2DArrayNode21_g76 = SAMPLE_TEXTURE2D_ARRAY( _MaterialArray, sampler_MaterialArray, uv_MaterialArray,( tex2DNode8_g76.r * 255.0 ) );
				float temp_output_14_0_g79 = max( ( temp_output_8_0_g79 - temp_output_10_0_g79 ) , 0.0 );
				float temp_output_14_0_g98 = max( ( temp_output_8_0_g98 - temp_output_10_0_g98 ) , 0.0 );
				float temp_output_14_0_g100 = max( ( temp_output_8_0_g100 - temp_output_10_0_g100 ) , 0.0 );
				float4 temp_output_62_0 = ( ( ( ( ( ( ( ( ( SAMPLE_TEXTURE2D_ARRAY( _MaterialArray, sampler_MaterialArray, uv_MaterialArray,( tex2DNode10_g80.r * 255.0 ) ) * temp_output_13_0_g83 ) + ( tex2DArrayNode21_g80 * temp_output_14_0_g83 ) ) / ( temp_output_13_0_g83 + temp_output_14_0_g83 ) ) * temp_output_13_0_g97 ) + ( ( ( ( SAMPLE_TEXTURE2D_ARRAY( _MaterialArray, sampler_MaterialArray, uv_MaterialArray,( tex2DNode10_g84.r * 255.0 ) ) * temp_output_13_0_g87 ) + ( tex2DArrayNode21_g84 * temp_output_14_0_g87 ) ) / ( temp_output_13_0_g87 + temp_output_14_0_g87 ) ) * temp_output_14_0_g97 ) ) / ( temp_output_13_0_g97 + temp_output_14_0_g97 ) ) * temp_output_13_0_g100 ) + ( ( ( ( ( ( ( SAMPLE_TEXTURE2D_ARRAY( _MaterialArray, sampler_MaterialArray, uv_MaterialArray,( tex2DNode10_g88.r * 255.0 ) ) * temp_output_13_0_g91 ) + ( tex2DArrayNode21_g88 * temp_output_14_0_g91 ) ) / ( temp_output_13_0_g91 + temp_output_14_0_g91 ) ) * temp_output_13_0_g98 ) + ( ( ( ( SAMPLE_TEXTURE2D_ARRAY( _MaterialArray, sampler_MaterialArray, uv_MaterialArray,( tex2DNode10_g76.r * 255.0 ) ) * temp_output_13_0_g79 ) + ( tex2DArrayNode21_g76 * temp_output_14_0_g79 ) ) / ( temp_output_13_0_g79 + temp_output_14_0_g79 ) ) * temp_output_14_0_g98 ) ) / ( temp_output_13_0_g98 + temp_output_14_0_g98 ) ) * temp_output_14_0_g100 ) ) / ( temp_output_13_0_g100 + temp_output_14_0_g100 ) );
				float ifLocalVar158_g104 = 0;
				UNITY_BRANCH 
				if( temp_output_147_0_g104 < 0.5 )
				ifLocalVar158_g104 = temp_output_62_0.w;
				
				float temp_output_7_0_g81 = ( tex2DArrayNode27_g80.b + tex2DNode11_g80.a );
				float temp_output_8_0_g81 = ( tex2DArrayNode24_g80.b + temp_output_30_0_g80 );
				float temp_output_10_0_g81 = ( max( temp_output_7_0_g81 , temp_output_8_0_g81 ) - 0.2 );
				float temp_output_13_0_g81 = max( ( temp_output_7_0_g81 - temp_output_10_0_g81 ) , 0.0 );
				float4 tex2DNode9_g80 = tex2D( _Tint, temp_output_6_0_g80 );
				float temp_output_14_0_g81 = max( ( temp_output_8_0_g81 - temp_output_10_0_g81 ) , 0.0 );
				float temp_output_7_0_g95 = ( temp_output_77_42 + temp_output_49_0 );
				float temp_output_8_0_g95 = ( temp_output_79_42 + break41.x );
				float temp_output_10_0_g95 = ( max( temp_output_7_0_g95 , temp_output_8_0_g95 ) - 0.2 );
				float temp_output_13_0_g95 = max( ( temp_output_7_0_g95 - temp_output_10_0_g95 ) , 0.0 );
				float temp_output_7_0_g85 = ( tex2DArrayNode27_g84.b + tex2DNode11_g84.a );
				float temp_output_8_0_g85 = ( tex2DArrayNode24_g84.b + temp_output_30_0_g84 );
				float temp_output_10_0_g85 = ( max( temp_output_7_0_g85 , temp_output_8_0_g85 ) - 0.2 );
				float temp_output_13_0_g85 = max( ( temp_output_7_0_g85 - temp_output_10_0_g85 ) , 0.0 );
				float4 tex2DNode9_g84 = tex2D( _Tint, temp_output_6_0_g84 );
				float temp_output_14_0_g85 = max( ( temp_output_8_0_g85 - temp_output_10_0_g85 ) , 0.0 );
				float temp_output_14_0_g95 = max( ( temp_output_8_0_g95 - temp_output_10_0_g95 ) , 0.0 );
				float temp_output_7_0_g99 = ( temp_output_44_0.z + temp_output_61_0 );
				float temp_output_8_0_g99 = ( temp_output_47_0.z + break41.y );
				float temp_output_10_0_g99 = ( max( temp_output_7_0_g99 , temp_output_8_0_g99 ) - 0.2 );
				float temp_output_13_0_g99 = max( ( temp_output_7_0_g99 - temp_output_10_0_g99 ) , 0.0 );
				float temp_output_7_0_g89 = ( tex2DArrayNode27_g88.b + tex2DNode11_g88.a );
				float temp_output_8_0_g89 = ( tex2DArrayNode24_g88.b + temp_output_30_0_g88 );
				float temp_output_10_0_g89 = ( max( temp_output_7_0_g89 , temp_output_8_0_g89 ) - 0.2 );
				float temp_output_13_0_g89 = max( ( temp_output_7_0_g89 - temp_output_10_0_g89 ) , 0.0 );
				float4 tex2DNode9_g88 = tex2D( _Tint, temp_output_6_0_g88 );
				float temp_output_14_0_g89 = max( ( temp_output_8_0_g89 - temp_output_10_0_g89 ) , 0.0 );
				float temp_output_7_0_g96 = ( temp_output_80_42 + temp_output_49_0 );
				float temp_output_8_0_g96 = ( temp_output_78_42 + break41.x );
				float temp_output_10_0_g96 = ( max( temp_output_7_0_g96 , temp_output_8_0_g96 ) - 0.2 );
				float temp_output_13_0_g96 = max( ( temp_output_7_0_g96 - temp_output_10_0_g96 ) , 0.0 );
				float temp_output_7_0_g77 = ( tex2DArrayNode27_g76.b + tex2DNode11_g76.a );
				float temp_output_8_0_g77 = ( tex2DArrayNode24_g76.b + temp_output_30_0_g76 );
				float temp_output_10_0_g77 = ( max( temp_output_7_0_g77 , temp_output_8_0_g77 ) - 0.2 );
				float temp_output_13_0_g77 = max( ( temp_output_7_0_g77 - temp_output_10_0_g77 ) , 0.0 );
				float4 tex2DNode9_g76 = tex2D( _Tint, temp_output_6_0_g76 );
				float temp_output_14_0_g77 = max( ( temp_output_8_0_g77 - temp_output_10_0_g77 ) , 0.0 );
				float temp_output_14_0_g96 = max( ( temp_output_8_0_g96 - temp_output_10_0_g96 ) , 0.0 );
				float temp_output_14_0_g99 = max( ( temp_output_8_0_g99 - temp_output_10_0_g99 ) , 0.0 );
				float4 temp_output_64_0 = ( ( ( ( ( ( ( ( ( tex2DNode11_g80 * temp_output_13_0_g81 ) + ( tex2DNode9_g80 * temp_output_14_0_g81 ) ) / ( temp_output_13_0_g81 + temp_output_14_0_g81 ) ) * temp_output_13_0_g95 ) + ( ( ( ( tex2DNode11_g84 * temp_output_13_0_g85 ) + ( tex2DNode9_g84 * temp_output_14_0_g85 ) ) / ( temp_output_13_0_g85 + temp_output_14_0_g85 ) ) * temp_output_14_0_g95 ) ) / ( temp_output_13_0_g95 + temp_output_14_0_g95 ) ) * temp_output_13_0_g99 ) + ( ( ( ( ( ( ( tex2DNode11_g88 * temp_output_13_0_g89 ) + ( tex2DNode9_g88 * temp_output_14_0_g89 ) ) / ( temp_output_13_0_g89 + temp_output_14_0_g89 ) ) * temp_output_13_0_g96 ) + ( ( ( ( tex2DNode11_g76 * temp_output_13_0_g77 ) + ( tex2DNode9_g76 * temp_output_14_0_g77 ) ) / ( temp_output_13_0_g77 + temp_output_14_0_g77 ) ) * temp_output_14_0_g96 ) ) / ( temp_output_13_0_g96 + temp_output_14_0_g96 ) ) * temp_output_14_0_g99 ) ) / ( temp_output_13_0_g99 + temp_output_14_0_g99 ) );
				float temp_output_3_0_g102 = ( temp_output_64_0.w * 2.0 );
				float ifLocalVar169_g104 = 0;
				if( temp_output_147_0_g104 < 1.0 )
				ifLocalVar169_g104 = min( temp_output_3_0_g102 , 1.0 );
				
				surfaceDescription.Normal = UnpackNormalScale( appendResult71, 1.0 );
				surfaceDescription.Smoothness = ifLocalVar158_g104;
				surfaceDescription.Alpha = ifLocalVar169_g104;

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

			#define _BLENDMODE_PRESERVE_SPECULAR_LIGHTING 1
			#define _DISABLE_SSR_TRANSPARENT 1
			#define _SPECULAR_OCCLUSION_FROM_AO 1
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
			float4 _MaterialArray_ST;
			float4 _Control_TexelSize;
			float4 _ShapeArray_ST;
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
			TEXTURE2D_ARRAY(_ShapeArray);
			sampler2D _GrassControl;
			sampler2D _Control;
			SAMPLER(sampler_ShapeArray);
			sampler2D _GrassTint;
			sampler2D _SpatterTex;
			sampler2D _Noise;
			TEXTURE2D_ARRAY(_MaterialArray);
			SAMPLER(sampler_MaterialArray);
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
				float2 uv_ShapeArray = packedInput.ase_texcoord3.xy;
				float3 ase_worldPos = packedInput.ase_texcoord4.xyz;
				float3 break17 = ase_worldPos;
				float2 appendResult26 = (float2(break17.x , break17.z));
				float4 break18 = _WorldBounds;
				float2 appendResult27 = (float2(break18.x , break18.y));
				float2 appendResult28 = (float2(break18.w , break18.z));
				float2 appendResult35 = (float2(_Control_TexelSize.z , _Control_TexelSize.w));
				float2 temp_output_36_0 = ( ( ( appendResult26 - appendResult27 ) / ( appendResult28 - appendResult27 ) ) * appendResult35 );
				float2 temp_output_39_0 = ( floor( ( temp_output_36_0 + float2( 0.5,0.5 ) ) ) - float2( 0.5,0.5 ) );
				float2 temp_output_6_0_g80 = ( ( temp_output_39_0 + float2( 0,0 ) ) / appendResult35 );
				float4 tex2DNode10_g80 = tex2D( _GrassControl, temp_output_6_0_g80 );
				float4 tex2DArrayNode27_g80 = SAMPLE_TEXTURE2D_ARRAY( _ShapeArray, sampler_ShapeArray, uv_ShapeArray,( tex2DNode10_g80.g * 255.0 ) );
				float4 tex2DNode11_g80 = tex2D( _GrassTint, temp_output_6_0_g80 );
				float temp_output_7_0_g82 = ( tex2DArrayNode27_g80.b + tex2DNode11_g80.a );
				float4 tex2DNode8_g80 = tex2D( _Control, temp_output_6_0_g80 );
				float4 tex2DArrayNode24_g80 = SAMPLE_TEXTURE2D_ARRAY( _ShapeArray, sampler_ShapeArray, uv_ShapeArray,( tex2DNode8_g80.g * 255.0 ) );
				float temp_output_30_0_g80 = ( 1.0 - tex2DNode11_g80.a );
				float temp_output_8_0_g82 = ( tex2DArrayNode24_g80.b + temp_output_30_0_g80 );
				float temp_output_10_0_g82 = ( max( temp_output_7_0_g82 , temp_output_8_0_g82 ) - 0.2 );
				float temp_output_13_0_g82 = max( ( temp_output_7_0_g82 - temp_output_10_0_g82 ) , 0.0 );
				float temp_output_14_0_g82 = max( ( temp_output_8_0_g82 - temp_output_10_0_g82 ) , 0.0 );
				float temp_output_77_42 = ( ( ( tex2DArrayNode27_g80 * temp_output_13_0_g82 ) + ( tex2DArrayNode24_g80 * temp_output_14_0_g82 ) ) / ( temp_output_13_0_g82 + temp_output_14_0_g82 ) ).z;
				float2 break41 = ( temp_output_36_0 - temp_output_39_0 );
				float temp_output_49_0 = ( 1.0 - break41.x );
				float temp_output_7_0_g93 = ( temp_output_77_42 + temp_output_49_0 );
				float2 temp_output_6_0_g84 = ( ( temp_output_39_0 + float2( 0,0 ) ) / appendResult35 );
				float4 tex2DNode10_g84 = tex2D( _GrassControl, temp_output_6_0_g84 );
				float4 tex2DArrayNode27_g84 = SAMPLE_TEXTURE2D_ARRAY( _ShapeArray, sampler_ShapeArray, uv_ShapeArray,( tex2DNode10_g84.g * 255.0 ) );
				float4 tex2DNode11_g84 = tex2D( _GrassTint, temp_output_6_0_g84 );
				float temp_output_7_0_g86 = ( tex2DArrayNode27_g84.b + tex2DNode11_g84.a );
				float4 tex2DNode8_g84 = tex2D( _Control, temp_output_6_0_g84 );
				float4 tex2DArrayNode24_g84 = SAMPLE_TEXTURE2D_ARRAY( _ShapeArray, sampler_ShapeArray, uv_ShapeArray,( tex2DNode8_g84.g * 255.0 ) );
				float temp_output_30_0_g84 = ( 1.0 - tex2DNode11_g84.a );
				float temp_output_8_0_g86 = ( tex2DArrayNode24_g84.b + temp_output_30_0_g84 );
				float temp_output_10_0_g86 = ( max( temp_output_7_0_g86 , temp_output_8_0_g86 ) - 0.2 );
				float temp_output_13_0_g86 = max( ( temp_output_7_0_g86 - temp_output_10_0_g86 ) , 0.0 );
				float temp_output_14_0_g86 = max( ( temp_output_8_0_g86 - temp_output_10_0_g86 ) , 0.0 );
				float temp_output_79_42 = ( ( ( tex2DArrayNode27_g84 * temp_output_13_0_g86 ) + ( tex2DArrayNode24_g84 * temp_output_14_0_g86 ) ) / ( temp_output_13_0_g86 + temp_output_14_0_g86 ) ).z;
				float temp_output_8_0_g93 = ( temp_output_79_42 + break41.x );
				float temp_output_10_0_g93 = ( max( temp_output_7_0_g93 , temp_output_8_0_g93 ) - 0.2 );
				float temp_output_13_0_g93 = max( ( temp_output_7_0_g93 - temp_output_10_0_g93 ) , 0.0 );
				float temp_output_14_0_g93 = max( ( temp_output_8_0_g93 - temp_output_10_0_g93 ) , 0.0 );
				float4 temp_output_44_0 = ( ( ( ( ( ( tex2DArrayNode27_g80 * temp_output_13_0_g82 ) + ( tex2DArrayNode24_g80 * temp_output_14_0_g82 ) ) / ( temp_output_13_0_g82 + temp_output_14_0_g82 ) ) * temp_output_13_0_g93 ) + ( ( ( ( tex2DArrayNode27_g84 * temp_output_13_0_g86 ) + ( tex2DArrayNode24_g84 * temp_output_14_0_g86 ) ) / ( temp_output_13_0_g86 + temp_output_14_0_g86 ) ) * temp_output_14_0_g93 ) ) / ( temp_output_13_0_g93 + temp_output_14_0_g93 ) );
				float temp_output_61_0 = ( 1.0 - break41.y );
				float temp_output_7_0_g103 = ( temp_output_44_0.z + temp_output_61_0 );
				float2 temp_output_6_0_g88 = ( ( temp_output_39_0 + float2( 0,0 ) ) / appendResult35 );
				float4 tex2DNode10_g88 = tex2D( _GrassControl, temp_output_6_0_g88 );
				float4 tex2DArrayNode27_g88 = SAMPLE_TEXTURE2D_ARRAY( _ShapeArray, sampler_ShapeArray, uv_ShapeArray,( tex2DNode10_g88.g * 255.0 ) );
				float4 tex2DNode11_g88 = tex2D( _GrassTint, temp_output_6_0_g88 );
				float temp_output_7_0_g90 = ( tex2DArrayNode27_g88.b + tex2DNode11_g88.a );
				float4 tex2DNode8_g88 = tex2D( _Control, temp_output_6_0_g88 );
				float4 tex2DArrayNode24_g88 = SAMPLE_TEXTURE2D_ARRAY( _ShapeArray, sampler_ShapeArray, uv_ShapeArray,( tex2DNode8_g88.g * 255.0 ) );
				float temp_output_30_0_g88 = ( 1.0 - tex2DNode11_g88.a );
				float temp_output_8_0_g90 = ( tex2DArrayNode24_g88.b + temp_output_30_0_g88 );
				float temp_output_10_0_g90 = ( max( temp_output_7_0_g90 , temp_output_8_0_g90 ) - 0.2 );
				float temp_output_13_0_g90 = max( ( temp_output_7_0_g90 - temp_output_10_0_g90 ) , 0.0 );
				float temp_output_14_0_g90 = max( ( temp_output_8_0_g90 - temp_output_10_0_g90 ) , 0.0 );
				float temp_output_80_42 = ( ( ( tex2DArrayNode27_g88 * temp_output_13_0_g90 ) + ( tex2DArrayNode24_g88 * temp_output_14_0_g90 ) ) / ( temp_output_13_0_g90 + temp_output_14_0_g90 ) ).z;
				float temp_output_7_0_g94 = ( temp_output_80_42 + temp_output_49_0 );
				float2 temp_output_6_0_g76 = ( ( temp_output_39_0 + float2( 0,0 ) ) / appendResult35 );
				float4 tex2DNode10_g76 = tex2D( _GrassControl, temp_output_6_0_g76 );
				float4 tex2DArrayNode27_g76 = SAMPLE_TEXTURE2D_ARRAY( _ShapeArray, sampler_ShapeArray, uv_ShapeArray,( tex2DNode10_g76.g * 255.0 ) );
				float4 tex2DNode11_g76 = tex2D( _GrassTint, temp_output_6_0_g76 );
				float temp_output_7_0_g78 = ( tex2DArrayNode27_g76.b + tex2DNode11_g76.a );
				float4 tex2DNode8_g76 = tex2D( _Control, temp_output_6_0_g76 );
				float4 tex2DArrayNode24_g76 = SAMPLE_TEXTURE2D_ARRAY( _ShapeArray, sampler_ShapeArray, uv_ShapeArray,( tex2DNode8_g76.g * 255.0 ) );
				float temp_output_30_0_g76 = ( 1.0 - tex2DNode11_g76.a );
				float temp_output_8_0_g78 = ( tex2DArrayNode24_g76.b + temp_output_30_0_g76 );
				float temp_output_10_0_g78 = ( max( temp_output_7_0_g78 , temp_output_8_0_g78 ) - 0.2 );
				float temp_output_13_0_g78 = max( ( temp_output_7_0_g78 - temp_output_10_0_g78 ) , 0.0 );
				float temp_output_14_0_g78 = max( ( temp_output_8_0_g78 - temp_output_10_0_g78 ) , 0.0 );
				float temp_output_78_42 = ( ( ( tex2DArrayNode27_g76 * temp_output_13_0_g78 ) + ( tex2DArrayNode24_g76 * temp_output_14_0_g78 ) ) / ( temp_output_13_0_g78 + temp_output_14_0_g78 ) ).z;
				float temp_output_8_0_g94 = ( temp_output_78_42 + break41.x );
				float temp_output_10_0_g94 = ( max( temp_output_7_0_g94 , temp_output_8_0_g94 ) - 0.2 );
				float temp_output_13_0_g94 = max( ( temp_output_7_0_g94 - temp_output_10_0_g94 ) , 0.0 );
				float temp_output_14_0_g94 = max( ( temp_output_8_0_g94 - temp_output_10_0_g94 ) , 0.0 );
				float4 temp_output_47_0 = ( ( ( ( ( ( tex2DArrayNode27_g88 * temp_output_13_0_g90 ) + ( tex2DArrayNode24_g88 * temp_output_14_0_g90 ) ) / ( temp_output_13_0_g90 + temp_output_14_0_g90 ) ) * temp_output_13_0_g94 ) + ( ( ( ( tex2DArrayNode27_g76 * temp_output_13_0_g78 ) + ( tex2DArrayNode24_g76 * temp_output_14_0_g78 ) ) / ( temp_output_13_0_g78 + temp_output_14_0_g78 ) ) * temp_output_14_0_g94 ) ) / ( temp_output_13_0_g94 + temp_output_14_0_g94 ) );
				float temp_output_8_0_g103 = ( temp_output_47_0.z + break41.y );
				float temp_output_10_0_g103 = ( max( temp_output_7_0_g103 , temp_output_8_0_g103 ) - 0.2 );
				float temp_output_13_0_g103 = max( ( temp_output_7_0_g103 - temp_output_10_0_g103 ) , 0.0 );
				float temp_output_14_0_g103 = max( ( temp_output_8_0_g103 - temp_output_10_0_g103 ) , 0.0 );
				float4 break65 = ( ( ( temp_output_44_0 * temp_output_13_0_g103 ) + ( temp_output_47_0 * temp_output_14_0_g103 ) ) / ( temp_output_13_0_g103 + temp_output_14_0_g103 ) );
				float4 appendResult71 = (float4(0.0 , break65.y , break65.y , break65.w));
				
				float3 ase_worldNormal = packedInput.ase_texcoord5.xyz;
				float dotResult145_g104 = dot( ase_worldNormal , float3( 0,1,0 ) );
				float3 break180_g104 = ase_worldPos;
				float2 appendResult102_g104 = (float2(break180_g104.x , break180_g104.z));
				float4 break174_g104 = _WorldBounds;
				float2 appendResult129_g104 = (float2(break174_g104.x , break174_g104.y));
				float2 appendResult130_g104 = (float2(break174_g104.z , break174_g104.w));
				float4 tex2DNode133_g104 = tex2D( _SpatterTex, ( ( appendResult102_g104 - appendResult129_g104 ) / ( appendResult130_g104 - appendResult129_g104 ) ) );
				float lerpResult139_g104 = lerp( 1.0 , -1.0 , ( tex2DNode133_g104.a - tex2D( _Noise, appendResult102_g104 ).r ));
				float temp_output_147_0_g104 = ( dotResult145_g104 >= lerpResult139_g104 ? 1.0 : 0.0 );
				float2 uv_MaterialArray = packedInput.ase_texcoord3.xy;
				float temp_output_7_0_g83 = ( tex2DArrayNode27_g80.b + tex2DNode11_g80.a );
				float temp_output_8_0_g83 = ( tex2DArrayNode24_g80.b + temp_output_30_0_g80 );
				float temp_output_10_0_g83 = ( max( temp_output_7_0_g83 , temp_output_8_0_g83 ) - 0.2 );
				float temp_output_13_0_g83 = max( ( temp_output_7_0_g83 - temp_output_10_0_g83 ) , 0.0 );
				float4 tex2DArrayNode21_g80 = SAMPLE_TEXTURE2D_ARRAY( _MaterialArray, sampler_MaterialArray, uv_MaterialArray,( tex2DNode8_g80.r * 255.0 ) );
				float temp_output_14_0_g83 = max( ( temp_output_8_0_g83 - temp_output_10_0_g83 ) , 0.0 );
				float temp_output_7_0_g97 = ( temp_output_77_42 + temp_output_49_0 );
				float temp_output_8_0_g97 = ( temp_output_79_42 + break41.x );
				float temp_output_10_0_g97 = ( max( temp_output_7_0_g97 , temp_output_8_0_g97 ) - 0.2 );
				float temp_output_13_0_g97 = max( ( temp_output_7_0_g97 - temp_output_10_0_g97 ) , 0.0 );
				float temp_output_7_0_g87 = ( tex2DArrayNode27_g84.b + tex2DNode11_g84.a );
				float temp_output_8_0_g87 = ( tex2DArrayNode24_g84.b + temp_output_30_0_g84 );
				float temp_output_10_0_g87 = ( max( temp_output_7_0_g87 , temp_output_8_0_g87 ) - 0.2 );
				float temp_output_13_0_g87 = max( ( temp_output_7_0_g87 - temp_output_10_0_g87 ) , 0.0 );
				float4 tex2DArrayNode21_g84 = SAMPLE_TEXTURE2D_ARRAY( _MaterialArray, sampler_MaterialArray, uv_MaterialArray,( tex2DNode8_g84.r * 255.0 ) );
				float temp_output_14_0_g87 = max( ( temp_output_8_0_g87 - temp_output_10_0_g87 ) , 0.0 );
				float temp_output_14_0_g97 = max( ( temp_output_8_0_g97 - temp_output_10_0_g97 ) , 0.0 );
				float temp_output_7_0_g100 = ( temp_output_44_0.z + temp_output_61_0 );
				float temp_output_8_0_g100 = ( temp_output_47_0.z + break41.y );
				float temp_output_10_0_g100 = ( max( temp_output_7_0_g100 , temp_output_8_0_g100 ) - 0.2 );
				float temp_output_13_0_g100 = max( ( temp_output_7_0_g100 - temp_output_10_0_g100 ) , 0.0 );
				float temp_output_7_0_g91 = ( tex2DArrayNode27_g88.b + tex2DNode11_g88.a );
				float temp_output_8_0_g91 = ( tex2DArrayNode24_g88.b + temp_output_30_0_g88 );
				float temp_output_10_0_g91 = ( max( temp_output_7_0_g91 , temp_output_8_0_g91 ) - 0.2 );
				float temp_output_13_0_g91 = max( ( temp_output_7_0_g91 - temp_output_10_0_g91 ) , 0.0 );
				float4 tex2DArrayNode21_g88 = SAMPLE_TEXTURE2D_ARRAY( _MaterialArray, sampler_MaterialArray, uv_MaterialArray,( tex2DNode8_g88.r * 255.0 ) );
				float temp_output_14_0_g91 = max( ( temp_output_8_0_g91 - temp_output_10_0_g91 ) , 0.0 );
				float temp_output_7_0_g98 = ( temp_output_80_42 + temp_output_49_0 );
				float temp_output_8_0_g98 = ( temp_output_78_42 + break41.x );
				float temp_output_10_0_g98 = ( max( temp_output_7_0_g98 , temp_output_8_0_g98 ) - 0.2 );
				float temp_output_13_0_g98 = max( ( temp_output_7_0_g98 - temp_output_10_0_g98 ) , 0.0 );
				float temp_output_7_0_g79 = ( tex2DArrayNode27_g76.b + tex2DNode11_g76.a );
				float temp_output_8_0_g79 = ( tex2DArrayNode24_g76.b + temp_output_30_0_g76 );
				float temp_output_10_0_g79 = ( max( temp_output_7_0_g79 , temp_output_8_0_g79 ) - 0.2 );
				float temp_output_13_0_g79 = max( ( temp_output_7_0_g79 - temp_output_10_0_g79 ) , 0.0 );
				float4 tex2DArrayNode21_g76 = SAMPLE_TEXTURE2D_ARRAY( _MaterialArray, sampler_MaterialArray, uv_MaterialArray,( tex2DNode8_g76.r * 255.0 ) );
				float temp_output_14_0_g79 = max( ( temp_output_8_0_g79 - temp_output_10_0_g79 ) , 0.0 );
				float temp_output_14_0_g98 = max( ( temp_output_8_0_g98 - temp_output_10_0_g98 ) , 0.0 );
				float temp_output_14_0_g100 = max( ( temp_output_8_0_g100 - temp_output_10_0_g100 ) , 0.0 );
				float4 temp_output_62_0 = ( ( ( ( ( ( ( ( ( SAMPLE_TEXTURE2D_ARRAY( _MaterialArray, sampler_MaterialArray, uv_MaterialArray,( tex2DNode10_g80.r * 255.0 ) ) * temp_output_13_0_g83 ) + ( tex2DArrayNode21_g80 * temp_output_14_0_g83 ) ) / ( temp_output_13_0_g83 + temp_output_14_0_g83 ) ) * temp_output_13_0_g97 ) + ( ( ( ( SAMPLE_TEXTURE2D_ARRAY( _MaterialArray, sampler_MaterialArray, uv_MaterialArray,( tex2DNode10_g84.r * 255.0 ) ) * temp_output_13_0_g87 ) + ( tex2DArrayNode21_g84 * temp_output_14_0_g87 ) ) / ( temp_output_13_0_g87 + temp_output_14_0_g87 ) ) * temp_output_14_0_g97 ) ) / ( temp_output_13_0_g97 + temp_output_14_0_g97 ) ) * temp_output_13_0_g100 ) + ( ( ( ( ( ( ( SAMPLE_TEXTURE2D_ARRAY( _MaterialArray, sampler_MaterialArray, uv_MaterialArray,( tex2DNode10_g88.r * 255.0 ) ) * temp_output_13_0_g91 ) + ( tex2DArrayNode21_g88 * temp_output_14_0_g91 ) ) / ( temp_output_13_0_g91 + temp_output_14_0_g91 ) ) * temp_output_13_0_g98 ) + ( ( ( ( SAMPLE_TEXTURE2D_ARRAY( _MaterialArray, sampler_MaterialArray, uv_MaterialArray,( tex2DNode10_g76.r * 255.0 ) ) * temp_output_13_0_g79 ) + ( tex2DArrayNode21_g76 * temp_output_14_0_g79 ) ) / ( temp_output_13_0_g79 + temp_output_14_0_g79 ) ) * temp_output_14_0_g98 ) ) / ( temp_output_13_0_g98 + temp_output_14_0_g98 ) ) * temp_output_14_0_g100 ) ) / ( temp_output_13_0_g100 + temp_output_14_0_g100 ) );
				float ifLocalVar158_g104 = 0;
				UNITY_BRANCH 
				if( temp_output_147_0_g104 < 0.5 )
				ifLocalVar158_g104 = temp_output_62_0.w;
				
				float temp_output_7_0_g81 = ( tex2DArrayNode27_g80.b + tex2DNode11_g80.a );
				float temp_output_8_0_g81 = ( tex2DArrayNode24_g80.b + temp_output_30_0_g80 );
				float temp_output_10_0_g81 = ( max( temp_output_7_0_g81 , temp_output_8_0_g81 ) - 0.2 );
				float temp_output_13_0_g81 = max( ( temp_output_7_0_g81 - temp_output_10_0_g81 ) , 0.0 );
				float4 tex2DNode9_g80 = tex2D( _Tint, temp_output_6_0_g80 );
				float temp_output_14_0_g81 = max( ( temp_output_8_0_g81 - temp_output_10_0_g81 ) , 0.0 );
				float temp_output_7_0_g95 = ( temp_output_77_42 + temp_output_49_0 );
				float temp_output_8_0_g95 = ( temp_output_79_42 + break41.x );
				float temp_output_10_0_g95 = ( max( temp_output_7_0_g95 , temp_output_8_0_g95 ) - 0.2 );
				float temp_output_13_0_g95 = max( ( temp_output_7_0_g95 - temp_output_10_0_g95 ) , 0.0 );
				float temp_output_7_0_g85 = ( tex2DArrayNode27_g84.b + tex2DNode11_g84.a );
				float temp_output_8_0_g85 = ( tex2DArrayNode24_g84.b + temp_output_30_0_g84 );
				float temp_output_10_0_g85 = ( max( temp_output_7_0_g85 , temp_output_8_0_g85 ) - 0.2 );
				float temp_output_13_0_g85 = max( ( temp_output_7_0_g85 - temp_output_10_0_g85 ) , 0.0 );
				float4 tex2DNode9_g84 = tex2D( _Tint, temp_output_6_0_g84 );
				float temp_output_14_0_g85 = max( ( temp_output_8_0_g85 - temp_output_10_0_g85 ) , 0.0 );
				float temp_output_14_0_g95 = max( ( temp_output_8_0_g95 - temp_output_10_0_g95 ) , 0.0 );
				float temp_output_7_0_g99 = ( temp_output_44_0.z + temp_output_61_0 );
				float temp_output_8_0_g99 = ( temp_output_47_0.z + break41.y );
				float temp_output_10_0_g99 = ( max( temp_output_7_0_g99 , temp_output_8_0_g99 ) - 0.2 );
				float temp_output_13_0_g99 = max( ( temp_output_7_0_g99 - temp_output_10_0_g99 ) , 0.0 );
				float temp_output_7_0_g89 = ( tex2DArrayNode27_g88.b + tex2DNode11_g88.a );
				float temp_output_8_0_g89 = ( tex2DArrayNode24_g88.b + temp_output_30_0_g88 );
				float temp_output_10_0_g89 = ( max( temp_output_7_0_g89 , temp_output_8_0_g89 ) - 0.2 );
				float temp_output_13_0_g89 = max( ( temp_output_7_0_g89 - temp_output_10_0_g89 ) , 0.0 );
				float4 tex2DNode9_g88 = tex2D( _Tint, temp_output_6_0_g88 );
				float temp_output_14_0_g89 = max( ( temp_output_8_0_g89 - temp_output_10_0_g89 ) , 0.0 );
				float temp_output_7_0_g96 = ( temp_output_80_42 + temp_output_49_0 );
				float temp_output_8_0_g96 = ( temp_output_78_42 + break41.x );
				float temp_output_10_0_g96 = ( max( temp_output_7_0_g96 , temp_output_8_0_g96 ) - 0.2 );
				float temp_output_13_0_g96 = max( ( temp_output_7_0_g96 - temp_output_10_0_g96 ) , 0.0 );
				float temp_output_7_0_g77 = ( tex2DArrayNode27_g76.b + tex2DNode11_g76.a );
				float temp_output_8_0_g77 = ( tex2DArrayNode24_g76.b + temp_output_30_0_g76 );
				float temp_output_10_0_g77 = ( max( temp_output_7_0_g77 , temp_output_8_0_g77 ) - 0.2 );
				float temp_output_13_0_g77 = max( ( temp_output_7_0_g77 - temp_output_10_0_g77 ) , 0.0 );
				float4 tex2DNode9_g76 = tex2D( _Tint, temp_output_6_0_g76 );
				float temp_output_14_0_g77 = max( ( temp_output_8_0_g77 - temp_output_10_0_g77 ) , 0.0 );
				float temp_output_14_0_g96 = max( ( temp_output_8_0_g96 - temp_output_10_0_g96 ) , 0.0 );
				float temp_output_14_0_g99 = max( ( temp_output_8_0_g99 - temp_output_10_0_g99 ) , 0.0 );
				float4 temp_output_64_0 = ( ( ( ( ( ( ( ( ( tex2DNode11_g80 * temp_output_13_0_g81 ) + ( tex2DNode9_g80 * temp_output_14_0_g81 ) ) / ( temp_output_13_0_g81 + temp_output_14_0_g81 ) ) * temp_output_13_0_g95 ) + ( ( ( ( tex2DNode11_g84 * temp_output_13_0_g85 ) + ( tex2DNode9_g84 * temp_output_14_0_g85 ) ) / ( temp_output_13_0_g85 + temp_output_14_0_g85 ) ) * temp_output_14_0_g95 ) ) / ( temp_output_13_0_g95 + temp_output_14_0_g95 ) ) * temp_output_13_0_g99 ) + ( ( ( ( ( ( ( tex2DNode11_g88 * temp_output_13_0_g89 ) + ( tex2DNode9_g88 * temp_output_14_0_g89 ) ) / ( temp_output_13_0_g89 + temp_output_14_0_g89 ) ) * temp_output_13_0_g96 ) + ( ( ( ( tex2DNode11_g76 * temp_output_13_0_g77 ) + ( tex2DNode9_g76 * temp_output_14_0_g77 ) ) / ( temp_output_13_0_g77 + temp_output_14_0_g77 ) ) * temp_output_14_0_g96 ) ) / ( temp_output_13_0_g96 + temp_output_14_0_g96 ) ) * temp_output_14_0_g99 ) ) / ( temp_output_13_0_g99 + temp_output_14_0_g99 ) );
				float temp_output_3_0_g102 = ( temp_output_64_0.w * 2.0 );
				float ifLocalVar169_g104 = 0;
				if( temp_output_147_0_g104 < 1.0 )
				ifLocalVar169_g104 = min( temp_output_3_0_g102 , 1.0 );
				
				surfaceDescription.Normal = UnpackNormalScale( appendResult71, 1.0 );
				surfaceDescription.Smoothness = ifLocalVar158_g104;
				surfaceDescription.Alpha = ifLocalVar169_g104;

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

			#define _BLENDMODE_PRESERVE_SPECULAR_LIGHTING 1
			#define _DISABLE_SSR_TRANSPARENT 1
			#define _SPECULAR_OCCLUSION_FROM_AO 1
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
			float4 _MaterialArray_ST;
			float4 _Control_TexelSize;
			float4 _ShapeArray_ST;
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
			sampler2D _Noise;
			TEXTURE2D_ARRAY(_MaterialArray);
			sampler2D _GrassControl;
			sampler2D _Control;
			SAMPLER(sampler_MaterialArray);
			TEXTURE2D_ARRAY(_ShapeArray);
			SAMPLER(sampler_ShapeArray);
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
				float dotResult145_g104 = dot( normalWS , float3( 0,1,0 ) );
				float3 ase_worldPos = GetAbsolutePositionWS( positionRWS );
				float3 break180_g104 = ase_worldPos;
				float2 appendResult102_g104 = (float2(break180_g104.x , break180_g104.z));
				float4 break174_g104 = _WorldBounds;
				float2 appendResult129_g104 = (float2(break174_g104.x , break174_g104.y));
				float2 appendResult130_g104 = (float2(break174_g104.z , break174_g104.w));
				float4 tex2DNode133_g104 = tex2D( _SpatterTex, ( ( appendResult102_g104 - appendResult129_g104 ) / ( appendResult130_g104 - appendResult129_g104 ) ) );
				float lerpResult139_g104 = lerp( 1.0 , -1.0 , ( tex2DNode133_g104.a - tex2D( _Noise, appendResult102_g104 ).r ));
				float temp_output_147_0_g104 = ( dotResult145_g104 >= lerpResult139_g104 ? 1.0 : 0.0 );
				float2 uv_MaterialArray = packedInput.ase_texcoord7.xy;
				float3 break17 = ase_worldPos;
				float2 appendResult26 = (float2(break17.x , break17.z));
				float4 break18 = _WorldBounds;
				float2 appendResult27 = (float2(break18.x , break18.y));
				float2 appendResult28 = (float2(break18.w , break18.z));
				float2 appendResult35 = (float2(_Control_TexelSize.z , _Control_TexelSize.w));
				float2 temp_output_36_0 = ( ( ( appendResult26 - appendResult27 ) / ( appendResult28 - appendResult27 ) ) * appendResult35 );
				float2 temp_output_39_0 = ( floor( ( temp_output_36_0 + float2( 0.5,0.5 ) ) ) - float2( 0.5,0.5 ) );
				float2 temp_output_6_0_g80 = ( ( temp_output_39_0 + float2( 0,0 ) ) / appendResult35 );
				float4 tex2DNode10_g80 = tex2D( _GrassControl, temp_output_6_0_g80 );
				float2 uv_ShapeArray = packedInput.ase_texcoord7.xy;
				float4 tex2DArrayNode27_g80 = SAMPLE_TEXTURE2D_ARRAY( _ShapeArray, sampler_ShapeArray, uv_ShapeArray,( tex2DNode10_g80.g * 255.0 ) );
				float4 tex2DNode11_g80 = tex2D( _GrassTint, temp_output_6_0_g80 );
				float temp_output_7_0_g83 = ( tex2DArrayNode27_g80.b + tex2DNode11_g80.a );
				float4 tex2DNode8_g80 = tex2D( _Control, temp_output_6_0_g80 );
				float4 tex2DArrayNode24_g80 = SAMPLE_TEXTURE2D_ARRAY( _ShapeArray, sampler_ShapeArray, uv_ShapeArray,( tex2DNode8_g80.g * 255.0 ) );
				float temp_output_30_0_g80 = ( 1.0 - tex2DNode11_g80.a );
				float temp_output_8_0_g83 = ( tex2DArrayNode24_g80.b + temp_output_30_0_g80 );
				float temp_output_10_0_g83 = ( max( temp_output_7_0_g83 , temp_output_8_0_g83 ) - 0.2 );
				float temp_output_13_0_g83 = max( ( temp_output_7_0_g83 - temp_output_10_0_g83 ) , 0.0 );
				float4 tex2DArrayNode21_g80 = SAMPLE_TEXTURE2D_ARRAY( _MaterialArray, sampler_MaterialArray, uv_MaterialArray,( tex2DNode8_g80.r * 255.0 ) );
				float temp_output_14_0_g83 = max( ( temp_output_8_0_g83 - temp_output_10_0_g83 ) , 0.0 );
				float temp_output_7_0_g82 = ( tex2DArrayNode27_g80.b + tex2DNode11_g80.a );
				float temp_output_8_0_g82 = ( tex2DArrayNode24_g80.b + temp_output_30_0_g80 );
				float temp_output_10_0_g82 = ( max( temp_output_7_0_g82 , temp_output_8_0_g82 ) - 0.2 );
				float temp_output_13_0_g82 = max( ( temp_output_7_0_g82 - temp_output_10_0_g82 ) , 0.0 );
				float temp_output_14_0_g82 = max( ( temp_output_8_0_g82 - temp_output_10_0_g82 ) , 0.0 );
				float temp_output_77_42 = ( ( ( tex2DArrayNode27_g80 * temp_output_13_0_g82 ) + ( tex2DArrayNode24_g80 * temp_output_14_0_g82 ) ) / ( temp_output_13_0_g82 + temp_output_14_0_g82 ) ).z;
				float2 break41 = ( temp_output_36_0 - temp_output_39_0 );
				float temp_output_49_0 = ( 1.0 - break41.x );
				float temp_output_7_0_g97 = ( temp_output_77_42 + temp_output_49_0 );
				float2 temp_output_6_0_g84 = ( ( temp_output_39_0 + float2( 0,0 ) ) / appendResult35 );
				float4 tex2DNode10_g84 = tex2D( _GrassControl, temp_output_6_0_g84 );
				float4 tex2DArrayNode27_g84 = SAMPLE_TEXTURE2D_ARRAY( _ShapeArray, sampler_ShapeArray, uv_ShapeArray,( tex2DNode10_g84.g * 255.0 ) );
				float4 tex2DNode11_g84 = tex2D( _GrassTint, temp_output_6_0_g84 );
				float temp_output_7_0_g86 = ( tex2DArrayNode27_g84.b + tex2DNode11_g84.a );
				float4 tex2DNode8_g84 = tex2D( _Control, temp_output_6_0_g84 );
				float4 tex2DArrayNode24_g84 = SAMPLE_TEXTURE2D_ARRAY( _ShapeArray, sampler_ShapeArray, uv_ShapeArray,( tex2DNode8_g84.g * 255.0 ) );
				float temp_output_30_0_g84 = ( 1.0 - tex2DNode11_g84.a );
				float temp_output_8_0_g86 = ( tex2DArrayNode24_g84.b + temp_output_30_0_g84 );
				float temp_output_10_0_g86 = ( max( temp_output_7_0_g86 , temp_output_8_0_g86 ) - 0.2 );
				float temp_output_13_0_g86 = max( ( temp_output_7_0_g86 - temp_output_10_0_g86 ) , 0.0 );
				float temp_output_14_0_g86 = max( ( temp_output_8_0_g86 - temp_output_10_0_g86 ) , 0.0 );
				float temp_output_79_42 = ( ( ( tex2DArrayNode27_g84 * temp_output_13_0_g86 ) + ( tex2DArrayNode24_g84 * temp_output_14_0_g86 ) ) / ( temp_output_13_0_g86 + temp_output_14_0_g86 ) ).z;
				float temp_output_8_0_g97 = ( temp_output_79_42 + break41.x );
				float temp_output_10_0_g97 = ( max( temp_output_7_0_g97 , temp_output_8_0_g97 ) - 0.2 );
				float temp_output_13_0_g97 = max( ( temp_output_7_0_g97 - temp_output_10_0_g97 ) , 0.0 );
				float temp_output_7_0_g87 = ( tex2DArrayNode27_g84.b + tex2DNode11_g84.a );
				float temp_output_8_0_g87 = ( tex2DArrayNode24_g84.b + temp_output_30_0_g84 );
				float temp_output_10_0_g87 = ( max( temp_output_7_0_g87 , temp_output_8_0_g87 ) - 0.2 );
				float temp_output_13_0_g87 = max( ( temp_output_7_0_g87 - temp_output_10_0_g87 ) , 0.0 );
				float4 tex2DArrayNode21_g84 = SAMPLE_TEXTURE2D_ARRAY( _MaterialArray, sampler_MaterialArray, uv_MaterialArray,( tex2DNode8_g84.r * 255.0 ) );
				float temp_output_14_0_g87 = max( ( temp_output_8_0_g87 - temp_output_10_0_g87 ) , 0.0 );
				float temp_output_14_0_g97 = max( ( temp_output_8_0_g97 - temp_output_10_0_g97 ) , 0.0 );
				float temp_output_7_0_g93 = ( temp_output_77_42 + temp_output_49_0 );
				float temp_output_8_0_g93 = ( temp_output_79_42 + break41.x );
				float temp_output_10_0_g93 = ( max( temp_output_7_0_g93 , temp_output_8_0_g93 ) - 0.2 );
				float temp_output_13_0_g93 = max( ( temp_output_7_0_g93 - temp_output_10_0_g93 ) , 0.0 );
				float temp_output_14_0_g93 = max( ( temp_output_8_0_g93 - temp_output_10_0_g93 ) , 0.0 );
				float4 temp_output_44_0 = ( ( ( ( ( ( tex2DArrayNode27_g80 * temp_output_13_0_g82 ) + ( tex2DArrayNode24_g80 * temp_output_14_0_g82 ) ) / ( temp_output_13_0_g82 + temp_output_14_0_g82 ) ) * temp_output_13_0_g93 ) + ( ( ( ( tex2DArrayNode27_g84 * temp_output_13_0_g86 ) + ( tex2DArrayNode24_g84 * temp_output_14_0_g86 ) ) / ( temp_output_13_0_g86 + temp_output_14_0_g86 ) ) * temp_output_14_0_g93 ) ) / ( temp_output_13_0_g93 + temp_output_14_0_g93 ) );
				float temp_output_61_0 = ( 1.0 - break41.y );
				float temp_output_7_0_g100 = ( temp_output_44_0.z + temp_output_61_0 );
				float2 temp_output_6_0_g88 = ( ( temp_output_39_0 + float2( 0,0 ) ) / appendResult35 );
				float4 tex2DNode10_g88 = tex2D( _GrassControl, temp_output_6_0_g88 );
				float4 tex2DArrayNode27_g88 = SAMPLE_TEXTURE2D_ARRAY( _ShapeArray, sampler_ShapeArray, uv_ShapeArray,( tex2DNode10_g88.g * 255.0 ) );
				float4 tex2DNode11_g88 = tex2D( _GrassTint, temp_output_6_0_g88 );
				float temp_output_7_0_g90 = ( tex2DArrayNode27_g88.b + tex2DNode11_g88.a );
				float4 tex2DNode8_g88 = tex2D( _Control, temp_output_6_0_g88 );
				float4 tex2DArrayNode24_g88 = SAMPLE_TEXTURE2D_ARRAY( _ShapeArray, sampler_ShapeArray, uv_ShapeArray,( tex2DNode8_g88.g * 255.0 ) );
				float temp_output_30_0_g88 = ( 1.0 - tex2DNode11_g88.a );
				float temp_output_8_0_g90 = ( tex2DArrayNode24_g88.b + temp_output_30_0_g88 );
				float temp_output_10_0_g90 = ( max( temp_output_7_0_g90 , temp_output_8_0_g90 ) - 0.2 );
				float temp_output_13_0_g90 = max( ( temp_output_7_0_g90 - temp_output_10_0_g90 ) , 0.0 );
				float temp_output_14_0_g90 = max( ( temp_output_8_0_g90 - temp_output_10_0_g90 ) , 0.0 );
				float temp_output_80_42 = ( ( ( tex2DArrayNode27_g88 * temp_output_13_0_g90 ) + ( tex2DArrayNode24_g88 * temp_output_14_0_g90 ) ) / ( temp_output_13_0_g90 + temp_output_14_0_g90 ) ).z;
				float temp_output_7_0_g94 = ( temp_output_80_42 + temp_output_49_0 );
				float2 temp_output_6_0_g76 = ( ( temp_output_39_0 + float2( 0,0 ) ) / appendResult35 );
				float4 tex2DNode10_g76 = tex2D( _GrassControl, temp_output_6_0_g76 );
				float4 tex2DArrayNode27_g76 = SAMPLE_TEXTURE2D_ARRAY( _ShapeArray, sampler_ShapeArray, uv_ShapeArray,( tex2DNode10_g76.g * 255.0 ) );
				float4 tex2DNode11_g76 = tex2D( _GrassTint, temp_output_6_0_g76 );
				float temp_output_7_0_g78 = ( tex2DArrayNode27_g76.b + tex2DNode11_g76.a );
				float4 tex2DNode8_g76 = tex2D( _Control, temp_output_6_0_g76 );
				float4 tex2DArrayNode24_g76 = SAMPLE_TEXTURE2D_ARRAY( _ShapeArray, sampler_ShapeArray, uv_ShapeArray,( tex2DNode8_g76.g * 255.0 ) );
				float temp_output_30_0_g76 = ( 1.0 - tex2DNode11_g76.a );
				float temp_output_8_0_g78 = ( tex2DArrayNode24_g76.b + temp_output_30_0_g76 );
				float temp_output_10_0_g78 = ( max( temp_output_7_0_g78 , temp_output_8_0_g78 ) - 0.2 );
				float temp_output_13_0_g78 = max( ( temp_output_7_0_g78 - temp_output_10_0_g78 ) , 0.0 );
				float temp_output_14_0_g78 = max( ( temp_output_8_0_g78 - temp_output_10_0_g78 ) , 0.0 );
				float temp_output_78_42 = ( ( ( tex2DArrayNode27_g76 * temp_output_13_0_g78 ) + ( tex2DArrayNode24_g76 * temp_output_14_0_g78 ) ) / ( temp_output_13_0_g78 + temp_output_14_0_g78 ) ).z;
				float temp_output_8_0_g94 = ( temp_output_78_42 + break41.x );
				float temp_output_10_0_g94 = ( max( temp_output_7_0_g94 , temp_output_8_0_g94 ) - 0.2 );
				float temp_output_13_0_g94 = max( ( temp_output_7_0_g94 - temp_output_10_0_g94 ) , 0.0 );
				float temp_output_14_0_g94 = max( ( temp_output_8_0_g94 - temp_output_10_0_g94 ) , 0.0 );
				float4 temp_output_47_0 = ( ( ( ( ( ( tex2DArrayNode27_g88 * temp_output_13_0_g90 ) + ( tex2DArrayNode24_g88 * temp_output_14_0_g90 ) ) / ( temp_output_13_0_g90 + temp_output_14_0_g90 ) ) * temp_output_13_0_g94 ) + ( ( ( ( tex2DArrayNode27_g76 * temp_output_13_0_g78 ) + ( tex2DArrayNode24_g76 * temp_output_14_0_g78 ) ) / ( temp_output_13_0_g78 + temp_output_14_0_g78 ) ) * temp_output_14_0_g94 ) ) / ( temp_output_13_0_g94 + temp_output_14_0_g94 ) );
				float temp_output_8_0_g100 = ( temp_output_47_0.z + break41.y );
				float temp_output_10_0_g100 = ( max( temp_output_7_0_g100 , temp_output_8_0_g100 ) - 0.2 );
				float temp_output_13_0_g100 = max( ( temp_output_7_0_g100 - temp_output_10_0_g100 ) , 0.0 );
				float temp_output_7_0_g91 = ( tex2DArrayNode27_g88.b + tex2DNode11_g88.a );
				float temp_output_8_0_g91 = ( tex2DArrayNode24_g88.b + temp_output_30_0_g88 );
				float temp_output_10_0_g91 = ( max( temp_output_7_0_g91 , temp_output_8_0_g91 ) - 0.2 );
				float temp_output_13_0_g91 = max( ( temp_output_7_0_g91 - temp_output_10_0_g91 ) , 0.0 );
				float4 tex2DArrayNode21_g88 = SAMPLE_TEXTURE2D_ARRAY( _MaterialArray, sampler_MaterialArray, uv_MaterialArray,( tex2DNode8_g88.r * 255.0 ) );
				float temp_output_14_0_g91 = max( ( temp_output_8_0_g91 - temp_output_10_0_g91 ) , 0.0 );
				float temp_output_7_0_g98 = ( temp_output_80_42 + temp_output_49_0 );
				float temp_output_8_0_g98 = ( temp_output_78_42 + break41.x );
				float temp_output_10_0_g98 = ( max( temp_output_7_0_g98 , temp_output_8_0_g98 ) - 0.2 );
				float temp_output_13_0_g98 = max( ( temp_output_7_0_g98 - temp_output_10_0_g98 ) , 0.0 );
				float temp_output_7_0_g79 = ( tex2DArrayNode27_g76.b + tex2DNode11_g76.a );
				float temp_output_8_0_g79 = ( tex2DArrayNode24_g76.b + temp_output_30_0_g76 );
				float temp_output_10_0_g79 = ( max( temp_output_7_0_g79 , temp_output_8_0_g79 ) - 0.2 );
				float temp_output_13_0_g79 = max( ( temp_output_7_0_g79 - temp_output_10_0_g79 ) , 0.0 );
				float4 tex2DArrayNode21_g76 = SAMPLE_TEXTURE2D_ARRAY( _MaterialArray, sampler_MaterialArray, uv_MaterialArray,( tex2DNode8_g76.r * 255.0 ) );
				float temp_output_14_0_g79 = max( ( temp_output_8_0_g79 - temp_output_10_0_g79 ) , 0.0 );
				float temp_output_14_0_g98 = max( ( temp_output_8_0_g98 - temp_output_10_0_g98 ) , 0.0 );
				float temp_output_14_0_g100 = max( ( temp_output_8_0_g100 - temp_output_10_0_g100 ) , 0.0 );
				float4 temp_output_62_0 = ( ( ( ( ( ( ( ( ( SAMPLE_TEXTURE2D_ARRAY( _MaterialArray, sampler_MaterialArray, uv_MaterialArray,( tex2DNode10_g80.r * 255.0 ) ) * temp_output_13_0_g83 ) + ( tex2DArrayNode21_g80 * temp_output_14_0_g83 ) ) / ( temp_output_13_0_g83 + temp_output_14_0_g83 ) ) * temp_output_13_0_g97 ) + ( ( ( ( SAMPLE_TEXTURE2D_ARRAY( _MaterialArray, sampler_MaterialArray, uv_MaterialArray,( tex2DNode10_g84.r * 255.0 ) ) * temp_output_13_0_g87 ) + ( tex2DArrayNode21_g84 * temp_output_14_0_g87 ) ) / ( temp_output_13_0_g87 + temp_output_14_0_g87 ) ) * temp_output_14_0_g97 ) ) / ( temp_output_13_0_g97 + temp_output_14_0_g97 ) ) * temp_output_13_0_g100 ) + ( ( ( ( ( ( ( SAMPLE_TEXTURE2D_ARRAY( _MaterialArray, sampler_MaterialArray, uv_MaterialArray,( tex2DNode10_g88.r * 255.0 ) ) * temp_output_13_0_g91 ) + ( tex2DArrayNode21_g88 * temp_output_14_0_g91 ) ) / ( temp_output_13_0_g91 + temp_output_14_0_g91 ) ) * temp_output_13_0_g98 ) + ( ( ( ( SAMPLE_TEXTURE2D_ARRAY( _MaterialArray, sampler_MaterialArray, uv_MaterialArray,( tex2DNode10_g76.r * 255.0 ) ) * temp_output_13_0_g79 ) + ( tex2DArrayNode21_g76 * temp_output_14_0_g79 ) ) / ( temp_output_13_0_g79 + temp_output_14_0_g79 ) ) * temp_output_14_0_g98 ) ) / ( temp_output_13_0_g98 + temp_output_14_0_g98 ) ) * temp_output_14_0_g100 ) ) / ( temp_output_13_0_g100 + temp_output_14_0_g100 ) );
				float temp_output_7_0_g81 = ( tex2DArrayNode27_g80.b + tex2DNode11_g80.a );
				float temp_output_8_0_g81 = ( tex2DArrayNode24_g80.b + temp_output_30_0_g80 );
				float temp_output_10_0_g81 = ( max( temp_output_7_0_g81 , temp_output_8_0_g81 ) - 0.2 );
				float temp_output_13_0_g81 = max( ( temp_output_7_0_g81 - temp_output_10_0_g81 ) , 0.0 );
				float4 tex2DNode9_g80 = tex2D( _Tint, temp_output_6_0_g80 );
				float temp_output_14_0_g81 = max( ( temp_output_8_0_g81 - temp_output_10_0_g81 ) , 0.0 );
				float temp_output_7_0_g95 = ( temp_output_77_42 + temp_output_49_0 );
				float temp_output_8_0_g95 = ( temp_output_79_42 + break41.x );
				float temp_output_10_0_g95 = ( max( temp_output_7_0_g95 , temp_output_8_0_g95 ) - 0.2 );
				float temp_output_13_0_g95 = max( ( temp_output_7_0_g95 - temp_output_10_0_g95 ) , 0.0 );
				float temp_output_7_0_g85 = ( tex2DArrayNode27_g84.b + tex2DNode11_g84.a );
				float temp_output_8_0_g85 = ( tex2DArrayNode24_g84.b + temp_output_30_0_g84 );
				float temp_output_10_0_g85 = ( max( temp_output_7_0_g85 , temp_output_8_0_g85 ) - 0.2 );
				float temp_output_13_0_g85 = max( ( temp_output_7_0_g85 - temp_output_10_0_g85 ) , 0.0 );
				float4 tex2DNode9_g84 = tex2D( _Tint, temp_output_6_0_g84 );
				float temp_output_14_0_g85 = max( ( temp_output_8_0_g85 - temp_output_10_0_g85 ) , 0.0 );
				float temp_output_14_0_g95 = max( ( temp_output_8_0_g95 - temp_output_10_0_g95 ) , 0.0 );
				float temp_output_7_0_g99 = ( temp_output_44_0.z + temp_output_61_0 );
				float temp_output_8_0_g99 = ( temp_output_47_0.z + break41.y );
				float temp_output_10_0_g99 = ( max( temp_output_7_0_g99 , temp_output_8_0_g99 ) - 0.2 );
				float temp_output_13_0_g99 = max( ( temp_output_7_0_g99 - temp_output_10_0_g99 ) , 0.0 );
				float temp_output_7_0_g89 = ( tex2DArrayNode27_g88.b + tex2DNode11_g88.a );
				float temp_output_8_0_g89 = ( tex2DArrayNode24_g88.b + temp_output_30_0_g88 );
				float temp_output_10_0_g89 = ( max( temp_output_7_0_g89 , temp_output_8_0_g89 ) - 0.2 );
				float temp_output_13_0_g89 = max( ( temp_output_7_0_g89 - temp_output_10_0_g89 ) , 0.0 );
				float4 tex2DNode9_g88 = tex2D( _Tint, temp_output_6_0_g88 );
				float temp_output_14_0_g89 = max( ( temp_output_8_0_g89 - temp_output_10_0_g89 ) , 0.0 );
				float temp_output_7_0_g96 = ( temp_output_80_42 + temp_output_49_0 );
				float temp_output_8_0_g96 = ( temp_output_78_42 + break41.x );
				float temp_output_10_0_g96 = ( max( temp_output_7_0_g96 , temp_output_8_0_g96 ) - 0.2 );
				float temp_output_13_0_g96 = max( ( temp_output_7_0_g96 - temp_output_10_0_g96 ) , 0.0 );
				float temp_output_7_0_g77 = ( tex2DArrayNode27_g76.b + tex2DNode11_g76.a );
				float temp_output_8_0_g77 = ( tex2DArrayNode24_g76.b + temp_output_30_0_g76 );
				float temp_output_10_0_g77 = ( max( temp_output_7_0_g77 , temp_output_8_0_g77 ) - 0.2 );
				float temp_output_13_0_g77 = max( ( temp_output_7_0_g77 - temp_output_10_0_g77 ) , 0.0 );
				float4 tex2DNode9_g76 = tex2D( _Tint, temp_output_6_0_g76 );
				float temp_output_14_0_g77 = max( ( temp_output_8_0_g77 - temp_output_10_0_g77 ) , 0.0 );
				float temp_output_14_0_g96 = max( ( temp_output_8_0_g96 - temp_output_10_0_g96 ) , 0.0 );
				float temp_output_14_0_g99 = max( ( temp_output_8_0_g99 - temp_output_10_0_g99 ) , 0.0 );
				float4 temp_output_64_0 = ( ( ( ( ( ( ( ( ( tex2DNode11_g80 * temp_output_13_0_g81 ) + ( tex2DNode9_g80 * temp_output_14_0_g81 ) ) / ( temp_output_13_0_g81 + temp_output_14_0_g81 ) ) * temp_output_13_0_g95 ) + ( ( ( ( tex2DNode11_g84 * temp_output_13_0_g85 ) + ( tex2DNode9_g84 * temp_output_14_0_g85 ) ) / ( temp_output_13_0_g85 + temp_output_14_0_g85 ) ) * temp_output_14_0_g95 ) ) / ( temp_output_13_0_g95 + temp_output_14_0_g95 ) ) * temp_output_13_0_g99 ) + ( ( ( ( ( ( ( tex2DNode11_g88 * temp_output_13_0_g89 ) + ( tex2DNode9_g88 * temp_output_14_0_g89 ) ) / ( temp_output_13_0_g89 + temp_output_14_0_g89 ) ) * temp_output_13_0_g96 ) + ( ( ( ( tex2DNode11_g76 * temp_output_13_0_g77 ) + ( tex2DNode9_g76 * temp_output_14_0_g77 ) ) / ( temp_output_13_0_g77 + temp_output_14_0_g77 ) ) * temp_output_14_0_g96 ) ) / ( temp_output_13_0_g96 + temp_output_14_0_g96 ) ) * temp_output_14_0_g99 ) ) / ( temp_output_13_0_g99 + temp_output_14_0_g99 ) );
				float4 ifLocalVar157_g104 = 0;
				if( temp_output_147_0_g104 > 0.0 )
				ifLocalVar157_g104 = ( tex2DNode133_g104 / tex2DNode133_g104.a );
				else if( temp_output_147_0_g104 < 0.0 )
				ifLocalVar157_g104 = ( temp_output_62_0 * temp_output_64_0 );
				
				float temp_output_7_0_g103 = ( temp_output_44_0.z + temp_output_61_0 );
				float temp_output_8_0_g103 = ( temp_output_47_0.z + break41.y );
				float temp_output_10_0_g103 = ( max( temp_output_7_0_g103 , temp_output_8_0_g103 ) - 0.2 );
				float temp_output_13_0_g103 = max( ( temp_output_7_0_g103 - temp_output_10_0_g103 ) , 0.0 );
				float temp_output_14_0_g103 = max( ( temp_output_8_0_g103 - temp_output_10_0_g103 ) , 0.0 );
				float4 break65 = ( ( ( temp_output_44_0 * temp_output_13_0_g103 ) + ( temp_output_47_0 * temp_output_14_0_g103 ) ) / ( temp_output_13_0_g103 + temp_output_14_0_g103 ) );
				float4 appendResult71 = (float4(0.0 , break65.y , break65.y , break65.w));
				
				float temp_output_3_0_g102 = ( temp_output_64_0.w * 2.0 );
				float ifLocalVar159_g104 = 0;
				if( temp_output_147_0_g104 < 0.0 )
				ifLocalVar159_g104 = max( ( temp_output_3_0_g102 - 1.0 ) , 0.0 );
				
				float ifLocalVar158_g104 = 0;
				UNITY_BRANCH 
				if( temp_output_147_0_g104 < 0.5 )
				ifLocalVar158_g104 = temp_output_62_0.w;
				
				float ifLocalVar169_g104 = 0;
				if( temp_output_147_0_g104 < 1.0 )
				ifLocalVar169_g104 = min( temp_output_3_0_g102 , 1.0 );
				
				surfaceDescription.Albedo = ifLocalVar157_g104.rgb;
				surfaceDescription.Normal = UnpackNormalScale( appendResult71, 1.0 );
				surfaceDescription.BentNormal = float3( 0, 0, 1 );
				surfaceDescription.CoatMask = 0;
				surfaceDescription.Metallic = ifLocalVar159_g104;

				#ifdef _MATERIAL_FEATURE_SPECULAR_COLOR
				surfaceDescription.Specular = 0;
				#endif

				surfaceDescription.Emission = 0;
				surfaceDescription.Smoothness = ifLocalVar158_g104;
				surfaceDescription.Occlusion = 1;
				surfaceDescription.Alpha = ifLocalVar169_g104;

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
        
			#define _BLENDMODE_PRESERVE_SPECULAR_LIGHTING 1
			#define _DISABLE_SSR_TRANSPARENT 1
			#define _SPECULAR_OCCLUSION_FROM_AO 1
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
			float4 _MaterialArray_ST;
			float4 _Control_TexelSize;
			float4 _ShapeArray_ST;
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
			sampler2D _Noise;
			sampler2D _GrassTint;
			sampler2D _Control;
			TEXTURE2D_ARRAY(_ShapeArray);
			sampler2D _GrassControl;
			SAMPLER(sampler_ShapeArray);
			sampler2D _Tint;

        
            
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/PickingSpaceTransforms.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Material.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Unlit/Unlit.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/BuiltinUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/MaterialUtilities.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderGraphFunctions.hlsl"
        

			

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
				float dotResult145_g104 = dot( packedInput.normalWS , float3( 0,1,0 ) );
				float3 ase_worldPos = packedInput.ase_texcoord2.xyz;
				float3 break180_g104 = ase_worldPos;
				float2 appendResult102_g104 = (float2(break180_g104.x , break180_g104.z));
				float4 break174_g104 = _WorldBounds;
				float2 appendResult129_g104 = (float2(break174_g104.x , break174_g104.y));
				float2 appendResult130_g104 = (float2(break174_g104.z , break174_g104.w));
				float4 tex2DNode133_g104 = tex2D( _SpatterTex, ( ( appendResult102_g104 - appendResult129_g104 ) / ( appendResult130_g104 - appendResult129_g104 ) ) );
				float lerpResult139_g104 = lerp( 1.0 , -1.0 , ( tex2DNode133_g104.a - tex2D( _Noise, appendResult102_g104 ).r ));
				float temp_output_147_0_g104 = ( dotResult145_g104 >= lerpResult139_g104 ? 1.0 : 0.0 );
				float3 break17 = ase_worldPos;
				float2 appendResult26 = (float2(break17.x , break17.z));
				float4 break18 = _WorldBounds;
				float2 appendResult27 = (float2(break18.x , break18.y));
				float2 appendResult28 = (float2(break18.w , break18.z));
				float2 appendResult35 = (float2(_Control_TexelSize.z , _Control_TexelSize.w));
				float2 temp_output_36_0 = ( ( ( appendResult26 - appendResult27 ) / ( appendResult28 - appendResult27 ) ) * appendResult35 );
				float2 temp_output_39_0 = ( floor( ( temp_output_36_0 + float2( 0.5,0.5 ) ) ) - float2( 0.5,0.5 ) );
				float2 temp_output_6_0_g80 = ( ( temp_output_39_0 + float2( 0,0 ) ) / appendResult35 );
				float4 tex2DNode11_g80 = tex2D( _GrassTint, temp_output_6_0_g80 );
				float2 uv_ShapeArray = packedInput.ase_texcoord3.xy;
				float4 tex2DNode10_g80 = tex2D( _GrassControl, temp_output_6_0_g80 );
				float4 tex2DArrayNode27_g80 = SAMPLE_TEXTURE2D_ARRAY( _ShapeArray, sampler_ShapeArray, uv_ShapeArray,( tex2DNode10_g80.g * 255.0 ) );
				float temp_output_7_0_g81 = ( tex2DArrayNode27_g80.b + tex2DNode11_g80.a );
				float4 tex2DNode8_g80 = tex2D( _Control, temp_output_6_0_g80 );
				float4 tex2DArrayNode24_g80 = SAMPLE_TEXTURE2D_ARRAY( _ShapeArray, sampler_ShapeArray, uv_ShapeArray,( tex2DNode8_g80.g * 255.0 ) );
				float temp_output_30_0_g80 = ( 1.0 - tex2DNode11_g80.a );
				float temp_output_8_0_g81 = ( tex2DArrayNode24_g80.b + temp_output_30_0_g80 );
				float temp_output_10_0_g81 = ( max( temp_output_7_0_g81 , temp_output_8_0_g81 ) - 0.2 );
				float temp_output_13_0_g81 = max( ( temp_output_7_0_g81 - temp_output_10_0_g81 ) , 0.0 );
				float4 tex2DNode9_g80 = tex2D( _Tint, temp_output_6_0_g80 );
				float temp_output_14_0_g81 = max( ( temp_output_8_0_g81 - temp_output_10_0_g81 ) , 0.0 );
				float temp_output_7_0_g82 = ( tex2DArrayNode27_g80.b + tex2DNode11_g80.a );
				float temp_output_8_0_g82 = ( tex2DArrayNode24_g80.b + temp_output_30_0_g80 );
				float temp_output_10_0_g82 = ( max( temp_output_7_0_g82 , temp_output_8_0_g82 ) - 0.2 );
				float temp_output_13_0_g82 = max( ( temp_output_7_0_g82 - temp_output_10_0_g82 ) , 0.0 );
				float temp_output_14_0_g82 = max( ( temp_output_8_0_g82 - temp_output_10_0_g82 ) , 0.0 );
				float temp_output_77_42 = ( ( ( tex2DArrayNode27_g80 * temp_output_13_0_g82 ) + ( tex2DArrayNode24_g80 * temp_output_14_0_g82 ) ) / ( temp_output_13_0_g82 + temp_output_14_0_g82 ) ).z;
				float2 break41 = ( temp_output_36_0 - temp_output_39_0 );
				float temp_output_49_0 = ( 1.0 - break41.x );
				float temp_output_7_0_g95 = ( temp_output_77_42 + temp_output_49_0 );
				float2 temp_output_6_0_g84 = ( ( temp_output_39_0 + float2( 0,0 ) ) / appendResult35 );
				float4 tex2DNode10_g84 = tex2D( _GrassControl, temp_output_6_0_g84 );
				float4 tex2DArrayNode27_g84 = SAMPLE_TEXTURE2D_ARRAY( _ShapeArray, sampler_ShapeArray, uv_ShapeArray,( tex2DNode10_g84.g * 255.0 ) );
				float4 tex2DNode11_g84 = tex2D( _GrassTint, temp_output_6_0_g84 );
				float temp_output_7_0_g86 = ( tex2DArrayNode27_g84.b + tex2DNode11_g84.a );
				float4 tex2DNode8_g84 = tex2D( _Control, temp_output_6_0_g84 );
				float4 tex2DArrayNode24_g84 = SAMPLE_TEXTURE2D_ARRAY( _ShapeArray, sampler_ShapeArray, uv_ShapeArray,( tex2DNode8_g84.g * 255.0 ) );
				float temp_output_30_0_g84 = ( 1.0 - tex2DNode11_g84.a );
				float temp_output_8_0_g86 = ( tex2DArrayNode24_g84.b + temp_output_30_0_g84 );
				float temp_output_10_0_g86 = ( max( temp_output_7_0_g86 , temp_output_8_0_g86 ) - 0.2 );
				float temp_output_13_0_g86 = max( ( temp_output_7_0_g86 - temp_output_10_0_g86 ) , 0.0 );
				float temp_output_14_0_g86 = max( ( temp_output_8_0_g86 - temp_output_10_0_g86 ) , 0.0 );
				float temp_output_79_42 = ( ( ( tex2DArrayNode27_g84 * temp_output_13_0_g86 ) + ( tex2DArrayNode24_g84 * temp_output_14_0_g86 ) ) / ( temp_output_13_0_g86 + temp_output_14_0_g86 ) ).z;
				float temp_output_8_0_g95 = ( temp_output_79_42 + break41.x );
				float temp_output_10_0_g95 = ( max( temp_output_7_0_g95 , temp_output_8_0_g95 ) - 0.2 );
				float temp_output_13_0_g95 = max( ( temp_output_7_0_g95 - temp_output_10_0_g95 ) , 0.0 );
				float temp_output_7_0_g85 = ( tex2DArrayNode27_g84.b + tex2DNode11_g84.a );
				float temp_output_8_0_g85 = ( tex2DArrayNode24_g84.b + temp_output_30_0_g84 );
				float temp_output_10_0_g85 = ( max( temp_output_7_0_g85 , temp_output_8_0_g85 ) - 0.2 );
				float temp_output_13_0_g85 = max( ( temp_output_7_0_g85 - temp_output_10_0_g85 ) , 0.0 );
				float4 tex2DNode9_g84 = tex2D( _Tint, temp_output_6_0_g84 );
				float temp_output_14_0_g85 = max( ( temp_output_8_0_g85 - temp_output_10_0_g85 ) , 0.0 );
				float temp_output_14_0_g95 = max( ( temp_output_8_0_g95 - temp_output_10_0_g95 ) , 0.0 );
				float temp_output_7_0_g93 = ( temp_output_77_42 + temp_output_49_0 );
				float temp_output_8_0_g93 = ( temp_output_79_42 + break41.x );
				float temp_output_10_0_g93 = ( max( temp_output_7_0_g93 , temp_output_8_0_g93 ) - 0.2 );
				float temp_output_13_0_g93 = max( ( temp_output_7_0_g93 - temp_output_10_0_g93 ) , 0.0 );
				float temp_output_14_0_g93 = max( ( temp_output_8_0_g93 - temp_output_10_0_g93 ) , 0.0 );
				float4 temp_output_44_0 = ( ( ( ( ( ( tex2DArrayNode27_g80 * temp_output_13_0_g82 ) + ( tex2DArrayNode24_g80 * temp_output_14_0_g82 ) ) / ( temp_output_13_0_g82 + temp_output_14_0_g82 ) ) * temp_output_13_0_g93 ) + ( ( ( ( tex2DArrayNode27_g84 * temp_output_13_0_g86 ) + ( tex2DArrayNode24_g84 * temp_output_14_0_g86 ) ) / ( temp_output_13_0_g86 + temp_output_14_0_g86 ) ) * temp_output_14_0_g93 ) ) / ( temp_output_13_0_g93 + temp_output_14_0_g93 ) );
				float temp_output_61_0 = ( 1.0 - break41.y );
				float temp_output_7_0_g99 = ( temp_output_44_0.z + temp_output_61_0 );
				float2 temp_output_6_0_g88 = ( ( temp_output_39_0 + float2( 0,0 ) ) / appendResult35 );
				float4 tex2DNode10_g88 = tex2D( _GrassControl, temp_output_6_0_g88 );
				float4 tex2DArrayNode27_g88 = SAMPLE_TEXTURE2D_ARRAY( _ShapeArray, sampler_ShapeArray, uv_ShapeArray,( tex2DNode10_g88.g * 255.0 ) );
				float4 tex2DNode11_g88 = tex2D( _GrassTint, temp_output_6_0_g88 );
				float temp_output_7_0_g90 = ( tex2DArrayNode27_g88.b + tex2DNode11_g88.a );
				float4 tex2DNode8_g88 = tex2D( _Control, temp_output_6_0_g88 );
				float4 tex2DArrayNode24_g88 = SAMPLE_TEXTURE2D_ARRAY( _ShapeArray, sampler_ShapeArray, uv_ShapeArray,( tex2DNode8_g88.g * 255.0 ) );
				float temp_output_30_0_g88 = ( 1.0 - tex2DNode11_g88.a );
				float temp_output_8_0_g90 = ( tex2DArrayNode24_g88.b + temp_output_30_0_g88 );
				float temp_output_10_0_g90 = ( max( temp_output_7_0_g90 , temp_output_8_0_g90 ) - 0.2 );
				float temp_output_13_0_g90 = max( ( temp_output_7_0_g90 - temp_output_10_0_g90 ) , 0.0 );
				float temp_output_14_0_g90 = max( ( temp_output_8_0_g90 - temp_output_10_0_g90 ) , 0.0 );
				float temp_output_80_42 = ( ( ( tex2DArrayNode27_g88 * temp_output_13_0_g90 ) + ( tex2DArrayNode24_g88 * temp_output_14_0_g90 ) ) / ( temp_output_13_0_g90 + temp_output_14_0_g90 ) ).z;
				float temp_output_7_0_g94 = ( temp_output_80_42 + temp_output_49_0 );
				float2 temp_output_6_0_g76 = ( ( temp_output_39_0 + float2( 0,0 ) ) / appendResult35 );
				float4 tex2DNode10_g76 = tex2D( _GrassControl, temp_output_6_0_g76 );
				float4 tex2DArrayNode27_g76 = SAMPLE_TEXTURE2D_ARRAY( _ShapeArray, sampler_ShapeArray, uv_ShapeArray,( tex2DNode10_g76.g * 255.0 ) );
				float4 tex2DNode11_g76 = tex2D( _GrassTint, temp_output_6_0_g76 );
				float temp_output_7_0_g78 = ( tex2DArrayNode27_g76.b + tex2DNode11_g76.a );
				float4 tex2DNode8_g76 = tex2D( _Control, temp_output_6_0_g76 );
				float4 tex2DArrayNode24_g76 = SAMPLE_TEXTURE2D_ARRAY( _ShapeArray, sampler_ShapeArray, uv_ShapeArray,( tex2DNode8_g76.g * 255.0 ) );
				float temp_output_30_0_g76 = ( 1.0 - tex2DNode11_g76.a );
				float temp_output_8_0_g78 = ( tex2DArrayNode24_g76.b + temp_output_30_0_g76 );
				float temp_output_10_0_g78 = ( max( temp_output_7_0_g78 , temp_output_8_0_g78 ) - 0.2 );
				float temp_output_13_0_g78 = max( ( temp_output_7_0_g78 - temp_output_10_0_g78 ) , 0.0 );
				float temp_output_14_0_g78 = max( ( temp_output_8_0_g78 - temp_output_10_0_g78 ) , 0.0 );
				float temp_output_78_42 = ( ( ( tex2DArrayNode27_g76 * temp_output_13_0_g78 ) + ( tex2DArrayNode24_g76 * temp_output_14_0_g78 ) ) / ( temp_output_13_0_g78 + temp_output_14_0_g78 ) ).z;
				float temp_output_8_0_g94 = ( temp_output_78_42 + break41.x );
				float temp_output_10_0_g94 = ( max( temp_output_7_0_g94 , temp_output_8_0_g94 ) - 0.2 );
				float temp_output_13_0_g94 = max( ( temp_output_7_0_g94 - temp_output_10_0_g94 ) , 0.0 );
				float temp_output_14_0_g94 = max( ( temp_output_8_0_g94 - temp_output_10_0_g94 ) , 0.0 );
				float4 temp_output_47_0 = ( ( ( ( ( ( tex2DArrayNode27_g88 * temp_output_13_0_g90 ) + ( tex2DArrayNode24_g88 * temp_output_14_0_g90 ) ) / ( temp_output_13_0_g90 + temp_output_14_0_g90 ) ) * temp_output_13_0_g94 ) + ( ( ( ( tex2DArrayNode27_g76 * temp_output_13_0_g78 ) + ( tex2DArrayNode24_g76 * temp_output_14_0_g78 ) ) / ( temp_output_13_0_g78 + temp_output_14_0_g78 ) ) * temp_output_14_0_g94 ) ) / ( temp_output_13_0_g94 + temp_output_14_0_g94 ) );
				float temp_output_8_0_g99 = ( temp_output_47_0.z + break41.y );
				float temp_output_10_0_g99 = ( max( temp_output_7_0_g99 , temp_output_8_0_g99 ) - 0.2 );
				float temp_output_13_0_g99 = max( ( temp_output_7_0_g99 - temp_output_10_0_g99 ) , 0.0 );
				float temp_output_7_0_g89 = ( tex2DArrayNode27_g88.b + tex2DNode11_g88.a );
				float temp_output_8_0_g89 = ( tex2DArrayNode24_g88.b + temp_output_30_0_g88 );
				float temp_output_10_0_g89 = ( max( temp_output_7_0_g89 , temp_output_8_0_g89 ) - 0.2 );
				float temp_output_13_0_g89 = max( ( temp_output_7_0_g89 - temp_output_10_0_g89 ) , 0.0 );
				float4 tex2DNode9_g88 = tex2D( _Tint, temp_output_6_0_g88 );
				float temp_output_14_0_g89 = max( ( temp_output_8_0_g89 - temp_output_10_0_g89 ) , 0.0 );
				float temp_output_7_0_g96 = ( temp_output_80_42 + temp_output_49_0 );
				float temp_output_8_0_g96 = ( temp_output_78_42 + break41.x );
				float temp_output_10_0_g96 = ( max( temp_output_7_0_g96 , temp_output_8_0_g96 ) - 0.2 );
				float temp_output_13_0_g96 = max( ( temp_output_7_0_g96 - temp_output_10_0_g96 ) , 0.0 );
				float temp_output_7_0_g77 = ( tex2DArrayNode27_g76.b + tex2DNode11_g76.a );
				float temp_output_8_0_g77 = ( tex2DArrayNode24_g76.b + temp_output_30_0_g76 );
				float temp_output_10_0_g77 = ( max( temp_output_7_0_g77 , temp_output_8_0_g77 ) - 0.2 );
				float temp_output_13_0_g77 = max( ( temp_output_7_0_g77 - temp_output_10_0_g77 ) , 0.0 );
				float4 tex2DNode9_g76 = tex2D( _Tint, temp_output_6_0_g76 );
				float temp_output_14_0_g77 = max( ( temp_output_8_0_g77 - temp_output_10_0_g77 ) , 0.0 );
				float temp_output_14_0_g96 = max( ( temp_output_8_0_g96 - temp_output_10_0_g96 ) , 0.0 );
				float temp_output_14_0_g99 = max( ( temp_output_8_0_g99 - temp_output_10_0_g99 ) , 0.0 );
				float4 temp_output_64_0 = ( ( ( ( ( ( ( ( ( tex2DNode11_g80 * temp_output_13_0_g81 ) + ( tex2DNode9_g80 * temp_output_14_0_g81 ) ) / ( temp_output_13_0_g81 + temp_output_14_0_g81 ) ) * temp_output_13_0_g95 ) + ( ( ( ( tex2DNode11_g84 * temp_output_13_0_g85 ) + ( tex2DNode9_g84 * temp_output_14_0_g85 ) ) / ( temp_output_13_0_g85 + temp_output_14_0_g85 ) ) * temp_output_14_0_g95 ) ) / ( temp_output_13_0_g95 + temp_output_14_0_g95 ) ) * temp_output_13_0_g99 ) + ( ( ( ( ( ( ( tex2DNode11_g88 * temp_output_13_0_g89 ) + ( tex2DNode9_g88 * temp_output_14_0_g89 ) ) / ( temp_output_13_0_g89 + temp_output_14_0_g89 ) ) * temp_output_13_0_g96 ) + ( ( ( ( tex2DNode11_g76 * temp_output_13_0_g77 ) + ( tex2DNode9_g76 * temp_output_14_0_g77 ) ) / ( temp_output_13_0_g77 + temp_output_14_0_g77 ) ) * temp_output_14_0_g96 ) ) / ( temp_output_13_0_g96 + temp_output_14_0_g96 ) ) * temp_output_14_0_g99 ) ) / ( temp_output_13_0_g99 + temp_output_14_0_g99 ) );
				float temp_output_3_0_g102 = ( temp_output_64_0.w * 2.0 );
				float ifLocalVar169_g104 = 0;
				if( temp_output_147_0_g104 < 1.0 )
				ifLocalVar169_g104 = min( temp_output_3_0_g102 , 1.0 );
				
				surfaceDescription.Alpha = ifLocalVar169_g104;
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
738;175;2560;1329;-1079.637;935.7509;1;True;True
Node;AmplifyShaderEditor.Vector4Node;16;-1326.163,-180.3144;Inherit;False;Property;_WorldBounds;World Bounds;0;0;Create;True;0;0;0;True;0;False;0,0,1,1;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldPosInputsNode;15;-1329.271,-397.7642;Inherit;True;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.BreakToComponentsNode;18;-1078.439,-217.4756;Inherit;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.BreakToComponentsNode;17;-1076.288,-339.7155;Inherit;False;FLOAT3;1;0;FLOAT3;0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.DynamicAppendNode;26;-925.8248,-328.5683;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;33;-1040.178,57.06238;Inherit;True;Property;_Control;Control;3;0;Create;True;0;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.DynamicAppendNode;28;-924.6082,-138.7936;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;27;-924.6081,-237.3304;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexelSizeNode;34;-822.422,57.06236;Inherit;False;-1;1;0;SAMPLER2D;;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;30;-744.566,-192.3199;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;29;-749.4317,-303.0216;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;35;-633.8641,98.42353;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;31;-592.503,-267.7431;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;36;-397.8634,-115.6812;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;37;-247.0155,-119.33;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FloorOpNode;38;-125.3649,-119.33;Inherit;False;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;39;-6.53832,-116.4581;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;40;142.4769,-120.5242;Inherit;True;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;52;-513.2698,446.4729;Inherit;True;Property;_Tint;Tint;5;0;Create;True;0;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.TexturePropertyNode;53;-513.1725,263.0598;Inherit;True;Property;_GrassTint;Grass Tint;6;0;Create;True;0;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.TexturePropertyNode;51;-509.2027,631.9224;Inherit;True;Property;_GrassControl;Grass Control;4;0;Create;True;0;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.BreakToComponentsNode;41;347.9775,-110.5106;Inherit;False;FLOAT2;1;0;FLOAT2;0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.TexturePropertyNode;54;-507.9515,818.5945;Inherit;True;Property;_ShapeArray;Shape Array;7;0;Create;True;0;0;0;False;0;False;None;None;False;white;LockedToTexture2DArray;Texture2DArray;-1;0;2;SAMPLER2DARRAY;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.TexturePropertyNode;55;-506.82,1010.949;Inherit;True;Property;_MaterialArray;Material Array;8;0;Create;True;0;0;0;False;0;False;None;None;False;white;LockedToTexture2DArray;Texture2DArray;-1;0;2;SAMPLER2DARRAY;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.FunctionNode;78;662.8171,389.8142;Inherit;False;Sample Terrain Tile;-1;;76;c342fffb3b8f6cb438364b74e632a480;3,36,0,39,0,38,0;9;49;SAMPLER2D;0;False;50;SAMPLER2D;0;False;51;SAMPLER2D;0;False;52;SAMPLER2D;0;False;47;SAMPLER2DARRAY;0;False;48;SAMPLER2DARRAY;0;False;44;FLOAT2;0,0;False;45;FLOAT2;0,0;False;46;FLOAT2;4,4;False;4;FLOAT4;0;FLOAT4;40;FLOAT4;41;FLOAT;42
Node;AmplifyShaderEditor.FunctionNode;77;129.9295,-546.2559;Inherit;False;Sample Terrain Tile;-1;;80;c342fffb3b8f6cb438364b74e632a480;3,36,0,39,0,38,0;9;49;SAMPLER2D;0;False;50;SAMPLER2D;0;False;51;SAMPLER2D;0;False;52;SAMPLER2D;0;False;47;SAMPLER2DARRAY;0;False;48;SAMPLER2DARRAY;0;False;44;FLOAT2;0,0;False;45;FLOAT2;0,0;False;46;FLOAT2;4,4;False;4;FLOAT4;0;FLOAT4;40;FLOAT4;41;FLOAT;42
Node;AmplifyShaderEditor.FunctionNode;79;651.4239,-1149.736;Inherit;False;Sample Terrain Tile;-1;;84;c342fffb3b8f6cb438364b74e632a480;3,36,0,39,0,38,0;9;49;SAMPLER2D;0;False;50;SAMPLER2D;0;False;51;SAMPLER2D;0;False;52;SAMPLER2D;0;False;47;SAMPLER2DARRAY;0;False;48;SAMPLER2DARRAY;0;False;44;FLOAT2;0,0;False;45;FLOAT2;0,0;False;46;FLOAT2;4,4;False;4;FLOAT4;0;FLOAT4;40;FLOAT4;41;FLOAT;42
Node;AmplifyShaderEditor.FunctionNode;80;138.6501,135.4975;Inherit;False;Sample Terrain Tile;-1;;88;c342fffb3b8f6cb438364b74e632a480;3,36,0,39,0,38,0;9;49;SAMPLER2D;0;False;50;SAMPLER2D;0;False;51;SAMPLER2D;0;False;52;SAMPLER2D;0;False;47;SAMPLER2DARRAY;0;False;48;SAMPLER2DARRAY;0;False;44;FLOAT2;0,0;False;45;FLOAT2;0,0;False;46;FLOAT2;4,4;False;4;FLOAT4;0;FLOAT4;40;FLOAT4;41;FLOAT;42
Node;AmplifyShaderEditor.OneMinusNode;49;359.8262,-6.526394;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;44;639.8651,-659.0211;Inherit;False;Height Based Mix;-1;;93;9ef83f4fd28e7eb4aaa0d4ca3f9da236;0;6;24;FLOAT;0;False;25;FLOAT;0;False;22;FLOAT;0;False;23;FLOAT;0;False;20;FLOAT4;0,0,0,0;False;21;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.FunctionNode;47;635.7642,-91.40449;Inherit;False;Height Based Mix;-1;;94;9ef83f4fd28e7eb4aaa0d4ca3f9da236;0;6;24;FLOAT;0;False;25;FLOAT;0;False;22;FLOAT;0;False;23;FLOAT;0;False;20;FLOAT4;0,0,0,0;False;21;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.FunctionNode;45;635.7672,-469.1409;Inherit;False;Height Based Mix;-1;;95;9ef83f4fd28e7eb4aaa0d4ca3f9da236;0;6;24;FLOAT;0;False;25;FLOAT;0;False;22;FLOAT;0;False;23;FLOAT;0;False;20;FLOAT4;0,0,0,0;False;21;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.FunctionNode;48;631.6663,98.47571;Inherit;False;Height Based Mix;-1;;96;9ef83f4fd28e7eb4aaa0d4ca3f9da236;0;6;24;FLOAT;0;False;25;FLOAT;0;False;22;FLOAT;0;False;23;FLOAT;0;False;20;FLOAT4;0,0,0,0;False;21;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.OneMinusNode;61;1123.487,-17.91464;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;60;944.0956,-80.95349;Inherit;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.BreakToComponentsNode;57;944.4772,-648.9832;Inherit;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.FunctionNode;46;639.8721,-279.31;Inherit;False;Height Based Mix;-1;;98;9ef83f4fd28e7eb4aaa0d4ca3f9da236;0;6;24;FLOAT;0;False;25;FLOAT;0;False;22;FLOAT;0;False;23;FLOAT;0;False;20;FLOAT4;0,0,0,0;False;21;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.FunctionNode;64;1435.181,-68.84736;Inherit;False;Height Based Mix;-1;;99;9ef83f4fd28e7eb4aaa0d4ca3f9da236;0;6;24;FLOAT;0;False;25;FLOAT;0;False;22;FLOAT;0;False;23;FLOAT;0;False;20;FLOAT4;0,0,0,0;False;21;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.FunctionNode;43;647.3912,-877.472;Inherit;False;Height Based Mix;-1;;97;9ef83f4fd28e7eb4aaa0d4ca3f9da236;0;6;24;FLOAT;0;False;25;FLOAT;0;False;22;FLOAT;0;False;23;FLOAT;0;False;20;FLOAT4;0,0,0,0;False;21;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.FunctionNode;62;1441.357,-457.9277;Inherit;False;Height Based Mix;-1;;100;9ef83f4fd28e7eb4aaa0d4ca3f9da236;0;6;24;FLOAT;0;False;25;FLOAT;0;False;22;FLOAT;0;False;23;FLOAT;0;False;20;FLOAT4;0,0,0,0;False;21;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.BreakToComponentsNode;66;1782.573,-8.631248;Inherit;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;67;1810.367,-479.5421;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.FunctionNode;81;2013.146,-63.40363;Inherit;False;Alpha Split;-1;;102;15cd6f3f954a80543b41ed67ca95494c;0;1;2;FLOAT;0;False;2;FLOAT;0;FLOAT;1
Node;AmplifyShaderEditor.TexturePropertyNode;76;1873.914,-770.4827;Inherit;True;Property;_SpatterTex;Spatter Texture;9;0;Create;False;0;0;0;True;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.BreakToComponentsNode;68;1782.574,-317.4251;Inherit;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.DynamicAppendNode;71;2077.565,-240.4193;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.FunctionNode;63;1435.181,-261.8437;Inherit;False;Height Based Mix;-1;;103;9ef83f4fd28e7eb4aaa0d4ca3f9da236;0;6;24;FLOAT;0;False;25;FLOAT;0;False;22;FLOAT;0;False;23;FLOAT;0;False;20;FLOAT4;0,0,0,0;False;21;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.FunctionNode;73;2177.579,-621.8306;Inherit;True;Add Spatter;1;;104;c2f011ec950e65243864f0c5daebe4b8;0;6;173;FLOAT4;0,0,0,0;False;175;SAMPLER2D;0;False;176;FLOAT;0;False;177;FLOAT;0;False;178;FLOAT;0;False;179;FLOAT4;0,0,0,0;False;4;FLOAT;170;FLOAT;171;FLOAT;172;COLOR;0
Node;AmplifyShaderEditor.UnpackScaleNormalNode;72;2272.565,-267.4193;Inherit;False;Tangent;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.BreakToComponentsNode;65;1782.572,-158.3963;Inherit;False;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;4;0,0;Float;False;False;-1;2;Rendering.HighDefinition.LightingShaderGraphGUI;0;1;New Amplify Shader;53b46d85872c5b24c8f4f0a1c3fe4c87;True;DepthOnly;0;4;DepthOnly;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=HDRenderPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;5;True;7;d3d11;metal;vulkan;xboxone;xboxseries;playstation;switch;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;0;True;-30;False;False;False;False;False;False;False;False;False;True;True;0;True;-8;255;False;-1;255;True;-9;7;False;-1;3;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;1;False;-1;False;False;True;1;LightMode=DepthOnly;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;10;0,0;Float;False;False;-1;2;Rendering.HighDefinition.LightingShaderGraphGUI;0;1;New Amplify Shader;53b46d85872c5b24c8f4f0a1c3fe4c87;True;Forward;0;10;Forward;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=HDRenderPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;5;True;7;d3d11;metal;vulkan;xboxone;xboxseries;playstation;switch;0;False;True;1;0;True;-22;0;True;-23;1;0;True;-24;0;True;-25;False;False;False;False;False;False;False;False;False;False;False;False;True;0;True;-33;False;False;False;True;True;True;True;True;0;True;-53;False;False;False;False;False;True;True;0;True;-6;255;False;-1;255;True;-7;7;False;-1;3;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;0;True;-28;True;0;True;-36;False;True;1;LightMode=Forward;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;2;0,0;Float;False;False;-1;2;Rendering.HighDefinition.LightingShaderGraphGUI;0;1;New Amplify Shader;53b46d85872c5b24c8f4f0a1c3fe4c87;True;ShadowCaster;0;2;ShadowCaster;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=HDRenderPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;5;True;7;d3d11;metal;vulkan;xboxone;xboxseries;playstation;switch;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;0;True;-30;False;True;False;False;False;False;0;False;-1;False;False;False;False;False;False;False;False;False;True;1;False;-1;True;3;False;-1;False;True;1;LightMode=ShadowCaster;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;6;0,0;Float;False;False;-1;2;Rendering.HighDefinition.LightingShaderGraphGUI;0;1;New Amplify Shader;53b46d85872c5b24c8f4f0a1c3fe4c87;True;Distortion;0;6;Distortion;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=HDRenderPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;5;True;7;d3d11;metal;vulkan;xboxone;xboxseries;playstation;switch;0;False;True;4;1;False;-1;1;False;-1;4;1;False;-1;1;False;-1;True;1;False;-1;1;False;-1;False;False;False;False;False;False;False;False;False;False;False;True;0;True;-30;False;False;False;False;False;False;False;False;False;True;True;0;True;-12;255;False;-1;255;True;-13;7;False;-1;3;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;2;False;-1;True;3;False;-1;False;True;1;LightMode=DistortionVectors;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;7;0,0;Float;False;False;-1;2;Rendering.HighDefinition.LightingShaderGraphGUI;0;1;New Amplify Shader;53b46d85872c5b24c8f4f0a1c3fe4c87;True;TransparentBackface;0;7;TransparentBackface;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=HDRenderPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;5;True;7;d3d11;metal;vulkan;xboxone;xboxseries;playstation;switch;0;False;True;1;0;True;-22;0;True;-23;1;0;True;-24;0;True;-25;False;False;False;False;False;False;False;False;False;False;False;False;True;1;False;-1;False;False;False;True;True;True;True;True;0;True;-53;False;False;False;False;False;False;False;True;0;True;-28;True;0;True;-37;False;True;1;LightMode=TransparentBackface;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;1;0,0;Float;False;False;-1;2;Rendering.HighDefinition.LightingShaderGraphGUI;0;1;New Amplify Shader;53b46d85872c5b24c8f4f0a1c3fe4c87;True;META;0;1;META;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=HDRenderPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;5;True;7;d3d11;metal;vulkan;xboxone;xboxseries;playstation;switch;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=Meta;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;11;0,0;Float;False;False;-1;2;Rendering.HighDefinition.LightingShaderGraphGUI;0;1;New Amplify Shader;53b46d85872c5b24c8f4f0a1c3fe4c87;True;ScenePickingPass;0;11;ScenePickingPass;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=HDRenderPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;5;True;7;d3d11;metal;vulkan;xboxone;xboxseries;playstation;switch;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;0;True;-30;False;False;False;False;False;False;False;False;False;False;False;True;2;False;-1;True;3;False;-1;False;True;1;LightMode=Picking;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;3;0,0;Float;False;False;-1;2;Rendering.HighDefinition.LightingShaderGraphGUI;0;1;New Amplify Shader;53b46d85872c5b24c8f4f0a1c3fe4c87;True;SceneSelectionPass;0;3;SceneSelectionPass;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=HDRenderPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;5;True;7;d3d11;metal;vulkan;xboxone;xboxseries;playstation;switch;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;1;LightMode=SceneSelectionPass;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;9;0,0;Float;False;False;-1;2;Rendering.HighDefinition.LightingShaderGraphGUI;0;1;New Amplify Shader;53b46d85872c5b24c8f4f0a1c3fe4c87;True;TransparentDepthPostpass;0;9;TransparentDepthPostpass;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=HDRenderPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;5;True;7;d3d11;metal;vulkan;xboxone;xboxseries;playstation;switch;0;False;True;1;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;True;0;True;-30;False;True;False;False;False;False;0;False;-1;False;False;False;False;False;False;False;False;False;True;1;False;-1;False;False;True;1;LightMode=TransparentDepthPostpass;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;5;0,0;Float;False;False;-1;2;Rendering.HighDefinition.LightingShaderGraphGUI;0;1;New Amplify Shader;53b46d85872c5b24c8f4f0a1c3fe4c87;True;Motion Vectors;0;5;Motion Vectors;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=HDRenderPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;5;True;7;d3d11;metal;vulkan;xboxone;xboxseries;playstation;switch;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;0;True;-30;False;False;False;False;False;False;False;False;False;True;True;0;True;-10;255;False;-1;255;True;-11;7;False;-1;3;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;1;False;-1;False;False;True;1;LightMode=MotionVectors;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;8;0,0;Float;False;False;-1;2;Rendering.HighDefinition.LightingShaderGraphGUI;0;1;New Amplify Shader;53b46d85872c5b24c8f4f0a1c3fe4c87;True;TransparentDepthPrepass;0;8;TransparentDepthPrepass;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=HDRenderPipeline;RenderType=Opaque=RenderType;Queue=Geometry=Queue=0;True;5;True;7;d3d11;metal;vulkan;xboxone;xboxseries;playstation;switch;0;False;True;1;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;True;0;True;-30;False;False;False;False;False;False;False;False;False;True;True;0;True;-8;255;False;-1;255;True;-9;7;False;-1;3;False;-1;1;False;-1;1;False;-1;7;False;-1;3;False;-1;1;False;-1;1;False;-1;False;True;1;False;-1;False;False;True;1;LightMode=TransparentDepthPrepass;False;False;0;;0;0;Standard;0;False;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;2946.206,-528.3061;Float;False;True;-1;2;Rendering.HighDefinition.LightingShaderGraphGUI;0;2;Terrain Splatter;53b46d85872c5b24c8f4f0a1c3fe4c87;True;GBuffer;0;0;GBuffer;35;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;3;RenderPipeline=HDRenderPipeline;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;True;5;True;7;d3d11;metal;vulkan;xboxone;xboxseries;playstation;switch;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;0;True;-30;False;False;False;False;False;False;False;False;False;True;True;0;True;-15;255;False;-1;255;True;-14;7;False;-1;3;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;False;True;0;True;-16;False;True;1;LightMode=GBuffer;False;False;0;;0;0;Standard;42;Surface Type;1;638022620641894018;  Rendering Pass;1;0;  Refraction Model;0;0;    Blending Mode;0;0;    Blend Preserves Specular;1;0;  Receive Fog;1;0;  Back Then Front Rendering;0;0;  Transparent Depth Prepass;0;0;  Transparent Depth Postpass;0;0;  Transparent Writes Motion Vector;0;0;  Distortion;0;0;    Distortion Mode;0;0;    Distortion Depth Test;1;0;  ZWrite;0;0;  Z Test;4;0;Double-Sided;0;0;Alpha Clipping;1;638022620987582268;  Use Shadow Threshold;0;0;Material Type,InvertActionOnDeselection;0;0;  Energy Conserving Specular;1;0;  Transmission;1;0;Receive Decals;1;638022620289379904;Receives SSR;1;638022620285733445;Receive SSR Transparent;0;0;Motion Vectors;1;0;  Add Precomputed Velocity;0;0;Specular AA;0;0;Specular Occlusion Mode;1;0;Override Baked GI;0;0;Depth Offset;0;0;DOTS Instancing;0;0;LOD CrossFade;0;0;Tessellation;0;0;  Phong;0;0;  Strength;0.5,False,-1;0;  Type;0;0;  Tess;16,False,-1;0;  Min;10,False,-1;0;  Max;25,False,-1;0;  Edge Length;16,False,-1;0;  Max Displacement;25,False,-1;0;Vertex Position;1;0;0;12;True;True;True;True;True;True;False;False;False;False;True;True;False;;False;0
WireConnection;18;0;16;0
WireConnection;17;0;15;0
WireConnection;26;0;17;0
WireConnection;26;1;17;2
WireConnection;28;0;18;3
WireConnection;28;1;18;2
WireConnection;27;0;18;0
WireConnection;27;1;18;1
WireConnection;34;0;33;0
WireConnection;30;0;28;0
WireConnection;30;1;27;0
WireConnection;29;0;26;0
WireConnection;29;1;27;0
WireConnection;35;0;34;3
WireConnection;35;1;34;4
WireConnection;31;0;29;0
WireConnection;31;1;30;0
WireConnection;36;0;31;0
WireConnection;36;1;35;0
WireConnection;37;0;36;0
WireConnection;38;0;37;0
WireConnection;39;0;38;0
WireConnection;40;0;36;0
WireConnection;40;1;39;0
WireConnection;41;0;40;0
WireConnection;78;49;53;0
WireConnection;78;50;51;0
WireConnection;78;51;52;0
WireConnection;78;52;33;0
WireConnection;78;47;54;0
WireConnection;78;48;55;0
WireConnection;78;44;39;0
WireConnection;78;46;35;0
WireConnection;77;49;53;0
WireConnection;77;50;51;0
WireConnection;77;51;52;0
WireConnection;77;52;33;0
WireConnection;77;47;54;0
WireConnection;77;48;55;0
WireConnection;77;44;39;0
WireConnection;77;46;35;0
WireConnection;79;49;53;0
WireConnection;79;50;51;0
WireConnection;79;51;52;0
WireConnection;79;52;33;0
WireConnection;79;47;54;0
WireConnection;79;48;55;0
WireConnection;79;44;39;0
WireConnection;79;46;35;0
WireConnection;80;49;53;0
WireConnection;80;50;51;0
WireConnection;80;51;52;0
WireConnection;80;52;33;0
WireConnection;80;47;54;0
WireConnection;80;48;55;0
WireConnection;80;44;39;0
WireConnection;80;46;35;0
WireConnection;49;0;41;0
WireConnection;44;24;49;0
WireConnection;44;25;41;0
WireConnection;44;22;77;42
WireConnection;44;23;79;42
WireConnection;44;20;77;40
WireConnection;44;21;79;40
WireConnection;47;24;49;0
WireConnection;47;25;41;0
WireConnection;47;22;80;42
WireConnection;47;23;78;42
WireConnection;47;20;80;40
WireConnection;47;21;78;40
WireConnection;45;24;49;0
WireConnection;45;25;41;0
WireConnection;45;22;77;42
WireConnection;45;23;79;42
WireConnection;45;20;77;41
WireConnection;45;21;79;41
WireConnection;48;24;49;0
WireConnection;48;25;41;0
WireConnection;48;22;80;42
WireConnection;48;23;78;42
WireConnection;48;20;80;41
WireConnection;48;21;78;41
WireConnection;61;0;41;1
WireConnection;60;0;47;0
WireConnection;57;0;44;0
WireConnection;46;24;49;0
WireConnection;46;25;41;0
WireConnection;46;22;80;42
WireConnection;46;23;78;42
WireConnection;46;20;80;0
WireConnection;46;21;78;0
WireConnection;64;24;61;0
WireConnection;64;25;41;1
WireConnection;64;22;57;2
WireConnection;64;23;60;2
WireConnection;64;20;45;0
WireConnection;64;21;48;0
WireConnection;43;24;49;0
WireConnection;43;25;41;0
WireConnection;43;22;77;42
WireConnection;43;23;79;42
WireConnection;43;20;77;0
WireConnection;43;21;79;0
WireConnection;62;24;61;0
WireConnection;62;25;41;1
WireConnection;62;22;57;2
WireConnection;62;23;60;2
WireConnection;62;20;43;0
WireConnection;62;21;46;0
WireConnection;66;0;64;0
WireConnection;67;0;62;0
WireConnection;67;1;64;0
WireConnection;81;2;66;3
WireConnection;68;0;62;0
WireConnection;71;1;65;1
WireConnection;71;2;65;1
WireConnection;71;3;65;3
WireConnection;63;24;61;0
WireConnection;63;25;41;1
WireConnection;63;22;57;2
WireConnection;63;23;60;2
WireConnection;63;20;44;0
WireConnection;63;21;47;0
WireConnection;73;173;16;0
WireConnection;73;175;76;0
WireConnection;73;176;81;1
WireConnection;73;177;68;3
WireConnection;73;178;81;0
WireConnection;73;179;67;0
WireConnection;72;0;71;0
WireConnection;65;0;63;0
WireConnection;0;0;73;0
WireConnection;0;1;72;0
WireConnection;0;4;73;170
WireConnection;0;7;73;171
WireConnection;0;9;73;172
ASEEND*/
//CHKSM=1D9D323276BA0DE13862853AF1190413C154BFE3