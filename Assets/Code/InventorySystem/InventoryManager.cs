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
namespace Hardcode.ITFOD.Items
{
    public class InventoryManager : MonoBehaviour
    {
        #region Start Up

        public void OnCreated()
        {
            #region singleton

            if (instance != null)
            {
                Debug.Log("Dude, why are there multiple InventoryController Instances?");
                return;
            }

            instance = this;

            #endregion
        }

        #endregion

        #region Game Loop

        public void OnUpdate()
        {
        }

        #endregion

        #region Field Declarations

        public static InventoryManager instance;
        public Inventory inventory;

        public PlayerStats playerStats;

        #endregion
    }
}