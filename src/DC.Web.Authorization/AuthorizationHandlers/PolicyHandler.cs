using System.Linq;
using System.Threading.Tasks;
using DC.Web.Authorization.Base;
using DC.Web.Authorization.FileSubmissionPolicy;
using DC.Web.Authorization.Idams;
using DC.Web.Authorization.Requirements;
using DC.Web.Ui.Settings.Models;
using Microsoft.AspNetCore.Authorization;

namespace DC.Web.Authorization.AuthorizationHandlers
{
    public class PolicyHandler : PolicyHandlerBase<IPolicyRequirement>
    {
        private readonly IPolicyService _policyService;

        public PolicyHandler(IPolicyService policyService, AuthenticationSettings authenticationSettings)
            :base(authenticationSettings)
        {
            _policyService = policyService;
        }

        protected override Task HandleAsync(AuthorizationHandlerContext context, IPolicyRequirement requirement)
        {

            if (context.User.Claims == null || !context.User.Claims.Any())
            {
                context.Fail();
                return Task.CompletedTask;
            }

            var idamsClaims = context.User.Claims.Select(x => new IdamsClaim()
            {
                Type = x.Type,
                Value = x.Value
            });

            if (_policyService.IsRequirementMet(idamsClaims,requirement))
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }

            return Task.CompletedTask;
        }
    }
}