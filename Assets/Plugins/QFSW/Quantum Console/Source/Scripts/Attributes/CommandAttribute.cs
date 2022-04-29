using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace QFSW.QC
{
    /// <summary>
    ///     Marks the associated method as a command, allowing it to be loaded by the QuantumConsoleProcessor. This means it
    ///     will be usable as a command from a Quantum Console.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true,
        Inherited = false)]
    public sealed class CommandAttribute : Attribute
    {
        private static readonly char[] _bannedAliasChars = {' ', '(', ')', '{', '}', '[', ']', '<', '>'};
        public readonly string Alias;
        public readonly string Description;
        public readonly MonoTargetType MonoTarget;
        public readonly Platform SupportedPlatforms;
        public readonly bool Valid = true;

        public CommandAttribute([CallerMemberName] string aliasOverride = "",
            Platform supportedPlatforms = Platform.AllPlatforms, MonoTargetType targetType = MonoTargetType.Single)
        {
            Alias = aliasOverride;
            MonoTarget = targetType;
            SupportedPlatforms = supportedPlatforms;

            for (var i = 0; i < _bannedAliasChars.Length; i++)
                if (Alias.Contains(_bannedAliasChars[i]))
                {
                    var errorMessage =
                        $"Development Processor Error: Command with alias '{Alias}' contains the char '{_bannedAliasChars[i]}' which is banned. Unexpected behaviour may occur.";
                    Debug.LogError(errorMessage);
                    Valid = false;
                    throw new ArgumentException(errorMessage, nameof(aliasOverride));
                }
        }

        public CommandAttribute(string aliasOverride, MonoTargetType targetType,
            Platform supportedPlatforms = Platform.AllPlatforms) : this(aliasOverride, supportedPlatforms, targetType)
        {
        }

        public CommandAttribute(string aliasOverride, string description,
            Platform supportedPlatforms = Platform.AllPlatforms, MonoTargetType targetType = MonoTargetType.Single) :
            this(aliasOverride, supportedPlatforms, targetType)
        {
            Description = description;
        }

        public CommandAttribute(string aliasOverride, string description, MonoTargetType targetType,
            Platform supportedPlatforms = Platform.AllPlatforms) : this(aliasOverride, description, supportedPlatforms,
            targetType)
        {
        }
    }
}