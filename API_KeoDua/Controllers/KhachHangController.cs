using API_KeoDua.Data;
using API_KeoDua.Models;
using API_KeoDua.Reponsitory.Interface;
using Microsoft.AspNetCore.Mvc;

namespace API_KeoDua.Controllers
{
    [Route("~/api/[controller]/[action]")]
    [ApiController]
    public class KhachHangController : BaseController
    {
        private readonly KhachHangContext khachHangContext;
        private readonly IKhachHangReponsitory khachHangReponsitory;

        public KhachHangController(KhachHangContext khachHangContext, IKhachHangReponsitory khachHangReponsitory)
        {
            this.khachHangContext = khachHangContext;
            this.khachHangReponsitory = khachHangReponsitory;
        }

        /// <summary>
        /// Hàm lấy danh sách tất cả các khách hàng
        /// </summary>
        /// <param name="dicData">{"PageIndex":int,"PageSize":int, "SearchString": "string"}</param>
        /// <returns>Customers</returns>
        [HttpPost]
        public async Task<ActionResult> getAllCustomers([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------End getAllCustomers-------");
                ResponseModel repData = await ResponseFail();

                int pageIndex = Convert.ToInt32(dicData["PageIndex"].ToString());
                int pageSize = Convert.ToInt32(dicData["PageSize"].ToString());
                string searchString = dicData["SearchString"].ToString();

                int startRow = (pageIndex - 1) * pageSize;
                int maxRow = pageSize;

                List<KhachHang> customers = await this.khachHangReponsitory.GetAllCustomer(searchString, startRow, maxRow);

                if (customers != null && customers.Any())
                {
                    repData = await ResponseSucceeded();
                }

                repData.data = new { TotalRows = this.khachHangReponsitory.TotalRows, Customers = customers };
                return Ok(repData);
            }
            catch (Exception ex)
            {
                ResponseModel repData = await ResponseException();
                return Ok(repData);
            }
            finally
            {
                logger.Debug("-------End getAllCustomers-------");
            }
        }


    }
}
