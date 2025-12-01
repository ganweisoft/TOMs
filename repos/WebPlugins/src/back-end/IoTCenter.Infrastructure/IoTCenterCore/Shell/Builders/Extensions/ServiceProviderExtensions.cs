using System;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace IoTCenterCore.Environment.Shell.Builders
{
    public static class ServiceProviderExtensions
    {
        public static IServiceCollection CreateChildContainer(this IServiceProvider serviceProvider, IServiceCollection serviceCollection)
        {
            IServiceCollection clonedCollection = new ServiceCollection();
            var servicesByType = serviceCollection.GroupBy(s => s.ServiceType);

            foreach (var services in servicesByType)
            {
                if (services.Key == typeof(IStartupFilter))
                {
                }

                else if (services.Key.IsGenericTypeDefinition)
                {
                    foreach (var service in services)
                    {
                        clonedCollection.Add(service);
                    }
                }

                else if (services.Count() == 1)
                {
                    var service = services.First();

                    if (service.Lifetime == ServiceLifetime.Singleton)
                    {

                        if (typeof(IDisposable).IsAssignableFrom(service.GetImplementationType()) || service.ImplementationFactory != null)
                        {
                            clonedCollection.CloneSingleton(service, serviceProvider.GetService(service.ServiceType));
                        }
                        else
                        {
                            clonedCollection.CloneSingleton(service, sp => serviceProvider.GetService(service.ServiceType));

                        }
                    }
                    else
                    {
                        clonedCollection.Add(service);
                    }
                }

                else if (services.All(s => s.Lifetime != ServiceLifetime.Singleton))
                {
                    foreach (var service in services)
                    {
                        clonedCollection.Add(service);
                    }
                }

                else if (services.All(s => s.Lifetime == ServiceLifetime.Singleton))
                {
                    var instances = serviceProvider.GetServices(services.Key);

                    for (var i = 0; i < services.Count(); i++)
                    {
                        if (instances.Count() - 1 < i)
                        {
                            continue;
                        }

                        clonedCollection.CloneSingleton(services.ElementAt(i), instances.ElementAt(i));
                    }
                }

                else
                {
                    using (var scope = serviceProvider.CreateScope())
                    {
                        var instances = scope.ServiceProvider.GetServices(services.Key);

                        for (var i = 0; i < services.Count(); i++)
                        {
                            if (instances.Count() - 1 < i)
                            {
                                continue;
                            }

                            if (services.ElementAt(i).Lifetime == ServiceLifetime.Singleton)
                            {
                                clonedCollection.CloneSingleton(services.ElementAt(i), instances.ElementAt(i));
                            }
                            else
                            {
                                clonedCollection.Add(services.ElementAt(i));
                            }
                        }
                    }
                }
            }

            return clonedCollection;
        }
    }
}
