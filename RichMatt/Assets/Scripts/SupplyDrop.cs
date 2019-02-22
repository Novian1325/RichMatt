using UnityEngine;

namespace PolygonPilgrimage.BattleRoyaleKit
{
    [RequireComponent(typeof(Rigidbody))]
    public class SupplyDrop : BRS_Interactable
    {
        //how many supply drops exist
        private static int supplyDropCount = 0;
        //reference to supply drop manager class
        private static SupplyDropManager supplyDropManager;

        [Header("---SupplyDrop---")]
        [SerializeField] private SkyDivingStateENUM freefallingState = SkyDivingStateENUM.freeFalling;

        [Header("---Physics---")]
        [Tooltip("Fastest downward speed of object. MUST BE NEGATIVE.")]
        [Range(-900, 0)]
        [SerializeField] private int terminalVelocity = -18;//should be negative, but will be remedied

        [Tooltip("Fastest downward speed of object when in parachute state. MUST BE NEGATIVE.")]
        [Range(-900, 0)]
        [SerializeField] private int parachuteTerminalVelocity = -9;

        [Tooltip("How much physics force is applied to the Supply Drop to drift forward")]
        [SerializeField] private float forwardMomentum = .05f;

        [Header("---Parachute---")]
        [Range(.2f, 1)]//after the object is this percentage of the distance to the ground, pull the chute
        [Tooltip("At what percent of initial height should the parachute deploy at? Lower number means lower altitude.")]
        [SerializeField] private float deployParachuteDistancePercent = .9f;//lower number means lower altitude

        [Tooltip("Prefab of Parachute")]
        [SerializeField] private Parachute parachute;

        [Tooltip("Particle effects with sounds that play when this object is destroyed.")]
        [SerializeField] private GameObject destructionEffect;

        [Header("---Destruction---")]
        [Tooltip("Destroys Supply Drops after this many seconds. Time starts counting when Supply Drop touches ground. Value <= 0 disables this.")]
        [SerializeField] private int destroyAfterSeconds_lifetime = -1; // value <= 0 means this will not take effect -- infinite lifespan
        [Tooltip("Destroys Supply Drops if they are outside Zone Wall. Value <= 0 disables this.")]
        [SerializeField] private int destroyAfterSeconds_outsideZoneWall = -1; //value <= 0 means this will not take effect -- infinite

        private Vector3 terminalVelocityVector;

        /// <summary>
        /// Distance from the ground to this object when it is created.
        /// </summary>
        private int initialDistanceToGround = 0;
        private Rigidbody rb;

        /// <summary>
        /// Holder variable for supplies. probably scriptable objects or itemManagers
        /// </summary>
        private GameObject[] supplies;//
        
        // Use this for initialization
        void Start()
        {
            //
            supplyDropManager = SupplyDropManager.supplyDropManagerInstance as SupplyDropManager;
            if (supplyDropManager) supplyDropManager.AddSupplyDrop(this);

            //init state
            freefallingState = SkyDivingStateENUM.startFreeFalling;

            //init variables
            terminalVelocity = -Mathf.Abs(terminalVelocity); //get the absolutely negative value for downwards velocity
            terminalVelocityVector = new Vector3(0, terminalVelocity, 0);//convert to vector3
            parachuteTerminalVelocity = -Mathf.Abs(parachuteTerminalVelocity); //get the absolutely negative value for downwards velocity
                                                                               //enforce parachuteTerminalVelocity being lower
            parachuteTerminalVelocity = parachuteTerminalVelocity > terminalVelocity ? parachuteTerminalVelocity : terminalVelocity + 1;
            
            //snag references
            this.rb = this.GetComponent<Rigidbody>() as Rigidbody;
            if (this.parachute == null) this.parachute = this.GetComponentInChildren<Parachute>();

            //set name
            SetName(this.gameObject);
        }

        // Update is called once per frame
        new void Update()
        {
            base.Update();//tooltip stuff

            switch (freefallingState)
            {
                case SkyDivingStateENUM.startFreeFalling:
                    StartFreeFalling();
                    break;
                case SkyDivingStateENUM.freeFalling:
                    FreeFalling();
                    break;

                case SkyDivingStateENUM.startparachute:
                    DeployParachute();
                    break;

                case SkyDivingStateENUM.parachuting:
                    break;

                case SkyDivingStateENUM.startLanded:
                    StartLanded();
                    break;

                case SkyDivingStateENUM.landed:
                    //do stuff. like wait to be opened
                    //release smoke and what not. 
                    break;

                default:
                    break;
            }//end switch
        }//end Update()

        private void FixedUpdate()
        {
            //apply physics
            switch (freefallingState)
            {
                case SkyDivingStateENUM.freeFalling:
                //fall
                case SkyDivingStateENUM.startparachute:
                //fall
                case SkyDivingStateENUM.parachuting:
                    //force forward
                    this.rb.AddForce(this.transform.forward * forwardMomentum, ForceMode.Impulse);
                    break;

                case SkyDivingStateENUM.startLanded:
                    break;

                case SkyDivingStateENUM.landed:
                    //do stuff. like wait to be opened
                    break;

                default:
                    break;
            }//end switch

            //cap downward velocity
            this.rb.velocity = this.rb.velocity.y < terminalVelocity ? terminalVelocityVector : this.rb.velocity;

            //Debug.Log("Velocity: " + this.rb.velocity + "/ terminal velocity: " + terminalVelocity);
        }//end FixedUpdate()

        private void OnCollisionEnter(Collision collision)
        {
            if (freefallingState == SkyDivingStateENUM.landed) return;
            else if (collision.gameObject.CompareTag("Terrain"))
            {
                freefallingState = SkyDivingStateENUM.startLanded;

            }
        }//end OnCollisionEnter

        private void OnDestroy()
        {
            if (supplyDropManager) supplyDropManager.RemoveSupplyDrop(this);

            //TODO Other things that happen when thing is destroyed

            //play sounds
        }//end OnDestroy

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("ZoneWall"))
            {
                if(destroyAfterSeconds_outsideZoneWall > 0)
                {
                    Destroy(this.gameObject, destroyAfterSeconds_outsideZoneWall);
                }
            }
        }

        /// <summary>
        /// behavior at the moment the free fall began
        /// </summary>
        private void StartFreeFalling()
        {
            initialDistanceToGround = BRS_Utility.GetDistanceToTerrain(this.transform.position);

            //what state to start in?
            if(initialDistanceToGround <= 5)
            {
                freefallingState = SkyDivingStateENUM.startLanded;
            }
            else
            {
                freefallingState = SkyDivingStateENUM.freeFalling;
            }
        }

        /// <summary>
        /// behavior when supply drop is falling
        /// </summary>
        private void FreeFalling()
        {
            //check distance to ground
            if (BRS_Utility.GetDistanceToTerrain(this.transform.position) < deployParachuteDistancePercent * initialDistanceToGround)
            {
                DeployParachute();
            }
        }

        /// <summary>
        /// Behavior the moment the supply drop touches the ground.
        /// </summary>
        private void StartLanded()
        {
            freefallingState = SkyDivingStateENUM.landed;
            if (parachute) parachute.DestroyParachute();//how the parachute is destroyed is up to the class implementation
            rb.freezeRotation = true;
            
            //handle destruction timer
            if(destroyAfterSeconds_lifetime > 0)
            {
                Destroy(this.gameObject, destroyAfterSeconds_lifetime);
            }
        }

        /// <summary>
        /// Handles physics and other things when parachute is deployed
        /// </summary>
        private void DeployParachute()
        {
            //do it!
            parachute.DeployParachute();
            terminalVelocity = parachuteTerminalVelocity;
            terminalVelocityVector = new Vector3(0, terminalVelocity, 0);//convert to vector3
            freefallingState = SkyDivingStateENUM.parachuting;
        }

        /// <summary>
        /// Sets the name of the supply drop so the developer can find it in hierarchy.
        /// </summary>
        /// <param name="go"></param>
        private static void SetName(GameObject go)
        {
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();

            stringBuilder.Append("Supply Drop # ");
            stringBuilder.Append(++supplyDropCount);//track name

            go.name = stringBuilder.ToString();//faster than concatenation +
        }

        /// <summary>
        /// Do the behavior. Cue interaction.
        /// </summary>
        /// <param name="im"></param>
        override public void Interact(BRS_InteractionManager im)
        {
            base.Interact(im);//send output to Console

            //throw loot all over the ground like a maniac
            //play an effect
            Debug.Log("Destroying Supply Drop.");
            Destroy(this.gameObject); //Destroy(this.gameObject, 3);
        }
    }    
}
