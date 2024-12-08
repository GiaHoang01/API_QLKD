using Microsoft.AspNetCore.Mvc;
using API_KeoDua.Data;
using API_KeoDua.Models;
using API_KeoDua.Reponsitory.Interface;
using API_KeoDua;
using API_KeoDua.Services;

namespace API_KeoDua.Controllers
{
    [Route("~/api/[controller]/[action]")]
    [ApiController]
    public class TaiKhoanController : BaseController
    {
        private readonly TaiKhoanContext _context;
        private readonly ITaiKhoanReponsitory taiKhoanReponsitory;
        private readonly IConnectionManager _connectionManager;

        public TaiKhoanController(TaiKhoanContext context, ITaiKhoanReponsitory taiKhoanReponsitory, IConnectionManager connectionManager)
        {
            _context = context;
            this.taiKhoanReponsitory = taiKhoanReponsitory;
            _connectionManager = connectionManager;
        }
        /// <summary>
        /// Hàm kiem tra tất cả các tài khoản co ton tai khong
        /// </summary>
        /// <param name="dicData">{NameAccount:"string",PasswordAccount:"string"}</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> IsCheckAccount([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------Begin IsCheckAccount-------");
                string nameAccount = dicData["tenTaiKhoan"].ToString();
                string passwordAccount = dicData["matKhau"].ToString();
                ResponseModel repData = await ResponseFail();

                Boolean isCheckAccount = await this.taiKhoanReponsitory.IsCheckAccount(nameAccount, passwordAccount);
                if (isCheckAccount)
                {
                    repData = await ResponseSucceeded();
                }
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
                logger.Debug("-------End IsCheckAccount-------");
            }
        }

        /// <summary>
        /// Hàm đăng nhập 
        /// </summary>
        /// <param name="dicData">{UserName:"string",PassWord:"string"}</param>
        /// <returns> TenDangNhap</returns>
        [HttpPost]
        public async Task<ActionResult> Login([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------End Login-------");
                ResponseModel repData = await ResponseFail();
                string userName = dicData["UserName"].ToString();
                string password = dicData["PassWord"].ToString();
                _connectionManager.SetConnectionString(userName, password);
                string tendn = await this.taiKhoanReponsitory.login(userName, password);

                if (tendn != null)
                {
                    repData = await ResponseSucceeded();
                }

                repData.data = new { TenDangNhap = tendn,UserName=userName };
                return Ok(repData);
            }
            catch (Exception ex)
            {
                ResponseModel repData = await ResponseException();
                return Ok(repData);
            }
            finally
            {
                logger.Debug("-------End Login-------");
            }
        }

        /// <summary>
        /// Hàm lấy tất cả các quyền của user truyền vào 
        /// </summary>
        /// <param name="dicData">{UserName:"string"}</param>
        /// <returns> Quyens</returns>
        [HttpPost]
        public async Task<ActionResult> getPermission([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------Start getPermission-------");
                ResponseModel repData = await ResponseFail();
                string userName = dicData["UserName"].ToString();

                List<string> quyens = await this.taiKhoanReponsitory.getDataPermission(userName);

                if (quyens != null)
                {
                    repData = await ResponseSucceeded();
                }

                repData.data = new { Quyens = quyens };
                return Ok(repData);
            }
            catch (Exception ex)
            {
                ResponseModel repData = await ResponseException();
                return Ok(repData);
            }
            finally
            {
                logger.Debug("-------End getPermission-------");
            }
        }

        /// <summary>
        /// Hàm lấy danh sách tất cả các tên nhân viên
        /// </summary>
        /// <param name="dicData"></param>
        /// <returns>Employees</returns>
        [HttpPost]
        public async Task<ActionResult> getAllNameAccount([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------End getAllNameAccount-------");
                ResponseModel repData = await ResponseFail();


                List<string> NameAccount = await this.taiKhoanReponsitory.GetAccountName();

                if (NameAccount != null && NameAccount.Any())
                {
                    repData = await ResponseSucceeded();
                }

                repData.data = new { NameAccount = NameAccount };
                return Ok(repData);
            }
            catch (Exception ex)
            {
                ResponseModel repData = await ResponseException();
                return Ok(repData);
            }
            finally
            {
                logger.Debug("-------End getAllNameEmployees-------");
            }
        }


        /// <summary>
        /// Hàm đổi mật khẩu
        /// </summary>
        /// <param name="dicData"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> resetPass([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------End resetPass-------");
                ResponseModel repData = await ResponseFail();
                string userName = dicData["UserName"].ToString();
                string passwordnew = dicData["PassWordNew"].ToString();
                bool isCheck=await this.taiKhoanReponsitory.ChangePasswordAsync(userName, passwordnew);
                if (!isCheck)
                {
                    repData.message = "thay đổi mật khẩu thất bại";
                    repData.data = new { };
                    return Ok(repData);
                }
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
                logger.Debug("-------End resetPass-------");
            }
        }


    }


}
