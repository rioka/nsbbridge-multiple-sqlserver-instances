using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NServiceBus;

namespace Receiver;

class Program
{
    static async Task Main()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
        var connectionString = configuration.GetConnectionString("Receiver");
        
        Console.Title = "Receiver";

        #region ReceiverConfiguration

        var endpointConfiguration = new EndpointConfiguration("Receiver");
        endpointConfiguration.UseTransport(new SqlServerTransport(connectionString));

        endpointConfiguration.UseSerialization<SystemJsonSerializer>();
        endpointConfiguration.EnableInstallers();

        #endregion

        SqlHelper.EnsureDatabaseExists(connectionString);

        var endpointInstance = await Endpoint.Start(endpointConfiguration);

        Console.WriteLine("Press any key to exit");
        Console.WriteLine("Waiting for Order messages from the Sender");
        Console.ReadKey();
        await endpointInstance.Stop();
    }
}