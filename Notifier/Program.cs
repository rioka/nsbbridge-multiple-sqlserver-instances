using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NServiceBus;

namespace Notifier;

class Program
{
    static async Task Main()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
        var connectionString = configuration.GetConnectionString("Notifier");
        
        Console.Title = "Notifier";

        #region NotifierConfiguration

        var endpointConfiguration = new EndpointConfiguration("Notifier");
        endpointConfiguration.UseTransport(new SqlServerTransport(connectionString));

        endpointConfiguration.UseSerialization<SystemJsonSerializer>();
        endpointConfiguration.EnableInstallers();

        #endregion

        SqlHelper.EnsureDatabaseExists(connectionString);

        var endpointInstance = await Endpoint.Start(endpointConfiguration);

        Console.WriteLine("Press any key to exit");
        Console.WriteLine("Waiting for Notification from Receiver...");
        Console.ReadKey();
        await endpointInstance.Stop();
    }
}