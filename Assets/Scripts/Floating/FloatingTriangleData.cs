using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct FloatingTriangleData 
{
    //The corners of this triangle in global coordinates
    public Vector3 p1;
    public Vector3 p2;
    public Vector3 p3;

    //The center of the triangle in global coordinates
    public Vector3 center;

    //The distance to the surface from the center of the triangle
    public float distanceToSurface;

    //The normal to the triangle
    public Vector3 normal;

    //The area of the triangle
    public float area;



    public FloatingTriangleData(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        this.p1 = p1;
        this.p2 = p2;
        this.p3 = p3;

        //Center of the triangle
        this.center = (p1 + p2 + p3) / 3f;

        //Distance to the surface from the center of the triangle
        this.distanceToSurface = Mathf.Abs(WaterController.current.DistanceToWater(this.center, Time.time));

        //Normal to the triangle
        this.normal = Vector3.Cross(p2 - p1, p3 - p1).normalized;

        //Area of the triangle
        float a = Vector3.Distance(p1, p2);

        float c = Vector3.Distance(p3, p1);

        this.area = (a * c * Mathf.Sin(Vector3.Angle(p2 - p1, p3 - p1) * Mathf.Deg2Rad)) / 2f;
    }
}
