using System;
using MessageBus;
using MessageBus.Messages;

namespace NeominalRabbitMq.Subscriber.Consumers
{
	public class Test2Consumer: IConsumer<OrderCreateCommand>
	{
		public Test2Consumer()
		{
		}

        public void Handle(OrderCreateCommand message)
        {
            
        }
    }
}

