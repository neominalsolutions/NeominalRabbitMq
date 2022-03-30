// See https://aka.ms/new-console-template for more information


using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory();
factory.Uri = new Uri("amqps://qosgqhkq:b-jNboVWOOVgxSzmWKL36oCE7qLX51vg@sparrow.rmq.cloudamqp.com/qosgqhkq");


using (var cnn = factory.CreateConnection()) // connection açtık
{
    var channel = cnn.CreateModel(); // kanal açtık
    // durable önemli fiziki olarak mesaj kaybolmasın persistant yaptık. false in-memory
    // exclusive burada oluşan kanal üzerinden sadece erişebilirim fakat subscriber farklı kanaldan bağlantı kurulacaktır.
    // işlem bitince kuyruğu otomatik silmesin. en son kalan subscriber da işini bittikten sonra kuyruğu siler.
    channel.QueueDeclare("test-queue",durable:true, exclusive:false,autoDelete:false);
    string message = "Hello Word"; // byte[] olarak verileri gönderebiliryoruz bu sebeple istediğimiz herşeyi gönderebiliriz.
    var messageBody = Encoding.UTF8.GetBytes(message);
    channel.BasicPublish(string.Empty, "test-queue", null, messageBody); // exchange kullanmadığımız işleme default exchange denir.

    Console.WriteLine("Mesaj gönderildi");
    Console.ReadKey();

}


Console.WriteLine("Hello, World!");
