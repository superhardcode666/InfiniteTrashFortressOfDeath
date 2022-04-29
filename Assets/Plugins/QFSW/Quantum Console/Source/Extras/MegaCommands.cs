using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using QFSW.QC.Utilities;

namespace QFSW.QC.Extras
{
    public static class MegaCommands
    {
        private static readonly QuantumSerializer Serializer = new QuantumSerializer();
        private static readonly QuantumParser Parser = new QuantumParser();

        private static MethodInfo[] ExtractMethods(Type type, string name)
        {
            const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.InvokeMethod |
                                       BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy;

            var methods = type.GetMethods(flags).Where(x => x.Name == name).ToArray();
            if (!methods.Any())
            {
                var property = type.GetProperty(name, flags);
                if (property != null)
                {
                    methods = new[] {property.GetMethod, property.SetMethod}.Where(x => x != null).ToArray();
                    if (methods.Length > 0) return methods;
                }

                throw new ArgumentException(
                    $"No method or property named {name} could be found in class {Serializer.SerializeFormatted(type)}");
            }

            return methods;
        }

        private static string GenerateSignature(MethodInfo method)
        {
            var paramParts = method.GetParameters()
                .Select(x => (x.Name, x.ParameterType))
                .Select(x => $"{x.ParameterType.GetDisplayName()} {x.Name}");

            var paramSignature = string.Join(", ", paramParts);
            return $"{method.Name}({paramSignature})";
        }

        private static MethodInfo GetIdealOverload(MethodInfo[] methods, bool isStatic, int argc)
        {
            methods = methods.Where(x => x.IsStatic == isStatic).ToArray();

            if (methods.Length == 0)
                throw new ArgumentException($"No {(isStatic ? "static" : "non-static")} overloads could be found.");

            if (methods.Length == 1) return methods[0];

            methods = methods.Where(x => !x.IsGenericMethod).ToArray();
            if (methods.Length == 0) throw new ArgumentException("Generic methods are not supported.");

            var argcMatches = methods.Where(x => x.GetParameters().Length == argc).ToArray();
            if (argcMatches.Length == 1) return argcMatches[0];

            if (argcMatches.Length == 0)
            {
                var signatures = methods.Select(GenerateSignature);
                var combinedSignatures = string.Join("\n", signatures);
                throw new ArgumentException(
                    $"No overloads with {argc} arguments were found. the following overloads are available:\n{combinedSignatures}");
            }
            else
            {
                var signatures = argcMatches.Select(GenerateSignature);
                var combinedSignatures = string.Join("\n", signatures);
                throw new ArgumentException(
                    $"Multiple overloads with the same argument count were found: please specify the types explicitly.\n{combinedSignatures}");
            }
        }

        private static MethodInfo GetIdealOverload(MethodInfo[] methods, bool isStatic, Type[] argTypes)
        {
            // Exact matching
            foreach (var method in methods)
                if (method.IsStatic == isStatic)
                {
                    var methodParamTypes = method.GetParameters().Select(x => x.ParameterType);
                    if (methodParamTypes.SequenceEqual(argTypes)) return method;
                }

            // Polymorphic matching
            foreach (var method in methods)
                if (method.IsStatic == isStatic)
                {
                    var methodParams = method.GetParameters();
                    if (methodParams.Length == argTypes.Length)
                    {
                        var isMatch = methodParams
                            .Select(x => x.ParameterType)
                            .Zip(argTypes, (x, y) => (x, y))
                            .All(pair => pair.x.IsAssignableFrom(pair.y));

                        if (isMatch) return method;
                    }
                }

            throw new ArgumentException("No overload with the supplied argument types could be found.");
        }

        private static object[] CreateArgs(MethodInfo method, string[] rawArgs)
        {
            var methodParams = method.GetParameters();
            var argTypes = methodParams.Select(x => x.ParameterType).ToArray();
            return CreateArgs(method, argTypes, rawArgs);
        }

        private static object[] CreateArgs(MethodInfo method, Type[] argTypes, string[] rawArgs)
        {
            var methodParams = method.GetParameters();
            var defaultArgs = methodParams.Count(x => x.HasDefaultValue);

            if (rawArgs.Length < argTypes.Length - defaultArgs || rawArgs.Length > argTypes.Length)
                throw new ArgumentException(
                    $"Incorrect number ({rawArgs.Length}) of arguments supplied for {Serializer.SerializeFormatted(method.DeclaringType)}.{method.Name}" +
                    $", expected {argTypes.Length}");

            var parsedArgs = new object[argTypes.Length];
            for (var i = 0; i < parsedArgs.Length; i++)
                if (i < rawArgs.Length)
                    parsedArgs[i] = Parser.Parse(rawArgs[i], argTypes[i]);
                else
                    parsedArgs[i] = methodParams[i].DefaultValue;

            return parsedArgs;
        }

        private static object InvokeAndUnwrapException(this MethodInfo method, object[] args)
        {
            try
            {
                return method.Invoke(null, args);
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException;
            }
        }

        private static object InvokeAndUnwrapException(this MethodInfo method, IEnumerable<object> targets,
            object[] args)
        {
            try
            {
                return InvocationTargetFactory.InvokeOnTargets(method, targets, args);
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException;
            }
        }

        [Command("call-static")]
        private static object CallStatic(Type classType, string funcName)
        {
            return CallStatic(classType, funcName, Array.Empty<string>());
        }

        [Command("call-static")]
        private static object CallStatic(Type classType, string funcName, string[] args)
        {
            var methods = ExtractMethods(classType, funcName);
            var method = GetIdealOverload(methods, true, args.Length);

            var parsedArgs = CreateArgs(method, args);
            return method.InvokeAndUnwrapException(parsedArgs);
        }

        [Command("call-static")]
        [CommandDescription(
            "Invokes the specified static method or property with the provided arguments. Provide [argTypes] if there are ambiguous overloads")]
        private static object CallStatic(
            [CommandParameterDescription("Namespace qualified typename of the class.")]
            Type classType,
            [CommandParameterDescription("Name of the method or property.")]
            string funcName,
            [CommandParameterDescription("The arguments for the function call.")]
            string[] args,
            [CommandParameterDescription("The types of the arguments to resolve ambiguous overloads.")]
            Type[] argTypes)
        {
            var methods = ExtractMethods(classType, funcName);
            var method = GetIdealOverload(methods, true, argTypes);

            var parsedArgs = CreateArgs(method, argTypes, args);
            return method.InvokeAndUnwrapException(parsedArgs);
        }

        [Command("call-instance")]
        private static object CallInstance(Type classType, string funcName, MonoTargetType targetType)
        {
            return CallInstance(classType, funcName, targetType, Array.Empty<string>());
        }

        [Command("call-instance")]
        private static object CallInstance(Type classType, string funcName, MonoTargetType targetType, string[] args)
        {
            var methods = ExtractMethods(classType, funcName);
            var method = GetIdealOverload(methods, false, args.Length);

            var parsedArgs = CreateArgs(method, args);
            var targets = InvocationTargetFactory.FindTargets(classType, targetType);
            return method.InvokeAndUnwrapException(targets, parsedArgs);
        }

        [Command("call-instance")]
        [CommandDescription(
            "Invokes the specified non-static method or property with the provided arguments. Provide [argTypes] if there are ambiguous overloads")]
        private static object CallInstance(
            [CommandParameterDescription("Namespace qualified typename of the class.")]
            Type classType,
            [CommandParameterDescription("Name of the method or property.")]
            string funcName,
            [CommandParameterDescription("The MonoTargetType used to find the target instances.")]
            MonoTargetType targetType,
            [CommandParameterDescription("The arguments for the function call.")]
            string[] args,
            [CommandParameterDescription("The types of the arguments to resolve ambiguous overloads.")]
            Type[] argTypes)
        {
            var methods = ExtractMethods(classType, funcName);
            var method = GetIdealOverload(methods, false, argTypes);

            var parsedArgs = CreateArgs(method, argTypes, args);
            var targets = InvocationTargetFactory.FindTargets(classType, targetType);
            return method.InvokeAndUnwrapException(targets, parsedArgs);
        }
    }
}