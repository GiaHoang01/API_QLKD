using API_KeoDua.Data;
using API_KeoDua.DataView;
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
        private readonly CT_PhieuNhapContext cT_PhieuNhapContext;
        public PhieuNhapHangReponsitory(PhieuNhapHangContext phieuNhapHangContext, CT_PhieuNhapContext cT_PhieuNhapContext)
        {
            this.phieuNhapHangContext = phieuNhapHangContext;
            this.cT_PhieuNhapContext = cT_PhieuNhapContext;
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

        public async Task<(PhieuNhapHang phieuNhap, List<CT_PhieuNhap> chiTietPhieuNhap)> GetPurchase_ByID(Guid maPhieuNhap)
        {
            try
            {
                // Query cho bảng PhieuNhapHang
                var sqlPhieuNhap = @"SELECT * FROM tbl_PhieuNhapHang WHERE MaPhieuNhap = @MaPhieuNhap;";

                // Query cho bảng CT_PhieuNhap và bảng liên quan
                var sqlChiTietPhieuNhap = @"
                SELECT * FROM  tbl_CT_PhieuNhap WHERE MaPhieuNhap = @MaPhieuNhap;";

                // Biến lưu trữ kết quả
                PhieuNhapHang phieuNhap;
                List<CT_PhieuNhap> chiTietPhieuNhap;

                // Truy vấn bảng tbl_PhieuNhapHang từ PhieuNhapHangContext
                using (var connection1 = this.phieuNhapHangContext.CreateConnection())
                {
                    phieuNhap = await connection1.QueryFirstOrDefaultAsync<PhieuNhapHang>(sqlPhieuNhap, new { MaPhieuNhap = maPhieuNhap });
                }

                // Truy vấn bảng tbl_CT_PhieuNhap từ CT_PhieuNhapContext
                using (var connection2 = this.cT_PhieuNhapContext.CreateConnection())
                {
                    chiTietPhieuNhap = (await connection2.QueryAsync<CT_PhieuNhap>(sqlChiTietPhieuNhap, new { MaPhieuNhap = maPhieuNhap })).ToList();
                }

                return (phieuNhap, chiTietPhieuNhap);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching the purchase order details.", ex);
            }
        }
    }
}
