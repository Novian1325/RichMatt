using UnityEngine;

public class RLS_Foundation : MonoBehaviour
{
    [Header("Spawn Weight (percent)")]

    [Tooltip("Minimum number of loot spots to spawn. +=0 ")]
    [SerializeField] private int MinSpawnPoints = 1;

    [Tooltip("Max number of loot spots to spawn. -= total number of spots.")]
    [SerializeField] private int MaxSpawnPoints = 99;

    [Tooltip("List of all LootSpots to be controlled by this Foundation.")]
    [SerializeField] private RLS_LootSpot[] LootSpots;

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

    void OnEnable()
    {

    }

    /// <summary>
    /// Makes sure Min and Max spawn values are legitimate.
    /// </summary>
    private void ClampLimits()
    {
        //max cannot be more than total number of loot spots
        MaxSpawnPoints = Mathf.Clamp(MaxSpawnPoints, 0, LootSpots.Length);
        //min must be between 0 and max
        MinSpawnPoints = Mathf.Clamp(MinSpawnPoints, 0, MaxSpawnPoints);

        if (DEBUG)
        {
            Debug.LogFormat("Foundation #{0}: Min: {1} / Max: {2} Spawns", foundationID, MinSpawnPoints, MaxSpawnPoints);
        }
    }

    /// <summary>
    /// Tell given LootSpots they have been chosen.
    /// </summary>
    /// <param name="selectedLootSpots"></param>
    private static void NotifyLootSpots(RLS_LootSpot[] selectedLootSpots)
    {
          foreach (var spot in selectedLootSpots)
          {
                spot.SelectVariant();
          }
    }

    public void SelectLootSpots()
    {
        //select how many loot spots will be spawned
        var numberOfSpawns = Random.Range(MinSpawnPoints, MaxSpawnPoints);

        if (DEBUG)
        {
            Debug.LogFormat("Foundation #{0}: Will spawn {1} Loot", foundationID, numberOfSpawns);
        }

        //get a collection of those Loot Spots
        var selectedSpawns = GetListOfLootSpawns(numberOfSpawns);  //temporarily the 'weight' is static
        //sort out which loot spots are hidden, and which get items spawned in them
        NotifyLootSpots(selectedSpawns);

        //Delete unused spawn points
    }

    // later add an optional 'weight'
    public RLS_LootSpot[] GetListOfLootSpawns(int numberOfLootSpawns)
    {
          //create a new random number generator
          var randomNumberGenerator = new System.Random();
          //create an empty list to hold the selected loot spots
          var lootSpotArray = new RLS_LootSpot[numberOfLootSpawns];
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
                lootSpotArray[n] = LootSpots[randomIndex];
          }
          
          return lootSpotArray;
    }

}