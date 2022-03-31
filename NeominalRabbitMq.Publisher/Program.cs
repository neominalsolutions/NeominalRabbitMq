// See https://aka.ms/new-console-template for more information


using NeominalRabbitMq.Publisher.Logs;
using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory();
factory.Uri = new Uri("amqps://qosgqhkq:b-jNboVWOOVgxSzmWKL36oCE7qLX51vg@sparrow.rmq.cloudamqp.com/qosgqhkq");

// Direct Exchange route parametresine göre ilgili kuyruğa route ile gönderir.
// Loglama işlemini yaparken critical-route, error-route, warning-route, info-route olarak nitelendirelim. Bu yapı için kullanımı doğru olur.Bir Logger Consumer tanımlayıp. Loglamalaroı asenkron olarak direct exchange ile tanımlayıp kibanaya merkezi bir yerden ilgili kuyruklamalar ile yapabiliriz.

// Her bir log durumu için hem bir kuyruk hemde bir rouyting key oluştururuz.
// Loglama işlemlerinde queue oluşturma işlemini publisher üzerinden gerçekleştirdik.

using (var cnn = factory.CreateConnection()) // connection açtık
{
    var channel = cnn.CreateModel(); // kanal açtık

    channel.ExchangeDeclare("logs-direct", durable: true, type: ExchangeType.Direct); // durable fiziksel olarak kaydolsun


    // he biri için ayrı queue oluşturduk.
    Enum.GetNames(typeof(LogLevel)).ToList().ForEach(x =>
    {
        var routeKey = $"route-{x}"; // yönlendireceğimiz route yazıyoruz.

        Console.WriteLine("routeKey" + routeKey);
        var queueName = $"direct-queue-{x}";
        channel.QueueDeclare(queueName,true,false,false);
        channel.QueueBind(queueName, exchange: "logs-direct", routeKey, null); // hangi route yönlendireceğimiz

    });


    Enumerable.Range(1, 50).ToList().ForEach(x =>
     {
         

         LogLevel logLevel = (LogLevel)new Random().Next(0,4);


         string message = $"Log Level: {x}: {logLevel}"; // byte[] olarak verileri gönderebiliryoruz bu sebeple istediğimiz herşeyi gönderebiliriz.
         var messageBody = Encoding.UTF8.GetBytes(message);

         var routeKey = $"route-{logLevel.ToString()}"; // yönlendireceğimiz route yazıyoruz.

         Console.WriteLine("routeKey" + routeKey);

         channel.BasicPublish("logs-direct", routeKey, null, messageBody); // hiç bir kuyruk bağlı değil.

         // exchange hangi kuyruklar bağlı olup olmadığını exchangebind ile tanımlıyoruz.
         Console.WriteLine($"Log gönderildi {message}");

     });

  

   
}


Console.WriteLine("Hello, World!");
Console.ReadKey();