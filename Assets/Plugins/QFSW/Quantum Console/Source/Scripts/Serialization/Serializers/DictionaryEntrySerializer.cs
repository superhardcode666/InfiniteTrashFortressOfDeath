using System.Collections;

namespace QFSW.QC.Serializers
{
    public class DictionaryEntrySerializer : BasicQcSerializer<DictionaryEntry>
    {
        public override string SerializeFormatted(DictionaryEntry value, QuantumTheme theme)
        {
            var innerKey = SerializeRecursive(value.Key, theme);
            var innerValue = SerializeRecursive(value.Value, theme);

            return $"{innerKey}: {innerValue}";
        }
    }
}