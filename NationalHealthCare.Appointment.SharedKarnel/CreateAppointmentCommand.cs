

using NationalHealthCare.Appointment.SharedKarnel.Abstractions;

namespace NationalHealthCare.Appointment.SharedKarnel
{
    public class CreateAppointmentCommand: ICommand
    {
        public string PatientName { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string DoctorName { get; set; }
        public string ReasonForVisit { get; set; }
    }
}
