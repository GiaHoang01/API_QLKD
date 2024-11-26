using API_KeoDua.Data;
using API_KeoDua.Reponsitory.Interface;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Text;
using Dapper;
using Microsoft.Data.SqlClient;
using API_KeoDua.DataView;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Linq;
using System.Transactions;

namespace API_KeoDua.Reponsitory.Implement
{
    public class HoaDonBanHangReponsitory:IHoaDonBanHangReponsitory
    {
        private readonly HoaDonBanHangContext hoaDonBanHangContext;
        private readonly NhanVienContext nhanVienContext;
        private readonly KhachHangContext khachHangContext;
        private readonly HinhThucThanhToanContext hinhThucThanhToanContext;
        private readonly GioHangContext gioHangContext;
        private readonly CT_HoaDonBanHangContext cT_HoaDonBanHangContext;
        public HoaDonBanHangReponsitory(HoaDonBanHangContext hoaDonBanHangContext,NhanVienContext nhanVienContext,KhachHangContext khachHangContext,HinhThucThanhToanContext hinhThucThanhToanContext,GioHangContext gioHangContext, CT_HoaDonBanHangContext cT_HoaDonBanHangContext)
        {
            this.hoaDonBanHangContext = hoaDonBanHangContext;
            this.nhanVienContext = nhanVienContext;
            this.khachHangContext = khachHangContext;
            this.hinhThucThanhToanContext = hinhThucThanhToanContext;
            this.gioHangContext = gioHangContext;
            this.cT_HoaDonBanHangContext=cT_HoaDonBanHangContext ;
        }
        public int TotalRows { get; set; }

        #region Xác nhận hóa đơn bán hàng
        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchString"></param>
        /// <param name="employeeId"></param>
        /// <param name="cartId"></param>
        /// <param name="customerId"></param>
        /// <param name="maHinhThuc"></param>
        /// <param name="startRow"></param>
        /// <param name="maxRows"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<List<HoaDonBanHangView>> GetAllSaleInVoiceWithWait(DateTime fromDate, DateTime toDate, string searchString, Guid?employeeId,Guid?cartId,Guid? customerId,string? maHinhThuc, int startRow, int maxRows)
        {
            try
            {
                string sqlWhere = " WHERE TrangThai = N'Chờ xác nhận' AND h.MaKhachHang = k.MaKhachHang AND  NgayBan >= @FromDate AND NgayBan <= @ToDate"; // Điều kiện mặc định
                var param = new DynamicParameters();
                param.Add("@FromDate", fromDate);
                param.Add("@ToDate", toDate);
                // Lọc theo từ khóa tìm kiếm
                if (!string.IsNullOrEmpty(searchString))
                {
                    sqlWhere += " AND LTRIM(RTRIM(GhiChu)) COLLATE SQL_Latin1_General_CP1_CI_AS LIKE @SearchString";
                    param.Add("@SearchString", $"%{searchString}%");
                }

                // Lọc theo EmployeeID (nếu có)
                if (employeeId.HasValue)
                {
                    sqlWhere += " AND h.MaNV = @MaNV";
                    param.Add("@MaNV", employeeId);
                }

                // Lọc theo CartID (nếu có)
                if (cartId!=null)
                {
                    sqlWhere += " AND h.MaGioHang = @MaGioHang";
                    param.Add("@MaGioHang", cartId);
                }

                // Lọc theo CustomerID
                if (customerId != null)
                {
                    sqlWhere += " AND h.MaKhachHang = @MaKhachHang";
                    param.Add("@MaKhachHang", customerId);
                }

                // Lọc theo mã hình thức (nếu có)
                if (!string.IsNullOrEmpty(maHinhThuc))
                {
                    sqlWhere += " AND h.MaHinhThuc = @MaHinhThuc";
                    param.Add("@MaHinhThuc", maHinhThuc);
                }


                // Tạo câu truy vấn với điều kiện WHERE và phân trang
                string sqlQuery = $@"
                    SELECT COUNT(1) FROM tbl_HoaDonBanHang  h,tbl_KhachHang k WITH (NOLOCK) {sqlWhere};
                    SELECT * FROM tbl_HoaDonBanHang h, tbl_KhachHang k WITH (NOLOCK) {sqlWhere}
                    ORDER BY NgayBan ASC
                    OFFSET @StartRow ROWS FETCH NEXT @MaxRows ROWS ONLY;";

                param.Add("@StartRow", startRow);
                param.Add("@MaxRows", maxRows);

                using (var connection = this.hoaDonBanHangContext.CreateConnection())
                {
                    using (var multi = await connection.QueryMultipleAsync(sqlQuery, param))
                    {
                        // Lấy tổng số hàng từ truy vấn đầu tiên
                        this.TotalRows = (await multi.ReadAsync<int>()).Single();
                        // Lấy danh sách nhân viên từ truy vấn thứ hai
                        return (await multi.ReadAsync<HoaDonBanHangView>()).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                // Ghi log hoặc xử lý ngoại lệ
                throw new Exception("An error occurred while fetching saleinvoice", ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="maHoaDon"></param>
        /// <param name="maNV"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> ConfirmSaleInvoice(Guid maHoaDon,Guid maNV)
        { 
            try
            {
                string sqlUpdate = @"
                UPDATE tbl_HoaDonBanHang
                SET TrangThai = N'Mới tạo',
                    NgayBan = GETDATE(),
                    MaNV = @MaNV
                WHERE MaHoaDon = @MaHoaDon AND TrangThai = N'Chờ xác nhận';"; // Điều kiện mặc định
                var param = new DynamicParameters();
                param.Add("@MaHoaDon", maHoaDon);
                param.Add("@MaNV", maNV);
               

                using (var connection = this.hoaDonBanHangContext.CreateConnection())
                {
                    int rowsAffected = await connection.ExecuteAsync(sqlUpdate, param);
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                // Ghi log hoặc xử lý ngoại lệ
                throw new Exception("An error occurred while fetching saleinvoice", ex);
            }
        }

        /// <summary>
        /// Hủy hàng do khách đổi ý trên giao diện khách hàng
        /// </summary>
        /// <param name="maHoaDon"></param>
        /// <param name="maNV"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<bool> CancelSaleInvoice(Guid maHoaDon, Guid maNV)
        {
            try
            {
                string sqlUpdate = @"
                UPDATE tbl_HoaDonBanHang
                SET TrangThai = N'Đã hủy do khách đổi ý',
                    NgayBan = GETDATE(),
                    MaNV = @MaNV
                WHERE MaHoaDon = @MaHoaDon AND TrangThai = N'Chờ xác nhận' AND GhiChu LIKE N'%Đã hủy do khách đổi ý%'"; // Điều kiện mặc định
                var param = new DynamicParameters();
                param.Add("@MaHoaDon", maHoaDon);
                param.Add("@MaNV", maNV);


                using (var connection = this.hoaDonBanHangContext.CreateConnection())
                {
                    int rowsAffected = await connection.ExecuteAsync(sqlUpdate, param);
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                // Ghi log hoặc xử lý ngoại lệ
                throw new Exception("An error occurred while fetching saleinvoice", ex);
            }
        }
        #endregion

        #region Hóa đơn bán hàng
        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchString"></param>
        /// <param name="employeeId"></param>
        /// <param name="cartId"></param>
        /// <param name="customerId"></param>
        /// <param name="maHinhThuc"></param>
        /// <param name="startRow"></param>
        /// <param name="maxRows"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<List<HoaDonBanHangView>> GetAllSaleInVoice(DateTime fromDate, DateTime toDate, string searchString, Guid? employeeId, Guid? cartId, Guid? customerId, string? maHinhThuc, int startRow, int maxRows)
        {
            try
            {
                string sqlWhere = " WHERE h.MaKhachHang=k.MaKhachHang AND h.MaNV=n.MaNV AND NgayBan >= @FromDate AND NgayBan <= @ToDate"; // Điều kiện mặc định
                var param = new DynamicParameters();
                param.Add("@FromDate", fromDate);
                param.Add("@ToDate", toDate);

                // Lọc theo từ khóa tìm kiếm
                if (!string.IsNullOrEmpty(searchString))
                {
                    sqlWhere += " AND LTRIM(RTRIM(GhiChu)) COLLATE SQL_Latin1_General_CP1_CI_AS LIKE @SearchString";
                    param.Add("@SearchString", $"%{searchString}%");
                }

                // Lọc theo EmployeeID (nếu có)
                if (employeeId!=null)
                {
                    sqlWhere += " AND h.MaNV = @MaNV";
                    param.Add("@MaNV", employeeId);
                }

                // Lọc theo CartID (nếu có)
                if (cartId!=null)
                {
                    sqlWhere += " AND h.MaGioHang = @MaGioHang";
                    param.Add("@MaGioHang", cartId);
                }

                // Lọc theo CustomerID
                if (customerId != null)
                {
                    sqlWhere += " AND h.MaKhachHang = @MaKhachHang";
                    param.Add("@MaKhachHang", customerId);
                }
                // Lọc theo mã hình thức (nếu có)
                if (!string.IsNullOrEmpty(maHinhThuc))
                {
                    sqlWhere += " AND h.MaHinhThuc = @MaHinhThuc";
                    param.Add("@MaHinhThuc", maHinhThuc);
                }

                // Tạo câu truy vấn với điều kiện WHERE và phân trang
                string sqlQuery = $@"
                    SELECT COUNT(1) FROM tbl_HoaDonBanHang h,tbl_NhanVien n, tbl_KhachHang k WITH (NOLOCK) {sqlWhere};
                    SELECT * FROM tbl_HoaDonBanHang h,tbl_NhanVien n, tbl_KhachHang k WITH (NOLOCK) {sqlWhere}
                    ORDER BY NgayBan ASC
                    OFFSET @StartRow ROWS FETCH NEXT @MaxRows ROWS ONLY;";

                param.Add("@StartRow", startRow);
                param.Add("@MaxRows", maxRows);

                using (var connection = this.hoaDonBanHangContext.CreateConnection())
                {
                    using (var multi = await connection.QueryMultipleAsync(sqlQuery, param))
                    {
                        // Lấy tổng số hàng từ truy vấn đầu tiên
                        this.TotalRows = (await multi.ReadAsync<int>()).Single();
                        // Lấy danh sách nhân viên từ truy vấn thứ hai
                        return (await multi.ReadAsync<HoaDonBanHangView>()).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                // Ghi log hoặc xử lý ngoại lệ
                throw new Exception("An error occurred while fetching saleinvoice", ex);
            }
        }
        

        public async Task<List<object>> QuickSearchSaleInvoiceNewCreated(string searchString)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                var sqlWhere = new StringBuilder();

                if (!string.IsNullOrEmpty(searchString))
                {
                    sqlWhere.Append(" AND MaHoaDon like @SearchString ESCAPE '\\' ");
                    param.Add("SearchString", $"%{searchString}%");
                }

                string sqlQuery = @"
                                    SELECT 
                                        h.*, 
                                        k.* 
                                    FROM tbl_HoaDonBanHang h
                                    INNER JOIN tbl_KhachHang k ON h.MaKhachHang = k.MaKhachHang
                                    WHERE h.TrangThai = N'Mới tạo' " + sqlWhere;

                using (var connection = this.hoaDonBanHangContext.CreateConnection())
                {
                    var resultData = await connection.QueryAsync(sqlQuery, param);
                    var response = resultData.Select(row => new
                    {
                        MaHoaDon = row.MaHoaDon,
                        NgayBan = row.NgayBan,
                        TrangThai = row.TrangThai,
                        TongTriGia = row.TongTriGia,
                        GhiChu = row.GhiChu,
                        MaKhachHang = row.MaKhachHang,
                        TenKhachHang = row.TenKhachHang,
                        Email = row.Email,
                        SDT = row.SDT,
                        GioiTinh = row.GioiTinh,
                        MaLoaiKH = row.MaLoaiKH,
                    }).ToList<object>();

                    return response;
                    
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hoaDonBanHang"></param>
        /// <param name="cT_HoaDonBanHangs"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task AddSaleInvoice(HoaDonBanHang hoaDonBanHang, List<CT_HoaDonBanHang> cT_HoaDonBanHangs)
        {
            // Sử dụng TransactionScope cho tất cả các DbContext
            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    // Thêm phiếu nhập vào bảng tbl_PhieuNhapHang
                    hoaDonBanHang.NgayBan = DateTime.Now;
                    hoaDonBanHang.TrangThai = "Mới tạo";
                    hoaDonBanHangContext.tbl_HoaDonBanHang.Add(hoaDonBanHang);
                    await hoaDonBanHangContext.SaveChangesAsync();

                    // Thêm chi tiết phiếu nhập vào bảng tbl_CT_PhieuNhap
                    foreach (var detail in cT_HoaDonBanHangs)
                    {
                        detail.MaHoaDon = (Guid)hoaDonBanHang.MaHoaDon;
                        cT_HoaDonBanHangContext.tbl_CT_HoaDonBanHang.Add(detail);
                    }
                    await cT_HoaDonBanHangContext.SaveChangesAsync();

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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hoaDonBanHang"></param>
        /// <param name="cT_HoaDonBanHangs"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<bool> UpdateSaleInvoice (HoaDonBanHang hoaDonBanHang, List<CT_HoaDonBanHang> cT_HoaDonBanHangs)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    // Tìm phiếu nhập hiện tại
                    var existingHoaDon = await hoaDonBanHangContext.tbl_HoaDonBanHang.FindAsync(hoaDonBanHang.MaHoaDon);
                    if (existingHoaDon == null)
                    {
                        return false; // Không tìm thấy phiếu nhập
                    }

                    // Cập nhật thông tin phiếu nhập
                    existingHoaDon.MaNV = hoaDonBanHang.MaNV;
                    existingHoaDon.MaKhachHang = hoaDonBanHang.MaKhachHang;
                    existingHoaDon.NgayBan = hoaDonBanHang.NgayBan;
                    existingHoaDon.NgayThanhToan = hoaDonBanHang.NgayThanhToan;
                    existingHoaDon.MaHinhThuc = hoaDonBanHang.MaHinhThuc;
                    existingHoaDon.GhiChu = hoaDonBanHang.GhiChu;
                    await hoaDonBanHangContext.SaveChangesAsync();

                    string deleteQuery = @"DELETE FROM tbl_CT_HoaDonBanHang WHERE MaHoaDon = @MaHoaDon";
                    var parameter = new SqlParameter("@MaHoaDon", hoaDonBanHang.MaHoaDon);
                    await cT_HoaDonBanHangContext.Database.ExecuteSqlRawAsync(deleteQuery, parameter);

                    // **Thêm chi tiết phiếu nhập mới**
                    foreach (var detail in cT_HoaDonBanHangs)
                    {
                        detail.MaHoaDon = (Guid)hoaDonBanHang.MaHoaDon;
                        detail.ThanhTien = detail.DonGia * detail.SoLuong;
                        cT_HoaDonBanHangContext.tbl_CT_HoaDonBanHang.Add(detail);
                    }
                    await cT_HoaDonBanHangContext.SaveChangesAsync();

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="maHoaDon"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public async Task<bool> DeleteSaleInvoice (Guid maHoaDon)
        {
            using var transaction = await hoaDonBanHangContext.Database.BeginTransactionAsync();
            try
            {
                // Kiểm tra trạng thái của hóa đơn trước khi xóa
                var hoaDon = await hoaDonBanHangContext.tbl_HoaDonBanHang
                    .FirstOrDefaultAsync(p => p.MaHoaDon == maHoaDon);

                if (hoaDon == null)
                {
                    return false;
                }
                if (hoaDon.TrangThai != "Mới tạo" && hoaDon.TrangThai!="Chờ xác nhận")
                {
                    return false;
                }

                // Tạo tham số cho stored procedure
                var parameters = new[]
                {
                    new SqlParameter("@MaHoaDon", SqlDbType.UniqueIdentifier) { Value = maHoaDon }
                };

                // Thực thi stored procedure xóa chi tiết phiếu nhập
                await hoaDonBanHangContext.Database.ExecuteSqlRawAsync(
                    "EXEC DeleteSaleInvoice @MaHoaDon", parameters);

                // Commit transaction sau khi thực hiện thành công
                await transaction.CommitAsync();
                return true; // Nếu xóa thành công, trả về true
            }
            catch (Exception ex)
            {
                // Nếu có lỗi, rollback transaction
                await transaction.RollbackAsync();
                throw new Exception("An error occurred while deleting the sale invoice and its details", ex);
            }
        }
        #endregion
    }
}
