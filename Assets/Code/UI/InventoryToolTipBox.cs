using Hardcode.ITFOD.Items;
using Hardcode.ITFOD.UI;
using TMPro;
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
public class InventoryToolTipBox : MonoBehaviour
{
    private void Awake()
    {
        //InventoryManager.instance.inventory.onItemPickedUpCallback += SetItemTooltip;
        UIManager.focusToolTip += SetItemTooltip;
        InventorySlotUI.activateItemTooltip += SetItemTooltip;
        InventorySlotUI.clearItemTooltip += ClearItemTooltip;

        ClearItemTooltip();
    }

    private void OnDestroy()
    {
        //InventoryManager.instance.inventory.onItemPickedUpCallback -= SetItemTooltip; 
        UIManager.focusToolTip -= SetItemTooltip;
        InventorySlotUI.activateItemTooltip -= SetItemTooltip;
        InventorySlotUI.clearItemTooltip -= ClearItemTooltip;
    }

    public void SetItemTooltip(Item item)
    {
        ItemTooltipName.SetText(item.itemName);
        ItemTooltipStats.SetText(item.GetItemStats());
        ItemTooltipDescription.SetText(item.flavorText);
    }

    public void ClearItemTooltip()
    {
        ItemTooltipName.SetText("");
        ItemTooltipStats.SetText("");
        ItemTooltipDescription.SetText("");
    }

    #region Field Declarations

    [SerializeField] private TextMeshProUGUI ItemTooltipName;
    [SerializeField] private TextMeshProUGUI ItemTooltipStats;
    [SerializeField] private TextMeshProUGUI ItemTooltipDescription;

    #endregion
}