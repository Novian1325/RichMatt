using UnityEngine;

public class PlaneManager : MonoBehaviour
{
	public int airspeed = 100;
    public GameObject[] cargo;
    public bool hasDroppedCargo = false;
    public GameObject planeCameraHolder;

    //for debugging statements
    public bool DEBUG = false;
    //to which drop zone is the plane headed?
    private GameObject targetDropZone;

    private static int planeCounter = 0;


    //public UnityStandardAssets.Characters.FirstPerson.FirstPersonController fpsController;

    private bool CheckIfPlayerOnBoard()
    {
        //returns true if there is at least one player in cargo
        bool playerIsOnBoard = false;
        foreach (GameObject burden in cargo)
        {
            if (burden.gameObject.CompareTag("Player"))
            {
                //Debug.Log("Player On Board");
                playerIsOnBoard = true;
            }
        }
        return playerIsOnBoard;
    }

    //this is basically the constructor class
    public void InitPlane(GameObject incomingTargetDropZone, GameObject[] incomingCargo, int incomingAirSpeed = 100)
    {
        if (DEBUG) Debug.Log("This plane heading towards DZ: " + incomingTargetDropZone);
        this.cargo = incomingCargo;
        this.targetDropZone = incomingTargetDropZone;
        this.airspeed = incomingAirSpeed;
        planeCameraHolder.SetActive(CheckIfPlayerOnBoard());
        //gives this plane a name for tracking purposes
        this.gameObject.name = "Plane Number: " + ++planeCounter;

    }
    // Use this for initialization
    void Start ()
	{
        //play sound
        //randomly select plane model
        if(this.planeCameraHolder == null)
        {
            Debug.LogError("ERROR! planeCam not set!");
        }
		
	}
	
	// Update is called once per frame
	void Update ()
	{
        //move the plane forward using translation
		transform.position += transform.forward * Time.deltaTime * airspeed;
	}

    private void OnTriggerEnter(Collider otherCollider)
    {
        if(otherCollider.gameObject == targetDropZone)
        {
            if (DEBUG) Debug.Log("Now Entering Target Drop Zone: " + otherCollider.name);
            //drop supplies
            //signify to Players they can drop
        }
    }

    private void OnTriggerExit(Collider otherCollider)
    {
        //if leaving target dropzone
        if (otherCollider.gameObject == targetDropZone)
        {
            if (DEBUG) Debug.Log("Now Leaving Target Drop Zone: " + otherCollider.name);
            //Force All Players out (if there are any)
        }

        //Destroy when leaving boundary
        else if (otherCollider.gameObject.CompareTag("MapBounds"))
        {
            if (DEBUG)
            {
                Debug.Log("Plane " + this.name + " leaving boundary. Destroying....");
                this.gameObject.SetActive(false);
            }
            else Destroy(this.gameObject);
        }

        


    }
}
