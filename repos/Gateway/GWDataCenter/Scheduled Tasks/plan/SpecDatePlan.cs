﻿// Copyright (c) 2004-2025  Beijing TOMs Software Technology Co., Ltd
using GWDataCenter.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
namespace GWDataCenter
{
    public class SpecDatePlan
    {
        public int TableID;
        public string TableName;
        public Dictionary<DateTime, List<IPlanItem>> SpecDatePlanDict = new Dictionary<DateTime, List<IPlanItem>>();
        public SpecDatePlan()
        {
            List<GWProcSpecTableRow> Rows;
            lock (GWDbProvider.lockstate)
            {
                using (var db = StationItem.MyGWDbProvider.serviceProvider.GetService<GWDataContext>())
                {
                    Rows = db.GWProcSpecTable.FromSqlRaw("SELECT * FROM GWProcSpecTable").AsNoTracking().ToList();
                }
            }
            if (!Rows.Any())
                return;
            SpecDatePlanDict.Clear();
            foreach (GWProcSpecTableRow r in Rows)
            {
                DateTime EndDate = r.EndDate;
                DateTime BeginDate = r.BeginDate;
                List<IPlanItem> list = ProcTimeManage.GetPlan(r.TableID);
                if (list == null)
                    continue;
                while ((EndDate - BeginDate).Days >= 0)
                {
                    if (SpecDatePlanDict.ContainsKey(BeginDate))
                    {
                        SpecDatePlanDict[BeginDate].AddRange(list);
                    }
                    else
                    {
                        SpecDatePlanDict.Add(BeginDate, list);
                    }
                    BeginDate = BeginDate.AddDays(1);
                }
            }
        }
        public void StopAllPlanItem()
        {
            foreach (KeyValuePair<DateTime, List<IPlanItem>> pair in SpecDatePlanDict)
            {
                if (pair.Value != null)
                {
                    foreach (IPlanItem item in pair.Value)
                        item.bStopPlanItem = true;
                }
            }
        }
        public void SetAllNoWork()
        {
            foreach (KeyValuePair<DateTime, List<IPlanItem>> pair in SpecDatePlanDict)
            {
                if (pair.Value != null)
                {
                    foreach (IPlanItem item in pair.Value)
                        item.bWorkState = false;
                }
            }
        }
        public void StopPlanItemForNoWork()
        {
            foreach (KeyValuePair<DateTime, List<IPlanItem>> pair in SpecDatePlanDict)
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
    }
}
