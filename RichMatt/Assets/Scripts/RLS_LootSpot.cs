using UnityEngine;

public class RLS_LootSpot : MonoBehaviour
{
	public GameObject[] variants;
	//public int numVariants;

	// Use this for initialization
	void Start ()
	{
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}

	public void SelectVariant()
	{
		var varChoice = variants[Random.Range(0, variants.Length)];

        Debug.Log(selectedVariantID + " / " + variants.Length);

		varChoice.SetActive(true);
	}
}
