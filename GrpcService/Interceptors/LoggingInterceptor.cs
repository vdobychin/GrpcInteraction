using Grpc.Core;
using Grpc.Core.Interceptors;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace GrpcService.Interceptors
{
    public class LoggingInterceptor : Interceptor
    {
        private readonly ILogger<LoggingInterceptor> _logger;
        public LoggingInterceptor(ILogger<LoggingInterceptor> logger)
        {
            _logger = logger;
        }

        public override async Task DuplexStreamingServerHandler<TRequest, TResponse>(IAsyncStreamReader<TRequest> requestStream, IServerStreamWriter<TResponse> responseStream, ServerCallContext context,
            DuplexStreamingServerMethod<TRequest, TResponse> continuation)
        {
            _logger.Log(LogLevel.Information, $"{DateTime.UtcNow}: Start SayHelloStream - {context.Method}");
            await base.DuplexStreamingServerHandler(requestStream, responseStream, context, continuation);
            _logger.Log(LogLevel.Information, $"{DateTime.UtcNow}: End SayHelloStream - {context.Method}");
        }

        public override async Task ServerStreamingServerHandler<TRequest, TResponse>(TRequest request, IServerStreamWriter<TResponse> responseStream, ServerCallContext context, ServerStreamingServerMethod<TRequest, TResponse> continuation)
        {
            _logger.Log(LogLevel.Information, $"{DateTime.UtcNow}: Start SayHelloReplyStream - {context.Method}");
            await base.ServerStreamingServerHandler(request, responseStream, context, continuation);
            _logger.Log(LogLevel.Information, $"{DateTime.UtcNow}: End SayHelloReplyStream - {context.Method}");
        }

        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
        {
            _logger.Log(LogLevel.Information, $"{DateTime.UtcNow}: Start SayHello - {context.Method}");
            var unaryServerHandler = await base.UnaryServerHandler(request, context, continuation);
            _logger.Log(LogLevel.Information, $"{DateTime.UtcNow}: End SayHello - {context.Method}");
            return unaryServerHandler;
        }

    }
}
