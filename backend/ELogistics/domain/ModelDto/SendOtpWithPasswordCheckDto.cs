using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace domain.ModelDto
{
    public class SendOtpWithPasswordCheckDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
