﻿using API_KeoDua.Data;
using API_KeoDua.Reponsitory.Interface;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Text;
using Dapper;
using Microsoft.Data.SqlClient;
using API_KeoDua.DataView;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

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
        public async Task<List<HoaDonBanHang>> GetAllSaleInVoiceWithWait(string searchString, Guid?employeeId,Guid?cartId,Guid? customerId,string? maHinhThuc, int startRow, int maxRows)
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
                    sqlWhere += " AND MaNV = @MaNV";
                    param.Add("@MaNV", employeeId);
                }

                // Lọc theo CartID (nếu có)
                if (cartId!=null)
                {
                    sqlWhere += " AND MaGioHang = @MaGioHang";
                    param.Add("@MaGioHang", cartId);
                }

                // Lọc theo CustomerID
                if (customerId != null)
                {
                    sqlWhere += " AND MaKhachHang = @MaKhachHang";
                    param.Add("@MaKhachHang", customerId);
                }

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
        public async Task<List<HoaDonBanHang>> GetAllSaleInVoice(string searchString, Guid? employeeId, Guid? cartId, Guid? customerId, string? maHinhThuc, int startRow, int maxRows)
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
                    sqlWhere += " AND MaNV = @MaNV";
                    param.Add("@MaNV", employeeId);
                }

                // Lọc theo CartID (nếu có)
                if (cartId!=null)
                {
                    sqlWhere += " AND MaGioHang = @MaGioHang";
                    param.Add("@MaGioHang", cartId);
                }

                // Lọc theo CustomerID
                if (customerId != null)
                {
                    sqlWhere += " AND MaKhachHang = @MaKhachHang";
                    param.Add("@MaKhachHang", customerId);
                }
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
                throw new Exception("An error occurred while fetching saleinvoice", ex);
            }
        }
        #endregion

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
                                        h.MaHoaDon, h.NgayTao, h.TrangThai, h.MaKhachHang, 
                                        k.TenKhachHang, k.Email, k.Sdt
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
    }
}
