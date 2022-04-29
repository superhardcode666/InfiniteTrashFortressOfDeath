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
    public class EquipmentSlotUI : InventorySlotUI
    {
        // EXACTLY the same code as base class
        public override void ClearSlot()
        {
            item = null;

            trashButton.interactable = false;

            itemSprite.sprite = defaultSprite;
            itemSprite.enabled = true;
        }

        public void OnUnequipButton()
        {
            EquipmentManager.instance.Unequip((int) slot);
        }

        #region Field Declarations

        [SerializeField] private EquipmentSlot slot;
        [SerializeField] private Sprite defaultSprite;

        #endregion
    }
}