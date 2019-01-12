using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BRS_SetDeathPose : MonoBehaviour
{
    private Animator anim;
    public int DeathPose;


	// Use this for initialization
	void Start ()
    {
        anim = gameObject.GetComponent<Animator>();
        anim.SetInteger("DeathPose", DeathPose);
	}
}
