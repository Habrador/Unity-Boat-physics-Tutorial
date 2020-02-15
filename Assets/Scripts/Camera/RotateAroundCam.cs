using UnityEngine;
using System.Collections;

public class RotateAroundCam : MonoBehaviour 
{
    //The transform the camera should look at
	public Transform lookAtThisTrans;

    //The height, if we dont want to look at the center position of the object
    private float heightOffset = 1f;
    //The distance from the camera to the object we look at
	float distanceToObject = 50f;
    //The min and max distance for zooming
    float minDistance = 10f;
    float maxDistance = 100f;
    float zoomSpeed = 5f;
    //The speed which we move the camera
	float xSpeed = 180f;
	float ySpeed = 90f;
    //Limit so we cant move the camera too much
	float yMin = 0f;
	float yMax = 80f;

    //Camera angles
	float xAngle = 45f;
	float yAngle = 45f;

    //Has to save the rotation if we hold the mouse button
    //or it will reset the rotation if we release the mouse button
    Quaternion rotation;


    private void Start()
    {
        rotation = Quaternion.Euler(yAngle, xAngle, 0f);
    }



	void LateUpdate() 
    {
        if (lookAtThisTrans)
        {
            UpdateCamera();
        }
	}



    private void UpdateCamera()
    {
        //Move the camera when we hold left mouse
        //Can also use transform.RotateAround();
        if (Input.GetMouseButton(0))
        {
            xAngle += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
            yAngle -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;

            //Make sure we cant move y angle too much
            yAngle = ClampAngle(yAngle, yMin, yMax);

            rotation = Quaternion.Euler(yAngle, xAngle, 0f);
        }

        //Add the new rotation to the camera
        Vector3 newCamPosition = lookAtThisTrans.position;

        //Move the camera away from the car to the distance we want based on the angle we have
        //When you multiply a quaternion and a vector, it is essentially a transformation of the 
        //vector according to the rotation represented by the quaternion
        newCamPosition += rotation * (-Vector3.forward * distanceToObject);

        transform.position = newCamPosition;

        //Make sure the camera looks at whatever we want to look at
        transform.LookAt(lookAtThisTrans.position + Vector3.up * heightOffset);
    }



	float ClampAngle(float angle, float min, float max) 
    {
		if (angle < -360f)
        {
            angle += 360f;
        }
		else if (angle > 360f)
        {
            angle -= 360f;
        }

		return Mathf.Clamp(angle, min, max);
	}



	void Update() 
    {
		//Zoom in/out
		if (Input.GetAxis("Mouse ScrollWheel") > 0f) 
        {
            distanceToObject -= zoomSpeed;
		} 
		else if (Input.GetAxis("Mouse ScrollWheel") < 0f) 
        {
            distanceToObject += zoomSpeed;
		}

        distanceToObject = Mathf.Clamp(distanceToObject, minDistance, maxDistance);
	}
}
