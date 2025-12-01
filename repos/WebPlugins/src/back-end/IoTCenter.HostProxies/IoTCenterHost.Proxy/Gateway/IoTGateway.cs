// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Utilities;
using IoTCenterHost.Proxy.Proxies;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace IoTCenterHost.Proxy.Connect
{
    public class IoTGateway
    {

        private string _url { get; set; }
        private bool disposed = false;
        IoTSubGatewayService _GatewayService { get; set; }

        private readonly ILoggingService _logService;

        private readonly IServiceScopeFactory _serviceScopeFactory;

        public IoTGateway(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logService = _serviceScopeFactory.CreateScope().ServiceProvider.GetService<ILoggingService>();
        }

        internal IEquipManager EquipManager { get; private set; }

        internal string Address
        {
            get
            {
                return _url;
            }
        }
        internal IoTSubGatewayService GatewayService
        {
            get
            {
                return _GatewayService;
            }
        }
        internal void Create(int gatewayId, string url)
        {
            _url = url;
            EquipManager = new EquipManager(gatewayId, url, false);
            _GatewayService = _serviceScopeFactory.CreateScope().ServiceProvider.GetService<IoTSubGatewayService>();
            _GatewayService.Connect(url, gatewayId, EquipManager);
            _GatewayService.ChangeAddress(url);
            _GatewayService.GatewayId = gatewayId;
        }

        internal void ChangeAddress(string url)
        {
            _url = url;
            _GatewayService.ChangeAddress(url);
        }


        internal void Close()
        {
            (this.EquipManager as IDisposable).Dispose();
            this.Dispose();
        }

        internal bool Connected
        {
            get { return _GatewayService.Connected; }
        }

        internal void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }
            if (disposing)
            {
                if (_GatewayService != null)
                {
                    _GatewayService = null;
                }
            }
            disposed = true;
        }
    }
}
