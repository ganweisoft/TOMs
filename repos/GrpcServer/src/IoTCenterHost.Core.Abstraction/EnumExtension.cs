//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System.ComponentModel;

namespace IoTCenterHost.Core.Extension
{
    public static class EnumExtension
    {
        public static string ToDescription<T>(this T item)
        {
            var desc = string.Empty;
            var attrs = item.GetType().GetField(item.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), true);
            if (attrs != null && attrs.Length > 0)
            {
                DescriptionAttribute descAttr = attrs[0] as DescriptionAttribute;
                desc = descAttr.Description;
            }
            return desc;
        }
    }
}
