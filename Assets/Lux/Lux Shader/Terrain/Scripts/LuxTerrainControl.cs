using System.Collections.Generic;
using UnityEngine;
//using System.Collections;
//using System.Linq;
//using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]

public class LuxTerrainControl : MonoBehaviour {

	private Terrain targetTerrain;
	private Material terrainMaterial;
	private float terrainWidth;
	private float terrainLength;
	private Vector2 tileSize;
	
	public float DetailFadeLength = 100;
	public bool Colormap = false;
	public bool DiffuseCubeIBL = false;

	public bool LinearLightingFixBillboards = true;
	public bool LinearLightingFixMeshtrees = true;
	public bool ShowFogSettingsWarning = true;

	// Use this for initialization
	void Start () {
		setShaderkeyword();
	}
	
	// Update only in editor
	void Update () {
		#if UNITY_EDITOR
		if (!Application.isPlaying) {
			targetTerrain = (Terrain)GetComponent(typeof(Terrain));
			
			if(targetTerrain.materialTemplate) {
				terrainMaterial = targetTerrain.materialTemplate;
				// Only if Lux terrain shader is assigned
				if (terrainMaterial.shader == Shader.Find("Lux/Terrain/Spec Bumped")) {
					terrainWidth = targetTerrain.terrainData.size.x;
					terrainLength = targetTerrain.terrainData.size.z;
					// Sync tiling
					for (int i = 0; i < targetTerrain.terrainData.splatPrototypes.Length; i++ ) {
						tileSize = targetTerrain.terrainData.splatPrototypes[i].tileSize;
						terrainMaterial.SetVector( "_UVs" + i.ToString(), new Vector4 (
							terrainWidth/tileSize.x,
							terrainLength/tileSize.y,
							0.0f,
							0.0f)
						);
					}
					// Synch Basemap Distance
					terrainMaterial.SetFloat( "_BasemapDistance", targetTerrain.basemapDistance);
					terrainMaterial.SetFloat( "_FadeLength", DetailFadeLength);
				}
				if (RenderSettings.fogMode != FogMode.ExponentialSquared && ShowFogSettingsWarning == true) {
					var option = (UnityEditor.EditorUtility.DisplayDialogComplex(
						"Please note:", "Lux Terrain and Tree Creator shaders only support FogMode=Exp2 by default.\nPlease change your fog settings or edit the shaders manually. See: '_Lux Terrain Shader.txt' for further details.", 
						"Ok",
						"Ok, but always remind me",
						"Ok, I have noticed this."));
					switch (option) {
						case 0:
							return;
						case 1:
							ShowFogSettingsWarning = true;
							return;
						case 2:
							ShowFogSettingsWarning = false;
							return;
					}
				}
			}
		}
		#endif
		setShaderkeyword();
	}

	void setShaderkeyword () {
		// terrain materials are a bit picky so we have to use global vars!
		if (Colormap) {
			Shader.EnableKeyword("COLORMAP_ON");
			Shader.DisableKeyword("COLORMAP_OFF");
		}
		else {
			Shader.DisableKeyword("COLORMAP_ON");
			Shader.EnableKeyword("COLORMAP_OFF");
		}
		if (DiffuseCubeIBL) {
			Shader.EnableKeyword("GLDIFFCUBE_ON");
			Shader.DisableKeyword("GLDIFFCUBE_OFF");
		}
		else {
			Shader.DisableKeyword("GLDIFFCUBE_ON");
			Shader.EnableKeyword("GLDIFFCUBE_OFF");
		}
		if (LinearLightingFixBillboards) {
			Shader.EnableKeyword("LUX_LLFIX_BILLBOARDS_ON");
			Shader.DisableKeyword("LUX_LLFIX_BILLBOARDS_OFF");
		}
		else {
			Shader.DisableKeyword("LUX_LLFIX_BILLBOARDS_ON");
			Shader.EnableKeyword("LUX_LLFIX_BILLBOARDS_OFF");
		}
		if (LinearLightingFixMeshtrees) {
			Shader.EnableKeyword("LUX_LLFIX_MESHTREESRDS_ON");
			Shader.DisableKeyword("LUX_LLFIX_MESHTREES_OFF");
		}
		else {
			Shader.DisableKeyword("LUX_LLFIX_MESHTREES_ON");
			Shader.EnableKeyword("LUX_LLFIX_MESHTREES_OFF");
		}
	}
}
