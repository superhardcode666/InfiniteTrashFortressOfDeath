using System;
using System.Collections;
using FMODUnity;
using Hardcode.ITFOD.Audio;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Hardcode.ITFOD.UI
{
    public class TitleScreen : MonoBehaviour
    {

        public void StartRun()
        {
            playUIAudio?.Invoke(startGame);
            SceneManager.LoadScene(1);
        }
        
        private IEnumerator ButtonHoverRoutine()
        {
            hoverTweenId = LeanTween.moveLocalY(startButton, startButton.transform.localPosition.y + hoverHeight, hoverSpeed)
                .setEase(LeanTweenType.easeInOutSine).setLoopPingPong(-1).id;

            yield return null;
        }

        private void OnEnable()
        {
            MusicManager.Init();
            
            StartCoroutine("ButtonHoverRoutine");
        }

        #region Field Declarations
        
        [SerializeField] private EventReference startGame;
        [SerializeField] private GameObject startButton;
        
        // just in case we need to access the tween id later
        private int hoverTweenId;
        
        [SerializeField] private float hoverHeight = 10f;
        [SerializeField] private float hoverSpeed = 1.25f;
        public static event Action<EventReference> playUIAudio;

        #endregion
    }

}