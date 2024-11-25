using API_KeoDua.Data;
using API_KeoDua.Reponsitory.Interface;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Text;
using Dapper;
using Microsoft.Data.SqlClient;
using API_KeoDua.DataView;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace API_KeoDua.Reponsitory.Implement
{
    public class HoaDonBanHangReponsitory:IHoaDonBanHangReponsitory
    {
        private readonly HoaDonBanHangContext hoaDonBanHangContext;
        private readonly NhanVienContext nhanVienContext;
        private readonly KhachHangContext khachHangContext;
        private readonly HinhThucThanhToanContext hinhThucThanhToanContext;
        private readonly GioHangContext gioHangContext;
        public HoaDonBanHangReponsitory(HoaDonBanHangContext hoaDonBanHangContext,NhanVienContext nhanVienContext,KhachHangContext khachHangContext,HinhThucThanhToanContext hinhThucThanhToanContext,GioHangContext gioHangContext)
        {
            this.hoaDonBanHangContext = hoaDonBanHangContext;
            this.nhanVienContext = nhanVienContext;
            this.khachHangContext = khachHangContext;
            this.hinhThucThanhToanContext = hinhThucThanhToanContext;
            this.gioHangContext = gioHangContext;
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
                string sqlWhere = " WHERE TrangThai <> N'Chờ xác nhận' AND h.MaKhachHang=k.MaKhachHang AND h.MaNV=n.MaNV AND NgayBan >= @FromDate AND NgayBan <= @ToDate"; // Điều kiện mặc định
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
        #endregion
    }
}
