Shader "Custom/AtmosphericCRTBackground"
{
    // =========================================================
    //  INSPECTOR PROPERTIES
    //  Tweak these in the Unity Material Inspector
    // =========================================================
    Properties
    {
        // --- Base ---
        // CSS: background: #0d0f14
        _BackgroundColor    ("Background Color",        Color)  = (0.051, 0.059, 0.078, 1)

        // --- Blobs ---
        // CSS Blob A: rgba(140,20,20) at -10% 50%, ellipse 60%×80%, opacity 0.18
        _BlobColorA         ("Blob Color A (left-center red)",  Color)  = (0.549, 0.078, 0.078, 1)
        // CSS Blob B: rgba(20,40,100) at 110% 50%, ellipse 40%×60%, opacity 0.12
        _BlobColorB         ("Blob Color B (right-center blue)", Color) = (0.078, 0.157, 0.392, 1)
        // Slot C unused by default — set to black (zero contribution)
        _BlobColorC         ("Blob Color C (unused)",   Color)  = (0.0,  0.0,  0.0,  1)
        _BlobSpeed          ("Blob Movement Speed",     Range(0.0, 1.0))  = 0.04
        // Global multiplier on top of per-blob CSS intensities (0.18 / 0.12 baked in)
        _BlobIntensity      ("Blob Intensity",          Range(0.0, 2.0))  = 1.0

        // --- Noise / Film Grain ---
        // CSS: .noise { opacity: 0.04 } fractalNoise
        _NoiseIntensity     ("Noise Intensity",         Range(0.0, 0.5))  = 0.04

        // --- Scanlines ---
        // CSS: repeating-linear-gradient rgba(255,255,255,0.012) every 4px
        _ScanlineIntensity  ("Scanline Intensity",      Range(0.0, 1.0))  = 0.15
        _ScanlineSpeed      ("Scanline Speed",          Range(0.0, 1.0))  = 0.18
        // ~270 lines on 1080p ≈ 4px repeat matching CSS
        _ScanlineDensity    ("Scanline Density",        Range(50, 800))   = 270

        // --- Vignette ---
        _VignetteIntensity  ("Vignette Intensity",      Range(0.0, 3.0))  = 1.8
        _VignetteSoftness   ("Vignette Softness",       Range(0.1, 2.0))  = 1.1

        // --- Glow / Bloom softness ---
        _GlowIntensity      ("Glow Intensity",          Range(0.0, 1.0))  = 0.06

        // --- Corner Bracket Markers (L-shaped lines visible in the reference image) ---
        _BracketBrightness  ("Bracket Brightness",      Range(0.0, 1.0))  = 0.18
        _BracketLength      ("Bracket Arm Length",      Range(0.005, 0.15)) = 0.038
        _BracketThickness   ("Bracket Line Thickness",  Range(0.0005, 0.006)) = 0.0015
    }

    SubShader
    {
        // Render in UI layer, no depth write, alpha blend
        Tags
        {
            "Queue"           = "Background"
            "RenderType"      = "Opaque"
            "IgnoreProjector" = "True"
            "PreviewType"     = "Plane"
        }

        Cull Off
        ZWrite Off
        ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex   vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            // -------------------------------------------------------
            //  Uniforms (matched to Properties block above)
            // -------------------------------------------------------
            fixed4  _BackgroundColor;
            fixed4  _BlobColorA;
            fixed4  _BlobColorB;
            fixed4  _BlobColorC;
            float   _BlobSpeed;
            float   _BlobIntensity;
            float   _NoiseIntensity;
            float   _ScanlineIntensity;
            float   _ScanlineSpeed;
            float   _ScanlineDensity;
            float   _VignetteIntensity;
            float   _VignetteSoftness;
            float   _GlowIntensity;
            float   _BracketBrightness;
            float   _BracketLength;
            float   _BracketThickness;

            // -------------------------------------------------------
            //  Vertex struct
            // -------------------------------------------------------
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv  = v.uv;
                return o;
            }

            // =========================================================
            //  UTILITY FUNCTIONS
            // =========================================================

            // --- Fast hash (no texture needed) ---
            float hash(float2 p)
            {
                p = frac(p * float2(234.34, 435.345));
                p += dot(p, p + 34.23);
                return frac(p.x * p.y);
            }

            // --- Value noise 2D ---
            // Smooth interpolated lattice noise, no tiling at normal UVs.
            float valueNoise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                // Smooth Hermite interpolation
                float2 u = f * f * (3.0 - 2.0 * f);

                float a = hash(i);
                float b = hash(i + float2(1, 0));
                float c = hash(i + float2(0, 1));
                float d = hash(i + float2(1, 1));

                return lerp(lerp(a, b, u.x), lerp(c, d, u.x), u.y);
            }

            // --- Multi-octave smooth noise (fbm) ---
            // Used for blob shape distortion. Low octaves = very soft blobs.
            float fbm(float2 p, int octaves)
            {
                float val    = 0.0;
                float amp    = 0.5;
                float freq   = 1.0;
                float maxVal = 0.0;
                for (int i = 0; i < octaves; i++)
                {
                    val    += valueNoise(p * freq) * amp;
                    maxVal += amp;
                    amp    *= 0.5;
                    freq   *= 2.0;
                }
                return val / maxVal;
            }

            // --- Elliptical blob shape (matches CSS radial-gradient ellipse) ---
            // `radii` = float2(radiusX, radiusY) in UV space.
            // Mirrors CSS: ellipse 60% 80% → radii = (0.60, 0.80)
            // Returns 0..1 smooth weight; 1 at center, 0 at ellipse boundary.
            float blob(float2 uv, float2 center, float2 radii, float distort, float time)
            {
                // Organic fbm distortion for soft, non-perfect edges
                float2 distortedUV = uv + fbm(uv * 1.8 + time * 0.07, 3) * distort;
                // Elliptical distance: divide each axis by its radius, then length
                float2 delta       = (distortedUV - center) / radii;
                float  d           = length(delta);
                // d < 1 = inside ellipse, d > 1 = outside. Smoothstep for soft edge.
                return smoothstep(1.0, 0.0, d);
            }

            // --- Static noise (matches CSS feTurbulence — no animation, baked at UV) ---
            // Multi-octave value noise. No time dependency = permanently frozen texture.
            // Frequency tuned to approximate CSS baseFrequency='0.9' on a 256×256 SVG.
            float staticNoise(float2 uv)
            {
                return fbm(uv * 210.0, 4);
            }

            // --- L-shaped corner bracket markers ---
            // Draws thin L-shaped lines simultaneously in all 4 screen corners.
            // Uses mirrored UV so a single pass covers every corner at once.
            //
            // After mirroring: m=(0,0) is any corner, m.x grows inward, m.y grows inward.
            // Horizontal arm: along y≈0 (horizontal edge), x from 0 to armLen.
            // Vertical arm:   along x≈0 (vertical edge), y from 0 to armLen.
            //
            // lineThick: line width in UV space — 0.001 is ~1px on 1080p
            // armLen:    how far each arm extends from its corner in UV space
            float cornerBrackets(float2 uv, float lineThick, float armLen)
            {
                // Mirror UV: (0,0) → all 4 corners simultaneously
                float2 m = float2(
                    uv.x < 0.5 ? uv.x : 1.0 - uv.x,
                    uv.y < 0.5 ? uv.y : 1.0 - uv.y
                );

                float aa = lineThick; // anti-alias band = one line width

                // Horizontal arm: y close to 0 (edge), x from 0 to armLen
                // smoothstep(a, b, x) with a>b: 1 when x<b, 0 when x>a — draws the line band
                float hArm = smoothstep(lineThick + aa, lineThick - aa, m.y)   // on edge row
                           * smoothstep(armLen + aa,    armLen - aa,    m.x);   // within arm

                // Vertical arm: x close to 0 (edge), y from 0 to armLen
                float vArm = smoothstep(lineThick + aa, lineThick - aa, m.x)   // on edge column
                           * smoothstep(armLen + aa,    armLen - aa,    m.y);   // within arm

                return saturate(hArm + vArm); // saturate handles overlapping corner joint
            }

            // =========================================================
            //  FRAGMENT SHADER
            // =========================================================
            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float  t  = _Time.y;

                // =======================================================
                // 1. BASE BACKGROUND COLOR
                // =======================================================
                float3 col = _BackgroundColor.rgb;

                // =======================================================
                // 2. MOVING AMBIENT BLOBS
                //    Positions and ellipse sizes directly transcribed from CSS.
                //    CSS uses additive gradient layers, so we blend additively here.
                //    Per-blob CSS alpha intensities are baked in (0.18 / 0.12).
                //    _BlobIntensity is a global multiplier over both (default 1.0).
                // =======================================================
                float bs = _BlobSpeed;

                // --- Blob A ---
                // CSS: radial-gradient(ellipse 60% 80% at -10% 50%, rgba(140,20,20,0.18))
                // Center: x=-10% → -0.10 UV, y=50% → 0.50 UV
                // Radii:  60% wide → 0.60 UV, 80% tall → 0.80 UV
                // Drifts very slightly around its anchor to stay organic
                float2 centerA = float2(
                    -0.10 + sin(t * bs * 0.31) * 0.04,
                     0.50 + cos(t * bs * 0.27) * 0.03
                );
                float blobA = blob(uv, centerA, float2(0.60, 0.80), 0.12, t * bs);

                // --- Blob B ---
                // CSS: radial-gradient(ellipse 40% 60% at 110% 50%, rgba(20,40,100,0.12))
                // Center: x=110% → 1.10 UV, y=50% → 0.50 UV
                // Radii:  40% wide → 0.40 UV, 60% tall → 0.60 UV
                float2 centerB = float2(
                    1.10 + cos(t * bs * 0.23) * 0.04,
                    0.50 + sin(t * bs * 0.19) * 0.03
                );
                float blobB = blob(uv, centerB, float2(0.40, 0.60), 0.10, t * bs + 3.7);

                // --- Blob C (disabled by default — _BlobColorC is black) ---
                float2 centerC = float2(0.5, 0.5);
                float blobC = blob(uv, centerC, float2(0.20, 0.20), 0.08, t * bs + 7.1);

                // Additive blend — matches CSS gradient layering behaviour.
                // Each blob's CSS alpha is baked in as a constant weight.
                float3 blobContrib  = _BlobColorA.rgb * blobA * 0.18;  // CSS: opacity 0.18
                       blobContrib += _BlobColorB.rgb * blobB * 0.12;  // CSS: opacity 0.12
                       blobContrib += _BlobColorC.rgb * blobC * 0.10;  // zero by default

                col += blobContrib * _BlobIntensity;

                // =======================================================
                // 3. SUBTLE INNER GLOW
                //    A very faint radial brightening at screen center
                //    simulates the slight bloom/glow of an old CRT phosphor.
                // =======================================================
                float2 centeredUV  = uv - 0.5;
                float  radialDist  = length(centeredUV);
                float  glowFactor  = smoothstep(0.8, 0.0, radialDist) * _GlowIntensity;
                col += col * glowFactor; // Multiply-add to boost existing luminance


                //    Single soft CRT refresh band sweeping TOP → BOTTOM once per cycle.
                //    frac(t * speed) gives a 0..1 position looping over time.
                //    We invert (1 - pos) so the band travels downward in UV space
                //    (UV y=1 is top, y=0 is bottom in Unity's default orientation).
                // =======================================================

                // Band position: 0 = top of screen, 1 = bottom, cycling every ~5-6s
                float scanCycleSpeed = _ScanlineSpeed * 0.18;   // tune: ~0.18 ≈ one pass per 5.5s
                float scanPos        = 1.0 - frac(t * scanCycleSpeed); // top→bottom direction

                // Soft band: distance from current UV.y to the band's position
                float bandWidth      = 0.06;   // half-width of the band (in UV units)
                float bandDist       = abs(uv.y - scanPos);

                // Smooth falloff — a single gentle horizontal stripe
                float scanBand       = smoothstep(bandWidth, 0.0, bandDist);

                // Add a slightly wider, dimmer halo around the band for softness
                float scanHalo       = smoothstep(bandWidth * 3.5, 0.0, bandDist) * 0.35;

                float scanFactor     = saturate(scanBand + scanHalo);

                // Apply as a soft brightening pass (like CRT phosphor re-energising)
                col += col * scanFactor * _ScanlineIntensity * 1.4;

                // A very faint static scanline texture across the whole screen.
                // CSS: repeating-linear-gradient rgba(255,255,255,0.012) every 4px.
                // sin gives a 0..1 wave; pow sharpens peaks into thin bright lines.
                // We ADD (not subtract) to match CSS which adds white.
                float fineLine  = sin(uv.y * _ScanlineDensity * 3.14159) * 0.5 + 0.5;
                float fineAlpha = pow(fineLine, 6.0) * _ScanlineIntensity * 0.10;
                col            += fineAlpha;  // additive — CSS adds rgba(255,255,255,0.012)

                // =======================================================
                // 5. STATIC NOISE / FILM GRAIN
                //    Permanently frozen fractal noise — matches CSS feTurbulence
                //    (fractalNoise, 4 octaves) which is static by design.
                //    No time argument = same pattern on every frame.
                // =======================================================
                float grain    = staticNoise(uv);
                // Remap fbm 0..1 to -0.5..0.5 and scale by intensity
                float grainVal = (grain - 0.5) * _NoiseIntensity;
                col           += grainVal;

                // =======================================================
                // 6. VIGNETTE
                //    Elliptical radial darkening tuned to match reference image:
                //    near-black corners, smooth falloff, edge midpoints still
                //    slightly visible so blob colors bleed through at screen edges.
                // =======================================================
                float2 vigUV  = uv - 0.5;
                // Elliptical scale: taller on Y to account for 16:9 — concentrates
                // darkening at top/bottom edges more than left/right, matching image.
                vigUV        *= float2(0.88, 1.28);
                float vigDist = dot(vigUV, vigUV); // squared elliptical distance
                // smoothstep inner edge at 0.12 (where darkening begins), outer at
                // _VignetteSoftness-driven range (default ≈ 0.67).
                float vigRange = 0.12 + _VignetteSoftness * 0.5;
                float vig     = 1.0 - smoothstep(0.12, vigRange, vigDist);
                // Power curve: < 1.0 = gentler falloff; > 1.0 = more aggressive
                vig           = pow(max(vig, 0.0), _VignetteIntensity * 0.44);
                col          *= vig;

                // =======================================================
                // 7. CORNER BRACKET MARKERS
                //    L-shaped UI lines in all 4 corners, as seen in reference image.
                //    Rendered after vignette so brackets stay visible despite darkening.
                //    Additive blend: bright lines over the (already darkened) corners.
                // =======================================================
                float brackets = cornerBrackets(uv, _BracketThickness, _BracketLength);
                col           += brackets * _BracketBrightness;

                // =======================================================
                // 7. CLAMP & OUTPUT
                // =======================================================
                col = saturate(col);

                return fixed4(col, 1.0);
            }
            ENDCG
        }
    }

    FallBack Off
}
