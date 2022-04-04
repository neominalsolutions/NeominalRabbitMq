// See https://aka.ms/new-console-template for more information


using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NeominalRabbitMq.Subscriber.BackgroundServices;
using NeominalRabbitMq.Subscriber.Consumers;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Reflection;
using System.Text;
using System.Threading;





public static class Program
{
    public static async Task Main(string[] args)
    {
        await CreateHostBuilder(args).RunConsoleAsync();
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseConsoleLifetime()
            .ConfigureServices((hostContext, services) =>
            {

                Assembly.GetExecutingAssembly()
    .GetTypes()
    .Where(a => a.Name.EndsWith("Consumer") && !a.IsAbstract && !a.IsInterface)
    .Select(a => new { assignedType = a, serviceTypes = a.GetInterfaces().ToList() })
    .ToList()
    .ForEach(typesToRegister =>
    {
        typesToRegister.serviceTypes.ForEach(typeToRegister => services.AddScoped(typeToRegister, typesToRegister.assignedType));
    });

                // services.AddScoped<MyTestConsumer>();

                services.AddHostedService<MessageSubscriberBackgroundService>();
                services.AddSingleton(Console.Out);
               
            });
}

