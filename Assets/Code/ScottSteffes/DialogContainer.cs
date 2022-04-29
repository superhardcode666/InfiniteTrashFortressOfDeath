using System;
using System.Collections.Generic;
using UnityEngine;

namespace ScottSteffes.AnimatedText
{
    [CreateAssetMenu(fileName = "New Dialog Container", menuName = "SUPERETERNALDEATH/Dialog Container")]
    public class DialogContainer : ScriptableObject
    {
        [TextArea] public string[] dialogue;
        public List<DialogResponse> responseOptions;

        public string GetLine(int index)
        {
            if (dialogue.Length >= index) return dialogue[index];

            return null;
        }
    }

    [Serializable]
    public class DialogResponse
    {
        public DialogContainer nextDialog;
        public string responseText;
    }
}