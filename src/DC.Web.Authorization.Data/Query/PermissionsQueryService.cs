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

        public bool HasPermission(string role, IEnumerable<string> features)
        {
            return _dbContext.RoleFeatures.Any(r => r.Role.Name == role &&
                                                       features.Any(p => p == r.Feature.Name));
        }
    }
}