using System.Collections.Generic;
using System.Linq;
using DC.Web.Authorization.Base;
using DC.Web.Authorization.Idams;
using Microsoft.AspNetCore.Authorization;

namespace DC.Web.Authorization.FileSubmissionPolicy
{
    public class AuthorizationPolicyService : IAuthorizationPolicyService
    {
        private readonly IEnumerable<string> _allowedRoles = new List<string> { "DCFT", "DAA", "DCS" };

        public bool IsRequirementMet(IEnumerable<IdamsClaim> claims, IAuthorizationRequirement requirement)
        {
            if (claims == null || requirement == null)
            {
                return false;
            }

            return claims.Any(x => x.Type == IdamsClaimTypes.Service && _allowedRoles.Contains(x.Value));
        }
    }
}