//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using IoTCenterHost.AppServices.Interfaces;
using IoTCenterHost.Core.Extension;
using IoTCenterHost.Proto;
using Microsoft.AspNetCore.Authorization;

namespace IoTCenterHost.AppServices.GrpcConstracts
{
    [Authorize]
    public class IotCallbackService : IoTCenterHost.Proto.IotCallbackService.IotCallbackServiceBase
    {
        private readonly IotRealTimeDataService _iotRealTimeDataService;
        public IotCallbackService(IotRealTimeDataService iotRealTimeDataService)
        {
            _iotRealTimeDataService = iotRealTimeDataService;
        }



        private static Task AwaitCancellation(CancellationToken token)
        {
            var completion = new TaskCompletionSource<object>();
            token.Register(() => completion.SetResult(null));
            return completion.Task;
        }

        [AllowAnonymous]
        public override async Task AddRealTimeSnapshot(Empty request, IServerStreamWriter<StringResult> responseStream, ServerCallContext context)
        {
            _iotRealTimeDataService.GetRealTimeDataItem().AddRealTimeSnapshot += (sender, e) => { responseStream.WriteAsync(new StringResult { Result = e.ToJson() }); };
            await AwaitCancellation(context.CancellationToken);
        }


        [AllowAnonymous]
        public override async Task DeleteRealTimeSnapshot(Empty request, IServerStreamWriter<StringResult> responseStream, ServerCallContext context)
        {
            _iotRealTimeDataService.GetRealTimeDataItem().DeleteRealTimeSnapshot += (sender, e) => { responseStream.WriteAsync(new StringResult { Result = e.ToJson() }); };
            await AwaitCancellation(context.CancellationToken);
        }

        [AllowAnonymous]
        public override async Task YcChangeEvent(Empty request, IServerStreamWriter<StringResult> responseStream, ServerCallContext context)
        {
            _iotRealTimeDataService.GetRealTimeDataItem().OnYCChanged += (sender, e) =>
            {
                Console.WriteLine(e.ToJson());
                responseStream.WriteAsync(new StringResult { Result = e.ToJson() });
            };
            await AwaitCancellation(context.CancellationToken);
        }

        [AllowAnonymous]
        public override async Task YxChangeEvent(Empty request, IServerStreamWriter<StringResult> responseStream, ServerCallContext context)
        {
            _iotRealTimeDataService.GetRealTimeDataItem().OnYXChanged += (sender, e) =>
            {
                Console.WriteLine(e.ToJson());
                responseStream.WriteAsync(new StringResult { Result = e.ToJson() });
            };
            await AwaitCancellation(context.CancellationToken);
        }

        [AllowAnonymous]
        public override async Task EquipAddEvent(Empty request, IServerStreamWriter<StringResult> responseStream, ServerCallContext context)
        {
            _iotRealTimeDataService.GetRealTimeDataItem().OnEquipAdd += (sender, e) => { responseStream.WriteAsync(new StringResult { Result = e.ToJson() }); };
            await AwaitCancellation(context.CancellationToken);
        }

        [AllowAnonymous]
        public override async Task EquipChangeEvent(Empty request, IServerStreamWriter<StringResult> responseStream, ServerCallContext context)
        {
            _iotRealTimeDataService.GetRealTimeDataItem().OnEquipChanged += (sender, e) => { responseStream.WriteAsync(new StringResult { Result = e.ToJson() }); };
            await AwaitCancellation(context.CancellationToken);
        }

        [AllowAnonymous]
        public override async Task EquipDeleteEvent(Empty request, IServerStreamWriter<IntegerDefine> responseStream, ServerCallContext context)
        {
            _iotRealTimeDataService.GetRealTimeDataItem().OnEquipDeleted += (sender, e) =>
            {
                responseStream.WriteAsync(new IntegerDefine { Result = e });
            };
            await AwaitCancellation(context.CancellationToken);
        }

        [AllowAnonymous]
        public override async Task EquipStateEvent(Empty request, IServerStreamWriter<StringResult> responseStream, ServerCallContext context)
        {
            _iotRealTimeDataService.GetRealTimeDataItem().OnEquipStatusChanged += (sender, e) =>
            {
                Console.WriteLine(e.ToJson());
                responseStream.WriteAsync(new StringResult { Result = e.ToJson() });
            };
            await AwaitCancellation(context.CancellationToken);
        }
    }
}
