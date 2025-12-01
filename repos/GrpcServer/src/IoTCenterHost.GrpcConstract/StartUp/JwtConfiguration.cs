//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace IoTCenterHost.AppServices
{
    public static class JwtConfiguration
    {
        public static IServiceCollection RegistAuthentication(this IServiceCollection services, IConfiguration _configuration)
        {
            services.AddAuthentication(s =>
            {
                s.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                s.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                s.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                            .AddJwtBearer(options =>
                            {
                                options.TokenValidationParameters = new TokenValidationParameters
                                {
                                    ValidateIssuerSigningKey = true,
                                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration["SecurityKey"])),

                                    ValidateIssuer = true,
                                    ValidIssuer = _configuration["Issuer"],

                                    ValidateAudience = true,
                                    ValidAudience = _configuration["Audience"],


                                    ValidateLifetime = true,

                                    ClockSkew = TimeSpan.Zero
                                };

                                options.Events = new JwtBearerEvents
                                {
                                    OnAuthenticationFailed = context =>
                                    {
                                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                                        {
                                            context.Response.Headers.Add("Token-Expired", "true");
                                        }
                                        return Task.CompletedTask;
                                    }
                                };
                            });
            return services;
        }
    }
}
