using System;
using FMODUnity;
using Hardcode.ITFOD.Audio;
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
namespace Hardcode.ITFOD.StatSystem
{
    public class PlayerStats : ActorStats
    {
        private void OnDestroy()
        {
            EquipmentManager.instance.onEquipmentChanged -= OnEquipmentChanged;
            GameEvents.onPlayerItemConsumed -= ModifyHp;
        }

        public void OnCreated()
        {
            EquipmentManager.instance.onEquipmentChanged += OnEquipmentChanged;
            GameEvents.onPlayerItemConsumed += ModifyHp;

            nextExp = CalculateExpToNextLevel(currentLvl);
        }

        private void OnEquipmentChanged(Equipment newItem, Equipment oldItem)
        {
            if (newItem != null)
            {
                armor.AddModifier(newItem.armorModifier);
                strength.AddModifier(newItem.damageModifier);
            }

            if (oldItem != null)
            {
                armor.RemoveModifier(oldItem.armorModifier);
                strength.RemoveModifier(oldItem.damageModifier);
            }

            UpdateStatUI();
        }

        public override void TakeDamage(int damage)
        {
            base.TakeDamage(damage);
            playerDamageTaken?.Invoke(damage);
            playerHealthChanged?.Invoke(currentHealth, maxHealth);
        }

        private void ModifyHp(int amount)
        {
            if (amount < 0)
            {
                var damage = Math.Abs(amount);
                TakeDamage(damage);
            }
            else if (amount >= 0)
            {
                playAudio?.Invoke(playerHeal);
                currentHealth += amount;
                playerHealthChanged?.Invoke(currentHealth, maxHealth);
            }
        }

        private void LevelUp()
        {
            currentLvl++;
            nextExp += CalculateExpToNextLevel(currentLvl);

            // stat growth locked to +1d6/level
            strength.baseValue += diceRoller.Roll(1, 6);
            armor.baseValue += diceRoller.Roll(1, 6);
            speed.baseValue += diceRoller.Roll(1, 6);
            luck.baseValue += diceRoller.Roll(1, 6);

            // hp growth locked to +2d10/level
            maxHealth += diceRoller.Roll(2, 10);
            // free heals
            currentHealth = maxHealth;

            // Update Run Tracker
            playerLevelIncreased?.Invoke();
            playAudio?.Invoke(playerLevelUp);
            UpdateStatUI();
        }

        public void UpdateStatUI()
        {
            playerGeneralStatsUpdated?.Invoke(currentHealth, maxHealth, currentLvl, currentExp, nextExp);
            playerPowerStatsUpdated?.Invoke(strength.GetValue(), armor.GetValue(), speed.GetValue(), luck.GetValue());
        }

        public override void Die()
        {
            base.Die();
            playerDeath?.Invoke();
        }

        public void AddExp(int expGained)
        {
            currentExp += expGained;
            UpdateStatUI();
            while (currentExp >= nextExp) LevelUp();
        }

        public void AddCoins(int coinsGained)
        {
            currentGold += coinsGained;
        }

        private int CalculateExpToNextLevel(int lvl)
        {
            switch (expFormula)
            {
                case ProgressionMode.Runescape:
                    var div = currentLvl / runescapeExpDivisor;
                    return (int) (runescapeBaseExp + (Mathf.Pow(2, div) - 1));
                case ProgressionMode.Disgaea:
                    return Mathf.CeilToInt(0.5f * (Mathf.Pow(lvl, 3) + 0.8f * (Mathf.Pow(lvl, 2) + 2 * lvl)));
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #region Field Declarations

        [SerializeField] private int runescapeBaseExp = 300;
        [SerializeField] private float runescapeExpDivisor = 7f;

        [SerializeField] private int nextExp;
        [SerializeField] private int currentLvl = 1;

        [SerializeField] private ProgressionMode expFormula;
        private readonly DiceRoller diceRoller = new();

        public static event Action<EventReference> playAudio;

        [SerializeField] private EventReference playerHeal;
        [SerializeField] private EventReference playerLevelUp;
        
        
        #endregion

        #region static actions

        public static event Action<int, int, int, int, int> playerGeneralStatsUpdated;
        public static event Action<int, int, int, int> playerPowerStatsUpdated;
        public static event Action<int, int> playerHealthChanged;
        public static event Action<string> displayLevelUpMessage;
        public static event Action playerLevelIncreased;
        public static event Action playerDeath;

        public static event Action<int> playerDamageTaken;

        #endregion
    }
}

public enum ProgressionMode
{
    Runescape,
    Disgaea
}