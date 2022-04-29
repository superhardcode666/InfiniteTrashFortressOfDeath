using Hardcode.ITFOD.Items;
using UnityEngine;

/*      __  _____    ____  ____ 
       / / / /   |  / __ \/ __ \
      / /_/ / /| | / /_/ / / / /
     / __  / ___ |/ _, _/ /_/ / 
    /_/_/_/_/__|_/_/_|_/_____/_ 
      / ____/ __ \/ __ \/ ____/ 
     / /   / / / / / / / __/    
    / /___/ /_/ / /_/ / /___    
    \____/\____/_____/_____/ 
        アンフェタミンを燃料
*/
namespace Hardcode.ITFOD.UI
{
    public class InventoryUI : MonoBehaviour
    {
        private void UpdateInventoryUI()
        {
            for (var i = 0; i < slots.Length; i++)
                if (i < inventory.items.Count)
                    slots[i].AddItem(inventory.items[i]);
                else
                    slots[i].ClearSlot();
        }

        #region Start Up

        public void OnCreated()
        {
            inventory = InventoryManager.instance.inventory;
            inventory.onItemChangedCallback += UpdateInventoryUI;

            slots = inventorySlotContainer.GetComponentsInChildren<InventorySlotUI>();

            Debug.Log("<color=cyan>InventoryUI:</color> Set up Dependencies, booting up.");
        }

        private void OnDestroy()
        {
            inventory.onItemChangedCallback -= UpdateInventoryUI;

            Debug.Log("<color=cyan>InventoryUI:</color> Unsubbed from all Events, shutting down.");
        }

        #endregion

        #region Field Declarations

        private Inventory inventory;
        private InventorySlotUI[] slots;

        public Transform inventorySlotContainer;

        #endregion
    }
}