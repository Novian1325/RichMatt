using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupplyDropManager : MonoBehaviour
{

    [Header("Supply Drop")]
    [SerializeField] private GameObject supplyDropPrefab;
    [SerializeField] private bool queSupplyDrop = false;

    [SerializeField] private BRS_PlanePathManager planePathManager;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (queSupplyDrop)
        {
            queSupplyDrop = false;//immediately set flag to false
            DeploySupplyDrop();
        }
    

    }

    
    public void DeploySupplyDrop()
    {
        planePathManager.InitPlaneDrop(DropTypeENUM.SUPPLY, supplyDropPrefab); //can catch plane manager and track it

    }
}
