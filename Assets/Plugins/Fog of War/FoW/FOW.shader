// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

//----------------------------------------------
//           Tasharen Fog of War
// Copyright � 2012-2015 Tasharen Entertainment
//----------------------------------------------

Shader "Image Effects/Fog of War"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_FogTex0 ("Fog 0", 2D) = "white" {}
		_FogTex1 ("Fog 1", 2D) = "white" {}
		_Unexplored ("Unexplored Color", Color) = (0.05, 0.05, 0.05, 0.05)
		_Explored ("Explored Color", Color) = (0.35, 0.35, 0.35, 0.35)
	}
	SubShader
	{
		Pass
		{
			ZTest Always
			Cull Off
			ZWrite Off
			Fog { Mode off }

			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag vertex:vert
			#pragma fragmentoption ARB_precision_hint_fastest
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _FogTex0;
			sampler2D _FogTex1;
			sampler2D _CameraDepthTexture;

			uniform float4x4 _InverseMVP;
			uniform float4 _Params;
			uniform float4 _CamPos;
			uniform half4 _Unexplored;
			uniform half4 _Explored;

			struct Input
			{
				float4 position : POSITION;
				float2 uv : TEXCOORD0;
			};

			void vert (inout appdata_full v, out Input o)
			{
				o.position = UnityObjectToClipPos(v.vertex);
				o.uv = v.texcoord.xy;
			}

			float3 CamToWorld (in float2 uv, in float depth)
			{
				float4 pos = float4(uv.x, uv.y, depth, 1.0);
				pos.xyz = pos.xyz * 2.0 - 1.0;
				pos = mul(_InverseMVP, pos);
				return pos.xyz / pos.w;
			}

			fixed4 frag (Input i) : COLOR
			{
				half4 original = tex2D(_MainTex, i.uv);

      // This assumption no longer appears to function as original intended, no longer needed?

			// #if UNITY_UV_STARTS_AT_TOP
			// 	float2 depthUV = i.uv;
			// 	depthUV.y = lerp(depthUV.y, 1.0 - depthUV.y, _CamPos.w);
			// 	float depth = 1.0 - UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, depthUV));
			// 	float3 pos = CamToWorld(depthUV, depth);
			// #else
				float depth = 1.0 - UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, i.uv));
				float3 pos = CamToWorld(i.uv, depth);
			// #endif

				// Limit the fog of war to sea level
				if (pos.y < 0.0)
				{
					// This is a simplified version of the ray-plane intersection formula: t = -( N.O + d ) / ( N.D )
					float3 dir = normalize(pos - _CamPos.xyz);
					pos = _CamPos.xyz - dir * (_CamPos.y / dir.y);
				}

				float2 uv = pos.xz * _Params.z + _Params.xy;
				half4 fog = lerp(tex2D(_FogTex0, uv), tex2D(_FogTex1, uv), _Params.w);

				return lerp(lerp(original * _Unexplored, original * _Explored, fog.g), original, fog.r);
			}
			ENDCG
		}
	}
	Fallback off
}
