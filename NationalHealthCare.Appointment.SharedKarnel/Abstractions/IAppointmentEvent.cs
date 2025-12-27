using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NationalHealthCare.Appointment.SharedKarnel.Abstractions;

public interface IAppointmentEvent
{
}

public class AppointmentCreatedEvent : IAppointmentEvent
{
    public Guid AppointmentId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public string PatientName { get; set; }
    public string DoctorName { get; set; }
    public string ReasonForVisit { get; set; }

}

