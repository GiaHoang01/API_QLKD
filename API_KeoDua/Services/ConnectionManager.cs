namespace API_KeoDua.Services
{
    public class ConnectionManager: IConnectionManager
    {
        public string ConnectionString { get; set; }

        public void SetConnectionString(string user, string pass)
        {
            // Tạo chuỗi kết nối động dựa trên thông tin người dùng
            string serverName = "LAPTOP-JIUBMDF6\\SQLEXPRESS";
            string databaseName = "dtb_QuanLyKeoDua"; // Hoặc lấy từ thông tin người dùng
            string username = user; // Tên người dùng
            string password = pass; // Mật khẩu
            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
            {
                // Sử dụng Windows Authentication và thêm các thông số Trusted Connection và Trust Server Certificate
                ConnectionString = $"Data Source={serverName};Initial Catalog={databaseName};Integrated Security=True;Trusted_Connection=True;TrustServerCertificate=True;";
            }
            else
            {
                // Sử dụng SQL Server Authentication và thêm các thông số Trusted Connection và Trust Server Certificate
                ConnectionString = $"Data Source={serverName};Initial Catalog={databaseName};Persist Security Info=True;User ID={user};Password={pass};Trusted_Connection=True;TrustServerCertificate=True;";
            }
        }

        public void ClearConnectionString()
        {
            // Xóa chuỗi kết nối khi người dùng đăng xuất
            ConnectionString = null;
        }
    }
}
