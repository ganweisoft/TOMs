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
    public class YXAppServiceImpl : IYXClientAppService
    {
        private readonly IotService.IotServiceClient _alarmCenterClient;
        private readonly IotQueryService.IotQueryServiceClient _iotQueryService;
        private readonly IConnectPoolManager _connectPoolManager;
        private readonly ILoggingService _loggingService;
        private readonly Empty empty;
        private IoTGatewayManager _ioTGatewayManager;
        private const string PointType = "x";
        public YXAppServiceImpl(
            IotService.IotServiceClient alarmCenterClient,
            IotQueryService.IotQueryServiceClient iotQueryServiceClient,
            IConnectPoolManager connectPoolManager,
            ILoggingService loggingService,
            IoTGatewayManager ioTGatewayManager)
        {
            _alarmCenterClient = alarmCenterClient;
            _iotQueryService = iotQueryServiceClient;
            empty = new Empty();
            _connectPoolManager = connectPoolManager;
            _loggingService = loggingService;
            _ioTGatewayManager = ioTGatewayManager;
        }


        public void GetChangedRTYXItemData1()
        {
            _alarmCenterClient.GetChangedRTYXItemData1(empty);
        }

        public void GetTotalRTYXItemData()
        {
            _alarmCenterClient.GetTotalRTYXItemData(empty);
        }

        public void GetTotalRTYXItemData1()
        {
            _alarmCenterClient.GetTotalRTYXItemData1(empty);
        }
        public string GetYXAlarmComments(int iEqpNo, int iYXPNo)
        {
            var item = _ioTGatewayManager.GetSubGatewayInfo<IotService.IotServiceClient>(iEqpNo, iYXPNo, PointType);
            return item.service.GetYXAlarmComments(new IoTCenterHost.Proto.HaveHistoryCurveRequest { EquipNo = iEqpNo, YCPNo = iYXPNo }).Result;

        }

        public async System.Threading.Tasks.Task<bool> GetYXAlarmStateAsync(int iEquipNo, int iYxpNo)
        {
            var result = _connectPoolManager.EquipManager.GetYXAlarmState(iEquipNo, iYxpNo);
            if (result == null)
            {
                int gatewayId = _ioTGatewayManager.FindEquipGatewayId(iEquipNo);
                if (gatewayId != 0)
                {
                    var item = _ioTGatewayManager.GetSubGatewayInfo<IotService.IotServiceClient>(iEquipNo, iYxpNo, PointType);
                    var resultDefine = await item.service.GetYXAlarmStateAsync(new IoTCenterHost.Proto.GetYXAlarmStateRequest { IEquipNo = item.equipNo, IYxpNo = item.pointNo.Value });
                    result = resultDefine.Result;
                }
                else
                {
                    var resultDefine = await _alarmCenterClient.GetYXAlarmStateAsync(new IoTCenterHost.Proto.GetYXAlarmStateRequest { IEquipNo = iEquipNo, IYxpNo = iYxpNo });
                    result = resultDefine.Result;
                }

            }
            return result.Value;
        }

        public Dictionary<int, bool> GetYXAlarmStateDictFromEquip(int iEquipNo)
        {
            var result = _connectPoolManager.EquipManager.GetYXAlarmStateDictFromEquip(iEquipNo);
            if (result == null)
            {
                int gatewayId = _ioTGatewayManager.FindEquipGatewayId(iEquipNo);
                if (gatewayId != 0)
                {
                    var item = _ioTGatewayManager.GetSubGatewayInfo<IotService.IotServiceClient>(iEquipNo);
                    result = item.service.GetYXAlarmStateDictFromEquip(new IoTCenterHost.Proto.IntegerDefine { Result = item.equipNo }).Result.FromJson<Dictionary<int, bool>>();
                }
                else
                {
                    result = _alarmCenterClient.GetYXAlarmStateDictFromEquip(new IoTCenterHost.Proto.IntegerDefine { Result = iEquipNo }).Result.FromJson<Dictionary<int, bool>>();
                }

            }
            return result;
        }

        public string GetYXEvt01(int iEquipNo, int iYxpNo)
        {
            var item = _ioTGatewayManager.GetSubGatewayInfo<IotService.IotServiceClient>(iEquipNo, iYxpNo, PointType);
            return item.service.GetYXEvt01(new IoTCenterHost.Proto.GetYXEvt01Request { IEquipNo = iEquipNo, IYxpNo = iYxpNo }).Result;
        }

        public string GetYXEvt10(int iEquipNo, int iYxpNo)
        {
            var item = _ioTGatewayManager.GetSubGatewayInfo<IotService.IotServiceClient>(iEquipNo, iYxpNo, PointType);
            return item.service.GetYXEvt10(new IoTCenterHost.Proto.GetYXEvt01Request { IEquipNo = iEquipNo, IYxpNo = iYxpNo }).Result;
        }

        public string GetYXPListStr(int iEquipNo)
        {
            var item = _ioTGatewayManager.GetSubGatewayInfo<IotService.IotServiceClient>(iEquipNo);
            return item.service.GetYXPListStr(new IoTCenterHost.Proto.IntegerDefine { Result = iEquipNo }).Result;
        }

        public object GetYXValue(int iEquipNo, int iYxpNo)
        {
            var result = _connectPoolManager.EquipManager.GetYXValue(iEquipNo, iYxpNo);
            if (result == null)
            {
                int gatewayId = _ioTGatewayManager.FindEquipGatewayId(iEquipNo);
                if (gatewayId != 0)
                {
                    var item = _ioTGatewayManager.GetSubGatewayInfo<IotService.IotServiceClient>(iEquipNo, iYxpNo, PointType);
                    result = item.service.GetYXValue(new IoTCenterHost.Proto.GetYXValueRequest { IEquipNo = iEquipNo, IYxpNo = iYxpNo }).Result.FromJson<object>();
                }
                else
                {
                    result = _alarmCenterClient.GetYXValue(new IoTCenterHost.Proto.GetYXValueRequest { IEquipNo = iEquipNo, IYxpNo = iYxpNo }).Result.FromJson<object>();
                }
                _loggingService.Debug($"获取远程遥信实时值---{iEquipNo},{result.ToJson()}");
            }
            _loggingService.Debug($"设备{iEquipNo}{iYxpNo}获取遥信实时值为{result}");
            return result;
        }

        public Dictionary<int, string> GetYXValueDictFromEquip(int iEquipNo)
        {
            var result = _connectPoolManager.EquipManager.GetYXValueDictFromEquip(iEquipNo);
            if (result == null || !result.Any())
            {
                int gatewayId = _ioTGatewayManager.FindEquipGatewayId(iEquipNo);
                if (gatewayId != 0)
                {
                    var item = _ioTGatewayManager.GetSubGatewayInfo<IotService.IotServiceClient>(iEquipNo);
                    result = item.service.GetYXValueDictFromEquip(new IoTCenterHost.Proto.IntegerDefine { Result = item.equipNo }).Result.FromJson<Dictionary<int, string>>();
                }
                else
                {
                    result = _alarmCenterClient.GetYXValueDictFromEquip(new IoTCenterHost.Proto.IntegerDefine { Result = iEquipNo }).Result.FromJson<Dictionary<int, string>>();
                }

            }
            _loggingService.Debug($"设备{iEquipNo}获取遥信实时值为{result.ToJson()}");
            return result;
        }

        public bool HaveYXP(int EquipNo)
        {
            var result = _connectPoolManager.EquipManager.HaveYXP(EquipNo);
            if (result == null)
            {
                int gatewayId = _ioTGatewayManager.FindEquipGatewayId(EquipNo);
                if (gatewayId != 0)
                {
                    var item = _ioTGatewayManager.GetSubGatewayInfo<IotService.IotServiceClient>(EquipNo);
                    result = item.service.HaveYXP(new IoTCenterHost.Proto.IntegerDefine { Result = item.equipNo }).Result;
                }
                else
                {
                    result = _alarmCenterClient.HaveYXP(new IoTCenterHost.Proto.IntegerDefine { Result = EquipNo }).Result;
                }

            }
            return result.Value;
        }

        public void SetYxpNm(int EquipNo, int YcpNo, string Nm)
        {
            var item = _ioTGatewayManager.GetSubGatewayInfo<IotService.IotServiceClient>(EquipNo, YcpNo, PointType);
            item.service.SetYxpNm(new IoTCenterHost.Proto.SetYxpNmRequest { EquipNo = EquipNo, YcpNo = YcpNo, Nm = Nm });
        }

        public List<GrpcYxItem> GetTotalYXData(bool isDynamic)
        {
            var result = _connectPoolManager.EquipManager.EquipYxItems.SelectMany(outerPair => outerPair.Value.Values).ToList();
            if (result == null || !result.Any())
            {
                result = _iotQueryService.GetTotalYXData(new BoolDefine { Result = isDynamic }).Result.FromJson<List<GrpcYxItem>>();
            }
            return result;

        }
        public PaginationData GetYXPListByEquipNo(Pagination pagination)
        {
            return _alarmCenterClient.GetYXPListByEquipNo(new StringResult { Result = pagination.ToJson() }).Result.FromJson<PaginationData>();
        }

        public bool GetYXAlarmState(int iEquipNo, int iYxpNo)
        {
            var result = _connectPoolManager.EquipManager.GetYXAlarmState(iEquipNo, iYxpNo);
            if (!result.HasValue)
            {
                int gatewayId = _ioTGatewayManager.FindEquipGatewayId(iEquipNo);
                if (gatewayId != 0)
                {
                    var item = _ioTGatewayManager.GetSubGatewayInfo<IotService.IotServiceClient>(iEquipNo, iYxpNo, PointType);
                    result = item.service.GetYXAlarmState(new GetYXAlarmStateRequest { IEquipNo = item.equipNo, IYxpNo = item.equipNo }).Result;
                }
                else
                {
                    result = _alarmCenterClient.GetYXAlarmState(new GetYXAlarmStateRequest { IEquipNo = iEquipNo, IYxpNo = iYxpNo }).Result;
                }

            }
            return result.Value;
        }
    }
}
