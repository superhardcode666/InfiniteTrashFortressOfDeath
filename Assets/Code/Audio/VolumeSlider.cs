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

namespace Hardcode.ITFOD.Audio
{
    public class VolumeSlider : MonoBehaviour
    {
        #region Field Declarations

        [SerializeField] private Slider slider;

        #endregion

        private void Awake()
        {
            slider.onValueChanged.AddListener(val => AudioManager.instance.ChangeMasterVolume(val));
        }
    }
}