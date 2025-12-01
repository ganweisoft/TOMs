//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Dapr;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace IoTCenterHost.Dapr;

public static class DaprExtensions
{
    /// <summary>
    /// 添加Dapr
    /// </summary>
    /// <param name="mvcBuilder"></param>
    /// <returns></returns>
    public static IMvcBuilder AddHostDapr(this IMvcBuilder mvcBuilder)
    {
        return mvcBuilder.AddDapr();
    }

    /// <summary>
    /// 使用Dapr
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseDapr(this IApplicationBuilder app)
    {
        return app.UseCloudEvents(new CloudEventsMiddlewareOptions
        {
            ForwardCloudEventPropertiesAsHeaders = true
        });
    }

    /// <summary>
    /// 映射Dapr
    /// </summary>
    /// <param name="endpoint"></param>
    /// <returns></returns>
    public static IEndpointRouteBuilder MapDapr(this IEndpointRouteBuilder endpoint)
    {
        endpoint.MapSubscribeHandler();
        return endpoint;
    }
}