﻿// Copyright (c) 2004-2025  Beijing TOMs Software Technology Co., Ltd
using GWDataCenter.Database;
using OpenGWDataCenter.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
namespace GWDataCenter
{
    public class CEquipBase : IEquip
    {
        int iSta_no, iEquip_no, iRetrytime;
        bool b = true;
        EquipItem myequipitem;
        public List<YcpTableRow> ycprows;
        public List<YxpTableRow> yxprows;
        public EquipTableRow equiprow;
        public string EquipNm;
        public SerialPort serialport;
        Dictionary<int, object> ycresults;
        Dictionary<int, object> yxresults;
        List<EquipEvent> equipEventlist;
        object runSetParmFlag = (object)false;
        object resetFlag = (object)false;
        public Dictionary<int, bool> ycpdataflag = new Dictionary<int, bool>();
        public Dictionary<int, bool> yxpdataflag = new Dictionary<int, bool>();
        public string setparmexecutor = null;
        public bool RunSetParmFlag
        {
            get
            {
                lock (runSetParmFlag)
                {
                    return (bool)runSetParmFlag;
                }
            }
            set
            {
                lock (runSetParmFlag)
                {
                    runSetParmFlag = (object)value;
                }
            }
        }
        public bool ResetFlag
        {
            get
            {
                lock (resetFlag)
                {
                    return (bool)resetFlag;
                }
            }
            set
            {
                lock (resetFlag)
                {
                    resetFlag = (object)value;
                }
            }
        }
        public EquipItem equipitem
        {
            get
            {
                return myequipitem;
            }
            set
            {
                myequipitem = value;
                serialport = myequipitem.serialport;
            }
        }
        public EquipItem Equipitem
        {
            get
            {
                return DataCenter.GetEquipItem(iEquip_no);
            }
        }
        public Dictionary<int, object> YCResults
        {
            get
            {
                return ycresults;
            }
        }
        public Dictionary<int, object> YXResults
        {
            get
            {
                return yxresults;
            }
        }
        public List<EquipEvent> EquipEventList
        {
            get
            {
                return equipEventlist;
            }
        }
        public int m_sta_no
        {
            get
            {
                return iSta_no;
            }
            set
            {
                iSta_no = value;
            }
        }
        public int m_equip_no
        {
            get
            {
                return iEquip_no;
            }
            set
            {
                iEquip_no = value;
            }
        }
        public int m_retrytime
        {
            get
            {
                return iRetrytime;
            }
            set
            {
                iRetrytime = value;
            }
        }
        public bool bCanConfirm2NormalState
        {
            get
            {
                return Equipitem.bCanConfirm2NormalState;
            }
            set
            {
                Equipitem.bCanConfirm2NormalState = value;
            }
        }
        public string SetParmExecutor
        {
            get
            {
                return setparmexecutor;
            }
            set
            {
                setparmexecutor = value;
            }
        }
        public CEquipBase()
        {
            ycresults = null;
            yxresults = null;
            equiprow = null;
            ycresults = new Dictionary<int, object>();
            yxresults = new Dictionary<int, object>();
            equipEventlist = new List<EquipEvent>();
            iRetrytime = 3;
        }
        public virtual bool init(EquipItem item)
        {
            iSta_no = item.iStano;
            iEquip_no = item.iEquipno;
            m_retrytime = serialport.CommFaultReTryTime;
            if (b || ResetFlag)
            {
                if (ResetFlag)
                    ResetFlag = false;
                equiprow = StationItem.db_Eqp.Where(m => m.equip_no == iEquip_no).First();
                if (equiprow == null)
                    return false;
                List<YcpTableRow> rs = StationItem.db_Ycp.Where(m => m.equip_no == iEquip_no).ToList();
                if (rs.Any())
                {
                    ycprows = rs;
                }
                else
                {
                    ycprows = null;
                }
                List<YxpTableRow> rs1 = StationItem.db_Yxp.Where(m => m.equip_no == iEquip_no).ToList();
                if (rs1.Any())
                {
                    yxprows = rs1;
                }
                else
                {
                    yxprows = null;
                }
                EquipNm = equiprow.equip_nm;
                b = false;
            }
            return true;
        }
        public void Sleep(int t, bool bBreak = true)
        {
            if (!bBreak)
            {
                Thread.Sleep(t);
                return;
            }
            int count = t / 10;
            for (int k = 0; k < count; k++)
            {
                if (RunSetParmFlag)
                    break;
                Thread.Sleep(10);
            }
        }
        public virtual CommunicationState GetData(CEquipBase pEquip)
        {
            try
            {
                if (RunSetParmFlag)
                {
                    return CommunicationState.setreturn;
                }
                if (ycprows != null)
                {
                    foreach (YcpTableRow r in ycprows)
                    {
                        if (RunSetParmFlag)
                        {
                            return CommunicationState.setreturn;
                        }
                        if (pEquip.GetYC(r))
                        {
                        }
                        else
                        {
                            return CommunicationState.fail;
                        }
                    }
                }
                if (yxprows != null)
                {
                    foreach (YxpTableRow r in yxprows)
                    {
                        if (RunSetParmFlag)
                        {
                            return CommunicationState.setreturn;
                        }
                        if (pEquip.GetYX(r))
                        {
                        }
                        else
                        {
                            return CommunicationState.fail;
                        }
                    }
                }
                if (!pEquip.GetEvent())
                    return CommunicationState.fail;
            }
            catch (Exception e)
            {
                MessageService.AddMessage(MessageLevel.Debug, General.GetExceptionInfo(e), m_equip_no, false);
                return CommunicationState.fail;
            }
            return CommunicationState.ok;
        }
        public virtual bool GetYC(YcpTableRow r)
        {
            return false;
        }
        public virtual bool GetYX(YxpTableRow r)
        {
            return false;
        }
        public virtual bool GetEvent()
        {
            return true;
        }
        public virtual bool SetParm(string MainInstruct, string MinorInstruct, string Value)
        {
            return false;
        }
        public virtual bool Confirm2NormalState(string sYcYxType, int iYcYxNo)
        {
            return false;
        }
        public virtual bool CloseCommunication()
        {
            return true;
        }
        public void YCToPhysic(YcpTableRow r)
        {
            return;
        }
        public void YXToPhysic(YxpTableRow r)
        {
            return;
        }
        public void SetYCDataNoRead(IQueryable<YcpTableRow> Rows)
        {
            if (Rows == null)
                return;
            foreach (YcpTableRow r in Rows)
            {
                int iycno = r.yc_no;
                if (!ycpdataflag.ContainsKey(iycno))
                {
                    ycpdataflag.Add(iycno, false);
                }
                else
                    ycpdataflag[iycno] = false;
            }
        }
        public void SetYXDataNoRead(IQueryable<YxpTableRow> Rows)
        {
            if (Rows == null)
                return;
            foreach (YxpTableRow r in Rows)
            {
                int iyxno = r.yx_no;
                if (!yxpdataflag.ContainsKey(iyxno))
                {
                    yxpdataflag.Add(iyxno, false);
                }
                else
                    yxpdataflag[iyxno] = false;
            }
        }
        public void SetYCData(YcpTableRow r, object o)
        {
            lock (YCResults)
            {
                int iycno = r.yc_no;
                if (!YCResults.ContainsKey(iycno))
                    YCResults.Add(iycno, o);
                else
                    YCResults[iycno] = o;
            }
        }
        public object GetYCData(YcpTableRow r)
        {
            int iycno = r.yc_no;
            if (YCResults.ContainsKey(iycno))
            {
                return YCResults[iycno];
            }
            return null;
        }
        public void SetYXData(YxpTableRow r, object o)
        {
            lock (YXResults)
            {
                int iyxno = r.yx_no;
                if (!YXResults.ContainsKey(iyxno))
                    YXResults.Add(iyxno, o);
                else
                    YXResults[iyxno] = o;
            }
        }
        public object GetYXData(YxpTableRow r)
        {
            int iyxno = r.yx_no;
            if (YXResults.ContainsKey(iyxno))
            {
                return YXResults[iyxno];
            }
            return null;
        }
    }
}
