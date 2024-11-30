using API_KeoDua.Data;
using API_KeoDua.Models;
using API_KeoDua.Models.VnPAY;
using API_KeoDua.Services.VnPAY;
using Microsoft.AspNetCore.Mvc;
using API_KeoDua.Reponsitory.Interface;
using API_KeoDua.DataView;
using Newtonsoft.Json;
using API_KeoDua.Libraries;
namespace API_KeoDua.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : BaseController
    {
        private readonly IVnPayService _vnPayService;
        private readonly IConfiguration _configuration;

        public PaymentController(IVnPayService vnPayService, IConfiguration configuration)
        {
            _vnPayService = vnPayService;
            _configuration = configuration;
        }

        [HttpPost]
        public async Task<ActionResult> CreatePaymentUrlVnpay([FromBody] PaymentInformationModel paymentInformation)
        {
            try
            {
                Logger.Debug("-------End CreatePaymentUrlVnpay-------");
                ResponseModel repData = await ResponseFail();
                var url = _vnPayService.CreatePaymentUrl(paymentInformation, HttpContext);

                repData = await ResponseSucceeded();
                repData.data = new { url=url };
                return Ok(repData);
            }
            catch (Exception ex)
            {
                ResponseModel repData = await ResponseException();
                return Ok(repData);
            }
            finally
            {
                logger.Debug("-------End CreatePaymentUrlVnpay-------");
            }
        }

        [HttpGet("PaymentCallbackVnpay")]
        public IActionResult PaymentCallbackVnpay()
        {
            var queryString = Request.Query;

            // Lấy HashSecret từ _configuration
            var hashSecret = _configuration["Vnpay:HashSecret"];


            // Kiểm tra mã ResponseCode và TransactionStatus
            if (queryString["vnp_ResponseCode"] != "00" || queryString["vnp_TransactionStatus"] != "00")
            {
                return BadRequest("Giao dịch thất bại.");
            }

            return Ok("Giao dịch thành công.");
        }

    }
}
