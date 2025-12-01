// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Utilities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.WindowsServices;
using Serilog;
using System;
using System.IO;

namespace IoTCenterWebApi;


public static class Program
{
    public static Serilogger ApiLog { get; private set; }
    public static bool SingleAppStart { get; private set; }
    public static bool StartHostApp { get; private set; }

    static Program()
    {
        Directory.SetCurrentDirectory(AppContext.BaseDirectory);
    }
    static void Main(string[] args)
    {
        var options = new WebApplicationOptions()
        {
            Args = args,
            ContentRootPath = AppContext.BaseDirectory
        };

        ApiLog = new Serilogger();

        ApiLog.Logger.Information("程序正在启动...");

        StartHostApp = true;

        while (StartHostApp)
        {
            StartHostApp = false;

            var host = CreateHostBuilder(options).Build();

            ApiLog.Logger.Information("启动web中....");

            try
            {
                host.Run();
            }
            catch (Exception ex)
            {
                ApiLog.Logger.Error(ex, ex.Message);
                ApiLog.Logger.Information("web已关闭....");
                continue;
                throw;
            }
            ApiLog.Logger.Information("web已关闭....");
        }

    }

    public static IHostBuilder CreateHostBuilder(WebApplicationOptions options) =>
        Host.CreateDefaultBuilder(options.Args)
            .UseWindowsService(opt => opt.ServiceName = "IoTCenterWeb")
            .UseSystemd()
            .UseSerilog()
            .UseContentRoot(options.ContentRootPath)
            .ConfigureAppConfiguration((context, builder) =>
            {
                builder.SetBasePath(options.ContentRootPath);
                builder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true).AddJsonFile("logsetting.json", optional: true, reloadOnChange: true);
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseKestrel(kestrel =>
                {
                    kestrel.BindOptions().BindHttpPort().BindHttpsPort();

                    ApiLog.Logger.Information("Web应用已启动....");

                    Console.Out.WriteLine("Web应用已启动....");
                })
                .UseIIS()
                .UseStartup<Startup>();
            });
}