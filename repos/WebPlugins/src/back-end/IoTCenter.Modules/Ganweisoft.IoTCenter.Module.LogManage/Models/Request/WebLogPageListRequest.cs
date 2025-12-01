// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Utilities;
using System;
using System.ComponentModel.DataAnnotations;

namespace Ganweisoft.IoTCenter.Module.LogManage;


public class WebLogPageListRequest : PageQueryModel
{

    public WebLogType WebLogType { get; set; }

    [DataType(DataType.DateTime)]
    public DateTime? Time { get; set; }
}


public enum WebLogType
{
    Debug = 0,
    Information = 1,
    Warning = 2,
    Error = 3,
    Fatal = 4,
    Verbose = 5
}
