// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Utilities;
using IoTCenterHost.Core.Abstraction;
using IoTCenterHost.Proxy.Connect;
using IoTCenterHost.Proxy.Proxies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace IoTCenterHost.Proxy.Core
{
    public class ConnectPoolManager : IConnectPoolManager
    {
        private bool _syncCalled = false;
        private readonly object _syncLock = new object();
        private readonly IServiceProvider _serviceProvider;
        private readonly ILoggingService _logService;
        public ConnectPoolManager(IServiceProvider serviceProvider, IEquipManager equipManager)
        {
            CurrentPool = new ConcurrentDictionary<string, IConnectService>();
            ConnectionStatusPool = new ConcurrentDictionary<string, ConnectionStatus>();
            _serviceProvider = serviceProvider;
            EquipManager = equipManager;
            _logService = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<ILoggingService>();
        }
        public void AddOrUpdateConnectService(string token, ConnectionStatus connectionStatus, IConnectService connectService)
        {
            CurrentPool.AddOrUpdate(token, connectService, (k, v) => connectService);
            ConnectionStatusPool.AddOrUpdate(token, connectionStatus, (k, v) => connectionStatus);
        }

        public void Clear()
        {
            CurrentPool.Clear();
        }

        public IConnectService GetConnectService(string token)
        {
            return CurrentPool[token];
        }

        public void RemoveConnectService(string token)
        {
            CurrentPool.TryRemove(token, out IConnectService _);
        }
        public ConcurrentDictionary<string, IConnectService> CurrentPool { get; set; }
        public ConcurrentDictionary<string, ConnectionStatus> ConnectionStatusPool { get; set; }
        public string ConnectId { get; set; }
        public string NetMqIdentity { get; set; }
        public IEquipManager EquipManager { get; protected set; }


        private System.Threading.Timer threadTimer { get; set; }

        IoTSubGatewayService _GatewayService { get; set; }
        IoTSubGatewayService GatewayService
        {
            get
            {
                return _GatewayService;
            }
        }
        public void SyncEquipManager()
        {
           
            var _serviceScopedFactory = _serviceProvider.GetService<IServiceScopeFactory>();
            var configuration = _serviceProvider.GetService<IConfiguration>();
            var url = configuration["HostSetting:Address"];
            _GatewayService = _serviceScopedFactory.CreateScope().ServiceProvider.GetService<IoTSubGatewayService>();
            _GatewayService.Connect(url, 0, EquipManager);
            _GatewayService.ChangeAddress(url);
            _GatewayService.GatewayId = 0;
            this.InitEquipData();
        }

        private void InitEquipData()
        {
            Task.Run(() =>
            {
                try
                {
                    _logService.Info($"开始获取设备数据");
                    var item = this.GatewayService.GetInitEquipData();
                    this.EquipManager.OnLine(item.Item1, out string msg);
                    if (!string.IsNullOrEmpty(msg))
                        _logService.Error($"获取初始化数据失败：{msg}");
                    this.EquipManager.AddYcItems(item.Item2, out msg);
                    if (!string.IsNullOrEmpty(msg))
                        _logService.Error($"获取初始化数据失败：{msg}");
                    this.EquipManager.AddYxItems(item.Item3, out msg);
                    if (!string.IsNullOrEmpty(msg))
                        _logService.Error($"获取初始化数据失败：{msg}");
                    _logService.Info($"获取设备数据完成");
                }
                catch (Exception ex)
                {
                    _logService.Error($"获取初始化数据失败：{ex.ToString()},{System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}", ex);
                }
            });

        }
    }
}
