//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System.Collections.Generic;
using System.Linq;

namespace IoTCenterHost.Core
{
    public static class GWCommonExtension
    {

        public static List<string> GetSpecialEquipList(this string browserEquip)
        {
            return browserEquip.Split('#').Where(u => !string.IsNullOrEmpty(u)).ToList();
        }

        public static List<int> GetModuleList(this string systemModel)
        {
            return systemModel.Split('#').Where(u => !string.IsNullOrEmpty(u)).Select(s => int.Parse(s)).ToList();
        }

        public static List<int> GetBrowserEquipList(this string browserEquips)
        {
            return browserEquips.Split('#').Where(u => !string.IsNullOrEmpty(u)).Select(s => int.Parse(s)).ToList();
        }

        public static List<string> GetSetItemList(this string equipUnits)
        {
            return equipUnits.Split('#').Where(u => !string.IsNullOrEmpty(u)).ToList();
        }

        public static List<int> GetControlEquipList(this string controlEquips)
        {
            return controlEquips.Split('#').Where(u => !string.IsNullOrEmpty(u)).Select(s => int.Parse(s)).ToList();
        }

        public static List<string> GetAutoInspectionPages(this string autoInspPages)
        {
            try
            {
                return autoInspPages.Split('#').Length > 0 ? autoInspPages.Split('#').ToList() : null;
            }
            catch
            {
                throw;
            }

        }

        public static List<string> GetHomePageList(this string homePages)
        {
            try
            {
                return homePages.Split('#').Length > 0 ? homePages.Split('#').ToList() : null;
            }
            catch
            {
                throw;
            }
        }

    }
}
