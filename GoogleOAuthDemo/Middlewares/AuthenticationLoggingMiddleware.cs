using Common.Auth.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GoogleOAuthDemo.Middlewares
{
    public class AuthenticationLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly AuthService authService;

        public AuthenticationLoggingMiddleware(RequestDelegate next, AuthService authService)
        {
            _next = next;

            this.authService = authService;
        }

        public async Task InvokeAsync(HttpContext context)
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

                if (!await authService.IsExistUserByGoogle(userId))
                {
                    await authService.AddUserByGoogle(userId);
                }
            }
            else
            {
                System.IO.File.AppendAllText("output.log", $"{DateTime.Now} User is not authenticated.\n");
            }

            await _next(context);
        }
    }
}
