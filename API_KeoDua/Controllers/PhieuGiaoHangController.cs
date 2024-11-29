using API_KeoDua.Data;
using API_KeoDua.DataView;
using API_KeoDua.Models;
using API_KeoDua.Reponsitory.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


namespace API_KeoDua.Controllers
{
    [Route("~/api/[controller]/[action]")]
    [ApiController]
    public class PhieuGiaoHangController : BaseController
    {
        private readonly PhieuGiaoHangContext phieuGiaoHangContext;
        private readonly IPhieuGiaoHangReponsitory phieuGiaoHangReponsitory;

        public PhieuGiaoHangController(PhieuGiaoHangContext phieuGiaoHangContext, IPhieuGiaoHangReponsitory phieuGiaoHangReponsitory)
        {
            this.phieuGiaoHangContext = phieuGiaoHangContext;
            this.phieuGiaoHangReponsitory = phieuGiaoHangReponsitory;
        }

        /// <summary>
        /// Hàm lấy danh sách phiếu giao hàng
        /// </summary>
        /// <param name="dicData">{SearchString:"string",FromDate:"date",ToDate:"Date",PageIndex:"int",PageSize:""}</param>
        /// <returns>Employees</returns>
        [HttpPost]
        public async Task<ActionResult> getAllShippingNote([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------End getAllShippingNote-------");
                ResponseModel repData = await ResponseFail();

                int pageIndex = Convert.ToInt32(dicData["PageIndex"].ToString());
                int pageSize = Convert.ToInt32(dicData["PageSize"].ToString());
                string searchString = dicData["SearchString"].ToString();
                DateTime fromDate = DateTime.Parse(dicData["FromDate"].ToString());
                DateTime toDate = DateTime.Parse(dicData["ToDate"].ToString());
                int startRow = (pageIndex - 1) * pageSize;
                int maxRows = pageSize;

                List<dynamic> phieuGiaoHangs = await this.phieuGiaoHangReponsitory.GetAllShippingNote(fromDate, toDate, searchString, startRow, maxRows);

                if (phieuGiaoHangs != null)
                {
                    repData = await ResponseSucceeded();
                }

                repData.data = new { TotalRows = this.phieuGiaoHangReponsitory.TotalRows, ShippingNotes = phieuGiaoHangs };
                return Ok(repData);
            }
            catch (Exception ex)
            {
                ResponseModel repData = await ResponseException();
                return Ok(repData);
            }
            finally
            {
                logger.Debug("-------End getAllShippingNote-------");
            }
        }

        /// <summary>
        /// Hàm lấy danh sách phiếu giao hàng có trạng thái 'Mới tạo'
        /// </summary>
        /// <param name="dicData">{SearchString:"string",FromDate:"date",ToDate:"Date",PageIndex:"int",PageSize:""}</param>
        /// <returns>ShippingNotes</returns>
        [HttpPost]
        public async Task<ActionResult> getAllShippingNoteNewCreated([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------Start getAllShippingNoteNewCreated-------");

                // Mặc định trả về ResponseFail
                ResponseModel repData = await ResponseFail();

                // Lấy dữ liệu từ dicData
                int pageIndex = Convert.ToInt32(dicData["PageIndex"].ToString());
                int pageSize = Convert.ToInt32(dicData["PageSize"].ToString());
                string searchString = dicData["SearchString"].ToString();
                DateTime fromDate = DateTime.Parse(dicData["FromDate"].ToString());
                DateTime toDate = DateTime.Parse(dicData["ToDate"].ToString());

                // Tính toán số dòng bắt đầu và số dòng tối đa
                int startRow = (pageIndex - 1) * pageSize;
                int maxRows = pageSize;

                // Lấy danh sách phiếu giao hàng từ repository
                List<dynamic> phieuGiaoHangs = await this.phieuGiaoHangReponsitory.GetAllShippingNoteNewCreated(fromDate, toDate, searchString, startRow, maxRows);

                if (phieuGiaoHangs != null)
                {
                    repData = await ResponseSucceeded();
                }

                // Trả về dữ liệu gồm tổng số bản ghi và danh sách phiếu giao hàng
                repData.data = new { TotalRows = this.phieuGiaoHangReponsitory.TotalRows, ShippingNotes = phieuGiaoHangs };

                return Ok(repData);
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ và trả về ResponseException
                ResponseModel repData = await ResponseException();
                return Ok(repData);
            }
            finally
            {
                logger.Debug("-------End getAllShippingNoteNewCreated-------");
            }
        }

        /// <summary>
        /// Hàm lấy thông tin chi tiết phiếu giao hàng
        /// </summary>
        /// <param name="dicData">{MaPhieuGiao: Guid}</param>
        /// <returns>KhachHang</returns>
        [HttpPost]
        public async Task<ActionResult> GetShippingNoteByID([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------End GetShippingNoteByID-------");
                ResponseModel repData = await ResponseFail();

                Guid? maPhieuGiao = dicData.ContainsKey("MaPhieuGiao") && !string.IsNullOrEmpty(dicData["MaPhieuGiao"]?.ToString()) ? Guid.Parse(dicData["MaPhieuGiao"].ToString()) : (Guid?)null;
                int status = Convert.ToInt32(dicData["Status"].ToString());

                if (status == 1)//Thêm mới
                {
                    PhieuGiaoHang phieuGiao = new PhieuGiaoHang();
                    phieuGiao.MaPhieuGiao = Guid.NewGuid();
                    phieuGiao.TrangThai = "Mới tạo";
                    phieuGiao.NgayTao = DateTime.Now;
                    phieuGiao.NgayGiao = DateTime.Now;
                    phieuGiao.MaThongTin = null;
                    repData = await ResponseSucceeded();
                    repData.data = new { PhieuGiaoHang = phieuGiao };
                }
                else
                {
                    dynamic phieuGiao = await this.phieuGiaoHangReponsitory.GetAllShippingNoteByID(maPhieuGiao);
                    if (phieuGiao == null)
                    {
                        repData = await ResponseFail();
                    }
                    else
                    {
                        repData = await ResponseSucceeded();
                        repData.data = new { PhieuGiaoHang = phieuGiao };
                    }
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
                logger.Debug("-------End GetShippingNoteByID-------");
            }
        }

        /// <summary>
        /// Lưu phiếu giao hàng
        /// </summary>
        /// <param name="dicData">{MaPhieuGiao: Guid}</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> SaveShippingNote([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------Start SaveShippingNote-------");
                ResponseModel repData = await ResponseFail();

                PhieuGiaoHang phieuGiaoHang = JsonConvert.DeserializeObject<PhieuGiaoHang>(dicData["PhieuGiaoHang"].ToString());
                Guid? maThongTin = dicData.ContainsKey("MaThongTin") && !string.IsNullOrEmpty(dicData["MaThongTin"]?.ToString()) ? Guid.Parse(dicData["MaThongTin"].ToString()) : (Guid?)null;
                int status = Convert.ToInt32(dicData["Status"].ToString());
                if (status == 1)
                {
                    phieuGiaoHang.NgayTao = DateTime.Now;
                    phieuGiaoHang.MaThongTin = maThongTin;
                    await this.phieuGiaoHangReponsitory.AddShippingNote(phieuGiaoHang);
                }
                else
                {
                    await this.phieuGiaoHangReponsitory.UpdateShippingNote(phieuGiaoHang.MaPhieuGiao, maThongTin);
                }

                repData = await ResponseSucceeded();
                repData.data = new { maThongTin = maThongTin, status = 2 };
                return Ok(repData);
            }
            catch (Exception ex)
            {
                ResponseModel repData = await ResponseException();
                return Ok(repData);
            }
            finally
            {
                logger.Debug("-------End SaveShippingNote-------");
            }
        }

        /// <summary>
        /// Xóa phiếu giao hàng
        /// </summary>
        /// <param name="dicData">{MaPhieuGiao: Guid}</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> DeleteShippingNote([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------End DeleteShippingNote-------");
                ResponseModel repData = await ResponseFail();

                Guid maPhieuGiao = (dicData["MaPhieuGiao"] == null || string.IsNullOrEmpty(dicData["MaPhieuGiao"].ToString())) ? Guid.Empty : Guid.Parse(dicData["MaPhieuGiao"].ToString());

                await this.phieuGiaoHangReponsitory.DeleteShippingNote(maPhieuGiao);

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
                logger.Debug("-------End DeleteShippingNote-------");
            }
        }

        /// <summary>
        /// Chuyển trạng thái phiếu giao hàng
        /// </summary>
        /// <param name="dicData">{MaPhieuGiao: Guid, TrangThai: string, Status: int}</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> ChangeShippingNoteStatus([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------Start ChangeShippingNoteStatus-------");

                // Khởi tạo dữ liệu phản hồi mặc định
                ResponseModel repData = await ResponseFail();

                // Lấy thông tin từ dicData
                Guid? maPhieuGiao = dicData.ContainsKey("MaPhieuGiao") && !string.IsNullOrEmpty(dicData["MaPhieuGiao"]?.ToString())
                    ? Guid.Parse(dicData["MaPhieuGiao"].ToString())
                    : (Guid?)null;

                Guid? maNhanVien = !string.IsNullOrEmpty(dicData["MaNhanVien"]?.ToString())
                    ? Guid.Parse(dicData["MaNhanVien"].ToString())
                    : (Guid?)null;

                int trangThai = Convert.ToInt32(dicData["TrangThai"].ToString());


                // Gọi repository để cập nhật trạng thái
                bool isUpdated = await this.phieuGiaoHangReponsitory.ChangeShippingNoteStatus(maPhieuGiao.Value, maNhanVien, trangThai);

                // Cập nhật phản hồi thành công
                if (isUpdated)
                {
                    repData = await ResponseSucceeded();
                    repData.data = new { maPhieuGiao = maPhieuGiao, status = trangThai, trangThaiMoi = trangThai };
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
                logger.Debug("-------End ChangeShippingNoteStatus-------");
            }
        }

        /// <summary>
        /// QuickSearch phiếu giao hàng trạng thái Chưa hoàn tất
        /// </summary>
        /// <param name="dicData"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> quickSearchShippingNoteIncpmplete([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------Begin quickSearchShippingNoteIncpmplete-------");
                ResponseModel repData = await ResponseFail();

                string searchString = dicData["SearchString"].ToString();

                var resultData = await this.phieuGiaoHangReponsitory.QuickSearchShippingNoteIncpmplete(searchString);

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
                logger.Debug("-------End quickSearchShippingNoteIncpmplete-------");
            }
        }

    }
}
