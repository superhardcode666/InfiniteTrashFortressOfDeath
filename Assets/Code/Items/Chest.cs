using System;
using FMODUnity;
using Hardcode.ITFOD.Audio;
using Hardcode.ITFOD.Interactions;
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
    public class Chest : Interaction, IInteractable
    {
        public Chest(Item drop)
        {
            this.drop = drop;
        }

        private void Awake()
        {
            drop = lootTable.GetDrop();
        }

        public void Interact()
        {
            TryPickUp();
            if (wasPickedUp)
            {
                playAudio?.Invoke(itemGet);
                GameEvents.displayPlayerInfoMessage?.Invoke($"Picked up: <color=green>{drop.itemName}!</color>");
                //TODO: remove chest from activeChests list in gamemanager
                GameEvents.onClearSpawnPosition?.Invoke(transform.position);

                itemPickedUp?.Invoke();

                Destroy(gameObject);
            }
            else
            {
                playAudio?.Invoke(inventoryFull);
                GameEvents.displayPlayerInfoMessage?.Invoke("<color=red>Inventory full!</color>");
                Debug.Log("Your bag is full!");
            }
        }

        public void InFocus()
        {
            if (!interactionEmote.activeSelf) interactionEmote.SetActive(true);
        }

        public void OutOfFocus()
        {
            if (interactionEmote.activeSelf) interactionEmote.SetActive(false);
        }

        private void TryPickUp()
        {
            wasPickedUp = InventoryManager.instance.inventory.AddItem(drop);
        }

        public void SetChestContents(Item loot)
        {
            drop = loot;
        }

        #region Field Declarations

        [SerializeField] private Item drop;
        public Item Drop { get; private set; }
        private SpriteRenderer spriteRenderer;

        [SerializeField] private LootTable lootTable;

        private bool wasPickedUp;

        public static event Action itemPickedUp;
        public static event Action<EventReference> playAudio;

        [SerializeField] private EventReference itemGet;
        [SerializeField] private EventReference inventoryFull;

        #endregion
    }
}