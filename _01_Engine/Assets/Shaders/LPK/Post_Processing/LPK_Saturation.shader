/***************************************************
File:           LPK_Saturation.shader
Authors:        Christopher Onorati
Last Updated:   6/10/2019
Last Version:   2018.3.4

Description:
Shader to modify color saturation of the screen.

This script is a basic and generic implementation of its
functionality. It is designed for educational purposes and
aimed at helping beginners.

Copyright 2018-2019, DigiPen Institute of Technology
***************************************************/

Shader "LPK/PostProcessing/LPK_Saturation"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _SaturationRedSaturation("Red Saturation", float) = 1.0
        _SaturationGreenSaturation("Green Saturation", float) = 1.0
        _SaturationBlueSaturation("Blue Saturation", float) = 1.0
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

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;

            float _SaturationRedSaturation;
            float _SaturationGreenSaturation;
            float _SaturationBlueSaturation;

            fixed4 frag(v2f i) : SV_Target
            {
                float colR = saturate(tex2D(_MainTex, i.uv).r * _SaturationRedSaturation);
                float colG = saturate(tex2D(_MainTex, i.uv).g * _SaturationGreenSaturation);
                float colB = saturate(tex2D(_MainTex, i.uv).b * _SaturationBlueSaturation);

                return fixed4(colR, colG, colB, 1);
            }
            ENDCG
        }
    }
}
