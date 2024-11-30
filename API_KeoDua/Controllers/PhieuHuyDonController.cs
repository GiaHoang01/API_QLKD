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
    public class PhieuHuyDonController : BaseController
    {
        private readonly PhieuHuyDonConText phieuHuyDonContext;
        private readonly IPhieuHuyDonReponsitory phieuHuyDonReponsitory;

        public PhieuHuyDonController(PhieuHuyDonConText phieuHuyDonContext, IPhieuHuyDonReponsitory phieuHuyDonReponsitory)
        {
            this.phieuHuyDonContext = phieuHuyDonContext;
            this.phieuHuyDonReponsitory = phieuHuyDonReponsitory;
        }

        /// <summary>
        /// Hàm lấy danh sách phiếu hủy đơn
        /// </summary>
        /// <param name="dicData">{FromDate:"date",ToDate:"Date",PageIndex:"int",PageSize:""}</param>
        /// <returns>Employees</returns>
        [HttpPost]
        public async Task<ActionResult> getAllShippingNoteCancel([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------End getAllShippingNoteCancel-------");
                ResponseModel repData = await ResponseFail();

                int pageIndex = Convert.ToInt32(dicData["PageIndex"].ToString());
                int pageSize = Convert.ToInt32(dicData["PageSize"].ToString());
                DateTime fromDate = DateTime.Parse(dicData["FromDate"].ToString());
                DateTime toDate = DateTime.Parse(dicData["ToDate"].ToString());
                int startRow = (pageIndex - 1) * pageSize;
                int maxRows = pageSize;

                List<PhieuHuyDon> phieuHuyDons = await this.phieuHuyDonReponsitory.GetAllShippingNoteCancel(fromDate, toDate, startRow, maxRows);

                if (phieuHuyDons != null)
                {
                    repData = await ResponseSucceeded();
                }

                repData.data = new { TotalRows = this.phieuHuyDonReponsitory.TotalRows, ShippingNotesCancel = phieuHuyDons };
                return Ok(repData);
            }
            catch (Exception ex)
            {
                ResponseModel repData = await ResponseException();
                return Ok(repData);
            }
            finally
            {
                logger.Debug("-------End getAllShippingNoteCancel-------");
            }
        }

        /// <summary>
        /// Hàm lấy thông tin chi tiết phiếu hủy đơn
        /// </summary>
        /// <param name="dicData">{MaPhieuHuy: Guid}</param>
        /// <returns>KhachHang</returns>
        [HttpPost]
        public async Task<ActionResult> GetShippingNoteCancelByID([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------Start GetShippingNoteCancelByID-------");
                ResponseModel repData = await ResponseFail();

                Guid? maPhieuGiao = dicData.ContainsKey("MaPhieuHuy") && !string.IsNullOrEmpty(dicData["MaPhieuHuy"]?.ToString()) ? Guid.Parse(dicData["MaPhieuHuy"].ToString()) : (Guid?)null;
                int status = Convert.ToInt32(dicData["Status"].ToString());

                if (status == 1)//Thêm mới
                {
                    PhieuHuyDon phieuHuy = new PhieuHuyDon();
                    phieuHuy.MaPhieuHuy = Guid.NewGuid();
                    phieuHuy.NgayHuy = DateTime.Now;
                    phieuHuy.LyDo = null;
                    repData = await ResponseSucceeded();
                    repData.data = new { PhieuHuyDon = phieuHuy };
                }
                else
                {
                    dynamic phieuHuy = await this.phieuHuyDonReponsitory.GetShippingNoteCancelByID(maPhieuGiao);
                    if (phieuHuy == null)
                    {
                        repData = await ResponseFail();
                    }
                    else
                    {
                        repData = await ResponseSucceeded();
                        repData.data = new { PhieuHuyDon = phieuHuy };
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
                logger.Debug("-------End GetShippingNoteCancelByID-------");
            }
        }

        /// <summary>
        /// Lưu phiếu giao hàng
        /// </summary>
        /// <param name="dicData">{MaPhieuGiao: Guid}</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> SaveShippingNoteCancel([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------Start SaveShippingNoteCancel-------");
                ResponseModel repData = await ResponseFail();

                PhieuHuyDon phieuHuyDon = JsonConvert.DeserializeObject<PhieuHuyDon>(dicData["PhieuHuyDon"].ToString());
                //int status = Convert.ToInt32(dicData["Status"].ToString());
                //if (status == 1)
                //{
                //    phieuHuyDon.NgayHuy = DateTime.Now;
                //    phieuHuyDon.MaPhieuGiao = maPhieuGiao;
                //    await this.phieuHuyDonReponsitory.AddShippingNoteCancel(phieuHuyDon);
                //}
                //else
                //{
                //    await this.phieuHuyDonReponsitory.UpdateShippingNoteCancel(phieuHuyDon.MaPhieuHuy, maPhieuGiao);
                //}
                phieuHuyDon.NgayHuy = DateTime.Now;
                await this.phieuHuyDonReponsitory.AddShippingNoteCancel(phieuHuyDon);
                repData = await ResponseSucceeded();
                repData.data = new { status = 2 };
                return Ok(repData);
            }
            catch (Exception ex)
            {
                ResponseModel repData = await ResponseException();
                return Ok(repData);
            }
            finally
            {
                logger.Debug("-------End SaveShippingNoteCancel-------");
            }
        }

        /// <summary>
        /// Xóa phiếu giao hàng
        /// </summary>
        /// <param name="dicData">{MaPhieuGiao: Guid}</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> DeleteShippingNoteCancel([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------End DeleteShippingNoteCancel-------");
                ResponseModel repData = await ResponseFail();

                Guid maPhieuHuy = (dicData["MaPhieuHuy"] == null || string.IsNullOrEmpty(dicData["MaPhieuHuy"].ToString())) ? Guid.Empty : Guid.Parse(dicData["MaPhieuHuy"].ToString());

                await this.phieuHuyDonReponsitory.DeleteShippingNoteCancel(maPhieuHuy);

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
                logger.Debug("-------End DeleteShippingNoteCancel-------");
            }
        }
    }
}
