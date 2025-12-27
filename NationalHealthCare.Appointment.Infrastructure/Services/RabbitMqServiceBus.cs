using NationalHealthCare.Appointment.SharedKarnel;
using NationalHealthCare.Appointment.SharedKarnel.Abstractions;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SharedKernel.Abstractions;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace NationalHealthCare.Appointment.Infrastructure.Services;

public class RabbitMqServiceBus : IServiceBus
{

    string connectionString = "amqps://iqpehepr:dk0_-GdhN5eQA-R4w583zNmx5LtO-i_3@fuji.lmq.cloudamqp.com/iqpehepr";
    string queueName = "iron-man-appointment-queue";
    string exchangeName = "iron-man-appointment-exchange";

    public RabbitMqServiceBus()
    {
    }

    string binding = "direct-bind";
    public async Task PublishAsync(ICommand messege, CancellationToken cancellationToken = default)
    {
        var factory = new ConnectionFactory
        {
            Uri = new Uri(connectionString)
        };

        await using var connection = await factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var json = JsonSerializer.Serialize(messege, messege.GetType());
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
                queue: queueName,
                exchange: exchangeName,
                routingKey: queueName);


        await channel.BasicPublishAsync(
            exchange: exchangeName,
            routingKey: queueName,
            mandatory: true,
            basicProperties: properties,
            body: body);



    }

    public async Task StartConsumingAsync(ICommandHandler commandHandler, CancellationToken cancellationToken = default)
    {
        var factory = new ConnectionFactory
        {
            Uri = new Uri(connectionString)
        };

        await using var connection = await factory.CreateConnectionAsync(cancellationToken);
        await using var channel = await connection.CreateChannelAsync();

        // Ensure queue & exchange exist
        await channel.ExchangeDeclareAsync(
            exchange: exchangeName,
            type: ExchangeType.Direct,
            durable: true,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);

        await channel.QueueDeclareAsync(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: cancellationToken);

        await channel.QueueBindAsync(
            queue: queueName,
            exchange: exchangeName,
            routingKey: queueName,
            arguments: null,
            cancellationToken: cancellationToken);

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (sender, ea) =>
        {
            try
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var command = JsonSerializer.Deserialize<ICommand>(message);
                await commandHandler.HandleAsync(command);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing message: {ex.Message}");
            }
        };

        await channel.BasicConsumeAsync(
            queue: queueName,
            autoAck: false, // manual ack
            consumer: consumer,
            cancellationToken: cancellationToken);

        Console.WriteLine($"[x] Waiting for messages on queue '{queueName}'. Press Ctrl+C to exit.");

        // Keep the consumer alive until cancellation is requested
        var tcs = new TaskCompletionSource();
        using (cancellationToken.Register(() => tcs.SetResult()))
        {
            await tcs.Task;
        }
    }
}
