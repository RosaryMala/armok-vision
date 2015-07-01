Shader "Transparent/InvisibleShadowCasterTextured" 
 { 
 
     Properties 
     { 
          // Ususal stuffs
         _Color ("Main Color", Color) = (1,1,1,1)
         _MainTex ("Base (RGB) TransGloss (A)", 2D) = "white" {}
         
         // Shadow Stuff
         _Cutoff ("Shadow Alpha cutoff", Range(0.25,0.9)) = 1.0
     } 
 
 
     SubShader 
     { 
         Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
     
         LOD 200
     
     
     
 // Pass to render object as a shadow caster
 Pass {
 Name "Caster"
 Tags { "LightMode" = "ShadowCaster" }
 Offset 1, 1
         
 Fog {Mode Off}
 ZWrite On ZTest LEqual Cull Off
 
 CGPROGRAM
 #pragma vertex vert
 #pragma fragment frag
 #pragma multi_compile_shadowcaster
 #include "UnityCG.cginc"
 
 struct v2f { 
     V2F_SHADOW_CASTER;
     float2  uv : TEXCOORD1;
 };
 
 uniform float4 _MainTex_ST;
 
 v2f vert( appdata_base v )
 {
     v2f o;
     TRANSFER_SHADOW_CASTER(o)
     o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
     return o;
 }
 
 uniform sampler2D _MainTex;
 uniform fixed _Cutoff;
 uniform fixed4 _Color;
 
 float4 frag( v2f i ) : COLOR
 {
     fixed4 texcol = tex2D( _MainTex, i.uv );
     clip( texcol.a*_Color.a - _Cutoff );
     
     SHADOW_CASTER_FRAGMENT(i)
 }
 ENDCG
 }
     
 // Pass to render object as a shadow collector
 Pass {
 Name "ShadowCollector"
 Tags { "LightMode" = "ShadowCollector" }
         
 Fog {Mode Off}
 ZWrite On ZTest LEqual
 
 CGPROGRAM
 #pragma vertex vert
 #pragma fragment frag
 #pragma multi_compile_shadowcollector
 
 #define SHADOW_COLLECTOR_PASS
 #include "UnityCG.cginc"
 
 struct v2f {
     V2F_SHADOW_COLLECTOR;
     float2  uv : TEXCOORD5;
 };
 
 uniform float4 _MainTex_ST;
 
 v2f vert (appdata_base v)
 {
     v2f o;
     TRANSFER_SHADOW_COLLECTOR(o)
     o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
     return o;
 }
 
 uniform sampler2D _MainTex;
 uniform fixed _Cutoff;
 uniform fixed4 _Color;
 
 fixed4 frag (v2f i) : COLOR
 {
     fixed4 texcol = tex2D( _MainTex, i.uv );
     clip( texcol.a*_Color.a - _Cutoff );
     
     SHADOW_COLLECTOR_FRAGMENT(i)
 }
 ENDCG
 }
     
     
         CGPROGRAM
         #pragma surface surf Lambert alphatest:_Cutoff
 
         sampler2D _MainTex;
         fixed4 _Color;
 
         struct Input {
             float2 uv_MainTex;
         };
 
         void surf (Input IN, inout SurfaceOutput o) {
             discard;
         }
         ENDCG
     }
     
     Fallback "Transparent/VertexLit"
 }