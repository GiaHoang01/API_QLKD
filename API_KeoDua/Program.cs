using API_KeoDua.Data;
using API_KeoDua.Models;
using API_KeoDua.Reponsitory.Implement;
using API_KeoDua.Reponsitory.Interface;
using log4net.Config;
using log4net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using API_KeoDua.Services.VnPAY;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpContextAccessor(); // Add IHttpContextAccessor service
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60); // Session tồn tại trong 30 phút
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
// Add session service
builder.Services.AddDistributedMemoryCache();

// Configure log4net
var logRepository = LogManager.GetRepository(System.Reflection.Assembly.GetEntryAssembly());
XmlConfigurator.Configure(new FileInfo("log4net.config"));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<TaiKhoanContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectString")
 ));

builder.Services.AddDbContext<QuyenContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectString")
 ));

builder.Services.AddDbContext<CT_HoaDonBanHangContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectString")
 ));

builder.Services.AddDbContext<NhanVienContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectString")
 ));

builder.Services.AddDbContext<NhomQuyenContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectString")
 ));

builder.Services.AddDbContext<PhieuNhapHangContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectString")
 ));

builder.Services.AddDbContext<CT_PhieuNhapContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectString")
 ));
builder.Services.AddDbContext<HangHoaContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectString")
 ));
builder.Services.AddDbContext<HoaDonBanHangContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectString")
 ));
builder.Services.AddDbContext<CT_HoaDonBanHangContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectString")
 ));
builder.Services.AddDbContext<NhaCungCapContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectString")
 ));
builder.Services.AddDbContext<LichSuGiaContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectString")
 ));
builder.Services.AddDbContext<KhachHangContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectString")
 ));
builder.Services.AddDbContext<GioHangContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectString")
 ));
builder.Services.AddDbContext<HinhThucThanhToanContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectString")
 ));
builder.Services.AddDbContext<PhieuGiaoHangContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectString")
 ));
builder.Services.AddDbContext<PhieuHuyDonConText>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectString")
 ));
builder.Services.AddDbContext<ThongTinGiaoHangContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectString")
 ));
// Add Repositories
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
builder.Services.AddScoped<IPhieuHuyDonReponsitory, PhieuHuyDonReponsitory>();
builder.Services.AddScoped<IThongTinGiaoHangReponsitory, ThongTinGiaoHangReponsitory>();
builder.Services.AddScoped<IVnPayService, VnPAYService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

var app = builder.Build();

// Use CORS
app.UseCors("AllowOrigin");

// Enable session

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
app.UseSession();
app.UseAuthorization();
app.MapControllers();

app.Run();
