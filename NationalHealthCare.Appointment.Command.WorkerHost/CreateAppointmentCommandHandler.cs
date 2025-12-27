using NationalHealthCare.Appointment.SharedKarnel;
using NationalHealthCare.Appointment.SharedKarnel.Abstractions;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace NationalHealthCare.Appointment.Command.WorkerHost;


public class CreateAppointmentCommandHandler : ICommandHandler<CreateAppointmentCommand>
{

    public async Task HandleAsync(CreateAppointmentCommand command, CancellationToken cancellationToken = default)
    {
        //await consumer.StartConsumingAsync(async message =>
        //{
        //    // Deserialize your command
        //    var command = JsonSerializer.Deserialize<AppointmentRequest>(message);

        Console.WriteLine($"Received appointment for: {command?.PatientName} at {command?.AppointmentDate}");

        //    // TODO: process message (e.g., save to DB)
        //}, cts.Token);
    }

}

