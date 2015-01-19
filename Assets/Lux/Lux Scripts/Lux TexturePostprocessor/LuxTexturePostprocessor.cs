// LUX TEXTURPOSTPROCESSOR 

// both textures have to be in the same folder
// both textures have to have the "same" name
// both textures have to have the same extension
// both textures have to have the same size
// both textures have to be marked as readable

#if UNITY_EDITOR

using System;
using System.IO;
using UnityEditor;
using UnityEngine;

internal class LuxTexturePostprocessor : AssetPostprocessor {

	public const string SpecSuffix = "_LuxSPEC";
	public const string SpecShortSuffix = "LuxSPEC";
	public const string NormalSuffix = "_LuxNRM";
	public const string NormalShortSuffix = "LuxNRM";

	public void OnPostprocessTexture (Texture2D specMap) {
		if (assetPath.Contains(SpecSuffix)) {
			string filename = Path.GetFileNameWithoutExtension(assetPath);
			string[] arr = filename.Split('_');
			var origFilename = System.String.Empty;
			for (int i = 0; i < arr.Length; i++) {
				if (arr[i] == SpecShortSuffix) {
					break;
				}
				else {
					origFilename+=arr[i]+'_';
				}
			}
			origFilename += NormalShortSuffix;
			var normalpath = Path.Combine(Path.GetDirectoryName(assetPath), Path.GetFileNameWithoutExtension(origFilename));
			normalpath += Path.GetExtension(assetPath);
			
			if (File.Exists(normalpath)) {

				Debug.Log("Filtering Texture: " + filename);
				var normal = AssetDatabase.LoadAssetAtPath(normalpath, typeof (Texture2D) ) as Texture2D;
				
				int width = specMap.width;
				int height = specMap.height;
				//if(normal.width != width || normal.height != height)
				//{
				//	normal.Resize(width, height);
				//}
				int mipmapCount = specMap.mipmapCount;
				
				// Start with mip level 1
				for (int mipLevel = 1; mipLevel < mipmapCount; mipLevel++) {
					ProcessMipLevel(ref specMap, normal, width, height, mipLevel);
				}
				specMap.Apply(false, false);
				normal = null;
			}
		}	
	}


	private static void ProcessMipLevel(ref Texture2D specMap, Texture2D bumpMap, int maxwidth, int maxheight, int mipLevel)
	{
		// Create color array which will hold the processed texels for the given MipLevel
		Color32[] colors = specMap.GetPixels32(mipLevel);

		// Get NormalMap MipLevel 0
		Color32[] BumpMap = bumpMap.GetPixels32(0);

		// Calculate Width and Height for the given mipLevel
		int width = Mathf.Max(1, specMap.width >> mipLevel);
		int height = Mathf.Max(1, specMap.height >> mipLevel);

		int pointer = 0;
		int texelFootprint = 1 << mipLevel;

		for (int row = 0; row < height; row++)
			{
			for (int col = 0; col < width; col++)
				{

					float texelPosX = (float)col/width;														// equals U
					float texelPosY = (float)row/height;													// equals V
					int texelPointerX = Mathf.FloorToInt(texelPosX * maxwidth);								// remap to mipLevel 0
					int texelPointerY = Mathf.FloorToInt( (texelPosY) * maxheight);							// remap to mipLevel 0
				
				//	Sample all normal map texels from the base mip level that are within the footprint of the current mipmap texel
					Vector3 avgNormal = Vector3.zero;
					for(int y = 0; y < texelFootprint; y++)
						{
						for(int x = 0; x < texelFootprint; x++)
						{
							int samplePosX = texelPointerX + x;
							int samplePosY = texelPointerY + y;
							// Read Pixel from BumpMap out of Array
			             	Color32 normalSample = BumpMap[ samplePosY * maxheight + samplePosX];
							// Decode Normal
							Vector3 sampleNormal = new Vector3(normalSample.a/255.0f*2-1, normalSample.g/255.0f*2-1, 0);
							sampleNormal.z = Mathf.Sqrt(1.0f - sampleNormal.x * sampleNormal.x - sampleNormal.y * sampleNormal.y);
							// If normal was not compressed
							// sampleNormal = new Vector3(normalSample.r/255.0f*2-1, normalSample.g/255.0f*2-1, normalSample.b/255.0f*2-1);
							sampleNormal.Normalize();
							avgNormal += sampleNormal;
						}
					}
					avgNormal /= (float)(texelFootprint * texelFootprint);
		    	
		    	//	Get Roughness (byte to float)
		    		float glossiness = colors[pointer].a / 255.0f;
		    		float N_alpha = ((Vector3) avgNormal).magnitude;
					float variance = Mathf.Clamp01( (1.0f - N_alpha) / N_alpha );
					// Convert Roughness to Specular Power (matches Lux Blinn Phong)
					float specPower = Mathf.Pow(2, glossiness * 10 + 1) - 1.75f;
					// Apply Toksvig factor
					specPower = specPower / (1.0f + variance * specPower);
					// Convert Specular Power to Roughness and store new Roughness value (float to byte)
					colors[pointer].a = (byte)( (Mathf.Log( (specPower + 1.75f), 2.0f) - 1 ) / 10 * 255 );

				/*
				//	This would need a non dxt5 compressed normal map
					float r = ((Vector3)avgNormal).magnitude;
	        		float kappa = 10000.0f;
			        if(r < 1.0f)
			        {
			            kappa = (3 * r - r * r * r) / (1 - r * r);
			        }
			        // Compute the new roughness value
			        float roughness = colors[pointer].a / 255.0f;
			        colors[pointer].a = (byte) (Mathf.Sqrt(roughness * roughness + (1.0f / kappa))*255); 
				*/
		        	pointer++;
				}
			}
		// Apply modified mipLevel
		specMap.SetPixels32(colors, mipLevel);
	}
}

#endif
