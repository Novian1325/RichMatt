﻿using UnityEngine;

public class Interactable : MonoBehaviour {

    protected bool playerIsLookingAtObject = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public virtual void Interact(InteractionManager interactingObject)
    {
        //this method should probably be overridden by derived class, ie a vehicle should do something that an item does not
        System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();

        stringBuilder.Append(interactingObject.name);
        stringBuilder.Append(" is interacting with ");
        stringBuilder.Append(this.gameObject.name);

        Debug.Log(stringBuilder.ToString());
    }

    public virtual void PlayerIsLookingAtObject(bool b)
    {
        playerIsLookingAtObject = b;
    }

    public virtual bool GetPlayerIsLookingAtObject()
    {
        return playerIsLookingAtObject;
    }

    public virtual void ToggleTooltip(bool active)
    {

    }
    
}