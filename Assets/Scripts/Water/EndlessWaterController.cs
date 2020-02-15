using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Creates an endless sea
public class EndlessWaterController : MonoBehaviour 
{
    public static EndlessWaterController current;

    //What the water is following
    public GameObject toFollowObj;
    //Water square
    public GameObject waterSquareObj;
    //Water cylinder that will be below the squares to make it seem endless
    //Important that it's a cylinder
    //Will move together with the camera
    public Transform waterCylinder;

    //Water chunk parameters
    //The distance between each vertice
    //private float waterChunkResolution = 20f;
    //The total width in m of one chunk
    private float chunkWidth = 200f;

    //A list with all water squares that we will move around
    private List<WaterChunk> allWaterChunks = new List<WaterChunk>();

    //A list with coordinates that has no water chunk
    private List<Vector3> coordinatesWithoutChunkList = new List<Vector3>();

    //A dictionary with all water chunks
    //private Dictionary<Vector3, WaterChunk> waterChunkDictionary = new Dictionary<Vector3, WaterChunk>();

    //Which was the last position of the camera in chunk-space
    //We dont need to update any chunks if it was the same position
    private int oldChunkPosX;
    private int oldChunkPosZ;
    //If the resolution of the chunks is based on height, then we need to check which the old chunk resolution
    private int oldChunkResolution;



    void Start() 
	{
        current = this;

        //Generate the water meshes
        GenerateSea();

        //Set it to something big so we always update the first iteration
        oldChunkPosX = 200000;
        oldChunkPosZ = 200000;

        oldChunkResolution = 20000;
    }



    void Update() 
	{
        //Move the circle water that will make the sea look endless to the position of the camera
        MoveWaterToObjToFollow();

        //Update which water chunks are visible
        UpdateWaterChunks();
    }



    //Generates the sea meshes that we will move around to make the sea endless
    private void GenerateSea()
    {
        allWaterChunks.Clear();

        //Pool water chunks
        for (int i = 0; i < 20; i++)
        {
            AddNewWaterSquare();
        }
    }



    //Add a new water square if needed
    private void AddNewWaterSquare()
    {
        //Create a new water chunk
        WaterChunk newChunk = new WaterChunk();

        //Add it to the list of all chunks
        allWaterChunks.Add(newChunk);

        //Need to set its width and resolution before instantiating a chunk because we 
        //are creating the mesh in awake
        WaterSquare waterSquareScript = waterSquareObj.GetComponent<WaterSquare>();

        waterSquareScript.width = chunkWidth;


        //Add water chunks with different resolutions
        waterSquareScript.resolution = 2;

        GameObject highDetailedWaterChunk = Instantiate(waterSquareObj, transform) as GameObject;

        newChunk.highDetailedWaterChunk = highDetailedWaterChunk;


        waterSquareScript.resolution = 5;

        GameObject mediumDetailedWaterChunk = Instantiate(waterSquareObj, transform) as GameObject;

        newChunk.mediumDetailedWaterChunk = mediumDetailedWaterChunk;


        waterSquareScript.resolution = 50;

        GameObject lowDetailedWaterChunk = Instantiate(waterSquareObj, transform) as GameObject;

        newChunk.lowDetailedWaterChunk = lowDetailedWaterChunk;

        //Deactivate the chunk
        newChunk.DeactivateChunk();
    }



    //Move the water cylinder as we move the camera
    private void MoveWaterToObjToFollow()
    {
        //Position of what we are following
        Vector3 toFollowPos = toFollowObj.transform.position;

        toFollowPos.y = -20f;

        //Move the water fill cylinder to this position
        waterCylinder.position = toFollowPos;
    }



    //Update which water chuncks are visible
    private void UpdateWaterChunks()
    {
        float maxViewDistance = 500f;

        //How many chunks are visible in one direction
        int chunksVisible = Mathf.RoundToInt(maxViewDistance / chunkWidth);

        //What's the coordinate of the chunk the camera is positioned above?
        //Coordinate is not world position, but something like (2, 1)
        int currentChunkCoordX = Mathf.RoundToInt(toFollowObj.transform.position.x / chunkWidth);
        int currentChunkCoordZ = Mathf.RoundToInt(toFollowObj.transform.position.z / chunkWidth);


        //Update the chunks if we have moved the camera sideways to a new chunk or heightways to a new chunk resolution
        if ((currentChunkCoordX == oldChunkPosX && currentChunkCoordZ == oldChunkPosZ) && oldChunkResolution == GetChunkResolution())
        {
            return;
        }


        //The water has moved so we need to update the chunks

        //Save the new positions
        oldChunkPosX = currentChunkCoordX;
        oldChunkPosZ = currentChunkCoordZ;

        oldChunkResolution = GetChunkResolution();


        //First deactivate all water chunks
        for (int i = 0; i < allWaterChunks.Count; i++)
        {
            allWaterChunks[i].DeactivateChunk();
        }

        //Reset the list with coordinates where we dont have any chunks
        coordinatesWithoutChunkList.Clear();


        //Loop through all water chunks that should be active
        for (int zOffset = -chunksVisible; zOffset <= chunksVisible; zOffset++)
        {
            for (int xOffset = -chunksVisible; xOffset <= chunksVisible; xOffset++)
            {
                //This coordinate doesn't take into account the width of the chunk
                Vector3 thisChunkCoord = new Vector3(currentChunkCoordX + xOffset, 0f, currentChunkCoordZ + zOffset);

                //Get the real coordinates of this chunk
                thisChunkCoord.x *= chunkWidth;
                thisChunkCoord.z *= chunkWidth;

                //Figure out if we already have a water chunk at this position
                //If so, we don't need to add a new one, just make the old one visible and change its resolution if needed
                bool hasAlreadyAChunk = false;

                for (int i = 0; i < allWaterChunks.Count; i++)
                {
                    if (allWaterChunks[i].chunkPosition == thisChunkCoord)
                    {
                        //What's the resolution of this chunk?
                        int resolution = GetChunkResolution();

                        allWaterChunks[i].ActivateChunk(resolution);

                        hasAlreadyAChunk = true;

                        break;
                    }
                }

                //This coordinate didn't have a chunk so add one later
                if (!hasAlreadyAChunk)
                {
                    coordinatesWithoutChunkList.Add(thisChunkCoord);
                }
            }
        }


        //Add water chunks to all coordinates without a chunk
        for (int i = 0; i < coordinatesWithoutChunkList.Count; i++)
        {
            //Find a deactivated water chunk
            bool hasFoundDeactivatedChunk = false;
            
            for (int j = 0; j < allWaterChunks.Count; j++)
            {
                if (!allWaterChunks[j].isActive)
                {
                    Vector3 chunkPos = coordinatesWithoutChunkList[i];

                    //What's the resolution of this chunk?
                    int resolution = GetChunkResolution();

                    allWaterChunks[i].ActivateChunk(resolution, chunkPos);

                    hasFoundDeactivatedChunk = true;

                    break;
                }
            }

            //We are out of pooled chunks and need a new chunk
            if (!hasFoundDeactivatedChunk)
            {
                AddNewWaterSquare();

                Vector3 chunkPos = coordinatesWithoutChunkList[i];

                //The newly added water chunk is the last one in the list
                //GameObject chunkObjToActivate = allWaterChunks[allWaterChunks.Count - 1].highDetailedWaterChunk;

                //What's the resolution of this chunk?
                int resolution = GetChunkResolution();

                //Activate the correct chunk
                allWaterChunks[allWaterChunks.Count - 1].ActivateChunk(resolution, chunkPos);
            }
        }
    }



    //Figure out which resolution this chunk should have
    private int GetChunkResolution()
    {
        Vector3 followPos = toFollowObj.transform.position;

        //2d space
        //chunkCoord.y = 0f;
        //followPos.y = 0f;

        //The sqr distance
        //float sqrDist = (followPos - chunkCoord).sqrMagnitude;

        //int chunkRes = 2;

        //if (sqrDist < chunkWidth * chunkWidth)
        //{
        //    chunkRes = 0;
        //}
        //else if (sqrDist < (chunkWidth * 2f) * (chunkWidth * 2f))
        //{
        //    chunkRes = 1;
        //}
        //else
        //{
        //    chunkRes = 2;
        //}

        //The chunk resolution is based on height
        float followHeight = followPos.y;

        int chunkRes = 0;

        if (followHeight < 50f)
        {
            //High resolution
            chunkRes = 0;
        }
        else if (followHeight < 150f)
        {
            chunkRes = 1;
        }
        else
        {
            chunkRes = 2;
        }


        return chunkRes;
    }


    //Help stuff
    public class WaterChunk
    {
        public GameObject highDetailedWaterChunk;
        public GameObject mediumDetailedWaterChunk;
        public GameObject lowDetailedWaterChunk;
        //What's the global position of this chunk?
        public Vector3 chunkPosition;
        //Is any of the chunk's meshes active?
        public bool isActive;


        public WaterChunk()
        {
            //Important to move it away from 0 to make it work
            chunkPosition = new Vector3(50000000f, 0f, 0f);
        }


        //Deactivate all
        public void DeactivateChunk()
        {
            highDetailedWaterChunk.SetActive(false);
            mediumDetailedWaterChunk.SetActive(false);
            lowDetailedWaterChunk.SetActive(false);

            isActive = false;
        }



        //Activate a chunk
        public void ActivateChunk(int resolution)
        {
            GameObject chunkToActivate = GetChunk(resolution);

            chunkToActivate.SetActive(true);

            isActive = true;
        }



        public void ActivateChunk(int resolution, Vector3 chunkPos)
        {
            GameObject chunkToActivate = GetChunk(resolution);

            chunkToActivate.SetActive(true);

            chunkToActivate.transform.position = chunkPos;

            isActive = true;
        }



        //Get the chunk with the correct resolution
        private GameObject GetChunk(int resolution)
        {
            if (resolution == 0)
            {
                return highDetailedWaterChunk;
            }
            else if (resolution == 1)
            {
                return mediumDetailedWaterChunk;
            }
            else
            {
                return lowDetailedWaterChunk;
            }
        }
    }
}
