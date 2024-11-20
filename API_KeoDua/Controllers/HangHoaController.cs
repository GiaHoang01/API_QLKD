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
        [HttpPost]
        public async Task<ActionResult> AddProduct([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------Begin AddProduct-------");
                LoaiHangHoa loaiHangHoa = new LoaiHangHoa();
                HangHoa hangHoa=new HangHoa();
                hangHoa.MaHangHoa = Guid.NewGuid();
                hangHoa.TenHangHoa = dicData["TenHangHoa"].ToString();
                hangHoa.MoTa = dicData["MoTa"].ToString();
                hangHoa.HinhAnh = dicData["HinhAnh"].ToString();
                hangHoa.MaLoai = dicData["MaLoai"].ToString();
                LichSuGia lichSuGia = new LichSuGia();
                lichSuGia.GiaBan = Convert.ToDecimal(dicData["GiaBan"].ToString());
                ResponseModel repData = await ResponseFail();

                await this.hangHoaReponsitory.AddProduct(hangHoa, lichSuGia.GiaBan);
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
    }
}
