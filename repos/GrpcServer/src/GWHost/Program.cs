//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using GWDataCenter;
using GWHost;
using IoTCenterHost.Core.Extension;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.WindowsServices;
using Serilog;
using Serilog.Events;
using System;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Security.Authentication;
using System.Threading.Tasks;
using FileExtension = IoTCenterHost.Core.Extension.FileExtension;

namespace GwWebHost
{
    public class Program
    {
        private const string localhost = "127.0.0.1";
        private const string anyIpAddr = "0.0.0.0";
        private static string configurationXml = "";

        static Program()
        {
            if (WindowsServiceHelpers.IsWindowsService())
            {
                Directory.SetCurrentDirectory(AppContext.BaseDirectory);
            }
        }

        public static async Task Main(string[] args)
        {
            string RootPathName = Path.Combine(General.GetApplicationRootPath(), "database");
            string RootBakPathName = Path.Combine(General.GetApplicationRootPath(), "databak");
            configurationXml = Path.Combine(Directory.GetParent(AppContext.BaseDirectory).Parent.FullName, "data", "AlarmCenter", "AlarmCenterProperties.xml");
            string pathname = Path.Combine(RootPathName, "Database.db");
            string bakpathname = Path.Combine(RootBakPathName, "Database.db");

            if (!File.Exists(pathname))
            {
                Console.WriteLine(" Fail To Find Database File In /database Folder. ");

                if (!File.Exists(bakpathname))
                {
                    Console.WriteLine("Fail To Find Database File In /databak Folder. ");
                }
                else
                {
                    FileInfo f = new FileInfo(bakpathname);
                    f.CopyTo(pathname);
                    Console.WriteLine("Copy Default Database File From /databak To /database Folder.");
                }
            }
            try
            {
                var _host = CreateHostBuilder(args).Build();

                bool singleAppStart = bool.Parse(FileExtension.ReadXml(configurationXml, "HostServer", "SingleAppStart") ?? "false");
                if (singleAppStart)
                {
                    await StartIoTCenterWebApi();
                }
                _host.Run();
            }
            catch (Exception ex)
            {
                Console.Out.WriteLine(@$"网关启动失败：{ex}");
            }
        }
        public static async Task StartIoTCenterWebApi()
        {
            await Task.Factory.StartNew(async () =>
            {
                try
                {
                    while (!Startup.HostStart)
                    {
                        await Task.Delay(100);
                        continue;
                    }
                    LoadContext();
                }
                catch (Exception ex)
                {
                    Console.Out.WriteLine(@$"Web应用启动失败：{ex}");
                }
            }, TaskCreationOptions.LongRunning);
        }

        private static void LoadContext()
        {
            string path = System.IO.Path.Combine(Directory.GetParent(AppContext.BaseDirectory).Parent.FullName, "IoTCenterWeb", "publish");
            string file = System.IO.Path.Combine(path, "IoTCenterWebApi.dll");//"D:\\ganwei\\v6.1开发测试\\IoTCenterWeb\\publish\\IoTCenterWebApi.dll";
            if (!System.IO.File.Exists(file)) return;

            var webApiAlc = new WebApiAssemblyLoadContext(file);
            Assembly assembly = webApiAlc.LoadFromAssemblyPath(file);
            MethodInfo entryPoint = assembly.EntryPoint;

            string[] methodParams = new string[] { path };

            entryPoint.Invoke(null, new object[] { methodParams });
        }




        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            string sslPassword = string.Empty;
            string httpPort = FileExtension.ReadXml(configurationXml, "HostServer", "HttpPort");
            string httpsPort = FileExtension.ReadXml(configurationXml, "HostServer", "HttpsPort");
            string daprPort = FileExtension.ReadXml(configurationXml, "HostServer", "DaprPort");
            if (int.TryParse(httpsPort, out int port))
                IsPortUseable(port, out string msg);
            if (int.TryParse(httpPort, out port))
                IsPortUseable(port, out string msg);
            if (int.TryParse(daprPort, out port))
                IsPortUseable(port, out string msg);

            var path = FileExtension.ReadXml(configurationXml, "HostServer", "Urls");// GWDataCenter.DataCenter.GetPropertyFromPropertyService("HostServer", "Urls", "");

            var build = Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .UseSystemd()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    string hostIpAddress = FileExtension.ReadXml(configurationXml, "HostServer", "HostIpAddr");

                    if (string.IsNullOrEmpty(hostIpAddress)) hostIpAddress = localhost;

                    IPAddress ipAddr = IPAddress.Any;
                    if (!IPAddress.TryParse(hostIpAddress, out ipAddr))
                    {
                        Console.Out.WriteLine($"服务无法启动，输入的IP地址【{hostIpAddress}】不合法。");
                    }

                    webBuilder.UseKestrel(options =>
                    {
                        string url = string.Empty;

                        if (!string.IsNullOrWhiteSpace(httpPort))
                        {
                            options.Listen(ipAddr, Convert.ToInt32(httpPort), listenOptions => { listenOptions.Protocols = HttpProtocols.Http2; });

                            if (!string.Equals(hostIpAddress, localhost) && !string.Equals(hostIpAddress, anyIpAddr))
                            {
                                options.ListenLocalhost(Convert.ToInt32(httpPort), listenOptions => { listenOptions.Protocols = HttpProtocols.Http2; });
                            }
                            url += $"http://{hostIpAddress}:{httpPort} for http";
                        }
                        if (!string.IsNullOrWhiteSpace(httpsPort))
                        {
                            var certFile = FileExtension.ReadXml(configurationXml, "HostServer", "HttpsCertFile");
                            string sslPath = string.Empty;
                            if (!string.IsNullOrEmpty(certFile))
                            {
                                sslPath = Path.Combine(AppContext.BaseDirectory, "StorageFile", "general", "certificate", certFile);
                            }
                            var password = FileExtension.ReadXml(configurationXml, "HostServer", "HttpsPassword");

                            options.Listen(ipAddr, Convert.ToInt32(httpsPort), listenOption =>
                            {
                                if (File.Exists(sslPath) && !string.IsNullOrWhiteSpace(password))
                                {
                                    listenOption.UseHttps(sslPath, password);

                                    options.ConfigureHttpsDefaults(s => { s.SslProtocols = SslProtocols.Tls12; });

                                    url += $";https://{hostIpAddress}:{httpsPort} for httpsPort";
                                }
                                else
                                {
                                    url += $";http://{hostIpAddress}:{httpsPort} for netmq";
                                }
                                listenOption.Protocols = HttpProtocols.Http1;
                            });
                        }
                        if (!string.IsNullOrEmpty(daprPort))
                        {
                            // 使用dapr端口
                            options.Listen(ipAddr, Convert.ToInt32(daprPort), listenOption =>
                            {
                                url += $";http://127.0.0.1:{daprPort} for dapr";
                                listenOption.Protocols = HttpProtocols.Http1;
                            });
                        }
                        var language = FileExtension.ReadXml(configurationXml, "", "", "CoreProperties.UILanguage");
                        if (string.Equals(language, "en-US"))
                        {
                            Console.Out.WriteLine($"Server is started，the port is {url}");
                        }
                        else
                            Console.Out.WriteLine($"服务已启动，端口为{url}");

                    }).UseStartup<Startup>();
                })
                .ConfigureAppConfiguration(opt =>
                {
                    opt.SetBasePath(AppContext.BaseDirectory)
                   .AddJsonFile("appsettings.json", optional: true)
                   .AddJsonFile("logsetting.json", optional: true, reloadOnChange: true)
                   .AddCommandLine(args);
                });


            build.UseSerilog((context, services, loggerConfig) =>
            {
                var config = context.Configuration;

                var outputTemplate = config["Serilog:WriteTo:0:Args:outputTemplate"];
                var fileSizeLimitBytes = long.TryParse(config["Serilog:WriteTo:0:Args:fileSizeLimitBytes"], out var sizeLimit)
                    ? sizeLimit : 10 * 1024 * 1024; // 默认10MB
                var rollOnFileSizeLimit = bool.TryParse(config["Serilog:WriteTo:0:Args:rollOnFileSizeLimit"], out var roll) && roll;
                var retainedFileCountLimit = int.TryParse(config["Serilog:WriteTo:0:Args:retainedFileCountLimit"], out var countLimit)
                    ? countLimit : 10;

                var strLevel = config["Serilog:MinimumLevel"] ?? "Information";
                var msLevel = config["Logging:LogLevel:Default"] ?? "Warning";

                var defaultLevel = Enum.TryParse<LogEventLevel>(strLevel, out var parsedLevel) ? parsedLevel : LogEventLevel.Information;
                var microsoftLevel = Enum.TryParse<LogEventLevel>(msLevel, out var parsedMsLevel) ? parsedMsLevel : LogEventLevel.Warning;

                loggerConfig.ReadFrom.Configuration(config)
                    .Enrich.FromLogContext()
                    .MinimumLevel.Is(defaultLevel)
                    .MinimumLevel.Override("Microsoft", microsoftLevel)
                    .MinimumLevel.Override("Microsoft.AspNetCore", microsoftLevel)
                    .MinimumLevel.Override("Quartz", microsoftLevel)
                    .MinimumLevel.Override("Grpc", microsoftLevel)
                    .MinimumLevel.Override("System", microsoftLevel)
                    .WriteTo.Map(
                        le => new Tuple<DateTime, LogEventLevel>(
                            new DateTime(le.Timestamp.Year, le.Timestamp.Month, le.Timestamp.Day), le.Level),
                        (key, log) => log.File(
                            path: Path.Combine(AppContext.BaseDirectory, "../log", $"{key.Item1:yyyy-MM-dd}/{key.Item2}-{key.Item1:yyyyMMdd}.txt"),
                            outputTemplate: outputTemplate,
                            fileSizeLimitBytes: fileSizeLimitBytes,
                            rollOnFileSizeLimit: rollOnFileSizeLimit,
                            retainedFileCountLimit: retainedFileCountLimit,
                            shared: true
                        ),
                        sinkMapCountLimit: 1
                    );
            });
            return build;
        }


        private static bool IsPortUseable(int port, out string msg)
        {
            msg = "";
            if ((port != 0 && port < 1024) || port > 65536)
            {
                msg = "端口应介于1024到65536之间";
                return false;
            }
            IPGlobalProperties iPGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] iPEndPoints = iPGlobalProperties.GetActiveTcpListeners();
            foreach (IPEndPoint endpoint in iPEndPoints)
            {
                if (endpoint.Port == port)
                {
                    msg = $"应用程序将无法正常访问网关服务端，原因：端口【{port}】被占用，请检查端口占用情况或修改服务端口.";
                    Console.Out.WriteLine("*******************");
                    Console.Out.WriteLine(msg);
                    Console.Out.WriteLine("*******************");
                    return false;
                }
            }
            return true;
        }
    }
}

