// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Data.Model;
using IoTCenter.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Ganweisoft.IoTCenter.Module.Login;

public interface IUserManageService
{
    OperateResult<PagedResult<string>> GetUserDataList(CommonSearchPageModel commonSearchPageModel);
    Task<OperateResult<PagedResult<GetUsersDataModel>>> GetUsersList(CommonSearchPageModel request);

    Task<OperateResult<IEnumerable<UserRoleDataListModel>>> GetUserRoleDataList();

    Task<OperateResult> AddUserData(AddUserDataModel addUserDataModel);

    Task<OperateResult> EditUserData(EditUserDataModel editUserDataModel);

    Task<OperateResult> ResetUserPassword(ResetUserPwdModel model);

    Task<OperateResult> DelUserData(int Id);

    OperateResult<PagedResult<string>> GetRoleDataList(CommonSearchPageModel commonSearchPageModel);

    Task<OperateResult> AddRoleData(AddRoleDataModel addRoleDataModel);

    Task<OperateResult> EditRoleData(EditRoleDataModel editRoleDataModel);
    Task<OperateResult> DelRoleData(string roleName);


    Task<Gwuser> FindUser(string userName);

    Task<OperateResult> CreateAccountPasswordRule(AccountPasswordRuleModel model);
    Task<OperateResult<AccountPasswordRuleModel>> GetAccountPasswordRule();
    Task<OperateResult> CheckPassword(Gwuser user, int? failures, string oldPassword, AuditAction auditAction);
    Task<OperateResult<PasswordPolicyModel>> CheckFirstLogin(Gwuser user, AccountPasswordRuleModel rule);
    Task<OperateResult> LockUserAccount(Gwuser user, string encryptUserPassword,
        AccountPasswordRuleModel rule, string decryptUserName);
    Task<OperateResult<string>> Lock(int id);
    Task<OperateResult> Unlock(UnlockUserModel model);
    OperateResult<PasswordPolicyModel> CheckPasswordExpired(Gwuser user, AccountPasswordRuleModel rule);
}
