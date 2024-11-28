using API_KeoDua.Data;
using API_KeoDua.Models;
using API_KeoDua.Reponsitory.Interface;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
        /// Hàm lấy danh sách tất cả các nhà cung cấp
        /// </summary>
        /// <param name="dicData">{"PageIndex":int,"PageSize":int, "SearchString": "string"}</param>
        /// <returns>Customers</returns>
        [HttpPost]
        public async Task<ActionResult> getAllVendors([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------End getAllVendors-------");
                ResponseModel repData = await ResponseFail();

                int pageIndex = Convert.ToInt32(dicData["PageIndex"].ToString());
                int pageSize = Convert.ToInt32(dicData["PageSize"].ToString());
                string searchString = dicData["SearchString"].ToString();

                int startRow = (pageIndex - 1) * pageSize;
                int maxRow = pageSize;

                List<NhaCungCap> vendors = await this.nhaCungCapReponsitory.GetAllVendors(searchString, startRow, maxRow);

                if (vendors != null && vendors.Any())
                {
                    repData = await ResponseSucceeded();
                }

                repData.data = new { TotalRows = this.nhaCungCapReponsitory.TotalRows, vendors = vendors };
                return Ok(repData);
            }
            catch (Exception ex)
            {
                ResponseModel repData = await ResponseException();
                return Ok(repData);
            }
            finally
            {
                logger.Debug("-------End getAllVendors-------");
            }
        }

        /// <summary>
        /// Hàm lấy thông tin chi tiết nhà cung cấp
        /// </summary>
        /// <param name="dicData">{VendorID: Guid}</param>
        /// <returns>KhachHang</returns>
        [HttpPost]
        public async Task<ActionResult> GetVendorByID([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------End GetVendorByID-------");
                ResponseModel repData = await ResponseFail();

                Guid maNCC = Guid.Parse(dicData["MaNCC"].ToString());
                NhaCungCap vendor = await this.nhaCungCapReponsitory.GetVendorByID(maNCC);

                if (vendor != null)
                {
                    repData = await ResponseSucceeded();
                }

                repData.data = new { Vendor = vendor };
                return Ok(repData);
            }
            catch (Exception ex)
            {
                ResponseModel repData = await ResponseException();
                return Ok(repData);
            }
            finally
            {
                logger.Debug("-------End GetVendorByID-------");
            }
        }

        /// <summary>
        /// Hàm thêm NHÀ CUNG CẤP
        /// </summary>
        /// <param name="dicData">{"NhaCungCap": NhaCungCap}</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> AddVendor([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------End AddVendor-------");
                ResponseModel repData = await ResponseFail();

                NhaCungCap nhaCungCap = JsonConvert.DeserializeObject<NhaCungCap>(dicData["NhaCungCap"].ToString());
                nhaCungCap.MaNCC = Guid.NewGuid();
                await this.nhaCungCapReponsitory.AddVendor(nhaCungCap);
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
                logger.Debug("-------End AddVendor-------");
            }
        }

        /// <summary>
        /// Hàm Xóa nhà cung cấp
        /// </summary>
        /// <param name="dicData">{"MaNCC": Guid}</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> DeleteVendor([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------End DeleteVendor-------");
                ResponseModel repData = await ResponseFail();

                Guid maNCC = Guid.Parse(dicData["MaNCC"].ToString());
                await this.nhaCungCapReponsitory.DeleteVendor(maNCC);
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
                logger.Debug("-------End DeleteVendor-------");
            }
        }

        /// <summary>
        /// Hàm sửa nhà cung cấp
        /// </summary>
        /// <param name="dicData">{NhaCungCap: "NhaCungCap"}</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> UpdateVendor([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------End UpdateVendor-------");
                ResponseModel repData = await ResponseFail();

                NhaCungCap nhaCungCap = JsonConvert.DeserializeObject<NhaCungCap>(dicData["NhaCungCap"].ToString());
                await this.nhaCungCapReponsitory.UpdateVendor(nhaCungCap);
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
                logger.Debug("-------End UpdateVendor-------");
            }
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

                Guid? maNCC = string.IsNullOrWhiteSpace(dicData["MaNCC"]?.ToString())
                 ? null
                 : Guid.Parse(dicData["MaNCC"].ToString());

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
