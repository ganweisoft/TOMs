//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using GWDataCenter;
using IoTCenterHost.AppServices.Interfaces;
using IoTCenterHost.AppServices.Interfaces.Token;
using IoTCenterHost.Core.Abstraction;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace IoTCenterHost.AppServices.AppServices
{
    public class IotRealTimeDataServiceImpl : IotRealTimeDataService
    {

        private volatile RealTimeDataItem realTimeDataItem;
        private ConcurrentDictionary<string, RealTimeDataItem> RealTimeDatas = null;
        private ConcurrentDictionary<string, string> ConnectIpAddrs = null;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMemoryCacheService _memoryCacheService;
        private object lockHelp = new object();
        public string IpAddr { get; set; }
        private readonly ILogger<RealTimeDataItem> _logger;

        public IotRealTimeDataServiceImpl(IHttpContextAccessor contextAccessor, ILogger<RealTimeDataItem> logger)
        {
            RealTimeDatas = new ConcurrentDictionary<string, RealTimeDataItem>();
            ConnectIpAddrs = new ConcurrentDictionary<string, string>();
            _httpContextAccessor = contextAccessor;
            _logger = logger;
        }

        public void CreateRealDataItem()
        {
            lock (lockHelp)
            {
                if (realTimeDataItem == null)
                {
                    IEnumerable<int> debugEquipNos = GetDebugEquipNos();
                    realTimeDataItem = new RealTimeDataItem(_logger, debugEquipNos);
                }
            }
        }

        private IEnumerable<int>? GetDebugEquipNos()
        {
            var debugEquipNosStr =
                DataCenter.GetPropertyFromPropertyService("AlarmCenter.Debug", "", "");

            return debugEquipNosStr.Split(',')
                .Select((string u) =>
                {
                    if (int.TryParse(u, out int result))
                    {
                        return result;
                    }

                    return 0;
                });
        }



        public RealTimeDataItem GetRealTimeDataItem()
        {
            string ipAddr = GetConnectIpPort();
            if (realTimeDataItem == null)
                CreateRealDataItem();
            realTimeDataItem.IPandPort = ipAddr;
            return realTimeDataItem;
        }


        public RealTimeDataItem GetIotRealTimeData(string connectId)
        {
            string ipAddr = GetConnectIpPort();
            CreateRealDataItem();
            if (!string.IsNullOrEmpty(connectId) && !ConnectIpAddrs.ContainsKey(connectId))
            {
                ConnectIpAddrs.TryAdd(connectId, ipAddr);
            }
            return realTimeDataItem;
        }
        private string GetConnectIpPort()
        {
            try
            {
                string? ipAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress.MapToIPv4().ToString();
                string port = _httpContextAccessor.HttpContext?.Connection.RemotePort.ToString();
                return IpAddr = $"({ipAddress}:{port})";
            }
            catch
            {
                if (!string.IsNullOrEmpty(IpAddr))
                    return IpAddr;
                else
                    return string.Empty;
            }

        }
        public void RemoveRealData(string guid)
        {
            RealTimeDatas.TryRemove(guid, out _);
        }

        public RealTimeDataItem GetIotRealTimeData(string connectId, LoginUser loginUser)
        {
            return GetIotRealTimeData(connectId).SetCurrentUser(loginUser).SetCurrentIpAddr(GetConnectIpPort());
        }
    }
}
