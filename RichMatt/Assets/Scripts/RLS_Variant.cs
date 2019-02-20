using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RLS_Variant : MonoBehaviour
{
	public GameObject loot;
	public Transform spawnPoint;

	void Start()
	{
		Instantiate(loot, spawnPoint);
	}
}
