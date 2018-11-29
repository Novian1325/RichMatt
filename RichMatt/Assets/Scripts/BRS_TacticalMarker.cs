using UnityEngine;
using System.Collections.Generic;

public class BRS_TacticalMarker : MonoBehaviour
{
    //tactical marker stuff
	public GameObject TacticalMarkerPrefab;

	private float tacticalMarkerOffset;
	private Transform FPCameraTransform;
	private float MinimapCamHeight;
    private GameObject tacticalMarkerInstance;

    //TODO
    //hold 't' for 3 seconds to remove marker from map
    //destroy marker if distance is too great

    //what is the maximum distance away a player can set a tactical marker
    private readonly int tacticalMarkerPlaceDistanceLimit = 300;

    

    void Start ()
	{
        
		FPCameraTransform = GetComponentInChildren<Camera>().transform;
		MinimapCamHeight = GameObject.FindGameObjectWithTag ("MiniMap Camera").transform.position.y;
		tacticalMarkerOffset = MinimapCamHeight - 10.0f;//marker always shown below map
	}

    private void DestroyExistingTacticalMarkerAtDistanceLimit()
    {
        //if distance between player and marker > distance limit
        if (Vector3.Distance(this.transform.position, tacticalMarkerInstance.transform.position) > tacticalMarkerPlaceDistanceLimit)
        {
            //TODO Should test 2D distance!
            Debug.Log("Distance: " + Vector3.Distance(this.transform.position, tacticalMarkerInstance.transform.position));
            Destroy(tacticalMarkerInstance);
        }
    }

    // Update is called once per frame
    void Update ()
	{
        //if(tacticalMarkerInstance) DestroyExistingTacticalMarkerAtDistanceLimit();
        //Handle Tactical Marker
        if (Input.GetKeyDown (KeyCode.T))
		{
            PlaceTacticalMarker();
		}
        
    }

    

	private void PlaceTacticalMarker()
	{
        RaycastHit hitInfo;
        // Are we pointing at something in the world?
        if (Physics.Raycast(FPCameraTransform.position, FPCameraTransform.forward, out hitInfo, tacticalMarkerPlaceDistanceLimit))
		{
            Vector3 markerLocation = new Vector3(hitInfo.point.x, tacticalMarkerOffset, hitInfo.point.z);
            if (tacticalMarkerInstance != null)
            {
                Destroy(tacticalMarkerInstance);
            }
            tacticalMarkerInstance = Instantiate(TacticalMarkerPrefab, markerLocation, Quaternion.identity);
            
		}
	}
    

    
}
