using UnityEngine;
using UnityEngine.UI;

public class BRS_CompassMarker : MonoBehaviour {

    private RawImage compassMarkerImage;
    private float revealDistance;

    private void Awake()
    {
        if (!compassMarkerImage)
        {
            compassMarkerImage = this.gameObject.GetComponent<RawImage>() as RawImage;
        }

    }

    public void InitCompassMarker(Texture icon, Color color = new Color(), float revealDistance = -1)
    {
        this.compassMarkerImage.texture = icon;
        this.compassMarkerImage.color = color;
        this.revealDistance = revealDistance;
    }

    public void InitCompassMarker(BRS_Trackable trackable)
    {
        InitCompassMarker(trackable.GetCompassImage(), trackable.GetIconColor(), trackable.GetRevealDistance());
    }

    public RawImage GetCompassMarkerImage()
    {
        return this.compassMarkerImage;
    }

    public void SetCompassMarkerImage(Texture texture)
    {
        compassMarkerImage.texture = texture;
    }

    public void SetCompassMarkerColor(Color newColor)
    {
        compassMarkerImage.color = newColor;
    }

    public void SetRevealDistance(float revealDistance)
    {
        this.revealDistance = revealDistance;
    }

    public float GetRevealDistance()
    {
        return this.revealDistance;
    }
}
