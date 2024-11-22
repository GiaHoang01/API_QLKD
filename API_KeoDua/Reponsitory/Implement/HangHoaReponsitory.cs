using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Text;
using Dapper;
using Microsoft.Data.SqlClient;
using API_KeoDua.DataView;
using Microsoft.AspNetCore.Mvc;
using API_KeoDua.Reponsitory.Interface;
using API_KeoDua.Data;
using Microsoft.CodeAnalysis;

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
                    sqlWhere += " AND h.TenHangHoa COLLATE SQL_Latin1_General_CP1_CI_AS LIKE @SearchString";
                    param.Add("@SearchString", $"%{searchString}%");
                }

                // Sử dụng GROUP BY để loại bỏ trùng lặp
                string sqlQuery = $@"
                SELECT COUNT(1) 
                FROM tbl_HangHoa h
                INNER JOIN (
                    SELECT MaHangHoa, MAX(NgayCapNhatGia) AS MaxNgayCapNhatGia
                    FROM tbl_LichSuGia
                    GROUP BY MaHangHoa
                ) gMax
                ON h.MaHangHoa = gMax.MaHangHoa
                INNER JOIN tbl_LichSuGia g
                ON g.MaHangHoa = gMax.MaHangHoa AND g.NgayCapNhatGia = gMax.MaxNgayCapNhatGia
                WHERE 1=1 {sqlWhere};

                SELECT 
                    h.MaHangHoa,
                    h.TenHangHoa,
                    h.HinhAnh,
                    g.GiaBan,
                    h.MoTa,
                    h.MaLoai
                FROM tbl_HangHoa h
                INNER JOIN (
                    SELECT MaHangHoa, MAX(NgayCapNhatGia) AS MaxNgayCapNhatGia
                    FROM tbl_LichSuGia
                    GROUP BY MaHangHoa
                ) gMax
                ON h.MaHangHoa = gMax.MaHangHoa
                INNER JOIN tbl_LichSuGia g
                ON g.MaHangHoa = gMax.MaHangHoa AND g.NgayCapNhatGia = gMax.MaxNgayCapNhatGia
                WHERE 1=1 {sqlWhere}
                ORDER BY h.TenHangHoa ASC
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
                throw new Exception("An error occurred while fetching products", ex);
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
        public async Task<bool> UpdateProduct(HangHoa updatedProduct, decimal giaBan,string? ghiChu)
        {
            // Bắt đầu giao dịch với IsolationLevel.ReadUncommitted để tránh lock
            using var transaction = await lichSuGiaContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.ReadUncommitted);
            try
            {
                // Lấy sản phẩm hiện tại từ database với NOLOCK (ReadUncommitted)
                var existingProduct = await hangHoaContext.tbl_HangHoa
                    .FromSqlRaw("SELECT * FROM tbl_HangHoa WITH (NOLOCK) WHERE MaHangHoa = {0}", updatedProduct.MaHangHoa)
                    .FirstOrDefaultAsync();

                if (existingProduct == null)
                {
                    throw new Exception("Product not found");
                }

                // Cập nhật thông tin sản phẩm
                existingProduct.TenHangHoa = updatedProduct.TenHangHoa;
                existingProduct.MoTa = updatedProduct.MoTa;
                existingProduct.HinhAnh = updatedProduct.HinhAnh;
                existingProduct.MaLoai = updatedProduct.MaLoai;

                // Lưu thay đổi vào bảng hàng hóa
                await hangHoaContext.SaveChangesAsync();

                // Kiểm tra và thêm lịch sử giá nếu cần
                var latestPrice = await lichSuGiaContext.tbl_LichSuGia
                    .Where(g => g.MaHangHoa == existingProduct.MaHangHoa)
                    .OrderByDescending(g => g.NgayCapNhatGia)
                    .FirstOrDefaultAsync();

                if (latestPrice == null || latestPrice.GiaBan != giaBan || latestPrice.GhiChu!=ghiChu)
                {
                    // Thêm giá mới vào bảng lịch sử giá
                    var newPriceHistory = new LichSuGia
                    {
                        MaLichSu = Guid.NewGuid(),
                        MaHangHoa = existingProduct.MaHangHoa,
                        GiaBan = giaBan,
                        NgayCapNhatGia = DateTime.Now,
                        GhiChu = ghiChu
                    };

                    await lichSuGiaContext.tbl_LichSuGia.AddAsync(newPriceHistory);

                    // Lưu lịch sử giá vào database
                    await lichSuGiaContext.SaveChangesAsync();
                }

                // Commit transaction
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Rollback transaction nếu có lỗi xảy ra
                await transaction.RollbackAsync();
                throw new Exception("An error occurred while updating the product", ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="MaHangHoa"></param>
        /// <returns></returns>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <exception cref="Exception"></exception>
        public async Task DeleteProduct(Guid MaHangHoa)
        {
            try
            {
                // Kiểm tra hàng hóa có tồn tại không
                var hangHoa = await hangHoaContext.tbl_HangHoa.FirstOrDefaultAsync(nv => nv.MaHangHoa == MaHangHoa);
                if (hangHoa == null)
                {
                    throw new KeyNotFoundException($"Không tìm thấy hàng hóa với mã: {MaHangHoa}");
                }

                // Xóa hàng hóa
                lichSuGiaContext.tbl_LichSuGia.RemoveRange(this.lichSuGiaContext.tbl_LichSuGia.Where(m => m.MaHangHoa == MaHangHoa));
                hangHoaContext.tbl_HangHoa.Remove(hangHoa);
                await lichSuGiaContext.SaveChangesAsync();
                await hangHoaContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa hàng hóa: {ex.Message}", ex);
            }
        }
    }
}
