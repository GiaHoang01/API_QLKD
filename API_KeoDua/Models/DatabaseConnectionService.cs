using API_KeoDua.Data;
using Microsoft.Data.SqlClient;

public class DatabaseConnectionService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private string _connectionString;

    public DatabaseConnectionService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetConnectionString(string username, string password)
    {
        _connectionString = $"Server=LAPTOP-JIUBMDF6\\SQLEXPRESS;Database=dtb_QuanLyKeoDua;User Id={username};Password={password};TrustServerCertificate=True;";
        return _connectionString;
    }


    public async Task<TaiKhoan> GetAccountInfo(string username)
    {
        // Fetch account information
        var command = new SqlCommand("SELECT * FROM tbl_TaiKhoan WHERE TenTaiKhoan = @username", new SqlConnection(_connectionString));
        command.Parameters.AddWithValue("@username", username);

        await command.Connection.OpenAsync();
        var reader = await command.ExecuteReaderAsync();
        TaiKhoan account = null;

        if (await reader.ReadAsync())
        {
            account = new TaiKhoan
            {
                TenTaiKhoan = reader["TenTaiKhoan"].ToString(),
                MatKhau = reader["MatKhau"].ToString(),
            };
        }

        await command.Connection.CloseAsync();
        return account;
    }

    public void SetConnectionStringInSession(string connectionString)
    {
        _httpContextAccessor.HttpContext.Session.SetString("UserConnectionString", connectionString);
    }

    public string GetConnectionStringFromSession()
    {
        return _httpContextAccessor.HttpContext.Session.GetString("UserConnectionString");
    }
}
