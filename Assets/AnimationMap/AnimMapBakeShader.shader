Shader "Unlit/AnimMapBakeShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_AnimationTex("AnimMap", 2D) = "white" {}
		_AnimLen("AnimLen", float) = 1.0
		_NoiseTex("NoiseMap", 2D) = "white" {}
		_Width("Width", int) = 100
		_Height("Height", int) = 100
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
            // make fog work
			#pragma multi_compile_instancing


            #include "UnityCG.cginc"

            struct appdata
            {
                float2 uv : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			sampler2D _AnimationTex;
			float4 _AnimationTex_TexelSize;
			float _AnimLen;
			sampler2D _NoiseTex;
			int _Width;
			int _Height;
            v2f vert (appdata v, uint vid : SV_VERTEXID)
            {
				UNITY_SETUP_INSTANCE_ID(v);
				
				v2f o;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				float f = _Time.y / _AnimLen;
				fmod(f, 1.0);
				
				float animap_x = (vid + 0.5) * _AnimationTex_TexelSize.x;
				float animap_y = f;
				float4 pos = tex2Dlod(_AnimationTex, float4(animap_x, animap_y, 0, 0));
                o.vertex = UnityObjectToClipPos(pos);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }
    }
}
