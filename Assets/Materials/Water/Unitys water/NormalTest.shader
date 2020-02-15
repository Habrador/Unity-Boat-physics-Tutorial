Shader "Custom/NormalTest" 
{
	Properties 
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		//Wave
		_Speed("Wave Speed", Range(0.1, 80)) = 5
		_Frequency("Wave Frequency", Range(0,5)) = 2
		_Amplitude("Wave Amplitude", Range(-1, 1)) = 1
	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Standard vertex:vert addshadow

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input 
		{
			float2 uv_MainTex;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		//Wave
		float _Speed;
		float _Frequency;
		float _Amplitude;


		float4 getNewVertPosition(float4 p)
		{
			float time = _Time * _Speed;
			
			p.y += sin(time + p.x * _Frequency) * _Amplitude;
		
			return p;
		}


		void vert(inout appdata_full v)
		{
			float4 vertPosition = getNewVertPosition(v.vertex);


			//
			//Update the normal
			//
			//Calculate the bitangent (sometimes called binormal) from the cross product of the normal and the tangent
			float4 biTangent = float4(cross(v.normal, v.tangent), 0);

			//How far we want to offset our vert position to calculate the new normal
			float vertOffset = 0.01;

			float4 positionAlongTangent = getNewVertPosition(v.vertex + v.tangent * vertOffset);
			float4 positionAlongBiTangent = getNewVertPosition(v.vertex + biTangent * vertOffset);

			//Now we can create new tangents and bitangents based on the deformed positions
			float4 newTangent = normalize(positionAlongTangent - vertPosition);
			float4 newBitangent = normalize(positionAlongBiTangent - vertPosition);

			//Recalculate the normal based on the new tangent and bitangent
			v.normal = cross(newTangent, newBitangent);

			v.vertex = vertPosition;
		}



		void surf (Input IN, inout SurfaceOutputStandard o) 
		{
			// Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			// Metallic and smoothness come from slider variables
			//o.Metallic = _Metallic;
			//o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
