// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Google.Protobuf.WellKnownTypes;
using IoTCenter.Utilities;
using IoTCenterHost.Core.Abstraction;
using IoTCenterHost.Core.Abstraction.AppServices;
using IoTCenterHost.Core.Abstraction.BaseModels;
using IoTCenterHost.Core.Abstraction.IotModels;
using IoTCenterHost.Proto;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IoTCenterHost.Proxy.ServiceImpl
{
    public class AlarmEventAppServiceImpl : IAlarmEventClientAppService
    {
        private readonly IotService.IotServiceClient _AlarmCenterClient;
        private readonly IotQueryService.IotQueryServiceClient _iotQueryClient;
        private readonly Empty empty;
        private readonly int _waitTime = 10;
        private readonly IConfiguration _configuration;
        public AlarmEventAppServiceImpl(IotService.IotServiceClient alarmCenterClient,
                                        IotQueryService.IotQueryServiceClient iotQueryClient, IConfiguration configuration)
        {
            _AlarmCenterClient = alarmCenterClient;
            _iotQueryClient = iotQueryClient;
            _configuration = configuration;
            empty = new Empty();
        }
        public void ConfirmedRealTimeEventItem(WcfRealTimeEventItem item)
        {
            _AlarmCenterClient.ConfirmedRealTimeEventItem(new StringResult { Result = item.ToJson() });
        }

        public void GetAddRealEventItem1()
        {
            _AlarmCenterClient.GetAddRealEventItem1(empty);
        }

        public void GetDelRealEventItem1()
        {
            _AlarmCenterClient.GetDelRealEventItem1(empty);
        }

        public async Task<List<WcfRealTimeEventItem>> FirstGetRealEventItemExAsync()
        {
            int tryTime = 0;
            var tryConnectTime = int.Parse(_configuration["HostSetting:TryConnectTime"]);
            while (tryTime <= tryConnectTime)
            {
                tryTime += 1;
                try
                {
                    var result = _iotQueryClient.FirstGetRealEventItem(empty).Result;

                    return result.FromJson<List<WcfRealTimeEventItem>>();
                }
                catch (Exception ex)
                {
                    if (tryTime <= tryConnectTime)
                        Console.WriteLine($"连接网关出现错误:{ex},{_waitTime}秒进行第{tryTime}次重试。");
                    else
                    {
                        Console.WriteLine($"连接网关出现错误:{ex},已超出最大次数{tryConnectTime}。");
                    }
                }
                await Task.Delay(TimeSpan.FromSeconds(_waitTime));
            }
            return null;
        }

        public List<WcfRealTimeEventItem> GetRealEventItem(bool isFirst = false)
        {
            return _iotQueryClient.GetRealEventItem(new BoolDefine { Result = isFirst }).Result.FromJson<List<WcfRealTimeEventItem>>();
        }

        public List<WcfRealTimeEventItem> GetLastRealEventItem(int lastCount)
        {
            return _iotQueryClient.GetLastRealEventItem(new IntegerDefine { Result = lastCount }).Result.FromJson<List<WcfRealTimeEventItem>>();
        }

        public PaginationData GetRealEventItems(Pagination pagination)
        {
            return _iotQueryClient.GetPaginationRealEventItem(new StringResult { Result = pagination.ToJson() }).Result.FromJson<PaginationData>();
        }

        public IEnumerable<RealTimeGroupCount> GetRealTimeGroupCount()
        {
            return _iotQueryClient.GetRealTimeGroupCount(empty).Result.FromJson<IEnumerable<RealTimeGroupCount>>();
        }

        public WcfRealTimeEventItem GetRealTimeEventItem(string guid)
        {
            return _AlarmCenterClient.GetRealTimeEventItem(new StringResult { Result = guid }).Result.FromJson<WcfRealTimeEventItem>();
        }

        public bool Contains(string guid)
        {
            return _AlarmCenterClient.ContainsRealTimeEventItem(new StringResult { Result = guid }).Result;
        }


        public async Task<List<GrpcGwEventInfo>> GetGWEventInfoAsync(GrpcGetEventInfo grpcGetEventInfo)
        {
            var response = await _AlarmCenterClient.GetGWEventInfoAsync(new StringResult { Result = grpcGetEventInfo.ToJson() });
            return response.EventInfoList.Select(u => new GrpcGwEventInfo
            {
                datetime = u.Datetime.ToDateTime(),
                gwevent = u.Gwevent,
                id = u.Id
            }).ToList();
        }
    }
}
