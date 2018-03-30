using System.Collections.Generic;
using System.Linq;
using DC.Web.Authorization.Base;
using DC.Web.Authorization.ClaimTypes;

namespace DC.Web.Authorization.FileSubmissionPolicy
{
    public class FileSubmissionPolicyService : IFileSubmissionPolicyService
    {
        public bool IsRequirementMet(IEnumerable<IdamsClaim> claims)
        {
            return claims.Any(x => x.Type == IdamsClaimTypes.Service  && x.Value == "DAA");
        }
    }
}