//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GWDataCenter;
using IoTCenterHost.Core.Abstraction.AppServices;
using IoTCenterHost.Core.ServerInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using IoTCenterHost.Proto;

namespace IoTCenterHost.AppServices.GrpcConstracts.IotHostService
{
    [Authorize]
    public partial class IotHostService : IotService.IotServiceBase
    {
        #region 私有字段 
        private readonly IServiceScopeFactory _serviceScopeFactory;

        private ILogger<IotHostService> _logger
        {
            get
            {
                return _serviceScopeFactory.CreateScope().ServiceProvider.GetService<ILogger<IotHostService>>();
            }
        }
        private IAccountAppService AccountAppService
        {
            get
            {
                return _serviceScopeFactory.CreateScope().ServiceProvider.GetService<IAccountAppService>();
            }
        }

        private IAlarmEventServerAppService AlarmEventAppService
        {
            get
            {
                return _serviceScopeFactory.CreateScope().ServiceProvider.GetService<IAlarmEventServerAppService>();
            }
        }

        private ICommandAppService CommandAppService
        {
            get
            {
                return _serviceScopeFactory.CreateScope().ServiceProvider.GetService<ICommandAppService>();
            }
        }
        private ICurveServerAppService CurveAppService
        {
            get
            {
                return _serviceScopeFactory.CreateScope().ServiceProvider.GetService<ICurveServerAppService>();
            }
        }
        private IEquipAlarmAppService EquipAlarmAppService
        {
            get
            {
                return _serviceScopeFactory.CreateScope().ServiceProvider.GetService<IEquipAlarmAppService>();
            }
        }
        private IEquipBaseServerService EquipBaseAppService
        {
            get
            {
                return _serviceScopeFactory.CreateScope().ServiceProvider.GetService<IEquipBaseServerService>();
            }
        }

        private IYCServerAppService YCAppService
        {
            get
            {
                return _serviceScopeFactory.CreateScope().ServiceProvider.GetService<IYCServerAppService>();
            }
        }
        private IYXServerAppService YXAppService
        {
            get
            {
                return _serviceScopeFactory.CreateScope().ServiceProvider.GetService<IYXServerAppService>();
            }
        }
        #endregion

        #region  构造方法
        public IotHostService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;

        }
        #endregion


        #region 接口实现的方法


        public override Task<BaseResult> GetPropertyFromPropertyService(GetPropertyFromPropertyServiceRequest request, ServerCallContext context)
        {
            return Task.FromResult(new BaseResult()
            {
                Result = DataCenter.GetPropertyFromPropertyService(request.PropertyName, request.NodeName, request.DefaultValue)
            });
        }

        public override Task<Empty> SetPropertyToPropertyService(GetPropertyFromPropertyServiceRequest request, ServerCallContext context)
        {
            DataCenter.SetPropertyToPropertyService(request.PropertyName, request.NodeName, request.DefaultValue);
            return Task.FromResult(new Empty());
        }
        #endregion
    }
}
