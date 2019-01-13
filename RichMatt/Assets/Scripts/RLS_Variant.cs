using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RLS_Variant : MonoBehaviour
{
	public GameObject loot;
	public Transform SpawnPoint;

	// Use this for initialization
	void OnAwake ()
	{
		Instantiate(loot, SpawnPoint);
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	void Start()
	{
		Instantiate(loot, SpawnPoint);
	}
}
