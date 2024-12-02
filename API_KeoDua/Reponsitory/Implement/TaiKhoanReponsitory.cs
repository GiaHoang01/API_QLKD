﻿using API_KeoDua.Data;
using API_KeoDua.Models;
using API_KeoDua.Reponsitory.Interface;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using System.Text;

namespace API_KeoDua.Reponsitory.Implement
{
    public class TaiKhoanReponsitory : ITaiKhoanReponsitory
    {
        private readonly TaiKhoanContext _context;
        private readonly NhanVienContext _nhanVienContext;
        private readonly NhomQuyenContext nhomQuyenContext;
        private readonly QuyenContext quyenContext;
        public TaiKhoanReponsitory(TaiKhoanContext context, NhanVienContext nhanVienContext, NhomQuyenContext nhomQuyenContext, QuyenContext quyenContext)
        {
            _context = context;
            _nhanVienContext = nhanVienContext;
            this.nhomQuyenContext = nhomQuyenContext;
            this.quyenContext = quyenContext;
        }
        public int TotalRows { get; set; }

        public async Task<bool> IsCheckAccount(string username, string password)
        {
            var account = await _context.TaiKhoan.FirstOrDefaultAsync(acc => acc.TenTaiKhoan == username && acc.MatKhau == password);
            return account != null;
        }

        public async Task<string> login(string user, string pass)
        {
            try
            {
                // Kiểm tra tài khoản trong context mới
                var account = await _context.TaiKhoan.FirstOrDefaultAsync(acc => acc.TenTaiKhoan == user && acc.MatKhau == pass);
                if (account == null)
                {
                    return null;
                }

                // Fetch employee info from database using NhânViênContext
                var employee = await _nhanVienContext.tbl_NhanVien
                    .FirstOrDefaultAsync(nv => nv.TenTaiKhoan == account.TenTaiKhoan);

                return employee?.TenNV ?? null;
            }
            catch (Exception ex)
            {
                return $"Đã xảy ra lỗi: {ex.Message}"; // Handle error if any
            }
        }

        public async Task<List<string>> getDataPermission(string userName)
        {
            try
            {
                // Ensure userName is provided
                if (string.IsNullOrEmpty(userName))
                {
                    throw new ArgumentException("Username cannot be null or empty.");
                }

                // Initialize the parameters for SQL query
                DynamicParameters param = new DynamicParameters();
                param.Add("UserName", userName);

                // SQL query to fetch permissions
                string sqlQuery = @"
            SELECT q.TenQuyen
            FROM tbl_Quyen q
            INNER JOIN tbl_CapQuyen cq ON q.MaQuyen = cq.MaQuyen
            INNER JOIN tbl_NhomQuyen nq ON nq.MaNhomQuyen = cq.MaNhomQuyen
            INNER JOIN tbl_TaiKhoan tk ON tk.MaNhomQuyen = nq.MaNhomQuyen
            WHERE tk.TenTaiKhoan = @UserName";

                // Ensure nhomQuyenContext is initialized
                if (this.nhomQuyenContext == null)
                {
                    throw new InvalidOperationException("nhomQuyenContext is not initialized.");
                }

                // Create connection using nhomQuyenContext
                using (var connection = this.nhomQuyenContext.CreateConnection())
                {
                    // Execute query using Dapper
                    var permissions = await connection.QueryAsync<string>(sqlQuery, param);

                    // If no permissions found, return an empty list
                    return permissions?.ToList() ?? new List<string>();
                }
            }
            catch (Exception ex)
            {
                // Log the error for debugging
                // You can replace this with your logging mechanism if available
                Console.Error.WriteLine($"Error occurred while fetching permissions for user {userName}: {ex.Message}");

                // Rethrow the exception with additional context
                throw new Exception($"An error occurred while fetching permissions for user: {userName}", ex);
            }
        }

    }
}
