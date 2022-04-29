using UnityEditor;
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
    [CustomEditor(typeof(AbstractDungeonGenerator), true)]
    public class DungeonEditor : Editor
    {
        #region Field Declarations

        private AbstractDungeonGenerator generator;

        #endregion

        private void Awake()
        {
            generator = (AbstractDungeonGenerator) target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("Generate Dungeon")) generator.GenerateDungeon();
        }
    }
}