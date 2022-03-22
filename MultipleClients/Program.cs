using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace MultipleClients
{
    internal class Program
    {
        static int GetNumberOfClientsFromUser()
        {
            do
            {
                Console.WriteLine("Please Enter the number of clients to simulate");

                if (int.TryParse(Console.ReadLine(), out int numberOfClientsToSimulate)) //input validation
                {
                    return numberOfClientsToSimulate;
                }
            }
            while (true);
        }

        static Task RunsimulationAsync(int numberOfClients, CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(
                () =>
                {
                    var randomclient = new Random();
                    var randomTimeFrame = new Random();
                    Parallel.For(
                        0,
                        numberOfClients,
                        async (i, state) =>
                        {
                            var client = new HttpClient();
                            do
                            {
                                string clienturl = $"https://localhost:5001/?clientid={randomclient.Next(1, numberOfClients + 1)}";
                                var response = await client.GetAsync(clienturl, cancellationToken);
                                Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff} Url {clienturl} Status {response.StatusCode}");

                                await Task.Delay(randomTimeFrame.Next(1, 300), cancellationToken);
                            }
                            while (!cancellationToken.IsCancellationRequested);
                        });
                },
                cancellationToken);
        }

        static async Task Main(string[] args)
        {
            var cancallationTokenSource = new CancellationTokenSource();
            var SimulationTask = RunsimulationAsync(GetNumberOfClientsFromUser(), cancallationTokenSource.Token);
            Console.WriteLine("Press Enter To Exit");
            Console.ReadLine();
            cancallationTokenSource.Cancel();
            Console.WriteLine("Task Was Cancelled Waiting For Gracefull ShutDown");
            await SimulationTask;
        }
    }
}

