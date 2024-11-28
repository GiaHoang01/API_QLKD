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
                                    ORDER BY TenKhachHang ASC OFFSET @StartRow ROWS FETCH NEXT @MaxRow ROWS ONLY";
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

        public async Task<KhachHang> GetCustomerByID(Guid MaKH)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                var sqlQuery = @"SELECT * FROM tbl_KhachHang WHERE MaKhachHang = @MaKH";
                param.Add("@MaKH", MaKH);

                using (var connection = this.khachHangContext.CreateConnection())
                {
                    var result = await connection.QueryFirstOrDefaultAsync<KhachHang>(sqlQuery, param);
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Have an error when get info customer", ex);
            }
        }

        public async Task<bool> DeleteCustomer (Guid MaKH)
        {
            try
            {
                var khachHang = await khachHangContext.tbl_KhachHang.FirstOrDefaultAsync(kh => kh.MaKhachHang == MaKH);
                if (khachHang == null)
                {
                    return false;
                }

                khachHangContext.tbl_KhachHang.Remove(khachHang);
                await khachHangContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa nhân viên: {ex.Message}", ex);
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
