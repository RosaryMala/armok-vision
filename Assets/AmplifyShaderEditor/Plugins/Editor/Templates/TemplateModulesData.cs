// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine;

namespace AmplifyShaderEditor
{
	public enum TemplateModuleDataType
	{
		ModuleShaderModel,
		ModuleBlendMode,
		ModuleBlendMode1,
		ModuleBlendMode2,
		ModuleBlendMode3,
		ModuleBlendOp,
		ModuleBlendOp1,
		ModuleBlendOp2,
		ModuleBlendOp3,
		ModuleAlphaToMask,
		ModuleCullMode,
		ModuleColorMask,
		ModuleColorMask1,
		ModuleColorMask2,
		ModuleColorMask3,
		ModuleStencil,
		ModuleZwrite,
		ModuleZTest,
		ModuleZOffset,
		ModuleTag,
		ModuleGlobals,
		ModuleSRPBatcher,
		ModuleFunctions,
		ModulePragma,
		ModulePragmaBefore,
		ModulePass,
		ModuleInputVert,
		ModuleInputFrag,
		PassVertexFunction,
		PassFragmentFunction,
		PassVertexData,
		PassInterpolatorData,
		PassNameData,
		AllModules,
		VControl,
		ControlData,
		DomainData,
		ModuleRenderPlatforms
		//EndPass
	}

	public enum TemplateSRPType
	{
		BuiltIn,
		HD,
		Lightweight
	}

	[Serializable]
	public class TemplateModulesData
	{
		[SerializeField]
		TemplateRenderPlatformHelper m_renderPlatformHelper;

		[SerializeField]
		private TemplateBlendData m_blendData = new TemplateBlendData();

		[SerializeField]
		private TemplateBlendData m_blendData1 = new TemplateBlendData();

		[SerializeField]
		private TemplateBlendData m_blendData2 = new TemplateBlendData();

		[SerializeField]
		private TemplateBlendData m_blendData3 = new TemplateBlendData();

		[SerializeField]
		private TemplateAlphaToMaskData m_alphaToMaskData = new TemplateAlphaToMaskData();

		[SerializeField]
		private TemplateCullModeData m_cullModeData = new TemplateCullModeData();

		[SerializeField]
		private TemplateColorMaskData m_colorMaskData = new TemplateColorMaskData();

		[SerializeField]
		private TemplateColorMaskData m_colorMaskData1 = new TemplateColorMaskData();

		[SerializeField]
		private TemplateColorMaskData m_colorMaskData2 = new TemplateColorMaskData();

		[SerializeField]
		private TemplateColorMaskData m_colorMaskData3 = new TemplateColorMaskData();

		[SerializeField]
		private TemplateStencilData m_stencilData = new TemplateStencilData();

		[SerializeField]
		private TemplateDepthData m_depthData = new TemplateDepthData();

		[SerializeField]
		private TemplateTagsModuleData m_tagData = new TemplateTagsModuleData();

		[SerializeField]
		private TemplateTagData m_globalsTag = new TemplateTagData( TemplatesManager.TemplateGlobalsTag, true );

		[SerializeField]
		private TemplateTagData m_srpBatcherTag = new TemplateTagData( TemplatesManager.TemplateSRPBatcherTag, true );

		[SerializeField]
		private TemplateTagData m_allModulesTag = new TemplateTagData( TemplatesManager.TemplateAllModulesTag, true );

		[SerializeField]
		private TemplateTagData m_functionsTag = new TemplateTagData( TemplatesManager.TemplateFunctionsTag, true );

		[SerializeField]
		private TemplateTagData m_pragmaTag = new TemplateTagData( TemplatesManager.TemplatePragmaTag, true );

		[SerializeField]
		private TemplateTagData m_pragmaBeforeTag = new TemplateTagData( TemplatesManager.TemplatePragmaBeforeTag, true );

		[SerializeField]
		private TemplateTagData m_passTag = new TemplateTagData( TemplatesManager.TemplatePassTag, true );

		[SerializeField]
		private TemplateTagData m_inputsVertTag = new TemplateTagData( TemplatesManager.TemplateInputsVertParamsTag, false );

		[SerializeField]
		private TemplateTagData m_inputsFragTag = new TemplateTagData( TemplatesManager.TemplateInputsFragParamsTag, false );

		[SerializeField]
		private TemplateShaderModelData m_shaderModel = new TemplateShaderModelData();

		[SerializeField]
		private TemplateSRPType m_srpType = TemplateSRPType.BuiltIn;

		[SerializeField]
		private bool m_srpIsPBR = false;

		[SerializeField]
		private string m_uniquePrefix;

		[SerializeField]
		private TemplateIncludePragmaContainter m_includePragmaContainer = new TemplateIncludePragmaContainter();

		[SerializeField]
		private bool m_allModulesMode = false;

		[SerializeField]
		private string m_passUniqueName = string.Empty;

		public void Destroy()
		{
			m_renderPlatformHelper.Destroy();
			m_renderPlatformHelper = null;

			m_blendData = null;
			m_blendData1 = null;
			m_blendData2 = null;
			m_blendData3 = null;
			m_alphaToMaskData = null;
			m_cullModeData = null;
			m_colorMaskData = null;
			m_colorMaskData1 = null;
			m_colorMaskData2 = null;
			m_colorMaskData3 = null;
			m_stencilData = null;
			m_depthData = null;
			m_tagData.Destroy();
			m_tagData = null;
			m_globalsTag = null;
			m_srpBatcherTag = null;
			m_allModulesTag = null;
			m_functionsTag = null;
			m_pragmaTag = null;
			m_pragmaBeforeTag = null;
			m_passTag = null;
			m_inputsVertTag = null;
			m_inputsFragTag = null;
			m_includePragmaContainer.Destroy();
			m_includePragmaContainer = null;
		}

		public void ConfigureCommonTag( TemplateTagData tagData, TemplatePropertyContainer propertyContainer, TemplateIdManager idManager, string uniquePrefix, int offsetIdx, string subBody )
		{
			int id = subBody.IndexOf( tagData.Id );
			if ( id >= 0 )
			{
				tagData.StartIdx = offsetIdx + id;
				idManager.RegisterId( tagData.StartIdx, uniquePrefix + tagData.Id, tagData.Id );
				propertyContainer.AddId( subBody, tagData.Id, tagData.SearchIndentation );
			}
		}

		public TemplateModulesData( TemplateOptionsContainer optionsContainer, TemplateIdManager idManager, TemplatePropertyContainer propertyContainer, string uniquePrefix, int offsetIdx, string subBody, bool isSubShader )
		{
			if ( string.IsNullOrEmpty( subBody ) )
				return;

			m_uniquePrefix = uniquePrefix;
			
			//RENDERING PLATFORMS
			m_renderPlatformHelper = new TemplateRenderPlatformHelper();
			TemplateHelperFunctions.FillRenderingPlatform( m_renderPlatformHelper , subBody );
			if( m_renderPlatformHelper.IsValid )
			{
				m_renderPlatformHelper.Index = offsetIdx + m_renderPlatformHelper.Index;
				idManager.RegisterId( m_renderPlatformHelper.Index , uniquePrefix + m_renderPlatformHelper.ID , m_renderPlatformHelper.ID );
			}

			//PRAGMAS AND INCLUDES
			TemplateHelperFunctions.CreatePragmaIncludeList( subBody, m_includePragmaContainer );

			//COMMON TAGS
			ConfigureCommonTag( m_globalsTag, propertyContainer, idManager, uniquePrefix, offsetIdx, subBody );
			ConfigureCommonTag( m_srpBatcherTag, propertyContainer, idManager, uniquePrefix, offsetIdx, subBody );
			ConfigureCommonTag( m_functionsTag, propertyContainer, idManager, uniquePrefix, offsetIdx, subBody );
			ConfigureCommonTag( m_pragmaTag, propertyContainer, idManager, uniquePrefix, offsetIdx, subBody );
			ConfigureCommonTag( m_pragmaBeforeTag, propertyContainer, idManager, uniquePrefix, offsetIdx, subBody );
			if( !TemplateHelperFunctions.GetPassUniqueId( m_passTag, propertyContainer, idManager, uniquePrefix, offsetIdx, subBody, ref m_passUniqueName ) )
			{
				ConfigureCommonTag( m_passTag, propertyContainer, idManager, uniquePrefix, offsetIdx, subBody );
			}
			ConfigureCommonTag( m_inputsVertTag, propertyContainer, idManager, uniquePrefix, offsetIdx, subBody );
			ConfigureCommonTag( m_inputsFragTag, propertyContainer, idManager, uniquePrefix, offsetIdx, subBody );

			// If Options are enabled then remove them so they won't influence Regex matches
			if( optionsContainer.Enabled && optionsContainer.EndIndex  > 0 )
			{
				offsetIdx += optionsContainer.EndIndex;
				subBody = subBody.Substring( optionsContainer.EndIndex );
			}
			//BlEND MODE
			{
				Match blendModeMatch = Regex.Match( subBody, TemplateHelperFunctions.BlendModePattern1 );
				if( blendModeMatch.Success )
				{
					int blendModeIdx = blendModeMatch.Index;
					int end = blendModeMatch.Length + blendModeIdx;
					string blendParams = subBody.Substring( blendModeIdx, end - blendModeIdx );
					m_blendData1.BlendModeId = blendParams;
					m_blendData1.BlendModeStartIndex = offsetIdx + blendModeIdx;
					idManager.RegisterId( m_blendData1.BlendModeStartIndex, uniquePrefix + m_blendData1.BlendModeId, m_blendData1.BlendModeId );

					TemplateHelperFunctions.CreateBlendMode( blendParams, ref m_blendData1, TemplateHelperFunctions.BlendModePattern1 );
					if( m_blendData1.ValidBlendMode )
					{
						propertyContainer.AddId( subBody, blendParams, false );
					}
				}
			}
			{
				Match blendModeMatch = Regex.Match( subBody, TemplateHelperFunctions.BlendModePattern2 );
				if( blendModeMatch.Success )
				{
					int blendModeIdx = blendModeMatch.Index;
					int end = blendModeMatch.Length + blendModeIdx;
					string blendParams = subBody.Substring( blendModeIdx, end - blendModeIdx );
					m_blendData2.BlendModeId = blendParams;
					m_blendData2.BlendModeStartIndex = offsetIdx + blendModeIdx;
					idManager.RegisterId( m_blendData2.BlendModeStartIndex, uniquePrefix + m_blendData2.BlendModeId, m_blendData2.BlendModeId );

					TemplateHelperFunctions.CreateBlendMode( blendParams, ref m_blendData2, TemplateHelperFunctions.BlendModePattern2 );
					if( m_blendData2.ValidBlendMode )
					{
						propertyContainer.AddId( subBody, blendParams, false );
					}
				}
			}
			{
				Match blendModeMatch = Regex.Match( subBody, TemplateHelperFunctions.BlendModePattern3 );
				if( blendModeMatch.Success )
				{
					int blendModeIdx = blendModeMatch.Index;
					int end = blendModeMatch.Length + blendModeIdx;
					string blendParams = subBody.Substring( blendModeIdx, end - blendModeIdx );
					m_blendData3.BlendModeId = blendParams;
					m_blendData3.BlendModeStartIndex = offsetIdx + blendModeIdx;
					idManager.RegisterId( m_blendData3.BlendModeStartIndex, uniquePrefix + m_blendData3.BlendModeId, m_blendData3.BlendModeId );

					TemplateHelperFunctions.CreateBlendMode( blendParams, ref m_blendData3, TemplateHelperFunctions.BlendModePattern3 );
					if( m_blendData3.ValidBlendMode )
					{
						propertyContainer.AddId( subBody, blendParams, false );
					}
				}
			}
			{
				string pattern = TemplateHelperFunctions.BlendModePattern;
				Match blendModeMatch = Regex.Match( subBody, pattern );
				if( !blendModeMatch.Success && !m_blendData1.ValidBlendMode && !m_blendData2.ValidBlendMode && !m_blendData3.ValidBlendMode )
				{
					pattern = TemplateHelperFunctions.BlendModePatternFirst;
					blendModeMatch = Regex.Match( subBody, pattern );
				}
				if( blendModeMatch.Success )
				{
					int blendModeIdx = blendModeMatch.Index;
					int end = blendModeMatch.Length + blendModeIdx;
					string blendParams = subBody.Substring( blendModeIdx, end - blendModeIdx );
					m_blendData.BlendModeId = blendParams;
					m_blendData.BlendModeStartIndex = offsetIdx + blendModeIdx;
					idManager.RegisterId( m_blendData.BlendModeStartIndex, uniquePrefix + m_blendData.BlendModeId, m_blendData.BlendModeId );

					TemplateHelperFunctions.CreateBlendMode( blendParams, ref m_blendData, pattern );
					if( m_blendData.ValidBlendMode )
					{
						propertyContainer.AddId( subBody, blendParams, false );
					}
					
				}
			}
			//BLEND OP
			{
				Match blendOpMatch = Regex.Match( subBody, TemplateHelperFunctions.BlendOpPattern1 );
				if( blendOpMatch.Success )
				{
					int blendOpIdx = blendOpMatch.Index;
					int end = blendOpMatch.Length + blendOpIdx;
					string blendOpParams = subBody.Substring( blendOpIdx, end - blendOpIdx );
					m_blendData1.BlendOpId = blendOpParams;
					m_blendData1.BlendOpStartIndex = offsetIdx + blendOpIdx;
					idManager.RegisterId( m_blendData1.BlendOpStartIndex, uniquePrefix + m_blendData1.BlendOpId, m_blendData1.BlendOpId );
					TemplateHelperFunctions.CreateBlendOp( blendOpParams, ref m_blendData1, TemplateHelperFunctions.BlendOpPattern1 );
					if( m_blendData1.ValidBlendOp )
					{
						propertyContainer.AddId( subBody, blendOpParams, false );
					}
				}

				m_blendData1.DataCheck = ( m_blendData1.ValidBlendMode || m_blendData1.ValidBlendOp ) ? TemplateDataCheck.Valid : TemplateDataCheck.Invalid;
			}
			{
				Match blendOpMatch = Regex.Match( subBody, TemplateHelperFunctions.BlendOpPattern2 );
				if( blendOpMatch.Success )
				{
					int blendOpIdx = blendOpMatch.Index;
					int end = blendOpMatch.Length + blendOpIdx;
					string blendOpParams = subBody.Substring( blendOpIdx, end - blendOpIdx );
					m_blendData2.BlendOpId = blendOpParams;
					m_blendData2.BlendOpStartIndex = offsetIdx + blendOpIdx;
					idManager.RegisterId( m_blendData2.BlendOpStartIndex, uniquePrefix + m_blendData2.BlendOpId, m_blendData2.BlendOpId );
					TemplateHelperFunctions.CreateBlendOp( blendOpParams, ref m_blendData2, TemplateHelperFunctions.BlendOpPattern2 );
					if( m_blendData2.ValidBlendOp )
					{
						propertyContainer.AddId( subBody, blendOpParams, false );
					}
				}

				m_blendData2.DataCheck = ( m_blendData2.ValidBlendMode || m_blendData2.ValidBlendOp ) ? TemplateDataCheck.Valid : TemplateDataCheck.Invalid;
			}
			{
				Match blendOpMatch = Regex.Match( subBody, TemplateHelperFunctions.BlendOpPattern3 );
				if( blendOpMatch.Success )
				{
					int blendOpIdx = blendOpMatch.Index;
					int end = blendOpMatch.Length + blendOpIdx;
					string blendOpParams = subBody.Substring( blendOpIdx, end - blendOpIdx );
					m_blendData3.BlendOpId = blendOpParams;
					m_blendData3.BlendOpStartIndex = offsetIdx + blendOpIdx;
					idManager.RegisterId( m_blendData3.BlendOpStartIndex, uniquePrefix + m_blendData3.BlendOpId, m_blendData3.BlendOpId );
					TemplateHelperFunctions.CreateBlendOp( blendOpParams, ref m_blendData3, TemplateHelperFunctions.BlendOpPattern3 );
					if( m_blendData3.ValidBlendOp )
					{
						propertyContainer.AddId( subBody, blendOpParams, false );
					}
				}

				m_blendData3.DataCheck = ( m_blendData3.ValidBlendMode || m_blendData3.ValidBlendOp ) ? TemplateDataCheck.Valid : TemplateDataCheck.Invalid;
			}
			{
				string pattern = TemplateHelperFunctions.BlendOpPattern;
				Match blendOpMatch = Regex.Match( subBody, pattern );
				if( !blendOpMatch.Success && !m_blendData1.ValidBlendOp && !m_blendData2.ValidBlendOp && !m_blendData3.ValidBlendOp )
				{
					pattern = TemplateHelperFunctions.BlendOpPatternFirst;
					blendOpMatch = Regex.Match( subBody, pattern );
				}

				if( blendOpMatch.Success )
				{
					int blendOpIdx = blendOpMatch.Index;
					int end = blendOpMatch.Length + blendOpIdx;
					string blendOpParams = subBody.Substring( blendOpIdx, end - blendOpIdx );
					m_blendData.BlendOpId = blendOpParams;
					m_blendData.BlendOpStartIndex = offsetIdx + blendOpIdx;
					idManager.RegisterId( m_blendData.BlendOpStartIndex, uniquePrefix + m_blendData.BlendOpId, m_blendData.BlendOpId );
					TemplateHelperFunctions.CreateBlendOp( blendOpParams, ref m_blendData, pattern );
					if( m_blendData.ValidBlendOp )
					{
						propertyContainer.AddId( subBody, blendOpParams, false );
					}
				}

				m_blendData.DataCheck = ( m_blendData.ValidBlendMode || m_blendData.ValidBlendOp ) ? TemplateDataCheck.Valid : TemplateDataCheck.Invalid;
			}
			
			//ALPHA TO MASK
			{
				Match alphaToMaskMatch = Regex.Match( subBody, TemplateHelperFunctions.AlphaToMaskPattern );
				if( alphaToMaskMatch.Success )
				{
					int alphaIdx = alphaToMaskMatch.Index;
					int end = subBody.IndexOf( TemplatesManager.TemplateNewLine, alphaIdx );
					string alphaParams = subBody.Substring( alphaIdx, end - alphaIdx );
					m_alphaToMaskData.AlphaToMaskId = alphaParams;
					m_alphaToMaskData.StartIdx = offsetIdx + alphaIdx;
					idManager.RegisterId( m_alphaToMaskData.StartIdx, uniquePrefix + m_alphaToMaskData.AlphaToMaskId, m_alphaToMaskData.AlphaToMaskId );
					TemplateHelperFunctions.CreateAlphaToMask( alphaParams, ref m_alphaToMaskData );
					if( m_alphaToMaskData.DataCheck == TemplateDataCheck.Valid )
						propertyContainer.AddId( subBody, alphaParams, false, string.Empty );
				}
			}

			//CULL MODE
			{
				Match cullMatch = Regex.Match( subBody, TemplateHelperFunctions.CullWholeWordPattern );
				if( cullMatch.Success )
				{
					int cullIdx = cullMatch.Index;
					int end = subBody.IndexOf( TemplatesManager.TemplateNewLine, cullIdx );
					string cullParams = subBody.Substring( cullIdx, end - cullIdx );
					m_cullModeData.CullModeId = cullParams;
					m_cullModeData.StartIdx = offsetIdx + cullIdx;
					idManager.RegisterId( m_cullModeData.StartIdx, uniquePrefix + m_cullModeData.CullModeId, m_cullModeData.CullModeId );
					TemplateHelperFunctions.CreateCullMode( cullParams, ref m_cullModeData );
					if( m_cullModeData.DataCheck == TemplateDataCheck.Valid )
						propertyContainer.AddId( subBody, cullParams, false, string.Empty );
					
				}
			}
			//COLOR MASK
			{
				Match colorMaskMatch = Regex.Match( subBody, TemplateHelperFunctions.ColorMask1Pattern );
				if( colorMaskMatch.Success )
				{
					int colorMaskIdx = colorMaskMatch.Index;
					int end = colorMaskMatch.Length + colorMaskIdx;// subBody.IndexOf( TemplatesManager.TemplateNewLine, colorMaskIdx );
					string colorMaskParams = subBody.Substring( colorMaskIdx, end - colorMaskIdx );
					m_colorMaskData1.ColorMaskId = colorMaskParams;
					m_colorMaskData1.StartIdx = offsetIdx + colorMaskIdx;
					idManager.RegisterId( m_colorMaskData1.StartIdx, uniquePrefix + m_colorMaskData1.ColorMaskId, m_colorMaskData1.ColorMaskId );
					TemplateHelperFunctions.CreateColorMask( colorMaskParams, ref m_colorMaskData1, TemplateHelperFunctions.ColorMask1Pattern );
					if( m_colorMaskData1.DataCheck == TemplateDataCheck.Valid )
						propertyContainer.AddId( subBody, colorMaskParams, false );

				}
			}
			{
				Match colorMaskMatch = Regex.Match( subBody, TemplateHelperFunctions.ColorMask2Pattern );
				if( colorMaskMatch.Success )
				{
					int colorMaskIdx = colorMaskMatch.Index;
					int end = colorMaskMatch.Length + colorMaskIdx;// subBody.IndexOf( TemplatesManager.TemplateNewLine, colorMaskIdx );
					string colorMaskParams = subBody.Substring( colorMaskIdx, end - colorMaskIdx );
					m_colorMaskData2.ColorMaskId = colorMaskParams;
					m_colorMaskData2.StartIdx = offsetIdx + colorMaskIdx;
					idManager.RegisterId( m_colorMaskData2.StartIdx, uniquePrefix + m_colorMaskData2.ColorMaskId, m_colorMaskData2.ColorMaskId );
					TemplateHelperFunctions.CreateColorMask( colorMaskParams, ref m_colorMaskData2, TemplateHelperFunctions.ColorMask2Pattern );
					if( m_colorMaskData2.DataCheck == TemplateDataCheck.Valid )
						propertyContainer.AddId( subBody, colorMaskParams, false );

				}
			}
			{
				Match colorMaskMatch = Regex.Match( subBody, TemplateHelperFunctions.ColorMask3Pattern );
				if( colorMaskMatch.Success )
				{
					int colorMaskIdx = colorMaskMatch.Index;
					int end = colorMaskMatch.Length + colorMaskIdx;// subBody.IndexOf( TemplatesManager.TemplateNewLine, colorMaskIdx );
					string colorMaskParams = subBody.Substring( colorMaskIdx, end - colorMaskIdx );
					m_colorMaskData3.ColorMaskId = colorMaskParams;
					m_colorMaskData3.StartIdx = offsetIdx + colorMaskIdx;
					idManager.RegisterId( m_colorMaskData3.StartIdx, uniquePrefix + m_colorMaskData3.ColorMaskId, m_colorMaskData3.ColorMaskId );
					TemplateHelperFunctions.CreateColorMask( colorMaskParams, ref m_colorMaskData3, TemplateHelperFunctions.ColorMask3Pattern );
					if( m_colorMaskData3.DataCheck == TemplateDataCheck.Valid )
						propertyContainer.AddId( subBody, colorMaskParams, false );

				}
			}
			{
				string pattern = TemplateHelperFunctions.ColorMaskPattern;
				Match colorMaskMatch = Regex.Match( subBody, pattern );
				if( !colorMaskMatch.Success && m_colorMaskData1.DataCheck == TemplateDataCheck.Invalid && m_colorMaskData2.DataCheck == TemplateDataCheck.Invalid && m_colorMaskData3.DataCheck == TemplateDataCheck.Invalid )
				{
					pattern = TemplateHelperFunctions.ColorMaskPatternFirst;
					colorMaskMatch = Regex.Match( subBody, pattern );
				}

				if( colorMaskMatch.Success )
				{
					int colorMaskIdx = colorMaskMatch.Index;
					int end = colorMaskMatch.Length + colorMaskIdx; //subBody.IndexOf( TemplatesManager.TemplateNewLine, colorMaskIdx );
					string colorMaskParams = subBody.Substring( colorMaskIdx, end - colorMaskIdx );
					m_colorMaskData.ColorMaskId = colorMaskParams;
					m_colorMaskData.StartIdx = offsetIdx + colorMaskIdx;
					idManager.RegisterId( m_colorMaskData.StartIdx, uniquePrefix + m_colorMaskData.ColorMaskId, m_colorMaskData.ColorMaskId );
					TemplateHelperFunctions.CreateColorMask( colorMaskParams, ref m_colorMaskData, pattern );
					if( m_colorMaskData.DataCheck == TemplateDataCheck.Valid )
						propertyContainer.AddId( subBody, colorMaskParams, false );

				}
			}
			//STENCIL
			{
				Match stencilMatch = Regex.Match( subBody, TemplateHelperFunctions.StencilWholeWordPattern );
				if( stencilMatch.Success )
				{
					int stencilIdx = stencilMatch.Index;
					int stencilEndIdx = subBody.IndexOf( "}", stencilIdx );
					if( stencilEndIdx > 0 )
					{
						string stencilParams = subBody.Substring( stencilIdx, stencilEndIdx + 1 - stencilIdx );
						m_stencilData.StencilBufferId = stencilParams;
						m_stencilData.StartIdx = offsetIdx + stencilIdx;
						idManager.RegisterId( m_stencilData.StartIdx, uniquePrefix + m_stencilData.StencilBufferId, m_stencilData.StencilBufferId );
						TemplateHelperFunctions.CreateStencilOps( stencilParams, ref m_stencilData );
						if( m_stencilData.DataCheck == TemplateDataCheck.Valid )
						{
							propertyContainer.AddId( subBody, stencilParams, true );
						}
					}
				}
				else
				{
					int stencilTagIdx = subBody.IndexOf( TemplatesManager.TemplateStencilTag );
					if( stencilTagIdx > -1 )
					{
						m_stencilData.SetIndependentDefault();
						m_stencilData.StencilBufferId = TemplatesManager.TemplateStencilTag;
						m_stencilData.StartIdx = offsetIdx + stencilTagIdx;
						idManager.RegisterId( m_stencilData.StartIdx, uniquePrefix + m_stencilData.StencilBufferId, m_stencilData.StencilBufferId );
						propertyContainer.AddId( subBody, m_stencilData.StencilBufferId, true );
					}
				}
			}
			//ZWRITE
			{
				Match zWriteMatch = Regex.Match( subBody, TemplateHelperFunctions.ZWriteWholeWordPattern );
				if( zWriteMatch.Success )
				{
					int zWriteOpIdx = zWriteMatch.Index;
					int zWriteEndIdx = subBody.IndexOf( TemplatesManager.TemplateNewLine, zWriteOpIdx );
					if( zWriteEndIdx > 0 )
					{
						m_depthData.ZWriteModeId = subBody.Substring( zWriteOpIdx, zWriteEndIdx + 1 - zWriteOpIdx );
						m_depthData.ZWriteStartIndex = offsetIdx + zWriteOpIdx;
						idManager.RegisterId( m_depthData.ZWriteStartIndex, uniquePrefix + m_depthData.ZWriteModeId, m_depthData.ZWriteModeId );
						TemplateHelperFunctions.CreateZWriteMode( m_depthData.ZWriteModeId, ref m_depthData );
						if( m_depthData.DataCheck == TemplateDataCheck.Valid )
						{
							propertyContainer.AddId( subBody, m_depthData.ZWriteModeId, true );
						}
					}
				}
			}

			//ZTEST
			{
				Match zTestMatch = Regex.Match( subBody, TemplateHelperFunctions.ZTestWholeWordPattern );
				if( zTestMatch.Success )
				{
					int zTestOpIdx = zTestMatch.Index;
					int zTestEndIdx = subBody.IndexOf( TemplatesManager.TemplateNewLine, zTestOpIdx );
					if( zTestEndIdx > 0 )
					{
						m_depthData.ZTestModeId = subBody.Substring( zTestOpIdx, zTestEndIdx + 1 - zTestOpIdx );
						m_depthData.ZTestStartIndex = offsetIdx + zTestOpIdx;
						idManager.RegisterId( m_depthData.ZTestStartIndex, uniquePrefix + m_depthData.ZTestModeId, m_depthData.ZTestModeId );
						TemplateHelperFunctions.CreateZTestMode( m_depthData.ZTestModeId, ref m_depthData );
						if( m_depthData.DataCheck == TemplateDataCheck.Valid )
						{
							propertyContainer.AddId( subBody, m_depthData.ZTestModeId, true );
						}
					}
				}
			}

			//ZOFFSET
			{
				Match zOffsetMatch = Regex.Match( subBody, TemplateHelperFunctions.ZOffsetWholeWordPattern );
				if( zOffsetMatch.Success )
				{
					int zOffsetIdx = zOffsetMatch.Index;
					int zOffsetEndIdx = subBody.IndexOf( TemplatesManager.TemplateNewLine, zOffsetIdx );
					if( zOffsetEndIdx > 0 )
					{
						m_depthData.OffsetId = subBody.Substring( zOffsetIdx, zOffsetEndIdx + 1 - zOffsetIdx );
						m_depthData.OffsetStartIndex = offsetIdx + zOffsetIdx;
						idManager.RegisterId( m_depthData.OffsetStartIndex, uniquePrefix + m_depthData.OffsetId, m_depthData.OffsetId );
						TemplateHelperFunctions.CreateZOffsetMode( m_depthData.OffsetId, ref m_depthData );
						if( m_depthData.DataCheck == TemplateDataCheck.Valid )
						{
							propertyContainer.AddId( subBody, m_depthData.OffsetId, true );
						}
					}
				}
				m_depthData.SetDataCheck();
			}
			//TAGS
			{
				Match tagsMatch = Regex.Match( subBody, TemplateHelperFunctions.TagsWholeWordPattern );
				if ( tagsMatch.Success )
				{
					int tagsIdx = tagsMatch.Index;
					int tagsEndIdx = subBody.IndexOf( "}", tagsIdx );
					if ( tagsEndIdx > -1 )
					{
						m_tagData.Reset();
						m_tagData.TagsId = subBody.Substring( tagsIdx, tagsEndIdx + 1 - tagsIdx );
						m_tagData.StartIdx = offsetIdx + tagsIdx;
						idManager.RegisterId( m_tagData.StartIdx, uniquePrefix + m_tagData.TagsId, m_tagData.TagsId );
						m_srpType = TemplateHelperFunctions.CreateTags( ref m_tagData, isSubShader );

						propertyContainer.AddId( subBody, m_tagData.TagsId, false );
						m_tagData.DataCheck = TemplateDataCheck.Valid;
					}
					else
					{
						m_tagData.DataCheck = TemplateDataCheck.Invalid;
					}
				}
				else
				{
					m_tagData.DataCheck = TemplateDataCheck.Invalid;
				}
			}

			//SHADER MODEL
			{
				Match match = Regex.Match( subBody, TemplateHelperFunctions.ShaderModelPattern );
				if ( match != null && match.Groups.Count > 1 )
				{
					if ( TemplateHelperFunctions.AvailableInterpolators.ContainsKey( match.Groups[ 1 ].Value ) )
					{
						m_shaderModel.Id = match.Groups[ 0 ].Value;
						m_shaderModel.StartIdx = offsetIdx + match.Index;
						m_shaderModel.Value = match.Groups[ 1 ].Value;
						m_shaderModel.InterpolatorAmount = TemplateHelperFunctions.AvailableInterpolators[ match.Groups[ 1 ].Value ];
						m_shaderModel.DataCheck = TemplateDataCheck.Valid;
						idManager.RegisterId( m_shaderModel.StartIdx, uniquePrefix + m_shaderModel.Id, m_shaderModel.Id );
					}
					else
					{
						m_shaderModel.DataCheck = TemplateDataCheck.Invalid;
					}
				}
			}

			// ALL MODULES
			int allModulesIndex = subBody.IndexOf( TemplatesManager.TemplateAllModulesTag );
			if( allModulesIndex > 0 )
			{
				//ONLY REGISTER MISSING TAGS
				ConfigureCommonTag( m_allModulesTag, propertyContainer, idManager, uniquePrefix, offsetIdx, subBody );
				m_allModulesMode = true;
				
				m_blendData.SetAllModulesDefault();

				if( !m_alphaToMaskData.IsValid )
					m_alphaToMaskData.SetAllModulesDefault();

				if( !m_cullModeData.IsValid )
					m_cullModeData.SetAllModulesDefault();

				if( !m_colorMaskData.IsValid )
					m_colorMaskData.SetAllModulesDefault();

				if( !m_stencilData.IsValid )
					m_stencilData.SetAllModulesDefault();

				if( !m_depthData.IsValid )
					m_depthData.SetAllModulesDefault();

				if( !m_shaderModel.IsValid )
					m_shaderModel.SetAllModulesDefault();
			}
		}

		public void TestPropertyInternalName( string name, ref List<TemplateShaderPropertyData> availableShaderProperties, ref Dictionary<string, TemplateShaderPropertyData> duplicatesHelper )
		{
			if( !string.IsNullOrEmpty( name ) && !duplicatesHelper.ContainsKey( name ))
			{
				TemplateShaderPropertyData newData = new TemplateShaderPropertyData( -1, string.Empty, string.Empty, name, name, WirePortDataType.INT, PropertyType.Property,-1,-1 );
				availableShaderProperties.Add( newData );
				duplicatesHelper.Add( newData.PropertyName , newData );
			}
		}

		public void RegisterInternalUnityInlines( ref List<TemplateShaderPropertyData> availableShaderProperties, ref Dictionary<string, TemplateShaderPropertyData> duplicatesHelper )
		{
			TestPropertyInternalName( m_depthData.ZWriteInlineValue, ref availableShaderProperties , ref duplicatesHelper);
			TestPropertyInternalName( m_depthData.ZTestInlineValue, ref availableShaderProperties, ref duplicatesHelper );
			TestPropertyInternalName( m_depthData.OffsetFactorInlineValue, ref availableShaderProperties, ref duplicatesHelper );
			TestPropertyInternalName( m_depthData.OffsetUnitsInlineValue, ref availableShaderProperties, ref duplicatesHelper );		

			TestPropertyInternalName( m_blendData.SourceFactorRGBInline, ref availableShaderProperties, ref duplicatesHelper );
			TestPropertyInternalName( m_blendData.DestFactorRGBInline, ref availableShaderProperties, ref duplicatesHelper );
			TestPropertyInternalName( m_blendData.SourceFactorAlphaInline, ref availableShaderProperties, ref duplicatesHelper );
			TestPropertyInternalName( m_blendData.DestFactorAlphaInline, ref availableShaderProperties, ref duplicatesHelper );
			TestPropertyInternalName( m_blendData.BlendOpRGBInline, ref availableShaderProperties, ref duplicatesHelper );
			TestPropertyInternalName( m_blendData.BlendOpAlphaInline, ref availableShaderProperties, ref duplicatesHelper );

			TestPropertyInternalName( m_blendData1.SourceFactorRGBInline, ref availableShaderProperties, ref duplicatesHelper );
			TestPropertyInternalName( m_blendData1.DestFactorRGBInline, ref availableShaderProperties, ref duplicatesHelper );
			TestPropertyInternalName( m_blendData1.SourceFactorAlphaInline, ref availableShaderProperties, ref duplicatesHelper );
			TestPropertyInternalName( m_blendData1.DestFactorAlphaInline, ref availableShaderProperties, ref duplicatesHelper );
			TestPropertyInternalName( m_blendData1.BlendOpRGBInline, ref availableShaderProperties, ref duplicatesHelper );
			TestPropertyInternalName( m_blendData1.BlendOpAlphaInline, ref availableShaderProperties, ref duplicatesHelper );

			TestPropertyInternalName( m_blendData2.SourceFactorRGBInline, ref availableShaderProperties, ref duplicatesHelper );
			TestPropertyInternalName( m_blendData2.DestFactorRGBInline, ref availableShaderProperties, ref duplicatesHelper );
			TestPropertyInternalName( m_blendData2.SourceFactorAlphaInline, ref availableShaderProperties, ref duplicatesHelper );
			TestPropertyInternalName( m_blendData2.DestFactorAlphaInline, ref availableShaderProperties, ref duplicatesHelper );
			TestPropertyInternalName( m_blendData2.BlendOpRGBInline, ref availableShaderProperties, ref duplicatesHelper );
			TestPropertyInternalName( m_blendData2.BlendOpAlphaInline, ref availableShaderProperties, ref duplicatesHelper );

			TestPropertyInternalName( m_blendData3.SourceFactorRGBInline, ref availableShaderProperties, ref duplicatesHelper );
			TestPropertyInternalName( m_blendData3.DestFactorRGBInline, ref availableShaderProperties, ref duplicatesHelper );
			TestPropertyInternalName( m_blendData3.SourceFactorAlphaInline, ref availableShaderProperties, ref duplicatesHelper );
			TestPropertyInternalName( m_blendData3.DestFactorAlphaInline, ref availableShaderProperties, ref duplicatesHelper );
			TestPropertyInternalName( m_blendData3.BlendOpRGBInline, ref availableShaderProperties, ref duplicatesHelper );
			TestPropertyInternalName( m_blendData3.BlendOpAlphaInline, ref availableShaderProperties, ref duplicatesHelper );

			TestPropertyInternalName( m_alphaToMaskData.InlineData, ref availableShaderProperties, ref duplicatesHelper );

			TestPropertyInternalName( m_stencilData.ReferenceInline, ref availableShaderProperties, ref duplicatesHelper );
			TestPropertyInternalName( m_stencilData.ReadMaskInline, ref availableShaderProperties, ref duplicatesHelper );
			TestPropertyInternalName( m_stencilData.WriteMaskInline, ref availableShaderProperties, ref duplicatesHelper );
			TestPropertyInternalName( m_stencilData.ComparisonFrontInline, ref availableShaderProperties, ref duplicatesHelper );
			TestPropertyInternalName( m_stencilData.PassFrontInline, ref availableShaderProperties, ref duplicatesHelper );
			TestPropertyInternalName( m_stencilData.FailFrontInline, ref availableShaderProperties, ref duplicatesHelper );
			TestPropertyInternalName( m_stencilData.ZFailFrontInline, ref availableShaderProperties, ref duplicatesHelper );
			TestPropertyInternalName( m_stencilData.ComparisonBackInline, ref availableShaderProperties, ref duplicatesHelper );
			TestPropertyInternalName( m_stencilData.PassBackInline, ref availableShaderProperties, ref duplicatesHelper );
			TestPropertyInternalName( m_stencilData.FailBackInline, ref availableShaderProperties, ref duplicatesHelper );
			TestPropertyInternalName( m_stencilData.ZFailBackInline, ref availableShaderProperties, ref duplicatesHelper );

			TestPropertyInternalName( m_cullModeData.InlineData, ref availableShaderProperties, ref duplicatesHelper );

			TestPropertyInternalName( m_colorMaskData.InlineData, ref availableShaderProperties, ref duplicatesHelper );
			TestPropertyInternalName( m_colorMaskData1.InlineData, ref availableShaderProperties, ref duplicatesHelper );
			TestPropertyInternalName( m_colorMaskData2.InlineData, ref availableShaderProperties, ref duplicatesHelper );
			TestPropertyInternalName( m_colorMaskData3.InlineData, ref availableShaderProperties, ref duplicatesHelper );
		}

		public void SetPassUniqueNameIfUndefined( string value )
		{
			if( string.IsNullOrEmpty( m_passUniqueName ) )
				m_passUniqueName = value;
		}

		public bool HasValidData
		{
			get
			{
				return m_blendData.DataCheck == TemplateDataCheck.Valid ||
						m_blendData1.DataCheck == TemplateDataCheck.Valid ||
						m_blendData2.DataCheck == TemplateDataCheck.Valid ||
						m_blendData3.DataCheck == TemplateDataCheck.Valid ||
						m_alphaToMaskData.DataCheck == TemplateDataCheck.Valid ||
						m_cullModeData.DataCheck == TemplateDataCheck.Valid ||
						m_colorMaskData.DataCheck == TemplateDataCheck.Valid ||
						m_colorMaskData1.DataCheck == TemplateDataCheck.Valid ||
						m_colorMaskData2.DataCheck == TemplateDataCheck.Valid ||
						m_colorMaskData3.DataCheck == TemplateDataCheck.Valid ||
						m_stencilData.DataCheck == TemplateDataCheck.Valid ||
						m_depthData.DataCheck == TemplateDataCheck.Valid ||
						m_tagData.DataCheck == TemplateDataCheck.Valid ||
						m_shaderModel.DataCheck == TemplateDataCheck.Valid ||
						m_globalsTag.IsValid ||
						m_srpBatcherTag.IsValid ||
						m_allModulesTag.IsValid ||
						m_functionsTag.IsValid ||
						m_pragmaTag.IsValid ||
						m_pragmaBeforeTag.IsValid ||
						m_passTag.IsValid ||
						m_inputsVertTag.IsValid ||
						m_inputsFragTag.IsValid ||
						m_renderPlatformHelper.IsValid;
			}
		}

		public TemplateBlendData BlendData { get { return m_blendData; } }
		public TemplateBlendData BlendData1 { get { return m_blendData1; } }
		public TemplateBlendData BlendData2 { get { return m_blendData2; } }
		public TemplateBlendData BlendData3 { get { return m_blendData3; } }
		public TemplateAlphaToMaskData AlphaToMaskData { get { return m_alphaToMaskData; } }
		public TemplateCullModeData CullModeData { get { return m_cullModeData; } }
		public TemplateColorMaskData ColorMaskData { get { return m_colorMaskData; } }
		public TemplateColorMaskData ColorMaskData1 { get { return m_colorMaskData1; } }
		public TemplateColorMaskData ColorMaskData2 { get { return m_colorMaskData2; } }
		public TemplateColorMaskData ColorMaskData3 { get { return m_colorMaskData3; } }
		public TemplateStencilData StencilData { get { return m_stencilData; } }
		public TemplateDepthData DepthData { get { return m_depthData; } }
		public TemplateTagsModuleData TagData { get { return m_tagData; } }
		public TemplateTagData GlobalsTag { get { return m_globalsTag; } }
		public TemplateTagData SRPBatcherTag { get { return m_srpBatcherTag; } }
		public TemplateTagData AllModulesTag { get { return m_allModulesTag; } }
		public TemplateTagData FunctionsTag { get { return m_functionsTag; } }
		public TemplateTagData PragmaTag { get { return m_pragmaTag; } }
		public TemplateTagData PragmaBeforeTag { get { return m_pragmaBeforeTag; } }
		public TemplateTagData PassTag { get { return m_passTag; } }
		public TemplateTagData InputsVertTag { get { return m_inputsVertTag; } }
		public TemplateTagData InputsFragTag { get { return m_inputsFragTag; } }
		public TemplateShaderModelData ShaderModel { get { return m_shaderModel; } }
		public TemplateSRPType SRPType { get { return m_srpType; } set { m_srpType = value; } }
		public bool SRPIsPBR { get { return m_srpIsPBR; } set { m_srpIsPBR = value; } }
		public bool SRPIsPBRHD { get { return m_srpIsPBR && m_srpType == TemplateSRPType.HD; }  }
		public string UniquePrefix { get { return m_uniquePrefix; } }
		public string PassUniqueName { get { return m_passUniqueName; } }
		public bool HasPassUniqueName { get { return !string.IsNullOrEmpty( m_passUniqueName ); } }
		public TemplateIncludePragmaContainter IncludePragmaContainer { get { return m_includePragmaContainer; } }
		public TemplateRenderPlatformHelper RenderPlatformHelper { get { return m_renderPlatformHelper; } }
		public bool AllModulesMode { get { return m_allModulesMode; } }
	}
}
