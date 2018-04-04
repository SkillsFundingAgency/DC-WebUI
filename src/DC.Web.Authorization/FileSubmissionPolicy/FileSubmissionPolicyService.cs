using DC.Web.Authorization.Data.Query;
using DC.Web.Authorization.Idams;
using System.Collections.Generic;
using System.Linq;

namespace DC.Web.Authorization.FileSubmissionPolicy
{
    public class FileSubmissionPolicyService : IFileSubmissionPolicyService
    {
        private readonly IPermissionsQueryService _permissionsQueryService;

        public FileSubmissionPolicyService(IPermissionsQueryService permissionsQueryService)
        {
            _permissionsQueryService = permissionsQueryService;
        }

        public bool IsRequirementMet(IEnumerable<IdamsClaim> claims, FileSubmissionPolicyRequirement requirement)
        {
            var roles = claims.Where(x => x.Type == IdamsClaimTypes.Service);
            return roles.Any(role =>
                _permissionsQueryService.HasPermission(role.Value, requirement.AllowedPermissions)
            );
        }
    }
}