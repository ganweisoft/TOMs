using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace IoTCenterCore.Modules
{
    public abstract class StartupBase : IStartup
    {
        public virtual int Order { get; } = 0;

        public virtual int ConfigureOrder => Order;

        public virtual void ConfigureServices(IServiceCollection services)
        {
        }

        public virtual void Configure(IApplicationBuilder app, IEndpointRouteBuilder routes, IServiceProvider serviceProvider)
        {
        }
    }
}
