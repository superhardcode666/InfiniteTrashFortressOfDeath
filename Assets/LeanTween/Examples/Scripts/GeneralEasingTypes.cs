﻿using UnityEngine;

public class GeneralEasingTypes : MonoBehaviour
{
    private readonly string[] easeTypes =
    {
        "EaseLinear", "EaseAnimationCurve", "EaseSpring",
        "EaseInQuad", "EaseOutQuad", "EaseInOutQuad",
        "EaseInCubic", "EaseOutCubic", "EaseInOutCubic",
        "EaseInQuart", "EaseOutQuart", "EaseInOutQuart",
        "EaseInQuint", "EaseOutQuint", "EaseInOutQuint",
        "EaseInSine", "EaseOutSine", "EaseInOutSine",
        "EaseInExpo", "EaseOutExpo", "EaseInOutExpo",
        "EaseInCirc", "EaseOutCirc", "EaseInOutCirc",
        "EaseInBounce", "EaseOutBounce", "EaseInOutBounce",
        "EaseInBack", "EaseOutBack", "EaseInOutBack",
        "EaseInElastic", "EaseOutElastic", "EaseInOutElastic",
        "EasePunch", "EaseShake"
    };

    public AnimationCurve animationCurve;

    public float lineDrawScale = 10f;

    private void Start()
    {
        demoEaseTypes();
    }

    private void demoEaseTypes()
    {
        for (var i = 0; i < easeTypes.Length; i++)
        {
            var easeName = easeTypes[i];
            var obj1 = GameObject.Find(easeName).transform.Find("Line");
            var obj1val = 0f;
            var lt = LeanTween.value(obj1.gameObject, 0f, 1f, 5f).setOnUpdate(val =>
            {
                var vec = obj1.localPosition;
                vec.x = obj1val * lineDrawScale;
                vec.y = val * lineDrawScale;

                obj1.localPosition = vec;

                obj1val += Time.deltaTime / 5f;
                if (obj1val > 1f)
                    obj1val = 0f;
            });
            if (easeName.IndexOf("AnimationCurve") >= 0)
            {
                lt.setEase(animationCurve);
            }
            else
            {
                var theMethod = lt.GetType().GetMethod("set" + easeName);
                theMethod.Invoke(lt, null);
            }

            if (easeName.IndexOf("EasePunch") >= 0)
                lt.setScale(1f);
            else if (easeName.IndexOf("EaseOutBounce") >= 0) lt.setOvershoot(2f);
        }

        LeanTween.delayedCall(gameObject, 10f, resetLines);
        LeanTween.delayedCall(gameObject, 10.1f, demoEaseTypes);
    }

    private void resetLines()
    {
        for (var i = 0; i < easeTypes.Length; i++)
        {
            var obj1 = GameObject.Find(easeTypes[i]).transform.Find("Line");
            obj1.localPosition = new Vector3(0f, 0f, 0f);
        }
    }
}