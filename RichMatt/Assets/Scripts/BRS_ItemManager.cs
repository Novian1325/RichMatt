using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace PolygonPilgrimage.BattleRoyaleKit
{
    public class BRS_ItemManager : BRS_Interactable
    {
        public static readonly char interactButtonPrompt = 'F';//make sure this matches the button in the Interaction Manager!

        [Header("---Scriptable Object---")]
        [Tooltip("This is the slot for the Scriptable Object that holds the item data.")]
        public Item scriptableObject_Item;

        [Tooltip("Quantity of this item in stack.")]
        public int itemQuantity;

        [Header("---Setup UI References---")]
        [Tooltip("The background image that changes color based on item rarity.")]
        [SerializeField] private RawImage coloredBackground;

        [Tooltip("Text that displays the item's rarity.")]
        [SerializeField] private TextMeshProUGUI TMP_ItemRarity;

        [Tooltip("The single-letter prompt that tells the Player which button to press. Should be same as Interact in Input Manager.")]
        [SerializeField] private TextMeshProUGUI TMP_PickUpButton;

        [Tooltip("This text displays the Item Type, same as Class Type.")]
        [SerializeField] private TextMeshProUGUI TMP_ItemType;

        [Tooltip("Text that displays the name of the item.")]
        [SerializeField] private TextMeshProUGUI TMP_ItemName;

        [Tooltip("Text that displays quantity of item in this stack. Can be greater than stack amount.")]
        [SerializeField] private TextMeshProUGUI TMP_ItemAmount;

        [Header("---Model Parent---")]
        [SerializeField] private Transform itemModelHolder;//used to modify rotation of object before model is spawned

        // Use this for initialization
        void Start()
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

            //quantity cannot be less than 1
            if (itemQuantity < 1)
            {
                itemQuantity = 1;
            }

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


}