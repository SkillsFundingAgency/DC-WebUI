using DC.Web.Authorization.Data.Constants;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace DC.Web.Authorization.FileSubmissionPolicy
{
    public class FileSubmissionPolicyRequirement : IAuthorizationRequirement
    {
        public IEnumerable<string> AllowedPermissions { get; set; } = new List<string>() { PermissionNames.SubmissionAllowed };
    }
}