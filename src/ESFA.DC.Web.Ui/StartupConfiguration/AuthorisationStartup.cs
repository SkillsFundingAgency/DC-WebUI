using DC.Web.Authorization;
using DC.Web.Authorization.Data.Constants;
using DC.Web.Authorization.FileSubmissionPolicy;
using DC.Web.Ui.AuthorizationHandlers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace DC.Web.Ui.StartupConfiguration
{
    public static class AuthorisationStartup
    {
        public static void AddAndConfigureAuthorisation(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(PermissionNames.SubmissionAllowed, policy => policy.Requirements.Add(new FileSubmissionPolicyRequirement()));
            });

        }
    }
}