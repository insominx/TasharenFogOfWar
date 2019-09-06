// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

//----------------------------------------------
//           Tasharen Fog of War
// Copyright © 2012-2015 Tasharen Entertainment
//----------------------------------------------

Shader "Fog of War/Diffuse"
{
	Properties
	{
		_Color ("Main Color", Color) = (1,1,1,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Lambert vertex:vert

		sampler2D _MainTex;
		fixed4 _Color;

		// FOW #1: Requires parameters (set by FOWSystem)
		sampler2D _FOWTex0, _FOWTex1;
		float4 _FOWParams;
		half4 _FOWUnexplored, _FOWExplored;

		struct Input
		{
			float2 uv_MainTex : TEXCOORD0;

			// FOW #2: Fog texture coordinates
			float2 fog : TEXCOORD1;
		};

		void vert (inout appdata_full v, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);

			// FOW #3: Set the fog texture coordinates
			float4 worldPos = mul (unity_ObjectToWorld, v.vertex);
			o.fog.xy = worldPos.xz * _FOWParams.z + _FOWParams.xy;
		}

		void surf (Input IN, inout SurfaceOutput o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;

			// FOW #4: Tint the final color by the Fog of War
			half4 fog0 = tex2D(_FOWTex0, IN.fog);
			half4 fog1 = tex2D(_FOWTex1, IN.fog);
			half2 fog = lerp(fog0.xy, fog1.xy, _FOWParams.w);
			o.Albedo = lerp(lerp(c.rgb * _FOWUnexplored, c.rgb * _FOWExplored, fog.y), c.rgb, fog.x);
			o.Alpha = c.a;

			// DEBUG
			//o.Albedo.r = fog0.x; // Current fog
			//o.Albedo.g = fog1.x; // Target fog
			//o.Albedo.b = 0.0;
		}
		ENDCG
	}
	Fallback "Diffuse"
}
