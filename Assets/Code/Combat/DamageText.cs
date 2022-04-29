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
public class DamageText : MonoBehaviour
{
    protected void Awake()
    {
        var force = new Vector2(Random.Range(-100, 100), Random.Range(200, 350));
        rb.AddForce(force, ForceMode2D.Impulse);
        rb.AddTorque(250);

        TweenIn();
        TweenOut();
    }

    protected virtual void TweenIn()
    {
        LeanTween.cancel(gameObject);
        transform.localScale = new Vector3(2, 2, 2);

        LeanTween.scale(gameObject, Vector3.one, tweenTime)
            .setEasePunch();
    }

    protected virtual void TweenOut()
    {
        LeanTween.cancel(gameObject);
        LeanTween.scale(gameObject, Vector3.zero, tweenTime).setOnComplete(DestroyMe);
    }

    protected void DestroyMe()
    {
        Destroy(gameObject, 1f);
    }

    #region Field Declarations

    public float tweenTime = 0.5f;

    [SerializeField] private float forceRange = 250;
    [SerializeField] private Rigidbody2D rb;

    #endregion
}