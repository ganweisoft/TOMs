//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenterHost.Core.Extension;
using System.ComponentModel;

namespace IoTCenterHost.Core.Abstraction
{
    public class ServiceStatusModel
    {
        public ServiceStatus ServiceStatus { get; set; }

        public bool LicenseStatus { get; set; }

        public string License { get; set; }

        public bool IsDebug { get; set; }

        public string ServiceStatusDesc { get; set; }

        public static ServiceStatusModel Empty
        {
            get
            {
                return new ServiceStatusModel
                {
                    ServiceStatus = ServiceStatus.Idle,
                    ServiceStatusDesc = ServiceStatus.Idle.ToDescription(),
                    LicenseStatus = true,
                };
            }
        }
        public static ServiceStatusModel ServiceRunning
        {
            get
            {
                return new ServiceStatusModel
                {
                    ServiceStatus = ServiceStatus.Running,
                    ServiceStatusDesc = ServiceStatus.Running.ToDescription(),
                    LicenseStatus = true
                };
            }
        }
    }

    public enum ServiceStatus
    {
        [Description("未连接")]
        Idle,
        [Description("停止")]
        Stop,
        [Description("运行中")]
        Running,
    }
}
