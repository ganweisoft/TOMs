// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Utilities;

namespace Ganweisoft.IoTCenter.Module.EquipList;

public class EGroupSearchRequest : QueryRequest
{
    private int _pageSize;
    public new int PageSize
    {
        get
        {
            return _pageSize;
        }
        set
        {
            if (value <= 0)
                _pageSize = 5;
            else _pageSize = value;
        }
    }

    public string EquipName { get; set; }
    public string SystemName { get; set; }
}
