using UnityEngine;
using UnityEngine.UI;

public class BRS_Trackable : MonoBehaviour {

    [SerializeField] private Texture compassImage;
    [SerializeField] private Texture minimapImage;
    [SerializeField] private float revealDistance;
    [SerializeField] private Color iconColor;

    private static Compass compassInstance;

    private void Awake()
    {
        InitStaticCompassInstance();


    }

    // Use this for initialization
    void Start () {
        compassInstance.RegisterTrackable(this);
        //register with minimap
        //Destroy(this); //no longer needed after registering
	}
	
	// Update is called once per frame
	void Update () {
		
        //hide from map if distance to player too great
	}

    private static void InitStaticCompassInstance()
    {
        if (!compassInstance)
        {
            compassInstance = GameObject.FindGameObjectWithTag("Compass").GetComponent<Compass>() as Compass;

            if (!compassInstance)
            {
                Debug.LogError("ERROR! No BRS_Compass in scene! nothing to register trackable to.");
            }
        }
    }

    public Texture GetCompassImage()
    {
        return this.compassImage;
    }

    public Texture GetMinimapImage()
    {
        return this.minimapImage;
    }

    public float GetRevealDistance()
    {
        return this.revealDistance;
    }

    public Color GetIconColor()
    {
        return this.iconColor;
    }
}
