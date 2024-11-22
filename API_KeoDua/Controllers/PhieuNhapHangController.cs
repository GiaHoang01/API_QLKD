using API_KeoDua.Data;
using API_KeoDua.Models;
using API_KeoDua.Reponsitory.Interface;
using Microsoft.AspNetCore.Mvc;

namespace API_KeoDua.Controllers
{
    [Route("~/api/[controller]/[action]")]
    [ApiController]
    public class PhieuNhapHangController : BaseController
    {
        private readonly PhieuNhapHangContext phieuNhapHangContext;
        private readonly IPhieuNhapHangReponsitory phieuNhapHangReponsitory;
        public PhieuNhapHangController(PhieuNhapHangContext phieuNhapHangContex, IPhieuNhapHangReponsitory phieuNhapHangReponsitory)
        {
            this.phieuNhapHangContext = phieuNhapHangContext;
            this.phieuNhapHangReponsitory = phieuNhapHangReponsitory;
        }

        /// <summary>
        /// Hàm lấy danh sách tất cả các đơn hàng
        /// </summary>
        /// <param name="dicData">{SearchString:"string",FromDate:"date",ToDate:"Date",PageIndex:"int",PageSize:""}</param>
        /// <returns>Employees</returns>
        [HttpPost]
        public async Task<ActionResult> getAllPurchaseOrder([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------End getAllPurchaseOrder-------");
                ResponseModel repData = await ResponseFail();

                int pageIndex = Convert.ToInt32(dicData["PageIndex"].ToString());
                int pageSize = Convert.ToInt32(dicData["PageSize"].ToString());
                string searchString = dicData["SearchString"].ToString();
                DateTime fromDate = DateTime.Parse(dicData["FromDate"].ToString());
                DateTime toDate = DateTime.Parse(dicData["ToDate"].ToString());
                int startRow = (pageIndex - 1) * pageSize;
                int maxRows = pageSize;

                List<PhieuNhapHang> phieuNhapHangs = await this.phieuNhapHangReponsitory.GetAllPurchase(fromDate,toDate,searchString, startRow, maxRows);

                if (phieuNhapHangs != null && phieuNhapHangs.Any())
                {
                    repData = await ResponseSucceeded();
                }

                repData.data = new { TotalRows = this.phieuNhapHangReponsitory.TotalRows, PurchaseOrders = phieuNhapHangs };
                return Ok(repData);
            }
            catch (Exception ex)
            {
                ResponseModel repData = await ResponseException();
                return Ok(repData);
            }
            finally
            {
                logger.Debug("-------End getAllPurchaseOrder-------");
            }
        }

        /// <summary>
        /// Hàm lấy chi tiết phiếu nhập hàng
        /// </summary>
        /// <param name="dicData">{MaPhieuNhap:"Guid"}</param>
        /// <returns>Employees</returns>
        [HttpPost]
        public async Task<ActionResult> getPurchaseOrder_ByID([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------Start getPurchaseOrder_ByID-------");
                ResponseModel repData = await ResponseFail();
                Guid maPhieuNhap = Guid.Parse(dicData["MaPhieuNhap"].ToString());
                var result = await this.phieuNhapHangReponsitory.GetPurchase_ByID(maPhieuNhap);
                var phieuNhap = result.phieuNhap;
                var chiTietPhieuNhap = result.chiTietPhieuNhap;
                if (phieuNhap == null && (chiTietPhieuNhap == null || !chiTietPhieuNhap.Any()))
                {
                    repData = await ResponseFail();
                    repData.message = "Không tìm thấy phiếu nhập hoặc chi tiết phiếu nhập.";
                    return Ok(repData);
                }

                repData = await ResponseSucceeded();
                repData.data = new{PhieuNhap = phieuNhap, ChiTietPhieuNhap = chiTietPhieuNhap };

                return Ok(repData);
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ và trả về thông báo lỗi
                logger.Error("Error in getPurchaseOrder_ByID", ex);
                ResponseModel repData = await ResponseException();
                return Ok(repData);
            }
            finally
            {
                logger.Debug("-------End getPurchaseOrder_ByID-------");
            }
        }

    }
}
