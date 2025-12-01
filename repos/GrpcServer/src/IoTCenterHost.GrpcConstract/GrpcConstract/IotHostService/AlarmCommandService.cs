//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using IoTCenterHost.Proto;

namespace IoTCenterHost.AppServices.GrpcConstracts.IotHostService
{
    public partial class IotHostService : IotService.IotServiceBase
    {

        public override Task<Empty> DoSetParmFromString(BaseResult request, ServerCallContext context)
        {
            this.CommandAppService.DoSetParmFromString(request.Result);
            return Task.FromResult(new Empty());
        }
        public override Task<Empty> SetParm(SetParmRequest request, ServerCallContext context)
        {
            this.CommandAppService.SetParm(request.EquipNo, request.StrCMD1, request.StrCMD2, request.StrCMD3, request.StrUser);
            return Task.FromResult(new Empty());
        }


        public override async Task SetParmEx(SetParmRequest request, IServerStreamWriter<StringResult> responseStream, ServerCallContext context)
        {
            await this.CommandAppService.SetParmExAsync(request.EquipNo, request.StrCMD1, request.StrCMD2, request.StrCMD3, request.StrUser, request.RequestId, (string args) =>
            {
                responseStream.WriteAsync(new StringResult { Result = args });
            });
            await AwaitCancellation(context.CancellationToken);
        }

        private static Task AwaitCancellation(CancellationToken token)
        {
            var completion = new TaskCompletionSource<object>();
            token.Register(() => completion.SetResult(null));
            return completion.Task;
        }



        public override Task<Empty> SetParm1(SetParm1Request request, ServerCallContext context)
        {
            this.CommandAppService.SetParm1(request.EquipNo, request.SetNo, request.StrUser);
            return Task.FromResult(new Empty());
        }
        public override Task<BoolDefine> HaveSet(IntegerDefine request, ServerCallContext context)
        {
            return Task.FromResult(new BoolDefine() { Result = this.CommandAppService.HaveSet(request.Result) });
        }

        public override Task<Empty> SetParm1_1(SetParm1_1Request request, ServerCallContext context)
        {
            this.CommandAppService.SetParm1_1(request.EquipNo, request.SetNo, request.StrValue, request.StrUser, request.BShowDlg, request.RequestId);
            return Task.FromResult(new Empty());
        }
        public override Task<Empty> SetParm2(SetParm2Request request, ServerCallContext context)
        {
            this.CommandAppService.SetParm2(request.EquipNo, request.StrCMD1, request.StrCMD2, request.StrCMD3, request.StrType, request.StrUser);
            return Task.FromResult(new Empty());
        }
        public override Task<Empty> SetParm2_1(SetParm2_1Request request, ServerCallContext context)
        {
            this.CommandAppService.SetParm2_1(request.EquipNo, request.StrCMD1, request.StrCMD2, request.StrCMD3, request.StrType, request.StrUser, request.BShowDlg);
            return Task.FromResult(new Empty());
        }
        public override Task<Empty> SetParm_1(SetParm_1Request request, ServerCallContext context)
        {
            this.CommandAppService.SetParm_1(request.EquipNo, request.StrCMD1, request.StrCMD2, request.StrCMD3, request.StrUser, request.BShowDlg);
            return Task.FromResult(new Empty());
        }
        public override Task<BaseResult> GetSetListStr(IntegerDefine request, ServerCallContext context)
        {
            var result = this.CommandAppService.GetSetListStr(request.Result);
            return Task.FromResult(new BaseResult() { Result = result });
        }

        public override Task<StringResult> DoEquipSetItem(SetParm1_1Request request, ServerCallContext context)
        {
            var result = this.CommandAppService.DoEquipSetItem(request.EquipNo, request.SetNo, request.StrValue, request.StrUser, request.BShowDlg, "", request.RequestId);
            return Task.FromResult(new StringResult() { Result = result });
        }

    }
}
