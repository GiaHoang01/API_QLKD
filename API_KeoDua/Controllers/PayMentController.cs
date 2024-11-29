using API_KeoDua.Models.VnPAY;
using API_KeoDua.Services.VnPAY;
using Microsoft.AspNetCore.Mvc;

namespace API_KeoDua.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IVnPayService _vnPayService;

        public PaymentController(IVnPayService vnPayService)
        {
            _vnPayService = vnPayService;
        }

        // API endpoint to create payment URL
        [HttpPost("create-payment-url")]
        public IActionResult CreatePaymentUrlVnpay([FromBody] PaymentInformationModel model)
        {
            // Create the payment URL using the service
            var url = _vnPayService.CreatePaymentUrl(model, HttpContext);

            // Redirect to the payment URL
            return Ok(new { paymentUrl = url });
        }

        // API endpoint to handle the payment callback
        [HttpGet("payment-callback")]
        public IActionResult PaymentCallbackVnpay()
        {
            // Process the callback and get the payment result
            var response = _vnPayService.PaymentExecute(Request.Query);

            // Return the response from VnPay
            return Ok(response);
        }
    }
}
