// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>
//
// Custom Node Compare (A > B) 
// Donated by The Four Headed Cat - @fourheadedcat

using UnityEngine;
using System;

namespace AmplifyShaderEditor
{
    [Serializable]
	[NodeAttributes("Compare (A > B)", "Logical Operators", "Check if A is greater than B. If true return value of True else return value of False", null, KeyCode.None, true, true, "Compare", typeof( Compare ), "The Four Headed Cat - @fourheadedcat" )]
    public sealed class TFHCCompareGreater : TFHCStub
	{
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			m_inputPorts[ 0 ].Name = "A";
			m_inputPorts[ 1 ].Name = "B";
			AddInputPort( WirePortDataType.FLOAT, false, "True" );
			AddInputPort( WirePortDataType.FLOAT, false, "False" );
			m_textLabelWidth = 100;
			m_useInternalPortData = true;
			m_previewShaderGUID = "363192dbd019ad2478f2fe6c277b7e48";
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			GetInputData( ref dataCollector, ignoreLocalvar );
			string strout = "(( " + m_inputDataPort0 + " > " + m_inputDataPort1 + " ) ? " + m_inputDataPort2 + " :  " + m_inputDataPort3  + " )";
			//Debug.Log(strout);
			return CreateOutputLocalVariable( 0, strout, ref dataCollector );
		}
	}
}
