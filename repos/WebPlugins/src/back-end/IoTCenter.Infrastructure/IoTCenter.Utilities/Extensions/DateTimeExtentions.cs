// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace IoTCenter.Utilities.Extensions
{
    public static class DateTimeExtentions
    {
        public static ICollection<NaturalMonthRangeModel> GetDatetimeNaturalMonthRanges(this DateTime beginTime, DateTime endTime)
        {
            if (beginTime > endTime)
            {
                return default;
            }
            var endMonthTime = new DateTime(endTime.Year, endTime.Month, 1);

            var dateTimeList = new List<NaturalMonthRangeModel>()
            {
                new NaturalMonthRangeModel
                {
                    BeginTime =endMonthTime
                }
            };

            var index = 1;
            var beginMonthTime = new DateTime(beginTime.Year, beginTime.Month, 1);
            var endDatetime = endMonthTime;
            while (beginMonthTime < endDatetime)
            {
                endDatetime = endMonthTime.AddMonths(-index);
                dateTimeList.Add(new NaturalMonthRangeModel { BeginTime = endDatetime });
                index++;
            }

            return dateTimeList;
        }

        public static long ToUnixTimestamp(this DateTime date, TimestampFormat timestampType = TimestampFormat.Seconds)
        {
            long divisor = timestampType == TimestampFormat.Seconds ? 10000000 : 10000;
            return (date.ToUniversalTime().Ticks - 621355968000000000) / divisor;
        }

        public static DateTime FromUnixTimeStampMillisecond(this long unixTimeStamp)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Local);
            dateTime = dateTime.AddMilliseconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }
        public static long GetUnixTimeStampSeconds(this DateTime now)
        {
            return (long)now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        }

        public static long GetUnixTimeStampMilliseconds(this DateTime now)
        {
            return (long)now.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
        }
    }

    public class NaturalMonthRangeModel
    {
        public DateTime BeginTime { get; set; }
        public DateTime EndTime => BeginTime.AddMonths(1);
    }

    public enum TimestampFormat : byte
    {
        [Description("秒")]
        Seconds = 0,
        [Description("毫秒")]
        Milliseconds = 1
    }
}
