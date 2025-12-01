// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System.Collections.Generic;

namespace IoTCenter.Utilities
{
    public class WebApiConfigOptions
    {
        public string IpAddress { get; set; }
        public string HttpPort { get; set; }

        public string HttpsPort { get; set; }

        public bool SSLAutoGenerate { get; set; }

        public bool CipherAdapterEnable { get; set; }

        public string SSLName { get; set; }

        public string SSLPassword { get; set; }

        public bool RSAAutoGenerate { get; set; }

        public string RSAPadding { get; set; }

        public int RequestBodySize { get; set; }

        public int FormFileSize { get; set; }

        public ShowSystemInfo ShowSystemInfo { get; set; }

        public int ExpiredTime { get; set; }

        public int? GatewayKeepAlive { get; set; }

        public int? MaxGatewayStoreExpire { get; set; }

        public bool EnableGatewayCache { get; set; }

        public string IsInitMaintainPwd { get; set; }

        public bool IsManyLoginEnabled { get; set; }

        public string OnlyVerifyClaimsSystem { get; set; }
    }

    public class ShowSystemInfo
    {
        public bool ShowSystemRunEvnInfo { get; set; }
        public bool ShowApplicationRunInfo { get; set; }
        public bool ShowPlatformInfo { get; set; }
    }

    public class RedisConfigOption
    {
        public string ConnectString { get; set; }
        public string Password { get; set; }
    }

    public class HostServerOption
    {
        public string MqPort { get; set; }
        public string ZmqSubscribeKey { get; set; }
        public string PluginsPath { get; set; }
        public string SafetyLevel { get; set; }
        public string SingleAppStart { get; set; }
        public string AppStoreHost { get; set; }
        public bool ContainNoAuditPlugin { get; set; }
        public string StorageFile { get; set; }
        public List<int> DebugEquipNos { get; set; }
    }
}
