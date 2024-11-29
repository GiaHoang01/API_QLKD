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
    public class NhaCungCapReponsitory: INhaCungCapReponsitory
    {
        private readonly NhaCungCapContext nhaCungCapContext;
        public NhaCungCapReponsitory(NhaCungCapContext nhaCungCapContext)
        {
            this.nhaCungCapContext = nhaCungCapContext;
        }

        public int TotalRows { get; set; }

        public async Task<List<NhaCungCap>> GetAllVendors(string searchString, int startRow, int maxRow)
        {
            try
            {
                string sqlWhere = "";
                var param = new DynamicParameters();

                if (!string.IsNullOrEmpty(searchString))
                {
                    sqlWhere += " WHERE TenNCC COLLATE SQL_Latin1_General_CP1_CI_AS LIKE @SearchString";
                    param.Add("@SearchString", $"%{searchString}%");
                }

                string sqlQuery = $@"
                                    SELECT COUNT(1) FROM tbl_NhaCungCap WITH (NOLOCK) {sqlWhere};
                                    SELECT * FROM tbl_NhaCungCap WITH (NOLOCK) {sqlWhere}
                                    ORDER BY TenNCC ASC OFFSET @StartRow ROWS FETCH NEXT @MaxRow ROWS ONLY";
                param.Add("@StartRow", startRow);
                param.Add("@MaxRow", maxRow);

                using (var connection = this.nhaCungCapContext.CreateConnection())
                {
                    using (var multi = await connection.QueryMultipleAsync(sqlQuery, param))
                    {
                        this.TotalRows = (await multi.ReadAsync<int>()).Single();
                        return (await multi.ReadAsync<NhaCungCap>()).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Have an error when load vendor!", ex);
            }
        }

        public async Task<bool> AddVendor(NhaCungCap nhaCungCap)
        {
            try
            {

                await this.nhaCungCapContext.tbl_NhaCungCap.AddAsync(nhaCungCap);
                await this.nhaCungCapContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Have an error when add vendor", ex);
            }
        }

        public async Task<NhaCungCap> GetVendorByID(Guid maNCC)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                var sqlQuery = @"SELECT * FROM tbl_NhaCungCap WHERE MaNCC = @MaNCC";
                param.Add("@MaNCC", maNCC);

                using (var connection = this.nhaCungCapContext.CreateConnection())
                {
                    var result = await connection.QueryFirstOrDefaultAsync<NhaCungCap>(sqlQuery, param);
                    return result;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Have an error when get info customer", ex);
            }
        }

        public async Task<bool> DeleteVendor(Guid MaNCC)
        {
            try
            {
                var vendor = await nhaCungCapContext.tbl_NhaCungCap.FirstOrDefaultAsync(kh => kh.MaNCC == MaNCC);
                if (vendor == null)
                {
                    return false;
                }

                nhaCungCapContext.tbl_NhaCungCap.Remove(vendor);
                await nhaCungCapContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa vendor: {ex.Message}", ex);
            }
        }

        public async Task<bool> UpdateVendor(NhaCungCap nhaCungCap)
        {
            try
            {
                this.nhaCungCapContext.Entry(nhaCungCap).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                await this.nhaCungCapContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<NhaCungCap>> QuickSearchNhaCungCap(string searchString)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                var sqlWhere = new StringBuilder();

                if (!string.IsNullOrEmpty(searchString))
                {
                    sqlWhere.Append(" Where (MaNCC like @SearchString ESCAPE '\\' OR (TenNCC) like @SearchString ESCAPE '\\')");
                    param.Add("SearchString", "%" + searchString + "%");
                }

                string sqlQuery = @"SELECT * FROM tbl_NhaCungCap WITH (NOLOCK)" + sqlWhere;

                using (var connection = this.nhaCungCapContext.CreateConnection())
                {
                    var resultData = (await connection.QueryAsync<NhaCungCap>(sqlQuery, param)).ToList();
                    return resultData;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<string> SearchNhaCungCap_ByMaNCC(Guid? maNCC)
        {
            try
            {
                string sqlQuery = @" SELECT tenNCC FROM tbl_NhaCungCap WHERE maNCC = @MaNCC";
                var param = new DynamicParameters();
                param.Add("@MaNCC", maNCC);

                using (var connection = this.nhaCungCapContext.CreateConnection())
                {
                    var result = await connection.QueryFirstOrDefaultAsync<string>(sqlQuery, param);
                    return result;
                }
            }
            catch (Exception ex)
            {
                // Ghi log hoặc xử lý ngoại lệ
                throw new Exception("An error occurred while fetching employees", ex);
            }
        }
    }
}
