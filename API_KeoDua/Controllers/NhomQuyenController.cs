using Microsoft.AspNetCore.Mvc;
using API_KeoDua.Data;
using API_KeoDua.Models;
using API_KeoDua.Reponsitory.Interface;

namespace API_KeoDua.Controllers
{
    [Route("~/api/[controller]/[action]")]
    [ApiController]
    public class NhomQuyenController : BaseController
    {
        private readonly NhomQuyenContext nhomQuyenContext;
        private readonly INhomQuyenRepository nhomQuyenRepository;
        public NhomQuyenController( NhomQuyenContext nhomQuyenContext,INhomQuyenRepository nhomQuyenRepository)
        {
            this.nhomQuyenContext = nhomQuyenContext;
            this.nhomQuyenRepository = nhomQuyenRepository;
        }

        /// <summary>
        /// Hàm quick search nhóm quyền 
        /// </summary>
        /// <param name="dicData">{SearchString:"string"}</param>
        /// <returns>NhomQuyen</returns>
        [HttpPost]
        public async Task<ActionResult> quickSearchNhomQuyen([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("------- quickSearchNhomQuyen-------");
                ResponseModel repData = await ResponseFail();

                string searchString = dicData["SearchString"].ToString();

                List<NhomQuyen> nhomQuyen = await this.nhomQuyenRepository.quickSearchNhomQuyen(searchString);

                if (nhomQuyen != null && nhomQuyen.Any())
                {
                    repData = await ResponseSucceeded();
                }

                repData.data = new {NhomQuyens = nhomQuyen };
                return Ok(repData);
            }
            catch (Exception ex)
            {
                ResponseModel repData = await ResponseException();
                return Ok(repData);
            }
            finally
            {
                logger.Debug("-------End quickSearchNhomQuyen-------");
            }
        }


        /// <summary>
        /// Hàm lấy danh sách tất cả các nhóm quyền theo tên nhan viên
        /// </summary>
        /// <param name="dicData"></param>
        /// <returns>Employees</returns>
        [HttpPost]
        public async Task<ActionResult> GetNhomQuyen([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------End GetNhomQuyen-------");
                ResponseModel repData = await ResponseFail();

                List<NhomQuyen> nhomQuyens = await this.nhomQuyenRepository.GetNhomQuyen();
                repData = await ResponseSucceeded();
                repData.data = new { nhomQuyens = nhomQuyens };
                return Ok(repData);
            }
            catch (Exception ex)
            {
                ResponseModel repData = await ResponseException();
                return Ok(repData);
            }
            finally
            {
                logger.Debug("-------End GetNhomQuyen-------");
            }
        }

        /// <summary>
        /// Hàm lấy danh sách tất cả các quyền dựa vào ten nhan vien
        /// </summary>
        /// <param name="dicData"></param>
        /// <returns>Employees</returns>
        [HttpPost]
        public async Task<ActionResult> GetQuyenByTenNhomQuyen([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------End GetQuyenByTenNhomQuyen-------");
                ResponseModel repData = await ResponseFail();

                string TenNQ = dicData["TenNQ"].ToString();
                List<Quyen> quyens = await this.nhomQuyenRepository.GetQuyenByTenNhomQuyen(TenNQ);
                repData = await ResponseSucceeded();
                repData.data = new { quyens = quyens };
                return Ok(repData);
            }
            catch (Exception ex)
            {
                ResponseModel repData = await ResponseException();
                return Ok(repData);
            }
            finally
            {
                logger.Debug("-------End GetQuyenByTenNhomQuyen-------");
            }
        }

        /// <summary>
        /// Hàm update quyen
        /// </summary>
        /// <param name="dicData"></param>
        /// <returns>Employees</returns>
        [HttpPost]
        public async Task<ActionResult> UpdateRole([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------End UpdateRole-------");
                ResponseModel repData = await ResponseFail();

                string tenNhomQuyen = dicData["TenNhomQuyen"].ToString();
                string tenTaiKhoan = dicData["TenTaiKhoan"].ToString();
                await this.nhomQuyenRepository.UpdateRole(tenTaiKhoan,tenNhomQuyen);
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
                logger.Debug("-------End UpdateRole-------");
            }
        }

    }
}
