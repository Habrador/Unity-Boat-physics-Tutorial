using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//Attach this to a GUI text

public class FPS : MonoBehaviour {
	//How often to update the fps text
	public float updateInterval = 0.5f;

	//FPS accumulated over the interval
	private float accum = 0f;
	//Frames drawn over the interval
	private int frames = 0; 
	//Left time for current interval
	private float timeleft; 

	private Text TextFPS;


	void Start() {
		TextFPS = this.GetComponent<Text>();

		timeleft = updateInterval;  
	}
	

	void Update() {
		timeleft -= Time.deltaTime;
		accum += Time.timeScale/Time.deltaTime;
		frames++;
		
		//Interval ended - update GUI text and start new interval
		if (timeleft <= 0f) {
			TextFPS.text = " FPS: " + (accum/frames).ToString("f0");

			//Debug.Log(accum/frames);

			timeleft = updateInterval;
			accum = 0f;
			frames = 0;
		}
	}
}
