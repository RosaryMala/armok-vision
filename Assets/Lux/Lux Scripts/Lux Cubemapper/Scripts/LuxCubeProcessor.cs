//	////////////////////
//	Lux LuxCubeProcessor

//	http://seblagarde.wordpress.com/2012/06/10/amd-cubemapgen-for-physically-based-rendering/

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
public enum ConvoModes
{
	Diffuse = 0,
	Specular = 1
}

//public class LuxCubeProcessor : ScriptableWizard {
public class LuxCubeProcessor {

	public Cubemap cubeMap;
	public bool TakeNewProbe;
	public GameObject probe;

	public ConvoModes ConvolutionMode;
	public bool HighestMipIsReflection = true;
	public bool PullHDR;

	private Color[] CubeMapColors;
	
	public float SpecularPower = 2048; // Matches Lux SpecPower



//--------------------------------------------------------------------------------------
//	parameter, vars and arrays needed by the script

	private float[] solidAngles;
	private static float CP_PI = 3.14159265358979323846f;

	// matrices that map cube map indexing vectors in 3d (after face selection and divide through by the _ABSOLUTE VALUE_ of the max coord) into NVC space
	// Note this currently assumes the D3D cube face ordering and orientation
	private Vector3[ , ] sgFace2DMapping = new Vector3[ , ] {
	    //XPOS face 0
	    { new Vector3( 0,  0, -1),   	//u towards negative Z
	      new Vector3( 0, -1,  0),   	//v towards negative Y
	      new Vector3( 1,  0,  0)},  	//pos X axis  
	    //XNEG face 1
	    { new Vector3( 0,  0,  1),   	//u towards positive Z
	      new Vector3( 0, -1,  0),   	//v towards negative Y
	      new Vector3( -1,  0,  0)}, 	//neg X axis       
	    //YPOS face 2
	    { new Vector3(1, 0, 0),			//u towards positive X
	      new Vector3(0, 0, 1),			//v towards positive Z
	      new Vector3(0, 1 , 0)},		//pos Y axis  
	    //YNEG face 3
	    { new Vector3(1, 0, 0),			//u towards positive X
	      new Vector3(0, 0 , -1),		//v towards negative Z
	      new Vector3(0, -1 , 0)},		//neg Y axis  
	    //ZPOS face 4
	    { new Vector3(1, 0, 0),			//u towards positive X
	      new Vector3(0, -1, 0),		//v towards negative Y
	      new Vector3(0, 0,  1)},		//pos Z axis  
	    //ZNEG face 5
	    { new Vector3(-1, 0, 0),		//u towards negative X
	      new Vector3(0, -1, 0),		//v towards negative Y
	      new Vector3(0, 0, -1)}		//neg Z axis  
	};

	
	//The 12 edges of the cubemap
	// this table is used to average over the edges.
	private int[ , ] sg_CubeEdgeList = new int[ , ] {
	// face1, face2 
		{0,2},				// X_POS Y_POS
		{0,3},				// X_POS Y_NEG
		{0,4},				// X_POS Z_POS
		{0,5},				// X_POS Z_NEG

		{1,2},				// X_NEG Y_POS
		{1,3},				// X_NEG Y_NEG
		{1,4},				// X_NEG Z_POS
		{1,5},				// X_NEG Z_NEG

		{2,4},				// Y_POS Z_POS
		{2,5},				// Y_POS Z_NEG
		{3,4},				// Y_NEG Z_POS
		{3,5}				// Y_NEG Z_NEG
	};

	private int[ , ] sg_CubeCornerList = new int[ , ] {
	// face1, face2, face3 
		{0,2,4},			// X_POS Y_POS Z_POS
		{0,2,5},			// X_POS Y_POS Z_NEG
		{0,3,4},			// X_POS Y_NEG Z_POS
		{0,3,5},			// X_POS Y_NEG Z_NEG

		{1,2,4},			// X_NEG Y_POS Z_POS
		{1,2,5},			// X_NEG Y_POS Z_NEG
		{1,3,4},			// X_NEG Y_NEG Z_POS
		{1,3,5},			// X_NEG Y_NEG Z_NEG

	};


//--------------------------------------------------------------------------------------

    // Helper for Cubemapper Probe
    public void ProcessCubemap(Cubemap cube, bool diff, bool hdr)
    {
        if (diff)
        {
            ConvolveIrradianceEnvironmentMap(cube);
            if (hdr) FakeHDR(cube, false);
            Debug.Log("Diff Cube Processed");
        }
        else 
        {
            ConvolveRadianceEnvironmentMap(cube);
            FixupCubeEdges(cube);
            if (hdr) FakeHDR(cube, true);
            cube.filterMode = FilterMode.Trilinear;
            cube.mipMapBias = 0.5f;
            Debug.Log("Spec Cube Processed");
        }
    }

    //[MenuItem ("Lux Cubemapper/Cubemapper")]
    //static void CreateWizard () {
    //    ScriptableWizard.DisplayWizard<LuxCubeMapper>("Convolve cubemap", "Go");
    //}

    //void OnWizardCreate () {
    //    if (TakeNewProbe) RenderToCubeMap(cubeMap,probe);


    //    //	diffuse	
    //    if (ConvolutionMode == ConvoModes.Diffuse) {
    //        ConvolveIrradianceEnvironmentMap (cubeMap);
    //        if (PullHDR) FakeHDR(cubeMap, false);
    //    }

    //    if (ConvolutionMode == ConvoModes.Specular) {
    //        ConvolveRadianceEnvironmentMap (cubeMap);
    //        FixupCubeEdges(cubeMap);
    //        if (PullHDR) FakeHDR(cubeMap, true);
    //        cubeMap.filterMode = FilterMode.Trilinear;
    //        cubeMap.mipMapBias = 0.5f;
    //    }
    //}  


    //void OnWizardUpdate () {
    //}   

    //void RenderToCubeMap(Cubemap dest, GameObject probe) {
    //    var cubeCamera = new GameObject( "CubemapCamera", typeof(Camera) ) as GameObject;
    ////	cubeCamera.hideFlags = HideFlags.HideInHierarchy;
    //    var cubeCam = cubeCamera.GetComponent("Camera") as Camera;
    //    cubeCam.nearClipPlane = 0.001f;
    //    cubeCam.farClipPlane = 1000.0f;
    //    cubeCam.aspect = 1.0f;
    ////	cubeCam.hdr = true;
    //    cubeCam.cullingMask = 1 << 0;		
    //    cubeCamera.transform.position = probe.transform.position;
    //    cubeCam.RenderToCubemap(dest);
    //    GameObject.DestroyImmediate(cubeCamera);
    //}



//--------------------------------------------------------------------------------------
// ConvolveRadianceEnvironmentMap

    void ConvolveRadianceEnvironmentMap (Cubemap RadianceCube) {
	    int a_Size = RadianceCube.width;
	    int n_Size = a_Size;
	    int startMipLevel = 0;
	    float specularPower;
	    float[] specPowr = new float [] {
		    SpecularPower, SpecularPower/2.0f, SpecularPower/4.0f, SpecularPower/8.0f, SpecularPower/16.0f, SpecularPower/32.0f, SpecularPower/64.0f, SpecularPower/128.0f, SpecularPower/256.0f, SpecularPower/512.0f
	    };

	    // Calculate maxmiplevels
	    int maxMipLevels = (int)Mathf.Log(a_Size, 2) + 1;
    //
        float mytime = Time.realtimeSinceStartup;
	
	    // if HighestMipIsReflection == true then skip processing of the highest mip level
	    if (HighestMipIsReflection) {
		    startMipLevel = 1;
		    n_Size = n_Size >> 1;		
	    } 

	    for (int mipLevel = startMipLevel; mipLevel < maxMipLevels; mipLevel++)
	    {
		    Vector4[] m_NormCubeMapArray = new Vector4[n_Size*n_Size*6];
		    BuildNormalizerSolidAngleArray(n_Size, ref m_NormCubeMapArray);

		    specularPower = specPowr[mipLevel];
		    float Angle;
		    // If we use SpecularPower, automatically calculate the a_BaseFilterAngle required, this will speed the process
		    Angle = GetBaseFilterAngle(specularPower);
		    // Go for it:
		    FilterCubefaces(RadianceCube, m_NormCubeMapArray, mipLevel, Angle, specularPower);
    //		FilterCubefacesBF(RadianceCube, mipLevel, Angle, specularPower);
		    n_Size = n_Size >> 1;
	    }
    //
        Debug.Log(Time.realtimeSinceStartup - mytime);

	    RadianceCube.Apply(false);
	
    }


    void FilterCubefaces (Cubemap RadianceCubeMap, Vector4[] m_NormCubeMapArray, int mipLevel, float a_FilterConeAngle, float a_SpecularPower)
    {
	    // Read the first CubeFace
	    Color[] InputCubeFacePixels = RadianceCubeMap.GetPixels(CubemapFace.PositiveX, mipLevel);
	    // Get its dimensions
	    int faceLength = InputCubeFacePixels.Length;
	    int a_Size = (int)Mathf.Sqrt(faceLength);
	    // Create new array for all Faces
	    Color[] PixelsOfAllFaces = new Color[faceLength * 6];
	    // Copy first face
	    InputCubeFacePixels.CopyTo(PixelsOfAllFaces, 0);
	    // Copy all other Faces
	    for (int readFace = 1; readFace < 6; readFace++ ) {
		    InputCubeFacePixels = RadianceCubeMap.GetPixels((CubemapFace)readFace, mipLevel);
		    InputCubeFacePixels.CopyTo(PixelsOfAllFaces, faceLength * readFace);
	    }
	    InputCubeFacePixels = null;
	    // Declare jagged output array and init its child arrays
	    Color[][] OutputCubeFacePixels = new Color[6][];
	    OutputCubeFacePixels[0] = new Color[faceLength];
	    OutputCubeFacePixels[1] = new Color[faceLength];
	    OutputCubeFacePixels[2] = new Color[faceLength];
	    OutputCubeFacePixels[3] = new Color[faceLength];
	    OutputCubeFacePixels[4] = new Color[faceLength];
	    OutputCubeFacePixels[5] = new Color[faceLength];

	    // FilterCubeSurfaces
	    float srcTexelAngle;
        float dotProdThresh;
        int filterSize;
	    // Angle about center tap to define filter cone
	    float filterAngle;
	    // Min angle a src texel can cover (in degrees)
	    srcTexelAngle = (180.0f / CP_PI) * Mathf.Atan2(1.0f, (float)a_Size);  
	    // Filter angle is 1/2 the cone angle
        filterAngle = a_FilterConeAngle / 2.0f;
        // Ensure filter angle is larger than a texel
        if(filterAngle < srcTexelAngle)
        {
            filterAngle = srcTexelAngle;    
        }
        // Ensure filter cone is always smaller than the hemisphere
        if(filterAngle > 90.0f)
        {
            filterAngle = 90.0f;
        }
	    // The maximum number of texels in 1D the filter cone angle will cover
	    // Used to determine bounding box size for filter extents
	    filterSize = (int)Mathf.Ceil(filterAngle / srcTexelAngle);
	    // Ensure conservative region always covers at least one texel
        if(filterSize < 1)
        {
            filterSize = 1;
        }
	    // dotProdThresh threshold based on cone angle to determine whether or not taps 
	    // Reside within the cone angle
	    dotProdThresh = Mathf.Cos( (CP_PI / 180.0f) * filterAngle );

	    // Process required faces
	    for (int a_FaceIdx = 0; a_FaceIdx < 6; a_FaceIdx++)
	    {
		    // Iterate over dst cube map face texel
		    for (int a_V  = 0; a_V < a_Size; a_V++)
		    {
			    for (int a_U = 0; a_U < a_Size; a_U++)
			    {
				    Vector4 Sample = new Vector4();
				    Color tempCol = new Color();
				    // Get center tap direction
				    Vector3 centerTapDir = TexelToVect(a_FaceIdx, (float)a_U, (float)a_V, a_Size);
				    //--------------------------------------------------------------------------------------
				    // ProcessFilterExtents 
				    float weightAccum = 0.0f;
				    int startFacePtr = 0;
				    // Iterate over cubefaces
				    for(int iFaceIdx = 0; iFaceIdx < 6; iFaceIdx++ )
				    {
					    // Pointer to the start of the given face
					    startFacePtr = a_Size*a_Size*iFaceIdx;
					    for(int v = 0; v < a_Size; v++)
					    {
						    for(int u = 0; u < a_Size; u++)
						    {
							    // CP_FILTER_TYPE_COSINE_POWER

							    // Read normalCube single "pixel"
							    Vector4 m_NormCubeMap_pixel = m_NormCubeMapArray[startFacePtr + a_Size*v + u];
							    // Pointer to direction in cube map associated with texel
							    // Vector3 texelVect = TexelToVect(iFaceIdx, (float)u, (float)v, a_Size);
							    Vector3 texelVect;
							    // Optimized version
							    texelVect.x = m_NormCubeMap_pixel[0];// * 2.0f - 1.0f;
							    texelVect.y = m_NormCubeMap_pixel[1];// * 2.0f - 1.0f;
							    texelVect.z = m_NormCubeMap_pixel[2];// * 2.0f - 1.0f;
							    // Check dot product to see if texel is within cone
							    float tapDotProd = Vector3.Dot(texelVect, centerTapDir);
							    float weight = 0.0f;
							    if( tapDotProd >= dotProdThresh && tapDotProd > 0.0f )
							    {
								    // Weight should be proportional to the solid angle of the tap
                 				    // weight = TexelCoordSolidAngle(iFaceIdx, (float)u, (float)v, a_Size);
								    weight = m_NormCubeMap_pixel[3];
								    // Here we decide if we use a Phong/Blinn or a Phong/Blinn BRDF.
								    // Phong/Blinn BRDF is just the Phong/Blinn model multiply by the cosine of the lambert law
								    // so just adding one to specularpower do the trick.					   
								    // weight *= pow(tapDotProd, (a_SpecularPower + (float32)IsPhongBRDF));
							    // CP_FILTER_TYPE_COSINE_POWER
								    weight *= Mathf.Pow(tapDotProd, a_SpecularPower);
							    // CP_FILTER_TYPE_COSINE
								    //weight *= tapDotProd;
								
							    }
							    // Accumulate weight
							    weightAccum += weight;
							    // Get pixel from the input cubeMap array
							    tempCol = PixelsOfAllFaces[a_Size*a_Size*iFaceIdx + a_Size*v + u];
							    Sample += new Vector4(tempCol.r * weight, tempCol.g * weight, tempCol.b * weight, 1.0f);
						    }
					    }
				    }
				    // one pixel processed
				    // Lux needs alpha!
				    OutputCubeFacePixels[a_FaceIdx][a_V*a_Size + a_U] = new Color(Sample.x/ weightAccum, Sample.y/ weightAccum, Sample.z/ weightAccum, 1.0f);
			    // end inner loops
			    }
		    }
	    }
	    // Write Pixel from the jagged array back to the cubemap faces
	    for (int writeFace = 0; writeFace < 6; writeFace++ ) {
		    Color[] tempColors = OutputCubeFacePixels[writeFace];
		    RadianceCubeMap.SetPixels(tempColors, (CubemapFace)writeFace, mipLevel);
	    }
    }




// brute force: no look up table used

    void FilterCubefacesBF (Cubemap RadianceCubeMap, int mipLevel, float a_FilterConeAngle, float a_SpecularPower)
    {
	    // Read the first CubeFace
	    Color[] InputCubeFacePixels = RadianceCubeMap.GetPixels(CubemapFace.PositiveX, mipLevel);
	    // Get its dimensions
	    int faceLength = InputCubeFacePixels.Length;
	    int a_Size = (int)Mathf.Sqrt(faceLength);
	    // Create new array for all Faces
	    Color[] PixelsOfAllFaces = new Color[faceLength * 6];
	    // Copy first face
	    InputCubeFacePixels.CopyTo(PixelsOfAllFaces, 0);
	    // Copy all other Faces
	    for (int readFace = 1; readFace < 6; readFace++ ) {
		    InputCubeFacePixels = RadianceCubeMap.GetPixels((CubemapFace)readFace, mipLevel);
		    InputCubeFacePixels.CopyTo(PixelsOfAllFaces, faceLength * readFace);
	    }
	    InputCubeFacePixels = null;

	    // declare jagged output array and init its child arrays
	    Color[][] OutputCubeFacePixels = new Color[6][];
	    OutputCubeFacePixels[0] = new Color[faceLength];
	    OutputCubeFacePixels[1] = new Color[faceLength];
	    OutputCubeFacePixels[2] = new Color[faceLength];
	    OutputCubeFacePixels[3] = new Color[faceLength];
	    OutputCubeFacePixels[4] = new Color[faceLength];
	    OutputCubeFacePixels[5] = new Color[faceLength];


	    // FilterCubeSurfaces
	    float srcTexelAngle;
        float dotProdThresh;
        int filterSize;
	    //angle about center tap to define filter cone
	    float filterAngle;
	    //min angle a src texel can cover (in degrees)
	    srcTexelAngle = (180.0f / CP_PI) * Mathf.Atan2(1.0f, (float)a_Size);  
	    //filter angle is 1/2 the cone angle
        filterAngle = a_FilterConeAngle / 2.0f;
        //ensure filter angle is larger than a texel
        if(filterAngle < srcTexelAngle)
        {
            filterAngle = srcTexelAngle;    
        }
        //ensure filter cone is always smaller than the hemisphere
        if(filterAngle > 90.0f)
        {
            filterAngle = 90.0f;
        }

	    // The maximum number of texels in 1D the filter cone angle will cover
	    // Used to determine bounding box size for filter extents
	    filterSize = (int)Mathf.Ceil(filterAngle / srcTexelAngle);
	    // Ensure conservative region always covers at least one texel
        if(filterSize < 1)
        {
            filterSize = 1;
        }

	    // dotProdThresh threshold based on cone angle to determine whether or not taps 
	    // Reside within the cone angle
	    dotProdThresh = Mathf.Cos( (CP_PI / 180.0f) * filterAngle );

	    // Process required faces
	    for (int a_FaceIdx = 0; a_FaceIdx < 6; a_FaceIdx++)
	    {
		    // Iterate over dst cube map face texel
		    for (int a_V  = 0; a_V < a_Size; a_V++)
		    {
			    for (int a_U = 0; a_U < a_Size; a_U++)
			    {
				    Vector4 Sample = new Vector4();
				    Color tempCol = new Color();
				
				    // get center tap direction
				    Vector3 centerTapDir = TexelToVect(a_FaceIdx, (float)a_U, (float)a_V, a_Size);

				    //--------------------------------------------------------------------------------------
				    //	ProcessFilterExtents 

				    float weightAccum = 0.0f;

				    // Iterate over cubefaces
				    for(int iFaceIdx = 0; iFaceIdx < 6; iFaceIdx++ )
				    {
					    for(int v = 0; v < a_Size; v++)
					    {
						    for(int u = 0; u < a_Size; u++)
						    {
							    // CP_FILTER_TYPE_COSINE_POWER
							    // Pointer to direction in cube map associated with texel
							    Vector3 texelVect = TexelToVect(iFaceIdx, (float)u, (float)v, a_Size);
							    // Check dot product to see if texel is within cone
							    float tapDotProd = Vector3.Dot(texelVect, centerTapDir);


							    float weight = 0.0f;
							    if( tapDotProd >= dotProdThresh && tapDotProd > 0.0f )
							    {
								
								    // Weight should be proportional to the solid angle of the tap
                 				    weight = TexelCoordSolidAngle(iFaceIdx, (float)u, (float)v, a_Size);
								    // Here we decide if we use a Phong/Blinn or a Phong/Blinn BRDF.
								    // Phong/Blinn BRDF is just the Phong/Blinn model multiply by the cosine of the lambert law
								    // so just adding one to specularpower do the trick.					   
								    // weight *= pow(tapDotProd, (a_SpecularPower + (float32)IsPhongBRDF));
							    // CP_FILTER_TYPE_COSINE_POWER
								    weight *= Mathf.Pow(tapDotProd, a_SpecularPower);
							    // CP_FILTER_TYPE_COSINE
								    //weight *= tapDotProd;
							    }
							    // Accumulate weight
							    weightAccum += weight;

							    // Get pixel from the input cubeMap array
							    tempCol = PixelsOfAllFaces[a_Size*a_Size*iFaceIdx + a_Size*v + u];
							    Sample += new Vector4(tempCol.r * weight, tempCol.g * weight, tempCol.b * weight, 1.0f);
						    }
					    }
				    }
				    // one pixel processed
				    // Lux needs alpha!
				    OutputCubeFacePixels[a_FaceIdx][a_V*a_Size + a_U] = new Color(Sample.x/ weightAccum, Sample.y/ weightAccum, Sample.z/ weightAccum, 1.0f);
			    // end inner loops
			    }
		    }
	    }
	    // Write Pixel from the jagged array back to the cubemap faces
	    for (int writeFace = 0; writeFace < 6; writeFace++ ) {
		    Color[] tempColors = OutputCubeFacePixels[writeFace];
		    RadianceCubeMap.SetPixels(tempColors, (CubemapFace)writeFace, mipLevel);
	    }
    }

    //--------------------------------------------------------------------------------------
    // Irridiance Convolution based on SH

    void ConvolveIrradianceEnvironmentMap(Cubemap irrCubeMap)
    {
	    int a_Size = irrCubeMap.width;

	    Vector4[] m_NormCubeMapArray = new Vector4[a_Size*a_Size*6];
	    BuildNormalizerSolidAngleArray(a_Size, ref m_NormCubeMapArray);

	    //This is a custom implementation of D3DXSHProjectCubeMap to avoid to deal with LPDIRECT3DSURFACE9 pointer
	    //Use Sh order 2 for a total of 9 coefficient as describe in http://www.cs.berkeley.edu/~ravir/papers/envmap/
	    //accumulators are 64-bit floats in order to have the precision needed 
	    //over a summation of a large number of pixels 
	    double[] SHr = new double[25]; // NUM_SH_COEFFICIENT
	    double[] SHg = new double[25];
	    double[] SHb = new double[25];
	    double[] SHdir = new double[25];

	    double weightAccum = 0.0;
	    double weight = 0.0;

	    int startFacePtr = 0;

	    for (int iFaceIdx = 0; iFaceIdx < 6; iFaceIdx++) {

		    // read pixels of m_NormCubeMap
		    //var m_NormCubeMap_pixels  = new Color[m_NormCubeMap.width*m_NormCubeMap.height];
		    //m_NormCubeMap_pixels = m_NormCubeMap.GetPixels((CubemapFace)iFaceIdx);

		    // Pointer to the start of the given face in m_NormCubeMapArray 
		    startFacePtr = a_Size*a_Size*iFaceIdx;

		    // read all pixels of irrCubeMap
            var cubeMap_pixels = new Color[irrCubeMap.width * irrCubeMap.height];
		    cubeMap_pixels = irrCubeMap.GetPixels((CubemapFace)iFaceIdx);


			    for (int y = 0; y < a_Size; y++) {
				    for (int x = 0; x < a_Size; x++) {

					    // read normalCube single pixel
					    Vector4 m_NormCubeMap_pixel = m_NormCubeMapArray[startFacePtr + y*a_Size + x];

					    // read originalCube single pixel
					    Color cubeMap_pixel = cubeMap_pixels[y*a_Size + x];

					    // solid angle stored in 4th channel of normalizer/solid angle cube map
					    weight = m_NormCubeMap_pixel[3];
					    //weight = TexelCoordSolidAngle(iFaceIdx, (float)x, (float)y, a_Size);

					    // pointer to direction and solid angle in cube map associated with texel
					    Vector3 texelVect;
					    texelVect.x = m_NormCubeMap_pixel[0];
					    texelVect.y = m_NormCubeMap_pixel[1];
					    texelVect.z = m_NormCubeMap_pixel[2];
					    //texelVect = TexelToVect(iFaceIdx, (float)x, (float)y, a_Size);
	
					    EvalSHBasis(texelVect, ref SHdir);

					    // read original colors and convert to float64
					    double R = cubeMap_pixel[0];
					    double G = cubeMap_pixel[1];
					    double B = cubeMap_pixel[2];

					    for (int i = 0; i < 25; i++)
					    {
						    SHr[i] += R * SHdir[i] * weight;
						    SHg[i] += G * SHdir[i] * weight;
						    SHb[i] += B * SHdir[i] * weight;
					    }
					    weightAccum += weight;
			    }
		    }
	    }
	    // Normalization - The sum of solid angle should be equal to the solid angle of the sphere (4 PI), so
	    // Normalize in order our weightAccum exactly match 4 PI.
	    for (int i = 0; i < 25; ++i)
	    {
		    SHr[i] *= 4.0 * CP_PI / weightAccum;
		    SHg[i] *= 4.0 * CP_PI / weightAccum;
		    SHb[i] *= 4.0 * CP_PI / weightAccum;
	    }

	    // Second step - Generate cubemap from SH coefficient

	    // Normalized vectors per cubeface and per-texel solid angle
	    // Why do we do it a 2nd time????
	    BuildNormalizerSolidAngleArray(a_Size, ref m_NormCubeMapArray);

	    for (int iFaceIdx = 0; iFaceIdx < 6; iFaceIdx++) {

		    // Pointer to the start of the given face in m_NormCubeMapArray 
		    startFacePtr = a_Size*a_Size*iFaceIdx;

		    for (int y = 0; y < a_Size; y++) {
			    for (int x = 0; x < a_Size; x++) {
				    // read normalCube pixel
				    Vector4 m_NormCubeMap_pixel = m_NormCubeMapArray[startFacePtr + y*a_Size + x];

				    // read normalvector and pass it to EvalSHBasis to get SHdir
				    Vector3 texelVect;
				    texelVect.x = m_NormCubeMap_pixel[0];
				    texelVect.y = m_NormCubeMap_pixel[1];
				    texelVect.z = m_NormCubeMap_pixel[2];
				    //texelVect = TexelToVect(iFaceIdx, (float)x, (float)y, a_Size);

				    EvalSHBasis( texelVect, ref SHdir);

				    // set color values
				    double R = 0.0;
				    double G = 0.0;
				    double B = 0.0;
				
				    for (int i = 0; i < 25; ++i)
				    {
					    R += (SHr[i] * SHdir[i] * SHBandFactor[i]);
					    G += (SHg[i] * SHdir[i] * SHBandFactor[i]);
					    B += (SHb[i] * SHdir[i] * SHBandFactor[i]);
				    }
				    // Lux needs alpha!
				    irrCubeMap.SetPixel((CubemapFace)iFaceIdx, x, y, new Color((float)R,(float)G,(float)B, 1.0f ));
			    }
		    }
	    }
	    irrCubeMap.Apply();
    }

    //--------------------------------------------------------------------------------------
    // This function return the BaseFilterAngle require by cubemapgen to its FilterExtends
    // It allow to optimize the texel to access base on the specular power.
    static float GetBaseFilterAngle(float cosinePower)
    {
	    // We want to find the alpha such that:
	    // cos(alpha)^cosinePower = epsilon
	    // That's: acos(epsilon^(1/cosinePower))
	    const float threshold = 0.000001f;  // Empirical threshold (Work perfectly, didn't check for a more big number, may get some performance and still god approximation)
	    float Angle = 180.0f;
	    if (Angle != 0.0f)
	    {
		    Angle = Mathf.Acos(Mathf.Pow(threshold, 1.0f / cosinePower)); 
		    Angle *= 180.0f / CP_PI; // Convert to degree
		    Angle *= 2.0f; // * 2.0f because cubemapgen divide by 2 later
	    }

	    return Angle;
    }

    //--------------------------------------------------------------------------------------
    // Convert cubemap face texel coordinates and face idx to 3D vector

    Vector3 TexelToVect(int a_FaceIdx, float a_U, float a_V, int a_Size) {
	    float nvcU, nvcV;

	    nvcU = (2.0f * ((float)a_U + 0.5f) / (float)a_Size ) - 1.0f;
	    nvcV = (2.0f * ((float)a_V + 0.5f) / (float)a_Size ) - 1.0f;
	    // U contribution
	    Vector3 Dir = sgFace2DMapping[a_FaceIdx, 0] * nvcU;
	    // V contribution
	    Vector3 tempVec = sgFace2DMapping[a_FaceIdx, 1] * nvcV;
	    Dir += tempVec;
	    // Add face axis
	    Dir += sgFace2DMapping[a_FaceIdx, 2];
	    // Normalize vector
	    Dir = Vector3.Normalize(Dir);
	    return(Dir);
    }

    //--------------------------------------------------------------------------------------
    // Convert 3D vector to cubemap face texel coordinates and face idx 

    private Vector3 VectToTexelCoord(Vector3 a_XYZ, int a_Size) {
	    float nvcU, nvcV;
	    float maxCoord;
	    Vector3 onFaceXYZ;
	    int faceIdx;
	    float u, v;
		    // Get Absolute value
	    Vector3 absXYZ = new Vector3(Mathf.Abs(a_XYZ.x), Mathf.Abs(a_XYZ.y), Mathf.Abs(a_XYZ.z));
	
	    if( (absXYZ[0] >= absXYZ[1]) && (absXYZ[0] >= absXYZ[2]) )
	    {
		    maxCoord = absXYZ[0];
		    if(a_XYZ[0] >= 0) faceIdx = 0; // face = XPOS -> FACE_X_POS
		    else faceIdx = 1; // FACE_X_NEG;                    
	    }
	    else if ( (absXYZ[1] >= absXYZ[0]) && (absXYZ[1] >= absXYZ[2]) )
	    {
		    maxCoord = absXYZ[1];
		    if(a_XYZ[1] >= 0) faceIdx = 2; // face = XPOS -> FACE_Y_POS  
		    else faceIdx = 3; // FACE_Y_NEG;                    
	    }
	    else
	    {
		    maxCoord = absXYZ[2];
		    if(a_XYZ[2] >= 0) faceIdx = 4;  // face = XPOS -> FACE_Z_POS 
		    else faceIdx = 5; // FACE_Z_NEG;                    
	    }
	    // Divide through by max coord so face vector lies on cube face
	    onFaceXYZ = a_XYZ * 1.0f/maxCoord;
	    nvcU = Vector3.Dot(sgFace2DMapping[faceIdx, 0] , onFaceXYZ ); // U-Direction
	    nvcV = Vector3.Dot(sgFace2DMapping[faceIdx, 1] , onFaceXYZ ); // V-Direction
	    // Modify original AMD code to return value from 0 to Size - 1
    //	As we will sample multiple pixels we skip (int)Mathf.Floor here
	    u = (a_Size - 1) * 0.5f * (nvcU + 1.0f);
	    v = (a_Size - 1) * 0.5f * (nvcV + 1.0f);
	    a_XYZ.x = faceIdx;
	    a_XYZ.y = u;
	    a_XYZ.z = v;
	    return a_XYZ;
    }

    //--------------------------------------------------------------------------------------
    //	Compute solid angle of given texel in cubemap face for weighting taps in the 
    //	kernel by the area they project to on the unit sphere.

    //	Original code from Ignacio Castaño

    float TexelCoordSolidAngle(int a_FaceIdx, float a_U, float a_V, int faceSize)
    {
	    // Scale up to [-1, 1] range (inclusive), offset by 0.5 to point to texel center.
		    float U = (2.0f * ((float)a_U + 0.5f) / (float)faceSize ) - 1.0f;
		    float V = (2.0f * ((float)a_V + 0.5f) / (float)faceSize ) - 1.0f;

		    float InvResolution = 1.0f / faceSize;
		    // U and V are the -1..1 texture coordinate on the current face.
        // Get projected area for this texel
        float x0 = U - InvResolution;
        float y0 = V - InvResolution;
        float x1 = U + InvResolution;
        float y1 = V + InvResolution;
        float SolidAngle = AreaElement(x0, y0) - AreaElement(x0, y1) - AreaElement(x1, y0) + AreaElement(x1, y1);
    //	Returns values between 0.001 and 0.009
    //	So lets multiply it by 100.0f
        return SolidAngle * 100.0f;
    }
    static float AreaElement( float x, float y )
    {
	    return Mathf.Atan2(x * y, Mathf.Sqrt(x * x + y * y + 1));
    }

    //--------------------------------------------------------------------------------------
    // Fixup cube edges

    void FixupCubeEdges (Cubemap CubeMap)
    {
	    int maxMipLevels = (int)(Mathf.Log((float)CubeMap.width, 2.0f)) + 1;
	    int base_Size = CubeMap.width;
	
	    // Do not perform any edge fixed for mip level 0
	    for (int mipLevel = 1; mipLevel < maxMipLevels; mipLevel++)
	    {
		    // declare jagged array for all faces and init its child arrays
		    Color[][] PixelsOfAllFaces = new Color[6][];
		    PixelsOfAllFaces[0] = CubeMap.GetPixels(CubemapFace.PositiveX, mipLevel);
		    PixelsOfAllFaces[1] = CubeMap.GetPixels(CubemapFace.NegativeX, mipLevel);
		    PixelsOfAllFaces[2] = CubeMap.GetPixels(CubemapFace.PositiveY, mipLevel);
		    PixelsOfAllFaces[3] = CubeMap.GetPixels(CubemapFace.NegativeY, mipLevel);
		    PixelsOfAllFaces[4] = CubeMap.GetPixels(CubemapFace.PositiveZ, mipLevel);
		    PixelsOfAllFaces[5] = CubeMap.GetPixels(CubemapFace.NegativeZ, mipLevel);

		    int a_Size = base_Size >> mipLevel;
		    // As we use a_Size as pointer in our arrays we have to lower it by 1
		    a_Size -= 1;


		    int a_FixupWidth = 3;
		    int fixupDist = (int)Mathf.Min( a_FixupWidth, a_Size / 2);

		    AverageCorner(a_Size, PixelsOfAllFaces[sg_CubeCornerList[0,0]], PixelsOfAllFaces[sg_CubeCornerList[0,1]], PixelsOfAllFaces[sg_CubeCornerList[0,2]], 0, 0, a_Size, a_Size, a_Size, 0);
		    AverageCorner(a_Size, PixelsOfAllFaces[sg_CubeCornerList[1,0]], PixelsOfAllFaces[sg_CubeCornerList[1,1]], PixelsOfAllFaces[sg_CubeCornerList[1,2]], a_Size, 0, a_Size, 0, 0, 0);
		    AverageCorner(a_Size, PixelsOfAllFaces[sg_CubeCornerList[2,0]], PixelsOfAllFaces[sg_CubeCornerList[2,1]], PixelsOfAllFaces[sg_CubeCornerList[2,2]], 0, a_Size, a_Size, 0, a_Size, a_Size);
		    AverageCorner(a_Size, PixelsOfAllFaces[sg_CubeCornerList[3,0]], PixelsOfAllFaces[sg_CubeCornerList[3,1]], PixelsOfAllFaces[sg_CubeCornerList[3,2]], a_Size, a_Size, a_Size, a_Size, 0, a_Size);
		    AverageCorner(a_Size, PixelsOfAllFaces[sg_CubeCornerList[4,0]], PixelsOfAllFaces[sg_CubeCornerList[4,1]], PixelsOfAllFaces[sg_CubeCornerList[4,2]], a_Size, 0, 0, a_Size, 0, 0);
		    AverageCorner(a_Size, PixelsOfAllFaces[sg_CubeCornerList[5,0]], PixelsOfAllFaces[sg_CubeCornerList[5,1]], PixelsOfAllFaces[sg_CubeCornerList[5,2]], 0, 0, 0, 0, a_Size, 0);
		    AverageCorner(a_Size, PixelsOfAllFaces[sg_CubeCornerList[6,0]], PixelsOfAllFaces[sg_CubeCornerList[6,1]], PixelsOfAllFaces[sg_CubeCornerList[6,2]], a_Size, a_Size, 0, 0, 0, a_Size);
		    AverageCorner(a_Size, PixelsOfAllFaces[sg_CubeCornerList[7,0]], PixelsOfAllFaces[sg_CubeCornerList[7,1]], PixelsOfAllFaces[sg_CubeCornerList[7,2]], 0, a_Size, 0, a_Size, a_Size, a_Size);

		    // Perform 2nd iteration in reverse order

		    AverageCorner(a_Size, PixelsOfAllFaces[sg_CubeCornerList[7,0]], PixelsOfAllFaces[sg_CubeCornerList[7,1]], PixelsOfAllFaces[sg_CubeCornerList[7,2]], 0, a_Size, 0, a_Size, a_Size, a_Size);
		    AverageCorner(a_Size, PixelsOfAllFaces[sg_CubeCornerList[6,0]], PixelsOfAllFaces[sg_CubeCornerList[6,1]], PixelsOfAllFaces[sg_CubeCornerList[6,2]], a_Size, a_Size, 0, 0, 0, a_Size);
		    AverageCorner(a_Size, PixelsOfAllFaces[sg_CubeCornerList[5,0]], PixelsOfAllFaces[sg_CubeCornerList[5,1]], PixelsOfAllFaces[sg_CubeCornerList[5,2]], 0, 0, 0, 0, a_Size, 0);
		    AverageCorner(a_Size, PixelsOfAllFaces[sg_CubeCornerList[4,0]], PixelsOfAllFaces[sg_CubeCornerList[4,1]], PixelsOfAllFaces[sg_CubeCornerList[4,2]], a_Size, 0, 0, a_Size, 0, 0);
		    AverageCorner(a_Size, PixelsOfAllFaces[sg_CubeCornerList[3,0]], PixelsOfAllFaces[sg_CubeCornerList[3,1]], PixelsOfAllFaces[sg_CubeCornerList[3,2]], a_Size, a_Size, a_Size, a_Size, 0, a_Size);
		    AverageCorner(a_Size, PixelsOfAllFaces[sg_CubeCornerList[2,0]], PixelsOfAllFaces[sg_CubeCornerList[2,1]], PixelsOfAllFaces[sg_CubeCornerList[2,2]], 0, a_Size, a_Size, 0, a_Size, a_Size);
		    AverageCorner(a_Size, PixelsOfAllFaces[sg_CubeCornerList[1,0]], PixelsOfAllFaces[sg_CubeCornerList[1,1]], PixelsOfAllFaces[sg_CubeCornerList[1,2]], a_Size, 0, a_Size, 0, 0, 0);
		    AverageCorner(a_Size, PixelsOfAllFaces[sg_CubeCornerList[0,0]], PixelsOfAllFaces[sg_CubeCornerList[0,1]], PixelsOfAllFaces[sg_CubeCornerList[0,2]], 0, 0, a_Size, a_Size, a_Size, 0);

	

		    // Average Edges
		    // Note that this loop does not process the corner texels, since they have already been averaged
		    for (int i = 1; i < (a_Size - 1); i++)
		    {
			    AverageEdge(a_Size, PixelsOfAllFaces[sg_CubeEdgeList[0,0]], PixelsOfAllFaces[sg_CubeEdgeList[0,1]], i, 0, a_Size, a_Size - i, fixupDist, new Vector4(0, 1, -1, 0) );
			    AverageEdge(a_Size, PixelsOfAllFaces[sg_CubeEdgeList[1,0]], PixelsOfAllFaces[sg_CubeEdgeList[1,1]], i, a_Size, a_Size, i, fixupDist, new Vector4(0, -1, -1, 0) );
			    AverageEdge(a_Size, PixelsOfAllFaces[sg_CubeEdgeList[2,0]], PixelsOfAllFaces[sg_CubeEdgeList[2,1]], 0, i, a_Size, i, fixupDist, new Vector4(1, 0, -1, 0) );
			    AverageEdge(a_Size, PixelsOfAllFaces[sg_CubeEdgeList[3,0]], PixelsOfAllFaces[sg_CubeEdgeList[3,1]], a_Size, i, 0, i, fixupDist, new Vector4(-1, 0, 1, 0) );
			
			    AverageEdge(a_Size, PixelsOfAllFaces[sg_CubeEdgeList[4,0]], PixelsOfAllFaces[sg_CubeEdgeList[4,1]], i, 0, 0, i, fixupDist, new Vector4(0, 1, 1, 0) );
			    AverageEdge(a_Size, PixelsOfAllFaces[sg_CubeEdgeList[5,0]], PixelsOfAllFaces[sg_CubeEdgeList[5,1]], i, a_Size, 0, a_Size - i, fixupDist, new Vector4(0, -1, 1, 0) );
			    AverageEdge(a_Size, PixelsOfAllFaces[sg_CubeEdgeList[6,0]], PixelsOfAllFaces[sg_CubeEdgeList[6,1]], a_Size, i, 0, i, fixupDist, new Vector4(-1, 0, 1, 0) );
			    AverageEdge(a_Size, PixelsOfAllFaces[sg_CubeEdgeList[7,0]], PixelsOfAllFaces[sg_CubeEdgeList[7,1]], 0, i, a_Size, i, fixupDist, new Vector4(1, 0, -1, 0) );
			
			    AverageEdge(a_Size, PixelsOfAllFaces[sg_CubeEdgeList[8,0]], PixelsOfAllFaces[sg_CubeEdgeList[8,1]], i, a_Size, i, 0, fixupDist, new Vector4(0, -1, 0, 1) );
			    AverageEdge(a_Size, PixelsOfAllFaces[sg_CubeEdgeList[9,0]], PixelsOfAllFaces[sg_CubeEdgeList[9,1]], i, 0, a_Size - i, 0, fixupDist, new Vector4(0, 1, 0, 1) );
			    AverageEdge(a_Size, PixelsOfAllFaces[sg_CubeEdgeList[10,0]], PixelsOfAllFaces[sg_CubeEdgeList[10,1]], i, 0, i, a_Size, fixupDist, new Vector4(0, 1, 0, -1) );
			    AverageEdge(a_Size, PixelsOfAllFaces[sg_CubeEdgeList[11,0]], PixelsOfAllFaces[sg_CubeEdgeList[11,1]], i, a_Size, a_Size - i, a_Size, fixupDist, new Vector4(0, -1, 0, -1) );
		
		    }
		    // Perform 2nd iteration in reverse order
	    /*	for (int i = 0; i < (a_Size); i++)
		    {
			    AverageEdge(a_Size, PixelsOfAllFaces[sg_CubeEdgeList[11,0]], PixelsOfAllFaces[sg_CubeEdgeList[11,1]], i, a_Size, a_Size - i, a_Size, fixupDist, new Vector4(0, -1, 0, -1) );
			    AverageEdge(a_Size, PixelsOfAllFaces[sg_CubeEdgeList[10,0]], PixelsOfAllFaces[sg_CubeEdgeList[10,1]], i, 0, i, a_Size, fixupDist, new Vector4(0, 1, 0, -1) );
			    AverageEdge(a_Size, PixelsOfAllFaces[sg_CubeEdgeList[9,0]], PixelsOfAllFaces[sg_CubeEdgeList[9,1]], i, 0, a_Size - i, 0, fixupDist, new Vector4(0, 1, 0, 1) );
			    AverageEdge(a_Size, PixelsOfAllFaces[sg_CubeEdgeList[8,0]], PixelsOfAllFaces[sg_CubeEdgeList[8,1]], i, a_Size, i, 0, fixupDist, new Vector4(0, -1, 0, 1) );
			
			    AverageEdge(a_Size, PixelsOfAllFaces[sg_CubeEdgeList[7,0]], PixelsOfAllFaces[sg_CubeEdgeList[7,1]], 0, i, a_Size, i, fixupDist, new Vector4(1, 0, -1, 0) );
			    AverageEdge(a_Size, PixelsOfAllFaces[sg_CubeEdgeList[6,0]], PixelsOfAllFaces[sg_CubeEdgeList[6,1]], a_Size, i, 0, i, fixupDist, new Vector4(-1, 0, 1, 0) );
			    AverageEdge(a_Size, PixelsOfAllFaces[sg_CubeEdgeList[5,0]], PixelsOfAllFaces[sg_CubeEdgeList[5,1]], i, a_Size, 0, a_Size - i, fixupDist, new Vector4(0, -1, 1, 0) );
			    AverageEdge(a_Size, PixelsOfAllFaces[sg_CubeEdgeList[4,0]], PixelsOfAllFaces[sg_CubeEdgeList[4,1]], i, 0, 0, i, fixupDist, new Vector4(0, 1, 1, 0) );	
			
			    AverageEdge(a_Size, PixelsOfAllFaces[sg_CubeEdgeList[3,0]], PixelsOfAllFaces[sg_CubeEdgeList[3,1]], a_Size, i, 0, i, fixupDist, new Vector4(-1, 0, 1, 0) );
			    AverageEdge(a_Size, PixelsOfAllFaces[sg_CubeEdgeList[2,0]], PixelsOfAllFaces[sg_CubeEdgeList[2,1]], 0, i, a_Size, i, fixupDist, new Vector4(1, 0, -1, 0) );
			    AverageEdge(a_Size, PixelsOfAllFaces[sg_CubeEdgeList[1,0]], PixelsOfAllFaces[sg_CubeEdgeList[1,1]], i, a_Size, a_Size, i, fixupDist, new Vector4(0, -1, -1, 0) );
			    AverageEdge(a_Size, PixelsOfAllFaces[sg_CubeEdgeList[0,0]], PixelsOfAllFaces[sg_CubeEdgeList[0,1]], i, 0, a_Size, a_Size - i, fixupDist, new Vector4(0, 1, -1, 0) );
		    } */

		

		    // Write Pixel from the jagged array back to the cubemap faces
		    for (int writeFace = 0; writeFace < 6; writeFace++ ) {
			    Color[] tempColors = PixelsOfAllFaces[writeFace];
			    CubeMap.SetPixels(tempColors, (CubemapFace)writeFace, mipLevel);
		    }
	    }
	    CubeMap.Apply(false); 
    }


    private void AverageEdge(int a_Size, Color[] face_A, Color[] face_B, int x_A, int y_A, int x_B, int y_B, int fixupDist, Vector4 dir)
    {
	    // As a_Size is used as factor we have to increase it by 1
	    a_Size += 1;
	    Color AverageEdgeColor = face_A[a_Size * y_A + x_A] * 0.5f + face_B[+ a_Size * y_B + x_B] * 0.5f;	
	    face_A[a_Size * y_A + x_A] = AverageEdgeColor;
	    face_B[a_Size * y_B + x_B] = AverageEdgeColor;
    /*	for (int iFixup = 0; iFixup < fixupDist; iFixup++)
	    {
		    float fixupFrac = (float)(fixupDist - iFixup) / (float)fixupDist;
		    Color AverageEdgeColor_A = face_A[a_Size * y_A + a_Size * (int)dir.y * iFixup + x_A + (int)dir.x * iFixup];
		    Color AverageEdgeColor_B = face_B[a_Size * y_B + a_Size * (int)dir.w * iFixup + x_B + (int)dir.z * iFixup];
		    // CP_FIXUP_PULL_HERMITE
		    float fixupWeight = ((-2.0f * fixupFrac + 3.0f) * fixupFrac * fixupFrac);
		
	    //	face_A[a_Size* y_A + a_Size * (int)dir.y * iFixup + x_A + (int)dir.x * iFixup ] = Color.red * (fixupWeight);
	    //	face_B[a_Size * y_B + a_Size * (int)dir.w * iFixup + x_B + (int)dir.z * iFixup ] = Color.blue * (fixupWeight);

		    face_A[a_Size* y_A + a_Size * (int)dir.y * iFixup + x_A + (int)dir.x * iFixup ] = AverageEdgeColor * fixupWeight + AverageEdgeColor_A * (1.0f-fixupWeight) ;
		    face_B[a_Size * y_B + a_Size * (int)dir.w * iFixup + x_B + (int)dir.z * iFixup ] = AverageEdgeColor * fixupWeight + AverageEdgeColor_B * (1.0f-fixupWeight) ;
	

	    } */
    }

    private void AverageCorner (int a_Size, Color[] face_A , Color[] face_B, Color [] face_C, int x_A, int y_A, int x_B, int y_B, int x_C, int y_C)
    {
	    // As a_Size is used as factor we have to increase it by 1
	    a_Size += 1;
	    Color AverageCornerColor = face_A[a_Size * y_A + x_A]/3.0f + face_B[a_Size * y_B + x_B]/3.0f + face_C[a_Size * y_C + x_C]/3.0f;
	    face_A[ a_Size * y_A + x_A] = AverageCornerColor;
	    face_B[a_Size * y_B + x_B] = AverageCornerColor;
	    face_C[a_Size * y_C + x_C] = AverageCornerColor;
    }


    //--------------------------------------------------------------------------------------
    // Builds a normalizer cubemap, with the texels solid angle stored in the fourth component

    void BuildNormalizerSolidAngleCubemap(int a_Size, Cubemap a_Surface)
    {
	    //a_Size = a_Surface.width;
	    int iCubeFace, u, v;
	    Vector3 a_XYZ = new Vector3(0,0,0);
	    Color[] i_CubeMapColors = new Color[a_Size*a_Size];
	    //
	    float weight = 0.0f;
	    //iterate over cube faces
	    for(iCubeFace=0; iCubeFace<6; iCubeFace++)
	    {
		    for(v=0; v< a_Size; v++)
		    {
			    for(u=0; u < a_Size; u++)
			    {
            	    // calc TexelToVect in a_XYZ
				    a_XYZ = TexelToVect(iCubeFace, (float)u, (float)v, a_Size);
				    // calc weight
        		    weight = TexelCoordSolidAngle(iCubeFace, (float)u, (float)v, a_Size);
				    // Compress a_XYZ to fit into Color
				    i_CubeMapColors[v * a_Size + u] = new Color ((a_XYZ[0]+1.0f)/2.0f, (a_XYZ[1]+1.0f)/2.0f, (a_XYZ[2]+1.0f)/2.0f, weight);
			    }         
		    }
		    // set CubemapFace
		    a_Surface.SetPixels(i_CubeMapColors, (CubemapFace)iCubeFace);
	    }
	    // set cubeMap
	    a_Surface.Apply(true);
	    // Debug.Log ("BuildNormalizerSolidAngleCubemap finished");
    }

    //--------------------------------------------------------------------------------------
    // Builds a normalizer array, with the texels solid angle stored in the fourth component

    void BuildNormalizerSolidAngleArray (int a_Size, ref Vector4[] normSolid)
    {
	    int iCubeFace, u, v;
	    Vector3 a_XYZ = new Vector3(0,0,0);
	    //
	    float weight = 0.0f;
	    //iterate over cube faces
	    for(iCubeFace=0; iCubeFace<6; iCubeFace++)
	    {
		    for(v=0; v< a_Size; v++)
		    {
			    for(u=0; u < a_Size; u++)
			    {
            	    // calc TexelToVect in a_XYZ
				    a_XYZ = TexelToVect(iCubeFace, (float)u, (float)v, a_Size);
				    // calc weight
        		    weight = TexelCoordSolidAngle(iCubeFace, (float)u, (float)v, a_Size);
				    //
				    normSolid[iCubeFace * a_Size* a_Size + v * a_Size + u] = new Vector4(a_XYZ[0], a_XYZ[1], a_XYZ[2], weight);
			    }         
		    }
	    }
	    // Debug.Log ("BuildNormalizerSolidAngleArray finished");	
    }

    ///////////////////////////////////////////////
    // SH order use for approximation of irradiance cubemap is 5, mean 5*5 equals 25 coefficients
    // #define MAX_SH_ORDER 5
    // #define NUM_SH_COEFFICIENT (MAX_SH_ORDER * MAX_SH_ORDER)

    void EvalSHBasis(Vector3 dir, ref double[] res )
    {
	    double SqrtPi = (double)Mathf.Sqrt(CP_PI);

	    double xx = dir[0];
	    double yy = dir[1];
	    double zz = dir[2];

	    // x[i] == pow(x, i), etc.
	    // float64 x[MAX_SH_ORDER+1], y[MAX_SH_ORDER+1], z[MAX_SH_ORDER+1];
	    double[] x = new double[26];
	    double[] y = new double[26];
	    double[] z = new double[26];
	    x[0] = 1.0;
	    y[0] = 1.0;
	    z[0] = 1.0;
	    for (int i = 1; i < 25+1; ++i)
	    {
		    x[i] = xx * x[i-1];
		    y[i] = yy * y[i-1];
		    z[i] = zz * z[i-1];
	    }

	    res[0]  = (1/(2.0*SqrtPi));

	    res[1]  = -(Mathf.Sqrt(3/CP_PI)*yy)/2.0;
	    res[2]  = (Mathf.Sqrt(3/CP_PI)*zz)/2.0;
	    res[3]  = -(Mathf.Sqrt(3/CP_PI)*xx)/2.0;

	    res[4]  = (Mathf.Sqrt(15/CP_PI)*xx*yy)/2.0;
	    res[5]  = -(Mathf.Sqrt(15/CP_PI)*yy*zz)/2.0;
	    res[6]  = (Mathf.Sqrt(5/CP_PI)*(-1 + 3*z[2]))/4.0;
	    res[7]  = -(Mathf.Sqrt(15/CP_PI)*xx*zz)/2.0;
	    res[8]  = Mathf.Sqrt(15/CP_PI)*(x[2] - y[2])/4.0;

	    res[9]  = (Mathf.Sqrt(35/(2.0f*CP_PI))*(-3*x[2]*yy + y[3]))/4.0;
	    res[10] = (Mathf.Sqrt(105/CP_PI)*xx*yy*zz)/2.0;
	    res[11] = -(Mathf.Sqrt(21/(2.0f*CP_PI))*yy*(-1 + 5*z[2]))/4.0;
	    res[12] = (Mathf.Sqrt(7/CP_PI)*zz*(-3 + 5*z[2]))/4.0;
	    res[13] = -(Mathf.Sqrt(21/(2.0f*CP_PI))*xx*(-1 + 5*z[2]))/4.0;
	    res[14] = (Mathf.Sqrt(105/CP_PI)*(x[2] - y[2])*zz)/4.0;
	    res[15] = -(Mathf.Sqrt(35/(2.0f*CP_PI))*(x[3] - 3*xx*y[2]))/4.0;

	    res[16] = (3*Mathf.Sqrt(35/CP_PI)*xx*yy*(x[2] - y[2]))/4.0;
	    res[17] = (-3*Mathf.Sqrt(35/(2.0f*CP_PI))*(3*x[2]*yy - y[3])*zz)/4.0;
	    res[18] = (3*Mathf.Sqrt(5/CP_PI)*xx*yy*(-1 + 7*z[2]))/4.0f;
	    res[19] = (-3*Mathf.Sqrt(5/(2.0f*CP_PI))*yy*zz*(-3 + 7*z[2]))/4.0;
	    res[20] = (3*(3 - 30*z[2] + 35*z[4]))/(16.0*SqrtPi);
	    res[21] = (-3*Mathf.Sqrt(5/(2.0f*CP_PI))*xx*zz*(-3+ 7*z[2]))/4.0;
	    res[22] = (3*Mathf.Sqrt(5/CP_PI)*(x[2] - y[2])*(-1 + 7*z[2]))/8.0;
	    res[23] = (-3*Mathf.Sqrt(35/(2.0f*CP_PI))*(x[3] - 3*xx*y[2])*zz)/4.0;
	    res[24] = (3*Mathf.Sqrt(35/CP_PI)*(x[4] - 6*x[2]*y[2] + y[4]))/16.0;
    }

    // See Peter-Pike Sloan paper for these coefficients
    static double[] SHBandFactor = { 
	    1.0,
	    2.0 / 3.0, 2.0 / 3.0, 2.0 / 3.0,
	    1.0 / 4.0, 1.0 / 4.0, 1.0 / 4.0, 1.0 / 4.0, 1.0 / 4.0,
	    0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, // The 4 band will be zeroed
	    - 1.0 / 24.0, - 1.0 / 24.0, - 1.0 / 24.0, - 1.0 / 24.0, - 1.0 / 24.0, - 1.0 / 24.0, - 1.0 / 24.0, - 1.0 / 24.0, - 1.0 / 24.0
    };

    void FakeHDR (Cubemap CubeMap, bool hasmipLevels) {
	    int maxMipLevels = 1;
	    if (hasmipLevels) {
		    maxMipLevels = (int)(Mathf.Log((float)CubeMap.width, 2.0f)) + 1;
	    }
	    int base_Size = CubeMap.width;
	    Vector4 rgbmColor = new Vector4();

	    for (int mipLevel = 0; mipLevel < maxMipLevels; mipLevel++)
	    {
		    int a_Size = base_Size >> mipLevel;
		    // As we use a_Size as pointer in our arrays we have to lower it by 1
		    //a_Size -= 1;


		    for (int a_FaceIdx = 0; a_FaceIdx < 6; a_FaceIdx++)
		    {
			    // Get all pixels of the given face
			    Color[] FaceColors = CubeMap.GetPixels((CubemapFace)a_FaceIdx, mipLevel);
			    // Iterate over dst cube map face texel
			    for (int a_V  = 0; a_V < a_Size; a_V++)
			    {
				    for (int a_U = 0; a_U < a_Size; a_U++)
				    {
					    rgbmColor.x = FaceColors[a_V * a_Size + a_U].r;
					    rgbmColor.y = FaceColors[a_V * a_Size + a_U].g;
					    rgbmColor.z = FaceColors[a_V * a_Size + a_U].b;
					    rgbmColor *= 1.0f/6.0f;
					    rgbmColor.w = Mathf.Clamp01(Mathf.Max(Mathf.Max(rgbmColor.x, rgbmColor.y), rgbmColor.z));
					    rgbmColor.w = Mathf.Ceil(rgbmColor.w*255.0f) / 255.0f;
					    rgbmColor.x = rgbmColor.x / rgbmColor.w;
					    rgbmColor.y = rgbmColor.y / rgbmColor.w;
					    rgbmColor.z = rgbmColor.z / rgbmColor.w;
					    FaceColors[a_V * a_Size + a_U] = rgbmColor;
				    }
			    }
			    CubeMap.SetPixels(FaceColors,(CubemapFace)a_FaceIdx, mipLevel);
		    }
	    }
	    CubeMap.Apply(false);
    }


}
#endif