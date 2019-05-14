using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace PolygonPilgrimage.BattleRoyaleKit
{
    /// <summary>
    /// Item Managers exist in the world. They display a ToolTip when looked at and show information to the Player. Can also be Interacted with.
    /// </summary>
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
            TMP_ItemType.text = scriptableObject_Item.GetType().ToString();//item type (literally, the typeof())
            TMP_ItemRarity.text = scriptableObject_Item.itemRarity.ToString();//rarity
            TMP_ItemAmount.text = itemQuantity.ToString();//quantity amount
            coloredBackground.color = Color_Rarity.GetRarityColor(scriptableObject_Item.itemRarity);//background color
        }

        protected override void HandleTooltip()
        {
            base.HandleTooltip();
        }

        public override void Interact(BRS_InteractionManager actor)
        {
            base.Interact(actor);//will output info to Console

            //tell actor's inventory manager that they picked up an item
            var inventoryMan = actor.GetComponent<BRS_InventoryManager>() as BRS_InventoryManager;

            if (inventoryMan)//if the Component exists
            {
                if (inventoryMan.AddToInventory(this))
                {
                    //item was successfully added!
                    Debug.Log("Item was successfully added to Inventory!");
                    //seppuko!
                    //Destroy(this.gameObject);
                }
                else
                {
                    Debug.Log("Item was not added properly -- left on ground.", gameObject);
                }

            }

            else//complain that Component doesn't exist, as now there is nothing left to do.
            {
                Debug.LogError("ERROR! No Inventory Manager on Actor. What should this Interactable do when an ItemManager is interacted with?", gameObject);
            }

        }

        public override void PlayerIsLookingAtObject(bool b)
        {
            base.PlayerIsLookingAtObject(b);
        }

        public override bool GetPlayerIsLookingAtObject()
        {
            return base.GetPlayerIsLookingAtObject();
        }

    }//end class
}//end namespace
