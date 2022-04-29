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
    public class Equipment : Item
    {
        public int armorModifier;
        public int damageModifier;
        public EquipmentSlot slot;

        public override void Use()
        {
            Debug.Log($"Trying to equip {itemName}");
            EquipmentManager.instance.Equip(this);
            RemoveFromInventory();
        }

        public override string GetItemStats()
        {
            return $"ATK: <color=green>+{damageModifier}</color> DEF: <color=green>+{armorModifier}</color>";
        }
    }

    public enum EquipmentSlot
    {
        Head,
        Chest,
        LeftHand,
        RightHand,
        Feet
    }
}