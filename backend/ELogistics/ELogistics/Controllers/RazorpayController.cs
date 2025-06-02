using core.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EComApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RazorpayController : ControllerBase
    {
        private readonly IRazorpayService _razorpayService;

        public RazorpayController(IRazorpayService razorpayService)
        {
            _razorpayService = razorpayService;
        }

        [HttpPost("create-order")]
        public async Task<IActionResult> CreateOrder([FromBody] decimal amount)
        {
            if (amount == null || amount <= 0)
            {
                return BadRequest("Invalid payment request.");
            }

            try
            {
                var order = await _razorpayService.CreateOrderAsync(amount);
                // iterate order which is an object


                if (order != null)
                {
                    return Ok(order);
                }
                else
                {
                    return NotFound("Order could not be created.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("verify-payment")]
        public async Task<IActionResult> VerifyPayment(domain.ModelDto.Payment.PaymentVerificationDto request)
        {
            var payment = await _razorpayService.VerifyPaymentAsync(request.PaymentId, request.OrderId);
            if (payment != null)
            {
                return Ok(payment);
            }
            return BadRequest("Payment verification failed");
        }
    }
}
