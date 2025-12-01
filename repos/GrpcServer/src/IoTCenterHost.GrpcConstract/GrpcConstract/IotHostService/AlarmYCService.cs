//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using IoTCenterHost.Core.Abstraction.IotModels;
using IoTCenterHost.Core.Extension;
using IoTCenterHost.Proto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IoTCenterHost.AppServices.GrpcConstracts.IotHostService
{
    public partial class IotHostService : IotService.IotServiceBase
    {
        [AllowAnonymous]
        public override Task<Empty> GetTotalRTYCItemData1(Empty request, ServerCallContext context)
        {
            this.YCAppService.GetTotalRTYCItemData1();
            return Task.FromResult(new Empty());
        }
        [AllowAnonymous]
        public override Task<BaseResult> GetYCAlarmComments(HaveHistoryCurveRequest request, ServerCallContext context)
        {
            var result = YCAppService.GetYCAlarmComments(request.EquipNo, request.YCPNo);
            return Task.FromResult(new BaseResult { Result = result, Code = 200 });
        }
        [AllowAnonymous]
        public override Task<BoolDefine> GetYCAlarmState(GetYCAlarmStateRequest request, ServerCallContext context)
        {
            return Task.FromResult(new BoolDefine { Result = YCAppService.GetYCAlarmState(request.IEquipNo, request.IYcpNo) });
        }
        [AllowAnonymous]
        public override Task<BaseResult> GetYCAlarmStateDictFromEquip(IntegerDefine request, ServerCallContext context)
        {
            var result = YCAppService.GetYCAlarmStateDictFromEquip(request.Result);
            return Task.FromResult(new BaseResult { Result = result.ToJson(), Code = 200 });
        }
        [AllowAnonymous]
        [ResponseCache(Duration = 10)]
        public override Task<BaseResult> GetYCPListStr(IntegerDefine request, ServerCallContext context)
        {
            var result = YCAppService.GetYCPListStr(request.Result);
            return Task.FromResult(new BaseResult { Result = result, Code = 200 });
        }
        [AllowAnonymous]
        public override Task<Empty> GetChangedRTYCItemData1(Empty request, ServerCallContext context)
        {
            YCAppService.GetChangedRTYCItemData1();
            return Task.FromResult(new Empty());
        }
        [AllowAnonymous]
        public override Task<BaseResult> GetYCValue(GetYCAlarmStateRequest request, ServerCallContext context)
        {
            return Task.FromResult(new BaseResult { Result = YCAppService.GetYCValue(request.IEquipNo, request.IYcpNo).ToJson() });
        }
        [AllowAnonymous]
        public override Task<BaseResult> GetYCValueDictFromEquip(IntegerDefine request, ServerCallContext context)
        {
            return Task.FromResult(new BaseResult { Result = YCAppService.GetYCValueDictFromEquip(request.Result).ToJson() });
        }
        [AllowAnonymous]
        public override Task<BoolDefine> HaveYCP(IntegerDefine request, ServerCallContext context)
        {
            return Task.FromResult(new BoolDefine { Result = YCAppService.HaveYCP(request.Result) });
        }
        public override Task<Empty> SetYcpNm(SetYxpNmRequest request, ServerCallContext context)
        {
            YCAppService.SetYcpNm(request.EquipNo, request.YcpNo, request.Nm);
            return Task.FromResult(new Empty());
        }
        [AllowAnonymous]
        [ResponseCache(Duration = 10)]
        public override Task<StringResult> GetYCPListByEquipNo(StringResult request, ServerCallContext context)
        {
            var result = YCAppService.GetYCPListByEquipNo(request.Result.FromJson<Pagination>());
            return Task.FromResult(new StringResult { Result = result.ToJson() });
        }

        public override Task<StringResult> GetYCPDict(StringResult request, ServerCallContext context)
        {
            var result = YCAppService.GetYCPDict(request.Result.FromJson<Pagination>());
            return Task.FromResult(new StringResult { Result = result.ToJson() });
        }
    }
}
