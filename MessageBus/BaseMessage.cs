namespace MessageBus;

public class BaseMessage
{
  public Guid Id { get; set; } = Guid.NewGuid();
}