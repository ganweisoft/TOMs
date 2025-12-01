//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using IoTCenterHost.Core.Abstraction;
using IoTCenterHost.Core.Abstraction.BaseModels;
using IoTCenterHost.Core.Extension;
using IoTCenterHost.Proto;

namespace IoTCenterHost.AppServices.GrpcConstracts.IotHostService
{
    public partial class IotHostService : IotService.IotServiceBase
    {

        public override Task<Empty> FirstGetRealEventItem1(Empty request, ServerCallContext context)
        {
            return Task.FromResult(new Empty());
        }

        public override Task<Empty> GetDelRealEventItem1(Empty request, ServerCallContext context)
        {
            this.AlarmEventAppService.GetDelRealEventItem1();
            return Task.FromResult(new Empty());

        }
        public override Task<Empty> GetAddRealEventItem1(Empty request, ServerCallContext context)
        {
            this.AlarmEventAppService.GetDelRealEventItem1();
            return Task.FromResult(new Empty());
        }
        public override Task<Empty> ConfirmedRealTimeEventItem(StringResult request, ServerCallContext context)
        {
            this.AlarmEventAppService.ConfirmedRealTimeEventItem(request.Result.FromJson<WcfRealTimeEventItem>());
            return Task.FromResult(new Empty());
        }

        public override Task<BoolDefine> ContainsRealTimeEventItem(StringResult request, ServerCallContext context)
        {
            var result = this.AlarmEventAppService.Contains(request.Result);
            return Task.FromResult(new BoolDefine { Result = result });
        }

        public override Task<BaseResult> GetRealTimeEventItem(StringResult request, ServerCallContext context)
        {
            var result = this.AlarmEventAppService.GetRealTimeEventItem(request.Result);
            return Task.FromResult(new BaseResult { Result = result.ToJson() });
        }

        public override async Task<EventInfoResultList> GetGWEventInfo(StringResult request, ServerCallContext context)
        {
            var parm = request.Result.FromJson<GrpcGetEventInfo>();
            var result = await this.AlarmEventAppService.GetGWEventInfoAsync(parm);
            EventInfoResultList eventInfoResultList = new EventInfoResultList();
            eventInfoResultList.EventInfoList.AddRange(result.Select(u => new EventInfo { Gwevent = u.gwevent, Id = u.id, Datetime = Google.Protobuf.WellKnownTypes.Timestamp.FromDateTime(DateTime.SpecifyKind(u.datetime, DateTimeKind.Utc)) }));
            return eventInfoResultList;
        }
    }
}
