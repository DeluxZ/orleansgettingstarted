using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Configuration;
using Orleans.Runtime;
using OrleansGettingStarted.GrainInterfaces;
using System;
using System.Threading.Tasks;

namespace OrleansGettingStarted.Client
{
    internal class Program
    {
        private const int _initializeAttemptsBeforeFailing = 5;
        private static int _attempt = 0;

        public static async Task<int> Main(string[] args)
        {
            try
            {
                using (IClusterClient client = await StartClientWithRetries())
                {
                    await DoClientWork(client);
                    Console.ReadKey();
                }

                return 0;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.ReadKey();
                return 1;
            }
        }

        private static async Task<IClusterClient> StartClientWithRetries()
        {
            _attempt = 0;

            IClusterClient client;
            client = new ClientBuilder()
                .UseLocalhostClustering()
                .Configure<ClusterOptions>(options =>
                {
                    options.ClusterId = "dev";
                    options.ServiceId = "HelloWorldApp";
                })
                .ConfigureLogging(logging => logging.AddConsole())
                .Build();

            await client.Connect(RetryFilter);

            Console.WriteLine("Client successfully connect to silo host");

            return client;
        }

        private static async Task<bool> RetryFilter(Exception exception)
        {
            if (exception.GetType() != typeof(SiloUnavailableException))
            {
                Console.WriteLine($"Cluster client failed to connect to cluster with unexpected error.  Exception: {exception}");
                return false;
            }
            _attempt++;
            Console.WriteLine($"Cluster client attempt {_attempt} of {_initializeAttemptsBeforeFailing} failed to connect to cluster.  Exception: {exception}");
            if (_attempt > _initializeAttemptsBeforeFailing)
            {
                return false;
            }

            await Task.Delay(TimeSpan.FromSeconds(4));

            return true;
        }

        private static async Task DoClientWork(IClusterClient client)
        {
            Console.WriteLine("Hello, what should I call you?");
            var name = Console.ReadLine();

            if (string.IsNullOrEmpty(name))
            {
                name = "anon";
            }

            var grain = client.GetGrain<IHelloWorld>(Guid.NewGuid());

            var response = await grain.SayHello(name);

            Console.WriteLine($"\n\n{response}\n\n");
        }
    }
}