using API_KeoDua.Data;
using API_KeoDua.Models;
using API_KeoDua.Reponsitory.Implement;
using API_KeoDua.Reponsitory.Interface;
using API_KeoDua.Services.VnPAY;
using API_KeoDua.Services;
using log4net;
using log4net.Config;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// Configure log4net
var logRepository = LogManager.GetRepository(System.Reflection.Assembly.GetEntryAssembly());
XmlConfigurator.Configure(new FileInfo("log4net.config"));

// Add services to the container.
builder.Services.AddHttpContextAccessor(); // Add IHttpContextAccessor service
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60); // Session tồn tại trong 60 phút
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddDistributedMemoryCache(); // Add session service

// Register connection manager
builder.Services.AddSingleton<IConnectionManager, ConnectionManager>();

// Register repositories
builder.Services.AddScoped<ITaiKhoanReponsitory, TaiKhoanReponsitory>();
builder.Services.AddScoped<INhanVienReponsitory, NhanVienReponsitory>();
builder.Services.AddScoped<INhomQuyenRepository, NhomQuyenRepository>();
builder.Services.AddScoped<IPhieuNhapHangReponsitory, PhieuNhapHangReponsitory>();
builder.Services.AddScoped<IHangHoaReponsitory, HangHoaReponsitory>();
builder.Services.AddScoped<ILichSuGiaReponsitory, LichSuGiaReponsitory>();
builder.Services.AddScoped<IKhachHangReponsitory, KhachHangReponsitory>();
builder.Services.AddScoped<IGioHangReponsitory, GioHangReponsitory>();
builder.Services.AddScoped<INhaCungCapReponsitory, NhaCungCapReponsitory>();
builder.Services.AddScoped<IHinhThucThanhToanReponsitory, HinhThucThanhToanReponsitory>();
builder.Services.AddScoped<IHoaDonBanHangReponsitory, HoaDonBanHangReponsitory>();
builder.Services.AddScoped<IPhieuGiaoHangReponsitory, PhieuGiaoHangReponsitory>();
builder.Services.AddScoped<IChuongTrinhKhuyenMaiReponsitory, ChuongTrinhKhuyenMaiReponsitory>();
builder.Services.AddScoped<IChiTietCT_KhuyenMaiReponsitory, ChiTietCT_KhuyenMaiReponsitory>();
builder.Services.AddScoped<IPhieuHuyDonReponsitory, PhieuHuyDonReponsitory>();
builder.Services.AddScoped<IThongTinGiaoHangReponsitory, ThongTinGiaoHangReponsitory>();
builder.Services.AddScoped<IVnPayService, VnPAYService>();

// Add DbContexts
builder.Services.AddDbContext<TaiKhoanContext>((serviceProvider, options) =>
{
    var connectionManager = serviceProvider.GetRequiredService<IConnectionManager>();
    options.UseSqlServer(connectionManager.ConnectionString);
});

builder.Services.AddDbContext<QuyenContext>((serviceProvider, options) =>
{
    var connectionManager = serviceProvider.GetRequiredService<IConnectionManager>();
    options.UseSqlServer(connectionManager.ConnectionString);
});
builder.Services.AddDbContext<CT_HoaDonBanHangContext>((serviceProvider, options) =>
{
    var connectionManager = serviceProvider.GetRequiredService<IConnectionManager>();
    options.UseSqlServer(connectionManager.ConnectionString);
});

builder.Services.AddDbContext<NhanVienContext>((serviceProvider, options) =>
{
    var connectionManager = serviceProvider.GetRequiredService<IConnectionManager>();
    options.UseSqlServer(connectionManager.ConnectionString);
});

builder.Services.AddDbContext<NhomQuyenContext>((serviceProvider, options) =>
{
    var connectionManager = serviceProvider.GetRequiredService<IConnectionManager>();
    options.UseSqlServer(connectionManager.ConnectionString);
});
builder.Services.AddDbContext<PhieuNhapHangContext>((serviceProvider, options) =>
{
    var connectionManager = serviceProvider.GetRequiredService<IConnectionManager>();
    options.UseSqlServer(connectionManager.ConnectionString);
});
builder.Services.AddDbContext<CT_PhieuNhapContext>((serviceProvider, options) =>
{
    var connectionManager = serviceProvider.GetRequiredService<IConnectionManager>();
    options.UseSqlServer(connectionManager.ConnectionString);
});
builder.Services.AddDbContext<HangHoaContext>((serviceProvider, options) =>
    options.UseSqlServer(serviceProvider.GetRequiredService<IConnectionManager>().ConnectionString));
builder.Services.AddDbContext<BackupRestoreContext>((serviceProvider, options) =>
    options.UseSqlServer(serviceProvider.GetRequiredService<IConnectionManager>().ConnectionString));
builder.Services.AddDbContext<HoaDonBanHangContext>((serviceProvider, options) =>
    options.UseSqlServer(serviceProvider.GetRequiredService<IConnectionManager>().ConnectionString));
builder.Services.AddDbContext<CT_HoaDonBanHangContext>((serviceProvider, options) =>
    options.UseSqlServer(serviceProvider.GetRequiredService<IConnectionManager>().ConnectionString));
builder.Services.AddDbContext<NhaCungCapContext>((serviceProvider, options) =>
    options.UseSqlServer(serviceProvider.GetRequiredService<IConnectionManager>().ConnectionString));
builder.Services.AddDbContext<LichSuGiaContext>((serviceProvider, options) =>
    options.UseSqlServer(serviceProvider.GetRequiredService<IConnectionManager>().ConnectionString));
builder.Services.AddDbContext<KhachHangContext>((serviceProvider, options) =>
    options.UseSqlServer(serviceProvider.GetRequiredService<IConnectionManager>().ConnectionString));
builder.Services.AddDbContext<GioHangContext>((serviceProvider, options) =>
    options.UseSqlServer(serviceProvider.GetRequiredService<IConnectionManager>().ConnectionString));
builder.Services.AddDbContext<HinhThucThanhToanContext>((serviceProvider, options) =>
    options.UseSqlServer(serviceProvider.GetRequiredService<IConnectionManager>().ConnectionString));
builder.Services.AddDbContext<PhieuGiaoHangContext>((serviceProvider, options) =>
    options.UseSqlServer(serviceProvider.GetRequiredService<IConnectionManager>().ConnectionString));
builder.Services.AddDbContext<ChuongTrinhKhuyenMaiContext>((serviceProvider, options) =>
    options.UseSqlServer(serviceProvider.GetRequiredService<IConnectionManager>().ConnectionString));
builder.Services.AddDbContext<PhieuHuyDonConText>((serviceProvider, options) =>
    options.UseSqlServer(serviceProvider.GetRequiredService<IConnectionManager>().ConnectionString));
builder.Services.AddDbContext<ChiTietCT_KhuyenMaiContext>((serviceProvider, options) =>
    options.UseSqlServer(serviceProvider.GetRequiredService<IConnectionManager>().ConnectionString));

// Configure controllers and Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuration from appsettings.json
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
builder.Services.AddLogging(logging =>
{
    logging.AddConsole(); // Log ra console
});

var app = builder.Build();

// Configure middleware
app.UseCors(options => options.WithOrigins("http://localhost:4200", "http://localhost:5002", "http://localhost:81", "https://localhost:7150")
    .AllowAnyMethod()
    .AllowAnyHeader());
app.UseHttpsRedirection();

app.UseSession();
app.UseAuthorization();
app.MapControllers();

// Enable Swagger in development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();
