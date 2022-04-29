using Hardcode.ITFOD.Items;
using Hardcode.ITFOD.StatSystem;
using TMPro;
using UnityEngine;
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
    public class StatUI : MonoBehaviour
    {
        private void OnDestroy()
        {
            PlayerStats.playerGeneralStatsUpdated -= UpdateGeneralValues;
            PlayerStats.playerPowerStatsUpdated -= UpdateStatsUI;
            PlayerStats.playerHealthChanged -= UpdateAllHealthDisplays;

            Debug.Log("<color=white>StatUI:</color> Unsubbed from all Events, shutting down.");
        }

        public void OnCreated()
        {
            PlayerStats.playerGeneralStatsUpdated += UpdateGeneralValues;
            PlayerStats.playerPowerStatsUpdated += UpdateStatsUI;
            PlayerStats.playerHealthChanged += UpdateAllHealthDisplays;

            Debug.Log("<color=white>StatUI:</color> Set up Dependencies, booting up.");
        }

        private void UpdateStatsUI(int totalDamage, int totalDefense, int speed, int luck)
        {
            // Damage should probably displayed as Range between min and max
            statValuesText.SetText($"{totalDamage}{lineBreak}{totalDefense}{lineBreak}{speed}{lineBreak}{luck}");
        }

        private void UpdateHealthUI(int currentHealth, int maxHealth)
        {
            UpdateAllHealthDisplays(currentHealth, maxHealth);
        }

        private void UpdateGeneralValues(int currentHp, int maxHp, int currentLvl, int currentXp, int xpToNextLevel)
        {
            UpdateAllHealthDisplays(currentHp, maxHp);
            levelText.SetText($"{currentLvl}");
            expText.SetText($"{currentXp}/{xpToNextLevel}");
        }

        private void UpdateAllHealthDisplays(int currentHealth, int maxHealth)
        {
            hpText.SetText($"{currentHealth}/{maxHealth}");
            hpSlider.value = currentHealth;
            hpSlider.maxValue = maxHealth;
            hpSliderText.text = $"{currentHealth}/{maxHealth}";

            hudHpSlider.value = currentHealth;
            hudHpSlider.maxValue = maxHealth;
        }

        #region Field Declarations

        private Inventory inventory;

        [SerializeField] private TextMeshProUGUI expText;
        [SerializeField] private TextMeshProUGUI statValuesText;
        [SerializeField] private TextMeshProUGUI hpText;
        [SerializeField] private TextMeshProUGUI levelText;
        [SerializeField] private TextMeshProUGUI moneyValueText;
        [SerializeField] private TextMeshProUGUI keyValueText;

        [Header("Player HP & XP UI")] [SerializeField]
        private TextMeshProUGUI hpSliderText;

        [SerializeField] private Slider hpSlider;
        [SerializeField] private Slider hudHpSlider;

        private readonly string lineBreak = "\n";

        #endregion
    }
}