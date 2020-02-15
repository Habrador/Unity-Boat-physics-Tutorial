using UnityEngine;
using System.Collections;

public class BoatController : MonoBehaviour
{

    //We need the type of boat so we can change parameters
    public enum BoatType
    {
        Cube,
        SmallBoat,
        Visby
    };

    public BoatType boatType;

    //Speed calculations
    private float currentSpeed;
    private Vector3 lastPosition;

    //Specific data for this boat
    private float shipLength;
    private float shipWidth;



    void Awake()
    {
        //Set the data specific for this boat
        switch (boatType)
        {
            case BoatType.Cube:
                shipLength = 1f;
                shipWidth = 1f;
                break;
            case BoatType.SmallBoat:
                shipLength = 5f;
                shipWidth = 3f;
                break;
            case BoatType.Visby:
                shipLength = VisbyData.length;
                shipWidth = VisbyData.width;
                break;
            default:
                shipLength = 1f;
                shipWidth = 1f;
                break;
        }
    }
	


	void FixedUpdate()
    {
		CalculateSpeed();

        //Debug.Log(currentSpeed);
	}



	//Calculate the current speed in m/s
	private void CalculateSpeed()
    {
		//Calculate the distance of the Transform Object between the fixedupdate calls with 
		//'(transform.position - lastPosition).magnitude' Now you know the 'meter per fixedupdate'
		//Divide this value by Time.deltaTime to get meter per second
		currentSpeed = (transform.position - lastPosition).magnitude / Time.deltaTime;

        //Save the position for the next update
        lastPosition = transform.position;
	}



    //
    // Get and set
    //

    public float CurrentSpeed
    {
        get
        {
            return this.currentSpeed;
        }
    }

    public float ShipWidth
    {
        get
        {
            return this.shipWidth;
        }
    }

    public float ShipLength
    {
        get
        {
            return this.shipLength;
        }
    }
}
