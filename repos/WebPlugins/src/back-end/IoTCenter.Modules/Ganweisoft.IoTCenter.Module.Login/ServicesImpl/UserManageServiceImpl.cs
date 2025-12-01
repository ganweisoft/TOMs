// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Data;
using IoTCenter.Data.Model;
using IoTCenter.Utilities;
using IoTCenterCore.RsaEncrypt;
using IoTCenterHost.Core.Abstraction.Interfaces.AppServices;
using IoTCenterHost.Core.Abstraction.IotModels;
using IoTCenterWebApi.BaseCore;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Ganweisoft.IoTCenter.Module.Login;

public class UserManageServiceImpl : IUserManageService
{
    private readonly Session _session;

    private readonly IRSAAlgorithm _rsaAlgorithm;
    private static readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1);


    private readonly IUserAppService _userAppService;

    private readonly IMemoryCacheService _memoryCacheService;
    private readonly ILoggingService _apiLog;
    private readonly GWDbContext _context;
    private readonly PermissionCacheService _permissionCacheService;
    private readonly EquipBaseImpl _equipBaseImpl;

    public UserManageServiceImpl(
        IUserAppService userAppService,
        IMemoryCacheService memoryCacheService,
        ILoggingService apiLog,
        Session session,
        GWDbContext context,
        EquipBaseImpl equipBaseImpl,
        IRSAAlgorithm rsaAlgorithm,
        PermissionCacheService permissionCacheService)
    {
        _session = session;
        _rsaAlgorithm = rsaAlgorithm;
        _userAppService = userAppService;
        _memoryCacheService = memoryCacheService;
        _apiLog = apiLog;
        _context = context;
        _equipBaseImpl = equipBaseImpl;
        _permissionCacheService = permissionCacheService;
    }

    public OperateResult<PagedResult<string>> GetUserDataList(CommonSearchPageModel commonSearchPageModel)
    {
        if (commonSearchPageModel == null)
        {
            return OperateResult.Failed<PagedResult<string>>("返回请求参数为空");
        }

        var searchName = commonSearchPageModel.SearchName;

        Pagination pagination = new Pagination
        {
            WhereCause = searchName, //模糊搜索名称
            PageIndex = commonSearchPageModel.PageNo.Value,
            PageSize = commonSearchPageModel.PageSize.Value
        };

        var paginationData = _userAppService.GetUserEntities(pagination);

        return OperateResult.Successed(PagedResult<string>.Create(paginationData.Total, paginationData.Data));
    }

    public async Task<OperateResult<PagedResult<GetUsersDataModel>>> GetUsersList(CommonSearchPageModel request)
    {
        var usersData = new List<GetUsersDataModel>();

        var roles = await _context.Gwrole.ToListAsync();

        var descryptRoles = roles.Select(d => new
        {
            roleName = d.Name
        }).ToList();

        descryptRoles.Add(new { roleName = "admin" });

        var users = await _context.Gwuser.AsNoTracking().ToListAsync();

        foreach (var user in users)
        {
            var roleName = user.Roles;
            var isCheck = descryptRoles.Any(d => d.roleName == roleName);

            var dbUser = new GetUsersDataModel()
            {
                Id = user.Id,
                UserName = user.Name,
                Remark = user.Remark,
                ControlLevel = Convert.ToInt32(user.ControlLevel),
                role_List = new List<UserRoleModel>()
                {
                    new UserRoleModel()
                    {
                        Name = roleName,
                        IsCheck = isCheck
                    }
                },
                IsAdministrator = roleName.ToLower(CultureInfo.CurrentCulture) == "admin"
            };

            if (user.LockoutEnd.HasValue && user.LockoutEnd.Value <= DateTime.Now)
            {
                dbUser.LockoutEnabled = false;
            }
            else if (user.LockoutEnabled.HasValue && user.LockoutEnabled.Value)
            {
                dbUser.LockoutEnabled = true;
            }

            usersData.Add(dbUser);
        }

        if (!string.IsNullOrEmpty(request.SearchName))
        {
            usersData = usersData.Where(d => d.UserName.Contains(request.SearchName)).ToList();
        }

        var total = usersData.Count;

        var result = usersData.Skip(((request.PageNo ?? 1) - 1) * (request.PageSize ?? 0))
            .Take(request.PageSize ?? 10).ToList();

        return OperateResult.Successed(PagedResult<GetUsersDataModel>.Create(total, result));
    }

    public async Task<OperateResult<IEnumerable<UserRoleDataListModel>>> GetUserRoleDataList()
    {
        var query = await _context.Gwrole.ToListAsync();

        List<UserRoleDataListModel> lists = new List<UserRoleDataListModel>
        {
            new UserRoleDataListModel
            {
                Name = "ADMIN",
                Value = "管理员"
            }
        };

        for (var i = 0; i < query.Count; i++)
        {
            string roleNm = Convert.ToString(query[i].Name);
            var value = roleNm;
            UserRoleDataListModel obj = new UserRoleDataListModel
            {
                Name = value,
                Value = value,
            };
            lists.Add(obj);
        }

        return OperateResult.Successed<IEnumerable<UserRoleDataListModel>>(lists);
    }

    public async Task<OperateResult> AddUserData(AddUserDataModel addUserDataModel)
    {

        if (addUserDataModel == null)
        {
            return OperateResult.Failed("请求参数为空");
        }

        var userName = addUserDataModel.UserName;

        var userPwd = addUserDataModel.UserPwd;
        var remark = addUserDataModel.Remark;
        var controlLevel = addUserDataModel.ControlLevel;
        var roleList = addUserDataModel.RoleList.Distinct().ToList();

        if (!_session.IsAdmin)
        {
            if (roleList.FirstOrDefault().ToLower() == "admin")
            {
                return OperateResult.Failed("非管理员无法创建管理员账号");
            }
        }

        if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrEmpty(userPwd))
        {
            return OperateResult.Failed("请求参数为空");
        }

        if (controlLevel > 9 || controlLevel <= 0)
        {
            return OperateResult.Failed("优先级不合法，只支持1-9");
        }

        if (roleList.Count <= 0)
        {
            return OperateResult.Failed("请选择用户所属角色");
        }

        var homePageList = addUserDataModel.HomePageList;
        var autoInspectionPagesList = addUserDataModel.AutoInspectionPagesList;

        var decryptUserName = string.Empty;
        var decryptUserPwd = string.Empty;

        try
        {
            decryptUserName = _rsaAlgorithm.Decrypt(userName);
            decryptUserPwd = _rsaAlgorithm.Decrypt(userPwd);
        }
        catch (Exception ex)
        {
            _apiLog.Error("Login【更新用户名密码】:" + ex);
            return OperateResult.Failed("参数加密异常，解密失败");
        }

        if (decryptUserName.ToArray().Any(d => char.IsWhiteSpace(d)))
        {
            return OperateResult.Failed("用户名包含空格，请修改！");
        }

        var invalidCharacters = @"^[&®©<>\""'/]*$";

        var match = Regex.Match(decryptUserName, invalidCharacters);

        if (match.Success)
        {
            return OperateResult.Failed("用户名包含无效特殊字符，请修改！");
        }

        var decryptUserNameChars = decryptUserName.ToArray();

        var decryptUserPwdChars = decryptUserPwd.ToArray();

        var accountRulePolicy = await GetAccountPasswordRule();
        var ruleData = accountRulePolicy.Data;
        if (ruleData != null)
        {
            var accountRule = ruleData.Account;
            if (accountRule != null && accountRule.Enabled)
            {
                var checkAccountResult = CheckAccountRule(accountRule, decryptUserName);
                if (!checkAccountResult.Succeeded)
                {
                    return checkAccountResult;
                }
            }
            else if (decryptUserNameChars.Length < 6 || decryptUserNameChars.Length > 32)
            {
                return OperateResult.Failed("不满足安全设置，用户名要求长度为6-32位");
            }
            var passwordRule = ruleData.Password;
            if (passwordRule != null && passwordRule.Enabled)
            {
                var checkPasswordResult = CheckPasswordRule(passwordRule, decryptUserName, decryptUserPwd);
                if (!checkPasswordResult.Succeeded)
                {
                    return checkPasswordResult;
                }
            }
        }
        else if (decryptUserNameChars.Length < 6 || decryptUserNameChars.Length > 32)
        {
            return OperateResult.Failed("不满足安全设置，用户名要求长度为6-32位");
        }
        else if (decryptUserPwdChars.Length < 6 || decryptUserPwdChars.Length > 32)
        {
            return OperateResult.Failed("不满足安全设置，密码要求长度为6-32位");
        }

        var name = decryptUserName;
        var query = await _context.Gwuser.AsNoTracking().FirstOrDefaultAsync(x => x.Name == name);

        if (query != null)
        {
            return OperateResult.Failed("用户已经存在");
        }

        var roleNames = new List<string>() { "ADMIN" };
        var systemRoleNames = await _context.Gwrole.AsNoTracking().Select(d => d.Name).ToListAsync();
        var descryptRoleNames = systemRoleNames.Select(d => d).ToList();
        roleNames.AddRange(descryptRoleNames);

        if (roleList.Count(d => !roleNames.Any(r => r == d)) > 0)
        {
            return OperateResult.Failed("用户所属角色不存在");
        }

        var securityStamp = Guid.NewGuid().ToString("N");
        var maxUserId = _context.Gwuser.Max(x => x.Id) + 1;
        var newUser = new Gwuser()
        {
            Id = maxUserId,
            Name = decryptUserName,
            Password = decryptUserPwd,
            Remark = remark,
            Roles = (roleList.FirstOrDefault() ?? string.Empty),
            HomePages = string.Empty,
            AutoInspectionPages = string.Empty,
            ControlLevel = controlLevel.ToString(),
            PwdUpdateTime = DateTime.Now,
            AccessFailedCount = 0,
            LockoutEnabled = false,
        };
        newUser.SetSecurityStamp(securityStamp);

        newUser.HistoryPasswords = JsonConvert.SerializeObject(new List<string>()
        {
            decryptUserPwd
        });

        await _context.Gwuser.AddAsync(newUser);

        var result = await _context.SaveChangesAsync();

        await _apiLog.Audit(new AuditAction()
        {
            ResourceName = "权限管理",
            EventType = "新增用户",
            Result = new AuditResult()
            {
                Default = $"新增用户({decryptUserName})成功"
            }
        });

        return OperateResult.Successed(result > 0);
    }

    public async Task<OperateResult> ResetUserPassword(ResetUserPwdModel model)
    {
        if (model == null)
        {
            return OperateResult.Failed("请求参数为空，请检查！");
        }

        if (model.Id <= 0)
        {
            return OperateResult.Failed("用户标识为空，请检查！");
        }

        if (string.IsNullOrEmpty(model.Password))
        {
            return OperateResult.Failed("用户密码为空，请检查！");
        }

        var user = await _context.Gwuser.FirstOrDefaultAsync(x => x.Id == model.Id);

        var old = user;
        if (user == null)
        {
            return OperateResult.Failed("用户不存在");
        }

        var role = user.Roles;
        if (role.ToLower(CultureInfo.CurrentCulture) == "admin" && string.IsNullOrEmpty(model.OldPassWord))
        {
            return OperateResult.Failed("原密码为空，请检查");
        }

        var ruleData = (AccountPasswordRuleModel)null;

        var rule = await _context.PasswordRules.AsNoTracking().FirstOrDefaultAsync();

        if (rule != null)
        {
            ruleData = JsonConvert.DeserializeObject<AccountPasswordRuleModel>(rule.JSON);
        }

        var decryptUserName = string.Empty;

        try
        {
            model.Password = _rsaAlgorithm.Decrypt(model.Password);

            var passwordChars = model.Password.ToArray();
            if (passwordChars.Length < 6 || passwordChars.Length > 32)
            {
                return OperateResult.Failed("不满足安全设置，密码要求长度为6-32位");
            }

            decryptUserName = user.Name;

            model.UserName = decryptUserName;
        }
        catch (Exception ex)
        {
            _apiLog.Error("【重置用户密码异常】:" + ex);
            return OperateResult.Failed("解析用户信息失败，请联系平台管理员处理！");
        }

        if (_session.IsAdmin && !_session.UserName.Equals(decryptUserName))
        {
            return OperateResult.Failed("不允许修改其他管理员密码！");
        }

        try
        {
            await _semaphoreSlim.WaitAsync();

            var failures = ruleData?.Password?.Failures;

            if (role.ToLower(CultureInfo.CurrentCulture) == "admin")
            {
                var rsaDecryptPassword = _rsaAlgorithm.Decrypt(model.OldPassWord);
                var sha512EncryptPassword = rsaDecryptPassword;

                var checkModifyPasswordResult = await CheckPassword(user, failures, sha512EncryptPassword);

                if (!checkModifyPasswordResult.Succeeded)
                {
                    return OperateResult.Failed(checkModifyPasswordResult.Code, checkModifyPasswordResult.Message);
                }

                var npDiffOps = ruleData?.Password?.NpDiffOpAtLeastCharacters;
                if (npDiffOps.HasValue && npDiffOps.Value > 0)
                {
                    var differentCharacters = model.Password.ToCharArray().Except(rsaDecryptPassword.ToCharArray()).Count();
                    if (differentCharacters < npDiffOps)
                    {
                        return OperateResult.Failed($"新密码和原密码至少要满足【{npDiffOps}】个及以上不同字符");
                    }
                }
            }
        }
        finally
        {
            _semaphoreSlim.Release();
        }

        var historyPassword = model.Password;

        var shaResetPassword = model.Password;

        if (shaResetPassword.Equals(user.Password))
        {
            return OperateResult.Failed("重置密码与当前登录密码相同，请另设其他密码");
        }

        var passwordRule = ruleData?.Password;
        if (passwordRule != null && passwordRule.Enabled)
        {
            var checkPasswordResult = CheckPasswordRule(passwordRule, decryptUserName, model.Password);
            if (!checkPasswordResult.Succeeded)
            {
                return checkPasswordResult;
            }

            var historyPasswords = JsonConvert.DeserializeObject<List<string>>(user.HistoryPasswords ?? "");

            if (historyPasswords == null)
            {
                historyPasswords = new List<string>();
            }

            var passwordPolicyTimes = passwordRule.CheckedHistoryPolicy;
            if (passwordPolicyTimes > 0)
            {
                var firstPasswords = historyPasswords.Take(passwordPolicyTimes);

                if (firstPasswords.Contains(historyPassword))
                {
                    return OperateResult.Failed($"不满足安全设置，密码与前【{passwordPolicyTimes}】次历史设置密码相同");
                }
                else if (historyPasswords.Count < passwordPolicyTimes)
                {
                    historyPasswords.Add(historyPassword);
                    user.HistoryPasswords = JsonConvert.SerializeObject(historyPasswords);
                }
            }
        }

        var salt = Guid.NewGuid().ToString("N");
        var newPassWordSaltSHA = model.Password;

        user.Password = newPassWordSaltSHA;
        user.PwdUpdateTime = DateTime.Now;
        user.SetSecurityStamp(salt);

        if (!_context.ChangeTracker.HasChanges())
        {
            return OperateResult.Success;
        }

        if (await _context.SaveChangesAsync() < 0)
        {
            return OperateResult.Failed("更新密码失败");
        }

        _equipBaseImpl.SysEvtLog(_session.UserName, $"重置用户:{decryptUserName} 密码");

        await _apiLog.Audit(new AuditAction()
        {
            ResourceName = "权限管理",
            EventType = "重置密码",
            Result = new AuditResult()
            {
                Default = $"重置用户({decryptUserName})密码成功"
            }
        });

        return OperateResult.Success;
    }

    private OperateResult CheckAccountRule(Account accountRule, string decryptUserName)
    {
        if (decryptUserName.Length < accountRule.Length)
        {
            return OperateResult.Failed($"不满足安全设置，用户名至少要求长度为【{accountRule.Length}】");
        }
        var userNameCharArrays = decryptUserName.ToCharArray();
        var accountElements = accountRule.Elements;
        foreach (var accountElement in accountElements)
        {
            if (accountElement == Element.Lowercase && !userNameCharArrays.Any(d => char.IsLower(d)))
            {
                return OperateResult.Failed("不满足安全设置，用户名要求包含小写字母");
            }
            else if (accountElement == Element.Uppercase && !userNameCharArrays.Any(d => char.IsUpper(d)))
            {
                return OperateResult.Failed("不满足安全设置，用户名要求包含大写字母");
            }
            else if (accountElement == Element.Number && !userNameCharArrays.Any(d => char.IsNumber(d)))
            {
                return OperateResult.Failed("不满足安全设置，用户名要求包含数字");
            }
            else if (accountElement == Element.Symbol)
            {
                if (!userNameCharArrays.Any(d => char.IsSymbol(d)) && !userNameCharArrays.Any(d => char.IsPunctuation(d)))
                {
                    return OperateResult.Failed("不满足安全设置，用户名要求包含符号");
                }
            }
        }

        return OperateResult.Success;
    }

    private OperateResult CheckPasswordRule(Password passwordRule, string decryptUserName, string decryptUserPwd)
    {
        if (decryptUserPwd.Length < passwordRule.Length)
        {
            return OperateResult.Failed($"不满足安全设置，密码至少要求长度为【{passwordRule.Length}】");
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
            else if (passwordElement == Element.Symbol && !userPasswordCharArrays.Any(d => char.IsSymbol(d)) && !userPasswordCharArrays.Any(d => char.IsPunctuation(d)))
            {
                return OperateResult.Failed("不满足安全设置，密码要求包含符号");
            }
        }

        var minCharacter = userPasswordCharArrays.Distinct().Count();
        if (minCharacter < passwordRule.MinCharacters)
        {
            return OperateResult.Failed($"不满足安全设置，密码要求最少包含的【{passwordRule.MinCharacters}】个不同字符数");
        }

        if (!passwordRule.AllowedUserName && decryptUserPwd.Contains(decryptUserName))
        {
            return OperateResult.Failed("不满足安全设置，密码不能包含用户名");
        }

        return OperateResult.Success;
    }

    public async Task<OperateResult> EditUserData(EditUserDataModel editUserDataModel)
    {
        string remark = editUserDataModel.Remark;
        int controlLevel = editUserDataModel.ControlLevel;
        bool isAdministrator = editUserDataModel.IsAdministrator;
        List<string> roleList = editUserDataModel.RoleList.Distinct().ToList();
        List<string> homePageList = editUserDataModel.HomePageList;
        List<string> autoInspectionPagesList = editUserDataModel.AutoInspectionPagesList;


        if (editUserDataModel == null)
        {
            return OperateResult.Failed("请求参数为空");
        }

        if (roleList.Count <= 0)
        {
            return OperateResult.Failed("请选择用户所属角色");
        }

        if (!int.TryParse(editUserDataModel.Id, out var userId) || userId <= 0)
        {
            return OperateResult.Failed("用户不存在");
        }

        if (controlLevel > 9 || controlLevel <= 0)
        {
            return OperateResult.Failed("优先级不合法，只支持1-9");
        }

        var userModel = await _context.Gwuser.AsNoTracking().FirstOrDefaultAsync(x => x.Id == userId);

        if (userModel == null)
        {
            return OperateResult.Failed("用户不存在！");
        }

        var roleNames = new List<string>() { "ADMIN" };
        var systemRoleNames = await _context.Gwrole.AsNoTracking().Select(d => d.Name).ToListAsync();
        var descryptRoleNames = systemRoleNames.Select(d => d).ToList();
        roleNames.AddRange(descryptRoleNames);

        if (roleList.Count(d => !roleNames.Any(r => r == d)) > 0)
        {
            return OperateResult.Failed("用户所属角色不存在");
        }

        if (!string.IsNullOrEmpty(userModel.Roles))
        {
            var decryptRole = userModel.Roles;

            var dbRoles = decryptRole.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            editUserDataModel.RoleChanged = !roleList.SequenceEqual(dbRoles);
        }

        var roleItemList = new List<GWRoleItem>();
        var roleLength = roleList.Count > 1 ? 1 : roleList.Count;
        for (int i = 0; i < roleLength; i++)
        {
            var roleItem = new GWRoleItem { name = roleList[i] };
            roleItemList.Add(roleItem);
        }

        userModel.Remark = remark;
        userModel.Roles = (roleList.FirstOrDefault() ?? string.Empty);
        userModel.ControlLevel = controlLevel.ToString();

        _context.Gwuser.Update(userModel);

        if (await _context.SaveChangesAsync() <= 0)
        {
            return OperateResult.Failed("更新失败");
        }

        await _apiLog.Audit(new AuditAction()
        {
            ResourceName = "权限管理",
            EventType = "编辑用户",
            Result = new AuditResult<object, object>()
            {
                Default = "编辑成功",
                Old = editUserDataModel,
                New = userModel
            }
        });

        editUserDataModel.UserName = userModel.Name;

        return OperateResult.Success;
    }

    public async Task<OperateResult> DelUserData(int Id)
    {
        var user = await _context.Gwuser.FirstOrDefaultAsync(x => x.Id == Id);

        if (user == null)
        {
            return OperateResult.Failed("用户信息不存在！");
        }

        var uName = user.Name;
        if (_session.UserName == uName)
        {
            return OperateResult.Failed("当前账号与删除帐号相同，暂不支持删除");
        }

        var role = user.Roles;

        if (role.ToLower(CultureInfo.CurrentCulture) == "admin")
        {
            return OperateResult.Failed("不允许删除管理员！");
        }

        bool flag = _userAppService.DeleteUserEntity(Id);
        if (flag)
        {
            _memoryCacheService.Remove(uName);
        }

        PermissionCache.UserLogin(uName);

        _equipBaseImpl.SysEvtLog(_session.UserName, $"删除用户:{uName}");
        await _apiLog.Audit(new AuditAction()
        {
            ResourceName = "权限管理",
            EventType = "删除用户",
            Result = new AuditResult()
            {
                Default = $"删除用户({uName})成功"
            }
        });

        return OperateResult.Success;
    }

    public OperateResult<PagedResult<string>> GetRoleDataList(CommonSearchPageModel commonSearchPageModel)
    {
        if (commonSearchPageModel == null)
        {
            return OperateResult.Failed<PagedResult<string>>("返回请求参数为空");
        }

        var roleInfos = _permissionCacheService.GetPermissionObjList();
        if (!roleInfos.Any())
        {
            OperateResult.Successed(PagedResult<string>.Create(0, string.Empty));
        }

        string searchName = commonSearchPageModel.SearchName;

        if (!string.IsNullOrWhiteSpace(searchName))
        {
            roleInfos = roleInfos.Where(d => d.Name.Contains(searchName, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        if (commonSearchPageModel.PageNo != null && commonSearchPageModel.PageSize != null)
        {
            roleInfos = roleInfos
                .Skip((commonSearchPageModel.PageNo - 1).Value * commonSearchPageModel.PageSize.Value)
                .Take(commonSearchPageModel.PageSize.Value);
        }

        var result = roleInfos.Select(d =>
            new
            {
                Control_Equip_List = d.ControlEquips, // 可控设备
                Control_SetItem_List = d.ControlEquipsUnit, // 可控设备设置项(当可控设备中所有设置项被选择时为空)
                Browse_Equip_List = d.BrowseEquips, // 可看设备
                AddinModule_List = d.SystemModule, // 系统功能
                Browse_Pages_List = d.BrowsePages, // 可看页面
                Name = d.Name,
            });

        return OperateResult.Successed(PagedResult<string>.Create(result.Count(), result.ToJson()));
    }

    public async Task<OperateResult> AddRoleData(AddRoleDataModel addRoleDataModel)
    {
        if (addRoleDataModel == null)
        {
            return OperateResult.Failed("请求参数为空");
        }

        CustomFilterAttribute filter = new CustomFilterAttribute(_apiLog);
        if (filter.CheckKeyWord(addRoleDataModel.Remark))
        {
            return OperateResult.Failed("请求参数存在非法字符!");
        }

        string roleName = addRoleDataModel.RoleName;
        string remark = addRoleDataModel.Remark;
        List<int> controlEquipList = addRoleDataModel.ControlEquipList;
        List<string> controlSetItemList = addRoleDataModel.ControlSetItemList;
        List<int> browseEquipList = addRoleDataModel.BrowseEquipList;
        List<string> browseSpecialEquipList = addRoleDataModel.BrowseSpecialEquipList;
        List<int> browsePagesList = addRoleDataModel.BrowsePagesList;
        List<int> addinModuleList = addRoleDataModel.AddinModuleList;

        if (addinModuleList.All(d => d <= 0))
        {
            return OperateResult.Failed("请至少选择一项可看页面");
        }

        if (string.IsNullOrWhiteSpace(roleName))
        {
            return OperateResult.Failed("请求参数为空");
        }

        if (roleName.ToUpperInvariant() == "ADMIN")
        {
            return OperateResult.Failed("角色名已存在！");
        }

        var query = await _context.Gwrole.FirstOrDefaultAsync(x => x.Name == roleName);
        if (query != null)
            return OperateResult.Failed("角色名已存在！");

        List<string> realControlSetItemList = new List<string>();
        AddRoleDataOnw(controlSetItemList, realControlSetItemList);

        GWRoleItem roleItem = new GWRoleItem
        {
            name = roleName,
            remark = remark,
            Control_Equip_List = controlEquipList,
            Control_SetItem_List = realControlSetItemList,
            Browse_Equip_List = browseEquipList,
            Browse_SpecialEquip_List = browseSpecialEquipList,
            Browse_Pages_List = browsePagesList,
            AddinModule_List = addinModuleList
        };
        var role = EncryptItem(roleItem, query);

        _context.Gwrole.Add(role);

        _context.SaveChanges();

        _permissionCacheService.ReSetByRoleNames(roleName);

        _equipBaseImpl.SysEvtLog(_session.UserName, $"添加角色:{roleName}");

        await _apiLog.Audit(new AuditAction()
        {
            ResourceName = "权限管理",
            EventType = "添加角色",
            Result = new AuditResult<object, object>()
            {
                Default = $"添加角色({roleName})成功"
            }
        });

        return OperateResult.Successed(true);
    }
    private Gwrole EncryptItem(GWRoleItem roleItem, Gwrole roleEntity = null)
    {
        if (roleEntity == null)
            roleEntity = new Gwrole()
            {
                Name = roleItem.name,
                ControlEquips = string.Join('#', roleItem.Control_Equip_List),
                ControlEquipsUnit = string.Join('#', roleItem.Control_SetItem_List),
                BrowseEquips = string.Join('#', roleItem.Browse_Equip_List),
                BrowsePages = string.Join('#', roleItem.Browse_Pages_List),
                SpecialBrowseEquip = string.Join('#', roleItem.Browse_SpecialEquip_List),
                SystemModule = string.Join('#', roleItem.AddinModule_List),
                Remark = roleItem.remark,
            };
        else
        {
            roleEntity.Name = roleItem.name;
            roleEntity.ControlEquips = string.Join('#', roleItem.Control_Equip_List);
            roleEntity.ControlEquipsUnit = string.Join('#', roleItem.Control_SetItem_List);
            roleEntity.BrowseEquips = string.Join('#', roleItem.Browse_Equip_List);
            roleEntity.BrowsePages = string.Join('#', roleItem.Browse_Pages_List);
            roleEntity.SpecialBrowseEquip = string.Join('#', roleItem.Browse_SpecialEquip_List);
            roleEntity.SystemModule = string.Join('#', roleItem.AddinModule_List);
            roleEntity.Remark = roleItem.remark;
        }
        roleEntity.Name = roleEntity.Name;
        roleEntity.ControlEquips = roleEntity.ControlEquips;
        roleEntity.ControlEquipsUnit = roleEntity.ControlEquipsUnit;
        roleEntity.BrowseEquips = roleEntity.BrowseEquips;
        roleEntity.BrowsePages = roleEntity.BrowsePages;
        roleEntity.Remark = roleEntity.Remark;
        roleEntity.SpecialBrowseEquip = roleEntity.SpecialBrowseEquip;
        roleEntity.SystemModule = roleEntity.SystemModule;
        return roleEntity;
    }


    private static void AddRoleDataOnw(List<string> controlSetItemList, List<string> realControlSetItemList)
    {
        if (controlSetItemList != null)
        {
            foreach (var item in controlSetItemList)
            {
                var newItem = item;
                if (item.Contains("-", StringComparison.Ordinal))
                {
                    newItem = item.Replace("-", ".", StringComparison.Ordinal);
                }

                realControlSetItemList.Add(newItem);
            }
        }
    }

    public async Task<OperateResult> EditRoleData(EditRoleDataModel editRoleDataModel)
    {
        if (editRoleDataModel == null)
        {
            return OperateResult.Failed("请求参数为空");
        }

        string roleName = editRoleDataModel.RoleName;
        string remark = editRoleDataModel.Remark;
        var controlEquipList = editRoleDataModel.ControlEquipList;
        var controlSetItemList = editRoleDataModel.ControlSetItemList;
        var browseEquipList = editRoleDataModel.BrowseEquipList;
        var browseSpecialEquipList = editRoleDataModel.BrowseSpecialEquipList;
        var browsePagesList = editRoleDataModel.BrowsePagesList;
        var addinModuleList = editRoleDataModel.AddinModuleList;

        if (addinModuleList == null)
        {
            return OperateResult.Failed("请至少选择一项可看页面");
        }

        if (string.IsNullOrWhiteSpace(roleName))
        {
            return OperateResult.Failed("请求参数为空");
        }

        var query = await _context.Gwrole.FirstOrDefaultAsync(x => x.Name == roleName);
        if (query == null)
        {
            return OperateResult.Failed("角色名不存在！");
        }

        List<string> realControlSetItemList = new List<string>();

        if (controlSetItemList != null)
        {
            foreach (var item in controlSetItemList)
            {
                var newItem = item.Sid;
                if (newItem.Contains("-", StringComparison.Ordinal))
                {
                    newItem = newItem.Replace("-", ".", StringComparison.Ordinal);
                }

                realControlSetItemList.Add(newItem);
            }
        }

        GWRoleItem roleItem = new GWRoleItem
        {
            name = roleName,
            remark = remark,
            Control_Equip_List = controlEquipList.Select(d => d.Id).ToList(),
            Control_SetItem_List = realControlSetItemList,
            Browse_Equip_List = browseEquipList.Select(d => d.Id).ToList(),
            Browse_SpecialEquip_List = browseSpecialEquipList.Select(d => d.Sid).ToList(),
            Browse_Pages_List = browsePagesList.Select(d => d.Id).ToList(),
            AddinModule_List = addinModuleList.Select(d => d.Id).ToList()
        };

        var role = EncryptItem(roleItem);

        _context.Update(role);

        _context.SaveChanges();

        _permissionCacheService.ReSetByRoleNames(roleName);

        _equipBaseImpl.SysEvtLog(_session.UserName, $"编辑角色:{roleName}");

        await _apiLog.Audit(new AuditAction()
        {
            ResourceName = "权限管理",
            EventType = "编辑角色",
            Result = new AuditResult<object, object>() { Default = "编辑成功", Old = editRoleDataModel, New = roleItem }
        });

        return OperateResult.Successed(true);
    }

    public async Task<OperateResult> DelRoleData(string roleName)
    {
        var role = await _context.Gwrole.FirstOrDefaultAsync(d => d.Name == roleName);

        if (role == null)
        {
            return OperateResult.Failed("角色信息不存在！");
        }

        _context.Gwrole.Remove(role);

        _permissionCacheService.RemoveByRoleNames(_session.RoleName);

        if (await _context.SaveChangesAsync() <= 0)
        {
            return OperateResult.Failed("删除角色失败");
        }

        await _apiLog.Audit(new AuditAction()
        {
            ResourceName = "权限管理",
            EventType = "删除角色",
            Result = new AuditResult()
            {
                Default = $"删除角色({roleName})成功"
            }
        });

        _equipBaseImpl.SysEvtLog(_session.UserName, $"删除角色:{roleName}");

        return OperateResult.Success;
    }

    public async Task<Gwuser> FindUser(string userName)
    {
        var user = await _context.Gwuser.FirstOrDefaultAsync(u => u.Name == userName);
        return user;
    }

    public async Task<OperateResult> CreateAccountPasswordRule(AccountPasswordRuleModel model)
    {
        var rule = await _context.PasswordRules.FirstOrDefaultAsync();

        if (rule != null)
        {
            _context.PasswordRules.Remove(rule);
        }

        var json = JsonConvert.SerializeObject(model);

        await _context.PasswordRules.AddAsync(new IoTAccountPasswordRule()
        {
            JSON = json
        });

        if (await _context.SaveChangesAsync() <= 0)
        {
            return OperateResult.Failed("保存失败，请稍后重试");
        }

        await _apiLog.Audit(new AuditAction()
        {
            ResourceName = "安全设置",
            EventType = "配置修改",
            Result = new AuditResult<object, object>()
            {
                Default = "修改成功",
                Old = rule?.JSON ?? null,
                New = json
            }
        });

        _equipBaseImpl.SysEvtLog(_session.UserName, $"创建账号、密码规则:{json}");

        return OperateResult.Success;
    }

    public async Task<OperateResult<AccountPasswordRuleModel>> GetAccountPasswordRule()
    {
        var model = (AccountPasswordRuleModel)null;

        var rule = await _context.PasswordRules.FirstOrDefaultAsync();

        if (rule != null)
        {
            model = JsonConvert.DeserializeObject<AccountPasswordRuleModel>(rule.JSON);
        }

        return OperateResult.Successed(model);
    }

    public async Task<OperateResult> CheckPassword(Gwuser user, int? failures, string oldPassword, AuditAction auditAction = null)
    {
        if (!user.Password.Equals(oldPassword))
        {
            if (!failures.HasValue || failures.Value <= 0)
            {
                return OperateResult.Failed("原密码验证失败！");
            }

            if (user.AccessFailedCount >= failures)
            {
                user.LockoutEnabled = true;
                user.LockoutEnd = null;

                if (await _context.SaveChangesAsync() <= 0)
                {
                    return OperateResult.Failed("平台网络异常，请稍后重试！");
                }

                return OperateResult.Failed(-100, "您已被平台锁定，禁止登录");
            }

            user.AccessFailedCount++;

            var remainTimes = failures - user.AccessFailedCount;

            if (remainTimes <= 0)
            {
                user.LockoutEnabled = true;
                user.LockoutEnd = null;
            }

            _context.Gwuser.Update(user);

            if (await _context.SaveChangesAsync() <= 0)
            {
                return OperateResult.Failed("平台网络异常，请稍后重试！");
            }

            if (remainTimes <= 0)
            {
                if (auditAction != null)
                {
                    await _apiLog.Audit(auditAction);
                }

                return OperateResult.Failed(-100, "您已被平台锁定，禁止登录");
            }
            else
            {
                return OperateResult.Failed($"剩余【{remainTimes}】次验证原密码失败后，您将被平台锁定禁止登录！");
            }
        }

        user.AccessFailedCount = 0;
        user.LockoutEnd = null;
        user.LockoutEnabled = false;

        _context.Update(user);

        if (!_context.ChangeTracker.HasChanges())
        {
            return OperateResult.Success;
        }

        if (await _context.SaveChangesAsync() <= 0)
        {
            return OperateResult.Failed("平台网络异常，请稍后重试！");
        }

        return OperateResult.Success;
    }

    public async Task<OperateResult<PasswordPolicyModel>> CheckFirstLogin(Gwuser user, AccountPasswordRuleModel rule)
    {
        var policy = new PasswordPolicyModel()
        {
            PasswordPolicy = PasswordPolicy.Unlimited
        };

        if (rule == null || rule.Password == null || !rule.Password.Enabled)
        {
            user.FirstLogin = true;

            _context.Update(user);

            if (_context.ChangeTracker.HasChanges())
            {
                await _context.SaveChangesAsync();
            }

            return OperateResult.Successed(policy);
        }

        var passwordRule = rule.Password;

        if (user.PwdUpdateTime.HasValue && passwordRule.TermOfValidity > 0)
        {
            policy.ReminderDaysInAdvance = GetReminderDayInAdvance(user, passwordRule);
        }

        if (user.FirstLogin.HasValue && user.FirstLogin.Value)
        {
            return OperateResult.Successed(policy);
        }

        if (rule.Password.ForceModifyFirstLogin)
        {
            policy.PasswordPolicy = PasswordPolicy.EnforcementModify;
        }

        return OperateResult.Successed(policy);
    }

    public async Task<OperateResult> LockUserAccount(Gwuser user, string userEncryptPassword,
        AccountPasswordRuleModel rule, string userName)
    {
        var failures = rule?.Login?.Failures;
        var lockMinutes = rule?.Login?.Lock;

        try
        {
            await _semaphoreSlim.WaitAsync();

            if (!user.Password.Equals(userEncryptPassword))
            {
                if (rule == null || rule.Login == null || !rule.Login.Enabled || rule.Login.Failures <= 0)
                {
                    return OperateResult.Failed("输入帐号或密码不正确！");
                }

                if (!lockMinutes.HasValue || lockMinutes.Value <= 0)
                {
                    return OperateResult.Failed("输入帐号或密码不正确！");
                }

                if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTime.Now)
                {
                    return OperateResult.Failed($"您已被平台锁定，请{lockMinutes}分钟后再试！");
                }
                else if (user.LockoutEnabled.HasValue && user.LockoutEnabled.Value)
                {
                    user.LockoutEnabled = false;
                    user.AccessFailedCount = 0;
                    user.LockoutEnd = null;
                }

                if (user.LockoutEnabled.HasValue && user.LockoutEnabled.Value)
                {
                    return OperateResult.Failed($"您已被平台锁定，请{lockMinutes}分钟后再试！");
                }

                if (user.AccessFailedCount >= failures)
                {
                    user.LockoutEnabled = true;
                    user.LockoutEnd = DateTime.Now.AddMinutes(lockMinutes.Value);

                    if (await _context.SaveChangesAsync() <= 0)
                    {
                        return OperateResult.Failed("平台网络异常，请稍后重试！");
                    }

                    return OperateResult.Failed($"您已被平台锁定，请{lockMinutes}分钟后再试！");
                }

                user.AccessFailedCount++;

                var remainTimes = failures - user.AccessFailedCount;

                if (remainTimes <= 0)
                {
                    user.LockoutEnabled = true;
                    user.LockoutEnd = DateTime.Now.AddMinutes(lockMinutes.Value);
                }

                _context.Gwuser.Update(user);

                if (await _context.SaveChangesAsync() <= 0)
                {
                    return OperateResult.Failed("平台网络异常，请稍后重试！");
                }

                if (remainTimes > 0)
                {
                    return OperateResult.Failed("输入帐号或密码不正确！");
                }
                else
                {
                    await _apiLog.Audit(new AuditAction(userName)
                    {
                        ResourceName = "登录",
                        EventType = "登录事件",
                        Result = new AuditResult()
                        {
                            Default = $"登录失败({userName})次，现已被平台锁定，({lockMinutes})分钟后方可再试",
                        }
                    });

                    return OperateResult.Failed($"您已被平台锁定，请{lockMinutes}分钟后再试！");
                }
            }
            else
            {
                if (rule == null || rule.Login == null || !rule.Login.Enabled || rule.Login.Failures <= 0)
                {
                    return OperateResult.Success;
                }

                if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTime.Now)
                {
                    return OperateResult.Failed($"您已被平台锁定，请{lockMinutes}分钟后再试！");
                }

                if (user.LockoutEnabled.HasValue && user.LockoutEnabled.Value && user.LockoutEnd.HasValue)
                {
                    await _apiLog.Audit(new AuditAction(userName)
                    {
                        ResourceName = "登录",
                        EventType = "登录事件",
                        Result = new AuditResult()
                        {
                            Default = $"帐号自动解锁，解锁时间({user.LockoutEnd.Value.ToString("yyyy-MM-dd HH:mm:ss")})",
                        }
                    });
                }

                user.AccessFailedCount = 0;
                user.LockoutEnd = null;
                user.LockoutEnabled = false;
            }

            _context.Gwuser.Update(user);

            if (!_context.ChangeTracker.HasChanges())
            {
                return OperateResult.Success;
            }

            if (await _context.SaveChangesAsync() <= 0)
            {
                return OperateResult.Failed("平台网络异常，请稍后重试！");
            }

            return OperateResult.Success;
        }
        catch (Exception ex)
        {
            throw ex;
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }

    public async Task<OperateResult<string>> Lock(int id)
    {
        var user = await _context.Gwuser.FindAsync(id);

        if (user == null)
        {
            return OperateResult.Failed<string>("用户不存在");
        }

        var userName = user.Name;

        if (userName == _session.UserName)
        {
            return OperateResult.Failed<string>("当前账号与锁定帐号相同，暂不支持锁定");
        }

        user.LockoutEnabled = true;
        user.LockoutEnd = null;

        if (!_context.ChangeTracker.HasChanges())
        {
            return OperateResult.Successed(userName);
        }

        if (await _context.SaveChangesAsync() <= 0)
        {
            return OperateResult.Failed<string>("系统网络异常，请稍后重试！");
        }

        await _apiLog.Audit(new AuditAction()
        {
            ResourceName = "权限管理",
            EventType = "锁定",
            Result = new AuditResult()
            {
                Default = $"锁定帐号:【{userName}】"
            }
        });

        _equipBaseImpl.SysEvtLog(_session.UserName, "锁定用户:" + userName);

        return OperateResult.Successed(userName);
    }

    public async Task<OperateResult> Unlock(UnlockUserModel model)
    {
        if (!_session.IsAdmin)
        {
            return OperateResult.Failed("当前操作需管理员权限");
        }

        if (string.IsNullOrEmpty(model.Password))
        {
            return OperateResult.Failed("请输入密码");
        }

        var user = await _context.Gwuser.FindAsync(model.Id);

        if (user == null)
        {
            return OperateResult.Failed("用户不存在");
        }

        var userName = user.Name;

        if (userName == _session.UserName)
        {
            return OperateResult.Failed<string>("当前账号与解锁帐号相同，暂不支持解锁");
        }

        var adminAccount = await _context.Gwuser.AsNoTracking()
            .FirstOrDefaultAsync(d => d.Name == _session.UserName);

        if (adminAccount == null)
        {
            return OperateResult.Failed("当前操作人员不存在，请联系平台人员处理");
        }

        var ruleData = (AccountPasswordRuleModel)null;

        var rule = await _context.PasswordRules.AsNoTracking().FirstOrDefaultAsync();

        if (rule != null)
        {
            ruleData = JsonConvert.DeserializeObject<AccountPasswordRuleModel>(rule.JSON);
        }

        try
        {
            model.Password = _rsaAlgorithm.Decrypt(model.Password);
        }
        catch (Exception ex)
        {
            _apiLog.Error("解锁【解析管理员密码异常】:" + ex);
            return OperateResult.Failed("密码不正确，请重新输入密码");
        }

        try
        {
            await _semaphoreSlim.WaitAsync();

            var failures = ruleData?.Login?.Failures;

            var sha512EncryptPassword = model.Password;

            var auditAction = new AuditAction()
            {
                ResourceName = "帐号管理",
                EventType = "解锁",
                Result = new AuditResult()
                {
                    Default = "验证原密码失败，您已被平台锁定禁止登录",
                }
            };

            var checkModifyPasswordResult = await CheckPassword(adminAccount, failures, sha512EncryptPassword, auditAction);

            if (!checkModifyPasswordResult.Succeeded)
            {
                return OperateResult.Failed(checkModifyPasswordResult.Code, checkModifyPasswordResult.Message);
            }
        }
        finally
        {
            _semaphoreSlim.Release();
        }

        user.AccessFailedCount = 0;
        user.LockoutEnd = null;
        user.LockoutEnabled = false;

        if (!_context.ChangeTracker.HasChanges())
        {
            return OperateResult.Success;
        }

        if (await _context.SaveChangesAsync() <= 0)
        {
            return OperateResult.Failed("系统网络异常，请稍后重试！");
        }

        await _apiLog.Audit(new AuditAction()
        {
            ResourceName = "权限管理",
            EventType = "解锁",
            Result = new AuditResult()
            {
                Default = $"解锁帐号:【{userName}】成功"
            }
        });

        _equipBaseImpl.SysEvtLog(_session.UserName, "解锁用户:" + userName);

        return OperateResult.Success;
    }

    public OperateResult<PasswordPolicyModel> CheckPasswordExpired(Gwuser user,
        AccountPasswordRuleModel ruleData)
    {
        var policy = new PasswordPolicyModel()
        {
            PasswordPolicy = PasswordPolicy.Unlimited
        };

        if (!user.PwdUpdateTime.HasValue)
        {
            return OperateResult.Successed(policy);
        }

        if (ruleData == null || ruleData.Password == null)
        {
            return OperateResult.Successed(policy);
        }

        var passwordRule = ruleData.Password;
        var termOfValidity = passwordRule.TermOfValidity;

        if (!passwordRule.Enabled || termOfValidity <= 0)
        {
            return OperateResult.Successed(policy);
        }

        if (!user.PwdUpdateTime.HasValue)
        {
            return OperateResult.Successed(policy);
        }

        var passwordUpdateDate = user.PwdUpdateTime.Value;

        var passwordExpiredDate = passwordUpdateDate.AddDays(termOfValidity);

        policy.ReminderDaysInAdvance = GetReminderDayInAdvance(user, passwordRule);

        if (DateTime.Now > passwordExpiredDate)
        {
            policy.PasswordPolicy = passwordRule.AfterExpired;
        }

        return OperateResult.Successed(policy);
    }

    int? GetReminderDayInAdvance(Gwuser user, Password passwordRule)
    {
        var passwordUpdateDate = user.PwdUpdateTime.Value;
        var termOfValidity = passwordRule.TermOfValidity;

        if (!passwordRule.ExpirationReminder)
        {
            return null;
        }

        if (passwordRule.ReminderDaysInAdvance <= 0 || passwordRule.ReminderDaysInAdvance > termOfValidity)
        {
            return null;
        }

        var passwordExpiredDate = passwordUpdateDate.AddDays(termOfValidity);

        var reminderDateInAdvance = passwordExpiredDate.AddDays(-passwordRule.ReminderDaysInAdvance);

        if (DateTime.Now < reminderDateInAdvance)
        {
            return null;
        }

        var days = (int)Math.Round((passwordExpiredDate - DateTime.Now).TotalDays);

        return days;
    }
}
