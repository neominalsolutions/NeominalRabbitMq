using System;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MessageBus;
using MessageBus.Messages;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace NeominalRabbitMq.Subscriber.BackgroundServices
{
    public class MessageSubscriberBackgroundService: Microsoft.Extensions.Hosting.BackgroundService
    {

        private IConnection? _connection;
        private IModel? _channel;
        private IServiceProvider? _serviceProvider;


        /*
        public MessageSubscriberBackgroundService()
        {
            
        }

        */


       
        public MessageSubscriberBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

       


        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory();
            factory.Uri = new Uri("amqps://qosgqhkq:b-jNboVWOOVgxSzmWKL36oCE7qLX51vg@sparrow.rmq.cloudamqp.com/qosgqhkq");


            _connection = factory.CreateConnection(); // connection açtık

            _channel = _connection.CreateModel();

            _channel.BasicQos(0, 1, false); 


            var consumer = new EventingBasicConsumer(_channel);

            _channel.BasicConsume("test-queue", false, consumer);

            // mesaj iletildiğinde çalışacak olan event.
            consumer.Received += (object? sender, BasicDeliverEventArgs e) =>
            {

                if(!string.IsNullOrEmpty(e.BasicProperties.Type))
                {

                    Assembly assembly = typeof(BaseMessage).Assembly;
                    Type? @type = assembly.GetType(e.BasicProperties.Type);
            

                    // e.BasicProperties.CorrelationId

                    var message = Encoding.UTF8.GetString(e.Body.ToArray());

                    // buradaki kodu gelen tipe göre çevirmek lazım.

                    //var @eventMessage = JsonConvert.DeserializeObject(message);
                    var @eventMessage = JsonConvert.DeserializeObject<OrderCreateCommand>(message);
                    //var @eventMessage = JsonConvert.DeserializeObject<OrderCreateCommand>(message);

                    //DispatchEvent(@eventMessage,@type);

                    DispatchEvent(@eventMessage);



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


        // 
       
        private  void DispatchEvent<TEvent>(TEvent @event) 
        {
            

                using (var scope = _serviceProvider.CreateScope())
                {

                var handlerType = typeof(IConsumer<>).MakeGenericType(@event.GetType());
                // bir event birden fazla Handler çalıştırabilir.
                var handlersCollectionType = typeof(IEnumerable<>).MakeGenericType(handlerType);

                try
                {
                    //c# 4.0 ile dynamic geldi.
                    //c# 4.0 ile dynamic geldi.
                    dynamic handlers = _serviceProvider.GetRequiredService(handlersCollectionType);
                    // dynamic handler da bir hatamız var.


                    if (handlers == null)
                        return;

                    foreach (var handler in handlers)
                    {
                        var dynamicEvent = @event;
                        handler.Handle(@event);
                        //handler.GetMethod("Handle").Invoke(handler, new object[] { @event });

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
