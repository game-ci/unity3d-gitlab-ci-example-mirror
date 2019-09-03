Shader "Custom/Cutscene"
{
    Properties
    {
        _FrontTex ("Front", 2D) = "black" {}
        _BackTex ("Back", 2D) = "black" {}
        _NoiseTex ("Noise", 2D) = "white" {}
        _ScreenTex ("Screen", 2D) = "black"
        _Ramp ("Ramp", 2D) = "gray" {}
        _Width ("Width", Range(0,1)) = 1
        _Fade ("Fade", Range(0,1)) = 1
        _Zoom ("Zoom", Range(0.001,2)) = 1
        _PanX ("PanX", Range(-1,1)) = 0
        _PanY ("PanY", Range(-1,1)) = 0
        _Alpha ("Alpha", Range(0,1)) = 1
    }
    SubShader
    {
        Tags { "Queue"="Transparent" }
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
                float2 uv_FrontTex : TEXCOORD0;
                float2 uv_BackTex : TEXCOORD1;
                float2 uv_NoiseTex : TEXCOORD2;
                float2 uv_ScreenTex : TEXCOORD3;
                float4 vertex : SV_POSITION;
            };

            sampler2D _FrontTex, _BackTex, _NoiseTex, _ScreenTex, _Ramp;
            half4 _FrontTex_ST, _BackTex_ST, _NoiseTex_ST, _Ramp_ST, _ScreenTex_ST;
            half _Fade, _Width;
            half _Zoom, _PanX, _PanY;
            half _Alpha;

            inline float4 InverseLerp(float4 A, float4 B, float4 T) {
                return saturate((T - A)/(B - A));
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv_ScreenTex = v.uv;
                half2 offset = half2(_PanX, _PanY);
                o.uv_FrontTex = (TRANSFORM_TEX(v.uv, _FrontTex) + offset);
                o.uv_BackTex = (TRANSFORM_TEX(v.uv, _BackTex) + offset);
                o.uv_FrontTex = ((o.uv_FrontTex - 0.5) * _Zoom) + 0.5;
                o.uv_BackTex = ((o.uv_BackTex - 0.5) * _Zoom) + 0.5;

                o.uv_NoiseTex = TRANSFORM_TEX(v.uv, _NoiseTex);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                half4 bg = tex2D (_ScreenTex, i.uv_ScreenTex);
                half4 a = tex2D (_FrontTex, i.uv_FrontTex);
                half4 b = tex2D (_BackTex, i.uv_BackTex);
                half n = tex2D (_NoiseTex, i.uv_NoiseTex).r;
                half threshold = smoothstep(0, _Width, (n - lerp(-_Width, 1+_Width, _Fade)));
			    half4 rc = tex2D (_Ramp, threshold); 
                half isFG = step(0.4, threshold);
                half isBG = step(threshold, 0.6);
                half isEdge = step(isFG+isBG,0);
                half4 color = (a*isFG + b*isBG) + (rc*isEdge);
                color.a = 1;
                color = lerp(bg, color, _Alpha);
                return color;
            }
            ENDCG
        }
    }
}
