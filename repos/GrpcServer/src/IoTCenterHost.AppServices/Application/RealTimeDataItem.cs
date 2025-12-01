//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using GWDataCenter;
using IoTCenterHost.AppServices.Interfaces.Token;
using IoTCenterHost.Core.Abstraction;
using IoTCenterHost.Core.Abstraction.BaseModels;
using IoTCenterHost.Core.Abstraction.IotModels;
using IoTCenterHost.Core.Extension;
using IoTCenterHost.Core.ProxyModels;
using Microsoft.Extensions.Logging;
using OpenGWDataCenter.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace IoTCenterHost.AppServices.AppServices
{
    public class RealTimeDataItem : IDisposable
    {
        public bool bRegisterDataSession = false;
        private ConcurrentDictionary<string, GWUserItem> _currentUserItemDict;
        private GWUserItem _currentUserItem;
        private const string EquipDictCacheKey = "GW_EQUIP_TOTAL_DICT_CACHEKEY";

        public List<RemoteRegisterData> RegisterDataList = new List<RemoteRegisterData>();

        public ObservableCollection<RealTimeEventItem> realTimeEventItems;
        private long index_currect = 0;
        private long remain_length;

        public List<int> AddEquipList = new List<int>();
        public List<int> DelEquipList = new List<int>();
        public List<int> EditEquipList = new List<int>();
        private Dictionary<int, RealTimeEventItem> _RealEventItemDict = null;

        private Timer threadTimer = null;

        private ConcurrentBag<ProxyYcItem> ToSendYcItems = null;
        public event EventHandler<ProxyEquipItem> OnEquipAdd;
        public event EventHandler<ConcurrentBag<ProxyYcItem>> OnYCChanged;
        public event EventHandler<ProxyYxItem> OnYXChanged;
        public event EventHandler<ProxyEquipStateItem> OnEquipStatusChanged;
        public event EventHandler<ProxyEquipItem> OnEquipChanged;
        public event EventHandler<int> OnEquipDeleted;
        public event EventHandler<WcfRealTimeEventItem> AddRealTimeSnapshot;
        public event EventHandler<WcfRealTimeEventItem> DeleteRealTimeSnapshot;

        public string IPandPort { get; set; }
        private readonly bool isPushEvent = true;
        private int _bufferSize = 0;
        private ILogger<RealTimeDataItem> _logger;
        private Stopwatch _stopWatcher = new Stopwatch();
        private IEnumerable<int> _debugEquipNos;
        private int waitTime = 100;

        public RealTimeDataItem(ILogger<RealTimeDataItem> logger, IEnumerable<int> debugEquioNos)
        {
            _currentUserItemDict = new ConcurrentDictionary<string, GWUserItem>();
            ToSendYcItems = new ConcurrentBag<ProxyYcItem>();
            _logger = logger;
            DoChangedEquipEvent();
            InitPushEvent();
            PushEquipMessages();
        }

        private async void PushEquipMessages()
        {
            await Task.Run(() =>
            {
                threadTimer = new System.Threading.Timer((object state) =>
                {
                    if (ToSendYcItems != null && ToSendYcItems.Any())
                    {
                        OnYCChanged?.Invoke(this, ToSendYcItems);
                        ToSendYcItems = null;
                        ToSendYcItems = new ConcurrentBag<ProxyYcItem>();
                    }
                }, null, Timeout.Infinite, waitTime);
                threadTimer.Change(0, waitTime);
            });
        }
        public void AddUser(string token, GWUserItem userItem)
        {
            if (!_currentUserItemDict.ContainsKey(token))
            {
                _currentUserItemDict.TryAdd(token, userItem);
            }
        }

        public RealTimeDataItem SetCurrentUser(LoginUser loginUser)
        {
            if (loginUser == null) return this;
            if (_currentUserItemDict.ContainsKey(loginUser.LoginMark))
            {
                _currentUserItemDict.TryGetValue(loginUser.LoginMark, out _currentUserItem);
            }
            else if (loginUser.UserItem != null)
            {
                _currentUserItemDict.TryAdd(loginUser.LoginMark, loginUser.UserItem);
            }
            return this;
        }


        public RealTimeDataItem SetCurrentIpAddr(string ipAddr)
        {
            IPandPort = ipAddr;
            return this;
        }


        void DoChangedEquipEvent()
        {
            lock (StationItem.EquipCategoryDict)
            {
                foreach (KeyValuePair<string, object> pair in StationItem.EquipCategoryDict)
                {
                    SubEquipList EquipList = (SubEquipList)pair.Value;
                    EquipList.EquipAdd -= EquipList_EquipAdd;
                    EquipList.EquipAdd += EquipList_EquipAdd;
                    EquipList.EquipDel -= EquipList_EquipDel;
                    EquipList.EquipDel += EquipList_EquipDel;
                    EquipList.EquipEdit -= EquipList_EquipEdit;
                    EquipList.EquipEdit += EquipList_EquipEdit;
                }

                StationItem.HaveEquipChanged -= StationItem_HaveEquipChanged;
                StationItem.HaveEquipChanged += StationItem_HaveEquipChanged;
            }
            RecordTime("DoChangedEquipEvent");
        }


        private void ClosePage(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        void InitPushEvent()
        {
            MessageService.GetEventList().CollectionChanged -= EditEventItem;
            MessageService.GetEventList().CollectionChanged += EditEventItem;
            BindEquipChanged(isPushEvent);
            BindAllEquipYXItems(isPushEvent);
            BindAllEquipYCItems(isPushEvent);

        }

        void EquipList_EquipDel(object sender, EventArgs e)
        {
            lock (this.DelEquipList)
            {
                int equipNo = (int)sender;
                if (!this.DelEquipList.Contains(equipNo))
                {
                    this.DelEquipList.Add(equipNo);
                }
                OnEquipDeleted?.Invoke(sender, equipNo);
            }
        }

        void EquipList_EquipAdd(object sender, EventArgs e)
        {
            lock (this.AddEquipList)
            {
                int equipNo = (int)sender;
                if (!this.AddEquipList.Contains(equipNo))
                {
                    BindEventToNewEquip(equipNo);
                    this.AddEquipList.Add(equipNo);
                    ProxyEquipItem item = new ProxyEquipItem(DataCenter.EquipItemDict[equipNo]);
                    OnEquipAdd?.Invoke(sender, item);
                }
            }
        }

        void EquipList_EquipEdit(object sender, EventArgs e)
        {
            lock (this.EditEquipList)
            {
                int equipNo = (int)sender;
                BindEventToEditEquip(equipNo);
                if (!this.EditEquipList.Contains(equipNo))
                    this.EditEquipList.Add(equipNo);
                lock (this.AddEquipList)
                {
                    if (!this.AddEquipList.Contains(equipNo))
                        this.AddEquipList.Add(equipNo);
                }
                lock (this.DelEquipList)
                {
                    if (!this.DelEquipList.Contains(equipNo))
                        this.DelEquipList.Add(equipNo);
                }
                if (DataCenter.EquipItemDict.ContainsKey(equipNo))
                {
                    ProxyEquipItem item = new ProxyEquipItem(DataCenter.EquipItemDict[equipNo]);
                    OnEquipChanged?.Invoke(sender, item);
                }
            }
        }

        void StationItem_HaveEquipChanged(object sender, EventArgs e)
        {
            SubEquipList m_SubEquipList = sender as SubEquipList;
            m_SubEquipList.EquipAdd -= EquipList_EquipAdd;
            m_SubEquipList.EquipAdd += EquipList_EquipAdd;
            m_SubEquipList.EquipDel -= EquipList_EquipDel;
            m_SubEquipList.EquipDel += EquipList_EquipDel;
            m_SubEquipList.EquipEdit -= EquipList_EquipEdit;
            m_SubEquipList.EquipEdit += EquipList_EquipEdit;
        }

        void RecordTime(string msg)
        {
            _stopWatcher.Stop();
            _stopWatcher.Restart();
        }

        public bool DllRegisterData(List<RemoteRegisterData> RegisterData)
        {
            RegisterDataList.AddRange(RegisterData);
            if (RegisterDataList.Count >= 0)
                bRegisterDataSession = true;
            return true;
        }
        [Obsolete]
        public void GetTotalRTEquipItemData(Action<IEnumerable<EquipItem>> action)
        {
            index_currect = 0;
            remain_length = 0;
            GWUserItem m_WcfLoginUser = _currentUserItem;
            Dictionary<int, GrpcEquipItem> Equip_dict = CreateEquipDict();
        }

        private object _lockHelper = new object();
        public Dictionary<int, GrpcEquipItem> CreateEquipDict()
        {
            var Equip_dict = GetEquipDict();
            return Equip_dict;
        }

        private Dictionary<int, GrpcEquipItem> GetEquipDict()
        {
            var Equip_dict = new Dictionary<int, GrpcEquipItem>();
            try
            {

                lock (_lockHelper)
                {
                    foreach (KeyValuePair<int, EquipItem> entry in DataCenter.EquipItemDict)
                    {
                        lock (entry.Value.EquipRWstate)
                        {
                            ProxyEquipItem item = new ProxyEquipItem(entry.Value);
                            Equip_dict.Add(item.m_iEquipNo, item.Map<GrpcEquipItem>());
                        }
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
            return Equip_dict;
        }

        public List<EquipItem> GetTotalRTEquipItemDataEx(bool bindEvent = false)
        {
            index_currect = 0;
            remain_length = 0;
            GWUserItem m_WcfLoginUser = _currentUserItem;
            List<EquipItem> equipItems = new List<EquipItem>();
            lock (DataCenter.EquipItemDict)
            {
                foreach (KeyValuePair<int, EquipItem> entry in DataCenter.EquipItemDict)
                {
                    lock (entry.Value.EquipRWstate)
                    {
                        equipItems.Add(entry.Value);
                    }
                }
            }
            return equipItems;
        }

        private void BindEquipChanged(bool bindEvent = false)
        {
            lock (DataCenter.EquipItemDict)
            {
                foreach (KeyValuePair<int, EquipItem> entry in DataCenter.EquipItemDict)
                {
                    lock (entry.Value.EquipRWstate)
                    {
                        if (bindEvent)
                            entry.Value.EqpStateChanged += new EquipItem.EqpStateChangedEventHandler(EquipState_StateChanged);
                    }
                }
            }
            RecordTime("BindEquipChanged");

        }
        void EquipState_StateChanged(object sender, EventArgs e)
        {
            EquipItem o = (EquipItem)sender;
            ProxyEquipStateItem item = new ProxyEquipStateItem(o);
            OnEquipStatusChanged?.Invoke(o, item);

            item = null;
            o = null;
        }
        public void GetChangedRTEquipItemData()
        {
            index_currect = 0;
            remain_length = 0;
            Dictionary<int, ProxyEquipItem> dict = new Dictionary<int, ProxyEquipItem>();

        }
        public Dictionary<ulong, GrpcYcItem> CreateProxyYCItems()
        {
            index_currect = 0;
            remain_length = 0;

            var YC_dict = new Dictionary<ulong, GrpcYcItem>();
            GWUserItem m_WcfLoginUser = _currentUserItem;
            lock (DataCenter.EquipItemDict)
            {
                List<YCItem> items = new List<YCItem>();
                foreach (KeyValuePair<int, EquipItem> entry in DataCenter.EquipItemDict)
                {
                    lock (entry.Value.YCItemDict)
                    {
                        foreach (KeyValuePair<int, YCItem> entry1 in entry.Value.YCItemDict)
                        {
                            if (bRegisterDataSession)
                            {
                                if (!IsDllRegisterData4Ycp(entry1.Value.Equip_no, entry1.Value.Yc_no))
                                    continue;
                            }
                            var item = new ProxyYcItem(entry1.Value).Map<GrpcYcItem>();
                            lock (YC_dict)
                            {

                                var key = ((UInt64)(item.m_iEquipNo) << 32) + (UInt64)item.m_iYCNo;
                                if (!YC_dict.ContainsKey(key))
                                    YC_dict.Add(key, item);
                                else
                                    YC_dict[key] = item;
                            }
                        }
                    }
                }
            }
            return YC_dict;
        }

        public void SetNewEquipValueChanged(int equipNo)
        {
            var changeEquipItem = DataCenter.EquipItemDict[equipNo];
            List<YCItem> items = new List<YCItem>();

            if (changeEquipItem != null)
            {
                BindYCItemValueChanged(changeEquipItem);
                _ = GetYXItemValue(changeEquipItem);
            }
        }

        public List<YCItem> GetTotalRTYCItemDataEx(bool bindEvent = false)
        {
            index_currect = 0;
            remain_length = 0;

            GWUserItem m_WcfLoginUser = _currentUserItem;
            List<YCItem> items = new List<YCItem>();

            lock (DataCenter.EquipItemDict)
            {
                foreach (KeyValuePair<int, EquipItem> entry in DataCenter.EquipItemDict)
                {
                    lock (entry.Value.YCItemDict)
                        items.AddRange(GetYCItems(entry.Value));
                }
            }
            return items;
        }

        private void BindAllEquipYCItems(bool bindEvent = false)
        {
            lock (DataCenter.EquipItemDict)
            {
                foreach (KeyValuePair<int, EquipItem> entry in DataCenter.EquipItemDict)
                {
                    lock (entry.Value.YCItemDict)
                        BindYCItemValueChanged(entry.Value, bindEvent);
                }
            }
            RecordTime("BindAllEquipYCItems");

        }
        private void BindYCItemValueChanged(EquipItem entry, bool bindEvent = false)
        {
            foreach (KeyValuePair<int, YCItem> entry1 in entry.YCItemDict)
            {
                if (bRegisterDataSession)
                {
                    if (!IsDllRegisterData4Ycp(entry1.Value.Equip_no, entry1.Value.Yc_no))
                        continue;
                }
                if (bindEvent)
                {
                    entry1.Value.ValueChanged -= new YCItem.YCValueChangedEventHandler(YCValue_ValueChanged);
                    entry1.Value.ValueChanged += new YCItem.YCValueChangedEventHandler(YCValue_ValueChanged);
                }
            }
        }
        private List<YCItem> GetYCItems(EquipItem entry, bool bindEvent = false)
        {
            List<YCItem> items = new List<YCItem>();
            foreach (KeyValuePair<int, YCItem> entry1 in entry.YCItemDict)
            {
                if (bRegisterDataSession)
                {
                    if (!IsDllRegisterData4Ycp(entry1.Value.Equip_no, entry1.Value.Yc_no))
                        continue;
                }
                items.Add(entry1.Value);
            }
            return items;
        }

        void YCValue_ValueChanged(object sender, EventArgs e)
        {
            YCItem o = (YCItem)sender;
            ProxyYcItem item = new ProxyYcItem(o);
            ToSendYcItems?.Add(item);
        }

        public void GetChangedRTYCItemData()
        {
            index_currect = 0;
            remain_length = 0;

            Dictionary<UInt64, ProxyYcItem> dict = new Dictionary<ulong, ProxyYcItem>();

        }

        public Dictionary<ulong, GrpcYxItem> CreateProxyYXItems()
        {
            index_currect = 0;
            remain_length = 0;
            GWUserItem m_WcfLoginUser = _currentUserItem;
            Dictionary<UInt64, GrpcYxItem> YX_dict = new Dictionary<ulong, GrpcYxItem>();

            lock (DataCenter.EquipItemDict)
            {
                List<YXItem> items = new List<YXItem>();
                foreach (KeyValuePair<int, EquipItem> entry in DataCenter.EquipItemDict)
                {
                    lock (entry.Value.YXItemDict)
                    {
                        foreach (KeyValuePair<int, YXItem> entry1 in entry.Value.YXItemDict)
                        {
                            if (bRegisterDataSession)
                            {
                                if (!IsDllRegisterData4Yxp(entry1.Value.Equip_no, entry1.Value.Yx_no))
                                    continue;
                            }
                            ProxyYxItem item = new ProxyYxItem
                            {
                                m_iEquipNo = entry1.Value.Equip_no,
                                m_iYXNo = entry1.Value.Yx_no,
                                m_YXNm = entry1.Value.Yx_nm
                            };
                            if (entry1.Value.YXValue.GetType() == typeof(string))
                            {
                                item.m_YXValue.s = entry1.Value.YXValue.ToString();
                            }
                            else
                            {
                                item.m_YXValue.temp = Convert.ToBoolean(entry1.Value.YXValue) == true ? 1 : -1;
                            }

                            item.m_YXState = entry1.Value.YXState;
                            item.m_IsAlarm.temp = Convert.ToBoolean(entry1.Value.IsAlarm) == true ? 1 : -1;
                            item.m_AdviceMsg = entry1.Value.Comments;
                            item.m_Bufang = entry1.Value.Bufang;
                            item.m_bHasHistoryXcurve = entry1.Value.Curve_rcd;

                            item.m_related_pic = entry1.Value.Related_pic;
                            item.m_related_video = entry1.Value.related_video;
                            item.m_PlanNo = entry1.Value.PlanNo;
                            item.m_ZiChanID = entry1.Value.ZiChanID;

                            YX_dict.Add((((UInt64)(item.m_iEquipNo) << 32) + (UInt64)item.m_iYXNo), item.Map<GrpcYxItem>());
                        }
                    }
                }
            }
            return YX_dict;
        }
        public List<YXItem> GetTotalRTYXItemDataEx(bool bindEvent = false)
        {
            index_currect = 0;
            remain_length = 0;
            GWUserItem m_WcfLoginUser = _currentUserItem;
            List<YXItem> items = new List<YXItem>();
            lock (DataCenter.EquipItemDict)
            {
                foreach (KeyValuePair<int, EquipItem> entry in DataCenter.EquipItemDict)
                {
                    lock (entry.Value.YXItemDict)
                    {
                        items.AddRange(GetYXItemValue(entry.Value, bindEvent));
                    }
                }
            }
            return items;
        }
        private void BindAllEquipYXItems(bool bindEvent = false)
        {
            lock (DataCenter.EquipItemDict)
            {
                foreach (KeyValuePair<int, EquipItem> entry in DataCenter.EquipItemDict)
                {
                    lock (entry.Value.YXItemDict)
                    {
                        BindYXValueChanged(entry.Value, bindEvent);
                    }
                }
            }
            RecordTime("BindEquipChanged");

        }
        private List<YXItem> GetYXItemValue(EquipItem equipItem, bool bindEvent = false)
        {
            List<YXItem> yXItems = new List<YXItem>();
            foreach (KeyValuePair<int, YXItem> entry1 in equipItem.YXItemDict)
            {
                if (bRegisterDataSession)
                {
                    if (!IsDllRegisterData4Yxp(entry1.Value.Equip_no, entry1.Value.Yx_no))
                        continue;
                }
                ProxyYxItem item = new ProxyYxItem();
                item.m_iEquipNo = entry1.Value.Equip_no;
                item.m_iYXNo = entry1.Value.Yx_no;
                item.m_YXNm = entry1.Value.Yx_nm;
                if (entry1.Value.YXValue.GetType() == typeof(string))
                {
                    item.m_YXValue.s = entry1.Value.YXValue.ToString();
                }
                else
                {
                    item.m_YXValue.temp = Convert.ToBoolean(entry1.Value.YXValue) == true ? 1 : -1;
                    item.m_YXValue.b = Convert.ToBoolean(entry1.Value.YXValue);
                }

                item.m_YXState = entry1.Value.YXState;
                item.m_IsAlarm.temp = Convert.ToBoolean(entry1.Value.IsAlarm) == true ? 1 : -1;
                item.m_AdviceMsg = entry1.Value.Comments;
                item.m_Bufang = entry1.Value.Bufang;
                item.m_bHasHistoryXcurve = entry1.Value.Curve_rcd;

                item.m_related_pic = entry1.Value.Related_pic;
                item.m_related_video = entry1.Value.related_video;
                item.m_PlanNo = entry1.Value.PlanNo;
                item.m_ZiChanID = entry1.Value.ZiChanID;

                var key = (((UInt64)(item.m_iEquipNo) << 32) + (UInt64)item.m_iYXNo);

                yXItems.Add(entry1.Value);
            }
            return yXItems;
        }

        private void BindYXValueChanged(EquipItem equipItem, bool bindEvent = false)
        {
            foreach (KeyValuePair<int, YXItem> entry1 in equipItem.YXItemDict)
            {
                if (bRegisterDataSession)
                {
                    if (!IsDllRegisterData4Yxp(entry1.Value.Equip_no, entry1.Value.Yx_no))
                        continue;
                }

                if (bindEvent)
                    entry1.Value.ValueChanged += new YXItem.YXValueChangedEventHandler(YXValue_ValueChanged);
            }
        }

        void YXValue_ValueChanged(object sender, EventArgs e)
        {
            YXItem o = (YXItem)sender;
            ProxyYxItem item = new ProxyYxItem(o);
            OnYXChanged?.Invoke(o, item);
            o = null;
            item = null;
        }

        public void GetChangedRTYXItemData()
        {
            index_currect = 0;
            remain_length = 0;

            Dictionary<UInt64, ProxyYxItem> dict = new Dictionary<ulong, ProxyYxItem>();
        }

        bool IsDllRegisterData4Equip(int iEqpNo)
        {
            foreach (RemoteRegisterData d in RegisterDataList)
            {
                if (d.iEquipNo == iEqpNo)
                    return true;
            }
            return false;
        }
        bool IsDllRegisterData4Ycp(int iEqpNo, int iYcYxNo)
        {
            foreach (RemoteRegisterData d in RegisterDataList)
            {
                if (d.iEquipNo == iEqpNo)
                {
                    if (d.iYCYxNo == iYcYxNo)
                        if (d.strType == "C")
                            return true;
                }
            }
            return false;
        }
        bool IsDllRegisterData4Yxp(int iEqpNo, int iYcYxNo)
        {
            foreach (RemoteRegisterData d in RegisterDataList)
            {
                if (d.iEquipNo == iEqpNo)
                {
                    if (d.iYCYxNo == iYcYxNo)
                        if (d.strType == "X")
                            return true;
                }
            }
            return false;
        }

        public bool ReadNextBuffer()
        {
            return false;
        }

        public byte[] GetCurrentBuffer()
        {
            return null;
        }
        [Obsolete]
        public int[] GetAddRTEquipItemData()
        {
            lock (AddEquipList)
            {
                foreach (int iEqpNo in AddEquipList)
                {
                    BindEventToNewEquip(iEqpNo);
                }
                int[] bs = AddEquipList.ToArray();
                AddEquipList.Clear();
                return bs;
            }
        }

        public bool BindEventToNewEquip(int iEqpNo)
        {
            EquipItem EItem = DataCenter.GetEquipItem(iEqpNo);
            if (EItem == null)
                EItem = GetEquipItemFromEquipNo(iEqpNo);
            if (EItem == null)
                return false;

            EItem.EqpStateChanged += new EquipItem.EqpStateChangedEventHandler(EquipState_StateChanged);
            foreach (KeyValuePair<int, YCItem> entry1 in EItem.YCItemDict)
            {
                entry1.Value.ValueChanged += new YCItem.YCValueChangedEventHandler(YCValue_ValueChanged);
            }
            foreach (KeyValuePair<int, YXItem> entry1 in EItem.YXItemDict)
            {
                entry1.Value.ValueChanged += new YXItem.YXValueChangedEventHandler(YXValue_ValueChanged);
            }
            return true;
        }

        public bool BindEventToEditEquip(int iEqpNo)
        {
            EquipItem EItem = DataCenter.GetEquipItem(iEqpNo);
            if (EItem == null)
                EItem = GetEquipItemFromEquipNo(iEqpNo);
            if (EItem == null)
                return false;
            EItem.EqpStateChanged += new EquipItem.EqpStateChangedEventHandler(EquipState_StateChanged);
            foreach (KeyValuePair<int, YCItem> entry1 in EItem.YCItemDict)
            {
                entry1.Value.ValueChanged -= new YCItem.YCValueChangedEventHandler(YCValue_ValueChanged);
                entry1.Value.ValueChanged += new YCItem.YCValueChangedEventHandler(YCValue_ValueChanged);
            }
            foreach (KeyValuePair<int, YXItem> entry1 in EItem.YXItemDict)
            {
                entry1.Value.ValueChanged -= new YXItem.YXValueChangedEventHandler(YXValue_ValueChanged);
                entry1.Value.ValueChanged += new YXItem.YXValueChangedEventHandler(YXValue_ValueChanged);
            }
            return true;
        }

        private EquipItem GetEquipItemFromEquipNo(int iEquipNo)
        {
            lock (StationItem.EquipCategoryDict)
            {
                foreach (KeyValuePair<string, object> keyValuePair in StationItem.EquipCategoryDict)
                {
                    var value = (SubEquipList)keyValuePair.Value;
                    if (value == null) continue;
                    lock (value.EquipList)
                        return value?.EquipList.Cast<EquipItem>().FirstOrDefault(equip => equip.iEquipno == iEquipNo);
                }
            }
            return (EquipItem)null;
        }

        ObservableCollection<RealTimeEventItem> RealTimeEventItemList_Now = new ObservableCollection<RealTimeEventItem>();
        List<RealTimeEventItem> RealTimeEventItemList_Add = new List<RealTimeEventItem>();
        List<RealTimeEventItem> RealTimeEventItemList_Del = new List<RealTimeEventItem>();

        WcfRealTimeEventItem wcfRealTimeEventItem;
        public List<WcfRealTimeEventItem> FirstGetRealEventItem(bool isFirst = false, int lastCount = 0)
        {
            try
            {
                index_currect = 0;
                remain_length = 0;
                Stopwatch stopwatch = new Stopwatch();

                stopwatch.Start();
                List<RealTimeEventItem> realTimeEventItems = MessageService.GetEventList().ToList();

                if (_currentUserItem != null)
                    GetUserRealTimeEventList(realTimeEventItems, _currentUserItem);

                IEnumerable<RealTimeEventItem> list = realTimeEventItems.OrderByDescending(u => u.Time).Take(lastCount > 0 ? lastCount : 100);


                if (isFirst)
                    MessageService.GetEventList().CollectionChanged += EditEventItem;
                WriteExtremeDataCount();

                var result = list.Select(item => CreateWcfRealTimeEventItem(null, item)).ToList();

                return result;

            }
            catch (Exception)
            {
                return null;
            }
        }
#if debug
        public static IEnumerable<RealTimeEventItem> staticRealtime = null;
        public static IEnumerable<RealTimeEventItem> GetMockRealtime()
        {
            if (staticRealtime == null)
            {
                staticRealtime = Enumerable.Range(0, 1000).Select((u) =>
                {
                    var time = System.DateTime.Now.Date.AddHours(10).AddSeconds(-u).AddMilliseconds(-u);
                    return new RealTimeEventItem { EventMsg = "12345", Time = time, TimeID = time.Ticks, GUID = new Guid().ToString() };
                }).ToList();
            }
            return staticRealtime;
        } 
#endif
        public PaginationData CreatePaginationRealEventItems(Pagination pagination)
        {
            var realTimePageModel = pagination.WhereCause.FromJson<RealTimeFilterPageModel>();

            var query = MessageService.GetEventList().ToList();

            if (_currentUserItem != null)
                GetUserRealTimeEventList(query, _currentUserItem);
            if (realTimePageModel != null)
            {
                if (!string.IsNullOrWhiteSpace(realTimePageModel.EventType))
                {
                    var eventType = realTimePageModel.EventType.Split(",", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);

                    query = query.Where(d => eventType.Contains(d.Level))./*OrderByDescending(d => d.Time)*/ToList();
                }

                if (realTimePageModel.Equips != null)
                {
                    query = query.Where(x => realTimePageModel.Equips.Contains(x.Equipno)).ToList();
                }
                if (realTimePageModel.StartTime != null)
                {
                    query = query.Where(x => x.Time > realTimePageModel.StartTime && x.Time < realTimePageModel.EndTime).ToList();
                }
                if (realTimePageModel.Level != null && realTimePageModel.Level.Any())
                {
                    query = query.Where(x => realTimePageModel.Level.Contains(x.Level)).ToList();
                }
                if (realTimePageModel.Confirmed.HasValue)
                {
                    query = query.Where(d => d.bConfirmed == realTimePageModel.Confirmed).ToList();
                }
                if (!string.IsNullOrWhiteSpace(realTimePageModel.Guid))
                {
                    query = query.Where(d => d.GUID == realTimePageModel.Guid).ToList();
                }
            }

            IEnumerable<WcfRealTimeEventItem> list = query.OrderByDescending(u => u.Time).Skip((pagination.PageIndex - 1) * pagination.PageSize).Take(pagination.PageSize).Select(item => CreateWcfRealTimeEventItem(null, item)).ToList();
            return new PaginationData
            {
                Data = list.ToJson(),
                Total = query.Count()
            };
        }

        public WcfRealTimeEventItem GetRealTimeEventItem(string guid)
        {
            var item = MessageService.GetEventList().FirstOrDefault(u => u.GUID == guid);
            if (item != null)
                return CreateWcfRealTimeEventItem(null, item);
            return new WcfRealTimeEventItem();
        }

        public bool Contains(string guid)
        {
            return MessageService.GetEventList().FirstOrDefault(u => u.GUID == guid) != null;
        }
        public IEnumerable<RealTimeGroupCount> GetRealTimeGroupCount()
        {
            var realTimeEventItems = MessageService.GetEventList().ToList();

            if (_currentUserItem != null)
                GetUserRealTimeEventList(realTimeEventItems, _currentUserItem);

            return realTimeEventItems.Where(u => !u.bConfirmed).GroupBy(item => item.Level).Select(item => new RealTimeGroupCount { Level = item.Key, Total = item.Count() });
        }

        private void WriteExtremeDataCount()
        {
#if DEBUG || DEVELOPMENT
#endif
        }

        public void GetAddRealEventItem()
        {
            index_currect = 0;
            remain_length = 0;


            for (int k = 0; k < RealTimeEventItemList_Add.Count(); k++)
            {
                wcfRealTimeEventItem = new WcfRealTimeEventItem();
                CreateWcfRealTimeEventItem(wcfRealTimeEventItem, RealTimeEventItemList_Add[k]);
            }


        }

        public void GetDelRealEventItem()
        {

        }

        void GetUserRealTimeEventList(List<RealTimeEventItem> List, GWUserItem m_WcfLoginUser)
        {
            bool bCanSeeZC = false;// m_WcfLoginUser.CanOpen2Addin("AlarmCenter.GWAssetsManagement.GWAssetsManagementPage");
            for (int k = List.Count - 1; k >= 0; k--)
            {
                if (List[k] == null) continue;

                if (List[k].Equipno == -1)//资产管理
                {
                    if (bCanSeeZC) continue;
                }

                if (!m_WcfLoginUser.CanBrowse2Equip(List[k].Equipno))
                {
                    List.RemoveAt(k);
                }
            }
        }

        void EditEventItem(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            try
            {
                if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
                {
                    if (e.NewItems != null && e.NewItems.Count > 0)
                        AddEventItem(e.NewItems[0]);
                }
                else if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
                {
                    if (e.OldItems != null && e.OldItems.Count > 0)
                        DelectEventItem(e.OldItems[0]);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"绑定实时快照错误：{ex}");
            }
        }


        void AddEventItem(object sender)
        {
            var wcfRealtime = CreateWcfRealTimeEventItem(null, (RealTimeEventItem)sender);
            AddRealTimeSnapshot?.Invoke(sender, wcfRealtime);
            RealTimeEventItem EventItem = new RealTimeEventItem((RealTimeEventItem)sender);
        }

        void DelectEventItem(object sender)
        {
            var wcfRealtime = CreateWcfRealTimeEventItem(null, (RealTimeEventItem)sender);
            DeleteRealTimeSnapshot?.Invoke(sender, wcfRealtime);
        }

        WcfRealTimeEventItem CreateWcfRealTimeEventItem(WcfRealTimeEventItem wcfitem, RealTimeEventItem item)
        {
            if (wcfitem == null)
                wcfitem = new WcfRealTimeEventItem();
            wcfitem.Equipno = item.Equipno;
            wcfitem.EventMsg = item.EventMsg;
            wcfitem.Level = item.Level;
            wcfitem.Proc_advice_Msg = item.Proc_advice_Msg;
            wcfitem.Time = item.Time;
            wcfitem.Type = item.Type;
            wcfitem.Wavefile = item.Wavefile;
            wcfitem.Related_pic = item.Related_pic;
            wcfitem.Ycyxno = item.Ycyxno;
            wcfitem.bConfirmed = item.bConfirmed;

            wcfitem.DT_Confirmed = item.DT_Confirm;
            wcfitem.User_Confirmed = item.User_confirm;
            wcfitem.m_related_video = item.Related_video;
            wcfitem.ZiChanID = item.ZiChanID;
            wcfitem.PlanNo = item.PlanNo;
            wcfitem.GUID = item.GUID;
            return wcfitem;
        }

        public void SetParm(int iEquipNo, string strCMD1, string strCMD2, string strCMD3, string strUser, string Instance_GUID)
        {
            SetItem item = new SetItem(iEquipNo, strCMD1, strCMD2, strCMD3, strUser)
            {
                UserIPandPort = IPandPort,
                Client_Instance_GUID = Instance_GUID
            };
            StationItem.GetEquipItemFromEquipNo(iEquipNo).AddSetItem(item);
        }
        public void SetParmEx(int iEquipNo, string strCMD1, string strCMD2, string strCMD3, string strUser, string Instance_GUID, string requestId, Action<string> action)
        {
            SetItem item = new SetItem(iEquipNo, strCMD1, strCMD2, strCMD3, strUser)
            {
                UserIPandPort = IPandPort,
                Client_Instance_GUID = Instance_GUID,
                RequestId = requestId,
            };
            StationItem.SetParmResultEvent += (sender, e) =>
            {
                var item = sender as SetItem;
            };


            StationItem.GetEquipItemFromEquipNo(iEquipNo).AddSetItem(item);
        }



        private void StationItem_SetParmResultEvent(object sender, EventArgs e)
        {
            StationItem.SetParmResultEvent -= StationItem_SetParmResultEvent;
        }

        public void FireSetParmResponseEvent(int iEquipNo, int iSetNo, string Value, string Response, bool bFinish, string GUID, Action<SetParmRequestModel> action)
        {
        }

        public void SetParm_1(int iEquipNo, string strCMD1, string strCMD2, string strCMD3, string strUser, bool bShowDlg, string Instance_GUID)
        {
            SetItem item = new SetItem(iEquipNo, strCMD1, strCMD2, strCMD3, strUser);
            item.UserIPandPort = IPandPort;
            item.bShowDlg = bShowDlg;
            item.Client_Instance_GUID = Instance_GUID;
            StationItem.GetEquipItemFromEquipNo(iEquipNo).AddSetItem(item);
        }


        public void SetParm1(int EquipNo, int SetNo, string strUser, string Instance_GUID)
        {
            var setParmTable = DataCenter.GetDataRowFromSetParm(EquipNo, SetNo);
            string strCMD1, strCMD2, strCMD3;
            if (setParmTable != null)
            {
                strCMD1 = Convert.ToString(setParmTable.main_instruction);
                strCMD2 = Convert.ToString(setParmTable.minor_instruction);
                strCMD3 = Convert.ToString(setParmTable.value);
                SetItem item = new SetItem(EquipNo, strCMD1, strCMD2, strCMD3, strUser)
                {
                    UserIPandPort = IPandPort,
                    Client_Instance_GUID = Instance_GUID
                };
                StationItem.GetEquipItemFromEquipNo(EquipNo).AddSetItem(item);
            }
        }

        public void SetParm1_1(int EquipNo, int SetNo, string strValue, string strUser, bool bShowDlg, string Instance_GUID, string requestId = "")
        {
            var setParmTableRow = DataCenter.GetDataRowFromSetParm(EquipNo, SetNo);
            string strCMD1, strCMD2, strCMD3;
            if (setParmTableRow != null)
            {
                strCMD1 = Convert.ToString(setParmTableRow.main_instruction);
                strCMD2 = Convert.ToString(setParmTableRow.minor_instruction);
                if (string.IsNullOrEmpty(strValue))
                    strCMD3 = Convert.ToString(setParmTableRow.value);
                else
                    strCMD3 = strValue;
                SetItem item = new SetItem(EquipNo, SetNo, strCMD1, strCMD2, strCMD3, strUser);
                item.UserIPandPort = IPandPort;
                item.Client_Instance_GUID = Instance_GUID;
                item.bShowDlg = bShowDlg;
                if (!string.IsNullOrEmpty(requestId))
                    item.RequestId = requestId;
                StationItem.GetEquipItemFromEquipNo(EquipNo).AddSetItem(item);
            }
        }
        public void SetParm2(int iEquipNo, string strCMD1, string strCMD2, string strCMD3, string strType, string strUser, string Instance_GUID)
        {
            try
            {
                SetItem item = new SetItem(iEquipNo, strCMD1, strCMD2, strCMD3, strType, strUser);
                item.UserIPandPort = IPandPort;
                item.Client_Instance_GUID = Instance_GUID;
                StationItem.GetEquipItemFromEquipNo(iEquipNo).AddSetItem(item);
            }
            catch (Exception)
            {
            }

        }
        public void SetParm2_1(int iEquipNo, string strCMD1, string strCMD2, string strCMD3, string strType, string strUser, bool bShowDlg, string Instance_GUID)
        {
            SetItem item = new SetItem(iEquipNo, strCMD1, strCMD2, strCMD3, strType, strUser);
            item.UserIPandPort = IPandPort;
            item.Client_Instance_GUID = Instance_GUID;
            item.bShowDlg = bShowDlg;
            StationItem.GetEquipItemFromEquipNo(iEquipNo).AddSetItem(item);
        }

        public void DoSetParmFromString(string csParmStr)
        {
            try
            {

            }
            catch (Exception)
            {
            }
        }

        public void SetVariables2Null()
        {
            lock (DataCenter.EquipItemDict)
            {
                foreach (KeyValuePair<int, EquipItem> entry in DataCenter.EquipItemDict)
                {
                    entry.Value.EqpStateChanged -= EquipState_StateChanged;
                    foreach (KeyValuePair<int, YCItem> entry1 in entry.Value.YCItemDict)
                    {
                        entry1.Value.ValueChanged -= YCValue_ValueChanged;
                    }
                    foreach (KeyValuePair<int, YXItem> entry1 in entry.Value.YXItemDict)
                    {
                        entry1.Value.ValueChanged -= YXValue_ValueChanged;
                    }
                }
            }
            RealTimeEventItemList_Now.Clear();
            RealTimeEventItemList_Add.Clear();
            RealTimeEventItemList_Del.Clear();
        }

        public string DoEquipSetItem(int EquipNo, int SetNo, string strValue, string strUser, bool bShowDlg, string Instance_GUID, string requestId = "")
        {


            return string.Empty;
        }

        public void Dispose()
        {
            lock (StationItem.EquipCategoryDict)
            {
                foreach (KeyValuePair<string, object> pair in StationItem.EquipCategoryDict)
                {
                    SubEquipList EquipList = (SubEquipList)pair.Value;
                    EquipList.EquipAdd -= EquipList_EquipAdd;
                    EquipList.EquipDel -= EquipList_EquipDel;
                }
            }
            AddEquipList.Clear();
            DelEquipList.Clear();
            SetVariables2Null();
        }
    }
}
