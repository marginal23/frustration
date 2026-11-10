// Шейдер в стиле PS1 для URP:
// - вершины "прилипают" к сетке пикселей, поэтому модели слегка дрожат при движении камеры;
// - текстуры без перспективной коррекции (аффинные), поэтому они "плывут" на больших полигонах;
// - освещение считается по вершинам, как на старых консолях;
// - поддерживает туман из Lighting Settings.
// Для работы нужен рендерер в режиме Forward (не Forward+).
Shader "Frustration/PS1 Lit"
{
    Properties
    {
        _BaseMap ("Texture", 2D) = "white" {}
        _BaseColor ("Color", Color) = (1, 1, 1, 1)
        [HDR] _EmissionColor ("Emission", Color) = (0, 0, 0, 1)
        _SnapScale ("Vertex Snap (0 = off)", Range(0, 1)) = 0.5
        _Affine ("Affine Texture Warp", Range(0, 1)) = 1
        _Cutoff ("Alpha Cutoff (0 = off)", Range(0, 1)) = 0
        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull", Float) = 2
        _FogScale ("Fog Strength", Range(0, 2)) = 1
        _ExtraLight ("Extra Light (sky)", Color) = (0, 0, 0, 1)
        // Сдвиг глубины: у ковров и наклеек отрицательный, чтобы они не мерцали на полу и стенах.
        _OffsetFactor ("Depth Offset Factor", Float) = 0
        _OffsetUnits ("Depth Offset Units", Float) = 0
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "Queue" = "Geometry" }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }
            Cull [_Cull]
            Offset [_OffsetFactor], [_OffsetUnits]

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                half4 _BaseColor;
                half4 _EmissionColor;
                float _SnapScale;
                float _Affine;
                float _Cutoff;
                float _FogScale;
                half4 _ExtraLight;
            CBUFFER_END

            // Плоскость зеркала (нормаль xyz, смещение w), задаётся скриптом MirrorWorld.
            // Свет с одной стороны зеркала не освещает предметы с другой стороны,
            // иначе лампы настоящей комнаты светили бы на комнату-отражение и наоборот.
            float4 _MirrorPlane;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 uvw : TEXCOORD0;
                half3 lighting : TEXCOORD1;
                half fogFactor : TEXCOORD2;
            };

            Varyings vert(Attributes input)
            {
                Varyings output;
                VertexPositionInputs positions = GetVertexPositionInputs(input.positionOS.xyz);
                float4 positionCS = positions.positionCS;

                if (_SnapScale > 0.001)
                {
                    float2 grid = _ScreenParams.xy * _SnapScale;
                    float2 ndc = positionCS.xy / positionCS.w;
                    ndc = floor(ndc * grid + 0.5) / grid;
                    positionCS.xy = ndc * positionCS.w;
                }
                output.positionCS = positionCS;

                float2 uv = TRANSFORM_TEX(input.uv, _BaseMap);
                float w = lerp(1.0, positionCS.w, _Affine);
                output.uvw = float3(uv * w, w);

                float3 normalWS = TransformObjectToWorldNormal(input.normalOS);
                Light mainLight = GetMainLight();
                half3 lighting = mainLight.color * saturate(dot(normalWS, mainLight.direction));

                #if defined(_ADDITIONAL_LIGHTS) || defined(_ADDITIONAL_LIGHTS_VERTEX)
                uint lightCount = GetAdditionalLightsCount();
                float vertexSide = dot(positions.positionWS, _MirrorPlane.xyz) + _MirrorPlane.w;
                for (uint i = 0u; i < lightCount; ++i)
                {
                    int lightIndex = GetPerObjectLightIndex(i);
                    #if USE_STRUCTURED_BUFFER_FOR_LIGHT_DATA
                    float3 lightPosition = _AdditionalLightsBuffer[lightIndex].position.xyz;
                    #else
                    float3 lightPosition = _AdditionalLightsPosition[lightIndex].xyz;
                    #endif
                    float lightSide = dot(lightPosition, _MirrorPlane.xyz) + _MirrorPlane.w;
                    if (vertexSide * lightSide < 0.0) continue;

                    Light light = GetAdditionalLight(i, positions.positionWS);
                    lighting += light.color * light.distanceAttenuation * saturate(dot(normalWS, light.direction));
                }
                #endif

                lighting += SampleSH(normalWS) + _ExtraLight.rgb;
                output.lighting = lighting;
                // _FogScale < 1 — туман слабее (двор за окнами), > 1 — гуще.
                output.fogFactor = ComputeFogFactor(positionCS.z) * _FogScale;
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float2 uv = input.uvw.xy / input.uvw.z;
                half4 color = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv) * _BaseColor;
                // Прозрачные пиксели (листья, лепестки) просто отбрасываются, как на старых консолях.
                if (_Cutoff > 0.0) clip(color.a - _Cutoff);
                color.rgb = color.rgb * input.lighting + _EmissionColor.rgb;
                color.rgb = MixFog(color.rgb, input.fogFactor);
                return color;
            }
            ENDHLSL
        }

        Pass
        {
            Name "DepthOnly"
            Tags { "LightMode" = "DepthOnly" }
            ZWrite On
            Cull [_Cull]
            Offset [_OffsetFactor], [_OffsetUnits]
            ColorMask R

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                half4 _BaseColor;
                half4 _EmissionColor;
                float _SnapScale;
                float _Affine;
                float _Cutoff;
                float _FogScale;
                half4 _ExtraLight;
            CBUFFER_END

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            struct DepthVaryings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            DepthVaryings vert(float4 positionOS : POSITION, float2 uv : TEXCOORD0)
            {
                DepthVaryings output;
                output.positionCS = TransformObjectToHClip(positionOS.xyz);
                output.uv = TRANSFORM_TEX(uv, _BaseMap);
                return output;
            }

            half frag(DepthVaryings input) : SV_Target
            {
                if (_Cutoff > 0.0)
                    clip(SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv).a * _BaseColor.a - _Cutoff);
                return 0;
            }
            ENDHLSL
        }
    }

    FallBack "Hidden/Universal Render Pipeline/FallbackError"
}
