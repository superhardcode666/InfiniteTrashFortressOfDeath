using System;
using FMODUnity;
using Hardcode.ITFOD.Audio;
using Hardcode.ITFOD.Game;
using Hardcode.ITFOD.Items;
using ScottSteffes.AnimatedText;
using UnityEngine;
using UnityEngine.InputSystem;

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
    public class UIManager : MonoBehaviour
    {
        public void EnableUIControls()
        {
            menuToggle.Enable();
        }

        public void DisableUIControls()
        {
            menuToggle.Disable();
        }

        #region Field Declarations

        [Header("Game Manager")] [SerializeField]
        private GameManager gameManager;

        [Header("Dialog")] [SerializeField] private DialogManager dialogManager;

        [Header("Mini Map UI")] [SerializeField]
        private GameObject miniMapUi;

        [Header("Inventory, Equipment & Stats UI")] [SerializeField]
        private InventoryUI inventoryUi;

        [SerializeField] private EquipmentUI equipmentUi;
        [SerializeField] private StatUI statsUi;

        [SerializeField] private GameObject inventoryWindows;

        [Header("Combat UI")] [SerializeField] private CombatUI combatUi;

        [SerializeField] private ScreenTransition screenTransition;

        [Header("UI Input Actions")] [SerializeField]
        private InputAction menuToggle;

        private readonly FocusHelper focusHelper = new();

        public static event Action<GameState> inventoryToggle;

        [SerializeField] private GameObject firstInventorySlot;

        public static event Action<Item> focusToolTip;
        public static event Action<EventReference> playUIAudio;
        
        [SerializeField] private EventReference toggleMenuSfx;

        #endregion

        #region Start Up

        public void OnCreated()
        {
            menuToggle.performed += ToggleMenus;

            Debug.Log("<color=magenta>UIManager:</color> Set up Dependencies, booting up.");
        }

        private void OnDestroy()
        {
            Debug.Log("<color=magenta>UIManager:</color> Unsubbed from all Events, shutting down.");
        }

        public void InitInventoryUi()
        {
            inventoryUi.OnCreated();
        }

        public void InitEquipmentUi()
        {
            equipmentUi.OnCreated();
        }

        public void InitStatsUi()
        {
            statsUi.OnCreated();
        }

        public void InitCombatUi()
        {
            combatUi.OnCreated();
        }

        public void InitScreenTransition()
        {
            screenTransition.OnCreated();
        }

        private void ToggleMenus(InputAction.CallbackContext ctx)
        {
            // if (gameManager.state == GameState.Explore || gameManager.state == GameState.Menu)
            // {
            //     inventoryWindows.SetActive(!inventoryWindows.activeSelf);
            //     inventoryToggle?.Invoke(inventoryWindows.activeSelf ? GameState.Menu : GameState.Explore);
            //     playAudioClip?.Invoke(ClipID.ToggleMenu);
            // }

            if (gameManager.state == GameState.Explore)
            {
                inventoryWindows.SetActive(true);
                playUIAudio?.Invoke(toggleMenuSfx);

                inventoryToggle?.Invoke(GameState.Menu);
            }
            else if (gameManager.state == GameState.Menu)
            {
                inventoryWindows.SetActive(false);
                playUIAudio?.Invoke(toggleMenuSfx);
                inventoryToggle?.Invoke(GameState.Explore);
            }

            if (inventoryWindows.activeSelf)
            {
                // Set Focus on first Inventory Slot each time Inventory is set active
                var focus = focusHelper.HasFocus(firstInventorySlot);

                if (!focus) focusHelper.SetFocus(firstInventorySlot);

                // display item tooltip here
                var slotContents = firstInventorySlot.GetComponent<InventorySlotUI>();
                if (slotContents.Item != null) focusToolTip?.Invoke(slotContents.Item);
            }
        }



        #endregion
    }
}