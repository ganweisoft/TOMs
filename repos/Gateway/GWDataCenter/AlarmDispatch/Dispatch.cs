﻿// Copyright (c) 2004-2025  Beijing TOMs Software Technology Co., Ltd
using GWDataCenter.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OpenGWDataCenter.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
namespace GWDataCenter
{
    public static class AlarmDispatch
    {
        static public Dictionary<int, IAlarmHandle> AlarmModuleDict = new Dictionary<int, IAlarmHandle>();
        static Assembly dll;
        static IAlarmHandle IAlarm;
        static public void LoadAlarmModule(object o)
        {
            try
            {
                List<AlarmProcTableRow> Rows;
                lock (GWDbProvider.lockstate)
                {
                    using (var db = StationItem.MyGWDbProvider.serviceProvider.GetService<GWDataContext>())
                    {
                        Rows = db.AlarmProcTable.FromSqlRaw("SELECT * FROM AlarmProc").AsNoTracking().ToList();
                    }
                }
                foreach (AlarmProcTableRow r in Rows)
                {
                    string FullPathName;
                    FullPathName = Path.Combine(General.GetApplicationRootPath(), "dll");
                    FullPathName = Path.Combine(FullPathName, r.Proc_Module);
                    try
                    {
                        dll = Assembly.LoadFrom(FullPathName);
                    }
                    catch (Exception e)
                    {
                        MessageService.AddMessage(MessageLevel.Fatal, General.GetExceptionInfo(e), 0, false);
                    }
                    if (dll == null)
                        break;
                    Type[] types = dll.GetTypes();
                    foreach (Type t in types)
                    {
                        if (t.Name == "CAlarm")
                        {
                            IAlarm = dll.CreateInstance(t.FullName) as IAlarmHandle;
                            break;
                        }
                    }
                    if (IAlarm == null)
                    {
                        string msg = FullPathName + " do not implement  Interface of IAlarmHandle";
                        throw new ArgumentException(msg);
                    }
                    else
                    {
                        if (IAlarm.init(r))
                        {
                            AlarmModuleDict.Add(r.Proc_Code, IAlarm);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                MessageService.AddMessage(MessageLevel.Fatal, e.ToString(), 0, false);
            }
        }
        static public void init()
        {
            LoadAlarmModule(null);
            foreach (KeyValuePair<string, object> pair in StationItem.EquipCategoryDict)
            {
                SubEquipList EquipList = (SubEquipList)pair.Value;
                foreach (EquipItem equipItem in EquipList.EquipList)
                {
                    Dispatch(equipItem);
                }
            }
        }
        static public void Dispatch(EquipItem equipItem)
        {
            equipItem.EquipCommError -= new EventHandler(AlarmDispatch.equipItem_EquipCommError);
            equipItem.EquipCommError += new EventHandler(AlarmDispatch.equipItem_EquipCommError);
            equipItem.EquipCommOk -= new EventHandler(AlarmDispatch.equipItem_EquipCommOk);
            equipItem.EquipCommOk += new EventHandler(AlarmDispatch.equipItem_EquipCommOk);
            foreach (KeyValuePair<int, YCItem> ycpair in equipItem.YCItemDict)
            {
                YCItem ycitem = (YCItem)ycpair.Value;
                ycitem.Alarmed -= new YCItem.YCAlarmEventHandler(AlarmDispatch.ycitem_Alarmed);
                ycitem.Alarmed += new YCItem.YCAlarmEventHandler(AlarmDispatch.ycitem_Alarmed);
                ycitem.AlarmRestored -= new YCItem.YCAlarmRestoreEventHandler(AlarmDispatch.ycitem_AlarmRestored);
                ycitem.AlarmRestored += new YCItem.YCAlarmRestoreEventHandler(AlarmDispatch.ycitem_AlarmRestored);
            }
            foreach (KeyValuePair<int, YXItem> yxpair in equipItem.YXItemDict)
            {
                YXItem yxitem = (YXItem)yxpair.Value;
                yxitem.Alarmed -= new YXItem.YXAlarmEventHandler(AlarmDispatch.yxitem_Alarmed);
                yxitem.Alarmed += new YXItem.YXAlarmEventHandler(AlarmDispatch.yxitem_Alarmed);
                yxitem.AlarmRestored -= new YXItem.YXAlarmRestoreEventHandler(AlarmDispatch.yxitem_AlarmRestored);
                yxitem.AlarmRestored += new YXItem.YXAlarmRestoreEventHandler(AlarmDispatch.yxitem_AlarmRestored);
            }
        }
        static public void equipItem_EquipCommOk(object sender, EventArgs e)
        {
            EquipItem item = (EquipItem)sender;
            if ((item.Alarm_scheme & 0x02) > 0)
            {
                DateTime t = General.Convert2DT(DateTime.Now);
                StationItem.MyDBGueueSave2Table.AddYcYxEvtTableRow2List(new YcYxEvtTableRow
                {
                    sta_n = item.iStano,
                    equip_no = item.iEquipno,
                    ycyx_no = 0,
                    ycyx_type = "E",
                    gwEvent = item.RestorealarmMsg,
                    time = t,
                    proc_rec = "",
                    alarmlevel = item.Alarm_scheme,
                    GUID = item.EventGUID,
                    snapshotlevel = 0
                });
            }
            foreach (KeyValuePair<int, IAlarmHandle> pair in AlarmModuleDict)
            {
                CAlarmItem AlarmItem = new CAlarmItem(item.iEquipno, "E", 0, item.RestorealarmMsg, item.restore_wave_file, General.Convert2DT(DateTime.Now), false);
                AlarmItem.AlarmRiseCycle = item.AlarmRiseCycle;
                if ((item.Alarm_scheme & pair.Key) > 0)
                {
                    pair.Value.AddAlarm(AlarmItem);
                }
            }
        }
        static public void equipItem_EquipCommError(object sender, EventArgs e)
        {
            EquipItem item = (EquipItem)sender;
            if ((item.Alarm_scheme & 0x02) > 0)
            {
                DateTime t = General.Convert2DT(DateTime.Now);
                StationItem.MyDBGueueSave2Table.AddYcYxEvtTableRow2List(new YcYxEvtTableRow
                {
                    sta_n = item.iStano,
                    equip_no = item.iEquipno,
                    ycyx_no = 0,
                    ycyx_type = "E",
                    gwEvent = item.alarmMsg,
                    time = t,
                    proc_rec = "",
                    alarmlevel = item.Alarm_scheme,
                    GUID = item.EventGUID,
                    snapshotlevel = 9
                });
            }
            foreach (KeyValuePair<int, IAlarmHandle> pair in AlarmModuleDict)
            {
                CAlarmItem AlarmItem = new CAlarmItem(item.iEquipno, "E", 0, item.alarmMsg, item.wave_file, General.Convert2DT(DateTime.Now));
                AlarmItem.AlarmRiseCycle = item.AlarmRiseCycle;
                if ((item.Alarm_scheme & pair.Key) > 0)
                {
                    pair.Value.AddAlarm(AlarmItem);
                }
            }
        }
        public static void Add2MessageService(int level, string msg, string msg1, int equipno, string type, int ycyxno, string wav, string related_pic, string V, string Z, string P, string guid)
        {
            MessageService.Edit_AddMessage(level, msg, msg1, equipno, type, ycyxno, wav, related_pic, V, Z, P, guid);
        }
        static public void ycitem_Alarmed(object sender, EventArgs e)
        {
            YCItem item = (YCItem)sender;
            if ((item.Alarm_scheme & 0x02) > 0 && !item.bRepeatAlarm)
            {
                if (item.AlarmStateTrack.IsDifferentState(2))
                {
                    DateTime t = General.Convert2DT(DateTime.Now);
                    StationItem.MyDBGueueSave2Table.AddYcYxEvtTableRow2List(new YcYxEvtTableRow
                    {
                        sta_n = item.Sta_n,
                        equip_no = item.Equip_no,
                        ycyx_no = item.Yc_no,
                        ycyx_type = "C",
                        gwEvent = item.AlarmMsg,
                        time = t,
                        proc_rec = "",
                        alarmlevel = item.Alarm_scheme,
                        GUID = item.EventGUID,
                        snapshotlevel = item.Level
                    });
                }
            }
            foreach (KeyValuePair<int, IAlarmHandle> pair in AlarmModuleDict)
            {
                CAlarmItem AlarmItem = new CAlarmItem(item.Equip_no, "C", item.Yc_no, item.AlarmMsg, item.Wave_file, General.Convert2DT(DateTime.Now));
                AlarmItem.AlarmRiseCycle = item.AlarmRiseCycle;
                if ((item.Alarm_scheme & pair.Key) > 0)
                {
                    if (item.bRepeatAlarm)
                        pair.Value.AddAlarm(AlarmItem);
                    else
                    {
                        if (item.AlarmStateTrack.IsDifferentState(pair.Key))
                            pair.Value.AddAlarm(AlarmItem);
                    }
                }
            }
        }
        static public void yxitem_Alarmed(object sender, EventArgs e)
        {
            YXItem item = (YXItem)sender;
            if ((item.Alarm_scheme & 0x02) > 0 && !item.bRepeatAlarm)
            {
                if (item.AlarmStateTrack.IsDifferentState(2))
                {
                    DateTime t = General.Convert2DT(DateTime.Now);
                    StationItem.MyDBGueueSave2Table.AddYcYxEvtTableRow2List(new YcYxEvtTableRow
                    {
                        sta_n = item.Sta_n,
                        equip_no = item.Equip_no,
                        ycyx_no = item.Yx_no,
                        ycyx_type = "X",
                        gwEvent = item.AlarmMsg,
                        time = t,
                        proc_rec = "",
                        alarmlevel = item.Alarm_scheme,
                        GUID = item.EventGUID,
                        snapshotlevel = item.AlarmLevel
                    });
                }
            }
            foreach (KeyValuePair<int, IAlarmHandle> pair in AlarmModuleDict)
            {
                CAlarmItem AlarmItem = new CAlarmItem(item.Equip_no, "X", item.Yx_no, item.AlarmMsg, item.Wave_file, General.Convert2DT(DateTime.Now));
                AlarmItem.AlarmRiseCycle = item.AlarmRiseCycle;
                if ((item.Alarm_scheme & pair.Key) > 0)
                {
                    if (item.bRepeatAlarm)
                        pair.Value.AddAlarm(AlarmItem);
                    else
                    {
                        if (item.AlarmStateTrack.IsDifferentState(pair.Key))
                            pair.Value.AddAlarm(AlarmItem);
                    }
                }
            }
        }
        static public void ycitem_AlarmRestored(object sender, EventArgs e)
        {
            YCItem item = (YCItem)sender;
            if ((item.Alarm_scheme & 0x02) > 0)
            {
                if (item.AlarmStateTrack.IsDifferentState(2))
                {
                    DateTime t = General.Convert2DT(DateTime.Now);
                    StationItem.MyDBGueueSave2Table.AddYcYxEvtTableRow2List(new YcYxEvtTableRow
                    {
                        sta_n = item.Sta_n,
                        equip_no = item.Equip_no,
                        ycyx_no = item.Yc_no,
                        ycyx_type = "C",
                        gwEvent = item.AlarmMsg,
                        time = t,
                        proc_rec = "",
                        alarmlevel = item.Alarm_scheme,
                        GUID = item.EventGUID,
                        snapshotlevel = 0
                    });
                }
            }
            foreach (KeyValuePair<int, IAlarmHandle> pair in AlarmModuleDict)
            {
                CAlarmItem AlarmItem = new CAlarmItem(item.Equip_no, "C", item.Yc_no, item.AlarmMsg, item.Restore_Wave_file, General.Convert2DT(DateTime.Now), false);
                AlarmItem.AlarmRiseCycle = item.AlarmRiseCycle;
                if ((item.Alarm_scheme & pair.Key) > 0)
                {
                    if (!item.AlarmStateTrack.IsDifferentState(pair.Key))
                        return;
                    if (item.Level >= 2)
                        pair.Value.AddAlarm(AlarmItem);
                    else
                    {
                        AlarmItem.IsSendAlarm = false;
                        pair.Value.AddAlarm(AlarmItem);
                    }
                }
            }
        }
        static public void yxitem_AlarmRestored(object sender, EventArgs e)
        {
            YXItem item = (YXItem)sender;
            if ((item.Alarm_scheme & 0x02) > 0)
            {
                if (item.AlarmStateTrack.IsDifferentState(2))
                {
                    DateTime t = General.Convert2DT(DateTime.Now);
                    StationItem.MyDBGueueSave2Table.AddYcYxEvtTableRow2List(new YcYxEvtTableRow
                    {
                        sta_n = item.Sta_n,
                        equip_no = item.Equip_no,
                        ycyx_no = item.Yx_no,
                        ycyx_type = "X",
                        gwEvent = item.RestoreMsg,
                        time = t,
                        proc_rec = "",
                        alarmlevel = item.Alarm_scheme,
                        GUID = item.EventGUID,
                        snapshotlevel = item.Restorelevel
                    });
                }
            }
            foreach (KeyValuePair<int, IAlarmHandle> pair in AlarmModuleDict)
            {
                CAlarmItem AlarmItem = new CAlarmItem(item.Equip_no, "X", item.Yx_no, item.RestoreMsg, item.Restore_Wave_file, General.Convert2DT(DateTime.Now), false);
                AlarmItem.AlarmRiseCycle = item.AlarmRiseCycle;
                if ((item.Alarm_scheme & pair.Key) > 0)
                {
                    if (!item.AlarmStateTrack.IsDifferentState(pair.Key))
                        return;
                    if (item.Level_d >= item.Level_r)
                    {
                        if (item.Level_r >= 2)
                            pair.Value.AddAlarm(AlarmItem);
                        else
                        {
                            AlarmItem.IsSendAlarm = false;
                            pair.Value.AddAlarm(AlarmItem);
                        }
                    }
                    else if (item.Level_r >= item.Level_d)
                    {
                        if (item.Level_d >= 2)
                            pair.Value.AddAlarm(AlarmItem);
                        else
                        {
                            AlarmItem.IsSendAlarm = false;
                            pair.Value.AddAlarm(AlarmItem);
                        }
                    }
                }
            }
        }
        static void DispatchAlarm(object sender, EventArgs e)
        {
        }
    }
}
