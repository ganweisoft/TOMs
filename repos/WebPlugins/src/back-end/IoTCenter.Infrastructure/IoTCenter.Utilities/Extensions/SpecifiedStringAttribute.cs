// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace IoTCenter.Utilities
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SpecifiedStringAttribute : ValidationAttribute
    {
        private readonly string[] _param;

        public SpecifiedStringAttribute([NotNull] params string[] param)
        {
            _param = param ?? throw new ArgumentNullException(nameof(param));
        }

        public override bool IsValid(object value)
        {
            return value is string && _param.Contains(value.ToString());
        }
    }
}