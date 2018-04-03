using System.Collections.Generic;
using System.Linq;
using DC.Web.Authorization.Base;
using DC.Web.Authorization.Data;
using DC.Web.Authorization.Data.Constants;
using DC.Web.Authorization.Data.Query;
using DC.Web.Authorization.Idams;

namespace DC.Web.Authorization.FileSubmissionPolicy
{
    public class FileSubmissionPolicyService : IFileSubmissionPolicyService
    {
        private readonly IPermissionsQueryService _permissionsQueryService;

        public FileSubmissionPolicyService(IPermissionsQueryService permissionsQueryService)
        {
            _permissionsQueryService = permissionsQueryService;
        }
        public bool IsRequirementMet(IEnumerable<IdamsClaim> claims)
        {
            var roles = claims.Where(x => x.Type == IdamsClaimTypes.Service);
            return roles.Any(role => 
                _permissionsQueryService.HasPermission(role.Value, PermissionNames.SubmissionAllowed)
                );
        }
    }
}