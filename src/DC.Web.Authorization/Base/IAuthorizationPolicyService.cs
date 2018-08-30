using System.Collections.Generic;
using DC.Web.Authorization.Idams;
using Microsoft.AspNetCore.Authorization;

namespace DC.Web.Authorization.Base
{
    public interface IAuthorizationPolicyService
    {
        bool IsRequirementMet(IEnumerable<IdamsClaim> claims, IAuthorizationRequirement requirement);
    }
}
