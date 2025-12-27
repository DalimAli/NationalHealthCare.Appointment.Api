using NationalHealthCare.Appointment.SharedKarnel.Abstractions;
using RabbitMQ.Client;
using SharedKernel.Abstractions;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NationalHealthCare.Appointment.Infrastructure.Services;

public class RabbitMqServiceBus : IServiceBus
{

    string connectionString = "amqps://iqpehepr:dk0_-GdhN5eQA-R4w583zNmx5LtO-i_3@fuji.lmq.cloudamqp.com/iqpehepr";
    string QueueName = "iron-man-appointment-queue";
    string exchangeName = "iron-man-appointment-exchange";

    public RabbitMqServiceBus()
    {


    }

    string binding = "direct-bind";
    public async Task PublishAsync(ICommand messege,CancellationToken cancellationToken = default)
    {
        var factory = new ConnectionFactory
        {
            Uri = new Uri(connectionString)
        };

        await using var connection = await factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(
            queue: QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var json = JsonSerializer.Serialize(messege);
        var body = Encoding.UTF8.GetBytes(json);


        var properties = new BasicProperties
        {
            Persistent = true   // ensures message durability
        };

        await channel.ExchangeDeclareAsync(
                exchange: exchangeName,
                type: ExchangeType.Direct,
                durable: true,
                autoDelete: false);

        await channel.QueueBindAsync(
                queue: QueueName,
                exchange: exchangeName,
                routingKey: QueueName);


        await channel.BasicPublishAsync(
            exchange: exchangeName,
            routingKey: QueueName,
            mandatory: true,
            basicProperties: properties,
            body: body);



    }


    public Task SendAsync<T>(T message, CancellationToken cancellationToken = default) where T : class
    {
        throw new NotImplementedException();
    }

    public Task SubscribeAsync<T>(Func<T, CancellationToken, Task> handler, CancellationToken cancellationToken = default) where T : class
    {
        throw new NotImplementedException();
    }
}
