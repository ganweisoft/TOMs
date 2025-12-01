// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Microsoft.Extensions.Configuration;

namespace IoTCenterWebApi.BaseCore;

public static class AllowOriginsExtension
{
    public static bool GetAllowOrigins(this IConfiguration configuration)
    {
        var allowOrigins = configuration.GetSection("AllowOrigins").Get<string[]>();
        if (allowOrigins != null && allowOrigins.Length == 1 && allowOrigins[0] == "*")
        {
            return true;
        }

        return false;
    }
}