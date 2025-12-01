//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using GWDataCenter;
using GWDataCenter.Database;
using IoTCenterHost.Core.Abstraction.IotModels;

namespace IoTCenterHost.AppServices.Domain.DO.Role
{
    public interface IRoleRepository
    {
        IQueryable<GWRoleTableRow> Roles { get; }

        IEnumerable<RoleItem> GetRoleEntities(Pagination pagination, out int total);
        void AddRole(GWRoleTableRow roleEntity);
        bool DeleteRole(string name);
        bool ModifyRole(GWRoleTableRow roleEntity);
        int Count(string name);
    }
}
