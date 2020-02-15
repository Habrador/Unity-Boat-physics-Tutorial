using UnityEngine;
using System.Collections;

//Different wavetypes

public class WaveTypes 
{

	//Sinus waves
	public static float SinXWave(
		Vector3 position, 
		float speed, 
		float scale,
		float waveDistance,
		float noiseStrength, 
		float noiseWalk,
        float timeSinceStart) 
	{
        float x = position.x;
        float y = 0f;
        float z = position.z;

		//Using only vertex.x or vertex.z will produce straight waves
		//Using only vertex.y will produce an up/down movement
		//vertex.x + vertex.y + vertex.z rolling waves
		//vertex.x * vertex.z produces a moving sea without rolling waves

		float waveType = z;

        y += Mathf.Sin((timeSinceStart * speed + waveType) / waveDistance) * scale;

        //Add noise to make it more realistic
        //y += Mathf.PerlinNoise(x + noiseWalk, y + Mathf.Sin(timeSinceStart * 0.1f)) * noiseStrength;

        return y;
	}



	//Gerstner Waves
	public static float GerstnerWave(
		float x_coord, 
	    float z_coord) 
	{
		/*
		 * y = A * cos(k * x_0 - w * t)
		 * 
		 * A amplitude
		 * x_0 = (x_o, z0)
		 * w - frequency
		 * t - time
		 */

		float y_coord = 0;

		Vector2 x_0 = new Vector2(x_coord, z_coord);

		/*
		 * k = 2 * pi / lamda
		 * 
		 * k - wavevector (determines the direction of the wave)
		 * lambda - length of the wave
		 */


		return y_coord;
	}	
}
