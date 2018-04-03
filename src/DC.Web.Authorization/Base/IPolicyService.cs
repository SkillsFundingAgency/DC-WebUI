using System.Collections.Generic;
using DC.Web.Authorization.Idams;

namespace DC.Web.Authorization.Base
{
    public interface IPolicyService
    {
        bool IsRequirementMet(IEnumerable<IdamsClaim> claims);
    }
}
