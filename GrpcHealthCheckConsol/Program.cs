using Grpc.Health.V1;
using Grpc.Net.Client;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GrpcHealthCheckConsol
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");  //для контейнера http
            var healthClient = new Health.HealthClient(channel);

            while (true)
            {
                var health = await healthClient.CheckAsync(new HealthCheckRequest { Service = "Живой" });
                Console.WriteLine($"Health Status: {health.Status}");
                Thread.Sleep(5000);
            }

            //Console.ReadLine();
        }
    }
}
