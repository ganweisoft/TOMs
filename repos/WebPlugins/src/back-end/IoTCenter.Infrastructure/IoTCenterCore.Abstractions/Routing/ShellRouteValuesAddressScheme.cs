using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace IoTCenterCore.Routing
{
    public sealed class ShellRouteValuesAddressScheme : IEndpointAddressScheme<RouteValuesAddress>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEnumerable<IShellRouteValuesAddressScheme> _schemes;

        private bool _defaultSchemeInitialized;
        private IEndpointAddressScheme<RouteValuesAddress> _defaultScheme;

        public ShellRouteValuesAddressScheme(IHttpContextAccessor httpContextAccessor, IEnumerable<IShellRouteValuesAddressScheme> schemes)
        {
            _httpContextAccessor = httpContextAccessor;
            _schemes = schemes;
        }

        public IEnumerable<Endpoint> FindEndpoints(RouteValuesAddress address)
        {
            if (address == null)
            {
                throw new ArgumentNullException(nameof(address));
            }

            foreach (var scheme in _schemes)
            {
                var endpoints = scheme.FindEndpoints(address);

                if (endpoints.Any())
                {
                    return endpoints;
                }
            }

            if (!_defaultSchemeInitialized)
            {
                lock (this)
                {
                    _defaultScheme = _httpContextAccessor.HttpContext?.RequestServices
                        .GetServices<IEndpointAddressScheme<RouteValuesAddress>>()
                        .Where(scheme => scheme.GetType() != GetType())
                        .LastOrDefault();

                    _defaultSchemeInitialized = true;
                }
            }

            return _defaultScheme?.FindEndpoints(address) ?? Enumerable.Empty<Endpoint>();
        }
    }
}
