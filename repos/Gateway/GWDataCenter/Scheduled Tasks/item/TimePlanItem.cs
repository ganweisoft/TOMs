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
    public class TimePlanItem : IPlanItem
    {
        DateTime dt_now, dt_old;
        public bool IsExcute = false;
        public int TableID;
        public string TableName;
        public List<EquipSetItem> EquipSetItemList = new List<EquipSetItem>();
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
                    foreach (ISetItem item in EquipSetItemList)
                    {
                        item.m_bTemporarilyBreak = value;
                    }
                }
            }
        }
        public TimePlanItem(int ID)
        {
            try
            {
                TableID = ID;
                List<GWProcTimeEqpTableRow> Rows;
                lock (GWDbProvider.lockstate)
                {
                    using (var db = StationItem.MyGWDbProvider.serviceProvider.GetService<GWDataContext>())
                    {
                        string sqlMsg = $"SELECT * FROM GWProcTimeEqpTable where TableID={ID}";
                        Rows = db.GWProcTimeEqpTable.FromSqlRaw(sqlMsg).AsNoTracking().ToList();
                    }
                }
                EquipSetItemList.Clear();
                if (Rows.Any())
                {
                    foreach (GWProcTimeEqpTableRow r in Rows)
                    {
                        EquipSetItemList.Add(new EquipSetItem(r));
                    }
                }
                List<GWProcTimeSysTableRow> Rows1;
                lock (GWDbProvider.lockstate)
                {
                    using (var db = StationItem.MyGWDbProvider.serviceProvider.GetService<GWDataContext>())
                    {
                        string sqlMsg = $"SELECT * FROM GWProcTimeSysTable where TableID={ID}";
                        Rows1 = db.GWProcTimeSysTable.FromSqlRaw(sqlMsg).AsNoTracking().ToList();
                    }
                }
            }
            catch (Exception e)
            {
                DataCenter.WriteLogFile(e.ToString());
            }
        }
        static public bool InSpanTime(DateTime start, DateTime span)
        {
            DateTime now = DateTime.Now;
            if (now.TimeOfDay < start.TimeOfDay)
                return false;
            if (span.TimeOfDay >= (now.TimeOfDay - start.TimeOfDay))
                return true;
            return false;
        }
        void Start()
        {
            if (!bThreadStart)
            {
                StartRefreshThread();
                bThreadStart = true;
            }
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
                dt_now = DateTime.Now;
                if (dt_now.Hour < dt_old.Hour)
                {
                    foreach (EquipSetItem item in EquipSetItemList)
                    {
                        item.IsExcute = false;
                    }
                }
                Thread.Sleep(SleepUnit);
                dt_old = dt_now;
                if (bStopPlanItem)
                {
                    IsExcute = false;
                    continue;
                }
                foreach (EquipSetItem item in EquipSetItemList)
                {
                    if (m_bTemporarilyBreak)
                    {
                        break;
                    }
                    if (!item.IsExcute)
                        item.DoSetItem();
                }
            }
        }
    }
}
