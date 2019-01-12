using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;

public enum degreeIncrement
{
    One = 1,
    Five = 5,
    Ten = 10
}

public class Compass : MonoBehaviour
{
	public RawImage CompassImage;
	public Transform mainCameraXform;
	public Text CompassDirectionText;

    [Header("Readout Options")]
    [SerializeField] private bool ordinalLetters = true;//show N instead of 0 or S instead of 180
    [SerializeField] private degreeIncrement degreeIncrement = degreeIncrement.Five;// round to this number

    [Header("Icons")]
    [SerializeField] private GameObject compassMarkerPrefab;// prefab used  //icons UV rect x value is between -.5 and .5
    private List<Transform> trackableTransforms = new List<Transform>(); //transforms of all the trackable locations
    private List<BRS_CompassMarker> compassMarkerList = new List<BRS_CompassMarker>();

    private void Start()
    {
        //find it 
        if (mainCameraXform == null) mainCameraXform = Camera.main.transform;
        //if STILL null
        if (mainCameraXform == null) Debug.LogError("ERROR! No GameObject tagged \"MainCamera\" in scene.");
    }

    private static void UpdateCompassMarker(BRS_CompassMarker compassMarker, Vector3 trackablePosition, Transform playerXform)
    {
        //get the distance to player
        float distanceFromPlayer = Vector3.Distance(playerXform.position, trackablePosition);

        if (distanceFromPlayer <= compassMarker.GetRevealDistance())
        {
            if (!compassMarker.gameObject.activeSelf)
            {
                compassMarker.gameObject.SetActive(true);
            }

            //float angle = Mathf.Atan2 (Vector3.Magnitude (Vector3.Cross (go1.transform.forward, go2.transform.forward)), Vector3.Dot (go1.transform.forward, go2.transform.forward));
            float angle = Vector3.SignedAngle(playerXform.forward, Vector3.Normalize(trackablePosition - playerXform.position), Vector3.up); // this one works best, but has reduced accuracy when angle is nearing 0
            //float angle = Vector3.Angle(playerXform.forward, trackablePosition - playerXform.position);
            //float angle = Mathf.Atan2(Vector3.Magnitude(Vector3.Cross(playerXform.forward, trackablePosition - playerXform.position)), 
                //Vector3.Dot(playerXform.forward, trackablePosition - playerXform.position));
            //angle *= Mathf.Deg2Rad;
            
            //float angle = Vector3.Dot(playerXform.forward, Vector3.Normalize(trackablePosition - playerXform.position));
            //angle = Mathf.Acos(angle);


            compassMarker.GetCompassMarkerImage().uvRect = new Rect(-angle / 180, 0, 1, 1);
            Debug.Log("Trackable Angle: " + angle.ToString());
        }
        else
        {
            compassMarker.gameObject.SetActive(false);
        }
        //get the angle from player to trackable transform

        //if distance to player < revealDistance
        //show icon
        //update position on compass

        //else hide icon



    }

    public void Update()
	{
        float headingAngle = mainCameraXform.eulerAngles.y;

        //Get a handle on the Image's uvRect
        CompassImage.uvRect = new Rect(headingAngle / 360, 0, 1, 1);
        
		headingAngle = (int)degreeIncrement * Mathf.RoundToInt(headingAngle / (int)degreeIncrement  );

        //convert the numbers to letters if pointing towards a direction (N/E/S/W)
        if (ordinalLetters)
        {
            ConvertAngleToLetter( (int)headingAngle );
        }
        else
        {
            CompassDirectionText.text = headingAngle.ToString();
        }
        
		for(int i = 0; i < compassMarkerList.Count; ++i)
        {
            UpdateCompassMarker(compassMarkerList[i], trackableTransforms[i].position, mainCameraXform);
        }

	}

    public void RegisterTrackable(BRS_Trackable newTrackable)
    {
        //create new marker
        BRS_CompassMarker compassMarker = Instantiate(compassMarkerPrefab, CompassImage.transform).GetComponent<BRS_CompassMarker>() as BRS_CompassMarker;

        //initialize marker with image, color, and distance
        compassMarker.InitCompassMarker(newTrackable);

        //add trackables to list
        trackableTransforms.Add(newTrackable.transform);//add transform
        compassMarkerList.Add(compassMarker);
        
    }

    private void ConvertAngleToLetter(int angle)
    {//Set the text of Compass Degree Text to the clamped value, but change it to the letter if it is a True direction
        switch (angle)
        {
            case 0:
                //Do this
                CompassDirectionText.text = "N";
                break;
            case 360:
                //Do this
                CompassDirectionText.text = "N";
                break;
            case 45:
                //Do this
                CompassDirectionText.text = "NE";
                break;
            case 90:
                //Do this
                CompassDirectionText.text = "E";
                break;
            case 135:
                //Do this
                CompassDirectionText.text = "SE";
                break;
            case 180:
                //Do this
                CompassDirectionText.text = "S";
                break;
            case 225:
                //Do this
                CompassDirectionText.text = "SW";
                break;
            case 270:
                //Do this
                CompassDirectionText.text = "W";
                break;
            case 315:
                //Do this
                CompassDirectionText.text = "NW";
                break;
            default:
                CompassDirectionText.text = angle.ToString();
                break;
        }


    }
}