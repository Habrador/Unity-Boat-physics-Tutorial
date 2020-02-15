using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class WaterSquare : MonoBehaviour 
{
    //What's the total width of the mesh
    public float width;
    //What's the mesh's resolution, which is the distance between each vertex
    //Make sure the width and resolution fit together
    public float resolution;



    void Awake() 
	{
        //Create a new mesh
        Mesh mesh = new Mesh();

        //Give it a name
        mesh.name = "Procedural Grid";

        //Generate the mesh
        mesh = GenerateGrid.GenerateMesh(mesh, width, resolution);
        
        //Give the mesh to the mesh filter
        GetComponent<MeshFilter>().mesh = mesh;
    }
}
