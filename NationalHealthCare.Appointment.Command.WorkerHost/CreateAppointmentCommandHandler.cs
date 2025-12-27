using NationalHealthCare.Appointment.SharedKarnel;
using NationalHealthCare.Appointment.SharedKarnel.Abstractions;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace NationalHealthCare.Appointment.Command.WorkerHost;


public class CreateAppointmentCommandHandler : ICommandHandler
{

    public async Task HandleAsync(ICommand command, CancellationToken cancellationToken = default)
    {
        CreateAppointmentCommand createAppointmentCommand = command as CreateAppointmentCommand;

        //await consumer.StartConsumingAsync(async message =>
        //{
        //    // Deserialize your command
        //    var command = JsonSerializer.Deserialize<AppointmentRequest>(message);

        Console.WriteLine($"Received appointment for: {createAppointmentCommand?.PatientName} at {createAppointmentCommand?.AppointmentDate}");

        //    // TODO: process message (e.g., save to DB)
        //}, cts.Token);
    }

}

