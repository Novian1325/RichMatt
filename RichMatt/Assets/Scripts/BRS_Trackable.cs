using UnityEngine;

namespace PolygonPilgrimage.BattleRoyaleKit
{
    public class BRS_Trackable : MonoBehaviour
    {

        [SerializeField] private Texture compassImage;
        [SerializeField] private float revealDistance;
        [SerializeField] private Color iconColor;
        private Transform cachedTransform;

        private static Compass compassInstance;

        private void Awake()
        {
            //only the first trackable has to do the hard work
            InitStaticCompassInstance();

            //if there's no compass, there's no work to do
            if (!compassInstance)
            {

                Debug.LogError("ERROR! No BRS_Compass in scene! nothing to register trackable to.");
                this.enabled = false;
            }
            
            //cache transform
            cachedTransform = this.transform;
        }

        /// <summary>
        /// Stop tracking and remove related objects from Compass Instance.
        /// </summary>
        public void RemoveTrackable()
        {
            compassInstance.RemoveTrackable(this);
        }

        /// <summary>
        /// Attempt to find the compass instance.
        /// </summary>
        private static void InitStaticCompassInstance()
        {
            if (!compassInstance)
            {
                var compassObject = GameObject.FindGameObjectWithTag("Compass");
                if(compassObject) compassInstance = compassObject.GetComponent<Compass>();
            }
        }

        private void OnEnable()
        {
            if (compassInstance && compassImage) {
                compassInstance.RegisterTrackable(this);
            }
            
        }

        private void OnDisable()
        {
           if(compassInstance) compassInstance.RemoveTrackable(this);
        }

        private void OnDestroy()
        {
            if (compassInstance) compassInstance.RemoveTrackable(this);
        }

        public Texture GetCompassImage()
        {
            return this.compassImage;
        }

        public float GetRevealDistance()
        {
            return this.revealDistance;
        }

        public Color GetIconColor()
        {
            return this.iconColor;
        }

        /// <summary>
        /// Changes color on Trackable, and also updates the compass trackable icon color
        /// </summary>
        /// <param name="newColor"></param>
        public void SetPlayerColor(Color newColor)
        {
            this.iconColor = newColor;
            //update color of trackable
            compassInstance.RemoveTrackable(this);
            compassInstance.RegisterTrackable(this);
        }

        public Transform GetTrackableTransform()
        {
            return this.cachedTransform;
        }
    }
}
