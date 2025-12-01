﻿// Copyright (c) 2004-2025  Beijing TOMs Software Technology Co., Ltd
using GWDataCenter.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace GWDataCenter
{
    public interface IAlarmHandle
    {
        bool init(AlarmProcTableRow Row);
        void AddAlarm(CAlarmItem item);
        bool DoAlarm(CAlarmItem item);
        bool Do(params object[] o);
    }
    public class CAlarmItem
    {
        public int Equipno;
        public string Type;
        public int YcYxNo;
        public string Event, Wave;
        public DateTime Time;
        public int Retrys;
        public bool IsAlarm;
        public bool IsSendAlarm;
        public int? AlarmRiseCycle;
        public int AlarmLevel = 0;
        public int OldAlarmLevel = 0;
        public CAlarmItem(int equipno, string type, int ycyxno, string strevent, string wave, DateTime time, bool isalarm = true, bool issendalarm = true)
        {
            Equipno = equipno;
            Type = type.ToUpper();
            YcYxNo = ycyxno;
            Event = strevent;
            Wave = wave;
            Time = time;
            Retrys = 0;
            IsAlarm = isalarm;
            IsSendAlarm = issendalarm;
        }
        public List<AdministratorTableRow> GetAdminsOfAlarm(int iEquip, DateTime time)
        {
            List<AdministratorTableRow> dt_Administrator;
            List<SpeAlmReportTableRow> dt_Spc;
            List<WeekAlmReportTableRow> dt_Week;
            List<AlmReportTableRow> dt_Limit;
            lock (GWDbProvider.lockstate)
            {
                using (var db = StationItem.MyGWDbProvider.serviceProvider.GetService<GWDataContext>())
                {
                    dt_Spc = db.SpeAlmReportTable.FromSqlRaw("Select * From SpeAlmReport").AsNoTracking().ToList();
                    dt_Week = db.WeekAlmReportTable.FromSqlRaw("Select * From WeekAlmReport").AsNoTracking().ToList();
                    dt_Limit = db.AlmReportTable.FromSqlRaw("Select * From AlmReport").AsNoTracking().ToList();
                    dt_Administrator = db.AdministratorTable.FromSqlRaw("Select * From Administrator Order By Administrator").AsNoTracking().ToList();
                }
            }
            List<AdministratorTableRow> dt = new List<AdministratorTableRow>();
            dt.Clear();
            List<string> AdminsList = new List<string>();
            foreach (SpeAlmReportTableRow r in dt_Spc)
            {
                if (time.CompareTo(r.begin_time) >= 0 && time.CompareTo(r.end_time) <= 0)
                {
                    if (AdminLimit2EquipNo(r.Administrator, iEquip))
                    {
                        foreach (AdministratorTableRow r1 in dt_Administrator)
                        {
                            if (r1.Administrator == r.Administrator)
                            {
                                if (!AdminsList.Contains(r1.Administrator))
                                {
                                    dt.Add(r1);
                                    AdminsList.Add(r1.Administrator);
                                }
                                break;
                            }
                        }
                    }
                }
            }
            if (dt.Count == 0)
            {
                foreach (WeekAlmReportTableRow r in dt_Week)
                {
                    if (r.week_day == General.GetDayOfWeek(time) || r.week_day == 0)
                    {
                        if (time.TimeOfDay >= r.begin_time.TimeOfDay && time.TimeOfDay <= r.end_time.TimeOfDay)
                        {
                            if (AdminLimit2EquipNo(r.Administrator, iEquip))
                            {
                                foreach (AdministratorTableRow r1 in dt_Administrator)
                                {
                                    if (r1.Administrator == r.Administrator)
                                    {
                                        if (!AdminsList.Contains(r1.Administrator))
                                        {
                                            dt.Add(r1);
                                            AdminsList.Add(r1.Administrator);
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (dt.Count == 0)
            {
                foreach (AdministratorTableRow r in StationItem.db_AdminForAlarm)
                {
                    if (AdminLimit2EquipNo(r.Administrator, iEquip))
                    {
                        foreach (AdministratorTableRow r1 in dt_Administrator)
                        {
                            if (r1.Administrator == r.Administrator)
                            {
                                if (!AdminsList.Contains(r1.Administrator))
                                {
                                    dt.Add(r1);
                                    AdminsList.Add(r1.Administrator);
                                }
                                break;
                            }
                        }
                    }
                }
            }
            if (dt.Count == 0)
            {
                foreach (AlmReportTableRow r in dt_Limit)
                {
                    if (AdminLimit2EquipNo(r.Administrator, iEquip))
                    {
                        foreach (AdministratorTableRow r1 in dt_Administrator)
                        {
                            if (r1.Administrator == r.Administrator)
                            {
                                if (!AdminsList.Contains(r1.Administrator))
                                {
                                    dt.Add(r1);
                                    AdminsList.Add(r1.Administrator);
                                }
                                break;
                            }
                        }
                    }
                }
            }
            return dt;
        }
        bool AdminLimit2EquipNo(string admin, int iEquip)
        {
            List<AlmReportTableRow> dt_Limit;
            List<EquipGroupTableRow> dt_EquipGroup;
            lock (GWDbProvider.lockstate)
            {
                using (var db = StationItem.MyGWDbProvider.serviceProvider.GetService<GWDataContext>())
                {
                    dt_Limit = db.AlmReportTable.FromSqlRaw("Select * From AlmReport").AsNoTracking().ToList();
                    dt_EquipGroup = db.EquipGroupTable.FromSqlRaw("Select * From EquipGroup").AsNoTracking().ToList();
                }
            }
            foreach (AlmReportTableRow r in dt_Limit)
            {
                if (r.Administrator.Trim() == admin.Trim())
                {
                    foreach (EquipGroupTableRow r1 in dt_EquipGroup)
                    {
                        if (r1.group_no == r.group_no)
                        {
                            string strEqp = "#" + iEquip + "#";
                            if (r1.equipcomb.IndexOf(strEqp) >= 0)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
            return false;
        }
    }
    public class CAlarmEquipBase : IAlarmHandle
    {
        public Queue AlarmItemQueue = new Queue();
        public List<CAlarmItem> AlarmItemList_Now = new List<CAlarmItem>();
        public virtual bool init(AlarmProcTableRow Row)
        {
            return true;
        }
        public virtual bool DoAlarm(CAlarmItem item)
        {
            return true;
        }
        public virtual bool Do(params object[] o)
        {
            return true;
        }
        public virtual void AddAlarm(CAlarmItem item)
        {
            AddAlarm_Now(item);
            lock (AlarmItemQueue)
            {
                foreach (CAlarmItem i in AlarmItemQueue)
                {
                    if (i.Equipno == item.Equipno && i.YcYxNo == item.YcYxNo && i.Type == item.Type && i.Event == item.Event && i.AlarmLevel == item.AlarmLevel)
                        return;
                }
                if (item.IsSendAlarm)
                    AlarmItemQueue.Enqueue(item);
            }
        }
        public void AddAlarm_Now(CAlarmItem item)
        {
            lock (AlarmItemList_Now)
            {
                foreach (CAlarmItem i in AlarmItemList_Now)
                {
                    if (i.Equipno == item.Equipno && i.YcYxNo == item.YcYxNo && i.Type == item.Type)
                    {
                        if (i.IsAlarm == true && item.IsAlarm == false)
                        {
                            for (int k = AlarmItemList_Now.Count - 1; k >= 0; k--)
                            {
                                if (AlarmItemList_Now[k].Equipno == item.Equipno
                                    && AlarmItemList_Now[k].YcYxNo == item.YcYxNo
                                    && AlarmItemList_Now[k].Type == item.Type)
                                {
                                    AlarmItemList_Now.RemoveAt(k);
                                }
                            }
                            return;
                        }
                        else if (i.IsAlarm == true && item.IsAlarm == true)
                        {
                            return;
                        }
                        else
                        {
                        }
                    }
                }
                if (item.IsAlarm)
                    AlarmItemList_Now.Add(item);
            }
        }
        public void CheckAlarmRise()
        {
            lock (AlarmItemList_Now)
            {
                foreach (CAlarmItem item in AlarmItemList_Now)
                {
                    TimeSpan tp = new TimeSpan();
                    tp = DateTime.Now - item.Time;
                    if (item.AlarmRiseCycle == 0)
                        continue;
                    item.AlarmLevel = (int)(tp.TotalMinutes / item.AlarmRiseCycle);
                    if (item.AlarmLevel > 10)
                        item.AlarmLevel = 0;
                    if (item.AlarmLevel > item.OldAlarmLevel)
                    {
                        lock (AlarmItemQueue)
                        {
                            if (item.IsSendAlarm)
                                AlarmItemQueue.Enqueue(item);
                        }
                        item.OldAlarmLevel = item.AlarmLevel;
                    }
                }
            }
        }
    }
}
