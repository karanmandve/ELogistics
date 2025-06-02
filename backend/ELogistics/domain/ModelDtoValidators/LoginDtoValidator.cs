//using domain.ModelDto;
//using FluentValidation;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace domain.ModelDtoValidators
//{
//    public class LoginDtoValidator : AbstractValidator<LoginDto>
//    {
//        public LoginDtoValidator()
//        {

//            RuleFor(x => x.Username)
//                .NotEmpty().WithMessage("Username is required.");

//            RuleFor(x => x.Password)
//                .NotEmpty().WithMessage("Password is required.")
//                .Matches(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9])(?=.*[!@#$%^&*]).{8,}$")
//                .WithMessage("Password must be at least 8 characters long, contain at least one uppercase letter, one lowercase letter, one number, and one special character.");

//        }
//    }
//}
