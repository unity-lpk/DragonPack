/***************************************************
File:           LPK_ScrollingTexture.shader
Authors:        Christopher Onorati
Last Updated:   4/3/2019
Last Version:   2018.3.4

Description:
    Shader to make a texture scroll to simulate movement
    or animation.

This script is a basic and generic implementation of its
functionality. It is designed for educational purposes and
aimed at helping beginners.

Copyright 2018-2019, DigiPen Institute of Technology
***************************************************/

Shader "LPK/LPK_ScrollingTexture"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _MoveSpeedX("Move Speed X", float) = 1
        _MoveSpeedY("Move Speed Y", float) = 1
    }

    SubShader
    {
        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Tags
        {
            "Queue" = "Transparent"
            "PreviewType" = "Plane"
            "DisableBatching" = "True"
        }

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
                half4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                half4 color : COLOR;
            };

            //Texture of the sprite.
            sampler2D _MainTex;
            float4 _MainTex_ST;

            //Movement of the texture scroll.
            float _MoveSpeedX;
            float _MoveSpeedY;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                float2 scroll = float2(_MoveSpeedX, _MoveSpeedY) * _Time.x;
                o.uv += scroll;

                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                return tex2D(_MainTex, i.uv) * i.color;
            }

            ENDCG
        }
    }
}
