//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenterHost.Core.Abstraction.EnumDefine;
using IoTCenterHost.Core.Abstraction.IotModels;
using System.Linq.Expressions;

namespace IoTCenterHost.AppServices.Domain.DO.User
{
    public interface IUserRepository
    {
        IQueryable<UserEntity> Users { get; }
        Task<UserEntity> GetUserEntity(string uid);
        IEnumerable<UserEntity> GetUserEntities(Expression<Func<UserEntity, bool>> expression);
        Task<UserEntity> FirstOrDefault(Expression<Func<UserEntity, bool>> expression);
        bool CheckUserInfo(string user, string password);

        Task<UserEntity> GetUserEntity(string user, string password = null);
        Task<GWUserItem> GetUserItem(UserEntity user, bool isClient = true, GwClientType gwClientType = GwClientType.Web);

        int Count(string userName);
        bool Any(string userName);
        IEnumerable<GWUserItem> GetUserEntities(Pagination pagination, out int total);
        void AddUserEntity(UserEntity userEntity);
        void DeleteUserEntity(int Id);
        void ModifyUserEntity(UserEntity userEntity);

        bool RevisePassword(int userId, string password);


    }
}
