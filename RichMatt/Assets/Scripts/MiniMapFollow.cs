using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapFollow : MonoBehaviour
{
	
    [SerializeField] private bool rotateWithPlayer = true;
    [SerializeField] private int miniMapHeight = 1000;
    [SerializeField] private Transform targetTransform;

    private Transform origParent;
    private Quaternion originalRotation;

	// Use this for initialization
	void Start ()
	{
		targetTransform = targetTransform == null ? GameObject.FindGameObjectWithTag ("Player").transform : targetTransform ;
        origParent = transform.parent;
        originalRotation = transform.rotation;
	}
	
	// Update is called once per frame
	void LateUpdate ()
	{
        if(rotateWithPlayer)
        {
            transform.SetParent(targetTransform);
            //TODO orient rotation with character's if toggled
        }
        else
        {
            transform.rotation = originalRotation;
            transform.SetParent(origParent);
        }

        transform.localPosition = new Vector3(0, miniMapHeight, 0);
    }
}
