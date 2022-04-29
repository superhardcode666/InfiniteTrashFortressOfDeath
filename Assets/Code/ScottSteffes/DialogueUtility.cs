﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace ScottSteffes.AnimatedText
{
    public class DialogueUtility : MonoBehaviour
    {
        // grab the remainder of the text until ">" or end of string
        private const string REMAINDER_REGEX = "(.*?((?=>)|(/|$)))";
        private const string PAUSE_REGEX_STRING = "<p:(?<pause>" + REMAINDER_REGEX + ")>";
        private const string SPEED_REGEX_STRING = "<sp:(?<speed>" + REMAINDER_REGEX + ")>";
        private const string ANIM_START_REGEX_STRING = "<anim:(?<anim>" + REMAINDER_REGEX + ")>";
        private const string ANIM_END_REGEX_STRING = "</anim>";
        private static readonly Regex pauseRegex = new(PAUSE_REGEX_STRING);
        private static readonly Regex speedRegex = new(SPEED_REGEX_STRING);
        private static readonly Regex animStartRegex = new(ANIM_START_REGEX_STRING);
        private static readonly Regex animEndRegex = new(ANIM_END_REGEX_STRING);

        private static readonly Dictionary<string, float> pauseDictionary = new()
        {
            {"tiny", .1f},
            {"short", .25f},
            {"normal", 0.666f},
            {"long", 1f},
            {"read", 2f}
        };

        public static List<DialogueCommand> ProcessInputString(string message, out string processedMessage)
        {
            var result = new List<DialogueCommand>();
            processedMessage = message;

            processedMessage = HandlePauseTags(processedMessage, result);
            processedMessage = HandleSpeedTags(processedMessage, result);
            processedMessage = HandleAnimStartTags(processedMessage, result);
            processedMessage = HandleAnimEndTags(processedMessage, result);

            return result;
        }

        private static string HandleAnimEndTags(string processedMessage, List<DialogueCommand> result)
        {
            var animEndMatches = animEndRegex.Matches(processedMessage);
            foreach (Match match in animEndMatches)
                result.Add(new DialogueCommand
                {
                    position = VisibleCharactersUpToIndex(processedMessage, match.Index),
                    type = DialogueCommandType.AnimEnd
                });

            processedMessage = Regex.Replace(processedMessage, ANIM_END_REGEX_STRING, "");
            return processedMessage;
        }

        private static string HandleAnimStartTags(string processedMessage, List<DialogueCommand> result)
        {
            var animStartMatches = animStartRegex.Matches(processedMessage);
            foreach (Match match in animStartMatches)
            {
                var stringVal = match.Groups["anim"].Value;
                result.Add(new DialogueCommand
                {
                    position = VisibleCharactersUpToIndex(processedMessage, match.Index),
                    type = DialogueCommandType.AnimStart,
                    textAnimValue = GetTextAnimationType(stringVal)
                });
            }

            processedMessage = Regex.Replace(processedMessage, ANIM_START_REGEX_STRING, "");
            return processedMessage;
        }

        private static string HandleSpeedTags(string processedMessage, List<DialogueCommand> result)
        {
            var speedMatches = speedRegex.Matches(processedMessage);
            foreach (Match match in speedMatches)
            {
                var stringVal = match.Groups["speed"].Value;
                if (!float.TryParse(stringVal, out var val)) val = 150f;

                result.Add(new DialogueCommand
                {
                    position = VisibleCharactersUpToIndex(processedMessage, match.Index),
                    type = DialogueCommandType.TextSpeedChange,
                    floatValue = val
                });
            }

            processedMessage = Regex.Replace(processedMessage, SPEED_REGEX_STRING, "");
            return processedMessage;
        }

        private static string HandlePauseTags(string processedMessage, List<DialogueCommand> result)
        {
            var pauseMatches = pauseRegex.Matches(processedMessage);
            foreach (Match match in pauseMatches)
            {
                var val = match.Groups["pause"].Value;
                var pauseName = val;
                Debug.Assert(pauseDictionary.ContainsKey(pauseName), "no pause registered for '" + pauseName + "'");
                result.Add(new DialogueCommand
                {
                    position = VisibleCharactersUpToIndex(processedMessage, match.Index),
                    type = DialogueCommandType.Pause,
                    floatValue = pauseDictionary[pauseName]
                });
            }

            processedMessage = Regex.Replace(processedMessage, PAUSE_REGEX_STRING, "");
            return processedMessage;
        }

        private static TextAnimationType GetTextAnimationType(string stringVal)
        {
            TextAnimationType result;
            try
            {
                result = (TextAnimationType) Enum.Parse(typeof(TextAnimationType), stringVal, true);
            }
            catch (ArgumentException)
            {
                Debug.LogError("Invalid Text Animation Type: " + stringVal);
                result = TextAnimationType.none;
            }

            return result;
        }

        private static int VisibleCharactersUpToIndex(string message, int index)
        {
            var result = 0;
            var insideBrackets = false;
            for (var i = 0; i < index; i++)
            {
                if (message[i] == '<')
                {
                    insideBrackets = true;
                }
                else if (message[i] == '>')
                {
                    insideBrackets = false;
                    result--;
                }

                if (!insideBrackets)
                    result++;
                else if (i + 6 < index && message.Substring(i, 6) == "sprite") result++;
            }

            return result;
        }
    }

    public struct DialogueCommand
    {
        public int position;
        public DialogueCommandType type;
        public float floatValue;
        public string stringValue;
        public TextAnimationType textAnimValue;
    }

    public enum DialogueCommandType
    {
        Pause,
        TextSpeedChange,
        AnimStart,
        AnimEnd
    }

    public enum TextAnimationType
    {
        none,
        shake,
        wave
    }
}