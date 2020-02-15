using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Generates a flat square mesh
//From http://catlikecoding.com/unity/tutorials/procedural-grid/
public static class GenerateGrid 
{
    //Generates a mesh width a width and a resolution which is the distance between each vertice
    //So you need to make sure that width and resolution fit together
    //The mesh is also centered around origo
    public static Mesh GenerateMesh(Mesh mesh, float width, float resolution)
    {
        //Calculate how many squares we need
        int squares = (int)(width / resolution);

        //How many squares do we have in each direction
        int xSize = squares;
        int ySize = squares;


        //
        // Add vertices, uvs, and tangents
        //
        Vector3[] vertices = new Vector3[(xSize + 1) * (ySize + 1)];

        Vector2[] uv = new Vector2[vertices.Length];

        Vector4[] tangents = new Vector4[vertices.Length];
        //As we have a flat surface, all tangents simply point in the same direction, which is to the right
        Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);

        //Center it around origo
        float yCoord = -width / 2f;

        //Array pos
        int arrayPos = 0;

        //Loop through all vertices
        for (int y = 0; y <= ySize; y++)
        {
            //Reset
            float xCoord = -width / 2f;

            for (int x = 0; x <= xSize; x++)
            {                
                //Make it flat in y direction
                vertices[arrayPos] = new Vector3(xCoord, 0f, yCoord);

                uv[arrayPos] = new Vector2((float)x / xSize, (float)y / ySize);

                tangents[arrayPos] = tangent;

                //Update for the next iteration
                xCoord += resolution;

                arrayPos++;
            }

            yCoord += resolution;
        }



        //
        // Build the triangles
        //
        int[] triangles = new int[xSize * ySize * 6];

        for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++)
        {
            for (int x = 0; x < xSize; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
                triangles[ti + 5] = vi + xSize + 2;
            }
        }



        //
        //Add everything to the mesh
        //
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.tangents = tangents;
        mesh.triangles = triangles;

        //Normals are defined per vertex, so we have to fill another vector array. 
        //Alternatively, we can ask the mesh to figure out the normals itself based on its triangles
        mesh.RecalculateNormals();

        
        return mesh;
    }
}
