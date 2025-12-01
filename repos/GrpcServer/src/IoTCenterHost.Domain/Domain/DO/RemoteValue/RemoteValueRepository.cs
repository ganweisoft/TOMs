//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using GWDataCenter;
using GWDataCenter.Database;
using IoTCenterHost.AppServices.Domain.PO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Data;

namespace IoTCenterHost.AppServices.Domain.DO.RemoteValue
{
    public class RemoteValueRepository : IRemoteValueRepository
    {
        private readonly GanweiDbContext CurrentDbContext;
        private IServiceScopeFactory serviceScopeFactory;
        private ILogger<RemoteValueRepository> _logger;
        public RemoteValueRepository(GanweiDbContext ganweiDbContext, ILogger<RemoteValueRepository> logger)
        {
            CurrentDbContext = ganweiDbContext;
            _logger = logger;
        }

        public object GetYCValue(int iEquipNo, int iYcpNo)
        {
            YCItem ycItem = GetYCItem(iEquipNo, iYcpNo);
            if (ycItem != null)
                return ycItem.YCValue;
            else return 0;
        }
        public Dictionary<int, object> GetYCValueDictFromEquip(int iEquipNo)
        {
            Dictionary<int, object> dict = new Dictionary<int, object>();
            var equipInfo = StationItem.GetEquipItemFromEquipNo(iEquipNo);
            if (equipInfo != null)
                foreach (KeyValuePair<int, YCItem> pair in equipInfo.YCItemDict)
                {
                    int iYCNo = pair.Key;
                    dict.Add(iYCNo, pair.Value.YCValue);
                }
            return dict;
        }
        public bool GetYCAlarmState(int iEquipNo, int iYcpNo)
        {
            var ycItem = GetYCItem(iEquipNo, iYcpNo);
            return (ycItem != null) ? ycItem.IsAlarm : false;
        }
        public YCItem GetYCItem(int iEquipNo, int iYcpNo)
        {
            var equip = StationItem.GetEquipItemFromEquipNo(iEquipNo);
            if (equip != null)
                return equip.GetYCItem(iYcpNo);
            return null;
        }
        public Dictionary<int, bool> GetYCAlarmStateDictFromEquip(int iEquipNo)
        {
            Dictionary<int, bool> dict = new Dictionary<int, bool>();
            var equip = StationItem.GetEquipItemFromEquipNo(iEquipNo);
            if (equip != null)
                foreach (KeyValuePair<int, YCItem> pair in equip.YCItemDict)
                {
                    int iYCNo = pair.Key;
                    dict.Add(iYCNo, pair.Value.IsAlarm);
                }
            return dict;
        }
        public bool HaveYCP(int EquipNo)
        {
            return StationItem.GetEquipItemFromEquipNo(EquipNo).YCItemDict.Count > 0;
        }
        public string GetYCPListStr(int iEquipNo)
        {
            var parmTableRows = GetYcpTableRows(iEquipNo, out int total);
            if (parmTableRows.Count() > 0)
            {
                var arr = parmTableRows.ToList().Select(u => "[{" + u.yc_no + "}][{" + u.yc_nm + "}];");
                return string.Join(';', arr).TrimEnd(';');
            }
            else
                return string.Empty;

        }

        public IEnumerable<YCP> GetYcpTableRows(int iEquipNo, out int total)
        {
            var tables = CurrentDbContext.YCP;
            var result = tables.Where(u => u.equip_no == iEquipNo);
            total = result.Count();
            return result;
        }
        public string GetYCAlarmComments(int iEqpNo, int iYCPNo)
        {
            return StationItem.GetEquipItemFromEquipNo(iEqpNo).YCItemDict[iYCPNo].Comments;
        }
        public bool HaveHistoryCurve(int EquipNo, int YCPNo)
        {
            return StationItem.GetEquipItemFromEquipNo(EquipNo).GetYCItem(YCPNo).Curve_rcd;
        }
        public void SetYcpNm(int EquipNo, int YcpNo, string Nm)
        {
            if (DataCenter.EquipItemDict.ContainsKey(EquipNo))
            {
                if (DataCenter.EquipItemDict[EquipNo].YXItemDict.ContainsKey(YcpNo))
                {
                    DataCenter.EquipItemDict[EquipNo].YXItemDict[YcpNo].Yx_nm = Nm;
                }
            }
            var remoteValueEntity = CurrentDbContext.YCP.FirstOrDefault(u => u.equip_no == EquipNo && u.yc_no == YcpNo);
            remoteValueEntity.yc_nm = Nm;
            CurrentDbContext.Update(remoteValueEntity);
        }
        public void MResetYcNo(int EquipNo, int YcYxNo)
        {
            DataCenter.GetEquipItem(EquipNo).YCItemDict[YcYxNo].ManualReset = false;
        }
        public bool SetWuBao(int eqpno, int ycyxno)
        {
            DataCenter.GetEquipItem(eqpno).YCItemDict[ycyxno].SetNoAlarm();
            DataCenter.GetEquipItem(eqpno).YCItemDict[ycyxno].IsWuBao = true;
            return true;
        }
    }
}
