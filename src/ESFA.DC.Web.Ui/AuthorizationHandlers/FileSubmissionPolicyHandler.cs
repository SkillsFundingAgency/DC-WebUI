using DC.Web.Authorization;
using DC.Web.Authorization.FileSubmissionPolicy;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;

namespace DC.Web.Ui.AuthorizationHandlers
{
    public class FileSubmissionPolicyHandler : AuthorizationHandler<FileSubmissionPolicyRequirement>
    {
        private readonly IFileSubmissionPolicyService _policyService;

        public FileSubmissionPolicyHandler(IFileSubmissionPolicyService policyService)
        {
            _policyService = policyService;
        }


        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, FileSubmissionPolicyRequirement requirement)
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