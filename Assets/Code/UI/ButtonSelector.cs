using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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

public class ButtonSelector : MonoBehaviour
{
    public void OnGUI()
    {
        currentSelected = EventSystem.current.currentSelectedGameObject;
        cursor.SetActive(currentSelected.name == button.name);
    }

    #region Field Declarations

    private GameObject currentSelected;

    [SerializeField] private Button button;
    [SerializeField] private GameObject cursor;

    #endregion
}