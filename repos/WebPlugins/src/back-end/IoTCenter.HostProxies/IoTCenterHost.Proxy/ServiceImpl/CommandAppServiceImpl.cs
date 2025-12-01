// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Grpc.Core;
using IoTCenter.Utilities;
using IoTCenterHost.Core.Abstraction;
using IoTCenterHost.Core.Abstraction.AppServices;
using IoTCenterHost.Core.Abstraction.BaseModels;
using IoTCenterHost.Proto;
using IoTCenterHost.Proxy.Gateway;
using System;
using System.Threading.Tasks;
using static IoTCenterHost.Proto.iotsubgatewayContract;

namespace IoTCenterHost.Proxy.ServiceImpl
{
    public class CommandAppServiceImpl : ICommandAppService
    {
        private readonly IotService.IotServiceClient _alarmCenterClient;
        private readonly IoTGatewayManager _ioTGatewayManager;
        private const string PointType = "set";
        public CommandAppServiceImpl(IotService.IotServiceClient alarmCenterClient,
            IoTGatewayManager ioTGatewayManager)
        {
            _alarmCenterClient = alarmCenterClient;
            _ioTGatewayManager = ioTGatewayManager;
        }

        public string DoEquipSetItem(int EquipNo, int SetNo, string strValue, string strUser, bool bShowDlg, string Instance_GUID, string requestId = "")
        {
            int gatewayId = _ioTGatewayManager.FindEquipGatewayId(EquipNo);
            if (gatewayId != 0)
            {
                var item = _ioTGatewayManager.GetSubGatewayInfo<IotService.IotServiceClient>(EquipNo, SetNo, PointType);
                var result = item.service.DoEquipSetItem(new IoTCenterHost.Proto.SetParm1_1Request
                {
                    EquipNo = item.equipNo,
                    SetNo = item.pointNo ?? 0,
                    StrValue = strValue,
                    StrUser = strUser,
                    BShowDlg = bShowDlg,
                    RequestId = requestId
                });
                return result.Result;
            }
            else
            {
                var result = _alarmCenterClient.DoEquipSetItem(new IoTCenterHost.Proto.SetParm1_1Request
                {
                    EquipNo = EquipNo,
                    SetNo = SetNo,
                    StrValue = strValue,
                    StrUser = strUser,
                    BShowDlg = bShowDlg,
                    RequestId = requestId
                });

                return result.Result;
            }
        }

        public void DoSetParmFromString(string csParmStr)
        {
            _alarmCenterClient.DoSetParmFromString(new IoTCenterHost.Proto.BaseResult { Result = csParmStr });
        }

        public string GetSetListStr(int iEquipNo)
        {
            var item = _ioTGatewayManager.GetSubGatewayInfo<IotService.IotServiceClient>(iEquipNo);

            return _alarmCenterClient.GetSetListStr(new IoTCenterHost.Proto.IntegerDefine { Result = item.equipNo }).Result;
        }

        public bool HaveSet(int EquipNo)
        {
            var item = _ioTGatewayManager.GetSubGatewayInfo<IotService.IotServiceClient>(EquipNo);

            return item.service.HaveSet(new IoTCenterHost.Proto.IntegerDefine { Result = item.equipNo }).Result;

        }

        public void SetParm(int EquipNo, string strCMD1, string strCMD2, string strCMD3, string strUser)
        {
            var item = _ioTGatewayManager.GetSubGatewayInfo<IotService.IotServiceClient>(EquipNo);

            item.service.SetParmAsync(new IoTCenterHost.Proto.SetParmRequest { EquipNo = item.equipNo, StrCMD1 = strCMD1, StrCMD2 = strCMD2, StrCMD3 = strCMD3, StrUser = strUser });
        }

        public void SetParm(int iEquipNo, string strCMD1, string strCMD2, string strCMD3, string strUser, string Instance_GUID)
        {
            int gatewayId = _ioTGatewayManager.FindEquipGatewayId(iEquipNo);
            if (gatewayId != 0)
            {
                var item = _ioTGatewayManager.GetSubGatewayInfo<IotService.IotServiceClient>(iEquipNo, null, PointType);
                item.service.SetParmAsync(new IoTCenterHost.Proto.SetParmRequest
                {
                    EquipNo = item.equipNo,
                    StrCMD1 = strCMD1,
                    StrCMD2 = strCMD2,
                    StrCMD3 = strCMD3,
                    StrUser = strUser,
                    RequestId = Instance_GUID
                });
            }
            else
            {
                _alarmCenterClient.SetParmAsync(new IoTCenterHost.Proto.SetParmRequest
                {
                    EquipNo = iEquipNo,
                    StrCMD1 = strCMD1,
                    StrCMD2 = strCMD2,
                    StrCMD3 = strCMD3,
                    StrUser = strUser,
                    RequestId = Instance_GUID
                });
            }
        }

        public void SetParm1(int EquipNo, int SetNo, string strUser)
        {
            int gatewayId = _ioTGatewayManager.FindEquipGatewayId(EquipNo);
            if (gatewayId != 0)
            {
                var item = _ioTGatewayManager.GetSubGatewayInfo<IotService.IotServiceClient>(EquipNo, SetNo, PointType);
                item.service.SetParm1Async(new IoTCenterHost.Proto.SetParm1Request
                {
                    EquipNo = item.equipNo,
                    SetNo = item.pointNo ?? 0,
                    StrUser = strUser
                });
            }
            else
            {
                _alarmCenterClient.SetParm1_1Async(new IoTCenterHost.Proto.SetParm1_1Request
                {
                    EquipNo = EquipNo,
                    SetNo = SetNo,
                    StrUser = strUser
                });
            }
        }

        public void SetParm1_1(int EquipNo, int SetNo, string strValue, string strUser, bool bShowDlg, string requestId = "")
        {
            int gatewayId = _ioTGatewayManager.FindEquipGatewayId(EquipNo);
            if (gatewayId != 0)
            {
                var item = _ioTGatewayManager.GetSubGatewayInfo<IotService.IotServiceClient>(EquipNo, SetNo, PointType);
                item.service.SetParm1_1Async(new IoTCenterHost.Proto.SetParm1_1Request
                {
                    EquipNo = item.equipNo,
                    SetNo = item.pointNo ?? 0,
                    StrValue = strValue,
                    StrUser = strUser,
                    BShowDlg = bShowDlg,
                    RequestId = requestId
                });
            }
            else
            {
                _alarmCenterClient.SetParm1_1Async(new IoTCenterHost.Proto.SetParm1_1Request
                {
                    EquipNo = EquipNo,
                    SetNo = SetNo,
                    StrValue = strValue,
                    StrUser = strUser,
                    BShowDlg = bShowDlg,
                    RequestId = requestId
                });
            }
        }

        public void SetParm2(int EquipNo, string strCMD1, string strCMD2, string strCMD3, string strType, string strUser)
        {
            int gatewayId = _ioTGatewayManager.FindEquipGatewayId(EquipNo);
            if (gatewayId != 0)
            {
                var item = _ioTGatewayManager.GetSubGatewayInfo<IotService.IotServiceClient>(EquipNo, null, PointType);
                item.service.SetParm2Async(new IoTCenterHost.Proto.SetParm2Request
                {
                    EquipNo = item.equipNo,
                    StrCMD1 = strCMD1,
                    StrCMD2 = strCMD2,
                    StrCMD3 = strCMD3,
                    StrType = strType,
                    StrUser = strUser
                });
            }
            else
            {
                _alarmCenterClient.SetParm2Async(new IoTCenterHost.Proto.SetParm2Request
                {
                    EquipNo = EquipNo,
                    StrCMD1 = strCMD1,
                    StrCMD2 = strCMD2,
                    StrCMD3 = strCMD3,
                    StrUser = strUser,
                    StrType = strType
                });
            }
        }

        public void SetParm2_1(int EquipNo, string strCMD1, string strCMD2, string strCMD3, string strType, string strUser, bool bShowDlg)
        {
            var item = _ioTGatewayManager.GetSubGatewayInfo<IotService.IotServiceClient>(EquipNo);

            item.service.SetParm2_1Async(new SetParm2_1Request { EquipNo = item.equipNo, StrCMD1 = strCMD1, StrCMD2 = strCMD2, StrCMD3 = strCMD3, StrType = strType, StrUser = strUser, BShowDlg = bShowDlg });
        }

        public async Task SetParmExAsync(int EquipNo, string strCMD1, string strCMD2, string strCMD3, string strUser, string requestId, Action<string> action)
        {
            var item = _ioTGatewayManager.GetSubGatewayInfo<IotService.IotServiceClient>(EquipNo);

            var stream = item.service.SetParmEx(new IoTCenterHost.Proto.SetParmRequest { EquipNo = item.equipNo, StrCMD1 = strCMD1, StrCMD2 = strCMD2, StrCMD3 = strCMD3, StrUser = strUser, RequestId = requestId });
            await foreach (var resp in stream.ResponseStream.ReadAllAsync())
            {
                action?.Invoke(resp.Result);
            }
        }

        public void SetParm_1(int EquipNo, string strCMD1, string strCMD2, string strCMD3, string strUser, bool bShowDlg)
        {
            var item = _ioTGatewayManager.GetSubGatewayInfo<IotService.IotServiceClient>(EquipNo);

            item.service.SetParm_1Async(new IoTCenterHost.Proto.SetParm_1Request { EquipNo = item.equipNo, StrCMD1 = strCMD1, StrCMD2 = strCMD2, StrCMD3 = strCMD3, StrUser = strUser, BShowDlg = bShowDlg });
        }
    }
}
