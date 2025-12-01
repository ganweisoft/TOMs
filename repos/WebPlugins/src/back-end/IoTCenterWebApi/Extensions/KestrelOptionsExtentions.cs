// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Utilities;
using IoTCenterCore.SelfSignedCertificate;
using IoTCenterWebApi.BaseCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Net;
using System.Security.Authentication;

namespace IoTCenterWebApi
{
    public static class KestrelOptionsExtentions
    {
        public static KestrelServerOptions BindOptions(this KestrelServerOptions options)
        {
            options.AddServerHeader = false;
            return options;
        }

        public static KestrelServerOptions BindHttpPort(this KestrelServerOptions options)
        {
            var webapiOptions = options.ApplicationServices
                .GetRequiredService<IOptions<WebApiConfigOptions>>().Value;

            if (string.IsNullOrEmpty(webapiOptions.HttpPort))
            {
                return options;
            }

            Program.ApiLog.Logger.Information($"开始绑定端口,http:{webapiOptions.HttpPort},https:{webapiOptions.HttpsPort}");

            var port = int.TryParse(webapiOptions.HttpPort, out int httpPort) ? httpPort : 44381;

            if (string.IsNullOrEmpty(webapiOptions.IpAddress) || webapiOptions.IpAddress == "*" || webapiOptions.IpAddress == "::")
            {
                options.ListenAnyIP(port);
            }
            else if (IPAddress.TryParse(webapiOptions.IpAddress, out IPAddress ipAddress))
            {
                options.Listen(ipAddress, port);
            }
            else
            {
                throw new ArgumentException($"无效的 IP 地址: {webapiOptions.IpAddress}");
            }

            Program.ApiLog.Logger.Debug($"监听端口,http:{webapiOptions.HttpPort} 完成.");
            return options;
        }

        public static KestrelServerOptions BindHttpsPort(this KestrelServerOptions options)
        {
            var webapiOptions = options.ApplicationServices
                .GetRequiredService<IOptions<WebApiConfigOptions>>().Value;

            if (string.IsNullOrEmpty(webapiOptions.HttpPort))
            {
                return options;
            }

            var sslPassword = IoTCenterCertificate.Init(Program.ApiLog, webapiOptions.SSLAutoGenerate);

            if (string.IsNullOrEmpty(sslPassword))
            {
                sslPassword = webapiOptions.SSLPassword;
            }

            if (string.IsNullOrEmpty(sslPassword))
            {
                return options;
            }

            var sslCertPath = Path.Combine(AppContext.BaseDirectory, webapiOptions.SSLName);

            var port = int.TryParse(webapiOptions.HttpsPort, out int httpsPort) ? httpsPort : 44380;

            if (string.IsNullOrEmpty(webapiOptions.IpAddress) || webapiOptions.IpAddress == "*" || webapiOptions.IpAddress == "::")
            {
                options.ListenAnyIP(httpsPort, listenOption =>
                {
                    HttpsBindCert(options, listenOption, webapiOptions, sslCertPath, httpsPort, sslPassword);
                });
            }
            else if (IPAddress.TryParse(webapiOptions.IpAddress, out IPAddress parsedIp))
            {
                options.Listen(parsedIp, httpsPort, listenOption =>
                {
                    HttpsBindCert(options, listenOption, webapiOptions, sslCertPath, httpsPort, sslPassword);
                });
            }
            else
            {
                throw new ArgumentException($"无效的 IP 地址: {webapiOptions.IpAddress}");
            }
            return options;
        }

        private static void HttpsBindCert(KestrelServerOptions kestrelServerOptions,        ListenOptions listenOption,
            WebApiConfigOptions webapiOptions, string sslCertPath,
            int httpsPort, string decryptPassword)
        {
            try
            {
                if (!webapiOptions.CipherAdapterEnable)
                {
                    kestrelServerOptions.ConfigureHttpsDefaults(co =>
                    {
                        co.SslProtocols = SslProtocols.Tls12;
                    });

                    listenOption.UseHttps(sslCertPath, decryptPassword);
                }
                else
                {
                    listenOption.UseHttps(sslCertPath, decryptPassword, configureOptions =>
                    {
                        configureOptions.AdapterOptions();
                    });
                }

                Program.ApiLog.Logger.Debug($"监听端口, https:{httpsPort} 完成.");
            }
            catch (Exception ex)
            {
                Program.ApiLog.Logger.Error($"开始绑定端口, https:{httpsPort}，加载证书出现异常: {ex}");
            }
        }
    }
}
