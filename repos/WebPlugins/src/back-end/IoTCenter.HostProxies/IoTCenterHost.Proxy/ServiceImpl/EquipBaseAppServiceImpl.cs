// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Google.Protobuf.WellKnownTypes;
using IoTCenter.Utilities;
using IoTCenterCore.AutoMapper;
using IoTCenterHost.Core.Abstraction.AppServices;
using IoTCenterHost.Core.Abstraction.BaseModels;
using IoTCenterHost.Core.Abstraction.IotModels;
using IoTCenterHost.Proto;
using IoTCenterHost.Proxy.Core;
using IoTCenterHost.Proxy.Gateway;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace IoTCenterHost.Proxy.ServiceImpl
{
    public class EquipBaseAppServiceImpl : IEquipBaseClientAppService
    {
        private readonly IotService.IotServiceClient _alarmCenterClient;
        private readonly IotQueryService.IotQueryServiceClient _alarmCenterCallbackClient;
        private readonly Empty empty;
        private readonly IMemoryCacheService _memoryCacheService;
        private readonly IConnectPoolManager _connectPoolManager;
        private readonly IoTGatewayManager _ioTGatewayManager;
        private readonly ILoggingService _logService;
        public EquipBaseAppServiceImpl(IotService.IotServiceClient alarmCenterClient,
            IoTCenterHost.Proto.IotQueryService.IotQueryServiceClient alarmCenterCallbackClient,
            IMemoryCacheService memoryCacheService,
            IConnectPoolManager connectPoolManager, IoTGatewayManager ioTGatewayManager, ILoggingService logService)
        {
            _alarmCenterClient = alarmCenterClient;
            _alarmCenterCallbackClient = alarmCenterCallbackClient;
            empty = new Google.Protobuf.WellKnownTypes.Empty();
            _memoryCacheService = memoryCacheService;
            _connectPoolManager = connectPoolManager;
            _ioTGatewayManager = ioTGatewayManager;
            _logService = logService;
        }

        public void AddChangedEquip(GrpcChangedEquip EqpList)
        {
            _alarmCenterClient.AddChangedEquipAsync(EqpList.MapTo<IoTCenterHost.Proto.ChangedEquip>());
        }

        public void DeleteDebugInfo(int iEquipNo)
        {
            _alarmCenterClient.DeleteDebugInfo(new IoTCenterHost.Proto.IntegerDefine { Result = iEquipNo });
        }

        public int[] GetAddRTEquipItemData()
        {
            return _alarmCenterClient.GetAddRTEquipItemData(empty).Result.ConvertRepeatedInt32();
        }

        public void GetChangedRTEquipItemData()
        {

        }

        public void GetChangedRTEquipItemData1()
        {
            _alarmCenterClient.GetChangedRTEquipItemData1(empty);
        }

        public void GetChangedRTYCItemData1()
        {
            _alarmCenterClient.GetChangedRTYCItemData1(empty);
        }

        public void GetChangedRTYXItemData1()
        {
            _alarmCenterClient.GetChangedRTYXItemData1(empty);
        }

        public int[] GetDelRTEquipItemData()
        {
            return _alarmCenterClient.GetDelRTEquipItemData(empty).Result.ConvertRepeatedInt32();
        }

        public int[] GetEditRTEquipItemData()
        {
            return _alarmCenterClient.GetEditRTEquipItemData(empty).Result.ConvertRepeatedInt32();
        }

        public bool GetEquipDebugState(int iEquipNo)
        {
            return _alarmCenterClient.GetEquipDebugState(new IoTCenterHost.Proto.IntegerDefine { Result = iEquipNo }).Result;
        }

        public string GetEquipListStr()
        {
            return _alarmCenterClient.GetEquipListStr(empty).Result;
        }

        public Dictionary<int, GrpcEquipState> GetEquipStateDict()
        {
            var equipCacheHashKey = $"{BrowserHashCacheKey}_TotalEquip";
            Dictionary<int, GrpcEquipState> keyValues = null;
            if (_memoryCacheService.IsSet(equipCacheHashKey))
            {
                keyValues = _connectPoolManager.EquipManager.GetEquipState();
            }
            else
            {
                keyValues = _alarmCenterClient.GetEquipStateDict(empty).Result.FromJson<Dictionary<int, GrpcEquipState>>();
                _memoryCacheService.Set(equipCacheHashKey, 1, DateTimeOffset.Now.AddSeconds(30));
                _connectPoolManager.EquipManager.OnLine(keyValues, out string msg);
            }
            return keyValues;
        }

        public GrpcEquipState GetEquipStateFromEquipNo(int iEquipNo)
        {
            return _alarmCenterClient.GetEquipStateFromEquipNo(new IoTCenterHost.Proto.IntegerDefine { Result = iEquipNo }).Result.MapTo<GrpcEquipState>();
        }
        public void GetTotalRTEquipItemData1()
        {
            _alarmCenterClient.GetTotalRTEquipItemData1(empty);
        }

        public void GetTotalRTYCItemData1()
        {
            _alarmCenterClient.GetTotalRTYCItemData1(empty);
        }

        public void GetTotalRTYXItemData1()
        {
            _alarmCenterClient.GetTotalRTYXItemData1(empty);
        }

        public void ResetEquipmentLinkage()
        {
            _alarmCenterClient.ResetEquipmentLinkageAsync(empty);
        }


        public void SetEquipDebug(int iEquipNo, bool bFlag)
        {
            _alarmCenterClient.SetEquipDebug(new IoTCenterHost.Proto.SetEquipDebugRequest { IEquipNo = iEquipNo, BFlag = bFlag });
        }

        public void SetEquipNm(int EquipNo, string Nm)
        {
            _alarmCenterClient.SetEquipNm(new IoTCenterHost.Proto.SetEquipNmDefine { EquipNo = EquipNo, Nm = Nm });
        }

        public IEnumerable<GrpcEquipItem> GetTotalEquipData(bool isDynamic)
        {
            return _alarmCenterCallbackClient.GetTotalEquipData(new BoolDefine { Result = isDynamic }).Result.FromJson<IEnumerable<GrpcEquipItem>>();
        }
        public void ResetEquips()
        {
            _alarmCenterClient.ResetEquipsAsync(empty);
        }

        public void ResetEquips(List<int> list)
        {
            _alarmCenterClient.ResetEquipsExAsync(new StringResult { Result = list.ToJson() });
        }

        private const string BrowserHashCacheKey = "EquipList_BrowserHashKey";

        public Dictionary<int, GrpcEquipState> GetEquipStateDict(IEnumerable<int> equipList)
        {
            var keyValues = this.GetEquipStateDict().Where(u => equipList.Contains(u.Key));
            if (keyValues == null || !keyValues.Any())
                keyValues = _alarmCenterClient.GetEquipStateFromEquipNoList(new StringResult { Result = equipList.ToJson() }).Result.FromJson<Dictionary<int, GrpcEquipState>>();
            return keyValues.ToDictionary(kv => kv.Key, kv => kv.Value);
        }
        public void AddChangedEquipList(IEnumerable<GrpcChangedEquip> changedEquips)
        {
            _alarmCenterClient.AddChangedEquipListAsync(new StringResult { Result = changedEquips.ToJson() });
        }

        public PaginationData GetEquipDict(Pagination pagination)
        {
            throw new System.NotImplementedException();
        }
    }
}
