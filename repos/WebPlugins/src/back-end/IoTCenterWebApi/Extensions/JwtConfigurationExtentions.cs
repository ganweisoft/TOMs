// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using IoTCenter.Utilities;
using IoTCenterWebApi.BaseCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace IoTCenterWebApi;

public static class JwtConfigurationExtentions
{
    public static string _jwtSecret;
    public static void AddJwtConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(s =>
        {
            s.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            s.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            s.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddCookie(options =>
        {
            _ = int.TryParse(configuration["WebApi:ExpiredTime"], out var expiredTime);

            var minutes = expiredTime <= 0 ? 120 : expiredTime;

            options.ExpireTimeSpan = TimeSpan.FromMinutes(minutes);
            options.Cookie.MaxAge = TimeSpan.FromMinutes(minutes);
            options.Cookie.Name = "iu-session";
            options.Cookie.SameSite = configuration.GetAllowOrigins() ? SameSiteMode.None : SameSiteMode.Lax;
            options.SlidingExpiration = true;
            options.Cookie.HttpOnly = true;
            options.AccessDeniedPath = new PathString("/");
            options.LoginPath = new PathString("/");
            if (configuration.GetAllowOrigins() && !string.IsNullOrEmpty(configuration["WebApi:HttpsPort"]))
            {
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            }
            options.EventsType = typeof(CookieAuthenticationEventsExtensions);
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = CreateValidationParaemters(configuration);

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    var accessToken = context.HttpContext.Request.Cookies["x-access-token"];
                    if (!string.IsNullOrEmpty(accessToken))
                    {
                        context.Token = accessToken;
                    }

                    return Task.CompletedTask;
                }
            };
        });
    }

    private static TokenValidationParameters CreateValidationParaemters(IConfiguration configuration)
    {
        return new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Authentication:JwtBearer:SecurityKey"])),
            ValidateIssuer = true,
            ValidIssuer = configuration["Authentication:JwtBearer:Issuer"],
            ValidateAudience = true,
            ValidAudience = configuration["Authentication:JwtBearer:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
        };
    }
}
