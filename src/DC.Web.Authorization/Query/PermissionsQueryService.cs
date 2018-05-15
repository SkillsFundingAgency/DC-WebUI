using System.Collections.Generic;
using System.Linq;
using DC.Web.Authorization.Data.Entities;
using DC.Web.Authorization.Data.Repository;

namespace DC.Web.Authorization.Query
{
    public class PermissionsQueryService : IPermissionsQueryService
    {
        private readonly IAuthorizeRepository _repository;

        public PermissionsQueryService(IAuthorizeRepository authorizeRepository)
        {
            _repository = authorizeRepository;
        }

        public bool HasPermission(string role, IEnumerable<string> features)
        {
            return _repository.GetAllRoleFeatures().Any(r => r.Role.Name == role &&
                                                       features.Any(p => p == r.Feature.Name));
        }
    }
}