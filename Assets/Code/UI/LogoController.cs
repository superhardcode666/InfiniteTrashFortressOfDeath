using System.Collections;
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

public class LogoController : MonoBehaviour
{
    [SerializeField] private Material logoMaterial;
    [SerializeField] private float duration = 2f;
    [SerializeField] private float fadeValue;

    private void Awake()
    {
        logoMaterial.SetFloat("_Fade", fadeValue);
    }

    private void Start()
    {
        StartCoroutine(PlayLogoIntro());
    }


    private IEnumerator PlayLogoIntro()
    {
        logoMaterial.SetFloat("_Fade", fadeValue);

        StartCoroutine(PlayHover());

        while (fadeValue <= 1f)
        {
            fadeValue += Time.deltaTime / duration;
            fadeValue = Mathf.Clamp01(fadeValue);

            logoMaterial.SetFloat("_Fade", fadeValue);

            yield return null;
        }
    }

    private IEnumerator PlayHover()
    {
        Debug.Log("Hover started!");
        LeanTween.moveLocalY(gameObject, transform.localPosition.y + 12f, 1f).setEase(LeanTweenType.easeInOutSine)
            .setLoopPingPong(-1);

        yield return null;
    }
}