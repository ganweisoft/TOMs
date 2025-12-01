// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Google.Protobuf.WellKnownTypes;
using IoTCenter.Utilities;
using IoTCenterHost.Core.Abstraction.AppServices;
using IoTCenterHost.Core.Abstraction.BaseModels;
using IoTCenterHost.Core.Abstraction.IotModels;
using IoTCenterHost.Proto;
using IoTCenterHost.Proxy.Core;
using IoTCenterHost.Proxy.Gateway;
using System.Collections.Generic;
using System.Linq;

namespace IoTCenterHost.Proxy.ServiceImpl
{
    public class YCAppServiceImpl : IYCClientAppService
    {
        private readonly IotService.IotServiceClient _alarmCenterClient;
        private readonly IotQueryService.IotQueryServiceClient _alarmCenterCallbackClient;
        private readonly Empty empty;
        private readonly IConnectPoolManager _connectPoolManager;
        private readonly IoTGatewayManager _ioTGatewayManager;
        private const string PointType = "c";
        private readonly ILoggingService _loggingService;
        public YCAppServiceImpl(IotService.IotServiceClient alarmCenterClient,
            IotQueryService.IotQueryServiceClient alarmCenterCallbackClient,
            IConnectPoolManager connectPoolManager,
            IoTGatewayManager ioTGatewayManager,
            ILoggingService loggingService)
        {
            _alarmCenterClient = alarmCenterClient;
            _alarmCenterCallbackClient = alarmCenterCallbackClient;
            empty = new Empty();
            _connectPoolManager = connectPoolManager;
            _ioTGatewayManager = ioTGatewayManager;
            _loggingService = loggingService;
        }
        public void GetChangedRTYCItemData()
        {
        }

        public void GetChangedRTYCItemData1()
        {
            _alarmCenterClient.GetChangedRTYCItemData1(empty);
        }
        public void GetTotalRTYCItemData()
        {
        }

        public void GetTotalRTYCItemData1()
        {
            _alarmCenterClient.GetTotalRTYCItemData1(empty);
        }

        public string GetYCAlarmComments(int iEqpNo, int iYCPNo)
        {
            var result = _connectPoolManager.EquipManager.GetYCAlarmComments(iEqpNo, iYCPNo);
            if (result == null)
            {
                var item = _ioTGatewayManager.GetSubGatewayInfo<IotService.IotServiceClient>(iEqpNo, iYCPNo, PointType);
                result = item.service.GetYCAlarmComments(new IoTCenterHost.Proto.HaveHistoryCurveRequest { EquipNo = item.equipNo, YCPNo = item.pointNo.Value }).Result;
            }
            return result;
        }

        public bool GetYCAlarmState(int iEquipNo, int iYcpNo)
        {
            var result = _connectPoolManager.EquipManager.GetYCAlarmState(iEquipNo, iYcpNo);
            if (!result.HasValue)
            {
                int gatewayId = _ioTGatewayManager.FindEquipGatewayId(iEquipNo);
                if (gatewayId != 0)
                {
                    var item = _ioTGatewayManager.GetSubGatewayInfo<IotService.IotServiceClient>(iEquipNo, iYcpNo, PointType);
                    result = item.service.GetYCAlarmState(new IoTCenterHost.Proto.GetYCAlarmStateRequest { IEquipNo = item.equipNo, IYcpNo = item.pointNo.Value }).Result;
                }
                else
                {
                    result = _alarmCenterClient.GetYCAlarmState(new IoTCenterHost.Proto.GetYCAlarmStateRequest { IEquipNo = iEquipNo, IYcpNo = iYcpNo }).Result;
                }

            }
            return result.Value;
        }

        public Dictionary<int, bool> GetYCAlarmStateDictFromEquip(int iEquipNo)
        {
            var data = _connectPoolManager.EquipManager.GetYCAlarmStateDictFromEquip(iEquipNo);
            if (data == null)
            {
                int gatewayId = _ioTGatewayManager.FindEquipGatewayId(iEquipNo);
                if (gatewayId != 0)
                {
                    var item = _ioTGatewayManager.GetSubGatewayInfo<IotService.IotServiceClient>(iEquipNo);
                    data = item.service.GetYCAlarmStateDictFromEquip(new IoTCenterHost.Proto.IntegerDefine { Result = item.equipNo }).Result.FromJson<Dictionary<int, bool>>();
                }
                else
                {
                    data = _alarmCenterClient.GetYCAlarmStateDictFromEquip(new IoTCenterHost.Proto.IntegerDefine { Result = iEquipNo }).Result.FromJson<Dictionary<int, bool>>();
                }
            }
            return data;
        }

        public string GetYCPListStr(int iEquipNo)
        {
            var item = _ioTGatewayManager.GetSubGatewayInfo<IotService.IotServiceClient>(iEquipNo);
            return item.service.GetYCPListStr(new IoTCenterHost.Proto.IntegerDefine { Result = iEquipNo }).Result;
        }

        public object GetYCValue(int iEquipNo, int iYcpNo)
        {
            var data = _connectPoolManager.EquipManager.GetYCValue(iEquipNo, iYcpNo);
            int gatewayId = _ioTGatewayManager.FindEquipGatewayId(iEquipNo);
            if (data == null || string.IsNullOrEmpty(data.ToString()) || gatewayId == 0)
            {
                if (gatewayId != 0)
                {
                    var item = _ioTGatewayManager.GetSubGatewayInfo<IotService.IotServiceClient>(iEquipNo);
                    data = item.service.GetYCValue(new IoTCenterHost.Proto.GetYCAlarmStateRequest { IEquipNo = item.equipNo, IYcpNo = item.pointNo.Value }).Result.FromJson<object>();
                }
                else
                {
                    data = _alarmCenterClient.GetYCValue(new IoTCenterHost.Proto.GetYCAlarmStateRequest { IEquipNo = iEquipNo, IYcpNo = iYcpNo }).Result.FromJson<object>();
                }
            }
            return data;
        }

        public Dictionary<int, object> GetYCValueDictFromEquip(int iEquipNo)
        {
            _loggingService.Debug($"开始遥测实时值");
            var data = _connectPoolManager.EquipManager.GetYCValueDictFromEquip(iEquipNo);
            if (data == null || data.All(u => u.Value == null))
            {
                int gatewayId = _ioTGatewayManager.FindEquipGatewayId(iEquipNo);
                if (gatewayId != 0)
                {
                    var item = _ioTGatewayManager.GetSubGatewayInfo<IotService.IotServiceClient>(iEquipNo);
                    data = item.service.GetYCValueDictFromEquip(new IoTCenterHost.Proto.IntegerDefine { Result = item.equipNo }).Result.FromJson<Dictionary<int, object>>();
                }
                else
                {
                    data = _alarmCenterClient.GetYCValueDictFromEquip(new IoTCenterHost.Proto.IntegerDefine { Result = iEquipNo }).Result.FromJson<Dictionary<int, object>>();
                }
                _loggingService.Debug($"获取远程遥测实时值---{iEquipNo},{data.ToJson()}");
            }
            return data;
        }

        public bool HaveYCP(int EquipNo)
        {
            var result = _connectPoolManager.EquipManager.HaveYCP(EquipNo);
            if (result == false)
            {
                var item = _ioTGatewayManager.GetSubGatewayInfo<IotService.IotServiceClient>(EquipNo);
                result = item.service.HaveYCP(new IoTCenterHost.Proto.IntegerDefine { Result = item.equipNo }).Result;
            }
            return result;
        }

        public void SetYcpNm(int EquipNo, int YcpNo, string Nm)
        {
            var item = _ioTGatewayManager.GetSubGatewayInfo<IotService.IotServiceClient>(EquipNo, EquipNo, PointType);
            item.service.SetYcpNm(new IoTCenterHost.Proto.SetYxpNmRequest { EquipNo = item.equipNo, YcpNo = item.pointNo.Value, Nm = Nm });
        }

        public List<GrpcYcItem> GetTotalYCData(bool isDynamic)
        {
            var result = _connectPoolManager.EquipManager.EquipYcItems.SelectMany(outerPair => outerPair.Value.Values).ToList();
            if (result == null || !result.Any())
            {
                result = _alarmCenterCallbackClient.GetTotalYCData(new BoolDefine { Result = isDynamic }).Result.FromJson<List<GrpcYcItem>>();
            }
            return result;
        }

        public PaginationData GetYCPListByEquipNo(Pagination pagination)
        {
            return _alarmCenterClient.GetYCPListByEquipNo(new StringResult { Result = pagination.ToJson() }).Result.FromJson<PaginationData>();
        }

        public PaginationData GetYCPDict(Pagination pagination)
        {
            throw new System.NotImplementedException();
        }
    }
}
