// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenterHost.Core.Abstraction;

namespace IoTCenterWebApi.Service
{
    public interface IServiceManageService
    {
        bool ShutDownService(out string msg);
        bool RebootService(out string msg);
        string GetServiceLog(out string msg);
        ServiceStatusModel GetServiceStatus();

        bool VerifyAccessToken();

        bool RebootApplication(out string msg);
        void ShutDownApplication();
        string GetLicenseInfo(out string msg);
        string IsInitMaintainPwd();
    }
}
