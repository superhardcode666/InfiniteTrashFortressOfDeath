using System;
using System.Collections.Generic;
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
namespace Hardcode.ITFOD.Items
{
    [CreateAssetMenu(fileName = "New Loot Table", menuName = "SUPERETERNALDEATH/Loottable")]
    public class LootTable : ScriptableObject
    {
        public Item GetDrop()
        {
            var roll = Random.Range(0, TotalWeight);

            foreach (var drop in table)
            {
                roll -= drop.weight;

                if (roll < 0) return drop.drop;
            }

            return table[0].drop;
        }

        #region Field Declarations

        public List<LootDrop> table;

        [NonSerialized] private float totalWeight = -1;

        public float TotalWeight
        {
            get
            {
                if (totalWeight == -1) CalculateTotalWeight();
                return totalWeight;
            }
        }

        private void CalculateTotalWeight()
        {
            totalWeight = 0;
            foreach (var item in table) totalWeight += item.weight;
        }

        #endregion
    }

    [Serializable]
    public class LootDrop
    {
        public Item drop;
        public float weight;
    }
}