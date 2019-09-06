// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Minimap/Heightmap"
{
	SubShader
	{
		LOD 300
		Tags { "RenderType" = "Opaque" }
		Fog { Mode Off }

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			float terrainOffset = 0.0;
			float terrainScale = 16.0;

			struct appdata_t
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert (appdata_t v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
				float f = (worldPos.y + terrainOffset) / terrainScale;
				o.uv = float2(f, f);
				return o;
			}

			fixed4 frag (v2f IN) : COLOR
			{
				return half4(IN.uv.xxx, 1.0);
			}
			ENDCG
		}
	}
	FallBack Off
}
