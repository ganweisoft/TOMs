﻿// Copyright (c) 2004-2025  Beijing TOMs Software Technology Co., Ltd
using GWDataCenter.Database;
namespace OpenGWDataCenter.Model
{
    public enum YXAlarmStateType
    {
        NoAlarm, Alarm01, Alarm10, Restore01, Restore10
    }
    public class YxpAlarmStateTrack
    {
        YXItem yxitem;
        YxpTableRow db;
        YXAlarmStateType state = YXAlarmStateType.NoAlarm;
        public bool? state4EquipmentLinkage = null;
        int Alarm_scheme = 0;
        public YxpAlarmStateTrack(YxpTableRow d, YXItem item)
        {
            yxitem = item;
            db = d;
        }
        public bool IsDifferentState4EquipmentLinkage()
        {
            if (yxitem.isAllZeroLevel())
                return false;
            if (yxitem.OriginalAlarmState != state4EquipmentLinkage)
            {
                return true;
            }
            return false;
        }
        public void SetState4EquipmentLinkage()
        {
            state4EquipmentLinkage = yxitem.OriginalAlarmState;
        }
        public bool IsDifferentState(int AlarmCode)
        {
            YXAlarmStateType type = GetAlarmStateType();
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
        public YXAlarmStateType GetAlarmStateType()
        {
            if (yxitem.IsAlarm)
            {
                if (yxitem.alarmflag == "0-1")
                    return YXAlarmStateType.Alarm01;
                if (yxitem.alarmflag == "1-0")
                    return YXAlarmStateType.Alarm10;
            }
            else
            {
                if (yxitem.alarmflag == "0-1")
                    return YXAlarmStateType.Restore01;
                if (yxitem.alarmflag == "1-0")
                    return YXAlarmStateType.Restore10;
            }
            return YXAlarmStateType.NoAlarm;
        }
    }
}
