using UnityEngine;
using System.Collections.Generic;

public class BRS_PlanePathManager : MonoBehaviour

{
    //public GameObject EndPointBall;
    [Header("Map Settings")]
    [Tooltip("This is the spawn boundary for the airplane. Airplane will spawn at the very edge of this cylinder/sphere")]
    [SerializeField] private Transform planeSpawnBounds;//where can the plane start and stop?
    [Tooltip("This is the list of volumes where the players are allowed to jump from the plane.")]
    [SerializeField] private GameObject[] playerDropZones;//how big should the zone be that the plane flies through
    [Tooltip("These are the volumes for supply drops. Supplies drop as soon as plane enters this volume.")]
    [SerializeField] private GameObject[] supplyDropZones;//list containing all areas the player can drop into

    [Header("Plane Settings")]
    [Tooltip("This is the plane prefab. Must include a PlaneManager script.")]
    [SerializeField] private GameObject BRS_PlaneSpawn;//plane object (model) to spawn

    [Tooltip("The plane's airspeed when carrying Players")]
    [SerializeField] private int planeSpeed_PlayerDrop = 150;

    [Tooltip("The plane's airspeed when on a supply drop sortie")]
    [SerializeField] private int planeSpeed_SupplyDrop = 300;

    [Tooltip("Enables Debug.Log statements and persistence of objects for debugging purposes.")]
    [SerializeField] private bool DEBUG = true;//if true, prints debug statements
    
    private GameObject[] acceptableDropZones;
    private List<GameObject> endpointMarkerList = new List<GameObject>();

    //how high does the plane fly?
    private float planeFlightAltitude = 800.0f;
    private float startingFlightAltitude;
    private bool planeContainsPlayers = false;

    private readonly int failedPathAltitudeIncrementAmount = 25;//if the flight path fails, raise the altitude by this much before trying again

    //radius of spawn zone
    private float spawnBoundsCircleRadius = 100.0f;
    private readonly int minimumDropZoneHeight = 1000;//drop zones should be really tall so they can be tested against
    
    //start and end points for plane to fly through
    private Vector3 planeStartPoint;
    private Vector3 planeEndPoint;
    private GameObject endpointMarker;

    private int unsuccessfulPasses = 0;
    private readonly int flightPathChecksUntilFailure = 15;

    //stuff to pass on to plane when deployed
    private GameObject targetDropZone;
    private List<GameObject> cargo_Players = new List<GameObject>();
    private GameObject cargo_Supplies;//can only carry one supply drop per sortie
    private int planeFlightSpeed = 200;

    private bool VerifyReferences()
    {
        if (planeSpawnBounds == null)
        {
            Debug.LogError("ERROR: plane spanw bounds not set!");
            Debug.Break();
            return false;
        }

        if (playerDropZones.Length < 1)
        {
            Debug.LogError("ERROR: No Player Drop Zones in list!");
            Debug.Break();
            return false;
        }

        if (supplyDropZones.Length < 1)
        {
            Debug.LogError("ERROR: No Supply Drop Zones in list!");
            Debug.Break();
            return false;
        }
        
        return true;
    }

    private void ConfigureDropZones()
    {
        //MAKE SURE Y SCALE IS LARGE ENOUGH!
        foreach(GameObject zone in playerDropZones)
        {
            if(zone.transform.localScale.y < minimumDropZoneHeight)
                zone.transform.localScale = new Vector3(zone.transform.localScale.x, minimumDropZoneHeight, zone.transform.localScale.z);
        }

        foreach (GameObject zone in supplyDropZones)
        {
            if (zone.transform.localScale.y < minimumDropZoneHeight)
                zone.transform.position = new Vector3(zone.transform.position.x, minimumDropZoneHeight / 2, zone.transform.position.z);//move up slightly
            zone.transform.localScale = new Vector3(zone.transform.localScale.x, minimumDropZoneHeight, zone.transform.localScale.z);//increase y scale
        }


    }

    private void ConfigureFlightType()
    {
        if (planeContainsPlayers)
        {
            acceptableDropZones = playerDropZones;
            planeFlightSpeed = planeSpeed_PlayerDrop;
        }
        else
        {
            acceptableDropZones = supplyDropZones;
            planeFlightSpeed = planeSpeed_SupplyDrop;

        }
        
    }

    private void DestroyMarkerObjects()
    {
        //destorys each marker that was used.
        foreach (GameObject marker in endpointMarkerList)
        {
            if(!DEBUG) Destroy(marker);
        }
        endpointMarkerList.Clear();//clear list so it doesn't balloon infinitely
    }

    void Start()
    {
        //set and check altitude
        planeFlightAltitude = planeSpawnBounds.position.y > 0 ? planeSpawnBounds.position.y : 200f;//verifies that altitude is above 0
        startingFlightAltitude = planeFlightAltitude;//set starting value

        //set radius of spawnBoundsCircleRadius
        //leave at default value if local scale is too small
        spawnBoundsCircleRadius = planeSpawnBounds.localScale.x / 2 > spawnBoundsCircleRadius ? planeSpawnBounds.localScale.x / 2 : spawnBoundsCircleRadius;

        //error checking
        if (VerifyReferences())
        {
            if (DEBUG) Debug.Log("Everything looks good here.");
        }
        else
        {
            Debug.Log("Failed to set up references.");
        }

        ConfigureDropZones();//make sure drop zones are proper size
    }

    private Vector3 GetRandomPointOnCircle()
    {
        //get terminal point on unit circle, then multiply by radius
        float randomArc = Random.Range(0, 2 * Mathf.PI);
        Vector3 randomPoint = new Vector3(//create new vector3
            (Mathf.Sin(randomArc) * spawnBoundsCircleRadius) + planeSpawnBounds.position.x, //get x coordiantes on unit circle, multiply by radius, offset relative to bounds
            planeFlightAltitude, // set the height
            (Mathf.Cos(randomArc) * spawnBoundsCircleRadius) + planeSpawnBounds.position.z);//get y coordinate on unity circle, multiply by radius, offset relative to bounds
        //Debug.Log("random point: " + randomPoint);
        return randomPoint;

    }

    private void LoadCargo(GameObject cargo)
    {
        
        if (cargo.CompareTag("Player"))
        {
            cargo_Players.Add(cargo);
            planeContainsPlayers = true;
        }
        else
        {
            cargo_Supplies = cargo;//set supplies
        }


    }

    public bool InitPlaneDrop()
    {
        ConfigureFlightType();
        if (SetupFlightPath())
        {
            SpawnPlane();//catch the plane Manager to keep track of the plane further
            return true;
        }

        return false;
    }

    public bool InitPlaneDrop(GameObject cargo)
    {
        LoadCargo(cargo);

        return InitPlaneDrop();
    }

    public bool InitPlaneDrop(GameObject[] incomingCargo)
    {
        foreach (GameObject cargo in incomingCargo)
        {
            LoadCargo(cargo);
        }
        
        return InitPlaneDrop();
    }

    private GameObject ConfigureEndpoint(Vector3 targetPosition)
    {
        GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        marker.transform.position = targetPosition;
        endpointMarkerList.Add(marker);//add to list to delete later
        if (DEBUG)
        {
            marker.transform.SetParent(this.transform);//set parent to locate easier in hierarchy
        }
        else
        {
            marker.GetComponent<MeshRenderer>().enabled = false;//show marker if debugging; hide if not
        }
        return marker;

    }

    private bool SetupFlightPath()
    {
        bool endpointHit = false;
        bool flightPathThroughLZ = false;

        //find a start point
        planeStartPoint = GetRandomPointOnCircle();
        //spawn debugger object. this object is the parent, so both will be destroyed
        if (DEBUG)
        {
            GameObject startMark = ConfigureEndpoint(planeStartPoint);
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
            stringBuilder.Append("StartMarker: ");
            stringBuilder.Append(unsuccessfulPasses);
            startMark.name = stringBuilder.ToString();
            startMark.GetComponent<MeshRenderer>().enabled = true;//makes marker visible for debugging purposes

        }

        //look for an endpoint
        for (int endPointsFound = 1; endPointsFound <= flightPathChecksUntilFailure; ++endPointsFound)//while you don't have a valid flight path...
        {
            //Debug.Log("Attempt No: " + endPointsFound);
            //get end point on circle
            planeEndPoint = GetRandomPointOnCircle();
            //create a new endpoint marker at that location
            endpointMarker = ConfigureEndpoint(planeEndPoint);
            if (DEBUG)
            {
                //name endpointMarker for debugging purposes
                System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
                stringBuilder.Append("Endpoint Marker ");
                stringBuilder.Append(unsuccessfulPasses);
                stringBuilder.Append(".");
                stringBuilder.Append(endPointsFound);
                endpointMarker.name = stringBuilder.ToString();
            }

            //test if flight path goes through LZ
            if (TestRaycastThroughDropZone(planeStartPoint, endpointMarker.transform.position, acceptableDropZones))
            {
                flightPathThroughLZ = true;
            }
            else
            {
                if (DEBUG) Debug.Log("INVALID: Flight path not through LZ.");
            }

            //test if flight path is clear all the way to endpoint
            if (TestRaycastHitEndPoint(planeStartPoint, endpointMarker))
            {
                endpointHit = true;
            }
            else
            {
                if (DEBUG) Debug.Log("INVALID: Endpoint Marker Not Hit.");
            }

            //is the flight unobstructed and through a drop zone
            if(endpointHit && flightPathThroughLZ)
            {
                //SUCCESS!!!!!!! reset for next path
                ToggleDropZones(true);//turn LZ on
                planeFlightAltitude = startingFlightAltitude;//reset altitude for next try
                unsuccessfulPasses = 0;//reset failures
                DestroyMarkerObjects();//clear excess markers
                planeContainsPlayers = false;//prove me right
                return true;
            }
            else
            {
                if (DEBUG)
                {
                    if (!endpointHit) Debug.Log("Flight path failed because ENDPOINT NOT HIT.");
                    if (!flightPathThroughLZ) Debug.Log("Flight path failed because NOT THROUGH LZ.");
                    Debug.Log(".................New test.......................");
                }
                endpointHit = false;
                flightPathThroughLZ = false;
            }

            
        }//end for
        
        //this altitude is not working. keep raising
        if (++unsuccessfulPasses > flightPathChecksUntilFailure)//we've been here before
        {
            Debug.LogWarning("ERROR! Flight path failed after " + unsuccessfulPasses * flightPathChecksUntilFailure + " attempts. Adjust planeSpawnBounds. Skipping Plane Deployment");
            unsuccessfulPasses = 0;//reset tracker
            DestroyMarkerObjects();
            return false;
        }
        //raise altitude and try again
        planeFlightAltitude += failedPathAltitudeIncrementAmount;
        //destroy markers
        DestroyMarkerObjects();
        //try again
        return SetupFlightPath();
        
        
    }//end func

    public PlaneManager SpawnPlane()
    {
        //create this plane in the world at this position, with no rotation
        GameObject plane = Instantiate(BRS_PlaneSpawn, planeStartPoint, Quaternion.identity);//do not set plane to be child of this object!
        plane.transform.LookAt(planeEndPoint);//point plane towards endpoint
        //get plane manager
        PlaneManager planeManager = plane.GetComponent<PlaneManager>();
        if(planeManager == null)
        {
            plane.AddComponent<PlaneManager>();//create it if it doesn't exist
        }
        planeManager.InitPlane(targetDropZone, cargo_Players.ToArray(), cargo_Supplies, planeFlightSpeed);
        cargo_Players.Clear();
        return planeManager;

    }

    private bool TestRaycastThroughDropZone(Vector3 startPoint, Vector3 targetObject, GameObject[] acceptableDropZones)
    {

        //turn drop zone colliders on
        ToggleDropZones(true);//turn LZ on
        //did the raycast go through a drop zone?
        bool raycastThroughDropZone = false;
        //RaycastHit will store information about anything hit by the raycast
        RaycastHit raycastHitInfo;
        //raycast
        if (Physics.Raycast(startPoint, targetObject - startPoint, out raycastHitInfo, spawnBoundsCircleRadius * 2))
        {
            if (DEBUG) Debug.Log("Testing Raycast Through DropZone. Hit: " + raycastHitInfo.collider.gameObject.name);
            for (int i = 0; i < acceptableDropZones.Length; ++i)//look through each drop zone in list
            {
                if (raycastHitInfo.collider.gameObject == acceptableDropZones[i])//if the game object that was hit is inside this list of good zones
                {
                    targetDropZone = acceptableDropZones[i];//this zone will be passed to the plane, so it knows when it hits said zone
                    raycastThroughDropZone = true;//booyah!
                    break;//break out of for loop looking through gameObjects in list
                }//end if
            }//end for

        }//end if
        else
        {
            //Debug.LogError("ERROR! Raycast missed target LZ");
        }

        return raycastThroughDropZone;
    }

    private bool TestRaycastHitEndPoint(Vector3 startPoint, GameObject targetObject)
    {
        //turn all drop zones off so they don't interfere with raycast
        ToggleDropZones(false);//turn LZ off

        //did the raycast hit the endpoint unobstructed by terrain or obstacles?
        bool raycastHitEndpoint = false;
        //RaycastHit holds info about raycast
        RaycastHit raycastHitInfo;
        //if something was hit...
        if (Physics.Raycast(startPoint, targetObject.transform.position - startPoint, out raycastHitInfo, spawnBoundsCircleRadius * 2))
        {
            if (DEBUG) Debug.Log("Testing Raycast against Endpoint. Hit: " + raycastHitInfo.collider.gameObject.name);
            if (raycastHitInfo.collider.gameObject == targetObject)//were we trying to hit this thing?
            {
                raycastHitEndpoint = true;//booyah!
            }
        }
        else
        {
            //Debug.LogError("ERROR! Raycast missed it's target: " + targetObject);
        }
        return raycastHitEndpoint;
    }

    //toggle drop zones on and off
    public void ToggleDropZones(bool active)
    {
        ToggleDropZones(playerDropZones, active);
        ToggleDropZones(supplyDropZones, active);
    }

    public void ToggleDropZones(GameObject[] acceptableDropZones, bool active)
    {
        //look at each dropZone in our list
        foreach (GameObject dropZone in acceptableDropZones)
        {
            //set it active or inactive
            dropZone.GetComponent<CapsuleCollider>().enabled = active;
        }
       
    }
}

