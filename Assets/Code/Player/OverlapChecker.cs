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
    public class OverlapChecker : MonoBehaviour
    {
        #region Field Declarations

        [SerializeField] private SpriteRenderer spriteRenderer;

        #endregion

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player")) spriteRenderer.color = new Color(1, 1, 1, 0.5f);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player")) spriteRenderer.color = new Color(1, 1, 1, 1);
        }
    }
}