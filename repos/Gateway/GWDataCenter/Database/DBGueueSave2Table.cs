﻿// Copyright (c) 2004-2025  Beijing TOMs Software Technology Co., Ltd
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
namespace GWDataCenter.Database
{
    public class DBGueueSave2Table
    {
        List<YcYxEvtTableRow> YcYxEvtTableRowList = new List<YcYxEvtTableRow>();
        List<YcYxEvtTableRow> tempYcYxEvtTableRowList = new List<YcYxEvtTableRow>();
        List<SysEvtTableRow> SysEvtTableRowList = new List<SysEvtTableRow>();
        List<SysEvtTableRow> tempSysEvtTableRowList = new List<SysEvtTableRow>();
        List<SetEvtTableRow> SetEvtTableRowList = new List<SetEvtTableRow>();
        List<SetEvtTableRow> tempSetEvtTableRowList = new List<SetEvtTableRow>();
        public object Lock4YcYxEvtTable = false;
        public object Lock4SysEvtTable = false;
        public object Lock4SetEvtTable = false;
        public void AddYcYxEvtTableRow2List(YcYxEvtTableRow r)
        {
            lock (Lock4YcYxEvtTable)
            {
                YcYxEvtTableRowList.Add(r);
            }
        }
        public void AddSysEvtTableRow2List(SysEvtTableRow r)
        {
            lock (Lock4SysEvtTable)
            {
                SysEvtTableRowList.Add(r);
            }
        }
        public void AddSetEvtTableRow2List(SetEvtTableRow r)
        {
            lock (Lock4SetEvtTable)
            {
                SetEvtTableRowList.Add(r);
            }
        }
        public void Run()
        {
            Task.Factory.StartNew(delegate
            {
                while (true)
                {
                    lock (Lock4YcYxEvtTable)
                    {
                        tempYcYxEvtTableRowList.AddRange(YcYxEvtTableRowList);
                        YcYxEvtTableRowList.Clear();
                    }
                    lock (Lock4SysEvtTable)
                    {
                        tempSysEvtTableRowList.AddRange(SysEvtTableRowList);
                        SysEvtTableRowList.Clear();
                    }
                    lock (Lock4SetEvtTable)
                    {
                        tempSetEvtTableRowList.AddRange(SetEvtTableRowList);
                        SetEvtTableRowList.Clear();
                    }
                    if (tempYcYxEvtTableRowList.Count > 0 || tempSysEvtTableRowList.Count > 0 || tempSetEvtTableRowList.Count > 0)
                    {
                        lock (GWDbProvider.lockstate)
                        {
                            int Count = 0;
#if DEBUG
                            Int64 startTime = Stopwatch.GetTimestamp();
#endif
                            using (var db = StationItem.MyGWDbProvider.serviceProvider.GetService<GWDataContext>())
                            {
                                if (tempYcYxEvtTableRowList.Count > 0)
                                {
                                    foreach (var item in tempYcYxEvtTableRowList)
                                        db.YcYxEvtTable.Add(item);
                                    Count += tempYcYxEvtTableRowList.Count;
                                    tempYcYxEvtTableRowList.Clear();
                                }
                                if (tempSysEvtTableRowList.Count > 0)
                                {
                                    foreach (var item in tempSysEvtTableRowList)
                                        db.SysEvtTable.Add(item);
                                    Count += tempSysEvtTableRowList.Count;
                                    tempSysEvtTableRowList.Clear();
                                }
                                if (tempSetEvtTableRowList.Count > 0)
                                {
                                    foreach (var item in tempSetEvtTableRowList)
                                        db.SetEvtTable.Add(item);
                                    Count += tempSetEvtTableRowList.Count;
                                    tempSetEvtTableRowList.Clear();
                                }
                                db.SaveChanges();
                            }
#if DEBUG
                            Console.WriteLine($"队列存储到数据库{Count}条总共耗时：");
                            Console.WriteLine((Stopwatch.GetTimestamp() - startTime) / (double)Stopwatch.Frequency);
#endif
                        }
                    }
                    Thread.Sleep(1000);
                }
            }, TaskCreationOptions.LongRunning);
        }
    }
}
