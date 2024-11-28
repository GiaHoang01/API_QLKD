using API_KeoDua.Data;
using API_KeoDua.DataView;
using API_KeoDua.Models;
using API_KeoDua.Reponsitory.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
        public async Task<ActionResult> CancelSaleInvoice([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------Begin CancelSaleInvoice-------");
                ResponseModel repData = await ResponseFail();
                Guid saleId = Guid.Parse(dicData["MaHoaDon"].ToString());
                Guid employeeId = Guid.Parse(dicData["MaNV"].ToString());
                if (await (this.hoaDonBanHangReponsitory.CancelSaleInvoice(saleId, employeeId)))
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
                logger.Debug("-------End CancelSaleInvoice-------");
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
        /// 
        /// </summary>
        /// <param name="dicData"></param>
        /// <returns></returns>

        [HttpPost]
        public async Task<ActionResult> getSaleInvoice_ByID([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------Start getSaleInvoice_ByID-------");
                ResponseModel repData = await ResponseFail();
                Guid? maHoaDon = dicData.ContainsKey("MaHoaDon") && !string.IsNullOrEmpty(dicData["MaHoaDon"]?.ToString()) ? Guid.Parse(dicData["MaHoaDon"].ToString()) : (Guid?)null;

                int status = Convert.ToInt32(dicData["Status"].ToString());
                if (status == 1)
                {
                    HoaDonBanHangView hoaDonBanHang = new HoaDonBanHangView();
                    hoaDonBanHang.MaHoaDon = Guid.NewGuid();
                    hoaDonBanHang.NgayBan = DateTime.Now;
                    hoaDonBanHang.NgayThanhToan = DateTime.Now;
                    hoaDonBanHang.TrangThai = "Mới tạo";
                    List<CT_HoaDonBanHang> cT_HoaDonBanHangs = new List<CT_HoaDonBanHang>();
                    repData = await ResponseSucceeded();
                    repData.data = new { hoaDonBanHang = hoaDonBanHang, CTHoaDonBanHang = cT_HoaDonBanHangs };
                }
                else
                {
                    var result = await this.hoaDonBanHangReponsitory.GetInvoice_ByID(maHoaDon);
                    var hoaDonBanHang = result.hoaDonBanHang;
                    var chiTietHoaDonBanHang = result.cT_HoaDonBanHangs;
                    if (hoaDonBanHang == null && (chiTietHoaDonBanHang == null || !chiTietHoaDonBanHang.Any()))
                    {
                        repData = await ResponseFail();
                        repData.message = "Không tìm thấy phiếu nhập hoặc chi tiết phiếu nhập.";
                        return Ok(repData);
                    }
                    repData = await ResponseSucceeded();
                    repData.data = new { HoaDonBanHang = hoaDonBanHang, ChiTietHoaDonBanHang = chiTietHoaDonBanHang };

                }
                return Ok(repData);
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ và trả về thông báo lỗi
                logger.Error("Error in getSaleInvoice_ByID", ex);
                ResponseModel repData = await ResponseException();
                return Ok(repData);
            }
            finally
            {
                logger.Debug("-------End getSaleInvoice_ByID-------");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dicData"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> SaveSaleInvoice([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------Start SaveSaleInvoice-------");
                ResponseModel repData = await ResponseFail();
                HoaDonBanHang hoaDonBanHang = JsonConvert.DeserializeObject<HoaDonBanHang>(dicData["HoaDonBanHang"].ToString());
                List<CT_HoaDonBanHang> ct_HoaDonBanHangs = JsonConvert.DeserializeObject<List<CT_HoaDonBanHang>>(dicData["CTHoaDonBanHang"].ToString());

                int status = Convert.ToInt32(dicData["Status"].ToString());
                if (status == 1)
                {
                    await this.hoaDonBanHangReponsitory.AddSaleInvoice(hoaDonBanHang, ct_HoaDonBanHangs);
                    repData = await ResponseSucceeded();
                    repData.data = new { };
                }
                else
                {
                    bool isUpdate = await this.hoaDonBanHangReponsitory.UpdateSaleInvoice(hoaDonBanHang, ct_HoaDonBanHangs);
                    if (!isUpdate)
                    {
                        repData = await ResponseFail();
                        repData.data = new { };
                    }
                    repData = await ResponseSucceeded();
                    repData.data = new { };
                }
                return Ok(repData);
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ và trả về thông báo lỗi
                logger.Error("Error in SaveSaleInvoice", ex);
                ResponseModel repData = await ResponseException();
                return Ok(repData);
            }
            finally
            {
                logger.Debug("-------End SaveSaleInvoice-------");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dicData"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> DeleteSaleInvoice([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------Start DeleteSaleInvoice-------");
                ResponseModel repData = await ResponseFail();
                Guid maHoaDon = Guid.Parse(dicData["MaHoaDon"].ToString());
                bool isCheck = await this.hoaDonBanHangReponsitory.DeleteSaleInvoice(maHoaDon);
                if (!isCheck)
                {
                    repData = await ResponseFail();
                    repData.message = "Phiếu nhập hàng này đã được phê duyệt";

                    return Ok(repData);
                }
                repData = await ResponseSucceeded();
                repData.data = new { };
                return Ok(repData);
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ và trả về thông báo lỗi
                logger.Error("Error in DeleteSaleInvoice", ex);
                ResponseModel repData = await ResponseException();
                return Ok(repData);
            }
            finally
            {
                logger.Debug("-------End DeletePurchaseOrder_Request-------");
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

        /// <summary>
        /// Hàm lấy tổng số hóa đơn đã thanh toán
        /// </summary>
        /// <param name="dicData"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> GetTotalCompletedSales([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------End GetTotalCompletedSales-------");

                // Initialize the response model
                ResponseModel repData = await ResponseFail();
                int totals = await this.hoaDonBanHangReponsitory.TotalSalesCompletedRecords();
                repData = await ResponseSucceeded();
                repData.data = new
                {
                    totalSales = totals
                };

                return Ok(repData);
            }
            catch (Exception ex)
            {
                // Handle exception and return error response
                ResponseModel repData = await ResponseException();
                return Ok(repData);
            }
            finally
            {
                logger.Debug("-------End GetTotalCompletedSales-------");
            }
        }

        /// <summary>
        /// Hàm lấy tổng thu từ bán hàng
        /// </summary>
        /// <param name="dicData"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> getTotalRevenue([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------End getTotalRevenue-------");

                // Initialize the response model
                ResponseModel repData = await ResponseFail();
                decimal totals = await this.hoaDonBanHangReponsitory.TotalSalesCompletedAmount();
                repData = await ResponseSucceeded();
                repData.data = new
                {
                    total = totals
                };

                return Ok(repData);
            }
            catch (Exception ex)
            {
                // Handle exception and return error response
                ResponseModel repData = await ResponseException();
                return Ok(repData);
            }
            finally
            {
                logger.Debug("-------End getTotalRevenue-------");
            }
        }

        /// <summary>
        /// Hàm lấy tổng thu từ năm truyền vào
        /// </summary>
        /// <param name="dicData"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> getTotalRevenueByYear([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------End getTotalRevenueByYear-------");

                // Initialize the response model
                ResponseModel repData = await ResponseFail();
                int year = Convert.ToInt32(dicData["Year"].ToString());
                decimal totals = await this.hoaDonBanHangReponsitory.TotalRevenueByYear(year);
                repData = await ResponseSucceeded();
                repData.data = new
                {
                    total = totals
                };

                return Ok(repData);
            }
            catch (Exception ex)
            {
                // Handle exception and return error response
                ResponseModel repData = await ResponseException();
                return Ok(repData);
            }
            finally
            {
                logger.Debug("-------End getTotalRevenueByYear-------");
            }
        }


    }
}
