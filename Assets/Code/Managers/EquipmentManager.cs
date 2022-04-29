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
namespace Hardcode.ITFOD.Items
{
    public class EquipmentManager : MonoBehaviour
    {
        public delegate void OnEquipmentChanged(Equipment newItem, Equipment oldItem);

        #region Singleton

        public static EquipmentManager instance;

        #endregion

        [SerializeField] private Equipment[] currentEquipment;
        [SerializeField] private int numberOfSlots = 5;

        private Inventory inventory;
        public OnEquipmentChanged onEquipmentChanged;


        public void OnCreated()
        {
            instance = this;
            currentEquipment = new Equipment[numberOfSlots];
            inventory = InventoryManager.instance.inventory;
        }

        public void Equip(Equipment newItem)
        {
            var slotIndex = (int) newItem.slot;

            Equipment oldItem = null;

            if (currentEquipment[slotIndex] != null)
            {
                oldItem = currentEquipment[slotIndex];
                inventory.AddItem(oldItem);
            }

            currentEquipment[slotIndex] = newItem;
            onEquipmentChanged?.Invoke(newItem, oldItem);
        }

        public void Unequip(int slotIndex)
        {
            if (currentEquipment[slotIndex] != null)
            {
                var oldItem = currentEquipment[slotIndex];
                inventory.AddItem(oldItem);

                currentEquipment[slotIndex] = null;
                onEquipmentChanged?.Invoke(null, oldItem);
            }
        }
    }
}