using core.Interface;
using Microsoft.Extensions.Configuration;
using Razorpay.Api;

namespace core.Service
{
    public class RazorpayService : IRazorpayService
    {
        private readonly RazorpayClient _razorpayClient;

        public RazorpayService(string keyId, string keySecret)
        {
            _razorpayClient = new RazorpayClient(keyId, keySecret);
        }

        public async Task<object> CreateOrderAsync(decimal amount, string currency = "INR")
        {
            var options = new Dictionary<string, object>
            {
                { "amount", amount * 100 },
                { "currency", currency },
                { "receipt", "order_rcptid_11" }
            };

            var order = _razorpayClient.Order.Create(options);

            var orderId = order["id"]?.ToString();
            var orderAmount = order["amount"]?.ToString();
            var orderCurrency = order["currency"]?.ToString();

            var orderData = new
            {
                Id = orderId,
                Amount = orderAmount,
                Currency = orderCurrency
            };

            return orderData;
        }

        public async Task<Payment> VerifyPaymentAsync(string paymentId, string orderId)
        {
            var payment = _razorpayClient.Payment.Fetch(paymentId);

            return payment;
        }
    }
}
