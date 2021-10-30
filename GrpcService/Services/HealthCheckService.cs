using Grpc.Core;
using Grpc.Health.V1;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
//using Serilog;
using System;
using System.Threading.Tasks;
using static Grpc.Health.V1.HealthCheckResponse.Types;

namespace GrpcService
{
    public class HealthCheckService : Health.HealthBase
    {
        private readonly ILogger<GreeterService> _logger;
        private readonly Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckService _healthCheckService;

        public HealthCheckService(ILogger<GreeterService> logger, Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckService healthCheckService)
        {
            _logger = logger;
            _healthCheckService = healthCheckService;
        }
        
        public override async Task<HealthCheckResponse> Check(HealthCheckRequest request, ServerCallContext context)
        {
            Func<HealthCheckRegistration, bool> GetHealthCheckPredicate()
            {
                string[] tags = request.Service?.Split(";") ?? Array.Empty<string>();

                static bool PassAlways(HealthCheckRegistration _) => true;

                if (tags.Length == 0)
                {
                    return PassAlways;
                }

                bool CheckContainsTags(HealthCheckRegistration healthCheck) =>
                healthCheck.Tags.IsSupersetOf(tags);

                return CheckContainsTags;
            }

            var result = await _healthCheckService.CheckHealthAsync(GetHealthCheckPredicate(), context.CancellationToken);
            var status = result.Status == HealthStatus.Healthy ? ServingStatus.Serving : ServingStatus.NotServing;

            _logger.Log(LogLevel.Information, status.ToString());
            return new HealthCheckResponse
            {
                Status = status
            };
        }
    }
}
