using core.API_Response;
using core.Interface;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class UserLoginQuery : IRequest<AppResponse<object>>
{
    public domain.ModelDto.LoginDto LoginUser { get; set; }
}

public class UserLoginQueryHandler : IRequestHandler<UserLoginQuery, AppResponse<object>>
{
    private readonly IAppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService; // Add email service

    public UserLoginQueryHandler(IAppDbContext context, IConfiguration configuration, IEmailService emailService)
    {
        _context = context;
        _configuration = configuration;
        _emailService = emailService;
    }

    public async Task<AppResponse<object>> Handle(UserLoginQuery request, CancellationToken cancellationToken)
    {
        var user = request.LoginUser;

        var userExist = await _context.Set<domain.Model.User.User>().FirstOrDefaultAsync(x => x.Username == user.Username);

        if (userExist == null || !BCrypt.Net.BCrypt.Verify(user.Password, userExist.Password) || userExist.IsDeleted)
        {
            return AppResponse.Fail<object>(message: "Email or Password Invalid", statusCode: HttpStatusCodes.Unauthorized);
        }

        var otp = await _context.Set<domain.Model.Otp.Otp>().FirstOrDefaultAsync(x => x.Username == user.Username && x.Code == user.Otp);

        if (otp == null || otp.Expiration < DateTime.Now)
        {
            return AppResponse.Fail<object>(message: "Invalid Otp", statusCode: HttpStatusCodes.Unauthorized);
        }

        var userType = await _context.Set<domain.Model.User.UserType>().FirstOrDefaultAsync(x => x.Id == userExist.UserTypeId);

        var claim = new[]
         {
                new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                new Claim("Id", userExist.Id.ToString()),
                new Claim("Email", userExist.Email),
                new Claim(ClaimTypes.Role, userType.UserTypeName)
            };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            _configuration["Jwt:Issuer"],
            _configuration["Jwt:Audience"],
            claim,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: signIn);
        var jwt = new JwtSecurityTokenHandler().WriteToken(token);

        var allOtps = await _context.Set<domain.Model.Otp.Otp>().Where(x => x.Username == user.Username).ToListAsync();
        _context.Set<domain.Model.Otp.Otp>().RemoveRange(allOtps);

        await _context.SaveChangesAsync();

        var data = new
        {
            Token = jwt,
            Expiration = token.ValidTo,
        };


        return AppResponse.Success<object>(data, message: "Login Successful", statusCode: HttpStatusCodes.OK);

    }
}
