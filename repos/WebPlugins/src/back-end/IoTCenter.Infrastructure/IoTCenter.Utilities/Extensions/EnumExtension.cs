// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace IoTCenter.Utilities
{
    public static class EnumExtension
    {
        public static string GetDescription(this Enum enumItem)
        {
            Type type = enumItem.GetType();
            var fieldInfo = type.GetField(enumItem.ToString());
            if (fieldInfo == null)
            {
                return enumItem.ToString();
            }

            var descriptions = fieldInfo.GetCustomAttributes<DescriptionAttribute>(false);
            if (descriptions == null || descriptions.Count() == 0)
            {
                return enumItem.ToString();
            }

            return descriptions.FirstOrDefault().Description;
        }

        public static string GetDisplayName(this Enum em)
        {
            Type type = em.GetType();
            FieldInfo fd = type.GetField(em.ToString());
            if (fd == null)
                return string.Empty;
            object[] attrs = fd.GetCustomAttributes(typeof(DisplayNameAttribute), false);
            string name = string.Empty;
            foreach (DisplayNameAttribute attr in attrs)
            {
                name = attr.DisplayName;
            }
            return name;
        }

        public static List<EnumberEntity> EnumToList<T>()
        {
            List<EnumberEntity> list = new List<EnumberEntity>();
            foreach (var e in Enum.GetValues(typeof(T)))
            {
                EnumberEntity m = new EnumberEntity();
                object[] objArr = e.GetType().GetField(e.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), true);
                if (objArr != null && objArr.Length > 0)
                {
                    DescriptionAttribute da = objArr[0] as DescriptionAttribute;
                    m.Desction = da.Description;
                }
                m.Value = Convert.ToInt32(e);
                m.Name = e.ToString();
                list.Add(m);
            }
            return list;
        }
    }

    public class EnumberEntity
    {
        public string Desction { set; get; }

        public string Name { set; get; }

        public int Value { set; get; }
    }
}
