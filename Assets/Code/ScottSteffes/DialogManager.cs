using System;
using FMODUnity;
using Hardcode.ITFOD.Npc;
using Hardcode.ITFOD.UI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;


// most gratefully adapted from Scott Steffe
// https://github.com/markv12/VertexTextAnimationDemo
namespace ScottSteffes.AnimatedText
{
    public class DialogManager : MonoBehaviour
    {
        public TMP_Text combatTextBox;
        public TMP_Text textBox;
        public TMP_Text combatResultsTextBox;

        [SerializeField] private GameObject combatWindow;

        public DialogContainer currentDialogue;

        [SerializeField] protected string defaultText = "";

        [SerializeField] private GameObject dialogueWindow;
        [SerializeField] private GameObject progressButton;
        private readonly FocusHelper focusHelper = new();

        private DialogueVertexAnimator combatTextVA;
        private int currentLineIndex;
        private DialogueVertexAnimator dialogTextVA;

        private string[] dialogueLines;
        private RectTransform dialogWindowRect;

        private PlayerInteraction playerInteraction;
        private DialogueVertexAnimator resultsTextVA;

        protected Coroutine typeRoutine;
        public bool IsDialogPlaying { get; private set; }
        
        [SerializeField] private EventReference dialogueNoise;
        public static event Action<EventReference> playAudio;

        private void PlayDialogueChatter()
        {
            playAudio?.Invoke(dialogueNoise);
        }
        
        private void OnDestroy()
        {
            DialogueVertexAnimator.playDialogueChatter -= PlayDialogueChatter;
            CombatUI.updateCombatDialog -= AnimateCombatDialog;
            CombatUI.showCombatResults -= AnimateCombatResults;
            Actor.playActorDialog -= StartDialogSequence;

            playerInteraction.Disable();

            Debug.Log("<color=yellow>DialogManager:</color>  Unsubbed from all Events, shutting down.");
        }

        #region static actions

        public static event Action<GameState> changeGameState;

        #endregion

        public void OnCreated()
        {
            DialogueVertexAnimator.playDialogueChatter += PlayDialogueChatter;
            CombatUI.updateCombatDialog += AnimateCombatDialog;
            CombatUI.showCombatResults += AnimateCombatResults;

            Actor.playActorDialog += StartDialogSequence;

            dialogTextVA = new DialogueVertexAnimator(textBox);
            combatTextVA = new DialogueVertexAnimator(combatTextBox);
            resultsTextVA = new DialogueVertexAnimator(combatResultsTextBox);

            playerInteraction = new PlayerInteraction();
            playerInteraction.Main.Interaction.performed += _ => OnProgressButton();

            dialogWindowRect = dialogueWindow.GetComponent<RectTransform>();
            if (dialogWindowRect == null) Debug.Log("Couldn't fetch RectTransform.");

            Debug.Log("<color=yellow>DialogManager:</color> Set up Dependencies, booting up.");
        }

        private void StartDialogSequence(DialogContainer container)
        {
            Debug.Log("<color=yellow>DialogManager:</color> Dialog Sequence triggered.");

            currentLineIndex = 0;

            SetDialogue(container);
            ShowDialogueWindow();

            PlaySingleDialogueLine();
        }

        public void SetDialogue(DialogContainer container)
        {
            currentDialogue = container;
            dialogueLines = currentDialogue.dialogue;
        }

        public void FlushDialogue()
        {
            currentDialogue = null;
            dialogueLines = null;
        }

        public void ShowDialogueWindow()
        {
            Debug.Log($"Currently selected Object: {EventSystem.current.name}");

            IsDialogPlaying = true;

            Debug.Log("<color=yellow>DialogManager:</color> attempting to show Dialogue Window.");
            dialogueWindow.SetActive(true);

            changeGameState?.Invoke(GameState.Cutscene);

            ActiveProgressButton();
            playerInteraction.Enable();
        }

        public void HideDialogueWindow()
        {
            FlushDialogue();

            dialogueWindow.SetActive(false);
            dialogTextVA.SkipToEndOfCurrentMessage();

            IsDialogPlaying = false;

            changeGameState?.Invoke(GameState.Explore);

            currentLineIndex = 0;
            focusHelper.DropFocus();

            DeactiveProgressButton();
            playerInteraction.Disable();
        }

        protected void PlaySingleDialogueLine()
        {
            if (dialogueLines == null)
            {
                Debug.Log("<color=yellow>DialogManager:</color> No DialogLines to play...");
                return;
            }

            if (currentLineIndex > dialogueLines.Length - 1)
            {
                if (dialogueWindow.activeSelf) HideDialogueWindow();
            }
            else if (currentLineIndex < dialogueLines.Length)
            {
                //AnimateText(dialogueLines[currentLineIndex]);
                AnimateNPCDialog(dialogueLines[currentLineIndex]);
                currentLineIndex += 1;
            }
        }

        public void OnProgressButton()
        {
            PlaySingleDialogueLine();
        }

        private void ActiveProgressButton()
        {
            progressButton.SetActive(true);
        }

        private void DeactiveProgressButton()
        {
            progressButton.SetActive(false);
        }

        private void AnimateAnyText(string message, DialogueVertexAnimator va, TMP_Text textBox)
        {
            va.SetTextBox(textBox);

            this.EnsureCoroutineStopped(ref typeRoutine);
            va.textAnimating = false;
            var commands = DialogueUtility.ProcessInputString(message, out var totalTextMessage);
            typeRoutine = StartCoroutine(va.AnimateTextIn(commands, totalTextMessage, null));
        }

        private void AnimateCombatResults(string message)
        {
            AnimateAnyText(message, resultsTextVA, combatResultsTextBox);
        }

        private void AnimateCombatDialog(string message)
        {
            AnimateAnyText(message, combatTextVA, combatTextBox);
        }

        private void AnimateNPCDialog(string message)
        {
            AnimateAnyText(message, dialogTextVA, textBox);
        }

        #region possible new Dialog Workflow

        public void StartActorDialog(DialogContainer container)
        {
            currentLineIndex = 0;

            SetDialogue(container);

            if (currentDialogue != null) DialogueWindowTweenIn();
        }

        [SerializeField] private Vector3 targetScale = new(1, 1, 1);
        [SerializeField] private Vector3 initialScale = new(1, 0, 1);

        private void DialogueWindowTweenIn()
        {
            changeGameState?.Invoke(GameState.Cutscene);

            IsDialogPlaying = true;

            dialogueWindow.SetActive(true);
            dialogWindowRect.localScale = initialScale;

            LeanTween.scale(dialogWindowRect, targetScale, 2f).setEase(LeanTweenType.easeInOutBack)
                .setOnComplete(PlaySingleDialogueLine);

            var focus = focusHelper.HasFocus(progressButton);
            if (!focus) focusHelper.SetFocus(progressButton);

            ActiveProgressButton();
            playerInteraction.Enable();
        }

        private void DialogueWindowTweenOut()
        {
            changeGameState?.Invoke(GameState.Explore);

            IsDialogPlaying = false;

            LeanTween.scale(dialogWindowRect, initialScale, 2f).setEase(LeanTweenType.easeInOutBack)
                .setOnComplete(DeactivateDialogueWindow);
        }

        private void DeactivateDialogueWindow()
        {
            focusHelper.DropFocus();

            DeactiveProgressButton();

            playerInteraction.Disable();
            dialogueWindow.SetActive(false);
        }

        #endregion
    }
}