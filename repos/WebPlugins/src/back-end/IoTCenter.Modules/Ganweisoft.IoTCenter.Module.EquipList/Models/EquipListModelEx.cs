// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System.Collections.Generic;

namespace Ganweisoft.IoTCenter.Module.EquipList;

public class EquipListModelEx
{
    public EquipListModelEx()
    {
        IsChecked = false;
    }

    public int AllCount { get; set; }
    public int GroupId { get; set; }
    public string GroupName { get; set; }
    public int? ParentGroupId { get; set; }
    public int Count { get; set; }
    public List<EquipListExInfo> EquipLists { get; set; } = new List<EquipListExInfo>();
    public List<EquipListModelEx> Children { get; set; } = new List<EquipListModelEx>();
    public bool IsChecked { get; }
}

public class EGroupListModel
{
    public int EGroupListId { get; set; }
    public int StaNo { get; set; }
    public int EquipNo { get; set; }
    public string EquipName { get; set; }
    public int GroupId { get; set; }
}
