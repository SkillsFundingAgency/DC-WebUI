using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DC.Web.Authorization.Idams;
using DC.Web.Authorization.Policies;
using Microsoft.AspNetCore.Authorization;

namespace DC.Web.Authorization.AuthorizationHandlers
{
    public class AuthorizationPolicyHandler : IAuthorizationHandler
    {
        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            if (context.User?.Claims == null || !context.User.Claims.Any())
            {
                context.Fail();
                return Task.CompletedTask;
            }

            var idamsClaims = context.User.Claims.Select(x => new IdamsClaim()
            {
                Type = x.Type,
                Value = x.Value
            }).ToList();

            var pendingRequirements = context.PendingRequirements.ToList();

            foreach (var requirement in pendingRequirements)
            {
                if (requirement is FileSubmissionPolicyRequirement)
                {
                    if (IsFileSubmissionAllowed(idamsClaims))
                    {
                        context.Succeed(requirement);
                    }
                }

                if (requirement is HelpDeskAccessPolicyRequirement)
                {
                    if (IsHelpDeskAreaAllowed(idamsClaims))
                    {
                        context.Succeed(requirement);
                    }
                }
            }

            return Task.CompletedTask;
        }

        private bool IsFileSubmissionAllowed(IEnumerable<IdamsClaim> claims)
        {
            return claims.Any(x => x.Type == IdamsClaimTypes.Service && ClaimAccessConstants.FileSubmissionRoles.Contains(x.Value.ToUpper()));
        }

        private bool IsHelpDeskAreaAllowed(IEnumerable<IdamsClaim> claims)
        {
            return claims.Any(x => x.Type == IdamsClaimTypes.UserType && ClaimAccessConstants.HelpDeskUserTypes.Contains(x.Value.ToUpper()));
        }
    }
}