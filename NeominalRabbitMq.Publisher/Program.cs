// See https://aka.ms/new-console-template for more information


using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory();
factory.Uri = new Uri("amqps://qosgqhkq:b-jNboVWOOVgxSzmWKL36oCE7qLX51vg@sparrow.rmq.cloudamqp.com/qosgqhkq");

// publisher mesajı gönderdiğinde subscriberlar tarafından doğru işlendiğinde ilgili mesajı silme işlemi yapıp. Daha güvenli bir şekilde mesajları işleyebiliriz.
// Kuyrukta 10 adet mesaj var tek bir seferde kaçar kaçar mesaj göndereceğiz kısmını halledeceğiz. Tek seferde 10'ar mesaj alsın diyebiliriz. Her subscriber tek seferde 10 mesaj gönder dilebiliriz. Mesaj işlenme süresi uzun ise her subscriber 1 yada 2 tane gönderilmesi uygun olacaktır.

using (var cnn = factory.CreateConnection()) // connection açtık
{
    var channel = cnn.CreateModel(); // kanal açtık

    channel.ExchangeDeclare("logs-fanout", durable: true, type: ExchangeType.Fanout); // durable fiziksel olarak kaydolsun



    string message = $"Test Fanout"; // byte[] olarak verileri gönderebiliryoruz bu sebeple istediğimiz herşeyi gönderebiliriz.
    var messageBody = Encoding.UTF8.GetBytes(message);


    channel.BasicPublish("logs-fanout", "", null, messageBody); // hiç bir kuyruk bağlı değil.

    // exchange hangi kuyruklar bağlı olup olmadığını exchangebind ile tanımlıyoruz.
    Console.WriteLine($"Mesaj gönderildi {message}");
}


Console.WriteLine("Hello, World!");
Console.ReadKey();