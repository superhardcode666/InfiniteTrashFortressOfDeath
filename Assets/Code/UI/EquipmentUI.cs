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
    public class EquipmentUI : MonoBehaviour
    {
        #region Field Declarations

        [SerializeField] private EquipmentManager equipmentManager;
        [SerializeField] private EquipmentSlotUI[] slots;
        [SerializeField] private Transform slotContainer;

        #endregion

        #region Start Up

        public void OnCreated()
        {
            equipmentManager.onEquipmentChanged += UpdateEquipmentUI;
            slots = slotContainer.GetComponentsInChildren<EquipmentSlotUI>();

            Debug.Log("<color=green>EquipmentUI:</color> Set up Dependencies, booting up.");
        }

        private void OnDestroy()
        {
            Debug.Log("<color=green>EquipmentUI:</color> Unsubbed from all Events, shutting down.");
        }

        private void UpdateEquipmentUI(Equipment newItem, Equipment oldItem)
        {
            if (newItem == null)
                slots[(int) oldItem.slot].ClearSlot();
            else
                slots[(int) newItem.slot].AddItem(newItem);
            Debug.Log("Updated EquipmentUI!");
        }

        #endregion
    }
}