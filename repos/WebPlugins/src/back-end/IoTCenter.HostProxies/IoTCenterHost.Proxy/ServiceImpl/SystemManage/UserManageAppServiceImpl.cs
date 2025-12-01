// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Utilities;
using IoTCenterHost.Core.Abstraction.Interfaces.AppServices;
using IoTCenterHost.Core.Abstraction.IotModels;

using IoTCenterHost.Proto;
using static IoTCenterHost.Proto.SystemManage;

namespace IoTCenterHost.Proxy.ServiceImpl
{
    public class UserManageAppServiceImpl : IUserAppService
    {
        private readonly SystemManageClient _systemManageClient;

        public UserManageAppServiceImpl(SystemManageClient systemManageClient)
        {
            _systemManageClient = systemManageClient;
        }

        public bool AddUserEntity(GWUserItem userItem)
        {
            StringResult stringResult = new StringResult()
            {
                Result = userItem.ToJson()
            };
            return _systemManageClient.AddUserEntity(stringResult).Result;
        }



        public bool DeleteUserEntity(int Id)
        {
            IntegerDefine integerDefine = new IntegerDefine()
            {
                Result = Id
            };
            return _systemManageClient.DeleteUserEntity(integerDefine).Result;
        }



        public PaginationData GetUserEntities(Pagination pagination)
        {
            StringResult stringResult = new StringResult()
            {
                Result = pagination.ToJson()
            };
            return _systemManageClient.GetUserEntities(stringResult).Result.FromJson<PaginationData>();
        }

        public GWUserItem GetWebUser(string userName)
        {
            return _systemManageClient.GetWebUser(new StringResult { Result = userName }).Result.FromJson<GWUserItem>();
        }

        public bool ModifyUserEntity(GWUserItem userEntity)
        {
            StringResult stringResult = new StringResult()
            {
                Result = userEntity.ToJson()
            };
            return _systemManageClient.ModifyUserEntity(stringResult).Result;
        }
        public bool RevisePassword(GWUserItem userItem)
        {
            StringResult stringResult = new StringResult()
            {
                Result = userItem.ToJson()
            };
            return _systemManageClient.RevisePassword(stringResult).Result;
        }
    }
}
