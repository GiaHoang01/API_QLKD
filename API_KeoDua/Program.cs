using API_KeoDua.Data;
using API_KeoDua.Models;
using API_KeoDua.Reponsitory.Implement;
using API_KeoDua.Reponsitory.Interface;
using log4net.Config;
using log4net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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

builder.Services.AddDbContext<NhanVienContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectString")
 ));

builder.Services.AddDbContext<NhomQuyenContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectString")
 ));



// Add Repositories
builder.Services.AddScoped<ITaiKhoanReponsitory, TaiKhoanReponsitory>();
builder.Services.AddScoped<INhanVienReponsitory, NhanVienReponsitory>();
builder.Services.AddScoped<INhomQuyenRepository, NhomQuyenRepository>();
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
