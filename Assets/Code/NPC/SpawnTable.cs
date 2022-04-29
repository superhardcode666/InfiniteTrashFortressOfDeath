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
[CreateAssetMenu(fileName = "New Spawn Table", menuName = "SUPERETERNALDEATH/Spawntable")]
public class SpawnTable : ScriptableObject
{
    public GameObject GetSpawn()
    {
        var roll = Random.Range(0, TotalWeight);

        foreach (var spawn in table)
        {
            roll -= spawn.weight;

            if (roll < 0) return spawn.spawn;
        }

        return table[0].spawn;
    }

    #region Field Declarations

    public List<Spawn> table;

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
        foreach (var spawn in table) totalWeight += spawn.weight;
    }

    #endregion
}


[Serializable]
public class Spawn
{
    public GameObject spawn;
    public float weight;
}