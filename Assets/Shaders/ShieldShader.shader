Shader "Unlit/ShieldShader"
{
    Properties
    {
        [PerRendererData] _MainTex("Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
        _CellSize("Cell size", Float) = 0.05
        _MinAlpha("Minimal alpha", Float) = 0.5
    }
        SubShader
        {
            Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }
            LOD 100

            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"

                struct appdata
                {
                    float4 vertex : POSITION;
                    fixed4 color : COLOR;
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    fixed4 color : COLOR;
                    float4 vertex : SV_POSITION;
                };

                sampler2D _MainTex;
                float4 _MainTex_ST;
                fixed _CellSize;
                fixed _MinAlpha;
                fixed4 _Color;
                StructuredBuffer<float3> ar;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                    o.color = v.color;
                    UNITY_TRANSFER_FOG(o,o.vertex);
                    return o;
                }

                float rand2(float2 p) {
                    float d = dot(p, float2(12.234389, 16.213255));
                    float s = cos(d);
                    return frac(s * 21434.43335344);
                }

                float2 voronoiNoise(float2 value) {
                    float2 bcell = floor(value);

                    float minDist = 10;
                    float2 closestCell;

                    [unroll]
                    for (int i = -1; i < 2; i++) {
                        [unroll]
                        for (int j = -1; j < 2; j++) {
                            float2 cell = bcell + float2(i, j);
                            float2 cellPosition = cell + rand2(cell);
                            float2 toCell = cellPosition - value;
                            float distToCell = length(toCell);

                            if (distToCell < minDist) {
                                minDist = distToCell;
                                closestCell = cell;
                            }
                        }
                    }

                    return float2(minDist, rand2(closestCell));
                }

                fixed4 frag(v2f IN) : SV_Target{
                    float t = _Time.y;
                    float2 uv = IN.uv;
                    float len = (pow(uv.x - 0.5, 2) + pow(uv.y - 0.5, 2));
                    if (len > 0.25) {
                        return (0, 0, 0, 0);
                    }

                    float alpha = 0;
                    [unroll]
                    for (int i = 0; i < 8; i++) {
                        float3 hinfo = ar[i];
                        hinfo.z += t / 4;
                        float2 vec = float2(hinfo.x - uv.x + 0.5, hinfo.y - uv.y + 0.5);
                        float hitLen = length(vec) + hinfo.z;
                        alpha += clamp(1 - hitLen, 0, 1);
                    }
                    float sa = clamp(alpha, 0, 1);

                    float2 vec = float2(0.5 - uv.x, 0.5 - uv.y);
                    float hitLen = clamp(length(vec) + (1 - abs(sin(t / 2))), 0.2, 0.8);
                    alpha += clamp(1 - hitLen, 0, 1);

                    alpha = clamp(alpha, _MinAlpha, 1);
                    float2 value = uv / _CellSize;
                    float noise = voronoiNoise(value).y;
                    fixed4 col = fixed4(noise, noise, noise, alpha);
                    fixed4 inCol = IN.color;
                    col *= lerp((1,1,1,1), inCol, sa);
                    col *= col.a;
                    return col;
                }
                ENDCG
            }
        }
}
