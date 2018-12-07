using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parachute : MonoBehaviour {

    private Animator anim;
    private MeshRenderer meshRenderer;

    public void DeployParachute()
    {
        meshRenderer.enabled = true;
        anim.SetTrigger("DeployChute");

    }


	// Use this for initialization
	void Start () {
        anim = this.gameObject.GetComponent<Animator>();
        meshRenderer = this.gameObject.GetComponent<MeshRenderer>();
        meshRenderer.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
