using UnityEngine;
using UnityEngine.UI;

public class BRS_Trackable : MonoBehaviour {

    [SerializeField] private Texture compassImage;
    [SerializeField] private Texture minimapImage;
    [SerializeField] private float revealDistance;
    [SerializeField] private Color iconColor;
    private Transform cachedTransform;

    private static Compass compassInstance;

    private void Awake()
    {
        InitStaticCompassInstance();


    }

    // Use this for initialization
    void Start () {
        cachedTransform = this.transform;
        
	}
	
	// Update is called once per frame
	void Update () {
		
        //hide from map if distance to player too great
	}

    public void RemoveTrackable()
    {
        compassInstance.RemoveTrackable(this);
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

    private void OnEnable()
    {
        compassInstance.RegisterTrackable(this);
        //minimap
    }

    private void OnDisable()
    {
        compassInstance.RemoveTrackable(this);
        //minimap
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

    public Transform GetTrackableTransform()
    {
        return this.cachedTransform;
    }
}
