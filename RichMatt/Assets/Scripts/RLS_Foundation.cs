using UnityEngine;

public class RLS_Foundation : MonoBehaviour
{
    [Header("Spawn Weight (percent)")]
    [Range(0, 100)]
    [SerializeField] private int MinSpawnPoints = 1;
    [Range(1, 100)]
    [SerializeField] private int MaxSpawnPoints = 99;

    [SerializeField] private GameObject[] LootSpots;

    // Use this for initialization
    void Start ()
    {
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
    }

    private static void NotifyLootSpots(GameObject[] selectedLootSpots)
    {
      foreach (var spot in selectedLootSpots)
      {
        spot.GetComponent<RLS_LootSpot> ().SelectVariant();
      }
    }

    public void SelectLootSpots()
    {
        //verify input
        ClampLimits();
        //select which indices will be used 
        var SelectedSpots = SelectRandomLoot(Random.Range(MinSpawnPoints, MaxSpawnPoints));  //temporarily the 'weight' is static
        //sort out which loot spots are hidden, and which get items spawned in them
        NotifyLootSpots(SelectedSpots);
    }

    // later add an optional 'weight'
    public GameObject[] SelectRandomLoot(int numberOfLootSpawns)
    {
      var rand = new System.Random();
      var gos = new GameObject[numberOfLootSpawns];
      var used = new int[numberOfLootSpawns];
      var totalSpawns = LootSpots.Length;

      for (var n = 0; n < numberOfLootSpawns; ++n)
      {
        var next = rand.Next(totalSpawns);
        //  make sure we do not duplicate items
        while (System.Array.Exists(used, element => element.Equals(next))) //element => element.Equals(1)
        {
          next = rand.Next(totalSpawns);
        }
        used[n] = next;
        gos[n] = LootSpots[next];
      }
      return gos;
    }

}