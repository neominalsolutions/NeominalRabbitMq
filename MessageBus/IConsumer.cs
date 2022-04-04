using System;
namespace MessageBus
{
	
	public interface IConsumer<TMessage> where TMessage : BaseMessage
	{
			void Handle(TMessage message);
	}
	
}

