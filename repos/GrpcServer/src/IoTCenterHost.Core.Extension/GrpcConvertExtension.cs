//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Google.Protobuf;
using Google.Protobuf.Collections;
using System;
using System.Collections.Generic;

namespace IoTCenterHost.Core.Extension
{
    public static class GrpcConvertExtension
    {
        public static byte[] ConvertByteStrToByte(this RepeatedField<ByteString> origin)
        {
            ByteString[] byteStrArr = new ByteString[origin.Count];
            origin.CopyTo(byteStrArr, 0);
            byte[] resultArr = new byte[byteStrArr.Length];
            byteStrArr.CopyTo(resultArr, resultArr.Length);
            return resultArr;
        }

        public static ByteString ConvertByteToByteString(this byte[] origin)
        {
            var byteString = ByteString.CopyFrom(origin);
            return byteString;
        }
        public static Int32[] ConvertRepeatedInt32(this RepeatedField<Int32> origin)
        {
            Int32[] byteStrArr = new Int32[origin.Count];
            origin.CopyTo(byteStrArr, 0);
            byte[] resultArr = new byte[byteStrArr.Length];
            byteStrArr.CopyTo(resultArr, resultArr.Length);
            return byteStrArr;
        }
        public static string[] ConvertRepeatStrToStringArr(this RepeatedField<string> origin)
        {
            string[] byteStrArr = new string[origin.Count];
            origin.CopyTo(byteStrArr, 0);
            return byteStrArr;
        }
        public static List<string> ConvertRepeatStrToString(this RepeatedField<string> origin)
        {
            string[] byteStrArr = new string[origin.Count];
            origin.CopyTo(byteStrArr, 0);
            return new List<string>(byteStrArr);
        }
        public static RepeatedField<Int32> ConvertToRepeatedInt32(this RepeatedField<Int32> origin)
        {
            RepeatedField<Int32> byteStrArr = new RepeatedField<Int32>
            {
                origin
            };
            return byteStrArr;
        }
        public static TDestination[] ConvertRepeatedOriginToDestination<TOrigin, TDestination>(this RepeatedField<TOrigin> origin) where TDestination : new() where TOrigin : new()
        {
            TOrigin[] byteStrArr = new TOrigin[origin.Count];
            origin.CopyTo(byteStrArr, 0);
            TDestination[] resultArr = new TDestination[byteStrArr.Length];
            byteStrArr.CopyTo(resultArr, resultArr.Length);
            return resultArr;
        }
        public static TDestination[] ConvertOriginToDestination<TOrigin, TDestination>(this TOrigin[] origin) where TDestination : new() where TOrigin : new()
        {
            TDestination[] byteStrArr = new TDestination[origin.Length];
            origin.CopyTo(byteStrArr, origin.Length);
            return byteStrArr;
        }
    }
}
