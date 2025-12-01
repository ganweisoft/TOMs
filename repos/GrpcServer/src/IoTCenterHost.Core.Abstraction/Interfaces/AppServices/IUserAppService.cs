//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenterHost.Core.Abstraction.IotModels;

namespace IoTCenterHost.Core.Abstraction.Interfaces.AppServices
{
    public interface IUserAppService
    {
        PaginationData GetUserEntities(Pagination pagination);

        bool DeleteUserEntity(int Id);
        bool AddUserEntity(GWUserItem userItem);
        bool RevisePassword(GWUserItem userItem);

        bool ModifyUserEntity(GWUserItem userEntity);

    }
}
