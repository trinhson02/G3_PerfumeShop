using CodeMegaVNPay.Services;
using G3_PerfumeShop.Models;
using G3_PerfumeShop.Service;
using G3_PerfumeShop.SignalR.Hubs;
using G3_PerfumeShop.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddTransient<S3Service>();
builder.Services.AddTransient<VnPayService>();


builder.Services.AddTransient<S3Service_Son>();

builder.Services.AddDbContext<G3_PerfumeShopDB_Iter3Context>(options =>
   options.UseSqlServer(builder.Configuration.GetConnectionString("G3_PerfumeShopDB_Test_TestConnection")));

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Thay đổi thời gian hết hạn nếu cần
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true; // Cần thiết cho cookie trong các ứng dụng không có auth
});
builder.Services.AddCors(o =>
{
    o.AddPolicy("AllowAnyOrigin", p => p
        .WithOrigins("null") // Origin of an html file opened in a browser
        .AllowAnyHeader()
        .AllowCredentials());
});
builder.Services.AddSignalR();
builder.Services.AddTransient<EmailService>();
// Thêm dịch vụ cho HttpContextAccessor nếu bạn cần truy cập HttpContext
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ISessionService, SessionService>(); // Thêm session service

// Cấu hình cache cho session
builder.Services.AddDistributedMemoryCache();

var app = builder.Build();
app.UseCors("AllowAnyOrigin");
app.MapHub<StatisticsHub>("/stathub");
app.MapHub<ChatHub>("/chathub");


var webSocketOptions = new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(2)
};
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthentication(); // Nếu bạn sử dụng Identity hoặc bất kỳ phương thức xác thực nào
app.UseAuthorization();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
