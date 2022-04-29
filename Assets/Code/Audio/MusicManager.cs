using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using Hardcode.ITFOD.Audio;
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
namespace Hardcode.ITFOD.Audio
{
    public static class MusicManager
    {
        public static void Init()
        {
            if (isInitialized) return;
            
            musicEventInstance.getPlaybackState(out state);
            Debug.Log(state);

            if (state == PLAYBACK_STATE.STOPPED)
            {
                musicEventInstance = RuntimeManager.CreateInstance(musicEventPath);
                musicEventInstance.start();

                musicDescription = RuntimeManager.GetEventDescription(musicEventPath);
                musicDescription.getParameterDescriptionByName("MusicMode", out parameterDescription);

                parameterId = parameterDescription.id;
            }

            GameEvents.switchMusic += SwitchMusic;
        }

        public static void SwitchMusic(MusicModes mode)
        {
            Debug.Log($"Tryna switch to {mode}");
            
            switch (mode)
            {
                case MusicModes.Title:
                    musicEventInstance.setParameterByID(parameterId, 0);
                    break;
                case MusicModes.Dungeon:
                    musicEventInstance.setParameterByID(parameterId, 1);
                    break;
                case MusicModes.Combat:
                    musicEventInstance.setParameterByID(parameterId, 2);
                    break;
                case MusicModes.Results:
                    musicEventInstance.setParameterByID(parameterId, 3);
                    break;
            }
        }

        #region Field Declarations

        private static EventInstance musicEventInstance;
        private static EventDescription musicDescription;
        private static string musicEventPath = "event:/Music/Music";
        private static PARAMETER_DESCRIPTION parameterDescription;
        private static PARAMETER_ID parameterId;

        private static PLAYBACK_STATE state;

        private static bool isInitialized;

        #endregion
    }
}
