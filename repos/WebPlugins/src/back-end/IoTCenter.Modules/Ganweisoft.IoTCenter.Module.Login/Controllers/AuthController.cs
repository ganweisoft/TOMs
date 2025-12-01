// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Data;
using IoTCenter.Utilities;
using IoTCenterCore.DeviceDetection;
using IoTCenterCore.Hei.Captcha;
using IoTCenterCore.RsaEncrypt;
using IoTCenterCore.SlideVerificationCode;
using IoTCenterHost.Core.Abstraction;
using IoTCenterHost.Core.Abstraction.EnumDefine;
using IoTCenterHost.Core.Abstraction.IotModels;
using IoTCenterHost.Proxy.Core;
using IoTCenterWebApi.BaseCore;
using IoTCenterWebApi.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using IConnectService = IoTCenterHost.Proxy.IConnectService;
using SameSiteMode = Microsoft.AspNetCore.Http.SameSiteMode;

namespace Ganweisoft.IoTCenter.Module.Login.Controllers
{
    [ApiController]
    [Route("/IoT/api/v3/[controller]")]
    public class AuthController : DefaultController
    {
        private readonly IRSAAlgorithm _rsaAlgorithm;
        private readonly IConfiguration _configuration;
        private readonly IConnectService _connectService;
        private readonly IAuthService _authService;
        private readonly IMemoryCacheService _memoryCacheService;
        private readonly ILoggingService _apiLog;
        private readonly IUserManageService _userManageService;
        private readonly IServiceManageService _serviceManageService;
        private readonly GWDbContext _context;
        private readonly PermissionCacheService _permissionCacheService;
        private readonly IConnectStatusProvider _connectStatusProvider;

        public AuthController(
            IRSAAlgorithm rsaAlgorithm,
            IConfiguration configuration,
            IConnectService connectService,
            IAuthService authService,
            IMemoryCacheService memoryCacheService,
            IUserManageService userManageService,
            ILoggingService apiLog,
            GWDbContext context,
            PermissionCacheService permissionCacheService,
            IServiceManageService serviceManageService,
            IConnectStatusProvider connectStatusProvider)
        {
            _rsaAlgorithm = rsaAlgorithm;
            _configuration = configuration;
            _connectService = connectService;
            _authService = authService;
            _userManageService = userManageService;
            _memoryCacheService = memoryCacheService;
            _apiLog = apiLog;
            _context = context;
            _permissionCacheService = permissionCacheService;
            _serviceManageService = serviceManageService;
            _connectStatusProvider = connectStatusProvider;
        }

        [HttpGet("[action]")]
        [AllowAnonymous]
        public OperateResult GetVerificationCode()
        {
            return _authService.GetVerificationCode("0");
        }

        private string GetAccessToken(List<Claim> claims)
        {
            _ = int.TryParse(_configuration["WebApi:ExpiredTime"], out var expiredTime);


            var securityKey = _configuration["Authentication:JwtBearer:SecurityKey"];

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _configuration["Authentication:JwtBearer:Issuer"],
                _configuration["Authentication:JwtBearer:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(expiredTime <= 0 ? 120 : expiredTime),
                signingCredentials: credentials
            );

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            return accessToken;
        }

        [HttpPost("[action]")]
        [AllowAnonymous]
        public async Task<OperateResult> Login(LoginInputModel inputModel)
        {
            if (inputModel == null)
            {
                return OperateResult.Failed("请求参数为空");
            }

            var check = LoginCheck(inputModel);
            if (!check.Succeeded)
            {
                return check;
            }

            var userName = inputModel.UserName;
            var password = inputModel.Password;
            var verificationKey = inputModel.VerificationKey;
            var verificationCode = inputModel.VerificationCode;
            var clientAdrress = $"{HttpContext.Connection.RemoteIpAddress}:{HttpContext.Connection.RemotePort}";

            try
            {
                password = _rsaAlgorithm.Decrypt(password);
                userName = _rsaAlgorithm.Decrypt(userName);
            }
            catch (Exception ex)
            {
                _apiLog.Error("Login【解密用户名或密码失败】:" + ex);
                return OperateResult.Failed("输入帐号或密码不正确！");
            }

            if (!CheckServiceStatus(out string errorMsg))
            {
                return OperateResult.Failed(errorMsg);
            }
            var userNameAes = EncrypHelper.AESEncrypt(userName);
            var passwordAes = EncrypHelper.AESEncrypt(password);

            var model = _context.Gwuser.FirstOrDefault(d => d.Name == userNameAes);

            if (model == null)
            {
                return OperateResult.Failed("输入帐号或密码不正确");
            }

            if (model.UseExpiredTime.HasValue && model.UseExpiredTime.Value < DateTime.Now)
            {
                return OperateResult.Failed("您的账号使用时间已到期");
            }

            if (model.LockoutEnabled.HasValue && model.LockoutEnabled.Value
                && !model.LockoutEnd.HasValue)
            {
                return OperateResult.Failed("您已被平台锁定，禁止登录");
            }

            var rule = await _userManageService.GetAccountPasswordRule();
            var ruleData = rule.Data;

            var lockResult = await _userManageService.LockUserAccount(model, passwordAes, ruleData, userName);

            if (!lockResult.Succeeded)
            {
                return lockResult;
            }

            var responseModel = _connectService.Login(userNameAes, passwordAes, inputModel.SIVC ? GwClientType.Mobile : GwClientType.WebServer)
                .FromJson<ResponseModel>();
            if (responseModel.Code != 200)
            {
                return LoginFailed(userName, responseModel);
            }

            PasswordPolicyModel passwordPolicy;

            if (!model.FirstLogin.HasValue || !model.FirstLogin.Value)
            {
                var checkFirstLoginResult = await _userManageService.CheckFirstLogin(model, ruleData);
                passwordPolicy = checkFirstLoginResult.Data;
            }
            else
            {
                var checkPasswordExpiredResult = _userManageService.CheckPasswordExpired(model, rule.Data);
                passwordPolicy = checkPasswordExpiredResult.Data;
            }

            var loginResult = responseModel.Result.FromJson<LoginResult>();

            var userItem = loginResult.UserItem.FromJson<GWUserItem>();

            var roleName = userItem.Role_List != null ? userItem.Role_List.FirstOrDefault()?.name : "";

            if (string.IsNullOrWhiteSpace(roleName))
            {
                await _apiLog.Audit(new AuditAction(userName)
                {
                    ResourceName = "登录接口",
                    EventType = "登录事件",
                    Result = new AuditResult { Default = "登录失败" }
                });
                return OperateResult.Failed("您未被授予角色权限信息，请联系平台运维人员处理");
            }

            Request.Headers.TryGetValue(HeaderNames.UserAgent, out var userAgent);

            PermissionCache.SetUserAgent(false, userAgent);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userName),
                new Claim("userid", userItem.ID.ToString()),
                new Claim(ClaimTypes.Role, roleName),
                new Claim(ClaimTypes.Sid, PermissionCache.UserLogin(userName)),
                new Claim("servertag",loginResult.Token),
            };

            _ = int.TryParse(_configuration["WebApi:ExpiredTime"], out var expiredTime);

            var data = new
            {
                Code = 200,
                RoleName = roleName,
                IsSingleSignOn = false,
                PasswordPolicy = passwordPolicy
            };

            var xat = new CookieOptions()
            {
                Path = "/",
                HttpOnly = true,
                Secure = Request.IsHttps,
                SameSite = _configuration.GetAllowOrigins() ? SameSiteMode.None : SameSiteMode.Lax,
            };

            var claimsIdentity = new ClaimsIdentity(claims, "iot_user_login");

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            await _apiLog.Audit(new AuditAction(userName)
            {
                ResourceName = "登录接口",
                EventType = "登录事件",
                Result = new AuditResult { Default = $"登录成功-UserAgent({userAgent})" }
            });

            return OperateResult.Successed<object>(data);
        }

        private bool CheckServiceStatus(out string message)
        {
            message = string.Empty;
            var serviceStatus = _serviceManageService.GetServiceStatus();
            if (serviceStatus.ServiceStatus != ServiceStatus.Running)
            {
                message = "登录失败，服务未启动！";
                return false;
            }
            if (!serviceStatus.LicenseStatus)
            {
                message = "登录失败，许可未生效！";
                return false;
            }
            return true;
        }

        private OperateResult LoginCheck(LoginInputModel request)
        {
            if (string.IsNullOrWhiteSpace(request.UserName) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return OperateResult.Failed("用户名或密码为空");
            }

            var userAgent = Request.Headers["User-Agent"].ToString();

            var deviceInfo = Detector.TryParseUserAgent(userAgent);

            if (deviceInfo != null && Detector.IsMobile(deviceInfo.PlatformType.Value))
            {
                request.SIVC = true;
                return OperateResult.Success;
            }

            if (string.IsNullOrWhiteSpace(request.VerificationKey) ||
                string.IsNullOrWhiteSpace(request.VerificationCode))
            {
                return OperateResult.Failed("验证码为空");
            }

            switch (request.VerificationType)
            {
                case VerificationType.Code:
                    {
                        if (!VerificationCodeUtil.CodeIsTrue(request.VerificationKey, request.VerificationCode))
                        {
                            return OperateResult.Failed("验证码已过期或不正确");
                        }

                        break;
                    }
                case VerificationType.Slide:
                    {
                        var slideCodeKey = "SlideVerification-" + request.VerificationKey;
                        try
                        {
                            if (!_memoryCacheService.TryGetValue(slideCodeKey, out object position) ||
                            !int.TryParse(position.ToString(), out var positionValue) ||
                                !int.TryParse(request.VerificationCode, out var pointCode))
                            {
                                return OperateResult.Failed(40015, "验证码已过期或不正确");
                            }

                            if (Math.Abs(positionValue - pointCode) > CaptchaHelper._deviationPx)
                            {
                                return OperateResult.Failed(40016, "验证码不正确");
                            }

                        }
                        finally
                        {
                            _memoryCacheService.Remove(slideCodeKey);
                        }
                        break;
                    }
            }

            return OperateResult.Success;
        }

        private OperateResult LoginFailed(string userName, ResponseModel responseModel)
        {
            string clientAdrress = $"{HttpContext.Connection.RemoteIpAddress}:{HttpContext.Connection.RemotePort}";

            if (responseModel.Code == 400)
            {
                _apiLog.Audit(new AuditAction(userName)
                {
                    ResourceName = "登录接口",
                    EventType = "登录事件",
                    Result = new AuditResult { Default = "登录失败" }
                });

                return OperateResult.Failed(responseModel.Description);
            }

            _apiLog.Audit(new AuditAction(userName)
            {
                ResourceName = "登录接口",
                EventType = "登录事件",
                Result = new AuditResult { Default = "登录失败系统内部异常" }
            });

            return OperateResult.Failed(responseModel.Description);
        }

        [HttpPost("[action]")]
        public async Task<OperateResult> UpdUserInfoData(UpdUserInfoModel updUserInfoModel)
        {
            var userName = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
            return await _authService.UpdUserInfoData(updUserInfoModel, userName);
        }

        [HttpGet("[action]")]
        public OperateResult GetUserName2SF()
        {
            return _authService.GetUserName2SF();
        }

        [HttpGet("[action]")]
        public OperateResult GetName2SF()
        {
            return _authService.GetName2SF();
        }

        [HttpGet("[action]")]
        public OperateResult GetGrpcVersionInfo()
        {
            return _authService.GetGrpcVersionInfo();
        }

        [AllowAnonymous]
        [HttpGet("[action]")]
        public OperateResult GetString()
        {
            return OperateResult.Successed(new
            {
                cipher = _rsaAlgorithm.GetPublicCipher(),
                padding = string.IsNullOrEmpty(_configuration["WebApi:RSAPadding"]) ? "pkcs1" : _configuration["WebApi:RSAPadding"]
            });
        }

        [HttpGet("[action]")]
        [AllowAnonymous]
        public OperateResult<VerificationCodeResponseModel> GetSlideVerificationCode()
        {
            var result = _authService.GetSlideVerificationCode();

            return OperateResult.Successed(result);
        }

        [HttpGet("[action]")]
        [HttpPost("[action]")]
        [AllowAnonymous]
        public async Task<OperateResult> LoginOut()
        {
            try
            {
                _connectService.CloseSession();
            }
            catch (Exception ex)
            {
                _apiLog.Info($"关闭会话失败:ex\"[{ex}");
            }

            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            var userName = HttpContext.User?.FindFirst(ClaimTypes.Name)?.Value;

            if (!string.IsNullOrEmpty(userName))
            {
                _authService.RemoveUserToken(userName);
            }

            foreach (var key in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(key);
            }

            await _apiLog.Audit(new AuditAction()
            {
                ResourceName = "登录",
                EventType = "退出事件",
                Result = new AuditResult()
                {
                    Default = "注销退出成功"
                }
            });

            return new OperateResult { Code = 200, Message = "注销成功" };
        }
    }
}
