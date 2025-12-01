//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenterHost.Core.Extension;
using System.Reflection;

namespace IoTCenterHost.AppServices.Domain.WebOptions;

public class RedisOption
{
    public string ConnectString { get; set; }
    public string Password { get; set; }

    public static RedisOption Create()
    {
        var directoryInfo = new DirectoryInfo(Assembly.GetEntryAssembly().Location).Parent.Parent.FullName;

        var configurationXml = Path.Combine(directoryInfo, "data", "AlarmCenter", "AlarmCenterProperties.xml");

        var redisConnectStr = configurationXml.ReadXml("HostServer", "Redis.ConnectString");

        var redisPwd = configurationXml.ReadXml("HostServer", "Redis.Password");

        return new RedisOption
        {
            ConnectString = redisConnectStr,
            Password = redisPwd
        };
    }

    public void Save()
    {
        GWDataCenter.DataCenter.SetPropertyToPropertyService("HostServer", "Redis.ConnectString", this.ConnectString);

        GWDataCenter.DataCenter.SetPropertyToPropertyService("HostServer", "Redis.Password", this.Password);
    }
}

public class RedisOptionComparer : IEqualityComparer<RedisOption>
{
    public bool Equals(RedisOption x, RedisOption y)
    {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        return x.ConnectString == y.ConnectString && x.Password == y.Password;
    }

    public int GetHashCode(RedisOption obj)
    {
        return HashCode.Combine(obj.ConnectString, obj.Password);
    }
}