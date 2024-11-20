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
                    sqlWhere += " WHERE TenKhachHang COLLATE SQL_Latin1_General_CP1_CI_AS LIKE @SearchString";
                    param.Add("@SearString", $"%{searchString}");
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
                throw new Exception("Have error when load customer!", ex);
            }
        }
    }

}
