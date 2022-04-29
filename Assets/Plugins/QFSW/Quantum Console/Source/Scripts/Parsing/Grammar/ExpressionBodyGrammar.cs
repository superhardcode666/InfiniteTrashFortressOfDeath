﻿using System;
using System.Text.RegularExpressions;
using QFSW.QC.Utilities;

namespace QFSW.QC.Grammar
{
    public class ExpressionBodyGrammar : IQcGrammarConstruct
    {
        private readonly Regex _expressionBodyRegex = new Regex(@"^{.+}\??$");

        public int Precedence => 0;

        public bool Match(string value, Type type)
        {
            return _expressionBodyRegex.IsMatch(value);
        }

        public object Parse(string value, Type type, Func<string, Type, object> recursiveParser)
        {
            var nullable = false;
            if (value.EndsWith("?"))
            {
                nullable = true;
                value = value.Substring(0, value.Length - 1);
            }

            value = value.ReduceScope('{', '}');
            var result = QuantumConsoleProcessor.InvokeCommand(value);

            if (result is null)
            {
                if (nullable)
                {
                    if (type.IsClass)
                        return result;
                    throw new ParserInputException(
                        $"Expression body {{{value}}} evaluated to null which is incompatible with the expected type '{type.GetDisplayName()}'.");
                }

                throw new ParserInputException(
                    $"Expression body {{{value}}} evaluated to null. If this is intended, please use nullable expression bodies, {{expr}}?");
            }

            if (result.GetType().IsCastableTo(type, true))
                return type.Cast(result);
            throw new ParserInputException(
                $"Expression body {{{value}}} evaluated to an object of type '{result.GetType().GetDisplayName()}', " +
                $"which is incompatible with the expected type '{type.GetDisplayName()}'.");
        }
    }
}