// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenterHost.Core.Abstraction.BaseModels;
using IoTCenterHost.Proxy.Connect;
using System;
using System.Collections.Generic;

namespace IoTCenterHost.Proxy.Proxies
{
    internal interface IoTSubGatewayService
    {
        void Connect(string address, int gatewayId, IEquipManager equipManager);
        void ChangeAddress(string address);
        bool Connected { get; internal set; }


        int GatewayId { get; internal set; }



        string Address { get; }
        int KeepAliveInterval { get; }

        (Dictionary<int, GrpcEquipState>, List<GrpcYcItem>, List<GrpcYxItem>) GetInitEquipData();
        IEnumerable<int>? GetDebugEquipNos();

        event EventHandler<EventArgs> OnConnected;
        event EventHandler<EventArgs> OnDisConnected;

        event EventHandler<EventArgs> OnTickChanged;
    }
}
