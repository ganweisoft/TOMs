// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Utilities;
using IoTCenterHost.Core.Abstraction.Interfaces.AppServices;
using IoTCenterHost.Core.Abstraction.IotModels;

using IoTCenterHost.Proto;
using static IoTCenterHost.Proto.SystemManage;

namespace IoTCenterHost.Proxy.ServiceImpl
{
    public class RoleManageAppServiceImpl : IRoleAppService
    {
        private readonly SystemManageClient _systemManageClient;


        public RoleManageAppServiceImpl(SystemManageClient systemManageClient)
        {
            _systemManageClient = systemManageClient;

        }
        public bool AddRole(GWRoleItem roleItem)
        {
            StringResult stringResult = new StringResult()
            {
                Result = roleItem.ToJson()
            };
            return _systemManageClient.AddRole(stringResult).Result;
        }


        public bool DeleteRole(string name)
        {
            StringResult stringResult = new StringResult()
            {
                Result = name
            };
            return _systemManageClient.DeleteRole(stringResult).Result;
        }



        public PaginationData GetRoleEntities(Pagination pagination)
        {
            return _systemManageClient.GetRoleEntities(new StringResult { Result = pagination.ToJson() }).Result.FromJson<PaginationData>();
        }



        public bool ModifyRole(GWRoleItem roleEntity)
        {
            StringResult stringResult = new StringResult()
            {
                Result = roleEntity.ToJson()
            };
            return _systemManageClient.ModifyRole(stringResult).Result;
        }
    }
}
