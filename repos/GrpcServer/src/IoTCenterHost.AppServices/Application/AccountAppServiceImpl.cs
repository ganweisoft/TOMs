//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenterHost.AppServices.Domain.DO.User;
using IoTCenterHost.AppServices.Interfaces;
using IoTCenterHost.AppServices.Interfaces.Token;
using IoTCenterHost.Core.Abstraction;
using IoTCenterHost.Core.Abstraction.AppServices;
using IoTCenterHost.Core.Abstraction.EnumDefine;
using IoTCenterHost.Core.Abstraction.IotModels;
using IoTCenterHost.Core.Extension;
using IoTCenterHost.Core.ServerInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace IoTCenterHost.AppServices.AppServices
{
    public class AccountAppServiceImpl : BaseAppServiceImpl, IAccountAppService
    {
        private readonly IUserRepository UserRepository;
        private readonly ITokenService TokenService;
        private readonly IotCenterService _iotCenterService;
        private readonly IotRealTimeDataService iotRealTimeDataService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStringLocalizer<LocalizationResource> _localizer;
        private readonly IAlarmEventServerAppService _alarmEventServerAppService;
        private readonly ILogger<AccountAppServiceImpl> _logger;
        public AccountAppServiceImpl(IUserRepository userRepository,
                                     ITokenService tokenService,
                                     IotRealTimeDataService realTimeDataService,
                                     IHttpContextAccessor contextAccessor,
                                     IStringLocalizer<LocalizationResource> localizer,
            IotCenterService iotCenterService,
            IAlarmEventServerAppService alarmEventServerAppService,
            ILogger<AccountAppServiceImpl> logger
            ) : base(contextAccessor, logger)
        {
            UserRepository = userRepository;
            TokenService = tokenService;
            iotRealTimeDataService = realTimeDataService;
            _httpContextAccessor = contextAccessor;
            _localizer = localizer;
            _iotCenterService = iotCenterService;
            _alarmEventServerAppService = alarmEventServerAppService;
            _logger = logger;
        }

        public async Task<ResponseModel> Login(string user, string pwd, GwClientType client)
        {
            string loginMark = Guid.NewGuid().ToString("N");
            var userEntity = await UserRepository.GetUserEntity(user, pwd);
            if (userEntity != null && userEntity.ID > 0)
            {
                try
                {
                    if (userEntity.UseExpiredTime.HasValue)
                    {
                        var useExpiredTime = userEntity.UseExpiredTime.Value;
                        if (useExpiredTime.Year > 1900 && useExpiredTime < DateTime.Now)
                        {
                            return new ResponseModel()
                            {
                                Code = (int)HttpStatusCode.BadRequest,
                                Description = $"{_localizer["UserUseExpiredTime"].Value}",
                                Result = ""
                            };
                        }
                    }

                    var userLoginItem = await UserRepository.GetUserItem(userEntity, true);
                    string connectId = Guid.NewGuid().ToString();
                    if (_httpContextAccessor.HttpContext != null)
                    {
                        _ = _httpContextAccessor.HttpContext.Request.Headers.TryGetValue("connectId", out StringValues stringValues);
                        connectId = stringValues.FirstOrDefault();
                    }
                    OnLoginSuccess(connectId, user, loginMark, userLoginItem, client);
                    var result = ResponseToken(connectId, user, loginMark, userLoginItem);
                    return result;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"{_localizer["BadUserInput"].Value}：{ex.Message},{ex.StackTrace}");
                    throw ex;
                }
            }
            else
            {
                return new ResponseModel()
                {
                    Code = (int)HttpStatusCode.BadRequest,
                    Description = $"{_localizer["BadUserInput"].Value}",
                    Result = ""
                };
            }
        }

        private ResponseModel ResponseToken(string connectId, string user, string loginMark, GWUserItem userLoginItem)
        {
            return new ResponseModel()
            {
                Code = (int)HttpStatusCode.OK,
                Result = new LoginResult
                {
                    Token = TokenService.WriteToken(connectId, new LoginUser()
                    {
                        NickName = user,
                        UserName = user,
                        LoginTime = System.DateTime.Now,
                        LoginMark = loginMark,
                        UserItem = userLoginItem,
                    }),
                    UserItem = userLoginItem.ToJson()
                }.ToJson(),
                Description = ""
            };
        }
        private void OnLoginSuccess(string connectId, string user, string loginMark, GWUserItem userLoginItem, GwClientType client)
        {
            Task.Factory.StartNew(() =>
            {
                var realItem = iotRealTimeDataService.GetIotRealTimeData(connectId);
                realItem.AddUser(loginMark, userLoginItem);
            });
        }

        public void CloseSession()
        {
            TokenService.SetTokenEmpired(base.GetJti());
        }
    }
}
