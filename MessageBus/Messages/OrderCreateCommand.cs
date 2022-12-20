using Newtonsoft.Json;

namespace MessageBus.Messages
{
  public class OrderCreateCommand : BaseMessage
  {
    [JsonConstructor]
    public OrderCreateCommand(string orderId)
    {
      OrderId = orderId;
    }

    public string OrderId { get; private set; }
  }
}