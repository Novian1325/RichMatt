using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

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
    private List<BRS_CompassMarker> compassMarkerList = new List<BRS_CompassMarker>();

    //courtine references
    private Coroutine coroutine_CompassMarkerSort;
    private static readonly float sortsPerSecond = .5f;// every other second

    private void Start()
    {
        //find it 
        if (mainCameraXform == null) mainCameraXform = Camera.main.transform;
        //if STILL null
        if (mainCameraXform == null) Debug.LogError("ERROR! No GameObject tagged \"MainCamera\" in scene.");

        coroutine_CompassMarkerSort = StartCoroutine(SortCompassMarker(sortsPerSecond));
    }

    /// <summary>
    /// Update the icon on the compass to match angle from player to trackable object
    /// </summary>
    /// <param name="compassMarker">marker with image to manipulate</param>
    /// <param name="trackablePosition">Position of trackable object in World Space</param>
    /// <param name="playerXform">Reference transform (player)</param>
    private static void UpdateCompassMarker(BRS_CompassMarker compassMarker, Vector3 trackablePosition, Transform playerXform)
    {
        Vector3 relative = playerXform.InverseTransformPoint(trackablePosition);
        float angle = Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;

        compassMarker.GetCompassMarkerImage().uvRect = new Rect(-angle / 360, 0, 1, 1); //need a value between -.5 an .5 for uvRect
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
        
        Vector3 trackablePosition;
        BRS_CompassMarker compassMarker;
        
        for (int i = 0; i < compassMarkerList.Count; ++i)
        {
            compassMarker = compassMarkerList[i];
            if (!compassMarker.GetTrackableTransform()) Debug.Log("ERROR! NO MARKER HERE");
            trackablePosition = compassMarker.GetTrackableTransform().position;

            //get and save the distance to player
            float distance = Vector3.Distance(mainCameraXform.position, trackablePosition);
            compassMarker.SetDistanceFromPlayer(distance);

            if (distance <= compassMarker.GetRevealDistance())
            {
                //enable it if it is not already so
                if (!compassMarker.isActiveAndEnabled)
                {
                    compassMarker.gameObject.SetActive(true);
                }

                //update uv rect on compass to reflect angle to player
                UpdateCompassMarker(compassMarker, trackablePosition, mainCameraXform);

            }
            else
            {
                compassMarker.gameObject.SetActive(false);
            }
        }
        
    }

    /// <summary>
    /// Coroutine used to restrict frequency of list sorting. Helps with performance
    /// </summary>
    /// <param name="sortsPerSecond"></param>
    /// <param name="compassMarkerList"></param>
    /// <returns></returns>
    private IEnumerator SortCompassMarker(float sortsPerSecond)
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(1 / sortsPerSecond);

            if (compassMarkerList.Count > 1)
            {
                //order icons so closest object to player is on top of all other icons
                compassMarkerList = compassMarkerList.OrderBy(o => o.GetDistanceFromPlayer()).ToList();
                
                for (int i = 0; i < compassMarkerList.Count; ++i)
                {
                    compassMarkerList[i].transform.SetSiblingIndex(compassMarkerList.Count - 1 - i);
                }
            }
        }
    }

    /// <summary>
    /// Adds the given trackable to the top compass.
    /// </summary>
    /// <param name="newTrackable">Trackable whose texture and color will be used on the icon.</param>
    public void RegisterTrackable(BRS_Trackable newTrackable)
    {
        //check if already exists
        foreach (var marker in compassMarkerList)
        {
            if (marker.CompareTrackable(newTrackable)) return;
        }

        //create new marker
        BRS_CompassMarker compassMarker = Instantiate(compassMarkerPrefab, CompassImage.transform).GetComponent<BRS_CompassMarker>() as BRS_CompassMarker;

        //initialize marker with image, color, and distance
        compassMarker.InitCompassMarker(newTrackable);

        //add trackables to list
        compassMarkerList.Add(compassMarker);
    }

    /// <summary>
    /// Removes trackable from the compass.
    /// </summary>
    /// <param name="trackable">trackable to remove</param>
    public void RemoveTrackable(BRS_Trackable trackable)
    {
        for(int i = 0; i < compassMarkerList.Count; ++i)
        {
            BRS_CompassMarker marker = compassMarkerList[i];//cache

            if (marker.CompareTrackable(trackable))
            {
                compassMarkerList.Remove(compassMarkerList[i]);//remove marker icon reference

                if(marker) Destroy(marker.gameObject);//destroy UI element
                break;
            }
        }
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