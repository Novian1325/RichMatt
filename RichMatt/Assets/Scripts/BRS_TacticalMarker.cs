using UnityEngine;
using UnityEngine.UI;

namespace PolygonPilgrimage.BattleRoyaleKit
{
    public class BRS_TacticalMarker : MonoBehaviour
    {
        #region Static Readonly Variables

        /// <summary>
        /// The name of the Button in the Input Manager to check.
        /// </summary>
        private static readonly string TMBUTTONNAME = "TacticalMarker";//name of the button in Input Manager that will trigger tactical marker placement

        /// <summary>
        /// The maximum distance from the Player that a Tactical Marker can be placed.
        /// </summary>
        private static readonly int tacticalMarkerPlaceDistanceLimit = 300;

        /// <summary>
        /// How high should the icon be placed to ensure that it is above all other objects?
        /// </summary>
        private static readonly int iconHeightOffset = 1000;

        /// <summary>
        /// Limits how often the distance is polled in order to save on performance. A larger number means more polls per second.
        /// </summary>
        private static readonly int distanceToMarkerPollsPerSecond = 2;

        #endregion

        //tactical marker stuff
        [Tooltip("The prefab of the UI element that will be shown when activated.")]
        [SerializeField] private GameObject TacticalMarkerPrefab;

        [Tooltip("The Player's color. Used to identify icons that belong to each Player.")]
        [SerializeField] private Color playerColor; //player's color to match marker

        [Tooltip("Text that tracks the distance between the Player and the Tactical Marker.")]
        [SerializeField] private Text distanceText;

        /// <summary>
        /// transform of player's camera
        /// </summary>
        private Transform playerCameraXform; // 

        /// <summary>
        /// The marker object that is currently existing in the world
        /// </summary>
        private GameObject tacticalMarkerInstance;//

        //distance polling stuff
        private int distanceToMarker;//how close is the player to the marker they placed?
        private float distancePollingTimer = 0f; //used to keep track of time and limit distance polling rate

        private Transform minimapCameraXform;

        //TODO
        //hold 't' for 3 seconds to remove marker from map

        void Start()
        {
            if (!TacticalMarkerPrefab) Debug.LogError("ERROR! No Tactical Marker Prefab set!");
            playerCameraXform = GameObject.FindGameObjectWithTag("MainCamera").transform as Transform;//get the player's camera
            minimapCameraXform = GameObject.FindGameObjectWithTag("MiniMap Camera").transform as Transform;
        }

        private void DestroyExistingTacticalMarkerAtDistanceLimit()
        {
            //if distance between player and marker > distance limit
            if (distanceToMarker > tacticalMarkerPlaceDistanceLimit)
            {
                Debug.Log("Distance: " + Vector3.Distance(this.transform.position, tacticalMarkerInstance.transform.position) + ". Destroying TacticalMarker.");
                if (tacticalMarkerInstance) Destroy(tacticalMarkerInstance);//if it exists, destroy it
            }
        }

        // Update is called once per frame
        void Update()
        {
            //Handle Tactical Marker
            if (Input.GetButtonDown(TMBUTTONNAME))
            {
                PlaceTacticalMarker();
            }

            if (tacticalMarkerInstance)
            {
                UpdateDistanceToMarker();

                DestroyExistingTacticalMarkerAtDistanceLimit();
            }
        }

        private void UpdateDistanceText()
        {
            if (distanceText)
            {
                System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();

                stringBuilder.Append(distanceToMarker.ToString());
                stringBuilder.Append("m");

                distanceText.text = stringBuilder.ToString();
            }
        }

        private void UpdateDistanceToMarker()
        {
            //limit polling rate to be more performant
            if (Time.time > distancePollingTimer) //if it's time to poll again...
            {
                //Calculate the distance from the player to the marker
                distanceToMarker = (int)Vector3.Distance(playerCameraXform.position, tacticalMarkerInstance.transform.position);

                //display this distance on the HUD
                UpdateDistanceText();

                //reset next update time
                distancePollingTimer = Time.time + (1 / distanceToMarkerPollsPerSecond);
            }
        }

        private void PlaceTacticalMarker()
        {
            //TODO 
            //react to item being tagged (eg the whole building gets tagged, an enemy gets a dot over its head, the player speaks aloud different quips, etc)
            RaycastHit hitInfo;

            // Are we pointing at something in the world? IGNORES TRIGGER COLLIDERS
            if (Physics.Raycast(playerCameraXform.position, playerCameraXform.forward, out hitInfo, tacticalMarkerPlaceDistanceLimit, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            {
                if (tacticalMarkerInstance)//if an TM already exists
                {
                    Destroy(tacticalMarkerInstance);//destroy it and replace it
                }

                //this will cause the UI to update right away
                distancePollingTimer = 1 / distanceToMarkerPollsPerSecond;
                //create a new marker in the world
                tacticalMarkerInstance = Instantiate(TacticalMarkerPrefab, hitInfo.point, Quaternion.identity);
                //set icon color to match player's color
                tacticalMarkerInstance.GetComponent<BRS_Trackable>().SetPlayerColor(playerColor);
                //make it a child of the hit object so if it moves, the marker moves with it. assigning childhood after instantiation preserves native scale
                tacticalMarkerInstance.transform.SetParent(hitInfo.collider.gameObject.transform);
                
                //raise children icons above everything else in scene
                foreach (Transform child in tacticalMarkerInstance.transform)
                {
                    child.Translate(0, minimapCameraXform.position.y - iconHeightOffset, 0);
                    var mr = child.GetComponent<MeshRenderer>() as MeshRenderer;
                    if(mr) mr.material.color = playerColor;
                }
            }
            else
            {
                //raycast hit nothing.
            }
        }
    }
}
