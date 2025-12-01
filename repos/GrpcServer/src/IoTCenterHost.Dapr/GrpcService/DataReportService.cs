//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Dapr.AppCallback.Autogen.Grpc.v1;
using Dapr.Client;
using Dapr.Client.Autogen.Grpc.v1;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GWDataCenter;
using IoTCenterHost.Dapr.Models;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace IoTCenterHost.Dapr.GrpcService
{
    public class DataReportService : AppCallback.AppCallbackBase
    {

        /// <summary>
        /// State store name.
        /// </summary>
        public const string PubSubName = "open_datacenter_pubsub";

        private readonly ILogger<DataReportService> _logger;
        private readonly DaprClient _daprClient;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="daprClient"></param>
        /// <param name="logger"></param>
        public DataReportService(DaprClient daprClient, ILogger<DataReportService> logger)
        {
            _daprClient = daprClient;
            _logger = logger;
        }

        readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

        /// <summary>
        /// implement OnInvoke to support
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<InvokeResponse> OnInvoke(InvokeRequest request, ServerCallContext context)
        {
            var response = new InvokeResponse();
            switch (request.Method)
            {
                // case "report":
                //     var input = request.Data.Unpack<DeviceDataExReport>();
                //     var output = await GetAccount(input, context);
                //     response.Data = Any.Pack(output);
                //     break;
                default:
                    break;
            }
            return response;
        }

        /// <summary>
        /// implement ListTopicSubscriptions to register report subscriber
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task<ListTopicSubscriptionsResponse> ListTopicSubscriptions(Empty request, ServerCallContext context)
        {
            var result = new ListTopicSubscriptionsResponse();
            result.Subscriptions.Add(new TopicSubscription
            {
                PubsubName = PubSubName,
                Topic = "report"
            });
            return Task.FromResult(result);
        }

        /// <summary>
        /// implement OnTopicEvent to handle event
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task<TopicEventResponse> OnTopicEvent(TopicEventRequest request, ServerCallContext context)
        {
            if (request.PubsubName == PubSubName)
            {
                var input = JsonSerializer.Deserialize<DeviceDataExReport>(request.Data.ToStringUtf8(), this.jsonOptions);
                if (request.Topic == "report")
                {
                    await Report(input, context);
                }
            }

            return new TopicEventResponse();
        }

        /// <summary>
        /// Report
        /// </summary>
        /// <param name="data"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Report(DeviceDataExReport data, ServerCallContext context)
        {
            _logger.LogDebug("Enter deposit");
            foreach (var item in data.DataItems)
            {
                var equipNo = item.DeviceId;
                var equipItem = DataCenter.GetEquipItem(equipNo);

                if (equipItem == null)
                {
                    _logger.LogWarning($"根据deviceId转换成equipNo，使用equipNo获取设备对象时为空,DeviceId:{item.DeviceId}",
                        LogLevel.Error);
                    continue;
                }

                switch (data.DataType)
                {
                    case 1:
                        {
                            var ycDictionary = equipItem.YCItemDict.ToDictionary(x => x.Value.Yc_no);

                            foreach (var kvp in item.Attribute)
                            {
                                if (!ycDictionary.TryGetValue(kvp.Key, out var ycp))
                                    continue;

                                ycp.Value.YCValue = decimal.TryParse(kvp.Value.ToString(), out var val)
                                    ? val
                                    : kvp.Value;
                            }

                            break;
                        }
                    case 2:
                        {
                            var yxDictionary = equipItem.YXItemDict.ToDictionary(x => x.Value.Yx_no);

                            foreach (var kvp in item.Attribute)
                            {
                                if (!yxDictionary.TryGetValue(kvp.Key, out var yxp))
                                    continue;

                                if (kvp.Value.ToString() == yxp.Value.Evt_01 ||
                                    kvp.Value?.ToString()?.ToLower() == "true")
                                {
                                    yxp.Value.YXValue = true;
                                }
                                else
                                {
                                    yxp.Value.YXValue = false;
                                }
                            }

                            break;
                        }
                }
            }
        }
    }
}