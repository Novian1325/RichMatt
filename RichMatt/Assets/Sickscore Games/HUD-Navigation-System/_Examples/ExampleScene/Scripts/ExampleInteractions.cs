using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using SickscoreGames.HUDNavigationSystem;
using SickscoreGames.ExampleScene;

public class ExampleInteractions : MonoBehaviour
{
	#region Variables
	public LayerMask layerMask = 1 << 0;
	public float interactionDistance = 4f;

	protected RaycastHit hit;
	private Transform pickupText;
	private Transform interactionText;
	private HUDNavigationSystem _HUDNavigationSystem;
	#endregion


	#region Main Methods
	void Start ()
	{
		_HUDNavigationSystem = HUDNavigationSystem.Instance;
	}


	void Update ()
	{
		HandleKeyInput ();
		HandleItemPickUp ();
		HandlePrismColorChange ();
	}
	#endregion


	#region Utility Methods
	void HandleKeyInput ()
	{
		// update radar zoom / indicator border input
		if (Input.GetKey (KeyCode.X) && _HUDNavigationSystem.radarZoom < 5f)
			_HUDNavigationSystem.radarZoom += .0175f;
		else if (Input.GetKey (KeyCode.C) && _HUDNavigationSystem.radarZoom > .25f)
			_HUDNavigationSystem.radarZoom -= .0175f;
		else if (Input.GetKey (KeyCode.V) && _HUDNavigationSystem.indicatorOffscreenBorder < .7f)
			_HUDNavigationSystem.indicatorOffscreenBorder += .01f;
		else if (Input.GetKey (KeyCode.B) && _HUDNavigationSystem.indicatorOffscreenBorder > .07f)
			_HUDNavigationSystem.indicatorOffscreenBorder -= .01f;
		else if (Input.GetKey (KeyCode.N) && _HUDNavigationSystem.minimapScale > .06f)
			_HUDNavigationSystem.minimapScale -= .01f;
		else if (Input.GetKey (KeyCode.M) && _HUDNavigationSystem.minimapScale < 1f)
			_HUDNavigationSystem.minimapScale += .01f;

		// update feature enable / disable input
		if (Input.GetKeyDown (KeyCode.Alpha1))
			_HUDNavigationSystem.EnableRadar (!_HUDNavigationSystem.useRadar);
		if (Input.GetKeyDown (KeyCode.Alpha2))
			_HUDNavigationSystem.EnableCompassBar (!_HUDNavigationSystem.useCompassBar);
		if (Input.GetKeyDown (KeyCode.Alpha3))
			_HUDNavigationSystem.EnableIndicators (!_HUDNavigationSystem.useIndicators);
		if (Input.GetKeyDown (KeyCode.Alpha4))
			_HUDNavigationSystem.EnableMinimap (!_HUDNavigationSystem.useMinimap);

		// toggle radar / minimap mode
		if (Input.GetKeyDown (KeyCode.Alpha5))
			_HUDNavigationSystem.radarMode = (_HUDNavigationSystem.radarMode == RadarModes.RotateRadar) ? RadarModes.RotatePlayer : RadarModes.RotateRadar;
		if (Input.GetKeyDown (KeyCode.Alpha6))
			_HUDNavigationSystem.minimapMode = (_HUDNavigationSystem.minimapMode == MinimapModes.RotateMinimap) ? MinimapModes.RotatePlayer : MinimapModes.RotateMinimap;

		// toggle minimap custom layers
		if (Input.GetKeyDown (KeyCode.Alpha7) && _HUDNavigationSystem.minimapProfile != null) {
			GameObject blackWhiteLayer = _HUDNavigationSystem.minimapProfile.GetCustomLayer ("blackWhite");
			if (blackWhiteLayer != null)
				blackWhiteLayer.SetActive (!blackWhiteLayer.activeSelf);
		}
	}


	void HandleItemPickUp ()
	{
		// check for pickup items
		if (Physics.Raycast (transform.position, transform.TransformDirection (Vector3.forward), out hit, interactionDistance, layerMask) && hit.collider.name.Contains ("PickUp")) {
			// get HUD navigation element component
			HUDNavigationElement element = hit.collider.gameObject.GetComponent<HUDNavigationElement> ();
			if (element != null) {
				// show pickup text
				if (element.Indicator != null) {
					pickupText = element.Indicator.GetCustomTransform ("pickupText");
					if (pickupText != null)
						pickupText.gameObject.SetActive (true);
				}

				// wait for interaction input and destroy gameobject
				if (Input.GetKeyDown (KeyCode.E))
					Destroy (element.gameObject);
			}
		} else {
			// reset pickup text
			if (pickupText != null) {
				pickupText.gameObject.SetActive (false);
				pickupText = null;
			}
		}
	}


	void HandlePrismColorChange ()
	{
		// check for colored prisms
		if (Physics.Raycast (transform.position, transform.TransformDirection (Vector3.forward), out hit, interactionDistance, layerMask) && hit.collider.name.Contains ("Prism")) {
			// get HUD navigation element component
			HUDNavigationElement element = hit.collider.gameObject.GetComponentInChildren<HUDNavigationElement> ();
			if (element != null) {
				// show interaction text
				if (element.Indicator != null) {
					interactionText = element.Indicator.GetCustomTransform ("interactionText");
					if (interactionText != null)
						interactionText.gameObject.SetActive (true);
				}

				// wait for interaction input and change prism color
				if (Input.GetKeyDown (KeyCode.E)) {
					// generate random color
					Color randomColor = Random.ColorHSV (0f, 1f, 1f, 1f, .5f, 1f);

					// change radar color
					if (element.Radar != null)
						element.Radar.ChangeIconColor (randomColor);

					// change compass bar color
					if (element.CompassBar != null)
						element.CompassBar.ChangeIconColor (randomColor);

					// change indicator colors
					if (element.Indicator != null) {
						element.Indicator.ChangeIconColor (randomColor);
						element.Indicator.ChangeOffscreenIconColor (randomColor);
					}

					// change minimap color
					if (element.Minimap != null)
						element.Minimap.ChangeIconColor (randomColor);

					// change prism material color
					Renderer renderer = element.transform.parent.GetComponent<Renderer> ();
					if (renderer != null)
						renderer.material.color = new Color (randomColor.r, randomColor.g, randomColor.b, renderer.material.color.a);
				}
			}
		} else {
			// reset interaction text
			if (interactionText != null) {
				interactionText.gameObject.SetActive (false);
				interactionText = null;
			}
		}
	}
	#endregion
}
