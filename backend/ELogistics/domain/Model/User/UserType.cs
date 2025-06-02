using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace domain.Model.User
{
    public class UserType
    {
        [Key]
       public int Id { get; set; }
       public string UserTypeName { get; set; }
       
    }
}
 