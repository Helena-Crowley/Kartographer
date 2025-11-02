Shader "Custom/MinimapFogReveal"
{
    Properties
    {
        _MainTex("Minimap Texture", 2D) = "white" {}
        _FogMask("Fog Mask", 2D) = "black" {}
        _UnexploredColor("Unexplored Color", Color) = (0, 0, 0, 1)
        _MinimapViewBounds("Minimap View Bounds", Vector) = (-10, 10, -10, 10)
        _WorldBounds("World Bounds", Vector) = (-100, 100, -100, 100)
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        Lighting Off
        ZWrite Off
        Cull Off
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
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _FogMask;
            float4 _UnexploredColor;
            float4 _MinimapViewBounds; // minX, maxX, minZ, maxZ
            float4 _WorldBounds; // minX, maxX, minZ, maxZ

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Map UV to world coordinates inside camera view
                float worldX = lerp(_MinimapViewBounds.x, _MinimapViewBounds.y, i.uv.x);
                float worldZ = lerp(_MinimapViewBounds.z, _MinimapViewBounds.w, i.uv.y);

                // Map world coordinates to global minimap UV
                float minimapU = (worldX - _WorldBounds.x) / (_WorldBounds.y - _WorldBounds.x);
                float minimapV = (worldZ - _WorldBounds.z) / (_WorldBounds.w - _WorldBounds.z);

                // Sample minimap texture
                fixed4 minimapColor = tex2D(_MainTex, float2(minimapU, minimapV));

                // Sample fog mask
                fixed fogValue = tex2D(_FogMask, float2(minimapU, minimapV)).r;

                // Blend with unexplored color
                fixed3 rgb = lerp(_UnexploredColor.rgb, minimapColor.rgb, fogValue);

                // Apply circular mask with feathered edges
                float2 center = float2(0.5, 0.5);
                float dist = distance(i.uv, center);
                float radius = 0.45; // adjust to make circle smaller
                float edgeWidth = 0.02; // feather width
                float alpha = smoothstep(radius, radius - edgeWidth, dist);

                return fixed4(rgb, minimapColor.a * alpha);

            }
            ENDCG
        }
    }
}
