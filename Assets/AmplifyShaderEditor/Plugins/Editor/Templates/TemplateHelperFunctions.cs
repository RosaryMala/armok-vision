// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace AmplifyShaderEditor
{
	public enum CustomTemplatePropertyUIEnum
	{
		None,
		HDPBR
	}

	public enum TemplateSemantics
	{
		NONE,
		POSITION,
		SV_POSITION,
		COLOR,
		COLOR0,
		COLOR1,
		TEXCOORD0,
		TEXCOORD1,
		TEXCOORD2,
		TEXCOORD3,
		TEXCOORD4,
		TEXCOORD5,
		TEXCOORD6,
		TEXCOORD7,
		TEXCOORD8,
		TEXCOORD9,
		TEXCOORD10,
		TEXCOORD11,
		TEXCOORD12,
		TEXCOORD13,
		TEXCOORD14,
		TEXCOORD15,
		NORMAL,
		TANGENT,
		VFACE,
		SV_VertexID,
		SV_PrimitiveID,
		SV_InstanceID,
		INTERNALTESSPOS,
		INSTANCEID_SEMANTIC,
		BLENDWEIGHTS,
		BLENDINDICES

	}

	public enum TemplateInfoOnSematics
	{
		NONE,
		POSITION,
		CLIP_POS,
		SCREEN_POSITION,
		SCREEN_POSITION_NORMALIZED,
		COLOR,
		TEXTURE_COORDINATES0,
		TEXTURE_COORDINATES1,
		TEXTURE_COORDINATES2,
		TEXTURE_COORDINATES3,
		TEXTURE_COORDINATES4,
		TEXTURE_COORDINATES5,
		TEXTURE_COORDINATES6,
		TEXTURE_COORDINATES7,
		NORMAL,
		TANGENT,
		WORLD_NORMAL,
		WORLD_TANGENT,
		WORLD_BITANGENT,
		WORLD_VIEW_DIR,
		WORLD_POSITION,
		RELATIVE_WORLD_POS,
		INSTANCE_ID,
		OTHER,
		VFACE,
		SHADOWCOORDS,
		VERTEXID,
		BLENDWEIGHTS,
		BLENDINDICES
	}

	public enum TemplateShaderPropertiesIdx
	{
		Identation = 1,
		Name = 3,
		InspectorName,
		Type
	}

	public enum TemplateShaderGlobalsIdx
	{
		Type = 1,
		Name = 2
	}
	public enum TemplateDataCheck
	{
		Valid,
		Invalid
	}

	public enum InvisibleOptionsEnum
	{
		SyncProperties = 1 << 0
	}

	public enum TemplateSpecialTags
	{
		RenderType,
		Queue,
		DisableBatching,
		None
	}

	public class TemplateReplaceHelper
	{
		public TemplateMultiPassMasterNode MasterNode = null;
		public bool Used = false;
		public TemplateReplaceHelper( TemplateMultiPassMasterNode masterNode ) { MasterNode = masterNode; }
	}

	[Serializable]
	public class TemplatesTagData
	{
		public string Name;
		public string Value;
		public TemplatesTagData( string name, string value )
		{
			Name = name;
			Value = value;
		}
	}

	[Serializable]
	public class TemplateModuleData
	{
		public bool IndependentModule = true;
		public TemplateDataCheck DataCheck = TemplateDataCheck.Invalid;
		public string InlineData = string.Empty;
		public int StartIdx;
		public bool IsValid { get { return DataCheck == TemplateDataCheck.Valid; } }
		public virtual void SetAllModulesDefault() { IndependentModule = false; DataCheck = TemplateDataCheck.Valid; }
	}

	[Serializable]
	public sealed class TemplateTagsModuleData : TemplateModuleData
	{
		public string TagsId;
		public List<TemplatesTagData> Tags = new List<TemplatesTagData>();
		public void Destroy()
		{
			Tags.Clear();
			Tags = null;
		}

		public void Reset()
		{
			Tags.Clear();
		}

		public void Dump()
		{
			string dump = string.Empty;
			for( int i = 0; i < Tags.Count; i++ )
			{
				dump += string.Format( "[{0}] Name: {1} Value: {2}\n", i, Tags[ i ].Name, Tags[ i ].Value );
			}
			Debug.Log( dump );
		}
	}

	[Serializable]
	public class TemplateShaderModelData : TemplateModuleData
	{
		public string Id = string.Empty;
		public string Value = "2.5";
		public int InterpolatorAmount = 8;
		public bool Encapsulate = false;
		public override void SetAllModulesDefault()
		{
			base.SetAllModulesDefault();
			Id = string.Empty;
			Value = "3.0";
			InterpolatorAmount = 10;
			Encapsulate = true;
		}
	}

	[Serializable]
	public sealed class TemplateDepthData : TemplateModuleData
	{
		public bool ValidZWrite;
		public string ZWriteModeId;
		public ZWriteMode ZWriteModeValue;
		public int ZWriteStartIndex;
		public string ZWriteInlineValue;


		public bool ValidZTest;
		public string ZTestModeId;
		public ZTestMode ZTestModeValue;
		public int ZTestStartIndex;
		public string ZTestInlineValue;

		public bool ValidOffset;
		public string OffsetId;
		public float OffsetFactor;
		public float OffsetUnits;
		public int OffsetStartIndex;
		public string OffsetFactorInlineValue;
		public string OffsetUnitsInlineValue;

		public override void SetAllModulesDefault()
		{
			base.SetAllModulesDefault();
			ValidZWrite = true;
			ZWriteModeId = string.Empty;
			ZWriteModeValue = ZWriteMode.On;
			ZWriteStartIndex = -1;
			ZWriteInlineValue = string.Empty;


			ValidZTest = true;
			ZTestModeId = string.Empty;
			ZTestModeValue = ZTestMode.LEqual;
			ZTestStartIndex = -1;
			ZTestInlineValue = string.Empty;

			ValidOffset = true;
			OffsetId = string.Empty;
			OffsetFactor = 0;
			OffsetUnits = 0;
			OffsetStartIndex = -1;
			OffsetFactorInlineValue = string.Empty;
			OffsetUnitsInlineValue = string.Empty;
		}

		public void SetDataCheck()
		{
			DataCheck = ( ValidZWrite || ValidZTest || ValidOffset )?TemplateDataCheck.Valid:TemplateDataCheck.Invalid;
		}
	}

	[Serializable]
	public sealed class TemplateStencilData : TemplateModuleData
	{
		public string StencilBufferId;
		public bool Active = true;

		public int Reference;
		public string ReferenceInline;

		public int ReadMask = 255;
		public string ReadMaskInline;

		public int WriteMask = 255;
		public string WriteMaskInline;

		public string ComparisonFront;
		public string ComparisonFrontInline;

		public string PassFront;
		public string PassFrontInline;

		public string FailFront;
		public string FailFrontInline;

		public string ZFailFront;
		public string ZFailFrontInline;

		public string ComparisonBack;
		public string ComparisonBackInline;

		public string PassBack;
		public string PassBackInline;

		public string FailBack;
		public string FailBackInline;

		public string ZFailBack;
		public string ZFailBackInline;

		public void SetDefaultValues()
		{
			Active = false;

			StencilBufferId = string.Empty;

			Reference = 255;
			ReferenceInline = string.Empty;

			ReadMask = 255;
			ReadMaskInline = string.Empty;

			WriteMask = 255;
			WriteMaskInline = string.Empty;

			ComparisonFront = "always";
			ComparisonFrontInline = string.Empty;

			PassFront = "keep";
			PassFrontInline = string.Empty;

			FailFront = "keep";
			FailFrontInline = string.Empty;

			ZFailFront = "keep";
			ZFailFrontInline = string.Empty;


			ComparisonBack = "always";
			ComparisonBackInline = string.Empty;

			PassBack = "keep";
			PassBackInline = string.Empty;

			FailBack = "keep";
			FailBackInline = string.Empty;

			ZFailBack = "keep";
			ZFailBackInline = string.Empty;
		}

		public void SetIndependentDefault()
		{
			IndependentModule = true;
			DataCheck = TemplateDataCheck.Valid;
			SetDefaultValues();
		}

		public override void SetAllModulesDefault()
		{
			base.SetAllModulesDefault();
			SetDefaultValues();
		}
	}

	[Serializable]
	public sealed class TemplateBlendData : TemplateModuleData
	{
		public bool ValidBlendMode = false;
		public bool BlendModeOff = true;

		public string Target = string.Empty;

		public string BlendModeId;
		public bool SeparateBlendFactors = false;
		public AvailableBlendFactor SourceFactorRGB = AvailableBlendFactor.One;
		public string SourceFactorRGBInline;
		public AvailableBlendFactor DestFactorRGB = AvailableBlendFactor.Zero;
		public string DestFactorRGBInline;
		public int BlendModeStartIndex;

		public AvailableBlendFactor SourceFactorAlpha = AvailableBlendFactor.One;
		public string SourceFactorAlphaInline;
		public AvailableBlendFactor DestFactorAlpha = AvailableBlendFactor.Zero;
		public string DestFactorAlphaInline;

		public bool ValidBlendOp = false;
		public string BlendOpId;
		public bool SeparateBlendOps = false;
		public AvailableBlendOps BlendOpRGB = AvailableBlendOps.OFF;
		public string BlendOpRGBInline;
		public AvailableBlendOps BlendOpAlpha = AvailableBlendOps.OFF;
		public string BlendOpAlphaInline;
		public int BlendOpStartIndex;


		public override void SetAllModulesDefault()
		{
			base.SetAllModulesDefault();


			if( !ValidBlendMode )
			{
				ValidBlendMode = true;
				BlendModeOff = true;
				BlendModeId = string.Empty;
				SeparateBlendFactors = false;
				SourceFactorRGB = AvailableBlendFactor.One;
				SourceFactorRGBInline = string.Empty;
				DestFactorRGB = AvailableBlendFactor.Zero;
				DestFactorRGBInline = string.Empty;
				BlendModeStartIndex = -1;
				SourceFactorAlpha = AvailableBlendFactor.One;
				SourceFactorAlphaInline = string.Empty;
				DestFactorAlpha = AvailableBlendFactor.Zero;
				DestFactorAlphaInline = string.Empty;
			}

			if( !ValidBlendOp )
			{
				ValidBlendOp = true;
				BlendOpId = string.Empty;
				SeparateBlendOps = false;
				BlendOpRGB = AvailableBlendOps.OFF;
				BlendOpRGBInline = string.Empty;
				BlendOpAlpha = AvailableBlendOps.OFF;
				BlendOpAlphaInline = string.Empty;
				BlendOpStartIndex = -1;
			}

			DataCheck = TemplateDataCheck.Valid;
		}

	}

	[Serializable]
	public sealed class TemplateAlphaToMaskData : TemplateModuleData
	{
		public string AlphaToMaskId;
		public bool AlphaToMaskData = false;
		public override void SetAllModulesDefault()
		{
			base.SetAllModulesDefault();
			AlphaToMaskId = string.Empty;
			AlphaToMaskData = false;
		}
	}

	[Serializable]
	public sealed class TemplateCullModeData : TemplateModuleData
	{
		public string CullModeId;
		public CullMode CullModeData = CullMode.Back;
		public override void SetAllModulesDefault()
		{
			base.SetAllModulesDefault();
			CullModeId = string.Empty;
			CullModeData = CullMode.Back;
		}
	}

	[Serializable]
	public sealed class TemplateColorMaskData : TemplateModuleData
	{
		public string ColorMaskId;
		public bool[] ColorMaskData = { true, true, true, true };
		public string Target = string.Empty;
		public override void SetAllModulesDefault()
		{
			base.SetAllModulesDefault();
			ColorMaskId = string.Empty;
			Target = string.Empty;
			for( int i = 0; i < ColorMaskData.Length; i++ )
			{
				ColorMaskData[ i ] = true;
			}
		}
	}

	public static class TemplateHelperFunctions
	{
		/*
		struct DirectionalLightData
		{
			uint lightLayers;
			float3 positionRWS;
			float3 color;
			int cookieIndex;
			float volumetricDimmer;
			float3 right;
			float3 up;
			float3 forward;
			int tileCookie;
			int shadowIndex;
			int contactShadowIndex;
			float4 shadowMaskSelector;
			int nonLightmappedOnly;
			float diffuseScale;
			float specularScale;
		}; 
		*/
		public static string HDLightInfoFormat = "_DirectionalLightDatas[{0}].{1}";

		public static string[] VectorSwizzle = { "x", "y", "z", "w" };
		public static string[] ColorSwizzle = { "r", "g", "b", "a" };

		public static readonly Dictionary<string, CustomTemplatePropertyUIEnum> CustomTemplatePropertyUI = new Dictionary<string, CustomTemplatePropertyUIEnum>
		{
			{ "None", CustomTemplatePropertyUIEnum.None},
			{ "HDPBR", CustomTemplatePropertyUIEnum.HDPBR}
		};

		public static readonly Dictionary<string, InvisibleOptionsEnum> InvisibleOptions = new Dictionary<string, InvisibleOptionsEnum>()
		{
			{ "SyncP", InvisibleOptionsEnum.SyncProperties }
		};

		public static readonly Dictionary<string, TemplateSpecialTags> StringToReservedTags = new Dictionary<string, TemplateSpecialTags>()
		{
			{ TemplateSpecialTags.RenderType.ToString(), TemplateSpecialTags.RenderType},
			{ TemplateSpecialTags.Queue.ToString(), TemplateSpecialTags.Queue},
			{ TemplateSpecialTags.DisableBatching.ToString(), TemplateSpecialTags.DisableBatching},
		};

		public static readonly Dictionary<string, DisableBatching> StringToDisableBatching = new Dictionary<string, DisableBatching>
		{
			{"true",DisableBatching.True},
			{"True",DisableBatching.True},
			{"false",DisableBatching.False},
			{"False",DisableBatching.False},
			{"LOD Fading",DisableBatching.LODFading},
			{"LODFading",DisableBatching.LODFading}
		};

		public static readonly Dictionary<string, RenderType> StringToRenderType = new Dictionary<string, RenderType>
		{
			{"Opaque",RenderType.Opaque},
			{"Transparent",RenderType.Transparent},
			{"TransparentCutout",RenderType.TransparentCutout},
			{"Background",RenderType.Background},
			{"Overlay",RenderType.Overlay},
			{"TreeOpaque",RenderType.TreeOpaque},
			{"TreeTransparentCutout",RenderType.TreeTransparentCutout},
			{"TreeBillboard",RenderType.TreeBillboard},
			{"Grass",RenderType.Grass},
			{"GrassBillboard",RenderType.GrassBillboard}
		};

		public static readonly Dictionary<string, RenderQueue> StringToRenderQueue = new Dictionary<string, RenderQueue>
		{
			{"Background",RenderQueue.Background },
			{"Geometry",RenderQueue.Geometry },
			{"AlphaTest",RenderQueue.AlphaTest },
			{"Transparent",RenderQueue.Transparent },
			{"Overlay",RenderQueue.Overlay }
		};

		public static readonly Dictionary<string, WirePortDataType> PropertyToWireType = new Dictionary<string, WirePortDataType>
		{
			{"Float",WirePortDataType.FLOAT},
			{"Range",WirePortDataType.FLOAT},
			{"Int",WirePortDataType.INT},
			{"Color",WirePortDataType.COLOR},
			{"Vector",WirePortDataType.FLOAT4},
			{"2D",WirePortDataType.SAMPLER2D},
			{"3D",WirePortDataType.SAMPLER3D},
			{"Cube",WirePortDataType.SAMPLERCUBE},
			{"2DArray",WirePortDataType.SAMPLER2DARRAY},
		};

		public static readonly Dictionary<WirePortDataType, int> DataTypeChannelUsage = new Dictionary<WirePortDataType, int>
		{
			{WirePortDataType.OBJECT,0 },
			{WirePortDataType.FLOAT,1 },
			{WirePortDataType.FLOAT2,2 },
			{WirePortDataType.FLOAT3,3 },
			{WirePortDataType.FLOAT4,4 },
			{WirePortDataType.FLOAT3x3,0 },
			{WirePortDataType.FLOAT4x4,0 },
			{WirePortDataType.COLOR,4 },
			{WirePortDataType.INT,1 },
			{WirePortDataType.UINT,1 },
			{WirePortDataType.UINT4,4 },
			{WirePortDataType.SAMPLER1D,0 },
			{WirePortDataType.SAMPLER2D,0 },
			{WirePortDataType.SAMPLER3D,0 },
			{WirePortDataType.SAMPLERCUBE,0 },
			{WirePortDataType.SAMPLER2DARRAY,0 },
			{WirePortDataType.SAMPLERSTATE,0 }
		};

		public static readonly Dictionary<int, WirePortDataType> ChannelToDataType = new Dictionary<int, WirePortDataType>
		{
			{1,WirePortDataType.FLOAT},
			{2,WirePortDataType.FLOAT2},
			{3,WirePortDataType.FLOAT3},
			{4,WirePortDataType.FLOAT4}
		};

		public static readonly Dictionary<TemplateSemantics, string> SemanticsDefaultName = new Dictionary<TemplateSemantics, string>
		{
			{TemplateSemantics.COLOR			,"ase_color"},
			{TemplateSemantics.NORMAL			,"ase_normal"},
			{TemplateSemantics.POSITION			,"ase_position"},
			{TemplateSemantics.SV_POSITION		,"ase_sv_position"},
			{TemplateSemantics.TANGENT			,"ase_tangent"},
			{TemplateSemantics.VFACE			,"ase_vface"},
			{TemplateSemantics.SV_VertexID		,"ase_vertexId"},
			{TemplateSemantics.SV_InstanceID    ,"ase_instanceId"},
			{TemplateSemantics.SV_PrimitiveID   ,"ase_primitiveId"},
			{TemplateSemantics.INTERNALTESSPOS  ,"ase_internalTessPos"},
			{TemplateSemantics.TEXCOORD0		,"ase_tex_coord0"},
			{TemplateSemantics.TEXCOORD1		,"ase_tex_coord1"},
			{TemplateSemantics.TEXCOORD2		,"ase_tex_coord2"},
			{TemplateSemantics.TEXCOORD3		,"ase_tex_coord3"},
			{TemplateSemantics.TEXCOORD4		,"ase_tex_coord4"},
			{TemplateSemantics.TEXCOORD5		,"ase_tex_coord5"},
			{TemplateSemantics.TEXCOORD6		,"ase_tex_coord6"},
			{TemplateSemantics.TEXCOORD7		,"ase_tex_coord7"},
			{TemplateSemantics.TEXCOORD8		,"ase_tex_coord8"},
			{TemplateSemantics.TEXCOORD9		,"ase_tex_coord9"},
			{TemplateSemantics.TEXCOORD10		,"ase_tex_coord10"},
			{TemplateSemantics.TEXCOORD11		,"ase_tex_coord11"},
			{TemplateSemantics.TEXCOORD12		,"ase_tex_coord12"},
			{TemplateSemantics.TEXCOORD13		,"ase_tex_coord13"},
			{TemplateSemantics.TEXCOORD14		,"ase_tex_coord14"},
			{TemplateSemantics.TEXCOORD15		,"ase_tex_coord15"},
		};

		public static readonly Dictionary<int, TemplateInfoOnSematics> IntToInfo = new Dictionary<int, TemplateInfoOnSematics>
		{
			{0,TemplateInfoOnSematics.TEXTURE_COORDINATES0 },
			{1,TemplateInfoOnSematics.TEXTURE_COORDINATES1 },
			{2,TemplateInfoOnSematics.TEXTURE_COORDINATES2 },
			{3,TemplateInfoOnSematics.TEXTURE_COORDINATES3 },
			{4,TemplateInfoOnSematics.TEXTURE_COORDINATES4 },
			{5,TemplateInfoOnSematics.TEXTURE_COORDINATES5 },
			{6,TemplateInfoOnSematics.TEXTURE_COORDINATES6 },
			{7,TemplateInfoOnSematics.TEXTURE_COORDINATES7 },
		};

		public static readonly Dictionary<string, TemplateInfoOnSematics> ShortcutToInfo = new Dictionary<string, TemplateInfoOnSematics>
		{
			{"p"    ,TemplateInfoOnSematics.POSITION },
			{"sp"   ,TemplateInfoOnSematics.CLIP_POS },
			{"spu"  ,TemplateInfoOnSematics.SCREEN_POSITION },
			{"spn"  ,TemplateInfoOnSematics.SCREEN_POSITION_NORMALIZED },
			{"c"    ,TemplateInfoOnSematics.COLOR },
			{"uv0"  ,TemplateInfoOnSematics.TEXTURE_COORDINATES0 },
			{"uv1"  ,TemplateInfoOnSematics.TEXTURE_COORDINATES1 },
			{"uv2"  ,TemplateInfoOnSematics.TEXTURE_COORDINATES2 },
			{"uv3"  ,TemplateInfoOnSematics.TEXTURE_COORDINATES3 },
			{"uv4"  ,TemplateInfoOnSematics.TEXTURE_COORDINATES4 },
			{"uv5"  ,TemplateInfoOnSematics.TEXTURE_COORDINATES5 },
			{"uv6"  ,TemplateInfoOnSematics.TEXTURE_COORDINATES6 },
			{"uv7"  ,TemplateInfoOnSematics.TEXTURE_COORDINATES7 },
			{"n"    ,TemplateInfoOnSematics.NORMAL },
			{"t"    ,TemplateInfoOnSematics.TANGENT },
			{"wn"   ,TemplateInfoOnSematics.WORLD_NORMAL},
			{"wt"   ,TemplateInfoOnSematics.WORLD_TANGENT},
			{"wbt"  ,TemplateInfoOnSematics.WORLD_BITANGENT},
			{"wvd"  ,TemplateInfoOnSematics.WORLD_VIEW_DIR},
			{"wp"   ,TemplateInfoOnSematics.WORLD_POSITION},
			{"rwp"  ,TemplateInfoOnSematics.RELATIVE_WORLD_POS},
			{"vf"   ,TemplateInfoOnSematics.VFACE},
			{"sc"   ,TemplateInfoOnSematics.SHADOWCOORDS}
		};

		public static readonly Dictionary<TemplateInfoOnSematics, string> InfoToDefineFrag = new Dictionary<TemplateInfoOnSematics, string>
		{
			{TemplateInfoOnSematics.POSITION ,"ASE_NEEDS_FRAG_POSITION"},
			{TemplateInfoOnSematics.CLIP_POS ,"ASE_NEEDS_FRAG_CLIP_POS"},
			{TemplateInfoOnSematics.SCREEN_POSITION,"ASE_NEEDS_FRAG_SCREEN_POSITION" },
			{TemplateInfoOnSematics.SCREEN_POSITION_NORMALIZED,"ASE_NEEDS_FRAG_SCREEN_POSITION_NORMALIZED" },
			{TemplateInfoOnSematics.COLOR, "ASE_NEEDS_FRAG_COLOR"},
			{TemplateInfoOnSematics.TEXTURE_COORDINATES0,"ASE_NEEDS_FRAG_TEXTURE_COORDINATES0" },
			{TemplateInfoOnSematics.TEXTURE_COORDINATES1,"ASE_NEEDS_FRAG_TEXTURE_COORDINATES1" },
			{TemplateInfoOnSematics.TEXTURE_COORDINATES2,"ASE_NEEDS_FRAG_TEXTURE_COORDINATES2" },
			{TemplateInfoOnSematics.TEXTURE_COORDINATES3,"ASE_NEEDS_FRAG_TEXTURE_COORDINATES3" },
			{TemplateInfoOnSematics.TEXTURE_COORDINATES4,"ASE_NEEDS_FRAG_TEXTURE_COORDINATES4" },
			{TemplateInfoOnSematics.TEXTURE_COORDINATES5,"ASE_NEEDS_FRAG_TEXTURE_COORDINATES5" },
			{TemplateInfoOnSematics.TEXTURE_COORDINATES6,"ASE_NEEDS_FRAG_TEXTURE_COORDINATES6" },
			{TemplateInfoOnSematics.TEXTURE_COORDINATES7,"ASE_NEEDS_FRAG_TEXTURE_COORDINATES7" },
			{TemplateInfoOnSematics.NORMAL,"ASE_NEEDS_FRAG_NORMAL" },
			{TemplateInfoOnSematics.TANGENT ,"ASE_NEEDS_FRAG_TANGENT"},
			{TemplateInfoOnSematics.WORLD_NORMAL,"ASE_NEEDS_FRAG_WORLD_NORMAL"},
			{TemplateInfoOnSematics.WORLD_TANGENT,"ASE_NEEDS_FRAG_WORLD_TANGENT"},
			{TemplateInfoOnSematics.WORLD_BITANGENT,"ASE_NEEDS_FRAG_WORLD_BITANGENT"},
			{TemplateInfoOnSematics.WORLD_VIEW_DIR,"ASE_NEEDS_FRAG_WORLD_VIEW_DIR"},
			{TemplateInfoOnSematics.WORLD_POSITION,"ASE_NEEDS_FRAG_WORLD_POSITION"},
			{TemplateInfoOnSematics.RELATIVE_WORLD_POS,"ASE_NEEDS_FRAG_RELATIVE_WORLD_POS"},
			{TemplateInfoOnSematics.VFACE,"ASE_NEEDS_FRAG_VFACE"},
			{TemplateInfoOnSematics.SHADOWCOORDS,"ASE_NEEDS_FRAG_SHADOWCOORDS"}
		};

		public static readonly Dictionary<TemplateInfoOnSematics, string> InfoToDefineVertex = new Dictionary<TemplateInfoOnSematics, string>
		{
			{TemplateInfoOnSematics.POSITION ,"ASE_NEEDS_VERT_POSITION"},
			{TemplateInfoOnSematics.CLIP_POS ,"ASE_NEEDS_VERT_CLIP_POS"},
			{TemplateInfoOnSematics.SCREEN_POSITION,"ASE_NEEDS_VERT_SCREEN_POSITION" },
			{TemplateInfoOnSematics.SCREEN_POSITION_NORMALIZED,"ASE_NEEDS_VERT_SCREEN_POSITION_NORMALIZED" },
			{TemplateInfoOnSematics.COLOR, "ASE_NEEDS_VERT_COLOR"},
			{TemplateInfoOnSematics.TEXTURE_COORDINATES0,"ASE_NEEDS_VERT_TEXTURE_COORDINATES0" },
			{TemplateInfoOnSematics.TEXTURE_COORDINATES1,"ASE_NEEDS_VERT_TEXTURE_COORDINATES1" },
			{TemplateInfoOnSematics.TEXTURE_COORDINATES2,"ASE_NEEDS_VERT_TEXTURE_COORDINATES2" },
			{TemplateInfoOnSematics.TEXTURE_COORDINATES3,"ASE_NEEDS_VERT_TEXTURE_COORDINATES3" },
			{TemplateInfoOnSematics.TEXTURE_COORDINATES4,"ASE_NEEDS_VERT_TEXTURE_COORDINATES4" },
			{TemplateInfoOnSematics.TEXTURE_COORDINATES5,"ASE_NEEDS_VERT_TEXTURE_COORDINATES5" },
			{TemplateInfoOnSematics.TEXTURE_COORDINATES6,"ASE_NEEDS_VERT_TEXTURE_COORDINATES6" },
			{TemplateInfoOnSematics.TEXTURE_COORDINATES7,"ASE_NEEDS_VERT_TEXTURE_COORDINATES7" },
			{TemplateInfoOnSematics.NORMAL,"ASE_NEEDS_VERT_NORMAL" },
			{TemplateInfoOnSematics.TANGENT ,"ASE_NEEDS_VERT_TANGENT"},
			{TemplateInfoOnSematics.WORLD_NORMAL,"ASE_NEEDS_VERT_WORLD_NORMAL"},
			{TemplateInfoOnSematics.WORLD_TANGENT,"ASE_NEEDS_VERT_WORLD_TANGENT"},
			{TemplateInfoOnSematics.WORLD_BITANGENT,"ASE_NEEDS_VERT_WORLD_BITANGENT"},
			{TemplateInfoOnSematics.WORLD_VIEW_DIR,"ASE_NEEDS_VERT_WORLD_VIEW_DIR"},
			{TemplateInfoOnSematics.WORLD_POSITION,"ASE_NEEDS_VERT_WORLD_POSITION"},
			{TemplateInfoOnSematics.RELATIVE_WORLD_POS,"ASE_NEEDS_VERT_RELATIVE_WORLD_POS"},
			{TemplateInfoOnSematics.VFACE,"ASE_NEEDS_VERT_VFACE"},
			{TemplateInfoOnSematics.SHADOWCOORDS,"ASE_NEEDS_VERT_SHADOWCOORDS"}
		};

		public static readonly Dictionary<TemplateInfoOnSematics, string> InfoToLocalVar = new Dictionary<TemplateInfoOnSematics, string>
		{
			{TemplateInfoOnSematics.POSITION,GeneratorUtils.VertexPosition4Str },
			{TemplateInfoOnSematics.CLIP_POS,GeneratorUtils.ClipPositionStr },
			{TemplateInfoOnSematics.SCREEN_POSITION,GeneratorUtils.ScreenPositionStr },
			{TemplateInfoOnSematics.SCREEN_POSITION_NORMALIZED,GeneratorUtils.ScreenPositionNormalizedStr },
			{TemplateInfoOnSematics.COLOR, "ase_color" },
			{TemplateInfoOnSematics.TEXTURE_COORDINATES0, "ase_uv0" },
			{TemplateInfoOnSematics.TEXTURE_COORDINATES1, "ase_uv1" },
			{TemplateInfoOnSematics.TEXTURE_COORDINATES2, "ase_uv2" },
			{TemplateInfoOnSematics.TEXTURE_COORDINATES3, "ase_uv3" },
			{TemplateInfoOnSematics.NORMAL, GeneratorUtils.VertexNormalStr },
			{TemplateInfoOnSematics.TANGENT, GeneratorUtils.VertexTangentStr },
			{TemplateInfoOnSematics.WORLD_NORMAL, GeneratorUtils.WorldNormalStr},
			{TemplateInfoOnSematics.WORLD_TANGENT, GeneratorUtils.WorldTangentStr},
			{TemplateInfoOnSematics.WORLD_BITANGENT, GeneratorUtils.WorldBitangentStr},
			{TemplateInfoOnSematics.WORLD_VIEW_DIR, GeneratorUtils.WorldViewDirectionStr},
			{TemplateInfoOnSematics.WORLD_POSITION, GeneratorUtils.WorldPositionStr},
			{TemplateInfoOnSematics.RELATIVE_WORLD_POS, GeneratorUtils.RelativeWorldPositionStr},
			{TemplateInfoOnSematics.VFACE, GeneratorUtils.VFaceStr},
			{TemplateInfoOnSematics.SHADOWCOORDS, GeneratorUtils.ShadowCoordsStr}
		};


		public static readonly Dictionary<TemplateInfoOnSematics, WirePortDataType> InfoToWirePortType = new Dictionary<TemplateInfoOnSematics, WirePortDataType>
		{
			{TemplateInfoOnSematics.POSITION,WirePortDataType.FLOAT4 },
			{TemplateInfoOnSematics.CLIP_POS,WirePortDataType.FLOAT4 },
			{TemplateInfoOnSematics.SCREEN_POSITION,WirePortDataType.FLOAT4 },
			{TemplateInfoOnSematics.SCREEN_POSITION_NORMALIZED,WirePortDataType.FLOAT4 },
			{TemplateInfoOnSematics.COLOR, WirePortDataType.COLOR },
			{TemplateInfoOnSematics.TEXTURE_COORDINATES0, WirePortDataType.FLOAT4 },
			{TemplateInfoOnSematics.TEXTURE_COORDINATES1, WirePortDataType.FLOAT4 },
			{TemplateInfoOnSematics.TEXTURE_COORDINATES2, WirePortDataType.FLOAT4 },
			{TemplateInfoOnSematics.TEXTURE_COORDINATES3, WirePortDataType.FLOAT4 },
			{TemplateInfoOnSematics.NORMAL, WirePortDataType.FLOAT3 },
			{TemplateInfoOnSematics.TANGENT, WirePortDataType.FLOAT4 },
			{TemplateInfoOnSematics.WORLD_NORMAL, WirePortDataType.FLOAT3},
			{TemplateInfoOnSematics.WORLD_TANGENT, WirePortDataType.FLOAT3},
			{TemplateInfoOnSematics.WORLD_BITANGENT, WirePortDataType.FLOAT3},
			{TemplateInfoOnSematics.WORLD_VIEW_DIR, WirePortDataType.FLOAT3},
			{TemplateInfoOnSematics.WORLD_POSITION, WirePortDataType.FLOAT3},
			{TemplateInfoOnSematics.RELATIVE_WORLD_POS, WirePortDataType.FLOAT3},
			{TemplateInfoOnSematics.VFACE, WirePortDataType.FLOAT},
			{TemplateInfoOnSematics.SHADOWCOORDS, WirePortDataType.FLOAT4},
		};
		public static readonly Dictionary<int, TemplateInfoOnSematics> IntToUVChannelInfo = new Dictionary<int, TemplateInfoOnSematics>
		{
			{0,TemplateInfoOnSematics.TEXTURE_COORDINATES0 },
			{1,TemplateInfoOnSematics.TEXTURE_COORDINATES1 },
			{2,TemplateInfoOnSematics.TEXTURE_COORDINATES2 },
			{3,TemplateInfoOnSematics.TEXTURE_COORDINATES3 },
			{4,TemplateInfoOnSematics.TEXTURE_COORDINATES4 },
			{5,TemplateInfoOnSematics.TEXTURE_COORDINATES5 },
			{6,TemplateInfoOnSematics.TEXTURE_COORDINATES6 },
			{7,TemplateInfoOnSematics.TEXTURE_COORDINATES7 }
		};

		public static readonly Dictionary<int, TemplateSemantics> IntToSemantic = new Dictionary<int, TemplateSemantics>
		{
			{ 0,TemplateSemantics.TEXCOORD0 },
			{ 1,TemplateSemantics.TEXCOORD1 },
			{ 2,TemplateSemantics.TEXCOORD2 },
			{ 3,TemplateSemantics.TEXCOORD3 },
			{ 4,TemplateSemantics.TEXCOORD4 },
			{ 5,TemplateSemantics.TEXCOORD5 },
			{ 6,TemplateSemantics.TEXCOORD6 },
			{ 7,TemplateSemantics.TEXCOORD7 },
			{ 8,TemplateSemantics.TEXCOORD8 },
			{ 9,TemplateSemantics.TEXCOORD9 },
			{ 10,TemplateSemantics.TEXCOORD10 },
			{ 11,TemplateSemantics.TEXCOORD11 },
			{ 12,TemplateSemantics.TEXCOORD12 },
			{ 13,TemplateSemantics.TEXCOORD13 },
			{ 14,TemplateSemantics.TEXCOORD14 },
			{ 15,TemplateSemantics.TEXCOORD15 }
		};

		public static readonly Dictionary<TemplateSemantics, int> SemanticToInt = new Dictionary<TemplateSemantics, int>
		{
			{ TemplateSemantics.TEXCOORD0,0 },
			{ TemplateSemantics.TEXCOORD1,1 },
			{ TemplateSemantics.TEXCOORD2,2 },
			{ TemplateSemantics.TEXCOORD3,3 },
			{ TemplateSemantics.TEXCOORD4,4 },
			{ TemplateSemantics.TEXCOORD5,5 },
			{ TemplateSemantics.TEXCOORD6,6 },
			{ TemplateSemantics.TEXCOORD7,7 },
			{ TemplateSemantics.TEXCOORD8,8 },
			{ TemplateSemantics.TEXCOORD9,9 },
			{ TemplateSemantics.TEXCOORD10,10 },
			{ TemplateSemantics.TEXCOORD11,11 },
			{ TemplateSemantics.TEXCOORD12,12 },
			{ TemplateSemantics.TEXCOORD13,13 },
			{ TemplateSemantics.TEXCOORD14,14 },
			{ TemplateSemantics.TEXCOORD15,15 },
		};

		public static readonly Dictionary<string, TemplateSemantics> ShortcutToSemantic = new Dictionary<string, TemplateSemantics>
		{
			{ "p"   ,TemplateSemantics.POSITION },
			{ "sp"  ,TemplateSemantics.SV_POSITION },
			{ "c"   ,TemplateSemantics.COLOR },
			{ "n"   ,TemplateSemantics.NORMAL },
			{ "t"   ,TemplateSemantics.TANGENT },
			{ "tc0" ,TemplateSemantics.TEXCOORD0 },
			{ "tc1" ,TemplateSemantics.TEXCOORD1 },
			{ "tc2" ,TemplateSemantics.TEXCOORD2 },
			{ "tc3" ,TemplateSemantics.TEXCOORD3 },
			{ "tc4" ,TemplateSemantics.TEXCOORD4 },
			{ "tc5" ,TemplateSemantics.TEXCOORD5 },
			{ "tc6" ,TemplateSemantics.TEXCOORD6 },
			{ "tc7" ,TemplateSemantics.TEXCOORD7 },
			{ "tc8" ,TemplateSemantics.TEXCOORD8 },
			{ "tc9" ,TemplateSemantics.TEXCOORD9 },
			{ "tc10" ,TemplateSemantics.TEXCOORD10 },
			{ "tc11" ,TemplateSemantics.TEXCOORD11 },
			{ "tc12" ,TemplateSemantics.TEXCOORD12 },
			{ "tc13" ,TemplateSemantics.TEXCOORD13 },
			{ "tc14" ,TemplateSemantics.TEXCOORD14 },
			{ "tc15" ,TemplateSemantics.TEXCOORD15 }
		};

		public static readonly Dictionary<string, WirePortDataType> CgToWirePortType = new Dictionary<string, WirePortDataType>()
		{
			{"float"            ,WirePortDataType.FLOAT},
			{"float2"           ,WirePortDataType.FLOAT2},
			{"float3"           ,WirePortDataType.FLOAT3},
			{"float4"           ,WirePortDataType.FLOAT4},
			{"float3x3"         ,WirePortDataType.FLOAT3x3},
			{"float4x4"         ,WirePortDataType.FLOAT4x4},
			{"half"             ,WirePortDataType.FLOAT},
			{"half2"            ,WirePortDataType.FLOAT2},
			{"half3"            ,WirePortDataType.FLOAT3},
			{"half4"            ,WirePortDataType.FLOAT4},
			{"half3x3"          ,WirePortDataType.FLOAT3x3},
			{"half4x4"          ,WirePortDataType.FLOAT4x4},
			{"fixed"            ,WirePortDataType.FLOAT},
			{"fixed2"           ,WirePortDataType.FLOAT2},
			{"fixed3"           ,WirePortDataType.FLOAT3},
			{"fixed4"           ,WirePortDataType.FLOAT4},
			{"fixed3x3"         ,WirePortDataType.FLOAT3x3},
			{"fixed4x4"         ,WirePortDataType.FLOAT4x4},
			{"int"              ,WirePortDataType.INT},
			{"uint"             ,WirePortDataType.INT},
			{"sampler1D"        ,WirePortDataType.SAMPLER1D},
			{"sampler2D"        ,WirePortDataType.SAMPLER2D},
			{"sampler2D_float"  ,WirePortDataType.SAMPLER2D},
			{"sampler3D"        ,WirePortDataType.SAMPLER3D},
			{"samplerCUBE"      ,WirePortDataType.SAMPLERCUBE},
			{"sampler2DArray"   ,WirePortDataType.SAMPLER2DARRAY},
			{"SamplerState"     ,WirePortDataType.SAMPLERSTATE}
		};

		public static readonly Dictionary<string, int> AvailableInterpolators = new Dictionary<string, int>()
		{
			{"2.0",8 },
			{"2.5",8 },
			{"3.0",10},
			{"3.5",10},
			{"4.0",16},
			{"4.5",16},
			{"4.6",16},
			{"5.0",16}
		};

		public static readonly string[] AvailableShaderModels =
		{ "2.0", "2.5", "3.0", "3.5", "4.0", "4.5", "4.6", "5.0" };

		public static readonly Dictionary<string, int> ShaderModelToArrayIdx = new Dictionary<string, int>()
		{
			{"2.0",0},
			{"2.5",1},
			{"3.0",2},
			{"3.5",3},
			{"4.0",4},
			{"4.5",5},
			{"4.6",6},
			{"5.0",7}
		};

		public static readonly string HDPBRTag = "UNITY_MATERIAL_LIT";
		public static readonly Dictionary<string, TemplateSRPType> TagToRenderPipeline = new Dictionary<string, TemplateSRPType>()
		{
			{ "UniversalPipeline",TemplateSRPType.Lightweight },
			{ "LightweightPipeline",TemplateSRPType.Lightweight },
			{ "HDRenderPipeline",TemplateSRPType.HD }
		};
#if UNITY_2018_3_OR_NEWER
		public static string CoreColorLib = "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl";
		public static string CoreCommonLib = "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl";
#else
		public static string CoreCommonLib = "CoreRP/ShaderLibrary/Common.hlsl";
		public static string CoreColorLib = "CoreRP/ShaderLibrary/Color.hlsl";
#endif

		public static string PragmaOnlyRendersPattern = @"#pragma\s+only_renderers\s+([\w .]*)";
		public static string PragmaExcludeRendersPattern = @"#pragma\s+exclude_renderers\s+([\w .]*)";
		public static string PragmaRendererElement = @"(\w+)";

		public static string FetchSubShaderBody = @"(SubShader.*)\/\*ase_lod\*\/";
		public static string TemplateCustomUI = @"\/\*CustomNodeUI:(\w*)\*\/";
		public static string HidePassPattern = @"\/\*ase_hide_pass[:]*([a-zA-Z:]*)\*\/";
		public static string ASEPassPattern = @"\/\*ase_pass[:]*([a-zA-Z:]*)\*\/";
		public static string BlendWholeWordPattern = @"\bBlend\b";
		public static string BlendOpWholeWordPattern = @"\bBlendOp\b";
		public static string AlphaToMaskPattern = @"\bAlphaToMask\s+(\[*\w+\]*)";
		public static string CullWholeWordPattern = @"\bCull\b";
		public static string ColorMaskWholeWordPattern = @"\bColorMask\b";
		public static string StencilWholeWordPattern = @"\bStencil\b";
		public static string ZWriteWholeWordPattern = @"\bZWrite\b";
		public static string ZTestWholeWordPattern = @"\bZTest\b";
		public static string ZOffsetWholeWordPattern = @"\bOffset\b";
		public static string TagsWholeWordPattern = @"\bTags\b";


		public static string CustomInspectorPattern = "^\\s*CustomEditor\\s+\\\"([\\w\\.]*)\\\"";
		public static string FallbackPattern = "^\\s*Fallback\\s+\\\"([\\w\\/\\\\]*)\\\"";
		public static string DefinesPattern = @"^\s*#define\s+([\w .]*)";
		public static string PragmasPattern = @"^\s*#pragma\s+([\w .]*)";
		public static string IncludesPattern = "^\\s*#include\\s+\"([\\w.\\/]*)\"";
		public static string GlobalDirectivesPattern = "[#]+(define|pragma|include)\\s+([\\w .\\/\\\"]*)";
		public static string BeforePragmaPattern = @"(?:CGPROGRAM|HLSLPROGRAM|GLSLPROGRAM).*?\n(\s*)(.)";
		public static string GlobalTOPDirectivesPattern = @"(CGPROGRAM|CGINCLUDE|HLSLPROGRAM|HLSLINCLUDE).*?\n\s*(.)";

		public static string VertexPragmaPattern = @"#pragma vertex\s+(\w+)";
		public static string FragmentPragmaPattern = @"#pragma fragment\s+(\w+)";
		public static string FunctionBodyStartPattern = @"\s+{0}\s*\(";

		public static string ShaderModelPattern = @"#pragma\s+target\s+([0-9]*[.]*[0-9]*)";

		public static readonly string LocalVarPattern = @"\/\*ase_local_var[:]*(\w*)\*\/\s*(\w*)\s+(\w*)";

		public static readonly string InlinePattern = @"\/\*ase_inline_begin\*\/(.*?)\/\*ase_inline_end\*\/";

		public static readonly string SubShaderLODPattern = @"\sLOD\s+(\d+)";

		public static readonly string PassNamePattern = "Name\\s+\\\"([\\w\\+\\-\\*\\/\\(\\) ]*)\\\"";

		public static readonly string TagsPattern = "\"(\\w+)\"\\s*=\\s*\"(\\w+\\+*\\w*)\"";
		public static readonly string ZTestPattern = @"^\s*ZTest\s+(\[*\w+\]*)";
		public static readonly string ZWritePattern = @"^\s*ZWrite\s+(\[*\w+\]*)";
		//public static readonly string ZOffsetPattern = @"\s*Offset\s+([-+]?[0-9]*\.?[0-9]+)\s*,\s*([-+]?[0-9]*\.?[0-9]+)";
		public static readonly string ZOffsetPattern = @"^\s*Offset\s+([-+]?[0-9]*\.?[0-9]+|\[*\w+\]*)\s*,\s*([-+]?[0-9]*\.?[0-9]+|\[*\w+\]*)\s*";
		public static readonly string VertexDataPattern = @"([a-z0-9D_]+|samplerCUBE|sampler2DArray)\s+(\w+)\s*:\s*([A-Z0-9_]+);";
		public static readonly string InterpRangePattern = @"ase_interp\((\d\.{0,1}\w{0,4}),(\d*)\)";
		//public static readonly string PropertiesPatternB = "(\\w*)\\s*\\(\\s*\"([\\w ]*)\"\\s*\\,\\s*(\\w*)\\s*.*\\)";
		//public static readonly string PropertiesPatternC = "^\\s*(\\w*)\\s*\\(\\s*\"([\\w\\(\\)\\+\\-\\\\* ]*)\"\\s*\\,\\s*(\\w*)\\s*.*\\)";
		//public static readonly string PropertiesPatternD = "(\\/\\/\\s*)*(\\w*)\\s*\\(\\s*\"([\\w\\(\\)\\+\\-\\\\* ]*)\"\\s*\\,\\s*(\\w*)\\s*.*\\)";
		//public static readonly string PropertiesPatternE = "(\\/\\/\\s*)*(\\w*)\\s*\\(\\s*\"([\\w\\(\\)\\+\\-\\\\* ]*)\"\\s*\\,\\s*(\\w*)\\s*.*\\)\\s*=\\s*[\\w,()\" {}]*";
		//public static readonly string PropertiesPatternF = "^(\\/\\/)*\\s*(\\[[\\[\\]\\w\\s\\(\\)\\_\\,]*\\])*\\s*(\\w*)\\s*\\(\\s*\"([\\w\\(\\)\\+\\-\\\\* ]*)\"\\s*\\,\\s*(\\w*)\\s*.*\\)\\s*=\\s*[\\w,()\" {}]*";
		//public static readonly string PropertiesPatternG = "^(\\s*)(\\[[\\[\\]\\w\\s\\(\\)\\_\\,]*\\])*\\s*(\\w*)\\s*\\(\\s*\"([\\w\\(\\)\\+\\-\\\\* ]*)\"\\s*\\,\\s*(\\w*)\\s*.*\\)\\s*=\\s*[\\w,()\" {}]*";
		public static readonly string PropertiesPatternG = "^(\\s*)(\\[[\\[\\]\\w\\s\\(\\)_,\\.]*\\])*[\\s\\/]*(\\w*)\\s*\\(\\s*\"([\\w\\(\\)\\+\\-\\\\* ]*)\"\\s*\\,\\s*(\\w*)\\s*.*\\)\\s*=\\s*[\\w,()\" {}\\.]*";
		public static readonly string CullModePattern = @"^\s*Cull\s+(\[*\w+\]*)";

		public static readonly string ColorMaskPatternFirst = @"\bColorMask\s+([\d\w\[\]]+)(\s+0)*";

		public static readonly string ColorMaskPattern = @"\bColorMask\s+([\d\w\[\]]+)(\s+0)";
		public static readonly string ColorMask1Pattern = @"\bColorMask\s+([\d\w\[\]]+)(\s+1)";
		public static readonly string ColorMask2Pattern = @"\bColorMask\s+([\d\w\[\]]+)(\s+2)";
		public static readonly string ColorMask3Pattern = @"\bColorMask\s+([\d\w\[\]]+)(\s+3)";
		//public static readonly string BlendModePattern = @"\s*Blend\s+(\w+)\s+(\w+)(?:[\s,]+(\w+)\s+(\w+)|)";
		//public static readonly string BlendModePattern = @"\s*Blend\s+(\[*\w+\]*)\s+(\[*\w+\]*)(?:[\s,]+(\[*\w+\]*)\s+(\[*\w+\]*)|)";
		//public static readonly string BlendModePattern = @"^\s*Blend\s+(?:(?=\d)|(\[*\w+\]*)\s+(\[*\w+\]*)(?:[\s,]+(\[*\w+\]*)\s+(\[*\w+\]*)|))";
		public static readonly string BlendModePatternFirst = @"\bBlend([ \t]+0)*[ \t]+(?:Off|(\[*\w+\]*)[ \t]+(\[*\w+\]*)(?:(?=[ \t]+Blend)|(?:[ \t,]+(\[*\w+\]*)[ \t]+(\[*\w+\]*)|)))";
		public static readonly string BlendModePattern = @"\bBlend([ \t]+0)[ \t]+(?:Off|(\[*\w+\]*)[ \t]+(\[*\w+\]*)(?:(?=[ \t]+Blend)|(?:[ \t,]+(\[*\w+\]*)[ \t]+(\[*\w+\]*)|)))";
		public static readonly string BlendModePattern1 = @"\bBlend([ \t]+1)[ \t]+(?:Off|(\[*\w+\]*)[ \t]+(\[*\w+\]*)(?:(?=[ \t]+Blend)|(?:[ \t,]+(\[*\w+\]*)[ \t]+(\[*\w+\]*)|)))";
		public static readonly string BlendModePattern2 = @"\bBlend([ \t]+2)[ \t]+(?:Off|(\[*\w+\]*)[ \t]+(\[*\w+\]*)(?:(?=[ \t]+Blend)|(?:[ \t,]+(\[*\w+\]*)[ \t]+(\[*\w+\]*)|)))";
		public static readonly string BlendModePattern3 = @"\bBlend([ \t]+3)[ \t]+(?:Off|(\[*\w+\]*)[ \t]+(\[*\w+\]*)(?:(?=[ \t]+Blend)|(?:[ \t,]+(\[*\w+\]*)[ \t]+(\[*\w+\]*)|)))";
		//public static readonly string BlendOpPattern = @"\s*BlendOp\s+(\w+)[\s,]*(?:(\w+)|)";
		//public static readonly string BlendOpPattern = @"\s*BlendOp\s+(\[*\w+\]*)[\s,]*(?:(\[*\w+\]*)|)";
		//public static readonly string BlendOpPattern = @"^\s*BlendOp\s+(?:(?=\d)|(\[*\w+\]*)[\s,]*(?:(\[*\w+\]*)|))";
		public static readonly string BlendOpPatternFirst = @"\bBlendOp([ \t]+0)*[ \t]+(\[*\w+\]*)(?:(?=[ \t]+Blend)|[ \t,]*(?:(\[*\w+\]*)|))";
		public static readonly string BlendOpPattern = @"\bBlendOp([ \t]+0)[ \t]+(\[*\w+\]*)(?:(?=[ \t]+Blend)|[ \t,]*(?:(\[*\w+\]*)|))";
		public static readonly string BlendOpPattern1 = @"\bBlendOp([ \t]+1)[ \t]+(\[*\w+\]*)(?:(?=[ \t]+Blend)|[ \t,]*(?:(\[*\w+\]*)|))";
		public static readonly string BlendOpPattern2 = @"\bBlendOp([ \t]+2)[ \t]+(\[*\w+\]*)(?:(?=[ \t]+Blend)|[ \t,]*(?:(\[*\w+\]*)|))";
		public static readonly string BlendOpPattern3 = @"\bBlendOp([ \t]+3)[ \t]+(\[*\w+\]*)(?:(?=[ \t]+Blend)|[ \t,]*(?:(\[*\w+\]*)|))";
		public static readonly string StencilOpGlobalPattern = @"Stencil\s*{([\w\W\s]*)}";
		public static readonly string StencilOpLinePattern = @"(\w+)\s*(\[*\w+\]*)";

		public static readonly string ShaderGlobalsOverallPattern = @"(?:\/\*ase_pragma\*\/|[\}\#])[\#\&\|\!\<\>\(\)\=\w\s\;\/\*\.\\""]*\/\*ase_globals\*\/";
		public static readonly string ShaderGlobalsMultilinePattern = @"^\s*(?:uniform\s*)*(\w*)\s*(\w*);$";

		public static readonly string TexSemantic = "float4 {0} : TEXCOORD{1};";
		public static readonly string TexFullSemantic = "float4 {0} : {1};";
		public static readonly string InterpFullSemantic = "{0} {1} : {2};";
		public static readonly string BaseInterpolatorName = "ase_texcoord";
		public static readonly string TexUVFullSemantic = "float4 ase_texcoord{0} : TEXCOORD{0};";
		public static readonly string InterpMacro = "{0}({1})";

		public static readonly string InterpolatorDecl = Constants.VertexShaderOutputStr + ".{0} = " + Constants.VertexShaderInputStr + ".{0};";
		public static readonly string TemplateVariableDecl = "{0} = {1};";
		public static readonly string TemplateVarFormat = "{0}.{1}";

		//public static readonly string StructsRemoval = @"struct\s+\w+\s+{[\s\w;\/\*]+};";
		public static readonly string StructsRemoval = @"struct\s+\w+\s+{[\#\&\|\!\<\>\s\w\(\).;:=,\/\*]+};";

		public static readonly string SRPBatcherFindTag = @"CBUFFER_START\s*\(\s*UnityPerMaterial\s*\)\s*\n(\s*)";

		public static string ReplaceAt( this string body, string oldStr, string newStr, int startIndex )
		{
			return body.Remove( startIndex, oldStr.Length ).Insert( startIndex, newStr );
		}

		public static bool GetPassUniqueId( TemplateTagData tagData, TemplatePropertyContainer propertyContainer, TemplateIdManager idManager, string uniquePrefix, int offsetIdx, string subBody, ref string  passUniqueID )
		{
			Match match = Regex.Match( subBody, ASEPassPattern );
			if( match.Success && match.Groups.Count > 1 && match.Groups[1].Length > 0  )
			{
				passUniqueID = match.Groups[ 1 ].Value;

				tagData.StartIdx = offsetIdx + match.Index;
				tagData.Id = match.Value;

				idManager.RegisterId( tagData.StartIdx, uniquePrefix + tagData.Id, tagData.Id );
				propertyContainer.AddId( subBody, tagData.Id, tagData.SearchIndentation );
				return true;
			}
			return false;
		}

		public static CustomTemplatePropertyUIEnum FetchCustomUI( string data )
		{
			Match match = Regex.Match( data, TemplateCustomUI );
			if( match.Success && CustomTemplatePropertyUI.ContainsKey( match.Groups[ 1 ].Value ) )
			{
				return CustomTemplatePropertyUI[ match.Groups[ 1 ].Value ];
			}
			return CustomTemplatePropertyUIEnum.None;
		}

		public static bool FetchInvisibleInfo( string input, ref int optionsArr, ref string id, ref int idIndex )
		{
			Match match = Regex.Match( input, HidePassPattern );
			if( match.Success )
			{
				id = match.Value;
				idIndex = match.Index;
				if( match.Groups.Count > 1 )
				{
					string[] properties = match.Groups[ 1 ].Value.Split( ':' );
					for( int i = 0; i < properties.Length; i++ )
					{
						if( InvisibleOptions.ContainsKey( properties[ i ] ) )
						{
							optionsArr |= (int)InvisibleOptions[ properties[ i ] ];
						}
					}
				}
			}
			return match.Success;
		}

		static public string GenerateTextureSemantic( ref MasterNodeDataCollector dataCollector, int uv )
		{
			string texCoordName = BaseInterpolatorName;
			if( uv > 0 )
			{
				texCoordName += uv.ToString();
			}

			string texCoordData = string.Format( TexSemantic, texCoordName, uv );
			dataCollector.AddToVertexInput( texCoordData );
			dataCollector.AddToInterpolators( texCoordData );
			dataCollector.AddToVertexInterpolatorsDecl( string.Format( InterpolatorDecl, texCoordName ) );
			return texCoordName;
		}

		public static void CreatePragmaIncludeList( string data, TemplateIncludePragmaContainter includePragmaContainer )
		{
			// this finds the topmost position for including directives
			int topIndex = -1;
			foreach( Match match in Regex.Matches( data, GlobalTOPDirectivesPattern, RegexOptions.Singleline ) )
			{
				if( match.Groups.Count == 3 )
				{
					topIndex = match.Groups[ 2 ].Index;
				}
			}

			foreach( Match match in Regex.Matches( data, GlobalDirectivesPattern, RegexOptions.Multiline ) )
			{
				if( match.Success )
				{
					includePragmaContainer.AddNativeDirective( match.Groups[ 0 ].Value, topIndex );
				}
			}

			foreach( Match match in Regex.Matches( data, PragmasPattern, RegexOptions.Multiline ) )
			{
				if( match.Groups.Count == 2 )
				{
					includePragmaContainer.AddPragma( match.Groups[ 1 ].Value );
				}
			}

			foreach( Match match in Regex.Matches( data, DefinesPattern, RegexOptions.Multiline ) )
			{
				if( match.Groups.Count == 2 )
				{
					includePragmaContainer.AddDefine( match.Groups[ 1 ].Value );
				}
			}

			foreach( Match match in Regex.Matches( data, IncludesPattern, RegexOptions.Multiline ) )
			{
				if( match.Groups.Count == 2 )
				{
					includePragmaContainer.AddInclude( match.Groups[ 1 ].Value );
				}
			}
		}

		public static void CreateShaderPropertiesList( string propertyData, ref List<TemplateShaderPropertyData> propertiesList, ref Dictionary<string, TemplateShaderPropertyData> duplicatesHelper, int subShaderId, int passId)
		{
			int identationIdx = (int)TemplateShaderPropertiesIdx.Identation;
			int nameIdx = (int)TemplateShaderPropertiesIdx.Name;
			int typeIdx = (int)TemplateShaderPropertiesIdx.Type;
			int inspectorNameIdx = (int)TemplateShaderPropertiesIdx.InspectorName;

			foreach( Match match in Regex.Matches( propertyData, PropertiesPatternG,RegexOptions.Multiline ) )
			{
				if( match.Groups.Count > 1 )
				{
					if( !duplicatesHelper.ContainsKey( match.Groups[ nameIdx ].Value ) && PropertyToWireType.ContainsKey( match.Groups[ typeIdx ].Value ) )
					{
						TemplateShaderPropertyData newData = new TemplateShaderPropertyData(	match.Index,
																								match.Value,
																								match.Groups[ identationIdx ].Value,
																								match.Groups[ inspectorNameIdx ].Value,
																								match.Groups[ nameIdx ].Value,
																								PropertyToWireType[ match.Groups[ typeIdx ].Value ],
																								PropertyType.Property,
																								subShaderId,
																								passId);
						propertiesList.Add( newData );
						duplicatesHelper.Add( newData.PropertyName, newData );
					}	
				}
			}
		}
		public const string DepthMacroDeclRegex = @"UNITY_DECLARE_DEPTH_TEXTURE\(\s*_CameraDepthTexture";
		public static void CheckUnityBuiltinGlobalMacros( string propertyData, ref List<TemplateShaderPropertyData> propertiesList, ref Dictionary<string, TemplateShaderPropertyData> duplicatesHelper, int subShaderId, int passId )
		{
			Match match = Regex.Match( propertyData, DepthMacroDeclRegex );
			if( match.Success )
			{
				TemplateShaderPropertyData newData = new TemplateShaderPropertyData( -1,
																							string.Empty,
																							string.Empty,
																							string.Empty,
																							Constants.CameraDepthTextureValue,
																							WirePortDataType.SAMPLER2D,
																							PropertyType.Global,
																							subShaderId,
																							passId,
																							true );
				duplicatesHelper.Add( newData.PropertyName, newData );
				propertiesList.Add( newData );
			}
		}

		public static void CreateShaderGlobalsList( string propertyData, ref List<TemplateShaderPropertyData> propertiesList, ref Dictionary<string, TemplateShaderPropertyData> duplicatesHelper,int subShaderId, int passId )
		{
			int typeIdx = (int)TemplateShaderGlobalsIdx.Type;
			int nameIdx = (int)TemplateShaderGlobalsIdx.Name;
			
			// removes structs
			propertyData = Regex.Replace( propertyData, StructsRemoval, "" );
			MatchCollection matchCollection = Regex.Matches( propertyData, ShaderGlobalsOverallPattern );
			string value = ( matchCollection.Count > 0 ) ? matchCollection[ 0 ].Groups[ 0 ].Value : propertyData;
			foreach( Match lineMatch in Regex.Matches( value, ShaderGlobalsMultilinePattern, RegexOptions.Multiline ) )
			{
				if( lineMatch.Groups.Count > 1 )
				{
					if( !duplicatesHelper.ContainsKey( lineMatch.Groups[ nameIdx ].Value ) && CgToWirePortType.ContainsKey( lineMatch.Groups[ typeIdx ].Value ) )
					{
						TemplateShaderPropertyData newData = new TemplateShaderPropertyData( -1,
																								string.Empty,
																								string.Empty,
																								string.Empty,
																								lineMatch.Groups[ nameIdx ].Value,
																								CgToWirePortType[ lineMatch.Groups[ typeIdx ].Value ],
																								PropertyType.Global,
																								subShaderId,
																								passId);
						duplicatesHelper.Add( newData.PropertyName, newData );
						propertiesList.Add( newData );
					}
				}
			}
		}

		public static void CreateStencilOps( string stencilData, ref TemplateStencilData stencilDataObj )
		{
			stencilDataObj.DataCheck = TemplateDataCheck.Invalid;
			MatchCollection overallGlobalMatch = Regex.Matches( stencilData, StencilOpGlobalPattern );
			if( overallGlobalMatch.Count == 1 && overallGlobalMatch[ 0 ].Groups.Count == 2 )
			{
				string property = string.Empty;
				string value = overallGlobalMatch[ 0 ].Groups[ 1 ].Value;
				foreach( Match match in Regex.Matches( value, StencilOpLinePattern ) )
				{
					stencilDataObj.DataCheck = TemplateDataCheck.Valid;
					if( match.Groups.Count == 3 )
					{
						switch( match.Groups[ 1 ].Value )
						{
							default:
								{
									stencilDataObj.DataCheck = TemplateDataCheck.Invalid;
									return;
								}
							case "Ref":
								{
									if( match.Groups[ 2 ].Success && IsInlineProperty( match.Groups[ 2 ].Value, ref property ) )
									{
										stencilDataObj.ReferenceInline = property;
									}
									else
									{
										try
										{
											stencilDataObj.Reference = Convert.ToInt32( match.Groups[ 2 ].Value );
										}
										catch( Exception e )
										{
											Debug.LogException( e );
											stencilDataObj.DataCheck = TemplateDataCheck.Invalid;
											return;
										}
									}
								}
								break;
							case "ReadMask":
								{
									if( match.Groups[ 2 ].Success && IsInlineProperty( match.Groups[ 2 ].Value, ref property ) )
									{
										stencilDataObj.ReadMaskInline = property;
									}
									else
									{
										try
										{
											stencilDataObj.ReadMask = Convert.ToInt32( match.Groups[ 2 ].Value );
										}
										catch( Exception e )
										{
											Debug.LogException( e );
											stencilDataObj.DataCheck = TemplateDataCheck.Invalid;
											return;
										}
									}
								}
								break;
							case "WriteMask":
								{
									if( match.Groups[ 2 ].Success && IsInlineProperty( match.Groups[ 2 ].Value, ref property ) )
									{
										stencilDataObj.WriteMaskInline = property;
									}
									else
									{
										try
										{
											stencilDataObj.WriteMask = Convert.ToInt32( match.Groups[ 2 ].Value );
										}
										catch( Exception e )
										{
											Debug.LogException( e );
											stencilDataObj.DataCheck = TemplateDataCheck.Invalid;
											return;
										}
									}
								}
								break;
							case "CompFront":
							case "Comp":
								{
									if( match.Groups[ 2 ].Success && IsInlineProperty( match.Groups[ 2 ].Value, ref property ) )
									{
										stencilDataObj.ComparisonFrontInline = property;
									}
									else
									{
										stencilDataObj.ComparisonFront = match.Groups[ 2 ].Value;
									}
								}
								break;
							case "PassFront":
							case "Pass":
								{
									if( match.Groups[ 2 ].Success && IsInlineProperty( match.Groups[ 2 ].Value, ref property ) )
									{
										stencilDataObj.PassFrontInline = property;
									}
									else
									{
										stencilDataObj.PassFront = match.Groups[ 2 ].Value;
									}
								}
								break;
							case "FailFront":
							case "Fail":
								{
									if( match.Groups[ 2 ].Success && IsInlineProperty( match.Groups[ 2 ].Value, ref property ) )
									{
										stencilDataObj.FailFrontInline = property;
									}
									else
									{
										stencilDataObj.FailFront = match.Groups[ 2 ].Value;
									}
								}
								break;
							case "ZFail":
							case "ZFailFront":
								{
									if( match.Groups[ 2 ].Success && IsInlineProperty( match.Groups[ 2 ].Value, ref property ) )
									{
										stencilDataObj.ZFailFrontInline = property;
									}
									else
									{
										stencilDataObj.ZFailFront = match.Groups[ 2 ].Value;
									}
								}
								break;
							case "CompBack":
								{
									if( match.Groups[ 2 ].Success && IsInlineProperty( match.Groups[ 2 ].Value, ref property ) )
									{
										stencilDataObj.ComparisonBackInline = property;
									}
									else
									{
										stencilDataObj.ComparisonBack = match.Groups[ 2 ].Value;
									}
								}
								break;
							case "PassBack":
								{
									if( match.Groups[ 2 ].Success && IsInlineProperty( match.Groups[ 2 ].Value, ref property ) )
									{
										stencilDataObj.PassBackInline = property;
									}
									else
									{
										stencilDataObj.PassBack = match.Groups[ 2 ].Value;
									}
								}
								break;
							case "FailBack":
								{
									if( match.Groups[ 2 ].Success && IsInlineProperty( match.Groups[ 2 ].Value, ref property ) )
									{
										stencilDataObj.FailBackInline = property;
									}
									else
									{
										stencilDataObj.FailBack = match.Groups[ 2 ].Value;
									}
								}
								break;
							case "ZFailBack":
								{
									if( match.Groups[ 2 ].Success && IsInlineProperty( match.Groups[ 2 ].Value, ref property ) )
									{
										stencilDataObj.ZFailBackInline = property;
									}
									else
									{
										stencilDataObj.ZFailBack = match.Groups[ 2 ].Value;
									}
								}
								break;
						}
					}
				}
			}
		}

		public static void CreateColorMask( string colorMaskData, ref TemplateColorMaskData colorMaskObj, string pattern )
		{
			colorMaskObj.DataCheck = TemplateDataCheck.Invalid;
			Match match = Regex.Match( colorMaskData, pattern );
			//if( match.Groups.Count == 3 /*&& !match.Groups[ 2 ].Success*/ ) // second group is the colormask MRT which isn't implemented yet
			{
				string property = string.Empty;
				if( match.Groups[ 1 ].Success && IsInlineProperty( match.Groups[ 1 ].Value, ref property ) )
				{
					colorMaskObj.InlineData = property;
					colorMaskObj.DataCheck = TemplateDataCheck.Valid;
					colorMaskObj.Target = match.Groups[ 2 ].Value;
				}
				else
				{
					for( int i = 0; i < 4; i++ )
					{
						colorMaskObj.ColorMaskData[ i ] = false;
					}

					colorMaskObj.Target = match.Groups[ 2 ].Value;
					colorMaskObj.DataCheck = TemplateDataCheck.Valid;
					try
					{
						for( int i = 0; i < match.Groups[ 1 ].Value.Length; i++ )
						{
							switch( Char.ToLower( match.Groups[ 1 ].Value[ i ] ) )
							{
								case 'r': colorMaskObj.ColorMaskData[ 0 ] = true; break;
								case 'g': colorMaskObj.ColorMaskData[ 1 ] = true; break;
								case 'b': colorMaskObj.ColorMaskData[ 2 ] = true; break;
								case 'a': colorMaskObj.ColorMaskData[ 3 ] = true; break;
								case '0':
									{
										for( int j = 0; j < 4; j++ )
										{
											colorMaskObj.ColorMaskData[ j ] = false;
										}
										return;
									}
								default:
									{
										colorMaskObj.DataCheck = TemplateDataCheck.Invalid;
										return;
									}
							}
						}
					}
					catch( Exception e )
					{
						Debug.LogException( e );
						colorMaskObj.DataCheck = TemplateDataCheck.Invalid;
						return;
					}
				}
			}
		}

		public static void CreateAlphaToMask( string alphaToMaskData, ref TemplateAlphaToMaskData alphaToMaskObj )
		{
			alphaToMaskObj.DataCheck = TemplateDataCheck.Invalid;
			Match match = Regex.Match( alphaToMaskData, AlphaToMaskPattern );
			if( match.Groups.Count == 2 )
			{
				string property = string.Empty;
				if( match.Groups[ 1 ].Success && IsInlineProperty( match.Groups[ 1 ].Value, ref property ) )
				{
					alphaToMaskObj.InlineData = property;
					alphaToMaskObj.DataCheck = TemplateDataCheck.Valid;
				}
				else
				{
					try
					{
						alphaToMaskObj.AlphaToMaskData = match.Groups[ 1 ].Value == "On" ? true : false;
						alphaToMaskObj.DataCheck = TemplateDataCheck.Valid;
					}
					catch( Exception e )
					{
						alphaToMaskObj.DataCheck = TemplateDataCheck.Invalid;
						Debug.LogException( e );
						return;
					}
				}
			}
		}

		public static void CreateCullMode( string cullModeData, ref TemplateCullModeData cullDataObj )
		{
			cullDataObj.DataCheck = TemplateDataCheck.Invalid;
			Match match = Regex.Match( cullModeData, CullModePattern );
			if( match.Groups.Count == 2 )
			{
				string property = string.Empty;
				if( match.Groups[ 1 ].Success && IsInlineProperty( match.Groups[ 1 ].Value, ref property ) )
				{
					cullDataObj.InlineData = property;
					cullDataObj.DataCheck = TemplateDataCheck.Valid;
				}
				else
				{
					try
					{
						cullDataObj.CullModeData = (CullMode)Enum.Parse( typeof( CullMode ), match.Groups[ 1 ].Value );
						cullDataObj.DataCheck = TemplateDataCheck.Valid;
					}
					catch( Exception e )
					{
						cullDataObj.DataCheck = TemplateDataCheck.Invalid;
						Debug.LogException( e );
						return;
					}
				}
			}
		}

		public static void CreateBlendMode( string blendModeData, ref TemplateBlendData blendDataObj, string pattern )
		{
			blendDataObj.ValidBlendMode = true;
			string property = string.Empty;
			bool noMatches = true;
			// TODO: OPTIMIZE REGEX EXPRESSIONS TO NOT CATCH EMPTY GROUPS
			Match match = Regex.Match( blendModeData, pattern );
			//Debug.Log( blendModeData + " | " + match.Groups.Count + "|" + match.Groups[ 0 ].Value + " " + match.Groups[ 0 ].Success + " " + match.Groups[ 1 ].Success + " " + match.Groups[ 2 ].Success + " " + match.Groups[ 3 ].Success + " " + match.Groups[ 4 ].Success + " " + match.Groups[ 5 ].Success );
			{
				if( match.Groups.Count <= 4 && match.Groups.Count >= 3 )
				{
					if( match.Groups[ 0 ].Success && match.Groups[ 2 ].Success )
					{
						try
						{
							if( IsInlineProperty( match.Groups[ 2 ].Value, ref property ) )
							{
								blendDataObj.SourceFactorRGBInline = property;
							}
							else
							{
								AvailableBlendFactor sourceAll = (AvailableBlendFactor)Enum.Parse( typeof( AvailableBlendFactor ), match.Groups[ 2 ].Value );
								blendDataObj.SourceFactorRGB = sourceAll;
							}
							if( match.Groups[ 3 ].Success && IsInlineProperty( match.Groups[ 3 ].Value, ref property ) )
							{
								blendDataObj.DestFactorRGBInline = property;
							}
							else
							{
								AvailableBlendFactor destAll = (AvailableBlendFactor)Enum.Parse( typeof( AvailableBlendFactor ), match.Groups[ 3 ].Value );
								blendDataObj.DestFactorRGB = destAll;
							}
							blendDataObj.Target = match.Groups[ 1 ].Value;
							blendDataObj.SeparateBlendFactors = false;
							blendDataObj.BlendModeOff = false;
							noMatches = false;
						}
						catch( Exception e )
						{
							Debug.LogException( e );
							blendDataObj.DataCheck = TemplateDataCheck.Invalid;
							blendDataObj.ValidBlendMode = false;
							return;
						}
					}
				}
				else if( match.Groups.Count >= 5 )
				{
					if( match.Groups[ 1 ].Success )
						blendDataObj.Target = match.Groups[ 1 ].Value;

					if( !match.Groups[ 2 ].Success && !match.Groups[ 3 ].Success )
					{
						if( match.Groups[ 0 ].Value.IndexOf("Off") > -1 )
						{
							blendDataObj.BlendModeOff = true;
							noMatches = false;
						}
					}

					if( match.Groups[ 0 ].Success && match.Groups[ 2 ].Success )
					{
						try
						{
							if( IsInlineProperty( match.Groups[ 2 ].Value, ref property ) )
							{
								blendDataObj.SourceFactorRGBInline = property;
							}
							else
							{
								AvailableBlendFactor sourceRGB = (AvailableBlendFactor)Enum.Parse( typeof( AvailableBlendFactor ), match.Groups[ 2 ].Value );
								blendDataObj.SourceFactorRGB = sourceRGB;
							}

							if( match.Groups[ 3 ].Success && IsInlineProperty( match.Groups[ 3 ].Value, ref property ) )
							{
								blendDataObj.DestFactorRGBInline = property;
							}
							else
							{
								AvailableBlendFactor destRGB = (AvailableBlendFactor)Enum.Parse( typeof( AvailableBlendFactor ), match.Groups[ 3 ].Value );
								blendDataObj.DestFactorRGB = destRGB;
							}

							if( match.Groups[ 4 ].Success && match.Groups[ 5 ].Success )
							{
								if( IsInlineProperty( match.Groups[ 4 ].Value, ref property ) )
								{
									blendDataObj.SourceFactorAlphaInline = property;
								}
								else
								{
									AvailableBlendFactor sourceA = (AvailableBlendFactor)Enum.Parse( typeof( AvailableBlendFactor ), match.Groups[ 4 ].Value );
									blendDataObj.SourceFactorAlpha = sourceA;
								}

								if( IsInlineProperty( match.Groups[ 5 ].Value, ref property ) )
								{
									blendDataObj.DestFactorAlphaInline = property;
								}
								else
								{
									AvailableBlendFactor destA = (AvailableBlendFactor)Enum.Parse( typeof( AvailableBlendFactor ), match.Groups[ 5 ].Value );
									blendDataObj.DestFactorAlpha = destA;
								}

								blendDataObj.SeparateBlendFactors = true;
							}
							else
							{
								blendDataObj.SeparateBlendFactors = false;
							}
							blendDataObj.Target = match.Groups[ 1 ].Value;
							blendDataObj.BlendModeOff = false;
							noMatches = false;
						}
						catch( Exception e )
						{
							Debug.LogException( e );
							blendDataObj.DataCheck = TemplateDataCheck.Invalid;
							blendDataObj.ValidBlendMode = false;
							return;
						}
					}
				}
			}

			if( noMatches )
				blendDataObj.ValidBlendMode = false;
		}

		public static void CreateBlendOp( string blendOpData, ref TemplateBlendData blendDataObj, string pattern )
		{
			bool noMatches = true;
			blendDataObj.ValidBlendOp = true;
			string property = string.Empty;
			// TODO: OPTIMIZE REGEX EXPRESSIONS TO NOT CATCH EMPTY GROUPS 
			Match match = Regex.Match( blendOpData, pattern );
			{
				if( match.Groups.Count == 3 )
				{
					if( match.Groups[ 0 ].Success && match.Groups[ 2 ].Success )
					{

						try
						{
							if( IsInlineProperty( match.Groups[ 2 ].Value, ref property ) )
							{
								blendDataObj.BlendOpRGBInline = property;
							}
							else
							{
								AvailableBlendOps blendOpsAll = (AvailableBlendOps)Enum.Parse( typeof( AvailableBlendOps ), match.Groups[ 2 ].Value );
								blendDataObj.BlendOpRGB = blendOpsAll;
							}
							blendDataObj.SeparateBlendOps = false;
							blendDataObj.Target = match.Groups[ 1 ].Value;
							noMatches = false;
						}
						catch( Exception e )
						{
							Debug.LogException( e );
							blendDataObj.DataCheck = TemplateDataCheck.Invalid;
							blendDataObj.ValidBlendOp = false;
							return;
						}
					}
				}
				else if( match.Groups.Count == 4 )
				{
					if( match.Groups[ 0 ].Success && match.Groups[ 2 ].Success )
					{
						try
						{
							if( IsInlineProperty( match.Groups[ 2 ].Value, ref property ) )
							{
								blendDataObj.BlendOpRGBInline = property;
							}
							else
							{
								AvailableBlendOps blendOpsRGB = (AvailableBlendOps)Enum.Parse( typeof( AvailableBlendOps ), match.Groups[ 2 ].Value );
								blendDataObj.BlendOpRGB = blendOpsRGB;
							}

							if( match.Groups[ 3 ].Success )
							{
								if( IsInlineProperty( match.Groups[ 3 ].Value, ref property ) )
								{
									blendDataObj.BlendOpAlphaInline = property;
								}
								else
								{
									AvailableBlendOps blendOpsA = (AvailableBlendOps)Enum.Parse( typeof( AvailableBlendOps ), match.Groups[ 3 ].Value );
									blendDataObj.BlendOpAlpha = blendOpsA;
								}
								blendDataObj.SeparateBlendOps = true;
							}
							else
							{
								blendDataObj.SeparateBlendOps = false;
							}
							blendDataObj.Target = match.Groups[ 1 ].Value;
							noMatches = false;
						}
						catch( Exception e )
						{
							Debug.LogException( e );
							blendDataObj.DataCheck = TemplateDataCheck.Invalid;
							blendDataObj.ValidBlendOp = false;
							return;
						}
					}
				}
			}

			if( noMatches )
				blendDataObj.ValidBlendOp = false;
		}

		public static void FetchLocalVars( string body, ref List<TemplateLocalVarData> localVarList, TemplateFunctionData vertexFunction, TemplateFunctionData fragFunction )
		{
			foreach( Match match in Regex.Matches( body, LocalVarPattern ) )
			{
				if( match.Groups.Count == 4 )
				{
					if( CgToWirePortType.ContainsKey( match.Groups[ 2 ].Value ) )
					{
						MasterNodePortCategory category;
						if( fragFunction.MainBodyLocalIdx > vertexFunction.MainBodyLocalIdx )
						{
							if( match.Index < fragFunction.MainBodyLocalIdx )
							{
								category = MasterNodePortCategory.Vertex;
							}
							else
							{
								category = MasterNodePortCategory.Fragment;
							}
						}
						else
						{
							if( match.Index < vertexFunction.MainBodyLocalIdx )
							{
								category = MasterNodePortCategory.Fragment;
							}
							else
							{
								category = MasterNodePortCategory.Vertex;
							}
						}

						if( !string.IsNullOrEmpty( match.Groups[ 1 ].Value ) && ShortcutToInfo.ContainsKey( match.Groups[ 1 ].Value ) )
						{
							string id = match.Groups[ 0 ].Value.Substring( 0, match.Groups[ 0 ].Value.IndexOf( "*/" ) + 2 );
							TemplateLocalVarData data = new TemplateLocalVarData( ShortcutToInfo[ match.Groups[ 1 ].Value ], id, CgToWirePortType[ match.Groups[ 2 ].Value ], category, match.Groups[ 3 ].Value, match.Index );
							localVarList.Add( data );
						}
						else
						{
							TemplateLocalVarData data = new TemplateLocalVarData( CgToWirePortType[ match.Groups[ 2 ].Value ], category, match.Groups[ 3 ].Value, match.Index );
							localVarList.Add( data );
						}

					}
				}
			}
		}

		public static void FetchInlineVars( string body, ref TemplateIdManager idManager )
		{
			foreach( Match match in Regex.Matches( body, InlinePattern ) )
			{
				if( match.Success && match.Groups.Count == 2 )
				{
					string id = match.Groups[ 0 ].Value;
					string prop = match.Groups[ 1 ].Value;
					idManager.RegisterTag( id, prop );
				}
			}
		}

		public static TemplateSRPType CreateTags( ref TemplateTagsModuleData tagsObj, bool isSubShader )
		{
			TemplateSRPType srpType = TemplateSRPType.BuiltIn;
			MatchCollection matchColl = Regex.Matches( tagsObj.TagsId, TagsPattern, RegexOptions.IgnorePatternWhitespace );
			int count = matchColl.Count;
			if( count > 0 )
			{
				for( int i = 0; i < count; i++ )
				{
					if( matchColl[ i ].Groups.Count == 3 )
					{
						if( isSubShader && matchColl[ i ].Groups[ 1 ].Value.Equals( "RenderPipeline" ) )
						{
							if( TagToRenderPipeline.ContainsKey( matchColl[ i ].Groups[ 2 ].Value ) )
								srpType = TagToRenderPipeline[ matchColl[ i ].Groups[ 2 ].Value ];
						}
						tagsObj.Tags.Add( new TemplatesTagData( matchColl[ i ].Groups[ 1 ].Value, matchColl[ i ].Groups[ 2 ].Value ) );
					}
				}
			}
			return srpType;
		}

		public static void CreateZWriteMode( string zWriteData, ref TemplateDepthData depthDataObj )
		{
			depthDataObj.DataCheck = TemplateDataCheck.Invalid;
			Match match = Regex.Match( zWriteData, ZWritePattern );
			if( match.Groups.Count == 2 )
			{
				string property = string.Empty;
				if( match.Groups[ 1 ].Success && IsInlineProperty( match.Groups[ 1 ].Value, ref property ) )
				{
					depthDataObj.ZWriteInlineValue = property;
					depthDataObj.DataCheck = TemplateDataCheck.Valid;
					depthDataObj.ValidZWrite = true;
				}
				else
				{
					try
					{
						depthDataObj.ZWriteModeValue = (ZWriteMode)Enum.Parse( typeof( ZWriteMode ), match.Groups[ 1 ].Value );
						depthDataObj.DataCheck = TemplateDataCheck.Valid;
						depthDataObj.ValidZWrite = true;
					}
					catch
					{
						depthDataObj.DataCheck = TemplateDataCheck.Invalid;
					}
				}
			}
		}

		public static void CreateZTestMode( string zTestData, ref TemplateDepthData depthDataObj )
		{
			depthDataObj.DataCheck = TemplateDataCheck.Invalid;
			Match match = Regex.Match( zTestData, ZTestPattern );
			if( match.Groups.Count == 2 )
			{
				string property = string.Empty;
				if( match.Groups[ 1 ].Success && IsInlineProperty( match.Groups[ 1 ].Value, ref property ) )
				{
					depthDataObj.ZTestInlineValue = property;
					depthDataObj.DataCheck = TemplateDataCheck.Valid;
					depthDataObj.ValidZTest = true;
				}
				else
				{
					try
					{
						depthDataObj.ZTestModeValue = (ZTestMode)Enum.Parse( typeof( ZTestMode ), match.Groups[ 1 ].Value );
						depthDataObj.DataCheck = TemplateDataCheck.Valid;
						depthDataObj.ValidZTest = true;
					}
					catch
					{
						depthDataObj.DataCheck = TemplateDataCheck.Invalid;
					}
				}
			}
		}

		public static void CreateZOffsetMode( string zOffsetData, ref TemplateDepthData depthDataObj )
		{
			depthDataObj.DataCheck = TemplateDataCheck.Invalid;
			Match match = Regex.Match( zOffsetData, ZOffsetPattern );
			if( match.Groups.Count == 3 )
			{
				try
				{
					string property = string.Empty;

					if( match.Groups[ 1 ].Success && IsInlineProperty( match.Groups[ 1 ].Value, ref property ) )
					{
						depthDataObj.OffsetFactorInlineValue = property;
					}
					else
					{
						depthDataObj.OffsetFactor = Convert.ToSingle( match.Groups[ 1 ].Value );
					}

					if( match.Groups[ 2 ].Success && IsInlineProperty( match.Groups[ 2 ].Value, ref property ) )
					{
						depthDataObj.OffsetUnitsInlineValue = property;
					}
					else
					{
						depthDataObj.OffsetUnits = Convert.ToSingle( match.Groups[ 2 ].Value );
					}

					depthDataObj.ValidOffset = true;
					depthDataObj.DataCheck = TemplateDataCheck.Valid;
				}
				catch
				{
					depthDataObj.DataCheck = TemplateDataCheck.Invalid;
				}
			}
		}

		public static List<TemplateVertexData> CreateVertexDataList( string vertexData, string parametersBody )
		{
			List<TemplateVertexData> vertexDataList = null;
			Dictionary<TemplateSemantics, TemplateVertexData> vertexDataDict = null;

			foreach( Match match in Regex.Matches( vertexData, VertexDataPattern ) )
			{
				if( match.Groups.Count > 1 )
				{
					if( vertexDataList == null )
					{
						vertexDataList = new List<TemplateVertexData>();
						vertexDataDict = new Dictionary<TemplateSemantics, TemplateVertexData>();
					}

					WirePortDataType dataType = CgToWirePortType[ match.Groups[ 1 ].Value ];
					string varName = match.Groups[ 2 ].Value;
					TemplateSemantics semantics = TemplateSemantics.NONE;
					try
					{
						semantics = (TemplateSemantics)Enum.Parse( typeof( TemplateSemantics ), match.Groups[ 3 ].Value );
					}
					catch(Exception e) 
					{
						Debug.LogException( e );
					}
					TemplateVertexData templateVertexData = new TemplateVertexData( semantics, dataType, varName );
					vertexDataList.Add( templateVertexData );
					vertexDataDict.Add( semantics, templateVertexData );
				}
			}

			if( vertexData.Contains( Constants.InstanceIdMacro ) )
			{
				if( vertexDataList == null )
				{
					vertexDataList = new List<TemplateVertexData>();
					vertexDataDict = new Dictionary<TemplateSemantics , TemplateVertexData>();
				}
				TemplateVertexData templateVertexData = new TemplateVertexData( TemplateSemantics.SV_InstanceID, WirePortDataType.UINT, Constants.InstanceIdVariable );
				templateVertexData.DataInfo = TemplateInfoOnSematics.INSTANCE_ID;
				templateVertexData.Available = true;
				templateVertexData.ExcludeStructPrefix = true;

				vertexDataList.Add( templateVertexData );
				vertexDataDict.Add( TemplateSemantics.SV_InstanceID, templateVertexData );
			}

			if( !string.IsNullOrEmpty( parametersBody ) )
			{
				string[] paramsArray = parametersBody.Split( IOUtils.FIELD_SEPARATOR );
				if( paramsArray.Length > 0 )
				{
					for( int i = 0; i < paramsArray.Length; i++ )
					{
						string[] paramDataArr = paramsArray[ i ].Split( IOUtils.VALUE_SEPARATOR );
						if( paramDataArr.Length == 2 )
						{
							string[] swizzleInfoArr = paramDataArr[ 1 ].Split( IOUtils.FLOAT_SEPARATOR );
							TemplateSemantics semantic = ShortcutToSemantic[ swizzleInfoArr[ 0 ] ];
							if( vertexDataDict.ContainsKey( semantic ) )
							{
								TemplateVertexData templateVertexData = vertexDataDict[ semantic ];
								if( templateVertexData != null )
								{
									if( swizzleInfoArr.Length > 1 )
									{
										templateVertexData.DataSwizzle = "." + swizzleInfoArr[ 1 ];
									}
									templateVertexData.DataInfo = ShortcutToInfo[ paramDataArr[ 0 ] ];
									templateVertexData.Available = true;
								}
							}
						}
					}
				}
			}

			if( vertexDataDict != null )
			{
				vertexDataDict.Clear();
				vertexDataDict = null;
			}

			return vertexDataList;
		}

		public static TemplateInterpData CreateInterpDataList( string interpData, string fullLine, int maxInterpolators )
		{
			TemplateInterpData interpDataObj = null;
			List<TemplateVertexData> interpDataList = null;
			Dictionary<TemplateSemantics, TemplateVertexData> interpDataDict = null;
			Match rangeMatch = Regex.Match( fullLine, InterpRangePattern );
			if( rangeMatch.Groups.Count > 0 )
			{
				interpDataObj = new TemplateInterpData();
				// Get range of available interpolators
				int minVal = 0;
				int maxVal = 0;
				try
				{
					string[] minValArgs = rangeMatch.Groups[ 1 ].Value.Split( IOUtils.FLOAT_SEPARATOR );
					minVal = Convert.ToInt32( minValArgs[ 0 ] );
					if( string.IsNullOrEmpty( rangeMatch.Groups[ 2 ].Value ) )
					{
						maxVal = maxInterpolators - 1;
						interpDataObj.DynamicMax = true;
					}
					else
					{
						maxVal = Convert.ToInt32( rangeMatch.Groups[ 2 ].Value );
					}
					if( minVal > maxVal )
					{
						//int aux = minVal;
						//minVal = maxVal;
						//maxVal = aux;
						maxVal = minVal;
					}

					for( int i = minVal; i <= maxVal; i++ )
					{
						interpDataObj.AvailableInterpolators.Add( new TemplateInterpElement( IntToSemantic[ i ] ) );
					}
					if( minValArgs.Length > 1 )
					{
						interpDataObj.AvailableInterpolators[ 0 ].SetAvailableChannelsFromString( minValArgs[ 1 ] );
					}
				}
				catch( Exception e )
				{
					Debug.LogException( e );
				}

				interpDataList = new List<TemplateVertexData>();
				interpDataDict = new Dictionary<TemplateSemantics, TemplateVertexData>();

				//Get Current interpolators
				int parametersBeginIdx = fullLine.IndexOf( ":" ) + 1;
				int parametersEnd = fullLine.IndexOf( TemplatesManager.TemplateEndOfLine );
				string parametersBody = fullLine.Substring( parametersBeginIdx, parametersEnd - parametersBeginIdx );

				foreach( Match match in Regex.Matches( interpData, VertexDataPattern ) )
				{
					if( match.Groups.Count > 1 )
					{
						WirePortDataType dataType = CgToWirePortType[ match.Groups[ 1 ].Value ];
						string varName = match.Groups[ 2 ].Value;
						TemplateSemantics semantics = TemplateSemantics.NONE;
						try
						{
							semantics = (TemplateSemantics)Enum.Parse( typeof( TemplateSemantics ), match.Groups[ 3 ].Value );
						}
						catch( Exception e )
						{
							Debug.LogException( e );
						}

						TemplateVertexData templateVertexData = new TemplateVertexData( semantics, dataType, varName );
						//interpDataList.Add( templateVertexData );
						interpDataDict.Add( semantics, templateVertexData );
						interpDataObj.RawInterpolators.Add( templateVertexData );
						//Check if they are also on the free channels list and update their names
						interpDataObj.ReplaceNameOnInterpolator( semantics, varName );
					}
				}

				if( interpData.Contains( Constants.InstanceIdMacro ) )
				{
					TemplateVertexData templateInterpData = new TemplateVertexData( TemplateSemantics.SV_InstanceID, WirePortDataType.UINT, Constants.InstanceIdVariable );
					templateInterpData.DataInfo = TemplateInfoOnSematics.INSTANCE_ID;
					templateInterpData.Available = true;
					templateInterpData.ExcludeStructPrefix = true;
					interpDataList.Add( templateInterpData );
					interpDataDict.Add( TemplateSemantics.SV_InstanceID, templateInterpData );
				}

				Dictionary<string, TemplateVertexData> auxDict = new Dictionary<string, TemplateVertexData>();
				// Get info for available interpolators
				string[] paramsArray = parametersBody.Split( IOUtils.FIELD_SEPARATOR );
				if( paramsArray.Length > 0 )
				{
					for( int i = 0; i < paramsArray.Length; i++ )
					{
						string[] paramDataArr = paramsArray[ i ].Split( IOUtils.VALUE_SEPARATOR );
						if( paramDataArr.Length == 2 )
						{
							string[] swizzleInfoArr = paramDataArr[ 1 ].Split( IOUtils.FLOAT_SEPARATOR );
							TemplateSemantics semantic = ShortcutToSemantic[ swizzleInfoArr[ 0 ] ];
							if( interpDataDict.ContainsKey( semantic ) )
							{
								if( interpDataDict[ semantic ] != null )
								{
									string[] multiComponent = paramDataArr[ 0 ].Split( IOUtils.FLOAT_SEPARATOR );

									if( multiComponent.Length > 1 )
									{
										TemplateVertexData templateInterpData = null;
										if( auxDict.ContainsKey( multiComponent[ 0 ] ) )
										{
											templateInterpData = auxDict[ multiComponent[ 0 ] ];
										}
										else
										{
											templateInterpData = new TemplateVertexData( interpDataDict[ semantic ] );
											//if( swizzleInfoArr.Length > 1 )
											//{
											//	templateInterpData.DataSwizzle = "." + swizzleInfoArr[ 1 ];
											//}
											templateInterpData.DataInfo = ShortcutToInfo[ multiComponent[ 0 ] ];
											templateInterpData.Available = true;
											interpDataList.Add( templateInterpData );
											auxDict.Add( multiComponent[ 0 ], templateInterpData );
										}

										if( swizzleInfoArr[ 1 ].Length == multiComponent[ 1 ].Length )
										{
											for( int channelIdx = 0; channelIdx < swizzleInfoArr[ 1 ].Length; channelIdx++ )
											{
												templateInterpData.RegisterComponent( multiComponent[ 1 ][ channelIdx ], interpDataDict[ semantic ].VarName + "." + swizzleInfoArr[ 1 ][ channelIdx ] );
											}
										}
									}
									else
									{
										TemplateVertexData templateInterpData = new TemplateVertexData( interpDataDict[ semantic ] );
										if( swizzleInfoArr.Length > 1 )
										{
											templateInterpData.DataSwizzle = "." + swizzleInfoArr[ 1 ];
										}
										templateInterpData.DataInfo = ShortcutToInfo[ paramDataArr[ 0 ] ];
										templateInterpData.Available = true;
										interpDataList.Add( templateInterpData );
									}
								}
							}
						}
					}
				}

				/*TODO: 
				1) Remove interpDataList.Add( templateVertexData ); from initial foreach 
				2) When looping though each foreach array element, create a new TemplateVertexData
				from the one containted on the interpDataDict and add it to interpDataList
				*/
				for( int i = 0; i < interpDataList.Count; i++ )
				{
					interpDataList[ i ].BuildVar();
				}

				auxDict.Clear();
				auxDict = null;

				interpDataObj.Interpolators = interpDataList;
				interpDataDict.Clear();
				interpDataDict = null;
			}
			return interpDataObj;
		}

		public static void FetchDependencies( TemplateInfoContainer dependencies, ref string body )
		{
			int index = body.IndexOf( TemplatesManager.TemplateDependenciesListTag );
			if( index > 0 )
			{
				dependencies.Index = index;
				dependencies.Id = TemplatesManager.TemplateDependenciesListTag;
				dependencies.Data = TemplatesManager.TemplateDependenciesListTag;
			}
			else
			{
				int lastIndex = body.LastIndexOf( '}' );
				if( lastIndex > 0 )
				{
					body = body.Insert( lastIndex, "\t" + TemplatesManager.TemplateDependenciesListTag + "\n" );
					FetchDependencies( dependencies, ref body );
				}
			}
		}

		public static void FetchCustomInspector( TemplateInfoContainer inspectorContainer, ref string body )
		{
			Match match = Regex.Match( body, CustomInspectorPattern, RegexOptions.Multiline );
			if( match != null && match.Groups.Count > 1 )
			{
				inspectorContainer.Index = match.Index;
				inspectorContainer.Id = match.Groups[ 0 ].Value;
				inspectorContainer.Data = match.Groups[ 1 ].Value;

#if UNITY_2019_3_OR_NEWER
				if( ASEPackageManagerHelper.CurrentHDVersion > ASESRPVersions.ASE_SRP_6_9_1 )
				{
					if( inspectorContainer.Data.Equals( "UnityEditor.Experimental.Rendering.HDPipeline.HDLitGUI" ) )
						inspectorContainer.Data = "UnityEditor.Rendering.HighDefinition.HDLitGUI";
				}
#endif
			}
			else
			{
				int index = body.LastIndexOf( '}' );
				if( index > 0 )
				{
					body = body.Insert( index, string.Format( "\tCustomEditor \"{0}\"\n", Constants.DefaultCustomInspector ) );
					FetchCustomInspector( inspectorContainer, ref body );
				}
			}
		}

		public static void FetchFallback( TemplateInfoContainer fallbackContainer, ref string body )
		{
			Match match = Regex.Match( body, FallbackPattern, RegexOptions.Multiline | RegexOptions.IgnoreCase );
			if( match != null && match.Groups.Count > 1 )
			{
				fallbackContainer.Index = match.Index;
				fallbackContainer.Id = match.Groups[ 0 ].Value;
				fallbackContainer.Data = match.Groups[ 1 ].Value;
			}
			else
			{
				int index = body.LastIndexOf( '}' );
				if( index > 0 )
				{
					body = body.Insert( index, "\tFallback \"\"\n" );
					FetchFallback( fallbackContainer, ref body );
				}
			}
		}

		public static string AutoSwizzleData( string dataVar, WirePortDataType from, WirePortDataType to, bool isPosition )
		{
			switch( from )
			{
				case WirePortDataType.COLOR:
				case WirePortDataType.FLOAT4:
					{
						switch( to )
						{
							case WirePortDataType.FLOAT3: dataVar += ".xyz"; break;
							case WirePortDataType.FLOAT2: dataVar += ".xy"; break;
							case WirePortDataType.INT:
							case WirePortDataType.FLOAT: dataVar += ".x"; break;
						}
					}
					break;
				case WirePortDataType.FLOAT3:
					{
						switch( to )
						{
							case WirePortDataType.FLOAT4: dataVar = string.Format( "float4({0},{1})", dataVar,(isPosition?1:0) ); break;
							case WirePortDataType.FLOAT2: dataVar += ".xy"; break;
							case WirePortDataType.INT:
							case WirePortDataType.FLOAT: dataVar += ".x"; break;
						}
					}
					break;
				case WirePortDataType.FLOAT2:
					{
						switch( to )
						{
							case WirePortDataType.FLOAT4: dataVar = string.Format( "float4({0},0,{1})", dataVar , (isPosition ? 1 : 0) ); break;
							case WirePortDataType.FLOAT3: dataVar = string.Format( "float3({0},0)", dataVar ); break;
							case WirePortDataType.INT:
							case WirePortDataType.FLOAT: dataVar += ".x"; break;
						}
					}
					break;
				case WirePortDataType.FLOAT:
					{
						switch( to )
						{
							case WirePortDataType.FLOAT4: dataVar = string.Format( "float4({0},0,0,{1})", dataVar, ( isPosition ? 1 : 0 ) ); break;
							case WirePortDataType.FLOAT3: dataVar = string.Format( "float3({0},0,0)", dataVar ); break;
							case WirePortDataType.FLOAT2: dataVar = string.Format( "float2({0},0)", dataVar ); break;
						}
					}
					break;
			}
			return dataVar;
		}

		public static bool CheckIfTemplate( string assetPath )
		{
			if( assetPath.EndsWith( ".shader" ) )
			{
				if( File.Exists( assetPath ) )
				{
					string body = IOUtils.LoadTextFileFromDisk( assetPath );
					return ( body.IndexOf( TemplatesManager.TemplateShaderNameBeginTag ) > -1 );
				}
			}
			return false;
		}

		public static bool CheckIfCompatibles( WirePortDataType first, WirePortDataType second )
		{
			switch( first )
			{
				case WirePortDataType.OBJECT:
				return true;
				case WirePortDataType.FLOAT:
				case WirePortDataType.FLOAT2:
				case WirePortDataType.FLOAT3:
				case WirePortDataType.FLOAT4:
				case WirePortDataType.COLOR:
				case WirePortDataType.INT:
				{
					switch( second )
					{
						case WirePortDataType.FLOAT3x3:
						case WirePortDataType.FLOAT4x4:
						case WirePortDataType.SAMPLER1D:
						case WirePortDataType.SAMPLER2D:
						case WirePortDataType.SAMPLER3D:
						case WirePortDataType.SAMPLERCUBE:
						case WirePortDataType.SAMPLER2DARRAY:
						case WirePortDataType.SAMPLERSTATE:
						return false;
					}
				}
				break;
				case WirePortDataType.FLOAT3x3:
				case WirePortDataType.FLOAT4x4:
				{
					switch( second )
					{
						case WirePortDataType.FLOAT:
						case WirePortDataType.FLOAT2:
						case WirePortDataType.FLOAT3:
						case WirePortDataType.FLOAT4:
						case WirePortDataType.COLOR:
						case WirePortDataType.INT:
						case WirePortDataType.SAMPLER1D:
						case WirePortDataType.SAMPLER2D:
						case WirePortDataType.SAMPLER3D:
						case WirePortDataType.SAMPLERCUBE:
						case WirePortDataType.SAMPLER2DARRAY:
						case WirePortDataType.SAMPLERSTATE:
						return false;
					}
				}
				break;
				case WirePortDataType.SAMPLER1D:
				case WirePortDataType.SAMPLER2D:
				case WirePortDataType.SAMPLER3D:
				case WirePortDataType.SAMPLERCUBE:
				case WirePortDataType.SAMPLER2DARRAY:
				{
					switch( second )
					{
						case WirePortDataType.FLOAT:
						case WirePortDataType.FLOAT2:
						case WirePortDataType.FLOAT3:
						case WirePortDataType.FLOAT4:
						case WirePortDataType.FLOAT3x3:
						case WirePortDataType.FLOAT4x4:
						case WirePortDataType.COLOR:
						case WirePortDataType.INT:
						case WirePortDataType.SAMPLERSTATE:
						return false;
					}
				}
				break;
				case WirePortDataType.SAMPLERSTATE:
				{
					switch( second )
					{
						default:
						return false;
						case WirePortDataType.SAMPLERSTATE:
						case WirePortDataType.OBJECT:
						break;
					}
				}
				break;
			}
			return true;
		}
		// Lightweight <-> Default functions
		public static string WorldSpaceViewDir( MasterNodeDataCollector dataCollector, string worldPosVec3, bool normalize )
		{
			string value = string.Empty;
			if( dataCollector.IsTemplate && dataCollector.IsSRP )
			{
				value = string.Format( "_WorldSpaceCameraPos.xyz - {0}", worldPosVec3 );
			}
			else
			{
				value = string.Format( "UnityWorldSpaceViewDir( {0} )", worldPosVec3 );
			}

			if( normalize )
			{
				value = SafeNormalize( dataCollector, value );
			}

			return value;
		}

		public static string SafeNormalize( MasterNodeDataCollector dataCollector, string value )
		{
			if( dataCollector.IsTemplate && dataCollector.IsSRP )
			{
				value = string.Format( "SafeNormalize( {0} )", value );
			}
			else
			{
				dataCollector.AddToIncludes( -1, Constants.UnityBRDFLib );
				value = string.Format( "Unity_SafeNormalize( {0} )", value );
			}
			return value;
		}

		public static bool IsInlineProperty( string data, ref string property )
		{
			if( data.Length > 0 && data[ 0 ] == '[' && data[ data.Length - 1 ] == ']' )
			{
				property = data.Substring( 1, data.Length - 2 );
				return true;
			}
			return false;
		}

	//	public static readonly string FetchDefaultDepthFormat = "UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD( {0} )))";
		public static readonly string FetchDefaultDepthFormat = "SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, {0}.xy )";
		public static readonly string FetchDefaultDepthFormatVertex = "SAMPLE_DEPTH_TEXTURE_LOD( _CameraDepthTexture, float4( {0}.xy, 0, 0 ) )";

		public static readonly string FetchLWDepthFormat = "SHADERGRAPH_SAMPLE_SCENE_DEPTH( {0}.xy )";
		public static readonly string FetchLWDepthFormatVertex = "SHADERGRAPH_SAMPLE_SCENE_DEPTH_LOD( {0}.xy )";
#if UNITY_2018_3_OR_NEWER
		public static readonly string FetchHDDepthFormat = "SampleCameraDepth( {0}.xy )";
#else
		public static readonly string FetchHDDepthFormat = "SAMPLE_TEXTURE2D( _CameraDepthTexture, s_point_clamp_sampler, {0}.xy ).r";
#endif
		public static string CreateDepthFetch( MasterNodeDataCollector dataCollector, string screenPos )
		{
			string screenDepthInstruction = string.Empty;
			if( dataCollector.IsTemplate && dataCollector.IsSRP )
			{
				if( dataCollector.TemplateDataCollectorInstance.CurrentSRPType == TemplateSRPType.Lightweight )
				{
					if( dataCollector.PortCategory == MasterNodePortCategory.Vertex )
					{
						string m_functionBody = string.Empty;
						GenerateLW( ref m_functionBody );
						dataCollector.AddFunctions( FetchLWDepthFormatVertex, m_functionBody, "0" );
						screenDepthInstruction = string.Format( FetchLWDepthFormatVertex, screenPos );
					}
					else
						screenDepthInstruction = string.Format( FetchLWDepthFormat, screenPos );
				}
				else if( dataCollector.TemplateDataCollectorInstance.CurrentSRPType == TemplateSRPType.HD )
					screenDepthInstruction = string.Format( FetchHDDepthFormat, screenPos );
			}
			else
			{
				if( dataCollector.PortCategory == MasterNodePortCategory.Vertex )
					screenDepthInstruction = string.Format( FetchDefaultDepthFormatVertex, screenPos );
				else
					screenDepthInstruction = string.Format( FetchDefaultDepthFormat, screenPos );
			}
			return screenDepthInstruction;
		}

		public static void GenerateLW( ref string body )
		{
			body = string.Empty;
			IOUtils.AddFunctionHeader( ref body, "float SHADERGRAPH_SAMPLE_SCENE_DEPTH_LOD(float2 uv)" );
			IOUtils.AddFunctionLine( ref body, "#if defined(REQUIRE_DEPTH_TEXTURE)" );
			IOUtils.AddFunctionLine( ref body, "#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)" );
			IOUtils.AddFunctionLine( ref body, " \tfloat rawDepth = SAMPLE_TEXTURE2D_ARRAY_LOD(_CameraDepthTexture, sampler_CameraDepthTexture, uv, unity_StereoEyeIndex, 0).r;" );
			IOUtils.AddFunctionLine( ref body, "#else" );
			IOUtils.AddFunctionLine( ref body, " \tfloat rawDepth = SAMPLE_DEPTH_TEXTURE_LOD(_CameraDepthTexture, sampler_CameraDepthTexture, uv, 0);" );
			IOUtils.AddFunctionLine( ref body, "#endif" );
			IOUtils.AddFunctionLine( ref body, "return rawDepth;" );
			IOUtils.AddFunctionLine( ref body, "#endif // REQUIRE_DEPTH_TEXTURE" );
			IOUtils.AddFunctionLine( ref body, "return 0;" );
			IOUtils.CloseFunctionBody( ref body );
		}

		public static bool GetShaderModelForInterpolatorAmount( int interpAmount, ref string shaderModel )
		{
			for( int i = 0; i < AvailableShaderModels.Length; i++ )
			{
				if( AvailableInterpolators[ AvailableShaderModels[ i ] ] >= interpAmount )
				{
					shaderModel = AvailableShaderModels[ i ];
					return true;
				}
			}
			return false;
		}

		public static string GetSubShaderFrom( string shaderBody )
		{
			Match match = Regex.Match( shaderBody, FetchSubShaderBody, RegexOptions.Singleline );
			if( match.Success && match.Groups.Count > 1 )
			{
				return match.Groups[ 1 ].Value;
			}

			return string.Empty;
		}

		public static void FillRenderingPlatform( TemplateRenderPlatformHelper renderPlatforms , string shaderBody )
		{
			int tagIndex = shaderBody.IndexOf( TemplatesManager.TemplateRenderPlatformsTag );
			if(  tagIndex > -1 )
			{
				renderPlatforms.InitByTag( tagIndex );
			}
			else
			{
				//Excluded
				Match excludePlatformsMatch = Regex.Match( shaderBody , PragmaExcludeRendersPattern );
				if( excludePlatformsMatch.Success )
				{
					renderPlatforms.InitByExcludeRenders( excludePlatformsMatch.Index, excludePlatformsMatch.Value );
					MatchCollection platformElements = Regex.Matches( excludePlatformsMatch.Groups[ 1 ].Value , PragmaRendererElement );
					try
					{
						for( int i = 0 ; i < platformElements.Count ; i++ )
						{
							if( platformElements[ i ].Success )
								renderPlatforms.SetupPlatform( platformElements[ i ].Groups[ 1 ].Value , false );
						}
					}
					catch( Exception e )
					{
						Debug.LogException( e );
					}
				}
				else //Only Renders
				{
					Match onlyRendersPlatformsMatch = Regex.Match( shaderBody , PragmaOnlyRendersPattern );
					if( onlyRendersPlatformsMatch.Success )
					{
						renderPlatforms.InitByOnlyRenders( onlyRendersPlatformsMatch.Index, onlyRendersPlatformsMatch.Value );
						MatchCollection platformElements = Regex.Matches( onlyRendersPlatformsMatch.Groups[ 1 ].Value , PragmaRendererElement );
						try
						{
							for( int i = 0 ; i < platformElements.Count ; i++ )
							{
								if( platformElements[ i ].Success )
									renderPlatforms.SetupPlatform( platformElements[ i ].Groups[ 1 ].Value, true );
							}
						}
						catch( Exception e )
						{
							Debug.LogException( e );
						}
					}
				}

			}
		}
	}
}
