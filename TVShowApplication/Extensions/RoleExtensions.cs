using TVShowApplication.Models;

namespace TVShowApplication.Extensions
{
    public static class RoleExtensions
    {
        public static Role GetRole(this User user)
        {
            if (user == null) return Role.Unauthorized;

            return user switch
            {
                Administrator => Role.Admin,
                Poster => Role.Poster,
                User => Role.User,
                _ => Role.Unauthorized,
            };
        }

        public static Role GetRole<TUser>() where TUser : User, new()
        {
            var user = new TUser();

            return user.GetRole();
        }

        public static Role[] GetRoles(this User user)
        {
            if (user == null) return Array.Empty<Role>();

            return user switch
            {
                Administrator => new[] { Role.Admin, Role.Poster, Role.User },
                Poster => new[] { Role.Poster, Role.User },
                User => new[] { Role.User },
                _ => Array.Empty<Role>(),
            };
        }

        public static bool IsInRoles(this Role role, params Role[] requiredRoles)
        {
            foreach (var r in requiredRoles)
            {
                if (role == r) return true;
            }

            return false;
        }
    }
}
