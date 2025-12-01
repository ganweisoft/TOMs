// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTClient.Models;

namespace GWModbusStandard.STD;

/// <summary>
/// 定义Modbus驱动设置命令传参
/// </summary>
public class EquipSetRequestModel
{
    public EquipSetRequestModel(ControlType controlType)
    {
        ControlType = controlType;
        WriteList = new List<ModbusInput>();
        ReadList = new List<ModbusInput>();
    }
    private ControlType ControlType { get; set; }

    public List<ModbusInput> WriteList { get; set; }

    public List<ModbusInput> ReadList { get; set; }
}


public class EquipSetResponseModel
{
    public int Code { get; set; } = 200;

    public string Message { get; set; }

    public object Data { get; set; }

    public void Fail(string msg, object data = null)
    {
        Data = data;
        Code = 400;
        Message = msg;
    }

    public void Ok(object data = null)
    {
        Data = data;
        Code = 200;
        Message = "命令执行成功";
    }
}

public enum ControlType
{
    Write,
    Read,
    WriteRead
}
