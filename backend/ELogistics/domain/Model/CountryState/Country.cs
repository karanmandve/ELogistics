using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace domain.Model.CountryState
{
    public class Country
    {
        public int CountryId { get; set; }
        public string ShortName { get; set; }
        public string Name { get; set; }
        public int PhoneCode { get; set; }
    }
}
