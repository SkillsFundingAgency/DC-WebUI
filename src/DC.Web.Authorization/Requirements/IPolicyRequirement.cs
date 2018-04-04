using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;

namespace DC.Web.Authorization.Requirements
{
    public interface IPolicyRequirement : IAuthorizationRequirement
    {
        IEnumerable<string> AllowedFeatures { get; }
    }
}