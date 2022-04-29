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

using Hardcode.ITFOD.Combat;
using Hardcode.ITFOD.Game;
using Hardcode.ITFOD.Items;
using Hardcode.ITFOD.Player;
using Hardcode.ITFOD.StatSystem;
using TMPro;
using UnityEngine;

namespace Hardcode.ITFOD.SAVEDATA
{
    public class RunTracker : MonoBehaviour
    {
        [SerializeField] private RunData runData;

        [SerializeField] private TextMeshProUGUI timerTextUI;
        private DataManager dataManager;

        private bool runStarted;

        // true if OnCreated has finished, meaning all Events are hooked up
        public RunData RunData => runData;
        public DeathTimer Timer { get; private set; }

        public bool TrackerInitialized { get; private set; }

        // Just Update the Time displayed 
        private void Update()
        {
            Timer.UpdateRunTimer();
            runData.finalTime = Timer.GetFinalTime();
            timerTextUI.SetText($"{runData.finalTime}");
        }

        private void OnDestroy()
        {
            ActorStats.actorKilled -= UpdateKillCounter;
            PlayerStats.playerLevelIncreased -= UpdateLevelCounter;
            PlayerStats.playerDamageTaken -= UpdateDamageTakenCounter;
            CombatManager.playerDamageDealt -= UpdateDamageDealtCounter;
            CombatManager.playerEscaped -= UpdateEscapeCounter;
            CombatManager.playerSlainBy -= UpdateSlainBy;
            Chest.itemPickedUp -= UpdateItemCounter;
            Exit.triggerMapChange -= UpdateFloorCounter;
            PlayerController.increaseStepCounter -= UpdateStepCounter;

            GameManager.startTrackingThisRun -= StartTracking;
            GameManager.pauseTrackingThisRun -= PauseTracking;
            GameManager.endTrackingThisRun -= StopTracking;

            GameManager.saveCurrentRunData -= SaveCurrentRunData;

            Debug.Log("<color=green>DataManager:</color> Unsubbed from all Events, shutting down.");
        }

        public void OnCreated()
        {
            ActorStats.actorKilled += UpdateKillCounter;
            PlayerStats.playerLevelIncreased += UpdateLevelCounter;
            PlayerStats.playerDamageTaken += UpdateDamageTakenCounter;
            CombatManager.playerDamageDealt += UpdateDamageDealtCounter;
            CombatManager.playerEscaped += UpdateEscapeCounter;
            CombatManager.playerSlainBy += UpdateSlainBy;
            Chest.itemPickedUp += UpdateItemCounter;
            Exit.triggerMapChange += UpdateFloorCounter;
            PlayerController.increaseStepCounter += UpdateStepCounter;

            GameManager.startTrackingThisRun += StartTracking;
            GameManager.pauseTrackingThisRun += PauseTracking;
            GameManager.endTrackingThisRun += StopTracking;

            GameManager.saveCurrentRunData += SaveCurrentRunData;

            Debug.Log("<color=magenta>RunTracker:</color> Dependencies initialized.");
        }

        public void InitTracker()
        {
            if (TrackerInitialized) return;

            dataManager = GetComponent<DataManager>();
            dataManager.SetPath();

            dataManager.KillExistingSaveData();
            runData = new RunData();

            Timer = gameObject.AddComponent<DeathTimer>();

            TrackerInitialized = true;
        }

        // The Tracker is controlled via GameManager Actions
        private void StartTracking()
        {
            Timer.StartTimer();
        }

        private void PauseTracking()
        {
            Timer.PauseTimer();
        }

        private void StopTracking()
        {
            Timer.StopTimer();
        }

        private void SaveCurrentRunData()
        {
            dataManager.SaveData(runData);
        }

        #region Every Statistic being tracked each Run

        private void UpdateSlainBy(string actorName)
        {
            runData.slainBy = actorName;
        }

        private void UpdateLevelCounter()
        {
            runData.level += 1;
        }

        private void UpdateKillCounter()
        {
            runData.kills += 1;
        }

        private void UpdateEscapeCounter()
        {
            runData.escapes += 1;
        }

        private void UpdateItemCounter()
        {
            runData.items += 1;
        }

        private void UpdateFloorCounter()
        {
            runData.floor += 1;
        }

        private void UpdateStepCounter()
        {
            runData.steps += 1;
        }

        private void UpdateDamageTakenCounter(int damageTaken)
        {
            runData.damageTaken += damageTaken;
        }

        private void UpdateDamageDealtCounter(int damageDealt)
        {
            runData.damageDealt += damageDealt;
        }

        #endregion
    }
}