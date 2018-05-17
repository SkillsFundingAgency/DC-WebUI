using System.Collections.Generic;
using DC.Web.Authorization.Data.Constants;

namespace DC.Web.Authorization.Requirements
{
    public class FileSubmissionPolicyRequirement : IPolicyRequirement
    {
        public IEnumerable<string> AllowedFeatures => new List<string>() { FeatureNames.FileSubmission };
    }
}