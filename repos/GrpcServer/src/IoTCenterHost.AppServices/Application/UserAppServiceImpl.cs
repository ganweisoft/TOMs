//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using GWDataCenter;
using IoTCenterHost.AppServices.Domain.DO.User;
using IoTCenterHost.Core.Abstraction;
using IoTCenterHost.Core.Abstraction.IotModels;
using IoTCenterHost.Core.Extension;
using IoTCenterHost.Core.ServerInterfaces;
using System.Linq;
using System.Threading.Tasks;

namespace IoTCenterHost.AppServices.AppServices
{
    public class UserAppServiceImpl : IUserAppServerService
    {
        private readonly IUserRepository _userRepository;
        public UserAppServiceImpl(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public bool AddUserEntity(GWUserItem userItem)
        {
            if (_userRepository.Count(userItem.UserName) == 0)
            {
                var userEntity = EncryptUser(userItem);
                userEntity.Password = userItem.UserPWD;
                _userRepository.AddUserEntity(userEntity.Map<UserEntity>());

                return true;
            }
            else
                return false;
        }
        public bool DeleteUserEntity(int Id)
        {
            _userRepository.DeleteUserEntity(Id);
            return true;
        }
        public PaginationData GetUserEntities(Pagination pagination)
        {
            var data = _userRepository.GetUserEntities(pagination, out int total);
            var paginationData = new PaginationData
            {
                Data = data.ToJson(),
                Total = total,
            };
            return paginationData;
        }
        public bool ModifyUserEntity(GWUserItem userItem)
        {
            GWUserVO userEntity = EncryptUser(userItem);
            userEntity.Id = userItem.ID;
            _userRepository.ModifyUserEntity(userEntity.Map<UserEntity>());
            return true;

        }
        private static GWUserVO EncryptUser(GWUserItem userItem)
        {
            var userEntity = new GWUserVO()
            {
                Name = userItem.UserName,
                AutoInspectionPages = userItem.AutoInspectionPages_List != null ? string.Join('#', userItem.AutoInspectionPages_List) : "",
                ControlLevel = userItem.ControlLevel.ToString(),
                HomePages = string.Join('#', userItem.HomePage_List),
                Roles = string.Join('#', userItem.Role_List),
                Remark = userItem.Remark,
            };
            userEntity.Name = userEntity.Name;
            userEntity.Roles = userEntity.Roles;
            userEntity.HomePages = userEntity.HomePages;
            userEntity.AutoInspectionPages = userEntity.AutoInspectionPages;
            userEntity.Remark = userEntity.Remark;
            userEntity.ControlLevel = userEntity.ControlLevel;
            return userEntity;
        }
        private static GWUserVO EncryptUser(UserItem userItem)
        {
            var userEntity = new GWUserVO()
            {
                Name = userItem.UserName,
                AutoInspectionPages = string.Join('#', userItem.AutoInspectionPages_List),
                ControlLevel = userItem.ControlLevel.ToString(),
                HomePages = string.Join('#', userItem.HomePage_List),
                Roles = string.Join('#', userItem.Role_List.Select(u => u.name)),
                Remark = userItem.Remark,
            };
            userEntity.Name = userEntity.Name;
            userEntity.Roles = userEntity.Roles;
            userEntity.HomePages = userEntity.HomePages;
            userEntity.AutoInspectionPages = userEntity.AutoInspectionPages;
            userEntity.Remark = userEntity.Remark;
            userEntity.ControlLevel = userEntity.ControlLevel;
            return userEntity;
        }

        public bool RevisePassword(GWUserItem gwUserItem)
        {
            return _userRepository.RevisePassword(gwUserItem.ID, gwUserItem.UserPWD);
        }

        public async Task<GWUserItem> GetWebUser(string userName)
        {
            var userEntity = await _userRepository.GetUserEntity(userName);
            return await _userRepository.GetUserItem(userEntity);
        }

        public void UpdateUserTable()
        {

        }
    }
}
