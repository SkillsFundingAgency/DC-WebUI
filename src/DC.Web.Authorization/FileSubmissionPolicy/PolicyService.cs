using System.Collections.Generic;
using System.Linq;
using DC.Web.Authorization.Base;
using DC.Web.Authorization.Data.Query;
using DC.Web.Authorization.Idams;
using DC.Web.Authorization.Requirements;

namespace DC.Web.Authorization.FileSubmissionPolicy
{
    public class PolicyService : IPolicyService
    {
        private readonly IPermissionsQueryService _permissionsQueryService;

        public PolicyService(IPermissionsQueryService permissionsQueryService)
        {
            _permissionsQueryService = permissionsQueryService;
        }

        public bool IsRequirementMet(IEnumerable<IdamsClaim> claims, IPolicyRequirement requirement)
        {
            if (claims == null || requirement == null)
            {
                return false;
            }

            var roles = claims.Where(x => x.Type == IdamsClaimTypes.Service);
            return roles.Any(role =>
                _permissionsQueryService.HasPermission(role.Value, requirement.AllowedFeatures));
        }
    }
}