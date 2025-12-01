//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using GWDataCenter;
using GWDataCenter.Database;
using IoTCenterHost.AppServices.Domain.PO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenGWDataCenter.Model;

namespace IoTCenterHost.AppServices.Domain.DO.RemoteStatus
{
    public class RemoteStatusRepository : IRemoteStatusRepository
    {
        private readonly GanweiDbContext CurrentDbContext;
        private IServiceScopeFactory serviceScopeFactory;
        private ILogger<RemoteStatusRepository> _logger;
        public RemoteStatusRepository(GanweiDbContext ganweiDbContext, ILogger<RemoteStatusRepository> logger)
        {
            CurrentDbContext = ganweiDbContext;
            _logger = logger;
        }

        public object GetYXValue(int iEquipNo, int iYxpNo)
        {
            var equip = StationItem.GetEquipItemFromEquipNo(iEquipNo);
            if (equip != null)
                return equip.GetYXItem(iYxpNo).YXValue;
            return "";
        }
        public Dictionary<int, string> GetYXValueDictFromEquip(int iEquipNo)
        {
            Dictionary<int, string> dict = new Dictionary<int, string>();
            var equip = StationItem.GetEquipItemFromEquipNo(iEquipNo);
            if (equip != null)
                foreach (KeyValuePair<int, YXItem> pair in equip.YXItemDict)
                {
                    int iYXNo = pair.Key;
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(pair.Value.YXState))
                            dict.Add(iYXNo, pair.Value.YXState);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"获取遥信失败,{iEquipNo},{ex}");
                    }
                }
            return dict;
        }
        public bool GetYXAlarmState(int iEquipNo, int iYxpNo)
        {
            var equip = StationItem.GetEquipItemFromEquipNo(iEquipNo);
            if (equip != null)
                return StationItem.GetEquipItemFromEquipNo(iEquipNo).GetYXItem(iYxpNo).IsAlarm;
            return false;
        }
        public string GetYXEvt01(int iEquipNo, int iYxpNo)
        {
            return StationItem.GetEquipItemFromEquipNo(iEquipNo).GetYXItem(iYxpNo).YXState;
        }
        public string GetYXEvt10(int iEquipNo, int iYxpNo)
        {
            return StationItem.GetEquipItemFromEquipNo(iEquipNo).GetYXItem(iYxpNo).YXState;
        }
        public Dictionary<int, bool> GetYXAlarmStateDictFromEquip(int iEquipNo)
        {
            Dictionary<int, bool> dict = new Dictionary<int, bool>();
            var equip = StationItem.GetEquipItemFromEquipNo(iEquipNo);
            if (equip != null)
                foreach (KeyValuePair<int, YXItem> pair in equip.YXItemDict)
                {
                    int iYXNo = pair.Key;
                    dict.Add(iYXNo, pair.Value.IsAlarm);
                }
            return dict;
        }
        public bool HaveYXP(int EquipNo)
        {
            var equip = StationItem.GetEquipItemFromEquipNo(EquipNo);
            if (equip != null)
                return StationItem.GetEquipItemFromEquipNo(EquipNo).YXItemDict.Count > 0;
            return false;
        }
        public string GetYXPListStr(int iEquipNo)
        {
            var parmTableRows = GetYxpTableRows(iEquipNo, out int total);
            if (parmTableRows.Count() > 0)
            {
                var arr = parmTableRows.ToList().Select(u => "[{" + u.yx_nm + "}][{" + u.yx_nm + "}];");
                return string.Join(';', arr).TrimEnd(';');
            }
            else
                return string.Empty;
        }
        public string GetYXAlarmComments(int iEqpNo, int iYXPNo)
        {
            return StationItem.GetEquipItemFromEquipNo(iEqpNo).YXItemDict[iYXPNo].Comments;
        }
        public void SetYxpNmAsync(int EquipNo, int YxpNo, string Nm)//设置状态量测点名称
        {
            if (DataCenter.EquipItemDict.ContainsKey(EquipNo))
            {
                if (DataCenter.EquipItemDict[EquipNo].YXItemDict.ContainsKey(YxpNo))
                {
                    DataCenter.EquipItemDict[EquipNo].YXItemDict[YxpNo].Yx_nm = Nm;
                }
            }
            var remoteValueEntity = CurrentDbContext.YXP.FirstOrDefault(u => u.equip_no == EquipNo && u.yx_no == YxpNo);
            remoteValueEntity.yx_nm = Nm;
            CurrentDbContext.Update(remoteValueEntity);
        }
        public void MResetYxNo(int EquipNo, int YcYxNo)
        {
            DataCenter.GetEquipItem(EquipNo).YXItemDict[YcYxNo].ManualReset = false;
        }
        public bool SetWuBao(int eqpno, int ycyxno)
        {
            DataCenter.GetEquipItem(eqpno).YXItemDict[ycyxno].SetNoAlarm();
            DataCenter.GetEquipItem(eqpno).YXItemDict[ycyxno].IsWuBao = true;
            return true;
        }

        public IEnumerable<YXP> GetYxpTableRows(int iEquipNo, out int total)
        {
            var tables = CurrentDbContext.YXP;
            var result = tables.Where(u => u.equip_no == iEquipNo);
            total = result.Count();
            return result;
        }
    }
}
