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
        /// Collection of the shrink phases
        /// </summary>
        private static ShrinkPhase[] shrinkPhaseArray;

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
        [Header("---Player Health Manager Handle---")]
        [Tooltip("The object that should handle the dealing of the damage.")]
        [SerializeField] private BRS_PlayerHealthManager healthManager;

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
            HandleZoneDamage();
        }

        void OnTriggerExit(Collider col)
        {
            //If we leave the zone, we will be damaged!
            if (col.gameObject == _zoneWallObject)
            {
                inZone = false;
                //set the next Time the healthManager should be dealt a damage tick
                nextDamageTickTime += 1 / shrinkPhaseArray[_zoneWallManager.GetShrinkPhase()].ticksPerSecond;
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
        }
        
        /// <summary>
        /// Handle the sending of damage.
        /// </summary>
        private void HandleZoneDamage()
        {
            if (!inZone)
            {
                if (Time.time > nextDamageTickTime)//if it's time to deal a damage tick
                {
                    //Damage the healthManager depending on the phase of the zone wall
                    healthManager.ChangeHealth(shrinkPhaseArray[_zoneWallManager.GetShrinkPhase()].damagePerTick);
                    //set the next Time to deal a tick damage
                    nextDamageTickTime += 1 / shrinkPhaseArray[_zoneWallManager.GetShrinkPhase()].ticksPerSecond;
                }
            }
            else if (_DebugHealth)//if inside zone and debugging health....
            {
                //increase health (for debugging purposes)
                healthManager.ChangeHealth(100);
            }
        }
    }

}
