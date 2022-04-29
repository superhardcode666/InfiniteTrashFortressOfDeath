﻿using System;
using System.Collections.Generic;
using System.Reflection;

namespace QFSW.QC.Internal
{
    internal class CustomParameter : ParameterInfo
    {
        private readonly ParameterInfo _internalParameter;

        public CustomParameter(ParameterInfo internalParameter, Type typeOverride, string nameOverride)
        {
            ParameterType = typeOverride;
            Name = nameOverride;
            _internalParameter = internalParameter;
        }

        public CustomParameter(ParameterInfo internalParameter, string nameOverride) : this(internalParameter,
            internalParameter.ParameterType, nameOverride)
        {
        }

        public override Type ParameterType { get; }

        public override string Name { get; }

        public override ParameterAttributes Attributes => _internalParameter.Attributes;
        public override object DefaultValue => _internalParameter.DefaultValue;
        public override IEnumerable<CustomAttributeData> CustomAttributes => _internalParameter.CustomAttributes;
        public override bool HasDefaultValue => _internalParameter.HasDefaultValue;
        public override MemberInfo Member => _internalParameter.Member;
        public override int MetadataToken => _internalParameter.MetadataToken;
        public override int Position => _internalParameter.Position;
        public override object RawDefaultValue => _internalParameter.RawDefaultValue;

        public override bool Equals(object obj)
        {
            return _internalParameter.Equals(obj);
        }

        public override object[] GetCustomAttributes(bool inherit)
        {
            return _internalParameter.GetCustomAttributes(inherit);
        }

        public override object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            return _internalParameter.GetCustomAttributes(attributeType, inherit);
        }

        public override IList<CustomAttributeData> GetCustomAttributesData()
        {
            return _internalParameter.GetCustomAttributesData();
        }

        public override int GetHashCode()
        {
            return _internalParameter.GetHashCode();
        }

        public override Type[] GetOptionalCustomModifiers()
        {
            return _internalParameter.GetOptionalCustomModifiers();
        }

        public override Type[] GetRequiredCustomModifiers()
        {
            return _internalParameter.GetRequiredCustomModifiers();
        }

        public override bool IsDefined(Type attributeType, bool inherit)
        {
            return _internalParameter.IsDefined(attributeType, inherit);
        }

        public override string ToString()
        {
            return _internalParameter.ToString();
        }
    }
}