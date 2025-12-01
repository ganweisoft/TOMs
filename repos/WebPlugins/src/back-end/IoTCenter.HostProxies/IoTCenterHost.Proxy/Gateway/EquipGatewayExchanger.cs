// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace IoTCenterHost.Proxy.Gateway
{
    internal class EquipGatewayExchanger
    {
        public ConcurrentDictionary<int, ConcurrentBag<GatewayEquip>> Collection { get; private set; } = new ConcurrentDictionary<int, ConcurrentBag<GatewayEquip>>();

        public ConcurrentDictionary<int, ConcurrentBag<GatewayEquip>> Create(IEnumerable<GatewayEquip> equiplist)
        {
            var equipGroup = equiplist
                .GroupBy(e => e.GatewayId);

            var equipDictionary = equipGroup.ToDictionary(
                g => g.Key,
                g => new ConcurrentBag<GatewayEquip>(g));

            return Collection = new ConcurrentDictionary<int, ConcurrentBag<GatewayEquip>>(equipDictionary);
        }
        public void AddGatewayEquip(int gatewayId, GatewayEquip equip)
        {
            var list = Collection.TryGetValue(gatewayId, out ConcurrentBag<GatewayEquip> equipList);
            if (equipList == null)
            {
                equipList = new ConcurrentBag<GatewayEquip>();
            }

            if (equipList.Any(e => e.EquipNo == equip.EquipNo && e.GatewayId == equip.GatewayId && e.SubgatewayEquipId == equip.SubgatewayEquipId))
            {
                return;
            }

            equipList.Add(equip);
        }
        public GatewayEquip RemoveEquip(int gatewayId, int equipNo)
        {
            return FindWithRemove(gatewayId, equipNo, null, true);
        }
        public GatewayEquip FindEquip(int? gatewayId, int equipNo)
        {
            if (gatewayId.HasValue)
                return FindWithRemove(gatewayId.Value, equipNo, null);
            else
                return Collection?.SelectMany(u => u.Value).FirstOrDefault(i => i.EquipNo == equipNo);
        }

        public void RemoveGateway(int gatewayId)
        {
            Collection.Remove(gatewayId, out _);
        }

        public GatewayEquip RemoveGatewayEquip(int gatewayId, int subgatewayEquipNo)
        {
            return FindWithRemove(gatewayId, null, subgatewayEquipNo, true);
        }
        public GatewayEquip FindGatewayEquip(int gatewayId, int? subgatewayEquipNo)
        {
            return FindWithRemove(gatewayId, null, subgatewayEquipNo);
        }

        public GatewayEquip[] FindGatewayEquipWithGatewayId(int gatewayId, int[] subgatewayEquipNos)
        {
            if (Collection == null)
                return null;
            var list = Collection.TryGetValue(gatewayId, out ConcurrentBag<GatewayEquip> equipList);
            return equipList.Where(u => subgatewayEquipNos.Contains(u.EquipNo)).ToArray();
        }

        private GatewayEquip FindWithRemove(int gatewayId, int? equipNo, int? subgatewayEquipNo, bool isRemove = false)
        {
            if (Collection == null)
                return null;

            var list = Collection.TryGetValue(gatewayId, out ConcurrentBag<GatewayEquip> equipList);
            if (!list || equipList == null)
            {
                return null;
            }

            var equip = equipList.FirstOrDefault(u =>
                equipNo.HasValue ? u.EquipNo == equipNo : u.SubgatewayEquipId == subgatewayEquipNo);
            if (isRemove)
            {
                equipList.TryTake(out equip);
            }

            return equip;
        }
    }

    public class GatewayEquip
    {
        public GatewayEquip(int equipNo, int gatewayId, int subgatewayEquipId, Dictionary<int, int> ycItems, Dictionary<int, int> yxItems, Dictionary<int, int> setItems)
        {
            this.EquipNo = equipNo;
            this.GatewayId = gatewayId;
            this.SubgatewayEquipId = subgatewayEquipId;
            this.YcItems = ycItems;
            this.YxItems = yxItems;
            this.SetItems = setItems;
        }

        public int EquipNo { get; private set; }
        public int GatewayId { get; private set; }
        public int SubgatewayEquipId { get; private set; }
        public Dictionary<int, int> YcItems { get; private set; }
        public Dictionary<int, int> YxItems { get; private set; }
        public Dictionary<int, int> SetItems { get; private set; }

        public override string ToString()
        {
            return $"网关{GatewayId},设备{EquipNo},子设备号{SubgatewayEquipId}";
        }

    }
}
