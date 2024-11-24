using API_KeoDua.Data;
using API_KeoDua.DataView;
using API_KeoDua.Reponsitory.Interface;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Transactions;
namespace API_KeoDua.Reponsitory.Implement
{
    public class PhieuNhapHangReponsitory : IPhieuNhapHangReponsitory
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

        public async Task<List<PhieuNhapHang>> GetAllPurchaseRequest(DateTime fromDate, DateTime toDate, string searchString, int startRow, int maxRows)
        {
            try
            {
                var sqlWhere = new StringBuilder();
                var param = new DynamicParameters();

                if (!string.IsNullOrEmpty(searchString))
                {
                    sqlWhere.Append("and MaPhieuNhap COLLATE SQL_Latin1_General_CP1_CI_AS LIKE @SearchString OR MaNV LIKE @SearchString");
                    param.Add("@SearchString", $"%{searchString}%");
                }
                else
                {
                    sqlWhere.Append("and NgayNhap >= @FromDate AND NgayNhap <= @ToDate AND NgayDat >= @FromDate AND NgayDat <= @ToDate");
                    param.Add("@FromDate", fromDate);
                    param.Add("@ToDate", toDate);
                }

                string sqlQuery = $@"SELECT COUNT(1) AS TotalRows FROM tbl_PhieuNhapHang WITH (NOLOCK) Where TrangThai=N'Mới tạo' {sqlWhere};
                                     SELECT * FROM tbl_PhieuNhapHang WITH (NOLOCK) Where TrangThai=N'Mới tạo' {sqlWhere} ORDER BY MaPhieuNhap ASC OFFSET @StartRow ROWS FETCH NEXT @MaxRows ROWS ONLY;";
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

        public async Task<(PhieuNhapHang phieuNhap, List<CT_PhieuNhap> chiTietPhieuNhap)> GetPurchase_ByID(Guid? maPhieuNhap)
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

        public async Task AddPurchaseOrderRequest(PhieuNhapHang phieuNhapHang, List<CT_PhieuNhap> ctPhieuNhaps)
        {
            // Sử dụng TransactionScope cho tất cả các DbContext
            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    // Thêm phiếu nhập vào bảng tbl_PhieuNhapHang
                    phieuNhapHang.NgayDat = DateTime.Now;
                    phieuNhapHang.TrangThai = "Mới tạo";
                    phieuNhapHangContext.tbl_PhieuNhapHang.Add(phieuNhapHang);
                    await phieuNhapHangContext.SaveChangesAsync();

                    // Thêm chi tiết phiếu nhập vào bảng tbl_CT_PhieuNhap
                    foreach (var detail in ctPhieuNhaps)
                    {
                        detail.MaPhieuNhap = (Guid)phieuNhapHang.MaPhieuNhap;
                        cT_PhieuNhapContext.tbl_CT_PhieuNhap.Add(detail);
                    }
                    await cT_PhieuNhapContext.SaveChangesAsync();

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

        public async Task<bool> UpdatePurchaseOrderRequest(PhieuNhapHang phieuNhapHang, List<CT_PhieuNhap> ctPhieuNhaps)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    // Tìm phiếu nhập hiện tại
                    var existingPhieuNhapHang = await phieuNhapHangContext.tbl_PhieuNhapHang.FindAsync(phieuNhapHang.MaPhieuNhap);
                    if (existingPhieuNhapHang == null)
                    {
                        return false; // Không tìm thấy phiếu nhập
                    }

                    // Cập nhật thông tin phiếu nhập
                    existingPhieuNhapHang.MaNCC = phieuNhapHang.MaNCC;
                    existingPhieuNhapHang.MaNV = phieuNhapHang.MaNV;
                    existingPhieuNhapHang.NgayNhap = phieuNhapHang.NgayNhap;
                    existingPhieuNhapHang.GhiChu = phieuNhapHang.GhiChu;
                    await phieuNhapHangContext.SaveChangesAsync();

                    string deleteQuery = @"DELETE FROM tbl_CT_PhieuNhap WHERE MaPhieuNhap = @MaPhieuNhap";
                    var parameter = new SqlParameter("@MaPhieuNhap", phieuNhapHang.MaPhieuNhap);
                    await cT_PhieuNhapContext.Database.ExecuteSqlRawAsync(deleteQuery, parameter);

                    // **Thêm chi tiết phiếu nhập mới**
                    foreach (var detail in ctPhieuNhaps)
                    {
                        detail.MaPhieuNhap = (Guid)phieuNhapHang.MaPhieuNhap;
                        detail.ThanhTien = detail.DonGia * detail.SoLuongDat;
                        cT_PhieuNhapContext.tbl_CT_PhieuNhap.Add(detail);
                    }
                    await cT_PhieuNhapContext.SaveChangesAsync();

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

        public async Task<bool> DeletePurchaseOrderRequest(Guid maPhieuNhap)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    // Tìm phiếu nhập hàng cần xóa
                    var existingPhieuNhapHang = await phieuNhapHangContext.tbl_PhieuNhapHang.FindAsync(maPhieuNhap);
                    if (existingPhieuNhapHang == null)
                    {
                        return false; // Không tìm thấy phiếu nhập hàng
                    }

                    // Xóa tất cả chi tiết phiếu nhập liên quan
                    string deleteDetailsQuery = @"DELETE FROM tbl_CT_PhieuNhap WHERE MaPhieuNhap = @MaPhieuNhap";
                    var parameter = new SqlParameter("@MaPhieuNhap", maPhieuNhap);
                    await cT_PhieuNhapContext.Database.ExecuteSqlRawAsync(deleteDetailsQuery, parameter);

                    // Xóa phiếu nhập hàng
                    phieuNhapHangContext.tbl_PhieuNhapHang.Remove(existingPhieuNhapHang);
                    await phieuNhapHangContext.SaveChangesAsync();

                    // Hoàn tất giao dịch
                    scope.Complete();
                    return true; // Xóa thành công
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Có lỗi xảy ra khi xóa dữ liệu.", ex);
                }
            }
        }


    }
}
