﻿// Copyright (c) 2004-2025  Beijing TOMs Software Technology Co., Ltd
using GWDataCenter.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
namespace GWDataCenter
{
    public class CyclePlanItem : IPlanItem
    {
        public bool IsExcute = false;
        public int TableID;
        public string TableName;
        public List<ISetItem> SetItemList = new List<ISetItem>();
        DateTime tBeginTime, tEndTime, tZhidingTime;
        bool bZhenDianDo, bZhidingDo, bCycleMustFinish;
        int iCurrentCycleNum, iMaxCycleNum;
        Thread RefreshThread;
        const int SleepUnit = 10;
        bool bThreadStart = false;
        object bstopplanitem = true;
        public bool bStopPlanItem
        {
            get
            {
                lock (bstopplanitem)
                {
                    return (bool)bstopplanitem;
                }
            }
            set
            {
                lock (bstopplanitem)
                {
                    bstopplanitem = value;
                    if (value == false)
                        Start();
                }
            }
        }
        object bworkstate = false;
        public bool bWorkState
        {
            get
            {
                lock (bworkstate)
                {
                    return (bool)bworkstate;
                }
            }
            set
            {
                lock (bworkstate)
                {
                    bworkstate = value;
                }
            }
        }
        object btemporarilybreak = false;
        public bool m_bTemporarilyBreak
        {
            get
            {
                lock (btemporarilybreak)
                {
                    return (bool)btemporarilybreak;
                }
            }
            set
            {
                lock (btemporarilybreak)
                {
                    btemporarilybreak = value;
                    foreach (ISetItem item in SetItemList)
                    {
                        item.m_bTemporarilyBreak = value;
                    }
                }
            }
        }
        public CyclePlanItem(int ID)
        {
            try
            {
                TableID = ID;
                List<GWProcCycleTableRow> Rows;
                lock (GWDbProvider.lockstate)
                {
                    using (var db = StationItem.MyGWDbProvider.serviceProvider.GetService<GWDataContext>())
                    {
                        Rows = db.GWProcCycleTable.FromSqlRaw($"SELECT * FROM GWProcCycleTable where TableID={TableID}").AsNoTracking().ToList();
                    }
                }
                if (!Rows.Any())
                    return;
                List<GWProcCycleTListTableRow> Rows1;
                lock (GWDbProvider.lockstate)
                {
                    using (var db = StationItem.MyGWDbProvider.serviceProvider.GetService<GWDataContext>())
                    {
                        Rows1 = db.GWProcCycleTListTable.FromSqlRaw($"SELECT * FROM GWProcCycleTList where TableID={TableID}").AsNoTracking().ToList();
                    }
                }
                if (!Rows1.Any())
                    return;
                tBeginTime = Rows1.First().BeginTime;
                tEndTime = Rows1.First().EndTime;
                tZhidingTime = Rows1.First().ZhidingTime;
                bZhenDianDo = Rows1.First().ZhenDianDo;
                bZhidingDo = Rows1.First().ZhidingDo;
                bCycleMustFinish = Rows1.First().CycleMustFinish;
                iMaxCycleNum = Rows1.First().MaxCycleNum;
                SetItemList.Clear();
                foreach (GWProcCycleTableRow r in Rows)
                {
                    switch (r.Type.ToUpper().Trim())
                    {
                        case "E":
                            SetItemList.Add(new EquipSetItem(r) { StartValidTime = tBeginTime, EndValidTime = tEndTime, bCycleMustFinish = bCycleMustFinish });
                            break;
                        case "T":
                            SetItemList.Add(new SleepSetItem(r) { StartValidTime = tBeginTime, EndValidTime = tEndTime, bCycleMustFinish = bCycleMustFinish });
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                DataCenter.WriteLogFile(e.ToString());
            }
        }
        static public bool InValidTime(DateTime start, DateTime end)
        {
            DateTime now = DateTime.Now;
            if (now.TimeOfDay < start.TimeOfDay)
                return false;
            if (now.TimeOfDay > end.TimeOfDay)
                return false;
            return true;
        }
        void Start()
        {
            if (!bThreadStart)
            {
                StartRefreshThread();
                bThreadStart = true;
            }
        }
        public void Stop()
        {
        }
        public void StartRefreshThread()
        {
            ThreadStart entryPoint = new ThreadStart(Refresh);
            RefreshThread = new Thread(entryPoint);
            RefreshThread.IsBackground = true;
            RefreshThread.Start();
        }
        public void Refresh()
        {
            while (true)
            {
                if (m_bTemporarilyBreak)
                {
                    Thread.Sleep(SleepUnit);
                    continue;
                }
                Thread.Sleep(SleepUnit);
                if (bStopPlanItem)
                {
                    iCurrentCycleNum = 0;
                    IsExcute = false;
                    Thread.Sleep(SleepUnit);
                    continue;
                }
                if ((System.DateTime.Now.TimeOfDay >= tBeginTime.TimeOfDay && System.DateTime.Now.TimeOfDay <= tEndTime.TimeOfDay)
                     || (tBeginTime.TimeOfDay.TotalSeconds == 0 && (int)tEndTime.TimeOfDay.TotalSeconds == 86399))
                {
                    if (bZhenDianDo)
                    {
                        if (!IsExcute && bZhenDianDo && DateTime.Now.Minute == 0 && DateTime.Now.Second == 0)
                        {
                            bZhenDianDo = false;
                            if (!DoOneCycle())
                                continue;
                            continue;
                        }
                    }
                    if (bZhidingDo)
                    {
                        if (!IsExcute && bZhidingDo && (int)DateTime.Now.TimeOfDay.TotalSeconds == tZhidingTime.TimeOfDay.TotalSeconds)
                        {
                            bZhidingDo = false;
                            if (!DoOneCycle())
                                continue;
                            continue;
                        }
                    }
                    if ((!bZhidingDo) && (!bZhenDianDo))
                    {
                        if (!DoOneCycle())
                            continue;
                        continue;
                    }
                }
                else
                {
                    iCurrentCycleNum = 0;
                    IsExcute = false;
                    continue;
                }
            }
        }
        bool DoOneCycle()
        {
            if (iMaxCycleNum > 0 && iCurrentCycleNum >= iMaxCycleNum)
            {
                IsExcute = false;
                return false;
            }
            bool bAbortCycle = false;
            foreach (ISetItem item in SetItemList)
            {
                if (m_bTemporarilyBreak)
                    return false;
                if (item.DoSetItem())
                    IsExcute = true;
                else
                {
                    bAbortCycle = true;
                    if (!bCycleMustFinish)
                        return false;
                }
            }
            iCurrentCycleNum += 1;
            return !bAbortCycle;
        }
    }
}
