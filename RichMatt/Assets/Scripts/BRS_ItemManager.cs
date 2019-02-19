using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BRS_ItemManager : BRS_Interactable
{
    public static readonly char interactButtonPrompt = 'f';//make sure this matches the button in the Interaction Manager!
    
	[Header("---Scriptable Object---")]
    public Item scriptableObject_Item;
    public int itemQuantity;

    [Header("--Setup UI References---")]
    [SerializeField] private RawImage coloredBackground;
    [SerializeField] private TextMeshProUGUI TMP_PickUpButton;
	[SerializeField] private TextMeshProUGUI TMP_ItemType;
	[SerializeField] private TextMeshProUGUI TMP_ItemName;
	[SerializeField] private TextMeshProUGUI TMP_ItemRarity;
    [SerializeField] private TextMeshProUGUI TMP_ItemAmount;

    [Header("---Model Parent---")]
    [SerializeField] private Transform itemModelHolder;//used to modify rotation of object before model is spawned
        
    // Use this for initialization
    void Start ()
    {
        if (scriptableObject_Item) InitFromSO();
        else Debug.LogError("Error! No Scriptable Object Loaded. What am I???");
    }

    private new void Update()
    {
        base.Update();
    }

    /// <summary>
    /// init from scriptable object if provided
    /// </summary>
    private void InitFromSO()
    {
        //create the model
        Instantiate(scriptableObject_Item.itemModel, itemModelHolder);

        //fill UI elements
        TMP_ItemName.text = scriptableObject_Item.itemName;//name
        TMP_PickUpButton.text = interactButtonPrompt.ToString();//button prompt
        TMP_ItemType.text = scriptableObject_Item.GetType().ToString();
        TMP_ItemRarity.text = scriptableObject_Item.itemRarity.ToString();
        TMP_ItemAmount.text = itemQuantity.ToString();
        coloredBackground.color = Color_Rarity.GetRarityColor(scriptableObject_Item.itemRarity);
    }

    protected override void HandleTooltip()
    {
        base.HandleTooltip();
    }

    public override void Interact(BRS_InteractionManager actor)
    {
        base.Interact(actor);
    }

    public override void PlayerIsLookingAtObject(bool b)
    {
        base.PlayerIsLookingAtObject(b);
    }

    public override bool GetPlayerIsLookingAtObject()
    {
        return base.GetPlayerIsLookingAtObject();
    }

    public override void ToggleTooltip(bool active)
    {
        base.ToggleTooltip(active);
    }
}
