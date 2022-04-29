﻿using System;

namespace QFSW.QC.Grammar
{
    internal class DynamicBinaryOperator : IBinaryOperator
    {
        private readonly Delegate _del;

        public DynamicBinaryOperator(Delegate del, Type lArg, Type rArg, Type ret)
        {
            _del = del;
            LArg = lArg;
            RArg = rArg;
            Ret = ret;
        }

        public Type LArg { get; }
        public Type RArg { get; }
        public Type Ret { get; }

        public object Invoke(object left, object right)
        {
            return _del.DynamicInvoke(left, right);
        }
    }
}