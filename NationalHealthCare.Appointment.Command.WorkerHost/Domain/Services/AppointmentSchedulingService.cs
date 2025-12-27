using NationalHealthCare.Appointment.Command.WorkerHost.Domain.Aggregates;
using NationalHealthCare.Appointment.Command.WorkerHost.Domain.Repositories;
using NationalHealthCare.Appointment.Command.WorkerHost.Domain.ValueObjects;

namespace NationalHealthCare.Appointment.Command.WorkerHost.Domain.Services;

public class AppointmentSchedulingService
{
    private readonly IAppointmentScheduleRepository _repository;

    public AppointmentSchedulingService(IAppointmentScheduleRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }

    public async Task<Entities.Appointment> ScheduleAppointmentAsync(
        DateTime appointmentDate,
        PatientInfo patient,
        DoctorInfo doctor,
        TimeSlot timeSlot,
        string reasonForVisit,
        CancellationToken cancellationToken = default)
    {
        var scheduleDate = appointmentDate.Date;
        var schedule = await _repository.GetByDateAsync(scheduleDate, cancellationToken);

        if (schedule == null)
        {
            schedule = new AppointmentSchedule(scheduleDate);
        }

        var appointment = schedule.CreateAppointment(patient, doctor, timeSlot, reasonForVisit);

        if (schedule.GetTotalAppointmentsCount() == 1)
        {
            await _repository.AddAsync(schedule, cancellationToken);
        }
        else
        {
            await _repository.UpdateAsync(schedule, cancellationToken);
        }

        return appointment;
    }

    public async Task<IEnumerable<TimeSlot>> GetAvailableSlotsAsync(
        DateTime date,
        DoctorInfo doctor,
        TimeSpan businessStart,
        TimeSpan businessEnd,
        int slotDurationMinutes = 30,
        CancellationToken cancellationToken = default)
    {
        var schedule = await _repository.GetByDateAsync(date.Date, cancellationToken);

        if (schedule == null)
        {
            schedule = new AppointmentSchedule(date.Date);
        }

        return schedule.GetAvailableTimeSlots(doctor, businessStart, businessEnd, slotDurationMinutes);
    }

    public async Task RescheduleAppointmentAsync(
        DateTime currentDate,
        Guid appointmentId,
        TimeSlot newTimeSlot,
        CancellationToken cancellationToken = default)
    {
        var schedule = await _repository.GetByDateAsync(currentDate.Date, cancellationToken);

        if (schedule == null)
            throw new InvalidOperationException("Schedule not found");

        schedule.RescheduleAppointment(appointmentId, newTimeSlot);
        await _repository.UpdateAsync(schedule, cancellationToken);
    }

    public async Task CancelAppointmentAsync(
        DateTime appointmentDate,
        Guid appointmentId,
        string reason = null,
        CancellationToken cancellationToken = default)
    {
        var schedule = await _repository.GetByDateAsync(appointmentDate.Date, cancellationToken);

        if (schedule == null)
            throw new InvalidOperationException("Schedule not found");

        schedule.CancelAppointment(appointmentId, reason);
        await _repository.UpdateAsync(schedule, cancellationToken);
    }
}
