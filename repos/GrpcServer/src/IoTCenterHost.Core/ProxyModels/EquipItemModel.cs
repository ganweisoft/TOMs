//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using GWDataCenter;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace IoTCenterHost.Core
{
    public class EquipItemModel
    {
        public string communication_drv;
        public object EquipRWstate;
        public bool DoSetParm;
        public bool bCanMonitor;
        public bool bCommunicationOk;
        public bool bInitOk;
        public int iCommFaultRetryCount;
        public Queue SetItemQueue;
        public Dictionary<int, YXItemModel> YXItemDict;
        public Dictionary<int, YCItemModel> YCItemDict;
        public string PlanNo;
        public string ZiChanID;
        public object EquipResetLock;
        public string related_video;
        public string Reserve2;
        public string Reserve1;
        public int? AlarmRiseCycle;
        public int attrib;
        public string equip_detail;
        public string related_pic;
        public string restore_wave_file;
        public string wave_file;
        public string advice_Msg;
        public string RestorealarmMsg;
        public string alarmMsg;
        public string communication_time_param;
        public string communication_param;
        public string Reserve3;
        public bool DataFrash { get; set; }
        public int iAcc_cyc { get; }
        public int Alarm_scheme { get; }
        public int iEquipno { get; }
        public int iStano { get; }
        public Assembly Dll { get; }
        public string Equip_addr { get; }
        public EquipState State { get; set; }
        public string Related_pic { get; }
        public bool bCanConfirm2NormalState { get; set; }
        public bool IsDebug { get; set; }
        public object Reset { get; set; }
        public int iAcc_num { get; set; }
        public string Local_addr { get; }
        public string Equip_nm { get; set; }
        public bool? Bufang { get; set; }

        public event EquipValueFrashEventHandler ValueFrashed;
        public event EqpStateChangedEventHandler EqpStateChanged;
        public event EventHandler EquipCommError;
        public event EventHandler EquipCommOk;
        public event EventHandler EquipHaveAlarm;
        public event EventHandler EquipNoAlarm;

        public delegate void EqpStateChangedEventHandler(object sender, EventArgs e);
        public delegate void EquipValueFrashEventHandler(object sender, EventArgs e);
    }
}