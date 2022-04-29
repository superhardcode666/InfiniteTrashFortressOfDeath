using UnityEngine;

namespace QFSW.QC.Parsers
{
    public class Vector3IntParser : BasicCachedQcParser<Vector3Int>
    {
        public override Vector3Int Parse(string value)
        {
            var vectorParts = value.Split(',');
            var parsedVector = new Vector3Int();

            if (vectorParts.Length < 2 || vectorParts.Length > 3)
                throw new ParserInputException(
                    $"Cannot parse '{value}' as an int vector, the format must be either x,y or x,y,z");

            var i = 0;
            try
            {
                for (; i < vectorParts.Length; i++) parsedVector[i] = int.Parse(vectorParts[i]);

                return parsedVector;
            }
            catch
            {
                throw new ParserInputException($"Cannot parse '{vectorParts[i]}' as it must be integral.");
            }
        }
    }
}