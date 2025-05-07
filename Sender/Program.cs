using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NServiceBus;
using Shared;

namespace Sender;

public class Program
{
    static async Task Main()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();
        var connectionString = configuration.GetConnectionString("Sender");
        
        Console.Title = "Sender";

        #region SenderConfiguration

        var endpointConfiguration = new EndpointConfiguration("Sender");
        var routing = endpointConfiguration.UseTransport(new SqlServerTransport(connectionString));

        endpointConfiguration.UseSerialization<SystemJsonSerializer>();
        endpointConfiguration.EnableInstallers();

        routing.RouteToEndpoint(typeof(ClientOrder), "Receiver");

        #endregion

        SqlHelper.EnsureDatabaseExists(connectionString);

        var endpointInstance = await Endpoint.Start(endpointConfiguration);

        Console.WriteLine("Press <enter> to send a message");
        Console.WriteLine("Press any other key to exit");
        while (true)
        {
            if (Console.ReadKey().Key != ConsoleKey.Enter)
            {
                break;
            }
            await PlaceOrder(endpointInstance);
        }
        await endpointInstance.Stop();
    }

    static async Task PlaceOrder(IEndpointInstance endpoint)
    {
        #region SendMessage

        var order = new ClientOrder
        {
            OrderId = Guid.NewGuid()
        };
        await endpoint.Send(order);

        #endregion

        Console.WriteLine($"ClientOrder message sent with ID {order.OrderId}");
    }
}