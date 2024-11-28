using API_KeoDua.Data;
using API_KeoDua.Models;
using API_KeoDua.Reponsitory.Interface;
using Microsoft.EntityFrameworkCore;

namespace API_KeoDua.Reponsitory.Implement
{
    public class TaiKhoanReponsitory : ITaiKhoanReponsitory
    {
        private readonly TaiKhoanContext _context;
        private readonly NhanVienContext _nhanVienContext;
        public TaiKhoanReponsitory(TaiKhoanContext context,NhanVienContext nhanVienContext)
        {
            _context = context;
            _nhanVienContext = nhanVienContext;
        }
        public int TotalRows { get ; set ; }

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


    }
}
