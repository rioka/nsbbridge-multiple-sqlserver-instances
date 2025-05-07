using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;
using Shared;

namespace Sender;

public class ClientOrderResponseHandler :
    IHandleMessages<ClientOrderResponse>
{
    static ILog log = LogManager.GetLogger<ClientOrderResponseHandler>();

    public Task Handle(ClientOrderResponse message, IMessageHandlerContext context)
    {
        log.Info($"Received {nameof(ClientOrderResponse)} for ID {message.OrderId}");
        return Task.CompletedTask;
    }
}