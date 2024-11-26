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

        /// <summary>
        /// Hàm quicksearch NhaCungCap
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Hàm search nha cung cap theo maNCC
        /// </summary>
        /// <param name="maNCC"></param>
        /// <returns></returns>
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
