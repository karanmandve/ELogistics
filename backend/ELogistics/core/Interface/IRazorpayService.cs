using Razorpay.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace core.Interface
{
    public interface IRazorpayService
    {
        Task<object> CreateOrderAsync(decimal amount, string currency = "INR");
        Task<Payment> VerifyPaymentAsync(string paymentId, string orderId);
    }
}
