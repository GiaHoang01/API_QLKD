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
    public class HangHoaController : BaseController
    {
        private readonly HangHoaContext hangHoaContext;
        private readonly IHangHoaReponsitory hangHoaReponsitory;
        private readonly LichSuGiaContext lichSuGiaContext;
        private readonly ILichSuGiaReponsitory lichSuGiaReponsitory;
        public HangHoaController(HangHoaContext context, IHangHoaReponsitory hangHoaReponsitory
            , LichSuGiaContext lichSuGiaContext, ILichSuGiaReponsitory lichSuGiaReponsitory)
        {
            this.hangHoaContext = context;
            this.hangHoaReponsitory = hangHoaReponsitory;
            this.lichSuGiaContext = lichSuGiaContext;
            this.lichSuGiaReponsitory = lichSuGiaReponsitory;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dicData"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> getAllProduct([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------End getAllProduct-------");
                ResponseModel repData = await ResponseFail();

                int pageIndex = Convert.ToInt32(dicData["PageIndex"].ToString());
                int pageSize = Convert.ToInt32(dicData["PageSize"].ToString());
                string searchString = dicData["SearchString"].ToString();

                int startRow = (pageIndex - 1) * pageSize;
                int maxRows = pageSize;

                List<HangHoaLichSuGia> productList = await this.hangHoaReponsitory.GetAllProduct(searchString, startRow, maxRows);

                if (productList != null && productList.Any())
                {
                    repData = await ResponseSucceeded();
                }

                repData.data = new { TotalRows = this.hangHoaReponsitory.TotalRows, ProductList = productList };
                return Ok(repData);
            }
            catch (Exception ex)
            {
                ResponseModel repData = await ResponseException();
                return Ok(repData);
            }
            finally
            {
                logger.Debug("-------End getAllProduct-------");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dicData"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> getAllProductInStock([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------End getAllProduct-------");
                ResponseModel repData = await ResponseFail();

                int pageIndex = Convert.ToInt32(dicData["PageIndex"].ToString());
                int pageSize = Convert.ToInt32(dicData["PageSize"].ToString());
                string searchString = dicData["SearchString"].ToString();

                int startRow = (pageIndex - 1) * pageSize;
                int maxRows = pageSize;

                List<HangHoaLichSuGia> productList = await this.hangHoaReponsitory.GetAllProductInStock(searchString, startRow, maxRows);

                if (productList != null && productList.Any())
                {
                    repData = await ResponseSucceeded();
                }

                repData.data = new { TotalRows = this.hangHoaReponsitory.TotalRows, ProductList = productList };
                return Ok(repData);
            }
            catch (Exception ex)
            {
                ResponseModel repData = await ResponseException();
                return Ok(repData);
            }
            finally
            {
                logger.Debug("-------End getAllProduct-------");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dicData"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> AddProduct([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------Begin AddProduct-------");
                HangHoa hangHoa = JsonConvert.DeserializeObject<HangHoa>(dicData["HangHoa"].ToString());
                hangHoa.MaHangHoa = Guid.NewGuid();
                //hangHoa.TenHangHoa = dicData["TenHangHoa"].ToString();
                //hangHoa.MoTa = dicData["MoTa"].ToString();
                //hangHoa.HinhAnh = dicData["HinhAnh"].ToString();
                //hangHoa.MaLoai = dicData["MaLoai"].ToString();
                LichSuGia lichSuGia = new LichSuGia();
                lichSuGia.GiaBan = Convert.ToDecimal(dicData["GiaBan"].ToString());
                lichSuGia.GhiChu = dicData.ContainsKey("GhiChu") && dicData["GhiChu"] != null
                ? dicData["GhiChu"].ToString()
                : null;
                ResponseModel repData = await ResponseFail();

                await this.hangHoaReponsitory.AddProduct(hangHoa, lichSuGia.GiaBan, lichSuGia.GhiChu);
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
                logger.Debug("-------Begin AddProduct-------");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dicData"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> UpdateProduct([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------Begin UpdateProduct-------");
                ResponseModel repData = await ResponseFail();
                //if (!dicData.ContainsKey("MaHangHoa") || !dicData.ContainsKey("GiaBan"))
                //{
                //    repData.message = "Thiếu thông tin mã hàng hóa hoặc giá bán.";
                //    return Ok(repData); // Trả về nếu thiếu thông tin quan trọng
                //}
                HangHoa hangHoa = JsonConvert.DeserializeObject<HangHoa>(dicData["HangHoa"].ToString());
                LichSuGia lichSuGia = new LichSuGia();
                lichSuGia.GiaBan = Convert.ToDecimal(dicData["GiaBan"].ToString());
                lichSuGia.GhiChu = dicData.ContainsKey("GhiChu") && dicData["GhiChu"] != null
                ? dicData["GhiChu"].ToString()
                : null;
                if (await (this.hangHoaReponsitory.UpdateProduct(hangHoa, lichSuGia.GiaBan, lichSuGia.GhiChu)))
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
                logger.Debug("-------Begin getTenHangHoa_withByMaHangHoa-------");

                logger.Debug("-------End UpdateProduct-------");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dicData"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> DeleteProduct([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------End DeleteProduct-------");
                Guid maHangHoa = Guid.Parse(dicData["MaHangHoa"].ToString());
                ResponseModel repData = await ResponseFail();
                await this.hangHoaReponsitory.DeleteProduct(maHangHoa);
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
                logger.Debug("-------End DeleteProduct-------");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dicData"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> getPriceHistoryProduct([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------End getAllProduct-------");
                ResponseModel repData = await ResponseFail();

                Guid maHangHoa = Guid.Parse(dicData["MaHangHoa"].ToString());

                List<LichSuGia> priceList = await this.hangHoaReponsitory.GetPriceHistoryProduct(maHangHoa);

                if (priceList != null && priceList.Any())
                {
                    repData = await ResponseSucceeded();
                }
                repData.data = new { PriceList = priceList };
                return Ok(repData);
            }
            catch (Exception ex)
            {
                ResponseModel repData = await ResponseException();
                return Ok(repData);
            }
            finally
            {
                logger.Debug("-------End getAllProduct-------");
            }
        }

        /// <summary>
        /// Hàm quick search hàng hóa 
        /// </summary>
        /// <param name="dicData">{SearchString:"string"}</param>
        /// <returns>NhaCungCaps</returns>
        [HttpPost]
        public async Task<ActionResult> quickSearchHangHoa([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("------- quickSearchHangHoa-------");
                ResponseModel repData = await ResponseFail();

                string searchString = dicData["SearchString"].ToString();

                List<HangHoaLichSuGia> hangHoas = await this.hangHoaReponsitory.QuickSearchHangHoa(searchString);

                if (hangHoas != null && hangHoas.Any())
                {
                    repData = await ResponseSucceeded();
                }

                repData.data = new { HangHoas = hangHoas };
                return Ok(repData);
            }
            catch (Exception ex)
            {
                ResponseModel repData = await ResponseException();
                return Ok(repData);
            }
            finally
            {
                logger.Debug("-------End quickSearchHangHoa-------");
            }
        }

        /// <summary>
        /// Lấy tên hàng hóa dựa vào mã 
        /// </summary>
        /// <param name="dicData"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> getTenHangHoa_withByMaHangHoa([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------Begin getTenHangHoa_withByMaHangHoa-------");

                ResponseModel repData = await ResponseFail();
                Guid maHangHoa = Guid.Parse(dicData["MaHangHoa"].ToString());
                string tenHangHoa = await this.hangHoaReponsitory.getTenHangHoa_withByMaHangHoa(maHangHoa);
                repData = await ResponseSucceeded();
                repData.data = new { TenHangHoa = tenHangHoa };
                return Ok(repData);
            }
            catch (Exception ex)
            {
                ResponseModel repData = await ResponseException();
                return Ok(repData);
            }
            finally
            {
                logger.Debug("-------Begin getTenHangHoa_withByMaHangHoa-------");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dicData"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> getGiaBan_withByMaHangHoa([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------Begin getGiaBan_withByMaHangHoa-------");

                ResponseModel repData = await ResponseFail();
                Guid maHangHoa = Guid.Parse(dicData["MaHangHoa"].ToString());
                decimal giaBan = await this.hangHoaReponsitory.getGiaBan_withByMaHangHoa(maHangHoa);
                repData = await ResponseSucceeded();
                repData.data = new { GiaBan = giaBan };
                return Ok(repData);
            }
            catch (Exception ex)
            {
                ResponseModel repData = await ResponseException();
                return Ok(repData);
            }
            finally
            {
                logger.Debug("-------Begin getGiaBan_withByMaHangHoa-------");
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dicData"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> getSoLuongTon([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------Begin getSoLuongTon-------");

                ResponseModel repData = await ResponseFail();
                Guid maHangHoa = Guid.Parse(dicData["MaHangHoa"].ToString());
                int soLuongTon = await this.hangHoaReponsitory.GetSoLuongTon(maHangHoa);
                repData = await ResponseSucceeded();
                repData.data = new { SoLuongTon = soLuongTon };
                return Ok(repData);
            }
            catch (Exception ex)
            {
                ResponseModel repData = await ResponseException();
                return Ok(repData);
            }
            finally
            {
                logger.Debug("-------Begin getGiaBan_withByMaHangHoa-------");
            }
        }
    }
}
