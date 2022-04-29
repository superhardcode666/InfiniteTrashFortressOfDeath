using Hardcode.ITFOD.Game;
using Hardcode.ITFOD.Interactions;
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
public class Interactor : MonoBehaviour
{
    #region Field Declarations

    public IInteractable currentFocus;
    private PlayerInteraction playerInteraction;

    [SerializeField] private LayerMask focusLayer;

    private void Awake()
    {
        playerInteraction = new PlayerInteraction();
        playerInteraction.Enable();
        playerInteraction.Main.Interaction.performed += _ => TryInteract();
    }

    private void Start()
    {
        RoomManager.setLootFocus += InjectNewFocus;
    }

    private void OnDestroy()
    {
        RoomManager.setLootFocus -= InjectNewFocus;
        playerInteraction.Disable();
    }

    private void TryInteract()
    {
        if (currentFocus != null)
        {
            currentFocus.Interact();
            Debug.Log($"Interacting with: {currentFocus}");
            RemoveFocus();
        }
        else
        {
            Debug.Log("Nothing to interact with...");
        }
    }

    private void InjectNewFocus(IInteractable interactable)
    {
        if (currentFocus != null) RemoveFocus();

        SetFocus(interactable);
        Debug.Log($"Injected: {currentFocus}");
    }

    public void CheckForFocus(Vector2 origin, Vector2 direction)
    {
        RemoveFocus();

        Debug.DrawRay(origin, direction, Color.red, 1f);
        var hit = Physics2D.Raycast(origin, direction, 1f, focusLayer);

        if (hit)
        {
            Debug.Log($"Detected: {hit.collider.name}");

            var newFocus = hit.collider.gameObject.GetComponent<IInteractable>();
            if (newFocus != null) SetFocus(newFocus);
        }
    }

    private void SetFocus(IInteractable newFocus)
    {
        currentFocus = newFocus;
        currentFocus.InFocus();
        Debug.Log("Focus set!");
    }

    private void RemoveFocus()
    {
        if (currentFocus == null) return;
        Debug.Log($"Removing: {currentFocus}");
        currentFocus.OutOfFocus();
        currentFocus = null;
    }

    public void ToggleInteractor(bool interactorActive)
    {
        if (interactorActive)
            playerInteraction.Enable();
        else
            playerInteraction.Disable();
    }

    #endregion
}