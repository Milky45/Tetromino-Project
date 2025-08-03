Shader "Custom/CRTFilter"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _ScanlineIntensity ("Scanline Intensity", Range(0, 1)) = 0.3
        _ScanlineFrequency ("Scanline Frequency", Float) = 600
        _RGBOffset ("RGB Split Offset", Range(0, 0.01)) = 0.001
        _ScanlineScrollSpeed ("Scanline Scroll Speed", Float) = 2.0
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

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _ScanlineIntensity;
            float _ScanlineFrequency;
            float _RGBOffset;
            float _ScanlineScrollSpeed;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // Use built-in _Time.y for time in seconds
                float scrollOffset = _Time.y * _ScanlineScrollSpeed;

                fixed4 col = tex2D(_MainTex, uv);

                // Scroll the scanlines vertically using time
                float scanline = sin((uv.y + scrollOffset) * _ScanlineFrequency) * 0.5 + 0.5;
                col.rgb *= lerp(1.0, scanline, _ScanlineIntensity);

                // RGB Split
                float2 offset = float2(_RGBOffset, 0);
                col.r = tex2D(_MainTex, uv + offset).r;
                col.b = tex2D(_MainTex, uv - offset).b;

                return col;
            }
            ENDCG
        }
    }
}
