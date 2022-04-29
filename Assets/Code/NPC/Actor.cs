using System;
using FMODUnity;
using Hardcode.ITFOD.Audio;
using Hardcode.ITFOD.Interactions;
using Hardcode.ITFOD.Items;
using Hardcode.ITFOD.StatSystem;
using ScottSteffes.AnimatedText;
using UnityEngine;
using Random = UnityEngine.Random;

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
namespace Hardcode.ITFOD.Npc
{
    public class Actor : Interaction, IInteractable
    {
        private void Awake()
        {
            RandomizeMood();

            stats = GetComponent<ActorStats>();
            stats.InitStats();
        }

        public void Interact()
        {
            if (isHostile)
            {
                playAudio?.Invoke(combatStartSfx);
                GameEvents.onCombatTriggered?.Invoke(this);
                Debug.Log($"{this} has entered Combat!");
            }
            else
            {
                playActorDialog?.Invoke(actorDialog);
                Debug.Log($"{this} should start talking!");
                GameEvents.onClearSpawnPosition?.Invoke(transform.position);
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

        public Item DropLoot()
        {
            return lootTable.GetDrop();
        }

        private void RandomizeMood()
        {
            var randomMood = Random.value;
            if (randomMood >= 0.4)
            {
                isHostile = true;
                interactionEmote.GetComponent<SpriteRenderer>().sprite = hostileEmote;
                miniMapIcon.GetComponent<SpriteRenderer>().sprite = hostileMiniMapIcon;
            }
            else
            {
                isHostile = false;
                interactionEmote.GetComponent<SpriteRenderer>().sprite = friendlyEmote;
                miniMapIcon.GetComponent<SpriteRenderer>().sprite = friendlyMiniMapIcon;
            }
        }

        #region Field Declarations

        public static event Action<DialogContainer> playActorDialog;

        [SerializeField] private string actorName;
        [SerializeField] private DialogContainer actorDialog;

        public ActorStats stats;

        [SerializeField] private Sprite combatSprite;
        [SerializeField] private LootTable lootTable;

        public bool isHostile;
        [SerializeField] private Sprite hostileEmote;
        [SerializeField] private Sprite friendlyEmote;

        [SerializeField] private GameObject miniMapIcon;
        [SerializeField] private Sprite friendlyMiniMapIcon;
        [SerializeField] private Sprite hostileMiniMapIcon;
        public string ActorName => actorName;
        public Sprite CombatSprite => combatSprite;

        public static event Action<EventReference> playAudio;
        [SerializeField] private EventReference combatStartSfx;
        
        #endregion
    }
}