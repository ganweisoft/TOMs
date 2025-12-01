//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using GWDataCenter;
using IoTCenterHost.AppServices.Domain.PO;

namespace IoTCenterHost.AppServices.Domain.DO.RemoteValue
{
    public interface IRemoteValueRepository
    {
        object GetYCValue(int iEquipNo, int iYcpNo);
        Dictionary<int, object> GetYCValueDictFromEquip(int iEquipNo);
        bool GetYCAlarmState(int iEquipNo, int iYcpNo);
        void MResetYcNo(int EquipNo, int YcYxNo);

        YCItem GetYCItem(int iEquipNo, int iYcpNo);

        bool SetWuBao(int eqpno, int ycyxno);
        string GetYCAlarmComments(int iEqpNo, int iYCPNo);
        Dictionary<int, bool> GetYCAlarmStateDictFromEquip(int iEquipNo);

        string GetYCPListStr(int iEquipNo);
        bool HaveYCP(int EquipNo);
        void SetYcpNm(int EquipNo, int YcpNo, string Nm);
        IEnumerable<YCP> GetYcpTableRows(int iEquipNo, out int total);
    }
}
