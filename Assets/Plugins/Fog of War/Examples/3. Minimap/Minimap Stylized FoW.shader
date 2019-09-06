// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Minimap/Stylized Fog of War"
{
	Properties
	{
		_Gradient ("Gradient", 2D) = "white" {}
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

			sampler2D _MainTex;
			sampler2D _FogTex0;
			sampler2D _FogTex1;
			sampler2D _Gradient;

			float4 _MainTex_ST;
			float4 _FogTex0_ST;
			half _Blend;

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
				half offset = tex2D(_MainTex, IN.texcoord).r;
	
				half4 visible = tex2D(_Gradient, half2(0.25, offset));
				half4 explored = tex2D(_Gradient, half2(0.5, offset));
				half4 hidden = tex2D(_Gradient, half2(0.75, offset));
	
				half4 final = lerp(lerp(hidden, explored, fog.g), visible, fog.r) * IN.color;

				float2 temp = abs(IN.texcoord * 2.0 - 1.0);
				float val = max(temp.x, temp.y);
				val *= val;
				val *= val;
				val *= val;
				val *= val;
				final.a *= 1.0 - val;
				return final;
			}
			ENDCG
		}
	}
}
