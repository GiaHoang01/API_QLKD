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

                List<PhieuNhapHang> phieuNhapHangs = await this.phieuNhapHangReponsitory.GetAllPurchase(fromDate, toDate, searchString, startRow, maxRows);

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
        /// Hàm lấy danh sách tất cả các đơn hàng
        /// </summary>
        /// <param name="dicData">{SearchString:"string",FromDate:"date",ToDate:"Date",PageIndex:"int",PageSize:""}</param>
        /// <returns>Employees</returns>
        [HttpPost]
        public async Task<ActionResult> getAllPurchaseOrderRequest([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------End getAllPurchaseOrderRequest-------");
                ResponseModel repData = await ResponseFail();

                int pageIndex = Convert.ToInt32(dicData["PageIndex"].ToString());
                int pageSize = Convert.ToInt32(dicData["PageSize"].ToString());
                string searchString = dicData["SearchString"].ToString();
                DateTime fromDate = DateTime.Parse(dicData["FromDate"].ToString());
                DateTime toDate = DateTime.Parse(dicData["ToDate"].ToString());
                int startRow = (pageIndex - 1) * pageSize;
                int maxRows = pageSize;

                List<PhieuNhapHang> phieuNhapHangs = await this.phieuNhapHangReponsitory.GetAllPurchaseRequest(fromDate, toDate, searchString, startRow, maxRows);

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
                logger.Debug("-------End getAllPurchaseOrderRequest-------");
            }
        }

        /// <summary>
        /// Hàm thêm mới,chi tiết đơn đặt hàng theo id  status:1 thêm mới , status :2 chi tiết
        /// </summary>
        /// <param name="dicData">{MaPhieuNhap:"Guid",Status:"int"}</param>
        /// <returns>Employees</returns>
        [HttpPost]
        public async Task<ActionResult> getPurchaseOrderRequest_ByID([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------Start getPurchaseOrderRequest_ByID-------");
                ResponseModel repData = await ResponseFail();
                Guid? maPhieuNhap = dicData.ContainsKey("MaPhieuNhap") && !string.IsNullOrEmpty(dicData["MaPhieuNhap"]?.ToString()) ? Guid.Parse(dicData["MaPhieuNhap"].ToString()) : (Guid?)null;

                int status = Convert.ToInt32(dicData["Status"].ToString());
                if (status == 1)
                {
                    PhieuNhapHang phieuNhap = new PhieuNhapHang();
                    phieuNhap.MaPhieuNhap = Guid.NewGuid();
                    phieuNhap.NgayDat = DateTime.Now;
                    phieuNhap.NgayNhap = DateTime.Now;
                    phieuNhap.TrangThai = "Mới tạo";
                    List<CT_PhieuNhap> cT_PhieuNhaps = new List<CT_PhieuNhap>();
                    repData = await ResponseSucceeded();
                    repData.data = new { phieuNhap = phieuNhap, ChiTietPhieuNhap = cT_PhieuNhaps };
                }
                else
                {
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
                    repData.data = new { PhieuNhap = phieuNhap, ChiTietPhieuNhap = chiTietPhieuNhap };

                }
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
                logger.Debug("-------End getPurchaseOrderRequest_ByID-------");
            }
        }

        /// <summary>
        /// Lưu và thay đổi trang thái đơn đặt hàng 
        /// </summary>
        /// <param name="dicData"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> SavePurchaseOrder_Request([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------Start SavePurchaseOrder-------");
                ResponseModel repData = await ResponseFail();
                PhieuNhapHang phieuNhapHang = JsonConvert.DeserializeObject<PhieuNhapHang>(dicData["PurchaseOrder"].ToString());
                List<CT_PhieuNhap> ct_PhieuNhaps = JsonConvert.DeserializeObject<List<CT_PhieuNhap>>(dicData["PurchaseOrderDetail"].ToString());

                int status = Convert.ToInt32(dicData["Status"].ToString());
                if (status == 1)
                {
                    await this.phieuNhapHangReponsitory.AddPurchaseOrderRequest(phieuNhapHang, ct_PhieuNhaps);
                    repData = await ResponseSucceeded();
                    repData.data = new { };
                }
                else
                {
                    bool isUpdate = await this.phieuNhapHangReponsitory.UpdatePurchaseOrderRequest(phieuNhapHang, ct_PhieuNhaps);
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
                logger.Error("Error in SavePurchaseOrder", ex);
                ResponseModel repData = await ResponseException();
                return Ok(repData);
            }
            finally
            {
                logger.Debug("-------End SavePurchaseOrder-------");
            }
        }

        /// <summary>
        /// Lưu và thay đổi trang thái đơn đặt hàng 
        /// </summary>
        /// <param name="dicData">{MaPhieuNhap:"Guid",status:"int"}</param>
        /// <returns>Employees</returns>
        [HttpPost]
        public async Task<ActionResult> DeletePurchaseOrder_Request([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------Start DeletePurchaseOrder_Request-------");
                ResponseModel repData = await ResponseFail();
                Guid maPhieuNhap = Guid.Parse(dicData["MaPhieuNhap"].ToString());
                this.phieuNhapHangReponsitory.DeletePurchaseOrderRequest(maPhieuNhap);
                repData = await ResponseSucceeded();
                repData.data = new { };

                return Ok(repData);
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ và trả về thông báo lỗi
                logger.Error("Error in DeletePurchaseOrder_Request", ex);
                ResponseModel repData = await ResponseException();
                return Ok(repData);
            }
            finally
            {
                logger.Debug("-------End DeletePurchaseOrder_Request-------");
            }
        }

        /// <summary>
        /// xác nhận đơn đặt hàng 
        /// </summary>
        /// <param name="dicData"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> ConfirmPurchaseOrder([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------Start ConfirmPurchaseOrder-------");
                ResponseModel repData = await ResponseFail();
                PhieuNhapHang phieuNhapHang = JsonConvert.DeserializeObject<PhieuNhapHang>(dicData["PurchaseOrder"].ToString());
                List<CT_PhieuNhap> ct_PhieuNhaps = JsonConvert.DeserializeObject<List<CT_PhieuNhap>>(dicData["PurchaseOrderDetail"].ToString());
                this.phieuNhapHangReponsitory.ConfirmPurchaseOrder(phieuNhapHang, ct_PhieuNhaps);
                repData = await ResponseSucceeded();
                repData.data = new { };
                return Ok(repData);
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ và trả về thông báo lỗi
                logger.Error("Error in ConfirmPurchaseOrder", ex);
                ResponseModel repData = await ResponseException();
                return Ok(repData);
            }
            finally
            {
                logger.Debug("-------End ConfirmPurchaseOrder-------");
            }
        }

        /// <summary>
        /// tạo phiếu nhập mới từ phiếu chưa hoàn thành
        /// </summary>
        /// <param name="dicData"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> CreateNewPurchaseOrder([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------Start CreateNewPurchaseOrder-------");
                ResponseModel repData = await ResponseFail();
                Guid maPhieuNhap= Guid.Parse(dicData["MaPhieuNhap"].ToString());

                Guid maPhieuNhapNew = await this.phieuNhapHangReponsitory.CreateNewPurchaseOrder(maPhieuNhap);
                repData = await ResponseSucceeded();
                repData.data = new { maPhieuNhap =maPhieuNhapNew };
                return Ok(repData);
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ và trả về thông báo lỗi
                logger.Error("Error in CreateNewPurchaseOrder", ex);
                ResponseModel repData = await ResponseException();
                return Ok(repData);
            }
            finally
            {
                logger.Debug("-------End CreateNewPurchaseOrder-------");
            }
        }

        /// <summary>
        /// Hàm lấy danh sách tất cả các đơn hàng chưa hoàn tất
        /// </summary>
        /// <param name="dicData">{SearchString:"string",FromDate:"date",ToDate:"Date",PageIndex:"int",PageSize:""}</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> getAllPurchaseOrderNoSubmit([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------End getAllPurchaseOrderNoSubmit-------");
                ResponseModel repData = await ResponseFail();

                int pageIndex = Convert.ToInt32(dicData["PageIndex"].ToString());
                int pageSize = Convert.ToInt32(dicData["PageSize"].ToString());
                string searchString = dicData["SearchString"].ToString();
                DateTime fromDate = DateTime.Parse(dicData["FromDate"].ToString());
                DateTime toDate = DateTime.Parse(dicData["ToDate"].ToString());
                int startRow = (pageIndex - 1) * pageSize;
                int maxRows = pageSize;

                List<PhieuNhapHang> phieuNhapHangs = await this.phieuNhapHangReponsitory.GetAllPurchaseRequest_NoSubmit(fromDate, toDate, searchString, startRow, maxRows);

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
                logger.Debug("-------End getAllPurchaseOrderNoSubmit-------");
            }
        }

    }
}
