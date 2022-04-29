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
    ///     Scriptable Object for all Consumables
    /// </summary>
    /// <remarks>
    ///     Contains Consumable Type and the amount of Health restored
    ///     Keys should have their own Class i guess
    /// </remarks>
    [CreateAssetMenu(fileName = "New Consumable Item", menuName = "SUPERETERNALDEATH/Consumable")]
    public class Consumable : Item
    {
        #region Field Declarations

        public int healthAmount;
        public bool isPoisoned;

        public enum ConsumableType
        {
            Potion,
            Key
        }

        public ConsumableType consumableType;

        public Consumable()
        {
            itemName = "New Consumable";
            itemType = ItemType.Consumable;
        }

        public override void Use()
        {
            GameEvents.onPlayerItemConsumed?.Invoke(healthAmount);
            Debug.Log($"{itemName} consumed!");
            RemoveFromInventory();
        }

        public override string GetItemStats()
        {
            return base.GetItemStats();
        }

        #endregion
    }
}