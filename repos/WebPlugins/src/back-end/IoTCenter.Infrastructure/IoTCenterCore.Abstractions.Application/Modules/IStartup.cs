using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace IoTCenterCore.Modules
{
    public interface IStartup
    {
        int Order { get; }

        int ConfigureOrder { get; }

        void ConfigureServices(IServiceCollection services);

        void Configure(IApplicationBuilder builder, IEndpointRouteBuilder routes, IServiceProvider serviceProvider);
    }
}
