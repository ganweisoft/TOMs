//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenterHost.Core.Abstraction.BaseModels;
using IoTCenterHost.Core.Abstraction.IotModels;
using System;
using System.Collections.Generic;

namespace IoTCenterHost.Core.Abstraction.AppServices
{
    public interface IYCAppService
    {
        object GetYCValue(int iEquipNo, int iYcpNo);
        Dictionary<int, object> GetYCValueDictFromEquip(int iEquipNo);
        bool GetYCAlarmState(int iEquipNo, int iYcpNo);
        Dictionary<int, bool> GetYCAlarmStateDictFromEquip(int iEquipNo);
        bool HaveYCP(int EquipNo);
        string GetYCPListStr(int iEquipNo);
        string GetYCAlarmComments(int iEqpNo, int iYCPNo);
        [Obsolete]
        void GetTotalRTYCItemData1();
        [Obsolete]
        void GetChangedRTYCItemData1();
        void SetYcpNm(int EquipNo, int YcpNo, string Nm);
        [Obsolete]
        void GetChangedRTYCItemData();
        [Obsolete]
        void GetTotalRTYCItemData();
        PaginationData GetYCPListByEquipNo(Pagination pagination);

        PaginationData GetYCPDict(Pagination pagination);

    }

    public interface IYCClientAppService : IYCAppService
    {
        List<GrpcYcItem> GetTotalYCData(bool isDynamic);

    }
}
