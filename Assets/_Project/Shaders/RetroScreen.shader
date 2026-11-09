// Финальный экранный эффект в духе PS1: урезанная палитра и дизеринг.
// Картинка игры (маленькая RenderTexture) выводится через этот шейдер на весь экран.
//
// Как это работает:
// - каждый канал цвета округляется до _ColorLevels ступеней (32 ступени = 15-битный цвет PS1);
// - чтобы на плавных переходах не было полос, перед округлением добавляется порог
//   из матрицы Байера 4x4: соседние пиксели округляются по-разному, получается узор из точек.
// Всё считается в пикселях маленькой текстуры, поэтому узор крупный, как на старых консолях.
Shader "Frustration/Retro Screen"
{
    Properties
    {
        [PerRendererData] _MainTex ("Game Texture", 2D) = "black" {}
        _ColorLevels ("Color Levels Per Channel", Range(2, 64)) = 24
        _DitherStrength ("Dither Strength", Range(0, 1)) = 1
        _Contrast ("Contrast", Range(0.5, 2)) = 1.1
        _Brightness ("Brightness", Range(-0.5, 0.5)) = 0.02
        _Tint ("Tint", Color) = (1, 0.97, 0.92, 1)
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Opaque" "PreviewType" = "Plane" "CanUseSpriteAtlas" = "False" }
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _ColorLevels;
            float _DitherStrength;
            float _Contrast;
            float _Brightness;
            fixed4 _Tint;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            static const float Bayer4x4[16] =
            {
                 0.0,  8.0,  2.0, 10.0,
                12.0,  4.0, 14.0,  6.0,
                 3.0, 11.0,  1.0,  9.0,
                15.0,  7.0, 13.0,  5.0
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 color = tex2D(_MainTex, i.uv).rgb;

                // Работаем в гамма-пространстве, как старое железо.
                #ifndef UNITY_COLORSPACE_GAMMA
                color = LinearToGammaSpace(color);
                #endif

                color = (color - 0.5) * _Contrast + 0.5 + _Brightness;
                color *= _Tint.rgb;
                color = saturate(color);

                int2 pixel = int2(floor(i.uv * _MainTex_TexelSize.zw)) % 4;
                float threshold = (Bayer4x4[pixel.y * 4 + pixel.x] + 0.5) / 16.0;
                threshold = lerp(0.5, threshold, _DitherStrength);

                float steps = max(_ColorLevels - 1.0, 1.0);
                color = floor(color * steps + threshold) / steps;

                #ifndef UNITY_COLORSPACE_GAMMA
                color = GammaToLinearSpace(color);
                #endif

                return fixed4(color, 1.0);
            }
            ENDCG
        }
    }
}
