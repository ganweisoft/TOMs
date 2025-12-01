// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System;

namespace Ganweisoft.IoTCenter.Module.EquipList;

public class CurDataRequest
{
    public DateTime Bgn { get; set; }
    public DateTime End { get; set; }

    public int Stano { get; set; }

    public int Eqpno { get; set; }

    public int Ycyxno { get; set; }

    public string @Type { get; set; }

    public bool IsSafe()
    {
        return Stano == 0 || Eqpno == 0 || Ycyxno == 0 || Type != "C" && Type != "X";
    }
}
