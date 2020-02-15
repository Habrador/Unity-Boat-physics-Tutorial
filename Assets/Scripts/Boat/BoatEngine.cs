using UnityEngine;
using System.Collections;

public class BoatEngine : MonoBehaviour 
{
    //Drags
    public Transform waterJetTransform;

    //How fast should the engine accelerate?
    public float powerFactor;

    //What's the boat maximum engine power?
    public float maxPower;

    //The boat's current engine power is public for debugging
    public float currentJetPower;

    private float thrustFromWaterJet = 0f;

    private Rigidbody boatRB;

    private float WaterJetRotation_Y = 0f;

    BoatController boatController;



    void Start() 
	{
        boatRB = GetComponent<Rigidbody>();

        boatController = GetComponent<BoatController>();
    }



    void Update() 
	{
        UserInput();
    }


    void FixedUpdate()
    {
        UpdateWaterJet();
    }


    void UserInput()
    {
        //Forward / reverse
        if (Input.GetKey(KeyCode.W))
        {
            if (boatController.CurrentSpeed < 50f && currentJetPower < maxPower)
            {
                currentJetPower += 1f * powerFactor;
            }
        }
        else
        {
            currentJetPower = 0f;
        }

        //Steer left
        if (Input.GetKey(KeyCode.A))
        {
            WaterJetRotation_Y = waterJetTransform.localEulerAngles.y + 2f;

            if (WaterJetRotation_Y > 30f && WaterJetRotation_Y < 270f)
            {
                WaterJetRotation_Y = 30f;
            }

            Vector3 newRotation = new Vector3(0f, WaterJetRotation_Y, 0f);

            waterJetTransform.localEulerAngles = newRotation;
        }
        //Steer right
        else if (Input.GetKey(KeyCode.D))
        {
            WaterJetRotation_Y = waterJetTransform.localEulerAngles.y - 2f;

            if (WaterJetRotation_Y < 330f && WaterJetRotation_Y > 90f)
            {
                WaterJetRotation_Y = 330f;
            }

            Vector3 newRotation = new Vector3(0f, WaterJetRotation_Y, 0f);

            waterJetTransform.localEulerAngles = newRotation;
        }
    }



    void UpdateWaterJet()
    {
        //Debug.Log(boatController.CurrentSpeed);

        Vector3 forceToAdd = -waterJetTransform.forward * currentJetPower;

        //Only add the force if the engine is below sea level
        float waveYPos = WaterController.current.GetWaveYPos(waterJetTransform.position, Time.time);

        if (waterJetTransform.position.y < waveYPos)
        {
            boatRB.AddForceAtPosition(forceToAdd, waterJetTransform.position);
        }
        else
        {
            boatRB.AddForceAtPosition(Vector3.zero, waterJetTransform.position);
        }
    }



    //void CalculatePropellerSpeed() {
    //	//Add propeller force
    //	float forceFromPropeller = v_jet * 5000f;

    //	//Notice transform.right is forward because the boat is rotated

    //	boatRigidBody.AddForceAtPosition(
    //		forceFromPropeller * transform.right, 
    //		WaterJetPosition.transform.position,
    //		ForceMode.Force);


    //	//Add rudder force
    //	float forceFromRudder = 30000f;

    //	if (boatRigidBody.velocity.magnitude > 1f) {

    //		//Is difficult to add a torque
    //		//boatRigidBody.AddTorque(transform.up * forceFromRudder * rudderDirection);


    //		boatRigidBody.AddForceAtPosition(
    //			forceFromRudder * WaterJetPosition.transform.right, 
    //			WaterJetPosition.transform.position,
    //			ForceMode.Force);

    //		//To cancel out the forward movement
    //		boatRigidBody.AddForceAtPosition(
    //			forceFromRudder * -transform.right, 
    //			WaterJetPosition.transform.position,
    //			ForceMode.Force);
    //	}

    //	//Notice transform.right because the boat is rotated
    //	//boatRigidBody.AddForce(forceFromPropeller * WaterJetPosition.transform.right, ForceMode.Impulse);

    //	Debug.Log(forceFromPropeller);

    //	Debug.DrawRay(WaterJetPosition.transform.position, WaterJetPosition.transform.right * 10f, Color.red);
    //}



    ////Calculate the thrust from the waterjets
    //void CalculateWaterJetThrust() {
    //	// T = m * (v_jet - v_in);
    //	// m - water mass
    //	// v_jet - velocity of the water
    //	// v_in - velocity of the ship

    //	float d = 0.5f;

    //	float Q = Mathf.PI * d * d * 0.25f * v_jet; 

    //	float m = PhysicsData.RHO_OCEAN_WATER * Q;

    //	float v_in = v;

    //	//The final thrust force
    //	float T = m * (v_jet - v_in);

    //	thrustFromWaterJet = T;

    //	//Apply the thrust

    //	//Move the center of mass back to get a more realistic motion from the water jet
    //	//Vector3 originalCOM = boatRigidBody.centerOfMass;
    //	//boatRigidBody.centerOfMass = originalCOM + transform.TransformDirection(new Vector3(0f, 0f, WaterJetPosition.transform.position.z));

    //	//The actionPoint has to be on the same y-level or it will produce a strange vobbling effect
    //	Vector3 actionPoint = new Vector3(
    //		0f, 
    //		boatRigidBody.transform.position.y, 
    //		WaterJetPosition.transform.position.z);

    //	boatRigidBody.AddForceAtPosition(thrustFromWaterJet * WaterJetPosition.transform.forward, actionPoint);

    //	//boatRigidBody.AddForce(thrustFromWaterJet * WaterJetPosition.transform.forward);

    //	Debug.Log(WaterJetPosition.transform.position);

    //	//boatRigidBody.centerOfMass = originalCOM;
    //}
}
