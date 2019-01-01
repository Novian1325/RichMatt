using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorHandler : Interactable {
    [Header("Door Handler")]

    [Tooltip("Current state the door is in.")]
    [SerializeField] bool doorOpen = false;

    [SerializeField] bool doorOpensBackward = false;
        
    private Animator animator;
    private Transform xform;
    private Vector3 actorDirection;
    
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

        //Debug.Log("Door Open Angle: " + xform.eulerAngles.y);


    }

    public override void Interact(InteractionManager interactingObject)
    {
        //actual interaction stuff here
        doorOpen = !doorOpen;

        if (doorOpensBackward)
        {
            actorDirection = interactingObject.transform.forward;
        }
        

    }

    private void CloseDoor()
    {
        animator.SetBool("DoorOpen", false);


    }

    private void OpenDoor()
    {

        animator.SetBool("DoorOpen", true);
        animator.SetBool("OpenBackward", DetermineDoorOpenDirection());

    }

    private bool DetermineDoorOpenDirection()
    {
        if (!doorOpensBackward) return false;

        bool openDoorBackwards = true;
        float angleOfPlayerToDoor = Vector3.Angle(actorDirection, xform.forward);

        Debug.Log(angleOfPlayerToDoor);

        if(angleOfPlayerToDoor < 90)
        {
            openDoorBackwards = false;
        }
        else
        {
            openDoorBackwards = true;
        }

        return openDoorBackwards;
        
    }

}
