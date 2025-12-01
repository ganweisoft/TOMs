//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenterHost.Core.Extension;
using System.Reflection;

namespace IoTCenterHost.AppServices.Domain.WebOptions;

public class WebApiOption
{
    public string IpAddress { get; set; }

    public string HttpPort { get; set; }

    public string HttpsPort { get; set; }

    public bool SSLAutoGenerate { get; set; }

    public bool CipherAdapterEnable { get; set; }

    public string SSLName { get; set; }

    public string SSLPassword { get; set; }

    public string ApplicationPartName { get; set; }

    public bool IsInitMaintainPwd { get; set; }

    public int TryConnectTime { get; set; }

    public int ExpiredTime { get; set; }

    public int GatewayKeepAlive { get; set; }

    public bool EnableGatewayCache { get; set; }

    public int MaxGatewayStoreExpire { get; set; }

    public bool RSAAutoGenerate { get; set; }

    public string RSAPadding { get; set; }

    public int RequestBodySize { get; set; }

    public int FormFileSize { get; set; }

    public ShowSystemInfo ShowSystemInfo { get; set; }

    public bool IsManyLoginEnabled { get; set; }

    public string OnlyVerifyClaimsSystem { get; set; }

    public static WebApiOption Create()
    {
        var directoryInfo = new DirectoryInfo(Assembly.GetEntryAssembly().Location).Parent.Parent.FullName;

        var configurationXml = Path.Combine(directoryInfo, "data", "AlarmCenter", "AlarmCenterProperties.xml");

        var ipAddressStr = configurationXml.ReadXml("WebApi", "IpAddress");

        var httpPortStr = configurationXml.ReadXml("WebApi", "HttpPort");

        var httpsPortStr = configurationXml.ReadXml("WebApi", "HttpsPort");

        var sslAutoGenerateStr = configurationXml.ReadXml("WebApi", "SSLAutoGenerate");
        var sslAutoGenerateParse = bool.TryParse(sslAutoGenerateStr, out var sslAutoGenerate);
        if (!sslAutoGenerateParse)
        {
        }

        var cipherAdapterPortStr = configurationXml.ReadXml("WebApi", "CipherAdapterEnable");
        var cipherAdapterPortParse = bool.TryParse(cipherAdapterPortStr, out var cipherAdapterPort);
        if (!cipherAdapterPortParse)
        {
        }

        var sslNameStr = configurationXml.ReadXml("WebApi", "SSLName");

        var sslPasswordStr = configurationXml.ReadXml("WebApi", "SSLPassword");

        var applicationPartNameStr = configurationXml.ReadXml("WebApi", "ApplicationPartName");

        var isInitMaintainPwdStr = configurationXml.ReadXml("WebApi", "IsInitMaintainPwd");
        var isInitMaintainPwdParse = bool.TryParse(isInitMaintainPwdStr, out var isInitMaintainPwd);
        if (!isInitMaintainPwdParse)
        {
        }

        var expiredTimeStr = configurationXml.ReadXml("WebApi", "ExpiredTime");
        var expiredTimeParse = int.TryParse(expiredTimeStr, out var expiredTime);
        if (!expiredTimeParse)
        {
        }

        var gatewayKeepAliveStr = configurationXml.ReadXml("WebApi", "GatewayKeepAlive");
        var gatewayKeepAliveParse = int.TryParse(gatewayKeepAliveStr, out var gatewayKeepAlive);
        if (!gatewayKeepAliveParse)
        {
        }

        var enableGatewayCacheStr = configurationXml.ReadXml("WebApi", "EnableGatewayCache");
        var enableGatewayCacheParse = bool.TryParse(enableGatewayCacheStr, out var enableGatewayCache);
        if (!enableGatewayCacheParse)
        {
        }

        var maxGatewayStoreExpireStr = configurationXml.ReadXml("WebApi", "MaxGatewayStoreExpire");
        var maxGatewayStoreExpireParse = int.TryParse(maxGatewayStoreExpireStr, out var maxGatewayStoreExpire);
        if (!maxGatewayStoreExpireParse)
        {
        }

        var rsaAutoGenerateStr = configurationXml.ReadXml("WebApi", "RSAAutoGenerate");
        var rsaAutoGenerateParse = bool.TryParse(rsaAutoGenerateStr, out var rsaAutoGenerate);
        if (!rsaAutoGenerateParse)
        {
        }

        var rsaPaddingStr = configurationXml.ReadXml("WebApi", "RSAPadding");

        var requestBodySizeStr = configurationXml.ReadXml("WebApi", "RequestBodySize");
        var requestBodySizeParse = int.TryParse(requestBodySizeStr, out var requestBodySize);
        if (!requestBodySizeParse)
        {
        }

        var formFileSizeStr = configurationXml.ReadXml("WebApi", "FormFileSize");
        var formFileSizeParse = int.TryParse(formFileSizeStr, out var formFileSize);
        if (!formFileSizeParse)
        {
        }

        var showPlatformInfoStr = configurationXml.ReadXml("WebApi", "ShowSystemInfo.ShowPlatformInfo");
        var showPlatformInfoParse = bool.TryParse(showPlatformInfoStr, out var showPlatformInfo);
        if (!showPlatformInfoParse)
        {
        }

        var isManyLoginEnabledStr = configurationXml.ReadXml("WebApi", "IsManyLoginEnabled");
        var isManyLoginEnabledParse = bool.TryParse(isManyLoginEnabledStr, out var isManyLoginEnabled);
        if (!isManyLoginEnabledParse)
        {
        }

        var onlyVerifyClaimsSystemStr = configurationXml.ReadXml("WebApi", "OnlyVerifyClaimsSystem");

        return new WebApiOption
        {
            IpAddress = ipAddressStr,
            HttpPort = httpPortStr,
            HttpsPort = httpsPortStr,
            SSLAutoGenerate = sslAutoGenerateParse && sslAutoGenerate,
            CipherAdapterEnable = cipherAdapterPortParse && cipherAdapterPort,
            SSLName = sslNameStr,
            SSLPassword = sslPasswordStr,
            ApplicationPartName = applicationPartNameStr,
            IsInitMaintainPwd = isInitMaintainPwdParse && isInitMaintainPwd,
            ExpiredTime = expiredTimeParse ? expiredTime : 525600,
            GatewayKeepAlive = gatewayKeepAliveParse ? gatewayKeepAlive : 15,
            EnableGatewayCache = !enableGatewayCacheParse || enableGatewayCache,
            MaxGatewayStoreExpire = maxGatewayStoreExpireParse ? maxGatewayStoreExpire : 10,
            RSAAutoGenerate = rsaAutoGenerateParse && rsaAutoGenerate,
            RSAPadding = rsaPaddingStr,
            RequestBodySize = requestBodySizeParse ? requestBodySize : 1073741824,
            FormFileSize = formFileSizeParse ? formFileSize : 210763776,
            ShowSystemInfo = new ShowSystemInfo { ShowPlatformInfo = !showPlatformInfoParse || showPlatformInfo },
            IsManyLoginEnabled = isManyLoginEnabledParse && isManyLoginEnabled,
            OnlyVerifyClaimsSystem = onlyVerifyClaimsSystemStr
        };
    }

    public void Save()
    {
        GWDataCenter.DataCenter.SetPropertyToPropertyService("WebApi", "IpAddress", this.IpAddress);

        GWDataCenter.DataCenter.SetPropertyToPropertyService("WebApi", "HttpPort", this.HttpPort);

        GWDataCenter.DataCenter.SetPropertyToPropertyService("WebApi", "HttpsPort", this.HttpsPort);

        GWDataCenter.DataCenter.SetPropertyToPropertyService("WebApi", "SSLAutoGenerate", this.SSLAutoGenerate.ToString());

        GWDataCenter.DataCenter.SetPropertyToPropertyService("WebApi", "CipherAdapterEnable", this.CipherAdapterEnable.ToString());

        GWDataCenter.DataCenter.SetPropertyToPropertyService("WebApi", "SSLName", this.SSLName);

        if (!string.IsNullOrEmpty(this.SSLPassword))
        {
            GWDataCenter.DataCenter.SetPropertyToPropertyService("WebApi", "SSLPassword", this.SSLPassword);
        }

        GWDataCenter.DataCenter.SetPropertyToPropertyService("WebApi", "ApplicationPartName", this.ApplicationPartName);

        GWDataCenter.DataCenter.SetPropertyToPropertyService("WebApi", "IsInitMaintainPwd", this.IsInitMaintainPwd.ToString());

        GWDataCenter.DataCenter.SetPropertyToPropertyService("WebApi", "GatewayKeepAlive", this.GatewayKeepAlive.ToString());

        GWDataCenter.DataCenter.SetPropertyToPropertyService("WebApi", "ExpiredTime", this.ExpiredTime.ToString());

        GWDataCenter.DataCenter.SetPropertyToPropertyService("WebApi", "EnableGatewayCache", this.EnableGatewayCache.ToString());

        GWDataCenter.DataCenter.SetPropertyToPropertyService("WebApi", "MaxGatewayStoreExpire", this.MaxGatewayStoreExpire.ToString());

        GWDataCenter.DataCenter.SetPropertyToPropertyService("WebApi", "RSAAutoGenerate", this.RSAAutoGenerate.ToString());

        GWDataCenter.DataCenter.SetPropertyToPropertyService("WebApi", "RSAPadding", this.RSAPadding);

        GWDataCenter.DataCenter.SetPropertyToPropertyService("WebApi", "FormFileSize", this.FormFileSize.ToString());

        GWDataCenter.DataCenter.SetPropertyToPropertyService("WebApi", "ShowSystemInfo.ShowPlatformInfo", this.ShowSystemInfo?.ShowPlatformInfo.ToString());

        GWDataCenter.DataCenter.SetPropertyToPropertyService("WebApi", "IsManyLoginEnabled", this.IsManyLoginEnabled.ToString());
    }
}

public class ShowSystemInfo
{
    public bool ShowPlatformInfo { get; set; }
}

public class WebApiOptionComparer : IEqualityComparer<WebApiOption>
{
    public bool Equals(WebApiOption x, WebApiOption y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        return x.IpAddress == y.IpAddress && x.HttpPort == y.HttpPort && x.HttpsPort == y.HttpsPort &&
               x.SSLAutoGenerate == y.SSLAutoGenerate && x.CipherAdapterEnable == y.CipherAdapterEnable &&
               x.SSLName == y.SSLName && x.SSLPassword == y.SSLPassword &&
               x.ApplicationPartName == y.ApplicationPartName && x.IsInitMaintainPwd == y.IsInitMaintainPwd &&
               x.TryConnectTime == y.TryConnectTime && x.ExpiredTime == y.ExpiredTime &&
               x.GatewayKeepAlive == y.GatewayKeepAlive && x.EnableGatewayCache == y.EnableGatewayCache &&
               x.MaxGatewayStoreExpire == y.MaxGatewayStoreExpire && x.RSAAutoGenerate == y.RSAAutoGenerate &&
               x.RSAPadding == y.RSAPadding && x.FormFileSize == y.FormFileSize &&
               x.ShowSystemInfo.Equals(y.ShowSystemInfo) && x.IsManyLoginEnabled == y.IsManyLoginEnabled;
    }

    public int GetHashCode(WebApiOption obj)
    {
        var hashCode = new HashCode();
        hashCode.Add(obj.IpAddress);
        hashCode.Add(obj.HttpPort);
        hashCode.Add(obj.HttpsPort);
        hashCode.Add(obj.SSLAutoGenerate);
        hashCode.Add(obj.CipherAdapterEnable);
        hashCode.Add(obj.SSLName);
        hashCode.Add(obj.SSLPassword);
        hashCode.Add(obj.ApplicationPartName);
        hashCode.Add(obj.IsInitMaintainPwd);
        hashCode.Add(obj.TryConnectTime);
        hashCode.Add(obj.ExpiredTime);
        hashCode.Add(obj.GatewayKeepAlive);
        hashCode.Add(obj.EnableGatewayCache);
        hashCode.Add(obj.MaxGatewayStoreExpire);
        hashCode.Add(obj.RSAAutoGenerate);
        hashCode.Add(obj.RSAPadding);
        hashCode.Add(obj.FormFileSize);
        hashCode.Add(obj.ShowSystemInfo);
        hashCode.Add(obj.IsManyLoginEnabled);
        return hashCode.ToHashCode();
    }
}