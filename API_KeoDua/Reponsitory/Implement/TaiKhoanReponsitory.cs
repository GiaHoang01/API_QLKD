using API_KeoDua.Data;
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
            this._nhanVienContext=nhanVienContext;
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
                // Kiểm tra tài khoản có hợp lệ hay không
                bool isAccountValid = await IsCheckAccount(user, pass);

                if (!isAccountValid)
                {
                    return "Tài khoản hoặc mật khẩu không đúng";
                }

                // Lấy thông tin tài khoản từ DbContext _context
                var account = await _context.TaiKhoan
                    .AsNoTracking()
                    .FirstOrDefaultAsync(tk => tk.TenTaiKhoan == user);

                if (account == null)
                {
                    return "Không tìm thấy tài khoản";
                }

                // Lấy thông tin nhân viên từ DbContext _nhanVienContext
                var employee = await _nhanVienContext.tbl_NhanVien
                    .AsNoTracking()
                    .FirstOrDefaultAsync(nv => nv.TenTaiKhoan == account.TenTaiKhoan);

                return employee?.TenNV ?? "Tên nhân viên không tồn tại";
            }
            catch (Exception ex)
            {
                // Xử lý lỗi và trả về thông báo lỗi
                return $"Đã xảy ra lỗi: {ex.Message}";
            }
        }



    }
}
