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
    // durable önemli fiziki olarak mesaj kaybolmasın persistant yaptık. false in-memory
    // exclusive burada oluşan kanal üzerinden sadece erişebilirim fakat subscriber farklı kanaldan bağlantı kurulacaktır.
    // işlem bitince kuyruğu otomatik silmesin. en son kalan subscriber da işini bittikten sonra kuyruğu siler.
    channel.QueueDeclare("test-queue",durable:true, exclusive:false,autoDelete:false);
   
    


    Enumerable.Range(1, 10).ToList().ForEach(x =>
     {
         string message = $"Test-{x}"; // byte[] olarak verileri gönderebiliryoruz bu sebeple istediğimiz herşeyi gönderebiliriz.
         var messageBody = Encoding.UTF8.GetBytes(message);
         

         Thread.Sleep(1000);
         channel.BasicPublish(string.Empty, "test-queue", null, messageBody); // exchange kullanmadığımız işleme default exchange denir.


         Console.WriteLine($"Mesaj gönderildi {x}");

     });



    Console.ReadKey();

}


Console.WriteLine("Hello, World!");
