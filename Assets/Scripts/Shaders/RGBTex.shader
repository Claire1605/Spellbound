Shader "Spellbound/RGBTex"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_DeformTex("Deform Texture", 2D) = "white" {}
		_Colour1 ("Colour1", Color) = (1,0,0,1)
		_Colour2 ("Colour2", Color) = (0,1,0,1)
		_Colour3 ("Colour3", Color) = (0,0,1,1)
		_Emission("Emission", Color) = (1,1,1,1)
		_EmitStrength("Emissive Strength", Range(0,10)) = 0.0
		_DeformHeight("Deform Height", Float) = 0.0
		_DeformZoom("Deform Zoom", Float) = 1.0
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
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

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
			sampler2D _DeformTex;
			float4 _MainTex_ST;
			fixed4 _Colour1;
			fixed4 _Colour2;
			fixed4 _Colour3;
			fixed4 _Emission;
			float _EmitStrength;
			float _DeformHeight;
			float _DeformZoom;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex + tex2Dlod(_DeformTex, float4(v.uv.x * _DeformZoom, v.uv.y * _DeformZoom, 0, 0)).r * float3(0, _DeformHeight, 0));
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				float red = tex2D(_MainTex, i.uv).r; // = 1 if red, 0 if green, 0 if blue
				float green = tex2D(_MainTex, i.uv).g; // = 0 if red, 1 if green, 0 if blue
				float blue = tex2D(_MainTex, i.uv).b; // = 0 if red, 0 if green, 1 if blue

				float4 result = tex2D(_MainTex, i.uv);

				result.rgb = (red * _Colour1) + (green * _Colour2) + (blue * _Colour3);

				result.a = tex2D(_MainTex, i.uv).a;

				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, result);

				result.rgb += _Emission * _EmitStrength;

				return result;
			}
			ENDCG
		}
	}
}
