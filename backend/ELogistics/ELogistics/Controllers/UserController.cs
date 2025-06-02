using core.App.User.Command;
using core.App.User.Query;
using core.App.Users.Command;
using domain.ModelDto;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata.Ecma335;

namespace EHR_Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;
        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser(domain.ModelDto.RegisterDto model)
        {
            //var validator = new RegisterDtoValidator();
            //var isModeltValid = validator.Validate(model);

            //if (!isModeltValid.IsValid)
            //{
            //    var errorMessage = isModeltValid.Errors[0].ErrorMessage;
            //    return BadRequest(new { Errors = isModeltValid.Errors.Select(x => x.ErrorMessage).ToList() });
            //}

            using (var stream = model.File.OpenReadStream())
            {
                var command = new CreateUserCommand
                {
                    FileStream = stream,
                    FileName = model.File.FileName,
                    RegisterUser = model
                };

                var result = await _mediator.Send(command);

                if (!result.IsSuccess)
                {
                    return Conflict(result);
                }

                return Ok(result);
            }

        }


        [HttpGet("sendotp/{username}")]
        public async Task<IActionResult> SendOtp(string username)
        {
            var result = await _mediator.Send(new SendOtpCommand { Username = username });

            if (!result.IsSuccess)
            {
                return NotFound(result);
            }

            return Ok(result);
        }


        [HttpPost("sendotp-with-password-check")]
        public async Task<IActionResult> SendOtpWithPasswordCheck([FromBody] SendOtpWithPasswordCheckDto model)
        {
            var result = await _mediator.Send(new SendOtpWithPasswordCheckCommand { SendOtpWithPasswordCheck = model });

            if (!result.IsSuccess)
            {
                return NotFound(result);
            }

            return Ok(result);
        }



        [HttpPost("Login")]
        public async Task<IActionResult> LoginUser([FromBody] LoginDto model)
        {
            //var validator = new LoginDtoValidator();
            //var isModeltValid = validator.Validate(model);

            //if (!isModeltValid.IsValid)
            //{
            //    var errorMessage = isModeltValid.Errors[0].ErrorMessage;
            //    return BadRequest(new { Errors = isModeltValid.Errors.Select(x => x.ErrorMessage).ToList() });
            //}


            var result = await _mediator.Send(new UserLoginQuery { LoginUser = model });
            if (!result.IsSuccess)
            {
                return Unauthorized(result);
            }

            return Ok(result);
        }


        [HttpGet("forgot-password/{username}/{otp}")]
        public async Task<IActionResult> ForgotPassword(string username, string otp)
        {
            var result = await _mediator.Send(new ForgetUserPassword { Username = username, Otp = otp });
            if (!result.IsSuccess)
            {
                return Unauthorized(result);
            }
            return Ok(result);
        }

        [HttpGet("get-user-details/{username}")]
        public async Task<IActionResult> GetUserDetails(string username)
        {
            var result = await _mediator.Send(new GetUserByUsernameQuery { Username = username });
            if (!result.IsSuccess)
            {
                return NotFound(result);
            }
            return Ok(result);
        }


        [HttpGet("get-all-provider-by-specialisationId/{specialisationId}")]
        public async Task<IActionResult> GetAllProviderBySpecialisationId(int specialisationId)
        {
            var result = await _mediator.Send(new GetAllProviderBySpecialisationIdQuery { SpecialisationId = specialisationId });
            if (!result.IsSuccess)
            {
                return NotFound(result);
            }
            return Ok(result);
        }


        [HttpPut("update-patient")]
        public async Task<IActionResult> UpdatePatient(domain.ModelDto.UpdatePatientDto user)
        {
            if (user.ImageFile == null)
            {
                var command = new UpdatePatientCommand
                {
                    FileStream = null,
                    FileName = null,
                    User = user
                };

                var result = await _mediator.Send(command);
                if (!result.IsSuccess)
                {
                    return NotFound(result);
                }
                return Ok(result);
            }

            using (var stream = user.ImageFile.OpenReadStream())
            {
                var command = new UpdatePatientCommand
                {
                    FileStream = stream,
                    FileName = user.ImageFile.FileName,
                    User = user
                };

                var result = await _mediator.Send(command);
                if (!result.IsSuccess)
                {
                    return NotFound(result);
                }
                return Ok(result);
            }
        }



        [HttpPut("update-provider")]
        public async Task<IActionResult> UpdateProvider(domain.ModelDto.UpdateProviderDto user)
        {
            if (user.ImageFile == null)
            {
                var command = new UpdateProviderCommand
                {
                    FileStream = null,
                    FileName = null,
                    User = user
                };

                var result = await _mediator.Send(command);
                if (!result.IsSuccess)
                {
                    return NotFound(result);
                }
                return Ok(result);
            }

            using (var stream = user.ImageFile.OpenReadStream())
            {
                var command = new UpdateProviderCommand
                {
                    FileStream = stream,
                    FileName = user.ImageFile.FileName,
                    User = user
                };

                var result = await _mediator.Send(command);
                if (!result.IsSuccess)
                {
                    return NotFound(result);
                }
                return Ok(result);
            }
        }





        [HttpGet("get-all-patient")]
        public async Task<IActionResult> GetAllPatient()
        {
            var result = await _mediator.Send(new GetAllPatientQuery());
            if (!result.IsSuccess)
            {
                return NotFound(result);
            }
            return Ok(result);
        }


        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(domain.ModelDto.ChangePasswordDto model)
        {
            var result = await _mediator.Send(new ChangeUserPasswordCommand { ChangePasswordData = model });
            if (!result.IsSuccess)
            {
                return NotFound(result);
            }
            return Ok(result);
        }


    }
}
