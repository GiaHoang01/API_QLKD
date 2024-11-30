using API_KeoDua.Models.VnPAY;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using API_KeoDua.Libraries;

namespace API_KeoDua.Services.VnPAY
{
    public class VnPAYService : IVnPayService
    {
        private readonly IConfiguration _configuration;

        // Constructor
        public VnPAYService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Tạo URL thanh toán cho VNPAY
        /// </summary>
        public string CreatePaymentUrl(PaymentInformationModel model, HttpContext context)
        {
            var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_configuration["TimeZoneId"]);
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
            var tick = DateTime.Now.Ticks.ToString();
            var pay = new VnPayLibrary();
            var urlCallBack = _configuration["PaymentCallBack:ReturnUrl"];

            // Thêm các tham số vào request
            pay.AddRequestData("vnp_Version", _configuration["Vnpay:Version"]);
            pay.AddRequestData("vnp_Command", _configuration["Vnpay:Command"]);
            pay.AddRequestData("vnp_TmnCode", _configuration["Vnpay:TmnCode"]);
            pay.AddRequestData("vnp_Amount", ((int)model.Amount * 100).ToString());
            pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", _configuration["Vnpay:CurrCode"]);
            pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(context));
            pay.AddRequestData("vnp_Locale", _configuration["Vnpay:Locale"]);
            pay.AddRequestData("vnp_OrderInfo", $"{model.Name} {model.OrderDescription} {model.Amount}");
            pay.AddRequestData("vnp_OrderType", model.OrderType);
            pay.AddRequestData("vnp_ReturnUrl", urlCallBack);
            pay.AddRequestData("vnp_TxnRef", tick);

            // Tạo URL thanh toán
            var paymentUrl = pay.CreateRequestUrl(_configuration["Vnpay:BaseUrl"], _configuration["Vnpay:HashSecret"]);

            return paymentUrl;
        }

        /// <summary>
        /// Xử lý callback và xác thực giao dịch từ VNPAY
        /// </summary>
        public PaymentResponseModel PaymentExecute(IQueryCollection collections)
        {
            try
            {
                var pay = new VnPayLibrary();
                var response = pay.GetFullResponseData(collections, _configuration["Vnpay:HashSecret"]);

                // Kiểm tra trạng thái giao dịch
                if (response.TransactionStatus == "00") // Thành công
                {
                    response.Message = "Giao dịch thành công!";
                    response.Success = true;
                }
                else
                {
                    response.Message = "Giao dịch thất bại!";
                    response.Success = false;
                }

                return response;
            }
            catch (Exception ex)
            {
                // Trả về thông báo lỗi nếu có vấn đề
                return new PaymentResponseModel
                {
                    Success = false,
                    Message = "Lỗi khi xử lý callback thanh toán: " + ex.Message
                };
            }
        }
    }
}
