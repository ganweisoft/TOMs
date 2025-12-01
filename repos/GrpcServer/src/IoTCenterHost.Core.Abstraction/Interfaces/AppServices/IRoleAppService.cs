//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenterHost.Core.Abstraction.IotModels;

namespace IoTCenterHost.Core.Abstraction.Interfaces.AppServices
{
    public interface IRoleAppService
    {
        PaginationData GetRoleEntities(Pagination pagination);

        bool DeleteRole(string name);

        bool ModifyRole(GWRoleItem roleEntity);

        bool AddRole(GWRoleItem roleItem);
    }
}
