using App.Core.Apps.Country.Query;
using App.Core.Apps.State.Query;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EHR_Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryStateController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CountryStateController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("get-all-country")]
        public async Task<IActionResult> GetAllCountry()
        {
            var allCountry = await _mediator.Send(new GetAllCountryQuery());
            return Ok(allCountry);
        }

        [HttpGet("state-by-countryId/{id}")]
        public async Task<IActionResult> GetAllStateByCountryId(int id)
        {
            var allStateByCountryId = await _mediator.Send(new GetAllStateByCountryIdQuery { CountryId = id });
            return Ok(allStateByCountryId);
        }

        [HttpGet("/all-state")]
        public async Task<IActionResult> GetAllState()
        {
            var allState = await _mediator.Send(new GetAllState());
            return Ok(allState);
        }
    }
}
