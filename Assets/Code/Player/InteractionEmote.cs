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
namespace Hardcode.ITFOD.Interactions
{
    public class InteractionEmote : MonoBehaviour
    {
        #region Field Declarations

        [SerializeField] private SpriteRenderer spriteRenderer;

        #endregion

        private void OnTriggerEnter2D(Collider2D other)
        {
            spriteRenderer.enabled = true;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            spriteRenderer.enabled = false;
        }
    }
}