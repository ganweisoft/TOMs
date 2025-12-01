//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using IoTCenterHost.Core.Abstraction.AppServices;
using IoTCenterHost.Core.Abstraction.IotModels;
using IoTCenterHost.Core.Extension;
using IoTCenterHost.Core.ProxyModels;
using IoTCenterHost.Core.ServerInterfaces;
using Microsoft.AspNetCore.Authorization;
using IoTCenterHost.Proto;


namespace IoTCenterHost.AppServices.GrpcConstracts
{
    [Authorize]
    public class IotQueryService : IoTCenterHost.Proto.IotQueryService.IotQueryServiceBase
    {
        private Empty empty;
        private readonly IoTCenterAppService _alarmCenterAppService;
        private readonly IAlarmEventServerAppService _alarmEventAppService;
        private readonly IEquipBaseServerService _equipBaseAppService;
        private readonly IYXServerAppService _yXAppService;
        private readonly IYCServerAppService _yCAppService;
        public IotQueryService(
            IAlarmEventServerAppService alarmEventAppService,
            IEquipBaseServerService equipBaseAppService,
            IYXServerAppService yXAppService,
            IYCServerAppService yCAppService
            )
        {
            _alarmEventAppService = alarmEventAppService;
            _equipBaseAppService = equipBaseAppService;
            _yXAppService = yXAppService;
            _yCAppService = yCAppService;
            empty = new Empty();
        }
        [AllowAnonymous]
        public override Task<StringResult> GetTotalEquipData(BoolDefine request, ServerCallContext context)
        {
            var list = _equipBaseAppService.GetTotalEquipDataEx(request.Result).Select(u => new ProxyEquipItem(u));
            return Task.FromResult(new StringResult { Result = list.ToJson() });
        }
        [AllowAnonymous]
        public override Task<StringResult> FirstGetRealEventItem(Empty empty, ServerCallContext context)
        {
            var list = _alarmEventAppService.FirstGetRealEventItem();
            return Task.FromResult(new StringResult { Result = list.ToJson() });
        }
        [AllowAnonymous]
        public override Task<StringResult> GetRealEventItem(BoolDefine request, ServerCallContext context)
        {
            var list = _alarmEventAppService.FirstGetRealEventItem(request.Result);
            return Task.FromResult(new StringResult { Result = list.ToJson() });
        }

        [AllowAnonymous]
        public override Task<StringResult> GetTotalYCData(BoolDefine request, ServerCallContext context)
        {
            var list = _yCAppService.GetTotalYCDataEx(request.Result).Select(u => new ProxyYcItem(u));
            return Task.FromResult(new StringResult { Result = list.ToJson() });
        }
        [AllowAnonymous]
        public override Task<StringResult> GetTotalYXData(BoolDefine request, ServerCallContext context)
        {
            var list = _yXAppService.GetTotalYXDataEx(request.Result).Select(u => new ProxyYxItem(u));
            return Task.FromResult(new StringResult { Result = list.ToJson() });
        }


        public override Task<StringResult> GetLastRealEventItem(IntegerDefine request, ServerCallContext context)
        {
            var list = _alarmEventAppService.GetLastRealEventItem(request.Result);
            return Task.FromResult(new StringResult { Result = list.ToJson() });
        }

        public override Task<StringResult> GetPaginationRealEventItem(StringResult request, ServerCallContext context)
        {
            return Task.FromResult(new StringResult
            {
                Result =
                _alarmEventAppService.GetRealEventItems(request.Result.FromJson<Pagination>()).ToJson()
            });
        }

        public override Task<StringResult> GetRealTimeGroupCount(Empty request, ServerCallContext context)
        {
            return Task.FromResult(new StringResult
            {
                Result =
                _alarmEventAppService.GetRealTimeGroupCount().ToJson()
            });
        }

    }
}
