//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System.Collections.Generic;

namespace IoTCenterHost.Core.Abstraction.IotModels
{
    public class GWRoleItem
    {
        public static string[] SpecialEqps;
        public static string[] SpecialDrvs;
        public static string strAlarmType;
        public static string strDoorType;
        public static string strVideoType;
        public List<int> Control_Equip_List;
        public List<int> Browse_Pages_List;
        public List<string> Browse_SpecialEquip_List;
        public List<int> Browse_Equip_List;
        public List<string> Control_SetItem_List;
        public List<int> AddinModule_List;

        public string remark { get; set; }
        public bool ischeck { get; set; }
        public string name { get; set; }
    }
}
