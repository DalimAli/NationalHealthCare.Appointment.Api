using NationalHealthCare.Appointment.Command.WorkerHost.Domain.Entities;
using NationalHealthCare.Appointment.Command.WorkerHost.Domain.ValueObjects;
using AppointmentEntity = NationalHealthCare.Appointment.Command.WorkerHost.Domain.Entities.Appointment;

namespace NationalHealthCare.Appointment.Command.WorkerHost.Domain.Aggregates;

public class AppointmentSchedule
{
    private readonly List<AppointmentEntity> _appointments;

    public Guid Id { get; private set; }
    public DateTime ScheduleDate { get; private set; }
    public IReadOnlyCollection<AppointmentEntity> Appointments => _appointments.AsReadOnly();
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }

    private AppointmentSchedule()
    {
        _appointments = new List<AppointmentEntity>();
    }

    public AppointmentSchedule(DateTime scheduleDate)
    {
        if (scheduleDate < DateTime.UtcNow.Date)
            throw new ArgumentException("Schedule date cannot be in the past", nameof(scheduleDate));

        Id = Guid.NewGuid();
        ScheduleDate = scheduleDate.Date;
        _appointments = new List<AppointmentEntity>();
        CreatedAt = DateTime.UtcNow;
    }

    public AppointmentEntity CreateAppointment(
        PatientInfo patient,
        DoctorInfo doctor,
        TimeSlot timeSlot,
        string reasonForVisit)
    {
        if (patient == null)
            throw new ArgumentNullException(nameof(patient));

        if (doctor == null)
            throw new ArgumentNullException(nameof(doctor));

        if (timeSlot == null)
            throw new ArgumentNullException(nameof(timeSlot));

        ValidateTimeSlot(timeSlot);
        ValidateNoConflicts(doctor, timeSlot);
        ValidatePatientNotDoubleBooked(patient, timeSlot);

        var appointment = new AppointmentEntity(patient, doctor, timeSlot, reasonForVisit);
        _appointments.Add(appointment);
        LastModifiedAt = DateTime.UtcNow;

        return appointment;
    }

    public void CancelAppointment(Guid appointmentId, string reason = null)
    {
        var appointment = _appointments.FirstOrDefault(a => a.Id == appointmentId);
        if (appointment == null)
            throw new InvalidOperationException($"Appointment with ID {appointmentId} not found");

        appointment.Cancel(reason);
        LastModifiedAt = DateTime.UtcNow;
    }

    public void RescheduleAppointment(Guid appointmentId, TimeSlot newTimeSlot)
    {
        var appointment = _appointments.FirstOrDefault(a => a.Id == appointmentId);
        if (appointment == null)
            throw new InvalidOperationException($"Appointment with ID {appointmentId} not found");

        ValidateTimeSlot(newTimeSlot);
        ValidateNoConflicts(appointment.Doctor, newTimeSlot, appointmentId);
        ValidatePatientNotDoubleBooked(appointment.Patient, newTimeSlot, appointmentId);

        appointment.Cancel("Rescheduled to new time slot");
        var rescheduledAppointment = new AppointmentEntity(
            appointment.Patient,
            appointment.Doctor,
            newTimeSlot,
            appointment.ReasonForVisit);

        _appointments.Add(rescheduledAppointment);
        LastModifiedAt = DateTime.UtcNow;
    }

    public IEnumerable<AppointmentEntity> GetAppointmentsByDoctor(string doctorId)
    {
        return _appointments.Where(a => a.Doctor.DoctorId == doctorId);
    }

    public IEnumerable<AppointmentEntity> GetAppointmentsByPatient(string patientId)
    {
        return _appointments.Where(a => a.Patient.PatientId == patientId);
    }

    public IEnumerable<TimeSlot> GetAvailableTimeSlots(
        DoctorInfo doctor,
        TimeSpan businessStart,
        TimeSpan businessEnd,
        int slotDurationMinutes = 30)
    {
        var availableSlots = new List<TimeSlot>();
        var currentTime = ScheduleDate.Add(businessStart);
        var endTime = ScheduleDate.Add(businessEnd);

        while (currentTime.AddMinutes(slotDurationMinutes) <= endTime)
        {
            var potentialSlot = new TimeSlot(currentTime, slotDurationMinutes);
            
            var hasConflict = _appointments
                .Where(a => a.Doctor.DoctorId == doctor.DoctorId)
                .Any(a => a.TimeSlot.OverlapsWith(potentialSlot));

            if (!hasConflict)
            {
                availableSlots.Add(potentialSlot);
            }

            currentTime = currentTime.AddMinutes(slotDurationMinutes);
        }

        return availableSlots;
    }

    public int GetTotalAppointmentsCount()
    {
        return _appointments.Count;
    }

    public int GetAppointmentCountByStatus(Enums.AppointmentStatus status)
    {
        return _appointments.Count(a => a.Status == status);
    }

    private void ValidateTimeSlot(TimeSlot timeSlot)
    {
        if (timeSlot.StartTime.Date != ScheduleDate)
            throw new InvalidOperationException("Appointment time slot must be on the schedule date");
    }

    private void ValidateNoConflicts(DoctorInfo doctor, TimeSlot timeSlot, Guid? excludeAppointmentId = null)
    {
        var hasConflict = _appointments
            .Where(a => a.Doctor.DoctorId == doctor.DoctorId)
            .Where(a => !excludeAppointmentId.HasValue || a.Id != excludeAppointmentId.Value)
            .Where(a => a.Status != Enums.AppointmentStatus.Cancelled && a.Status != Enums.AppointmentStatus.NoShow)
            .Any(a => a.TimeSlot.OverlapsWith(timeSlot));

        if (hasConflict)
            throw new InvalidOperationException($"Doctor {doctor.DoctorName} already has an appointment during this time slot");
    }

    private void ValidatePatientNotDoubleBooked(PatientInfo patient, TimeSlot timeSlot, Guid? excludeAppointmentId = null)
    {
        var hasConflict = _appointments
            .Where(a => a.Patient.PatientId == patient.PatientId)
            .Where(a => !excludeAppointmentId.HasValue || a.Id != excludeAppointmentId.Value)
            .Where(a => a.Status != Enums.AppointmentStatus.Cancelled && a.Status != Enums.AppointmentStatus.NoShow)
            .Any(a => a.TimeSlot.OverlapsWith(timeSlot));

        if (hasConflict)
            throw new InvalidOperationException($"Patient {patient.PatientName} already has an appointment during this time slot");
    }
}
