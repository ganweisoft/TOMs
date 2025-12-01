//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using GWDataCenter;
using OpenGWDataCenter.Model;

namespace IoTCenterHost.AppServices.Domain.DO.Equip
{
    public interface IEquipRepository
    {
        EquipItem GetEquip(int equipNo);
        GWDataCenter.EquipState GetEquipStateFromEquipNo(int iEquipNo);
        Dictionary<int, GWDataCenter.EquipState> GetEquipStateDict();


        Dictionary<int, GWDataCenter.EquipState> GetEquipStateDict(IEnumerable<int> equipList);

        string GetEquipListStr(int iEquipNo);
        string GetEquipListStr();
        void SetEquipNmAsync(int EquipNo, string Nm);
    }
}
