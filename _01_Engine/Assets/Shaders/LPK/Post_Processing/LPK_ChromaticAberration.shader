/***************************************************
File:           LPK_ChromaticAberration.shader
Authors:        Christopher Onorati
Last Updated:   6/10/2019
Last Version:   2018.3.4

Description:
Shader to add a chromatic aberration effect to the screen view.

This script is a basic and generic implementation of its
functionality. It is designed for educational purposes and
aimed at helping beginners.

Copyright 2018-2019, DigiPen Institute of Technology
***************************************************/

Shader "LPK/PostProcessing/LPK_ChromaticAberration"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _ChromaticAbberationRedMagnitudeX ("Red Magnitude X", float) = 0
        _ChromaticAberrationRedMagnitudeY ("Red Magnitude Y", float) = 0

        _ChromaticAbberationGreenMagnitudeX ("Green Magnitude X", float) = 0
        _ChromaticAberrationGreenMagnitudeY ("Green Magnitude Y", float) = 0

        _ChromaticAbberationBlueMagnitudeX ("Blue Magnitude X", float) = 0
        _ChromaticAberrationBlueMagnitudeY ("Blue Magnitude Y", float) = 0
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

            float _ChromaticAbberationRedMagnitudeX;
            float _ChromaticAberrationRedMagnitudeY;
             
            float _ChromaticAbberationGreenMagnitudeX;
            float _ChromaticAberrationGreenMagnitudeY;
             
            float _ChromaticAbberationBlueMagnitudeX;
            float _ChromaticAberrationBlueMagnitudeY;

            fixed4 frag(v2f i) : SV_Target
            {
                float colR = tex2D(_MainTex, float2(i.uv.x + _ChromaticAbberationRedMagnitudeX, i.uv.y + _ChromaticAberrationRedMagnitudeY)).r;
                float colG = tex2D(_MainTex, float2(i.uv.x + _ChromaticAbberationGreenMagnitudeX, i.uv.y + _ChromaticAberrationGreenMagnitudeY)).g;
                float colB = tex2D(_MainTex, float2(i.uv.x + _ChromaticAbberationBlueMagnitudeX, i.uv.y + _ChromaticAberrationBlueMagnitudeY)).b;

                return fixed4(colR, colG, colB, 1);
            }
            ENDCG
        }
    }
}
