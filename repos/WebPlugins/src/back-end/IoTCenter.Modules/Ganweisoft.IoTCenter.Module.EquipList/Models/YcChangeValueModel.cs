// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
namespace Ganweisoft.IoTCenter.Module.EquipList;

public class YcChangeValueModel
{
    public int EquipNo { get; set; }

    public int YcNo { get; set; }

    public object Value { get; set; }

    public bool IsAlarm { get; set; }

    public string Unit { get; set; }
}
