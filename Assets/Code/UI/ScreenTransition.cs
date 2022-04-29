using System.Collections;
using Hardcode.ITFOD.Combat;
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
public class ScreenTransition : MonoBehaviour
{
    private void Awake()
    {
        transitionMat = image.material;
        Debug.Log($"Material Fetched: {transitionMat}");
    }

    public void OnDestroy()
    {
        CombatManager.triggerCombatTransition -= BattleScreenTransition;
        Debug.Log("<color=yellow>ScreenTransition:</color> Unsubbed from all Events, shutting down.");
    }

    public void OnCreated()
    {
        ResetTransition();
        CombatManager.triggerCombatTransition += BattleScreenTransition;
        Debug.Log("<color=yellow>ScreenTransition:</color> Set up Dependencies, booting up.");
    }

    public void BattleScreenTransition(bool showBattleScreen)
    {
        ResetTransition();
        StartCoroutine(BattleTransitionRoutine(showBattleScreen));
    }

    private void ResetTransition()
    {
        transitionMat.SetFloat("_CutOff", fullyOpenState);
    }

    private IEnumerator BattleTransitionRoutine(bool combat)
    {
        // Set Shader to revealed state
        if (combat) ResetTransition();

        // conceal
        for (float t = 0; t <= 1; t += Time.deltaTime)
        {
            yield return null; // wait 1 frame

            LeanTween.init();

            LeanTween.value(transitionMat.GetFloat("_CutOff"), -0.6f, transitionTime).setOnUpdate(value =>
            {
                transitionMat.SetFloat("_CutOff", value);
            });
        }

        // Activate/Deactivate CombatScreen here or invoke event to activate it
        GameEvents.toggleCombatScreenUI?.Invoke(combat);

        // then reveal again
        for (float t = 0; t <= 1; t += Time.deltaTime)
        {
            yield return null; // wait 1 frame

            LeanTween.init();

            LeanTween.value(transitionMat.GetFloat("_CutOff"), 1.1f, transitionTime).setOnUpdate(value =>
            {
                transitionMat.SetFloat("_CutOff", value);
            });
        }

        yield return null; // wait 1 frame
    }

    #region Field Declaration

    [SerializeField] private Image image;
    [SerializeField] private float transitionTime = 0.25f;

    public Material transitionMat;

    private readonly float fullyOpenState = 1.1f;
    private float fullyClosedState = -0.6f;

    #endregion
}