using API_KeoDua.Data;
using API_KeoDua.Reponsitory.Interface;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Text;
using Dapper;
using Microsoft.Data.SqlClient;
using API_KeoDua.DataView;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<List<HoaDonBanHang>> GetAllSaleInVoiceWithWait(string searchString, Guid?employeeId,Guid?cartId,Guid customerId,string maHinhThuc, int startRow, int maxRows)
        {
            try
            {
                string sqlWhere = " WHERE TrangThai = N'Chờ xác nhận'"; // Điều kiện mặc định
                var param = new DynamicParameters();

                // Lọc theo từ khóa tìm kiếm
                if (!string.IsNullOrEmpty(searchString))
                {
                    sqlWhere += " AND LTRIM(RTRIM(GhiChu)) COLLATE SQL_Latin1_General_CP1_CI_AS LIKE @SearchString";
                    param.Add("@SearchString", $"%{searchString}%");
                }

                // Lọc theo EmployeeID (nếu có)
                if (employeeId.HasValue)
                {
                    sqlWhere += " AND EmployeeID = @EmployeeID";
                    param.Add("@EmployeeID", employeeId.Value);
                }

                // Lọc theo CartID (nếu có)
                if (cartId.HasValue)
                {
                    sqlWhere += " AND CartID = @CartID";
                    param.Add("@CartID", cartId.Value);
                }

                // Lọc theo CustomerID
                sqlWhere += " AND CustomerID = @CustomerID";
                param.Add("@CustomerID", customerId);

                // Lọc theo mã hình thức (nếu có)
                if (!string.IsNullOrEmpty(maHinhThuc))
                {
                    sqlWhere += " AND MaHinhThuc = @MaHinhThuc";
                    param.Add("@MaHinhThuc", maHinhThuc);
                }


                // Tạo câu truy vấn với điều kiện WHERE và phân trang
                string sqlQuery = $@"
                    SELECT COUNT(1) FROM tbl_HoaDonBanHang WITH (NOLOCK) {sqlWhere};
                    SELECT * FROM tbl_HoaDonBanHang WITH (NOLOCK) {sqlWhere}
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
                        return (await multi.ReadAsync<HoaDonBanHang>()).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                // Ghi log hoặc xử lý ngoại lệ
                throw new Exception("An error occurred while fetching employees", ex);
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
        public async Task<List<HoaDonBanHang>> GetAllSaleInVoice(string searchString, Guid? employeeId, Guid? cartId, Guid customerId, string maHinhThuc, int startRow, int maxRows)
        {
            try
            {
                string sqlWhere = " WHERE TrangThai <> N'Chờ xác nhận'"; // Điều kiện mặc định
                var param = new DynamicParameters();

                // Lọc theo từ khóa tìm kiếm
                if (!string.IsNullOrEmpty(searchString))
                {
                    sqlWhere += " AND LTRIM(RTRIM(GhiChu)) COLLATE SQL_Latin1_General_CP1_CI_AS LIKE @SearchString";
                    param.Add("@SearchString", $"%{searchString}%");
                }

                // Lọc theo EmployeeID (nếu có)
                if (employeeId!=null)
                {
                    sqlWhere += " AND EmployeeID = @EmployeeID";
                    param.Add("@EmployeeID", employeeId);
                }

                // Lọc theo CartID (nếu có)
                if (cartId!=null)
                {
                    sqlWhere += " AND CartID = @CartID";
                    param.Add("@CartID", cartId);
                }

                // Lọc theo CustomerID
                sqlWhere += " AND CustomerID = @CustomerID";
                param.Add("@CustomerID", customerId);

                // Lọc theo mã hình thức (nếu có)
                if (!string.IsNullOrEmpty(maHinhThuc))
                {
                    sqlWhere += " AND MaHinhThuc = @MaHinhThuc";
                    param.Add("@MaHinhThuc", maHinhThuc);
                }

                // Tạo câu truy vấn với điều kiện WHERE và phân trang
                string sqlQuery = $@"
                    SELECT COUNT(1) FROM tbl_HoaDonBanHang WITH (NOLOCK) {sqlWhere};
                    SELECT * FROM tbl_HoaDonBanHang WITH (NOLOCK) {sqlWhere}
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
                        return (await multi.ReadAsync<HoaDonBanHang>()).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                // Ghi log hoặc xử lý ngoại lệ
                throw new Exception("An error occurred while fetching employees", ex);
            }
        }
        #endregion
    }
}
