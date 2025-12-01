﻿// Copyright (c) 2004-2025  Beijing TOMs Software Technology Co., Ltd
using GWDataCenter.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OpenGWDataCenter.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
namespace GWDataCenter
{
    public static class EquipmentLinkage
    {
        static Queue SetItemQueue = new Queue();
        static List<EquipmentLinkageItem> EquipmentLinkageList = new List<EquipmentLinkageItem>();
        static Thread RefreshThread;
        const int ThreadInterval = 100;
        static object reset = false;
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
        public static void Run()
        {
            InitListFromDB();
        }
        static bool Reset()
        {
            if (EquipmentLinkageList.Count > 0)
            {
                foreach (EquipmentLinkageItem item in EquipmentLinkageList)
                {
                    item.HandleEvent(true);
                }
            }
            lock (SetItemQueue)
            {
                SetItemQueue.Clear();
            }
            List<AutoProcTableRow> Rows;
            lock (GWDbProvider.lockstate)
            {
                using (var db = StationItem.MyGWDbProvider.serviceProvider.GetService<GWDataContext>())
                {
                    Rows = db.AutoProcTable.FromSqlRaw("SELECT * FROM AutoProc").AsNoTracking().ToList();
                }
            }
            if (!Rows.Any())
                return false;
            EquipmentLinkageList.Clear();
            foreach (AutoProcTableRow r in Rows)
            {
                EquipmentLinkageItem item = new EquipmentLinkageItem(r);
                EquipmentLinkageList.Add(item);
                item.HandleEvent();
            }
            return true;
        }
        static void InitListFromDB()
        {
            Reset();
            ThreadStart entryPoint = new ThreadStart(Refresh);
            RefreshThread = new Thread(entryPoint);
            RefreshThread.Start();
        }
        static public void AddSetItem(SetItem item)
        {
            lock (SetItemQueue)
            {
                if (!SetItemQueue.Contains(item))
                    SetItemQueue.Enqueue(item);
            }
        }
        static public SetItem DelSetItem()
        {
            lock (SetItemQueue)
            {
                if (SetItemQueue.Count > 0)
                {
                    return (SetItem)SetItemQueue.Dequeue();
                }
                else
                    return null;
            }
        }
        static void Refresh()
        {
            try
            {
                while (true)
                {
                    if (bReset)
                    {
                        Reset();
                        bReset = false;
                    }
                    SetItem setitem = DelSetItem();
                    if (setitem != null)
                    {
                        setitem.sysExecutor = ResourceService.GetString("AlarmCenter.DataCenter.Linkage") + "(system)";
                        if (setitem.Type == null)
                            continue;
                        if (setitem.Type.ToUpper() == "J")
                        {
                            EquipItem Item = DataCenter.GetEquipItem(setitem.EquipNo);
                            if (Item == null)
                                continue;
                            if (System.Math.Abs(Environment.TickCount - setitem.StartTickCount) >= setitem.WaitingTime)
                            {
                                if (setitem.bRecord)
                                {
                                    string msg = ResourceService.GetString("AlarmCenter.DataCenter.Linkage") + ">>>" + Item.Equip_nm + ":" + setitem.GetSetItemDesc();
                                    string guid = Guid.NewGuid().ToString();
                                    MessageService.AddMessage(guid, MessageLevel.SetParm, msg, setitem.EquipNo, false);
                                    MessageService.InsertMsg2SysEvt(msg, guid);
                                }
                            }
                            else
                            {
                                AddSetItem(setitem);
                            }
                        }
                        else
                        {
                            EquipItem Item = DataCenter.GetEquipItem(setitem.EquipNo);
                            if (Item == null)
                                continue;
                            if (System.Math.Abs(Environment.TickCount - setitem.StartTickCount) >= setitem.WaitingTime)
                            {
                                Item.AddSetItem(setitem);
                                if (setitem.bRecord)
                                {
                                    string msg = ResourceService.GetString("AlarmCenter.DataCenter.Linkage") + ">>>" + Item.Equip_nm + ":" + setitem.GetSetItemDesc();
                                    string guid = Guid.NewGuid().ToString();
                                    MessageService.AddMessage(guid, MessageLevel.SetParm, msg, setitem.EquipNo, false);
                                    MessageService.InsertMsg2SysEvt(msg, guid);
                                }
                            }
                            else
                            {
                                AddSetItem(setitem);
                            }
                        }
                    }
                    Thread.Sleep(ThreadInterval);
                }
            }
            catch (Exception e)
            {
                MessageService.AddMessage(MessageLevel.Fatal, General.GetExceptionInfo(e), 0);
            }
        }
    }
    class EquipmentLinkageItem
    {
        string Type;
        int S_EquipNo;
        int S_YcYxNo;
        int D_EquipNo;
        int D_SetNo;
        string D_Value;
        int Delay;
        SetItem D_SetItem;
        string Description;
        bool? bEnable;
        public EquipmentLinkageItem(AutoProcTableRow Row)
        {
            try
            {
                S_EquipNo = Row.iequip_no;
                S_YcYxNo = Row.iycyx_no;
                Type = Row.iycyx_type;
                Delay = Row.delay;
                D_EquipNo = Row.oequip_no;
                D_SetNo = Row.oset_no;
                D_Value = Row.value;
                bEnable = null;
                bEnable = Row.Enable;
                string s;
                s = Row.value;
                lock (GWDbProvider.lockstate)
                {
                    using (var db = StationItem.MyGWDbProvider.serviceProvider.GetService<GWDataContext>())
                    {
                        try
                        {
                            var R = db.SetParmTable.Single(m => (m.equip_no == D_EquipNo && m.set_no == D_SetNo));
                            string cmd1 = R.main_instruction;
                            string cmd2 = R.minor_instruction;
                            string cmd3 = R.value;
                            if (string.IsNullOrEmpty(s.Trim()))
                                D_SetItem = new SetItem(D_EquipNo, cmd1, cmd2, cmd3);
                            else
                            {
                                D_SetItem = new SetItem(D_EquipNo, cmd1, cmd2, s);
                                D_SetItem.Type = "V";
                                D_SetItem.m_SetNo = D_SetNo;
                            }
                            Description = Row.ProcDesc;
                        }
                        catch (Exception e1)
                        {
                            string msg = string.Format("联动出错:SetParm表中不存在设备号为{0},设置号为{1}的动作. ", D_EquipNo, D_SetNo);
                            string guid = Guid.NewGuid().ToString();
                            MessageService.AddMessage(guid, MessageLevel.SetParm, msg, D_EquipNo, false);
                            MessageService.InsertMsg2SysEvt(msg, guid);
                            D_SetItem = null;
                            return;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageService.AddMessage(MessageLevel.Fatal, e.ToString() + e.Source, 0);
            }
        }
        public void HandleEvent(bool bRemove = false)
        {
            if (S_EquipNo == 0)
            {
                if (Type == "E")
                {
                    if (bRemove)
                        StationItem.StationCommError -= new EventHandler(Do);
                    else
                        StationItem.StationCommError += new EventHandler(Do);
                }
                if (Type == "e")
                {
                    if (bRemove)
                        StationItem.StationCommOk -= new EventHandler(Do);
                    else
                        StationItem.StationCommOk += new EventHandler(Do);
                }
                if (Type == "S")
                {
                    if (bRemove)
                        StationItem.StationHaveAlarm -= new EventHandler(Do);
                    else
                        StationItem.StationHaveAlarm += new EventHandler(Do);
                }
                if (Type == "s")
                {
                    if (bRemove)
                        StationItem.StationNoAlarm -= new EventHandler(Do);
                    else
                        StationItem.StationNoAlarm += new EventHandler(Do);
                }
                return;
            }
            EquipItem EqpIt = DataCenter.GetEquipItem(S_EquipNo);
            if (EqpIt == null)
                return;
            switch (Type)
            {
                case "X":
                    YXItem ItemX = null;
                    if (DataCenter.GetEquipItem(S_EquipNo).YXItemDict.ContainsKey(S_YcYxNo))
                        ItemX = DataCenter.GetEquipItem(S_EquipNo).YXItemDict[S_YcYxNo];
                    if (ItemX != null)
                    {
                        if (bRemove)
                            ItemX.Alarmed -= new YXItem.YXAlarmEventHandler(Do);
                        else
                            ItemX.Alarmed += new YXItem.YXAlarmEventHandler(Do);
                    }
                    break;
                case "x":
                    YXItem Itemx = null;
                    if (DataCenter.GetEquipItem(S_EquipNo).YXItemDict.ContainsKey(S_YcYxNo))
                        Itemx = DataCenter.GetEquipItem(S_EquipNo).YXItemDict[S_YcYxNo];
                    if (Itemx != null)
                    {
                        if (bRemove)
                            Itemx.AlarmRestored -= new YXItem.YXAlarmRestoreEventHandler(Do);
                        else
                            Itemx.AlarmRestored += new YXItem.YXAlarmRestoreEventHandler(Do);
                    }
                    break;
                case "C":
                    YCItem ItemC = null;
                    if (DataCenter.GetEquipItem(S_EquipNo).YCItemDict.ContainsKey(S_YcYxNo))
                        ItemC = DataCenter.GetEquipItem(S_EquipNo).YCItemDict[S_YcYxNo];
                    if (ItemC != null)
                    {
                        if (bRemove)
                            ItemC.Alarmed -= new YCItem.YCAlarmEventHandler(Do);
                        else
                            ItemC.Alarmed += new YCItem.YCAlarmEventHandler(Do);
                    }
                    break;
                case "c":
                    YCItem Itemc = null;
                    if (DataCenter.GetEquipItem(S_EquipNo).YCItemDict.ContainsKey(S_YcYxNo))
                        Itemc = DataCenter.GetEquipItem(S_EquipNo).YCItemDict[S_YcYxNo];
                    if (Itemc != null)
                    {
                        if (bRemove)
                            Itemc.AlarmRestored -= new YCItem.YCAlarmRestoreEventHandler(Do);
                        else
                            Itemc.AlarmRestored += new YCItem.YCAlarmRestoreEventHandler(Do);
                    }
                    break;
                case "E":
                    EquipItem ItemE = DataCenter.GetEquipItem(S_EquipNo);
                    if (ItemE != null)
                    {
                        if (bRemove)
                            ItemE.EquipCommError -= new EventHandler(Do);
                        else
                            ItemE.EquipCommError += new EventHandler(Do);
                    }
                    break;
                case "e":
                    EquipItem Iteme = DataCenter.GetEquipItem(S_EquipNo);
                    if (Iteme != null)
                    {
                        if (bRemove)
                            Iteme.EquipCommOk -= new EventHandler(Do);
                        else
                            Iteme.EquipCommOk += new EventHandler(Do);
                    }
                    break;
                case "S":
                    EquipItem ItemS = DataCenter.GetEquipItem(S_EquipNo);
                    if (ItemS != null)
                    {
                        if (bRemove)
                            ItemS.EquipHaveAlarm -= new EventHandler(Do);
                        else
                            ItemS.EquipHaveAlarm += new EventHandler(Do);
                    }
                    break;
                case "s":
                    EquipItem Items = DataCenter.GetEquipItem(S_EquipNo);
                    if (Items != null)
                    {
                        if (bRemove)
                            Items.EquipNoAlarm -= new EventHandler(Do);
                        else
                            Items.EquipNoAlarm += new EventHandler(Do);
                    }
                    break;
                default:
                    break;
            }
        }
        void Do(object sender, EventArgs e)
        {
            if (bEnable == false)
                return;
            if (D_SetItem != null)
            {
                if (GetStateTrack(sender))
                {
                    D_SetItem.WaitingTime = Delay;
                    D_SetItem.StartTickCount = Environment.TickCount;
                    D_SetItem.Description = Description;
                    EquipmentLinkage.AddSetItem(D_SetItem);
                }
            }
        }
        bool GetStateTrack(object sender)
        {
            if (sender is YCItem)
            {
                YCItem item = (YCItem)sender;
                return item.AlarmStateTrack.IsDifferentState4EquipmentLinkage();
            }
            if (sender is YXItem)
            {
                YXItem item = (YXItem)sender;
                return item.AlarmStateTrack.IsDifferentState4EquipmentLinkage();
            }
            return true;
        }
    }
}
