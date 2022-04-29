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
    /// <summary>
    ///     Base Class for all Items to derive from
    /// </summary>
    /// <remarks>
    ///     Contains all basic fields like Sprite, Name, Amount, Value (Ingame Coins), Whether its stackable and Item Type Enum
    /// </remarks>
    public class Item : ScriptableObject
    {
        #region Field Declarations

        public Sprite sprite;
        public string itemName = "New Item";
        public string useText = "put appropiate verb here";
        public int amount = 1;
        public int value;
        public string flavorText = "This is an item.";

        public bool isStackable = true;
        public bool isCursed;

        public ItemType itemType;

        protected void RemoveFromInventory()
        {
            InventoryManager.instance.inventory.RemoveItem(this);
        }

        public virtual void Use()
        {
        }

        public virtual string GetItemStats()
        {
            return "MYSTERIOUS!";
        }

        #endregion
    }

    public enum ItemType
    {
        Consumable,
        Weapon,
        Armor
    }
}