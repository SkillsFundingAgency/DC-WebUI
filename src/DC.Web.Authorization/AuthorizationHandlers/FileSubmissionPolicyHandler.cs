using System.Linq;
using System.Threading.Tasks;
using DC.Web.Authorization.Base;
using DC.Web.Authorization.FileSubmissionPolicy;
using DC.Web.Authorization.Idams;
using DC.Web.Ui.Settings.Models;
using Microsoft.AspNetCore.Authorization;

namespace DC.Web.Authorization.AuthorizationHandlers
{
    public class FileSubmissionPolicyHandler : AuthorizationPolicyHandlerBase<FileSubmissionPolicyRequirement>
    {
        private readonly IAuthorizationPolicyService _authorizationPolicyService;

        public FileSubmissionPolicyHandler(IAuthorizationPolicyService authorizationPolicyService, AuthenticationSettings authenticationSettings)
            : base(authenticationSettings)
        {
            _authorizationPolicyService = authorizationPolicyService;
        }

        protected override Task HandleAsync(AuthorizationHandlerContext context, FileSubmissionPolicyRequirement requirement)
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
            });

            if (_authorizationPolicyService.IsRequirementMet(idamsClaims, requirement))
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