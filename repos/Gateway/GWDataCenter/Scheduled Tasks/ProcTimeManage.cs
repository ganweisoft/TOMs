﻿// Copyright (c) 2004-2025  Beijing TOMs Software Technology Co., Ltd
using GWDataCenter.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
namespace GWDataCenter
{
    static public class ProcTimeManage
    {
        static public WeekPlan m_WeekPlan = null;
        static public SpecDatePlan m_SpecDatePlan = null;
        static Thread RefreshThread;
        const int ThreadInterval = 500;
        static DateTime dt_now, dt_old;
        static bool DoSpecTimePlan = false;
        static Assembly dll;
        static Dictionary<int, TimePlanItem> DT = new Dictionary<int, TimePlanItem>();
        static Dictionary<int, CyclePlanItem> DC = new Dictionary<int, CyclePlanItem>();
        public static object reset = false;
        public static bool bReset
        {
            get
            {
                lock (reset)
                {
                    return (bool)reset;
                }
            }
            set
            {
                lock (reset)
                {
                    reset = value;
                }
            }
        }
        public static void TemporarilyBreak4TC(string csTC, bool bTemporarilyBreak)
        {
            string[] ss = csTC.Split('+');
            foreach (string s in ss)
            {
                string s1 = s.Trim();
                if (s1.Length > 1)
                {
                    string sType = s1.Substring(0, 1).ToUpper();
                    int iTableID;
                    if (!Int32.TryParse(s1.Substring(1), out iTableID))
                        continue;
                    if (sType == "T")
                    {
                        if (DT.ContainsKey(iTableID))
                        {
                            DT[iTableID].m_bTemporarilyBreak = bTemporarilyBreak;
                        }
                    }
                    if (sType == "C")
                    {
                        if (DC.ContainsKey(iTableID))
                        {
                            DC[iTableID].m_bTemporarilyBreak = bTemporarilyBreak;
                        }
                    }
                }
            }
        }
        static bool Reset()
        {
            GetAllDTAndDC();
            if (m_WeekPlan != null)
            {
                m_WeekPlan.StopAllPlanItem();
                m_WeekPlan = null;
            }
            if (m_SpecDatePlan != null)
            {
                m_SpecDatePlan.StopAllPlanItem();
                m_SpecDatePlan = null;
            }
            m_WeekPlan = new WeekPlan();
            m_SpecDatePlan = new SpecDatePlan();
            ResetSpecDatePlan();
            ResetWeekPlan();
            return true;
        }
        static ProcTimeManage()
        {
            Reset();
        }
        public static void Run()
        {
            ThreadStart entryPoint = new ThreadStart(Refresh);
            RefreshThread = new Thread(entryPoint);
            RefreshThread.Start();
        }
        static void GetAllDTAndDC()
        {
            List<GWProcTimeTListTableRow> Rows;
            lock (GWDbProvider.lockstate)
            {
                using (var db = StationItem.MyGWDbProvider.serviceProvider.GetService<GWDataContext>())
                {
                    Rows = db.GWProcTimeTListTable.FromSqlRaw("SELECT * FROM GWProcTimeTList").AsNoTracking().ToList();
                }
            }
            if (Rows.Any())
            {
                DT.Clear();
                foreach (GWProcTimeTListTableRow r in Rows)
                {
                    DT.Add(r.TableID, new TimePlanItem(r.TableID));
                }
            }
            List<GWProcCycleTListTableRow> Rows1;
            lock (GWDbProvider.lockstate)
            {
                using (var db = StationItem.MyGWDbProvider.serviceProvider.GetService<GWDataContext>())
                {
                    Rows1 = db.GWProcCycleTListTable.FromSqlRaw("SELECT * FROM GWProcCycleTList").AsNoTracking().ToList();
                }
            }
            if (Rows1.Any())
            {
                DC.Clear();
                foreach (GWProcCycleTListTableRow r in Rows1)
                {
                    DC.Add(r.TableID, new CyclePlanItem(r.TableID));
                }
            }
        }
        static public List<IPlanItem> GetPlan(string cs)
        {
            if (string.IsNullOrEmpty(cs))
                return null;
            List<IPlanItem> L = new List<IPlanItem>();
            string[] ss = cs.Split('+');
            if (ss.Length > 0)
            {
                foreach (string s in ss)
                {
                    if (s[0] == 'T')
                    {
                        if (DT.ContainsKey(Convert.ToInt32(s.Substring(1))))
                        {
                            L.Add(DT[Convert.ToInt32(s.Substring(1))]);
                        }
                    }
                    if (s[0] == 'C')
                    {
                        if (DC.ContainsKey(Convert.ToInt32(s.Substring(1))))
                        {
                            L.Add(DC[Convert.ToInt32(s.Substring(1))]);
                        }
                    }
                }
            }
            return L;
        }
        static void ResetWeekPlan()
        {
            m_WeekPlan.SetAllNoWork();
            if (DoSpecTimePlan)
                return;
            foreach (KeyValuePair<string, List<IPlanItem>> pair in m_WeekPlan.WeekPlanDict)
            {
                switch (DateTime.Now.DayOfWeek)
                {
                    case DayOfWeek.Monday:
                        if (pair.Key == "Mon" && pair.Value != null)
                        {
                            foreach (IPlanItem item in pair.Value)
                            {
                                item.bStopPlanItem = false;
                                item.bWorkState = true;
                            }
                        }
                        break;
                    case DayOfWeek.Tuesday:
                        if (pair.Key == "Tues" && pair.Value != null)
                        {
                            foreach (IPlanItem item in pair.Value)
                            {
                                item.bStopPlanItem = false;
                                item.bWorkState = true;
                            }
                        }
                        break;
                    case DayOfWeek.Wednesday:
                        if (pair.Key == "Wed" && pair.Value != null)
                        {
                            foreach (IPlanItem item in pair.Value)
                            {
                                item.bStopPlanItem = false;
                                item.bWorkState = true;
                            }
                        }
                        break;
                    case DayOfWeek.Thursday:
                        if (pair.Key == "Thurs" && pair.Value != null)
                        {
                            foreach (IPlanItem item in pair.Value)
                            {
                                item.bStopPlanItem = false;
                                item.bWorkState = true;
                            }
                        }
                        break;
                    case DayOfWeek.Friday:
                        if (pair.Key == "Fri" && pair.Value != null)
                        {
                            foreach (IPlanItem item in pair.Value)
                            {
                                item.bStopPlanItem = false;
                                item.bWorkState = true;
                            }
                        }
                        break;
                    case DayOfWeek.Saturday:
                        if (pair.Key == "Sat" && pair.Value != null)
                        {
                            foreach (IPlanItem item in pair.Value)
                            {
                                item.bStopPlanItem = false;
                                item.bWorkState = true;
                            }
                        }
                        break;
                    case DayOfWeek.Sunday:
                        if (pair.Key == "Sun" && pair.Value != null)
                        {
                            foreach (IPlanItem item in pair.Value)
                            {
                                item.bStopPlanItem = false;
                                item.bWorkState = true;
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            m_WeekPlan.StopPlanItemForNoWork();
        }
        static void ResetSpecDatePlan()
        {
            DoSpecTimePlan = false;
            m_SpecDatePlan.SetAllNoWork();
            foreach (KeyValuePair<DateTime, List<IPlanItem>> pair in m_SpecDatePlan.SpecDatePlanDict)
            {
                if (pair.Key.Date == DateTime.Now.Date)
                {
                    DoSpecTimePlan = true;
                    if (pair.Value != null)
                    {
                        foreach (IPlanItem item in pair.Value)
                        {
                            item.bStopPlanItem = false;
                            item.bWorkState = true;
                        }
                    }
                }
            }
            m_SpecDatePlan.StopPlanItemForNoWork();
        }
        static void Refresh()
        {
            while (true)
            {
                if (bReset)
                {
                    Reset();
                    bReset = false;
                }
                dt_now = DateTime.Now;
                if (dt_now.Hour < dt_old.Hour)
                {
                    ResetSpecDatePlan();
                    ResetWeekPlan();
                }
                Thread.Sleep(ThreadInterval);
                dt_old = dt_now;
            }
        }
    }
}
