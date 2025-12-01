// Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;

namespace IoTCenterWebApi.BaseCore;

public static class MimeExtension
{
    public static IApplicationBuilder AddMimeMapping(this IApplicationBuilder app)
    {
        var configuration = app.ApplicationServices.GetRequiredService<IConfiguration>();

        var provider = new FileExtensionContentTypeProvider();
        foreach (var mimeType in configuration.GetSection("MimeType").GetChildren().ToList())
        {
            provider.Mappings[$".{mimeType.Key}"] = mimeType.Value;
        }
        app.UseStaticFiles(new StaticFileOptions
        {
            ContentTypeProvider = provider
        });
        return app;
    }
}
