using System.Collections;
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
namespace Hardcode.ITFOD.UI
{
    public class FadeOutUIText : MonoBehaviour
    {
        public Color ColorTint;
        public float FadeSpeed = 1.0F;
        public int RolloverCharacterSpread = 10;
        private TextMeshProUGUI m_TextComponent;

        public void OnCreated()
        {
            m_TextComponent = GetComponent<TextMeshProUGUI>();
            StartCoroutine(AnimateVertexColors());
        }

        /// <summary>
        ///     Method to animate vertex colors of a TMP Text object.
        /// </summary>
        /// <returns></returns>
        private IEnumerator AnimateVertexColors()
        {
            // Need to force the text object to be generated so we have valid data to work with right from the start.
            m_TextComponent.ForceMeshUpdate();
            var textInfo = m_TextComponent.textInfo;
            Color32[] newVertexColors;
            var currentCharacter = m_TextComponent.textInfo.characterCount;
            var startingCharacterRange = currentCharacter;
            var isRangeMax = false;
            while (!isRangeMax)
            {
                var characterCount = textInfo.characterCount;
                // Spread should not exceed the number of characters.
                var fadeSteps = (byte) Mathf.Max(1, 255 / RolloverCharacterSpread);
                for (var i = startingCharacterRange; i < currentCharacter + 1; i++)
                {
                    // Skip characters that are not visible
                    if (!textInfo.characterInfo[i].isVisible) continue;
                    // Get the index of the material used by the current character.
                    var materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
                    // Get the vertex colors of the mesh used by this text element (character or sprite).
                    newVertexColors = textInfo.meshInfo[materialIndex].colors32;
                    // Get the index of the first vertex used by this text element.
                    var vertexIndex = textInfo.characterInfo[i].vertexIndex;
                    // Get the current character's alpha value.
                    var alpha = (byte) Mathf.Clamp(newVertexColors[vertexIndex + 0].a - fadeSteps, 0, 255);
                    // Set new alpha values.
                    newVertexColors[vertexIndex + 0].a = alpha;
                    newVertexColors[vertexIndex + 1].a = alpha;
                    newVertexColors[vertexIndex + 2].a = alpha;
                    newVertexColors[vertexIndex + 3].a = alpha;
                    // Tint vertex colors
                    // Note: Vertex colors are Color32 so we need to cast to Color to multiply with tint which is Color.
                    newVertexColors[vertexIndex + 0] = newVertexColors[vertexIndex + 0] * ColorTint;
                    newVertexColors[vertexIndex + 1] = newVertexColors[vertexIndex + 1] * ColorTint;
                    newVertexColors[vertexIndex + 2] = newVertexColors[vertexIndex + 2] * ColorTint;
                    newVertexColors[vertexIndex + 3] = newVertexColors[vertexIndex + 3] * ColorTint;
                    if (alpha == 0)
                    {
                        startingCharacterRange += 1;
                        if (startingCharacterRange == characterCount)
                        {
                            // Update mesh vertex data one last time.
                            m_TextComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
                            yield return new WaitForSeconds(1.0f);
                            // Reset the text object back to original state.
                            m_TextComponent.ForceMeshUpdate();
                            yield return new WaitForSeconds(1.0f);
                            // Reset our counters.
                            currentCharacter = m_TextComponent.textInfo.characterCount;
                            startingCharacterRange = m_TextComponent.textInfo.characterCount;
                            isRangeMax = true;
                        }
                    }
                }

                // Upload the changed vertex colors to the Mesh.
                m_TextComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
                if (currentCharacter + 1 < characterCount) currentCharacter += 1;
                yield return new WaitForSeconds(0.25f - FadeSpeed * 0.01f);
            }
        }
    }
}