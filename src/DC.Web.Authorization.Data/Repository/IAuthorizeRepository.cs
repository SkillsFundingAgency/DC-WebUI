using System.Collections.Generic;
using DC.Web.Authorization.Data.Entities;

namespace DC.Web.Authorization.Data.Repository
{
    public interface IAuthorizeRepository
    {
        IEnumerable<RoleFeature> GetAllRoleFeatures();
    }
}