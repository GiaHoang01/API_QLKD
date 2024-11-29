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

        public async Task<PhieuHuyDon> GetAllShippingNoteCancelByID(Guid? maPhieuHuy)
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
                    SELECT * 
                    FROM tbl_PhieuHuyDon 
                    {sqlWhere}";

                using (var connection = this.phieuHuyDonConText.CreateConnection())
                {
                    // Sử dụng QuerySingleOrDefaultAsync để lấy một bản ghi duy nhất
                    var result = await connection.QuerySingleOrDefaultAsync<PhieuHuyDon>(sqlQuery, param);

                    // Trả về kết quả tìm được (hoặc null nếu không tìm thấy)
                    return result;
                }
            }
            catch (Exception ex)
            {
                // Xử lý ngoại lệ
                throw new Exception("An error occurred while fetching shipping note by ID", ex);
            }
        }


    }
}
