// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System;

namespace Ganweisoft.IoTCenter.Module.EquipList;

public class EGroupLink<T>
{
    private T _data;
    private EGroupLink<T> _last;
    private EGroupLink<T> _next;

    public EGroupLink<T> Last => _last;
    public EGroupLink<T> Next => _next;

    public EGroupLink(T data)
    {
        if (data is null)
            throw new ArgumentNullException(nameof(data));
        _data = data;
    }

    public T Data => _data;

    public void SetNext(EGroupLink<T> data)
    {
        if (data is null)
            return;
        _next = data;
    }

    public void SetLast(EGroupLink<T> data)
    {
        if (data is null)
            return;
        _last = data;
    }

    public void Invalidate()
    {
        _data = default(T);
        _last = null;
        _next = null;
    }

    ~EGroupLink()
    {
        Invalidate();
    }
}
