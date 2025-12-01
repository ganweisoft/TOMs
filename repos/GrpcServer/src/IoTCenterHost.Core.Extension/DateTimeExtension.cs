//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System;

namespace IoTCenterHost.Core.Extension
{
    public static class DateTimeExtension
    {
        public static long GetUnixTimeStampSeconds(this DateTime now)
        {
            return (long)now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        }

        public static long GetUnixTimeStampMilliseconds(this DateTime now)
        {
            return (long)now.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
        }
    }
}
