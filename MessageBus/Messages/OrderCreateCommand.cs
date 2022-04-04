using System;
namespace MessageBus.Messages
{
	public class OrderCreateCommand : BaseMessage
	{
		public OrderCreateCommand()
		{
		}

		public string OrderId { get; set; }

	}
}

