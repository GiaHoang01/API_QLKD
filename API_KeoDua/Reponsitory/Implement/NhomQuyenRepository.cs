using API_KeoDua.Data;
using API_KeoDua.Reponsitory.Interface;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Security.Policy;
using System.Text;
namespace API_KeoDua.Reponsitory.Implement
{
    public class NhomQuyenRepository:INhomQuyenRepository
    {
        private readonly NhomQuyenContext nhomQuyenContext;
        
        public NhomQuyenRepository(NhomQuyenContext nhomQuyenContext)
        {
            this.nhomQuyenContext = nhomQuyenContext;
        }

        public int TotalRows { get; set; }
        /// <summary>
        /// Hàm quicksearch NhomQuyen
        /// </summary>
        /// <returns></returns>
        public async Task<List<NhomQuyen>> quickSearchNhomQuyen(string searchString)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                var sqlWhere = new StringBuilder();

                if (!string.IsNullOrEmpty(searchString))
                {
                    sqlWhere.Append(" AND (MaNhomQuyen like @SearchString ESCAPE '\\' OR (TenNhomQuyen) like @SearchString ESCAPE '\\')");
                    param.Add("SearchString", "%" + searchString + "%");
                }

                string sqlQuery = @"SELECT * FROM tbl_NhomQuyen WITH (NOLOCK) where MaNhomQuyen!='NQ00000005' " + sqlWhere;

                using (var connection = this.nhomQuyenContext.CreateConnection())
                {
                    var resultData = (await connection.QueryAsync<NhomQuyen>(sqlQuery, param)).ToList();
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
