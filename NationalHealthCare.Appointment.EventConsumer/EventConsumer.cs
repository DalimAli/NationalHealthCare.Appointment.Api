using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using System.Threading;

namespace NationalHealthCare.Appointment.EventConsumer
{
    class FanoutConsumer
    {
        string connectionString = "amqps://iqpehepr:dk0_-GdhN5eQA-R4w583zNmx5LtO-i_3@fuji.lmq.cloudamqp.com/iqpehepr";
        public async Task Consume()
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri(connectionString)
            };

            await using var connection = await factory.CreateConnectionAsync();
            await using var channel = await connection.CreateChannelAsync();

            string exchangeName = "logs.fanout";

            await channel.ExchangeDeclareAsync(exchangeName, ExchangeType.Fanout, durable: true);

            // Create a queue (can be named or auto-generated)
            string queueName = "fanout-queue";

            await channel.QueueDeclareAsync(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                default);

            // Bind queue to fanout exchange
          await  channel.QueueBindAsync(
                queue: queueName,
                exchange: exchangeName,
                routingKey: "");

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (sender, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);


                    //var command = JsonSerializer.Deserialize<TResponse>(message);
                    //await commandHandler.HandleAsync(command);

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing message: {ex.Message}");
                }
            };

            await channel.BasicConsumeAsync(queue: queueName, autoAck: true, consumer: consumer);

            Console.WriteLine("👂 Waiting for messages...");
            Console.ReadLine();
        }
    }
}
