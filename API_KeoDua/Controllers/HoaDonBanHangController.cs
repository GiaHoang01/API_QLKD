using API_KeoDua.Data;
using API_KeoDua.DataView;
using API_KeoDua.Models;
using API_KeoDua.Reponsitory.Interface;
using Microsoft.AspNetCore.Http;
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

                int pageIndex = Convert.ToInt32(dicData["PageIndex"].ToString());
                int pageSize = Convert.ToInt32(dicData["PageSize"].ToString());
                string searchString = dicData["SearchString"].ToString();
                Guid customerId = Guid.Parse(dicData["MaKhachHang"].ToString());
                Guid? employeeId = string.IsNullOrWhiteSpace(dicData["MaNV"]?.ToString())
                  ? null
                  : Guid.Parse(dicData["MaNV"].ToString());
                Guid? cartId = string.IsNullOrWhiteSpace(dicData["MaGioHang"]?.ToString())
                 ? null
                 : Guid.Parse(dicData["MaGioHang"].ToString());
                string maHinhThuc = dicData["MaHinhThuc"].ToString();
                int startRow = (pageIndex - 1) * pageSize;
                int maxRows = pageSize;

                List<HoaDonBanHang> saleInvoiceList = await this.hoaDonBanHangReponsitory.GetAllSaleInVoiceWithWait(searchString,employeeId,cartId,customerId,maHinhThuc, startRow, maxRows);

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
        public async Task<ActionResult> getAllSaleInvoice([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------Begin getAllSaleInvoice-------");
                ResponseModel repData = await ResponseFail();

                int pageIndex = Convert.ToInt32(dicData["PageIndex"].ToString());
                int pageSize = Convert.ToInt32(dicData["PageSize"].ToString());
                string searchString = dicData["SearchString"].ToString();
                Guid customerId = Guid.Parse(dicData["MaKhachHang"].ToString());
                Guid? employeeId = string.IsNullOrWhiteSpace(dicData["MaNV"]?.ToString())
                 ? null
                 : Guid.Parse(dicData["MaNV"].ToString());
                Guid? cartId = string.IsNullOrWhiteSpace(dicData["MaGioHang"]?.ToString())
                 ? null
                 : Guid.Parse(dicData["MaGioHang"].ToString());
                string maHinhThuc = dicData["SearchString"].ToString();
                int startRow = (pageIndex - 1) * pageSize;
                int maxRows = pageSize;

                List<HoaDonBanHang> saleInvoiceList = await this.hoaDonBanHangReponsitory.GetAllSaleInVoice(searchString,employeeId,cartId,customerId,maHinhThuc, startRow, maxRows);

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
    }
}
