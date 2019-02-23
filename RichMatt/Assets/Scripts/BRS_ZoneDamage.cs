using UnityEngine;
using UnityEngine.PostProcessing;
//---------------------------------------------------------------------------------------------------
//This script is provided as a part of The Polygon Pilgrimage
//Subscribe to https://www.youtube.com/user/mentallic3d for more great tutorials and helpful scripts!
//---------------------------------------------------------------------------------------------------
namespace PolygonPilgrimage.BattleRoyaleKit
{
    public class BRS_ZoneDamage : MonoBehaviour
    {

        [Header("---Post Processing---")]
        [Tooltip("Post Processing Profile to use when OUTSIDE of the Zone Wall.")]
        [SerializeField] private PostProcessingProfile outsideZonePPP;

        [Tooltip("Post Processing Profile to use when INSIDE of the Zone Wall.")]
        [SerializeField] private PostProcessingProfile standardPPP;

        [Tooltip("The Camera Object whose PostProcessing Behavior will be interchanged.")]
        [SerializeField] private GameObject cameraToOverride;


        [Header("---Zone Damage Parameters---")]
        public float TickRate = 3.0f;
        public int TickDamage = 1;

        //Future values
        //For each succesive zone change we will damage
        //the player more for being outside the zone
        //public int zoneDamageMultiplier;

        [Header("---Player Health Manager Handle---")]
        public GameObject _BRS_Mechanics;
        public BRS_PlayerHealthManager _PHM;

        //Are we in the Zone?
        /// <summary>
        /// Is this Behavior inside the bounds of the Zone Wall?
        /// </summary>
        private bool inZone;

        /// <summary>
        /// Post Processing Behavior that is on the same GameObject as the camera.
        /// </summary>
        private PostProcessingBehaviour CamPPB;

        /// <summary>
        /// For DEMO purposes ONLY.  Reset our Health to full when we re-enter the Zone!
        /// </summary>
        private bool DebugHealth = true;

        void Start()
        {
            //Get a handle to the Player
            player = GameObject.FindGameObjectWithTag("Player");
            CamPPB = GameObject.Find("FirstPersonCharacter").GetComponent<PostProcessingBehaviour>();
            playername = player.transform.name;

            //Get a handle to the Player Health Manager
            _PHM = _BRS_Mechanics.GetComponent<BRS_PlayerHealthManager>();
            
        }

        private void Update()
        {
            HandleZoneDamage();
        }

        void OnTriggerExit(Collider col)
        {
            //If we leave the zone, we will be damaged!
            if (col.transform.name == playername)
            {
                CamPPB.profile = outsideZonePPP;
                inZone = false;
            }
        }

        void OnTriggerEnter(Collider col)
        {
            //If we are inside the zone, all is good!
            if (col.transform.name == playername)
            {
                CamPPB.profile = standardPPP;
                inZone = true;
            }
        }

        void HandleZoneDamage()
        {
            if (!inZone)
            {
                //Damage the player [TickDamage] amount
                _PHM.ChangeHealth(-TickDamage);
            }
            else if (DebugHealth)
            {
                _PHM.ChangeHealth(100);
            }
        }
    }

}
