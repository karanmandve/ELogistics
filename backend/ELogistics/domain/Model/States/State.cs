using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace domain.Model.States
{
    public class State
    {
        public int StateId { get; set; }
        public string Name { get; set; }
    }
}
