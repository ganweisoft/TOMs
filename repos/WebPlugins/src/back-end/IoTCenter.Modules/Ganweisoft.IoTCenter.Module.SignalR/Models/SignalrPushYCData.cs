// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
namespace Ganweisoft.IoTCenter.Module.SignalR;

internal class SignalrPushYCData
{
    public int equipNo { get; set; }
    public int ycNo { get; set; }
    public object value { get; set; }
    public bool state { get; set; }
    public string unit { get; set; }
}
