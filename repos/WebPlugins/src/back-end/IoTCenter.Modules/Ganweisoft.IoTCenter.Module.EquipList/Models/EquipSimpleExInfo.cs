// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
namespace Ganweisoft.IoTCenter.Module.EquipList;

public class EquipSimpleExInfo : EquipSimpleInfo
{
    public string ProcAdvice { get; set; }
}

public class EquipListInfo
{
    public EquipListInfo()
    {
        
    }
    public EquipListInfo(EGroupListModel model)
    {
        if (model == null)
        {
            return;
        }
        Id = model.EGroupListId;
        StaNo = model.StaNo;
        EquipNo = model.EquipNo;
        EquipName = model.EquipName;
    }

    public int Id { get; set; }
    public int StaNo { get; set; }
    public int EquipNo { get; set; }
    public string EquipName { get; set; }
    public bool IsChecked { get; }
    public object EquipState { get; set; }
    public string SystemName { get; set; }
}


public class EquipListExInfo : EquipListInfo
{
    public string ProcAdvice { get; set; }
}

public class IotDeviceSubSystemInfo
{
    public int EquipNo { get; set; }
    public string SystemName { get; set; }
}

public class EquipListGroup
{
    public int GroupId { get; set; }
    public string GroupName { get; set; }
    public int EGroupListId { get; set; }
    public int ParentGroupId { get; set; }
    public int EquipNo { get; set; }
    public int StaNo { get; set; }
}

public class EquipSimpleInfo
{
    public int EquipNo { get; set; }
    public string EquipName { get; set; }
    public string RelatedPicture { get; set; }
    public int StaNo { get; set; }
    public string SystemName { get; set; }
}
