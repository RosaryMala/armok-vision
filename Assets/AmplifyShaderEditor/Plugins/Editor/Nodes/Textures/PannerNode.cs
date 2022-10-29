// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using UnityEngine;
using System;

namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Panner", "UV Coordinates", "Pans UV texture coordinates according to its inputs" )]
	public sealed class PannerNode : ParentNode
	{
		private const string _speedXStr = "Speed X";
		private const string _speedYStr = "Speed Y";
		
		private int m_cachedUsingEditorId = -1;
		
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			AddInputPort( WirePortDataType.FLOAT2, false, "UV" ,-1,MasterNodePortCategory.Fragment,0);
			AddInputPort( WirePortDataType.FLOAT2, false, "Speed", -1, MasterNodePortCategory.Fragment, 2 );
			AddInputPort( WirePortDataType.FLOAT, false, "Time", -1, MasterNodePortCategory.Fragment, 1 );
			AddOutputPort( WirePortDataType.FLOAT2, "Out" );
			m_textLabelWidth = 70;
			m_useInternalPortData = true;
			m_previewShaderGUID = "6f89a5d96bdad114b9bbd0c236cac622";
			m_inputPorts[ 2 ].FloatInternalData = 1;
			m_continuousPreviewRefresh = true;
		}

		public override void SetPreviewInputs()
		{
			base.SetPreviewInputs();

			if ( m_cachedUsingEditorId == -1 )
				m_cachedUsingEditorId = Shader.PropertyToID( "_UsingEditor" );

			PreviewMaterial.SetFloat( m_cachedUsingEditorId, ( m_inputPorts[ 2 ].IsConnected ? 0 : 1 ) );
		}

		public override void OnInputPortConnected( int portId, int otherNodeId, int otherPortId, bool activateNode = true )
		{
			base.OnInputPortConnected( portId, otherNodeId, otherPortId, activateNode );
			if( portId == 1 )
			{
				m_continuousPreviewRefresh = false;
			}
		}

		public override void OnInputPortDisconnected( int portId )
		{
			base.OnInputPortDisconnected( portId );
			if( portId == 1 )
			{
				m_continuousPreviewRefresh = true;
			}
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			if( m_outputPorts[ 0 ].IsLocalValue( dataCollector.PortCategory ) )
				return m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory );

			string timePort = m_inputPorts[ 2 ].GeneratePortInstructions( ref dataCollector );
			
			if( !m_inputPorts[ 2 ].IsConnected )
			{
				if( !( dataCollector.IsTemplate && dataCollector.IsSRP ) )
					dataCollector.AddToIncludes( UniqueId, Constants.UnityShaderVariables );
				timePort += " * _Time.y";
			}

			string speed = m_inputPorts[ 1 ].GeneratePortInstructions( ref dataCollector );
			string result = "( " + timePort + " * " + speed + " + " + m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector ) + ")";

			RegisterLocalVariable( 0, result, ref dataCollector, "panner" + OutputId );
			return m_outputPorts[ 0 ].LocalValue( dataCollector.PortCategory );
		}

		public override void ReadFromString( ref string[] nodeParams )
		{
			base.ReadFromString( ref nodeParams );
			if( UIUtils.CurrentShaderVersion() < 13107 )
			{
				// The internal data for the new port can be set in here since it didn't existed
				// on older shader versions
				float speedX = Convert.ToSingle( GetCurrentParam( ref nodeParams ) );
				float speedY = Convert.ToSingle( GetCurrentParam( ref nodeParams ) );
				m_inputPorts[ 1 ].Vector2InternalData = new Vector2( speedX, speedY );
			}
		}

		public override void ReadInputDataFromString( ref string[] nodeParams )
		{
			base.ReadInputDataFromString( ref nodeParams );
			if( UIUtils.CurrentShaderVersion() < 13107 )
			{
				//Time Port must be rewritten after internal data is read
				// already existed in previous shaders 
				m_inputPorts[ 2 ].FloatInternalData = 1;
			}
		}

		public override void WriteToString( ref string nodeInfo, ref string connectionsInfo )
		{
			base.WriteToString( ref nodeInfo, ref connectionsInfo );
		}
	}
}
