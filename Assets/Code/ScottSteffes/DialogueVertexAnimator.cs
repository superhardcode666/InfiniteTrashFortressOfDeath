using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using Hardcode.ITFOD.Audio;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ScottSteffes.AnimatedText
{
    public class DialogueVertexAnimator
    {
        private const float CHAR_ANIM_TIME = 0.07f;

        private const float NOISE_MAGNITUDE_ADJUSTMENT = 0.06f;
        private const float NOISE_FREQUENCY_ADJUSTMENT = 15f;
        private const float WAVE_MAGNITUDE_ADJUSTMENT = 0.06f;

        private static readonly Color32 clear = new(0, 0, 0, 0);
        private static readonly Vector3 vecZero = Vector3.zero;
        private readonly float textAnimationScale;
        private float lastDialogueSound;
        private bool stopAnimating;
        public bool textAnimating;

        public TMP_Text textBox;

        private float timeUntilNextDialogueSound;

        public DialogueVertexAnimator(TMP_Text _textBox)
        {
            textBox = _textBox;
            textAnimationScale = textBox.fontSize;
        }

        public static event Action playDialogueChatter;

        private void PlayDialogueSound()
        {
            if (Time.unscaledTime - lastDialogueSound > timeUntilNextDialogueSound)
            {
                timeUntilNextDialogueSound = Random.Range(0.02f, 0.08f);
                lastDialogueSound = Time.unscaledTime;

                playDialogueChatter?.Invoke();
            }
        }

        public void SetTextBox(TMP_Text _textBox)
        {
            textBox = _textBox;
        }

        public IEnumerator AnimateTextIn(List<DialogueCommand> commands, string processedMessage, Action onFinish)
        {
            textAnimating = true;
            var secondsPerCharacter = 1f / 150f;
            float timeOfLastCharacter = 0;

            var textAnimInfo = SeparateOutTextAnimInfo(commands);
            var textInfo = textBox.textInfo;

            for (var i = 0; i < textInfo.meshInfo.Length; i++) //Clear the mesh 
            {
                var meshInfer = textInfo.meshInfo[i];
                if (meshInfer.vertices != null)
                    for (var j = 0; j < meshInfer.vertices.Length; j++)
                        meshInfer.vertices[j] = vecZero;
            }

            textBox.text = processedMessage;
            textBox.ForceMeshUpdate();

            var cachedMeshInfo = textInfo.CopyMeshInfoVertexData();
            var originalColors = new Color32[textInfo.meshInfo.Length][];
            for (var i = 0; i < originalColors.Length; i++)
            {
                var theColors = textInfo.meshInfo[i].colors32;
                originalColors[i] = new Color32[theColors.Length];
                Array.Copy(theColors, originalColors[i], theColors.Length);
            }

            var charCount = textInfo.characterCount;
            var charAnimStartTimes = new float[charCount];
            for (var i = 0; i < charCount; i++)
                charAnimStartTimes[i] = -1; //indicate the character as not yet started animating.

            var visableCharacterIndex = 0;
            while (true)
            {
                if (stopAnimating)
                {
                    for (var i = visableCharacterIndex; i < charCount; i++) charAnimStartTimes[i] = Time.unscaledTime;

                    visableCharacterIndex = charCount;
                    FinishAnimating(onFinish);
                }

                if (ShouldShowNextCharacter(secondsPerCharacter, timeOfLastCharacter))
                    if (visableCharacterIndex <= charCount)
                    {
                        ExecuteCommandsForCurrentIndex(commands, visableCharacterIndex, ref secondsPerCharacter,
                            ref timeOfLastCharacter);
                        if (visableCharacterIndex < charCount &&
                            ShouldShowNextCharacter(secondsPerCharacter, timeOfLastCharacter))
                        {
                            charAnimStartTimes[visableCharacterIndex] = Time.unscaledTime;

                            PlayDialogueSound();

                            visableCharacterIndex++;
                            timeOfLastCharacter = Time.unscaledTime;
                            if (visableCharacterIndex == charCount) FinishAnimating(onFinish);
                        }
                    }

                for (var j = 0; j < charCount; j++)
                {
                    var charInfo = textInfo.characterInfo[j];
                    if (charInfo.isVisible
                       ) //Invisible characters have a vertexIndex of 0 because they have no vertices and so they should be ignored to avoid messing up the first character in the string whic also has a vertexIndex of 0
                    {
                        var vertexIndex = charInfo.vertexIndex;
                        var materialIndex = charInfo.materialReferenceIndex;
                        var destinationColors = textInfo.meshInfo[materialIndex].colors32;
                        var theColor = j < visableCharacterIndex
                            ? originalColors[materialIndex][vertexIndex]
                            : clear;
                        destinationColors[vertexIndex + 0] = theColor;
                        destinationColors[vertexIndex + 1] = theColor;
                        destinationColors[vertexIndex + 2] = theColor;
                        destinationColors[vertexIndex + 3] = theColor;

                        var sourceVertices = cachedMeshInfo[materialIndex].vertices;
                        var destinationVertices = textInfo.meshInfo[materialIndex].vertices;
                        float charSize = 0;
                        var charAnimStartTime = charAnimStartTimes[j];
                        if (charAnimStartTime >= 0)
                        {
                            var timeSinceAnimStart = Time.unscaledTime - charAnimStartTime;
                            charSize = Mathf.Min(1, timeSinceAnimStart / CHAR_ANIM_TIME);
                        }

                        var animPosAdjustment =
                            GetAnimPosAdjustment(textAnimInfo, j, textBox.fontSize, Time.unscaledTime);
                        var offset = (sourceVertices[vertexIndex + 0] + sourceVertices[vertexIndex + 2]) / 2;
                        destinationVertices[vertexIndex + 0] =
                            (sourceVertices[vertexIndex + 0] - offset) * charSize + offset + animPosAdjustment;
                        destinationVertices[vertexIndex + 1] =
                            (sourceVertices[vertexIndex + 1] - offset) * charSize + offset + animPosAdjustment;
                        destinationVertices[vertexIndex + 2] =
                            (sourceVertices[vertexIndex + 2] - offset) * charSize + offset + animPosAdjustment;
                        destinationVertices[vertexIndex + 3] =
                            (sourceVertices[vertexIndex + 3] - offset) * charSize + offset + animPosAdjustment;
                    }
                }

                textBox.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
                for (var i = 0; i < textInfo.meshInfo.Length; i++)
                {
                    var theInfo = textInfo.meshInfo[i];
                    theInfo.mesh.vertices = theInfo.vertices;
                    textBox.UpdateGeometry(theInfo.mesh, i);
                }

                yield return null;
            }
        }

        private void ExecuteCommandsForCurrentIndex(List<DialogueCommand> commands, int visableCharacterIndex,
            ref float secondsPerCharacter, ref float timeOfLastCharacter)
        {
            for (var i = 0; i < commands.Count; i++)
            {
                var command = commands[i];
                if (command.position == visableCharacterIndex)
                {
                    switch (command.type)
                    {
                        case DialogueCommandType.Pause:
                            timeOfLastCharacter = Time.unscaledTime + command.floatValue;
                            break;
                        case DialogueCommandType.TextSpeedChange:
                            secondsPerCharacter = 1f / command.floatValue;
                            break;
                    }

                    commands.RemoveAt(i);
                    i--;
                }
            }
        }

        private void FinishAnimating(Action onFinish)
        {
            textAnimating = false;
            stopAnimating = false;
            onFinish?.Invoke();
        }

        private Vector3 GetAnimPosAdjustment(TextAnimInfo[] textAnimInfo, int charIndex, float fontSize, float time)
        {
            float x = 0;
            float y = 0;
            for (var i = 0; i < textAnimInfo.Length; i++)
            {
                var info = textAnimInfo[i];
                if (charIndex >= info.startIndex && charIndex < info.endIndex)
                {
                    if (info.type == TextAnimationType.shake)
                    {
                        var scaleAdjust = fontSize * NOISE_MAGNITUDE_ADJUSTMENT;
                        x += (Mathf.PerlinNoise((charIndex + time) * NOISE_FREQUENCY_ADJUSTMENT, 0) - 0.5f) *
                             scaleAdjust;
                        y += (Mathf.PerlinNoise((charIndex + time) * NOISE_FREQUENCY_ADJUSTMENT, 1000) - 0.5f) *
                             scaleAdjust;
                    }
                    else if (info.type == TextAnimationType.wave)
                    {
                        y += Mathf.Sin(charIndex * 1.5f + time * 6) * fontSize * WAVE_MAGNITUDE_ADJUSTMENT;
                    }
                }
            }

            return new Vector3(x, y, 0);
        }

        private static bool ShouldShowNextCharacter(float secondsPerCharacter, float timeOfLastCharacter)
        {
            return Time.unscaledTime - timeOfLastCharacter > secondsPerCharacter;
        }

        public void SkipToEndOfCurrentMessage()
        {
            if (textAnimating) stopAnimating = true;
            Debug.Log("Dialog Animation skipped!");
        }

        private TextAnimInfo[] SeparateOutTextAnimInfo(List<DialogueCommand> commands)
        {
            var tempResult = new List<TextAnimInfo>();
            var animStartCommands = new List<DialogueCommand>();
            var animEndCommands = new List<DialogueCommand>();
            for (var i = 0; i < commands.Count; i++)
            {
                var command = commands[i];
                if (command.type == DialogueCommandType.AnimStart)
                {
                    animStartCommands.Add(command);
                    commands.RemoveAt(i);
                    i--;
                }
                else if (command.type == DialogueCommandType.AnimEnd)
                {
                    animEndCommands.Add(command);
                    commands.RemoveAt(i);
                    i--;
                }
            }

            if (animStartCommands.Count != animEndCommands.Count)
                Debug.LogError("Unequal number of start and end animation commands. Start Commands: " +
                               animStartCommands.Count + " End Commands: " + animEndCommands.Count);
            else
                for (var i = 0; i < animStartCommands.Count; i++)
                {
                    var startCommand = animStartCommands[i];
                    var endCommand = animEndCommands[i];
                    tempResult.Add(new TextAnimInfo
                    {
                        startIndex = startCommand.position,
                        endIndex = endCommand.position,
                        type = startCommand.textAnimValue
                    });
                }

            return tempResult.ToArray();
        }
    }

    public struct TextAnimInfo
    {
        public int startIndex;
        public int endIndex;
        public TextAnimationType type;
    }
}