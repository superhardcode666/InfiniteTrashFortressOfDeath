using System;
using FMOD.Studio;
using UnityEngine;
using FMODUnity;
using Hardcode.ITFOD.Combat;
using Hardcode.ITFOD.Game;
using Hardcode.ITFOD.Items;
using Hardcode.ITFOD.Npc;
using Hardcode.ITFOD.Player;
using Hardcode.ITFOD.StatSystem;
using Hardcode.ITFOD.UI;
using ScottSteffes.AnimatedText;

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

namespace Hardcode.ITFOD.Audio
{
    // Responsible for Playing all Game Audio
    public enum MusicModes
    {
        Title,
        Dungeon,
        Combat,
        Results
    }
    
    public class AudioManager : MonoBehaviour
    {

        
        private void Awake()
        {
            #region Singleton

            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                Debug.Log("<color=yellow>AudioManager: </color> IMPOSTER AUDIO MANAGER DESTROYED!");
            }

            #endregion

            trackOne.volume = TrackOneMaxVolume;
            trackTwo.volume = TrackTwoMaxVolume;

            UIManager.playUIAudio += PlayAudioEvent;
            ActorStats.playAudio += PlayAudioEvent;
            Actor.playAudio += PlayAudioEvent;
            PlayerController.playAudio += PlayAudioEvent;
            PlayerStats.playAudio += PlayAudioEvent;
            Chest.playAudio += PlayAudioEvent;
            Exit.playAudio += PlayAudioEvent;
            CombatManager.playCombatAudio += PlayAudioEvent;
            DialogManager.playAudio += PlayAudioEvent;
            TitleScreen.playUIAudio += PlayAudioEvent;
            GameManager.playMusic += PlayAudioEvent;
        }

        private void OnDestroy()
        {
            UIManager.playUIAudio -= PlayAudioEvent;
            ActorStats.playAudio -= PlayAudioEvent;
            Actor.playAudio -= PlayAudioEvent;
            PlayerController.playAudio -= PlayAudioEvent;
            PlayerStats.playAudio -= PlayAudioEvent;
            Chest.playAudio -= PlayAudioEvent;
            Exit.playAudio -= PlayAudioEvent;
            CombatManager.playCombatAudio -= PlayAudioEvent;
            DialogManager.playAudio -= PlayAudioEvent;
            TitleScreen.playUIAudio -= PlayAudioEvent;
            GameManager.playMusic -= PlayAudioEvent;
        }

        private void PlayAudioEvent(EventReference eventReference)
        {
            RuntimeManager.PlayOneShot(eventReference);
        }

        [SerializeField] private int track;
        
        private void Update()
        {
            
        }

        public void ChangeMasterVolume(float value)
        {
            AudioListener.volume = value;
        }
        
        #region Field Declarations

        public static AudioManager instance;

        [SerializeField] private AudioSource trackOne, trackTwo;

        [SerializeField] [Range(0, 0.75f)] private float TrackOneMaxVolume = 0.35f;
        [SerializeField] [Range(0, 0.75f)] private float TrackTwoMaxVolume = 0.5f;
        
        #endregion
    }
}