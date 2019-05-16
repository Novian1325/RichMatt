using UnityEngine;
using UnityEngine.PostProcessing;
//---------------------------------------------------------------------------------------------------
//This script is provided as a part of The Polygon Pilgrimage
//Subscribe to https://www.youtube.com/user/mentallic3d for more great tutorials and helpful scripts!
//---------------------------------------------------------------------------------------------------
namespace PolygonPilgrimage.BattleRoyaleKit
{
    /// <summary>
    /// This class handles checking if the behavior is outside of the bounds and deals damage accordingly.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class BRS_ZoneDamage : MonoBehaviour
    {
        #region static variables
        /// <summary>
        /// For DEMO purposes ONLY.  Reset our Health to full when we re-enter the Zone!
        /// </summary>
        private static readonly bool _DebugHealth = false;

        /// <summary>
        /// The GameObject with Colliders that represent the bounds of the Zone Wall.
        /// </summary>
        private static GameObject _zoneWallObject;

        /// <summary>
        /// Class that controls manipulating the bounds of the Zone Wall Object.
        /// </summary>
        private static BRS_ZoneWallManager _zoneWallManager;
        
        /// <summary>
        /// Transform attached to this GO.
        /// </summary>
        private Transform cachedTransform;

        /// <summary>
        /// Collection of the shrink phases
        /// </summary>
        private static ShrinkPhase[] _shrinkPhaseArray;

        #endregion

        #region Visible In Inspector
        /// <summary>
        /// Post Processing Profile to use when OUTSIDE of the Zone Wall. 
        /// </summary>
        [Header("---Post Processing---")]
        [Tooltip("Post Processing Profile to use when OUTSIDE of the Zone Wall.")]
        [SerializeField] private PostProcessingProfile outsideZonePPP;
        
        /// <summary>
        /// The Camera Object whose PostProcessing Behavior will be interchanged. If left empty (like for Bots) Console will not complain.
        /// </summary>
        [Tooltip("The Camera Object whose PostProcessing Behavior will be interchanged. If left empty (like for Bots) Console will not complain.")]
        [SerializeField] private GameObject cameraToOverride;

        /// <summary>
        /// The object that should handle the dealing of the damage.
        /// </summary>
        [Header("---UI References---")]
        [Tooltip("The object that should handle the dealing of the damage.")]
        [SerializeField] private BRS_PlayerHealthManager healthManager;

        /// <summary>
        /// When Player is outside of the zone wall, this line will appear and guide the player back to the safe zone.
        /// </summary>
        [SerializeField]
        private LineRenderer linePointingToCircleCenter;

        #endregion

        #region Internal Private Variables

        /// <summary>
        /// Post Processing Profile to use when INSIDE of the Zone Wall.
        /// </summary>
        private PostProcessingProfile standardPPP;

        /// <summary>
        /// Is this Behavior inside the bounds of the Zone Wall?
        /// </summary>
        private bool inZone;
        
        /// <summary>
        /// Post Processing Behavior that is on the same GameObject as the camera.
        /// </summary>
        private PostProcessingBehaviour camPPB;

        /// <summary>
        /// What Time the next damage tick will occur.
        /// </summary>
        private float nextDamageTickTime;

        #endregion

        private void Awake()
        {
            //handle static references
            InitStaticReferences();
        }

        void Start()
        {
            //handle instance references
            VerifyReferences();
        }
    
        //called once per frame
        private void Update()
        {
            if (!inZone)//if not in the zone
            {
                HandleZoneDamage();
                DrawLineToCircle();
            }
            else if (_DebugHealth)//if inside zone and debugging health....
            {
                //increase health (for debugging purposes)
                healthManager.ChangeHealth(healthManager.GetMaxHealth());
            }
        }

        void OnTriggerExit(Collider col)
        {
            //If we leave the zone, we will be damaged!
            if (col.gameObject == _zoneWallObject)
            {
                inZone = false;
                //set the next Time the healthManager should be dealt a damage tick
                nextDamageTickTime += 1 / _shrinkPhaseArray[_zoneWallManager.GetShrinkPhase()].ticksPerSecond;
                //change visuals
                if (camPPB)
                {
                    camPPB.profile = outsideZonePPP;
                }
            
            }
        }

        void OnTriggerEnter(Collider col)
        {
            //If we are inside the zone, all is good!
            if (col.gameObject == _zoneWallObject)
            {
                inZone = true;
                //change visuals
                if (standardPPP)
                {
                    camPPB.profile = standardPPP;
                }
            }

            linePointingToCircleCenter.enabled = false;
        }

        private void DrawLineToCircle()
        {
            if (!linePointingToCircleCenter.enabled)
            {
                linePointingToCircleCenter.enabled = true;
            }
            
            //starting point is player's current position, drawn at appropriate height
            var pointPosition = cachedTransform.position;
            pointPosition.y = _zoneWallManager.GetDrawHeight();//set height
            linePointingToCircleCenter.SetPosition(0, pointPosition);//set starting point

            //set endoing point, a point on the edge of the circle
            var playerPositionRelativeToWall = _zoneWallObject.transform.InverseTransformPoint(cachedTransform.position);//convert world space point of player into local space of zoneWall circle
            var angle = Mathf.Atan2(playerPositionRelativeToWall.z, playerPositionRelativeToWall.x) * Mathf.Rad2Deg;//get the angle between Player and centerpoint of zone wall circle
            var radius = _zoneWallManager.GetCurrentRadius();//get radius of circle

            var zoneWallPosition = _zoneWallObject.transform.position;//get x,z coordinates of circle
            //yay trig!
            pointPosition.x = zoneWallPosition.x + radius * Mathf.Cos(angle * (Mathf.PI / 180));//get x coordinate of point on edge of circle
            pointPosition.z = zoneWallPosition.z + radius * Mathf.Sin(angle * (Mathf.PI / 180));//get y coordinate of point on edge of circle

            linePointingToCircleCenter.SetPosition(1, pointPosition);//set endpoint on edge of circle
        }

        /// <summary>
        /// Finds references to the Zone Wall objects.
        /// </summary>
        private static void InitStaticReferences()
        {
            if (!_zoneWallManager)
            {
                //find a reference to the GameObject with a tag
                _zoneWallObject = GameObject.FindGameObjectWithTag("ZoneWall") as GameObject;

                if (_zoneWallObject)//if it exists
                {
                    //get a reference to the Zone Wall Manager script
                    _zoneWallManager = _zoneWallObject.GetComponent<BRS_ZoneWallManager>() as BRS_ZoneWallManager;
                    _shrinkPhaseArray = _zoneWallManager.GetShrinkPhases();

                    //if it does not exist... complain
                    if (!_zoneWallManager) Debug.LogError("ERROR! ZoneDamage behavior cannot find zoneWallManager Component on " + _zoneWallObject.name);
                }
                else
                {
                    //complain
                    Debug.LogError("ERROR! No GameObject in Scene with tag 'ZoneWall'!");
                }
            }
        }

        /// <summary>
        /// Gets references that were not set and complains if they cannot be found.
        /// </summary>
        private void VerifyReferences()
        {
            if (cameraToOverride)
            {
                //this Behavior is usually on the same GameObject as the Camera.
                camPPB = cameraToOverride.GetComponent<PostProcessingBehaviour>();
                //get the normal behavior
                standardPPP = camPPB.profile;
            }
            else
            {
                Debug.Log("No Camera to Override on this Component. No visuals will occur if this object leaves the ZoneWall.", this.gameObject);
            }

            //verify reference to health manager
            if (!healthManager)
            {
                healthManager = GetComponent<BRS_PlayerHealthManager>() as BRS_PlayerHealthManager;

                if (!healthManager)
                {
                    Debug.LogError("ERROR! Reference to HealthManager not set and cannot be located on " + this.gameObject.name, this.gameObject);
                }
            }

            if (!linePointingToCircleCenter)
            {
                Debug.LogError("ERROR! Line Renderer not assigned on Player.", this);
            }
            linePointingToCircleCenter.enabled = false; //start with object disabled;

            cachedTransform = transform;
        }
        
        /// <summary>
        /// Handle the sending of damage.
        /// </summary>
        private void HandleZoneDamage()
        {
            if (Time.time > nextDamageTickTime)//if it's time to deal a damage tick
            {
                //Damage the healthManager depending on the phase of the zone wall
                healthManager.ChangeHealth(-_shrinkPhaseArray[_zoneWallManager.GetShrinkPhase()].damagePerTick);//DEAL DAMAGE

                //set the next Time to deal a tick damage
                nextDamageTickTime += 1 / _shrinkPhaseArray[_zoneWallManager.GetShrinkPhase()].ticksPerSecond;
            }
        }
    }

}
