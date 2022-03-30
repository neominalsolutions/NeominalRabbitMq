// See https://aka.ms/new-console-template for more information


using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

var factory = new ConnectionFactory();
factory.Uri = new Uri("amqps://qosgqhkq:b-jNboVWOOVgxSzmWKL36oCE7qLX51vg@sparrow.rmq.cloudamqp.com/qosgqhkq");


using (var cnn = factory.CreateConnection()) // connection açtık
{
    var channel = cnn.CreateModel(); // kanal açtık
    // durable önemli fiziki olarak mesaj kaybolmasın persistant yaptık. false in-memory
    // exclusive burada oluşan kanal üzerinden sadece erişebilirim fakat subscriber farklı kanaldan bağlantı kurulacaktır.
    // işlem bitince kuyruğu otomatik silmesin. en son kalan subscriber da işini bittikten sonra kuyruğu siler.
    channel.QueueDeclare("test-queue", durable: true, exclusive: false, autoDelete: false); // kuyruk tanımı
    // eğer publisher bu kuruğu oluşturmuş ise bundan eminsek bunu tekrar declare etmeye gerek yok. fakat publisher bu kuyruğu oluşturmamış ise o zaman subscriber bu kuyruğu oluşturur. channel.QueueDeclare için aynı parametrelerin olmasına dikkat edelim.

    var consumer = new EventingBasicConsumer(channel); // bu kanal üzerinden consumer oluşturduk.
    channel.BasicConsume("test-queue", autoAck: true, consumer); // autoAct false yaparsak mesajı kuyruktan silmek için biz bir işlem yapacağız.                                                     

    // mesaj iletildiğinde çalışacak olan event.
    consumer.Received += (object? sender, BasicDeliverEventArgs e) =>
    {
        var message = Encoding.UTF8.GetString(e.Body.ToArray()); // byte[] olan mesajı oku
        Console.WriteLine("Gelen Mesaj :" + message);
    };
 

}


Console.WriteLine("Hello, World!");
Console.ReadKey();
