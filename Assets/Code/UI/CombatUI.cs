using System;
using Hardcode.ITFOD.Audio;
using Hardcode.ITFOD.Combat;
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
namespace Hardcode.ITFOD.UI
{
    public class CombatUI : MonoBehaviour
    {
        [SerializeField] private CombatBackground combatBG;

        private void OnDestroy()
        {
            GameEvents.toggleCombatScreenUI -= ToggleCombatScreen;

            CombatManager.playerTurn -= ShowPlayerIcon;
            CombatManager.setupCombatScreenUi -= SetUpCombatScreenUI;
            CombatManager.displayDamageOnEnemy -= DisplayDamageOnEnemy;
            CombatManager.displayDamageOnPlayer -= DisplayDamageOnPlayer;
            CombatManager.displayPlayerMiss -= DisplayMiss;
            CombatManager.displayEnemyMiss -= DisplayEnemyMiss;
            CombatManager.combatUIKillDummy -= KillDummy;
            CombatManager.combatUICleanUp -= CleanUpAfterBattle;
            CombatManager.animateCombatResults -= DisplayCombatResults;

            CombatManager.animateCombatDialog -= DisplayCombatDialog;
            CombatManager.displayScreenShake -= PlayShakeScreen;

            Debug.Log("<color=red>CombatUI:</color> Unsubbed from all Events, shutting down.");
        }

        public void OnCreated()
        {
            GameEvents.toggleCombatScreenUI += ToggleCombatScreen;

            CombatManager.playerTurn += ShowPlayerIcon;
            CombatManager.setupCombatScreenUi += SetUpCombatScreenUI;
            CombatManager.displayDamageOnEnemy += DisplayDamageOnEnemy;
            CombatManager.displayDamageOnPlayer += DisplayDamageOnPlayer;
            CombatManager.displayPlayerMiss += DisplayMiss;
            CombatManager.displayEnemyMiss += DisplayEnemyMiss;
            CombatManager.combatUIKillDummy += KillDummy;
            CombatManager.combatUICleanUp += CleanUpAfterBattle;
            CombatManager.animateCombatResults += DisplayCombatResults;

            CombatManager.animateCombatDialog += DisplayCombatDialog;
            CombatManager.displayScreenShake += PlayShakeScreen;

            combatDummy.Init();
            combatDummy.DisableDummy();

            Debug.Log("<color=red>CombatUI:</color> Set up Dependencies, booting up.");
        }

        private void PlayShakeScreen(float time, float power)
        {
            combatBackground.ShakeScreen(time, power);
        }

        private void DisplayCombatDialog(string combatText)
        {
            updateCombatDialog?.Invoke(combatText);
        }

        private void DisplayCombatResults(string combatText)
        {
            combatResults.gameObject.SetActive(true);
            showCombatResults?.Invoke(combatText);
        }

        private void ToggleCombatScreen(bool activateCombat)
        {
            combatScreen.SetActive(activateCombat);

            if (activateCombat) combatScreenReady?.Invoke();
        }

        private void ShowPlayerIcon(bool isplayerturn)
        {
            if (isplayerturn)
                playerTurnIcon.LeanMoveY(130, 0.25f).setEaseOutBounce().delay = 0.1f;
            else
                playerTurnIcon.LeanMoveY(70, 0.25f).setEaseOutBounce().delay = 0.1f;
        }

        private void SetUpCombatScreenUI(Sprite enemyCombatSprite, string enemyName)
        {
            combatDummy.SetUpDummy(enemyCombatSprite);
            enemyNameText.SetText(enemyName);

            combatResults.SetText("");
            combatResults.gameObject.SetActive(false);
        }

        private void DisplayDamageOnEnemy(int damage, bool isCrit)
        {
            if (isCrit) combatBG.PlayHitFlash();

            CreateHitVisuals();
            combatDummy.PlayHitFlash();
            PlayDummyHitVfx();

            var dmg = Instantiate(damageText, combatDummy.transform.position, Quaternion.identity,
                combatDummy.transform);
            var dmgTxt = dmg.GetComponent<TextMeshProUGUI>();

            dmgTxt.text = isCrit ? $"<color=yellow>{damage}</color>" : $"<color=white>{damage}</color>";
        }

        private void PlayDummyHitVfx()
        {
        }

        private void CreateHitVisuals()
        {
            var slash = Instantiate(slashFx, combatDummy.transform.position, Quaternion.identity,
                combatDummy.transform);
            var blood = Instantiate(bloodFx, combatDummy.transform.position + bloodOffset, Quaternion.identity,
                combatDummy.transform);

            Destroy(slash, 1.25f);
            Destroy(blood, 1.25f);
        }

        private void KillDummy()
        {
            PlayDummyHitVfx();
            combatDummy.PlayDeathAnimation();
        }

        public void StopDummyHover()
        {
            combatDummy.StopIdleRoutine();
            //combatDummy.ClearDummy();
        }

        private void DisplayDamageOnPlayer(int damage)
        {
            playerDummy.PlayHitFlash();

            var dmg = Instantiate(damageText, playerDummy.transform.position, Quaternion.identity,
                playerDummy.transform);
            var dmgTxt = dmg.GetComponent<TextMeshProUGUI>();

            dmgTxt.text = $"<color=red>{damage}</color>";
        }

        private void DisplayMiss()
        {
            var miss = Instantiate(damageText, combatDummy.transform.position, Quaternion.identity,
                combatDummy.transform);
            var dmgTxt = miss.GetComponent<TextMeshProUGUI>();

            dmgTxt.text = "<color=red>MISS!</color>";
        }

        private void DisplayEnemyMiss()
        {
            var miss = Instantiate(damageText, playerDummy.transform.position, Quaternion.identity,
                playerDummy.transform);
            var misstxt = miss.GetComponent<TextMeshProUGUI>();

            misstxt.text = "<color=red>MISS!</color>";
        }

        private void CleanUpAfterBattle()
        {
            StopDummyHover();
            enemyNameText.SetText("");
        }

        #region Field Declarations

        [Header("Visuals")] [SerializeField] private CombatDummy combatDummy;

        [SerializeField] private PlayerDummy playerDummy;
        [SerializeField] private RectTransform playerTurnIcon;
        [SerializeField] private GameObject damageText;
        [SerializeField] private TextMeshProUGUI enemyNameText;
        [SerializeField] private GameObject combatScreen;
        [SerializeField] private CameraShaker combatBackground;

        [SerializeField] private TextMeshProUGUI combatResults;

        [Header("Effects")] [SerializeField] private GameObject slashFx;

        [SerializeField] private GameObject bloodFx;
        private readonly Vector3 bloodOffset = new(0.5f, 0.5f, 0);

        public static event Action<string> updateCombatDialog;
        public static event Action<string> showCombatResults;
        public static event Action combatScreenReady;

        #endregion
    }
}