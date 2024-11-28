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

                List<PhieuGiaoHang> phieuGiaoHangs = await this.phieuGiaoHangReponsitory.GetAllShippingNote(fromDate, toDate, searchString, startRow, maxRows);

                if (phieuGiaoHangs != null && phieuGiaoHangs.Any())
                {
                    repData = await ResponseSucceeded();
                }

                repData.data = new { TotalRows = this.phieuGiaoHangReponsitory.TotalRows, PurchaseOrders = phieuGiaoHangs };
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

    }
}
