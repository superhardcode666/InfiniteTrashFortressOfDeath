using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;

namespace QFSW.QC.Editor.Tools
{
    public static class SymbolEditor
    {
        public static IEnumerable<T> AsEnumerable<T>(this T val)
        {
            yield return val;
        }

        private static IEnumerable<BuildTargetGroup> GetPresentBuildTargetGroups()
        {
            foreach (var target in (BuildTarget[]) Enum.GetValues(typeof(BuildTarget)))
            {
                var group = BuildPipeline.GetBuildTargetGroup(target);
                if (BuildPipeline.IsBuildTargetSupported(group, target)) yield return group;
            }
        }

        public static void AddSymbol(string symbol)
        {
            AddSymbols(symbol.AsEnumerable());
        }

        public static void AddSymbols(IEnumerable<string> symbols)
        {
            AddSymbols(GetPresentBuildTargetGroups(), symbols);
        }

        public static void AddSymbols(IEnumerable<BuildTargetGroup> groups, IEnumerable<string> symbols)
        {
            foreach (var group in groups)
            {
                var currentSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
                foreach (var symbol in symbols)
                    if (!currentSymbols.Contains(symbol))
                        currentSymbols = $"{currentSymbols};{symbol}";

                PlayerSettings.SetScriptingDefineSymbolsForGroup(group, currentSymbols);
            }
        }

        public static void RemoveSymbol(string symbol)
        {
            RemoveSymbols(symbol.AsEnumerable());
        }

        public static void RemoveSymbols(IEnumerable<string> symbols)
        {
            RemoveSymbols(GetPresentBuildTargetGroups(), symbols);
        }

        public static void RemoveSymbols(IEnumerable<BuildTargetGroup> groups, IEnumerable<string> symbols)
        {
            foreach (var group in groups)
            {
                var currentSymbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
                foreach (var symbol in symbols) currentSymbols = Regex.Replace(currentSymbols, symbol, string.Empty);

                currentSymbols = string.Join(";",
                    currentSymbols.Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries));
                PlayerSettings.SetScriptingDefineSymbolsForGroup(group, currentSymbols);
            }
        }
    }
}