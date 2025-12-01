// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Grpc.Core;
using IoTCenter.Utilities;
using IoTCenterHost.Core.Abstraction;
using IoTCenterHost.Core.Abstraction.BaseModels;
using IoTCenterHost.Proto;
using IoTCenterHost.Proxy.Connect;
using IoTCenterHost.Proxy.Core;
using IoTCenterHost.Proxy.Proxies;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace IoTCenterHost.Proxy.Gateway
{
    public class IoTGatewayManager
    {
        private IServiceScopeFactory _serviceScopeFactory;
        private EquipGatewayExchanger _GatewayExchanger;
        private IotService.IotServiceClient _alarmCenterClient;
        private ILoggingService _logService;
        private readonly IoTServiceClientFactory _iotServiceFactory;

        public IoTGatewayManager(IServiceScopeFactory serviceScopeFactory,
            IConnectPoolManager connectPoolManager)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _GatewayExchanger = serviceScopeFactory.CreateScope().ServiceProvider.GetService<EquipGatewayExchanger>();
            _alarmCenterClient = serviceScopeFactory.CreateScope().ServiceProvider.GetService<IotService.IotServiceClient>();
            _logService = _serviceScopeFactory.CreateScope().ServiceProvider.GetService<ILoggingService>();
            _iotServiceFactory = _serviceScopeFactory.CreateScope().ServiceProvider.GetService<IoTServiceClientFactory>();
            Gateways = new ConcurrentDictionary<int, IoTGateway>();
            this.Main = connectPoolManager;
        }
        public IConnectPoolManager Main { get; }

        private ConcurrentDictionary<int, IoTGateway> Gateways { get; }

        public void Connect(int gatewayId, string url)
        {
            _logService.Info($"{url}开始创建连接");
            var gateway = _serviceScopeFactory.CreateScope().ServiceProvider.GetService<IoTGateway>();
            Gateways.TryAdd(gatewayId, gateway);
            gateway.Create(gatewayId, url);
            gateway.EquipManager.OnEquipStateChanged += EquipManager_OnEquipStateChanged;
            gateway.EquipManager.OnYcValuedChanged += EquipManager_OnYcValuedChanged;
            gateway.EquipManager.OnYxValuedChanged += EquipManager_OnYxValuedChanged;
            gateway.EquipManager.OnRealTimeEventAdded += EquipManager_OnRealTimeEventAdded;
            gateway.GatewayService.OnConnected += GatewayService_OnConnected;
            gateway.GatewayService.OnDisConnected += GatewayService_OnDisConnected;
            _logService.Info($"{url}创建连接已完成");
        }

        private void GatewayService_OnDisConnected(object sender, EventArgs e)
        {
            var service = (IoTSubGatewayService)sender;
            int gatewayId = service.GatewayId;
            _logService.Info($"{service.Address}已断开");
            Main.EquipManager.OffLine(_GatewayExchanger.Collection[gatewayId]?.Select(u => u.EquipNo));
        }

        private void GatewayService_OnTickChanged(object sender, EventArgs e)
        {
            int gatewayId = ((IoTSubGatewayService)sender).GatewayId;
            if (gatewayId != 0)
                BindEquipToMain(gatewayId);
        }

        private void GatewayService_OnConnected(object sender, EventArgs e)
        {
            try
            {
                var service = sender as IoTSubGatewayService;
                int gatewayId = service.GatewayId;
                _logService.Info($"{service.Address}已连接");
                if (gatewayId != 0)
                    BindEquipToMain(gatewayId);
            }
            catch (Exception ex)
            {
                _logService.Error(ex);
            }
        }

        public void ChangeAddress(int gatewayId, string url)
        {
            var gateway = Gateways[gatewayId];
            if (gateway == null)
                return;
            gateway.ChangeAddress(url);
        }

        public void Close(int gatewayId)
        {
            var gateway = Gateways[gatewayId];
            if (gateway == null)
                return;
            _GatewayExchanger.RemoveGateway(gatewayId);
            gateway.EquipManager.OnEquipStateChanged -= EquipManager_OnEquipStateChanged;
            gateway.EquipManager.OnYcValuedChanged -= EquipManager_OnYcValuedChanged;
            gateway.EquipManager.OnYxValuedChanged -= EquipManager_OnYxValuedChanged;
            gateway.EquipManager.OnRealTimeEventAdded -= EquipManager_OnRealTimeEventAdded;
            gateway.GatewayService.OnConnected -= GatewayService_OnConnected;
            gateway.GatewayService.OnTickChanged -= GatewayService_OnTickChanged;
            gateway.GatewayService.OnDisConnected -= GatewayService_OnDisConnected;
            gateway.Close();
            gateway = null;
        }

        public int FindEquipGatewayId(int equipNo)
        {
            var gatewayEquip = _GatewayExchanger.FindEquip(null, equipNo);
            if (gatewayEquip != null)
            {
                return gatewayEquip.GatewayId;
            }
            return 0;
        }

        public TClient GetSubGatewayClient<TClient>(int equipNo) where TClient : ClientBase
        {
            var gatewayEquip = _GatewayExchanger.FindEquip(null, equipNo);


            if (gatewayEquip != null)
            {
                var gateway = Gateways[gatewayEquip.GatewayId];
                if (gateway == null)
                    return null;
                _logService.Debug($"{gateway.Address}创建动态上下文");
                return _iotServiceFactory.CreateContractClient<TClient>(gateway.Address);
            }
            else
            {
                _logService.Debug($"创建依赖注入上下文");
                return _serviceScopeFactory.CreateScope().ServiceProvider.GetService<TClient>();
            }
        }

        public (TClient service, int equipNo, int? pointNo, int? gatewayId) GetSubGatewayInfo<TClient>(int equipNo, int? originPointNo = null, string? ycYxType = "") where TClient : ClientBase
        {
            TClient tclient = null;
            var gatewayEquip = _GatewayExchanger.FindEquip(null, equipNo);
            if (gatewayEquip != null)
            {
                var gateway = Gateways[gatewayEquip.GatewayId];
                _logService.Debug($"创建动态上下文");
                tclient = _iotServiceFactory.CreateContractClient<TClient>(gateway.Address);
                int targetPointNo = 0;
                if (originPointNo.HasValue)
                    switch (ycYxType.ToLower())
                    {
                        case "c":
                            targetPointNo = GetYcItemNo(gatewayEquip, originPointNo.Value);
                            break;
                        case "x":
                            targetPointNo = GetYxItemNo(gatewayEquip, originPointNo.Value);
                            break;
                        case "set":
                            targetPointNo = GetSetItemNo(gatewayEquip, originPointNo.Value);
                            break;
                    }

                _logService.Debug($"{equipNo}获取到子网关设备{gatewayEquip.SubgatewayEquipId},类型{ycYxType}，目标点位{targetPointNo},{gatewayEquip}");
                return (tclient, equipNo: gatewayEquip.SubgatewayEquipId, targetPointNo, gatewayEquip.GatewayId);
            }
            else
            {
                _logService.Debug($"{equipNo}未获取到子网关设备");
                tclient = _serviceScopeFactory.CreateScope().ServiceProvider.GetService<TClient>();
                return (tclient, equipNo, originPointNo, 0);
            }
        }





        private void EquipManager_OnRealTimeEventAdded(object sender, WcfRealTimeEventItem e)
        {
            var equipManager = sender as IEquipManager;
            if (e.Equipno != 0)
            {
                UpdateEquipAndYcyxno(equipManager, e);
            }
            Main.EquipManager.RealTimeEventAdd(e);
        }

        private void UpdateEquipAndYcyxno(IEquipManager equipManager, WcfRealTimeEventItem e)
        {
            var equip = _GatewayExchanger.FindGatewayEquip(equipManager.GatewayId, e.Equipno);
            if (equip == null)
                return;
            e.Equipno = equip.EquipNo;

            if (e.Ycyxno != 0)
            {
                switch (e.Type.ToLower())
                {
                    case "c":
                        e.Ycyxno = GetYcItemNo(equip, e.Ycyxno);
                        break;
                    case "x":
                        e.Ycyxno = GetYxItemNo(equip, e.Ycyxno);
                        break;
                }
            }
        }

        private int GetYcItemNo(GatewayEquip equip, int pointNo)
        {
            return equip.YcItems.ContainsKey(pointNo) ? equip.YcItems[pointNo] : 0;
        }

        private int GetYxItemNo(GatewayEquip equip, int pointNo)
        {
            return equip.YxItems.ContainsKey(pointNo) ? equip.YxItems[pointNo] : 0;
        }

        private int GetSetItemNo(GatewayEquip equip, int pointNo)
        {
            return equip.SetItems.ContainsKey(pointNo) ? equip.SetItems[pointNo] : 0;
        }


        private void EquipManager_OnYxValuedChanged(object sender, IoTCenterHost.Core.Abstraction.BaseModels.GrpcYxItem e)
        {
            var equipManage = sender as IEquipManager;
            if (equipManage == null)
                return;

            var equip = _GatewayExchanger.FindGatewayEquip(equipManage.GatewayId, e.m_iEquipNo);
            if (equip == null)
                return;
            e.m_iEquipNo = equip.EquipNo;
            e.m_iYXNo = equip.YxItems.TryGetValue(e.m_iYXNo, out int yxItem) ? yxItem : 0;
            Main.EquipManager.YxItemChanged(e, true);
        }
        private void EquipManager_OnYcValuedChanged(object sender, IoTCenterHost.Core.Abstraction.BaseModels.GrpcYcItem e)
        {
            var equipManage = sender as IEquipManager;
            if (equipManage == null)
                return;

            var equip = _GatewayExchanger.FindGatewayEquip(equipManage.GatewayId, e.m_iEquipNo);
            if (equip == null)
                return;
            e.m_iEquipNo = equip.EquipNo;
            e.m_iYCNo = equip.YcItems.ContainsKey(e.m_iYCNo) ? equip.YcItems[e.m_iYCNo] : 0;
            Main.EquipManager.YcItemChanged(e, true, out string msg);
            if (!string.IsNullOrEmpty(msg))
            {
                _logService.Error(msg);
            }
        }
        private void EquipManager_OnEquipStateChanged(object sender, IoTCenterHost.Core.Abstraction.ProxyModels.GrpcEquipStateItem e)
        {
            var equipManage = sender as IEquipManager;
            if (equipManage == null)
                return;

            var equip = _GatewayExchanger.FindGatewayEquip(equipManage.GatewayId, e.m_iEquipNo);
            if (equip == null)
                return;

            e.m_iEquipNo = equip.EquipNo;
            Main.EquipManager.EquipStateChanged(e, true);
        }
        public void CreateGatewayEquipList(List<GatewayEquip> equipList)
        {
            _GatewayExchanger.Create(equipList);
        }








        private void BindEquipToMain(int gatewayId)
        {
            if (_GatewayExchanger == null || _GatewayExchanger.Collection == null ||
                !_GatewayExchanger.Collection.ContainsKey(gatewayId) || !Gateways.ContainsKey(gatewayId))
                return;

            var exchangerItem = _GatewayExchanger.Collection[gatewayId];

            var gateway = Gateways[gatewayId];

            _logService.Info($"{gateway.Address}已连接，正在获取数据");

            var result = gateway.GatewayService.GetInitEquipData();

            _logService.Debug($"{gateway.Address}创建网关设备映射");
            var gatewayItems = CreateGatewayMapping(exchangerItem, result);
            _logService.Debug($"{gateway.Address}创建网关设备映射完成");

            if (gatewayItems.Item1 == null)
                return;
            Main.EquipManager.OnLine(gatewayItems.Item1, out string errorMsg);
            _logService.Debug($"{gateway.Address}绑定设备数据已完成，状态：{(string.IsNullOrEmpty(errorMsg) ? "成功" : errorMsg)}");

            Main.EquipManager.AddYcItems(gatewayItems.gatewayYcList, out errorMsg);
            _logService.Debug($"{gateway.Address}绑定遥测数据已完成，状态：{(string.IsNullOrEmpty(errorMsg) ? "成功" : errorMsg)}");

            Main.EquipManager.AddYxItems(gatewayItems.gatewayYxList, out errorMsg);
            _logService.Debug($"{gateway.Address}绑定遥信数据已完成，状态：{(string.IsNullOrEmpty(errorMsg) ? "成功" : errorMsg)}");

            _logService.Info($"{gateway.Address}获取数据已完成");
        }

        private (Dictionary<int, GrpcEquipState>, IEnumerable<GrpcYcItem> gatewayYcList, IEnumerable<GrpcYxItem> gatewayYxList)
            CreateGatewayMapping(ConcurrentBag<GatewayEquip> exchangerItem, (Dictionary<int, GrpcEquipState>, List<GrpcYcItem>, List<GrpcYxItem>) result)
        {
            var gatewayEquipStateJoin = result.Item1?.Join(exchangerItem,
                item => item.Key,
                gatewayEquip => gatewayEquip.SubgatewayEquipId,
                (item, gatewayEquip) => new KeyValuePair<int, GrpcEquipState>(gatewayEquip.EquipNo, item.Value)).ToList();

            var gatewayEquipState = gatewayEquipStateJoin?.ToDictionary(k => k.Key, k => k.Value);


            var gatewayYcList = result.Item2?.Join(exchangerItem,
                item => item.m_iEquipNo,
                gatewayEquip => gatewayEquip.SubgatewayEquipId,
                (item, gatewayEquip) =>
                {
                    if (!gatewayEquip.YcItems.ContainsKey(item.m_iYCNo))
                    {
                        _logService.Debug($"设备号{gatewayEquip.EquipNo} 未创建成功Yc-{item.m_iYCNo}的映射");
                        return null;
                    }

                    item.m_iEquipNo = gatewayEquip.EquipNo; // 交换设备号
                    item.m_iYCNo = gatewayEquip.YcItems[item.m_iYCNo]; // 交换遥测
                    return item;
                });

            var gatewayYxList = result.Item3?.Join(exchangerItem,
                item => item.m_iEquipNo,
                gatewayEquip => gatewayEquip.SubgatewayEquipId,
                (item, gatewayEquip) =>
                {
                    if (!gatewayEquip.YcItems.ContainsKey(item.m_iYXNo))
                    {
                        _logService.Debug($"设备号{gatewayEquip.EquipNo} 未创建成功Yx-{item.m_iYXNo}的映射");
                        return null;
                    }

                    item.m_iEquipNo = gatewayEquip.EquipNo; // 交换设备号
                    item.m_iYXNo = gatewayEquip.YxItems[item.m_iYXNo]; // 交换遥信
                    return item;
                });

            return (gatewayEquipState, gatewayYcList, gatewayYxList);

        }

        public void AddGatewayEquip(int gatewayId, GatewayEquip equip)
        {
            _GatewayExchanger.AddGatewayEquip(gatewayId, equip);

        }

        public void AddGatewayEquipList(List<GatewayEquip> equipList)
        {
            equipList.ForEach(item =>
            {
                _GatewayExchanger.AddGatewayEquip(item.GatewayId, item);
            });
            var newCollect = new ConcurrentDictionary<int, ConcurrentBag<GatewayEquip>>(equipList
           .GroupBy(e => e.GatewayId)
           .ToDictionary(
               g => g.Key,
               g => new ConcurrentBag<GatewayEquip>(g)));

            foreach (var exchangerItem in newCollect)
            {
                var gateway = Gateways[exchangerItem.Key];
                BindEquipToMain(exchangerItem.Key);
            }
        }

        public Dictionary<int, (int online, int offline)> GetGatewayEquipState()
        {
            try
            {
                return _GatewayExchanger.Collection.ToDictionary(
       keyvalue => keyvalue.Key,
       keyvalue =>
       {
           var result = keyvalue.Value.Join(Main.EquipManager.EquipStates, item => item.EquipNo, state => state.Key, (item, state) =>
           {
               return (item.EquipNo, state.Value);
           });
           int offline = result.Count(u => u.Value == GrpcEquipState.NoCommunication);
           int online = result.Count() - offline;
           return (online, offline);
       });
            }
            catch (Exception ex)
            {
                _logService.Error($"获取网关设备状态失败{ex}");
                return new Dictionary<int, (int online, int offline)>();
            }
        }


        public GatewayEquip RemoveGatewayEquip(int gatewayId, int subgatewayEquipNo)
        {
            return _GatewayExchanger.RemoveEquip(gatewayId, subgatewayEquipNo);
        }

        public GatewayEquip FindEquip(int gatewayId, int equipNo)
        {
            return _GatewayExchanger.FindEquip(gatewayId, equipNo);
        }
        public GatewayEquip FindGatewayEquip(int gatewayId, int subgatewayEquipNo)
        {
            return _GatewayExchanger.FindGatewayEquip(gatewayId, subgatewayEquipNo);
        }

        public bool HasSubGateway()
        {
            return Gateways.Any();
        }
    }
}
