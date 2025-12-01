using GWDataCenter.Database;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;

namespace GWDataCenter
{
    /// <summary>
    /// 设备动态库的基类，派生于IEquip
    /// </summary>
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

        bool bSetCacheData = false;//设备是否缓存标记，缓存意味着实时数据会记录到数据库，作为重启系统的初始数据
        bool bFirstGetData = true;//第一次调用GetData
        public bool RunSetParmFlag
        {
            get
            {
                lock(runSetParmFlag)
                {
                    return (bool)runSetParmFlag;
                }
            }
            set
            {
                lock(runSetParmFlag)
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
            equiprow= null;

            ycresults = new Dictionary<int, object>();
            yxresults = new Dictionary<int, object>();
            equipEventlist = new List<EquipEvent>();
            
            iRetrytime = 3;//默认通讯失败重试次数为3
        }

        /// <summary>
        /// 
        /// 在调用GetData 之前调用
        /// </summary>
        /// <param name="sta_no"></param>
        /// <param name="equip_no"></param>
        /// <param name="equip_addr"></param>
        /// <returns></returns>
        public virtual bool init(EquipItem item)
        {
            if (item == null)
            {
                GWDataCenter.DataCenter.WriteLogFile("CEquipBase调用init(EquipItem item)时，item==null)");
                return false;
            }
            iSta_no = item.iStano;
            iEquip_no = item.iEquipno;
            m_retrytime = serialport.CommFaultReTryTime;
            bSetCacheData = item.bSetCacheData;
            // 设备第一次运行时调用;
            //或者数据库配置进行了更改后调用
            if (b || ResetFlag)
            {
                myequipitem = item;
                equiprow = StationItem.db_Eqp.Where(m => m.equip_no == iEquip_no).FirstOrDefault();
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

                //        yxptable = db.GetDataTableOfYXP(iSta_no, iEquip_no);
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
                OnLoaded();
            }
            return true;
        }

        /// <summary>
        /// 休眠
        /// </summary>
        /// <param name="t">休眠时间，单位：毫秒</param>
        /// <param name="bBreak">当设置事件发生时，是否中断。在设置函数中休眠，不要中断。</param>
        public void Sleep(int t, bool bBreak = true)
        {
            if (!equipitem.IsRageMode)//非急速模式，正常休眠，避免海量线程场景下急剧的资源消耗（10000线程可能会卡死）
            {
                Thread.Sleep(t);
                return;
            }
            else//急速模式慎用，
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
        }
        /// <summary>
        /// 获取设备数据
        /// </summary>
        /// <returns></returns>

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
                        ///if have setparm, immediately return              
                        if (RunSetParmFlag)
                        {
                            return CommunicationState.setreturn;
                        }
                        else
                        {
                            if (pEquip.GetYC(r))
                            {
                            }
                            else
                            {
                                return CommunicationState.fail;
                            }
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
                        else
                        {
                            if (pEquip.GetYX(r))
                            {
                            }
                            else
                            {
                                return CommunicationState.fail;
                            }
                        }
                    }
                }
                if (!pEquip.GetEvent())
                    return CommunicationState.fail;
                if (bFirstGetData)
                    bFirstGetData = false;
            }
            catch (Exception e)
            {
                GWDataCenter.DataCenter.WriteLogFile(General.GetExceptionInfo(e));
                return CommunicationState.fail;
            }
            return CommunicationState.ok;
        }

        /// <summary>
        /// 加载完成处理函数，在第一次加载之后或者系统重置之后调用
        /// </summary>
        /// <param name="sender"></param>
        public virtual bool OnLoaded()
        {
            return true;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="MainInstruct"></param>
        /// <param name="MinorInstruct"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 设置YC点为未取值状态
        /// </summary>
        /// <param name="ycp"></param>
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
        /// <summary>
        /// 设置YX点为未取值状态
        /// </summary>
        /// <param name="yxp"></param>
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
        public void SetYcpTableRowData(YcpTableRow r, string o)
        {
            SetYCData(r, o);
        }
        public void SetYcpTableRowData(YcpTableRow r, int o)
        {
            SetYCData(r, o);
        }
        public void SetYcpTableRowData(YcpTableRow r, float o)
        {
            SetYCData(r, o);
        }

        public void SetYcpTableRowData(YcpTableRow r, double o)
        {
            SetYCData(r, o);
        }

        public void SetYcpTableRowData(YcpTableRow r, ValueTuple<DateTime, double> o)
        {
            SetYCData(r, o);
        }
        public void SetYcpTableRowData(YcpTableRow r, ValueTuple<double, double> o)
        {
            SetYCData(r, o);
        }
        public void SetYcpTableRowData(YcpTableRow r, ValueTuple<double, double, double> o)
        {
            SetYCData(r, o);
        }
        public void SetYcpTableRowData(YcpTableRow r, ValueTuple<double, double, double, double> o)
        {
            SetYCData(r, o);
        }
        public void SetYcpTableRowData(YcpTableRow r, ValueTuple<double, double, double, double, double> o)
        {
            SetYCData(r, o);
        }
        public void SetYcpTableRowData(YcpTableRow r, ValueTuple<double, double, double, double, double, double> o)
        {
            SetYCData(r, o);
        }
        public void SetYcpTableRowData(YcpTableRow r, ValueTuple<double, double, double, double, double, double, double> o)
        {
            SetYCData(r, o);
        }
        public void SetYCData(YcpTableRow r, object o)
        {
            int iycno = r.yc_no;
            lock (YCResults)
            {
                if (!YCResults.ContainsKey(iycno))
                    YCResults.Add(iycno, o);
                else
                    YCResults[iycno] = o;
            }
        }

        public object GetYCData(YcpTableRow r)
        {
            lock (YCResults)
            {
                int iycno = r.yc_no;
                if (YCResults.ContainsKey(iycno))
                {
                    return YCResults[iycno];
                }
                return null;
            }
        }

        public void SetYxpTableRowData(YxpTableRow r, bool o)
        {
            SetYXData(r, o);
        }
        public void SetYxpTableRowData(YxpTableRow r, string o)
        {
            SetYXData(r, o);
        }

        public void SetYXData(YxpTableRow r, object o)
        {
            int iyxno = r.yx_no;
            lock (YXResults)
            {
                if (!YXResults.ContainsKey(iyxno))
                    YXResults.Add(iyxno, o);
                else
                    YXResults[iyxno] = o;
            }
        }

        public object GetYXData(YxpTableRow r)
        {
            lock (YXResults)
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
}
