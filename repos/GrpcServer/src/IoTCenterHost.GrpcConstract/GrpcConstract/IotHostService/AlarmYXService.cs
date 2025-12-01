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
        public override Task<BaseResult> GetYXAlarmComments(HaveHistoryCurveRequest request, ServerCallContext context)
        {
            var result = this.YXAppService.GetYXAlarmComments(request.EquipNo, request.YCPNo);
            return Task.FromResult(new BaseResult { Result = result });
        }
        [AllowAnonymous]
        public override Task<BoolDefine> GetYXAlarmState(GetYXAlarmStateRequest request, ServerCallContext context)
        {
            var result = this.YXAppService.GetYXAlarmState(request.IEquipNo, request.IYxpNo);
            return Task.FromResult(new BoolDefine { Result = result });
        }
        [AllowAnonymous]
        public override Task<BaseResult> GetYXAlarmStateDictFromEquip(IntegerDefine request, ServerCallContext context)
        {
            return Task.FromResult(new BaseResult { Result = this.YXAppService.GetYXAlarmStateDictFromEquip(request.Result).ToJson() });
        }
        [AllowAnonymous]
        public override Task<BaseResult> GetYXEvt01(GetYXEvt01Request request, ServerCallContext context)
        {
            return Task.FromResult(new BaseResult { Result = this.YXAppService.GetYXEvt01(request.IEquipNo, request.IYxpNo).ToJson() });
        }
        [AllowAnonymous]
        public override Task<BaseResult> GetYXEvt10(GetYXEvt01Request request, ServerCallContext context)
        {
            return Task.FromResult(new BaseResult { Result = this.YXAppService.GetYXEvt10(request.IEquipNo, request.IYxpNo).ToJson() });
        }
        [AllowAnonymous]
        public override Task<StringResult> GetYXValue(GetYXValueRequest request, ServerCallContext context)
        {
            return Task.FromResult(new StringResult { Result = this.YXAppService.GetYXValue(request.IEquipNo, request.IYxpNo).ToJson() });
        }
        [AllowAnonymous]
        public override Task<BaseResult> GetYXValueDictFromEquip(IntegerDefine request, ServerCallContext context)
        {
            return Task.FromResult(new BaseResult { Result = this.YXAppService.GetYXValueDictFromEquip(request.Result).ToJson() });
        }
        [AllowAnonymous]
        public override Task<Empty> SetYxpNm(SetYxpNmRequest request, ServerCallContext context)
        {
            YXAppService.SetYxpNm(request.EquipNo, request.YcpNo, request.Nm);
            return Task.FromResult(new Empty());
        }
        [AllowAnonymous]
        public override Task<BoolDefine> HaveYXP(IntegerDefine request, ServerCallContext context)
        {
            return Task.FromResult(new BoolDefine { Result = YXAppService.HaveYXP(request.Result) });
        }
        [AllowAnonymous]
        [ResponseCache(Duration = 10)]
        public override Task<BaseResult> GetYXPListStr(IntegerDefine request, ServerCallContext context)
        {
            var result = YXAppService.GetYXPListStr(request.Result);
            return Task.FromResult(new BaseResult { Result = result, Code = 200 });
        }
        [AllowAnonymous]
        [ResponseCache(Duration = 10)]
        public override Task<StringResult> GetYXPListByEquipNo(StringResult request, ServerCallContext context)
        {
            var result = YXAppService.GetYXPListByEquipNo(request.Result.FromJson<Pagination>());
            return Task.FromResult(new StringResult { Result = result.ToJson() });
        }
    }
}
