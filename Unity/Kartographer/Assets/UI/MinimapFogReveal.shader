Shader "Custom/MinimapFogReveal"
{
    Properties
    {
        _MainTex("Minimap Texture", 2D) = "white" {}
        _FogMask("Fog Mask", 2D) = "black" {}
        _UnexploredColor("Unexplored Color", Color) = (0,0,0,1)
        _MinimapViewBounds("Minimap View Bounds", Vector) = (-10,10,-10,10)
        _WorldBounds("World Bounds", Vector) = (-100,100,-100,100)
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f { float2 uv : TEXCOORD0; float4 vertex : SV_POSITION; };

            sampler2D _MainTex;
            sampler2D _FogMask;
            float4 _UnexploredColor;
            float4 _MinimapViewBounds;
            float4 _WorldBounds;

            v2f vert(appdata v) { v2f o; o.vertex = UnityObjectToClipPos(v.vertex); o.uv = v.uv; return o; }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 minimapColor = tex2D(_MainTex, i.uv);

                // Convert minimap UV to world coords
                float worldX = lerp(_MinimapViewBounds.x, _MinimapViewBounds.y, i.uv.x);
                float worldZ = lerp(_MinimapViewBounds.z, _MinimapViewBounds.w, i.uv.y);

                // Convert world coords to fog UV
                float fogU = (worldX - _WorldBounds.x) / (_WorldBounds.y - _WorldBounds.x);
                float fogV = (worldZ - _WorldBounds.z) / (_WorldBounds.w - _WorldBounds.z);

                fixed fogValue = tex2D(_FogMask, float2(fogU, fogV)).r;

                fixed3 finalColor = lerp(_UnexploredColor.rgb, minimapColor.rgb, fogValue);
                fixed finalAlpha = minimapColor.a;

                return fixed4(finalColor, finalAlpha);
            }
            ENDCG
        }
    }
}
