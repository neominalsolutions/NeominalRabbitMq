using System;
using MessageBus;
using MessageBus.Messages;


namespace NeominalRabbitMq.Subscriber.Consumers
{
	public class MyTestConsumer:IConsumer<OrderCreateCommand>
	{
		public MyTestConsumer()
		{
		}

        public void Handle(OrderCreateCommand message)
        {
           
        }
    }
}

