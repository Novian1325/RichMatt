using System.Collections.Generic;
using UnityEngine;

namespace PolygonPilgrimage.BattleRoyaleKit
{
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

        //IS THIS CLASS NEEDED AFTER IT PERFORMS ITS WORK? OR CAN IT BE DESTROYED AFTERWARDS?

        // Use this for initialization
        void Start()
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
                Debug.LogFormat("Foundation #{0}: Inputs Clamped: Min: {1} / Max: {2}.", foundationID, MinSpawnPoints, MaxSpawnPoints);
            }
        }

        /// <summary>
        /// Randomly removes a number of elements from array, leaving only the selected spawns.
        /// </summary>
        private void SelectLootSpots()
        {
            //select how many loot spots will be spawned
            var numberOfSpawns = Random.Range(MinSpawnPoints, MaxSpawnPoints);

            //get a collection of indices of the loot spots to keep
            var selectedSpawnIndices = SelectSpawnIndices(numberOfSpawns, lootSpawnList.Length);  //temporarily the 'weight' is static

            //make a new list to hold selected 
            var spawnsToKeep = new List<BRS_ItemManager>();

            for (var i = 0; i < numberOfSpawns; ++i)
            {
                spawnsToKeep.Add(lootSpawnList[selectedSpawnIndices[i]]);
            }

            //destroy unused
            foreach (var v in lootSpawnList)
            {
                //if this element was not chosen
                if (!spawnsToKeep.Contains(v))
                {
                    Destroy(v.gameObject);//EXTERMINATE!
                }
            }

            //replace butchered list
            lootSpawnList = spawnsToKeep.ToArray();

            //debug to Console
            if (DEBUG)
            {
                Debug.LogFormat("Foundation #{0}: Will spawn {1} Loot", foundationID, numberOfSpawns);
            }
        }

        /// <summary>
        /// Randomly selects which indices will be used for random loot.
        /// </summary>
        /// <param name="numberOfLootSpawns"></param>
        /// <returns></returns>
        private static int[] SelectSpawnIndices(int numberOfLootSpawns, int totalSpawns)
        {
            //create an empty list to hold the selected loot spots
            var usedIndicesArray = new List<int>();

            //create a list of integers from 0 to number of selected spawns
            for (var i = 0; i < totalSpawns; ++i)
            {
                usedIndicesArray.Add(i);
            }

            //randomly remove a number of these indices
            for (var i = 0; i < totalSpawns - numberOfLootSpawns; ++i)
            {
                //get a new random number
                var randomIndex = Random.Range(0, usedIndicesArray.Count);
                usedIndicesArray.RemoveAt(randomIndex);
            }

            return usedIndicesArray.ToArray();
        }
    }

}
