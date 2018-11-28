using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    [Header("GameSettings")]
    public bool StartInPlane;
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
        if (players.Length < 1)
        {
            players = GameObject.FindGameObjectsWithTag("Player");
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

        return true;

    }
}
