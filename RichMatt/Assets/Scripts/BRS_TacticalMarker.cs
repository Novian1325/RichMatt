using UnityEngine;
using UnityEngine.UI;

public class BRS_TacticalMarker : MonoBehaviour
{
    //tactical marker stuff
    [Tooltip("The prefab of the UI element that will be shown when activated.")]
	[SerializeField] private GameObject TacticalMarkerPrefab;
    private GameObject tacticalMarkerInstance;//the marker object that is currently existing in the world
    //[SerializeField] private Color playerColor; //player's color to match marker
    [SerializeField] private Text distanceText;

    private Transform playerCameraXform; // transform of player's camera
    
    private readonly string TMBUTTONNAME = "TacticalMarker";//name of the button in Input Manager that will trigger tactical marker placement
    private readonly int tacticalMarkerPlaceDistanceLimit = 300;

    //distance polling stuff
    private int distanceToMarker;//how close is the player to the marker they placed?
    private float distancePollingTimer = 0f; //used to keep track of time and limit distance polling rate
    private readonly int distanceToMarkerPollsPerSecond = 2;// this affects perfomance. How often should the distance between the player and marker be checked?

    //TODO
    //hold 't' for 3 seconds to remove marker from map
    //destroy marker if distance is too great

    //what is the maximum distance away a player can set a tactical marker

    void Start ()
	{
        if (TacticalMarkerPrefab == null) Debug.LogError("ERROR! No Tactical Marker Prefab set!");
		playerCameraXform = Camera.main.transform;//get the player's camera
	}

    private void DestroyExistingTacticalMarkerAtDistanceLimit()
    {
        //if distance between player and marker > distance limit
        if (distanceToMarker > tacticalMarkerPlaceDistanceLimit)
        {
            Debug.Log("Distance: " + Vector3.Distance(this.transform.position, tacticalMarkerInstance.transform.position) + ". Destroying TacticalMarker.");
            if(tacticalMarkerInstance) Destroy(tacticalMarkerInstance);//if it exists, destroy it
            distancePollingTimer = 0;
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
        if (distanceText) distanceText.text = distanceToMarker.ToString() + "m";
    }

    private void UpdateDistanceToMarker()
    {
        //limit polling rate to be more performant
        if (distancePollingTimer >= (1 / distanceToMarkerPollsPerSecond)) //if it's time to poll again...
        {
            //Calculate the distance from the player to the marker
            distanceToMarker = (int)Vector3.Distance(this.transform.position, tacticalMarkerInstance.transform.position);

            //display this distance on the HUD
            UpdateDistanceText();

            //reset timer
            distancePollingTimer = 0;
        }
        else
        {
            //increase the timer
            distancePollingTimer += Time.deltaTime;
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
            
		}
	}
    

    
}
