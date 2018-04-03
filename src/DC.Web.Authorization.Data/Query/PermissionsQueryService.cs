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
        public bool HasPermission(string role,string permission)
        {
            return _dbContext.RolePermissions.Any(x => x.Role.Name == role && x.Permission.Name == permission);
        }
    }
}
