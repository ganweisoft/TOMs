using GWDataCenter.Database;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GWDataCenter
{
    public class SetItem
    {
        int equipno;
        string type;
        string strMainInstruct, strMinorInstruct, strValue;
        string executor;
        int waitingTime=0;//等待时间,比如联动的滞后,单位：毫秒
        int startTickCount;
        public bool bRecord;

 //       public bool bVerifyLimits = true;//是否验证权限
        public bool bCanRepeat = false;//是否允许设置项在执行队列中可以重复出现
        public bool bShowDlg = true;//是否弹出设置是否成功的确认框

        public string Client_Instance_GUID = null;//发起设置的客户端的GUID值
        public string Description;
        public bool IsCj=false;//该命令是否属于场景
        public bool IsWaitSetParm = false;//该命令是否等待返回的类型
        bool? waitSetParmIsFinish = null;//等待返回的设置是否完成
        object oo = true;

        public bool isQRTrigger = false;//设置是否由二维码扫描触发，默认为false

        public string RequestId = null;//请求命令的ID。第三方平台需要返回控制结果，往往会带入请求ID，比如华为OC平台
        public string csResponse { get; set; }//当前设置项的响应--用于当某些设置有返回信息的情景
        public bool isSynchronization = false;//命令是同步执行还是异步执行，默认是异步
        public bool? WaitSetParmIsFinish//等待返回的设置是否完成
        {
            get
            {
                lock(oo)
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
        public bool bStopSetParm//中断当前设置，比如某个场景设置时间较长，用户可能中断操作
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
        public string sysExecutor = "";//系统内部执行者，比如“联动”、“定时任务”等
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
            set
            {
                strValue = value;
            }
        }

        private object _lock = new object();
        private bool enable = true;
        /// <summary>
        /// 指示设置是否允许运行
        /// </summary>
        public bool Enable
        {
            get
            {
                lock (_lock)
                {
                    return enable;
                }
            }
            set
            {
                lock (_lock)
                {
                    enable = value;
                }
            }
        }

        public string set_code;

        public void GetEnableState()
        {
            GetSetNo();
            string strEnable = "True";
            if (m_SetNo != -1 && DataCenter.GetPropertyFromReserve(out strEnable, "setparm", "Reserve1", EquipNo, m_SetNo, "GWDataCenter.dll#EnableSetParm"))
            {
                if (strEnable.ToLower() == "true")
                    Enable = true;
                else if (strEnable.ToLower() == "false")
                    Enable = false;
            }
            GetSetCode();
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
                DateTime StartDoTime = DateTime.Now;//开始执行的时间
                while (((DateTime.Now - StartDoTime).TotalMilliseconds < iDelayTime))
                {
                    if (bStopSetParm)
                        return;
                    System.Threading.Thread.Sleep(1);
                }
            }
        }

        /// <summary>
        /// only 延时动作
        /// </summary>
        /// <param name="iTime"></param>
        public SetItem(int iTime)
        {
            bOnlyDelayType = true;
            iDelayTime = iTime;
            GetEnableState();
        }

        public SetItem(int Eqpno, string MainInstruct, string MinorInstruct, string Value/*, bool VerifyLimits=true*/)
        {
 //           bVerifyLimits = VerifyLimits;
            equipno = Eqpno;
            strMainInstruct = MainInstruct;
            strMinorInstruct = MinorInstruct;
            strValue = Value;
            type = GetSetType();
            GetRecord();
            GetEnableState();
        }

        public SetItem(int Eqpno, string MainInstruct, string MinorInstruct, string Value, string Type, string myexecutor)
        {
            //           bVerifyLimits = VerifyLimits;
            equipno = Eqpno;
            strMainInstruct = MainInstruct;
            strMinorInstruct = MinorInstruct;
            strValue = Value;
            type = Type;
            executor = myexecutor;
            GetRecord();
            GetEnableState();
        }

        /// <summary>
        /// </summary>
        /// <param name="Eqpno"></param>
        /// <param name="MainInstruct"></param>
        /// <param name="MinorInstruct"></param>
        /// <param name="Value"></param>
        /// <param name="myexecutor">命令执行人，应该根据登录用户的权限来决定是否生成下发命令</param>
        public SetItem(int Eqpno, string MainInstruct, string MinorInstruct, string Value, string myexecutor/*, bool VerifyLimits = true*/, bool CanRepeat = false)
        {
            bCanRepeat = CanRepeat;
   //         bVerifyLimits = VerifyLimits;
            equipno = Eqpno;
            strMainInstruct = MainInstruct;
            strMinorInstruct = MinorInstruct;
            strValue = Value;
            executor = myexecutor;
            type = GetSetType();
            GetRecord();
            GetEnableState();
        }

        public SetItem(int Eqpno, int Setno, string MainInstruct, string MinorInstruct, string Value, string myexecutor/*, bool VerifyLimits = true*/, bool CanRepeat = false)
        {
            bCanRepeat = CanRepeat;
            //         bVerifyLimits = VerifyLimits;
            equipno = Eqpno;
            m_SetNo = Setno;
            strMainInstruct = MainInstruct;
            strMinorInstruct = MinorInstruct;
            strValue = Value;
            executor = myexecutor;
            type = GetSetType();
            GetRecord();
            GetEnableState();
        }

        public SetItem(int Eqpno, int Setno, string Value, string myexecutor/*, bool VerifyLimits = true*/, bool CanRepeat = false)
        {
            bCanRepeat = CanRepeat;
            equipno = Eqpno;
            m_SetNo = Setno;

            var r = StationItem.db_Setparm.Single(m => (m.equip_no == Eqpno && m.set_no == Setno));

            strMainInstruct = r.main_instruction;
            strMinorInstruct = r.minor_instruction;
            strValue = string.IsNullOrWhiteSpace(Value) ? r.value : Value;

            executor = myexecutor;
            type = GetSetType();
            GetRecord();
            GetEnableState();
        }

        public string GetSetItemDesc()
        {
            string strSetNm = "";
            string s_strMainInstruct, s_strMinorInstruct, s_strValue;
            if(strMainInstruct==null)
                s_strMainInstruct = null;
            else if (strMainInstruct == "")
                s_strMainInstruct = "";
            else
                s_strMainInstruct = strMainInstruct;

            if (strMinorInstruct == null)
                s_strMinorInstruct = null;
            else if (strMinorInstruct == "")
                s_strMinorInstruct = "";
            else
                s_strMinorInstruct = strMinorInstruct;

            if (strValue == null)
                s_strValue = null;
            else if (strValue == "")
                s_strValue = "";
            else
                s_strValue = strValue;
            var query = StationItem.db_Setparm.Where(e => e.equip_no == equipno
                            && (e.main_instruction == s_strMainInstruct)
                            && (e.minor_instruction == s_strMinorInstruct)
                            && (e.value == s_strValue));

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


        /// <summary>
        /// 获取该命令在setparm表中对应的set_no
        /// </summary>
        /// <returns></returns>
        public int GetSetNo()
        {
            if (m_SetNo != -1)
                return m_SetNo;
            string s_strMainInstruct, s_strMinorInstruct, s_strValue;
            if (strMainInstruct == null)
                s_strMainInstruct = null;
            else if (strMainInstruct == "")
                s_strMainInstruct = "";
            else
                s_strMainInstruct = strMainInstruct;

            if (strMinorInstruct == null)
                s_strMinorInstruct = null;
            else if (strMinorInstruct == "")
                s_strMinorInstruct = "";
            else
                s_strMinorInstruct = strMinorInstruct;

            if (strValue == null)
                s_strValue = null;
            else if (strValue == "")
                s_strValue = "";
            else
                s_strValue = strValue;

            var query = StationItem.db_Setparm.Where(e => e.equip_no == equipno
                            && (e.main_instruction == s_strMainInstruct)
                            && (e.minor_instruction == s_strMinorInstruct)
                            && (e.value == s_strValue));

            if (query.ToList().Count == 0)
                return -1;
            else
            {
                return query.ToList()[0].set_no;
            }
        }

        /// <summary>
        /// 获取该命令在setparm表中对应的set_code
        /// </summary>
        /// <returns></returns>
        public void GetSetCode()
        {
            var query = StationItem.db_Setparm.Where(e => e.equip_no == equipno && e.set_no == m_SetNo);

            if (query.ToList().Count == 0)
                set_code = string.Empty;
            else
            {
                set_code = query.ToList()[0].set_code;
            }
        }



        /// <summary>
        /// 获取该命令在setparm表中对应的record状态,以及备注字段信息
        /// </summary>
        /// <returns></returns>
        public void GetRecord()
        {
            IEnumerable<SetParmTableRow> result;
            if(m_SetNo != -1)
            {
                result = StationItem.db_Setparm.Where(e => e.equip_no == equipno && e.set_no == m_SetNo);
            }
            else
            {
                string s_strMainInstruct, s_strMinorInstruct, s_strValue;
                if (strMainInstruct == null)
                    s_strMainInstruct = null;
                else if (strMainInstruct == "")
                    s_strMainInstruct = "";
                else
                    s_strMainInstruct = strMainInstruct;

                if (strMinorInstruct == null)
                    s_strMinorInstruct = null;
                else if (strMinorInstruct == "")
                    s_strMinorInstruct = "";
                else
                    s_strMinorInstruct = strMinorInstruct;

                if (strValue == null)
                    s_strValue = null;
                else if (strValue == "")
                    s_strValue = "";
                else
                    s_strValue = strValue;

                result = StationItem.db_Setparm.Where(e => e.equip_no == equipno
                                && (e.main_instruction == s_strMainInstruct)
                                && (e.minor_instruction == s_strMinorInstruct)
                                && (e.value == s_strValue));
            }


            if (result.ToList().Count == 0)
                bRecord = true;
            else
            {
                bRecord = result.ToList()[0].record;
            }

        }

        public string GetSetType()
        {
            try
            {
                if (m_SetNo == -1)
                {
                    string s_strMainInstruct, s_strMinorInstruct, s_strValue;
                    if (strMainInstruct == null)
                        s_strMainInstruct = null;
                    else if (strMainInstruct == "")
                        s_strMainInstruct = "";
                    else
                        s_strMainInstruct = strMainInstruct;

                    if (strMinorInstruct == null)
                        s_strMinorInstruct = null;
                    else if (strMinorInstruct == "")
                        s_strMinorInstruct = "";
                    else
                        s_strMinorInstruct = strMinorInstruct;

                    if (strValue == null)
                        s_strValue = null;
                    else if (strValue == "")
                        s_strValue = "";
                    else
                        s_strValue = strValue;
                    var query = StationItem.db_Setparm.Where(e => e.equip_no == equipno
                                && (e.main_instruction == s_strMainInstruct)
                                && (e.minor_instruction == s_strMinorInstruct)
                                && (e.value == s_strValue));

                    if (query.ToList().Count == 0)
                    {
                        DataCenter.WriteLogFile("set_type of SetParm is null");
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
                        DataCenter.WriteLogFile($"SetParm 不存在equip_no={equipno} set_no={m_SetNo}的对应项");
                        return null;
                    }
                    else
                    {
                        //顺带获取备注字段的信息 2020/5/24
                        csreserve1 = query.ToList()[0].Reserve1;
                        csreserve2 = query.ToList()[0].Reserve2;
                        csreserve3 = query.ToList()[0].Reserve3;
                        return query.ToList()[0].set_type;
                    }
                }
            }
            catch(Exception e)
            {
                return null;
            }
        }

        public override string ToString()
        {
            return base.ToString();
        }

    }
}
