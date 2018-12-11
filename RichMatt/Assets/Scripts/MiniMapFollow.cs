using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapFollow : MonoBehaviour
{
	
    [SerializeField] private bool rotateWithPlayer = true;
    [SerializeField] private int miniMapHeight = 1000;
    private Transform origParent;
    [SerializeField] private Transform targetTransform;

	// Use this for initialization
	void Start ()
	{
		targetTransform = targetTransform == null ? GameObject.FindGameObjectWithTag ("Player").transform : targetTransform ;
        origParent = transform.parent;
	}
	
	// Update is called once per frame
	void LateUpdate ()
	{
        if(rotateWithPlayer)
        {
            transform.SetParent(targetTransform);
        }
        else
        {
            transform.SetParent(origParent);
        }

        transform.localPosition = new Vector3(0, miniMapHeight, 0);
    }
}
