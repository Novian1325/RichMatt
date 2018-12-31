using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour {

    [SerializeField] private bool DEBUG = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Interact(InteractionManager interactingObject)
    {
        if (DEBUG)
        {
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();

            stringBuilder.Append(interactingObject.name);
            stringBuilder.Append(" is interacting with ");
            stringBuilder.Append(this.gameObject.name);

            Debug.Log(stringBuilder.ToString());
        }

        SendMessage("OnInteract", interactingObject);
    }

    public void ToggleToolTipVisibility(bool active)
    {
        if (DEBUG)
        {
            Debug.Log("Tooltip is visible: " + active);
        }

        SendMessage("ToggleToolTipVisibility", active);

    }
}
