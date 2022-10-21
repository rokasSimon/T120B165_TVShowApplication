using TVShowApplication.Exceptions;
using TVShowApplication.Models;

namespace TVShowApplication.Extensions
{
    public static class Fault
    {
        public static void IfMissingRole(Role role, params Role[] requiredRoles)
        {
            if (role.IsInRoles(requiredRoles)) return;

            throw new UnauthorizedException($"User with role {role} has missing roles.");
        }
    }
}
