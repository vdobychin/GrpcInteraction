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

        //SayHelloStream
        public override async Task DuplexStreamingServerHandler<TRequest, TResponse>(IAsyncStreamReader<TRequest> requestStream, IServerStreamWriter<TResponse> responseStream, ServerCallContext context,
            DuplexStreamingServerMethod<TRequest, TResponse> continuation)
        {
            _logger.Log(LogLevel.Information, $"{DateTime.UtcNow}: Start {context.Method}");
            await base.DuplexStreamingServerHandler(requestStream, responseStream, context, continuation);
            _logger.Log(LogLevel.Information, $"{DateTime.UtcNow}: End {context.Method}");
        }

        //SayHelloReplyStream
        public override async Task ServerStreamingServerHandler<TRequest, TResponse>(TRequest request, IServerStreamWriter<TResponse> responseStream, ServerCallContext context, ServerStreamingServerMethod<TRequest, TResponse> continuation)
        {
            _logger.Log(LogLevel.Information, $"{DateTime.UtcNow}: Start {context.Method}");
            await base.ServerStreamingServerHandler(request, responseStream, context, continuation);
            _logger.Log(LogLevel.Information, $"{DateTime.UtcNow}: End {context.Method}");
        }

        //SayHello
        public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(TRequest request, ServerCallContext context, UnaryServerMethod<TRequest, TResponse> continuation)
        {
            _logger.Log(LogLevel.Information, $"{DateTime.UtcNow}: Start {context.Method}");
            var unaryServerHandler = await base.UnaryServerHandler(request, context, continuation);
            _logger.Log(LogLevel.Information, $"{DateTime.UtcNow}: End {context.Method}");
            return unaryServerHandler;
        }

    }
}
