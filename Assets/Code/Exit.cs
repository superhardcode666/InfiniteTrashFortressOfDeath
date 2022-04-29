using System;
using FMODUnity;
using Hardcode.ITFOD.Interactions;
using UnityEngine;
using UnityEngine.SceneManagement;

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
public class Exit : Interaction, IInteractable
{
    private void Awake()
    {
        currentScene = SceneManager.GetActiveScene();
    }

    public void Interact()
    {
        Debug.Log("Exit used.");
        playAudio?.Invoke(exitUsed);
        triggerMapChange?.Invoke();
    }

    public void InFocus()
    {
        if (!interactionEmote.activeSelf) interactionEmote.SetActive(true);
    }

    public void OutOfFocus()
    {
        if (interactionEmote.activeSelf) interactionEmote.SetActive(false);
    }

    #region Field Declarations

    [SerializeField] private Scene currentScene;
    public static event Action triggerMapChange;

    public static event Action<EventReference> playAudio;
    [SerializeField] private EventReference exitUsed;

    #endregion
}