using Microsoft.AspNetCore.Mvc;
using API_KeoDua.Data;
using API_KeoDua.Models;
using API_KeoDua.Reponsitory.Interface;
using API_KeoDua;

namespace API_KeoDua.Controllers
{
    [Route("~/api/[controller]/[action]")]
    [ApiController]
    public class TaiKhoanController : BaseController
    {
        private readonly TaiKhoanContext _context;
        private readonly ITaiKhoanReponsitory taiKhoanReponsitory;
        public TaiKhoanController(TaiKhoanContext context, ITaiKhoanReponsitory taiKhoanReponsitory)
        {
            _context = context;
            this.taiKhoanReponsitory = taiKhoanReponsitory;
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
                string passwordAccount =dicData["matKhau"].ToString();
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
        /// Hàm thêm tài khoản (Đăng ký)
        /// </summary>
        /// <param name="dicData">{NameAccount:"string",PasswordAccount:"string"}</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> AddAccount([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------End AddAccount-------");

                string nameAccount = dicData["NameAccount"].ToString();
                string passwordAccount = dicData["PasswordAccount"].ToString();
                TaiKhoan accountsNew = new TaiKhoan();
                accountsNew.TenTaiKhoan = nameAccount;
                accountsNew.MatKhau = passwordAccount;
                accountsNew.TrangThai = "Hoạt động";
                accountsNew.MaNhomQuyen = dicData["MaNhomQuyen"].ToString();
                ResponseModel repData = await ResponseFail();

                Boolean isCheckCode = await this.taiKhoanReponsitory.IsCheckAccount(accountsNew.TenTaiKhoan, accountsNew.MatKhau);

                if (!isCheckCode)
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
                logger.Debug("-------End AddAccount-------");
            }
        }

    }

    
}
