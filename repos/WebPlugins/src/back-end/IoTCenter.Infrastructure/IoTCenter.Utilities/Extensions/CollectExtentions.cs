// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace IoTCenter.Utilities.Extensions
{
    public static class CollectExtentions
    {
        [DebuggerStepThrough]
        public static bool IsEmpty(this IEnumerable value)
        {
            return value == null || !value.GetEnumerator().MoveNext();
        }
        [DebuggerStepThrough]
        public static bool HasValues(this IEnumerable value)
        {
            return value != null && value.GetEnumerator().MoveNext();
        }

        public static string Join<T>(this IEnumerable<T> values, string separator = ",")
        {
            if (!IsEmpty(values))
            {
                return string.Empty;
            }
            if (separator.IsEmpty())
            {
                separator = ",";
            }
            return string.Join(separator, values);
        }

        public static void ForEach<T>(this IEnumerable<T> values, Action<T> action)
        {
            foreach (T item in values)
            {
                action(item);
            }
        }
    }
}
