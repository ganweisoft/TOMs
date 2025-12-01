// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System;
using System.Threading.Tasks;

namespace Ganweisoft.IoTCenter.Module.EquipList;

public class DeleteOneGroupEvent : IEGroupEvent
{
    public async Task Invoke(object data)
    {
        if (data == null || data.GetType() != typeof(int))
            return;
        int groupId = Convert.ToInt32(data);

         EGroupStaticStruct.DeleteGroupAsync(groupId);
        await Task.CompletedTask;
    }
}
