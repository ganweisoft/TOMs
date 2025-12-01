// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System;
using System.Linq;
using System.Xml;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IoTCenterCore.ExcelHelper
{
    public class PropertyByName<T>
    {
        private object _propertyValue;

        public PropertyByName(string propertyName, Func<T, object> func = null, bool ignore = false)
        {
            PropertyName = propertyName;
            GetProperty = func;
            PropertyOrderPosition = 0;
            Ignore = ignore;
        }

        public int PropertyOrderPosition { get; set; }

        public Func<T, object> GetProperty { get; private set; }

        public string PropertyName { get; private set; }

        public object PropertyValue
        {
            get
            {
                return IsDropDownCell ? GetItemId(_propertyValue) : _propertyValue;
            }
            set
            {
                _propertyValue = value;
            }
        }

        public int IntValue
        {
            get
            {
                if (PropertyValue == null || !int.TryParse(PropertyValue.ToString(), out int rez))
                    return default;
                return rez;
            }
        }

        public int? IntValueNullable
        {
            get
            {
                if (PropertyValue == null || !int.TryParse(PropertyValue.ToString(), out int rez))
                    return null;
                return rez;
            }
        }

        public long Int64Value
        {
            get
            {
                if (PropertyValue == null || !long.TryParse(PropertyValue.ToString(), out long rez))
                    return default;
                return rez;
            }
        }

        public long? LongValueNullable
        {
            get
            {
                if (PropertyValue == null || !long.TryParse(PropertyValue.ToString(), out long rez))
                    return null;
                return rez;
            }
        }

        public bool BooleanValue
        {
            get
            {
                if (PropertyValue == null || !bool.TryParse(PropertyValue.ToString(), out bool rez))
                    return default;
                return rez;
            }
        }

        public string StringValue
        {
            get
            {
                return PropertyValue == null ? string.Empty : Convert.ToString(PropertyValue);
            }
        }

        public decimal DecimalValue
        {
            get
            {
                if (PropertyValue == null || !decimal.TryParse(PropertyValue.ToString(), out decimal rez))
                    return default;
                return rez;
            }
        }

        public decimal? DecimalValueNullable
        {
            get
            {
                if (PropertyValue == null || !decimal.TryParse(PropertyValue.ToString(), out decimal rez))
                    return null;
                return rez;
            }
        }

        public double DoubleValue
        {
            get
            {
                if (PropertyValue == null || !double.TryParse(PropertyValue.ToString(), out double rez))
                    return default;
                return rez;
            }
        }

        public DateTime? DateTimeNullable
        {
            get
            {
                return PropertyValue == null ? null : DateTime.Parse(DoubleValue.ToString()) as DateTime?;
            }
        }

        public DateTime DateTimeValue
        {
            get
            {
                if (PropertyValue == null || !DateTime.TryParse(PropertyValue.ToString(), out DateTime rez))
                    return default;
                return rez;
            }
        }

        public override string ToString()
        {
            return PropertyName;
        }

        public bool Ignore { get; set; }

        public bool IsDropDownCell
        {
            get { return DropDownElements != null; }
        }

        public string[] GetDropDownElements()
        {
            return DropDownElements.Select(ev => ev.Text).ToArray();
        }

        public string GetItemText(object id)
        {
            return DropDownElements.FirstOrDefault(ev => ev.Value == id.ToString()).Return(ev => ev.Text, string.Empty);
        }

        public int GetItemId(object name)
        {
            return DropDownElements.FirstOrDefault(ev => ev.Text.Trim() == name.Return(s => s.ToString(), string.Empty).Trim()).Return(ev => Convert.ToInt32(ev.Value), 0);
        }

        public SelectList DropDownElements { get; set; }

        public bool AllowBlank { get; set; }

        public bool IsCaption
        {
            get { return PropertyName == StringValue || PropertyName == _propertyValue.ToString(); }
        }
    }

    public static class ExcelExtensions
    {
        public static bool IsNullOrDefault<T>(this T? value) where T : struct
        {
            return default(T).Equals(value.GetValueOrDefault());
        }

        public static string ElText(this XmlNode node, string elName)
        {
            return node.SelectSingleNode(elName).InnerText;
        }

        public static TResult Return<TInput, TResult>(this TInput o, Func<TInput, TResult> evaluator, TResult failureValue)
            where TInput : class
        {
            return o == null ? failureValue : evaluator(o);
        }
    }
}
