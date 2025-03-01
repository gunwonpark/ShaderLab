Shader "Custom/ScreenNoise"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" { }
        _NoiseStrength ("Noise Strength", Range(0, 1)) = 0.5
        _NoiseSpeed ("Noise Speed", Range(0, 10)) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            // 노이즈에 사용될 변수들
            uniform float _NoiseStrength;
            uniform float _NoiseSpeed;
            sampler2D _MainTex;
            float2 _MainTex_TexelSize;

            // 버텍스 쉐이더 함수
            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);  // 모델 공간에서 클립 공간으로 변환
                o.uv = v.uv;  // 텍스처 좌표 그대로 전달
                return o;
            }

            // 시간 기반 노이즈 생성 함수
            float random(float2 p)
            {
                return frac(sin(dot(p, float2(12.9898, 78.233))) * 43758.5453);
            }

            // 픽셀 단위에서 노이즈 효과를 적용하는 fragment 함수
            half4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float noise = random(uv * _NoiseSpeed + _Time.y);
                noise = (noise - 0.5) * 2.0;  // -1~1로 조정
                noise *= _NoiseStrength;

                half4 col = tex2D(_MainTex, uv);
                col.rgb += noise;  // 색상에 노이즈 추가
                return col;
            }

            ENDCG
        }
    }
    FallBack "Diffuse"
}
