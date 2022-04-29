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
public class PlayerInfoText : MonoBehaviour
{
    private void Awake()
    {
        TweenUp();
    }

    private void TweenUp()
    {
        LeanTween.moveY(gameObject, transform.position.y + offset, tweenTime).setEase(LeanTweenType.easeInOutBack)
            .setOnComplete(DestroyMe);
    }

    private void DestroyMe()
    {
        Destroy(gameObject, 1f);
    }

    #region Field Declarations

    public float tweenTime;

    [SerializeField] private float offset;

    #endregion
}