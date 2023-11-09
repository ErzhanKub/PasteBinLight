using System.Security.Claims;

namespace WebApi.Services
{
    public static class UserServices
    {
        /// <summary>
        /// Gets the user ID and role from the current user's claims.
        /// </summary>
        public static (string UserId, string UserRole) GetUserDetails(ClaimsPrincipal currentUser)
        {
            var userId = currentUser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            var userRole = currentUser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value ?? string.Empty;
            return (userId, userRole);
        }

        /// <summary>
        /// Gets the current user's ID as a Guid.
        /// </summary>
        public static Guid GetCurrentUserId(ClaimsPrincipal currentUser)
        {
            var userIdString = currentUser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var userId = userIdString != null ? new Guid(userIdString) : Guid.Empty;
            return userId;
        }
    }
}
