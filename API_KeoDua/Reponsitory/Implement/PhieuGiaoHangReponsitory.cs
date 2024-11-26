using API_KeoDua.Data;
using API_KeoDua.DataView;
using API_KeoDua.Reponsitory.Interface;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Transactions;

namespace API_KeoDua.Reponsitory.Implement
{
    public class PhieuGiaoHangReponsitory: IPhieuGiaoHangReponsitory
    {
        private readonly PhieuGiaoHangContext phieuGiaoHangContext;
        private readonly CT_PhieuNhapContext ct_PhieuNhapContext;

        public PhieuGiaoHangReponsitory(PhieuGiaoHangContext phieuGiaoHangContext, CT_PhieuNhapContext ct_PhieuNhapContext)
        {
            this.phieuGiaoHangContext = phieuGiaoHangContext;
            this.ct_PhieuNhapContext = ct_PhieuNhapContext;
        }

        public int TotalRows { get; set; }

        public async Task<List<PhieuGiaoHang>> GetAllShippingNote(DateTime fromDate, DateTime toDate, string searchString, int startRow, int maxRow)
        {
            try
            {
                var sqlWhere = new StringBuilder();
                sqlWhere.Append(" WHERE TrangThai <> N'Chờ xác nhận' AND h.MaKhachHang = k.MaKhachHang AND h.MaNV = n.MaNV ");
                sqlWhere.Append(" AND ((@FromDate <= NgayTao AND NgayTao <= @ToDate) OR (@FromDate <= NgayBan AND NgayBan <= @ToDate))");

                var param = new DynamicParameters();
                param.Add("@FromDate", fromDate);
                param.Add("@ToDate", toDate);

                // Lọc theo từ khóa tìm kiếm
                if (!string.IsNullOrEmpty(searchString))
                {
                    sqlWhere.Append(" AND (LTRIM(RTRIM(n.TenNV)) COLLATE SQL_Latin1_General_CP1_CI_AS LIKE @SearchString");
                    sqlWhere.Append(" OR LTRIM(RTRIM(h.MaPhieuGiao)) COLLATE SQL_Latin1_General_CP1_CI_AS LIKE @SearchString");
                    sqlWhere.Append(" OR LTRIM(RTRIM(k.TenKhachHang)) COLLATE SQL_Latin1_General_CP1_CI_AS LIKE @SearchString)");
                    param.Add("@SearchString", $"%{searchString}%");
                }

                // Phân trang
                param.Add("@StartRow", startRow);
                param.Add("@MaxRows", maxRow);

                // Tạo câu truy vấn với điều kiện WHERE và phân trang
                string sqlQuery = $@"
                                    SELECT COUNT(1) 
                                    FROM tbl_HoaDonBanHang h
                                    JOIN tbl_NhanVien n ON h.MaNV = n.MaNV
                                    JOIN tbl_KhachHang k ON h.MaKhachHang = k.MaKhachHang 
                                    {sqlWhere};

                                    SELECT * 
                                    FROM tbl_HoaDonBanHang h
                                    JOIN tbl_NhanVien n ON h.MaNV = n.MaNV
                                    JOIN tbl_KhachHang k ON h.MaKhachHang = k.MaKhachHang 
                                    {sqlWhere}
                                    ORDER BY NgayTao ASC
                                    OFFSET @StartRow ROWS FETCH NEXT @MaxRows ROWS ONLY;";

                using (var connection = this.phieuGiaoHangContext.CreateConnection())
                {
                    using (var multi = await connection.QueryMultipleAsync(sqlQuery, param))
                    {
                        // Lấy tổng số hàng từ truy vấn đầu tiên
                        this.TotalRows = (await multi.ReadAsync<int>()).Single();

                        // Lấy danh sách hóa đơn từ truy vấn thứ hai
                        return (await multi.ReadAsync<PhieuGiaoHang>()).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                // Ghi log hoặc xử lý ngoại lệ
                throw new Exception("An error occurred while fetching shipping notes", ex);
            }
        }

    }
}
