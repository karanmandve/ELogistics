using core.API_Response;
using core.Interface;
using domain.Model.Users;
using domain.ModelDtos;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace core.App.User.Query
{
    public class UserLoginQuery : IRequest<AppResponse<object>>
    {
        public LoginDto LoginUser { get; set; }
    }

    public class UserLoginQueryHandler : IRequestHandler<UserLoginQuery, AppResponse<object>>
    {
        private readonly IAppDbContext _context;
        private readonly IConfiguration _configuration;

        public UserLoginQueryHandler(IAppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<AppResponse<object>> Handle(UserLoginQuery request, CancellationToken cancellationToken)
        {
            var user = request.LoginUser;

            var userExist = await _context.Set<domain.Model.Users.User>()
                .FirstOrDefaultAsync(x => x.Email == user.Email, cancellationToken);

            if (userExist == null || !BCrypt.Net.BCrypt.Verify(user.Password, userExist.Password) || !userExist.IsActive)
            {
                return AppResponse.Fail<object>(message: "Email or Password Invalid", statusCode: HttpStatusCodes.Unauthorized);
            }

            var otp = await _context.Set<domain.Model.Otp.Otp>()
                        .Where(x => x.Email == user.Email && x.Code == user.Otp)
                        .OrderByDescending(x => x.Expiration)
                        .FirstOrDefaultAsync(cancellationToken);

            if (otp == null || otp.Expiration < DateTime.Now)
            {
                return AppResponse.Fail<object>(message: "Invalid Otp", statusCode: HttpStatusCodes.Unauthorized);
            }

            var userType = await _context.Set<UserType>().FirstOrDefaultAsync(x => x.Id == userExist.UserTypeId);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"] ?? string.Empty),
                new Claim("Id", userExist.Id.ToString()),
                new Claim("Email", userExist.Email),
                new Claim(ClaimTypes.Role, userType.Name ?? string.Empty),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? string.Empty));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: signIn);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            var allOtps = await _context.Set<domain.Model.Otp.Otp>()
                .Where(x => x.Email == user.Email)
                .ToListAsync(cancellationToken);

            _context.Set<domain.Model.Otp.Otp>().RemoveRange(allOtps);

            await _context.SaveChangesAsync(cancellationToken);

            var data = new
            {
                Token = jwt,
                Expiration = token.ValidTo,
            };

            return AppResponse.Success<object>(data, message: "Login Successful", statusCode: HttpStatusCodes.OK);
        }
    }
}
