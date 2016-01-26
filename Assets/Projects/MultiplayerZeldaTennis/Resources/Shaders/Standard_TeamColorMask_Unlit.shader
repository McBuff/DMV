Shader "Unlit/Standard_TeamColorMask_Unlit"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_MaskTex("ColorMask", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag fullforwardshadows
			// make fog work
			#pragma multi_compile_fog

			//#pragma surface surf Standard fullforwardshadows
			
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
			sampler2D _MaskTex;
			float4 _MainTex_ST;
			float4 _Color;

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				
				return o;
			}
			
			// Blending Modes
			float4 Overlay(float4 cBase, float4 cBlend)
			{
				//http://tech-artists.org/wiki/Blending_functions
				/*
				float4 cNew;
				if (cBase.r > .5) { cNew.r = 1 - (1 - 2 * (cBase.r - .5)) * (1 - cBlend.r); }
				else { cNew.r = (2 * cBase.r) * cBlend.r; }

				if (cBase.g > .5) { cNew.g = 1 - (1 - 2 * (cBase.g - .5)) * (1 - cBlend.g); }
				else { cNew.g = (2 * cBase.g) * cBlend.g; }

				if (cBase.b > .5) { cNew.b = 1 - (1 - 2 * (cBase.b - .5)) * (1 - cBlend.b); }
				else { cNew.b = (2 * cBase.b) * cBlend.b; }

				cNew.a = 1.0;
				return cNew;
				*/
				// Vectorized (easier for compiler)
				float4 cNew;

				// overlay has two output possbilities
				// which is taken is decided if pixel value
				// is below half or not

				cNew = step(0.5, cBase);

				// we pick either solution
				// depending on pixel

				// first is case of < 0.5
				// second is case for >= 0.5

				// interpolate between the two, 
				// using color as influence value
				cNew = lerp((cBase*cBlend * 2), (1.0 - (2.0*(1.0 - cBase)*(1.0 - cBlend))), cNew);

				cNew.a = 1.0;
				return cNew;
			}

			fixed4 frag (v2f i) : SV_Target
			{

				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);

				fixed4 maskValue = tex2D(_MaskTex, i.uv);

				//fixed MixedCol = lerp(float4(0.5,0.5,0.5,1), col * _Color, maskValue.r);

				fixed4 colorMask = lerp(float4(0.5, 0.5, 0.5, 1), col * _Color, maskValue.r);

				fixed4 maskedCol = Overlay(col, colorMask);


				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);				
				return maskedCol;
			}



			ENDCG
		}
		


			// Pass to render object as a shadow caster ( https://gist.github.com/pigeon6/4237385 )
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			Fog{ Mode Off }
			ZWrite On ZTest LEqual Cull Off
			Offset 1, 1

			CGPROGRAM
			
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_shadowcaster
			#pragma fragmentoption ARB_precision_hint_fastest
			
			#include "UnityCG.cginc"

			struct v2f {
				V2F_SHADOW_CASTER;
			};

			v2f vert(appdata_base v)
			{
				v2f o;
				TRANSFER_SHADOW_CASTER(o)
					return o;
			}

			float4 frag(v2f i) : COLOR
			{
				SHADOW_CASTER_FRAGMENT(i)
			}
				ENDCG
			}
	}
}
