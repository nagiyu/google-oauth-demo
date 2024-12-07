using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Google 認証をデフォルトスキームに設定
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "Google";
    options.DefaultChallengeScheme = "Google";
})
.AddGoogle(googleOptions =>
{
    googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
    googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
});

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// ミドルウェアの設定
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 認証と認可ミドルウェア
app.UseAuthentication(); // 必須: ユーザーを認証

app.Use(async (context, next) =>
{
    // 認証状態の確認と情報のログ出力
    if (context.User.Identity?.IsAuthenticated ?? false)
    {
        var claims = context.User.Claims;
        var email = claims.FirstOrDefault(c => c.Type == "email")?.Value;
        var name = claims.FirstOrDefault(c => c.Type == "name")?.Value;

        // 認証済みユーザー情報をコンソールに出力（デバッグ目的）
        System.IO.File.AppendAllText("output.log", $"{DateTime.Now} Logged in user: {name} ({email})\n");
    }
    else
    {
        System.IO.File.AppendAllText("output.log", $"{DateTime.Now} User is not authenticated.\n");
    }

    await next();
});

app.UseAuthorization(); // 必須: 認可チェック

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
