// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

//----------------------------------------------
//           Tasharen Fog of War
// Copyright © 2012-2015 Tasharen Entertainment
//----------------------------------------------

Shader "Fog of War/Terrain"
{
	Properties
	{
		[HideInInspector] _Control ("Control (RGBA)", 2D) = "red" {}
		[HideInInspector] _Splat3 ("Layer 3 (A)", 2D) = "white" {}
		[HideInInspector] _Splat2 ("Layer 2 (B)", 2D) = "white" {}
		[HideInInspector] _Splat1 ("Layer 1 (G)", 2D) = "white" {}
		[HideInInspector] _Splat0 ("Layer 0 (R)", 2D) = "white" {}
		[HideInInspector] _MainTex ("BaseMap (RGB)", 2D) = "white" {}
		[HideInInspector] _Color ("Main Color", Color) = (1,1,1,1)
	}

	SubShader
	{
		Tags
		{
			"SplatCount" = "4"
			"Queue" = "Geometry-100"
			"RenderType" = "Opaque"
		}

		CGPROGRAM
    #pragma target 4.0 // todo - refactor to use less interpolators so this is not needed
		#pragma surface surf Lambert vertex:vert

		sampler2D _Control;
		sampler2D _Splat0, _Splat1, _Splat2, _Splat3;

		// FOW #1: Requires parameters (set by FOWSystem)
		sampler2D _FOWTex0, _FOWTex1;
        float4 _FOWParams;
        half4 _FOWUnexplored, _FOWExplored;

		struct Input
		{
			float2 uv_Control : TEXCOORD0;
			float2 uv_Splat0 : TEXCOORD1;
			float2 uv_Splat1 : TEXCOORD2;
			float2 uv_Splat2 : TEXCOORD3;
			float2 uv_Splat3 : TEXCOORD4;

			// FOW #2: Fog texture coordinates
			float2 fog : TEXCOORD5;
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
			half4 splat = tex2D (_Control, IN.uv_Control);
			half4 col =
				splat.r * tex2D (_Splat0, IN.uv_Splat0) +
				splat.g * tex2D (_Splat1, IN.uv_Splat1) +
				splat.b * tex2D (_Splat2, IN.uv_Splat2) +
				splat.a * tex2D (_Splat3, IN.uv_Splat3);

			// FOW #4: Tint the final color by the Fog of War
			half4 fog = lerp(tex2D(_FOWTex0, IN.fog), tex2D(_FOWTex1, IN.fog), _FOWParams.w);
            col = lerp(lerp(col * _FOWUnexplored, col * _FOWExplored, fog.g), col, fog.r);

			o.Albedo = col.rgb;
			o.Alpha = 1.0;
		}
		ENDCG
	}

	Dependency "AddPassShader" = "Hidden/TerrainEngine/Splatmap/Lightmap-AddPass"
	Dependency "BaseMapShader" = "Diffuse"

	Fallback "Nature/Terrain/Diffuse"
}
