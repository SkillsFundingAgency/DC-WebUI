using DC.Web.Authorization;
using DC.Web.Authorization.AuthorizationHandlers;
using DC.Web.Authorization.Policies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace DC.Web.Ui.StartupConfiguration
{
    public static class AuthorisationStartup
    {
        public static void AddAndConfigureAuthorisation(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy(PolicyTypes.FileSubmission, policy => policy.Requirements.Add(new FileSubmissionPolicyRequirement()));
                options.AddPolicy(PolicyTypes.HelpDeskAccess, policy => policy.Requirements.Add(new HelpDeskAccessPolicyRequirement()));
            });

            services.AddSingleton<IAuthorizationHandler, AuthorizationPolicyHandler>();
        }
    }
}