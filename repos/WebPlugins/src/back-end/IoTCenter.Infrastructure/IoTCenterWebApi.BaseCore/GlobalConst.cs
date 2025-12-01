// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace System;

public static class GlobalConst
{
    public static Dictionary<string, List<UserSession>> UserSessions { get; set; } =
        new Dictionary<string, List<UserSession>>();

    public static ConcurrentDictionary<string, UserBaseSession> DownloadFileSession { get; set; } =
        new ConcurrentDictionary<string, UserBaseSession>();

    public static ConcurrentDictionary<string, UserBaseSession> EGroupNotifySession { get; set; } =
        new ConcurrentDictionary<string, UserBaseSession>();
}