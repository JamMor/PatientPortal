using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PatientPortal.Models;

namespace PatientPortal.Infrastructure
{
    public class CustomUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<IdentityUser>
    {
        private readonly PatientPortalContext _context;

        public CustomUserClaimsPrincipalFactory(
            UserManager<IdentityUser> userManager,
            IOptions<IdentityOptions> optionsAccessor,
            PatientPortalContext context)
            : base(userManager, optionsAccessor)
        {
            _context = context;
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(IdentityUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);

            // Look up Staff record associated with this Identity user
            var staff = await _context.Staff
                .Include(s => s.MessagingLink)
                .FirstOrDefaultAsync(s => s.User.Id == user.Id);

            if (staff != null)
            {
                // Add custom claims for application-specific data
                identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, staff.StaffId.ToString()));
                identity.AddClaim(new Claim(ClaimTypes.GivenName, staff.FirstName));
                identity.AddClaim(new Claim(ClaimTypes.Surname, staff.LastName));
                identity.AddClaim(new Claim(ClaimTypes.Role, staff.Role));
                identity.AddClaim(new Claim("MessageLinkId", staff.MessagingLink.MessagingLinkId.ToString()));
            }

            return identity;
        }
    }
}
