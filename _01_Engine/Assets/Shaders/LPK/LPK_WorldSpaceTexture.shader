/***************************************************
File:           LPK_WorldSpaceTexture.shader
Authors:        Christopher Onorati
Last Updated:   4/8/2019
Last Version:   2018.3.4

Description:
Shader to make a textured draw based on global coordinates
rather than per-mesh.  This shader assumes a uniformly
scaled sprite.

This script is a basic and generic implementation of its
functionality. It is designed for educational purposes and
aimed at helping beginners.

Copyright 2018-2019, DigiPen Institute of Technology
***************************************************/

Shader "LPK/LPK_WorldSpaceTexture"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
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

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                //NOTENOTE:  Assume the scale is ==ALWAYS== uniform.
                float scale = length(float3(unity_ObjectToWorld[0].x, unity_ObjectToWorld[1].x, unity_ObjectToWorld[2].x));

                float2 worldPos = mul(unity_ObjectToWorld, v.vertex).xy * 1 / scale;
                o.uv += worldPos;

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
