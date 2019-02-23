using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace PolygonPilgrimage.BattleRoyaleKit
{
    public class BRS_CompassMarker : MonoBehaviour
    {

        [Tooltip("Integrated. Optional.")]
        [SerializeField] private TextMeshProUGUI TMP_distanceText;
        private RawImage compassMarkerImage;
        private BRS_Trackable trackable;
        private float distanceFromPlayer;

        //coroutine trackers
        private Coroutine coroutine_updateDistanceText;//track to enable pausing game
        private static readonly int textUpdatesPerSecond = 2;

        private void Awake()
        {
            if (!compassMarkerImage)
            {
                compassMarkerImage = gameObject.GetComponent<RawImage>() as RawImage;
            }

        }

        private void Start()
        {
            if (TMP_distanceText)
            {
                coroutine_updateDistanceText = StartCoroutine(UpdateDistanceText());
            }
        }

        private void OnEnable()
        {
            ResumeUpdatingDistance();
        }

        private void OnDisable()
        {
            StopUpdatingDistance();
        }

        /// <summary>
        /// Initialize a new compass marker
        /// </summary>
        /// <param name="trackable">Associated trackable object this is paired with.</param>
        public void InitCompassMarker(BRS_Trackable trackable)
        {
            this.trackable = trackable;
            compassMarkerImage.texture = trackable.GetCompassImage();
            compassMarkerImage.color = trackable.GetIconColor();
        }

        /// <summary>
        /// Controls how often updates to the UI text are made.
        /// </summary>
        /// <returns></returns>
        private IEnumerator UpdateDistanceText()
        {
            yield return new WaitForSecondsRealtime(1 / textUpdatesPerSecond);
            TMP_distanceText.text = distanceFromPlayer.ToString();
        }

        /// <summary>
        /// Stops coroutine associated with distance polling. Useful if game is paused.
        /// </summary>
        public void StopUpdatingDistance()
        {
            if(coroutine_updateDistanceText != null) StopCoroutine(coroutine_updateDistanceText);
        }

        /// <summary>
        /// Causes coroutines to resume.
        /// </summary>
        public void ResumeUpdatingDistance()
        {
            //
            if (coroutine_updateDistanceText == null && TMP_distanceText)
            {
                coroutine_updateDistanceText = StartCoroutine(UpdateDistanceText());
            }
        }
        
        public RawImage GetCompassMarkerImage()
        {
            return this.compassMarkerImage;
        }

        public bool CompareTrackable(BRS_Trackable otherTrackable)
        {
            return otherTrackable == trackable;
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
}
