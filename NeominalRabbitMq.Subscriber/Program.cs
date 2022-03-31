// See https://aka.ms/new-console-template for more information


using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;






    var factory = new ConnectionFactory();
    factory.Uri = new Uri("amqps://qosgqhkq:b-jNboVWOOVgxSzmWKL36oCE7qLX51vg@sparrow.rmq.cloudamqp.com/qosgqhkq");


    using var cnn = factory.CreateConnection(); // connection açtık

    // random kuyruk oluşturma



    var channel = cnn.CreateModel();

// var randomQueueName = channel.QueueDeclare().QueueName; // random kuyruk

   var QueueName = "direct-queue-Critical"; // kalıcı kuyruğu random veremeyiz.

 

    channel.QueueBind(QueueName, exchange: "logs-direct", "route-Critical", null); 

    channel.BasicQos(0,1 ,false); // 1 er 1 er dağıtım local dağıtım.

    var consumer = new EventingBasicConsumer(channel); // bu kanal üzerinden consumer oluşturduk.

    channel.BasicConsume(QueueName, false, consumer); // autoAct false yaparsak mesajı kuyruktan silmek için biz bir işlem yapacağız.                                                     

    // mesaj iletildiğinde çalışacak olan event.
    consumer.Received += (object? sender, BasicDeliverEventArgs e) =>
    {
       
        var message = Encoding.UTF8.GetString(e.Body.ToArray()); // byte[] olan mesajı oku
        Console.WriteLine("Gelen Mesaj :" + message);
        Thread.Sleep(1000);

        channel.BasicAck(e.DeliveryTag, false); // kendimiz tek tek sileriz. rabbitmq haberdar ederiz.
        // Direkt olarak kuyruğa gönderirsek bu default exchange denk gelir.


        //channel.BasicAck(e.DeliveryTag,multiple:false); // multiple true dersek o anda ramde işlemiş mesajlarıda rabbitmq bildirir. false dersek tek tek iletildiğinde o zaman rabbitmq haberdar edilir ve silinmiş olur.  
    };

Console.ReadLine();



//Console.WriteLine("Hello, World!");
//Console.ReadKey();
