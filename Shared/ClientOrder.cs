using System;
using NServiceBus;

namespace Shared;

public class ClientOrder :
    IMessage
{
    public Guid OrderId { get; set; }
}