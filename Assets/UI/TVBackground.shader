Shader "Custom/TVBackground_BuiltIn"
{
    Properties
    {
        _MainColor ("Base Color", Color) = (0.05, 0.05, 0.05, 1)
        _BlobColor ("Blob Color", Color) = (0.4, 0, 0, 1)
        _BlobSpeed ("Blob Speed", Float) = 0.2
        _StaticIntensity ("Static Intensity", Range(0, 0.1)) = 0.03
        _ScanlineSpeed ("Scanline Speed", Float) = 0.5
        _ScanlineDensity ("Scanline Density", Float) = 15.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float4 _MainColor;
            float4 _BlobColor;
            float _BlobSpeed;
            float _StaticIntensity;
            float _ScanlineSpeed;
            float _ScanlineDensity;

            float random(float2 uv) {
                return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
            }

            float noise(float2 uv) {
                float2 i = floor(uv);
                float2 f = frac(uv);
                float a = random(i);
                float b = random(i + float2(1.0, 0.0));
                float c = random(i + float2(0.0, 1.0));
                float d = random(i + float2(1.0, 1.0));
                float2 u = f * f * (3.0 - 2.0 * f);
                return lerp(a, b, u.x) + (c - a) * u.y * (1.0 - u.x) + (d - b) * u.x * u.y;
            }

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                float time = _Time.y;
                float2 uv = i.uv;

                float vignette = 1.0 - length(uv - 0.5) * 0.7;
                float4 finalColor = _MainColor * vignette;

                float blobNoise = noise(uv * 3.0 + float2(time * _BlobSpeed, time * _BlobSpeed * 0.5));
                blobNoise += noise(uv * 2.0 - float2(time * _BlobSpeed * 0.3, time * _BlobSpeed * 0.7));
                float blobs = smoothstep(0.4, 0.8, blobNoise * 0.5);
                finalColor += blobs * _BlobColor * vignette;

                float grain = random(uv + time) * _StaticIntensity;
                finalColor += grain;

                float scanline = sin(uv.y * _ScanlineDensity * 10.0 + time * _ScanlineSpeed) * 0.02;
                float scrollingWave = sin(uv.y * 2.0 - time * _ScanlineSpeed * 2.0) * 0.01;
                finalColor += scanline + scrollingWave;

                return finalColor;
            }
            ENDCG
        }
    }
}
