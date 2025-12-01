// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System;

namespace Ganweisoft.IoTCenter.Module.EquipList;

public class CacheJobOptions
{
    public const string CacheOption = nameof(CacheOption);

    public int AbsoluteRepeatTimeSeconds
    {
        get;
        set;
    }

    public int AbsoluteExpirationRelativeToNow
    {
        get;
        set;
    }
}