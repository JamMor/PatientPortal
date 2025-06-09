using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using PatientPortal.Authorization.Requirements;
using PatientPortal.Extensions;

namespace PatientPortal.Authorization.Handlers
{
    public class AdminRequirementHandler : AuthorizationHandler<AdminRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            AdminRequirement requirement)
        {
            if (context.User == null)
            {
                return Task.CompletedTask;
            }

            if (
                context.User.IsAdmin()
                )
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
