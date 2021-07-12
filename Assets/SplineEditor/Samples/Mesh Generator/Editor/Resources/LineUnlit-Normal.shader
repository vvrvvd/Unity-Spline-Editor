// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Unlit shader. Simplest possible textured shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "Line Renderer Pro/Unlit/Texture" {
Properties {
	_MainTex ("Base (RGB)", 2D) = "white" {}
}

SubShader {
	Tags { "RenderType"="Opaque" }
	LOD 100
	
	Pass {  
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				float2 texcoord2 : TEXCOORD2;
				half4 color : COLOR;
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				float4 texcoord : TEXCOORD0;
				half4 color : COLOR;
				UNITY_FOG_COORDS(1)
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord.xy = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.texcoord.zw = v.texcoord2;
				o.color = v.color;
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			#define TRANSFORM_TEX_LRP(tex) ((tex.xy - _MainTex_ST.zw) / float2(1, tex.w) + _MainTex_ST.zw)
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, TRANSFORM_TEX_LRP(i.texcoord)) * i.color;
				UNITY_APPLY_FOG(i.fogCoord, col);
				UNITY_OPAQUE_ALPHA(col.a);
				return col;
			}
		ENDCG
	}
}

}
