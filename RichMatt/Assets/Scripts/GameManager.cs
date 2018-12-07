using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    [Header("GameSettings")]
    public bool StartInPlane = false;
    public bool StartSkyDiving = false;
    public GameObject[] players;

    public BRS_PlaneDropManager planeDropManager;

    public GameObject zoneWall;
    //public BRS_ChangeCircle zoneWallChangeCircle;

    [Header("Supply Drop")]
    public GameObject[] supplies;
    public bool QueSupplyDrop = false;

    [Header("SkyDiving")]
    public SkyDiveTesting skyDiveController;
    
    private void Awake()
    {
        VerifyReferences();
    }

    private void DeployPlayersInPlane()
    {
        planeDropManager.InitPlaneDrop(DropTypeENUM.PLAYER, players);


    }

    public void DeploySupplyDrop()
    {
        if (planeDropManager.InitPlaneDrop(DropTypeENUM.SUPPLY, supplies)) ; //do something;

    }

    // Use this for initialization
    void Start () {

        //populate loot
        //determine mission
        //spawn certain enemies and specific locations
        
		
	}
	
	// Update is called once per frame
	void Update () {
        if (StartInPlane)
        {
            StartInPlane = false;//immediately set flag to false
            DeployPlayersInPlane();
        }

        else if (StartSkyDiving)
        {
            StartSkyDiving = false;
            skyDiveController.BeginSkyDive();
            //if you're lower than 100 feet, raise it up to a default value
            if (skyDiveController.transform.position.y < 100)
                skyDiveController.transform.position = new Vector3(skyDiveController.transform.position.x,
                skyDiveController.transform.position.y + 500, skyDiveController.transform.position.z);

        }

        else if (QueSupplyDrop)
        {
            QueSupplyDrop = false;//immediately set flag to false
            DeploySupplyDrop();
        }
    }

    private bool VerifyReferences()
    {
        bool allReferencesOkay = true;
        //verify players
        if (players.Length < 1)//if no players loaded
        {
            players = GameObject.FindGameObjectsWithTag("Player");
            if(players.Length < 1)//if no players tagged
            {

                Debug.LogError("ERROR! No Players found in scene. Tag one or double check BRS TPC.");
                allReferencesOkay = false;
            }
            else
            {
            }
        }
        
        //verify skyDiveController
        if(skyDiveController == null)
        {
            if (players.Length < 1)
            {
                Debug.LogError("ERROR! cannot auto set skyDiveController without any players.");
                allReferencesOkay = false;
            }
            else
            {
                skyDiveController = players[0].GetComponent<SkyDiveTesting>();
            }

        }

        //if(supplies == null)
        //{
        //    Debug.LogError("ERROR! No supplies exist! Why is the plane flying?");
        //    return false;
        //}

        //if (zoneWall == null)
        //{
        //    zoneWall = GameObject.FindGameObjectWithTag("ZoneWall");
        //    zoneWallChangeCircle = zoneWall.GetComponentInChildren<BRS_ChangeCircle>();
        //}
        //else
        //{
        //    if (zoneWallChangeCircle == null)
        //    {
        //        zoneWallChangeCircle = zoneWall.GetComponentInChildren<BRS_ChangeCircle>();
        //    }
        //}

        return allReferencesOkay;

    }
}
