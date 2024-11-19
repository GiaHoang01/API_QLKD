using API_KeoDua.Data;
using API_KeoDua.Reponsitory.Interface;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Policy;
using System.Text;
namespace API_KeoDua.Reponsitory.Implement
{
    public class PhieuNhapHangReponsitory:IPhieuNhapHangReponsitory
    {
        private readonly PhieuNhapHangContext phieuNhapHangContext;

        public PhieuNhapHangReponsitory(PhieuNhapHangContext phieuNhapHangContext)
        {
            this.phieuNhapHangContext = phieuNhapHangContext;
        }

        public int TotalRows { get; set; }

        public async Task<List<PhieuNhapHang>> GetAllPurchase(DateTime fromDate, DateTime toDate, string searchString, int startRow, int maxRows)
        {
            try
            {
                var sqlWhere = new StringBuilder();
                var param = new DynamicParameters();

                if (!string.IsNullOrEmpty(searchString))
                {
                    sqlWhere.Append("WHERE MaPhieuNhap COLLATE SQL_Latin1_General_CP1_CI_AS LIKE @SearchString OR MaNV LIKE @SearchString");
                    param.Add("@SearchString", $"%{searchString}%");
                }
                else
                {
                    sqlWhere.Append("WHERE NgayNhap >= @FromDate AND NgayNhap <= @ToDate AND NgayDat >= @FromDate AND NgayDat <= @ToDate");
                    param.Add("@FromDate", fromDate);
                    param.Add("@ToDate", toDate);
                }

                string sqlQuery = $@"SELECT COUNT(1) AS TotalRows FROM tbl_PhieuNhapHang WITH (NOLOCK) {sqlWhere};
                                     SELECT * FROM tbl_PhieuNhapHang WITH (NOLOCK) {sqlWhere} ORDER BY MaPhieuNhap ASC
                                     OFFSET @StartRow ROWS FETCH NEXT @MaxRows ROWS ONLY;";
                param.Add("@StartRow", startRow);
                param.Add("@MaxRows", maxRows);
                using (var connection = this.phieuNhapHangContext.CreateConnection())
                {
                    using (var multi = await connection.QueryMultipleAsync(sqlQuery, param))
                    {
                        this.TotalRows = (await multi.ReadFirstOrDefaultAsync<int>());
                        return (await multi.ReadAsync<PhieuNhapHang>()).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ và ghi log
                throw new Exception("An error occurred while fetching purchase records", ex);
            }
        }

    }
}
