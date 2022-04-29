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
public class CameraShaker : MonoBehaviour
{
    public void ShakeScreen(float time, float power)
    {
        if (!isScreenShaking) StartCoroutine(ScreenShakeForTime(time, power));
    }

    private IEnumerator ScreenShakeForTime(float time, float power)
    {
        Debug.Log("ScreenShaker started!");

        initialPosition = transform.localPosition;

        shakeTime = time;
        shakeAmount = power;
        isScreenShaking = true;

        while (shakeTime >= 0)
        {
            yield return null; // wait 1 frame    
            transform.localPosition = Random.insideUnitSphere * shakeAmount + initialPosition;
            shakeTime -= Time.deltaTime;
        }

        if (isScreenShaking)
        {
            isScreenShaking = false;
            shakeTime = 0.0f;
            transform.localPosition = initialPosition;
        }
    }

    #region Field Declarations

    public float shakeAmount = 10f;

    private float shakeTime;
    private Vector3 initialPosition;
    private bool isScreenShaking;

    #endregion
}