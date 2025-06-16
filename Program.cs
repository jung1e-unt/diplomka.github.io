using BeeGroup.Models; // подключи пространство имён с AppDbContext
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddControllers();
builder.Services.AddAuthorization();
builder.Services.AddDistributedMemoryCache(); // ✅ обязательно
builder.Services.AddSession();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();


var app = builder.Build();

app.UseSession(); // <--- обязательно!


// Конфигурация middleware
if (!app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // временно для отладки

    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 4. Подключаем сессии перед авторизацией
app.UseSession();
app.UseAuthorization();

// 5. Настройка маршрута
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

