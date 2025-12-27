using Microsoft.VisualBasic;
using NationalHealthCare.Appointment.SharedKarnel;
using NationalHealthCare.Appointment.SharedKarnel.Abstractions;
using NationalHealthCare.Appointment.SharedKarnel.Aggregates;
using NationalHealthCare.Appointment.SharedKarnel.Domain.Entities;
using NationalHealthCare.Appointment.SharedKarnel.Domain.ValueObjects;
using NationalHealthCare.Appointment.SharedKarnel.Repositories;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using AppointmentEntity = NationalHealthCare.Appointment.SharedKarnel.Domain.Entities.Appointment;

namespace NationalHealthCare.Appointment.Command.WorkerHost;


public class CreateAppointmentCommandHandler : ICommandHandler<CreateAppointmentCommand>
{

    //public async Task HandleAsync(ICommand command, CancellationToken cancellationToken = default)
    //{
    //    CreateAppointmentCommand createAppointmentCommand = command as CreateAppointmentCommand;

    //    //await consumer.StartConsumingAsync(async message =>
    //    //{
    //    //    // Deserialize your command
    //    //    var command = JsonSerializer.Deserialize<AppointmentRequest>(message);

    //    Console.WriteLine($"Received appointment for: {createAppointmentCommand?.PatientName} at {createAppointmentCommand?.AppointmentDate}");

    //    //    // TODO: process message (e.g., save to DB)
    //    //}, cts.Token);

    //}
    private readonly IAppointmentScheduleRepository _appointmentScheduleRepository;
    public CreateAppointmentCommandHandler(IAppointmentScheduleRepository appointmentScheduleRepository)
    {
        _appointmentScheduleRepository = appointmentScheduleRepository;
    }


    public async Task HandleAsync(CreateAppointmentCommand command, CancellationToken cancellationToken = default)
    {
        CreateAppointmentCommand createAppointmentCommand = command as CreateAppointmentCommand;

        //await consumer.StartConsumingAsync(async message =>
        //{
        //    // Deserialize your command
        //    var command = JsonSerializer.Deserialize<AppointmentRequest>(message);

        Console.WriteLine($"Received appointment for: {createAppointmentCommand?.PatientName} at {createAppointmentCommand?.AppointmentDate}");
        IAppointmentEvent appointmentEvent = null;

        var appointment = new AppointmentSchedule((DateTime.Now.AddDays(5)));
        appointment.CreateAppointment(
            new PatientInfo(createAppointmentCommand.PatientName,
                Guid.NewGuid().ToString(),
                "123-456-7890",
               ""
            ),
            new DoctorInfo(createAppointmentCommand.DoctorName,
                Guid.NewGuid().ToString(),
                "Cardiology"),
            new TimeSlot(createAppointmentCommand.AppointmentDate, 20),
            createAppointmentCommand.ReasonForVisit,
               appointmentEvent
            );

        await _appointmentScheduleRepository.AddAsync(appointment);



    }
}

