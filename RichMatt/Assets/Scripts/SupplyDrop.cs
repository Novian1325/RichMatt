using UnityEngine;

namespace PolygonPilgrimage.BattleRoyaleKit
{
    [RequireComponent(typeof(Rigidbody))]
    public class SupplyDrop : BRS_Interactable
    {
        [Header("SupplyDrop")]
        //how many supply drops exist
        private static int supplyDropCount = 0;

        [SerializeField] private SkyDivingStateENUM freefallingState = SkyDivingStateENUM.freeFalling;

        [Tooltip("Fastest downward speed of object. MUST BE NEGATIVE.")]
        [Range(-99999, 0)]
        [SerializeField] private int terminalVelocity = -18;//should be negative, but will be remedied

        [Tooltip("Fastest downward speed of object when in parachute state. MUST BE NEGATIVE.")]
        [Range(-99999, 0)]
        [SerializeField] private int parachuteTerminalVelocity = -9;

        [Tooltip("How much physics force is applied to the Supply Drop to drift forward")]
        [SerializeField] private float forwardMomentum = .05f;

        [Range(.2f, 1)]//after the object is this percentage of the distance to the ground, pull the chute
        [Tooltip("At what percent of initial height should the parachute deploy at? Lower number means lower altitude.")]
        [SerializeField] private float deployParachuteDistancePercent = .9f;//lower number means lower altitude

        [Tooltip("Prefab of Parachute")]
        [SerializeField] private Parachute parachute;

        [Tooltip("Particle effects with sounds that play when this object is destroyed.")]
        [SerializeField] private GameObject destructionEffect;
        
        private Vector3 terminalVelocityVector;

        /// <summary>
        /// Distance from the ground to this object when it is created.
        /// </summary>
        private int initialDistanceToGround = 0;
        private Animator anim;
        private Rigidbody rb;

        /// <summary>
        /// Holder variable for supplies. probably scriptable objects or itemManagers
        /// </summary>
        [SerializeField] private GameObject[] supplies;//


        private void AddIconToMiniMap()
        {
            //TODO
        }

        /// <summary>
        /// behavior at the moment the free fall began
        /// </summary>
        private void StartFreeFalling()
        {
            initialDistanceToGround = BRS_Utility.GetDistanceToTerrain(this.transform.position);
            freefallingState = SkyDivingStateENUM.freeFalling;
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
            if (parachute) parachute.DestroyParachute();//how the parachute is destroyed is up to the class implementation
            freefallingState = SkyDivingStateENUM.landed;
            rb.freezeRotation = true;
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


        // Use this for initialization
        void Start()
        {
            //
            SupplyDropManager.supplyDropManagerInstance.AddSupplyDrop(this);

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
            this.anim = this.GetComponent<Animator>() as Animator;
            if (this.parachute == null) this.parachute = this.GetComponentInChildren<Parachute>();

            //set name
            SetName(this.gameObject);

        }

        // Update is called once per frame
        new void Update()
        {
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
                    //add icon to minimap
                    break;

                default:
                    break;
            }
        }

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
            }

            //cap downward velocity
            this.rb.velocity = this.rb.velocity.y < terminalVelocity ? terminalVelocityVector : this.rb.velocity;

            //Debug.Log("Velocity: " + this.rb.velocity + "/ terminal velocity: " + terminalVelocity);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (this.freefallingState == SkyDivingStateENUM.landed) return;
            else if (collision.gameObject.CompareTag("Terrain"))
            {
                freefallingState = SkyDivingStateENUM.startLanded;

            }
        }

        private void OnDestroy()
        {
            SupplyDropManager.supplyDropManagerInstance.RemoveSupplyDrop(this);

            //TODO Other things that happen when thing is destroyed

            //do animations
            if (anim) anim.SetTrigger("Destroy");

            //play sounds

            //remove icons from minimap
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
            //throw loot all over the ground like a maniac
            //remove icons and effects
            //play an effect
            Destroy(this.gameObject); //Destroy(this.gameObject, 3);

        }
    }    
}
