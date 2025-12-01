﻿// Copyright (c) 2004-2025  Beijing TOMs Software Technology Co., Ltd
using GWDataCenter;
using GWDataCenter.Database;
namespace OpenGWDataCenter.Model
{
    public enum YCAlarmStateType
    {
        NoAlarm, Alarm01, Alarm10, Restore
    }
    public class YcpAlarmStateTrack
    {
        YCItem ycitem;
        YcpTableRow db;
        YCAlarmStateType state = YCAlarmStateType.NoAlarm;
        public bool? state4EquipmentLinkage = null;
        int Alarm_scheme = 0;
        public YcpAlarmStateTrack(YcpTableRow d, YCItem item)
        {
            ycitem = item;
            db = d;
        }
        public bool IsDifferentState4EquipmentLinkage()
        {
            if (ycitem.OriginalAlarmState != state4EquipmentLinkage)
            {
                return true;
            }
            return false;
        }
        public void SetState4EquipmentLinkage()
        {
            state4EquipmentLinkage = ycitem.OriginalAlarmState;
        }
        public bool IsDifferentState(int AlarmCode)
        {
            YCAlarmStateType type = GetAlarmStateType();
            if (type != state)
            {
                Alarm_scheme = 0;
                state = type;
                Alarm_scheme = Alarm_scheme | AlarmCode;
                return true;
            }
            else
            {
                if ((Alarm_scheme & AlarmCode) == 0)
                {
                    state = type;
                    Alarm_scheme = Alarm_scheme | AlarmCode;
                    return true;
                }
            }
            return false;
        }
        public YCAlarmStateType GetAlarmStateType()
        {
            if (ycitem.IsAlarm)
            {
                if (ycitem.alarmflag == "0-1")
                    return YCAlarmStateType.Alarm01;
                if (ycitem.alarmflag == "1-0")
                    return YCAlarmStateType.Alarm10;
            }
            if (ycitem.RestoreAlarmState)
                return YCAlarmStateType.Restore;
            return YCAlarmStateType.NoAlarm;
        }
    }
}
