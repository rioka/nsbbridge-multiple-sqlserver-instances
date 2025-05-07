using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Logging;
using Shared;

namespace Receiver;

public class OrderHandler : IHandleMessages<ClientOrder>
{
    static ILog log = LogManager.GetLogger<OrderHandler>();

    #region Reply

    public async Task Handle(ClientOrder message, IMessageHandlerContext context)
    {
        log.Info($"Handling {nameof(ClientOrder)} with ID {message.OrderId}");
        var clientOrderAccepted = new ClientOrderResponse
        {
            OrderId = message.OrderId
        };
        await context.Reply(clientOrderAccepted);
        await context.Publish(new ClientOrderReceived()
        {
            OrderId = message.OrderId
        });
    }

    #endregion
}