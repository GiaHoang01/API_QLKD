using API_KeoDua.Data;

using API_KeoDua.DataView;
using API_KeoDua.Models;

using API_KeoDua.Reponsitory.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace API_KeoDua.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ChuongTrinhKhuyenMaiController : BaseController
    {
        private readonly HangHoaContext hangHoaContext;
        private readonly IHangHoaReponsitory hangHoaReponsitory;
        private readonly ChuongTrinhKhuyenMaiContext chuongTrinhKhuyenMaiContext;
        private readonly IChuongTrinhKhuyenMaiReponsitory chuongTrinhKhuyenMaiReponsitory;
        private readonly ChiTietCT_KhuyenMaiContext chiTietCT_KhuyenMaiContext;
        private readonly IChiTietCT_KhuyenMaiReponsitory chiTietCT_KhuyenMaiReponsitory;
        public ChuongTrinhKhuyenMaiController(HangHoaContext hangHoacontext, IHangHoaReponsitory hangHoaReponsitory
            , ChuongTrinhKhuyenMaiContext context, IChuongTrinhKhuyenMaiReponsitory chuongTrinhKhuyenMaiReponsitory, ChiTietCT_KhuyenMaiContext chiTietCT_KhuyenMaiContext, IChiTietCT_KhuyenMaiReponsitory chiTietCT_KhuyenMaiReponsitory)
        {
            this.hangHoaContext = hangHoaContext;
            this.hangHoaReponsitory = hangHoaReponsitory;
            this.chuongTrinhKhuyenMaiContext = chuongTrinhKhuyenMaiContext;
            this.chuongTrinhKhuyenMaiReponsitory = chuongTrinhKhuyenMaiReponsitory;
            this.chiTietCT_KhuyenMaiContext = chiTietCT_KhuyenMaiContext;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dicData"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> getAllPromotion([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------End getAllPromotion-------");
                ResponseModel repData = await ResponseFail();

                int pageIndex = Convert.ToInt32(dicData["PageIndex"].ToString());
                int pageSize = Convert.ToInt32(dicData["PageSize"].ToString());
                string searchString = dicData["SearchString"].ToString();

                int startRow = (pageIndex - 1) * pageSize;
                int maxRows = pageSize;

                List<ChuongTrinhKhuyenMai> promotionList = await this.chuongTrinhKhuyenMaiReponsitory.GetAllPromotion(searchString, startRow, maxRows);

                if (promotionList != null && promotionList.Any())
                {
                    repData = await ResponseSucceeded();
                }

                repData.data = new { TotalRows = this.chuongTrinhKhuyenMaiReponsitory.TotalRows, PromotionList = promotionList };
                return Ok(repData);
            }
            catch (Exception ex)
            {
                ResponseModel repData = await ResponseException();
                return Ok(repData);
            }
            finally
            {
                logger.Debug("-------End getAllPromotion-------");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dicData"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> DeletePromotion([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------Start DeletePromotion-------");
                ResponseModel repData = await ResponseFail();
                Guid maKhuyenMai = Guid.Parse(dicData["MaKhuyenMai"].ToString());

                bool isCheck = await this.chuongTrinhKhuyenMaiReponsitory.DeletePromotion(maKhuyenMai);
                if (!isCheck)
                {
                    repData = await ResponseFail();
                    repData.message = "Chương trình khuyến mãi này không thể xóa";

                    return Ok(repData);
                }
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
    }
}
