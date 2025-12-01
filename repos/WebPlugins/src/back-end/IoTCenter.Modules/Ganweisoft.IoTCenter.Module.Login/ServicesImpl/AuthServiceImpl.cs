// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Data;
using IoTCenter.Utilities;
using IoTCenterCore.Hei.Captcha;
using IoTCenterCore.RsaEncrypt;
using IoTCenterCore.SlideVerificationCode;
using IoTCenterHost.Core.Abstraction.Services;
using IoTCenterWebApi.BaseCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Ganweisoft.IoTCenter.Module.Login;

public class AuthServiceImpl : IAuthService
{
    private readonly IRSAAlgorithm _rsaAlgorithm;

    private static readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1);

    private readonly IConfiguration _configuration;
    private readonly IUserManageService _userManagerService;
    private readonly IotCenterHostService _proxy;
    private readonly IMemoryCacheService _memoryCacheService;
    private readonly EquipBaseImpl _equipBaseImpl;
    private readonly ILoggingService _apiLog;
    private readonly GWDbContext _context;
    public AuthServiceImpl(
        IRSAAlgorithm rsaAlgorithm,
        IotCenterHostService alarmCenterService,
        IMemoryCacheService memoryCacheService,
        EquipBaseImpl equipBaseImpl,
        ILoggingService apiLog,
        IConfiguration configuration,
        IUserManageService userManageService,
        GWDbContext context)
    {
        _rsaAlgorithm = rsaAlgorithm;
        _context = context;
        _proxy = alarmCenterService;
        _memoryCacheService = memoryCacheService;
        this._equipBaseImpl = equipBaseImpl;
        _apiLog = apiLog;
        _userManagerService = userManageService;
        _configuration = configuration;
    }

    public OperateResult<object> GetVerificationCode(string codeType)
    {
        var result = VerificationCodeUtil.GetImageCode(codeType);

        return OperateResult.Successed<object>(new
        {
            VerificationKey = result.Item1,
            VerificationCode = result.Item2
        });
    }

    public VerificationCodeResponseModel GetSlideVerificationCode()
    {
        var execDir = Directory.GetCurrentDirectory();
        var parentDir = Path.Combine(execDir, "wwwroot", "gallery");
        var captcha = new CaptchaHelper();
        var code = captcha.GetVerificationCode(parentDir);

        var guid = Guid.NewGuid().ToString();

        _memoryCacheService.Set("SlideVerification-" + guid, captcha._PositionX.ToString(), DateTimeOffset.Now.AddMinutes(1));

        return new VerificationCodeResponseModel
        {
            VerificationKey = guid,
            VerificationCode = code
        };
    }

    public OperateResult<object> GetVerificationCodeByKey(string codeType)
    {
        codeType = string.IsNullOrEmpty(codeType) ? "0" : codeType;
        var result = VerificationCodeUtil.GetValue(codeType);

        return OperateResult.Successed<object>(new
        {
            VerificationKey = result.Item1,
            VerificationCode = result.Item2
        });
    }

    public async Task<OperateResult> UpdUserInfoData(UpdUserInfoModel updUserInfoModel, string userName)
    {
        if (updUserInfoModel == null)
        {
            return OperateResult.Failed("请求参数为空，请检查！");
        }

        var oldPassWord = updUserInfoModel.oldPassWord;
        var newPassWord = updUserInfoModel.newPassWord;
        if (string.IsNullOrWhiteSpace(oldPassWord) || string.IsNullOrWhiteSpace(newPassWord))
        {
            return OperateResult.Failed("请求相关参数为空，请检查！");
        }

        if (string.IsNullOrWhiteSpace(userName))
        {
            return OperateResult.Failed("抱歉，您暂无权限处理进行此操作！");
        }

        var ruleData = (AccountPasswordRuleModel)null;

        var rule = await _context.PasswordRules.AsNoTracking().FirstOrDefaultAsync();

        if (rule != null)
        {
            ruleData = JsonConvert.DeserializeObject<AccountPasswordRuleModel>(rule.JSON);
        }

        try
        {
            oldPassWord = _rsaAlgorithm.Decrypt(oldPassWord);
            newPassWord = _rsaAlgorithm.Decrypt(newPassWord);
        }
        catch (Exception ex)
        {
            await _apiLog.Audit(new AuditAction(userName)
            {
                ResourceName = "帐号管理",
                EventType = "更新密码",
                Result = new AuditResult { Default = "更新登录密码失败" }
            });

            return OperateResult.Failed("无效密码，解密失败！");
        }

        var user = await _context.Gwuser.FirstOrDefaultAsync(x => x.Name == userName);
        if (user == null)
        {
            return OperateResult.Failed("指定用户不存在！");
        }

        var oldPassWordSHA = oldPassWord;
        var newPassWordSHA = newPassWord;

        var newHistoryPassword = newPassWord;

        var failures = ruleData?.Password?.Failures;
        if (user.LockoutEnabled.HasValue && user.LockoutEnabled.Value
            && !user.LockoutEnd.HasValue)
        {
            return OperateResult.Failed(-100, "您已被平台锁定");
        }

        try
        {
            await _semaphoreSlim.WaitAsync();

            var auditAction = new AuditAction(userName)
            {
                ResourceName = "帐号管理",
                EventType = "更新密码",
                Result = new AuditResult { Default = "验证原密码失败，您已被平台锁定禁止登录" }
            };

            var checkModifyPasswordResult = await _userManagerService.CheckPassword(user, failures, oldPassWordSHA, auditAction);

            if (!checkModifyPasswordResult.Succeeded)
            {
                return OperateResult.Failed(checkModifyPasswordResult.Code, checkModifyPasswordResult.Message);
            }
        }
        finally
        {
            _semaphoreSlim.Release();
        }

        if (newPassWordSHA.Equals(user.Password))
        {
            return OperateResult.Failed("修改密码与当前登录密码相同，请另设其他密码");
        }

        var passwordRule = ruleData?.Password;
        if (passwordRule != null && passwordRule.Enabled)
        {
            var checkPasswordResult = CheckPasswordRule(passwordRule, userName, newPassWord);
            if (!checkPasswordResult.Succeeded)
            {
                return OperateResult.Failed(checkPasswordResult.Message);
            }

            var historyPasswords = JsonConvert.DeserializeObject<List<string>>(user.HistoryPasswords ?? "");

            if (historyPasswords == null)
            {
                historyPasswords = new List<string>();
            }

            var npDiffOps = passwordRule.NpDiffOpAtLeastCharacters;
            if (npDiffOps > 0)
            {
                var differentCharacters = newPassWord.ToCharArray().Except(oldPassWord.ToCharArray()).Count();
                if (differentCharacters < npDiffOps)
                {
                    return OperateResult.Failed($"新密码和原密码至少要满足【{npDiffOps}】个及以上不同字符");
                }
            }

            var passwordPolicyTimes = passwordRule.CheckedHistoryPolicy;

            if (passwordPolicyTimes > 0)
            {
                var firstPasswords = historyPasswords.Take(passwordPolicyTimes);

                if (firstPasswords.Contains(newHistoryPassword))
                {
                    return OperateResult.Failed($"不满足安全设置，密码与前“{passwordPolicyTimes}”次历史设置密码相同");
                }
                else if (historyPasswords.Count < passwordPolicyTimes)
                {
                    historyPasswords.Add(newHistoryPassword);
                    user.HistoryPasswords = JsonConvert.SerializeObject(historyPasswords);
                }
            }
        }

        var salt = Guid.NewGuid().ToString("N");
        var newPassWordSaltSHA = newPassWord;
        user.Password = newPassWordSaltSHA;
        user.Name = userName;
        user.PwdUpdateTime = DateTime.Now;
        user.SetSecurityStamp(salt);

        if (!user.FirstLogin.HasValue || !user.FirstLogin.Value)
        {
            user.FirstLogin = true;
        }

        if (!_context.ChangeTracker.HasChanges())
        {
            return OperateResult.Success;
        }

        if (await _context.SaveChangesAsync() < 0)
        {
            return OperateResult.Failed("更新用户名密码失败");
        }

        await _apiLog.Audit(new AuditAction()
        {
            ResourceName = "登录接口",
            EventType = "修改密码事件",
            Result = new AuditResult { Default = "更新登录密码成功" },
        });

        return OperateResult.Success;
    }

    private OperateResult CheckPasswordRule(Password passwordRule, string decryptUserName, string decryptUserPwd)
    {
        if (decryptUserPwd.Length < passwordRule.Length)
        {
            return OperateResult.Failed($"不满足安全设置，密码至少要求长度为“{passwordRule.Length}”");
        }
        var userPasswordCharArrays = decryptUserPwd.ToCharArray();
        var passwordElements = passwordRule.Elements;
        foreach (var passwordElement in passwordElements)
        {
            if (passwordElement == Element.Lowercase && !userPasswordCharArrays.Any(d => char.IsLower(d)))
            {
                return OperateResult.Failed("不满足安全设置，密码要求包含小写字母");
            }
            else if (passwordElement == Element.Uppercase && !userPasswordCharArrays.Any(d => char.IsUpper(d)))
            {
                return OperateResult.Failed("不满足安全设置，密码要求包含大写字母");
            }
            else if (passwordElement == Element.Number && !userPasswordCharArrays.Any(d => char.IsNumber(d)))
            {
                return OperateResult.Failed("不满足安全设置，密码要求包含数字");
            }
            else if (passwordElement == Element.Symbol && !userPasswordCharArrays.Any(d => char.IsPunctuation(d) && !userPasswordCharArrays.Any(d => char.IsSymbol(d))))
            {
                return OperateResult.Failed("不满足安全设置，密码要求包含符号");
            }
        }

        var minCharacter = userPasswordCharArrays.Distinct().Count();
        if (minCharacter < passwordRule.MinCharacters)
        {
            return OperateResult.Failed($"不满足安全设置，密码要求最少包含的“{passwordRule.MinCharacters}”个不同字符数");
        }

        if (!passwordRule.AllowedUserName && decryptUserName.Equals(decryptUserPwd, StringComparison.OrdinalIgnoreCase))
        {
            return OperateResult.Failed("不满足安全设置，要求密码不能与帐号相同");
        }

        return OperateResult.Success;
    }

    public OperateResult GetUserName2SF()
    {
        return OperateResult.Successed("OpenIoTCenter");
    }

    public OperateResult GetName2SF()
    {
        return OperateResult.Successed("OpenIoTCenter");
    }

    public OperateResult GetGrpcVersionInfo()
    {
        return OperateResult.Successed(_proxy.GetVersionInfo());
    }

    public void RemoveUserToken(string userName)
    {
        if (string.IsNullOrWhiteSpace(userName))
        {
            return;
        }

        _apiLog.Audit(new AuditAction(userName)
        {
            ResourceName = "登录接口",
            EventType = "登录事件",
            Result = new AuditResult { Default = "登出平台-成功" },

        });
        _memoryCacheService.Remove(userName);
        _equipBaseImpl.SysEvtLog(userName, "登出平台");
    }
}
