using API_KeoDua.Data;
using API_KeoDua.DataView;
using API_KeoDua.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace API_KeoDua.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BackupRestoreController : BaseController
    {
        private readonly BackupRestoreContext backupRestoreContext;
        private readonly string backupDirectory = "C:/backup";

        public BackupRestoreController( BackupRestoreContext backupRestoreContext)
        {
            this.backupRestoreContext = backupRestoreContext;
            // Tạo thư mục sao lưu nếu chưa tồn tại
            if (!Directory.Exists(backupDirectory))
            {
                Directory.CreateDirectory(backupDirectory);
            }
        }

        [HttpPost]
        public async Task<ActionResult> CreateBackup([FromBody] Dictionary<string, object> dicData)
        {
            logger.Debug("-------Start CreateBackup-------");

            try
            {
                // Khởi tạo đối tượng phản hồi mặc định
                ResponseModel repData = await ResponseFail();
                repData = await ResponseFail();
                string backupFilePath = null;

                // Lấy kết nối từ DbContext và thực thi stored procedure
                using (var connection = backupRestoreContext.CreateConnection())
                {
                    connection.Open();
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "Proc_CreateBackup";
                        command.CommandType = System.Data.CommandType.StoredProcedure;

                        // Thêm tham số OUTPUT để nhận đường dẫn tệp backup
                        var outputParam = command.CreateParameter();
                        outputParam.ParameterName = "@BackupPath";
                        outputParam.DbType = System.Data.DbType.String;
                        outputParam.Size = -1; // NVARCHAR(MAX)
                        outputParam.Direction = System.Data.ParameterDirection.Output;

                        command.Parameters.Add(outputParam);

                        // Thực thi stored procedure
                        command.ExecuteNonQuery();

                        // Lấy giá trị từ tham số OUTPUT
                        backupFilePath = outputParam.Value?.ToString();
                    }
                }

                if (!string.IsNullOrEmpty(backupFilePath) && System.IO.File.Exists(backupFilePath))
                {
                    string relativePath = "C:/backup/" + Path.GetFileName(backupFilePath);

                    repData.data = new { link = relativePath };
                   
                }
                else
                {
                    return BadRequest(new { message = "Tạo sao lưu thất bại, không tìm thấy tệp sao lưu." });
                }
                repData = await ResponseSucceeded();
                return Ok(repData);
            }
            catch (Exception ex)
            {
                logger.Error($"Lỗi khi tạo sao lưu: {ex.Message}", ex);
                return StatusCode(500, new { message = $"Lỗi khi tạo sao lưu: {ex.Message}" });
            }
            finally
            {
                logger.Debug("-------End CreateBackup-------");
            }
        }

        //[HttpPost]
        //public IActionResult CreateBackup()
        //{
        //    try
        //    {
        //        string backupFilePath;

        //        // Lấy kết nối từ DbContext
        //        using (var connection = backupRestoreContext.CreateConnection())
        //        {
        //            connection.Open();

        //            using (var command = connection.CreateCommand())
        //            {
        //                command.CommandText = "Proc_CreateBackup";
        //                command.CommandType = System.Data.CommandType.StoredProcedure;

        //                // Thêm tham số OUTPUT để nhận đường dẫn tệp backup
        //                var outputParam = command.CreateParameter();
        //                outputParam.ParameterName = "@BackupPath";
        //                outputParam.DbType = System.Data.DbType.String;
        //                outputParam.Size = -1; // NVARCHAR(MAX)
        //                outputParam.Direction = System.Data.ParameterDirection.Output;

        //                command.Parameters.Add(outputParam);

        //                // Thực thi proc
        //                command.ExecuteNonQuery();

        //                // Lấy giá trị đường dẫn từ tham số OUTPUT
        //                backupFilePath = outputParam.Value?.ToString();
        //            }
        //        }

        //        // Kiểm tra nếu tệp backup tồn tại
        //        if (System.IO.File.Exists(backupFilePath))
        //        {
        //            string relativePath = "C:/backup/" + Path.GetFileName(backupFilePath); // Tạo link tải tệp
        //            return Ok(new { downloadLink = relativePath });
        //        }
        //        else
        //        {
        //            return BadRequest("Tạo sao lưu thất bại, không tìm thấy tệp sao lưu.");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Lỗi khi tạo sao lưu: {ex.Message}");
        //    }
        //}

        //[HttpPost]
        //public async Task<IActionResult> RestoreBackup([FromBody] Dictionary<string, object> dicData)
        //{
        //    try
        //    {
        //        logger.Debug("-------Start RestoreBackup-------");
        //        ResponseModel repData = await ResponseFail();
        //        string backupFile = dicData["BackupFile"].ToString();
        //        string full = backupDirectory + "/" + backupFile;
        //        using (var connection = backupRestoreContext.CreateConnection())
        //        {
        //            connection.Open();

        //            using (var command = connection.CreateCommand())
        //            {
        //                command.CommandText = "Proc_RestoreBackup";
        //                command.CommandType = System.Data.CommandType.StoredProcedure;
        //                var backupPathParam = command.CreateParameter();
        //                backupPathParam.ParameterName = "@BackupFilePath";
        //                backupPathParam.DbType = System.Data.DbType.String;
        //                backupPathParam.Value = full;

        //                command.Parameters.Add(backupPathParam);

        //                // Thực thi stored procedure
        //                command.ExecuteNonQuery();
        //            }
        //        }
        //        repData = await ResponseSucceeded();
        //        return Ok(repData);
        //    }
        //    catch (Exception ex)
        //    {
        //        ResponseModel repData = await ResponseFail();
        //        repData.message = ex.ToString();
        //        return Ok(repData);
        //    }
        //    finally
        //    {
        //        logger.Debug("-------End RestoreBackup-------");
        //    }
        //}


        [HttpPost]
        public async Task<IActionResult> RestoreBackup([FromBody] Dictionary<string, object> dicData)
        {
            try
            {
                logger.Debug("-------Start RestoreBackup-------");
                ResponseModel repData = await ResponseFail();
                string backupFile = dicData["BackupFile"].ToString();
                string full = backupDirectory + "/" + backupFile;
                using (var connection = backupRestoreContext.CreateConnection())
                {
                    connection.Open();

                    // Đảm bảo chuyển sang cơ sở dữ liệu master trước khi thực hiện phục hồi
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "USE master;";  // Chuyển sang cơ sở dữ liệu master
                        command.ExecuteNonQuery();
                    }

                    // Đặt cơ sở dữ liệu mục tiêu vào chế độ single user (để không có kết nối nào khác)
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "ALTER DATABASE dtb_QuanLyKeoDua SET SINGLE_USER WITH ROLLBACK IMMEDIATE;";
                        command.ExecuteNonQuery();
                    }

                    // Thực hiện phục hồi cơ sở dữ liệu
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "RESTORE DATABASE dtb_QuanLyKeoDua FROM DISK = @BackupFilePath WITH REPLACE;";
                        command.CommandType = System.Data.CommandType.Text;

                        var backupPathParam = command.CreateParameter();
                        backupPathParam.ParameterName = "@BackupFilePath";
                        backupPathParam.DbType = System.Data.DbType.String;
                        backupPathParam.Value = full;

                        command.Parameters.Add(backupPathParam);

                        // Thực thi lệnh phục hồi
                        command.ExecuteNonQuery();
                    }

                    // Đặt lại cơ sở dữ liệu vào chế độ multi-user sau khi phục hồi xong
                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = "ALTER DATABASE dtb_QuanLyKeoDua SET MULTI_USER;";
                        command.ExecuteNonQuery();
                    }
                }

                ResponseModel successResponse = await ResponseSucceeded();
                return Ok(successResponse);
            }
            catch (Exception ex)
            {
                logger.Error("Error during restore backup: " + ex.ToString());

                ResponseModel errorResponse = await ResponseFail();
                errorResponse.message = "An error occurred while restoring the backup: " + ex.Message;
                return StatusCode(500, errorResponse);  // Trả về mã lỗi 500 khi có lỗi
            }
            finally
            {
                logger.Debug("-------End RestoreBackup-------");
            }
        }

    }
}
