//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using IoTCenterHost.Core.Abstraction.IotModels;
using IoTCenterHost.Core.Extension;
using Microsoft.AspNetCore.Authorization;
using IoTCenterHost.Proto;

namespace IoTCenterHost.AppServices.GrpcConstracts.IotHostService
{
    public partial class IotHostService : IoTCenterHost.Proto.IotService.IotServiceBase
    {
        public override Task<Empty> AddChangedEquip(ChangedEquip request, ServerCallContext context)
        {
            EquipBaseAppService.AddChangedEquip(request.Map<GWDataCenter.ChangedEquip>());
            return Task.FromResult(new Empty());
        }
        public override Task<Empty> AddChangedEquipList(StringResult request, ServerCallContext context)
        {
            EquipBaseAppService.AddChangedEquipList(request.Result.FromJson<IEnumerable<GWDataCenter.ChangedEquip>>());
            return Task.FromResult(new Empty());
        }
        public override Task<Empty> DeleteDebugInfo(IntegerDefine request, ServerCallContext context)
        {
            return Task.FromResult(new Empty());
        }
        public override Task<Empty> GetChangedRTEquipItemData1(Empty request, ServerCallContext context)
        {
            EquipBaseAppService.GetChangedRTEquipItemData1();
            return Task.FromResult(new Empty());
        }
        public override Task<IntegerArrReply> GetEditRTEquipItemData(Empty request, ServerCallContext context)
        {
            var result = EquipBaseAppService.GetEditRTEquipItemData();
            var interArrResult = new IntegerArrReply();
            interArrResult.Result.Add(result);
            return Task.FromResult(interArrResult);
        }
        [AllowAnonymous]
        public override Task<GetEquipStateDictReply> GetEquipStateDict(Empty request, ServerCallContext context)
        {
            var result = EquipBaseAppService.GetEquipStateDict();
            var interArrResult = new GetEquipStateDictReply()
            {
                Result = result.ToJson()
            };
            return Task.FromResult(interArrResult);
        }
        public override Task<BoolDefine> GetEquipDebugState(IntegerDefine request, ServerCallContext context)
        {
            return Task.FromResult(new BoolDefine { Result = EquipBaseAppService.GetEquipDebugState(request.Result) });
        }
        [AllowAnonymous]
        public override Task<BaseResult> GetEquipListStr(Empty request, ServerCallContext context)
        {
            return Task.FromResult(new BaseResult { Result = EquipBaseAppService.GetEquipListStr() });
        }

        public override Task<IntegerArrReply> GetAddRTEquipItemData(Empty request, ServerCallContext context)
        {
            var interArrResult = new IntegerArrReply();
            interArrResult.Result.Add(EquipBaseAppService.GetAddRTEquipItemData());
            return Task.FromResult(interArrResult);
        }
        public override Task<IntegerArrReply> GetDelRTEquipItemData(Empty request, ServerCallContext context)
        {
            var interArrResult = new IntegerArrReply();
            interArrResult.Result.Add(EquipBaseAppService.GetDelRTEquipItemData());
            return Task.FromResult(interArrResult);
        }
        [AllowAnonymous]
        public override Task<Empty> GetTotalRTEquipItemData1(Empty request, ServerCallContext context)
        {
            EquipBaseAppService.GetTotalRTEquipItemData1();
            return Task.FromResult(new Empty());
        }

        public override Task<Empty> ResetEquipmentLinkage(Empty request, ServerCallContext context)
        {
            EquipBaseAppService.ResetEquipmentLinkage();
            return Task.FromResult(new Empty());
        }
        public override Task<Empty> ResetEquips(Empty request, ServerCallContext context)
        {
            EquipBaseAppService.ResetEquips();
            return Task.FromResult(new Empty());
        }

        public override Task<Empty> ResetEquipsEx(StringResult request, ServerCallContext context)
        {
            EquipBaseAppService.ResetEquips(request.Result.FromJson<List<int>>());
            return Task.FromResult(new Empty());
        }

        public override Task<Empty> SetEquipDebug(SetEquipDebugRequest request, ServerCallContext context)
        {
            EquipBaseAppService.SetEquipDebug(request.IEquipNo, request.BFlag);
            return Task.FromResult(new Empty());
        }
        public override Task<Empty> SetEquipNm(SetEquipNmDefine request, ServerCallContext context)
        {
            EquipBaseAppService.SetEquipNm(request.EquipNo, request.Nm);
            return Task.FromResult(new Empty());
        }
        [AllowAnonymous]
        public override Task<EquipStateReply> GetEquipStateFromEquipNo(IntegerDefine request, ServerCallContext context)
        {
            var stateReply = EquipBaseAppService.GetEquipStateFromEquipNo(request.Result);
            EquipStateReply reply = new EquipStateReply();
            reply.Result = stateReply.Map<EquipState>();
            return Task.FromResult(reply);
        }
        [AllowAnonymous]
        public override Task<StringResult> GetEquipStateFromEquipNoList(StringResult request, ServerCallContext context)
        {
            var equipList = request.Result.FromJson<IEnumerable<int>>();
            return Task.FromResult(new StringResult { Result = EquipBaseAppService.GetEquipStateDict(equipList).ToJson() });
        }

        [AllowAnonymous]
        public override Task<StringResult> GetEquipDict(StringResult request, ServerCallContext context)
        {
            return Task.FromResult(new StringResult { Result = EquipBaseAppService.GetEquipDict(request.Result.FromJson<Pagination>()).ToJson() });
        }
    }
}
