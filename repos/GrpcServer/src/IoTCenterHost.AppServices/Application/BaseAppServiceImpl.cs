//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenterHost.AppServices.Domain.DO.User;
using IoTCenterHost.AppServices.Interfaces.Token;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IoTCenterHost.AppServices.AppServices
{
    public abstract class BaseAppServiceImpl
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly ILogger<BaseAppServiceImpl> _logger;
        protected BaseAppServiceImpl(IHttpContextAccessor contextAccessor, ILogger<BaseAppServiceImpl> logger)
        {
            _httpContext = contextAccessor;
            _logger = logger;
        }

        public LoginUser GetLoginUser()
        {
            LoginUser loginUser = null;
            try
            {
                string userName = string.Empty;
                var token = GetJti();
                var tokenService = _httpContext.HttpContext?.RequestServices.GetRequiredService<ITokenService>();
                loginUser = tokenService?.GetLoginUser(token);
                if (loginUser == null)
                {
                    userName = (_httpContext.HttpContext?.User.Identity as ClaimsIdentity)?.FindFirst(JwtRegisteredClaimNames.Name)?.Value;
                    if (!string.IsNullOrEmpty(userName))
                    {
                        loginUser = QueryUser(userName, token).Result;
                        tokenService.SetTokenToMemory(token, loginUser);
                    }
                    else
                    {
                        _logger.LogError($"读取凭据信息失败，请求头未携带令牌和用户名参数。");
                    }
                }

                if (loginUser == null && (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(userName)))
                {
                    throw new Exception($"读取用户信息失败");
                }
            }
            catch (Exception)
            {
            }

            return loginUser;
        }
        public string GetJti()
        {
            string token = string.Empty;
            try
            {
                try
                {
                    token = (_httpContext.HttpContext?.User.Identity as ClaimsIdentity)?.FindFirst("jti")?.Value;
                }
                catch
                {
                    if (string.IsNullOrEmpty(token))
                    {
                        StringValues stringValues = new StringValues();
                        _httpContext.HttpContext?.Request?.Headers?.TryGetValue("jti", out stringValues);
                        token = stringValues.FirstOrDefault();
                    }
                }

                return token;
            }
            catch (Exception)
            {
            }
            return string.Empty;
        }

        private async Task<LoginUser> QueryUser(string queryName, string token)
        {
            LoginUser loginUser;
            IUserRepository userRepository = (IUserRepository)_httpContext.HttpContext?.RequestServices.GetService(typeof(IUserRepository));
            var userEntity = await userRepository.GetUserEntity(queryName);
            var userItem = await userRepository.GetUserItem(userEntity);
            loginUser = new LoginUser
            {
                LoginMark = token,
                UserItem = userItem,
                UserName = userItem.UserName,
                LoginTime = DateTime.Now,
                Id = userItem.ID
            };
            return loginUser;
        }

        public string ConnectId
        {
            get
            {
                string connectId = string.Empty;

                if (_httpContext.HttpContext != null && _httpContext.HttpContext.Request.Headers.ContainsKey("connectId") && _httpContext.HttpContext.Request.Headers.TryGetValue("connectId", out StringValues connectIdStr) && connectIdStr != StringValues.Empty)
                    connectId = connectIdStr.ToString();
                if (string.IsNullOrEmpty(connectId))
                {
                    connectId = Guid.NewGuid().ToString("N");
                }
                return connectId;
            }
        }

    }
}
