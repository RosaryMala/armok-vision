#ifndef LuxVertFunc_CG_INCLUDED
#define LuxVertFunc_CG_INCLUDED

// Lux Vertex Function

	void LuxVert (inout appdata_full v, out Input o) {
		UNITY_INITIALIZE_OUTPUT(Input,o);
		o.mainLightDir = mul( (float3x3) _World2Object, -Lux_MainLightDir);
		// create tangent space rotation
		float3 binormal = cross( v.normal, v.tangent.xyz ) * v.tangent.w;
		float3x3 rotation = float3x3( v.tangent.xyz, binormal, v.normal );
		o.mainLightDir = normalize(mul( rotation, o.mainLightDir ));
	}		

#endif
