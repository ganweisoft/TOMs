// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenterCore.RsaEncrypt;
using IoTCenterHost.Core.Abstraction.Interfaces.Services;
using IoTCenterHost.Proxy;
using IoTCenterHost.Proxy.Core;
using IoTCenterWebApi.BaseCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoTCenterWebApi;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;

        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
    }

    public IConfiguration Configuration { get; set; }



    public string NetMqIdentity { get; set; }

    public void ConfigureServices(IServiceCollection services)
    {
        _ = bool.TryParse(Configuration.GetSection("WebApi:RSAAutoGenerate").Value, out var change);
        RSAPaddingPkcs1.InitRsaPem(change);

        services.RegistIotHostServices(Configuration);

        services.AddInternalService();

        services.AddIoTService(Configuration);

        #region 建立服务端握手连接
        #endregion

        services.AddIoTCenterCore()
            .ConfigureServices((shellServices, current) =>
            {
                var cacheService = current.GetRequiredService<PermissionCacheService>();
                cacheService.ReSet();
            });

    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseHsts();
        }

        app.UseResponseCompression();

        app.UseResponseCaching();

        app.UseDefaultFiles();

        app.UseStaticFiles();

        app.UseStatusCodePages();

        MimeExtension.AddMimeMapping(app);

        app.UseRouting();

        app.UseCors(policy =>
        {
            CorsPolicyBuilder corsPolicyBuilder;

            var allowOrigins = Configuration.GetSection("AllowOrigins").Get<string[]>();
            if (allowOrigins == null || allowOrigins.Length == 0 || (allowOrigins.Length == 1 && allowOrigins[0] == "*"))
            {
                corsPolicyBuilder = policy.SetIsOriginAllowed(origin => true);
            }
            else
            {
                corsPolicyBuilder = policy.WithOrigins(allowOrigins)
                   .SetPreflightMaxAge(TimeSpan.FromHours(1))
                   .SetIsOriginAllowedToAllowWildcardSubdomains();
            }

            corsPolicyBuilder.AllowCredentials()
                .AllowAnyHeader().WithMethods(HttpMethods.Put, HttpMethods.Delete, HttpMethods.Get, HttpMethods.Post, HttpMethods.Options);
        });

        app.Use((context, next) =>
        {
            context.Response.OnStarting(state =>
            {
                if (context.Items.TryGetValue(DoNotRefreshSlidingExpirationAttribute.DontRenewSlidingCookie,
                    out var issuedSlidingCookie) && (bool)issuedSlidingCookie)
                {
                    var response = (HttpResponse)state;

                    var cookieHeader = response.Headers[HeaderNames.SetCookie].Where(s => !s.Contains("iu-session"))
                    .Aggregate(new StringValues(), (current, s) => StringValues.Concat(current, s));

                    response.Headers[HeaderNames.SetCookie] = cookieHeader;
                }
                return Task.CompletedTask;
            }, context.Response);

            return next(context);
        });

        app.UseAuthentication();

        app.UseAuthorization();

        app.UseWebSockets();

        app.UseIoTCenterCore();

        app.SyncEquipManager();

        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }
}

