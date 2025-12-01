// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System.Collections.Generic;

namespace Ganweisoft.IoTCenter.Module.EquipList;

public class EGroupStructResponse
{
    public int GroupId { get; set; }
    public string GroupName { get; set; }
    public int ParentGroupId { get; set; }

    public OneEGroupEquip[] Equips { get; set; }

    public int EquipTotalCount { get; set; }

    public List<Child> Children { get; set; }

    public class Child
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
    }
}
