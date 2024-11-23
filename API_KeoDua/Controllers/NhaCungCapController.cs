using API_KeoDua.Data;
using API_KeoDua.Models;
using API_KeoDua.Reponsitory.Interface;
using Microsoft.AspNetCore.Mvc;

namespace API_KeoDua.Controllers
{
    [Route("~/api/[controller]/[action]")]
    [ApiController]
    public class NhaCungCapController : BaseController
    {
        private readonly NhaCungCapContext nhaCungCapContext;
        private readonly INhaCungCapReponsitory nhaCungCapReponsitory;

        public NhaCungCapController(NhaCungCapContext nhaCungCapContext, INhaCungCapReponsitory nhaCungCapReponsitory)
        {
           this.nhaCungCapContext = nhaCungCapContext;
           this.nhaCungCapReponsitory= nhaCungCapReponsitory;
        }

        /// <summary>
        /// Hàm quick search nhà cung cấp 
        /// </summary>
        /// <param name="dicData">{SearchString:"string"}</param>
        /// <returns>NhaCungCaps</returns>
        [HttpPost]
        public async Task<ActionResult> quickSearchNhaCungCap([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("------- quickSearchNhaCungCap-------");
                ResponseModel repData = await ResponseFail();

                string searchString = dicData["SearchString"].ToString();

                List<NhaCungCap> nhaCungCaps = await this.nhaCungCapReponsitory.QuickSearchNhaCungCap(searchString);

                if (nhaCungCaps != null && nhaCungCaps.Any())
                {
                    repData = await ResponseSucceeded();
                }

                repData.data = new { NhaCungCaps = nhaCungCaps };
                return Ok(repData);
            }
            catch (Exception ex)
            {
                ResponseModel repData = await ResponseException();
                return Ok(repData);
            }
            finally
            {
                logger.Debug("-------End quickSearchNhaCungCap-------");
            }
        }

        /// <summary>
        /// Hàm searchNhaCungCap_byMaNCC 
        /// </summary>
        /// <param name="dicData">{maNCC:"string"}</param>
        /// <returns>NhaCungCaps</returns>
        [HttpPost]
        public async Task<ActionResult> searchNhaCungCap_byMaNCC([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("------- searchNhaCungCap_byMaNCC-------");
                ResponseModel repData = await ResponseFail();

                Guid maNCC = Guid.Parse(dicData["MaNCC"].ToString());

                string tenNCC = await this.nhaCungCapReponsitory.SearchNhaCungCap_ByMaNCC(maNCC);

                if (tenNCC != null)
                {
                    repData = await ResponseSucceeded();
                }

                repData.data = new { tenNCC = tenNCC };
                return Ok(repData);
            }
            catch (Exception ex)
            {
                ResponseModel repData = await ResponseException();
                return Ok(repData);
            }
            finally
            {
                logger.Debug("-------End searchNhaCungCap_byMaNCC-------");
            }
        }


    }
}
