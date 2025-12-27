using NationalHealthCare.Appointment.Command.WorkerHost.Domain.ValueObjects;
using NationalHealthCare.Appointment.SharedKarnel;

namespace NationalHealthCare.Appointment.Command.WorkerHost.Domain.Factories;

public static class AppointmentFactory
{
    public static (PatientInfo patient, DoctorInfo doctor, TimeSlot timeSlot, string reason) CreateFromCommand(
        CreateAppointmentCommand command,
        int appointmentDurationMinutes = 30)
    {
        if (command == null)
            throw new ArgumentNullException(nameof(command));

        var patient = new PatientInfo(
            patientName: command.PatientName,
            patientId: Guid.NewGuid().ToString());

        var doctor = new DoctorInfo(
            doctorName: command.DoctorName,
            doctorId: Guid.NewGuid().ToString());

        var timeSlot = new TimeSlot(
            startTime: command.AppointmentDate,
            durationInMinutes: appointmentDurationMinutes);

        return (patient, doctor, timeSlot, command.ReasonForVisit);
    }
}
