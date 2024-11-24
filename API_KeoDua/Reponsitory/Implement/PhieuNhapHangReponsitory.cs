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

        public async Task<List<PhieuNhapHang>> GetAllPurchaseRequest_NoSubmit(DateTime fromDate, DateTime toDate, string searchString, int startRow, int maxRows)
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

                string sqlQuery = $@"SELECT COUNT(1) AS TotalRows FROM tbl_PhieuNhapHang WITH (NOLOCK) Where TrangThai!=N'Hoàn tất' {sqlWhere};
                                     SELECT * FROM tbl_PhieuNhapHang WITH (NOLOCK) Where TrangThai!=N'Hoàn tất' {sqlWhere} ORDER BY MaPhieuNhap ASC OFFSET @StartRow ROWS FETCH NEXT @MaxRows ROWS ONLY;";
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

        public async Task<bool> ConfirmPurchaseOrder(PhieuNhapHang phieuNhapHang, List<CT_PhieuNhap> cT_PhieuNhaps)
        {
            // Xác định trạng thái phiếu nhập
            bool isComplete = true;
            bool hasPartial = false;

            foreach (var ct in cT_PhieuNhaps)
            {
                if (ct.SoLuong != ct.SoLuongDat)
                {
                    isComplete = false;
                    hasPartial = true;
                    break;
                }
            }

            // Cập nhật trạng thái của phiếu nhập
            if (isComplete)
            {
                phieuNhapHang.TrangThai = "Hoàn tất";
            }
            else if (hasPartial)
            {
                phieuNhapHang.TrangThai = "Hoàn tất một nữa";
            }

            // Bắt đầu giao dịch sử dụng DbContext quản lý giao dịch nội bộ
            var strategy = phieuNhapHangContext.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                // Bắt đầu transaction
                using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
                {
                    try
                    {
                        // Lấy phiếu nhập hiện tại
                        var existingPhieuNhapHang = await phieuNhapHangContext.tbl_PhieuNhapHang
                            .FirstOrDefaultAsync(p => p.MaPhieuNhap == phieuNhapHang.MaPhieuNhap);

                        if (existingPhieuNhapHang == null)
                        {
                            throw new InvalidOperationException($"Không tìm thấy phiếu nhập với mã: {phieuNhapHang.MaPhieuNhap}");
                        }

                        // Cập nhật trạng thái phiếu nhập
                        existingPhieuNhapHang.TrangThai = phieuNhapHang.TrangThai;
                        await phieuNhapHangContext.SaveChangesAsync();

                        // Xóa các chi tiết phiếu nhập cũ
                        string deleteQuery = @"DELETE FROM tbl_CT_PhieuNhap WHERE MaPhieuNhap = @MaPhieuNhap";
                        var parameter = new SqlParameter("@MaPhieuNhap", phieuNhapHang.MaPhieuNhap);
                        await cT_PhieuNhapContext.Database.ExecuteSqlRawAsync(deleteQuery, parameter);

                        // **Thêm chi tiết phiếu nhập mới**
                        foreach (var ct in cT_PhieuNhaps)
                        {
                            ct.MaPhieuNhap = phieuNhapHang.MaPhieuNhap;
                            ct.ThanhTien = ct.DonGia * ct.SoLuongDat; // Assuming ThanhTien should be recalculated
                            cT_PhieuNhapContext.tbl_CT_PhieuNhap.Add(ct);
                        }

                        await cT_PhieuNhapContext.SaveChangesAsync();

                        // Hoàn tất giao dịch
                        scope.Complete();

                        return true; // Cập nhật thành công
                    }
                    catch (Exception ex)
                    {
                        // Handle the exception and rollback if any error occurs
                        Console.WriteLine($"Error: {ex.Message}");
                        throw new InvalidOperationException("Có lỗi xảy ra khi xác nhận phiếu nhập.", ex);
                    }
                }
            });
        }

        public async Task<Guid> CreateNewPurchaseOrder(Guid maPhieuNhap)
        {
            // Lấy các chi tiết phiếu nhập của phiếu nhập cũ
            var ctPhieuNhaps = await cT_PhieuNhapContext.tbl_CT_PhieuNhap
                .Where(ct => ct.MaPhieuNhap == maPhieuNhap)
                .ToListAsync();

            if (ctPhieuNhaps == null || !ctPhieuNhaps.Any())
            {
                throw new InvalidOperationException("Không có chi tiết phiếu nhập để tạo phiếu mới.");
            }

            // Lấy thông tin phiếu nhập cũ
            var existingPhieuNhapHang = await phieuNhapHangContext.tbl_PhieuNhapHang
                .Where(p => p.MaPhieuNhap == maPhieuNhap)
                .FirstOrDefaultAsync();

            if (existingPhieuNhapHang == null)
            {
                throw new InvalidOperationException("Không tìm thấy phiếu nhập với mã: " + maPhieuNhap);
            }

            // Tạo phiếu nhập mới với thông tin gốc từ phiếu nhập cũ
            var newPhieuNhapHang = new PhieuNhapHang
            {
                MaPhieuNhap = Guid.NewGuid(), // Tạo mã phiếu nhập mới
                TrangThai = "Đặt lại hàng thiếu", // Trạng thái phiếu nhập mới
                NgayNhap = DateTime.Now,
                NgayDat = DateTime.Now,
                MaNCC = existingPhieuNhapHang.MaNCC,  // Gán lại nhà cung cấp từ phiếu cũ
                MaNV = existingPhieuNhapHang.MaNV,  // Gán lại nhân viên từ phiếu cũ
                GhiChu = existingPhieuNhapHang.GhiChu, // Ghi chú từ phiếu cũ
            };

            // Thêm phiếu nhập mới vào cơ sở dữ liệu
            phieuNhapHangContext.tbl_PhieuNhapHang.Add(newPhieuNhapHang);
            await phieuNhapHangContext.SaveChangesAsync();

            // Tạo chi tiết phiếu nhập mới dựa trên số lượng đặt trừ số lượng nhận
            foreach (var ct in ctPhieuNhaps)
            {
                var soLuongMoi = ct.SoLuongDat - ct.SoLuong; // Tính số lượng mới

                // Kiểm tra nếu số lượng còn lại sau khi trừ > 0
                if (soLuongMoi > 0)
                {
                    var newCT_PhieuNhap = new CT_PhieuNhap
                    {
                        MaPhieuNhap = newPhieuNhapHang.MaPhieuNhap,
                        MaHangHoa = ct.MaHangHoa,
                        SoLuongDat = soLuongMoi,
                        DonGia = ct.DonGia
                    };

                    cT_PhieuNhapContext.tbl_CT_PhieuNhap.Add(newCT_PhieuNhap);
                }
            }

            await cT_PhieuNhapContext.SaveChangesAsync();

            // Cập nhật phiếu nhập cũ
            existingPhieuNhapHang.TrangThai = "Hoàn tất"; // Cập nhật trạng thái phiếu nhập cũ

            // Cập nhật lại số lượng đặt của các chi tiết phiếu nhập cũ thành số lượng đã nhận
            foreach (var ct in ctPhieuNhaps)
            {
                ct.SoLuongDat = ct.SoLuong; 
                cT_PhieuNhapContext.tbl_CT_PhieuNhap.Update(ct);
            }

            // Lưu thay đổi cho phiếu nhập cũ
            await cT_PhieuNhapContext.SaveChangesAsync();

            // Trả về mã phiếu nhập mới
            return newPhieuNhapHang.MaPhieuNhap;
        }

    }
}
