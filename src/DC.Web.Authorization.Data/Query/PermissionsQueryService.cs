using System.Collections.Generic;
using System.Linq;

namespace DC.Web.Authorization.Data.Query
{
    public class PermissionsQueryService : IPermissionsQueryService
    {
        private readonly AuthorizeDbContext _dbContext;

        public PermissionsQueryService(AuthorizeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public bool HasPermission(string role, IEnumerable<string> permissions)
        {
            return _dbContext.RolePermissions.Any(r => r.Role.Name == role &&
                                                       permissions.Any(p => p == r.Permission.Name));
        }
    }
}