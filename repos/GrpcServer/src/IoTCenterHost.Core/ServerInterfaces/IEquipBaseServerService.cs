//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using GWDataCenter;
using IoTCenterHost.Core.Abstraction.AppServices;
using OpenGWDataCenter.Model;
using System.Collections.Generic;

namespace IoTCenterHost.Core.ServerInterfaces
{
    public interface IEquipBaseServerService : IEquipBaseAppService
    {
        void GetTotalEquipData(bool isDynamic);
        IEnumerable<EquipItem> GetTotalEquipDataEx(bool isDynamic);

        void AddChangedEquip(ChangedEquip EqpList, bool addSingle = true);
        void AddChangedEquipList(IEnumerable<ChangedEquip> changedEquips);

        EquipState GetEquipStateFromEquipNo(int iEquipNo);


        Dictionary<int, EquipState> GetEquipStateDict(IEnumerable<int> equipList);

        Dictionary<int, EquipState> GetEquipStateDict();

    }
}
