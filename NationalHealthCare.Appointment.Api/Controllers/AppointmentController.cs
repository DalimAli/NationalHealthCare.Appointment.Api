using Microsoft.AspNetCore.Mvc;
using NationalHealthCare.Appointment.Api.Models;
using NationalHealthCare.Appointment.SharedKarnel;
using NationalHealthCare.Appointment.SharedKarnel.Abstractions;

namespace NationalHealthCare.Appointment.Api.Controllers
{
    [Route("api/appointment")]
    [ApiController]
    public class AppointmentController(IServiceBus serviceBus) : ControllerBase
    {

        [HttpPost]
        [Route("schedule")]
        public async Task<IActionResult> ScheduleAppointment([FromBody] AppointmentRequest appointmentRequest)
        {
            var request = new CreateAppointmentCommand
            {
                PatientName = appointmentRequest.PatientName,
                AppointmentDate = appointmentRequest.AppointmentDate,
                DoctorName = appointmentRequest.DoctorName,
                ReasonForVisit = appointmentRequest.ReasonForVisit
            };

            await serviceBus.PublishAsync(request);
            return Ok(new { Message = "Appointment scheduled successfully", AppointmentDetails = request });
        }
    }
}
