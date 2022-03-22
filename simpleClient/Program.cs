using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace simpleClient
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Simulate simple HTTP call for a given clientId:100");

            HttpClient client = new HttpClient();


            do
            {
                Console.WriteLine("Press [Enter] to send a request or Ctrl + Break to exit");
                Console.ReadLine();

                var response = await client.GetAsync("https://localhost:5001/?clientid=5");

                Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff}: Response Status:{response.StatusCode}, " +
                                  $"Content:{await response.Content.ReadAsStringAsync()}");
            }
            while (true);
        }
    }
    }

