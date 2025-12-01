// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
namespace Ganweisoft.IoTCenter.Module.EquipList;

public class Page
{
    public long pageNo { get; set; }

    public long pageSize { get; set; }

    public int totalCount { get; set; }

    public int totalPage { get; set; }

    public object list { get; set; }
}
