// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
using UnityEngine;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "World Position", "Surface Data", "World space position" )]
	public sealed class WorldPosInputsNode : SurfaceShaderINParentNode
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_currentInput = SurfaceInputs.WORLD_POS;
			m_drawPreviewAsSphere = true;
			m_previewShaderGUID = "70d5405009b31a349a4d8285f30cf5d9";
			InitialSetup();
		}

		public override void DrawProperties() { }

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalVar )
		{
			if ( dataCollector.IsTemplate )
			{
				string varName = dataCollector.TemplateDataCollectorInstance.GetWorldPos();
				return GetOutputVectorItem( 0, outputId, varName );
			}
			
			string worldPosition = GeneratorUtils.GenerateWorldPosition( ref dataCollector, UniqueId );

			return GetOutputVectorItem( 0, outputId, worldPosition );
		}
	}
}
