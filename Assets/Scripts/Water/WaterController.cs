using UnityEngine;
using System.Collections;

//Controlls the water

public class WaterController : MonoBehaviour 
{
    public static WaterController current;

    public bool isMoving;

    //Wave height and speed
    public float scale = 0.1f;
    public float speed = 1.0f;
    //The width between the waves
    public float waveDistance = 1f;
    //Noise parameters
    public float noiseStrength = 1f;
    public float noiseWalk = 1f;



    void Start()
    {
        current = this;
    }



    //Get the y coordinate from whatever wavetype we are using
    public float GetWaveYPos(Vector3 position, float timeSinceStart)
    {
        if (isMoving)
        {
            return WaveTypes.SinXWave(position, speed, scale, waveDistance, noiseStrength, noiseWalk, timeSinceStart);
        }
        else
        {
            return 0f;
        }
    }



    //Find the distance from a vertice to water
    //Make sure the position is in global coordinates
    //Positive if above water
    //Negative if below water
    public float DistanceToWater(Vector3 position, float timeSinceStart)
    {
        float waterHeight = GetWaveYPos(position, timeSinceStart);

        float distanceToWater = position.y - waterHeight;

        return distanceToWater;
    }



    //Send water parameters to the shader
    //private void OnPreRender()
    //{
    //    Shader.SetGlobalFloat("_WaterScale", scale);
    //    Shader.SetGlobalFloat("_WaterSpeed", speed);
    //    Shader.SetGlobalFloat("_WaterDistance", waveDistance);
    //    Shader.SetGlobalFloat("_WaterTime", Time.time);
    //}

    void Update()
    {
        Shader.SetGlobalFloat("_WaterScale", scale);
        Shader.SetGlobalFloat("_WaterSpeed", speed);
        Shader.SetGlobalFloat("_WaterDistance", waveDistance);
        Shader.SetGlobalFloat("_WaterTime", Time.time);
        Shader.SetGlobalFloat("_WaterNoiseStrength", noiseStrength);
        Shader.SetGlobalFloat("_WaterNoiseWalk", noiseWalk);
    }
}
