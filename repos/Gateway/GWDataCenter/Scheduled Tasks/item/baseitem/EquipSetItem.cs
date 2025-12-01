﻿// Copyright (c) 2004-2025  Beijing TOMs Software Technology Co., Ltd
using GWDataCenter.Database;
using OpenGWDataCenter.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
namespace GWDataCenter
{
    public enum ItemType
    {
        TimePlan,
        CyclePlan
    }
    public class EquipSetItem : ISetItem
    {
        public ItemType Type;
        public bool IsExcute = false;
        public int ID;
        DateTime StartExcuteTime, ValueTimeSpan;
        public DateTime StartValidTime, EndValidTime;
        public SetItem EqpSetItem = null;
        string cmd1 = "", cmd2 = "", cmd3 = "";
        public bool bCycleMustFinish = false;
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
                }
            }
        }
        public EquipSetItem(object r)
        {
            if (r is GWProcTimeEqpTableRow)
            {
                GWProcTimeEqpTableRow Row = r as GWProcTimeEqpTableRow;
                ID = Row.ID;
                StartExcuteTime = Row.Time;
                ValueTimeSpan = Row.TimeDur;
                if (GetCMD(Row.equip_no, Row.set_no))
                {
                    if (string.IsNullOrEmpty(Row.value))
                    {
                        EqpSetItem = new SetItem(Row.equip_no, cmd1, cmd2, cmd3);
                    }
                    else
                    {
                        EqpSetItem = new SetItem(Row.equip_no, cmd1, cmd2, Row.value);
                    }
                    EqpSetItem.m_SetNo = Row.set_no;
                    EqpSetItem.Type = EqpSetItem.GetSetType();
                }
                Type = ItemType.TimePlan;
            }
            if (r is GWProcCycleTableRow)
            {
                GWProcCycleTableRow Row = r as GWProcCycleTableRow;
                ID = 0;
                if (GetCMD(Row.equip_no, Row.set_no))
                {
                    if (string.IsNullOrEmpty(Row.value))
                    {
                        EqpSetItem = new SetItem(Row.equip_no, cmd1, cmd2, cmd3);
                    }
                    else
                    {
                        EqpSetItem = new SetItem(Row.equip_no, cmd1, cmd2, Row.value);
                    }
                    EqpSetItem.m_SetNo = Row.set_no;
                    EqpSetItem.Type = EqpSetItem.GetSetType();
                }
                Type = ItemType.CyclePlan;
            }
        }
        bool GetCMD(int eqpno, int setno)
        {
            List<SetParmTableRow> Rows = StationItem.db_Setparm.Where(m => (m.equip_no == eqpno && m.sta_n == 1)).ToList();
            if (!Rows.Any())
                return false;
            else
            {
                foreach (SetParmTableRow r in Rows)
                {
                    if (r.set_no == setno)
                    {
                        cmd1 = r.main_instruction;
                        cmd2 = r.minor_instruction;
                        cmd3 = r.value;
                        return true;
                    }
                }
            }
            string msg = string.Format("定时任务出错:SetParm表中不存在设备号为{0},设置号为{1}的动作. ", eqpno, setno);
            string guid = Guid.NewGuid().ToString();
            MessageService.AddMessage(guid, MessageLevel.SetParm, msg, eqpno, false);
            MessageService.InsertMsg2SysEvt(msg, guid);
            return false;
        }
        public bool DoSetItem()
        {
            if (EqpSetItem != null)
            {
                EqpSetItem.sysExecutor = ResourceService.GetString("AlarmCenter.DataCenter.ProcTime") + "(system)";
                EquipItem EqpItem = DataCenter.GetEquipItem(EqpSetItem.EquipNo);
                if (EqpItem != null)
                {
                    if (Type == ItemType.TimePlan)
                    {
                        if (TimePlanItem.InSpanTime(StartExcuteTime, ValueTimeSpan))
                        {
                            IsExcute = true;
                            EqpItem.AddSetItem(EqpSetItem);
                            string msg = ResourceService.GetString("AlarmCenter.DataCenter.ProcTime") + ">>>" + EqpItem.Equip_nm + ":" + EqpSetItem.GetSetItemDesc();
                            string guid = Guid.NewGuid().ToString();
                            MessageService.AddMessage(guid, MessageLevel.SetParm, msg, EqpSetItem.EquipNo, false);
                            MessageService.InsertMsg2SysEvt(msg, guid);
                        }
                        else
                            return false;
                    }
                    if (Type == ItemType.CyclePlan)
                    {
                        if (CyclePlanItem.InValidTime(StartValidTime, EndValidTime) || bCycleMustFinish)
                        {
                            EqpItem.AddSetItem(EqpSetItem);
                            string msg = ResourceService.GetString("AlarmCenter.DataCenter.ProcTime") + ">>>" + EqpItem.Equip_nm + ":" + EqpSetItem.GetSetItemDesc();
                            string guid = Guid.NewGuid().ToString();
                            MessageService.AddMessage(guid, MessageLevel.SetParm, msg, EqpSetItem.EquipNo, false);
                            MessageService.InsertMsg2SysEvt(msg, guid);
                        }
                    }
                    else
                        return false;
                }
                return true;
            }
            return false;
        }
    }
}
