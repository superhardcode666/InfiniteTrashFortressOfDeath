using System;
using System.Collections;
using FMODUnity;
using Hardcode.ITFOD.Audio;
using Hardcode.ITFOD.Npc;
using Hardcode.ITFOD.StatSystem;
using Hardcode.ITFOD.UI;
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
namespace Hardcode.ITFOD.Combat
{
    public class CombatManager : MonoBehaviour
    {
        private bool hasEcapeFailed;

        private void OnDestroy()
        {
            GameEvents.onCombatTriggered -= InitCombat;
            CombatUI.combatScreenReady -= PlayFirstEnemyTaunt;
            PlayerStats.playerDeath -= OnCombatLost;

            Debug.Log("<color=red>CombatManager:</color>  Unsubbed from all Events, shutting down.");
        }

        public void OnCreated()
        {
            GameEvents.onCombatTriggered += InitCombat;
            CombatUI.combatScreenReady += PlayFirstEnemyTaunt;
            PlayerStats.playerDeath += OnCombatLost;

            Debug.Log("<color=red>CombatManager:</color> Set up Dependencies, booting up.");
        }

        private void PlayFirstEnemyTaunt()
        {
            if (enemyData != null)
            {
                var enemyIntroText = $"<color=red>{enemyData.ActorName}</color> {GetRandomIntro()}";
                UpdateCombatUIText(enemyIntroText);
            }
            else
            {
                Debug.Log("<color=red>CombatManager:</color> EnemyData not set...");
            }
        }

        public void InitCombat(Actor enemy)
        {
            //GameEvents.switchMusic?.Invoke(MusicModes.Combat);

            MusicManager.SwitchMusic(MusicModes.Combat);
            
            triggerCombatTransition?.Invoke(true);

            // 2 Second Initial Combat Delay, followed by Message about who gets the first attack
            
            var focus = focusHelper.HasFocus(attackButton);

            if (!focus) focusHelper.SetFocus(attackButton);

            enemyData = enemy;
            enemyStats = enemyData.stats;

            setupCombatScreenUi?.Invoke(enemyData.CombatSprite, enemyData.ActorName);

            changeGameState?.Invoke(GameState.Combat);

            UpdateCombatState(CombatState.Start);
        }

        public void OnPlayerAttackButton()
        {
            if (state == CombatState.PlayerTurn)
                if (!playerIsAttacking)
                {
                    if (enemyData == null)
                    {
                        UpdateCombatState(CombatState.Won);
                        return;
                    }

                    playerIsAttacking = true;
                    StartCoroutine(PlayerRegularAttack());
                }
        }

        public void OnPlayerFleeButton()
        {
            // replace with some percentage chance based on player luck
            var escapeRating = Random.value;

            if (escapeRating >= tempFlightChance)
            {
                hasEcapeFailed = false;
                OnCombatEscaped();
            }
            else
            {
                hasEcapeFailed = true;
                EndPlayerTurn();
            }
        }

        private bool CheckForHit()
        {
            var hit = Random.value;

            if (hit < 0.15)
            {
                playCombatAudio?.Invoke(attackMissed);
                return false;
            }
            
            playCombatAudio?.Invoke(meleeAttack);
            return true;
        }

        private bool CheckForCrit()
        {
            var hit = Random.value;

            if (hit >= 0.75)
            {
                playCombatAudio?.Invoke(criticalHit);
                return true;
            }

            return false;
        }

        private int CalculateBaseDamage(ActorStats attacker, ActorStats target, bool isCrit = false)
        {
            // DQ3 doubles basedamage, then halves it +- 10%
            var baseDamage = attacker.Damage * 2;
            var variance = Mathf.CeilToInt(baseDamage / 10);

            var damage = Random.Range(baseDamage - variance, baseDamage + variance);

            if (isCrit)
            {
                var critDamage = damage - target.Defense / 2 + damage;
                return (int) Mathf.Clamp(critDamage, 0, Mathf.Infinity);
            }

            var regularDamage = (damage - target.Defense / 2) / 2;
            return (int) Mathf.Clamp(regularDamage, 0, Mathf.Infinity);
        }

        private IEnumerator PlayerRegularAttack()
        {
            var damage = 0;

            var hit = CheckForHit();

            if (hit)
            {
                var crit = CheckForCrit();

                if (crit)
                {
                    // critical hit!

                    damage = CalculateBaseDamage(playerStats, enemyStats, true);

                    DamageEnemy(damage);

                    UpdateCombatUIText(
                        $"You just absolutely <size=150%>CRUSHED</size> <color=red>{enemyData.ActorName}</color> for <color=green>{damage} Damage</color> - <anim:shake>Ouch!</anim>");
                    displayDamageOnEnemy?.Invoke(damage, true);
                    displayScreenShake?.Invoke(0.25f, 5f);

                    EndPlayerTurn();
                }
                else
                {
                    // regular ass damage

                    damage = CalculateBaseDamage(playerStats, enemyStats);
                    DamageEnemy(damage);

                    displayDamageOnEnemy?.Invoke(damage, false);
                    displayScreenShake?.Invoke(0.2f, 1.8f);

                    UpdateCombatUIText(
                        $"You just hit <color=red>{enemyData.ActorName}</color> for <color=green>{damage} Damage</color>.");

                    EndPlayerTurn();
                }
            }
            else
            {
                UpdateCombatUIText("You <anim:shake><color=red>missed!</color></anim>");
                displayPlayerMiss?.Invoke();

                EndPlayerTurn();
            }

            yield break;
        }

        private IEnumerator PlayerTurn()
        {
            playerTurn?.Invoke(true);
            playerTurnCoroutineIsRunning = true;

            yield return null;
        }

        private void EndPlayerTurn()
        {
            playerTurnCoroutineIsRunning = false;
            playerIsAttacking = false;
            StopCoroutine(PlayerTurn());

            UpdateCombatState(enemyData.stats.CurrentHealth <= 0 ? CombatState.Won : CombatState.EnemyTurn);
        }

        private void DamageEnemy(int damage)
        {
            enemyStats.TakeDamage(damage);
            playerDamageDealt?.Invoke(damage);
        }

        private void DamagePlayer(int damage)
        {
            playerStats.TakeDamage(damage);
        }

        private IEnumerator EnemyTurn()
        {
            playerTurn?.Invoke(false);

            enemyTurnCoroutineIsRunning = true;

            if (hasEcapeFailed)
            {
                UpdateCombatUIText($"<color=red>{enemyData.ActorName}</color> won't let you escape!");
                hasEcapeFailed = false;

                yield return new WaitForSeconds(combatDelay);
            }

            yield return new WaitForSeconds(combatDelay);

            // do super evil stuff here
            // choose between available Combat Actions, 
            // with a small chance for the enemy to idle

            EnemyRegularAttack();

            yield return null;
        }

        private void EnemyRegularAttack()
        {
            var damage = 0;

            var hit = CheckForHit();

            if (hit)
            {
                var crit = CheckForCrit();

                if (crit)
                {
                    // critical hit!

                    damage = CalculateBaseDamage(enemyStats, playerStats, true);

                    DamagePlayer(damage);

                    UpdateCombatUIText(
                        $"<color=red>{enemyData.ActorName}</color> just <size=150%>obliterated</size> you with <color=green>{damage} Damage!</color>");
                    displayDamageOnPlayer?.Invoke(damage);
                    displayScreenShake?.Invoke(0.25f, 5f);

                    EndEnemyTurn();
                }
                else
                {
                    // regular ass damage

                    damage = CalculateBaseDamage(enemyStats, playerStats);
                    DamagePlayer(damage);

                    displayDamageOnPlayer?.Invoke(damage);
                    displayScreenShake?.Invoke(0.2f, 1.8f);

                    UpdateCombatUIText(
                        $"<color=red>{enemyData.ActorName}</color> hit you for <color=green>{damage} Damage.</color>");

                    EndEnemyTurn();
                }
            }
            else
            {
                UpdateCombatUIText(
                    $"<color=red>{enemyData.ActorName}</color> <anim:shake><color=red>missed!</color></anim>");
                displayEnemyMiss?.Invoke();

                EndEnemyTurn();
            }
        }

        private void EndEnemyTurn()
        {
            enemyTurnCoroutineIsRunning = false;
            StopCoroutine(EnemyTurn());

            UpdateCombatState(playerStats.CurrentHealth <= 0 ? CombatState.Lost : CombatState.PlayerTurn);
        }

        private void UpdateCombatState(CombatState newState)
        {
            state = newState;
        }

        private void UpdateCombatUIText(string combatText)
        {
            animateCombatDialog?.Invoke(combatText);
        }

        private void RollInititiative()
        {
            var playerSpeed = playerStats.Speed.GetValue();
            var enemySpeed = enemyStats.Speed.GetValue();

            UpdateCombatState(playerSpeed >= enemySpeed ? CombatState.PlayerTurn : CombatState.EnemyTurn);
        }

        private string GetRandomIntro()
        {
            return combatIntros[Random.Range(0, combatIntros.Length)];
        }

        public void OnUpdate()
        {
            switch (state)
            {
                case CombatState.Start:
                    RollInititiative();
                    break;
                case CombatState.PlayerTurn:
                    if (!playerTurnCoroutineIsRunning) StartCoroutine(PlayerTurn());
                    break;
                case CombatState.EnemyTurn:
                    if (!enemyTurnCoroutineIsRunning) StartCoroutine(EnemyTurn());
                    break;
                case CombatState.Won:
                    OnCombatWon();
                    break;
                case CombatState.Lost:
                    OnCombatLost();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnCombatWon()
        {
            // Hand out EXP and Coins, clear enemySlot, then switch States
            ProcessRewards();

            displayScreenShake?.Invoke(0.5f, 10f);
            combatUIKillDummy?.Invoke();

            var npcPosition = enemyData.transform.position;
            GameEvents.onClearSpawnPosition?.Invoke(npcPosition);

            var npcLoot = enemyData.DropLoot();

            enemyData.stats.Die();
            enemyData = null;

            changeGameState?.Invoke(GameState.Explore);

            //It should be determined whether or not Loot is dropped on NPC death first
            if (ShouldDropLoot()) GameEvents.onLootDropped(npcPosition, npcLoot);

            EndCombat();
        }

        private bool ShouldDropLoot()
        {
            var rand = new System.Random();
            return rand.Next(0, 2) == 0;
        }

        private void ProcessRewards()
        {
            UpdateCombatUIText(
                $"<anim:shake>YOU absolutely <color=red>MURDERED</color> {enemyData.ActorName}!</anim>.");

            playerStats.AddExp(enemyStats.currentExp);
            playerStats.AddCoins(enemyStats.currentGold);

            animateCombatResults?.Invoke(
                $"<sp:25><anim:wave>You've gained <color=green>{enemyStats.currentExp}</color> Experience Points!</anim>");
        }

        private void OnCombatEscaped()
        {
            UpdateCombatUIText("<color=green>You escaped</color> Death this time...");

            playerEscaped?.Invoke();

            StopCoroutine(PlayerTurn());

            enemyData = null;

            changeGameState?.Invoke(GameState.Explore);

            EndCombat();
        }

        private void OnCombatLost()
        {
            // Kill Player, clear enemy Data switch Game States
            UpdateCombatUIText("<anim:shake><color=red>YOU DIED</color></anim>");
            playerSlainBy?.Invoke(enemyData.ActorName);

            displayScreenShake?.Invoke(0.5f, 10f);

            // Set GameState to GameOver triggering Scene Change
            changeGameState?.Invoke(GameState.GameOver);

            EndCombat();
        }

        private void EndCombat()
        {
            playerTurnCoroutineIsRunning = false;
            playerIsAttacking = false;

            focusHelper.DropFocus();
            combatUICleanUp?.Invoke();

            StartCoroutine(PostCombatDelay(combatDelay));
        }

        private IEnumerator PostCombatDelay(float delay)
        {
            yield return new WaitForSeconds(delay * 2);
            GameEvents.switchMusic?.Invoke(MusicModes.Dungeon);
            triggerCombatTransition?.Invoke(false);
        }

        #region Field Declarations

        #region static actions

        public static event Action<bool> playerTurn;
        public static event Action<GameState> changeGameState;
        public static event Action<Sprite, string> setupCombatScreenUi;
        public static event Action<int, bool> displayDamageOnEnemy;
        public static event Action<int> displayDamageOnPlayer;
        public static event Action<float, float> displayScreenShake;
        public static event Action displayPlayerMiss;
        public static event Action displayEnemyMiss;
        public static event Action combatUIKillDummy;
        public static event Action<string> animateCombatDialog;
        public static event Action<string> animateCombatResults;
        public static event Action combatUICleanUp;
        public static event Action<bool> triggerCombatTransition;
        public static event Action<int> playerDamageDealt;
        public static event Action<string> playerSlainBy;
        public static event Action playerEscaped;

        public static event Action combatEnded;

        public static event Action<EventReference> playCombatAudio;

        #endregion

        [SerializeField] private EventReference meleeAttack;
        [SerializeField] private EventReference attackMissed;
        [SerializeField] private EventReference criticalHit;
        

        [SerializeField] private int combatDelay = 2;
        [SerializeField] private PlayerStats playerStats;
        [SerializeField] private Actor enemyData;

        private GameObject currentEnemy;
        private ActorStats enemyStats;

        [SerializeField] private CombatState state;
        private readonly DiceRoller diceRoller = new();

        private readonly FocusHelper focusHelper = new();

        [SerializeField] private bool playerTurnCoroutineIsRunning;
        [SerializeField] private bool enemyTurnCoroutineIsRunning;

        [SerializeField] private bool playerIsAttacking;

        [SerializeField] private GameObject attackButton;

        private readonly string[] combatIntros =
        {
            "is BIG MAD!.",
            "has had about e-n-o-u-g-h of your face.",
            "is jumping right at you!",
            "never learned how to love...",
            "is leaning in for a REALLY aggressive hug!"
        };

        [SerializeField] private double tempFlightChance;

        #endregion
    }
}

public enum CombatState
{
    Start,
    PlayerTurn,
    EnemyTurn,
    Won,
    Lost
}