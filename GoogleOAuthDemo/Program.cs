using App1Auth.Handlers;
using Common.Auth.Services;
using GoogleOAuthDemo.Middlewares;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

// サービス登録
builder.Services.AddSingleton<AuthService>();

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
                "/etc/letsencrypt/live/demo-google-oauth.nagiyu.com/fullchain.pem",
                "/etc/letsencrypt/live/demo-google-oauth.nagiyu.com/privkey.pem"
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

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("App1Policy", policy => policy.Requirements.Add(new App1Requirement()));
});

builder.Services.AddSingleton<IAuthorizationHandler, App1AuthorizationHandler>();

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

app.UseMiddleware<AuthenticationLoggingMiddleware>();

app.UseAuthorization(); // 必須: 認可チェック

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
