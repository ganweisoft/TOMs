// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ganweisoft.IoTCenter.Module.Event;

public class SysEventResonse
{
    public string Event { get; set; }
    public string Time { get; set; }
    public string Confirmname { get; set; }

    public string Confirmtime { get; set; }
    public string ConfirmRemark { get; set; }
    public bool IsAsc { get; set; }
}
