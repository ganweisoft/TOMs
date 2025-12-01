//  Copyright (c) 2020-2025 Beijing TOMs Software Technology Co., Ltd
using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;

namespace IoTCenterHost.Core.Startup
{
    public class ServerCallbackInterceptor : Interceptor
    {
        private readonly ILogger<ServerCallbackInterceptor> _logger;
        private readonly bool _EnableRequestLog = false;
        private readonly IEnumerable<System.Net.IPNetwork> _iPNetworks;

        public ServerCallbackInterceptor(ILogger<ServerCallbackInterceptor> logger, IConfiguration configuration)
        {
            _logger = logger;
            _EnableRequestLog = string.Equals(configuration["EnableRequestLog"], "true", StringComparison.InvariantCultureIgnoreCase);

            _iPNetworks = configuration.GetSection("GrpcAllowAddress").Get<string[]>().Select(ip =>
            {
                var ipSplit = ip.Split("/");
                return ipSplit.Length switch
                {
                    1 => new System.Net.IPNetwork(IPAddress.Parse(ipSplit[0]), 32),
                    >= 2 => new System.Net.IPNetwork(IPAddress.Parse(ipSplit[0]), int.Parse(ipSplit[1])),
                    _ => new System.Net.IPNetwork(IPAddress.Loopback, 32)
                };
            });
        }

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
            TRequest request,
            ServerCallContext context,
            UnaryServerMethod<TRequest, TResponse> continuation)
        {
            TResponse response = null;
            try
            {
                if (!this.ValidateIpAddress(context.Peer))
                {
                    return response;
                }
                response = await continuation(request, context);
            }
            catch (Exception ex)
            {
                LogError<TRequest, TResponse>(MethodType.Unary, context, ex);
            }
            return response;
        }

        public override async Task<TResponse> ClientStreamingServerHandler<TRequest, TResponse>(
            IAsyncStreamReader<TRequest> requestStream,
            ServerCallContext context,
            ClientStreamingServerMethod<TRequest, TResponse> continuation)
        {
            TResponse response = null;
            try
            {
                if (!this.ValidateIpAddress(context.Peer))
                {
                    return response;
                }

                response = await base.ClientStreamingServerHandler(requestStream, context, continuation);
            }
            catch (Exception ex)
            {
                LogError<TRequest, TResponse>(MethodType.ClientStreaming, context, ex);
            }
            return response;
        }

        public override async Task ServerStreamingServerHandler<TRequest, TResponse>(
            TRequest request,
            IServerStreamWriter<TResponse> responseStream,
            ServerCallContext context,
            ServerStreamingServerMethod<TRequest, TResponse> continuation)
        {
            try
            {
                if (!this.ValidateIpAddress(context.Peer))
                {
                    return;
                }

                await base.ServerStreamingServerHandler(request, responseStream, context, continuation);
            }
            catch (Exception ex)
            {
                LogError<TRequest, TResponse>(MethodType.ServerStreaming, context, ex);
            }
        }

        public override async Task DuplexStreamingServerHandler<TRequest, TResponse>(
            IAsyncStreamReader<TRequest> requestStream,
            IServerStreamWriter<TResponse> responseStream,
            ServerCallContext context,
            DuplexStreamingServerMethod<TRequest, TResponse> continuation)
        {
            try
            {
                if (!this.ValidateIpAddress(context.Peer))
                {
                    return;
                }

                await base.DuplexStreamingServerHandler(requestStream, responseStream, context, continuation);
            }
            catch (Exception ex)
            {
                LogError<TRequest, TResponse>(MethodType.ServerStreaming, context, ex);
            }
        }

        private void LogCall<TRequest, TResponse>(MethodType methodType, ServerCallContext context)
            where TRequest : class
            where TResponse : class
        {
            WriteMetadata(context.RequestHeaders, "caller-user");
            WriteMetadata(context.RequestHeaders, "caller-machine");
            WriteMetadata(context.RequestHeaders, "caller-os");

            void WriteMetadata(Metadata headers, string key)
            {
                var headerValue = headers.SingleOrDefault(h => h.Key == key)?.Value;
            }
        }

        private void LogError<TRequest, TResponse>(MethodType methodType, ServerCallContext context, Exception exception)
             where TRequest : class
             where TResponse : class
        {
            string msg = $"服务端处理请求出现异常{System.DateTime.Now}:Starting call. Type: {methodType},{context.Method}. Request: {typeof(TRequest)}. Response: {typeof(TResponse)},Execption:{exception}";
        }

        private bool ValidateIpAddress(string remoteAddress)
        {
            var peerSplit = remoteAddress.Split(":");
            return peerSplit.Length == 3 && _iPNetworks.Any(allow => allow.Contains(IPAddress.Parse(peerSplit[1])));
        }
    }
}
