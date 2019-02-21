using System.Collections.Generic;
using UnityEngine;

namespace PolygonPilgrimage.BattleRoyaleKit
{
    public class BRS_InventoryManager : MonoBehaviour
    {
        [SerializeField] private int maxInventorySlots = 4;

        private List<BRS_InventorySlot> inventoryList = new List<BRS_InventorySlot>();//or whatever base type of item you use in your project
        private int currentUsedSlots = 0;
        
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public bool AddToInventory(BRS_ItemManager newItem)
        {
            var addSuccessful = false;

            //is inventory at max capacity?
            if(currentUsedSlots == maxInventorySlots)
            {
                ;
            }
            else
            {
                //check if item already exists in inventory
                //if so, is there more room in the stack?
                //add to stack
                //is there any left over?
                //is there an empty slot?
                //put in empty slot
                //is there any left over?
                //repeat
                //is there any left over?
                //spawn a new item manager with the remaining 
                //GameObject newItemMan = Instantiate(newItem.gameObject, right in front of your face on the ground)
                //newItemMan.GetComponent<BRS_ItemManager>().quantity = whatever the hell quantity is left over;

            }


            return addSuccessful;
        }

    }//end class
}//end namespace
