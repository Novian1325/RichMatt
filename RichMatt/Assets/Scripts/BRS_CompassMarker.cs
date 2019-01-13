using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class BRS_CompassMarker : MonoBehaviour {

    [SerializeField] private TextMeshProUGUI TMP_distanceText;
    private RawImage compassMarkerImage;
    private BRS_Trackable trackable;
    private float distanceFromPlayer;

    //coroutine trackers
    private Coroutine coroutine_updateDistanceText;
    private static readonly int textUpdatesPerSecond = 2;

    private void Awake()
    {
        if (!compassMarkerImage)
        {
            compassMarkerImage = this.gameObject.GetComponent<RawImage>() as RawImage;
        }

    }

    private void Start()
    {
        if (TMP_distanceText)
        {
            coroutine_updateDistanceText = StartCoroutine(UpdateDistanceText());
        }
    }

    public void InitCompassMarker(BRS_Trackable trackable)
    {
        this.trackable = trackable;
        compassMarkerImage.texture = trackable.GetCompassImage();
        compassMarkerImage.color = trackable.GetIconColor();
    }

    private IEnumerator UpdateDistanceText()
    {
        yield return new WaitForSeconds(textUpdatesPerSecond / 1);
        TMP_distanceText.text = distanceFromPlayer.ToString();

    }


    public RawImage GetCompassMarkerImage()
    {
        return this.compassMarkerImage;
    }
    
    public bool CompareTrackable(BRS_Trackable otherTrackable)
    {
        return otherTrackable == this.trackable;
    }

    public float GetRevealDistance()
    {
        return this.trackable.GetRevealDistance();
    }

    public void SetDistanceFromPlayer(float distanceToPlayer)
    {
        this.distanceFromPlayer = distanceToPlayer;
    }

    public float GetDistanceFromPlayer()
    {
        return this.distanceFromPlayer;
    }

    public void UpdateColor()
    {
        compassMarkerImage.color = trackable.GetIconColor();
    }

    public void UpdateIcon()
    {
        compassMarkerImage.texture = trackable.GetCompassImage();
    }

    public Transform GetTrackableTransform()
    {
        return trackable.GetTrackableTransform();
    }
}
