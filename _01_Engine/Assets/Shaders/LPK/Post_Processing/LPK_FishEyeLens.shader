/***************************************************
File:           LPK_FishEyeLens.shader
Authors:        Christopher Onorati
Last Updated:   4/5/2019
Last Version:   2018.3.4

Description:
Shader to distort the screen in such a way to create a
fish eye lens effect.

This script is a basic and generic implementation of its
functionality. It is designed for educational purposes and
aimed at helping beginners.

Copyright 2018-2019, DigiPen Institute of Technology
***************************************************/

Shader "LPK/PostProcessing/LPK_FishEyeLens"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
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
                o.uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.uv.xy);
                return o;
            }

            sampler2D _MainTex;

            fixed4 frag(v2f i) : SV_Target
            {
                float PI = 3.14159;

                float aperture = 178.0f;
                float apertureHalf = 0.5 * aperture * (PI / 180.0);
                float maxFactor = sin(apertureHalf);

                float2 uv;
                float2 xy = 2.0 * i.uv.xy - 1.0;
                float d = length(xy);

                if (d < (2.0 - maxFactor))
                {
                    d = length(xy * maxFactor);
                    float z = sqrt(1.0 - d * d);
                    float r = atan2(d, z) / PI;
                    float phi = atan2(xy.y, xy.x);

                    uv.x = r * cos (phi) + 0.5;
                    uv.y = r * sin(phi) + 0.5;
                }
                else
                {
                    uv = i.uv;
                }

                return tex2D(_MainTex, uv);
            }

            ENDCG
        }
    }
}
