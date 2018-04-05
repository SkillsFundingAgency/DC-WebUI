using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace DC.Web.Authorization.Requirements
{
    public interface IPolicyRequirement : IAuthorizationRequirement
    {
        IEnumerable<string> AllowedFeatures { get; }
    }
}