using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//---------------------------------------------------------------------------------------------------
//This script is provided as a part of The Polygon Pilgrimage
//Subscribe to https://www.youtube.com/user/mentallic3d for more great tutorials and helpful scripts!
//---------------------------------------------------------------------------------------------------
public class BRS_PlayerHealthManager : MonoBehaviour
{
	[Header("---Player Health Parameters---")]
	public int startHealth;
	public int maxHealth;
	public int minHealth;
	public bool isAlive; //TODO: Future episode on Player stats and 3rd Person Animations!!

	[Header("---Health UI Slider---")]
	public GameObject _healthSlider;
	private Slider healthSlider;

	[Header("---Tutorial Only---")]
	public GameObject _HealthAmount;
	public Text healthText;

	// Use this for initialization
	void Start ()
	{
		healthSlider = _healthSlider.GetComponent<Slider> ();
		healthSlider.maxValue = maxHealth;
		healthSlider.minValue = minHealth;
		healthSlider.value = startHealth;

		//Remove after tutorial
		healthText = _HealthAmount.GetComponent<Text> ();
	}

	public void ChangeHealth(int changeAmount)
	{
		healthSlider.value += changeAmount;

		//Remove after tutorial
		healthText.text = healthSlider.value.ToString ();
	}
}