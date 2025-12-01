//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using IoTCenterHost.Core.Abstraction.EnumDefine;
using IoTCenterHost.Core.Extension;
using Microsoft.AspNetCore.Authorization;
using IoTCenterHost.Proto;


namespace IoTCenterHost.AppServices.GrpcConstracts.IotHostService
{
    [Authorize]
    public partial class IotHostService : IotService.IotServiceBase
    {
        [AllowAnonymous]
        public override async Task<BaseResult> Login(LoginModel request, ServerCallContext context)
        {
            var response = await AccountAppService.Login(request.User, request.Pwd, request.CT.Map<GwClientType>());
            return response.Map<BaseResult>();
        }

        public override Task<Empty> CloseSession(Empty request, ServerCallContext context)
        {
            AccountAppService.CloseSession();
            return Task.FromResult(new Empty());
        }
    }
}
