using NationalHealthCare.Appointment.Command.WorkerHost.Domain.Enums;
using NationalHealthCare.Appointment.Command.WorkerHost.Domain.ValueObjects;

namespace NationalHealthCare.Appointment.Command.WorkerHost.Domain.Entities;

public class Appointment
{
    public Guid Id { get; private set; }
    public PatientInfo Patient { get; private set; }
    public DoctorInfo Doctor { get; private set; }
    public TimeSlot TimeSlot { get; private set; }
    public string ReasonForVisit { get; private set; }
    public AppointmentStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastModifiedAt { get; private set; }
    public string Notes { get; private set; }

    private Appointment() { }

    public Appointment(
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

        if (string.IsNullOrWhiteSpace(reasonForVisit))
            throw new ArgumentException("Reason for visit cannot be empty", nameof(reasonForVisit));

        Id = Guid.NewGuid();
        Patient = patient;
        Doctor = doctor;
        TimeSlot = timeSlot;
        ReasonForVisit = reasonForVisit;
        Status = AppointmentStatus.Scheduled;
        CreatedAt = DateTime.UtcNow;
    }

    public void Confirm()
    {
        if (Status != AppointmentStatus.Scheduled)
            throw new InvalidOperationException("Only scheduled appointments can be confirmed");

        Status = AppointmentStatus.Confirmed;
        LastModifiedAt = DateTime.UtcNow;
    }

    public void CheckIn()
    {
        if (Status != AppointmentStatus.Confirmed && Status != AppointmentStatus.Scheduled)
            throw new InvalidOperationException("Cannot check in for this appointment");

        Status = AppointmentStatus.CheckedIn;
        LastModifiedAt = DateTime.UtcNow;
    }

    public void StartConsultation()
    {
        if (Status != AppointmentStatus.CheckedIn)
            throw new InvalidOperationException("Patient must be checked in before starting consultation");

        Status = AppointmentStatus.InProgress;
        LastModifiedAt = DateTime.UtcNow;
    }

    public void Complete(string notes = null)
    {
        if (Status != AppointmentStatus.InProgress)
            throw new InvalidOperationException("Cannot complete an appointment that is not in progress");

        Status = AppointmentStatus.Completed;
        Notes = notes;
        LastModifiedAt = DateTime.UtcNow;
    }

    public void Cancel(string reason = null)
    {
        if (Status == AppointmentStatus.Completed)
            throw new InvalidOperationException("Cannot cancel a completed appointment");

        Status = AppointmentStatus.Cancelled;
        Notes = reason;
        LastModifiedAt = DateTime.UtcNow;
    }

    public void MarkAsNoShow()
    {
        if (Status != AppointmentStatus.Scheduled && Status != AppointmentStatus.Confirmed)
            throw new InvalidOperationException("Cannot mark as no-show for this appointment");

        Status = AppointmentStatus.NoShow;
        LastModifiedAt = DateTime.UtcNow;
    }

    public void UpdateReasonForVisit(string newReason)
    {
        if (Status != AppointmentStatus.Scheduled && Status != AppointmentStatus.Confirmed)
            throw new InvalidOperationException("Cannot update reason for visit at this stage");

        if (string.IsNullOrWhiteSpace(newReason))
            throw new ArgumentException("Reason for visit cannot be empty", nameof(newReason));

        ReasonForVisit = newReason;
        LastModifiedAt = DateTime.UtcNow;
    }
}
