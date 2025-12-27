using NationalHealthCare.Appointment.SharedKarnel.Abstractions;
using RabbitMQ.Client;
using SharedKernel.Abstractions;
using System.Text;

namespace NationalHealthCare.Appointment.Infrastructure.Services;

public class RabbitMqServiceBus : IServiceBus
{

    string connectionString = "amqps://iqpehepr:dk0_-GdhN5eQA-R4w583zNmx5LtO-i_3@fuji.lmq.cloudamqp.com/iqpehepr";
    string queueName = "iron-man-appointment-queue";

    public RabbitMqServiceBus()
    {


    }
    public async Task PublishAsync(ICommand message, CancellationToken cancellationToken = default) 
    {

        var factory = new ConnectionFactory
        {
            Uri = new Uri(connectionString)
        };

        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(
              queue: queueName,
              durable: true,
              exclusive: false,
              autoDelete: false,
              arguments: null);

        var body = Encoding.UTF8.GetBytes(message);

        var properties = new BasicProperties
        {
            Persistent = true
        };

        await channel.BasicPublishAsync(
            exchange: "",
            routingKey: queueName,
            mandatory: false,
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
