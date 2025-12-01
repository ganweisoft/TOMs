// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
namespace Ganweisoft.IoTCenter.Module.EquipList;

public class GetExportEquipYcYxpsRespones
{
    public GetExportEquipYcpsRespones[] Ycps { get; set; }
    public GetExportEquipYxpsRespones[] Yxps { get; set; }
}

public class GetExportEquipYcpsRespones
{
    public int StaNo { get; set; }

    public int EquipNo { get; set; }

    public int YcNo { get; set; }

    public string YcName { get; set; }
}

public class GetExportEquipYxpsRespones
{
    public int StaNo { get; set; }

    public int EquipNo { get; set; }

    public int YxNo { get; set; }

    public string YxName { get; set; }
}
