using API_KeoDua.Data;
using API_KeoDua.Reponsitory.Implement;
using API_KeoDua.Reponsitory.Interface;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Serialization;
using log4net;
using log4net.Config;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Cấu hình log4net
var logRepository = LogManager.GetRepository(System.Reflection.Assembly.GetEntryAssembly());
XmlConfigurator.Configure(new FileInfo("log4net.config"));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<TaiKhoanContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectString"));
});

builder.Services.AddDbContext<QuyenContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectString"));
});

builder.Services.AddDbContext<CapQuyenContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectString"));
});

builder.Services.AddDbContext<NhomQuyenContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectString"));
});

builder.Services.AddDbContext<NhanVienContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectString"));
});

builder.Services.AddDbContext<KhachHangContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectString"));
});

builder.Services.AddDbContext<LoaiKhachHangContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectString"));
});

builder.Services.AddDbContext<ThongTinGiaoHangContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectString"));
});

builder.Services.AddDbContext<HoaDonBanHangContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectString"));
});

builder.Services.AddDbContext<HinhThucThanhToanContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectString"));
});

builder.Services.AddDbContext<PhieuGiaoHangContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectString"));
});

builder.Services.AddDbContext<CT_HoaDonBanHangContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectString"));
});

builder.Services.AddDbContext<GioHangContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectString"));
});

builder.Services.AddDbContext<PhieuHuyDonConText>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectString"));
});

builder.Services.AddDbContext<ChuongTrinhKhuyenMaiContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectString"));
});

builder.Services.AddDbContext<ChiTietCT_KhuyenMaiContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectString"));
});

builder.Services.AddDbContext<HangHoaContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectString"));
});

builder.Services.AddDbContext<ChiTietGioHangConText>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectString"));
});

builder.Services.AddDbContext<LichSuGiaContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectString"));
});

builder.Services.AddDbContext<LoaiHangHoaContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectString"));
});

builder.Services.AddDbContext<CT_PhieuNhapContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectString"));
});

builder.Services.AddDbContext<PhieuNhapHangContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectString"));
});

builder.Services.AddDbContext<NhaCungCapContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectString"));
});

builder.Services.AddScoped<ITaiKhoanReponsitory, TaiKhoanReponsitory>();
builder.Services.AddScoped<INhanVienReponsitory, NhanVienReponsitory>();
builder.Services.AddScoped<INhomQuyenRepository, NhomQuyenRepository>();
var app = builder.Build();

// Use CORS
app.UseCors("AllowOrigin");

 


// Configure the HTTP request pipeline. 
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(options => options.WithOrigins("http://localhost:4200", "http://localhost:5002", "http://localhost:81", "https://localhost:7150")
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
