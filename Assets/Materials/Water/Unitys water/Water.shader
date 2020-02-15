//Port of Unitys water shader from fragment to surface
//Lessons learned
// - Specular makes no difference because the surface is flat
// - Better to use a color than different gradients to determine night/day water
// - Updating the normal doesnt make a visible difference
// - Testing if we shoould have foam only if the vertex is moving down is not working because not smooth result
//Ideas
// - Test to make a fatter version of whatever is intersecting with the scene to create foam around that object
Shader "Custom/Water/Water" 
{
	Properties 
	{		
		//_TimeColor("Color", Color) = (1,1,1,1)
		_HorizonColor("Horizon color", COLOR) = (.172 , .463 , .435 , 0)
		
		//Noise waves
		//Move this to static in final version to save space
		_NoiseWaveScale("Noise wave scale", Range(0.02,0.15)) = .07
		[NoScaleOffset] _ColorControl("Reflective color (RGB) fresnel (A) ", 2D) = "" { }
		[NoScaleOffset] _BumpMap("Noise waves Normalmap", 2D) = "bump" { }
		//Move this to static in final version to save space
		_NoiseWaveSpeed("Noise wave speed (map1 x,y; map2 x,y)", Vector) = (9,4.5,-8,-3.5)
		
		//Big waves
		//How high are the waves
		_WaveHeight("Wave height", Float) = .07
		//How fast are the waves moving
		_WaveSpeed("Wave speed", Float) = .07
		//The distance between the waves
		_WaveDistance("Wave distance", Float) = .07
		//Direction of the waves
		_WaveDirectionX("Wave direction x", Range(-1, 1)) = 0
		_WaveDirectionZ("Wave direction z", Range(-1, 1)) = 0

		//Foam
		[NoScaleOffset] _FoamTex("Foam texture", 2D) = "" { }
		[NoScaleOffset] _FoamTex2("Foam texture 2", 2D) = "" { }

		//Other
		[NoScaleOffset] _NoiseTex("Noise texture", 2D) = "" { }
	}
	
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
	
		#pragma surface surf Standard vertex:vert addshadow
		#pragma target 3.0

		//Variables
		//Used to determine time of day
		//float4 _TimeColor;

		//Noise waves
		uniform float4 _HorizonColor;
		//The speed of the noise waves in 2 directions
		uniform float4 _NoiseWaveSpeed;
		//How big are the noise waves
		uniform float _NoiseWaveScale;
		//To get noise waves
		uniform sampler2D _BumpMap;
		//Gradient
		uniform sampler2D _ColorControl;

		//Big waves
		uniform float _WaveSpeed;
		uniform float _WaveHeight;
		uniform float _WaveDistance;
		//Direction of the waves
		uniform float _WaveDirectionX;
		uniform float _WaveDirectionZ;
		static float2 waveDir = float2(_WaveDirectionX, _WaveDirectionZ);

		//Foam
		uniform sampler2D _FoamTex;
		uniform sampler2D _FoamTex2;

		//Other
		uniform sampler2D _NoiseTex;

		static float PI = 3.14159265358979323846264338327;



		struct Input
		{
			//What unity is providing
			float2 uv_FoamTex;
			float3 worldPos;
			float3 viewDir;

			//What you have to calculate yourself
			float2 bumpuv1;
			float2 bumpuv2;
		};



		//Shader version of c#'s Mathf.Repeat(t, length) - never larger than maxValue and never smaller than 0
		float getRepeatedValue(float value, float maxValue)
		{
			if (value <= maxValue && value >= 0)
			{
				return value;
			}
			else if (value < 0)
			{
				//Ex value = -6, maxValue = 2.5, should result in 1.5

				//-6 -> 6
				float positiveValue = abs(value);

				//6 / 2.5 -> 0.4
				float remainder = fmod(positiveValue, maxValue);

				//(6 / 2.5) - 0.4 = 2
				float division = (positiveValue / maxValue) - remainder;

				//-6 + (2.5 * 2) = -1 
				value += maxValue * division;

				//-1 + 2.5 = 1.6
				value += maxValue;

				return value;
			}
			else
			{
				//Ex value = 6, maxValue = 2.5, should result in 1

				//6/2.5 = 2.4 -> 0.4
				float remainder = fmod(value, maxValue);

				//0.4 * 2.5 = 1
				value = remainder * maxValue;

				return value;
			}
		}



		//The wave function which should be the same as in the main script
		//Add a timeoffset so we can get a wave pos at another time
		float3 updateVertexWithWave(float3 pos)
		{
			pos.y = 0.0;

			//Old version
			//float waveType = pos.z;

			//pos.y += sin((t * _WaveSpeed + waveType) / _WaveDistance) * _WaveHeight;

			//Combine several sin waves from: http://http.developer.nvidia.com/GPUGems/gpugems_ch01.html
			//This time since beginning
			float t = _Time[1];

			//These are the values we can change to combine several sin waves
			float w = (2 * PI) / _WaveDistance;

			float phi = _WaveSpeed * w;
			
			float A = _WaveHeight;

			float2 D = normalize(float2(_WaveDirectionX, _WaveDirectionZ));
			
			//Normal waves
			//pos.y += sin(dot(pos.xz, D) * w + t * phi) * A;

			//Use a pow function to get less smooth ways
			float waveShapeParam = 2.5;
			//Should maybe multiply everything with 2
			pos.y += pow((sin(dot(pos.xz, D) * w + t * phi) + 1) / 2, waveShapeParam) * A;

			//Another sin perpendiculr
			w *= 0.6;
			
			phi *= 0.5;
			
			A *= 0.6;
			
			D = normalize(float2(D.y, -D.x));
			
			//pos.y += sin(dot(pos.xz, D) * w + t * phi) * A;

			waveShapeParam = 0.5;

			t *= 1.0;

			pos.y += pow((sin(dot(pos.xz, D) * w + t * phi) + 1) / 2, waveShapeParam) * A;

			pos.y /= 2;

			//To make it sort begin at the 0 level
			pos.y -= 1;

			//By dividing with the number if sin we add, we make sure it stays between -1 and 1
			//pos.y += ((sin(2*x) + sin(y * 1.5)) / 2) * _WaveHeight;

			//pos.y += ((sin(2 * insideSin) + sin(0.5 * insideSin) - sin(0.6 * insideSin)) / 3) * _WaveHeight;

			return pos;
		}

		//Get height by using a noise texture
		//Will make the waves a little different from each other, dont need to take it into account 
		//in the wave script in c# because it makes no big difference
		float getHeightFromNoise(float2 uv)
		{
			//pos.y = 0.0;

			//float waveType = pos.z;

			float t = _Time[1] / 100;

			//pos.y += sin((t * _WaveSpeed + waveType) / _WaveDistance) * _WaveScale;

			float height = tex2Dlod(_NoiseTex, float4(uv + t, 0, 3)).r;

			return height;
		}



		//The square magnitude between two vectors
		float getSquareMagnitude(float3 vec1, float3 vec2)
		{
			float3 vec = vec1 - vec2;

			float sqrMagnitude = vec.x * vec.x + vec.y * vec.y + vec.z * vec.z;

			return sqrMagnitude;
		}



		void vert(inout appdata_full i, out Input o)
		{
			UNITY_INITIALIZE_OUTPUT(Input, o);

			//Need the vertex position in global space (to make the waves seamless) and to move the water mesh indepentent of the texture
			float3 vertexWorldPos = mul(unity_ObjectToWorld, i.vertex).xyz;

			//
			// Noise waves
			//
			//Scroll bump waves so the are always seamless
			//_Time[0] = t / 20, which is how time was defined in the script, so just use _Time[0]
			float t = _Time[0];

			//Not really needed to * _WaveScale???
			float4 offset4 = _NoiseWaveSpeed * (t * _NoiseWaveScale);

			//Between 0 and 1.0f
			float4 offsetClamped = float4(
				getRepeatedValue(offset4.x, 1.0f),
				getRepeatedValue(offset4.y, 1.0f),
				getRepeatedValue(offset4.z, 1.0f),
				getRepeatedValue(offset4.w, 1.0f));


			//But is moved to the shader and renamed to offsetClamped()
			float4 temp = vertexWorldPos.xzxz * _NoiseWaveScale + offsetClamped;

			//o.bumpuv1 = temp.xy * float2(.4, .45);
			//By not multiplying we avoid getting a strange jump in the texture that happens for some reason???
			//The same jump is happening if using Unitys water prefab. If you need it then you should multiply it like this:
			//o.bumpuv.xy = vertexWorldPos.xz * _WaveScale * float2(.4, .45) + offsetClamped;
			o.bumpuv1 = temp.xy;
			//Doesnt make a visible differenze to swap w and z?
			o.bumpuv2 = temp.wz;


			//
			// Waves
			//
			//Update the vertex pos with wave height, which updates the y coordinate
			vertexWorldPos = updateVertexWithWave(vertexWorldPos);

			//Add a little noise
			vertexWorldPos.y += getHeightFromNoise(i.texcoord.xy);

			//Save the new position of the vertex in object space
			i.vertex = mul(unity_WorldToObject, float4(vertexWorldPos, 1));


			//Update the normal
			//float3 worldNormal = UnityObjectToWorldNormal(i.normal);
			//float3 worldTangent = UnityObjectToWorldDir(i.tangent.xyz);
			//float3 worldBitangent = cross(worldNormal, worldTangent) * i.tangent.w * unity_WorldTransformParams.w;

			////http://diary.conewars.com/vertex-displacement-shader/
			////float4 position = getNewVertPosition(i.vertex);
			//float3 worldVertex = mul(unity_ObjectToWorld, i.vertex).xyz;
			//float3 positionAndTangent = updateVertexWithWave(worldVertex + worldTangent * 0.01);
			//float3 positionAndBitangent = updateVertexWithWave(worldVertex + worldBitangent * 0.01);
			//
			////Leaves just the tangent and bitangent
			//float3 newTangent = (positionAndTangent - vertexWorldPos); 
			//float3 newBitangent = (positionAndBitangent - vertexWorldPos);

			//float3 newNormal = cross(newTangent, newBitangent) * i.tangent.w * unity_WorldTransformParams.w;
			//
			////Global to local
			//newNormal = mul(unity_WorldToObject, float4(newNormal, 0));

			//i.normal = newNormal;
		}



		void surf(Input IN, inout SurfaceOutputStandard o) 
		{
			//
			// Noise waves
			//
			//Combine the waves
			float3 bump1 = UnpackNormal(tex2D(_BumpMap, IN.bumpuv1)).rgb;
			float3 bump2 = UnpackNormal(tex2D(_BumpMap, IN.bumpuv2)).rgb;
			//Combine the bump into one
			float3 bump = (bump1 + bump2) * 0.5;

			//A texture with alpha channel controlling the Fresnel effect - how much reflection vs. refraction is visible, 
			//based on viewing angle. If you are looking at water from the top, you can see that it's transparent, but if
			//you are looking at water from the side, you can see that the sky is reflecting in it.  
			//So you can use the dot product to determine the amount of reflection vs. refraction
			
			//The view dir in tangent space
			float3 worldViewDir = IN.viewDir.xzy;

			float fresnel = dot(worldViewDir, bump);

			//Unitys original script had float2(fresnel, fresnel), but the last is not needed because gradient
			float4 waterColor = tex2D(_ColorControl, float2(fresnel, 0.5));

			//Add horizon so the distant reflections are not white but the horizon color
			waterColor = float4(lerp(waterColor.rgb, _HorizonColor.rgb, waterColor.a), _HorizonColor.a);
			

			//Add color so we can change time of day by making the water darker/lighter
			//waterColor.rgb *= _TimeColor.rgb;


			//
			//Foam
			//
			//https://developer.nvidia.com/gpugems/GPUGems2/gpugems2_chapter18.html
			//saturate() returns a number between 0 and 1

			//Our wave eq: sin((t * _WaveSpeed + waveType) / _WaveDistance) * _WaveScale;
			//sin(x) goes from -1 -> 1, so 
			float maxHeight = 1 * _WaveHeight;
			//float baseHeight = 0;
			float baseHeight = maxHeight * 0.0;
			float currentHeight = IN.worldPos.y;

			//This is alpha based on height so that we only get white foam on the top of the waves
			float foamAlpha = saturate((currentHeight - baseHeight) / (maxHeight - currentHeight));
			//float foamAlpha = 1;

			//Should move animated in the direction of the waves
			//float4 foamColor = tex2D(_FoamTex, (IN.uv_FoamTex * 5.0) - _Time[0]);
			//Use sin to animate the texture
			float3 noise = tex2D(_NoiseTex, IN.uv_FoamTex).r;
			float2 animatedCooridnates = (IN.uv_FoamTex * 5) + noise - _Time[0];

			float4 foamColor = tex2D(_FoamTex, animatedCooridnates);

			//Dont animate this with sin to hide the seam that appears between water meshes 
			foamColor *= tex2D(_FoamTex2, (IN.uv_FoamTex * 3) + _Time[0] * 0.1) * 2;

			//Add the foam to the water
			float3 waterWithFoamColor = lerp(waterColor.rgb, foamColor.rgb, foamAlpha);

			//This will make the foam transparent where there is water and not white foam
			waterColor.rgb = lerp(waterColor.rgb, waterWithFoamColor, foamColor.a);

			


			//Output
			o.Albedo = waterColor.rgb;
			o.Alpha = waterColor.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
