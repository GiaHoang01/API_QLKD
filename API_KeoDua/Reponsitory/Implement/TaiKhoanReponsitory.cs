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
        private readonly DatabaseConnectionService _dbConnectionService;
        private readonly DbContextFactory _dbContextFactory;
        public TaiKhoanReponsitory(TaiKhoanContext context, DbContextFactory dbContextFactory, DatabaseConnectionService dbConnectionService)
        {
            _context = context;
            _dbContextFactory = dbContextFactory;
            _dbConnectionService = dbConnectionService;
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
                // Lưu chuỗi kết nối vào session
                string connectionString = _dbConnectionService.GetConnectionString(user, pass);
                _dbConnectionService.SetConnectionStringInSession(connectionString);

                // Tạo lại DbContext với chuỗi kết nối mới
                var newContext = _dbContextFactory.CreateDbContext<TaiKhoanContext>(connectionString);  // Specify the type here

                // Kiểm tra tài khoản trong context mới
                var account = await newContext.TaiKhoan.FirstOrDefaultAsync(acc => acc.TenTaiKhoan == user && acc.MatKhau == pass);
                if (account == null)
                {
                    return "Tài khoản không tồn tại";
                }
                // Tạo lại NhânViênContext với chuỗi kết nối mới
                var nhanVienContext = _dbContextFactory.CreateDbContext<NhanVienContext>(connectionString);


                // Fetch employee info from database using NhânViênContext
                var employee = await nhanVienContext.tbl_NhanVien
                    .FirstOrDefaultAsync(nv => nv.TenTaiKhoan == account.TenTaiKhoan);

                return employee?.TenNV ?? "Tên nhân viên không tồn tại"; // Return employee name if found
            }
            catch (Exception ex)
            {
                return $"Đã xảy ra lỗi: {ex.Message}"; // Handle error if any
            }
        }


    }
}
