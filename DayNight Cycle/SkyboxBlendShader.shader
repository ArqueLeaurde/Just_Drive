Shader "Skybox/Blend2TexturesPanoramic"
{
    Properties
    {
        _Tint("Tint Color", Color) = (.5, .5, .5, .5)
        _Blend("Blend", Range(0.0,1.0)) = 0.0
        _Texture1("Texture 1 (HDR)", 2D) = "white" {}
        _Texture2("Texture 2 (HDR)", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "Queue" = "Background" "RenderType" = "Background" }
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"

            sampler2D _Texture1;
            sampler2D _Texture2;
            half4 _Texture1_TexelSize;
            half _Blend;
            fixed4 _Tint;

            struct appdata_t {
                float4 vertex : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float3 texcoord : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.vertex.xyz;
                return o;
            }

            // Latitude-Longitude mapping
            #define PI 3.14159265359

            float2 LatLong(float3 dir)
            {
                float2 longlat;
                longlat.x = atan2(dir.x, dir.z) / (2 * PI) + 0.5;
                longlat.y = asin(dir.y) / PI + 0.5;
                return longlat;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 dir = normalize(i.texcoord);
                float2 uv = LatLong(dir);

                fixed4 tex1 = tex2D(_Texture1, uv);
                fixed4 tex2 = tex2D(_Texture2, uv);

                fixed4 final = lerp(tex1, tex2, _Blend);
                return final * _Tint;
            }
            ENDCG
        }
    }
    Fallback "Skybox/Panoramic"
}