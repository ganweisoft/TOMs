// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Utilities;
using IoTCenterHost.Core.Abstraction.AppServices;
using IoTCenterHost.Core.Abstraction.BaseModels;
using IoTCenterHost.Proto;
using IoTCenterHost.Proxy.Gateway;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IoTCenterHost.Proxy.ServiceImpl
{
    public class CurveAppServiceImpl : ICurveClientAppService
    {
        private readonly IotService.IotServiceClient _alarmCenterClient;
        private readonly IoTGatewayManager _ioTGatewayManager;
        public CurveAppServiceImpl(IotService.IotServiceClient alarmCenterClient, IoTGatewayManager ioTGatewayManager)
        {
            _alarmCenterClient = alarmCenterClient;
            _ioTGatewayManager = ioTGatewayManager;
        }
        public async Task<List<GrpcMyCurveData>> GetChangedDataFromCurveAsync(DateTime bgn, DateTime end, int stano, int eqpno, int ycyxno, string type)
        {
            var item = _ioTGatewayManager.GetSubGatewayInfo<IotService.IotServiceClient>(eqpno, ycyxno, type);
            var result = await item.service.GetChangedDataFromCurveAsyncAsync(new IoTCenterHost.Proto.GetChangedDataFromCurveAsyncRequest { Bgn = bgn.ToString("yyyy-MM-dd HH:mm:ss fff"), End = end.ToString("yyyy-MM-dd HH:mm:ss fff"), Stano = stano, Eqpno = item.equipNo, Ycyxno = item.pointNo.Value, Type = type });
            return result.Result.FromJson<List<GrpcMyCurveData>>();


        }

        public byte[] GetCurveData(DateTime d, int eqpno, int ycno)
        {
            return _alarmCenterClient.GetCurveData(new IoTCenterHost.Proto.GetCurveDataRequest { D = d.ToString("yyyy-MM-dd HH:mm:ss fff"), Eqpno = eqpno, Ycno = ycno }).Result.ConvertByteStrToByte();
        }

        public byte[] GetCurveData1(DateTime d, int eqpno, int ycyxno, string type)
        {
            return _alarmCenterClient.GetCurveData1(new IoTCenterHost.Proto.GetCurveData1Request { D = d.ToString("yyyy-MM-dd HH:mm:ss fff"), Eqpno = eqpno, Ycyxno = ycyxno, Type = type }).Result.ConvertByteStrToByte();
        }

        public Task<List<GrpcMyCurveData>> GetDataFromCurve(List<DateTime> DTList, int stano, int eqpno, int ycyxno, string type)
        {
            return Task.FromResult(_alarmCenterClient.GetDataFromCurve(new IoTCenterHost.Proto.GetDataFromCurveRequest { DTList = DTList.ToJson(), Eqpno = eqpno, Stano = stano, Ycyxno = ycyxno, Type = type }).Result.FromJson<List<GrpcMyCurveData>>());
        }


        public bool HaveHistoryCurve(int EquipNo, int YCPNo)
        {
            return _alarmCenterClient.HaveHistoryCurve(new IoTCenterHost.Proto.HaveHistoryCurveRequest { EquipNo = EquipNo, YCPNo = YCPNo }).Result;
        }

        public void SetHistoryStorePeriod(int period)
        {
            _alarmCenterClient.SetHistoryStorePeriod(new IoTCenterHost.Proto.IntegerDefine { Result = period });
        }

        public int GetHistoryStorePeriod()
        {
            var result = _alarmCenterClient.GetHistoryStorePeriod(new Google.Protobuf.WellKnownTypes.Empty()).Result;
            int.TryParse(result, out int periodTime);
            return periodTime;
        }

        public async Task<List<GrpcMyCurveData>> GetCurveDataAsync(DateTime bgn, DateTime end, int stano, int eqpno, int ycyxno, string type)
        {
            var item = _ioTGatewayManager.GetSubGatewayInfo<IotService.IotServiceClient>(eqpno, ycyxno, type);
            var result = await item.service.GetChangedDataFromCurveAsyncAsync(new GetChangedDataFromCurveAsyncRequest
            {
                Bgn = bgn.ToString("yyyy-MM-dd HH:mm:ss fff"),
                End = end.ToString("yyyy-MM-dd HH:mm:ss fff"),
                Stano = stano,
                Eqpno = item.equipNo,
                Ycyxno = item.pointNo.Value,
                Type = type
            });
            return result.Result.FromJson<List<GrpcMyCurveData>>();
        }
    }
}
