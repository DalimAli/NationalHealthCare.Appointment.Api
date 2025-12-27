namespace NationalHealthCare.Appointment.Api.Models;

public class AppointmentRequest
{
    public string PatientName { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string DoctorName { get; set; }
    public string ReasonForVisit { get; set; }
}