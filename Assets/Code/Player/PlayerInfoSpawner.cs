using Hardcode.ITFOD.StatSystem;
using TMPro;
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
public class PlayerInfoSpawner : MonoBehaviour
{
    #region Field Declarations

    [SerializeField] private GameObject pickUpInfoText;

    #endregion

    private void Start()
    {
        GameEvents.displayPlayerInfoMessage += ShowPickupInfoMessage;
        PlayerStats.displayLevelUpMessage += ShowPickupInfoMessage;
    }

    private void OnDestroy()
    {
        GameEvents.displayPlayerInfoMessage -= ShowPickupInfoMessage;
        PlayerStats.displayLevelUpMessage -= ShowPickupInfoMessage;
    }

    private void ShowPickupInfoMessage(string message)
    {
        var feedback = Instantiate(pickUpInfoText, transform.position, Quaternion.identity);
        var feedbackTxt = feedback.GetComponent<TextMeshPro>();

        if (feedbackTxt != null)
            feedbackTxt.text = message;
        else Debug.Log("TextMesh Component not found!");
    }
}