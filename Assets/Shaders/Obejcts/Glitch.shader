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

            // ����� ���� ������
            uniform float _NoiseStrength;
            uniform float _NoiseSpeed;
            sampler2D _MainTex;
            float2 _MainTex_TexelSize;

            // ���ؽ� ���̴� �Լ�
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
                o.pos = UnityObjectToClipPos(v.vertex);  // �� �������� Ŭ�� �������� ��ȯ
                o.uv = v.uv;  // �ؽ�ó ��ǥ �״�� ����
                return o;
            }

            // �ð� ��� ������ ���� �Լ�
            float random(float2 p)
            {
                return frac(sin(dot(p, float2(12.9898, 78.233))) * 43758.5453);
            }

            // �ȼ� �������� ������ ȿ���� �����ϴ� fragment �Լ�
            half4 frag(v2f i) : SV_Target
            {
                float2 uv = i.uv;
                float noise = random(uv * _NoiseSpeed + _Time.y);
                noise = (noise - 0.5) * 2.0;  // -1~1�� ����
                noise *= _NoiseStrength;

                half4 col = tex2D(_MainTex, uv);
                col.rgb += noise;  // ���� ������ �߰�
                return col;
            }

            ENDCG
        }
    }
    FallBack "Diffuse"
}
