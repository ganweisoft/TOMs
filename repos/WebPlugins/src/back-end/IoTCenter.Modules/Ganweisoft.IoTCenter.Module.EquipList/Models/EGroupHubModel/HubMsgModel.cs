// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
namespace Ganweisoft.IoTCenter.Module.EquipList;

public class HubMsgModel
{
    public bool IsSuccess { get; set; }

    public string Message { get; set; }

    public object Data { get; set; }

    public static HubMsgModel Success(object data = null)
    {
        return new HubMsgModel() { IsSuccess = true, Data = data };
    }
    public static HubMsgModel Error(string msg)
    {
        return new HubMsgModel() { IsSuccess = false, Message = msg };
    }
}
