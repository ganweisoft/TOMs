﻿// Copyright (c) 2004-2025  Beijing TOMs Software Technology Co., Ltd
using GWDataCenter.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
namespace GWDataCenter
{
    public class WeekPlan
    {
        public int TableID;
        public string TableName;
        public Dictionary<string, List<IPlanItem>> WeekPlanDict = new Dictionary<string, List<IPlanItem>>();
        public WeekPlan()
        {
            List<GWProcWeekTableRow> Rows;
            lock (GWDbProvider.lockstate)
            {
                using (var db = StationItem.MyGWDbProvider.serviceProvider.GetService<GWDataContext>())
                {
                    Rows = db.GWProcWeekTable.FromSqlRaw("SELECT * FROM GWProcWeekTable").AsNoTracking().ToList();
                }
            }
            if (!Rows.Any())
                return;
            WeekPlanDict.Clear();
            foreach (GWProcWeekTableRow r in Rows)
            {
                WeekPlanDict.Add("Mon", ProcTimeManage.GetPlan(r.Mon));
                WeekPlanDict.Add("Tues", ProcTimeManage.GetPlan(r.Tues));
                WeekPlanDict.Add("Wed", ProcTimeManage.GetPlan(r.Wed));
                WeekPlanDict.Add("Thurs", ProcTimeManage.GetPlan(r.Thurs));
                WeekPlanDict.Add("Fri", ProcTimeManage.GetPlan(r.Fri));
                WeekPlanDict.Add("Sat", ProcTimeManage.GetPlan(r.Sat));
                WeekPlanDict.Add("Sun", ProcTimeManage.GetPlan(r.Sun));
            }
        }
        public void StopAllPlanItem()
        {
            foreach (KeyValuePair<string, List<IPlanItem>> pair in WeekPlanDict)
            {
                if (pair.Value != null)
                {
                    foreach (IPlanItem item in pair.Value)
                        item.bStopPlanItem = true;
                }
            }
        }
        public void StopPlanItemForNoWork()
        {
            foreach (KeyValuePair<string, List<IPlanItem>> pair in WeekPlanDict)
            {
                if (pair.Value != null)
                {
                    foreach (IPlanItem item in pair.Value)
                    {
                        if (item.bWorkState == false)
                            item.bStopPlanItem = true;
                    }
                }
            }
        }
        public void SetAllNoWork()
        {
            foreach (KeyValuePair<string, List<IPlanItem>> pair in WeekPlanDict)
            {
                if (pair.Value != null)
                {
                    foreach (IPlanItem item in pair.Value)
                        item.bWorkState = false;
                }
            }
        }
    }
}
