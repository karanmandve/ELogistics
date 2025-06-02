//using domain.ModelDto;
//using FluentValidation;

//namespace domain.ModelDtoValidators
//{
//    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
//    {
//        public RegisterDtoValidator()
//        {

//            RuleFor(user => user.FirstName)
//                .NotEmpty().WithMessage("First Name is required.")
//                .MaximumLength(50).WithMessage("First Name cannot exceed 50 characters.");

//            RuleFor(user => user.LastName)
//                .NotEmpty().WithMessage("Last Name is required.")
//                .MaximumLength(50).WithMessage("Last Name cannot exceed 50 characters.");

//            RuleFor(user => user.Email)
//                .NotEmpty().WithMessage("Email is required.")
//                .EmailAddress().WithMessage("Invalid email format.");

//            //RuleFor(user => user.Mobile)
//            //    .NotEmpty().WithMessage("Mobile number is required.")
//            //    .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Invalid mobile number format.");

//            RuleFor(user => user.DOB)
//                .NotEmpty().WithMessage("Date of Birth is required.")
//                .LessThanOrEqualTo(DateOnly.FromDateTime(DateTime.Today)).WithMessage("Date of Birth cannot be in the future.");


//            RuleFor(user => user.UserTypeId)
//                .GreaterThan(0).WithMessage("UserTypeId must be greater than 0.");

//            RuleFor(x => x.File)
//                .NotNull().WithMessage("File is required.")
//                .Must(file => file.Length > 0).WithMessage("File cannot be empty.")
//                .Must(file => file.Length <= 5 * 1024 * 1024).WithMessage("File size must not exceed 5 MB.")
//                .Must(file => IsValidFileType(file.FileName)).WithMessage("Only .jpeg, .jpg, and .png files are allowed.");

//            RuleFor(user => user.Address)
//                .NotEmpty().WithMessage("Address is required.")
//                .MaximumLength(250).WithMessage("Address cannot exceed 250 characters.");

//            RuleFor(user => user.Zipcode)
//                .GreaterThan(0).WithMessage("Zipcode must be greater than 0.");

//            RuleFor(user => user.StateId)
//                .GreaterThan(0).WithMessage("StateId must be greater than 0.");

//            RuleFor(user => user.CountryId)
//                .GreaterThan(0).WithMessage("CountryId must be greater than 0.");
//        }

//        private bool IsValidFileType(string fileName)
//        {
//            var allowedExtensions = new[] { ".jpeg", ".jpg", ".png" };
//            var fileExtension = System.IO.Path.GetExtension(fileName).ToLower();
//            return allowedExtensions.Contains(fileExtension);
//        }
//    }
//}
