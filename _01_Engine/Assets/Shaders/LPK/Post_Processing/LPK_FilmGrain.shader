/***************************************************
File:           LPK_FilmGrain.shader
Authors:        Christopher Onorati
Last Updated:   6/10/2019
Last Version:   2018.3.4

Description:
Shader to create film grain effect.

Copyright 2018-2019, DigiPen Institute of Technology
***************************************************/

Shader "LPK/PostProcessing/LPK_FilmGrain"
{
	Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _FilmGrainIntensity("Film Grain Intensity", float) = 0.25
    }
	SubShader
	{
		// No culling or depth
		Cull Off
        ZWrite Off
        ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
      uniform sampler2D _MainTex;
      uniform float _FilmGrainIntensity;

      float random(float2 uv)
      {
          return clamp(frac(sin(dot(uv, float2(12.9898, 78.233)))*43758.5453123), 1 - _FilmGrainIntensity, 1);
      }

			fixed4 frag (v2f i) : SV_Target
			{
                //No modification to the color of the pixel on the screen.
                float4 col = tex2D(_MainTex, i.uv.xy);

                float rnd = random(i.uv * _Time.x);
                float4 grain = float4( rnd, rnd, rnd, 1.0);

                return col *= grain;
			}
			ENDCG
		}
	}
}
