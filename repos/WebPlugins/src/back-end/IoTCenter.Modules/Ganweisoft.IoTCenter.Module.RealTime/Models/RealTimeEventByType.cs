// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System;
using System.Collections.Generic;
using System.Text;

namespace Ganweisoft.IoTCenter.Module.RealTime;

public class RealTimeEventByType
{
    public int PageNo { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPage { get; set; }
    public int IsbConfirmed { get; set; }
    public int NotbConfirmed { get; set; }
    public object List { get; set; }
}

public class RealTimeFilterEventByType : RealTimeEventByType
{
    public int EquipNo { get; set; }
}
