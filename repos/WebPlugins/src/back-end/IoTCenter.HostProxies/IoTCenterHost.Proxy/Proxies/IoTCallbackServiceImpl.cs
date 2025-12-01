// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Grpc.Core;
using IoTCenter.Utilities;
using IoTCenterHost.Core.Abstraction;
using IoTCenterHost.Core.Abstraction.BaseModels;
using IoTCenterHost.Core.Abstraction.ProxyModels;
using IoTCenterHost.Core.Abstraction.Services;
using IoTCenterHost.Proto;
using System;
using System.Threading.Tasks;

namespace IoTCenterHost.Proxy
{
    public class IoTCallbackServiceImpl : IotCenterHostCallbackService
    {

        private const string MockYXItemJson = "{\"equip_no\":\"101\",\"yx_nm\":\"123\",\"yx_no\":1,\"related_pic\":\"123+\"}";

        private readonly IotCallbackService.IotCallbackServiceClient _iotCallbackService;

        public IoTCallbackServiceImpl(IotCallbackService.IotCallbackServiceClient iotCallbackServiceClient)
        {
            _iotCallbackService = iotCallbackServiceClient;
        }

        public async Task AddRealTimeSnapshot(Action<WcfRealTimeEventItem> action, bool isReUse = false)
        {
            var stream = _iotCallbackService.AddRealTimeSnapshot(new Google.Protobuf.WellKnownTypes.Empty());
            await foreach (var resp in stream.ResponseStream.ReadAllAsync())
            {
                action?.Invoke(resp.Result.FromJson<WcfRealTimeEventItem>());
            }
        }

        public async Task DeleteRealTimeSnapshot(Action<WcfRealTimeEventItem> action, bool isReUse = false)
        {
            var stream = _iotCallbackService.DeleteRealTimeSnapshot(new Google.Protobuf.WellKnownTypes.Empty());
            await foreach (var resp in stream.ResponseStream.ReadAllAsync())
            {
                action?.Invoke(resp.Result.FromJson<WcfRealTimeEventItem>());
            }
        }

        public async Task EquipAddEvent(Action<GrpcEquipItem> action, bool isReUse = false)
        {
            var stream = _iotCallbackService.EquipAddEvent(new Google.Protobuf.WellKnownTypes.Empty());
            await foreach (var resp in stream.ResponseStream.ReadAllAsync())
            {
                action?.Invoke(resp.Result.FromJson<GrpcEquipItem>());
            }
        }

        public async Task EquipChangeEvent(Action<GrpcEquipItem> action, bool isReUse = false)
        {
            var stream = _iotCallbackService.EquipChangeEvent(new Google.Protobuf.WellKnownTypes.Empty());
            await foreach (var resp in stream.ResponseStream.ReadAllAsync())
            {
                action?.Invoke(resp.Result.FromJson<GrpcEquipItem>());
            }
        }

        public async Task EquipDeleteEvent(Action<int> action, bool isReUse = false)
        {
            var stream = _iotCallbackService.EquipDeleteEvent(new Google.Protobuf.WellKnownTypes.Empty());
            await foreach (var resp in stream.ResponseStream.ReadAllAsync())
            {
                action?.Invoke(resp.Result);
            }
        }

        public async Task EquipStateEvent(Action<GrpcEquipStateItem> action, bool isReUse = false)
        {
            var stream = _iotCallbackService.EquipStateEvent(new Google.Protobuf.WellKnownTypes.Empty());
            await foreach (var resp in stream.ResponseStream.ReadAllAsync())
            {
                action?.Invoke(resp.Result.FromJson<GrpcEquipStateItem>());
            }
        }

        public async Task YcChangeEvent(Action<GrpcYcItem> action, bool isReUse = false)
        {
            var stream = _iotCallbackService.YcChangeEvent(new Google.Protobuf.WellKnownTypes.Empty());
            await foreach (var resp in stream.ResponseStream.ReadAllAsync())
            {
                action?.Invoke(resp.Result.FromJson<GrpcYcItem>());
            }
        }

        public async Task YxChangeEvent(Action<GrpcYxItem> action, bool isReUse = false)
        {
            var stream = _iotCallbackService.YxChangeEvent(new Google.Protobuf.WellKnownTypes.Empty());
            await foreach (var resp in stream.ResponseStream.ReadAllAsync())
            {
                action?.Invoke(resp.Result.FromJson<GrpcYxItem>());
            }
        }
    }
}
