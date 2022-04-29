using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace QFSW.QC
{
    /// <summary>
    ///     Creates a prefix that will be prepended to all commands made within this class. Works recursively with sub-classes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Assembly, AllowMultiple = true,
        Inherited = false)]
    public sealed class CommandPrefixAttribute : Attribute
    {
        private static readonly char[] _bannedAliasChars = {' ', '(', ')', '{', '}', '[', ']', '<', '>'};
        public readonly string Prefix;
        public readonly bool Valid = true;

        public CommandPrefixAttribute([CallerMemberName] string prefixName = "")
        {
            Prefix = prefixName;
            foreach (var c in _bannedAliasChars)
                if (Prefix.Contains(c))
                {
                    var errorMessage =
                        $"Development Processor Error: Command prefix '{Prefix}' contains the char '{c}' which is banned. Unexpected behaviour may occurr.";
                    Debug.LogError(errorMessage);

                    Valid = false;
                    throw new ArgumentException(errorMessage, nameof(prefixName));
                }
        }
    }
}