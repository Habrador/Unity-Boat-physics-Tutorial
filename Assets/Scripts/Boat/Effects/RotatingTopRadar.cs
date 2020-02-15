using UnityEngine;
using System.Collections;

public class RotatingTopRadar : MonoBehaviour 
{
    public float rotationSpeed;

	
	void Start() 
	{
	
	}
	
	
	
	void Update() 
	{
        transform.Rotate(Vector3.up * Time.deltaTime * rotationSpeed);
    }
}
