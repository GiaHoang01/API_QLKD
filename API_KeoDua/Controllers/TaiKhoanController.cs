﻿using Microsoft.AspNetCore.Mvc;
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

                string tendn = await this.taiKhoanReponsitory.login(userName, password);

                if (tendn != null)
                {
                    repData = await ResponseSucceeded();
                }

                repData.data = new { TenDangNhap = tendn };
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



    }


}
