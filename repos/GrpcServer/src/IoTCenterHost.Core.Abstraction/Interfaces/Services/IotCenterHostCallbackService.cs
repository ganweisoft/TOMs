//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenterHost.Core.Abstraction.BaseModels;
using IoTCenterHost.Core.Abstraction.ProxyModels;
using System;
using System.Threading.Tasks;

namespace IoTCenterHost.Core.Abstraction.Services
{
    public interface IotCenterHostCallbackService
    {

        Task EquipAddEvent(Action<GrpcEquipItem> action, bool isReUse = false);
        Task EquipChangeEvent(Action<GrpcEquipItem> action, bool isReUse = false);
        Task EquipDeleteEvent(Action<int> action, bool isReUse = false);
        Task EquipStateEvent(Action<GrpcEquipStateItem> action, bool isReUse = false);

        Task YcChangeEvent(Action<GrpcYcItem> action, bool isReUse = false);
        Task YxChangeEvent(Action<GrpcYxItem> action, bool isReUse = false);
        Task AddRealTimeSnapshot(Action<WcfRealTimeEventItem> action, bool isReUse = false);
        Task DeleteRealTimeSnapshot(Action<WcfRealTimeEventItem> action, bool isReUse = false);

    }
}
