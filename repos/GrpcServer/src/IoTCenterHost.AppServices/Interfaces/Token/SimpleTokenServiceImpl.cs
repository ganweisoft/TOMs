//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenterHost.Core.Abstraction;
using Microsoft.Extensions.Configuration;
using System;

namespace IoTCenterHost.AppServices.Interfaces.Token
{
    public class SimpleTokenServiceImpl : BaseTokenServiceImpl, ITokenService
    {
        public SimpleTokenServiceImpl(IMemoryCacheService memoryCache, IConfiguration configuration) : base(memoryCache, configuration)
        {
        }
        public string WriteToken(string connectId, LoginUser loginUser)
        {
            string token = Guid.NewGuid().ToString();
            SetTokenToMemory(token, loginUser);
            return token;
        }
        public string RefreshToken(string connectId, string token)
        {
            LoginUser loginUser = GetLoginUser(token);
            return WriteToken(connectId, loginUser);
        }
    }
}
