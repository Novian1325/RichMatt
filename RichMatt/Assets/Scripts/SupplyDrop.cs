using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupplyDrop : MonoBehaviour {

    //[SerializeField] private float percent
    [SerializeField] private int terminalVelocity = -18;//should be negative, but will be remedied
    private Vector3 terminalVelocityVector;

    private int initialDistanceToGround = 0;//distance from instantiated point to ground
    private Animator anim;
    private Rigidbody rigidbody;
    

	// Use this for initialization
	void Start () {
        terminalVelocity = -Mathf.Abs(terminalVelocity); //get the absolutely negative value
        terminalVelocityVector = new Vector3(0, terminalVelocity, 0);//convert to vector3

        //snag references
        this.rigidbody = this.GetComponent<Rigidbody>() as Rigidbody;
        this.anim = this.GetComponent<Animator>() as Animator;
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private int GetDistanceToGround()
    {
        int distanceToGround = 0;
        //TODO
        return distanceToGround;
    }

    private void FixedUpdate()
    {
        //cap downward velocity
        this.rigidbody.velocity = this.rigidbody.velocity.y > terminalVelocity ? terminalVelocityVector : this.rigidbody.velocity;
    }
}
