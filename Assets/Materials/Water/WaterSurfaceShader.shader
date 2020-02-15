Shader "Custom/WaterSurfaceShader" 
{
	Properties 
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex("Main Texture", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
		_NoiseTex("Noise Texture", 2D) = "white" {}
	}
	
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		//Physically based Standard lighting model, and enable shadows on all light types
		//- Standard means standard lightning
		//- vertex:vert to be able to modify the vertices
		//- addshadow to make the shadows look correct after modifying the vertices
		#pragma surface surf Standard vertex:vert addshadow

		//Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 3.0

		sampler2D _MainTex;
		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		sampler2D _NoiseTex;

		//Water parameters
		float _WaterScale;
		float _WaterSpeed;
		float _WaterDistance;
		float _WaterTime;
		float _WaterNoiseStrength;
		float _WaterNoiseWalk;



		struct Input 
		{
			float2 uv_MainTex;
		};



		//The wave function
		float3 getWavePos(float3 pos)
		{			
			pos.y = 0.0;

			float waveType = pos.z;

			pos.y += sin((_WaterTime * _WaterSpeed + waveType) / _WaterDistance) * _WaterScale;

			//Add noise
			//pos.y += tex2Dlod(_NoiseTex, float4(pos.x, pos.z + sin(_WaterTime * 0.1), 0.0, 0.0) * _WaterNoiseWalk).a * _WaterNoiseStrength;

			return pos;
		}



		void vert(inout appdata_full IN) 
		{
			//Get the global position of the vertex
			float4 worldPos = mul(unity_ObjectToWorld, IN.vertex);

			//Manipulate the position
			float3 withWave = getWavePos(worldPos.xyz);

			//Convert the position back to local
			float4 localPos = mul(unity_WorldToObject, float4(withWave, worldPos.w));

			//Assign the modified vertice
			IN.vertex = localPos;
		}



		void surf (Input IN, inout SurfaceOutputStandard o) 
		{
			//Albedo comes from a texture tinted by color
			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			//Metallic and smoothness come from slider variables
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = c.a;
		}
		
		ENDCG
	}
	FallBack "Diffuse"
}
