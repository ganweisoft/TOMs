// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenterHost.Core.Abstraction;
using IoTCenterHost.Proxy.Connect;
using System.Collections.Concurrent;

namespace IoTCenterHost.Proxy.Core
{
    public interface IConnectPoolManager
    {
        void AddOrUpdateConnectService(string token, ConnectionStatus connectionStatus, IConnectService connectService);
        IConnectService GetConnectService(string token);
        void RemoveConnectService(string token);
        void Clear();
        ConcurrentDictionary<string, IConnectService> CurrentPool { get; set; }
        ConcurrentDictionary<string, ConnectionStatus> ConnectionStatusPool { get; set; }
        string ConnectId { get; set; }

        IEquipManager EquipManager { get; }

        void SyncEquipManager();
    }
}
