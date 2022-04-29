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
namespace Hardcode.ITFOD.Procedural
{
    public abstract class AbstractDungeonGenerator : MonoBehaviour
    {
        public void GenerateDungeon()
        {
            tilemapGenerator.Clear();
            RunProceduralGeneration();
        }

        protected abstract void RunProceduralGeneration();

        #region Field Declarations

        [SerializeField] protected TilemapGenerator tilemapGenerator;
        [SerializeField] protected Vector2Int startPosition = Vector2Int.zero;

        #endregion
    }
}