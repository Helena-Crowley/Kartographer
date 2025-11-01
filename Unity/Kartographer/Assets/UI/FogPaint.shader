Shader "Custom/FogPaint"
{
    Properties
    {
        _MainTex("Previous Fog Mask", 2D) = "black" {}
        _BrushPos("Brush Position", Vector) = (0,0,0,0)
        _BrushSize("Brush Size", Float) = 0.05
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f { float2 uv : TEXCOORD0; float4 vertex : SV_POSITION; };

            sampler2D _MainTex;
            float4 _BrushPos;
            float _BrushSize;

            v2f vert(appdata v) { v2f o; o.vertex = UnityObjectToClipPos(v.vertex); o.uv = v.uv; return o; }

            fixed4 frag(v2f i) : SV_Target
            {
                float dist = distance(i.uv, _BrushPos.xy);
                if (dist < _BrushSize)
                    return fixed4(1,1,1,1); // reveal
                else
                    return tex2D(_MainTex, i.uv); // keep previous
            }
            ENDCG
        }
    }
}
