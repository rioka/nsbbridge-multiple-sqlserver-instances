using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;
using Shared;

namespace Notifier;

public class OrderHandler : IHandleMessages<ClientOrderReceived>
{
    static ILog log = LogManager.GetLogger<OrderHandler>();

    public Task Handle(ClientOrderReceived message, IMessageHandlerContext context)
    {
        log.Info($"Handling {nameof(ClientOrderReceived)} with ID {message.OrderId}");
        return Task.CompletedTask;
    }
}