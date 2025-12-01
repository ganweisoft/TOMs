// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenterCore.DeviceDetection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace IoTCenterWebApi.BaseCore;

public static class PermissionCache
{
    private static readonly ConcurrentDictionary<string, Info> LoginInfo = new ConcurrentDictionary<string, Info>();


    private static string userAgent;

    private static bool internalAccount;

    public static void SetUserAgent(bool internalAccount = false,string userAgent=null)
    {
        PermissionCache.internalAccount = internalAccount;
        PermissionCache.userAgent = userAgent;
    }

    public static string UserLogin(string userName)
    {
        if (string.IsNullOrEmpty(userName))
        {
            return null;
        }

        var guid = Guid.NewGuid().ToString();

        if (LoginInfo.TryGetValue(userName, out var firstLoginInfo))
        {
            var loginDeviceCount = firstLoginInfo.LoginDeviceInfos.Count();

            var firstLoginDevice = firstLoginInfo.LoginDeviceInfos.FirstOrDefault();

            var loginDevices = firstLoginInfo.LoginDeviceInfos;

            if (internalAccount || firstLoginInfo.InternalAccount)
            {
                return firstLoginDevice.Guid;
            }

            if (string.IsNullOrEmpty(userAgent))
            {
                firstLoginInfo.LoginDeviceInfos.ForEach(l =>
                {
                    l.Guid = guid;
                });
                return guid;
            }

            var currentDeviceInfo = Detector.TryParseUserAgent(userAgent);

            if (loginDeviceCount == 1)
            {
                if (firstLoginDevice.DeviceInfo == null || currentDeviceInfo == null)
                {
                    firstLoginInfo.LoginDeviceInfos.Add(new LoginDeviceInfo()
                    {
                        Guid = guid,
                        DeviceInfo = currentDeviceInfo
                    });
                    return guid;
                }

                if (Detector.IsPC(firstLoginDevice.DeviceInfo.PlatformType.Value) && Detector.IsMobile(currentDeviceInfo.PlatformType.Value))
                {
                    firstLoginInfo.LoginDeviceInfos.Add(new LoginDeviceInfo()
                    {
                        Guid = guid,
                        DeviceInfo = currentDeviceInfo
                    });
                    return guid;
                }
                else if (Detector.IsMobile(firstLoginDevice.DeviceInfo.PlatformType.Value) && Detector.IsPC(currentDeviceInfo.PlatformType.Value))
                {
                    firstLoginInfo.LoginDeviceInfos.Add(new LoginDeviceInfo()
                    {
                        Guid = guid,
                        DeviceInfo = currentDeviceInfo
                    });
                    return guid;
                }
                else if (Detector.IsPC(firstLoginDevice.DeviceInfo.PlatformType.Value) && Detector.IsPC(currentDeviceInfo.PlatformType.Value)
                    && (firstLoginDevice.DeviceInfo.PlatformType.Value == currentDeviceInfo.PlatformType.Value)
                    && firstLoginDevice.DeviceInfo.BrowserType.Value == currentDeviceInfo.BrowserType.Value)
                {
                    return firstLoginDevice.Guid;
                }
                else
                {
                    firstLoginInfo.LoginDeviceInfos[0].Guid = guid;
                    firstLoginInfo.LoginDeviceInfos[0].DeviceInfo = currentDeviceInfo;
                    return guid;
                }
            }
            else if (loginDeviceCount == 2)
            {
                if (currentDeviceInfo == null)
                {
                    return firstLoginDevice.Guid;
                }

                if (loginDevices.Count(d => d.DeviceInfo == null) == 2)
                {
                    firstLoginInfo.LoginDeviceInfos[0].DeviceInfo = currentDeviceInfo;
                    return firstLoginDevice.Guid;
                }
                else if (loginDevices.Count(d => d.DeviceInfo == null) == 1)
                {
                    firstLoginInfo.LoginDeviceInfos[0].DeviceInfo = currentDeviceInfo;
                    return firstLoginDevice.Guid;
                }
                else if (loginDevices.Count(d => d.DeviceInfo == null) == 0)
                {
                    var pc = Detector.IsPC(currentDeviceInfo.PlatformType.Value);

                    var mobile = Detector.IsMobile(currentDeviceInfo.PlatformType.Value);

                    firstLoginInfo.LoginDeviceInfos.ForEach(f =>
                    {
                        if (pc && Detector.IsPC(f.DeviceInfo.PlatformType.Value))
                        {
                            f.DeviceInfo = currentDeviceInfo;
                            f.Guid = guid;
                        }
                        else if (mobile && Detector.IsMobile(f.DeviceInfo.PlatformType.Value))
                        {
                            f.DeviceInfo = currentDeviceInfo;
                            f.Guid = guid;
                        }
                    });

                    return guid;
                }
                else
                {
                    return guid;
                }
            }
        }

        var item = new Info
        {
            UserName = userName,
            InternalAccount = internalAccount
        };

        var deviceInfo = Detector.TryParseUserAgent(userAgent);

        item.LoginDeviceInfos.Add(new LoginDeviceInfo()
        {
            Guid = guid,
            DeviceInfo = deviceInfo
        });

        LoginInfo.TryAdd(userName, item);

        userAgent = null; //重置useragent，避免污染其他会话
        return guid;
    }


    public static bool CheckUserSession(string userName)
    {
        return LoginInfo.TryGetValue(userName, out var _);
    }

    public static bool CheckUsrHasLoginedOrHasUpdated(string userName, string gid)
    {
        if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(gid))
        {
            return true;
        }

        if (LoginInfo.TryGetValue(userName, out var tmp))
        {
            return gid != tmp.LoginDeviceInfos.FirstOrDefault(d => d.Guid == gid)?.Guid;
        }
        return true;

    }


    public static bool RemoveUserLogin(string userName, string gid = null)
    {
        if (LoginInfo.TryGetValue(userName, out var info) && !string.IsNullOrEmpty(gid))
        {
            info.LoginDeviceInfos.ForEach(d =>
            {
                if (d.Guid == gid)
                {
                    d.Guid = Guid.NewGuid().ToString();
                }
            });
            return true;
        }
        return false;
    }

    private class Info
    {
        public string UserName { get; set; }
        public bool InternalAccount { get; set; }
        public List<LoginDeviceInfo> LoginDeviceInfos { get; set; } = new List<LoginDeviceInfo>();
    }

    internal sealed class LoginDeviceInfo
    {
        public string Guid { get; set; }
        public DeviceInfo DeviceInfo { get; set; }
    }
}