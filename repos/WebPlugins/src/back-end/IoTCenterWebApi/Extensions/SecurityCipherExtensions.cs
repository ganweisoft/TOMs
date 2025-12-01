// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenterWebApi;
using Microsoft.AspNetCore.Server.Kestrel.Https;
using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Runtime.InteropServices;
using System.Security.Authentication;

namespace Microsoft.AspNetCore.Hosting
{
    public static class SecurityCipherExtensions
    {
        public static HttpsConnectionAdapterOptions AdapterOptions(this HttpsConnectionAdapterOptions options)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return options;
            }

            options.OnAuthenticate = (connectionContext, authenticationOptions) =>
            {
                var ciphers = new List<TlsCipherSuite>()
                {
                    TlsCipherSuite.TLS_DHE_RSA_WITH_AES_128_GCM_SHA256,
                    TlsCipherSuite.TLS_DHE_RSA_WITH_AES_256_GCM_SHA384,
                    TlsCipherSuite.TLS_DHE_DSS_WITH_AES_128_GCM_SHA256,
                    TlsCipherSuite.TLS_DHE_DSS_WITH_AES_256_GCM_SHA384,
                    TlsCipherSuite.TLS_PSK_WITH_AES_128_GCM_SHA256,
                    TlsCipherSuite.TLS_PSK_WITH_AES_256_GCM_SHA384,
                    TlsCipherSuite.TLS_DHE_PSK_WITH_AES_128_GCM_SHA256,
                    TlsCipherSuite.TLS_DHE_PSK_WITH_AES_256_GCM_SHA384,
                    TlsCipherSuite.TLS_DHE_PSK_WITH_CHACHA20_POLY1305_SHA256,
                    TlsCipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_GCM_SHA256,
                    TlsCipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_GCM_SHA384,
                    TlsCipherSuite.TLS_ECDHE_RSA_WITH_AES_128_GCM_SHA256,
                    TlsCipherSuite.TLS_ECDHE_RSA_WITH_AES_256_GCM_SHA384,
                    TlsCipherSuite.TLS_ECDHE_RSA_WITH_CHACHA20_POLY1305_SHA256,
                    TlsCipherSuite.TLS_ECDHE_PSK_WITH_CHACHA20_POLY1305_SHA256,
                    TlsCipherSuite.TLS_ECDHE_PSK_WITH_AES_128_GCM_SHA256,
                    TlsCipherSuite.TLS_ECDHE_PSK_WITH_AES_256_GCM_SHA384,
                    TlsCipherSuite.TLS_ECDHE_PSK_WITH_AES_128_CCM_SHA256,
                    TlsCipherSuite.TLS_DHE_RSA_WITH_AES_128_CCM,
                    TlsCipherSuite.TLS_DHE_RSA_WITH_AES_256_CCM,
                    TlsCipherSuite.TLS_DHE_RSA_WITH_AES_128_CCM_8,
                    TlsCipherSuite.TLS_DHE_RSA_WITH_AES_256_CCM_8,
                    TlsCipherSuite.TLS_DHE_RSA_WITH_CHACHA20_POLY1305_SHA256,
                    TlsCipherSuite.TLS_PSK_WITH_AES_128_CCM,
                    TlsCipherSuite.TLS_PSK_WITH_AES_256_CCM,
                    TlsCipherSuite.TLS_DHE_PSK_WITH_AES_128_CCM,
                    TlsCipherSuite.TLS_DHE_PSK_WITH_AES_256_CCM,
                    TlsCipherSuite.TLS_PSK_WITH_AES_128_CCM_8,
                    TlsCipherSuite.TLS_PSK_WITH_AES_256_CCM_8,
                    TlsCipherSuite.TLS_PSK_DHE_WITH_AES_128_CCM_8,
                    TlsCipherSuite.TLS_PSK_DHE_WITH_AES_256_CCM_8,
                    TlsCipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_CCM,
                    TlsCipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_CCM,
                    TlsCipherSuite.TLS_ECDHE_ECDSA_WITH_AES_128_CCM_8,
                    TlsCipherSuite.TLS_ECDHE_ECDSA_WITH_AES_256_CCM_8,
                    TlsCipherSuite.TLS_ECDHE_ECDSA_WITH_CHACHA20_POLY1305_SHA256
                };

                authenticationOptions.EnabledSslProtocols = SslProtocols.Tls12;

                try
                {
                    authenticationOptions.CipherSuitesPolicy = new CipherSuitesPolicy(ciphers);
                }
                catch (Exception ex)
                {
                    Program.ApiLog.Logger.Error($"自定义配置安全密码套件出现异常：{ex}");
                }
            };

            return options;
        }
    }
}
