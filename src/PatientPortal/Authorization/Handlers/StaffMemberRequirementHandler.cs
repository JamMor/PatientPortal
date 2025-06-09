using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using PatientPortal.Authorization.Requirements;
using PatientPortal.Extensions;

namespace PatientPortal.Authorization.Handlers
{
    public class StaffMemberRequirementHandler : AuthorizationHandler<StaffMemberRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            StaffMemberRequirement requirement)
        {
            if (context.User == null)
            {
                return Task.CompletedTask;
            }

            if (
                context.User.GetStaffId().HasValue
                )
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
