﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupplyDropManager : MonoBehaviour
{
    [Header("Supply Drop")]

    [Tooltip("Prefab of the Supply Drop. Should have a SupplyDrop component attached.")]
    [SerializeField] private GameObject supplyDropPrefab;
    
    [SerializeField] private BRS_PlanePathManager planePathManager;

    [Tooltip("This many seconds will pass before the first supply drop is sent.")]
    [SerializeField] private int initialDelayInSeconds = 60;

    [Header("Spawn Settings")]
    [Tooltip("Developer can use this to immediately cause a Supply Drop to occur.")]
    [SerializeField] private bool queSupplyDrop = false;

    [Tooltip("Should Supply Drops be Deployed on a random timeline? If false, relies on outside code.")]
    [SerializeField] private bool randomTimedDrops = true;

    [Tooltip("Wait at least this long before queing next Supply Drop")]
    [SerializeField] private int minSpawnTime = 30;

    [Tooltip("Max amount of time between Supply Drops")]
    [SerializeField] private int maxSpawnTime = 60;

    private float nextSupplyDropSpawnTime;//at what time will the next supply drop happen?

    //only one supplydropmanager should exist
    public static SupplyDropManager supplyDropManagerInstance;

    //keep track of the amount of supply drops in the scene;
    private List<SupplyDrop> supplyDropList = new List<SupplyDrop>();

    private static readonly int DestroySupplyDropDistance = 1000;

    // Use this for initialization
    void Start () {
        InitSingletonPattern();

        nextSupplyDropSpawnTime = Time.time + initialDelayInSeconds;
	}

    private void InitSingletonPattern()
    {
        //this pattern enforces only one of these things ever exists
        if (!supplyDropManagerInstance)
        {
            supplyDropManagerInstance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // Update is called once per frame
    void Update () {

        if(Time.time > nextSupplyDropSpawnTime)
        {
            DeploySupplyDrop();
            nextSupplyDropSpawnTime += Random.Range(minSpawnTime, maxSpawnTime);
        }
        
        else if (queSupplyDrop)
        {
            queSupplyDrop = false;//immediately set flag to false
            DeploySupplyDrop();
        }
    

    }

    public void AddSupplyDrop(SupplyDrop newSupplyDrop)
    {
        supplyDropList.Add(newSupplyDrop);
    }

    public void RemoveSupplyDrop(SupplyDrop supplyDrop)
    {
        supplyDropList.Remove(supplyDrop);
    }

    public void DeploySupplyDrop()
    {
        planePathManager.InitPlaneDrop(DropTypeENUM.SUPPLY, supplyDropPrefab); //can catch plane manager and track it

    }

    private void DestroySupplyDropsIfOutsidePlayArea()
    {
        //use this to destroy supply drops that are too far away from players

    }

}