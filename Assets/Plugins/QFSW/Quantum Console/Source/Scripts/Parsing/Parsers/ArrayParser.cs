using System;
using System.Collections;

namespace QFSW.QC.Parsers
{
    public class ArrayParser : IQcParser
    {
        public int Priority => -100;

        public bool CanParse(Type type)
        {
            return type.IsArray;
        }

        public object Parse(string value, Type type, Func<string, Type, object> recursiveParser)
        {
            var elementType = type.GetElementType();
            var valueParts = value.ReduceScope('[', ']').SplitScoped(',');

            IList dataArray = Array.CreateInstance(elementType, valueParts.Length);
            for (var i = 0; i < valueParts.Length; i++) dataArray[i] = recursiveParser(valueParts[i], elementType);

            return dataArray;
        }
    }
}