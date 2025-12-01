using GWDataCenter.Database;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace GWDataCenter
{
    public class SafeTimeSpan
    {
        public TimeSpan tStart;
        public TimeSpan tEnd;
        public SafeTimeSpan(TimeSpan t, TimeSpan t1)
        {
            tStart = t;
            tEnd = t1;
        }
    }

    public class NoSetItemPermissionEventArgs : EventArgs
    {
        public string strGUID { get; set; }
    }

    public class EquipItem : ICanReset, IComparable
    {
        int istano;
        object iequipno;
        int iacc_cyc, iacc_num, alarm_scheme;
        string equip_nm;
        string local_addr, equip_addr;
        public string communication_drv, communication_param, communication_time_param;
        public string alarmMsg, RestorealarmMsg, advice_Msg, wave_file, restore_wave_file, related_pic, equip_detail;
        public int attrib;
        public int? AlarmRiseCycle;//报警升级周期
        public string Reserve1;//保留字段1
        public string Reserve2;//保留字段2
        public string Reserve3;//保留字段3
        public string related_video, ZiChanID, PlanNo;
        public List<SafeTimeSpan> SafeTimeSpanList = new List<SafeTimeSpan>();

        Assembly dll;
        IEquip icommunication;
        public CEquipBase EquipBase { get; set; }
        public SetItem curSetItem { get; set; }//当前正在设置中的设置项
        object datafrash = (object)false;

        object state;

        public ConcurrentQueue<SetItem> SetItemQueue = new ConcurrentQueue<SetItem>();
        public Queue OffLineSetItemQueue = new Queue();

        [Obsolete("弃用，不要响应这个事件，会频繁调用，无意义2021-10-14")]
        public event EquipValueFrashEventHandler ValueFrashed;
        public delegate void EquipValueFrashEventHandler(object sender, EventArgs e);

        public event EqpStateChangedEventHandler EqpStateChanged;
        public delegate void EqpStateChangedEventHandler(object sender, EventArgs e);

        public event SetItemNoPermissionEventHandler NoSetItemPermissionEvent;
        public delegate void SetItemNoPermissionEventHandler(object sender, NoSetItemPermissionEventArgs e);

        public event EventHandler EquipCommError;
        public event EventHandler EquipCommOk;
        public event EventHandler EquipHaveAlarm;
        public event EventHandler EquipNoAlarm;

        public int iCommFaultRetryCount = 0;
        public bool bInitOk = false;
        public bool bCommunicationOk = false;
        EquipTableRow dr;

        public SerialPort serialport;//适用于串口通讯

        public bool bCanMonitor = false;//能否进行监控，与授权文件有关
        public bool DoSetParm = false;//标记是否该设备处于设置动作
        public object EquipRWstate = false;//设备读写状态锁定标记
        public object EquipResetLock = false;//设备重置状态锁定标记

        object bsetcachedata = false;//设备是否缓存标记，缓存意味着实时数据会记录到数据库，作为重启系统的初始数据
        public bool bSetCacheData
        {
            get
            {
                lock (bsetcachedata)
                {
                    return (bool)bsetcachedata;
                }
            }
            set
            {
                lock (bsetcachedata)
                {
                    bsetcachedata = value;
                }
            }
        }

        //////////////////////

        object reset = false;
        object debug = false;
        object canconfirm = false;
        object isbackup = false;

        public bool IsRageMode = false;//是否处于急速运行模式
        /// <summary>
        /// 指示设备是否需要重置
        /// </summary>
        public object Reset
        {
            get
            {
                lock (reset)
                {
                    return reset;
                }
            }
            set
            {
                lock (reset)
                {
                    reset = value;
                }
            }
        }
        private object _lock = new object();
        private bool enable = true;
        /// <summary>
        /// 指示设备是否需要运行
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

        /// <summary>
        /// 指示设备是否需要进入调试状态
        /// </summary>
        public bool IsDebug
        {
            get
            {
                lock (debug)
                {
                    return (bool)debug;
                }
            }
            set
            {
                lock (debug)
                {
                    debug = value;
                }
            }
        }

        public bool IsBackupEquip = false;//是否在数据库中配置为热备设备，这个状态是配置到数据库中，不随着主备机之间的状态进行改变

        /// <summary>
        /// 指示设备是否处于备份状态，这个状态会根据主备机之间的状态进行改变
        /// </summary>
        public bool IsBackupState
        {
            get
            {
                lock (isbackup)
                {
                    return (bool)isbackup;
                }
            }
            set
            {
                lock (isbackup)
                {
                    isbackup = value;
                }
            }
        }

        public event EventHandler RevEqpEvt;//收到设备的通讯事件，比如：用于门禁设备的刷卡记录
        public void DoRevEqpEvt(EquipEvent Evt)
        {
            RevEqpEvt?.Invoke(Evt, null);
        }
        public string strLinkageEvent;
        public bool bCanConfirm2NormalState
        {
            get
            {
                lock (canconfirm)
                {
                    return (bool)canconfirm;
                }
            }
            set
            {
                lock (canconfirm)
                {
                    debug = value;
                }
            }
        }

        string evtGUID;
        public string EventGUID
        {
            get
            {
                return evtGUID;
            }
            set
            {
                evtGUID = value;
            }
        }
        bool bFirstEnter = true;

        void GetSafeTimeSpanList(string s)
        {
            try
            {
                SafeTimeSpanList.Clear();
                s = s.Trim();
                if (string.IsNullOrEmpty(s))
                    return;
                string[] ss = s.Split('+');
                foreach (string s1 in ss)
                {
                    string[] tt = s1.Split('-');
                    if (tt.Length == 2)
                    {
                        TimeSpan t1 = new TimeSpan(Convert.ToInt32(tt[0].Split(':')[0]), Convert.ToInt32(tt[0].Split(':')[1]), Convert.ToInt32(tt[0].Split(':')[2]));
                        TimeSpan t2 = new TimeSpan(Convert.ToInt32(tt[1].Split(':')[0]), Convert.ToInt32(tt[1].Split(':')[1]), Convert.ToInt32(tt[1].Split(':')[2]));
                        SafeTimeSpanList.Add(new SafeTimeSpan(t1, t2));
                    }
                }
            }
            catch (Exception e)
            {
            }
        }

        public EquipState State
        {
            get
            {
                lock (state)
                    return (EquipState)state;
            }
            set
            {
                if (value == EquipState.Initial)
                    state = (EquipState)value;

                if (value != (EquipState)state)
                {
                     state = (EquipState)value;
                }
            }

        }
        /// <summary>
        /// 指示设备数据是否刷新
        /// </summary>
        public bool DataFrash
        {
            get
            {
                lock (datafrash)
                {
                    return (bool)datafrash;
                }
            }
            set
            {
                lock (datafrash)
                {
                    datafrash = (object)value;
                }
            }
        }

        public Assembly Dll
        {
            get
            {
                return dll;
            }
        }

        public IEquip ICommunication
        {
            get
            {
                return icommunication;
            }
        }

        public int iStano
        {
            get
            {
                return istano;
            }
        }

        public int iEquipno
        {
            get
            {
                lock (iequipno)
                {
                    return (int)iequipno;
                }
            }
        }

        public int Alarm_scheme
        {
            get
            {
                return alarm_scheme;
            }
        }

        public int iAcc_cyc
        {
            get
            {
                return iacc_cyc;
            }
        }

        public int iAcc_num
        {
            get
            {
                return iacc_num;
            }
            set
            {
                iacc_num = value;
            }
        }

        public string Local_addr
        {
            get
            {
                if (local_addr.Length > 2)
                {
                    return local_addr.Split('(', ')')[0].Trim();//末尾添加(k)样式，用于划分多线程，比如192.168.0.11(1)、192.168.0.11(2)
                }
                else
                {
                    return local_addr;
                }
            }
        }

        internal string Local_addr4Thread
        {
            get
            {
                return local_addr;
            }
        }

        public string Equip_addr
        {
            get
            {
                return equip_addr;
            }
        }

        public string Equip_nm
        {
            get
            {
                lock (equip_nm)
                {
                    return equip_nm;
                }
            }
            set
            {
                lock (equip_nm)
                {
                    equip_nm = value;
                }
            }
        }

        public int CompareTo(object obj)
        {
            EquipItem eqp = obj as EquipItem;
            if (iEquipno > eqp.iEquipno)
            {
                return 1;
            }
            else if (iEquipno == eqp.iEquipno)
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }

        public EquipItem(int sta, int eqp, SerialPort p, EquipTableRow r)
        {
            istano = sta;
            iequipno = eqp;
            State = EquipState.Initial;
            serialport = p;
            ResetWhenDBChanged(sta, eqp,r);
            iacc_num = 0;
        }

        public bool ResetWhenDBChanged(params object[] o)
        {
            lock (EquipResetLock)
            {
                int sta = (int)o[0];
                int eqp = (int)o[1];
                if (o.Length > 2)
                {
                    dr = (EquipTableRow)o[2];//设备初始化的时候会传入这个参数，加快大设备数的启动时间
                }
                else//动态修改设备的时候不传入
                {
                    dr = StationItem.db_Eqp.Single(m => m.equip_no == eqp);
                }
                try
                {
                    equip_nm = dr.equip_nm ?? "";
                    equip_detail = dr.equip_detail ?? "";
                    iacc_cyc = dr.acc_cyc;
                    alarm_scheme = dr.alarm_scheme;
                    local_addr = dr.local_addr ?? "";
                    equip_addr = dr.equip_addr ?? "";
                    related_pic = dr.related_pic ?? "";
                    communication_drv = dr.communication_drv.Trim();
                    alarmMsg = equip_nm + ":" + dr.out_of_contact ?? "";
                    advice_Msg = dr.proc_advice ?? "";
                    attrib = dr.attrib;
                    bSetCacheData = attrib == 1 ? true : false;
                    RestorealarmMsg = equip_nm + ":" + dr.contacted ?? "";

                    AlarmRiseCycle = dr.AlarmRiseCycle == null ? 0 : dr.AlarmRiseCycle;
                    Reserve1 = dr.Reserve1 ?? "";
                    Reserve2 = dr.Reserve2 ?? "";
                    Reserve3 = dr.Reserve3 ?? "";

                    related_video = dr.related_video ?? "";
                    ZiChanID = dr.ZiChanID ?? "";
                    PlanNo = dr.PlanNo ?? "";

                    string strEnable = "True";
                    if(DataCenter.GetPropertyFromReserveWithEquipTableRow(out strEnable, "Reserve1","GWDataCenter.dll#EnableEquip", dr))
                    {
                        if(strEnable.ToLower()=="true")
                            Enable = true;
                        else if(strEnable.ToLower()=="false")
                            Enable = false;
                    }

                    if (!string.IsNullOrEmpty(dr.backup))
                    {
                        IsBackupEquip = dr.backup.ToUpper() == "TRUE" ? true : false;
                    }
                    else
                    {
                        IsBackupEquip = false;
                    }
                    GetSafeTimeSpanList(dr.SafeTime ?? "");

                    string wf = dr.event_wav ?? "";

                    string[] fs = wf.Split('/');
                    wave_file = fs[0];
                    if (fs.Length == 2)
                        restore_wave_file = fs[1];

                    communication_param = dr.communication_param ?? "";
                    communication_time_param = dr.communication_time_param ?? "";
                    GetInterfaceOfEquip();//加入这一项，确保可以动态修改dll名称
                    bFirstEnter = false;
                }
                catch (Exception e)
                {
                    DataCenter.WriteLogFile(e.ToString());
                }
                return true;
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="item"></param>
        public void AddSetItem(SetItem item)
        {
            if (item == null)
                return;

            if ((State != EquipState.NoCommunication && State != EquipState.Initial) || item.Type == "S")
            {
                SetItemQueue.Enqueue(item);
            }
        }

        /// <summary>
        /// 得到设备动态库中的 IEquip 接口
        /// </summary>
        void GetInterfaceOfEquip()
        {
            icommunication = null;
            try
            {
                dll = Assembly.GetEntryAssembly();
                if (dll == null)
                {
                    throw new ArgumentException(communication_drv + ":load fault");
                }
                Type[] types = dll.GetTypes();
                foreach (Type t in types)
                {
                    if (t.Name == "CEquip")
                    {
                        icommunication = dll.CreateInstance(t.FullName) as IEquip;
                        icommunication.equipitem = this;
                        EquipBase = dll.CreateInstance(t.FullName) as CEquipBase;
                        break;
                    }
                }
                if (icommunication == null)
                    throw new ArgumentException("icommunication as  IEquip is null");

            }
            catch (Exception e)
            {
                DataCenter.WriteLogFile(e.ToString());
                bCommunicationOk = false;
                State = EquipState.NoCommunication;
            }
        }

        private string GetFullPathName4CommDrv(string moduleName)
        {
            string fullPathName1 = Path.Combine(General.GetApplicationRootPath(), "dll", moduleName);
            string fullPathName2 = Path.Combine(General.GetApplicationRootPath(), "dll", Path.GetFileNameWithoutExtension(moduleName), moduleName);

            if (System.IO.File.Exists(fullPathName1))
            {
                if (System.IO.File.Exists(fullPathName2))
                {
                    DataCenter.WriteLogFile($"Equip表中加载的文件{moduleName}同时存在于{fullPathName1}和{fullPathName2}两个目录,可能出错!");
                }
                return fullPathName1;
            }
            else if (System.IO.File.Exists(fullPathName2))
            {
                return fullPathName2;
            }
            return null;
        }
    }

    /// <summary>
    /// 事件延迟执行类。用于某些频繁事件在指定时间后只需激发一次事件
    /// </summary>
    public class DelayEventFire
    {
        System.Timers.Timer T = new System.Timers.Timer();
        EventHandler m_EventHandler;
        int m_Msec;
        object m_Sender;
        EventArgs m_Args;
        bool binit = false;
        object olock = false;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Hander">传入的事件</param>
        /// <param name="Msec">延时时间，单位毫秒</param>
        /// <param name="Sender">传入事件的sender参数</param>
        /// <param name="Args">传入事件的EventArgs参数</param>
        public DelayEventFire(EventHandler Hander,int Msec,object Sender, EventArgs Args)
        {
            m_EventHandler = Hander;
            m_Msec = Msec;
            m_Sender = Sender;
            m_Args = Args;
        }

        /// <summary>
        /// 在限定时间Msec必须触发，防止持续的事件涌入导致传入事件迟迟不触发
        /// </summary>
        public void AddEvent()
        {
            lock (olock)
            {
                if (!binit)
                {
                    T.Interval = m_Msec;
                    T.Elapsed -= new System.Timers.ElapsedEventHandler(theout);
                    T.Elapsed += new System.Timers.ElapsedEventHandler(theout);
                    T.AutoReset = false;
                    T.Enabled = true;
                    binit = true;
                }
                T.Start();
            }
        }

        public void theout(object source, System.Timers.ElapsedEventArgs e)
        {
            lock (olock)
            {
                m_EventHandler?.Invoke(m_Sender, m_Args);
                T.Stop();
            }
        }
    }
}
