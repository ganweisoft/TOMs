using GWDataCenter.Database;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GWDataCenter.MqService;

namespace GWDataCenter
{
    public enum EquipState
    {
        NoCommunication = 0,
        CommunicationOK = 1,
        HaveAlarm = 2,
        HaveSetParm = 3,
        Initial = 4,
        CheFang = 5,
        BackUp = 6
    }

    public enum SafetyLevel
    {
        Unsafety=1, Safety
    }

    /// <summary>
    /// 对设备进行分类，每一个类型对应一个通讯线程
    /// </summary>
    public static class StationItem
    {
        static public SafetyLevel m_SafetyLevel = new SafetyLevel();
        static public Dictionary<string, object> EquipCategoryDict = new Dictionary<string, object>();
        static public ConcurrentQueue<SetItem> SetItemQueue = new ConcurrentQueue<SetItem>();
        static public Dictionary<string, List<EquipTableRow>> SubEquipListDict;

        static public DataTable EqpDt;

        static public event EventHandler SetParmResultEvent;//通知执行设置后的结果 add by sd in 20190316
        static public event EventHandler SetParmResponseEvent;//通知执行设置后的响应值（不是返回值），这个事件通常在dll中去执行

        static public event EventHandler AppClose;//程序关闭事件
        static public event EventHandler RightHandWave;
        static public event EventHandler LeftHandWave;

        static public event EventHandler StationCommError;
        static public event EventHandler StationCommOk;
        static public event EventHandler StationHaveAlarm;
        static public event EventHandler StationNoAlarm;

        static public event EventHandler EquipBackUpChanged;//备份状态发生改变

        static public event EventHandler HaveEquipChanged;//发生设备动态添加、删除、修改的事件
        static public event EventHandler HaveEquipReset;//发生设备配置重置事件


        static object resetequip = false;
        static UTF8Encoding enc = new UTF8Encoding();


        static System.Timers.Timer T = new System.Timers.Timer();

        static List<EquipTableRow> db_eqp;//= new DataTable();
        static List<YcpTableRow> db_ycp; //= new DataTable();
        static List<YxpTableRow> db_yxp;// = new DataTable();
        static List<SetParmTableRow> db_setparm; //= new DataTable();

        static readonly object db_eqp_lock = true;
        static readonly object db_ycp_lock = true;
        static readonly object db_yxp_lock = true;
        static readonly object db_setparm_lock = true;

        static public List<EquipTableRow> db_Eqp
        {
            get
            {
                lock (db_eqp_lock)
                {
                    return db_eqp;
                }
            }
            set
            {
                lock (db_eqp_lock)
                {
                    db_eqp = value;
                }
            }
        }
        static public List<YcpTableRow> db_Ycp
        {
            get
            {
                lock (db_ycp_lock)
                {
                    return db_ycp;
                }
            }
            set
            {
                lock (db_ycp_lock)
                {
                    db_ycp = value;
                }
            }
        }
        static public List<YxpTableRow> db_Yxp
        {
            get
            {
                lock (db_yxp_lock)
                {
                    return db_yxp;
                }
            }
            set
            {
                lock (db_yxp_lock)
                {
                    db_yxp = value;
                }
            }
        }
        static public List<SetParmTableRow> db_Setparm
        {
            get
            {
                lock (db_setparm_lock)
                {
                    return db_setparm;
                }
            }
            set
            {
                lock (db_setparm_lock)
                {
                    db_setparm = value;
                }
            }
        }

        public static void FireSetParmResultEvent(SetItem item)
        {
            StationItem.SetParmResultEvent?.Invoke(item, new EventArgs());
        }
        public static void DoHaveEquipChanged(SubEquipList SE)
        {
            HaveEquipChanged?.Invoke(SE, new EventArgs());
        }
        public static void DoHaveEquipReset(List<int> EqpNoList)
        {
            HaveEquipReset?.Invoke(EqpNoList, new EventArgs());
        }
        static public Dictionary<string, List<EquipTableRow>> GetSubEquipListDataRow(List<EquipTableRow> Rows)
        {
            Dictionary<string, List<EquipTableRow>> DS = new Dictionary<string, List<EquipTableRow>>();
            try
            {
                foreach (EquipTableRow s in Rows)
                {
                    string key = s.local_addr.ToUpper().Trim();
                    if (DS.ContainsKey(key))
                    {
                        DS[key].Add(s);
                    }
                    else
                    {
                        List<EquipTableRow> list = new List<EquipTableRow>();
                        list.Add(s);
                        DS.Add(key, list);
                    }
                }
            }
            catch(Exception e)
            {
                GWDataCenter.DataCenter.WriteLogFile(e.ToString()+ "检查local_addr字段不能为空");
            }
            return DS;
        }

        public static bool init()
        { 
            DataCenter.brunning = true;
            GWDbProvider.Instance.Init();
            Console.WriteLine("初始化数据库");

            UpdateMainDataTable();

            SubEquipListDict = GetSubEquipListDataRow(db_Eqp);

            Parallel.ForEach(SubEquipListDict, pair =>
            {
                SubEquipList sl = new SubEquipList(pair.Key, pair.Value);
                if (sl.bCanExcute)
                {
                    lock (EquipCategoryDict)
                    {
                        EquipCategoryDict.TryAdd(pair.Key, sl);
                    }
                }
            });
            Console.WriteLine("初始化其它服务");
            return true;
        }

        static public void UpdateMainDataTable()//获取Equip\YCP\YXP三个表
        {
            try
            {
                db_eqp = GWDbProvider.Instance.GetEquipTableList();
                db_ycp = GWDbProvider.Instance.GetYcpTableList();
                db_yxp = GWDbProvider.Instance.GetYxpTableList();
                db_setparm = GWDbProvider.Instance.GetSetParmTableList();
            }
            catch (Exception e)
            {
                DataCenter.WriteLogFile(e.ToString());
            }
        }

        /// <summary>
        /// 从设备号得到设备对象
        /// </summary>
        /// <param name="iEquipNo"></param>
        /// <returns></returns>
        static public EquipItem GetEquipItemFromEquipNo(int iEquipNo)
        {
            lock (EquipCategoryDict)
            {
                foreach (KeyValuePair<string, object> pair in EquipCategoryDict)
                {
                    SubEquipList EquipList = (SubEquipList)pair.Value;
                    foreach (EquipItem i in EquipList.EquipList)
                    {
                        if (i.iEquipno == iEquipNo)
                            return i;
                    }
                }
            }
            return null;
        }


        static public List<ChangedEquip> ChangedEquipList = new List<ChangedEquip>();
        static public event EventHandler ChangedEquipListChanged;//动态添加、删除、修改设备状态发生改变
        static public DelayEventFire ChangedEquipListChangedDelayEvent = null;

        static public event EventHandler AddNewSubEquipList;//动态添加了一个新的设备通讯线程
        static int iCount = 0;
        /// <summary>
        /// 用于动态添加、删除、修改设备  -----事件延迟响应，合并更新，大大提高了运行效率，比如一次性添加1000个设备，现在只需要一次操作。 by 2021-12-16
        /// </summary>
        /// <param name="Eqp"></param>
        public static void AddChangedEquip(ChangedEquip Eqp)
        {
            lock (ChangedEquipList)
            {
                ChangedEquipList.Add(new ChangedEquip { State = Eqp.State, iStaNo = Eqp.iStaNo, iEqpNo = Eqp.iEqpNo });
            }
            if (ChangedEquipListChangedDelayEvent == null)
            {
                ChangedEquipListChanged += StationItem_ChangedEquipListChanged;
                ChangedEquipListChangedDelayEvent = new DelayEventFire(ChangedEquipListChanged, 500, ChangedEquipList, null);
            }
            ChangedEquipListChangedDelayEvent.AddEvent();
            iCount += 1;
            DataCenter.WriteLogFile($"AddChangedEquip>>总共{iCount}个设备");
        }

        private static void StationItem_ChangedEquipListChanged(object sender, EventArgs e)
        {
            lock (ChangedEquipList)
            {
                StationItem.UpdateMainDataTable();
                foreach (ChangedEquip Eqp in ChangedEquipList)
                {
                    SubEquipList m_SubEquipList = GetSubEquipList(Eqp.iStaNo, Eqp.iEqpNo, Eqp.State);
                    if (m_SubEquipList != null)
                    {
                        m_SubEquipList.StartRefreshThread();
                        m_SubEquipList.StartSetParmThread();
                        m_SubEquipList.AddChangedEquipList1(Eqp);
                    }
                }
                DataCenter.WriteLogFile($"StationItem_ChangedEquipListChanged>>{ChangedEquipList.Count}个设备");
                ChangedEquipList.Clear();
            }
        }

        public static SubEquipList GetSubEquipList(int iStaNo, int iEqpNo, ChangedEquipState State)
        {
            SubEquipList rtnSubEquipList = null;
            SubEquipList thisSubEquipList = null;

            lock (StationItem.EquipCategoryDict)
            {
                //删除设备时不要查数据库，因为延迟响应的时候数据库已经被调用方删掉了 2021-12-22
                if (State == ChangedEquipState.Delete)
                {
                    foreach (KeyValuePair<string, object> pair in StationItem.EquipCategoryDict)
                    {
                        SubEquipList subEquipList = (SubEquipList)pair.Value;
                        foreach (EquipItem e in subEquipList.EquipList)
                        {
                            if (e.iEquipno == iEqpNo)
                            {
                                return subEquipList;
                            }
                        }
                    }
                }

                string strlocal_addr = null;
                //DataRow r = StationItem.db.GetDataRowOfEquip(iStaNo, iEqpNo);
                EquipTableRow r = StationItem.db_Eqp.Where(m => m.equip_no == iEqpNo).First();
                if (r != null)
                {
                    strlocal_addr = r.local_addr.ToUpper().Trim();
                }
                else
                {
                    EquipItem EI = StationItem.GetEquipItemFromEquipNo(iEqpNo);
                    if (EI == null)
                        return null;
                    strlocal_addr = EI.Local_addr4Thread.ToUpper().Trim();
                }
                if (strlocal_addr == null)
                    return null;
                foreach (KeyValuePair<string, object> pair in StationItem.EquipCategoryDict)
                {
                    SubEquipList EquipList = (SubEquipList)pair.Value;
                    if (EquipList.local_addr == strlocal_addr)
                    {
                        rtnSubEquipList = EquipList;
                        thisSubEquipList = EquipList;
                        break;
                    }
                }

                if (rtnSubEquipList == null)
                {
                    StationItem.SubEquipListDict = StationItem.GetSubEquipListDataRow(StationItem.db_Eqp);
                    SubEquipList NewEquipList = new SubEquipList(strlocal_addr, StationItem.SubEquipListDict[strlocal_addr]);

                    thisSubEquipList = NewEquipList;
                    if (NewEquipList.bCanExcute)
                    {
                        StationItem.EquipCategoryDict.TryAdd(strlocal_addr, NewEquipList);
                    }

                    AddNewSubEquipList?.Invoke(NewEquipList, null);
                }
                /*/////////////////////////////////////////
                在设备动态修改的时候，可能会修改local_addr的参数，这个时候，需要删除掉原来的线程
                */
                /////////////////////////////////////////
                if (State == ChangedEquipState.Edit)
                {
                    foreach (KeyValuePair<string, object> pair in StationItem.EquipCategoryDict)
                    {
                        SubEquipList subEquipList = (SubEquipList)pair.Value;
                        if (subEquipList.local_addr != thisSubEquipList.local_addr)
                        {
                            foreach (EquipItem e in subEquipList.EquipList)
                            {
                                if (e.iEquipno == iEqpNo)
                                {
                                    //其它线程还存在相同的设备，则要删除这个设备。   
                                    ChangedEquip Eqp = new ChangedEquip() { iStaNo = iStaNo, iEqpNo = iEqpNo, State = ChangedEquipState.Delete };
                                    subEquipList.bEditLocalAddr = true;
                                    thisSubEquipList.bEditLocalAddr = true;//add by sd in 2018-10-12

                                    ///
                                    lock (DataCenter.EquipItemDict)
                                    {
                                        if (DataCenter.EquipItemDict.ContainsKey(iEqpNo))
                                        {
                                            DataCenter.EquipItemDict.Remove(iEqpNo);
                                        }
                                    }
                                    ///
                                    subEquipList.AddChangedEquipList1(Eqp);
                                    break;
                                }
                            }
                        }
                    }
                    //删除这个设备的数据
                }

                return thisSubEquipList;
            }
        }
    }
    public enum ChangedEquipState
    {
        Add, Delete, Edit
    }
    public class ChangedEquip
    {
        public int iStaNo;
        public int iEqpNo;
        public ChangedEquipState State;
    }

    public class DelEquip
    {
        public int iStaNo;
        public int iEqpNo;
        public DateTime DelTime;
    }

    /// <summary>
    /// 如果设备的local_addr 相同,则这些设备将归为一个SubEquipList 类，将使用同一个通讯线程通讯
    /// 比如 使用COM4的所有设备将运行在一个线程
    /// </summary>
    public partial class SubEquipList
    {
        bool bStartThread = true;
        bool bStartRefreshThread = false;
        bool bStartSetParmThread = false;
        public static int ThreadInterval = 10;//100;//这个参数的调整，对CPU的影响很大
        const int DelayDelEqpSecond = 2;//延迟删除被删除设备的时间
        public string local_addr;

        ArrayList equiplist = new ArrayList();
        public ArrayList EquipList
        {
            get
            {
                return equiplist;
            }
            set
            {
                equiplist = value;
            }
        }

        DataTable dt;
        Thread RefreshThread, SetParmThread;
        public EquipItem OldEquip;
        object existsetparm = (object)false;
        object datarefreshbreak = (object)false;
        List<ChangedEquip> ChangedEquipList = new List<ChangedEquip>();//动态添加删除的设备
        List<ChangedEquip> ChangedEquipList1 = new List<ChangedEquip>();//动态添加删除的设备
        object bChangingState = (object)false;//用于动态添加删除设备

        public List<int> SetParmEquipList = new List<int>();//正在进行设备设置的列表
        public bool bCanExcute = true;//是否可以执行
        public bool bEditLocalAddr = false;//是否处于动态修改设备的LocalAddr，并生成一个新SubEquipList---特殊情况

        public event SubEquipListChangedEventHandler SubEquipListChanged;
        public delegate void SubEquipListChangedEventHandler(object sender, EventArgs e);

        public event EquipAddEventHandler EquipAdd;
        public delegate void EquipAddEventHandler(object sender, EventArgs e);

        public event EquipDelEventHandler EquipDel;
        public delegate void EquipDelEventHandler(object sender, EventArgs e);

        public event EquipEditEventHandler EquipEdit;
        public delegate void EquipEditEventHandler(object sender, EventArgs e);

        System.Timers.Timer T = new System.Timers.Timer();

        public event EventHandler ChangedEquipListChanged;
        public DelayEventFire ChangedEquipListChangedDelayEvent = null;

        public void AddChangedEquipList(ChangedEquip e)
        {
            lock (bChangingState)
            {
                //将对象添加到结尾处
                ChangedEquipList.Add(new ChangedEquip { State = e.State, iStaNo = e.iStaNo, iEqpNo = e.iEqpNo });
                StationItem.DoHaveEquipChanged(this);
                Thread.Sleep(1000);
            }
        }

        public void AddChangedEquipList1(ChangedEquip e)
        {
            lock (ChangedEquipList1)
            {
                ChangedEquipList1.Add(new ChangedEquip { State = e.State, iStaNo = e.iStaNo, iEqpNo = e.iEqpNo });
            }
            if(ChangedEquipListChangedDelayEvent==null)
            {
                ChangedEquipListChangedDelayEvent = new DelayEventFire(ChangedEquipListChanged, 500, ChangedEquipList1, null);
            }
            ChangedEquipListChangedDelayEvent.AddEvent();
        }

        private void SubEquipList_ChangedEquipListChanged(object sender, EventArgs e)
        {
            lock (ChangedEquipList1)
            {
                lock (bChangingState)
                {
                    if (ChangedEquipList1.Count == 0)
                        return;
                    foreach (ChangedEquip item in ChangedEquipList1)
                    {
                        //将对象添加到结尾处
                        ChangedEquipList.Add(new ChangedEquip { State = item.State, iStaNo = item.iStaNo, iEqpNo = item.iEqpNo });
                    }
                    ChangedEquipList1.Clear();
                    StationItem.DoHaveEquipChanged(this);
                    Thread.Sleep(1000);
                }
            }
        }
        int iCount = 0;
        
        void EditEquipList()
        {
            lock (bChangingState)
            {
                if (ChangedEquipList.Count == 0)
                    return;
                //StationItem.UpdateMainDataTable(); ---加入设备时已经更新了数据库，这里就不要重复操作了 2020/8/17
                EquipItem eqpitm = null;
                for (int k = 0; k < ChangedEquipList.Count; k++)
                {
                    //StationItem.UpdateMainDataTable();
                    if (ChangedEquipList[k].State == ChangedEquipState.Add)
                    {
                        SerialPort spt = new SerialPort();
                        int stano = ChangedEquipList[k].iStaNo;
                        int eqpno = ChangedEquipList[k].iEqpNo;
                        
                        lock (DataCenter.EquipItemDict)
                        {
                            //eqpitm = new EquipItem(stano, eqpno, spt);
                            var v = from i in EquipList.OfType<EquipItem>() where (i.iStano==stano && i.iEquipno==eqpno) select i;
                            if (v.Count() == 0)//队列不存在该项的时候才加入，否则新加一个 通讯线程的时候会重复
                            {
                                EquipTableRow dr = StationItem.db_Eqp.Single(m => m.equip_no == eqpno);
                                eqpitm = new EquipItem(stano, eqpno, spt,dr);
                                eqpitm.ICommunication.ResetFlag = true;
                                lock (EquipList)
                                {
                                    EquipList.Add(eqpitm);
                                }
                            }
                            else
                            {
                                eqpitm = v.First();
                                eqpitm.ICommunication.ResetFlag = true;
                            }

                            if (!DataCenter.EquipItemDict.ContainsKey(eqpno))
                                DataCenter.EquipItemDict.Add(eqpno, eqpitm);
                            EquipAdd?.Invoke(eqpno, new EventArgs());
                            iCount += 1;
                        }
                        
                    }
                    if (ChangedEquipList[k].State == ChangedEquipState.Delete)
                    {
                        lock (EquipList)
                        {
                            foreach (EquipItem e in EquipList)
                            {
                                if (e.iEquipno == ChangedEquipList[k].iEqpNo)
                                {
                                    if (!bEditLocalAddr)
                                    {
                                        e.State = EquipState.NoCommunication;//删除前先设为不通讯，这样远端如果有站点级联的时候可以反映出状态 2022-03-10
                                        EquipDel?.Invoke(e.iEquipno, new EventArgs());
                                    }

                                    EquipList.Remove(e);

                                    if (!bEditLocalAddr)
                                    {
                                        lock (DataCenter.EquipItemDict)
                                        {
                                            if (DataCenter.EquipItemDict.ContainsKey(ChangedEquipList[k].iEqpNo))
                                            {
                                                DataCenter.EquipItemDict.Remove(ChangedEquipList[k].iEqpNo);
                                            }
                                        }
                                    }
                                    lock (StationItem.EquipCategoryDict)
                                    {
                                        if (EquipList.Count == 0)//没有设备的时候，清理线程
                                        {
                                            if (StationItem.EquipCategoryDict.Values.Contains(this))//if (StationItem.EquipCategoryDict.ContainsValue(this))
                                            {
                                                StationItem.EquipCategoryDict.Remove(local_addr);//Remove(local_addr);
                                            }
                                            bStartThread = false;//停止线程循环
                                            DataRefreshBreak = true;//停止线程循环
                                        }
                                    }
                                    e.ICommunication.CloseCommunication();//释放通讯资源，比如释放串口，否则资源占用。

                                    if (bEditLocalAddr)
                                        bEditLocalAddr = false;
                                    break;
                                }
                            }
                        }
                    }
                    if (ChangedEquipList[k].State == ChangedEquipState.Edit)
                    {
                        lock (EquipList)
                        {
                            foreach (EquipItem e in EquipList)
                            {
                                if (e.iEquipno == ChangedEquipList[k].iEqpNo)
                                {
                                    //修改时，直接对该设备进行复位操作即可
                                    e.ResetWhenDBChanged(e.iStano, e.iEquipno);
                                    if (e.ICommunication != null)
                                        e.ICommunication.ResetFlag = true;
                                    break;
                                }
                            }
                        }
                        ////////////////////////////////////////////////////////////
                        int eqpno = ChangedEquipList[k].iEqpNo;
                        EquipEdit?.Invoke(eqpno, new EventArgs());
                    }
                    //ChangedEquipList.Remove(ChangedEquipList[k]);//不能在循环过程中删除
                }
                ChangedEquipList.Clear();
            }
        }

        public bool ExistSetParm
        {
            get
            {
                lock (existsetparm)
                {
                    return (bool)existsetparm;
                }
            }
            set
            {
                lock (existsetparm)
                {
                    existsetparm = (object)value;
                }
            }
        }
        public bool DataRefreshBreak
        {
            get
            {
                lock (datarefreshbreak)
                {
                    return (bool)datarefreshbreak;
                }
            }
            set
            {
                lock (datarefreshbreak)
                {
                    datarefreshbreak = (object)value;
                }
            }
        }
        object resetequip = false;
        public bool ResetEquips
        {
            get
            {
                lock (resetequip)
                {
                    return (bool)resetequip;
                }
            }
            set
            {
                lock (resetequip)
                {
                    resetequip = value;
                }
            }
        }

        public SubEquipList(string str_local_addr, List<EquipTableRow> DataRowList)
        {
            //对挂在同一串口下的设备应该共用一个串口实例,否则会有冲突
            SerialPort spt = new SerialPort();
            local_addr = str_local_addr;

            foreach (EquipTableRow r in DataRowList)
            {
                int stano = r.sta_n;
                int eqpno = r.equip_no;

                EquipItem e = new EquipItem(stano, eqpno, spt,r);
                lock (EquipList)
                {
                    EquipList.Add(e);
                }
                lock (DataCenter.EquipItemDict)
                {
                    if (!DataCenter.EquipItemDict.ContainsKey(eqpno))
                    {
                        DataCenter.EquipItemDict.Add(eqpno, e);
                    }
                    else
                    {
                        DataCenter.EquipItemDict.Remove(eqpno);
                        DataCenter.EquipItemDict.Add(eqpno, e);
                    }
                }

            }

            ChangedEquipListChanged -= SubEquipList_ChangedEquipListChanged;
            ChangedEquipListChanged += SubEquipList_ChangedEquipListChanged;
            if (IsRageMode())
            {
                ThreadInterval = 0;
                lock (EquipList)
                {
                    foreach(EquipItem item in EquipList)
                        item.IsRageMode = true;
                }
            }
        }

        bool IsRageMode()
        {
            string[] ss;
            string strResult = DataCenter.GetPropertyFromPropertyService("RunRageMode", "", "");
            if (strResult.Trim().ToUpper() == "ALL")
                return true;
            if (string.IsNullOrEmpty(strResult.Trim()))
                return false;
            else
            {
                ss = strResult.Split('%');
            }
            foreach (string s in ss)
            {
                if (s.ToUpper().Trim() == local_addr.ToUpper().Trim())
                    return true;
            }
            return false;
        }

        public void Refresh()
        {
            while (bStartThread)
            {
                while (ExistSetParm && DataRefreshBreak && bStartThread)//设置状态
                {
                    Thread.Sleep(ThreadInterval);
                }
                while (!DataRefreshBreak && bStartThread)//获取数据状态
                {
                    //数据库更新后进行数据重置
                    if (ResetEquips)
                    {
                        try
                        {
                            lock (EquipList)
                            {
                                foreach (EquipItem e in EquipList)
                                {
                                    e.ResetWhenDBChanged(e.iStano, e.iEquipno);
                                    if (e.ICommunication != null)
                                        e.ICommunication.ResetFlag = true;
                                }
                            }
                        }
                        catch(Exception e1)
                        {
                            DataCenter.WriteLogFile(e1.ToString());
                        }
                        ResetEquips = false;
                    }
                    try
                    {
                        EditEquipList();
                        EquipRefrash();
                    }
                    catch (Exception e)
                    {
                        DataCenter.WriteLogFile(e.ToString());
                    }
                    Thread.Sleep(ThreadInterval);
                }
            }
        }

        void EquipRefrash()
        {
            var stateItems = new List<StateItem>();
            foreach (EquipItem e in EquipList)
            {
                lock (bChangingState)
                {
                    if (ChangedEquipList.Count != 0) //如果有设备添加变化，则立即返回
                        break;
                }

                if (e.ICommunication == null)
                    continue;
                if (e.IsBackupState == true) //备机状态不实际采集通讯 add by sd in 2020/4/15
                {
                    if (e.State != EquipState.BackUp)
                    {
                        e.State = EquipState.BackUp;
                    }

                    continue;
                }

                /////////add by sd in 2014/7/30,把设置的设备号放在一个队列里面，以便设备设置后的状态可以尽快在界面上反映出来
                lock (SetParmEquipList)
                {
                    if (SetParmEquipList.Count > 0)
                    {
                        if (!SetParmEquipList.Contains(e.iEquipno))
                        {
                            continue;
                        }
                        else
                        {
                            SetParmEquipList.Remove(e.iEquipno);
                        }
                    }
                }

                ////////////////////
                if (!e.Enable) //设备不允许运行
                {
                    e.State = EquipState.NoCommunication; //设置成不通讯
                    continue;
                }

                e.iAcc_num++;

                if ((e.iAcc_num % e.iAcc_cyc) != 0) continue;
                e.iAcc_num = 0;
                e.Reset = false;
                OldEquip = e;
                if (e.ICommunication == null)
                    continue;

                if (!e.bInitOk || e.ICommunication.ResetFlag) //如果第一次运行或者有重置才会调用或者通讯恢复后第一次调用才会调用init
                {
                    //e.State = EquipState.Initial;//2024-02-05 加入这行是因为级联动态添加设备的时候设备状态数据有时候不上传的原因
                    if (!e.ICommunication.init(e))
                    {
                        e.iCommFaultRetryCount += 1;
                        e.ICommunication.ResetFlag = true; //add in 2014-7-2   解决某些设备通讯中断后yxp值出现unknow 的现象
                        //通讯重试超过预设次数，则通讯失败
                        if (e.ICommunication.m_retrytime <= e.iCommFaultRetryCount)
                        {
                            lock (e.EquipRWstate)
                            {
                                e.bCommunicationOk = false;
                                e.State = EquipState.NoCommunication;
                                e.iCommFaultRetryCount = 0;
                                e.bInitOk = false;
                            }
                        }

                        continue;
                    }
                    else
                    {
                        e.bInitOk = true;
                        e.ICommunication.ResetFlag = false;
                    }

                    if (e.State != EquipState.NoCommunication) //2024-04-10 避免一直通讯失败的设备来回在通讯正常和失败之间切换
                        e.State = EquipState.Initial; //2024-02-05 加入这行是因为级联动态添加设备的时候设备状态数据有时候不上传的原因
                }

                CommunicationState ret = e.ICommunication.GetData((CEquipBase)e.ICommunication);
                if (ret == CommunicationState.ok) //数据成功获取
                {
                    e.bCommunicationOk = true;
                    e.iCommFaultRetryCount = 0;

                    lock (e.ICommunication.YCResults)
                    {
                        // 把pair.Value通过MQTT发出去
                        if (e.ICommunication.YCResults.Any())
                            MqttProvider.Instance.PublishYcRtValueAsync(new MqRtValueMessage
                            {
                                Time = DateTime.Now.ToString(CultureInfo.CurrentCulture),
                                Flow = Guid.NewGuid().ToString("N"),
                                DataItems =
                                [
                                    new DataItem
                                    {
                                        DeviceId = e.ICommunication.m_equip_no,
                                        Attribute = e.ICommunication.YCResults
                                    }
                                ]
                            });
                    }

                    lock (e.ICommunication.YXResults)
                    {
                        // 把pair.Value通过MQTT发出去
                        if (e.ICommunication.YXResults.Any())
                            MqttProvider.Instance.PublishYxRtValueAsync(new MqRtValueMessage
                            {
                                Time = DateTime.Now.ToString(CultureInfo.CurrentCulture),
                                Flow = Guid.NewGuid().ToString("N"),
                                DataItems =
                                [
                                    new DataItem
                                    {
                                        DeviceId = e.ICommunication.m_equip_no,
                                        Attribute = e.ICommunication.YXResults
                                    }
                                ]
                            });
                    }

                    //设备返回的事件列表，比如：用于门禁设备的刷卡记录，不同于YC和YX
                    lock (e.ICommunication.EquipEventList)
                    {
                        if (e.ICommunication.EquipEventList.Count > 0)
                        {
                            // 把evt通过MQTT发出去
                            MqttProvider.Instance.PublishEvtValueAsync(new MqEvtMessage
                            {
                                Time = DateTime.Now.ToString(CultureInfo.CurrentCulture),
                                Flow = Guid.NewGuid().ToString("N"),
                                EventItems =
                                [
                                    new MqService.EquipEvent
                                    {
                                        DeviceId = e.ICommunication.m_equip_no,
                                        EquipEvents = e.ICommunication.EquipEventList.Select(evt =>
                                            new EquipEventItem
                                            {
                                                Msg = evt.msg,
                                                Msg4Linkage = evt.msg4Linkage,
                                                Level = evt.level,
                                                OccurDateTime = evt.dt,
                                                EquipNo = evt.iEquipNo
                                            }).ToList()
                                    }
                                ]
                            });

                            e.ICommunication.EquipEventList.Clear();
                        }
                    }

                    e.DataFrash = true;
                }

                if (ret == CommunicationState.setreturn)
                {
                    if (e.DoSetParm && e.State == EquipState.CommunicationOK)
                    {
                        lock (e.EquipRWstate)
                        {
                            e.State = EquipState.HaveSetParm;
                        }
                    }

                    DataRefreshBreak = true;
                    return;
                }

                if (ret == CommunicationState.fail)
                {
                    e.iCommFaultRetryCount += 1;
                    e.ICommunication.ResetFlag = true;
                    //通讯重试超过预设次数，则通讯失败
                    if (e.ICommunication.m_retrytime <= e.iCommFaultRetryCount)
                    {
                        lock (e.EquipRWstate)
                        {
                            e.bCommunicationOk = false;
                            e.State = EquipState.NoCommunication;
                            e.iCommFaultRetryCount = 0;
                        }
                    }
                }
                
                stateItems.Add(new StateItem
                {
                    DeviceId = e.iEquipno,
                    State = Enum.GetName(EquipState.NoCommunication)
                });
            }

            if (stateItems.Any())
                MqttProvider.Instance.PublishRtStateAsync(new MqRtStateMessage
                {
                    Time = DateTime.Now.ToString(CultureInfo.CurrentCulture),
                    Flow = Guid.NewGuid().ToString("N"),
                    StateItems = stateItems
                });
        }
        public DateTime StartRefreshTime;
        public void StartRefreshThread()
        {
            if (!bStartRefreshThread)
            {
                ThreadStart entryPoint = new ThreadStart(Refresh);
                RefreshThread = new Thread(entryPoint);
                RefreshThread.Start();
                bStartRefreshThread = true;
                StartRefreshTime = DateTime.Now;
            }
        }

        ///////////////////////////////////////////////////

        public void SetParmScan()
        {
            while (bStartThread)
            {
                try
                {
                    lock (EquipList)
                    {
                        foreach (EquipItem e in EquipList)
                        {
                            {
                                if (e.SetItemQueue.Count > 0)
                                {
                                    ExistSetParm = true;
                                    SendSetParmFlag(true);
                                    e.DoSetParm = true;
                                    SetItem setitem;
                                    e.SetItemQueue.TryDequeue(out setitem);
                                    if ((e.Reserve3.ToUpper().Trim() != "FASTSET") && (setitem.Type.ToUpper() != "S"))//比如云台控制等对设置时效性要求很高的设备，或者系统命令，就不再等待数据刷新的终止
                                    {
                                        while (!DataRefreshBreak)//等待,确保数据刷新过程已经中止
                                        {
                                            Thread.Sleep(ThreadInterval);//必须sleep,否则DataRefreshBreak在同步的时候会耗时较大
                                        }
                                    }
                                    try
                                    {
                                        SetParm(e, setitem);
                                        /////////add by sd in 2014/7/30,把设置的设备号放在一个队列里面，以便设备设置后的状态可以尽快在界面上反映出来
                                        lock (SetParmEquipList)
                                        {
                                            if (!SetParmEquipList.Contains(e.iEquipno))
                                                SetParmEquipList.Add(e.iEquipno);
                                        }
                                    }
                                    catch (Exception e1)
                                    {
                                        DataCenter.WriteLogFile(e1.ToString());
                                    }

                                    e.DoSetParm = false;
                                }
                            }
                        }
                        ExistSetParm = false;
                        DataRefreshBreak = false;
                        SendSetParmFlag(false);
                    }
                }
                catch (Exception e)
                {
                    DataCenter.WriteLogFile(e.ToString());
                }
                Thread.Sleep(ThreadInterval);
            }
        }
        object bLockSetParm = true;
        public void SetParm(EquipItem e, SetItem setitem)
        {
            //SetParm函数采用线程池模式执行，避免某些设置函数由于耗时阻塞其它设备的设置。
            Task.Factory.StartNew(() =>
            {
                lock (bLockSetParm)//增加了DoEquipSetItem方式，要考虑线程安全
                {
                    if (e.ICommunication == null)
                        return;
                    if (e.EquipBase == null)
                        return;

                    e.State = EquipState.HaveSetParm;
                    //        e.DataFrash = true;
                    string msg = "";
                    e.ICommunication.init(e);

                    if (setitem.bStopSetParm)//中断的设置不执行
                    {
                        return;
                    }

                    string s1, s2, s3, s4;
                    s1 = ResourceService.GetString("AlarmCenter.DataCenter.Msg2");
                    s2 = ResourceService.GetString("AlarmCenter.DataCenter.Msg3");
                    s3 = ResourceService.GetString("AlarmCenter.DataCenter.Msg4");
                    s4 = ResourceService.GetString("AlarmCenter.DataCenter.Msg5");
                    string desc = setitem.GetSetItemDesc();

                    if (setitem.Type == null)
                    {
                        return;
                    }

                    e.ICommunication.SetParmExecutor = setitem.Executor;//add by sd in 20160801
                    string csExecutor;
                    csExecutor = setitem.Executor;
                    if (string.IsNullOrEmpty(setitem.Executor))
                        csExecutor = setitem.sysExecutor;

                    e.curSetItem = setitem;//传入当前设置项,设备dll在返回响应值的时候可能会用到

                    if (e.ICommunication.SetParm(setitem.MainInstruct, setitem.MinorInstruct, setitem.Value))
                    {
                        setitem.WaitSetParmIsFinish = true;
                        if (desc == null)
                        {
                            msg += string.Format(s1 + "{0}---" + s2 + s4 + "{1}-{2}-{3}", e.Equip_nm, setitem.MainInstruct, setitem.MinorInstruct, setitem.Value) + "---by:" + csExecutor;
                        }
                        else
                        {
                            msg += desc + ">>" + s2 + "---by:" + csExecutor;
                        }

                    }
                    else
                    {
                        setitem.WaitSetParmIsFinish = false;
                        if (desc == null)
                        {
                            msg += string.Format(s1 + "{0}---" + s3 + s4 + "{1}-{2}-{3}", e.Equip_nm, setitem.MainInstruct, setitem.MinorInstruct, setitem.Value) + "---by:" + csExecutor;
                        }
                        else
                        {
                            msg += desc + ">>" + s3 + "---by:" + csExecutor;
                        }
                        Console.WriteLine(msg);
                    }
                    //StationItem.FireSetParmResponseEvent(setitem.EquipNo, setitem.m_SetNo, setitem.Value, setitem.csResponse, bSetFinish, setitem.RequestId);
                    if (!setitem.isSynchronization)//只有异步命令才激发设置结果事件
                        StationItem.FireSetParmResultEvent(setitem);
                }
            });
        }

        public void SendSetParmFlag(bool flag)
        {
            lock (EquipList)
            {
                foreach (EquipItem e in EquipList)
                {
                    if (e.ICommunication != null)
                        e.ICommunication.RunSetParmFlag = flag;
                }
            }
        }

        public void StartSetParmThread()
        {
            if (!bStartSetParmThread)
            {
                ThreadStart entryPoint = new ThreadStart(SetParmScan);
                SetParmThread = new Thread(entryPoint);
                SetParmThread.Start();
                bStartSetParmThread = true;
            }
        }

    }

}
