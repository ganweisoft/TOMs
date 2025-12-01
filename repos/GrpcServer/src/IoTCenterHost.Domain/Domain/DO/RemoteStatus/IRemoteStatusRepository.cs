//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenterHost.AppServices.Domain.PO;

namespace IoTCenterHost.AppServices.Domain.DO.RemoteStatus
{

    public interface IRemoteStatusRepository
    {
        object GetYXValue(int iEquipNo, int iYxpNo);
        Dictionary<int, string> GetYXValueDictFromEquip(int iEquipNo);
        bool GetYXAlarmState(int iEquipNo, int iYxpNo);
        string GetYXEvt01(int iEquipNo, int iYxpNo);
        string GetYXEvt10(int iEquipNo, int iYxpNo);
        Dictionary<int, bool> GetYXAlarmStateDictFromEquip(int iEquipNo);
        void MResetYxNo(int EquipNo, int YcYxNo);
        bool SetWuBao(int eqpno, int ycyxno);
        bool HaveYXP(int EquipNo);
        string GetYXPListStr(int iEquipNo);
        string GetYXAlarmComments(int iEqpNo, int iYXPNo);
        void SetYxpNmAsync(int EquipNo, int YcpNo, string Nm);

        IEnumerable<YXP> GetYxpTableRows(int iEquipNo, out int total);

    }
}
