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
    public class HangHoaReponsitory : IHangHoaReponsitory
    {
        private readonly HangHoaContext hangHoaContext;
        private readonly LichSuGiaContext lichSuGiaContext;
        public HangHoaReponsitory(HangHoaContext hangHoaContext, LichSuGiaContext lichSuGiaContext)
        {
            this.hangHoaContext = hangHoaContext;
            this.lichSuGiaContext = lichSuGiaContext;
        }

        public int TotalRows { get; set; }
        public async Task<List<HangHoaLichSuGia>> GetAllProduct(string searchString, int startRow, int maxRows)
        {
            try
            {
                string sqlWhere = "";
                var param = new DynamicParameters();

                if (!string.IsNullOrEmpty(searchString))
                {
                    // Sử dụng TRIM() để loại bỏ khoảng trắng thừa và COLLATE để so sánh không phân biệt chữ hoa chữ thường
                    sqlWhere += " WHERE TenHangHoa COLLATE SQL_Latin1_General_CP1_CI_AS LIKE @SearchString AND g.NgayCapNhatGia = (\r\n SELECT MAX(g1.NgayCapNhatGia) \r\n        FROM tbl_LichSuGia g1 \r\n        WHERE g1.MaHangHoa = g.MaHangHoa\r\n    );";
                    param.Add("@SearchString", $"%{searchString}%");
                }
                // Tạo câu truy vấn với điều kiện WHERE và phân trang
                string sqlQuery = $@"
                    SELECT COUNT(1) FROM tbl_HangHoa WITH (NOLOCK) {sqlWhere};
                    SELECT 
                        h.MaHangHoa,
                        h.TenHangHoa,
                        h.HinhAnh,
                        g.GiaBan,
                        h.MoTa,
                        h.MaLoai
                    FROM 
                        tbl_HangHoa h
                    INNER JOIN 
                        tbl_LichSuGia g
                    ON 
                        h.MaHangHoa = g.MaHangHoa {sqlWhere}
                    ORDER BY TenHangHoa ASC
                    OFFSET @StartRow ROWS FETCH NEXT @MaxRows ROWS ONLY;";

                param.Add("@StartRow", startRow);
                param.Add("@MaxRows", maxRows);

                using (var connection = this.hangHoaContext.CreateConnection())
                {
                    using (var multi = await connection.QueryMultipleAsync(sqlQuery, param))
                    {
                        // Lấy tổng số hàng từ truy vấn đầu tiên
                        this.TotalRows = (await multi.ReadAsync<int>()).Single();
                        // Lấy danh sách hàng hóa từ truy vấn thứ hai
                        return (await multi.ReadAsync<HangHoaLichSuGia>()).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                // Ghi log hoặc xử lý ngoại lệ
                throw new Exception("An error occurred while fetching employees", ex);
            }
        }
        public async Task AddProduct(HangHoa newProduct, decimal giaBan)
        {
            using var transaction = await hangHoaContext.Database.BeginTransactionAsync();
            try
            {
                var parameters = new[]
                {
            new SqlParameter("@TenHangHoa", newProduct.TenHangHoa),
            new SqlParameter("@MoTa", string.IsNullOrEmpty(newProduct.MoTa) ? DBNull.Value : newProduct.MoTa),
            new SqlParameter("@HinhAnh", string.IsNullOrEmpty(newProduct.HinhAnh) ? DBNull.Value : newProduct.HinhAnh),
            new SqlParameter("@MaLoai", newProduct.MaLoai),
            new SqlParameter("@GiaBan", giaBan)
        };

                string storedProcedure = "EXEC SP_InsertHangHoa @TenHangHoa, @MoTa, @HinhAnh, @MaLoai, @GiaBan";

                // Gọi stored procedure
                await hangHoaContext.Database.ExecuteSqlRawAsync(storedProcedure, parameters);

                // Commit transaction
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("An error occurred while adding the product", ex);
            }
        }

    }
}
