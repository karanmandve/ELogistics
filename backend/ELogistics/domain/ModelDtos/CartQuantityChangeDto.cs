using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace domain.ModelDtos
{
    public class CartQuantityChangeDto
    {
        public Guid CustomerId { get; set; }
        public Guid ProductId { get; set; }
        public int QuantityChange { get; set; }
    }
}
