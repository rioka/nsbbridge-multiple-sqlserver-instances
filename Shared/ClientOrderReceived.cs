using System;
using NServiceBus;

namespace Shared;

public class ClientOrderReceived :
    IEvent
{
    public Guid OrderId { get; set; }
}