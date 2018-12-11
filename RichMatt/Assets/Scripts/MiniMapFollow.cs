using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapFollow : MonoBehaviour
{
	private GameObject player;
    public bool RotateWithPlayer;
    public float MiniMapHeight;
    private Transform origParent;
    private Transform PlayerParent;

	// Use this for initialization
	void Start ()
	{
		player = GameObject.FindGameObjectWithTag ("Player");
        transform.position = new Vector3(player.transform.position.x, MiniMapHeight, player.transform.position.z);
        origParent = transform.parent;
        PlayerParent = player.transform;
	}
	
	// Update is called once per frame
	void LateUpdate ()
	{
        if(RotateWithPlayer)
        {
            transform.SetParent(PlayerParent);
            transform.localPosition = new Vector3(0, MiniMapHeight, 0);
        }
        else
        {
            transform.SetParent(origParent);
            transform.position = new Vector3(player.transform.position.x, player.transform.position.y + MiniMapHeight, player.transform.position.z);
        }
	}
}
