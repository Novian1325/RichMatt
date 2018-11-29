using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BRS_TacticalMarker : MonoBehaviour
{
	public GameObject TacticalMarker;

	private float markerOffset;
	private Camera FPCamera;
	private float MinimapCamHeight;
	private Ray ray;
	private RaycastHit hit;
	private string MARKER_ID = "*NONE*"; //this is how we keep track of its existence in the world

	// Use this for initialization
	void Start ()
	{
		FPCamera = GetComponentInChildren<Camera>();
		MinimapCamHeight = GameObject.Find ("MiniMap Camera").transform.position.y;
		markerOffset = MinimapCamHeight - 10.0f;
	}

	// Update is called once per frame
	void Update ()
	{
			if (Input.GetKeyUp (KeyCode.T))
			{
				PlaceMarker();
			}
	}

	private void PlaceMarker()
	{
		ray = new Ray(FPCamera.transform.position, FPCamera.transform.forward);
		// Are we pointing at something in the world?
		if (Physics.Raycast(ray, out hit))
		{
			Vector3 markerLocation = new Vector3(hit.point.x, markerOffset, hit.point.z);
			if (MARKER_ID == "*NONE*") // no marker on the map
			{
				GameObject marker = Instantiate(TacticalMarker, markerLocation, Quaternion.identity, null );
				MARKER_ID = marker.name;  //remember this for the next time

			} else { //find the marker that exists and relocate it
				GameObject marker = GameObject.Find(MARKER_ID);
				Destroy (marker);
				marker = Instantiate(TacticalMarker, markerLocation, Quaternion.identity, null );
				MARKER_ID = marker.name;  //remember this for the next time
			}
		}
	}
}
