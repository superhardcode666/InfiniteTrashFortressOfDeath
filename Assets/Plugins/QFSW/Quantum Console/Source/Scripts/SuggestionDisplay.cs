using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace QFSW.QC
{
    public class SuggestionDisplay : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private QuantumConsole _quantumConsole;
        [SerializeField] private TextMeshProUGUI _textArea;

        public void OnPointerClick(PointerEventData eventData)
        {
            var linkIndex = TMP_TextUtilities.FindIntersectingLink(_textArea, eventData.position, null);
            if (linkIndex >= 0)
            {
                var link = _textArea.textInfo.linkInfo[linkIndex];
                if (int.TryParse(link.GetLinkID(), out var suggestionIndex))
                    _quantumConsole.SetCommandSuggestion(suggestionIndex);
            }
        }
    }
}