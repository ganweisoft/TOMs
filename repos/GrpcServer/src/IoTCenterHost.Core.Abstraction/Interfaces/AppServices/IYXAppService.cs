//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenterHost.Core.Abstraction.BaseModels;
using IoTCenterHost.Core.Abstraction.IotModels;
using System.Collections.Generic;

namespace IoTCenterHost.Core.Abstraction.AppServices
{
    public interface IYXAppService
    {
        object GetYXValue(int iEquipNo, int iYxpNo);
        Dictionary<int, string> GetYXValueDictFromEquip(int iEquipNo);
        bool GetYXAlarmState(int iEquipNo, int iYxpNo);
        string GetYXEvt01(int iEquipNo, int iYxpNo);
        string GetYXEvt10(int iEquipNo, int iYxpNo);
        Dictionary<int, bool> GetYXAlarmStateDictFromEquip(int iEquipNo);
        bool HaveYXP(int EquipNo);
        string GetYXPListStr(int iEquipNo);
        string GetYXAlarmComments(int iEqpNo, int iYXPNo);
        void GetTotalRTYXItemData1();
        void SetYxpNm(int EquipNo, int YcpNo, string Nm);
        void GetTotalRTYXItemData();
        PaginationData GetYXPListByEquipNo(Pagination pagination);
    }
    public interface IYXClientAppService : IYXAppService
    {
        List<GrpcYxItem> GetTotalYXData(bool isDynamic);
    }

}
