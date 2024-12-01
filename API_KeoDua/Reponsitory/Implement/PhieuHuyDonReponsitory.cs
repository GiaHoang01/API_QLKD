using API_KeoDua.Data;
using API_KeoDua.Reponsitory.Interface;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Text;
using Dapper;
using Microsoft.Data.SqlClient;
using API_KeoDua.DataView;
using System.Transactions;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace API_KeoDua.Reponsitory.Implement
{
    public class PhieuHuyDonReponsitory: IPhieuHuyDonReponsitory
    {
        private readonly PhieuHuyDonConText phieuHuyDonConText;
        public PhieuHuyDonReponsitory(PhieuHuyDonConText phieuHuyDonConText)
        {
            this.phieuHuyDonConText = phieuHuyDonConText;
        }

        public int TotalRows { get; set; }

        public async Task<List<PhieuHuyDon>> GetAllShippingNoteCancel(DateTime fromDate, DateTime toDate, int startRow, int maxRow)
        {
            try
            {
                var sqlWhere = new StringBuilder();

                // Điều kiện lọc theo ngày
                sqlWhere.Append(@" WHERE (DATEDIFF(DAY, @FromDate, NgayHuy) >= 0 OR DATEDIFF(DAY, NgayHuy, @ToDate) <= 0)");

                // Thêm điều kiện lọc theo ngày vào SQL
                var param = new DynamicParameters();
                param.Add("@FromDate", fromDate);
                param.Add("@ToDate", toDate);

                // Thêm phân trang vào các tham số
                param.Add("@StartRow", startRow);
                param.Add("@MaxRows", maxRow);

                // Câu truy vấn SQL với điều kiện lọc và phân trang
                string sqlQuery = $@"
                    -- Truy vấn đếm số lượng phiếu
                    SELECT COUNT(1) 
                    FROM tbl_PhieuHuyDon
                    {sqlWhere.ToString()};

                    -- Truy vấn lấy danh sách phiếu với phân trang
                    SELECT * 
                    FROM tbl_PhieuHuyDon
                    {sqlWhere.ToString()}
                    ORDER BY NgayHuy -- hoặc chọn cột phù hợp để sắp xếp
                    OFFSET @StartRow ROWS FETCH NEXT @MaxRows ROWS ONLY;";

                using (var connection = this.phieuHuyDonConText.CreateConnection())
                {
                    using (var multi = await connection.QueryMultipleAsync(sqlQuery, param))
                    {
                        // Lấy tổng số hàng từ truy vấn đầu tiên
                        this.TotalRows = (await multi.ReadAsync<int>()).Single();

                        // Lấy danh sách phiếu giao hàng từ truy vấn thứ hai, trả về dynamic
                        var result = (await multi.ReadAsync<PhieuHuyDon>()).ToList();

                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                // Ghi log hoặc xử lý ngoại lệ
                throw new Exception("An error occurred while fetching shipping notes", ex);
            }
        }

        public async Task<object> GetShippingNoteCancelByID(Guid? maPhieuHuy)
        {
            try
            {
                // Điều kiện WHERE cho truy vấn
                var sqlWhere = new StringBuilder();
                sqlWhere.Append(" WHERE p.MaPhieuHuy = @MaPhieuHuy");

                // Đối số truyền vào câu lệnh SQL
                var param = new DynamicParameters();
                param.Add("@MaPhieuHuy", maPhieuHuy);

                // Câu lệnh SQL để lấy dữ liệu
                string sqlQuery = $@"
                SELECT p.*, 
                       pg.NgayGiao, pg.MaNV,
                       nv.TenNV,
                       kh.MaKhachHang, kh.TenKhachHang
                FROM tbl_PhieuHuyDon p
                INNER JOIN tbl_PhieuGiaoHang pg ON p.MaPhieuGiao = pg.MaPhieuGiao
                INNER JOIN tbl_ThongTinGiaoHang tgh ON pg.MaThongTin = tgh.MaThongTin
                INNER JOIN tbl_KhachHang kh ON tgh.MaKhachHang = kh.MaKhachHang
                LEFT JOIN tbl_NhanVien nv ON pg.MaNV = nv.MaNV
                {sqlWhere}";

                using (var connection = this.phieuHuyDonConText.CreateConnection())
                {
                    // Thực hiện truy vấn
                    var result = await connection.QueryAsync<dynamic>(sqlQuery, param);

                    // Nhóm kết quả theo MaPhieuGiao và chọn các trường cần thiết
                    var groupedResult = result
                        .GroupBy(r => r.MaPhieuGiao)
                        .Select(g => new
                        {
                            MaPhieuHuy = g.First().MaPhieuHuy,
                            NgayHuy = g.First().NgayHuy,
                            LyDo = g.First().LyDo,
                            MaPhieuGiao = g.First(). MaPhieuGiao,
                            NgayGiao = g.First().NgayGiao,
                            MaNV = g.First().MaNV,
                            TenNV = g.First().TenNV,
                            MaKhachHang = g.First().MaKhachHang,
                            TenKhachHang = g.First().TenKhachHang
                        }).FirstOrDefault();

                    return groupedResult;
                }
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ
                throw new Exception("An error occurred while fetching shipping note by ID", ex);
            }
        }

        public async Task<bool> UpdateShippingNoteCancel(Guid maPhieuHuy, Guid? maPhieuGiao)
        {
            try
            {
                string sqlQuery = "UPDATE tbl_PhieuHuyDon SET MaPhieuGiao = @MaPhieuGiao WHERE MaPhieuHuy = @MaPhieuHuy";

                int rowsAffected = await this.phieuHuyDonConText.Database.ExecuteSqlRawAsync(sqlQuery,
                    new SqlParameter("@MaPhieuGiao", maPhieuGiao ?? (object)DBNull.Value),
                    new SqlParameter("@MaPhieuHuy", maPhieuHuy));

                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Error updating shipping note", ex);
            }
        }

        public async Task<bool> AddShippingNoteCancel(PhieuHuyDon phieuHuyDon)
        {
            try
            {
                await this.phieuHuyDonConText.tbl_PhieuHuyDon.AddAsync(phieuHuyDon);
                await this.phieuHuyDonConText.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> DeleteShippingNoteCancel(Guid maPhieuHuy)
        {
            try
            {
                // Đảm bảo rằng bạn sử dụng đúng tham số SqlParameter
                var sqlQuery = "DELETE FROM tbl_PhieuHuyDon WHERE MaPhieuHuy = @maPhieuHuy";

                // Khai báo tham số SqlParameter cho đúng
                var result = await this.phieuHuyDonConText.Database.ExecuteSqlRawAsync(
                    sqlQuery,
                    new SqlParameter("@maPhieuHuy", maPhieuHuy) // Chắc chắn tham số này đúng
                );

                // Kiểm tra số lượng bản ghi bị xóa
                if (result > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while deleting the shipping note cancel", ex);
            }
        }




    }
}
