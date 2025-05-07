using System;
using NServiceBus;

namespace Shared;

public class ClientOrderResponse :
    IMessage
{
    public Guid OrderId { get; set; }
}