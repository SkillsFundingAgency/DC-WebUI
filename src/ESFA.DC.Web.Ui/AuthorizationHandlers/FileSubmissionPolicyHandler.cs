using DC.Web.Authorization;
using DC.Web.Authorization.FileSubmissionPolicy;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;
using DC.Web.Authorization.Idams;
using DC.Web.Ui.Settings.Models;
using Microsoft.AspNetCore.Authorization.Infrastructure;

namespace DC.Web.Ui.AuthorizationHandlers
{
    public class FileSubmissionPolicyHandler : PolicyHandlerBase<OperationAuthorizationRequirement>
    {
        private readonly IFileSubmissionPolicyService _policyService;

        public FileSubmissionPolicyHandler(IFileSubmissionPolicyService policyService, AuthenticationSettings authenticationSettings)
            :base(authenticationSettings)
        {
            _policyService = policyService;
        }

        protected override Task HandleAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement)
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

            if (_policyService.IsRequirementMet(idamsClaims))
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