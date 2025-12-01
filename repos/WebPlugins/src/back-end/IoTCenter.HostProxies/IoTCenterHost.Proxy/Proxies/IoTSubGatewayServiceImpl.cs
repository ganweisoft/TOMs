// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Health.V1;
using IoTCenter.Utilities;
using IoTCenterHost.Core.Abstraction;
using IoTCenterHost.Core.Abstraction.AppServices;
using IoTCenterHost.Core.Abstraction.BaseModels;
using IoTCenterHost.Core.Abstraction.ProxyModels;
using IoTCenterHost.Proto;
using IoTCenterHost.Proxy.Connect;
using IoTCenterHost.Proxy.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static IoTCenterHost.Proto.iotsubgatewayContract;
namespace IoTCenterHost.Proxy.Proxies
{
    public class IoTSubGatewayServiceImpl : IoTSubGatewayService
    {
        private bool _openToken = false;
        private string _gatewayAddr;
        
        private IConnectStatusProvider _connectStatusProvider;
        private readonly IoTCenter.Utilities.IMemoryCacheService _memoryCacheService;
        private readonly ILoggingService _loggingService;
        private readonly IoTServiceClientFactory _iotServiceFactory;
        private readonly IEquipBaseClientAppService _equipBaseClientAppService;
        private readonly IYCClientAppService _yCClientAppService;
        private readonly IYXClientAppService _yXClientAppService;
        private System.Threading.Timer threadTimer { get; set; }
        private int _keepAliveExpired;
        private bool _isPushBegin;
        private string context = System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType?.Namespace;
        private const string pushingStatus = "isPushing";
        private const int pushingCacheTimeStamp = 3;
        private List<Task> TaskList { get; set; }
        private Task timerTask { get; set; }

        private ConcurrentDictionary<string, ConnectContext> _gateWayCts = new ();

        private IEnumerable<int>? _debugEquipNos;


        public IoTSubGatewayServiceImpl(IConnectStatusProvider connectStatusProvider,
            IMemoryCacheService memoryCacheService,
            ILoggingService loggingService,
            IoTServiceClientFactory ioTServiceFactory,
            IEquipBaseClientAppService equipBaseClientAppService,
            IYCClientAppService yCClientAppService,
            IYXClientAppService yXClientAppService)
        {
            _connectStatusProvider = connectStatusProvider;
            _memoryCacheService = memoryCacheService;
            _loggingService = loggingService;
            _keepAliveExpired =  10;
            TaskList = new List<Task>();
            _iotServiceFactory = ioTServiceFactory;
            _equipBaseClientAppService  = equipBaseClientAppService;
            _yCClientAppService = yCClientAppService;
            _yXClientAppService = yXClientAppService;
        }

        public bool Connected { get; set; }

        private int tickOn = 6;
        private int tickAdd = 0;
        public event EventHandler<EventArgs> OnConnected;
        public event EventHandler<EventArgs> OnDisConnected;
        public event EventHandler<EventArgs> OnTickChanged;

        public IEnumerable<int>? GetDebugEquipNos()
        {
            return _debugEquipNos;
        }

        public void Connect(string address, int gatewayId, IEquipManager equipManager)
        {
            var cts = new CancellationTokenSource();
            cts.Token.Register(() =>
            {
                try
                {
                    Task.WaitAll(TaskList.ToArray());
                    TaskList.Clear();
                }
                catch (Exception e)
                {
                    _loggingService.Error($"网关地址-{address}，CancellationToken注册的取消函数抛出异常：{e}");
                }
            });

            var connectCtx = new ConnectContext
            {
                GatewayId = gatewayId,
                EquipManager = equipManager,
                Cts = cts
            };

            _gateWayCts.TryAdd(address, connectCtx);
            KeepAlive(address);
        }


        public int KeepAliveInterval
        {
            get
            {
                return _keepAliveExpired;
            }
        }

        public int GatewayId { get; set; }
        string IoTSubGatewayService.Address
        {
            get
            {
                return _gatewayAddr;
            }
        }

        private void SetPushing(string gateWayAddr)
        {
            if (!_gateWayCts.TryGetValue(gateWayAddr, out ConnectContext gateWayCt))
            {
                return;
            }

            string key = $"{gateWayCt.GatewayId}:{pushingStatus}";
            if (!_memoryCacheService.IsSet(key))
            {
                _memoryCacheService.Set(key, 1, System.DateTime.Now.AddSeconds(pushingCacheTimeStamp));
            }
        }

        private bool IsPushing(string gateWayAddr)
        {
            if (!_gateWayCts.TryGetValue(gateWayAddr, out ConnectContext gateWayCt))
            {
                return false;
            }

            return _memoryCacheService.IsSet($"{gateWayCt.GatewayId}:{pushingStatus}");
        }

        private async void KeepAlive(string gateWayAddr)
        {
            await Task.Run(() =>
            {
                int waitTime = (int)TimeSpan.FromSeconds(_keepAliveExpired).TotalMilliseconds;
                if (waitTime > 0)
                {
                    threadTimer = new System.Threading.Timer((object state) =>
                    {
                        if (_gateWayCts.TryGetValue(gateWayAddr, out ConnectContext gateWayCt) && !IsPushing(gateWayAddr))
                            CheckHealthAndConnect(gateWayAddr, gateWayCt.Cts);
                        tickAdd += 1;
                        if (tickAdd == tickOn)
                            OnTickChanged?.Invoke(this, new EventArgs { });
                    }, null, Timeout.Infinite, waitTime);
                    threadTimer.Change(0, waitTime);
                }
            });
        }

        private async void CheckHealthAndConnect(string gateWayAddr, CancellationTokenSource cts)
        {
            try
            {
                _loggingService.Debug($"{gateWayAddr}开始获取健康状态", context);
                var status = HealthCheckResponse.Types.ServingStatus.ServiceUnknown;

                try
                {
                    var healthClient = new Health.HealthClient(_iotServiceFactory.CreateChannel(gateWayAddr));
                    var response = await healthClient.CheckAsync(new HealthCheckRequest());
                    status = response.Status;
                }
                catch (Exception)
                {
                }

                if (status != HealthCheckResponse.Types.ServingStatus.Serving)
                {
                    if (!cts.IsCancellationRequested)
                    {
                        cts.Cancel();
                    }

                    _isPushBegin = false;
                    if (Connected)
                    {
                        Connected = false;
                        OnDisConnected?.Invoke(this, EventArgs.Empty);
                    }
                }
                else if (!Connected)
                {
                    if (cts.IsCancellationRequested && _gateWayCts.TryGetValue(gateWayAddr, out ConnectContext gateWayCt))
                    {
                        var newCts = new CancellationTokenSource();
                        newCts.Token.Register(() =>
                        {
                            try
                            {
                                Task.WaitAll(TaskList.ToArray());
                                TaskList.Clear();
                            }
                            catch (Exception e)
                            {
                                _loggingService.Error($"网关地址-{gateWayAddr}，CancellationToken注册的取消函数抛出异常：{e}");
                            }
                        });

                        gateWayCt.Cts = newCts;
                    }

                    Connected = true;
                    OnConnected?.Invoke(this, EventArgs.Empty);

                    CreateDuplexEquipCallback(gateWayAddr);
                }

                _loggingService.Debug($"{gateWayAddr}获取健康状态已完成，状态为{status}", context);
            }
            catch (Exception ex)
            {
                _loggingService.Error($"获取健康状态失败：{ex}", ex);
            }
        }

        private async void CreateDuplexEquipCallback(string gateWayAddr)
        {
            _loggingService.Info($"{gateWayAddr}开始创建消息订阅.");
            if (!_gateWayCts.TryGetValue(gateWayAddr, out ConnectContext gateWayCt))
            {
                _loggingService.Error($"调用EquipAddEvent事件终止，网关缓存未获取到指定网关地址{gateWayAddr}的信息，使用本地网关.");
                return;
            }

            await this.EquipAddEvent(gateWayAddr, (GrpcEquipItem item) =>
            {
                gateWayCt.EquipManager.AddEquip(item, true);
            });

            await this.EquipChangeEvent(gateWayAddr, (GrpcEquipItem item) =>
            {
                gateWayCt.EquipManager.EditEquip(item, true);
            });
            await this.EquipDeleteEvent(gateWayAddr, (int equipNo) =>
            {
                gateWayCt.EquipManager.RemoveEquip(equipNo);
            });

            await this.EquipStateEvent(gateWayAddr, (GrpcEquipStateItem item) =>
            {
                gateWayCt.EquipManager.EquipStateChanged(item, true);
            });

            await this.YcChangeEvent(gateWayAddr, (GrpcYcItem ycItem) =>
            {
                gateWayCt.EquipManager.YcItemChanged(ycItem, true, out _);
            });

            await this.YxChangeEvent(gateWayAddr, (GrpcYxItem yxItem) =>
            {
                gateWayCt.EquipManager.YxItemChanged(yxItem, true);
            });

            await this.RealtimeSnapAddEvent(gateWayAddr, (WcfRealTimeEventItem realtimeItem) =>
            {
                gateWayCt.EquipManager.RealTimeEventAdd(realtimeItem);
            });

            _loggingService.Info($"{gateWayAddr}创建消息订阅已完成.");

        }

        Task EquipAddEvent(string gateWayAddr, Action<GrpcEquipItem> action, bool isReUse = false)
        {
            if (!_gateWayCts.TryGetValue(gateWayAddr, out ConnectContext gateWayCt))
            {
                _loggingService.Error($"调用EquipAddEvent事件终止，网关缓存未获取到指定网关地址{gateWayAddr}的信息，使用本地网关.");
                return Task.CompletedTask;
            }

            var task = Task.Factory.StartNew(async () =>
            {
                _loggingService.Debug($"网关{gateWayAddr},调用EquipAddEvent事件");
                var stream = new iotsubgatewayContractClient(_iotServiceFactory.CreateChannel(gateWayAddr)).EquipAddEvent(new Google.Protobuf.WellKnownTypes.Empty());
                await foreach (var resp in stream.ResponseStream.ReadAllAsync())
                {
                    try
                    {
                        var equipItem = resp.Result.FromJson<GrpcEquipItem>();

                        _loggingService.Debug($"网关{gateWayAddr},设备新增{resp.Result}已触发", context);

                        action?.Invoke(resp.Result.FromJson<GrpcEquipItem>());
                        SetPushing(gateWayAddr);
                    }
                    catch (Exception ex)
                    {
                        _loggingService.Error($"网关{gateWayAddr}订阅设备新增事件出现错误，异常信息为{ex}");
                    }
                }
                Connected = false;
            }, gateWayCt.Cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

            this.TaskList.Add(task);
            return Task.CompletedTask;
        }

        Task EquipChangeEvent(string gateWayAddr, Action<GrpcEquipItem> action, bool isReUse = false)
        {
            if (!_gateWayCts.TryGetValue(gateWayAddr, out ConnectContext gateWayCt))
            {
                _loggingService.Error($"调用EquipChangeEvent事件终止，网关缓存未获取到指定网关地址{gateWayAddr}的信息，使用本地网关.");
                return Task.CompletedTask;
            }

            var task = Task.Factory.StartNew(async () =>
            {
                _loggingService.Debug($"网关{gateWayAddr},调用EquipChangeEvent事件");
                var stream = new iotsubgatewayContractClient(_iotServiceFactory.CreateChannel(gateWayAddr)).EquipChangeEvent(new Empty());
                await foreach (var resp in stream.ResponseStream.ReadAllAsync())
                {
                    try
                    {
                        var equipItem = resp.Result.FromJson<GrpcEquipItem>();
                        _loggingService.Debug($"网关{gateWayAddr},设备修改{resp.Result}已触发", context);
                        action?.Invoke(equipItem);
                        SetPushing(gateWayAddr);
                    }
                    catch (Exception ex)
                    {
                        _loggingService.Error($"网关{gateWayAddr}订阅EquipChangeEvent事件出现错误，异常信息为,{ex}");
                    }
                }

                Connected = false;
            }, gateWayCt.Cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

            this.TaskList.Add(task);
            return Task.CompletedTask;
        }

        Task EquipDeleteEvent(string gateWayAddr, Action<int> action, bool isReUse = false)
        {
            if (!_gateWayCts.TryGetValue(gateWayAddr, out ConnectContext gateWayCt))
            {
                _loggingService.Error($"调用EquipDeleteEvent事件终止，网关缓存未获取到指定网关地址{gateWayAddr}的信息，使用本地网关.");
                return Task.CompletedTask;
            }

            var task = Task.Factory.StartNew(async () =>
            {
                _loggingService.Debug($"网关{gateWayAddr},调用EquipDeleteEvent事件");

                var stream = new iotsubgatewayContractClient(_iotServiceFactory.CreateChannel(gateWayAddr)).EquipDeleteEvent(new Google.Protobuf.WellKnownTypes.Empty());
                await foreach (var resp in stream.ResponseStream.ReadAllAsync())
                {
                    try
                    {
                        _loggingService.Debug($"网关{gateWayAddr},设备删除{resp.Result}已触发", context);
                        action?.Invoke(resp.Result);
                        SetPushing(gateWayAddr);
                    }
                    catch (Exception ex)
                    {
                        _loggingService.Error($"网关{gateWayAddr}订阅EquipDeleteEvent事件出现错误，异常信息为,{ex}");
                    }
                }

                Connected = false;
            }, gateWayCt.Cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

            this.TaskList.Add(task);
            return Task.CompletedTask;
        }

        Task EquipStateEvent(string gateWayAddr, Action<GrpcEquipStateItem> action, bool isReUse = false)
        {
            if (!_gateWayCts.TryGetValue(gateWayAddr, out ConnectContext gateWayCt))
            {
                _loggingService.Error($"调用EquipStateEvent事件终止，网关缓存未获取到指定网关地址{gateWayAddr}的信息，使用本地网关.");
                return Task.CompletedTask;
            }

            var task = Task.Factory.StartNew(async () =>
            {
                _loggingService.Debug($"网关{gateWayAddr},调用EquipStateEvent事件");
                var stream = new iotsubgatewayContractClient(_iotServiceFactory.CreateChannel(gateWayAddr)).EquipStateEvent(new Google.Protobuf.WellKnownTypes.Empty());
                await foreach (var resp in stream.ResponseStream.ReadAllAsync())
                {
                    try
                    {
                        var item = resp.Result.FromJson<GrpcEquipStateItem>();
                        if (_debugEquipNos != null && _debugEquipNos.Any(u => u == item.m_iEquipNo))
                        {
                            _loggingService.Debug($"网关{gateWayAddr},设备状态更新{item.m_iEquipNo}已触发" + item.ToJson(), context);
                        }
                        action?.Invoke(item);
                        SetPushing(gateWayAddr);
                    }
                    catch (Exception ex)
                    {
                        _loggingService.Error($"网关{gateWayAddr}订阅EquipStateEvent事件出现错误，异常信息为,{ex}");
                    }
                }

                Connected = false;
            }, gateWayCt.Cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

            this.TaskList.Add(task);
            return Task.CompletedTask;
        }

        Task RealtimeSnapAddEvent(string gateWayAddr, Action<WcfRealTimeEventItem> action, bool isReUse = false)
        {
            if (!_gateWayCts.TryGetValue(gateWayAddr, out ConnectContext gateWayCt))
            {
                _loggingService.Error($"调用RealtimeSnapAddEvent事件终止，网关缓存未获取到指定网关地址{gateWayAddr}的信息，使用本地网关.");
                return Task.CompletedTask;
            }

            var task = Task.Factory.StartNew(async () =>
            {
                _loggingService.Debug($"网关{gateWayAddr},调用RealtimeSnapAddEvent事件");

                var stream = new iotsubgatewayContractClient(_iotServiceFactory.CreateChannel(gateWayAddr)).AddRealTimeSnapshot(new Google.Protobuf.WellKnownTypes.Empty());
                await foreach (var resp in stream.ResponseStream.ReadAllAsync())
                {
                    try
                    {
                        var wcfRealTimeEventItem = resp.Result.FromJson<WcfRealTimeEventItem>();

                        _loggingService.Debug($"网关{gateWayAddr},设备实时快照新增{wcfRealTimeEventItem.Equipno}已触发" + wcfRealTimeEventItem.ToJson(), context);
                        action?.Invoke(wcfRealTimeEventItem);
                        SetPushing(gateWayAddr);
                    }
                    catch (Exception ex)
                    {
                        _loggingService.Error($"网关{gateWayAddr}订阅RealtimeSnapAddEvent事件出现错误，异常信息为,{ex}");
                    }
                }
                Connected = false;
            }, gateWayCt.Cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

            this.TaskList.Add(task);
            return Task.CompletedTask;
        }

        Task YcChangeEvent(string gateWayAddr, Action<GrpcYcItem> action, bool isReUse = false)
        {
            if (!_gateWayCts.TryGetValue(gateWayAddr, out ConnectContext gateWayCt))
            {
                _loggingService.Error($"调用遥测变更事件终止，网关缓存未获取到指定网关地址{gateWayAddr}的信息，使用本地网关.");
                return Task.CompletedTask;
            }

            var task = Task.Factory.StartNew(async () =>
            {
                _loggingService.Debug($"网关{gateWayAddr},调用遥测变更事件");
                var stream = new iotsubgatewayContractClient(_iotServiceFactory.CreateChannel(gateWayAddr)).YcChangeEvent(new Google.Protobuf.WellKnownTypes.Empty());
                await foreach (var resp in stream.ResponseStream.ReadAllAsync())
                {
                    var result = resp.YcItemList.OrderBy(u => u.Timestamp);
                    foreach (var item in result)
                    {
                        try
                        {
                            var ycItem = new GrpcYcItem
                            {
                                equipState = (GrpcEquipState)(int)item.EquipState,
                                m_YCNm = item.MYCNm,
                                m_iYCNo = item.MIYCNo,
                                m_AdviceMsg = item.MAdviceMsg,
                                m_bHasHistoryCcurve = item.MBHasHistoryCcurve,
                                m_Bufang = item.MBufang,
                                m_iEquipNo = item.MIEquipNo,
                                m_IsAlarm = item.MIsAlarm,
                                m_PlanNo = item.MPlanNo,
                                m_related_pic = item.MRelatedPic,
                                m_related_video = item.MRelatedVideo,
                                m_Unit = item.MUnit,
                                TimeStamp = item.Timestamp,
                                m_YCValue = new szYCValue
                                {
                                    s = string.IsNullOrEmpty(item.MYCValue.S) ? null : item.MYCValue.S,
                                    f = item.MYCValue.F
                                },
                                m_ZiChanID = item.MZiChanID
                            };
                            if (_debugEquipNos != null && _debugEquipNos.Any(u => u == ycItem.m_iEquipNo))
                            {
                                _loggingService.Debug($"网关{gateWayAddr},设备{ycItem.m_iEquipNo},{ycItem.m_iYCNo}已触发" + ycItem.ToJson(), context);
                            }
                            if (!_isPushBegin)
                            {
                                _loggingService.Debug($"网关{gateWayAddr},遥测推送已触发.{ycItem.m_iEquipNo},{ycItem.m_iYCNo}");
                                _isPushBegin = true;
                            }
                            action?.Invoke(ycItem);
                            SetPushing(gateWayAddr);
                        }
                        catch (Exception ex)
                        {
                            _loggingService.Error($"网关{gateWayAddr}订阅YcChangeEvent事件出现错误，异常信息为.{item.ToJson()},{ex}");
                        }
                    }
                }
                Connected = false;

            }, gateWayCt.Cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

            this.TaskList.Add(task);
            return Task.CompletedTask;
        }

        Task YxChangeEvent(string gateWayAddr, Action<GrpcYxItem> action, bool isReUse = false)
        {
            if (!_gateWayCts.TryGetValue(gateWayAddr, out ConnectContext gateWayCt))
            {
                _loggingService.Error($"调用YxChangeEvent事件终止，网关缓存未获取到指定网关地址{gateWayAddr}的信息，使用本地网关.");
                return Task.CompletedTask;
            }

            var task = Task.Factory.StartNew(async () =>
            {
                _loggingService.Debug($"网关{gateWayAddr},调用YxChangeEvent事件");
                var stream = new iotsubgatewayContractClient(_iotServiceFactory.CreateChannel(gateWayAddr)).YxChangeEvent(new Google.Protobuf.WellKnownTypes.Empty());
                await foreach (var resp in stream.ResponseStream.ReadAllAsync())
                {
                    try
                    {
                        var yxItem = resp.Result.FromJson<GrpcYxItem>();
                        if (_debugEquipNos != null && _debugEquipNos.Any(u => u == yxItem.m_iEquipNo))
                        {
                            _loggingService.Debug($"设备{yxItem.m_iEquipNo},{yxItem.m_iYXNo}已触发" + yxItem.ToJson(), context);
                        }
                        action?.Invoke(yxItem);
                        SetPushing(gateWayAddr);
                    }
                    catch (Exception ex)
                    {
                        _loggingService.Error($"网关{gateWayAddr}订阅YxChangeEvent事件出现错误，异常信息为{ex}");
                    }

                }
                Connected = false;

            }, gateWayCt.Cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

            this.TaskList.Add(task);
            return Task.CompletedTask;
        }

        public (Dictionary<int, GrpcEquipState>, List<GrpcYcItem>, List<GrpcYxItem>) GetInitEquipData()
        {
            try
            {
                _loggingService.Debug($"{_gatewayAddr}开始获取设备状态集合");
                var stateDic = _equipBaseClientAppService.GetEquipStateDict();
                _loggingService.Debug($"网关{_gatewayAddr}, {new System.Diagnostics.StackTrace()}获取全部设备状态" + stateDic.ToJson());
                _loggingService.Debug($"{_gatewayAddr}开始获取遥测集合");
                var ycItems = _yCClientAppService.GetTotalYCData(true);
                _loggingService.Debug($"{_gatewayAddr}开始获取遥信集合");
                var yxItems = _yXClientAppService.GetTotalYXData(true);
                _loggingService.Debug($"{_gatewayAddr}获取数据完成");
                return (stateDic, ycItems, yxItems);
            }
            catch (Exception ex)
            {
                _loggingService.Error($"网关{_gatewayAddr},获取初始化数据失败：{ex.ToString()},{System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}", ex);
                return (null, null, null);
            }
        }
        public void ChangeAddress(string address)
        {
            this._gatewayAddr = address;
        }
    }

    public class ConnectContext
    {
        public int GatewayId { get; set; }
        public IEquipManager EquipManager { get; set; }
        public CancellationTokenSource Cts { get; set; }
    }
}
