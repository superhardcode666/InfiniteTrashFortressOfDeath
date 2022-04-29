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

using System;
using UnityEngine;

namespace Hardcode.ITFOD.SAVEDATA
{
    [Serializable]
    public class RunData
    {
        #region Super Basic Run Statistics

        public int steps,
            floor,
            level,
            damageTaken,
            damageDealt,
            kills,
            escapes,
            items;

        public string slainBy, finalTime;

        public RunData()
        {
            floor = 1;
            level = 1;
            Debug.Log("New RunData created!");
        }

        public string GetRunSummary()
        {
            return
                $"Steps taken: {steps}, Dungeon Floor: {floor}, Character Level: {level}, " +
                $"Damage Taken: {damageTaken}, Damage Dealt: {damageDealt}, Kills: {kills}, " +
                $"Successful Escapes {escapes}, Items found: {items}. Time until Death: {finalTime}.";
        }

        public void SetFinalRunTime(string timeAsString)
        {
            finalTime = timeAsString;
            Debug.Log($"Final Time {finalTime} saved.");
        }

        #endregion
    }
}