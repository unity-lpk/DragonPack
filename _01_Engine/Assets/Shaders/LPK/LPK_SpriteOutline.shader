/***************************************************
File:           LPK_SpriteOutline.shader
Authors:        Christopher Onorati, Evan Kau
Last Updated:   3/29/2019
Last Version:   2018.3.4

Description:
Shader to outline a sprite in a given color. 

The outline can go in or out of the sprite as desired.
Both can have different colors. Added functionality to
increase the number of samples taken. Added functionality
to square off the corners. Added functionality to change
the angle of the samples.
Recommended value sample count for square corners is 8.
To do: The image still clips off for the outline if you
go very far. The approach to fix this is to render a
larger version of the mesh, either in two passes, or with
UV coordinates appropriately set to accommodate the increased
size.

Copyright 2018-2019, DigiPen Institute of Technology
***************************************************/

Shader "LPK/LPK_SpriteOutline"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _InnerColor("Inner Outline Color", Color) = (1, 1, 1, 1)
        _OuterColor("Outer Outline Color", Color) = (1, 1, 1, 1)
        _InnerThickness("Inner Thickness In Pixels", float) = 3
        _OuterThickness("Outer Thickness In Pixels", float) = 3
        _SampleCount("Sample Count", Range(1, 64)) = 4
        _Angle("Angle", Range(0, 6.283185)) = 0
        [Toggle] _SquareCorners("Square Corners", float) = 1
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            sampler2D _MainTex;

            //Size of pixels.
            float4 _MainTex_TexelSize;
            
            float4 _InnerColor;
            float4 _OuterColor;
            float _InnerThickness;
            float _OuterThickness;
            float _SampleCount;
            float _Angle;
            float _SquareCorners;

            fixed4 frag(v2f frag) : SV_Target
            {
                //Get Texture Color
                float4 col = tex2D(_MainTex, frag.uv) * frag.color;
                col.rgb *= col.a;

                //Sample the surrounding area a given number of times, and determine the state of the nearby pixels
                float innerAlpha = 0;
                float outerAlpha = 0;
                for (int i = 0; i < ceil(_SampleCount); ++i)
                {
                    //Get angle for each sample
                    fixed angle = (i / _SampleCount) * 2 * 3.14159265f + _Angle;
                    float2 dir = float2(cos(angle) * _MainTex_TexelSize.x, sin(angle) * _MainTex_TexelSize.y);
                    //If we are using square corners, place the sample on a unit square rather and a unit circle
                    if (_SquareCorners)
                    {
                        //Dividing by the maximum value makes sure the sample location lands on a square
                        float maxDir = max(abs(cos(angle)), abs(sin(angle)));
                        dir *= (1 / maxDir);
                    }
                    //Sample both for inner and outer
                    //For inner, increase the value if any fully transparent pixels are found
                    innerAlpha += floor(1 - tex2D(_MainTex, frag.uv + dir * _InnerThickness).a);
                    //For for outer, increase the value if any non-fully transparent pixels are found
                    outerAlpha += tex2D(_MainTex, frag.uv + dir * _OuterThickness).a;
                }
                
                //If this pixel has any value, and there was a pixel in range that was fully transparent, highlight it
                float innerOutlineValue = ceil(col.a) * min(ceil(innerAlpha), 1);
                //If this pixel is fully transparent, and there is any pixel in range that was not fully transparent, highlight it
                float outerOutlineValue = (1 - ceil(col.a)) * min(ceil(outerAlpha), 1);

                //If this fragment is either highlighted by the inner or outer outline, recolor it
                col = lerp(col, _InnerColor, innerOutlineValue);
                col = lerp(col, _OuterColor, outerOutlineValue); //Inner and outer values cannot overlap
                return col;
            }
            ENDCG
        }   
    }
}
