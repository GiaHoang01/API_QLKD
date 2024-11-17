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

builder.Services.AddDbContext<TaiKhoanContext>((serviceProvider, options) =>
{
    var dbConnectionService = serviceProvider.GetRequiredService<DatabaseConnectionService>();
    string connectionString = dbConnectionService.GetConnectionStringFromSession() ?? builder.Configuration.GetConnectionString("ConnectString");
    options.UseSqlServer(connectionString);
});

builder.Services.AddDbContext<QuyenContext>((serviceProvider, options) =>
{
    var dbConnectionService = serviceProvider.GetRequiredService<DatabaseConnectionService>();
    string connectionString = dbConnectionService.GetConnectionStringFromSession();
    options.UseSqlServer(connectionString);
});

builder.Services.AddDbContext<NhanVienContext>((serviceProvider, options) =>
{
    var dbConnectionService = serviceProvider.GetRequiredService<DatabaseConnectionService>();
    string connectionString = dbConnectionService.GetConnectionStringFromSession();
    options.UseSqlServer(connectionString);
});

builder.Services.AddDbContext<NhomQuyenContext>((serviceProvider, options) =>
{
    var dbConnectionService = serviceProvider.GetRequiredService<DatabaseConnectionService>();
    string connectionString = dbConnectionService.GetConnectionStringFromSession();
    options.UseSqlServer(connectionString);
});

// Add Repositories
builder.Services.AddScoped<ITaiKhoanReponsitory, TaiKhoanReponsitory>();
builder.Services.AddScoped<INhanVienReponsitory, NhanVienReponsitory>();
builder.Services.AddScoped<INhomQuyenRepository, NhomQuyenRepository>();
builder.Services.AddScoped<DatabaseConnectionService>();
builder.Services.AddScoped<DbContextFactory>();
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
