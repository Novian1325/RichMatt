using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour {
    [SerializeField] private Vector3 rotateSpeed;

    private Transform xform;//cache for performance

	// Use this for initialization
	void Start () {
        xform = this.transform;
	}
	
	// Update is called once per frame
	void Update () {
        xform.Rotate(rotateSpeed * Time.deltaTime, Space.World);
		
	}
}
