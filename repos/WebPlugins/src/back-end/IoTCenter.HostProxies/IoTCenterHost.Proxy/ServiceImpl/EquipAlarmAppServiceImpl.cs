// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenterHost.Core.Abstraction.AppServices;
using IoTCenterHost.Proto;

namespace IoTCenterHost.Proxy.ServiceImpl
{
    public class EquipAlarmAppServiceImpl : IEquipAlarmAppService
    {
        private readonly IotService.IotServiceClient _alarmCenterClient;
        public EquipAlarmAppServiceImpl(IotService.IotServiceClient alarmCenterClient)
        {
            _alarmCenterClient = alarmCenterClient;
        }
        public bool Confirm2NormalState(int iEqpNo, string sYcYxType, int iYcYxNo)
        {
            return _alarmCenterClient.Confirm2NormalState(new IoTCenterHost.Proto.Confirm2NormalStateRequest { IEqpNo = iEqpNo, SYcYxType = sYcYxType, IYcYxNo = iYcYxNo }).Result;

        }

        public bool SetNoAlarm(int eqpno, string type, int ycyxno)
        {
            return _alarmCenterClient.SetNoAlarm(new IoTCenterHost.Proto.SetNoAlarmRequest { Eqpno = eqpno, Type = type, Ycyxno = ycyxno }).Result;

        }

        public bool SetWuBao(int eqpno, string type, int ycyxno)
        {
            return _alarmCenterClient.SetWuBao(new IoTCenterHost.Proto.SetWuBaoRequest { Eqpno = eqpno, Type = type, Ycyxno = ycyxno }).Result;

        }
    }
}
