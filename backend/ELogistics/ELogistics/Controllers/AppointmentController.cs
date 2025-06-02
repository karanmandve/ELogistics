using core.App.Appointment.Command;
using core.App.Appointment.Query;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EHR_Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AppointmentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("add-appointment")]
        public async Task<IActionResult> AddAppointment(domain.ModelDto.Appointment.AppointmentDto model)
        {
            var result = await _mediator.Send(new AddAppointmentCommand { Appointment = model });
            if (!result.IsSuccess)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpGet("get-appointment-by-patient/{patientId}")]
        public async Task<IActionResult> GetAppointmentByPatient(int patientId)
        {
            var result = await _mediator.Send(new GetAppointmentByPatientQuery { PatientId = patientId });
            return Ok(result);
        }


        [HttpGet("get-appointment-by-provider/{providerId}")]
        public async Task<IActionResult> GetAppointmentByProvider(int providerId)
        {
            var result = await _mediator.Send(new GetAppointmentByProviderQuery { ProviderId = providerId });
            return Ok(result);
        }

        [HttpPut("update-appointment")]
        public async Task<IActionResult> UpdateAppointment(domain.ModelDto.Appointment.UpdateAppointmentDto model)
        {
            var result = await _mediator.Send(new UpdateAppointmentCommand { Appointment = model });
            if (!result.IsSuccess)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpDelete("cancel-appointment/{appointmentId}")]
        public async Task<IActionResult> CanceledAppointment(int appointmentId)
        {
            var result = await _mediator.Send(new CanceledAppointmentCommand { AppointmentId = appointmentId });
            if (!result.IsSuccess)
            {
                return NotFound(result);
            }
            return Ok(result);
        }



    }
}
