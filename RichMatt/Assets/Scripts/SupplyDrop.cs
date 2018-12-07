﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 [RequireComponent(typeof(Rigidbody))]
public class SupplyDrop : MonoBehaviour
{
    
    [SerializeField] private SkyDivingStateENUM freefallingState = SkyDivingStateENUM.freeFalling;
    [SerializeField] private int terminalVelocity = -18;//should be negative, but will be remedied
    [SerializeField] private int parachuteTerminalVelocity = -9;
    [SerializeField] private float forwardMomentum = .03f;
    [Range(0, 1)]//after the object has fallen this percentage of the distance to the ground, pull the chute
    [SerializeField] private float deployParachuteDistancePercent = .9f;

    private Vector3 terminalVelocityVector;

    private int initialDistanceToGround = 0;//distance from instantiated point to ground
    private Animator anim;
    private Rigidbody rb;
    [SerializeField] private Parachute parachute;
    //[SerializeField] private bool parachuteDeployed = false;

    public GameObject[] supplies;//holder variable for supplies. probably scriptable objects or itemManagers
    
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
	void Start () {
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
        if (this.parachute == null) this.parachute = this.GetComponent<Parachute>();

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
                //Destroy parachute
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
                this.rb.AddForce(Vector3.forward * forwardMomentum, ForceMode.Impulse);
                break;

            case SkyDivingStateENUM.startLanded:
                parachute.DestroyParachute();//how the parachute is destroyed is up to the class implementation
                freefallingState = SkyDivingStateENUM.landed;
                break;

            case SkyDivingStateENUM.landed:
                //do stuff. like wait to be opened
                break;

            default:
                break;
        }

        //cap downward velocity
        this.rb.velocity = this.rb.velocity.y > terminalVelocity ? terminalVelocityVector : this.rb.velocity;
        
        //Debug.Log("Velocity: " + this.rb.velocity);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Terrain"))
        {
            freefallingState = SkyDivingStateENUM.startLanded;

        }
    }
}
