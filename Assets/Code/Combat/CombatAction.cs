using Hardcode.ITFOD.StatSystem;
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

namespace Hardcode.ITFOD.Combat
{
    [CreateAssetMenu(fileName = "New Combat Action", menuName = "SUPERETERNALDEATH/Combat Action")]
    public class CombatAction : ScriptableObject
    {
        public ActionType actionType;
        public int cost;
        public ActorStats target;
        public int value;
        public GameObject visualFX;

        public void GetTarget()
        {
        }

        public void UseCombatAction()
        {
        }
    }

    public enum ActionType
    {
        Attack,
        Heal
    }
}