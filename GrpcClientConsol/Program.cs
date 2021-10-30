using Grpc.Core;
using Grpc.Health.V1;
using Grpc.Net.Client;
using ProtoLib;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GrpcClientConsol
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            //await SayHelloAsync_Rpc();

            //await SayHelloStream_Rpc();

            //await SayHelloReplyStream_Rpc();

            //await SayHelloRequestStream_Rpc();

            await HealthCheck_Rpc();    //docker run --rm -it -p 5001:80 grpcservice

            Console.ReadLine();
        }


        static async Task SayHelloAsync_Rpc()
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Greeter.GreeterClient(channel);

            var reply = await client.SayHelloAsync(new HelloRequest() { Name = "Виктор" }); //Ответ сервера

            Console.WriteLine(reply.Message);
        }

        static async Task SayHelloStream_Rpc()
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Greeter.GreeterClient(channel);

            //streaming клиент (объект)
            using var call = client.SayHelloStream();

            //Отдельный поток для ответов от сервера
            var readTask = Task.Run(async () =>
            {
                await foreach (var response in call.ResponseStream.ReadAllAsync()) //ReadAllAsync - метод, который получает все ответы от сервера
                {
                    Console.WriteLine(response.Message);
                }
            });

            //Цикл для бесконечной отправки сообщения на сервер
            while (true)
            {
                var result = Console.ReadLine();
                if (String.IsNullOrWhiteSpace(result))
                    break;

                await call.RequestStream.WriteAsync(new HelloRequest() { Name = result });
            }

            //Заэвейтить стринговый запрос, чтобы понять что получили все ответы от сервера
            await call.RequestStream.CompleteAsync();
            //Дожидаемся всех ответов от сервера
            await readTask;
        }

        static async Task SayHelloReplyStream_Rpc()
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");
            var client = new Greeter.GreeterClient(channel);

            using var call = client.SayHelloReplyStream(new HelloRequest() { Name = "Виктор" });

            //Отдельный поток для ответов от сервера
            var readTask = Task.Run(async () =>
            {
                await foreach (var response in call.ResponseStream.ReadAllAsync()) //ReadAllAsync - метод, который получает все ответы от сервера
                {
                    Console.WriteLine(response.Message);
                }
            });

            await readTask;
        }

        static async Task SayHelloRequestStream_Rpc()
        {
            var client = new Greeter.GreeterClient(GrpcChannel.ForAddress("https://localhost:5001"));
            using var call = client.SayHelloRequestStream();    //streaming клиент (объект)

            //await call.RequestStream.WriteAsync(new HelloRequest { Name = "Привет сервер" });


            //var request = Console.ReadLine();
            var request = "";
            for (var i = 0; i < 3; i++)
            {
                request += "Виктор ";
                await call.RequestStream.WriteAsync(new HelloRequest { Name = request });
            }


            //Цикл для бесконечной отправки сообщения на сервер
            /*while (true)
            {
                var request = Console.ReadLine();
                if (String.IsNullOrWhiteSpace(request))
                    break;

                await call.RequestStream.WriteAsync(new HelloRequest() { Name = request });
            }*/

            await call.RequestStream.CompleteAsync(); //Сообщение серверу, что клиент закончил отправку сообщений
            var response = await call; //Ответ сервера

            Console.WriteLine(response.Message);
        }

        static async Task HealthCheck_Rpc()
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:5001");  //для контейнера http
            var healthClient = new Health.HealthClient(channel);

            while (true)
            {
                var health = await healthClient.CheckAsync(new HealthCheckRequest { Service = "Живой" });
                Console.WriteLine($"Health Status: {health.Status}");
                Thread.Sleep(5000);
            }            
        }
    }
}
