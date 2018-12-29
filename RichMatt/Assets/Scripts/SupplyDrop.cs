using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 [RequireComponent(typeof(Rigidbody))]
public class SupplyDrop : MonoBehaviour
{
    
    [SerializeField] private SkyDivingStateENUM freefallingState = SkyDivingStateENUM.freeFalling;

    [Tooltip("Fastest downward speed of object. MUST BE NEGATIVE.")]
    [SerializeField] private int terminalVelocity = -18;//should be negative, but will be remedied

    [Tooltip("Fastest downward speed of object when in parachute state. MUST BE NEGATIVE.")]
    [SerializeField] private int parachuteTerminalVelocity = -9;

    [Tooltip("How much physics force is applied to the Supply Drop to drift forward")]
    [SerializeField] private float forwardMomentum = .05f;
    
    [Tooltip("Prefab of Parachute")]
    [SerializeField] private Parachute parachute;

    [Tooltip("Particle effects with sounds that play when this object is destroyed.")]
    [SerializeField] private GameObject destructionEffect;

    [Range(.2f, 1)]//after the object is this percentage of the distance to the ground, pull the chute
    [Tooltip("At what percent of initial height should the parachute deploy at? Lower number means lower altitude.")]
    [SerializeField] private float deployParachuteDistancePercent = .9f;//lower number means lower altitude

    [Tooltip("Destroys the supply drop after this many seconds have passed")]
    [SerializeField] private int destroySupplyDropAfterTime = 300;//5 mins by default
    
    private Vector3 terminalVelocityVector;

    private int initialDistanceToGround = 0;//distance from instantiated point to ground
    private Animator anim;
    private Rigidbody rb;

    [SerializeField] private GameObject[] supplies;//holder variable for supplies. probably scriptable objects or itemManagers


    private void AddIconToMiniMap()
    {
        //TODO
    }

    private void StartFreeFalling()
    {
        initialDistanceToGround = PPBRS_Utility.GetDistanceToTerrain(this.transform.position);
        freefallingState = SkyDivingStateENUM.freeFalling;
    }

    private void FreeFalling()
    {
        //check distance to ground
        if (PPBRS_Utility.GetDistanceToTerrain(this.transform.position) < deployParachuteDistancePercent * initialDistanceToGround)
        {
            DeployParachute();

        }

    }

    private void StartLanded()
    {
        if (parachute) parachute.DestroyParachute();//how the parachute is destroyed is up to the class implementation
        freefallingState = SkyDivingStateENUM.landed;
        rb.freezeRotation = true;
        Destroy(this.gameObject, destroySupplyDropAfterTime);
    }


    private void DeployParachute()
    {
        //do it!
        parachute.DeployParachute();
        terminalVelocity = parachuteTerminalVelocity;
        terminalVelocityVector = new Vector3(0, terminalVelocity, 0);//convert to vector3
        freefallingState = SkyDivingStateENUM.parachuting;
    }

    private static int supplyDropCount = 0;

	// Use this for initialization
	void Start ()
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
        this.gameObject.name = "Supply Drop # " + ++supplyDropCount;//track name
		
	}
	
	// Update is called once per frame
	void Update () {

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

    public void Interact()
    {
        //throw loot all over the ground like a maniac
        //remove icons and effects
        //play an effect
        Destroy(this.gameObject);
        
    }

    private void OnDestroy()
    {
        SupplyDropManager.supplyDropManagerInstance.RemoveSupplyDrop(this);

        //TODO Other things that happen when thing is destroyed

        //do animations
        if(anim) anim.SetTrigger("Destroy");
        

        //play sounds

        //remove icons from minimap
    }
}
