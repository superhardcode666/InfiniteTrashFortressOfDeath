using UnityEngine;
using UnityEngine.EventSystems;

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
namespace Hardcode.ITFOD.UI
{
    public class FocusHelper
    {
        public bool HasFocus(GameObject target)
        {
            return EventSystem.current.currentSelectedGameObject == target;
        }

        public void SetFocus(GameObject target)
        {
            EventSystem.current.SetSelectedGameObject(target);
        }

        public void DropFocus()
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}