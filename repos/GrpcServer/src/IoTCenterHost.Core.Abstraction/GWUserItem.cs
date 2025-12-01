//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System.Collections.Generic;

namespace IoTCenterHost.Core.Abstraction.IotModels
{
    public class GWUserItem
    {
        public List<GWRoleItem> Role_List;
        public List<string> HomePage_List;
        public List<string> AutoInspectionPages_List;
        public int ControlLevel { get; set; }
        public string UserPWD { get; set; }
        public bool IsAdministrator { get; set; }
        public string UserName { get; set; }
        public int ID { get; set; }
        public string Remark { get; set; }

        public bool CanBrowse2Equip(int equipNo)
        {
            if (Role_List.Exists(m => m.name == "ADMIN")) return true;
            var result = false;
            foreach (var item in Role_List)
            {
                if (item.Browse_Equip_List.Exists(m => m == equipNo))
                {
                    result = true;
                    break;
                }
            }
            return result;
        }
    }
}
