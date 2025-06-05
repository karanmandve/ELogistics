/*
using core.API_Response;
using core.Interface;
using domain.ModelDto;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.App.User.Command
{
    public class ChangeUserPasswordCommand : IRequest<AppResponse<object>>
    {
        public ChangePasswordDto ChangePasswordData { get; set; }
    }

    public class ChangeUserPasswordCommandHandler : IRequestHandler<ChangeUserPasswordCommand, AppResponse<object>>
    {
        private readonly IAppDbContext _context;
        private readonly IEmailService _emailService;

        public ChangeUserPasswordCommandHandler(IAppDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        public async Task<AppResponse<object>> Handle(ChangeUserPasswordCommand request, CancellationToken cancellationToken)
        {
            var username = request.ChangePasswordData.Username;
            var newPassword = request.ChangePasswordData.NewPassword;

            var existingCustomer = await _context.Set<domain.Model.Users.Customer>().FirstOrDefaultAsync(user => user.Username == username);

            if (existingCustomer == null)
            {
                return AppResponse.Fail<object>(message: "User Not Found", statusCode: HttpStatusCodes.NotFound);
            }

            existingCustomer.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);

            await _context.SaveChangesAsync();

            return AppResponse.Success<object>(message: "Password Changed Successfully", statusCode: HttpStatusCodes.OK);
        }
    }

}
*/
