using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Move the trail positions to the wave positions
public class WaterWakesTR : MonoBehaviour 
{
    TrailRenderer trailRenderer;
	


	void Start() 
	{
        trailRenderer = GetComponent<TrailRenderer>();	
	}
	
	
	void Update() 
	{
        //Vector3[] positions = trailRenderer.GetPositions();
        //trailRenderer.getp
    }
}
