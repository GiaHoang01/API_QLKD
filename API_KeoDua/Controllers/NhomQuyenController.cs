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

    }
}
