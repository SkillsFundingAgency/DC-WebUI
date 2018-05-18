using System.Collections.Generic;
using DC.Web.Authorization.Idams;
using DC.Web.Authorization.Requirements;
using Microsoft.AspNetCore.Authorization;

namespace DC.Web.Authorization.Base
{
    public interface IPolicyService
    {
        bool IsRequirementMet(IEnumerable<IdamsClaim> claims, IPolicyRequirement requirement);
    }
}
