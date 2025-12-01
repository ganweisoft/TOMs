// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace IoTCenterCore.Hei.Captcha;

public static class VerificationCodeUtil
{
    private static readonly char[] CodeList = { '1', '2', 'L', 'M', 'N', 'y', 'A', 'B', 'C', '7', '8', 'W', 'X', '9', 'a', 'b', 'd', 'e', 'f', 'h', 'k', 'm', 'n', 'r', 'x', 'D', 'E', '3', '4', '5', '6', 'F', 'G', 'H', 'J', 'K', 'P', 'R', 'S', 'T', 'Y' };

    private static readonly ConcurrentDictionary<string, string> VerificationList = new ConcurrentDictionary<string, string>();

    public static (string, string) GetImageCode(string codeType)
    {
        int[] num = new int[4] { RandomNumberGenerator.GetInt32(0, 41), RandomNumberGenerator.GetInt32(0, 41), RandomNumberGenerator.GetInt32(0, 41), RandomNumberGenerator.GetInt32(0, 41) };
        StringBuilder code = new StringBuilder();
        code.Append(CodeList[num[0]]);
        code.Append(CodeList[num[1]]);
        code.Append(CodeList[num[2]]);
        code.Append(CodeList[num[3]]);

        var result = GetImageBase64(code.ToString(), codeType);
        var guid = Guid.NewGuid().ToString();

        VerificationList.TryAdd(guid, code.ToString());

        return (guid, result);
    }

    private static string GetImageBase64(string code, string codeType)
    {
        if (codeType != "0")
        {
            return code;
        }
        var pngBytes = new SkiaSharpVerificationCodeCreater(120, 50).CreateVerificationImage(code);
        var result = "data:image/png;base64," + Convert.ToBase64String(pngBytes);

        return result;
    }

    public static (string, string) GetValue(string strKey)
    {
        VerificationList.TryGetValue(strKey, out var value);

        return (strKey, value);
    }

    public static bool CodeIsTrue(string strGuid, string code)
    {
        if (!VerificationList.TryGetValue(strGuid, out var c))
        {
            return false;
        }

        if (code.ToUpper(CultureInfo.InvariantCulture) == c.ToUpper(CultureInfo.InvariantCulture))
        {
            VerificationList.TryRemove(strGuid, out var _);

            return true;
        }

        return false;
    }
}
