// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System;

namespace IoTCenter.Utilities
{
    public static class ByteSizeUtil
    {
        public enum SizeUnits
        {
            Byte, KB, MB, GB, TB, PB, EB, ZB, YB
        }

        public static string ToSize(this long value, SizeUnits unit)
        {
            return (value / Math.Pow(1024, (long)unit)).ToString("0.00");
        }
        public static double ToSizeDouble(this long value, SizeUnits unit)
        {
            return (value / Math.Pow(1024, (long) unit));
        }

        public static string ToSize(this long value, SizeUnits inPutUnit, SizeUnits outPutUnits)
        {
            return (value / Math.Pow(1024, outPutUnits - inPutUnit)).ToString("0.00");
        }
    }
}
