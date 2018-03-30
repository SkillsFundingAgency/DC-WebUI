using System.Collections.Generic;

namespace DC.Web.Authorization.Base
{
    public interface IPolicyService
    {
        bool IsRequirementMet(IEnumerable<IdamsClaim> claims);
    }
}
