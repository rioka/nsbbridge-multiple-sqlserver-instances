using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NServiceBus;

namespace Bridge
{
    class Program
    {
        public static async Task Main()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            
            var notifierConnectionString = configuration.GetConnectionString("Notifier");
            var receiverConnectionString = configuration.GetConnectionString("Receiver");
            var senderConnectionString = configuration.GetConnectionString("Sender");

            SqlHelper.EnsureDatabaseExists(receiverConnectionString);
            SqlHelper.EnsureDatabaseExists(senderConnectionString);
            SqlHelper.EnsureDatabaseExists(notifierConnectionString);

            var builder = Host.CreateApplicationBuilder();
            var bridgeConfiguration = new BridgeConfiguration();

            #region BridgeConfiguration

            var receiverTransport = new BridgeTransport(new SqlServerTransport(receiverConnectionString))
            {
                Name = "SQL-Receiver",
                AutoCreateQueues = true
            };
            receiverTransport.HasEndpoint("Receiver");

            var senderTransport = new BridgeTransport(new SqlServerTransport(senderConnectionString))
            {
                Name = "SQL-Sender",
                AutoCreateQueues = true
            };
            senderTransport.HasEndpoint("Sender");

            var notifierTransport = new BridgeTransport(new SqlServerTransport(notifierConnectionString))
            {
                Name = "SQL-Notifier",
                AutoCreateQueues = true
            };
            var notifierEndpoint = new BridgeEndpoint("Notifier");
            notifierEndpoint.RegisterPublisher("Shared.ClientOrderReceived, Shared, Version=1.0.0.0", "Receiver");
            notifierTransport.HasEndpoint(notifierEndpoint);

            bridgeConfiguration.AddTransport(senderTransport);
            bridgeConfiguration.AddTransport(receiverTransport);
            bridgeConfiguration.AddTransport(notifierTransport);
            
            bridgeConfiguration.RunInReceiveOnlyTransactionMode();

            #endregion

            builder.UseNServiceBusBridge(bridgeConfiguration);
            var host = builder.Build();
            await host.RunAsync();
        }
    }
}