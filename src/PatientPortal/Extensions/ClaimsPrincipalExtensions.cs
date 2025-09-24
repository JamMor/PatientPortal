using System.Security.Claims;

#nullable enable
namespace PatientPortal.Extensions
{
    /// <summary>
    /// Extension methods for ClaimsPrincipal to easily access custom claims.
    /// </summary>
    public static class ClaimsPrincipalExtensions
    {
        /// <summary>
        /// Gets the current staff member's ID from claims.
        /// </summary>
        public static int? GetStaffId(this ClaimsPrincipal user)
        {
            var claim = user.FindFirst("StaffId")?.Value;
            return int.TryParse(claim, out var id) ? id : null;
        }

        /// <summary>
        /// Gets the current staff member's messaging link ID from claims.
        /// </summary>
        public static int? GetMessageLinkId(this ClaimsPrincipal user)
        {
            var claim = user.FindFirst("MessageLinkId")?.Value;
            return int.TryParse(claim, out var id) ? id : null;
        }

        /// <summary>
        /// Gets the current staff member's full name from claims.
        /// </summary>
        public static string? GetFullName(this ClaimsPrincipal user)
        {
            var givenName = user.FindFirst(ClaimTypes.GivenName)?.Value;
            var surname = user.FindFirst(ClaimTypes.Surname)?.Value;
            
            if (string.IsNullOrEmpty(givenName) || string.IsNullOrEmpty(surname))
                return null;
                
            return $"{givenName} {surname}";
        }

        /// <summary>
        /// Gets the current staff member's role from claims.
        /// Note: Prefer using User.IsInRole("RoleName") for role checks.
        /// </summary>
        public static string? GetRole(this ClaimsPrincipal user)
        {
            return user.FindFirst(ClaimTypes.Role)?.Value;
        }

        /// <summary>
        /// Checks if the current staff member has admin privileges.
        /// Reads from the IsAdmin claim (stored as ClaimValueTypes.Boolean).
        /// </summary>
        public static bool IsAdmin(this ClaimsPrincipal user)
        {
            var claim = user.FindFirst("IsAdmin")?.Value;
            return bool.TryParse(claim, out var isAdmin) && isAdmin;
        }
    }
}
