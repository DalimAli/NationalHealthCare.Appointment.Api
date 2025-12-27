using NationalHealthCare.Appointment.SharedKarnel.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NationalHealthCare.Appointment.SharedKarnel
{

    public class ResponseAppointmentCommand : ICommand
    {
        public string PatientName { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string DoctorName { get; set; }
        public string ReasonForVisit { get; set; }
    }
}
