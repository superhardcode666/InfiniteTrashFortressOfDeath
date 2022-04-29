using System;
using System.Collections.Generic;
using FMODUnity;
using Hardcode.ITFOD.Audio;
using Hardcode.ITFOD.Combat;
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
namespace Hardcode.ITFOD.StatSystem
{
    public class ActorStats : MonoBehaviour
    {
        public virtual void TakeDamage(int damage)
        {
            if (currentHealth - damage > 0)
            {
                currentHealth -= damage;
                playAudio?.Invoke(npcHit);
                Debug.Log($"{transform.name} takes {damage} Damage!");
            }
            else currentHealth = 0;
        }

        public virtual void Die()
        {
            actorKilled?.Invoke();
            playAudio?.Invoke(npcDeath);

            Debug.Log($"{transform.name} was destroyed!");
            Destroy(gameObject);
        }

        public void SetFlatBuff(int factor)
        {
        }

        public void InitStats()
        {
            strength.baseValue = baseStats.strength.baseValue;
            armor.baseValue = baseStats.armor.baseValue;
            speed.baseValue = baseStats.speed.baseValue;
            luck.baseValue = baseStats.luck.baseValue;
            maxHealth = baseStats.maxHealth;

            currentHealth = maxHealth;
        }

        #region Field Declarations

        // Used to track the global kill counter in DataManager
        public static event Action actorKilled;

        [SerializeField] protected int maxHealth = 100;
        [SerializeField] protected int currentHealth;

        [SerializeField] protected Stat strength;
        [SerializeField] protected Stat armor;
        [SerializeField] protected Stat speed;
        [SerializeField] protected Stat luck;

        [SerializeField] protected StatData baseStats;

        public static event Action<EventReference> playAudio;

        [SerializeField] private EventReference npcHit;
        [SerializeField] private EventReference npcDeath;

        public int MaxHealth => maxHealth;
        public int CurrentHealth => currentHealth;
        public Stat Strength => strength;
        public Stat Armor => armor;
        public Stat Speed => speed;

        public Stat Luck => luck;

        public int Damage => Strength.GetValue();
        public int Defense => Armor.GetValue();

        [SerializeField] public int currentExp;
        [SerializeField] public int currentGold;

        [SerializeField] private List<CombatAction> actions;

        #endregion
    }
}