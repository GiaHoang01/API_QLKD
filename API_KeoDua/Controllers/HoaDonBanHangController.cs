using API_KeoDua.Data;
using API_KeoDua.DataView;
using API_KeoDua.Models;
using API_KeoDua.Reponsitory.Interface;
using Microsoft.AspNetCore.Mvc;

namespace API_KeoDua.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HoaDonBanHangController : BaseController
    {
        private readonly HoaDonBanHangContext hoaDonBanHangContext;
        private readonly IHoaDonBanHangReponsitory hoaDonBanHangReponsitory;
        private readonly NhanVienContext nhanVienContext;
        private readonly INhanVienReponsitory nhanVienReponsitory;
        private readonly KhachHangContext khachHangContext;
        private readonly IKhachHangReponsitory khachHangReponsitory;
        private readonly GioHangContext gioHangContext;
        private readonly IGioHangReponsitory gioHangReponsitory;
        private readonly HinhThucThanhToanContext hinhThucThanhToanContext;
        private readonly IHinhThucThanhToanReponsitory hinhThucThanhToanReponsitory;

        public HoaDonBanHangController(HoaDonBanHangContext context, IHoaDonBanHangReponsitory hoaDonBanHangReponsitory, NhanVienContext nhanVienContext, INhanVienReponsitory nhanVienReponsitory
            , KhachHangContext khachHangContext, IKhachHangReponsitory khachHangReponsitory, GioHangContext gioHangContext, IGioHangReponsitory gioHangReponsitory, HinhThucThanhToanContext hinhThucThanhToanContext, IHinhThucThanhToanReponsitory hinhThucThanhToanReponsitory)
        {
            this.hoaDonBanHangContext = context;
            this.hoaDonBanHangReponsitory = hoaDonBanHangReponsitory;
            this.khachHangContext = khachHangContext;
            this.khachHangReponsitory = khachHangReponsitory;
            this.nhanVienContext = nhanVienContext;
            this.nhanVienReponsitory = nhanVienReponsitory;
            this.gioHangContext = gioHangContext;
            this.gioHangReponsitory = gioHangReponsitory;
            this.hinhThucThanhToanContext = hinhThucThanhToanContext;
            this.hinhThucThanhToanReponsitory = hinhThucThanhToanReponsitory;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dicData"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> getAllSaleInvoiceWithWait([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------Begin getAllSaleInvoiceWithWait-------");
                ResponseModel repData = await ResponseFail();
                DateTime fromDate = DateTime.Parse(dicData["FromDate"].ToString());
                DateTime toDate = DateTime.Parse(dicData["ToDate"].ToString());
                int pageIndex = Convert.ToInt32(dicData["PageIndex"].ToString());
                int pageSize = Convert.ToInt32(dicData["PageSize"].ToString());
                string searchString = dicData["SearchString"].ToString();
                Guid? customerId = string.IsNullOrWhiteSpace(dicData["MaKhachHang"]?.ToString())
                 ? null
                 : Guid.Parse(dicData["MaKhachHang"].ToString());
                Guid? employeeId = string.IsNullOrWhiteSpace(dicData["MaNV"]?.ToString())
                  ? null
                  : Guid.Parse(dicData["MaNV"].ToString());
                Guid? cartId = string.IsNullOrWhiteSpace(dicData["MaGioHang"]?.ToString())
                 ? null
                 : Guid.Parse(dicData["MaGioHang"].ToString());
                string? maHinhThuc = string.IsNullOrWhiteSpace(dicData["MaHinhThuc"].ToString()) ? null : dicData["MaHinhThuc"].ToString();
                int startRow = (pageIndex - 1) * pageSize;
                int maxRows = pageSize;

                List<HoaDonBanHangView> saleInvoiceList = await this.hoaDonBanHangReponsitory.GetAllSaleInVoiceWithWait(fromDate, toDate, searchString, employeeId, cartId, customerId, maHinhThuc, startRow, maxRows);

                if (saleInvoiceList != null && saleInvoiceList.Any())
                {
                    repData = await ResponseSucceeded();
                }

                repData.data = new { TotalRows = this.hoaDonBanHangReponsitory.TotalRows, SaleInvoiceList = saleInvoiceList };
                return Ok(repData);
            }
            catch (Exception ex)
            {
                ResponseModel repData = await ResponseException();
                return Ok(repData);
            }
            finally
            {
                logger.Debug("-------End getAllSaleInvoiceWithWait-------");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dicData"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> ConfirmSaleInvoice([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------Begin ConfirmSaleInvoicet-------");
                ResponseModel repData = await ResponseFail();
                Guid saleId = Guid.Parse(dicData["MaHoaDon"].ToString());
                Guid employeeId = Guid.Parse(dicData["MaNV"].ToString());
                if (await (this.hoaDonBanHangReponsitory.ConfirmSaleInvoice(saleId, employeeId)))
                {
                    repData = await ResponseSucceeded();
                }

                repData.data = new { };
                if (repData.status == 1)
                {
                    repData.message = "Đã cập nhật thành công";
                }
                else
                {
                    repData.message = "Đã cập nhật thất bại hoặc đã cập nhật rồi";
                }
                return Ok(repData);
            }
            catch (Exception ex)
            {
                ResponseModel repData = await ResponseException();
                return Ok(repData);
            }
            finally
            {
                logger.Debug("-------End ConfirmSaleInvoice-------");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dicData"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> getAllSaleInvoice([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------Begin getAllSaleInvoice-------");
                ResponseModel repData = await ResponseFail();
                DateTime fromDate = DateTime.Parse(dicData["FromDate"].ToString());
                DateTime toDate = DateTime.Parse(dicData["ToDate"].ToString());
                int pageIndex = Convert.ToInt32(dicData["PageIndex"].ToString());
                int pageSize = Convert.ToInt32(dicData["PageSize"].ToString());
                string searchString = dicData["SearchString"].ToString();
                Guid? customerId = string.IsNullOrWhiteSpace(dicData["MaKhachHang"]?.ToString())
                 ? null
                 : Guid.Parse(dicData["MaKhachHang"].ToString());
                Guid? employeeId = string.IsNullOrWhiteSpace(dicData["MaNV"]?.ToString())
                 ? null
                 : Guid.Parse(dicData["MaNV"].ToString());
                Guid? cartId = string.IsNullOrWhiteSpace(dicData["MaGioHang"]?.ToString())
                 ? null
                 : Guid.Parse(dicData["MaGioHang"].ToString());
                string? maHinhThuc = string.IsNullOrWhiteSpace(dicData["MaHinhThuc"].ToString()) ? null : dicData["MaHinhThuc"].ToString();
                int startRow = (pageIndex - 1) * pageSize;
                int maxRows = pageSize;

                List<HoaDonBanHangView> saleInvoiceList = await this.hoaDonBanHangReponsitory.GetAllSaleInVoice(fromDate, toDate, searchString, employeeId, cartId, customerId, maHinhThuc, startRow, maxRows);

                if (saleInvoiceList != null && saleInvoiceList.Any())
                {
                    repData = await ResponseSucceeded();
                }

                repData.data = new { TotalRows = this.hoaDonBanHangReponsitory.TotalRows, SaleInvoiceList = saleInvoiceList };
                return Ok(repData);
            }
            catch (Exception ex)
            {
                ResponseModel repData = await ResponseException();
                return Ok(repData);
            }
            finally
            {
                logger.Debug("-------End getAllSaleInvoice-------");
            }
        }

        /// <summary>
        /// QuickSearch hóa đơn mới tạo
        /// </summary>
        /// <param name="dicData"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> quickSearchSaleInvoiceNewCreated([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------Begin quickSearchSaleInvoiceNewCreated-------");
                ResponseModel repData = await ResponseFail();

                string searchString = dicData["SearchString"].ToString();

                var resultData = await this.hoaDonBanHangReponsitory.QuickSearchSaleInvoiceNewCreated(searchString);

                if (resultData != null && resultData.Any())
                {
                    repData = await ResponseSucceeded();
                    repData.data = new { HoaDonKhachHang = resultData }; // Trả dữ liệu kết hợp
                }
                return Ok(repData);
            }
            catch (Exception ex)
            {
                ResponseModel repData = await ResponseException();
                return Ok(repData);
            }
            finally
            {
                logger.Debug("-------End quickSearchSaleInvoiceNewCreated-------");
            }
        }
    }
}
