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
        planeDropManager.InitPlaneDrop(DropTypeENUM.SUPPLY, supplies);

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

        if (QueSupplyDrop)
        {
            QueSupplyDrop = false;//immediately set flag to false
            DeploySupplyDrop();
        }
    }

    private bool VerifyReferences()
    {
        bool allReferencesOkay = true;
        if (players.Length < 1)//if no players loaded
        {
            players = GameObject.FindGameObjectsWithTag("Player");
            if(players.Length < 1)//if no players tagged
            {
                players = new GameObject[1];//limit game to single player
                players[0] = FindObjectOfType<BRS_TPCharacter>().gameObject;//find the first BRS TPC in game and assume it
            }
            else
            {
                Debug.LogError("ERROR! No Players found in scene. Tag one or double check BRS TPC.");
                allReferencesOkay = false;
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
