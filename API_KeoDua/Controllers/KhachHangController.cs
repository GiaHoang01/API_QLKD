using API_KeoDua.Data;
using API_KeoDua.DataView;
using API_KeoDua.Models;
using API_KeoDua.Reponsitory.Interface;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

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

        /// <summary>
        /// Hàm lấy thông tin chi tiết khách hàng
        /// </summary>
        /// <param name="dicData">{CustomerID: Guid}</param>
        /// <returns>KhachHang</returns>
        [HttpPost]
        public async Task<ActionResult> GetCustomerByID([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------End GetCustomerByID-------");
                ResponseModel repData = await ResponseFail();

                Guid maKH = Guid.Parse(dicData["MaKH"].ToString());
                KhachHang khachHang = await this.khachHangReponsitory.GetCustomerByID(maKH);

                if (khachHang != null)
                {
                    repData = await ResponseSucceeded();
                }

                repData.data = new { KhachHang = khachHang };
                return Ok(repData);
            }
            catch (Exception ex)
            {
                ResponseModel repData = await ResponseException();
                return Ok(repData);
            }
            finally
            {
                logger.Debug("-------End GetCustomerByID-------");
            }
        }

        /// <summary>
        /// Hàm thêm khách hàng
        /// </summary>
        /// <param name="dicData">{"KhachHang": KhachHang}</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> AddCustomer([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------End AddCustomer-------");
                ResponseModel repData = await ResponseFail();

                KhachHang khachHang = JsonConvert.DeserializeObject<KhachHang>(dicData["KhachHang"].ToString());
                khachHang.MaKhachHang = Guid.NewGuid();

                // Kiểm tra nếu số điện thoại đã tồn tại
                if (await this.khachHangReponsitory.IsPhoneNumberExists(khachHang.Sdt))
                {
                    repData.message = "Số điện thoại đã tồn tại.";
                    return Ok(repData);
                }

                await this.khachHangReponsitory.AddCustomer(khachHang);
                repData = await ResponseSucceeded();
                repData.data = new { };
                return Ok(repData);
            }
            catch (Exception ex)
            {
                ResponseModel repData = await ResponseException();
                return Ok(repData);
            }
            finally
            {
                logger.Debug("-------End AddCustomer-------");
            }
        }

        /// <summary>
        /// Hàm Xóa khách hàng
        /// </summary>
        /// <param name="dicData">{"MaKhachHang": Guid}</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> DeleteCustomer([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------End DeleteCustomer-------");
                ResponseModel repData = await ResponseFail();

                Guid maKH = Guid.Parse(dicData["MaKhachHang"].ToString());
                await this.khachHangReponsitory.DeleteCustomer(maKH);
                repData = await ResponseSucceeded();
                repData.data = new { };
                return Ok(repData);
            }
            catch (Exception ex)
            {
                ResponseModel repData = await ResponseException();
                return Ok(repData);
            }
            finally
            {
                logger.Debug("-------End DeleteCustomer-------");
            }
        }

        /// <summary>
        /// Hàm sửa khách hàng
        /// </summary>
        /// <param name="dicData">{KhachHang: "KhachHang"}</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> UpdateCustomer([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------End UpdatetCustomer-------");
                ResponseModel repData = await ResponseFail();

                KhachHang khachHang = JsonConvert.DeserializeObject<KhachHang>(dicData["KhachHang"].ToString());
                await this.khachHangReponsitory.UpdateCustomer(khachHang);
                repData = await ResponseSucceeded();
                repData.data = new { };
                return Ok(repData);
            }
            catch (Exception ex)
            {
                ResponseModel repData = await ResponseException();
                return Ok(repData);
            }
            finally
            {
                logger.Debug("-------End UpdatetCustomer-------");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dicData"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> quickSearchKhachHang([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("------- quickSearchKhachHang-------");
                ResponseModel repData = await ResponseFail();

                string searchString = dicData["SearchString"].ToString();

                List<KhachHang> khachhangs = await this.khachHangReponsitory.QuickSearchKhachHang(searchString);

                if (khachhangs != null && khachhangs.Any())
                {
                    repData = await ResponseSucceeded();
                }

                repData.data = new { KhachHangs = khachhangs };
                return Ok(repData);
            }
            catch (Exception ex)
            {
                ResponseModel repData = await ResponseException();
                return Ok(repData);
            }
            finally
            {
                logger.Debug("-------End quickSearchKhachHang-------");
            }
        }
    }
}

