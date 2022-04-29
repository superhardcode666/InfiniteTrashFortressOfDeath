using System;
using System.Collections.Generic;

namespace QFSW.QC.Extras
{
    public static class TypeCommands
    {
        [Command("enum-info", "gets all of the numeric values and value names for the specified enum type.")]
        private static IEnumerable<object> GetEnumInfo(Type enumType)
        {
            if (!enumType.IsEnum) throw new ArgumentException($"Supplied type '{enumType}' must be an enum type");

            var enumInnerType = enumType.GetEnumUnderlyingType();
            var vals = enumType.GetEnumValues();

            for (var i = 0; i < vals.Length; i++)
            {
                var name = vals.GetValue(i);
                var val = Convert.ChangeType(name, enumInnerType);
                var pair = new KeyValuePair<object, object>(val, name);
                yield return pair;
            }
        }
    }
}