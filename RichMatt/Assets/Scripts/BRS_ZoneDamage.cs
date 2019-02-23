using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PostProcessing;
//---------------------------------------------------------------------------------------------------
//This script is provided as a part of The Polygon Pilgrimage
//Subscribe to https://www.youtube.com/user/mentallic3d for more great tutorials and helpful scripts!
//---------------------------------------------------------------------------------------------------
public class BRS_ZoneDamage : MonoBehaviour
{
	[Header("---Player Object---")]
	public GameObject player;
	private string playername;

	[Header("---Post Processing Objects---")]
	public PostProcessingProfile outsideZonePPP;
	public PostProcessingProfile standardPPP;
	private PostProcessingBehaviour CamPPB;

	[Header("---Zone Damage Parameters---")]
	public float TickRate = 3.0f;
	public int TickDamage = 1;

	//Future values
	//For each succesive zone change we will damage
	//the player more for being outside the zone
	//public int zoneDamageMultiplier;

	[Header("---Player Health Manager Handle---")]
	public GameObject _BRS_Mechanics;
	public BRS_PlayerHealthManager _PHM;

	//Are we in the Zone?
	private bool inZone;

	//For DEMO purposes ONLY.  Reset our Health to full when we
	//re-enter the Zone!
	private bool DebugHealth = true;

	void Start ()
	{
		//Get a handle to the Player
		player = GameObject.FindGameObjectWithTag ("Player");
		CamPPB = GameObject.Find ("FirstPersonCharacter").GetComponent<PostProcessingBehaviour> ();
		playername = player.transform.name;

		//Get a handle to the Player Health Manager
		_PHM = _BRS_Mechanics.GetComponent<BRS_PlayerHealthManager> ();

		//Setup the DamagePlayer to run every X seconds
		InvokeRepeating ("DamagePlayer", 0.0f, TickRate);
	}

	void OnTriggerExit(Collider col)
	{
		//If we leave the zone, we will be damaged!
		if (col.transform.name == playername)
		{
			CamPPB.profile = outsideZonePPP;
			inZone = false;
		}
	}

	void OnTriggerEnter(Collider col)
	{
		//If we are inside the zone, all is good!
		if (col.transform.name == playername)
		{
			CamPPB.profile = standardPPP;
			inZone = true;
		}
	}

	void DamagePlayer()
	{
		if (!inZone)
		{
			//Damage the player [TickDamage] amount
			_PHM.ChangeHealth (-TickDamage);
		}
		else if(DebugHealth)
		{
			_PHM.ChangeHealth (100);
		}
	}
}