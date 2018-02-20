using System;
using System.Threading.Tasks;

namespace MyIP.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var task = Task.Run(async () =>
            {
                await Run(args);
            });

            task.GetAwaiter().GetResult();
        }

        static async Task Run(string[] args)
        {
            try
            {
                var client = new MyIPClient();
                var response = await client.GetAsync();

                if (response.IPv4Address != null)
                    Console.WriteLine($"IPv4 Address: {response.IPv4Address}");

                if (response.IPv6Address != null)
                    Console.WriteLine($"IPv6 Address: {response.IPv6Address}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex}");
            }

            Console.WriteLine();
        }
    }
}
