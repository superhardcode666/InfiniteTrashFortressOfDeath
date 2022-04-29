using System.Collections;
using Hardcode.ITFOD.UI;
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
namespace Hardcode.ITFOD.Combat
{
    public class CombatDummy : MonoBehaviour
    {
        [SerializeField] private Vector3 basePosition;

        [SerializeField] private float tweenTime = 0.25f;

        private void OnDestroy()
        {
            CombatUI.combatScreenReady -= PlayIdleRoutine;
        }

        public void Init()
        {
            CombatUI.combatScreenReady += PlayIdleRoutine;
            combatDummy = GetComponent<Image>();
        }

        public void SetUpDummy(Sprite dummySprite)
        {
            isDying = false;
            SetDefaultMaterial();
            combatDummy.sprite = dummySprite;
            EnableDummy();
        }

        public void DisableDummy()
        {
            Debug.Log("CombatDummy disabled.");
            combatDummy.enabled = false;
            StopCoroutine(IdleRoutine());
            ;
        }

        private void EnableDummy()
        {
            Debug.Log("CombatDummy Enabled!");
            combatDummy.enabled = enabled;
        }

        public void ClearDummy()
        {
            isDying = false;
            SetDefaultMaterial();

            combatDummy.sprite = null;
            DisableDummy();
        }

        public void PlayDeathAnimation()
        {
            if (!isDying) StartCoroutine(DeathAnimationRoutine());
        }

        private IEnumerator DeathAnimationRoutine()
        {
            isDying = true;
            fadeValue = 1f;

            deathMaterial.SetFloat("_Fade", fadeValue);
            combatDummy.material = deathMaterial;

            while (fadeValue >= 0f)
            {
                fadeValue -= Time.deltaTime / deathDuration;
                fadeValue = Mathf.Clamp01(fadeValue);

                combatDummy.material.SetFloat("_Fade", fadeValue);

                yield return null;
            }
        }

        private void SetDefaultMaterial()
        {
            if (!isDying) combatDummy.material = defaultMaterial;
        }

        public void PlayHitFlash()
        {
            StartCoroutine(HitFlashRoutine());
            StartCoroutine(HitWobbleRoutine());
        }

        public void PlayIdleRoutine()
        {
            transform.localPosition = basePosition;
            StartCoroutine(IdleRoutine());
        }

        public void StopIdleRoutine()
        {
            StopCoroutine(IdleRoutine());
            LeanTween.cancel(hoverTweenId);
            Debug.Log("Dummy shouldnt hover anymore...");
        }

        private IEnumerator HitFlashRoutine()
        {
            if (combatDummy == null)
            {
                Debug.Log("CombatDummy Image Reference Missing");
                yield break;
            }

            combatDummy.material = hitFlashMaterial;
            yield return new WaitForSeconds(0.25f);
        }

        private IEnumerator HitWobbleRoutine()
        {
            LeanTween.scaleY(gameObject, 1.25f, tweenTime).setEase(LeanTweenType.punch).setLoopPingPong(1);
            LeanTween.scaleX(gameObject, 1.25f, tweenTime).setEase(LeanTweenType.punch).setLoopPingPong(1)
                .setOnComplete(SetDefaultMaterial);
            yield return null;
        }

        private IEnumerator IdleRoutine()
        {
            hoverTweenId = LeanTween.moveLocalY(gameObject, transform.localPosition.y + 16f, 1.25f)
                .setEase(LeanTweenType.easeInOutSine).setLoopPingPong(-1).id;

            yield return null;
        }

        #region Field Declarations

        [SerializeField] private Material deathMaterial;
        [SerializeField] private Material hitFlashMaterial;
        [SerializeField] private Material defaultMaterial;

        [SerializeField] private Image combatDummy;

        [SerializeField] private float fadeValue = 1f;
        [SerializeField] private float deathDuration = 20f;
        [SerializeField] private bool isDying;

        private int hoverTweenId;

        #endregion
    }
}