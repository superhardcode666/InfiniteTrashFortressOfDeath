using System.Collections.Generic;
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
    [CreateAssetMenu(fileName = "New Weapon Item", menuName = "SUPERETERNALDEATH/Weapon")]
    public class Weapon : Equipment
    {
        #region Field Declarations

        public int baseDamage;
        public bool ignoreArmor;

        public WeaponType weaponType;

        public enum WeaponType
        {
            Sword,
            Lance,
            Axe
        }

        public enum DamageType
        {
            Poison,
            Curse,
            Fire,
            Ice,
            Acid
        }

        public List<DamageType> damageType;

        public Weapon()
        {
            itemName = "New Weapon";
            itemType = ItemType.Weapon;
            damageType = new List<DamageType>();
            isStackable = false;
        }

        public override string GetItemStats()
        {
            return $"ATK: <color=green>+{damageModifier}</color> DEF: <color=green>+{armorModifier}</color>";
        }

        #endregion
    }
}