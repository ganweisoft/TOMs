// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Utilities;
using IoTCenterCore.RsaEncrypt;
using IoTCenterHost.Core.Abstraction;
using IoTCenterWebApi.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace IoTCenterWebApi.BaseCore;

public class ServiceManageServiceImpl : IServiceManageService
{
    private readonly IRSAAlgorithm _rsaAlgorithm;
    private readonly IoTCenter.Utilities.WebApiConfigOptions _configurationWebOptions;
    private readonly IHttpContextAccessor _contextAccessor;
    readonly IHostApplicationLifetime _hostApplicationLifetime;
    private readonly IMemoryCacheService _memoryCacheService;
    private readonly object _lockHelper = new object();
    private readonly string _rootPath;
    private readonly ILoggingService _loggingService;
    private const string VerifyRebootableCacheId = "VerifyRebootableCacheId";

    private const string VerifiyCookieId = "x-verify-connect";

    public ServiceManageServiceImpl(
       IRSAAlgorithm rsaAlgorithm,
       IOptions<IoTCenter.Utilities.WebApiConfigOptions> options,
       IHttpContextAccessor httpContextAccessor,
       IHostApplicationLifetime hostApplicationLifetime,
       IMemoryCacheService memoryCacheService,
       ILoggingService loggingService)
    {
        _rsaAlgorithm = rsaAlgorithm;
        _configurationWebOptions = options.Value;
        _contextAccessor = httpContextAccessor;
        _hostApplicationLifetime = hostApplicationLifetime;
        _memoryCacheService = memoryCacheService;
        _rootPath = System.IO.Directory.GetParent(System.IO.Directory.GetParent(System.IO.Directory.GetParent(AppContext.BaseDirectory).FullName).FullName).FullName;
        _loggingService = loggingService;
    }

    public string IsInitMaintainPwd()
    {
        var configuration = ServiceLocator.Current
            .GetRequiredService<IConfiguration>();

        var response = new InitStatusResponse();
        if (bool.TryParse(configuration.GetSection("WebApi:IsInitMaintainPwd").Value, out bool isInitMaintainPwd))
        {
            if (isInitMaintainPwd)
            {
                response.InitStatus = InitStatus.Normal;
                response.Result = "用户已经完成初始化";
            }
            else
            {
                response.InitStatus = InitStatus.Init;
            }
            return response.ToJson();
        }
        else
        {
            response.InitStatus = InitStatus.Null;
            response.Result = "未读取到配置";
            return response.ToJson();
        }
    }

    public bool RebootApplication(out string msg)
    {
        if (VerifyCacheTagIsVaild())
        {
            msg = string.Empty;

            var pathToExe = Process.GetCurrentProcess().MainModule.FileName;
            if (System.IO.Path.GetFileNameWithoutExtension(pathToExe).Equals("dotnet"))
            {
                Environment.Exit(1);
            }
            else
            {

                _hostApplicationLifetime.StopApplication();
            }
            return true;
        }
        else
        {
            msg = "用户信息验证失败，请重试";
            return false;
        }
    }


    public void ShutDownApplication()
    {
        _hostApplicationLifetime.StopApplication();
    }

    public string GetServiceLog(out string msg)
    {
        msg = string.Empty;
        if (VerifyAccessToken())
        {
            return ReadFromXlog();
        }
        else
        {
            msg = "您输入的凭据不正确";
        }
        return string.Empty;
    }

    public bool RebootService(out string msg)
    {
        msg = string.Empty;
        try
        {
            if (VerifyCacheTagIsVaild())
            {
                return true;
            }
            else
            {
                msg = "用户信息验证失败，请重试";
                return false;
            }
        }
        catch (Exception ex)
        {
            msg = ex.Message;
            return false;
        }
    }

    private bool VerifyCacheTagIsVaild()
    {
        string cookieToken = _contextAccessor.HttpContext.Request?.Cookies[VerifiyCookieId];
        if (string.IsNullOrEmpty(cookieToken))
        {
            return false;
        }
        else
        {
            string cacheToken = _memoryCacheService.Get(VerifyRebootableCacheId);
            return cacheToken.Equals(_rsaAlgorithm.Decrypt(cookieToken), StringComparison.InvariantCulture);
        }
    }
    public bool ShutDownService(out string msg)
    {
        msg = string.Empty;

        if (VerifyAccessToken())
        {
            return true;
        }
        else
        {
            msg = "您输入的凭据不正确";
            return false;
        }
    }

    private string ReadFromXlog()
    {
        string fileName = System.IO.Path.Combine(_rootPath, "log", "XLog.txt");
        string xlogInfo = string.Empty;
        lock (_lockHelper)
        {
            int lineCount = GetTextLineCount(fileName);
            int readCount = 0;
            using (StreamReader streamReader = new StreamReader(fileName))
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("服务日志只显示后100条");
                string lineStr = streamReader.ReadLine();
                while (readCount < lineCount)
                {
                    if (readCount > lineCount - 100)
                    {
                        stringBuilder.Append(lineStr);
                    }
                    lineStr = streamReader.ReadLine();
                    readCount += 1;
                }
                xlogInfo = stringBuilder.ToString();
            }
        }
        return xlogInfo;
    }

    private List<string> ReadFromXlogV2()
    {
        string path = System.IO.Path.Combine(_rootPath, "log");
        var files = System.IO.Directory.GetFiles(path, "Error*.txt");
        var fileName = files.FirstOrDefault();
        List<string> strList = new List<string>();
        lock (_lockHelper)
        {
            int lineCount = GetTextLineCount(fileName);
            int readCount = 0;

            var fileStream = new FileStream(fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read, FileShare.ReadWrite);
            using (StreamReader streamReader = new StreamReader(fileStream))
            {
                StringBuilder stringBuilder = new StringBuilder();
                stringBuilder.Append("服务日志只显示后100条");
                string lineStr = streamReader.ReadLine();
                while (readCount < lineCount)
                {
                    if (readCount > lineCount - 100)
                    {
                        strList.Add(lineStr);
                    }
                    lineStr = streamReader.ReadLine();
                    readCount += 1;
                }
            }

        }
        return strList;
    }

    private static int GetTextLineCount(string path)
    {
        int lineCount = 0;
        var directoryName = System.IO.Path.GetDirectoryName(path);
        var files = System.IO.Directory.GetFiles(directoryName, "Error*.txt");
        var licenseFile = files.FirstOrDefault();
        var fileStream = new FileStream(licenseFile, System.IO.FileMode.Open, System.IO.FileAccess.Read, FileShare.ReadWrite);
        using (StreamReader sr = new StreamReader(fileStream))
            while (sr.Peek() >= 0)
            {
                sr.ReadLine();
                lineCount++;
            }
        return lineCount;
    }
    public bool VerifyAccessToken()
    {
        string requestToken = _contextAccessor.HttpContext.Request.Headers["token"];
        if (string.IsNullOrEmpty(requestToken))
            return VerifyCacheTagIsVaild();

        string fileName = System.IO.Path.Combine(AppContext.BaseDirectory, "access.keys");
        string accessKey = ReadStream(fileName);
        return string.Equals(requestToken, accessKey, StringComparison.InvariantCultureIgnoreCase);
    }

    private static string ReadStream(string fileName)
    {
        if (System.IO.File.Exists(fileName))
            using (StreamReader streamReader = new StreamReader(fileName))
            {
                return streamReader.ReadToEnd();
            }
        return string.Empty;
    }

    public ServiceStatusModel GetServiceStatus()
    {
        return new ServiceStatusModel() { LicenseStatus = true, ServiceStatus = ServiceStatus.Running };
    }

    public string GetLicenseInfo(out string msg)
    {
        msg = string.Empty;
        string result = string.Empty;

        return string.Empty;
    }

    public string GetLicenseInfo_Old(out string msg)
    {
        msg = string.Empty;
        try
        {
            msg = "已生效";
            return msg;
        }
        catch (Exception ex)
        {
            msg = ex.Message;
            return string.Empty;
        }
    }
}
public class InitStatusResponse
{
    public InitStatus InitStatus { get; set; }


    public string Result { get; set; }
}

public enum InitStatus
{
    Null = 0,//未设置
    Init = 1,//初始化
    Normal = 2,//正常链接
}
