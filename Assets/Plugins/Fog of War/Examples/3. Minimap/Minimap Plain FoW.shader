// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Minimap/Plain Fog of War"
{
    Properties
    {
        _Color0 ("Explored", Color) = (0.5, 0.5, 0.5, 1)
		_Color1 ("Unexplored", Color) = (1, 1, 1, 0)
    }

	SubShader
	{
		LOD 100

		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
		}

		Pass
		{
			Cull Off
			Lighting Off
			ZWrite Off
			Offset -1, -1
			Fog { Mode Off }
			ColorMask RGB
			Blend SrcAlpha OneMinusSrcAlpha
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			uniform sampler2D _MainTex;
			uniform sampler2D _FogTex0;
			uniform sampler2D _FogTex1;
			uniform float4 _MainTex_ST;
			uniform float4 _FogTex0_ST;
			uniform half4 _Color0;
			uniform half4 _Color1;
			uniform half _Blend;

			struct appdata_t
			{
				float4 vertex : POSITION;
				half4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
			};

			struct v2f
			{
				float4 vertex : POSITION;
				half4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
			};

			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				o.texcoord1 = TRANSFORM_TEX(v.texcoord1, _FogTex0);
				return o;
			}

			half4 frag (v2f IN) : COLOR
			{
				half4 fog = lerp(tex2D(_FogTex0, IN.texcoord1), tex2D(_FogTex1, IN.texcoord1), _Blend);
				half4 tex = tex2D(_MainTex, IN.texcoord);

				half lum = (tex.r + tex.g + tex.b) * 0.3334;
				half4 grey = half4(lum, lum, lum, 1.0);

				tex.a = 1.0;
				half4 final = lerp(lerp(grey * _Color1, grey * _Color0, fog.g), tex, fog.r) * IN.color;
				return final;
			}
			ENDCG
		}
	}
}
