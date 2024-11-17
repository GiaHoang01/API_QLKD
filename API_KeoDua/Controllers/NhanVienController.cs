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
    public class NhanVienController : BaseController
    {
        private readonly NhanVienContext nhanVienContext;
        private readonly INhanVienReponsitory nhanVienReponsitory;
        private readonly TaiKhoanContext taiKhoanContext;
        private readonly ITaiKhoanReponsitory taiKhoanReponsitory;
        private readonly DatabaseConnectionService _dbConnectionService;

        public NhanVienController(NhanVienContext context, INhanVienReponsitory nhanVienReponsitory
            ,TaiKhoanContext taiKhoanContext,ITaiKhoanReponsitory taiKhoanReponsitory,
            DatabaseConnectionService dbConnectionService)
        {
            this.taiKhoanContext = taiKhoanContext;
            this.taiKhoanReponsitory=taiKhoanReponsitory;
            this.nhanVienContext = nhanVienContext;
            this.nhanVienReponsitory = nhanVienReponsitory;
            this._dbConnectionService = dbConnectionService;
        }

        /// <summary>
        /// Hàm lấy danh sách tất cả các nhân viên
        /// </summary>
        /// <param name="dicData">{NameAccount:"string",PasswordAccount:"string"}</param>
        /// <returns>Employees</returns>
        [HttpPost]
        public async Task<ActionResult> getAllEmployees([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------End getAllEmployees-------");
                ResponseModel repData = await ResponseFail();

                int pageIndex = Convert.ToInt32(dicData["PageIndex"].ToString());
                int pageSize = Convert.ToInt32(dicData["PageSize"].ToString());
                string searchString = dicData["SearchString"].ToString();

                int startRow = (pageIndex - 1) * pageSize;
                int maxRows = pageSize;

                List<NhanVien> employees = await this.nhanVienReponsitory.GetAllEmployee(searchString,startRow,maxRows);

                if (employees != null && employees.Any())
                {
                    repData = await ResponseSucceeded();
                }

                repData.data = new { TotalRows = this.nhanVienReponsitory.TotalRows ,Employees = employees };
                return Ok(repData);
            }
            catch (Exception ex)
            {
                ResponseModel repData = await ResponseException();
                return Ok(repData);
            }
            finally
            {
                logger.Debug("-------End getAllEmployees-------");
            }
        }

        /// <summary>
        /// Hàm thêm nhân viên và đăng kí tài khoản
        /// </summary>
        /// <param name="dicData">{}</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> AddAccountEmployee([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------End AddAccountEmployee-------");

                NhanVienTaiKhoan nhanVienTaiKhoan = JsonConvert.DeserializeObject<NhanVienTaiKhoan>(dicData["NhanVienTaiKhoan"].ToString());
             
                ResponseModel repData = await ResponseFail();

                Boolean isCheckCode = await this.taiKhoanReponsitory.IsCheckAccount(nhanVienTaiKhoan.TenTaiKhoan, nhanVienTaiKhoan.MatKhau);

                if (isCheckCode)
                {
                    repData = await ResponseFail();
                    repData.message = "Tài khoản đã tồn tại ";
                    return Ok(repData);
                }
                await this.nhanVienReponsitory.AddEmployee(nhanVienTaiKhoan);
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
                logger.Debug("-------End AddAccountEmployee-------");
            }
        }

        /// <summary>
        /// Hàm lấy chi tiết nhân viên
        /// </summary>
        /// <param name="dicData">{NameAccount:"string",PasswordAccount:"string"}</param>
        /// <returns>Employees</returns>
        [HttpPost]
        public async Task<ActionResult> GetEmployeeByID([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------End getEmployeeByID-------");
                ResponseModel repData = await ResponseFail();
                Guid maNV = Guid.Parse(dicData["MaNV"].ToString());

                NhanVienTaiKhoan employeeAccount = await this.nhanVienReponsitory.GetEmployeeByID(maNV);

                if (employeeAccount != null)
                {
                    repData = await ResponseSucceeded();
                }

                repData.data = new { EmployeeAccount = employeeAccount };
                return Ok(repData);
            }
            catch (Exception ex)
            {
                ResponseModel repData = await ResponseException();
                return Ok(repData);
            }
            finally
            {
                logger.Debug("-------End getEmployeeByID-------");
            }
        }

        /// <summary>
        /// Hàm Xóa nhân viên
        /// </summary>
        /// <param name="dicData">{MaNV:"guid"}</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> DeleteEmployee([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------End DeleteEmployee-------");
                Guid maNV = Guid.Parse(dicData["MaNV"].ToString());
                ResponseModel repData = await ResponseFail();
                await this.nhanVienReponsitory.DeleteEmployee(maNV);
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
                logger.Debug("-------End DeleteEmployee-------");
            }
        }

        /// <summary>
        /// Hàm sửa nhân viên
        /// </summary>
        /// <param name="dicData">{MaNV:"guid",NhanVien:"NhanVien",TaiKhoan:"TaiKhoan"}</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> UpdateEmployee([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------End UpdatetEmployee-------");
                NhanVienTaiKhoan nhanVienTaiKhoan = JsonConvert.DeserializeObject<NhanVienTaiKhoan>(dicData["NhanVienTaiKhoan"].ToString());
                Guid maNV = Guid.Parse(dicData["MaNV"].ToString());

                ResponseModel repData = await ResponseFail();

                await this.nhanVienReponsitory.UpdateEmployee(nhanVienTaiKhoan,maNV);
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
                logger.Debug("-------End UpdatetEmployee-------");
            }
        }

    }
}
