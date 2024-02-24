Shader "Custom/FractalFun" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _Area("Area", vector) = (0, 0, 4, 4)
        _FractalColor("Fractal color", vector) = (0.1, 0.34, 0.01, 1)
        _ColorWeight("Color weight", Float) = 20
        _Angle("Angle", range(-3.1415, 3.1415)) = 0
        _ZoomCenter("Zoom center", vector) = (0.5, 0.5, 0, 0)
        _ZoomSpeed("Zoom speed", range(0, 1)) = 0
        _MaxIter("Max iterations", range(1, 255)) = 255
        _Speed("Speed", range(0, 1)) = 0
        _Symmetry("Symmetry", range(0, 1)) = 1
    }

    SubShader {
        Cull Off ZWrite On ZTest LEqual

        Pass {
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
                float4 pos : POSITION;
            };

            v2f vert(appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float4 _Area;
            float4 _FractalColor;
            float4 _ZoomCenter;
            float _ZoomSpeed;
            float _Angle;
            float _MaxIter;
            float _Color;
            float _ColorWeight;
            float _Speed;
            float _Symmetry;
            sampler2D _MainTex;

            float2 rot(float2 p, float2 pivot, float a) {
                float s = sin(a);
                float c = cos(a);
                p -= pivot;
                p = float2(p.x * c - p.y * s, p.x * s + p.y * c);

                p += pivot;
                return p;
            }

            fixed4 frag(v2f i) : SV_Target {
                float2 uv = i.uv - _ZoomCenter.xy;
                uv = abs(uv);
                uv = rot(uv, 0, 0.25 * 3.1415);
                uv = lerp(i.uv - _ZoomCenter.xy, uv, _Symmetry);

                float2 c = _Area.xy + (uv - 0.5) * (_Area.zw / 1000);
                c = rot(c, _Area.xy, _Angle);

                float r = 20;
                float r2 = r * r;

                float2 z;
                float2 zPrevious;
                float iter;
                for(iter = 0; iter < _MaxIter; iter++) {
                    zPrevious = rot(z, 0, _Time.y / 2);
                    z = float2(z.x * z.x - z.y * z.y, 2 * z.x * z.y) + c;
                    // z = float2(z.x * z.x - 3 * z.y * z.y, 3 * z.x * z.x - z.y * z.y) + c;
                    if(dot(z, zPrevious) > r2) break;
                }
                if(iter > _MaxIter) return 0;

                float dist = length(z);
                float fracIter = (dist - r) / (r2 - r);
                fracIter = log2(log(dist) / log(r));

                // iter -= fracIter;

                float m = sqrt(iter / _MaxIter);
                float4 col = sin(_FractalColor * m * _ColorWeight + _Time.y * _Speed) * 0.5 + 0.5; // procedural colors
                // col = tex2D(_MainTex, float2(m * _ColorWeight, _Color))
                float angle = atan2(z.x, z.y);

                col *= smoothstep(3, 0, fracIter);
                col *= 1+sin(angle * 2 + _Time.y * 4) * 0.2;
                return col;
            }

            ENDCG
        }
    }
}