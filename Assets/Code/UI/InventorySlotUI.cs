using System;
using Hardcode.ITFOD.Items;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
    public class InventorySlotUI : MonoBehaviour, ISelectHandler, IDeselectHandler
    {
        public void OnDeselect(BaseEventData eventData)
        {
            if (eventData.selectedObject == gameObject)
            {
                ClearTooltip();
                toolTipWasDisplayed = false;
            }
        }

        public void OnSelect(BaseEventData eventData)
        {
            if (item != null)
                if (eventData.selectedObject == gameObject)
                {
                    SetTooltip(item);
                    toolTipWasDisplayed = true;
                }
        }

        public void AddItem(Item newItem)
        {
            item = newItem;

            itemSprite.enabled = true;
            itemSprite.sprite = item.sprite;

            trashButton.interactable = true;

            SetTooltip(item);
            toolTipWasDisplayed = true;
        }

        public virtual void ClearSlot()
        {
            item = null;

            itemSprite.sprite = null;
            itemSprite.enabled = false;

            trashButton.interactable = false;

            ClearTooltip();
            toolTipWasDisplayed = false;
        }

        public void OnTrashButton()
        {
            InventoryManager.instance.inventory.RemoveItem(item);
            InventoryManager.instance.inventory.onItemDroppedCallback?.Invoke(item);
        }

        public void UseItem()
        {
            if (item != null)
            {
                InventoryManager.instance.inventory.onItemUsedCallback?.Invoke(item);
                item.Use();
            }
        }

        protected void ClearTooltip()
        {
            clearItemTooltip?.Invoke();
            toolTipWasDisplayed = false;
        }

        protected void SetTooltip(Item itemToDisplay)
        {
            activateItemTooltip?.Invoke(itemToDisplay);
            toolTipWasDisplayed = true;
        }

        #region Field Declarations

        protected Item item;

        public Item Item => item;

        public static event Action<Item> activateItemTooltip;
        public static event Action clearItemTooltip;

        [SerializeField] protected Button trashButton;
        [SerializeField] protected Image itemSprite;
        [SerializeField] protected TextMeshProUGUI itemAmount;

        [SerializeField] private bool toolTipWasDisplayed;

        #endregion
    }
}