// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Utilities;
using IoTCenter.Utilities.Extensions;
using IoTCenterHost.Core.Abstraction;
using IoTCenterHost.Core.Abstraction.BaseModels;
using IoTCenterHost.Core.Abstraction.ProxyModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IoTCenterHost.Proxy.Connect
{
    public class EquipManager : IDisposable, IEquipManager
    {
        private bool isCaching = true;
        private bool disposed = false;
        private readonly ILoggingService _logService;
        private readonly IServiceProvider _serviceProvider;
        private static readonly ParallelOptions parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = Math.Max(Environment.ProcessorCount / 2, 1)
        };

        public EquipManager(IServiceProvider serviceProvider)
        {
            _logService = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<ILoggingService>();
        }

        public EquipManager(int gatewayId, string url, bool cached)
        {
            GatewayId = gatewayId;
            Url = url;
            isCaching = cached;
        }
        public int GatewayId { get; }
        public string Url { get; }
        public ConcurrentDictionary<int, GrpcEquipItem> Equips { protected set; get; } = new ConcurrentDictionary<int, GrpcEquipItem>();

        public ConcurrentDictionary<int, GrpcEquipState> EquipStates { protected set; get; } = new ConcurrentDictionary<int, GrpcEquipState>();

        public ConcurrentDictionary<int, ConcurrentDictionary<int, GrpcYcItem>> EquipYcItems { protected set; get; } = new ConcurrentDictionary<int, ConcurrentDictionary<int, GrpcYcItem>>();

        public ConcurrentDictionary<int, ConcurrentDictionary<int, GrpcYxItem>> EquipYxItems { protected set; get; } = new ConcurrentDictionary<int, ConcurrentDictionary<int, GrpcYxItem>>();


        public event EventHandler<GrpcEquipStateItem> OnEquipStateChanged;

        public event EventHandler<GrpcYcItem> OnYcValuedChanged;

        public event EventHandler<GrpcYxItem> OnYxValuedChanged;

        public event EventHandler<string> OnOpenWindow;

        public event EventHandler<WcfRealTimeEventItem> OnRealTimeEventAdded;


        public void AddEquips(IEnumerable<GrpcEquipItem> grpcEquipItems)
        {
            Equips = new ConcurrentDictionary<int, GrpcEquipItem>(grpcEquipItems.ToDictionary(u => u.m_iEquipNo));
        }


        public void OnLine(Dictionary<int, GrpcEquipState> keyValuePairs, out string msg)
        {
            msg = string.Empty;
            try
            {
                Parallel.ForEach(keyValuePairs, parallelOptions, kvp =>
                {
                    var item = new GrpcEquipStateItem { m_iEquipNo = kvp.Key, m_State = kvp.Value };
                    EquipStateChanged(item, true);
                });
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
        }



        public void AddEquip(GrpcEquipItem grpcEquipItem, bool eventHandler)
        {
            if (grpcEquipItem == null) return;

            EquipStates?.TryAdd(grpcEquipItem.m_iEquipNo, grpcEquipItem.m_State);

            EquipYcItems.TryAdd(grpcEquipItem.m_iEquipNo, new ConcurrentDictionary<int, GrpcYcItem>());
            EquipYxItems.TryAdd(grpcEquipItem.m_iEquipNo, new ConcurrentDictionary<int, GrpcYxItem>());

            OnEquipStateChanged?.Invoke(this, new GrpcEquipStateItem { m_iEquipNo = grpcEquipItem.m_iEquipNo, m_State = grpcEquipItem.m_State });
        }



        public void EditEquip(GrpcEquipItem grpcEquipItem, bool eventHandler)
        {
            if (!EquipStates.ContainsKey(grpcEquipItem.m_iEquipNo))
                EquipStates.TryAdd(grpcEquipItem.m_iEquipNo, grpcEquipItem.m_State);
            OnEquipStateChanged?.Invoke(this, new GrpcEquipStateItem { m_iEquipNo = grpcEquipItem.m_iEquipNo, m_State = grpcEquipItem.m_State });
        }

        public void RemoveEquip(int equipNo)
        {
            EquipStates?.TryRemove(equipNo, out _);
            EquipYcItems?.TryRemove(equipNo, out _);
            EquipYxItems?.TryRemove(equipNo, out _); 
        }

        public void RemoveEquips(int[] equipNos)
        {
            foreach (var item in equipNos)
                RemoveEquip(item);
        }

        public void OffLine(IEnumerable<int> equipNos)
        {
            if (equipNos == null) return;

            var equipNoList = equipNos as IList<int> ?? equipNos.ToList();
            if (!equipNoList.Any()) return;

            Parallel.ForEach(equipNoList, parallelOptions, equipNo =>
            {
                var item = new GrpcEquipStateItem
                {
                    m_iEquipNo = equipNo,
                    m_State = GrpcEquipState.NoCommunication
                };
                EquipStateChanged(item, true);
            });
        }

        public void YcItemChanged(GrpcYcItem item, bool eventHandler, out string msg)
        {
            msg = string.Empty;
            try
            {
                if (isCaching)
                {
                    if (!EquipYcItems.ContainsKey(item.m_iEquipNo))
                    {
                        var dict = new ConcurrentDictionary<int, GrpcYcItem>();
                        dict.TryAdd(item.m_iYCNo, item);
                        EquipYcItems.TryAdd(item.m_iEquipNo, dict);
                    }
                    var ycList = EquipYcItems[item.m_iEquipNo];

                    if (ycList.TryGetValue(item.m_iYCNo, out GrpcYcItem cacheYxItem))
                    {
                        if (cacheYxItem.TimeStamp > item.TimeStamp) return;
                    }
                    ycList.AddOrUpdate(item.m_iYCNo, item, (i, u) => item);
                    
                }
                if (eventHandler)
                    OnYcValuedChanged?.Invoke(this, item);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
        }


        public void YxItemChanged(GrpcYxItem item, bool eventHandler)
        {
            try
            {
                if (isCaching)
                {
                    if (!EquipYxItems.ContainsKey(item.m_iEquipNo))
                    {
                        var dict = new ConcurrentDictionary<int, GrpcYxItem>();
                        dict.TryAdd(item.m_iYXNo, item);
                        EquipYxItems.TryAdd(item.m_iEquipNo, dict);
                    }
                    if (!EquipYxItems.ContainsKey(item.m_iEquipNo))
                        return;
                    var yxList = EquipYxItems[item.m_iEquipNo];

                    if (yxList.TryGetValue(item.m_iYXNo, out GrpcYxItem cacheYxItem))
                    {
                        if (cacheYxItem.TimeStamp > item.TimeStamp) return;
                    }
                    yxList.AddOrUpdate(item.m_iYXNo, item, (i, u) => item);
                }
                if (eventHandler)
                    OnYxValuedChanged?.Invoke(this, item);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void EquipStateChanged(GrpcEquipStateItem grpcEquipItem, bool eventHandler)
        {
            if (grpcEquipItem == null)  return;

            var equipNo = grpcEquipItem.m_iEquipNo;
            var newState = grpcEquipItem.m_State;

            bool needRaiseEvent = false;

            if (EquipStates.TryGetValue(equipNo, out var oldState))
            {
                if (!Equals(oldState, newState))
                {
                    EquipStates[equipNo] = newState;
                    needRaiseEvent = true;
                }
            }
            else
            {
                if (EquipStates.TryAdd(equipNo, newState))
                {
                    needRaiseEvent = true;
                }
            }

            if (needRaiseEvent)
            {
                OnEquipStateChanged?.Invoke(this, grpcEquipItem);
            }
        }

        public void AddYcItems(IEnumerable<GrpcYcItem> grpcYcItems, out string msg)
        {
            msg = string.Empty;
            try
            {
                if (!isCaching)  return;
                var notNullGrpcYcItems = grpcYcItems.Where(yc => yc != null).ToList();
                Parallel.ForEach(Partitioner.Create(notNullGrpcYcItems), parallelOptions, u => YcItemChanged(u, false, out _));
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
            }
        }

        public void AddYxItems(IEnumerable<GrpcYxItem> grpcYxItems, out string msg)
        {
            msg = string.Empty;
            try
            {
                if (!isCaching) return;

                var notNullGrpcYxItems = grpcYxItems.Where(yc => yc != null).ToList();
                Parallel.ForEach(Partitioner.Create(notNullGrpcYxItems), parallelOptions, u => YxItemChanged(u, false));
            }
            catch (Exception ex)
            {
                msg = ex.ToString();
            }
        }

        public Dictionary<int, GrpcEquipState> GetEquipState()
        {
            return EquipStates?.ToDictionary(kev => kev.Key, kev => kev.Value);
        }


        public object GetYCValue(int iEquipNo, int iYcpNo)
        {
            if (EquipYcItems.TryGetValue(iEquipNo, out var equipItem) && equipItem.TryGetValue(iYcpNo, out var ycItem))
            {
                object value = ycItem.m_YCValue.f;
                if (value == null) value = ycItem.m_YCValue.s;
                return value;
            }
            return null;
        }

        public DateTime GetDataTime(int equipNo, int pointNo, string pointType)
        {
            switch (pointType.ToLower())
            {
                case "c":
                    var ycItem = EquipYcItems[equipNo][pointNo];
                    if (ycItem != null)
                    {
                        return ycItem.TimeStamp.FromUnixTimeStampMillisecond();
                    }
                    break;
                case "x":
                    break;
            }
            return DateTime.MinValue;
        }



        public Dictionary<int, object> GetYCValueDictFromEquip(int iEquipNo)
        {
            if (!EquipYcItems.TryGetValue(iEquipNo, out var equipItem))
            {
                return null;
            }
            return equipItem.ToDictionary(u => u.Key, u => u.Value.m_YCValue.s != null ? u.Value.m_YCValue.s.Equals("***") ? null : (object)u.Value.m_YCValue.s : u.Value.m_YCValue.f);
        }

        public Dictionary<int, DateTime> GetEquipDataTime(int equipNo, string pointType)
        {
            if (!EquipYcItems.TryGetValue(equipNo, out var equipItem))
            {
                return null;
            }
            return equipItem.ToDictionary(u => u.Key, u => u.Value.TimeStamp.FromUnixTimeStampMillisecond());
        }

        public bool HaveYCP(int EquipNo)
        {
            return EquipYcItems[EquipNo].Any();
        }

        public Dictionary<int, bool> GetYCAlarmStateDictFromEquip(int iEquipNo)
        {
            if (!EquipYcItems.TryGetValue(iEquipNo, out var equipYcItem))
            {
                return null;
            }
            return equipYcItem.ToDictionary(u => u.Value.m_iYCNo, u => u.Value.m_IsAlarm);
        }

        public bool? GetYCAlarmState(int iEquipNo, int iYcpNo)
        {
            if (EquipYcItems.TryGetValue(iEquipNo, out var equipItem) && equipItem.TryGetValue(iYcpNo, out var ycItem))
            {
                return ycItem.m_IsAlarm;
            }
            return null;
        }

        public string GetYCAlarmComments(int iEqpNo, int iYcpNo)
        {
            if (EquipYcItems.TryGetValue(iEqpNo, out var equipItem) && equipItem.TryGetValue(iYcpNo, out var ycItem))
            {
                return ycItem.m_AdviceMsg;
            }
            return null;
        }

        public object GetYXValue(int iEquipNo, int iYxpNo)
        {
            if (EquipYxItems.TryGetValue(iEquipNo, out var equipItem) && equipItem.TryGetValue(iYxpNo, out var yxItem))
            {
                object value = yxItem.m_YXValue.s;
                if (value == null) value = yxItem.m_YXValue.b;
                return value;
            }
            return null;
        }

        public Dictionary<int, string> GetYXValueDictFromEquip(int iEquipNo)
        {
            if (EquipYxItems.TryGetValue(iEquipNo, out var equipYxItem))
            {
                return equipYxItem.ToDictionary(u => u.Key, u => u.Value.m_YXState);
            }
            return null;
        }

        public bool? HaveYXP(int iEquipNo)
        {
            if (EquipYxItems.TryGetValue(iEquipNo, out var equipYxItem))
            {
                return equipYxItem.Any();
            }
            return false;
        }

        public Dictionary<int, bool> GetYXAlarmStateDictFromEquip(int iEquipNo)
        {
            if (EquipYxItems.TryGetValue(iEquipNo, out var equipYxItem))
            {
                return equipYxItem.ToDictionary(u => u.Value.m_iYXNo, u => u.Value.m_IsAlarm.b);
            }
            return null;
        }

        public bool? GetYXAlarmState(int iEquipNo, int iYxpNo)
        {
            if (EquipYxItems.TryGetValue(iEquipNo, out var equipItem) && equipItem.TryGetValue(iYxpNo, out var yxItem))
            {
                return yxItem.m_IsAlarm.b;
            }
            return null;
        }


        public void RealTimeEventAdd(WcfRealTimeEventItem wcfRealTimeEventItem)
        {
            OnRealTimeEventAdded?.Invoke(this, wcfRealTimeEventItem);
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;
            if (disposing)
            {
                Equips = new ConcurrentDictionary<int, GrpcEquipItem>();
                EquipStates = new ConcurrentDictionary<int, GrpcEquipState>();
                EquipYcItems = new ConcurrentDictionary<int, ConcurrentDictionary<int, GrpcYcItem>>();
                EquipYxItems = new ConcurrentDictionary<int, ConcurrentDictionary<int, GrpcYxItem>>();
            }
            disposed = true;
        }
    }
}
