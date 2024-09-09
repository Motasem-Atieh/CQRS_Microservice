using System.Security.Claims;

namespace CQRS_Microservice.Models;

public static class Permissions
{
    public static bool UserHasPermission(ClaimsPrincipal user, string requiredPermission)
    {
        var permissions = user.FindAll("Permissions").Select(c => c.Value).ToList();
        return permissions.Contains(requiredPermission);
    }
}
