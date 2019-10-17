/***************************************************
File:           LPK_Vingnette.shader
Authors:        Christopher Onorati
Last Updated:   6/10/2019
Last Version:   2018.3.4

Description:
Shader to create a basic vingnette effect.

This script is a basic and generic implementation of its
functionality. It is designed for educational purposes and
aimed at helping beginners.

Copyright 2018-2019, DigiPen Institute of Technology
***************************************************/

Shader "LPK/PostProcessing/LPK_Vingnette"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
    _VignetteColor ("Color", Color) = (1.0, 1.0, 1.0, 1.0)
    _VignetteGradientMin ("Vignette Gradient Min", float) = 0.0
    _VignetteGradientMax ("Vignette Gradient Max", float) = 1.0
    _VignetteRoundness ("Vignette Roundness", float) = 0.5
	}

	SubShader
	{
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
			
			sampler2D _MainTex;

            float4 _VignetteColor;
            
            float _VignetteGradientMin;
            float _VignetteGradientMax;

            float _VignetteRoundness;
            
			fixed4 frag (v2f i) : SV_Target
			{
                float4 col = tex2D(_MainTex, i.uv);

                //Distance of pixel to center of the screen.
                float2 screenDistance = abs(i.uv - 0.5f);
                
                //Calculate roundness of the effect.
                float aspectRatio = _ScreenParams.x / _ScreenParams.y;
                screenDistance.x *= 1 + (aspectRatio - 1) * (_VignetteRoundness);

                //Get shape of the vignette.
                float ellipseDistance = length(screenDistance);
                float ellipseMask = smoothstep(_VignetteGradientMin, _VignetteGradientMax, ellipseDistance);

                //Apply effect.
                return float4(lerp(col, _VignetteColor, ellipseMask));
			}
			ENDCG
		}
	}
}
