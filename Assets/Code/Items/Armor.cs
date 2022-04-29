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
    ///     Scriptable Object for all Weapons
    /// </summary>
    /// <remarks>
    ///     Contains Weapon Type and the Damage Bonus, Damage Type, etc.
    /// </remarks>
    [CreateAssetMenu(fileName = "New Armor Item", menuName = "SUPERETERNALDEATH/Armor")]
    public class Armor : Equipment
    {
        #region Field Declarations

        public int baseDefense;

        public Armor()
        {
            itemName = "New Armor Item";
            itemType = ItemType.Armor;
            isStackable = false;
        }

        #endregion
    }
}