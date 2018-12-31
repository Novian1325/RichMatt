using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorHandler : Interactable {
    [Header("Door Handler")]

    [Tooltip("Current state the door is in.")]
    [SerializeField] bool doorOpen = false;

    [SerializeField] bool doorOpensBothWays = false;
        
    private Animator animator;
    private Transform xform;
    
    // Use this for initialization
    void Start () {

        animator = this.GetComponent<Animator>();
        xform = this.transform;//cache for performance
       		
	}

    new private void Update()
    {
        //handleTooltips
        HandleTooltip();

        if (doorOpen)
        {
            OpenDoor();

        }
        else
        {
            CloseDoor();
        }

        Debug.Log("Door Open Angle: " + xform.eulerAngles.y);


    }

    public override void Interact(InteractionManager interactingObject)
    {
        //base.Interact(interactingObject); //posts info to Debug.Log
        if (doorOpensBothWays && !doorOpen)
        {
            //determine which way the door should open
            float angleOfPlayerToDoor = Vector3.Angle(interactingObject.transform.position, xform.position); 
            

        }

        //actual interaction stuff here
        doorOpen = !doorOpen;
        

    }

    private void CloseDoor()
    {
        animator.SetBool("DoopOpen", false);
        

    }

    private void OpenDoor()
    {

        animator.SetBool("DoopOpen", true);

    }
}
