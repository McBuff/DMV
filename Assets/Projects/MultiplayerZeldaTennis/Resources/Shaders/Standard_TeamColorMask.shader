Shader "Custom/Standard_TeamColorMask" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Mask("Mask",  2D) = "white" {} // Team Color Mask
		_RimColor("Rim Color", Color) = (0.9,0.9,0.9,0.0) // Rim Lgiht ( http://docs.unity3d.com/Manual/SL-SurfaceShaderExamples.html )
		_RimPower("Rim Power", Range(0.5,8.0)) = 3.0
	}
	SubShader {
		Tags { "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma surface surf Standard fullforwardshadows
			//#pragma surface surf NoLighting 
			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0

			sampler2D _MainTex;
			sampler2D _Mask;
			float4 _RimColor;
			float4 _RimPower;

			struct Input {
				float2 uv_MainTex;
				float3 viewDir;
				float3 worldNormal;
			};

			half _Glossiness;
			half _Metallic;
			fixed4 _Color;


			void surf(Input IN, inout SurfaceOutputStandard o) {
				// Albedo comes from a texture tinted by color
				fixed4 maskValue = tex2D(_Mask, IN.uv_MainTex);

				fixed4 c = tex2D(_MainTex, IN.uv_MainTex);

				fixed4 mixedColor = lerp(c, c * _Color, maskValue.r);

				o.Albedo = mixedColor.rgb;
				o.Smoothness = _Glossiness;
				o.Alpha = c.a;

				// Tutorial code

				float3 localNormal = IN.worldNormal;

				half rim = 1.0 - saturate((dot(normalize(IN.viewDir), IN.worldNormal)));
				o.Emission = 0;// _RimColor.rgb * pow(rim, _RimPower) * 0.2;
			}

			fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
			{
				fixed4 c;
				c.rgb = s.Albedo;
				c.a = s.Alpha;
				return c;
			}
			ENDCG
		}
		
			FallBack "Diffuse"
		
}
