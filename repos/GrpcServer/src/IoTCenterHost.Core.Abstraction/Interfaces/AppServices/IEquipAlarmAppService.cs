//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
namespace IoTCenterHost.Core.Abstraction.AppServices
{
    public interface IEquipAlarmAppService
    {
        bool SetWuBao(int eqpno, string type, int ycyxno);

        bool SetNoAlarm(int eqpno, string type, int ycyxno);
        bool Confirm2NormalState(int iEqpNo, string sYcYxType, int iYcYxNo);
    }
}
