Shader /*ase_name*/ "Hidden/HD/Decal" /*end*/
{
    Properties
    {
		/*ase_props*/
        [HideInInspector]_DrawOrder("Draw Order", Int) = 0
		[HideInInspector][Enum(Depth Bias, 0, View Bias, 1)]_DecalMeshBiasType("Float", Float) = 0
        [HideInInspector]_DecalMeshDepthBias("DecalMesh DepthBias", Float) = 0
		[HideInInspector]_DecalMeshViewBias("DecalMesh ViewBias", Float) = 0
        [HideInInspector]_DecalStencilWriteMask("Float", Int) = 0
        [HideInInspector]_DecalStencilRef("Float", Int) = 0
        [HideInInspector][ToggleUI]_AffectAlbedo("Boolean", Float) = 1
        [HideInInspector][ToggleUI]_AffectNormal("Boolean", Float) = 1
        [HideInInspector][ToggleUI]_AffectAO("Boolean", Float) = 1
        [HideInInspector][ToggleUI]_AffectMetal("Boolean", Float) = 1
        [HideInInspector][ToggleUI]_AffectSmoothness("Boolean", Float) = 1
        [HideInInspector][ToggleUI]_AffectEmission("Boolean", Float) = 1
        [HideInInspector]_DecalColorMask0("Float", Int) = 0
        [HideInInspector]_DecalColorMask1("Float", Int) = 0
        [HideInInspector]_DecalColorMask2("Float", Int) = 0
        [HideInInspector]_DecalColorMask3("Float", Int) = 0
        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
    }
    SubShader
    {
		/*ase_subshader_options:Name=Additional Options
			Option:Affect BaseColor:false,true:true
				true:SetDefine:pragma shader_feature _ _MATERIAL_AFFECTS_ALBEDO
				false:RemoveDefine:pragma shader_feature _ _MATERIAL_AFFECTS_ALBEDO
			Option:Affect Normal:false,true:true
				true:SetDefine:pragma shader_feature _ _MATERIAL_AFFECTS_NORMAL
				false:RemoveDefine:pragma shader_feature _ _MATERIAL_AFFECTS_NORMAL
			Option:Affect Metal:false,true:true
				true:SetDefine:pragma shader_feature _ _MATERIAL_AFFECTS_MASKMAP
				false:RemoveDefine:pragma shader_feature _ _MATERIAL_AFFECTS_MASKMAP
			Option:Affect AO:false,true:true
				true:SetDefine:pragma shader_feature _ _MATERIAL_AFFECTS_MASKMAP
				false:RemoveDefine:pragma shader_feature _ _MATERIAL_AFFECTS_MASKMAP
			Option:Affect Smoothness:false,true:true
				true:SetDefine:pragma shader_feature _ _MATERIAL_AFFECTS_MASKMAP
				false:RemoveDefine:pragma shader_feature _ _MATERIAL_AFFECTS_MASKMAP
			Option:Affect Emission:false,true:true
				true:SetDefine:_MATERIAL_AFFECTS_EMISSION
				true:IncludePass:DecalProjectorForwardEmissive
				true:IncludePass:DecalMeshForwardEmissive
				false:RemoveDefine:_MATERIAL_AFFECTS_EMISSION
				false:ExcludePass:DecalProjectorForwardEmissive
				false:ExcludePass:DecalMeshForwardEmissive
			Option:Support LOD CrossFade:false,true:true
				true:SetDefine:pragma multi_compile _ LOD_FADE_CROSSFADE
				false:RemoveDefine:pragma multi_compile _ LOD_FADE_CROSSFADE
			Option:Vertex Position,InvertActionOnDeselection:Absolute,Relative:Relative
				Absolute:SetDefine:ASE_ABSOLUTE_VERTEX_POS 1
				Absolute:SetPortName:DecalProjectorForwardEmissive:9,Vertex Position
				Relative:SetPortName:DecalProjectorForwardEmissive:9,Vertex Offset
		*/
        Tags
        {
            "RenderPipeline"="HDRenderPipeline"
            "RenderType"="Opaque"
            "Queue"="Geometry+0"
        }

		HLSLINCLUDE
		#pragma target 4.5
		#pragma only_renderers d3d11 playstation xboxone xboxseries vulkan metal switch

		struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
            float3 NormalTS;
            float NormalAlpha;
            float Metallic;
            float Occlusion;
            float Smoothness;
            float MAOSAlpha;
			float3 Emission;
        };
		ENDHLSL
		
		/*ase_pass*/
        Pass
        { 
			/*ase_main_pass*/
            Name "DBufferProjector"
            Tags 
            { 
                "LightMode" = "DBufferProjector"
            }
            
			Cull Front
            Blend 0 SrcAlpha OneMinusSrcAlpha, Zero OneMinusSrcAlpha Blend 1 SrcAlpha OneMinusSrcAlpha, Zero OneMinusSrcAlpha Blend 2 SrcAlpha OneMinusSrcAlpha, Zero OneMinusSrcAlpha Blend 3 Zero OneMinusSrcColor
            ZTest Greater
            ZWrite Off
            ColorMask [_DecalColorMask0] ColorMask [_DecalColorMask1] 1 ColorMask [_DecalColorMask2] 2 ColorMask [_DecalColorMask3] 3
            Stencil
			{
				WriteMask [_DecalStencilWriteMask]
				Ref [_DecalStencilRef]
				CompFront Always
				PassFront Replace
				CompBack Always
				PassBack Replace
			}
    
            HLSLPROGRAM
    
            #pragma vertex Vert
            #pragma fragment Frag
            
            #pragma multi_compile_instancing
    
            #pragma multi_compile DECALS_3RT DECALS_4RT
			#pragma multi_compile_fragment _ DECAL_SURFACE_GRADIENT

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/NormalSurfaceGradient.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"
    
            #define SHADERPASS SHADERPASS_DBUFFER_PROJECTOR
    
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Packing.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/Functions.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/FragInputs.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Decal/Decal.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Decal/DecalPrepassBuffer.hlsl"

			/*ase_pragma*/

            struct AttributesMesh
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
				/*ase_vdata:p=p;n=n;t=t*/
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

      		struct PackedVaryingsToPS
			{
				float4 positionCS : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_RELATIVE_WORLD_POS)
				float3 positionRWS : TEXCOORD0;
				#endif
				/*ase_interp(1,):sp=sp.xyzw;rwp=tc0*/
                UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

            
            CBUFFER_START(UnityPerMaterial)
            float _DrawOrder;
			float _DecalMeshBiasType;
            float _DecalMeshDepthBias;
			float _DecalMeshViewBias;
            float _DecalStencilWriteMask;
            float _DecalStencilRef;
			#ifdef _MATERIAL_AFFECTS_ALBEDO
            float _AffectAlbedo;
			#endif
			#ifdef _MATERIAL_AFFECTS_NORMAL
            float _AffectNormal;
			#endif
            #ifdef _MATERIAL_AFFECTS_MASKMAP
            float _AffectAO;
			float _AffectMetal;
            float _AffectSmoothness;
			#endif
			#ifdef _MATERIAL_AFFECTS_EMISSION
            float _AffectEmission;
			#endif
            float _DecalColorMask0;
            float _DecalColorMask1;
            float _DecalColorMask2;
            float _DecalColorMask3;
            CBUFFER_END
                
			/*ase_globals*/

			/*ase_funcs*/
                
            void GetSurfaceData(SurfaceDescription surfaceDescription, FragInputs fragInputs, float3 V, PositionInputs posInput, float angleFadeFactor, out DecalSurfaceData surfaceData)
            {
                #if (SHADERPASS == SHADERPASS_DBUFFER_PROJECTOR) || (SHADERPASS == SHADERPASS_FORWARD_EMISSIVE_PROJECTOR)
                    float4x4 normalToWorld = UNITY_ACCESS_INSTANCED_PROP(Decal, _NormalToWorld);
                    float fadeFactor = clamp(normalToWorld[0][3], 0.0f, 1.0f) * angleFadeFactor;
                    float2 scale = float2(normalToWorld[3][0], normalToWorld[3][1]);
                    float2 offset = float2(normalToWorld[3][2], normalToWorld[3][3]);
                    fragInputs.texCoord0.xy = fragInputs.texCoord0.xy * scale + offset;
                    fragInputs.texCoord1.xy = fragInputs.texCoord1.xy * scale + offset;
                    fragInputs.texCoord2.xy = fragInputs.texCoord2.xy * scale + offset;
                    fragInputs.texCoord3.xy = fragInputs.texCoord3.xy * scale + offset;
                    fragInputs.positionRWS = posInput.positionWS;
                    fragInputs.tangentToWorld[2].xyz = TransformObjectToWorldDir(float3(0, 1, 0));
                    fragInputs.tangentToWorld[1].xyz = TransformObjectToWorldDir(float3(0, 0, 1));
                #else
                    #ifdef LOD_FADE_CROSSFADE 
                    LODDitheringTransition(ComputeFadeMaskSeed(V, posInput.positionSS), unity_LODFade.x);
                    #endif
    
                    float fadeFactor = 1.0;
                #endif
    
                ZERO_INITIALIZE(DecalSurfaceData, surfaceData);
    
                #ifdef _MATERIAL_AFFECTS_EMISSION
                #endif
    
                #ifdef _MATERIAL_AFFECTS_ALBEDO
                    surfaceData.baseColor.xyz = surfaceDescription.BaseColor;
                    surfaceData.baseColor.w = surfaceDescription.Alpha * fadeFactor;
                #endif
    
                #ifdef _MATERIAL_AFFECTS_NORMAL
                    #ifdef DECAL_SURFACE_GRADIENT
                        #if (SHADERPASS == SHADERPASS_DBUFFER_PROJECTOR) || (SHADERPASS == SHADERPASS_FORWARD_EMISSIVE_PROJECTOR)
                            float3x3 tangentToWorld = transpose((float3x3)normalToWorld);
                        #else
                            float3x3 tangentToWorld = fragInputs.tangentToWorld;
                        #endif
        
                        surfaceData.normalWS.xyz = SurfaceGradientFromTangentSpaceNormalAndFromTBN(surfaceDescription.NormalTS.xyz, tangentToWorld[0], tangentToWorld[1]);
                    #else
                        #if (SHADERPASS == SHADERPASS_DBUFFER_PROJECTOR)
                            surfaceData.normalWS.xyz = mul((float3x3)normalToWorld, surfaceDescription.NormalTS);
                        #elif (SHADERPASS == SHADERPASS_DBUFFER_MESH) || (SHADERPASS == SHADERPASS_FORWARD_PREVIEW)
                            
                            surfaceData.normalWS.xyz = normalize(TransformTangentToWorld(surfaceDescription.NormalTS, fragInputs.tangentToWorld));
                        #endif
                    #endif
        
                    surfaceData.normalWS.w = surfaceDescription.NormalAlpha * fadeFactor;
                #else
                    #if (SHADERPASS == SHADERPASS_FORWARD_PREVIEW) 
                        #ifdef DECAL_SURFACE_GRADIENT
                            surfaceData.normalWS.xyz = float3(0.0, 0.0, 0.0);
                        #else
                            surfaceData.normalWS.xyz = normalize(TransformTangentToWorld(float3(0.0, 0.0, 0.1), fragInputs.tangentToWorld));
                        #endif
                    #endif
                #endif
    
                #ifdef _MATERIAL_AFFECTS_MASKMAP
                    surfaceData.mask.z = surfaceDescription.Smoothness;
                    surfaceData.mask.w = surfaceDescription.MAOSAlpha * fadeFactor;
    
                    #ifdef DECALS_4RT
                        surfaceData.mask.x = surfaceDescription.Metallic;
                        surfaceData.mask.y = surfaceDescription.Occlusion;
                        surfaceData.MAOSBlend.x = surfaceDescription.MAOSAlpha * fadeFactor;
                        surfaceData.MAOSBlend.y = surfaceDescription.MAOSAlpha * fadeFactor;
                    #endif
                                                                  
                #endif
            }
                
			PackedVaryingsToPS Vert(AttributesMesh inputMesh  /*ase_vert_input*/)
			{
				PackedVaryingsToPS output;
					
				UNITY_SETUP_INSTANCE_ID(inputMesh);
				UNITY_TRANSFER_INSTANCE_ID(inputMesh, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( output );
				/*ase_vert_code:inputMesh=AttributesMesh;output=PackedVaryingsToPS*/
				#ifdef ASE_ABSOLUTE_VERTEX_POS
				float3 defaultVertexValue = inputMesh.positionOS.xyz;
				#else
				float3 defaultVertexValue = float3( 0, 0, 0 );
				#endif
				float3 vertexValue = /*ase_vert_out:Vertex Offset;Float3;9;-1;_VertexOffset*/defaultVertexValue/*end*/;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
				inputMesh.positionOS.xyz = vertexValue;
				#else
				inputMesh.positionOS.xyz += vertexValue;
				#endif

				inputMesh.normalOS = /*ase_vert_out:Vertex Normal;Float3;10;-1;_VertexNormal*/inputMesh.normalOS/*end*/;
				inputMesh.tangentOS = /*ase_vert_out:Vertex Tangent;Float4;11;-1;_VertexTangent*/inputMesh.tangentOS/*end*/;

				float3 positionRWS = TransformObjectToWorld(inputMesh.positionOS);
				float3 normalWS = TransformObjectToWorldNormal(inputMesh.normalOS);
				float4 tangentWS = float4(TransformObjectToWorldDir(inputMesh.tangentOS.xyz), inputMesh.tangentOS.w);
				
				output.positionCS = TransformWorldToHClip(positionRWS);
				#if defined(ASE_NEEDS_FRAG_RELATIVE_WORLD_POS)
				output.positionRWS = positionRWS;
				#endif
		
				return output;
			}

			void Frag( PackedVaryingsToPS packedInput,
			#if (SHADERPASS == SHADERPASS_DBUFFER_PROJECTOR) || (SHADERPASS == SHADERPASS_DBUFFER_MESH)
				OUTPUT_DBUFFER(outDBuffer)
			#else
				out float4 outEmissive : SV_Target0
			#endif
			/*ase_frag_input*/
			)
			{
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(packedInput);
				UNITY_SETUP_INSTANCE_ID(packedInput);
				
				FragInputs input;
                ZERO_INITIALIZE(FragInputs, input);
                input.tangentToWorld = k_identity3x3;
				#if defined(ASE_NEEDS_FRAG_RELATIVE_WORLD_POS)
				/*ase_local_var:rwp*/float3 positionRWS = packedInput.positionRWS;
				input.positionRWS = positionRWS;
				#endif

                input.positionSS = packedInput.positionCS;

				DecalSurfaceData surfaceData;
				float clipValue = 1.0;
				float angleFadeFactor = 1.0;

			#if (SHADERPASS == SHADERPASS_DBUFFER_PROJECTOR) || (SHADERPASS == SHADERPASS_FORWARD_EMISSIVE_PROJECTOR)    

				float depth = LoadCameraDepth(input.positionSS.xy);
				PositionInputs posInput = GetPositionInput(input.positionSS.xy, _ScreenSize.zw, depth, UNITY_MATRIX_I_VP, UNITY_MATRIX_V);

				DecalPrepassData material;
				ZERO_INITIALIZE(DecalPrepassData, material);
				if (_EnableDecalLayers)
				{
					uint decalLayerMask = uint(UNITY_ACCESS_INSTANCED_PROP(Decal, _DecalLayerMaskFromDecal).x);

					DecodeFromDecalPrepass(posInput.positionSS, material);

					if ((decalLayerMask & material.decalLayerMask) == 0)
						clipValue -= 2.0;
				}

				
				float3 positionDS = TransformWorldToObject(posInput.positionWS);
				positionDS = positionDS * float3(1.0, -1.0, 1.0) + float3(0.5, 0.5, 0.5);
				if (!(all(positionDS.xyz > 0.0f) && all(1.0f - positionDS.xyz > 0.0f)))
				{
					clipValue -= 2.0; 
				}

			#ifndef SHADER_API_METAL
				clip(clipValue);
			#else
				if (clipValue > 0.0)
				{
			#endif
				input.texCoord0.xy = positionDS.xz;
				input.texCoord1.xy = positionDS.xz;
				input.texCoord2.xy = positionDS.xz;
				input.texCoord3.xy = positionDS.xz;

				float3 V = GetWorldSpaceNormalizeViewDir(posInput.positionWS);
				if (_EnableDecalLayers)
				{
					float4x4 normalToWorld = UNITY_ACCESS_INSTANCED_PROP(Decal, _NormalToWorld);
					float2 angleFade = float2(normalToWorld[1][3], normalToWorld[2][3]);

					if (angleFade.y < 0.0f) 
					{
						float3 decalNormal = float3(normalToWorld[0].z, normalToWorld[1].z, normalToWorld[2].z);
						float dotAngle = dot(material.geomNormalWS, decalNormal);
						angleFadeFactor = saturate(angleFade.x + angleFade.y * (dotAngle * (dotAngle - 2.0)));
					}
				}

			#else
				PositionInputs posInput = GetPositionInput(input.positionSS.xy, _ScreenSize.zw, input.positionSS.z, input.positionSS.w, input.positionRWS.xyz, uint2(0, 0));
				#if defined(ASE_NEEDS_FRAG_RELATIVE_WORLD_POS)
					float3 V = GetWorldSpaceNormalizeViewDir(input.positionRWS);
				#else
					float3 V = float3(1.0, 1.0, 1.0);
				#endif
			#endif

				/*ase_local_var:uv0*/float4 texCoord0 = input.texCoord0;
				/*ase_local_var:uv1*/float4 texCoord1 = input.texCoord1;
				/*ase_local_var:uv2*/float4 texCoord2 = input.texCoord2;
				/*ase_local_var:uv3*/float4 texCoord3 = input.texCoord3;
				SurfaceDescription surfaceDescription = (SurfaceDescription)0;
				/*ase_frag_code:packedInput=PackedVaryingsToPS*/
				surfaceDescription.BaseColor = /*ase_frag_out:Albedo;Float3;0;-1;_Albedo*/float3( 0.7353569, 0.7353569, 0.7353569 )/*end*/;
				surfaceDescription.Alpha = /*ase_frag_out:Alpha Albedo;Float;1;-1;_AlphaAlbedo*/1/*end*/;
				surfaceDescription.NormalTS = /*ase_frag_out:Normal;Float3;2;-1;_Normal*/float3( 0, 0, 1 )/*end*/;
				surfaceDescription.NormalAlpha = /*ase_frag_out:Alpha Normal;Float;3;-1;_AlphaNormal*/1/*end*/;
				surfaceDescription.Metallic = /*ase_frag_out:Metallic;Float;4;-1;_Metallic*/0/*end*/;
				surfaceDescription.Occlusion = /*ase_frag_out:Occlusion;Float;5;-1;_Occlusion*/1/*end*/;
				surfaceDescription.Smoothness = /*ase_frag_out:Smoothness;Float;6;-1;_Smoothness*/0.5/*end*/;
				surfaceDescription.MAOSAlpha = /*ase_frag_out:MAOSOpacity;Float;7;-1;_MAOSOpacity*/1/*end*/;
				surfaceDescription.Emission = /*ase_frag_out:Emission;Float3;8;-1;_Emission*/float3( 0, 0, 0 )/*end*/;

				GetSurfaceData(surfaceDescription, input, V, posInput, angleFadeFactor, surfaceData);

			#if ((SHADERPASS == SHADERPASS_DBUFFER_PROJECTOR) || (SHADERPASS == SHADERPASS_FORWARD_EMISSIVE_PROJECTOR)) && defined(SHADER_API_METAL)
				} // if (clipValue > 0.0)

				clip(clipValue);
			#endif

			#if (SHADERPASS == SHADERPASS_DBUFFER_PROJECTOR) || (SHADERPASS == SHADERPASS_DBUFFER_MESH)
				ENCODE_INTO_DBUFFER(surfaceData, outDBuffer);
			#else
				// Emissive need to be pre-exposed
				outEmissive.rgb = surfaceData.emissive * GetCurrentExposureMultiplier();
				outEmissive.a = 1.0;
			#endif
			}

            ENDHLSL
        }

		/*ase_pass*/
        Pass
        { 
			/*ase_hide_pass*/
            Name "DecalProjectorForwardEmissive"
            Tags 
            { 
                "LightMode" = "DecalProjectorForwardEmissive"
            }
            
			Cull Front
            Blend 0 SrcAlpha One
            ZTest Greater
            ZWrite Off
    
            HLSLPROGRAM
    
           
            #pragma vertex Vert
            #pragma fragment Frag
            
            #pragma multi_compile_instancing
    
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/NormalSurfaceGradient.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"
    
            #define SHADERPASS SHADERPASS_FORWARD_EMISSIVE_PROJECTOR
            
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Packing.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/Functions.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/FragInputs.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Decal/Decal.hlsl"

			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Decal/DecalPrepassBuffer.hlsl"

			/*ase_pragma*/

            struct AttributesMesh
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
				/*ase_vdata:p=p;n=n;t=t*/
				UNITY_VERTEX_INPUT_INSTANCE_ID
            };

      		struct PackedVaryingsToPS
			{
				float4 positionCS : SV_POSITION;
				#if defined(ASE_NEEDS_FRAG_RELATIVE_WORLD_POS)
				float3 positionRWS : TEXCOORD0;
				#endif
				/*ase_interp(1,):sp=sp.xyzw;rwp=tc0*/
                UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

            
            CBUFFER_START(UnityPerMaterial)
            float _DrawOrder;
			float _DecalMeshBiasType;
            float _DecalMeshDepthBias;
			float _DecalMeshViewBias;
            float _DecalStencilWriteMask;
            float _DecalStencilRef;
            #ifdef _MATERIAL_AFFECTS_ALBEDO
            float _AffectAlbedo;
			#endif
            #ifdef _MATERIAL_AFFECTS_NORMAL
            float _AffectNormal;
			#endif
            #ifdef _MATERIAL_AFFECTS_MASKMAP
            float _AffectAO;
			float _AffectMetal;
            float _AffectSmoothness;
			#endif
            #ifdef _MATERIAL_AFFECTS_EMISSION
            float _AffectEmission;
			#endif
            float _DecalColorMask0;
            float _DecalColorMask1;
            float _DecalColorMask2;
            float _DecalColorMask3;
            CBUFFER_END
                
			/*ase_globals*/

			/*ase_funcs*/
                
            void GetSurfaceData(SurfaceDescription surfaceDescription, FragInputs fragInputs, float3 V, PositionInputs posInput, float angleFadeFactor, out DecalSurfaceData surfaceData)
            {
                #if (SHADERPASS == SHADERPASS_DBUFFER_PROJECTOR) || (SHADERPASS == SHADERPASS_FORWARD_EMISSIVE_PROJECTOR)
                    float4x4 normalToWorld = UNITY_ACCESS_INSTANCED_PROP(Decal, _NormalToWorld);
                    float fadeFactor = clamp(normalToWorld[0][3], 0.0f, 1.0f) * angleFadeFactor;
                    float2 scale = float2(normalToWorld[3][0], normalToWorld[3][1]);
                    float2 offset = float2(normalToWorld[3][2], normalToWorld[3][3]);
                    fragInputs.texCoord0.xy = fragInputs.texCoord0.xy * scale + offset;
                    fragInputs.texCoord1.xy = fragInputs.texCoord1.xy * scale + offset;
                    fragInputs.texCoord2.xy = fragInputs.texCoord2.xy * scale + offset;
                    fragInputs.texCoord3.xy = fragInputs.texCoord3.xy * scale + offset;
                    fragInputs.positionRWS = posInput.positionWS;
                    fragInputs.tangentToWorld[2].xyz = TransformObjectToWorldDir(float3(0, 1, 0));
                    fragInputs.tangentToWorld[1].xyz = TransformObjectToWorldDir(float3(0, 0, 1));
                #else
                    #ifdef LOD_FADE_CROSSFADE 
                    LODDitheringTransition(ComputeFadeMaskSeed(V, posInput.positionSS), unity_LODFade.x);
                    #endif
    
                    float fadeFactor = 1.0;
                #endif
    
                ZERO_INITIALIZE(DecalSurfaceData, surfaceData);
    
                #ifdef _MATERIAL_AFFECTS_EMISSION
                    surfaceData.emissive.rgb = surfaceDescription.Emission.rgb * fadeFactor;
                #endif 
            }
                
			PackedVaryingsToPS Vert(AttributesMesh inputMesh  /*ase_vert_input*/)
			{
				PackedVaryingsToPS output;
					
				UNITY_SETUP_INSTANCE_ID(inputMesh);
				UNITY_TRANSFER_INSTANCE_ID(inputMesh, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( output );
				/*ase_vert_code:inputMesh=AttributesMesh;output=PackedVaryingsToPS*/
				#ifdef ASE_ABSOLUTE_VERTEX_POS
				float3 defaultVertexValue = inputMesh.positionOS.xyz;
				#else
				float3 defaultVertexValue = float3( 0, 0, 0 );
				#endif
				float3 vertexValue = /*ase_vert_out:Vertex Offset;Float3;9;-1;_VertexOffset*/defaultVertexValue/*end*/;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
				inputMesh.positionOS.xyz = vertexValue;
				#else
				inputMesh.positionOS.xyz += vertexValue;
				#endif

				inputMesh.normalOS = /*ase_vert_out:Vertex Normal;Float3;10;-1;_VertexNormal*/inputMesh.normalOS/*end*/;
				inputMesh.tangentOS = /*ase_vert_out:Vertex Tangent;Float4;11;-1;_VertexTangent*/inputMesh.tangentOS/*end*/;

				float3 positionRWS = TransformObjectToWorld(inputMesh.positionOS);
				float3 normalWS = TransformObjectToWorldNormal(inputMesh.normalOS);
				float4 tangentWS = float4(TransformObjectToWorldDir(inputMesh.tangentOS.xyz), inputMesh.tangentOS.w);
				
				output.positionCS = TransformWorldToHClip(positionRWS);
				#if defined(ASE_NEEDS_FRAG_RELATIVE_WORLD_POS)
				output.positionRWS = positionRWS;
				#endif
		
				return output;
			}

			void Frag( PackedVaryingsToPS packedInput,
			#if (SHADERPASS == SHADERPASS_DBUFFER_PROJECTOR) || (SHADERPASS == SHADERPASS_DBUFFER_MESH)
				OUTPUT_DBUFFER(outDBuffer)
			#else
				out float4 outEmissive : SV_Target0
			#endif
			/*ase_frag_input*/
			)
			{
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(packedInput);
				UNITY_SETUP_INSTANCE_ID(packedInput);
				
				FragInputs input;
                ZERO_INITIALIZE(FragInputs, input);
                input.tangentToWorld = k_identity3x3;
				#if defined(ASE_NEEDS_FRAG_RELATIVE_WORLD_POS)
				/*ase_local_var:rwp*/float3 positionRWS = packedInput.positionRWS;
				input.positionRWS = positionRWS;
				#endif

                input.positionSS = packedInput.positionCS;

				DecalSurfaceData surfaceData;
				float clipValue = 1.0;
				float angleFadeFactor = 1.0;

			#if (SHADERPASS == SHADERPASS_DBUFFER_PROJECTOR) || (SHADERPASS == SHADERPASS_FORWARD_EMISSIVE_PROJECTOR)    

				float depth = LoadCameraDepth(input.positionSS.xy);
				#if (SHADERPASS == SHADERPASS_FORWARD_EMISSIVE_PROJECTOR) && UNITY_REVERSED_Z
					depth = max(0.0001f, depth);
				#endif

				PositionInputs posInput = GetPositionInput(input.positionSS.xy, _ScreenSize.zw, depth, UNITY_MATRIX_I_VP, UNITY_MATRIX_V);

				DecalPrepassData material;
				ZERO_INITIALIZE(DecalPrepassData, material);
				if (_EnableDecalLayers)
				{
					uint decalLayerMask = uint(UNITY_ACCESS_INSTANCED_PROP(Decal, _DecalLayerMaskFromDecal).x);

					DecodeFromDecalPrepass(posInput.positionSS, material);

					if ((decalLayerMask & material.decalLayerMask) == 0)
						clipValue -= 2.0;
				}

				
				float3 positionDS = TransformWorldToObject(posInput.positionWS);
				positionDS = positionDS * float3(1.0, -1.0, 1.0) + float3(0.5, 0.5, 0.5);
				if (!(all(positionDS.xyz > 0.0f) && all(1.0f - positionDS.xyz > 0.0f)))
				{
					clipValue -= 2.0; 
				}

			#ifndef SHADER_API_METAL
				clip(clipValue);
			#else
				if (clipValue > 0.0)
				{
			#endif
				input.texCoord0.xy = positionDS.xz;
				input.texCoord1.xy = positionDS.xz;
				input.texCoord2.xy = positionDS.xz;
				input.texCoord3.xy = positionDS.xz;

				float3 V = GetWorldSpaceNormalizeViewDir(posInput.positionWS);
				if (_EnableDecalLayers)
				{
					float4x4 normalToWorld = UNITY_ACCESS_INSTANCED_PROP(Decal, _NormalToWorld);
					float2 angleFade = float2(normalToWorld[1][3], normalToWorld[2][3]);

					if (angleFade.y < 0.0f) 
					{
						float3 decalNormal = float3(normalToWorld[0].z, normalToWorld[1].z, normalToWorld[2].z);
						float dotAngle = dot(material.geomNormalWS, decalNormal);
						
						angleFadeFactor = saturate(angleFade.x + angleFade.y * (dotAngle * (dotAngle - 2.0)));
					}
				}

			#else
				PositionInputs posInput = GetPositionInput(input.positionSS.xy, _ScreenSize.zw, input.positionSS.z, input.positionSS.w, input.positionRWS.xyz, uint2(0, 0));
				#if defined(ASE_NEEDS_FRAG_RELATIVE_WORLD_POS)
					float3 V = GetWorldSpaceNormalizeViewDir(input.positionRWS);
				#else
					float3 V = float3(1.0, 1.0, 1.0);
				#endif
			#endif

				/*ase_local_var:uv0*/float4 texCoord0 = input.texCoord0;
				/*ase_local_var:uv1*/float4 texCoord1 = input.texCoord1;
				/*ase_local_var:uv2*/float4 texCoord2 = input.texCoord2;
				/*ase_local_var:uv3*/float4 texCoord3 = input.texCoord3;
				SurfaceDescription surfaceDescription = (SurfaceDescription)0;
				/*ase_frag_code:packedInput=PackedVaryingsToPS*/
				surfaceDescription.BaseColor = /*ase_frag_out:Albedo;Float3;0;-1;_Albedo*/float3( 0.7353569, 0.7353569, 0.7353569 )/*end*/;
				surfaceDescription.Alpha = /*ase_frag_out:Alpha Albedo;Float;1;-1;_AlphaAlbedo*/1/*end*/;
				surfaceDescription.NormalTS = /*ase_frag_out:Normal;Float3;2;-1;_Normal*/float3( 0, 0, 1 )/*end*/;
				surfaceDescription.NormalAlpha = /*ase_frag_out:Alpha Normal;Float;3;-1;_AlphaNormal*/1/*end*/;
				surfaceDescription.Metallic = /*ase_frag_out:Metallic;Float;4;-1;_Metallic*/0/*end*/;
				surfaceDescription.Occlusion = /*ase_frag_out:Occlusion;Float;5;-1;_Occlusion*/1/*end*/;
				surfaceDescription.Smoothness = /*ase_frag_out:Smoothness;Float;6;-1;_Smoothness*/0.5/*end*/;
				surfaceDescription.MAOSAlpha = /*ase_frag_out:MAOSOpacity;Float;7;-1;_MAOSOpacity*/1/*end*/;
				surfaceDescription.Emission = /*ase_frag_out:Emission;Float3;8;-1;_Emission*/float3( 0, 0, 0 )/*end*/;

				GetSurfaceData(surfaceDescription, input, V, posInput, angleFadeFactor, surfaceData);

			#if ((SHADERPASS == SHADERPASS_DBUFFER_PROJECTOR) || (SHADERPASS == SHADERPASS_FORWARD_EMISSIVE_PROJECTOR)) && defined(SHADER_API_METAL)
				}

				clip(clipValue);
			#endif

			#if (SHADERPASS == SHADERPASS_DBUFFER_PROJECTOR) || (SHADERPASS == SHADERPASS_DBUFFER_MESH)
				ENCODE_INTO_DBUFFER(surfaceData, outDBuffer);
			#else
				// Emissive need to be pre-exposed
				outEmissive.rgb = surfaceData.emissive * GetCurrentExposureMultiplier();
				outEmissive.a = 1.0;
			#endif
			}

            ENDHLSL
        }

		/*ase_pass*/
        Pass
        { 
			/*ase_hide_pass*/
            Name "DBufferMesh"
            Tags 
            { 
                "LightMode" = "DBufferMesh"
            }
            
			Blend 0 SrcAlpha OneMinusSrcAlpha, Zero OneMinusSrcAlpha Blend 1 SrcAlpha OneMinusSrcAlpha, Zero OneMinusSrcAlpha Blend 2 SrcAlpha OneMinusSrcAlpha, Zero OneMinusSrcAlpha Blend 3 Zero OneMinusSrcColor
            ZTest LEqual
            ZWrite Off
            ColorMask [_DecalColorMask0] ColorMask [_DecalColorMask1] 1 ColorMask [_DecalColorMask2] 2 ColorMask [_DecalColorMask3] 3
            
			Stencil
            {
                WriteMask [_DecalStencilWriteMask]
                Ref [_DecalStencilRef]
                CompFront Always
                PassFront Replace
                CompBack Always
                PassBack Replace
            }
    
            HLSLPROGRAM
    
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma multi_compile_instancing
    
            #pragma multi_compile DECALS_3RT DECALS_4RT
            #pragma multi_compile_fragment _ DECAL_SURFACE_GRADIENT
    
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/NormalSurfaceGradient.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"
    
            
            #define SHADERPASS SHADERPASS_DBUFFER_MESH
    
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Packing.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/Functions.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/FragInputs.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Decal/Decal.hlsl"
			
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Decal/DecalPrepassBuffer.hlsl"
			
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/DecalMeshBiasTypeEnum.cs.hlsl"
			
			/*ase_pragma*/

            struct AttributesMesh
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float4 uv0 : TEXCOORD0;
				/*ase_vdata:p=p;n=n;t=t;uv0=tc0*/
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

			struct PackedVaryingsToPS
			{
				float4 positionCS : SV_POSITION;
                float3 positionRWS : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float4 tangentWS : TEXCOORD2;
                float4 uv0 : TEXCOORD3;
				/*ase_interp(4,):sp=sp.xyzw;rwp=tc0;wn=tc1;wt=tc2;uv0=tc3*/
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			
            CBUFFER_START(UnityPerMaterial)
            float _DrawOrder;
			float _DecalMeshBiasType;
            float _DecalMeshDepthBias;
			float _DecalMeshViewBias;
            float _DecalStencilWriteMask;
            float _DecalStencilRef;
            #ifdef _MATERIAL_AFFECTS_ALBEDO
            float _AffectAlbedo;
			#endif
            #ifdef _MATERIAL_AFFECTS_NORMAL
            float _AffectNormal;
			#endif
            #ifdef _MATERIAL_AFFECTS_MASKMAP
            float _AffectAO;
			float _AffectMetal;
            float _AffectSmoothness;
			#endif
            #ifdef _MATERIAL_AFFECTS_EMISSION
            float _AffectEmission;
			#endif
            float _DecalColorMask0;
            float _DecalColorMask1;
            float _DecalColorMask2;
            float _DecalColorMask3;
            CBUFFER_END
       
	   		/*ase_globals*/

			/*ase_funcs*/

            void GetSurfaceData(SurfaceDescription surfaceDescription, FragInputs fragInputs, float3 V, PositionInputs posInput, float angleFadeFactor, out DecalSurfaceData surfaceData)
            {
                #if (SHADERPASS == SHADERPASS_DBUFFER_PROJECTOR) || (SHADERPASS == SHADERPASS_FORWARD_EMISSIVE_PROJECTOR)
                    float4x4 normalToWorld = UNITY_ACCESS_INSTANCED_PROP(Decal, _NormalToWorld);
                    float fadeFactor = clamp(normalToWorld[0][3], 0.0f, 1.0f) * angleFadeFactor;
                    float2 scale = float2(normalToWorld[3][0], normalToWorld[3][1]);
                    float2 offset = float2(normalToWorld[3][2], normalToWorld[3][3]);
                    fragInputs.texCoord0.xy = fragInputs.texCoord0.xy * scale + offset;
                    fragInputs.texCoord1.xy = fragInputs.texCoord1.xy * scale + offset;
                    fragInputs.texCoord2.xy = fragInputs.texCoord2.xy * scale + offset;
                    fragInputs.texCoord3.xy = fragInputs.texCoord3.xy * scale + offset;
                    fragInputs.positionRWS = posInput.positionWS;
                    fragInputs.tangentToWorld[2].xyz = TransformObjectToWorldDir(float3(0, 1, 0));
                    fragInputs.tangentToWorld[1].xyz = TransformObjectToWorldDir(float3(0, 0, 1));
                #else
                    #ifdef LOD_FADE_CROSSFADE
                    LODDitheringTransition(ComputeFadeMaskSeed(V, posInput.positionSS), unity_LODFade.x);
                    #endif
    
                    float fadeFactor = 1.0;
                #endif
    
                ZERO_INITIALIZE(DecalSurfaceData, surfaceData);
    
                #ifdef _MATERIAL_AFFECTS_EMISSION
                #endif
    
                #ifdef _MATERIAL_AFFECTS_ALBEDO
                    surfaceData.baseColor.xyz = surfaceDescription.BaseColor;
                    surfaceData.baseColor.w = surfaceDescription.Alpha * fadeFactor;
                #endif
    
                #ifdef _MATERIAL_AFFECTS_NORMAL
                    #ifdef DECAL_SURFACE_GRADIENT
                        #if (SHADERPASS == SHADERPASS_DBUFFER_PROJECTOR) || (SHADERPASS == SHADERPASS_FORWARD_EMISSIVE_PROJECTOR)
                            float3x3 tangentToWorld = transpose((float3x3)normalToWorld);
                        #else
                            float3x3 tangentToWorld = fragInputs.tangentToWorld;
                        #endif
        
                        surfaceData.normalWS.xyz = SurfaceGradientFromTangentSpaceNormalAndFromTBN(surfaceDescription.NormalTS.xyz, tangentToWorld[0], tangentToWorld[1]);
                    #else
                        #if (SHADERPASS == SHADERPASS_DBUFFER_PROJECTOR)
                            surfaceData.normalWS.xyz = mul((float3x3)normalToWorld, surfaceDescription.NormalTS);
                        #elif (SHADERPASS == SHADERPASS_DBUFFER_MESH) || (SHADERPASS == SHADERPASS_FORWARD_PREVIEW)
                            
                            surfaceData.normalWS.xyz = normalize(TransformTangentToWorld(surfaceDescription.NormalTS, fragInputs.tangentToWorld));
                        #endif
                    #endif
        
                    surfaceData.normalWS.w = surfaceDescription.NormalAlpha * fadeFactor;
                #else
                    #if (SHADERPASS == SHADERPASS_FORWARD_PREVIEW) 
                        #ifdef DECAL_SURFACE_GRADIENT
                            surfaceData.normalWS.xyz = float3(0.0, 0.0, 0.0);
                        #else
                            surfaceData.normalWS.xyz = normalize(TransformTangentToWorld(float3(0.0, 0.0, 0.1), fragInputs.tangentToWorld));
                        #endif
                    #endif
                #endif
    
                #ifdef _MATERIAL_AFFECTS_MASKMAP
                    surfaceData.mask.z = surfaceDescription.Smoothness;
                    surfaceData.mask.w = surfaceDescription.MAOSAlpha * fadeFactor;
    
                    #ifdef DECALS_4RT
                        surfaceData.mask.x = surfaceDescription.Metallic;
                        surfaceData.mask.y = surfaceDescription.Occlusion;
                        surfaceData.MAOSBlend.x = surfaceDescription.MAOSAlpha * fadeFactor;
                        surfaceData.MAOSBlend.y = surfaceDescription.MAOSAlpha * fadeFactor;
                    #endif
                                                                  
                #endif
            }

			PackedVaryingsToPS Vert(AttributesMesh inputMesh /*ase_vert_input*/ )
			{
				PackedVaryingsToPS output;

				UNITY_SETUP_INSTANCE_ID(inputMesh);
				UNITY_TRANSFER_INSTANCE_ID(inputMesh, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
				/*ase_vert_code:inputMesh=AttributesMesh;output=PackedVaryingsToPS*/
				#ifdef ASE_ABSOLUTE_VERTEX_POS
				float3 defaultVertexValue = inputMesh.positionOS.xyz;
				#else
				float3 defaultVertexValue = float3( 0, 0, 0 );
				#endif
				float3 vertexValue = /*ase_vert_out:Vertex Offset;Float3;9;-1;_VertexOffset*/defaultVertexValue/*end*/;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
				inputMesh.positionOS.xyz = vertexValue;
				#else
				inputMesh.positionOS.xyz += vertexValue;
				#endif

				inputMesh.normalOS = /*ase_vert_out:Vertex Normal;Float3;10;-1;_VertexNormal*/inputMesh.normalOS/*end*/;
				inputMesh.tangentOS = /*ase_vert_out:Vertex Tangent;Float4;11;-1;_VertexTangent*/inputMesh.tangentOS/*end*/;

				float3 worldSpaceBias = 0.0f;
				
				if (_DecalMeshBiasType == DECALMESHDEPTHBIASTYPE_VIEW_BIAS)
				{
					float3 positionRWS = TransformObjectToWorld(inputMesh.positionOS);
					float3 V = GetWorldSpaceNormalizeViewDir(positionRWS);
					worldSpaceBias = V * (_DecalMeshViewBias);
				}
				
				float3 positionRWS = TransformObjectToWorld(inputMesh.positionOS) + worldSpaceBias;
				float3 normalWS = TransformObjectToWorldNormal(inputMesh.normalOS);
				float4 tangentWS = float4(TransformObjectToWorldDir(inputMesh.tangentOS.xyz), inputMesh.tangentOS.w);

				output.positionRWS.xyz = positionRWS;
				output.positionCS = TransformWorldToHClip(positionRWS);
				output.normalWS.xyz = normalWS;
				output.tangentWS.xyzw = tangentWS;
				output.uv0.xyzw = inputMesh.uv0;

				if (_DecalMeshBiasType == DECALMESHDEPTHBIASTYPE_DEPTH_BIAS)
				{
					#if UNITY_REVERSED_Z
						output.positionCS.z -= _DecalMeshDepthBias;
					#else
						output.positionCS.z += _DecalMeshDepthBias;
					#endif
				}


				return output;
			}

			void Frag(  PackedVaryingsToPS packedInput,
			#if (SHADERPASS == SHADERPASS_DBUFFER_PROJECTOR) || (SHADERPASS == SHADERPASS_DBUFFER_MESH)
				OUTPUT_DBUFFER(outDBuffer)
			#else
				out float4 outEmissive : SV_Target0
			#endif
			/*ase_frag_input*/
			)
			{
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(packedInput);
				UNITY_SETUP_INSTANCE_ID(packedInput);

                FragInputs input;
                ZERO_INITIALIZE(FragInputs, input);
                
                input.tangentToWorld = k_identity3x3;
                input.positionSS = packedInput.positionCS;
                
                input.positionRWS = packedInput.positionRWS.xyz;
				/*ase_local_var:rwp*/float3 positionRWS = input.positionRWS;

                input.tangentToWorld = BuildTangentToWorld(packedInput.tangentWS.xyzw, packedInput.normalWS.xyz);
                input.texCoord0 = packedInput.uv0.xyzw;

				DecalSurfaceData surfaceData;
				float clipValue = 1.0;
				float angleFadeFactor = 1.0;

			#if (SHADERPASS == SHADERPASS_DBUFFER_PROJECTOR) || (SHADERPASS == SHADERPASS_FORWARD_EMISSIVE_PROJECTOR)    

				float depth = LoadCameraDepth(input.positionSS.xy);
				PositionInputs posInput = GetPositionInput(input.positionSS.xy, _ScreenSize.zw, depth, UNITY_MATRIX_I_VP, UNITY_MATRIX_V);

				DecalPrepassData material;
				ZERO_INITIALIZE(DecalPrepassData, material);
				if (_EnableDecalLayers)
				{
					uint decalLayerMask = uint(UNITY_ACCESS_INSTANCED_PROP(Decal, _DecalLayerMaskFromDecal).x);

					DecodeFromDecalPrepass(posInput.positionSS, material);

					if ((decalLayerMask & material.decalLayerMask) == 0)
						clipValue -= 2.0;
				}

				float3 positionDS = TransformWorldToObject(posInput.positionWS);
				positionDS = positionDS * float3(1.0, -1.0, 1.0) + float3(0.5, 0.5, 0.5);
				if (!(all(positionDS.xyz > 0.0f) && all(1.0f - positionDS.xyz > 0.0f)))
				{
					clipValue -= 2.0;
				}

			#ifndef SHADER_API_METAL
				clip(clipValue);
			#else
				if (clipValue > 0.0)
				{
			#endif
				input.texCoord0.xy = positionDS.xz;
				input.texCoord1.xy = positionDS.xz;
				input.texCoord2.xy = positionDS.xz;
				input.texCoord3.xy = positionDS.xz;

				float3 V = GetWorldSpaceNormalizeViewDir(posInput.positionWS);

				if (_EnableDecalLayers)
				{
					float4x4 normalToWorld = UNITY_ACCESS_INSTANCED_PROP(Decal, _NormalToWorld);
					float2 angleFade = float2(normalToWorld[1][3], normalToWorld[2][3]);

					if (angleFade.y < 0.0f) 
					{
						float3 decalNormal = float3(normalToWorld[0].z, normalToWorld[1].z, normalToWorld[2].z);
						float dotAngle = dot(material.geomNormalWS, decalNormal);
						angleFadeFactor = saturate(angleFade.x + angleFade.y * (dotAngle * (dotAngle - 2.0)));
					}
				}

			#else
				PositionInputs posInput = GetPositionInput(input.positionSS.xy, _ScreenSize.zw, input.positionSS.z, input.positionSS.w, input.positionRWS.xyz, uint2(0, 0));
				float3 V = GetWorldSpaceNormalizeViewDir(input.positionRWS);
			#endif

				/*ase_local_var:uv0*/float4 texCoord0 = input.texCoord0;
				/*ase_local_var:uv1*/float4 texCoord1 = input.texCoord1;
				/*ase_local_var:uv2*/float4 texCoord2 = input.texCoord2;
				/*ase_local_var:uv3*/float4 texCoord3 = input.texCoord3;

				SurfaceDescription surfaceDescription = (SurfaceDescription)0;
				/*ase_frag_code:packedInput=PackedVaryingsToPS*/
				surfaceDescription.BaseColor = /*ase_frag_out:Albedo;Float3;0;-1;_Albedo*/float3( 0.7353569, 0.7353569, 0.7353569 )/*end*/;
				surfaceDescription.Alpha = /*ase_frag_out:Alpha Albedo;Float;1;-1;_AlphaAlbedo*/1/*end*/;
				surfaceDescription.NormalTS = /*ase_frag_out:Normal;Float3;2;-1;_Normal*/float3( 0, 0, 1 )/*end*/;
				surfaceDescription.NormalAlpha = /*ase_frag_out:Alpha Normal;Float;3;-1;_AlphaNormal*/1/*end*/;
				surfaceDescription.Metallic = /*ase_frag_out:Metallic;Float;4;-1;_Metallic*/0/*end*/;
				surfaceDescription.Occlusion = /*ase_frag_out:Occlusion;Float;5;-1;_Occlusion*/1/*end*/;
				surfaceDescription.Smoothness = /*ase_frag_out:Smoothness;Float;6;-1;_Smoothness*/0.5/*end*/;
				surfaceDescription.MAOSAlpha = /*ase_frag_out:MAOSOpacity;Float;7;-1;_MAOSOpacity*/1/*end*/;
				surfaceDescription.Emission = /*ase_frag_out:Emission;Float3;8;-1;_Emission*/float3( 0, 0, 0 )/*end*/;

				GetSurfaceData(surfaceDescription, input, V, posInput, angleFadeFactor, surfaceData);

			#if ((SHADERPASS == SHADERPASS_DBUFFER_PROJECTOR) || (SHADERPASS == SHADERPASS_FORWARD_EMISSIVE_PROJECTOR)) && defined(SHADER_API_METAL)
				} 

				clip(clipValue);
			#endif

			#if (SHADERPASS == SHADERPASS_DBUFFER_PROJECTOR) || (SHADERPASS == SHADERPASS_DBUFFER_MESH)
				ENCODE_INTO_DBUFFER(surfaceData, outDBuffer);
			#else
				outEmissive.rgb = surfaceData.emissive * GetCurrentExposureMultiplier();
				outEmissive.a = 1.0;
			#endif
			}
            ENDHLSL
        }

		/*ase_pass*/
        Pass
        { 
			/*ase_hide_pass*/
            Name "DecalMeshForwardEmissive"
            Tags 
            { 
                "LightMode" = "DecalMeshForwardEmissive"
            }
    
            Blend 0 SrcAlpha One
            ZTest LEqual
            ZWrite Off
    
            HLSLPROGRAM
    
            #pragma vertex Vert
            #pragma fragment Frag
            #pragma multi_compile_instancing
    
            
    
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/NormalSurfaceGradient.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"
    
            
            #define SHADERPASS SHADERPASS_FORWARD_EMISSIVE_MESH
			
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Packing.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/Functions.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/FragInputs.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Decal/Decal.hlsl"
			
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Decal/DecalPrepassBuffer.hlsl"

			/*ase_pragma*/

            struct AttributesMesh
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float4 uv0 : TEXCOORD0;
				/*ase_vdata:p=p;n=n;t=t;uv0=tc0*/
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

			struct PackedVaryingsToPS
			{
				float4 positionCS : SV_POSITION;
                float3 positionRWS : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float4 tangentWS : TEXCOORD2;
                float4 uv0 : TEXCOORD3;
				/*ase_interp(4,):sp=sp.xyzw;rwp=tc0;wn=tc1;wt=tc2;uv0=tc3*/
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			
            CBUFFER_START(UnityPerMaterial)
            float _DrawOrder;
			float _DecalMeshBiasType;
            float _DecalMeshDepthBias;
			float _DecalMeshViewBias;
            float _DecalStencilWriteMask;
            float _DecalStencilRef;
            #ifdef _MATERIAL_AFFECTS_ALBEDO
            float _AffectAlbedo;
			#endif
            #ifdef _MATERIAL_AFFECTS_NORMAL
            float _AffectNormal;
			#endif
            #ifdef _MATERIAL_AFFECTS_MASKMAP
            float _AffectAO;
			float _AffectMetal;
            float _AffectSmoothness;
			#endif
            #ifdef _MATERIAL_AFFECTS_EMISSION
            float _AffectEmission;
			#endif
            float _DecalColorMask0;
            float _DecalColorMask1;
            float _DecalColorMask2;
            float _DecalColorMask3;
            CBUFFER_END
       
	   		/*ase_globals*/

			/*ase_funcs*/

            void GetSurfaceData(SurfaceDescription surfaceDescription, FragInputs fragInputs, float3 V, PositionInputs posInput, float angleFadeFactor, out DecalSurfaceData surfaceData)
            {
                #if (SHADERPASS == SHADERPASS_DBUFFER_PROJECTOR) || (SHADERPASS == SHADERPASS_FORWARD_EMISSIVE_PROJECTOR)
                    float4x4 normalToWorld = UNITY_ACCESS_INSTANCED_PROP(Decal, _NormalToWorld);
                    float fadeFactor = clamp(normalToWorld[0][3], 0.0f, 1.0f) * angleFadeFactor;
                    float2 scale = float2(normalToWorld[3][0], normalToWorld[3][1]);
                    float2 offset = float2(normalToWorld[3][2], normalToWorld[3][3]);
                    fragInputs.texCoord0.xy = fragInputs.texCoord0.xy * scale + offset;
                    fragInputs.texCoord1.xy = fragInputs.texCoord1.xy * scale + offset;
                    fragInputs.texCoord2.xy = fragInputs.texCoord2.xy * scale + offset;
                    fragInputs.texCoord3.xy = fragInputs.texCoord3.xy * scale + offset;
                    fragInputs.positionRWS = posInput.positionWS;
                    fragInputs.tangentToWorld[2].xyz = TransformObjectToWorldDir(float3(0, 1, 0));
                    fragInputs.tangentToWorld[1].xyz = TransformObjectToWorldDir(float3(0, 0, 1));
                #else
                    #ifdef LOD_FADE_CROSSFADE
                    LODDitheringTransition(ComputeFadeMaskSeed(V, posInput.positionSS), unity_LODFade.x);
                    #endif
    
                    float fadeFactor = 1.0;
                #endif
    
                ZERO_INITIALIZE(DecalSurfaceData, surfaceData);
    
                #ifdef _MATERIAL_AFFECTS_EMISSION
                    surfaceData.emissive.rgb = surfaceDescription.Emission.rgb * fadeFactor;
                #endif
    
                #ifdef _MATERIAL_AFFECTS_ALBEDO
                    surfaceData.baseColor.xyz = surfaceDescription.BaseColor;
                    surfaceData.baseColor.w = surfaceDescription.Alpha * fadeFactor;
                #endif
    
        
                #ifdef _MATERIAL_AFFECTS_NORMAL
                    #ifdef DECAL_SURFACE_GRADIENT
                        #if (SHADERPASS == SHADERPASS_DBUFFER_PROJECTOR) || (SHADERPASS == SHADERPASS_FORWARD_EMISSIVE_PROJECTOR)
                            float3x3 tangentToWorld = transpose((float3x3)normalToWorld);
                        #else
                            float3x3 tangentToWorld = fragInputs.tangentToWorld;
                        #endif
        
                        surfaceData.normalWS.xyz = SurfaceGradientFromTangentSpaceNormalAndFromTBN(surfaceDescription.NormalTS.xyz, tangentToWorld[0], tangentToWorld[1]);
                    #else
                        #if (SHADERPASS == SHADERPASS_DBUFFER_PROJECTOR)
                            surfaceData.normalWS.xyz = mul((float3x3)normalToWorld, surfaceDescription.NormalTS);
                        #elif (SHADERPASS == SHADERPASS_DBUFFER_MESH) || (SHADERPASS == SHADERPASS_FORWARD_PREVIEW)
                            
                            surfaceData.normalWS.xyz = normalize(TransformTangentToWorld(surfaceDescription.NormalTS, fragInputs.tangentToWorld));
                        #endif
                    #endif
        
                    surfaceData.normalWS.w = surfaceDescription.NormalAlpha * fadeFactor;
                #else
                    #if (SHADERPASS == SHADERPASS_FORWARD_PREVIEW) 
                        #ifdef DECAL_SURFACE_GRADIENT
                            surfaceData.normalWS.xyz = float3(0.0, 0.0, 0.0);
                        #else
                            surfaceData.normalWS.xyz = normalize(TransformTangentToWorld(float3(0.0, 0.0, 0.1), fragInputs.tangentToWorld));
                        #endif
                    #endif
                #endif
    
                #ifdef _MATERIAL_AFFECTS_MASKMAP
                    surfaceData.mask.z = surfaceDescription.Smoothness;
                    surfaceData.mask.w = surfaceDescription.MAOSAlpha * fadeFactor;
    
                    #ifdef DECALS_4RT
                        surfaceData.mask.x = surfaceDescription.Metallic;
                        surfaceData.mask.y = surfaceDescription.Occlusion;
                        surfaceData.MAOSBlend.x = surfaceDescription.MAOSAlpha * fadeFactor;
                        surfaceData.MAOSBlend.y = surfaceDescription.MAOSAlpha * fadeFactor;
                    #endif
                                                                  
                #endif
            }

			PackedVaryingsToPS Vert(AttributesMesh inputMesh /*ase_vert_input*/ )
			{
				PackedVaryingsToPS output;

				UNITY_SETUP_INSTANCE_ID(inputMesh);
				UNITY_TRANSFER_INSTANCE_ID(inputMesh, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
				/*ase_vert_code:inputMesh=AttributesMesh;output=PackedVaryingsToPS*/
				#ifdef ASE_ABSOLUTE_VERTEX_POS
				float3 defaultVertexValue = inputMesh.positionOS.xyz;
				#else
				float3 defaultVertexValue = float3( 0, 0, 0 );
				#endif
				float3 vertexValue = /*ase_vert_out:Vertex Offset;Float3;9;-1;_VertexOffset*/defaultVertexValue/*end*/;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
				inputMesh.positionOS.xyz = vertexValue;
				#else
				inputMesh.positionOS.xyz += vertexValue;
				#endif

				inputMesh.normalOS = /*ase_vert_out:Vertex Normal;Float3;10;-1;_VertexNormal*/inputMesh.normalOS/*end*/;
				inputMesh.tangentOS = /*ase_vert_out:Vertex Tangent;Float4;11;-1;_VertexTangent*/inputMesh.tangentOS/*end*/;

				float3 positionRWS = TransformObjectToWorld(inputMesh.positionOS);
				float3 normalWS = TransformObjectToWorldNormal(inputMesh.normalOS);
				float4 tangentWS = float4(TransformObjectToWorldDir(inputMesh.tangentOS.xyz), inputMesh.tangentOS.w);

				output.positionRWS.xyz = positionRWS;
				output.positionCS = TransformWorldToHClip(positionRWS);
				output.normalWS.xyz = normalWS;
				output.tangentWS.xyzw = tangentWS;
				output.uv0.xyzw = inputMesh.uv0;

				return output;
			}

			void Frag(  PackedVaryingsToPS packedInput,
			#if (SHADERPASS == SHADERPASS_DBUFFER_PROJECTOR) || (SHADERPASS == SHADERPASS_DBUFFER_MESH)
				OUTPUT_DBUFFER(outDBuffer)
			#else
				out float4 outEmissive : SV_Target0
			#endif
			/*ase_frag_input*/
			)
			{
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(packedInput);
				UNITY_SETUP_INSTANCE_ID(packedInput);

                FragInputs input;
                ZERO_INITIALIZE(FragInputs, input);
                
                input.tangentToWorld = k_identity3x3;
                input.positionSS = packedInput.positionCS;
                
                input.positionRWS = packedInput.positionRWS.xyz;
				/*ase_local_var:rwp*/float3 positionRWS = input.positionRWS;

                input.tangentToWorld = BuildTangentToWorld(packedInput.tangentWS.xyzw, packedInput.normalWS.xyz);
                input.texCoord0 = packedInput.uv0.xyzw;

				DecalSurfaceData surfaceData;
				float clipValue = 1.0;
				float angleFadeFactor = 1.0;

			#if (SHADERPASS == SHADERPASS_DBUFFER_PROJECTOR) || (SHADERPASS == SHADERPASS_FORWARD_EMISSIVE_PROJECTOR)    

				float depth = LoadCameraDepth(input.positionSS.xy);
				PositionInputs posInput = GetPositionInput(input.positionSS.xy, _ScreenSize.zw, depth, UNITY_MATRIX_I_VP, UNITY_MATRIX_V);

				DecalPrepassData material;
				ZERO_INITIALIZE(DecalPrepassData, material);
				if (_EnableDecalLayers)
				{
					uint decalLayerMask = uint(UNITY_ACCESS_INSTANCED_PROP(Decal, _DecalLayerMaskFromDecal).x);

					DecodeFromDecalPrepass(posInput.positionSS, material);

					if ((decalLayerMask & material.decalLayerMask) == 0)
						clipValue -= 2.0;
				}

				float3 positionDS = TransformWorldToObject(posInput.positionWS);
				positionDS = positionDS * float3(1.0, -1.0, 1.0) + float3(0.5, 0.5, 0.5);
				if (!(all(positionDS.xyz > 0.0f) && all(1.0f - positionDS.xyz > 0.0f)))
				{
					clipValue -= 2.0;
				}

			#ifndef SHADER_API_METAL
				clip(clipValue);
			#else
				if (clipValue > 0.0)
				{
			#endif
				input.texCoord0.xy = positionDS.xz;
				input.texCoord1.xy = positionDS.xz;
				input.texCoord2.xy = positionDS.xz;
				input.texCoord3.xy = positionDS.xz;

				float3 V = GetWorldSpaceNormalizeViewDir(posInput.positionWS);

				if (_EnableDecalLayers)
				{
					float4x4 normalToWorld = UNITY_ACCESS_INSTANCED_PROP(Decal, _NormalToWorld);
					float2 angleFade = float2(normalToWorld[1][3], normalToWorld[2][3]);

					if (angleFade.y < 0.0f) 
					{
						float3 decalNormal = float3(normalToWorld[0].z, normalToWorld[1].z, normalToWorld[2].z);
						float dotAngle = dot(material.geomNormalWS, decalNormal);
						
						angleFadeFactor = saturate(angleFade.x + angleFade.y * (dotAngle * (dotAngle - 2.0)));
					}
				}

			#else
				PositionInputs posInput = GetPositionInput(input.positionSS.xy, _ScreenSize.zw, input.positionSS.z, input.positionSS.w, input.positionRWS.xyz, uint2(0, 0));
				float3 V = GetWorldSpaceNormalizeViewDir(input.positionRWS);
			#endif

				/*ase_local_var:uv0*/float4 texCoord0 = input.texCoord0;
				/*ase_local_var:uv1*/float4 texCoord1 = input.texCoord1;
				/*ase_local_var:uv2*/float4 texCoord2 = input.texCoord2;
				/*ase_local_var:uv3*/float4 texCoord3 = input.texCoord3;

				SurfaceDescription surfaceDescription = (SurfaceDescription)0;
				/*ase_frag_code:packedInput=PackedVaryingsToPS*/
				surfaceDescription.BaseColor = /*ase_frag_out:Albedo;Float3;0;-1;_Albedo*/float3( 0.7353569, 0.7353569, 0.7353569 )/*end*/;
				surfaceDescription.Alpha = /*ase_frag_out:Alpha Albedo;Float;1;-1;_AlphaAlbedo*/1/*end*/;
				surfaceDescription.NormalTS = /*ase_frag_out:Normal;Float3;2;-1;_Normal*/float3( 0, 0, 1 )/*end*/;
				surfaceDescription.NormalAlpha = /*ase_frag_out:Alpha Normal;Float;3;-1;_AlphaNormal*/1/*end*/;
				surfaceDescription.Metallic = /*ase_frag_out:Metallic;Float;4;-1;_Metallic*/0/*end*/;
				surfaceDescription.Occlusion = /*ase_frag_out:Occlusion;Float;5;-1;_Occlusion*/1/*end*/;
				surfaceDescription.Smoothness = /*ase_frag_out:Smoothness;Float;6;-1;_Smoothness*/0.5/*end*/;
				surfaceDescription.MAOSAlpha = /*ase_frag_out:MAOSOpacity;Float;7;-1;_MAOSOpacity*/1/*end*/;
				surfaceDescription.Emission = /*ase_frag_out:Emission;Float3;8;-1;_Emission*/float3( 0, 0, 0 )/*end*/;

				GetSurfaceData(surfaceDescription, input, V, posInput, angleFadeFactor, surfaceData);

			#if ((SHADERPASS == SHADERPASS_DBUFFER_PROJECTOR) || (SHADERPASS == SHADERPASS_FORWARD_EMISSIVE_PROJECTOR)) && defined(SHADER_API_METAL)
				} 

				clip(clipValue);
			#endif

			#if (SHADERPASS == SHADERPASS_DBUFFER_PROJECTOR) || (SHADERPASS == SHADERPASS_DBUFFER_MESH)
				ENCODE_INTO_DBUFFER(surfaceData, outDBuffer);
			#else
				outEmissive.rgb = surfaceData.emissive * GetCurrentExposureMultiplier();
				outEmissive.a = 1.0;
			#endif
			}
            ENDHLSL
        }
				
		/*ase_pass*/
        Pass
        { 
			/*ase_hide_pass*/
            Name "ScenePickingPass"
            Tags 
            { 
                "LightMode" = "Picking"
            }
        
            Cull Back
        
            HLSLPROGRAM
        
            
            #pragma target 4.5
			#pragma vertex Vert
			#pragma fragment Frag
			#pragma only_renderers d3d11 playstation xboxone xboxseries vulkan metal switch
			#pragma multi_compile_instancing
        
            
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/NormalSurfaceGradient.hlsl"
            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPass.cs.hlsl"
        
            
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT
        
            #define SHADERPASS SHADERPASS_DEPTH_ONLY
			#define SCENEPICKINGPASS 1
            
            float4 _SelectionID;

            
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Packing.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.shadergraph/ShaderGraphLibrary/Functions.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/FragInputs.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/PickingSpaceTransforms.hlsl"
			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Decal/Decal.hlsl"
        
			/*ase_pragma*/    
        
            struct AttributesMesh
			{
				float3 positionOS : POSITION;
				/*ase_vdata:p=p*/
                UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct PackedVaryingsToPS
			{
				float4 positionCS : SV_POSITION;
				/*ase_interp(0,):sp=sp.xyzw*/
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};


            CBUFFER_START(UnityPerMaterial)
            float _DrawOrder;
			float _DecalMeshBiasType;
            float _DecalMeshDepthBias;
			float _DecalMeshViewBias;
            float _DecalStencilWriteMask;
            float _DecalStencilRef;
            #ifdef _MATERIAL_AFFECTS_ALBEDO
            float _AffectAlbedo;
			#endif
            #ifdef _MATERIAL_AFFECTS_NORMAL
            float _AffectNormal;
			#endif
            #ifdef _MATERIAL_AFFECTS_MASKMAP
            float _AffectAO;
			float _AffectMetal;
            float _AffectSmoothness;
			#endif
            #ifdef _MATERIAL_AFFECTS_EMISSION
            float _AffectEmission;
			#endif
            float _DecalColorMask0;
            float _DecalColorMask1;
            float _DecalColorMask2;
            float _DecalColorMask3;
            CBUFFER_END
       
	   		/*ase_globals*/

			/*ase_funcs*/

			#include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Decal/DecalPrepassBuffer.hlsl"

			PackedVaryingsToPS Vert(AttributesMesh inputMesh /*ase_vert_input*/)
			{
				PackedVaryingsToPS output;

				UNITY_SETUP_INSTANCE_ID(inputMesh);
				UNITY_TRANSFER_INSTANCE_ID(inputMesh, output);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				/*ase_vert_code:inputMesh=AttributesMesh;output=PackedVaryingsToPS*/
				#ifdef ASE_ABSOLUTE_VERTEX_POS
				float3 defaultVertexValue = inputMesh.positionOS.xyz;
				#else
				float3 defaultVertexValue = float3( 0, 0, 0 );
				#endif
				float3 vertexValue = /*ase_vert_out:Vertex Offset;Float3;1;-1;_VertexOffset*/defaultVertexValue/*end*/;
				#ifdef ASE_ABSOLUTE_VERTEX_POS
				inputMesh.positionOS.xyz = vertexValue;
				#else
				inputMesh.positionOS.xyz += vertexValue;
				#endif

				float3 positionRWS = TransformObjectToWorld(inputMesh.positionOS) ;
				output.positionCS = TransformWorldToHClip(positionRWS);
			
				return output;
			}

			void Frag(  PackedVaryingsToPS packedInput,
						out float4 outColor : SV_Target0
						/*ase_frag_input*/
						)
			{
				/*ase_frag_code:packedInput=PackedVaryingsToPS*/

				//This port is needed as templates always require fragment ports to correctly work...this will be discarded by the compiler
				float3 baseColor = /*ase_frag_out:Albedo;Float3;0;-1;_Albedo*/float3( 0,0,0)/*end*/;
				outColor = _SelectionID;
			}

            ENDHLSL
        }
		/*ase_pass_end*/
    }
    CustomEditor "Rendering.HighDefinition.DecalShaderGraphGUI"
    FallBack "Hidden/Shader Graph/FallbackError"
}
