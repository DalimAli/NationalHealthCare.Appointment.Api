using Microsoft.AspNetCore.Mvc;
using NationalHealthCare.Appointment.Api.Models;

namespace NationalHealthCare.Appointment.Api.Controllers
{
    [Route("api/appointment")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {

        [HttpPost]
        [Route("schedule")]
        public IActionResult ScheduleAppointment([FromBody] AppointmentRequest request)
        {
            return Ok(new { Message = "Appointment scheduled successfully", AppointmentDetails = request });
        }
    }
}
