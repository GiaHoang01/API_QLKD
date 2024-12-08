using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Text;
using Dapper;
using Microsoft.Data.SqlClient;
using API_KeoDua.DataView;
using Microsoft.AspNetCore.Mvc;
using API_KeoDua.Reponsitory.Interface;
using API_KeoDua.Data;

namespace API_KeoDua.Reponsitory.Implement
{
    public class KhachHangReponsitory:IKhachHangReponsitory
    {
        private readonly KhachHangContext khachHangContext;
        public KhachHangReponsitory(KhachHangContext khachHangContext)
        {
            this.khachHangContext = khachHangContext;
        }

        public int TotalRows { get; set; }

        public async Task<List<KhachHang>> GetAllCustomer(string searchString, int startRow, int maxRow)
        {
            try
            {
                string sqlWhere = "";
                var param = new DynamicParameters();

                if(!string.IsNullOrEmpty(searchString))
                {
                    sqlWhere += " WHERE SDT COLLATE SQL_Latin1_General_CP1_CI_AS LIKE @SearchString";
                    param.Add("@SearchString", $"%{searchString}%");
                }

                string sqlQuery = $@"
                                SELECT COUNT(1) FROM tbl_KhachHang WITH (NOLOCK) {sqlWhere};
                                SELECT * FROM tbl_KhachHang WITH (NOLOCK) {sqlWhere}
                                ORDER BY 
                                    REVERSE(SUBSTRING(REVERSE(TenKhachHang), 
                                                      1, 
                                                      CHARINDEX(' ', REVERSE(TenKhachHang) + ' ') - 1)) ASC
                                OFFSET @StartRow ROWS FETCH NEXT @MaxRow ROWS ONLY";

                param.Add("@StartRow", startRow);
                param.Add("@MaxRow", maxRow);

                using (var connection = this.khachHangContext.CreateConnection())
                {
                    using (var multi = await connection.QueryMultipleAsync(sqlQuery, param))
                    {
                        this.TotalRows = (await multi.ReadAsync<int>()).Single();
                        return (await multi.ReadAsync<KhachHang>()).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Have an error when load customer!", ex);
            }
        }

        public async Task<bool> IsPhoneNumberExists(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
            {
                // Nếu phoneNumber là null hoặc rỗng, không cần kiểm tra, trả về false
                return false;
            }

            // Thực hiện kiểm tra trong cơ sở dữ liệu nếu phoneNumber không null
            return await this.khachHangContext.tbl_KhachHang
                .AnyAsync(kh => kh.Sdt == phoneNumber);
        }


        public async Task<bool> AddCustomer(KhachHang khachHang)
        {
            try
            {
                if (await IsPhoneNumberExists(khachHang.Sdt))
                {
                    throw new Exception("Số điện thoại đã tồn tại.");
                }

                await this.khachHangContext.tbl_KhachHang.AddAsync(khachHang);
                await this.khachHangContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Have an error when add customer", ex);
            }
        }

        public async Task<object> GetCustomerByID(Guid MaKH)
        {
            try
            {
                var sqlQuery = @"
            SELECT 
                kh.MaKhachHang,
                kh.TenKhachHang,
                kh.GioiTinh,
                kh.Email,
                kh.TenTaiKhoan,
                kh.MaLoaiKH,
                kh.Sdt AS SDTKH,
                tt.MaThongTin,
                tt.DiaChi,
                tt.MacDinh,
                tt.SDT AS SDTThongTin
            FROM tbl_KhachHang kh
            LEFT JOIN tbl_ThongTinGiaoHang tt 
            ON kh.MaKhachHang = tt.MaKhachHang
            WHERE kh.MaKhachHang = @MaKH";

                using (var connection = this.khachHangContext.CreateConnection())
                {
                    var result = await connection.QueryAsync(sqlQuery, new { MaKH });

                    // Nhóm dữ liệu theo MaKhachHang
                    var customer = result
                        .GroupBy(r => r.MaKhachHang)
                        .Select(g => new
                        {
                            // Lấy thông tin khách hàng từ g.First()
                            MaKhachHang = g.Key,
                            TenKhachHang = g.First().TenKhachHang,
                            GioiTinh = g.First().GioiTinh,
                            Email = g.First().Email,
                            TenTaiKhoan = g.First().TenTaiKhoan,
                            MaLoaiKH = g.First().MaLoaiKH,
                            SDT = g.First().SDTKH,
                            // Lấy danh sách thông tin giao hàng
                            ShippingInfos = g
                                .Where(x => x.MaThongTin != null)
                                .Select(x => new
                                {
                                    MaThongTin = x.MaThongTin,
                                    DiaChi = x.DiaChi,
                                    MacDinh = x.MacDinh,
                                    SDT = x.SDTThongTin
                                }).ToList()
                        }).FirstOrDefault();

                    return customer;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error when fetching customer info", ex);
            }
        }

        public async Task<bool> CheckCustomerCode(Guid maKH)
        {
            try
            {
                var sqlQuery = @"
                    SELECT COUNT(1)
                    FROM [dtb_QuanLyKeoDua].[dbo].[tbl_HoaDonBanHang]
                    WHERE MaKhachHang = @MaKH";

                using (var connection = this.khachHangContext.CreateConnection())
                {
                    var result = await connection.ExecuteScalarAsync<int>(sqlQuery, new { MaKH = maKH });
                    if (result > 0)
                    {
                        return false; // Không thể xóa khách hàng này vì có liên kết với hóa đơn
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                // Xử lý lỗi
                throw new Exception("Error occurred while checking customer code", ex);
            }
        }


        public async Task<bool> DeleteCustomer(Guid MaKH)
        {
            try
            {
                // Kiểm tra xem mã khách hàng có bị liên kết với hóa đơn hay không
                bool canDelete = await CheckCustomerCode(MaKH);
                if (!canDelete)
                {
                    // Nếu không thể xóa (tức là có liên kết với hóa đơn), trả về false
                    return false;
                }

                // Sử dụng Dapper để thực hiện câu lệnh DELETE
                var query = "DELETE FROM tbl_KhachHang WHERE MaKhachHang = @MaKH";

                // Thực thi câu lệnh DELETE với Dapper
                using (var connection = this.khachHangContext.CreateConnection())
                {
                    var affectedRows = await connection.ExecuteAsync(query, new { MaKH });

                    // Kiểm tra nếu có bản ghi bị xóa (affectedRows > 0)
                    return affectedRows > 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa khách hàng: {ex.Message}", ex);
            }
        }

        public async Task<bool> UpdateCustomer(KhachHang kh)
        {
            try
            {
                this.khachHangContext.Entry(kh).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                await this.khachHangContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<KhachHang>> QuickSearchKhachHang(string searchString)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                var sqlWhere = new StringBuilder();

                if (!string.IsNullOrEmpty(searchString))
                {
                    sqlWhere.Append(" Where (MaKhachHang like @SearchString ESCAPE '\\' OR (TenKhachHang) like @SearchString ESCAPE '\\')");
                    param.Add("SearchString", "%" + searchString + "%");
                }

                string sqlQuery = @"SELECT * FROM tbl_KhachHang WITH (NOLOCK)" + sqlWhere;

                using (var connection = this.khachHangContext.CreateConnection())
                {
                    var resultData = (await connection.QueryAsync<KhachHang>(sqlQuery, param)).ToList();
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
