using MessageBus;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Reflection;
using System.Text;

namespace NeominalRabbitMq.Subscriber.BackgroundServices
{
  public class MessageSubscriberBackgroundService : Microsoft.Extensions.Hosting.BackgroundService
  {
    private IConnection? _connection;
    private IModel? _channel;
    private IServiceProvider? _serviceProvider;

    public MessageSubscriberBackgroundService(IServiceProvider serviceProvider)
    {
      _serviceProvider = serviceProvider;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
      var factory = new ConnectionFactory();
      factory.Uri = new Uri("amqp://guest:guest@localhost:5672//");

      _connection = factory.CreateConnection(); // connection açtık

      _channel = _connection.CreateModel();

      _channel.BasicQos(0, 1, false);

      var consumer = new EventingBasicConsumer(_channel);

      _channel.BasicConsume("test-queue", false, consumer);

      // mesaj iletildiğinde çalışacak olan event.
      consumer.Received += (object? sender, BasicDeliverEventArgs e) =>
      {
        if (!string.IsNullOrEmpty(e.BasicProperties.Type))
        {
          Assembly assembly = typeof(BaseMessage).Assembly;
          Type? messageType = assembly.GetType(e.BasicProperties.Type);

          var message = Encoding.UTF8.GetString(e.Body.ToArray());
          var messageInstance = JsonConvert.DeserializeObject(message, messageType);

          DispatchEvent(messageInstance, messageType);
          Console.Write($"message listening...{message}");


          _channel.BasicAck(e.DeliveryTag, false);
        }
      };

      return Task.CompletedTask;
    }
    
    public override void Dispose()
    {
      _channel.Close();
      _connection.Close();
      base.Dispose();
    }

    private void DispatchEvent(object messageInstance, Type messageType)
    {
      using (var scope = _serviceProvider.CreateScope())
      {
        var handlerType = typeof(IConsumer<>).MakeGenericType(messageType);
        // bir event birden fazla Handler çalıştırabilir.
        var handlersCollectionType = typeof(IEnumerable<>).MakeGenericType(handlerType);

        try
        {
          dynamic handlers = _serviceProvider.GetRequiredService(handlersCollectionType);

          if (handlers == null)
            return;

          foreach (var handler in handlers)
          {
            object? handlerInstance = _serviceProvider.GetService(handlerType);

            var method = handlerType.GetMethod("Handle");
            method?.Invoke(handlerInstance, new object[] { messageInstance });
          }
        }
        catch (RuntimeBinderException)
        {
          throw new RuntimeBinderException("Dynamic tipine çeviriken bir hata oluştu");
        }
      }
    }
  }
}