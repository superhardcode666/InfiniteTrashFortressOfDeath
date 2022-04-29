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
namespace Hardcode.ITFOD.UI
{
    public class BasicMenuTween : MonoBehaviour
    {
        private void Awake()
        {
            box.localPosition = new Vector2(0, -Screen.height);
            UIManager.inventoryToggle += MoveBox;
        }

        private void MoveBox(GameState state)
        {
            if (state == visibleState)
                box.LeanMoveY(inPosition, time).setEaseOutBounce().delay = delay;
            else
                box.LeanMoveY(-inPosition, time).setEaseOutBounce().delay = delay;
        }

        #region Field Declarations

        [SerializeField] private RectTransform box;

        [SerializeField] private float time = 0.5f;
        [SerializeField] private float delay = 0.1f;

        [SerializeField] private GameState visibleState;

        [SerializeField] private float inPosition;

        #endregion
    }
}