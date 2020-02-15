using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//Generate a convex hull in 2d space with different algorithms
public class ConvexHull 
{
    //Sort vertices based on a convex hull algorithm
    //Will remove the vertices that are not on the convex hull
    public static List<Vector3> SortVerticesConvexHull(List<Vector3> unSortedList)
    {
        List<Vector3> sortedList = new List<Vector3>();

        //Graham's scan algorithm

        //
        // Init
        //
        //Find the vertice with the smallest x coordinate

        //Init with just the first in the list
        float smallestValue = unSortedList[0].x;
        int smallestIndex = 0;

        for (int i = 1; i < unSortedList.Count; i++)
        {
            if (unSortedList[i].x < smallestValue)
            {
                smallestValue = unSortedList[i].x;

                smallestIndex = i;
            }
            //If the are the same, choose the one with the smallest z value
            else if (unSortedList[i].x == smallestValue)
            {
                if (unSortedList[i].z < unSortedList[smallestIndex].z)
                {
                    smallestIndex = i;
                }
            }
        }


        //Remove the smallest value from the list and add it as the first
        //coordinate on the convex hull
        sortedList.Add(unSortedList[smallestIndex]);

        unSortedList.RemoveAt(smallestIndex);


        //Sort the unsorted vertices based on angle
        Vector3 firstPoint = sortedList[0];
        //Important that everything is in 2d space
        firstPoint.y = 0f;

        //Will sort from smallest to highest angle
        unSortedList = unSortedList.OrderBy(n => GetAngle(new Vector3(n.x, 0f, n.z) - firstPoint)).ToList();

        //Check if some angles are the same, if so we need to swap them
        //Is most likely not needed 
        //Can we do this in OderBy
        //for (int i = 1; i < unSortedList.Count; i++)
        //{
        //    float lastAngle = GetAngle(new Vector3(unSortedList[i - 1].x, 0f, unSortedList[i - 1].z) - firstPoint);
            
        //    float thisAngle = GetAngle(new Vector3(unSortedList[i].x, 0f, unSortedList[i].z) - firstPoint);

        //    if (thisAngle == lastAngle)
        //    {
        //        float distSqrLast = Vector3.SqrMagnitude(unSortedList[i - 1] - firstPoint);

        //        float distSqrThis = Vector3.SqrMagnitude(unSortedList[i] - firstPoint);

        //        //The one closest to the point should be first in the list
        //        if (distSqrThis < distSqrLast)
        //        {
        //            Vector3 tmp = unSortedList[i - 1];

        //            unSortedList[i - 1] = unSortedList[i];

        //            unSortedList[i] = tmp;
        //        }
        //    }
        //}


        //Reverse because it's faster to remove vertices from the end
        unSortedList.Reverse();

        //The vertice with the smallest angle is also on the convex hull so add it
        sortedList.Add(unSortedList[unSortedList.Count - 1]);

        unSortedList.RemoveAt(unSortedList.Count - 1);


        //
        //Main algorithm
        //
        //To avoid infinite loop
        int safety = 0;
        while (unSortedList.Count > 0 && safety < 1000)
        {
            safety += 1;

            //Is this a clockwise or a counter-clockwise triangle
            Vector3 a = sortedList[sortedList.Count - 2];
            Vector3 b = sortedList[sortedList.Count - 1];

            Vector3 c = unSortedList[unSortedList.Count - 1];

            unSortedList.RemoveAt(unSortedList.Count - 1);

            sortedList.Add(c);


            if (sortedList.Count == 3 && isClockWise(a, b, c))
            {
                sortedList.RemoveAt(sortedList.Count -1);
            }
            else
            {
                //Need to back track in case we messed up at an earlier point
                while (isClockWise(a, b, c) && safety < 1000)
                {
                    //Remove the next to last one because we know it aint on the hull
                    sortedList.RemoveAt(sortedList.Count - 2);

                    a = sortedList[sortedList.Count - 3];
                    b = sortedList[sortedList.Count - 2];
                    c = sortedList[sortedList.Count - 1];

                    safety += 1;
                }
            }
        }


        return sortedList;
    }



    //Is a triangle in 2d space clockwise or counter-clockwise
    //https://www.youtube.com/watch?v=0HZaRu5IupM
    private static bool isClockWise(Vector3 a, Vector3 b, Vector3 c)
    {
        float signedArea = (b.x - a.x) * (c.z - a.z) - (b.z - a.z) * (c.x - a.x);

        if (signedArea > 0f)
        {
            return false;
        }
        //Is also returning true if all points are on one line
        //Which is good because then we can remove one of them because is not needed
        else
        {
            return true;
        }
    }



    //This returns the angle with some measurement
    private static float GetAngle(Vector3 vec)
    {
        //Angle between the vector and x-axis
        //Vector3.Angle has too low precision so is not working!
        float angle = Mathf.Atan2(vec.z, vec.x);

        return angle;
    }
}
