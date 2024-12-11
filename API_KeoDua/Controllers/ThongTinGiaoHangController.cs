using API_KeoDua.Data;
using API_KeoDua.DataView;
using API_KeoDua.Models;
using API_KeoDua.Reponsitory.Interface;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace API_KeoDua.Controllers
{
    [Route("~/api/[controller]/[action]")]
    [ApiController]
    public class ThongTinGiaoHangController: BaseController
    {
        private readonly ThongTinGiaoHangContext thongTinGiaoHangContext;
        private readonly IThongTinGiaoHangReponsitory thongTinGiaoHangReponsitory;

        public ThongTinGiaoHangController(ThongTinGiaoHangContext thongTinGiaoHangContext, IThongTinGiaoHangReponsitory thongTinGiaoHangReponsitory)
        {
            this.thongTinGiaoHangContext = thongTinGiaoHangContext;
            this.thongTinGiaoHangReponsitory = thongTinGiaoHangReponsitory;
        }

        /// <summary>
        /// Hàm lấy danh sách thông tin giao hàng của khách hàng
        /// </summary>
        /// <param name="dicData"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> getAllInfoDelivery([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------Start getAllInfoDelivery-------");
                ResponseModel repData = await ResponseFail();

                Guid maKH = Guid.Parse(dicData["MaKhachHang"].ToString());
                List<ThongTinGiaoHang> shippingInfo = await this.thongTinGiaoHangReponsitory.GetAllInfoDelivery(maKH);
                repData = await ResponseSucceeded();

                repData.data = new { ShippingInfo = shippingInfo };
                return Ok(repData);
            }
            catch (Exception ex)
            {
                ResponseModel repData = await ResponseException();
                return Ok(repData);
            }
            finally
            {
                logger.Debug("-------End getAllInfoDelivery-------");
            }
        }

        /// <summary>
        /// Hàm thêm thông tin giao hàng
        /// </summary>
        /// <param name="dicData">{"ThongTinGiaoHang": ThongTinGiaoHang}</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> AddInfoDelivery([FromBody] Dictionary<string, object> dicData)
        {
            logger.Debug("-------Start AddInfoDelivery-------");

            ResponseModel repData = await ResponseFail();

            try
            {
                Guid maKhachHang = Guid.Parse(dicData["MaKH"].ToString());
               
                // Deserialize ThongTinGiaoHang
                ThongTinGiaoHang ttGiaoHang = JsonConvert.DeserializeObject<ThongTinGiaoHang>(dicData["ThongTinGiaoHang"].ToString());
                // Gán giá trị cho MaThongTin
                ttGiaoHang.MaThongTin = Guid.NewGuid();

                // Thêm thông tin giao hàng vào cơ sở dữ liệu
                bool isAdded = await this.thongTinGiaoHangReponsitory.AddInfoDelivery(maKhachHang, ttGiaoHang);
                if (!isAdded)
                {
                    repData = await ResponseFail();
                    return Ok(repData);
                }

                // Phản hồi thành công
                repData = await ResponseSucceeded();
                repData.data = new { };
                return Ok(repData);
            }
            catch (Exception ex)
            {
                repData = await ResponseException();
                return Ok(repData);
            }
            finally
            {
                logger.Debug("-------End AddInfoDelivery-------");
            }
        }

        [HttpPost]
        public async Task<ActionResult> DeleteInfoDelivery([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------Start DeleteInfoDelivery-------");
                ResponseModel repData = await ResponseFail();

                // Lấy mã khách hàng và mã thông tin từ request body
                Guid maKhachHang = Guid.Parse(dicData["MaKhachHang"].ToString());
                Guid maThongTin = Guid.Parse(dicData["MaThongTin"].ToString());

                // Gọi repository để xóa thông tin
                bool isDeleted = await this.thongTinGiaoHangReponsitory.DeleteInfoDelivery(maKhachHang, maThongTin);

                if (isDeleted)
                {
                    repData = await ResponseSucceeded();
                    repData.data = new { Message = "Delete successful." };
                }
                else
                {
                    repData = await ResponseFail();
                    repData.data = new { Message = "No records found to delete." };
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
                logger.Debug("-------End DeleteInfoDelivery-------");
            }
        }

        [HttpPost]
        public async Task<ActionResult> UpdateInfoDelivery([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------Start UpdateInfoDelivery-------");
                ResponseModel repData = await ResponseFail();

                // Lấy mã khách hàng và mã thông tin từ request body
                Guid maKhachHang = Guid.Parse(dicData["MaKhachHang"].ToString());
                Guid maThongTin = Guid.Parse(dicData["MaThongTin"].ToString());

                // Deserialize thông tin giao hàng
                ThongTinGiaoHang thongTinGiaoHang = JsonConvert.DeserializeObject<ThongTinGiaoHang>(dicData["ThongTinGiaoHang"].ToString());

                // Gọi repository để cập nhật thông tin
                bool isUpdated = await this.thongTinGiaoHangReponsitory.UpdateInfoDelivery(maKhachHang, maThongTin, thongTinGiaoHang);

                if (isUpdated)
                {
                    repData = await ResponseSucceeded();
                    repData.data = new { Message = "Update successful." };
                }
                else
                {
                    repData.data = new { Message = "No records found to update." };
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
                logger.Debug("-------End UpdateInfoDelivery-------");
            }
        }

    }
}
