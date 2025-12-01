// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Utilities;
using IoTCenterHost.Core.Abstraction;
using IoTCenterHost.Proxy.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Security.Claims;

namespace IoTCenterHost.Proxy
{
    public class HttpConnectStatusProvider : IConnectStatusProvider
    {

        private readonly IHttpContextAccessor _httpContext;
        private readonly IConnectPoolManager _connectPoolManager;
        private readonly IMemoryCacheService _memoryCacheService;
        private readonly ILoggingService _logService;
        private readonly IServiceProvider _serviceProvider;

        const string ServerTag = "servertag";
        private readonly ClaimsPrincipal _user;

        public HttpConnectStatusProvider(IHttpContextAccessor contextAccessor,
            IConnectPoolManager connectPoolManager,
            ILoggingService logService,IServiceProvider serviceProvider)
        {
            _httpContext = contextAccessor;
            _connectPoolManager = connectPoolManager;
            _logService = logService;
            _user = _httpContext.HttpContext?.User;
            _memoryCacheService = serviceProvider.GetService<IMemoryCacheService>();
        }
        private string GetClaim(string type) => _httpContext.HttpContext?.User?.FindFirst(d => d.Type == type)?.Value;
        #region IConnectStatusProvider的代码实现
        public ConnectionStatus ConnectionStatus
        {
            get
            {
                if (string.IsNullOrEmpty(Token))
                    return null;
                return _connectPoolManager.ConnectionStatusPool[Token];
            }
            set
            {
                if (_connectPoolManager.ConnectionStatusPool.ContainsKey(Token))
                    _connectPoolManager.ConnectionStatusPool[Token] = value;
            }
        }

        public string Token
        {
            get
            {
                string jtiValue = GetClaim(ServerTag);
                if (string.IsNullOrEmpty(jtiValue))
                {
                    jtiValue = GetAuthorization();
                }

                if (!string.IsNullOrEmpty(jtiValue))
                    return jtiValue;
                else
                    return _memoryCacheService.Get<string>("GrpcServerJtiValue")?.ToString() ?? string.Empty; //用于单点登录模块
            }
        }
        #endregion

        private string GetAuthorization()
        {
            var authorization = _httpContext?.HttpContext?.Request?.Headers["Authorization"].FirstOrDefault();

            if (string.IsNullOrEmpty(authorization))
            {
                return null;
            }

            if (authorization.StartsWith("Bearer" + " ", StringComparison.OrdinalIgnoreCase))
            {
                return authorization.Substring("Bearer".Length + 1).Trim();
            }

            return null;
        }
    }
}
