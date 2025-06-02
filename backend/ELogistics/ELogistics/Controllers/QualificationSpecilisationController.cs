using core.App.QualificationSpecilisation.Query;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EHR_Application.Controllers
{
    public class QualificationSpecilisationController : Controller
    {
        private readonly IMediator _mediator;
        public QualificationSpecilisationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("get-all-qualification")]
        public async Task<IActionResult> GetAllQualification()
        {
            var allQualification = await _mediator.Send(new GetAllQualificationQuery());
            return Ok(allQualification);
        }

        [HttpGet("get-all-specialisation")]
        public async Task<IActionResult> GetAllSpecialisation()
        {
            var allSpecialisation = await _mediator.Send(new GetAllSpecialisationQuery());
            return Ok(allSpecialisation);
        }

    }
}
