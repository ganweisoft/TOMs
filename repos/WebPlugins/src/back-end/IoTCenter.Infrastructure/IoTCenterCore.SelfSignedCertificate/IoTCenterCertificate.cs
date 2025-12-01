// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Utilities;
using IoTCenterWebApi.BaseCore;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace IoTCenterCore.SelfSignedCertificate;

public static class IoTCenterCertificate
{
    private const string CRFSH = "crf.sh";
    private const string CRFP = "crf.p";
    private const string SSL = "SSL";
    public static string Init(Serilogger serilogger, bool scu)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            return "";
        }

        if (!scu)
        {
            return "";
        }

        var filePath = Path.Combine(AppContext.BaseDirectory, CRFSH);

        var sslCertificatePath = Path.Combine(AppContext.BaseDirectory, SSL);

        if (Directory.Exists(sslCertificatePath))
        {
            Directory.Delete(sslCertificatePath, true);
        }

        if (!File.Exists(filePath))
        {
            return "";
        }

        var psi = new ProcessStartInfo(filePath)
        {
            RedirectStandardOutput = true
        };

        try
        {
            var process = Process.Start(psi);

            process.WaitForExit();
        }
        catch (Exception ex)
        {
            serilogger.Logger.Error($"证书生成失败：{ex.Message}");
            return String.Empty;
        }

        var crfp = Path.Combine(AppContext.BaseDirectory, SSL, CRFP);
        if (!File.Exists(crfp))
        {
            serilogger.Logger.Error($"证书生成失败：{CRFP}未正确生成");
            return "";
        }

        var password = File.ReadAllText(crfp);

        if (string.IsNullOrEmpty(password))
        {
            serilogger.Logger.Error($"证书生成失败：{CRFP}生成为空");

            return "";
        }

        AppConfigurationExtensions.SetAppSettingValue("WebApi", "SSLPassword", password);

        File.Delete(crfp);

        return password;
    }
}
