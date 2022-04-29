using System;
using System.Collections.Generic;
using QFSW.QC.Utilities;
using TMPro;
using UnityEngine;

namespace QFSW.QC
{
    [CreateAssetMenu(fileName = "Untitled Theme", menuName = "Quantum Console/Theme")]
    public class QuantumTheme : ScriptableObject
    {
        [SerializeField] public List<CollectionFormatter> CollectionFormatters = new List<CollectionFormatter>(0);

        [SerializeField] public Color CommandLogColor = new Color(0, 1, 1);

        [SerializeField] public Color DefaultReturnValueColor = Color.white;
        [SerializeField] public Color ErrorColor = Color.red;
        [SerializeField] public TMP_FontAsset Font;
        [SerializeField] public Color PanelColor = Color.white;
        [SerializeField] public Material PanelMaterial;
        [SerializeField] public Color SelectedSuggestionColor = new Color(1, 1, 0.55f);
        [SerializeField] public Color SuccessColor = Color.green;
        [SerializeField] public Color SuggestionColor = Color.gray;

        [SerializeField] public string TimestampFormat = "[{0}:{1}:{2}]";
        [SerializeField] public List<TypeColorFormatter> TypeFormatters = new List<TypeColorFormatter>(0);
        [SerializeField] public Color WarningColor = new Color(1, 0.5f, 0);

        private T FindTypeFormatter<T>(List<T> formatters, Type type) where T : TypeFormatter
        {
            foreach (var formatter in formatters)
                if (type == formatter.Type || type.IsGenericTypeOf(formatter.Type))
                    return formatter;

            foreach (var formatter in formatters)
                if (formatter.Type.IsAssignableFrom(type))
                    return formatter;

            return null;
        }

        public string ColorizeReturn(string data, Type type)
        {
            var formatter = FindTypeFormatter(TypeFormatters, type);
            if (formatter == null)
                return data.ColorText(DefaultReturnValueColor);
            return data.ColorText(formatter.Color);
        }

        public void GetCollectionFormatting(Type type, out string leftScoper, out string seperator,
            out string rightScoper)
        {
            var formatter = FindTypeFormatter(CollectionFormatters, type);
            if (formatter == null)
            {
                leftScoper = "[";
                seperator = ",";
                rightScoper = "]";
            }
            else
            {
                leftScoper = formatter.LeftScoper.Replace("\\n", "\n");
                seperator = formatter.SeperatorString.Replace("\\n", "\n");
                rightScoper = formatter.RightScoper.Replace("\\n", "\n");
            }
        }
    }
}