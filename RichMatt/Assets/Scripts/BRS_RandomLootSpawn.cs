using System.Collections.Generic;
using UnityEngine;

public class BRS_RandomLootSpawn : MonoBehaviour
{
    [Header("---Spawn Range---")]

    [Tooltip("Minimum number of loot spots to spawn. +=0 ")]
    [SerializeField] private int MinSpawnPoints = 1;

    [Tooltip("Max number of loot spots to spawn. -= total number of spots.")]
    [SerializeField] private int MaxSpawnPoints = 99;

    [Tooltip("List of all LootSpots to be controlled by this Foundation.")]
    [SerializeField] private BRS_ItemManager[] lootSpawnList;

    [Space]
    [Tooltip("Set to true to see details in the Console.")]
    [SerializeField] private bool DEBUG = false;

    //track how many foundations exist
    private static int foundationCounter = 0;
    //which foundation # is this one?
    private int foundationID;

    // Use this for initialization
    void Start ()
    {
        foundationID = foundationCounter++;

        //verify input
        ClampLimits();

        SelectLootSpots();
    }

    /// <summary>
    /// Makes sure Min and Max spawn values are legitimate.
    /// </summary>
    private void ClampLimits()
    {
        //max cannot be more than total number of loot spots
        MaxSpawnPoints = Mathf.Clamp(MaxSpawnPoints, 0, lootSpawnList.Length);
        //min must be between 0 and max
        MinSpawnPoints = Mathf.Clamp(MinSpawnPoints, 0, MaxSpawnPoints);

        if (DEBUG)
        {
            Debug.LogFormat("Foundation #{0}: Min: {1} / Max: {2} Spawns", foundationID, MinSpawnPoints, MaxSpawnPoints);
        }
    }

    /// <summary>
    /// Randomly removes a number of elements from array, leaving only the selected spawns.
    /// </summary>
    private void SelectLootSpots()
    {
        //select how many loot spots will be spawned
        var numberOfSpawns = Random.Range(MinSpawnPoints, MaxSpawnPoints);

        if (DEBUG)
        {
            Debug.LogFormat("Foundation #{0}: Will spawn {1} Loot", foundationID, numberOfSpawns);
        }

        //get a collection of indices of the loot spots to keep
        var selectedSpawnIndices = SelectSpawnIndices(numberOfSpawns);  //temporarily the 'weight' is static

        //make a new list to hold selected 
        var spawnsToKeep = new List<BRS_ItemManager>();
        for(var i = 0; i < numberOfSpawns; ++i)
        {
            spawnsToKeep.Add(lootSpawnList[selectedSpawnIndices[i]]);
        }

        //destroy unused
        foreach(var v in lootSpawnList)
        {
            //if this element was not chosen
            if (!spawnsToKeep.Contains(v))
            {
                Destroy(v.gameObject);//EXTERMINATE!
            }
        }

        //replace butchered list
        lootSpawnList = spawnsToKeep.ToArray();
    }

    /// <summary>
    /// Randomly selects which indices will be used for random loot.
    /// </summary>
    /// <param name="numberOfLootSpawns"></param>
    /// <returns></returns>
    private static int[] SelectSpawnIndices(int numberOfLootSpawns)
    {
        //create a new random number generator
        var randomNumberGenerator = new System.Random();
        //create an empty list to hold the selected loot spots
        var usedIndicesArray = new int[numberOfLootSpawns];

        for (var n = 0; n < numberOfLootSpawns; ++n)
        {
            //get a new random number
            var randomIndex = randomNumberGenerator.Next(numberOfLootSpawns);
            //  make sure we do not duplicate items
            while (System.Array.Exists(usedIndicesArray, element => element.Equals(randomIndex))) //if this index is already in the list of used indices
            {
                //get a new random number
                randomIndex = randomNumberGenerator.Next(numberOfLootSpawns);
            }
            usedIndicesArray[n] = randomIndex;
        }

        return usedIndicesArray;
    }
}