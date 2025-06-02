using core.App.SoapNotes.Command;
using core.App.SoapNotes.Query;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EHR_Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SoapNoteController : ControllerBase
    {
        private readonly IMediator _mediator;
        public SoapNoteController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpPost("add-soapnote")]
        public async Task<IActionResult> AddSoapNote(domain.ModelDto.SoapNote.SoapNoteDto model)
        {
            var result = await _mediator.Send(new AddSoapNoteCommand { SoapNote = model });
            if (!result.IsSuccess)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

        [HttpGet("get-soap-note-by-appointmentId/{appointmentId}")]
        public async Task<IActionResult> GetSoapNoteByAppointmentId(int appointmentId)
        {
            var result = await _mediator.Send(new GetSoapNoteByAppointmentIdQuery { AppointmentId = appointmentId });
            if (result == null)
            {
                return NotFound(result);
            }
            return Ok(result);
        }

    }
}
