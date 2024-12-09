using App1Auth.Models;
using Common.Auth.Services;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;

namespace App1Auth.Handlers
{
    public class App1AuthorizationHandler : AuthorizationHandler<App1Requirement>
    {
        private readonly AuthService authService;

        public App1AuthorizationHandler(AuthService authService)
        {
            this.authService = authService;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, App1Requirement requirement)
        {
            if (context.User.HasClaim(c => c.Type == nameof(App1UserAuth.UserId)))
            {
                var userId = Guid.Parse(context.User.FindFirst(c => c.Type == nameof(App1UserAuth.UserId)).Value);
                var user = new App1UserAuth(await authService.GetUserByUserId<App1UserAuth>(userId));
                var role = user.App1Role;

                if (string.IsNullOrEmpty(role))
                {
                    user.App1Role = "User";
                    await authService.UpdateUser(user);
                }

                if (role == "Admin")
                {
                    context.Succeed(requirement);
                }
            }
        }
    }
}
