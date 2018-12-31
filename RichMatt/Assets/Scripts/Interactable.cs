using UnityEngine;

public class Interactable : MonoBehaviour
{
    [Tooltip("UI Tooltip Prompt that gets displayed to Player.")]
    [SerializeField] protected GameObject tooltipObject;

    protected bool playerIsLookingAtObject = false;

    // Use this for initialization
    void Start ()
    {
		
	}

    protected virtual void HandleTooltip()
    {
        ToggleTooltip(playerIsLookingAtObject);

        if (playerIsLookingAtObject)
        {
            playerIsLookingAtObject = false;

        }

    }
	
	// Update is called once per frame
	protected void Update ()
    {
        //Update must be called from derived class!
        //Handle Tooltips
        HandleTooltip();
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
        if (tooltipObject)
        {
            tooltipObject.SetActive(active);
        }
        else
        {
            //Debug.Log("No tooltip on Interactable. Tooltips active: " + active);
        }
    }
    
}
