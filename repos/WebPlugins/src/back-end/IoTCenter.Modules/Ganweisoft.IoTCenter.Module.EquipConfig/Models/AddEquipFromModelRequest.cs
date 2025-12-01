// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
namespace Ganweisoft.IoTCenter.Module.EquipConfig;

public class AddEquipFromModelRequest
{
    public AddEquipFromModelRequest(int staNo,int equipNo, string equipName)
    {
        StaNo = staNo;
        EquipNo = equipNo;
        EquipName = equipName;
    }
    
    public int StaNo{ get; set; }
    public int EquipNo { get; set; }
    public string EquipName { get; set; }
}
