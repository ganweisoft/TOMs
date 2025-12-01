﻿// Copyright (c) 2004-2025  Beijing TOMs Software Technology Co., Ltd
using OpenGWDataCenter.Model;
using System;
using System.Collections.Generic;
namespace GWDataCenter
{
    public enum CommunicationState
    {
        fail,
        ok,
        setreturn,
        retry
    }
    public class EquipEvent
    {
        public string msg;
        public MessageLevel level;
        public DateTime dt;
        public EquipEvent(string Msg, MessageLevel Level, DateTime Dt)
        {
            msg = Msg;
            level = Level;
            dt = Dt;
        }
    }
    public interface IEquip
    {
        int m_sta_no
        {
            get;
            set;
        }
        int m_equip_no
        {
            get;
            set;
        }
        int m_retrytime
        {
            get;
            set;
        }
        Dictionary<int, object> YCResults
        {
            get;
        }
        Dictionary<int, object> YXResults
        {
            get;
        }
        List<EquipEvent> EquipEventList
        {
            get;
        }
        bool RunSetParmFlag
        {
            get;
            set;
        }
        bool ResetFlag
        {
            get;
            set;
        }
        EquipItem equipitem
        {
            get;
            set;
        }
        bool bCanConfirm2NormalState
        {
            get;
            set;
        }
        string SetParmExecutor
        {
            get;
            set;
        }
        bool init(EquipItem eqpitm);
        CommunicationState GetData(CEquipBase p);
        bool SetParm(string cmd1, string cmd2, string value);
        bool Confirm2NormalState(string sYcYxType, int iYcYxNo);
        bool CloseCommunication();
    }
}
