using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Simples possible script to make a mesh float
public class FloatingPhysics : MonoBehaviour 
{
    //Drags
    //The object that's floating
    public GameObject floatingObj;
    //The debug to this gameobject, which will display the mesh that underwater
    public GameObject underWaterObj;

    //Script that's doing everything needed with the boat mesh, such as finding out which part is above the water
    private ModifyFloatingMesh modifyFloatingMesh;

    //Mesh for debugging to see which part of the original mesh is below the water
    private Mesh underWaterMesh;

    //The objects rigidbody
    private Rigidbody objectRb;



    void Start()
    {
        //Get the object's rigidbody
        objectRb = gameObject.GetComponent<Rigidbody>();

        //If theres no special floating object, then that means the mesh is attached to this game object
        if (floatingObj == null)
        {
            floatingObj = gameObject;
        }

        //Init the script that will modify the mesh
        modifyFloatingMesh = new ModifyFloatingMesh(floatingObj);

        //Meshes that are below and above the water
        if (underWaterObj != null)
        {
            underWaterMesh = underWaterObj.GetComponent<MeshFilter>().mesh;
        }
    }



    void Update()
    {
        //Generate the under water mesh
        modifyFloatingMesh.GenerateUnderwaterMesh();

        //Display the under water mesh - will take some computer power so remove in final version
        if (underWaterMesh != null)
        {
            //modifyFloatingMesh.DisplayMesh(underWaterMesh, "UnderWater Mesh", modifyFloatingMesh.underWaterTriangleData);
        }
    }



    void FixedUpdate()
    {
        //Add forces to the part of the object that's below the water
        if (modifyFloatingMesh.underWaterTriangleData.Count > 0)
        {
            AddUnderWaterForces();
        }
    }



    //Add all forces that act on the triangles below the water
    void AddUnderWaterForces()
    {
        //Get all triangles
        List<FloatingTriangleData> underWaterTriangleData = modifyFloatingMesh.underWaterTriangleData;

        //Add forces to all triangles
        for (int i = 0; i < underWaterTriangleData.Count; i++)
        {
            //This triangle
            FloatingTriangleData triangleData = underWaterTriangleData[i];

            //Calculate the buoyancy force
            Vector3 buoyancyForce = BuoyancyForce(triangleData);

            //Add the force to the boat
            objectRb.AddForceAtPosition(buoyancyForce, triangleData.center);


            //Debug

            //Normal
            //Debug.DrawRay(triangleData.center, triangleData.normal * 3f, Color.white);

            //Buoyancy
            //Debug.DrawRay(triangleData.center, buoyancyForce.normalized * -3f, Color.blue);
        }
    }



    //The buoyancy force so the boat can float
    private Vector3 BuoyancyForce(FloatingTriangleData triangleData)
    {
        //Buoyancy is a hydrostatic force - it's there even if the water isn't flowing or if the boat stays still

        // F_buoyancy = rho * g * V
        // rho - density of the mediaum you are in
        // g - gravity
        // V - volume of fluid directly above the curved surface 

        //The density of the water
        float rhoWater = 1027f;
        float gravity = -9.81f;

        // V = z * S * n 
        // z - distance to surface
        // S - surface area
        // n - normal to the surface
        Vector3 buoyancyForce = rhoWater * gravity * triangleData.distanceToSurface * triangleData.area * triangleData.normal;

        //The vertical component of the hydrostatic forces don't cancel out but the horizontal do
        buoyancyForce.x = 0f;
        buoyancyForce.z = 0f;

        return buoyancyForce;
    }
}
