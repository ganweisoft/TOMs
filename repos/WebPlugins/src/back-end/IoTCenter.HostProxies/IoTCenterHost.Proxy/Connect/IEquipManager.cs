// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenterHost.Core.Abstraction;
using IoTCenterHost.Core.Abstraction.BaseModels;
using IoTCenterHost.Core.Abstraction.ProxyModels;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace IoTCenterHost.Proxy.Connect
{
    public interface IEquipManager
    {
        public int GatewayId { get; }
        public string Url { get; }
        internal ConcurrentDictionary<int, GrpcEquipItem> Equips { get; }
        internal ConcurrentDictionary<int, GrpcEquipState> EquipStates { get; }
        internal ConcurrentDictionary<int, ConcurrentDictionary<int, GrpcYcItem>> EquipYcItems { get; }

        internal ConcurrentDictionary<int, ConcurrentDictionary<int, GrpcYxItem>> EquipYxItems { get; }

        event EventHandler<GrpcEquipStateItem> OnEquipStateChanged;

        event EventHandler<GrpcYcItem> OnYcValuedChanged;

        event EventHandler<GrpcYxItem> OnYxValuedChanged;


        event EventHandler<WcfRealTimeEventItem> OnRealTimeEventAdded;


        Dictionary<int, GrpcEquipState> GetEquipState();
        void OffLine(IEnumerable<int> equipNos);

        internal void OnLine(Dictionary<int, GrpcEquipState> keyValuePairs, out string msg);

        internal void AddEquips(IEnumerable<GrpcEquipItem> grpcEquipItem);
        internal void AddEquip(GrpcEquipItem grpcEquipItem, bool eventHandler);

        internal void EditEquip(GrpcEquipItem grpcEquipItem, bool eventHandler);

        internal void RemoveEquip(int equipNo);


        internal void AddYcItems(IEnumerable<GrpcYcItem> grpcYcItems, out string msg);

        internal void YcItemChanged(GrpcYcItem item, bool eventHandler, out string msg);




        internal void AddYxItems(IEnumerable<GrpcYxItem> grpcYcItems, out string msg);

        internal void YxItemChanged(GrpcYxItem item, bool eventHandler);

        internal void EquipStateChanged(GrpcEquipStateItem grpcEquipItem, bool eventHandler);

        internal object GetYCValue(int iEquipNo, int iYcpNo);
        Dictionary<int, object> GetYCValueDictFromEquip(int iEquipNo);
        DateTime GetDataTime(int equipNo, int pointNo, string pointType);
        Dictionary<int, DateTime> GetEquipDataTime(int equipNo, string pointType);

        bool HaveYCP(int EquipNo);

        Dictionary<int, bool> GetYCAlarmStateDictFromEquip(int iEquipNo);

        bool? GetYCAlarmState(int iEquipNo, int iYcpNo);
        string GetYCAlarmComments(int iEqpNo, int iYCPNo);


        object GetYXValue(int iEquipNo, int iYxpNo);


        Dictionary<int, string> GetYXValueDictFromEquip(int iEquipNo);


        bool? HaveYXP(int EquipNo);

        Dictionary<int, bool> GetYXAlarmStateDictFromEquip(int iEquipNo);

        bool? GetYXAlarmState(int iEquipNo, int iYxpNo);

        void RealTimeEventAdd(WcfRealTimeEventItem wcfRealTimeEventItem);
    }
}
