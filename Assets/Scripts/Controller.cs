using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour 
{

	
	void Start() 
	{
		
	}
	
	
	void Update() 
	{
		if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
	}
}
