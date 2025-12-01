//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenterHost.Core.Abstraction.BaseModels;
using IoTCenterHost.Core.Abstraction.IotModels;
using System.Collections.Generic;

namespace IoTCenterHost.Core.Abstraction.AppServices
{
    public interface IEquipBaseAppService
    {
        int[] GetAddRTEquipItemData();

        void SetEquipDebug(int iEquipNo, bool bFlag);
        bool GetEquipDebugState(int iEquipNo);
        string GetEquipListStr();
        void GetTotalRTEquipItemData1();
        void GetChangedRTEquipItemData1();
        int[] GetEditRTEquipItemData();

        void ResetEquips();
        void ResetEquipmentLinkage();
        void SetEquipNm(int EquipNo, string Nm);


        void GetChangedRTEquipItemData();
        void GetTotalRTYCItemData1();
        void GetTotalRTYXItemData1();
        void GetChangedRTYCItemData1();
        void GetChangedRTYXItemData1();
        int[] GetDelRTEquipItemData();

        void ResetEquips(List<int> list);

        PaginationData GetEquipDict(Pagination pagination);
    }

    public interface IEquipBaseClientAppService : IEquipBaseAppService
    {
        IEnumerable<GrpcEquipItem> GetTotalEquipData(bool isDynamic);

        Dictionary<int, GrpcEquipState> GetEquipStateDict(IEnumerable<int> equipList);

        GrpcEquipState GetEquipStateFromEquipNo(int iEquipNo);

        void AddChangedEquipList(IEnumerable<Core.Abstraction.BaseModels.GrpcChangedEquip> changedEquips);
        void AddChangedEquip(Core.Abstraction.BaseModels.GrpcChangedEquip EqpList);

        Dictionary<int, GrpcEquipState> GetEquipStateDict();

    }
}
