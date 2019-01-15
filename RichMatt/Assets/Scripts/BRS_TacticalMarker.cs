using UnityEngine;
using UnityEngine.UI;

public class BRS_TacticalMarker : MonoBehaviour
{
    //tactical marker stuff
    [Tooltip("The prefab of the UI element that will be shown when activated.")]
	[SerializeField] private GameObject TacticalMarkerPrefab;
    [SerializeField] private Color playerColor; //player's color to match marker
    [SerializeField] private Text distanceText;

    private Transform playerCameraXform; // transform of player's camera
    private GameObject tacticalMarkerInstance;//the marker object that is currently existing in the world

    private static readonly string TMBUTTONNAME = "TacticalMarker";//name of the button in Input Manager that will trigger tactical marker placement
    private static readonly int tacticalMarkerPlaceDistanceLimit = 300;

    //distance polling stuff
    private int distanceToMarker;//how close is the player to the marker they placed?
    private float distancePollingTimer = 0f; //used to keep track of time and limit distance polling rate
    private static readonly int distanceToMarkerPollsPerSecond = 2;// this affects perfomance. How often should the distance between the player and marker be checked?

    private Transform minimapCameraXform;
    private static readonly int iconHeightOffset = 1000;

    //TODO
    //hold 't' for 3 seconds to remove marker from map

    void Start ()
	{
        if (TacticalMarkerPrefab == null) Debug.LogError("ERROR! No Tactical Marker Prefab set!");
		playerCameraXform = GameObject.FindGameObjectWithTag("MainCamera").transform;//get the player's camera
        minimapCameraXform = GameObject.FindGameObjectWithTag("MiniMap Camera").transform;

    }

    private void DestroyExistingTacticalMarkerAtDistanceLimit()
    {
        //if distance between player and marker > distance limit
        if (distanceToMarker > tacticalMarkerPlaceDistanceLimit)
        {
            Debug.Log("Distance: " + Vector3.Distance(this.transform.position, tacticalMarkerInstance.transform.position) + ". Destroying TacticalMarker.");
            if(tacticalMarkerInstance) Destroy(tacticalMarkerInstance);//if it exists, destroy it
        }
    }

    // Update is called once per frame
    void Update ()
	{
        //Handle Tactical Marker
        if (Input.GetButtonDown (TMBUTTONNAME))
		{
            PlaceTacticalMarker();
		}

        if (tacticalMarkerInstance)
        {
            UpdateDistanceToMarker();

            DestroyExistingTacticalMarkerAtDistanceLimit();
        }

        
    }

    private void UpdateDistanceText()
    {
        if (distanceText)
        {
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
            stringBuilder.Append(distanceToMarker.ToString());
            stringBuilder.Append("m");
            distanceText.text =  stringBuilder.ToString();
        }

    }

    private void UpdateDistanceToMarker()
    {
        //limit polling rate to be more performant
        if (Time.time >= distancePollingTimer) //if it's time to poll again...
        {
            //Calculate the distance from the player to the marker
            distanceToMarker = (int)Vector3.Distance(playerCameraXform.position, tacticalMarkerInstance.transform.position);

            //display this distance on the HUD
            UpdateDistanceText();

            //reset next update time
            distancePollingTimer = Time.time + (1 / distanceToMarkerPollsPerSecond);
        }
    }
    
	private void PlaceTacticalMarker()
	{
        //TODO 
        //react to item being tagged (eg the whole building gets tagged, an enemy gets a dot over its head, the player speaks aloud different quips, etc)
        RaycastHit hitInfo;
        // Are we pointing at something in the world?
        
        if (Physics.Raycast(playerCameraXform.position, playerCameraXform.forward, out hitInfo, tacticalMarkerPlaceDistanceLimit))
		{
            if (tacticalMarkerInstance)//if an TM already exists
            {
                Destroy(tacticalMarkerInstance);//destroy it and replace it
            }

            //this will cause the UI to update right away
            distancePollingTimer = (1 / distanceToMarkerPollsPerSecond);

            tacticalMarkerInstance = Instantiate(TacticalMarkerPrefab, hitInfo.point, Quaternion.identity);//create a new marker in the world
            //make it a child of the hit object so if it moves, the marker moves with it. assigning childhood after instantiation preserves native scale
            tacticalMarkerInstance.transform.SetParent(hitInfo.collider.gameObject.transform);
            //TODO change the color of this marker to match the player's color for identification purposes
            
            //raise children icons above everything else in scene
            foreach(Transform child in tacticalMarkerInstance.transform)
            {
                child.Translate(0, minimapCameraXform.position.y - iconHeightOffset, 0);
                child.GetComponent<MeshRenderer>().material.color = playerColor;
            }

		}
	}
    

    
}
