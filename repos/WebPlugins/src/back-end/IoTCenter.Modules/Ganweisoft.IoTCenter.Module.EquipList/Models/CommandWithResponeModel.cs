// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System;
using System.Collections.Generic;
using System.Text;

namespace Ganweisoft.IoTCenter.Module.EquipList;

public class CommandWithResponeModel
{
    public string RquestId { get; set; }

    public int EquipNo { get; set; }

    public int SetNo { get; set; }

    public string MainInstr { get; set; }

    public string MinoInstr { get; set; }

    public string Value { get; set; }

    public string UserName { get; set; }

    public string ResultNotifyUrl { get; set; }

    public string ResponeNotifyUrl { get; set; }
}

public class CommandResponeModel
{
    public object CommandRespones { get; set; }
}

public class CommandResultModel
{
    public object CommandResult { get; set; }
}
