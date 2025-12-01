﻿// Copyright (c) 2004-2025  Beijing TOMs Software Technology Co., Ltd
using GWDataCenter.Database;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
namespace GWDataCenter
{
    public class SetItem
    {
        int equipno;
        string type;
        string strMainInstruct, strMinorInstruct, strValue;
        string executor;
        int waitingTime = 0;
        int startTickCount;
        public bool bRecord;
        public bool bCanRepeat = false;
        public bool bShowDlg = true;
        public string Client_Instance_GUID = null;
        public string Description;
        public bool IsCj = false;
        public bool IsWaitSetParm = false;
        bool? waitSetParmIsFinish = null;
        object oo = true;
        public string RequestId = null;
        public bool? WaitSetParmIsFinish
        {
            get
            {
                lock (oo)
                {
                    return waitSetParmIsFinish;
                }
            }
            set
            {
                lock (oo)
                {
                    waitSetParmIsFinish = value;
                }
            }
        }
        bool bstopsetparm = false;
        public bool bStopSetParm
        {
            get
            {
                lock (oo)
                {
                    return bstopsetparm;
                }
            }
            set
            {
                lock (oo)
                {
                    bstopsetparm = value;
                }
            }
        }
        public int m_SetNo = -1;
        public int CJ_EqpNo = -1, CJ_SetNo = -1;
        public string Executor
        {
            get
            {
                return executor;
            }
            set
            {
                executor = value;
            }
        }
        public string sysExecutor = "";
        public int EquipNo
        {
            get
            {
                return equipno;
            }
        }
        public int WaitingTime
        {
            get
            {
                return waitingTime;
            }
            set
            {
                waitingTime = value;
            }
        }
        public int StartTickCount
        {
            get
            {
                return startTickCount;
            }
            set
            {
                startTickCount = value;
            }
        }
        public string MainInstruct
        {
            get
            {
                return strMainInstruct;
            }
        }
        public string MinorInstruct
        {
            get
            {
                return strMinorInstruct;
            }
        }
        public string Type
        {
            get
            {
                return type;
            }
            set
            {
                type = value;
            }
        }
        public string Value
        {
            get
            {
                return strValue;
            }
        }
        public bool bOnlyDelayType = false;
        public int iDelayTime = 0;
        public string UserIPandPort { get => userIPandPort; set => userIPandPort = value; }
        private string userIPandPort = "";
        public string csReserve1 { get => csreserve1; set => csreserve1 = value; }
        private string csreserve1 = "";
        public string csReserve2 { get => csreserve2; set => csreserve2 = value; }
        private string csreserve2 = "";
        public string csReserve3 { get => csreserve3; set => csreserve3 = value; }
        private string csreserve3 = "";
        public void DoDelay()
        {
            if (bOnlyDelayType)
            {
                DateTime StartDoTime = DateTime.Now;
                while (((DateTime.Now - StartDoTime).TotalMilliseconds < iDelayTime))
                {
                    if (bStopSetParm)
                        return;
                    System.Threading.Thread.Sleep(1);
                }
            }
        }
        public SetItem(int iTime)
        {
            bOnlyDelayType = true;
            iDelayTime = iTime;
            DoEvent();
        }
        public SetItem(int Eqpno, string MainInstruct, string MinorInstruct, string Value)
        {
            equipno = Eqpno;
            strMainInstruct = MainInstruct;
            strMinorInstruct = MinorInstruct;
            strValue = Value;
            type = GetSetType();
            GetRecord();
            DoEvent();
        }
        public SetItem(int Eqpno, string MainInstruct, string MinorInstruct, string Value, string Type, string myexecutor)
        {
            equipno = Eqpno;
            strMainInstruct = MainInstruct;
            strMinorInstruct = MinorInstruct;
            strValue = Value;
            type = Type;
            executor = myexecutor;
            GetRecord();
            DoEvent();
        }
        public SetItem(int Eqpno, string MainInstruct, string MinorInstruct, string Value, string myexecutor, bool CanRepeat = false)
        {
            bCanRepeat = CanRepeat;
            equipno = Eqpno;
            strMainInstruct = MainInstruct;
            strMinorInstruct = MinorInstruct;
            strValue = Value;
            executor = myexecutor;
            type = GetSetType();
            GetRecord();
            DoEvent();
        }
        public SetItem(int Eqpno, int Setno, string MainInstruct, string MinorInstruct, string Value, string myexecutor, bool CanRepeat = false)
        {
            bCanRepeat = CanRepeat;
            equipno = Eqpno;
            m_SetNo = Setno;
            strMainInstruct = MainInstruct;
            strMinorInstruct = MinorInstruct;
            strValue = Value;
            executor = myexecutor;
            type = GetSetType();
            GetRecord();
            DoEvent();
        }
        public SetItem(int Eqpno, int Setno, string Value, string myexecutor, bool CanRepeat = false)
        {
            bCanRepeat = CanRepeat;
            equipno = Eqpno;
            m_SetNo = Setno;
            lock (GWDbProvider.lockstate)
            {
                using (var db = StationItem.MyGWDbProvider.serviceProvider.GetService<GWDataContext>())
                {
                    var r = db.SetParmTable.Single(m => (m.equip_no == Eqpno && m.set_no == Setno));
                    strMainInstruct = r.main_instruction;
                    strMinorInstruct = r.minor_instruction;
                    strValue = string.IsNullOrWhiteSpace(Value) ? r.value : Value;
                }
            }
            executor = myexecutor;
            type = GetSetType();
            GetRecord();
            DoEvent();
        }
        public string GetSetItemDesc()
        {
            string strSetNm = "";
            var query = StationItem.db_Setparm.Where(e => e.equip_no == equipno
                            && (string.IsNullOrEmpty(strMainInstruct) ? e.main_instruction == null : e.main_instruction == strMainInstruct)
                            && (string.IsNullOrEmpty(strMinorInstruct) ? e.minor_instruction == null : e.minor_instruction == strMinorInstruct)
                            && (string.IsNullOrEmpty(strValue) ? e.value == null : e.value == strValue));
            if (query.ToList().Count > 0)
            {
                strSetNm = query.ToList()[0].set_nm;
            }
            else
            {
                var query1 = StationItem.db_Setparm.Where(e => (e.equip_no == equipno && e.set_no == m_SetNo));
                if (query1.ToList().Count > 0)
                {
                    strSetNm = query1.ToList()[0].set_nm;
                }
            }
            if (strSetNm == "")
                return null;
            string eqpnm = "";
            try
            {
                eqpnm = StationItem.GetEquipItemFromEquipNo(EquipNo).Equip_nm;
            }
            catch (Exception e)
            {
            }
            return eqpnm + "-" + strSetNm;
        }
        public int GetSetNo()
        {
            if (m_SetNo != -1)
                return m_SetNo;
            var query = StationItem.db_Setparm.Where(e => e.equip_no == equipno
                            && (string.IsNullOrEmpty(strMainInstruct) ? e.main_instruction == null : e.main_instruction == strMainInstruct)
                            && (string.IsNullOrEmpty(strMinorInstruct) ? e.minor_instruction == null : e.minor_instruction == strMinorInstruct)
                            && (string.IsNullOrEmpty(strValue) ? e.value == null : e.value == strValue));
            if (query.ToList().Count == 0)
                return -1;
            else
            {
                return query.ToList()[0].set_no;
            }
        }
        public void GetRecord()
        {
            var query = StationItem.db_Setparm.Where(e => e.equip_no == equipno
                            && (string.IsNullOrEmpty(strMainInstruct) ? e.main_instruction == null : e.main_instruction == strMainInstruct)
                            && (string.IsNullOrEmpty(strMinorInstruct) ? e.minor_instruction == null : e.minor_instruction == strMinorInstruct)
                            && (string.IsNullOrEmpty(strValue) ? e.value == null : e.value == strValue));
            if (query.ToList().Count == 0)
                bRecord = true;
            else
            {
                bRecord = query.ToList()[0].record;
            }
        }
        public string GetSetType()
        {
            try
            {
                if (m_SetNo == -1)
                {
                    var query = StationItem.db_Setparm.Where(e => e.equip_no == equipno
                                && (string.IsNullOrEmpty(strMainInstruct) ? e.main_instruction == null : e.main_instruction == strMainInstruct)
                                && (string.IsNullOrEmpty(strMinorInstruct) ? e.minor_instruction == null : e.minor_instruction == strMinorInstruct));
                    if (query.ToList().Count == 0)
                    {
                        MessageService.AddMessage(MessageLevel.Fatal, "set_type of SetParm is null", this.equipno, false);
                        return null;
                    }
                    else
                    {
                        return query.ToList()[0].set_type;
                    }
                }
                else
                {
                    var query = StationItem.db_Setparm.Where(e => (e.equip_no == equipno && e.set_no == m_SetNo));
                    if (query.ToList().Count == 0)
                    {
                        MessageService.AddMessage(MessageLevel.Fatal, $"SetParm 不存在equip_no={equipno} set_no={m_SetNo}的对应项", this.equipno, false);
                        return null;
                    }
                    else
                    {
                        csreserve1 = query.ToList()[0].Reserve1;
                        csreserve2 = query.ToList()[0].Reserve2;
                        csreserve3 = query.ToList()[0].Reserve3;
                        return query.ToList()[0].set_type;
                    }
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public override string ToString()
        {
            return base.ToString();
        }
        void DoEvent()
        {
            StationItem.StopSetParmEvent -= StationItem_StopSetParmEvent;
            StationItem.StopSetParmEvent += StationItem_StopSetParmEvent;
        }
        public void DisposeEvent()
        {
            StationItem.StopSetParmEvent -= StationItem_StopSetParmEvent;
        }
        private void StationItem_StopSetParmEvent(object sender, EventArgs e)
        {
            StopSetParmEventArgs Args = (StopSetParmEventArgs)e;
            if (Args.iEqpNo == equipno && Args.iSetNo == m_SetNo)
            {
                bStopSetParm = true;
            }
            if (Args.iEqpNo == CJ_EqpNo && Args.iSetNo == CJ_SetNo)
            {
                bStopSetParm = true;
            }
        }
    }
}
