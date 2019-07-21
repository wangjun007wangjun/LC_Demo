Shader "Custom/ImageCutter"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_HeightRate("HeightRate", float) = 0
		_Angle("Angle", Range(0,90))=45
		_CutPos("CutPosition", Range(0,1)) = 0.5
	}
	SubShader
	{
		Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			half _HeightRate;
			half _Angle;
			half _CutPos;

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				half angle = _Angle * 0.0174444;
				if (i.uv.x > _CutPos && i.uv.y * _HeightRate / tan(angle) < (i.uv.x - _CutPos))
				{
					col.a = 0;
				}		
				return col;
			}
		ENDCG
	}
		}
}
