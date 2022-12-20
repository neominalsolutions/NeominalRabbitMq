using MessageBus;
using MessageBus.Messages;

namespace NeominalRabbitMq.Subscriber.Consumers
{
  public class Test1Consumer : IConsumer<OrderCreateCommand>
  {
    public Test1Consumer()
    {
    }

    public void Handle(OrderCreateCommand message)
    {
    }
  }
}