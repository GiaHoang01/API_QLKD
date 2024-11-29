using API_KeoDua.Data;
using API_KeoDua.DataView;
using API_KeoDua.Reponsitory.Interface;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Security.Policy;

using System.Text;
using System.Transactions;
namespace API_KeoDua.Reponsitory.Implement
{
    public class ChuongTrinhKhuyenMaiReponsitory:IChuongTrinhKhuyenMaiReponsitory
    {
        private readonly ChuongTrinhKhuyenMaiContext chuongTrinhKhuyenMaiContext;
        private readonly ChiTietCT_KhuyenMaiContext chiTietCT_KhuyenMaiContext;

        public ChuongTrinhKhuyenMaiReponsitory(ChuongTrinhKhuyenMaiContext chuongTrinhKhuyenMaiContext, ChiTietCT_KhuyenMaiContext chiTietCT_KhuyenMaiContext)
        {
            this.chuongTrinhKhuyenMaiContext = chuongTrinhKhuyenMaiContext;
            this.chiTietCT_KhuyenMaiContext = chiTietCT_KhuyenMaiContext;
        }

        public int TotalRows { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchString"></param>
        /// <param name="startRow"></param>
        /// <param name="maxRows"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<List<ChuongTrinhKhuyenMai>> GetAllPromotion(string searchString, int startRow, int maxRows)
        {
            try
            {
                string sqlWhere = "";
                var param = new DynamicParameters();

                if (!string.IsNullOrEmpty(searchString))
                {
                    sqlWhere += " AND TenCTKhuyenMai COLLATE SQL_Latin1_General_CP1_CI_AS LIKE @SearchString";
                    param.Add("@SearchString", $"%{searchString}%");
                }

                // Sử dụng GROUP BY để loại bỏ trùng lặp
                string sqlQuery = $@"
                select Count(1) from tbl_ChuongTrinhKhuyenMai
                WHERE 1=1 {sqlWhere};

                select * from tbl_ChuongTrinhKhuyenMai
                WHERE 1=1 {sqlWhere}
                ORDER BY TenCTKhuyenMai ASC
                OFFSET @StartRow ROWS FETCH NEXT @MaxRows ROWS ONLY;";

                param.Add("@StartRow", startRow);
                param.Add("@MaxRows", maxRows);

                using (var connection = this.chuongTrinhKhuyenMaiContext.CreateConnection())
                {
                    using (var multi = await connection.QueryMultipleAsync(sqlQuery, param))
                    {
                        // Lấy tổng số hàng từ truy vấn đầu tiên
                        this.TotalRows = (await multi.ReadAsync<int>()).Single();
                        // Lấy danh sách chương trình khuyến mãi từ truy vấn thứ hai
                        return (await multi.ReadAsync<ChuongTrinhKhuyenMai>()).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching products", ex);
            }
        }
        
        public async Task<(ChuongTrinhKhuyenMai chuongTrinhKhuyenMai, List<ChiTietCT_KhuyenMai> chiTietKhuyenMaiList)> GetPromotion_ByID(Guid? maKhuyenMai)
        {
            try
            {
                // Query cho bảng Chương trình khuyến mãi
                var sqlKhuyenMai = @"SELECT * FROM tbl_ChuongTrinhKhuyenMai WHERE MaKhuyenMai = @MaKhuyenMai;";

                // Query cho bảng Chi tiết chương trình khuyến mãi và bảng liên quan
                var sqlChiTietKhuyenMai = @"
                SELECT * FROM  tbl_ChiTietCT_KhuyenMai WHERE MaKhuyenMai = @MaKhuyenMai;";

                // Biến lưu trữ kết quả
                ChuongTrinhKhuyenMai chuongTrinhKhuyenMai;
                List<ChiTietCT_KhuyenMai> chiTietKhuyenMaiList;

                // Truy vấn bảng tbl_PhieuNhapHang từ PhieuNhapHangContext
                using (var connection1 = this.chuongTrinhKhuyenMaiContext.CreateConnection())
                {
                    chuongTrinhKhuyenMai = await connection1.QueryFirstOrDefaultAsync<ChuongTrinhKhuyenMai>(sqlKhuyenMai, new { MaKhuyenMai = maKhuyenMai });
                }

                // Truy vấn bảng tbl_CT_PhieuNhap từ CT_PhieuNhapContext
                using (var connection2 = this.chiTietCT_KhuyenMaiContext.CreateConnection())
                {
                    chiTietKhuyenMaiList = (await connection2.QueryAsync<ChiTietCT_KhuyenMai>(sqlChiTietKhuyenMai, new { MaKhuyenMai = maKhuyenMai })).ToList();
                }

                return (chuongTrinhKhuyenMai,chiTietKhuyenMaiList);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while fetching the promotion details.", ex);
            }
        }

        public async Task AddPromotion(ChuongTrinhKhuyenMai chuongTrinhKhuyenMai, List<ChiTietCT_KhuyenMai> chiTietKhuyenMaiList)
        {
            // Sử dụng TransactionScope cho tất cả các DbContext
            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    // Thêm chương trình khuyến mãi vào bảng tbl_ChuongTrinhKhuyenMai
                    chuongTrinhKhuyenMaiContext.tbl_ChuongTrinhKhuyenMai.Add(chuongTrinhKhuyenMai);
                    await chuongTrinhKhuyenMaiContext.SaveChangesAsync();

                    // Thêm chi tiết phiếu nhập vào bảng tbl_CT_PhieuNhap
                    foreach (var detail in chiTietKhuyenMaiList)
                    {
                        detail.MaKhuyenMai = (Guid)chuongTrinhKhuyenMai.MaKhuyenMai;
                        chiTietCT_KhuyenMaiContext.tbl_ChiTietCT_KhuyenMai.Add(detail);
                    }
                    await chiTietCT_KhuyenMaiContext.SaveChangesAsync();

                    // Commit giao dịch
                    transaction.Complete();
                }
                catch (Exception ex)
                {
                    // Xử lý lỗi và rollback nếu có
                    throw new InvalidOperationException("Có lỗi xảy ra khi thêm dữ liệu.", ex);
                }
            }
        }

        public async Task<bool> UpdatePromotion(ChuongTrinhKhuyenMai chuongTrinhKhuyenMai, List<ChiTietCT_KhuyenMai> chiTietKhuyenMaiList)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    // Tìm chương trình khuyến mãi hiện tại
                    var existingKhuyenMai = await chuongTrinhKhuyenMaiContext.tbl_ChuongTrinhKhuyenMai.FindAsync(chuongTrinhKhuyenMai.MaKhuyenMai);
                    if (existingKhuyenMai == null)
                    {
                        return false; 
                    }
                    existingKhuyenMai.MaKhuyenMai = chuongTrinhKhuyenMai.MaKhuyenMai;
                    existingKhuyenMai.TenCTKhuyenMai = chuongTrinhKhuyenMai.TenCTKhuyenMai;
                    existingKhuyenMai.GhiChu = chuongTrinhKhuyenMai.GhiChu;
                    await chuongTrinhKhuyenMaiContext.SaveChangesAsync();

                    string deleteQuery = @"DELETE FROM tbl_ChiTietCT_KhuyenMai WHERE MaKhuyenMai = @MaKhuyenMai";
                    var parameter = new SqlParameter("@MaKhuyenMai", chuongTrinhKhuyenMai.MaKhuyenMai);
                    await chiTietCT_KhuyenMaiContext.Database.ExecuteSqlRawAsync(deleteQuery, parameter);

                    // **Thêm chi tiết phiếu nhập mới**
                    foreach (var detail in chiTietKhuyenMaiList)
                    {
                        detail.MaKhuyenMai = (Guid)chuongTrinhKhuyenMai.MaKhuyenMai;
                        detail.NgayBD = DateTime.Today;
                        chiTietCT_KhuyenMaiContext.tbl_ChiTietCT_KhuyenMai.Add(detail);
                    }
                    await chiTietCT_KhuyenMaiContext.SaveChangesAsync();

                    // Hoàn tất giao dịch
                    scope.Complete();
                    return true; // Cập nhật thành công
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Có lỗi xảy ra khi cập nhật dữ liệu.", ex);
                }
            }
        }

        public async Task<bool> DeletePromotion(Guid maKhuyenMai)
        {
            using var transaction = await chuongTrinhKhuyenMaiContext.Database.BeginTransactionAsync();
            try
            {
                // Kiểm tra trạng thái của chương trình khuyến mãi trước khi xóa
                var chuongTrinhKhuyenMai = await chuongTrinhKhuyenMaiContext.tbl_ChuongTrinhKhuyenMai
                    .FirstOrDefaultAsync(p => p.MaKhuyenMai == maKhuyenMai);

                if (chuongTrinhKhuyenMai == null)
                {
                    return false;
                }

                // Tạo tham số cho stored procedure
                var parameters = new[]
                {
                    new SqlParameter("@MaKhuyenMai", SqlDbType.UniqueIdentifier) { Value = maKhuyenMai }
                };

                // Thực thi stored procedure xóa chi tiết phiếu nhập
                await chuongTrinhKhuyenMaiContext.Database.ExecuteSqlRawAsync(
                    "EXEC DeletePurchaseOrderRequest @MaKhuyenMai", parameters);

                // Commit transaction sau khi thực hiện thành công
                await transaction.CommitAsync();
                return true; // Nếu xóa thành công, trả về true
            }
            catch (Exception ex)
            {
                // Nếu có lỗi, rollback transaction
                await transaction.RollbackAsync();
                throw new Exception("An error occurred while deleting the purchase order and its details", ex);
            }
        }
    }
}
