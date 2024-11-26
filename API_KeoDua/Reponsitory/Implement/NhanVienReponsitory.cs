using API_KeoDua.Data;
using API_KeoDua.Reponsitory.Interface;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Text;
using Dapper;
using Microsoft.Data.SqlClient;
using API_KeoDua.DataView;
using Microsoft.AspNetCore.Mvc;

namespace API_KeoDua.Reponsitory.Implement
{
    public class NhanVienReponsitory : INhanVienReponsitory
    {
        private readonly NhanVienContext nhanVienContext;
        private readonly TaiKhoanContext taiKhoanContext;
        public NhanVienReponsitory(NhanVienContext nhanVienContext,TaiKhoanContext taiKhoanContext)
        {
            this.nhanVienContext = nhanVienContext;
            this.taiKhoanContext= taiKhoanContext;
        }

        public int TotalRows { get; set; }

        public async Task<List<NhanVien>> GetAllEmployee(string searchString, int startRow, int maxRows)
        {
            try
            {
                string sqlWhere = "";
                var param = new DynamicParameters();

                if (!string.IsNullOrEmpty(searchString))
                {
                    // Sử dụng TRIM() để loại bỏ khoảng trắng thừa và COLLATE để so sánh không phân biệt chữ hoa chữ thường
                    sqlWhere += " WHERE TenNV COLLATE SQL_Latin1_General_CP1_CI_AS LIKE @SearchString OR SDT LIKE @SearchString";
                    param.Add("@SearchString", $"%{searchString}%");
                }


                // Tạo câu truy vấn với điều kiện WHERE và phân trang
                string sqlQuery = $@"
                    SELECT COUNT(1) FROM tbl_NhanVien WITH (NOLOCK) {sqlWhere};
                    SELECT * FROM tbl_NhanVien WITH (NOLOCK) {sqlWhere}
                    ORDER BY TenNV ASC
                    OFFSET @StartRow ROWS FETCH NEXT @MaxRows ROWS ONLY;";

                param.Add("@StartRow", startRow);
                param.Add("@MaxRows", maxRows);

                using (var connection = this.nhanVienContext.CreateConnection())
                {
                    using (var multi = await connection.QueryMultipleAsync(sqlQuery, param))
                    {
                        // Lấy tổng số hàng từ truy vấn đầu tiên
                        this.TotalRows = (await multi.ReadAsync<int>()).Single();
                        // Lấy danh sách nhân viên từ truy vấn thứ hai
                        return (await multi.ReadAsync<NhanVien>()).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                // Ghi log hoặc xử lý ngoại lệ
                throw new Exception("An error occurred while fetching employees", ex);
            }
        }

        public async Task AddEmployee(NhanVienTaiKhoan nhanVienTaiKhoan)
        {
            using var transaction = await nhanVienContext.Database.BeginTransactionAsync();
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@Username", nhanVienTaiKhoan.TenTaiKhoan),
                    new SqlParameter("@Password", nhanVienTaiKhoan.MatKhau ),
                    new SqlParameter("@Email", nhanVienTaiKhoan.Email),
                    new SqlParameter("@TenNhanVien",nhanVienTaiKhoan.TenNV), 
                    new SqlParameter("@SoDT", nhanVienTaiKhoan.SDT),
                    new SqlParameter("@DiaChi", nhanVienTaiKhoan.DiaChi),     
                    new SqlParameter("@GioiTinh", nhanVienTaiKhoan.GioiTinh),
                    new SqlParameter("@NgaySinh", nhanVienTaiKhoan.NgaySinh != null ? nhanVienTaiKhoan.NgaySinh : DBNull.Value),
                };


                string storedProcedure = nhanVienTaiKhoan.MaNhomQuyen switch
                {
                    "NQ00000001" => "EXEC AddAccountQuanLi @Username, @Password, @Email, @TenNhanVien, @SoDT, @DiaChi, @GioiTinh, @NgaySinh",
                    "NQ00000002" => "EXEC AddAccountNhanVienGiaoHang @Username, @Password, @Email, @TenNhanVien, @SoDT, @DiaChi, @GioiTinh, @NgaySinh",
                    "NQ00000003" => "EXEC AddAccountNhanVienKho @Username, @Password, @Email, @TenNhanVien, @SoDT, @DiaChi, @GioiTinh, @NgaySinh",
                    "NQ00000004" => "EXEC AddAccountNhanVienBanHang @Username, @Password, @Email, @TenNhanVien, @SoDT, @DiaChi, @GioiTinh, @NgaySinh",
                    _ => throw new ArgumentException("Invalid role (MaNhomQuyen)."),
                };

                // Execute the stored procedure
                await nhanVienContext.Database.ExecuteSqlRawAsync(storedProcedure, parameters);

                // Commit the transaction
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("An error occurred while adding the employee and account", ex);
            }
            

        }

        public async Task<NhanVienTaiKhoan> GetEmployeeByID(Guid? MaNV)
        {
            try
            {
                string sqlQuery = @"
                    SELECT tk.TenTaiKhoan, tk.MatKhau, tk.MaNhomQuyen, nv.TenNV, nv.Email, nv.ChucVu, nv.DiaChi, 
                           nv.SDT, nv.NgaySinh, nv.GioiTinh,nv.NgayVaoLam
                    FROM tbl_NhanVien nv
                    LEFT JOIN tbl_TaiKhoan tk ON nv.TenTaiKhoan = tk.TenTaiKhoan
                    WHERE nv.MaNV = @MaNV";

                var param = new DynamicParameters(); 
                param.Add("@MaNV", MaNV);

                using (var connection = this.nhanVienContext.CreateConnection())
                {
                    var result = await connection.QueryFirstOrDefaultAsync<NhanVienTaiKhoan>(sqlQuery, param);

                    return result;
                }
            }
            catch (Exception ex)
            {
                // Ghi log hoặc xử lý ngoại lệ
                throw new Exception("An error occurred while fetching employees", ex);
            }
        }

        public async Task DeleteEmployee(Guid MaNV)
        {
            try
            {
                // Kiểm tra nhân viên có tồn tại không
                var nhanVien = await nhanVienContext.tbl_NhanVien.FirstOrDefaultAsync(nv => nv.MaNV == MaNV);
                if (nhanVien == null)
                {
                    throw new KeyNotFoundException($"Không tìm thấy nhân viên với mã: {MaNV}");
                }

                // Xóa nhân viên
                nhanVienContext.tbl_NhanVien.Remove(nhanVien);
                await nhanVienContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa nhân viên: {ex.Message}", ex);
            }
        }

        public async Task UpdateEmployee(NhanVienTaiKhoan nhanVienTaiKhoan, Guid maNV)
        {
            using var transaction = await nhanVienContext.Database.BeginTransactionAsync();
            try
            {
                var parameters = new[]
                {
                    new SqlParameter("@MaNV", maNV),
                    new SqlParameter("@TenNV", nhanVienTaiKhoan.TenNV),
                    new SqlParameter("@Email", nhanVienTaiKhoan.Email),
                    new SqlParameter("@SoDT", nhanVienTaiKhoan.SDT),
                    new SqlParameter("@DiaChi", nhanVienTaiKhoan.DiaChi),
                    new SqlParameter("@GioiTinh", nhanVienTaiKhoan.GioiTinh),
                    new SqlParameter("@NgaySinh", nhanVienTaiKhoan.NgaySinh),
                    new SqlParameter("@NgayVaoLam", nhanVienTaiKhoan.NgayVaoLam),
                    new SqlParameter("@Username", nhanVienTaiKhoan.TenTaiKhoan),
                    new SqlParameter("@MaNhomQuyen", nhanVienTaiKhoan.MaNhomQuyen),
                };

                // Execute the stored procedure for updating employee information
                await nhanVienContext.Database.ExecuteSqlRawAsync("EXEC UpdateEmployee @MaNV, @TenNV, @Email, @SoDT, @DiaChi, @GioiTinh, @NgaySinh, @NgayVaoLam, @Username, @MaNhomQuyen", parameters);

                // Commit the transaction
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                // Rollback if there's an error
                await transaction.RollbackAsync();
                throw new Exception("An error occurred while updating the employee and account", ex);
            }
        }

        public async Task<List<NhanVien>> QuickSearchNhanVien(string searchString)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                var sqlWhere = new StringBuilder();

                if (!string.IsNullOrEmpty(searchString))
                {
                    sqlWhere.Append(" Where (MaNV like @SearchString ESCAPE '\\' OR (TenNV) like @SearchString ESCAPE '\\')");
                    param.Add("SearchString", "%" + searchString + "%");
                }

                string sqlQuery = @"SELECT * FROM tbl_NhanVien WITH (NOLOCK)" + sqlWhere;

                using (var connection = this.nhanVienContext.CreateConnection())
                {
                    var resultData = (await connection.QueryAsync<NhanVien>(sqlQuery, param)).ToList();
                    return resultData;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<NhanVien>> QuickSearchDeliveryEmployee(string searchString)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                var sqlWhere = new StringBuilder();

                if (!string.IsNullOrEmpty(searchString))
                {
                    sqlWhere.Append(" AND (MaNV like @SearchString ESCAPE '\\' OR (TenNV) like @SearchString ESCAPE '\\') ");
                    param.Add("SearchString", $"%{searchString}%");
                }

                string sqlQuery = @"SELECT * FROM tbl_NhanVien WITH (NOLOCK) WHERE ChucVu = N'Nhân viên giao hàng' " + sqlWhere;

                using (var connection = this.nhanVienContext.CreateConnection())
                {
                    var resultData = (await connection.QueryAsync<NhanVien>(sqlQuery, param)).ToList();
                    return resultData;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
