using System.Security.Claims;

namespace WebApi.Services;

public static class UserServices
{
    public static (string, string) GetUserDetails(ClaimsPrincipal currentUser)
    {
        var userId = currentUser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        var userRole = currentUser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        return (userId, userRole)!;
    }

    public static Guid GetCurrentUserId(ClaimsPrincipal currentUser)
    {
        var userIdString = currentUser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        var userId = userIdString != null ? new Guid(userIdString) : Guid.Empty;
        return userId;
    }
}
