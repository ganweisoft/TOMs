//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using GWDataCenter;
using GWDataCenter.Database;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenGWDataCenter.Model;
using System.Diagnostics;
using System.Text;

namespace IoTCenterHost.AppServices.Domain.DO.Equip
{
    public class EquipRepository : IEquipRepository
    {
        private readonly object lockHelper = new object();
        private readonly ILogger<EquipRepository> _logger;
        public EquipRepository(ILogger<EquipRepository> logger)
        {
            _logger = logger;
        }

        public GWDataContext CurrentDbContext
        {
            get
            {
                lock (lockHelper)
                {
                    return StationItem.MyGWDbProvider.serviceProvider.GetService<GWDataContext>(); ;
                }
            }
        }
        public EquipItem GetEquip(int equipNo)
        {
            return GetEquipItemFromEquipNo(equipNo);
        }

        public Dictionary<int, GWDataCenter.EquipState> GetEquipStateDict()
        {
            try
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                lock (DataCenter.EquipItemDict)
                {
                    var result = new Dictionary<int, GWDataCenter.EquipState>(DataCenter.EquipItemDict.Select(u => new KeyValuePair<int, GWDataCenter.EquipState>(u.Key, u.Value.State)));
                    if (stopwatch.Elapsed.TotalMilliseconds > TimeSpan.FromSeconds(10).TotalMilliseconds)
                    {
                        _logger.LogError($"查询设备状态耗时:{stopwatch.Elapsed.TotalSeconds}");
                    }
                    return result;
                }
                stopwatch.Stop();
            }
            catch
            {
                return null;
            }
        }
        public GWDataCenter.EquipState GetEquipStateFromEquipNo(int iEquipNo)
        {
            var equip = DataCenter.EquipItemDict.ContainsKey(iEquipNo) ? DataCenter.EquipItemDict[iEquipNo] : null;
            return equip == null ? GWDataCenter.EquipState.Initial : equip.State;
        }

        public void SetEquipDebug(int iEquipNo, bool bFlag)
        {
            StationItem.GetEquipItemFromEquipNo(iEquipNo).IsDebug = bFlag;
        }

        public bool GetEquipDebugState(int iEquipNo)
        {
            return (bool)StationItem.GetEquipItemFromEquipNo(iEquipNo).IsDebug;
        }
        public EquipItem GetEquipItemFromEquipNo(int iEquipNo)
        {
            lock (StationItem.EquipCategoryDict)
            {
                foreach (KeyValuePair<string, object> keyValuePair in StationItem.EquipCategoryDict)
                {
                    var value = (SubEquipList)keyValuePair.Value;
                    if (value == null) continue;
                    lock (value.EquipList)
                        return value?.EquipList.Cast<EquipItem>().FirstOrDefault(equip => equip.iEquipno == iEquipNo);
                }
            }
            return (EquipItem)null;
        }

        public string GetEquipListStr(int iEquipNo)
        {
            StringBuilder str = new StringBuilder();
            var parmTableRows = StationItem.db_Eqp.Where(u => u.equip_no == iEquipNo);
            return string.Concat(parmTableRows.Select(u => "[{" + u.equip_no + "}][{" + u.equip_nm + "}];"), ";").TrimEnd(';');
        }
        public string GetEquipListStr()
        {
            StringBuilder str = new StringBuilder();
            var parmTableRows = StationItem.db_Eqp;
            if (parmTableRows == null || !parmTableRows.Any())
            {
                parmTableRows = CurrentDbContext.EquipTable.Where(u => u != null).ToList();
            }
            return string.Join(";", parmTableRows.Select(u => new string("[{" + u.equip_no + "}][{" + u.equip_nm + "}]"))).TrimEnd(';');
        }


        public void SetEquipNmAsync(int EquipNo, string Nm)//设置设备名称
        {
            lock (DataCenter.EquipItemDict)
            {
                if (DataCenter.EquipItemDict.ContainsKey(EquipNo))
                {
                    DataCenter.EquipItemDict[EquipNo].Equip_nm = Nm;
                }
            }
            var equipPo = CurrentDbContext.EquipTable.Find(EquipNo);
            equipPo.equip_nm = Nm;
            CurrentDbContext.EquipTable.Update(equipPo);
        }

        private object lockhelper = new object();
        public Dictionary<int, GWDataCenter.EquipState> GetEquipStateDict(IEnumerable<int> equipList)
        {
            return new Dictionary<int, GWDataCenter.EquipState>(DataCenter.EquipItemDict.Where(u => equipList.Any(i => i == u.Key)).Select(item => new KeyValuePair<int, GWDataCenter.EquipState>(item.Key, item.Value.State)));
        }
    }
}
