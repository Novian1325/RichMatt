using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RLS_LootSpot : MonoBehaviour
{
	public GameObject[] Variants;
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
		var selectedVariantID = Random.Range(0, (Variants.Length-1));
        Debug.Log(Variants.Length);
        Debug.Log(selectedVariantID);
		var varChoice = Variants[selectedVariantID];
		varChoice.SetActive(true);
	}
}
