using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

// 環境ごとの Kestrel 設定をロード
builder.WebHost.ConfigureKestrel(options =>
{
    var environment = builder.Environment.EnvironmentName;

    if (environment == "Production")
    {
        // 本番環境（Let’s Encrypt 証明書を使用）
        options.ConfigureHttpsDefaults(httpsOptions =>
        {
            // .pem ファイルを直接指定
            var certificate = X509Certificate2.CreateFromPemFile(
                "/etc/letsencrypt/live/yourdomain.com/fullchain.pem",
                "/etc/letsencrypt/live/yourdomain.com/privkey.pem"
            );

            // Kestrel に証明書を設定
            httpsOptions.ServerCertificate = certificate;
        });
    }
});

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(options =>
{
    // サインインスキームを Cookies に設定
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = "Google";
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie() // Cookie 認証を追加
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

        // 既存スキーマにマッピングされたClaimを取得
        var userId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        var fullName = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
        var firstName = claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value;
        var lastName = claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value;
        var email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

        System.IO.File.AppendAllText("output.log",
            $"{DateTime.Now} User Authenticated:\n" +
            $"- User ID: {userId}\n" +
            $"- Email: {email}\n" +
            $"- Full Name: {fullName}\n" +
            $"- First Name: {firstName}\n" +
            $"- Last Name: {lastName}\n\n");
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
