using System;

namespace QFSW.QC
{
    /// <summary>Provides a command paremeter with a description.</summary>
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class CommandParameterDescriptionAttribute : Attribute
    {
        public readonly string Description;
        public readonly bool Valid;

        public CommandParameterDescriptionAttribute(string description)
        {
            Description = description;
            Valid = !string.IsNullOrWhiteSpace(description);
        }
    }
}