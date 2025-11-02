Shader "Custom/FogPaint"
{
    Properties
    {
        _MainTex("Previous Fog Mask", 2D) = "black" {}
        _BrushPos("Brush Position (xy=pos, z=size)", Vector) = (0, 0, 0.05, 0)
        _WorldBounds("World Bounds (minX,maxX,minZ,maxZ)", Vector) = (-100, 100, -100, 100)
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
            float4 _BrushPos; // xy = position, z = size
            float4 _WorldBounds; // (minX, maxX, minZ, maxZ)

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 diff = i.uv - _BrushPos.xy;

                // -- - Aspect ratio correction -- -
                float worldWidth = (_WorldBounds.y - _WorldBounds.x)/1.0;
                float worldHeight = (_WorldBounds.w - _WorldBounds.z)/1.0;
                diff.x *= worldHeight / worldWidth; // scale X, not Y
                // -- -- -- -- -- -- -- -- -- -- -- -- -- -- -- -

                float dist = length(diff);
                float brushSize = _BrushPos.z;

                if (dist < brushSize)
                return fixed4(1, 1, 1, 1); // reveal
                else
                return tex2D(_MainTex, i.uv);
            }


            ENDCG
        }
    }
}
