using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using BlogApp;
using BlogApp.Models.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Конфигурация строки подключения к базе данных
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

// Добавление сервисов в контейнер
builder.Services.AddControllersWithViews();

// Регистрация сервисов
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IArticleService, ArticleService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<ICommentService, CommentService>();

// Настройка аутентификации и авторизации
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Administrator"));
    options.AddPolicy("ModeratorOnly", policy => policy.RequireRole("Moderator"));
    options.AddPolicy("UserOnly", policy => policy.RequireRole("DefaultUser"));
});

var app = builder.Build();

// Конфигурация HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Аутентификация и авторизация
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();