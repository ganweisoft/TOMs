//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenterHost.Core.Abstraction;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace IoTCenterHost.AppServices.Interfaces.Token
{
    public class JwtTokenServiceImpl : BaseTokenServiceImpl, ITokenService
    {
        private readonly ILogger<JwtTokenServiceImpl> _logger;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly string _securityKey;
        private readonly int _expiredTime;

        public JwtTokenServiceImpl(IMemoryCacheService memoryCache, IConfiguration configuration, ILogger<JwtTokenServiceImpl> logger)
            : base(memoryCache, configuration)
        {
            _logger = logger;
            _issuer = configuration["Issuer"];
            _audience = configuration["Audience"];
            _securityKey = configuration["SecurityKey"];
            _expiredTime = int.TryParse(configuration["ExpiredTime"], out var time) ? time : 1440;
        }

        public string WriteToken(string connectId, LoginUser loginUser)
        {
            if (!ValidateConfig(loginUser)) return string.Empty;

            SetTokenToMemory(loginUser.LoginMark, loginUser);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Jti, loginUser.LoginMark),
                new Claim(JwtRegisteredClaimNames.UniqueName, loginUser.UserName),
                new Claim(JwtRegisteredClaimNames.Name, loginUser.UserName)
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_securityKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(_expiredTime),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        }

        public string RefreshToken(string connectId, string token)
        {
            var loginUser = GetLoginUser(token);
            return loginUser != null ? WriteToken(connectId, loginUser) : string.Empty;
        }

        private bool ValidateConfig(LoginUser loginUser)
        {
            if (string.IsNullOrEmpty(_issuer))
            {
                _logger.LogError("Issuer 配置为空");
                return false;
            }
            if (string.IsNullOrEmpty(_audience))
            {
                _logger.LogError("Audience 配置为空");
                return false;
            }
            if (string.IsNullOrEmpty(_securityKey))
            {
                _logger.LogError("securityKey 配置为空");
                return false;
            }
            if (string.IsNullOrEmpty(loginUser?.UserName))
            {
                _logger.LogError("用户信息为空");
                return false;
            }
            return true;
        }
    }
}
