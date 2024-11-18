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
    }
}
