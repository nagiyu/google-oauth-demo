using Microsoft.AspNetCore.Authorization;

namespace App1Auth.Handlers
{
    public class App1Requirement : IAuthorizationRequirement
    {
        public string App1Role { get; }
    }
}
