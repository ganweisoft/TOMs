using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GWDataCenter; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IoTCenterHost.Core.Abstraction;
using Microsoft.AspNetCore.Authorization;
using IoTCenterHost.Core.Abstraction.Interfaces.AppServices;
using IoTCenterHost.Core.ServerInterfaces;
using IoTCenterHost.Core.Abstraction.IotModels;
using IoTCenterHost.Core.Extension;
using IoTCenterHost.Proto;
using IoTCenter.Extensions.Logging;

namespace IoTCenterHost.AppServices.GrpcConstracts
{
    [Authorize]
    public class SystemManageService : SystemManage.SystemManageBase
    {
        private readonly IUserAppServerService _userAppService;
        private readonly ILogger _logger;
        public SystemManageService( IUserAppServerService userAppService, ILogger logger)
        {
            _userAppService = userAppService;
            _logger = logger;
        }

        [AllowAnonymous]
        public override Task<Empty> UpdateUserTable(Empty request, ServerCallContext context)
        {
            StationItem.UpdateUserTable();
            _logger.Info($"StationItem.UpdateUserTable调用成功");
            return Task.FromResult(new Empty());
        }
    }
}
