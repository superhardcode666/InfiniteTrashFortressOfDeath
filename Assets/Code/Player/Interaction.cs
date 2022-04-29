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
namespace Hardcode.ITFOD.Interactions
{
    /// <summary>
    ///     Base Class for all interactable Objects within the Game World
    /// </summary>
    /// <remarks>
    /// </remarks>
    public class Interaction : MonoBehaviour
    {
        #region Field Declarations

        [SerializeField] protected GameObject interactionEmote;

        #endregion
    }
}