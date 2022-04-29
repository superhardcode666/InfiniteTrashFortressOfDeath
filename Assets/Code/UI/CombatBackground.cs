using System.Collections;
using UnityEngine;
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

namespace Hardcode.ITFOD.UI
{
    public class CombatBackground : MonoBehaviour
    {
        [SerializeField] private Material hitFlashMaterial;
        [SerializeField] private Material defaultMaterial;
        private RawImage backgroundRender;

        private void Awake()
        {
            backgroundRender = GetComponent<RawImage>();
            if (backgroundRender == null) Debug.LogError("Combatbackground missing!");
        }

        public void PlayHitFlash()
        {
            StartCoroutine(HitFlashRoutine());
        }

        private IEnumerator HitFlashRoutine()
        {
            backgroundRender.material = hitFlashMaterial;
            yield return new WaitForSeconds(0.25f);
            ResetBGMaterial();
        }

        private void ResetBGMaterial()
        {
            if (defaultMaterial == null)
                backgroundRender.material = null;
            else backgroundRender.material = defaultMaterial;
        }
    }
}