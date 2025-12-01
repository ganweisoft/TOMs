//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Grpc.Core;
using IoTCenterHost.Proto;

namespace IoTCenterHost.AppServices.GrpcConstracts.IotHostService
{
    public partial class IotHostService : IotService.IotServiceBase
    {
        public override Task<BoolDefine> Confirm2NormalState(Confirm2NormalStateRequest request, ServerCallContext context)
        {
            ;
            return Task.FromResult(new BoolDefine { Result = EquipAlarmAppService.Confirm2NormalState(request.IEqpNo, request.SYcYxType, request.IYcYxNo) });
        }
        public override Task<BoolDefine> SetWuBao(SetWuBaoRequest request, ServerCallContext context)
        {
            var result = EquipAlarmAppService.SetWuBao(request.Eqpno, request.Type, request.Ycyxno);
            return Task.FromResult(new BoolDefine { Result = result });
        }
        public override Task<BoolDefine> SetNoAlarm(SetNoAlarmRequest request, ServerCallContext context)
        {
            var result = EquipAlarmAppService.SetNoAlarm(request.Eqpno, request.Type, request.Ycyxno);
            return Task.FromResult(new BoolDefine { Result = result });
        }
    }
}
