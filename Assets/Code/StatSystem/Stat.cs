using System;
using System.Collections.Generic;
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
    /// <summary>
    ///     Base Class for all Player & Monster Stats
    /// </summary>
    /// <remarks>
    /// </remarks>
    [Serializable]
    public class Stat
    {
        public int GetValue()
        {
            var finalValue = baseValue;
            modifiers.ForEach(x => finalValue += x);
            return finalValue;
        }

        public void AddModifier(int modifier)
        {
            if (modifier != 0) modifiers.Add(modifier);
        }

        public void RemoveModifier(int modifier)
        {
            if (modifier != 0) modifiers.Remove(modifier);
        }

        #region Field Declarations

        public int baseValue = 1;
        [SerializeField] private List<int> modifiers = new();

        #endregion
    }
}