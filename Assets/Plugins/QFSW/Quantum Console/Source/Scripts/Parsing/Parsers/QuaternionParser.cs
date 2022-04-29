﻿using UnityEngine;

namespace QFSW.QC.Parsers
{
    public class QuaternionParser : BasicCachedQcParser<Quaternion>
    {
        public override Quaternion Parse(string value)
        {
            var vector = ParseRecursive<Vector4>(value);
            return new Quaternion(vector.x, vector.y, vector.z, vector.w);
        }
    }
}