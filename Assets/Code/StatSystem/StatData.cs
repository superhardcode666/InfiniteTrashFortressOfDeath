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
    [CreateAssetMenu(fileName = "New Stat Data", menuName = "SUPERETERNALDEATH/StatData")]
    public class StatData : ScriptableObject
    {
        #region Field Declarations

        public int maxHealth;
        public Stat strength;
        public Stat armor;
        public Stat luck;
        public Stat speed;

        #endregion
    }
}