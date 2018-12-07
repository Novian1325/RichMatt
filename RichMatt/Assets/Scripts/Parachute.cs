using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parachute : MonoBehaviour {

    private Animator anim;

    public void DeployParachute()
    {
        anim.SetTrigger("DeployChute");

    }


	// Use this for initialization
	void Start () {
        anim = this.gameObject.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
