using Grpc.Core;
using Microsoft.Extensions.Logging;
using ProtoLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GrpcService
{
    public class GreeterService : Greeter.GreeterBase
    {
        private readonly ILogger<GreeterService> _logger;
        public GreeterService(ILogger<GreeterService> logger)
        {
            _logger = logger;
        }

        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            return Task.FromResult(new HelloReply
            {
                Message = "Hello " + request.Name
            });
        }

        public override async Task SayHelloStream(IAsyncStreamReader<HelloRequest> requestStream, IServerStreamWriter<HelloReply> replyStream, ServerCallContext context)
        {
            await foreach(var request in requestStream.ReadAllAsync())
            {
                await replyStream.WriteAsync(new HelloReply()
                {
                    Message = "Hello " + request.Name + DateTime.UtcNow
                });
            }
        }

        public override async Task SayHelloReplyStream(HelloRequest request, IServerStreamWriter<HelloReply> replyStream, ServerCallContext context)
        {            
            await replyStream.WriteAsync(new HelloReply()
            {
                Message = "Hello " + request.Name + DateTime.UtcNow
            });            
        }
    }
}
