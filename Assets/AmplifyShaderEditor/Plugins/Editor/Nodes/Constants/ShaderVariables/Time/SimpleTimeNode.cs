// Amplify Shader Editor - Visual Shader Editing Tool
// Copyright (c) Amplify Creations, Lda <info@amplify.pt>

using System;
namespace AmplifyShaderEditor
{
	[Serializable]
	[NodeAttributes( "Time", "Time", "Time in seconds with a scale multiplier" )]
	public sealed class SimpleTimeNode : ShaderVariablesNode
	{
		private const string TimeStandard = "_Time.y";
#if UNITY_2018_3_OR_NEWER
		private const string TimeSRP = "_TimeParameters.x";
#endif
		protected override void CommonInit( int uniqueId )
		{
			base.CommonInit( uniqueId );
			ChangeOutputProperties( 0, "Out", WirePortDataType.FLOAT );
			AddInputPort( WirePortDataType.FLOAT, false, "Scale" );
			m_inputPorts[ 0 ].FloatInternalData = 1;
			m_useInternalPortData = true;
			m_previewShaderGUID = "45b7107d5d11f124fad92bcb1fa53661";
			m_continuousPreviewRefresh = true;
		}

		public override string GenerateShaderForOutput( int outputId, ref MasterNodeDataCollector dataCollector, bool ignoreLocalvar )
		{
			base.GenerateShaderForOutput( outputId, ref dataCollector, ignoreLocalvar );
			string multiplier = m_inputPorts[ 0 ].GeneratePortInstructions( ref dataCollector );
			string timeGlobalVar = TimeStandard;
#if UNITY_2018_3_OR_NEWER
			if( dataCollector.IsTemplate )
			{
				if( ( dataCollector.TemplateDataCollectorInstance.IsHDRP && ASEPackageManagerHelper.CurrentHDVersion > ASESRPVersions.ASE_SRP_5_16_1 ) ||
					( dataCollector.TemplateDataCollectorInstance.IsLWRP && ASEPackageManagerHelper.CurrentLWVersion > ASESRPVersions.ASE_SRP_5_16_1 ) )
					timeGlobalVar = TimeSRP;
			}
#endif
			if( multiplier == "1.0" )
				return timeGlobalVar;

			string scaledVarName = "mulTime" + OutputId;
			string scaledVarValue = timeGlobalVar + " * " + multiplier;
			dataCollector.AddLocalVariable( UniqueId, CurrentPrecisionType, WirePortDataType.FLOAT, scaledVarName, scaledVarValue );
			return scaledVarName;
		}
	}
}
