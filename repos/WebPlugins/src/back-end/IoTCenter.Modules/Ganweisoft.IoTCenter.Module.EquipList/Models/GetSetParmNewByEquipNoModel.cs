// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
namespace Ganweisoft.IoTCenter.Module.EquipList;

public class GetSetParmNewByEquipNoModel
{
    public int staN { get; set; }
    public int equipNo { get; set; }
    public int setNo { get; set; }
    public string setNm { get; set; }
    public string setType { get; set; }
    public string value { get; set; }
    public string mainInstruction { get; internal set; }
    public bool EnableSetParm { get; set; } = true;
}
