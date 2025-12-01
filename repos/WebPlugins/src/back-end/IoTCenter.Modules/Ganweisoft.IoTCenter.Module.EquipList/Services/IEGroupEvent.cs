// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System.Threading.Tasks;

namespace Ganweisoft.IoTCenter.Module.EquipList;

public interface IEGroupEvent
{
    Task Invoke(object data);
}
