// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System.Collections.Generic;

namespace Ganweisoft.IoTCenter.Module.EquipList;

public class EquipSetParmQuery
{
    public int StaN { get; set; }
    public int EquipNo { get; set; }
    public string EquipNm { get; set; }
    public int SetNo { get; set; }
    public string SetNm { get; set; }
    public string SetType { get; set; }
    public string Value { get; set; }
}

public class EquipSetParmResonse
{
    public int StaN { get; set; }
    public int EquipNo { get; set; }
    public string EquipNm { get; set; }

    public List<SetList> SetInfos { get; set; }
}

public class SetList
{
    public int SetNo { get; set; }
    public string SetNm { get; set; }
    public string SetType { get; set; }
    public string Value { get; set; }
}