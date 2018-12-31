using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManager : MonoBehaviour {


    private Transform playerCameraXform;
    private readonly int interactionRaycastLimit = 100;

    //Variables for raycasting tooltips
    [SerializeField] private List<Interactable> interactableObjectsWithinRange = new List<Interactable>();

    //variables for inventory manager
    //private InventoryManager inventoryManager;
    private Interactable interactablePlayerIsLookingAt;


    // Use this for initialization
    void Start () {
        
        playerCameraXform = Camera.main.transform;

    }
	
	// Update is called once per frame
	void Update ()
    {
        //show tooltip to player if w/n range and looking at model
        HandleToolTipRaycasting();

        //handle interactions
        if (Input.GetButtonDown("Interact"))
        {
            if (!interactablePlayerIsLookingAt)
            {
                Debug.Log("Interaction button pressed. Player is not looking at anything interactable.");
            }
            else
            {
                interactablePlayerIsLookingAt.Interact(this);
            }

        }

    }

    private void HandleToolTipRaycasting()
    {
        if (interactableObjectsWithinRange.Count > 0)
        {
            Interactable interactable = WhatIsPlayerLookingAt(); 
            //make sure player can only interact with things within their range

            if (interactable)
            {
                if (interactableObjectsWithinRange.Contains(interactable))
                {
                    interactable.ToggleToolTipVisibility(true);
                }
                
            }
            
        }
        else
        {//there is nothing nearby for the player to look at
            interactablePlayerIsLookingAt = null;
        }
    }

    private Interactable WhatIsPlayerLookingAt()
    {
        //returns the Interactable component that the player is looking at, if any

        //check to see if player is looking at interactable object's model
        RaycastHit hitInfo;

        //shoot a raycast from the cameras position forward, store the info, limit the ray to this length, use normal raycast layers, ignore triggerColliders
        if (Physics.Raycast(new Ray(playerCameraXform.position, playerCameraXform.forward), out hitInfo, interactionRaycastLimit, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
        {
            //Debug.Log("Raycast hit: " + hitInfo.collider.gameObject.name);
            //is the player looking at the item model?
            if (hitInfo.collider.CompareTag("Interactable"))
            {
                return hitInfo.collider.GetComponent<Interactable>();
            }
            else
            {
                //Debug.Log("Raycast did not hit an ItemModel.");
            }
        }
        else
        {
            //Debug.Log("Raycast hit nothing.");
        }

        return null;

    }

    public void OnTriggerEnter(Collider triggerVolume)
    {
        Debug.Log("ONTriggerEnter.");
        if (triggerVolume.CompareTag("Interactable"))
        {
            Interactable interactable = triggerVolume.GetComponent<Interactable>();
            if (!interactable)//check against null

            {
                Debug.LogError("ERROR! No Interactable Component on object that is tagged \"Interactable\"!");
            }
            else
            {
                //if list does not already contain, then add
                if (!interactableObjectsWithinRange.Contains(interactable))
                    interactableObjectsWithinRange.Add(interactable);//add game object to list

            }


        }
    }

    public void OnTriggerStay(Collider triggerVolume)
    {

    }

    public void OnTriggerExit(Collider triggerVolume)
    {
        if (triggerVolume.CompareTag("Interactable"))
        {
            Interactable interactable = triggerVolume.GetComponent<Interactable>();

            interactable.ToggleToolTipVisibility(false);//stop showing item tooltip
            interactableObjectsWithinRange.Remove(interactable);//remove game object from list
        }

    }
}
