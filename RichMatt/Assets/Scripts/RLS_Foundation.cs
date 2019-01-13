using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RLS_Foundation : MonoBehaviour
{
    [Header("Spawn Weight (percent)")]
    [Range(1, 100)]
    public int MinSpawnPoints = 1;
    [Range(1, 100)]
    public int MaxSpawnPoints = 99;

    public GameObject[] LootSpots;
    public int numLootSpots;
    public GameObject[] SelectedSpots;

    // Use this for initialization
    void Start ()
    {
        ClearLoot();
        var range = CalculateRange();
        SelectedSpots = SelectRandomLoot(Random.Range(range[0], range[1]));  //temporarily the 'weight' is static
		NotifyLootSpots();
    }

    void OnEnable()
    {
      if (MinSpawnPoints >= MaxSpawnPoints)
      {
        if (MinSpawnPoints < 2)
          MinSpawnPoints = 2;
          MinSpawnPoints = MaxSpawnPoints - 1;
      }
    }

    // later add an optional 'weight'
    public GameObject[] SelectRandomLoot(int number)
    {
      System.Random rand = new System.Random();
      GameObject[] gos = new GameObject[number];
      int[] used = new int[number];
      var total = LootSpots.Length;

      for (var n = 0; n < number; n++)
      {
        var next = rand.Next(total);
        //  make sure we do not duplicate items
        while (System.Array.IndexOf(used, next) > -1)
        {
          next = rand.Next(total);
        }
        used[n] = next;
        gos[n] = LootSpots[next];
      }
      return gos;
    }

    // Using the total number of LootSpots and the ranges this will calculate a number
    // to be used when calling 'SelectRandomLoot'
    private int[] CalculateRange()
    {
       var total = LootSpots.Length;

       //determine resolution (how many overall)
       var low = (int)Mathf.Clamp(total * 0.01f * MinSpawnPoints, 1.0f, total);
       var high = (int)Mathf.Clamp(total * 0.01f * MaxSpawnPoints, low, total);

      return new int[2]{low, high};
    }

	public void NotifyLootSpots()
	{
		foreach (GameObject spot in SelectedSpots)
		{
			spot.GetComponent<RLS_LootSpot> ().SelectVariant();
		}
	}

    public void ClearLoot()
    {
        //Get all of the Variants in the scene and set them to false
        var lootvars = GameObject.FindGameObjectsWithTag("RLS_Variant");
        foreach(GameObject go in lootvars)
        {
            go.SetActive(false);
        }
    }
}