using DC.Web.Authorization.Idams;
using System.Collections.Generic;

namespace DC.Web.Authorization.FileSubmissionPolicy
{
    public interface IFileSubmissionPolicyService
    {
        bool IsRequirementMet(IEnumerable<IdamsClaim> claims, FileSubmissionPolicyRequirement requirement);
    }
}